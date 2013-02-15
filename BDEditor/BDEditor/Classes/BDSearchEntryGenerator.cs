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
                if(!string.IsNullOrEmpty(resolvedName) && htmlPageId != Guid.Empty && !(ibdNode is BDLinkedNote))
                    generateLinkForEntryWithDisplayParent(pDataContext, htmlPageId, ibdNode, resolvedName, pNodeContext.ToString());
            }
        }

        private void generateLinkForEntryWithDisplayParent(Entities pDataContext, Guid pOriginalNodeId, IBDNode pNode, string pResolvedName, string pDisplayContext)
        {
            //string entryName = pNode.Name.Trim();
            BDSearchEntry searchEntry = null;
            if (!string.IsNullOrEmpty(pResolvedName))
            {
                // get existing matching search entries
                IQueryable<BDSearchEntry> entries = (from entry in pDataContext.BDSearchEntries
                                                     where entry.name.ToLower().Contains(pResolvedName.ToLower())
                                                     select entry);

                // get existing matching search entries
                IQueryable<BDSearchEntry> contains = (from entry in pDataContext.BDSearchEntries
                                                      where pResolvedName.ToLower().Contains(entry.name.ToLower())
                                                     select entry);
                if (entries.Count() > 0)
                    searchEntry = entries.First<BDSearchEntry>();
                else if(contains.Count() > 0)
                    searchEntry = contains.First<BDSearchEntry>();
                if(searchEntry != null)
                {
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

            return BDUtilities.ProcessTextForSubscriptAndSuperscriptMarkup(pContext, resolvedName);
        }
    }
}
