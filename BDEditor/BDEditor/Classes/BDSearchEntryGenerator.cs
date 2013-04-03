﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    /// <summary>
    /// Clear existing search entries and generate new search entry and search entry association entities 
    /// </summary>
    public class BDSearchEntryGenerator
    {
        private List<BDSearchEntry> searchEntryList;
        private List<BDHtmlPage> htmlPages;

        public BDSearchEntryGenerator() { }
        
        /// <summary>
        /// Generate new Search Entries from the data
        /// </summary>
        public void Generate(Entities pDataContext, IBDNode pNode)
        {
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDSearchGeneratorLog.txt", string.Format("Start: {0} -------------------------------", DateTime.Now));

            // reset Id of display parent in search entry association records
            BDSearchEntryAssociation.ResetForRegeneration(pDataContext);

            searchEntryList = BDSearchEntry.RetrieveAll(pDataContext);
            htmlPages = BDHtmlPage.RetrieveAll(pDataContext);

            foreach (BDSearchEntry entry in searchEntryList)
            {
                List<BDSearchEntryAssociation> associations = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(pDataContext, entry.Uuid);

                foreach (BDSearchEntryAssociation seAssociation in associations)
                {
                    // determine the displayParentId : (the HTML page Id where the anchorNode has been rendered)
                    // NB: this may change at any time, so it is always repopulated on a Build.
                    Guid htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForOriginalIBDNodeId(pDataContext, seAssociation.anchorNodeId.Value);
                    if (htmlPageId != Guid.Empty)
                    {
                        BDHtmlPage htmlPage = BDHtmlPage.RetrieveWithId(pDataContext, htmlPageId);
                        if (htmlPage != null)
                        {
                            BDNode masterNode = BDNode.RetrieveNodeWithId(pDataContext, htmlPage.displayParentId.Value);
                            if (masterNode != null)
                            {
                                // determine the masterNode for the HTML page; generate the display context from that. 
                                seAssociation.displayContext = BDUtilities.BuildHierarchyString(pDataContext, masterNode, " : ");
                                BDSearchEntryAssociation.Save(pDataContext, seAssociation);
                            }
                            else
                            {
                                BDLinkedNoteAssociation masterAssn = BDLinkedNoteAssociation.RetrieveLinkedNoteAssociationWithId(pDataContext, htmlPage.displayParentId.Value);
                                seAssociation.displayContext = BDUtilities.BuildHierarchyString(pDataContext, masterAssn, " : ");
                                BDSearchEntryAssociation.Save(pDataContext, seAssociation);
                            }
                        }
                    }
                }
            }

            //[Obsolete] generateSearchEntries(pDataContext, pNode);  

            BDHtmlPageGeneratorLogEntry.AppendToFile("BDSearchGeneratorLog.txt", string.Format("End: {0} -------------------------------", DateTime.Now));
        }

        #region obsolete code to remove for viewer v2

        [Obsolete]
        private void generateSearchEntries(Entities pDataContext, IBDNode pNode)
        {
            List<IBDNode> chapters = new List<IBDNode>();  
            StringBuilder displayContext = new StringBuilder();
            if (pNode != null && pNode.NodeType == BDConstants.BDNodeType.BDChapter)
                chapters.Add(pNode);
            else 
                chapters.AddRange(BDNode.RetrieveNodesForType(pDataContext, BDConstants.BDNodeType.BDChapter));
            // processNodeList(pDataContext, chapters.ToList<IBDNode>(), editorContext);
       }

        [Obsolete]
        private void processNodeList(Entities pDataContext, List<IBDNode> pNodes, StringBuilder pNodeContext)
        {
            //string resolvedName = string.Empty;
            //foreach (IBDNode ibdNode in pNodes)
            //{
            //    resolvedName = string.Empty;
            //    switch (ibdNode.NodeType)
            //    {
            //        case BDConstants.BDNodeType.BDAttachment:
            //            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDAttachment.PROPERTYNAME_NAME);
            //            break;
            //        case BDConstants.BDNodeType.BDCombinedEntry:
            //            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDCombinedEntry.PROPERTYNAME_NAME);
            //            break;
            //        case BDConstants.BDNodeType.BDConfiguredEntry:
            //            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDConfiguredEntry.PROPERTYNAME_NAME);
            //            break;
            //        case BDConstants.BDNodeType.BDDosage:
            //            // no valid properties
            //            break;
            //        case BDConstants.BDNodeType.BDLinkedNote:
            //            break;
            //        case BDConstants.BDNodeType.BDPrecaution:
            //            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDPrecaution).Description, BDPrecaution.PROPERTYNAME_ORGANISM_1);
            //            break;
            //        case BDConstants.BDNodeType.BDTableCell:
            //            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTableCell).value, BDTableCell.PROPERTYNAME_CONTENTS);
            //            break;
            //        case BDConstants.BDNodeType.BDTherapy:
            //            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTherapy).Description, BDTherapy.PROPERTYNAME_THERAPY);
            //            break;
            //        case BDConstants.BDNodeType.BDTherapyGroup:
            //            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTherapyGroup).Description, BDTherapyGroup.PROPERTYNAME_NAME);
            //            break;
            //        default:
            //            // process all BDNodes, any type
            //            resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
            //            break;
            //    }

            //    List<IBDNode> childnodes = BDFabrik.GetChildrenForParent(pDataContext, ibdNode);
            //    Guid htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForOriginalIBDNodeId(pDataContext, ibdNode.Uuid);
            //    StringBuilder newContext = new StringBuilder();

            //    // build a string representation of the search entry's location in the hierarchy
            //    if (!string.IsNullOrEmpty(pNodeContext.ToString()))
            //        newContext.AppendFormat("{0} : {1}", pNodeContext, resolvedName);
            //    else
            //        newContext.Append(resolvedName);

            //    // recurse to process the next child layer
            //    if (childnodes.Count > 0)
            //        processNodeList(pDataContext, childnodes, newContext);

            //    // build the search entry
            //    //bool generateSearchAssociation = false;
            //    if (!string.IsNullOrEmpty(resolvedName) && htmlPageId != Guid.Empty && !(ibdNode is BDLinkedNote))
            //    {
            //        #region filter target node
            //        //switch (ibdNode.NodeType)
            //        //{
            //        //    case BDConstants.BDNodeType.BDAntimicrobial:
            //        //        switch (ibdNode.LayoutVariant)
            //        //        {
            //        //            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
            //        //                generateSearchAssociation = true;
            //        //                break;
            //        //            default:
            //        //                generateSearchAssociation = false;
            //        //                break;
            //        //        }
            //        //        break;
            //        //    case BDConstants.BDNodeType.BDMicroorganism:
            //        //        switch (ibdNode.LayoutVariant)
            //        //        {
            //        //            case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
            //        //                generateSearchAssociation = true;
            //        //                break;
            //        //            default:
            //        //                generateSearchAssociation = false;
            //        //                break;
            //        //        }
            //        //        break;
            //        //    case BDConstants.BDNodeType.BDPathogen:
            //        //        switch (ibdNode.LayoutVariant)
            //        //        {
            //        //            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
            //        //            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
            //        //                generateSearchAssociation = true;
            //        //                break;
            //        //            default:
            //        //                generateSearchAssociation = false;
            //        //                break;
            //        //        }
            //        //        break;
            //        //    //case BDConstants.BDNodeType.BDChapter:
            //        //    //case BDConstants.BDNodeType.BDSection:
            //        //    //case BDConstants.BDNodeType.BDCategory:
            //        //    //case BDConstants.BDNodeType.BDDisease:
            //        //    //case BDConstants.BDNodeType.BDPresentation:
            //        //    //    generateSearchAssociation = false;
            //        //    //    break;
            //        //    default:
            //        //        generateSearchAssociation = true;
            //        //        break;
            //        //}
            //        //if (generateSearchAssociation)
            //        #endregion
            //        generateSearchEntryLink(pDataContext, htmlPageId, ibdNode, resolvedName, pNodeContext.ToString());
            //    }
            //}
        }

        [Obsolete]
        private void generateSearchEntryLink(Entities pDataContext, Guid pOriginalNodeId, IBDNode pNode, string pResolvedName, string pDisplayContext)
        {
            //string entryName = pNode.Name.Trim();
            //List<BDSearchEntry> matchingSearchEntries = new List<BDSearchEntry>();
            //if (!string.IsNullOrEmpty(pResolvedName))
            //{
            //    pDisplayContext = pDisplayContext.Replace(":  :", ":");

            //    foreach (string searchEntryTerm in searchEntryList)
            //    {
            //        if (pResolvedName.IndexOf(searchEntryTerm, StringComparison.OrdinalIgnoreCase) >= 0)
            //        {
            //            BDSearchEntry matchedSearchEntry = BDSearchEntry.RetrieveWithName(pDataContext, searchEntryTerm);
            //            matchingSearchEntries.Add(matchedSearchEntry);
            //            matchedSearchEntry.show = true;
            //        }
            //        else
            //        {
            //            string shortName = pResolvedName.Replace(" ", "");
            //            string shortSearchTerm = searchEntryTerm.Replace(" ", "");
            //            if (shortName.IndexOf(shortSearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
            //            {
            //                BDSearchEntry matchedSearchEntry = BDSearchEntry.RetrieveWithName(pDataContext, searchEntryTerm);
            //                matchingSearchEntries.Add(matchedSearchEntry);
            //                matchedSearchEntry.show = true;
            //            }
            //        }
            //    }
            //    pDataContext.SaveChanges();
            //}
            ////BDHtmlPageGeneratorLogEntry.AppendToFile("BDSearchGeneratorLog.txt", string.Format(@"{0} matches for name: {1}", matchingSearchEntries.Count, pResolvedName));
            
            //foreach (BDSearchEntry entry in matchingSearchEntries)
            //{
            //    List<BDSearchEntryAssociation> associations = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryIdAndDisplayParentid(pDataContext, entry.uuid, pNode.Uuid);

            //    if (associations.Count() == 0)
            //    {
            //        BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, entry.Uuid, pNode.NodeType, pOriginalNodeId, pNode.LayoutVariant, pDisplayContext);
            //    }
            //}
        }

        //private string buildResolvedNameForNode(Entities pContext, IBDNode pNode, string pPropertyValue, string pPropertyName)
        //{
        //    List<Guid> immedAssociations;
        //    List<BDLinkedNote> immediate = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Immediate, out immedAssociations);

        //    //ks: added "New " prefix to permit the use of terms like "Table A" to appear in the name of a BDTable instance
        //    string namePlaceholderText = string.Format(@"New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
        //    if (pPropertyValue.Contains(namePlaceholderText) || pPropertyValue == "SINGLE PRESENTATION" || pPropertyValue == "(Header)")
        //        pPropertyValue = string.Empty;

        //    if (pNode.NodeType == BDConstants.BDNodeType.BDConfiguredEntry && (pNode.Name.Length >=5 && pNode.Name.Substring(0, 5) == "Entry"))
        //        pPropertyValue = string.Empty;

        //    string immediateText = BDUtilities.BuildTextFromInlineNotes(immediate);

        //    string resolvedName = string.Format("{0}{1}",pPropertyValue.Trim(), immediateText.Trim());

        //    if (resolvedName.Length == 0) resolvedName = null;

        //    return BDUtilities.ProcessTextToPlainText(pContext, resolvedName);
        //}
        #endregion
    }
}
