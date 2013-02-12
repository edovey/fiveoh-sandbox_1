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
        public BDSearchEntryGenerator() { }
        
        /// <summary>
        /// Generate new Search Entries from the data
        /// </summary>
        public void Generate(Entities pDataContext, IBDNode pNode)
        {
            // clear the data from the database
            // BDSearchEntry.DeleteAll(pDataContext);
            BDSearchEntryAssociation.DeleteAll(pDataContext);

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
            string currentContext = string.Empty;
            string searchableText = string.Empty;
            foreach (IBDNode ibdNode in pNodes)
            {
                switch (ibdNode.NodeType)
                {
                    case BDConstants.BDNodeType.BDAntimicrobial:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                        searchableText = currentContext;
                        break;
                    case BDConstants.BDNodeType.BDAntimicrobialRisk:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                        searchableText = currentContext;
                        break;
                    case BDConstants.BDNodeType.BDCombinedEntry:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDCombinedEntry.PROPERTYNAME_NAME);
                        switch (ibdNode.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                                searchableText = currentContext;
                                break;
                            default:
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDCategory:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                        searchableText = currentContext;
                        break;
                    case BDConstants.BDNodeType.BDConfiguredEntry:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDConfiguredEntry.PROPERTYNAME_NAME);
                        switch (ibdNode.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                                searchableText = currentContext;
                                break;
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01_CNS_Meningitis_Table:
                            default:
                                break;
                        }
                        searchableText = currentContext;
                        break;
                    case BDConstants.BDNodeType.BDDosage:
                        // no valid properties
                        break;
                    case BDConstants.BDNodeType.BDLinkedNote:
                        currentContext = (ibdNode as BDLinkedNote).DescriptionForLinkedNote;
                        searchableText = currentContext;
                        break;
                    case BDConstants.BDNodeType.BDPrecaution:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDPrecaution).Description, BDPrecaution.PROPERTYNAME_ORGANISM_1);
                        searchableText = currentContext;
                        break;
                    case BDConstants.BDNodeType.BDSection:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                        searchableText = currentContext;
                        break;
                    case BDConstants.BDNodeType.BDSubcategory:
                                currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                        switch (ibdNode.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                                searchableText = currentContext;
                                break;
                            default:
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSubtopic:
                                currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                        switch (ibdNode.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                                searchableText = currentContext;
                                break;
                            default:
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDTableCell:
                        if (ibdNode.DisplayOrder == 0)
                        {
                                    currentContext = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTableCell).value, BDTableCell.PROPERTYNAME_CONTENTS);
                            switch (ibdNode.LayoutVariant)
                            {
                                case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_Classifications_ContentRow:
                                case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity_ContentRow:
                                case BDConstants.LayoutVariantType.Antibiotics_NameListing_ContentRow:
                                case BDConstants.LayoutVariantType.Antibiotics_Stepdown_ContentRow:
                                    searchableText = currentContext;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case BDConstants.BDNodeType.BDTherapy:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTherapy).Description, BDTherapy.PROPERTYNAME_THERAPY);
                        searchableText = currentContext;
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, (ibdNode as BDTherapyGroup).Description, BDTherapyGroup.PROPERTYNAME_NAME);
                        break;
                    case BDConstants.BDNodeType.BDTableRow:
                    case BDConstants.BDNodeType.BDTopic:
                    default:
                        // process the node name to add to the context
                        currentContext = buildResolvedNameForNode(pDataContext, ibdNode, ibdNode.Name, BDNode.PROPERTYNAME_NAME);
                        break;
                }

                List<IBDNode> childnodes = BDFabrik.GetChildrenForParent(pDataContext, ibdNode);
                Guid htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForOriginalIBDNodeId(pDataContext, ibdNode.Uuid); 
                StringBuilder newContext = new StringBuilder();
                
                // build a string representation of the search entry's location in the hierarchy
                if (pNodeContext.Length > 0)
                    newContext.AppendFormat("{0} : {1}", pNodeContext, currentContext);
                else
                    newContext.Append(currentContext);

                // recurse to process the next child layer
                if (childnodes.Count > 0)
                    processNodeList(pDataContext, childnodes, newContext);

                    // build the search entry
                if(!string.IsNullOrEmpty(searchableText) && htmlPageId != Guid.Empty && !(ibdNode is BDLinkedNote))
                    generateEntryWithDisplayParent(pDataContext, htmlPageId, ibdNode, searchableText, pNodeContext.ToString());
            }
        }

        private void generateEntryWithDisplayParent(Entities pDataContext, Guid pOriginalNodeId, IBDNode pNode, string pCurrentContext, string pDisplayContext)
        {
            //string entryName = pNode.Name.Trim();
            string entryName = buildResolvedNameForNode(pDataContext, pNode, pNode.Name, BDNode.PROPERTYNAME_NAME);

            if (!string.IsNullOrEmpty(entryName))
            {
                // get existing matching search entries
                IQueryable<BDSearchEntry> entries = (from entry in pDataContext.BDSearchEntries
                                                     where entry.name == entryName
                                                     select entry);

                // if matching search entry is not found, create one
                if (entries.Count() == 0)
                {
                    // create and save new search entry
                    BDSearchEntry searchEntry = BDSearchEntry.CreateBDSearchEntry(pDataContext, entryName);

                    // Create search association record
                    BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, searchEntry.Uuid, pNode.NodeType, pOriginalNodeId, pNode.LayoutVariant, pDisplayContext);
                }
                else
                {
                    BDSearchEntry searchEntry = entries.First<BDSearchEntry>();
                    // get matching search association records for search entry
                    IQueryable<BDSearchEntryAssociation> associations = (from entry in pDataContext.BDSearchEntryAssociations
                                                                         where (entry.searchEntryId == searchEntry.uuid
                                                                         && entry.displayParentId == pOriginalNodeId)
                                                                         select entry);

                    if (associations.Count() == 0)
                    {
                        BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, searchEntry.Uuid, pNode.NodeType, pOriginalNodeId, pNode.LayoutVariant, pDisplayContext);
                    }
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

            //TextInfo tInfo = new CultureInfo("en-US", false).TextInfo;

            //string resolvedName = string.Format("{0}{1}", tInfo.ToTitleCase(pPropertyValue.Trim()), immediateText.Trim());
            string resolvedName = string.Format("{0}{1}",pPropertyValue.Trim(), immediateText.Trim());

            if (resolvedName.Length == 0) resolvedName = null;

            return resolvedName;
        }
    }
}
