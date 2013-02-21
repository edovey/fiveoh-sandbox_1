using System;
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
        private List<string> searchEntryList;

        public BDSearchEntryGenerator() { }
        
        /// <summary>
        /// Generate new Search Entries from the data
        /// </summary>
        public void Generate(Entities pDataContext, IBDNode pNode)
        {
            // clear the data from the database
            // BDSearchEntry.DeleteAll(pDataContext);
            BDSearchEntryAssociation.DeleteAll(pDataContext);

            searchEntryList = BDSearchEntry.RetrieveSearchEntryNames(pDataContext);

            generateSearchEntries(pDataContext, pNode);
        }

        private void generateSearchEntries(Entities pDataContext, IBDNode pNode)
        {
            List<IBDNode> chapters = new List<IBDNode>();  
            StringBuilder displayContext = new StringBuilder();
            if (pNode != null && pNode.NodeType == BDConstants.BDNodeType.BDChapter)
                chapters.Add(pNode);
            else 
                chapters.AddRange(BDNode.RetrieveNodesForType(pDataContext, BDConstants.BDNodeType.BDChapter));
             processNodeList(pDataContext, chapters.ToList<IBDNode>(), displayContext);
       }

        private void processNodeList(Entities pDataContext, List<IBDNode> pNodes, StringBuilder pNodeContext)
        {
            string resolvedName = string.Empty;
            foreach (IBDNode ibdNode in pNodes)
            {
                switch (ibdNode.NodeType)
                {
                    case BDConstants.BDNodeType.BDCombinedEntry:
                        resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDCombinedEntry.PROPERTYNAME_NAME);
                        break;
                    case BDConstants.BDNodeType.BDConfiguredEntry:
                        resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDConfiguredEntry.PROPERTYNAME_NAME);
                        break;
                    case BDConstants.BDNodeType.BDDosage:
                        // no valid properties
                        break;
                    case BDConstants.BDNodeType.BDLinkedNote:
                        break;
                    case BDConstants.BDNodeType.BDPrecaution:
                        resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDPrecaution).Description, BDPrecaution.PROPERTYNAME_ORGANISM_1);
                        break;
                    case BDConstants.BDNodeType.BDTableCell:
                        resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTableCell).value, BDTableCell.PROPERTYNAME_CONTENTS);
                        break;
                    case BDConstants.BDNodeType.BDTherapy:
                        resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTherapy).Description, BDTherapy.PROPERTYNAME_THERAPY);
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTherapyGroup).Description, BDTherapyGroup.PROPERTYNAME_NAME);
                        break;
                    default:
                        // process all BDNodes, any type
                        resolvedName = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                        break;
                }

                List<IBDNode> childnodes = BDFabrik.GetChildrenForParent(pDataContext, ibdNode);
                Guid htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForOriginalIBDNodeId(pDataContext, ibdNode.Uuid);
                StringBuilder newContext = new StringBuilder();

                // build a string representation of the search entry's location in the hierarchy
                if (pNodeContext.Length > 0)
                    newContext.AppendFormat("{0} : {1}", pNodeContext, resolvedName);
                else
                    newContext.Append(resolvedName);

                // recurse to process the next child layer
                if (childnodes.Count > 0)
                    processNodeList(pDataContext, childnodes, newContext);

                // build the search entry
                //bool generateSearchAssociation = false;
                if (!string.IsNullOrEmpty(resolvedName) && htmlPageId != Guid.Empty && !(ibdNode is BDLinkedNote))
                {
                    //switch (ibdNode.NodeType)
                    //{
                    //    case BDConstants.BDNodeType.BDAntimicrobial:
                    //        switch (ibdNode.LayoutVariant)
                    //        {
                    //            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                    //                generateSearchAssociation = true;
                    //                break;
                    //            default:
                    //                generateSearchAssociation = false;
                    //                break;
                    //        }
                    //        break;
                    //    case BDConstants.BDNodeType.BDMicroorganism:
                    //        switch (ibdNode.LayoutVariant)
                    //        {
                    //            case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                    //                generateSearchAssociation = true;
                    //                break;
                    //            default:
                    //                generateSearchAssociation = false;
                    //                break;
                    //        }
                    //        break;
                    //    case BDConstants.BDNodeType.BDPathogen:
                    //        switch (ibdNode.LayoutVariant)
                    //        {
                    //            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                    //            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                    //                generateSearchAssociation = true;
                    //                break;
                    //            default:
                    //                generateSearchAssociation = false;
                    //                break;
                    //        }
                    //        break;
                    //    //case BDConstants.BDNodeType.BDChapter:
                    //    //case BDConstants.BDNodeType.BDSection:
                    //    //case BDConstants.BDNodeType.BDCategory:
                    //    //case BDConstants.BDNodeType.BDDisease:
                    //    //case BDConstants.BDNodeType.BDPresentation:
                    //    //    generateSearchAssociation = false;
                    //    //    break;
                    //    default:
                    //        generateSearchAssociation = true;
                    //        break;
                    //}
                    //if (generateSearchAssociation)
                    generateSearchEntryLink(pDataContext, htmlPageId, ibdNode, resolvedName, pNodeContext.ToString());
                }
            }
        }

        private void generateSearchEntryLink(Entities pDataContext, Guid pOriginalNodeId, IBDNode pNode, string pResolvedName, string pDisplayContext)
        {
            //string entryName = pNode.Name.Trim();
            List<BDSearchEntry> matchingSearchEntries = new List<BDSearchEntry>();
            if (!string.IsNullOrEmpty(pResolvedName))
            {
                foreach (string searchEntryTerm in searchEntryList)
                {
                    if (searchEntryTerm.IndexOf(pResolvedName, StringComparison.OrdinalIgnoreCase) >= 0)
                        matchingSearchEntries.Add(BDSearchEntry.RetrieveWithName(pDataContext, searchEntryTerm));
                    if (pResolvedName.IndexOf(searchEntryTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        matchingSearchEntries.Add(BDSearchEntry.RetrieveWithName(pDataContext, searchEntryTerm));
                }
            }
            foreach (BDSearchEntry entry in matchingSearchEntries)
            {
                List<BDSearchEntryAssociation> associations = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(pDataContext, entry.uuid);

                if (associations.Count() == 0)
                {
                    BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, entry.Uuid, pNode.NodeType, pOriginalNodeId, pNode.LayoutVariant, pDisplayContext);
                }
            }
        }

        private string buildResolvedNameForNode(Entities pContext, IBDNode pNode, string pPropertyValue, string pPropertyName)
        {
            List<BDLinkedNote> immediate = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Immediate);

            //ks: added "New " prefix to permit the use of terms like "Table A" to appear in the name of a BDTable instance
            string namePlaceholderText = string.Format(@"New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
            if (pPropertyValue.Contains(namePlaceholderText) || pPropertyValue == "SINGLE PRESENTATION" || pPropertyValue == "(Header)")
                pPropertyValue = string.Empty;

            if (pNode.NodeType == BDConstants.BDNodeType.BDConfiguredEntry && (pNode.Name.Length >=5 && pNode.Name.Substring(0, 5) == "Entry"))
                pPropertyValue = string.Empty;

            string immediateText = BDUtilities.BuildTextFromInlineNotes(immediate, null);

            string resolvedName = string.Format("{0}{1}",pPropertyValue.Trim(), immediateText.Trim());

            if (resolvedName.Length == 0) resolvedName = null;

            return BDUtilities.ProcessTextToPlainText(pContext, resolvedName);
        }
    }
}
