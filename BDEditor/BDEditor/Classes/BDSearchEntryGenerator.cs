using System;
using System.Collections.Generic;
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
        public static void Generate(Entities pDataContext, IBDNode pNode, List<BDHtmlPageMap> pPagesMap)
        {
            // clear the data from the database
            BDSearchEntry.DeleteAll();
            BDSearchEntryAssociation.DeleteAll();

            generateSearchEntries(pDataContext, pNode, pPagesMap);
        }

        private static void generateSearchEntries(Entities pDataContext, IBDNode pNode, List<BDHtmlPageMap> pPagesMap)
        {
            List<IBDNode> chapters = new List<IBDNode>();  
            StringBuilder displayContext = new StringBuilder();
            if (pNode != null)
                chapters.Add(pNode);
            else 
                chapters.AddRange(BDNode.RetrieveNodesForType(pDataContext, BDConstants.BDNodeType.BDChapter));
             processNodeList(pDataContext, chapters.ToList<IBDNode>(), displayContext, pPagesMap);
       }

        private static void processNodeList(Entities pDataContext, List<IBDNode> pNodes, StringBuilder pNodeContext, List<BDHtmlPageMap> pPagesMap)
        {
            string currentContext = string.Empty;
            foreach (IBDNode ibdNode in pNodes)
            {
                switch (ibdNode.NodeType)
                {
                    case BDConstants.BDNodeType.BDAntimicrobialRisk:
                        currentContext = (ibdNode as BDAntimicrobialRisk).name;
                        break;
                    case BDConstants.BDNodeType.BDConfiguredEntry:
                        currentContext = (ibdNode as BDConfiguredEntry).Name;
                        break;
                    case BDConstants.BDNodeType.BDDosage:
                        // no valid properties
                        break;
                    case BDConstants.BDNodeType.BDLinkedNote:
                        currentContext = (ibdNode as BDLinkedNote).DescriptionForLinkedNote;
                        break;
                    case BDConstants.BDNodeType.BDPrecaution:
                        currentContext = (ibdNode as BDPrecaution).Description;
                        break;
                    case BDConstants.BDNodeType.BDTableCell:
                        currentContext = (ibdNode as BDTableCell).value;
                        break;
                    case BDConstants.BDNodeType.BDTableRow:
                        break;
                    case BDConstants.BDNodeType.BDTherapy:
                        currentContext = (ibdNode as BDTherapy).name;
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        currentContext = (ibdNode as BDTherapyGroup).name;
                        break;
                    default:
                        if (ibdNode.Name != string.Empty && !ibdNode.Name.Contains(BDUtilities.GetEnumDescription(ibdNode.NodeType)))
                            currentContext = ibdNode.Name;
                        break;
                }
                List<IBDNode> childnodes = BDFabrik.GetChildrenForParent(pDataContext, ibdNode);
                BDHtmlPageMap mapEntry = pPagesMap.FirstOrDefault<BDHtmlPageMap>(x => x.OriginalIBDObjectId == ibdNode.Uuid);
                StringBuilder newContext = new StringBuilder();
                
                if (pNodeContext.Length > 0)
                    newContext.AppendFormat("{0} : {1}", pNodeContext, currentContext);
                else
                    newContext.Append(currentContext);

                if (childnodes.Count > 0)
                    processNodeList(pDataContext, childnodes, newContext, pPagesMap);

                else if(null != mapEntry)
                    generateEntryWithDisplayParent(pDataContext, mapEntry.HtmlPageId, ibdNode, pNodeContext.ToString());
            }
        }

        private static void GenerateSearchEntries(Entities pDataContext)
        {
            List<BDNode> chapters = BDNode.RetrieveNodesForType(pDataContext, BDConstants.BDNodeType.BDChapter);

            foreach (BDNode chapter in chapters)
            {
                string chapterDisplayContext = chapter.Name;
                switch (chapter.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        {
                            List<IBDNode> sections = BDFabrik.GetChildrenForParent(pDataContext, chapter);
                            foreach (IBDNode section in sections)
                            {
                                string sectionDisplayContext = chapterDisplayContext + " : " + section.Name;
                                switch (section.LayoutVariant)
                                {
                                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                                        {
                                            List<IBDNode> categories = BDFabrik.GetChildrenForParent(pDataContext, section);
                                            foreach (IBDNode category in categories)
                                            {
                                                string categoryDisplayContext = sectionDisplayContext + " : " + category.Name;
                                                List<IBDNode> diseases = BDFabrik.GetChildrenForParent(pDataContext, category);
                                                foreach (IBDNode disease in diseases)
                                                {
                                                    string diseaseDisplayContext = categoryDisplayContext + " : " + disease.Name;

                                                    List<IBDNode> presentations = BDFabrik.GetChildrenForParent(pDataContext, disease);
                                                    foreach (IBDNode presentation in presentations)
                                                    {
                                                        string presentationDisplayContext = string.Empty;
                                                        if (presentation.Name != "SINGLE PRESENTATION")
                                                            presentationDisplayContext = diseaseDisplayContext + " : " + presentation.Name;
                                                        else
                                                            presentationDisplayContext = diseaseDisplayContext;

                                                        List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pDataContext, presentation);
                                                        foreach (IBDNode pathogenGroup in pathogenGroups)
                                                        {
                                                            string pathogenGroupDisplayContext = presentationDisplayContext + " : " + pathogenGroup.Name;
                                                            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pDataContext, pathogenGroup);
                                                            foreach (IBDNode node in childNodes)
                                                            {
                                                                BDNode displayParentNode = presentation as BDNode;
                                                                switch (node.NodeType)
                                                                {
                                                                    case BDConstants.BDNodeType.BDPathogen:
                                                                        {
                                                                            if (null != displayParentNode && node.Name.Length > 0)
                                                                                generateEntryWithDisplayParent(pDataContext, displayParentNode, node, pathogenGroupDisplayContext);
                                                                        }
                                                                        break;
                                                                    case BDConstants.BDNodeType.BDTherapyGroup:
                                                                        {
                                                                            string therapyGroupDisplayContext = pathogenGroupDisplayContext + " : " + node.Name;
                                                                            List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pDataContext, node);
                                                                            foreach (IBDNode therapy in therapies)
                                                                            {
                                                                                if (null != displayParentNode && therapy.Name.Length > 0)
                                                                                    generateEntryWithDisplayParent(pDataContext, displayParentNode, therapy, therapyGroupDisplayContext);
                                                                            }
                                                                        }
                                                                        break;

                                                                    default:
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public static void GenerateSearchEntryFromPageMap(Entities pDataContext, BDHtmlPageMap pHtmlPageMap)
        {
            // retrieve original node using uuid in pHtmlPage
            IBDNode node = BDFabrik.RetrieveNode(pDataContext, pHtmlPageMap.OriginalIBDObjectId);
            // query for an existing search entry
            // get existing matching search entries
            IQueryable<BDSearchEntry> entries = (from entry in pDataContext.BDSearchEntries
                                                 where entry.name == node.Name
                                                 select entry);
            if (node != null)
            {
                // generate the hierarchy from the node parent data
                string nodeContext = BDUtilities.BuildHierarchyString(pDataContext, node, " : ");

                // create the search entry & association as required.
                // if matching search entry is not found, create one
                if (entries.Count() == 0)
                {
                    // create and save new search entry
                    BDSearchEntry searchEntry = BDSearchEntry.CreateBDSearchEntry(pDataContext, node.Name);

                    // Create search association record
                    BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, searchEntry.Uuid, node.NodeType, pHtmlPageMap.HtmlPageId, node.NodeType, node.LayoutVariant, nodeContext);
                }
                else
                {
                    BDSearchEntry searchEntry = entries.First<BDSearchEntry>();
                    // get matching search association records for search entry
                    IQueryable<BDSearchEntryAssociation> associations = (from entry in pDataContext.BDSearchEntryAssociations
                                                                         where (entry.searchEntryId == searchEntry.uuid
                                                                         && entry.displayParentId == pHtmlPageMap.HtmlPageId
                                                                         && entry.displayParentType == (int)node.NodeType)
                                                                         select entry);

                    if (associations.Count() == 0)
                    {
                        BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, searchEntry.Uuid, node.NodeType, pHtmlPageMap.HtmlPageId, node.NodeType, node.LayoutVariant, nodeContext);
                    }
                }
            }
            else
                Debug.WriteLine("Search Entry Generation:  node was not found for:  {0}",pHtmlPageMap.OriginalIBDObjectId);
        }

        private static void generateEntryWithDisplayParent(Entities pDataContext, Guid pOriginalNodeId, IBDNode pNode, string pDisplayContext)
        {
            string entryName = pNode.Name.Trim();
            if (entryName.Length > 0)
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

        private static void generateEntryWithDisplayParent(Entities pDataContext, BDNode pDisplayParent, IBDNode pNode, string pDisplayContext)
        {
            string entryName = pNode.Name.Trim();
            if(pNode.Name.Contains(BDUtilities.GetEnumDescription(pNode.NodeType)) || pNode.Name == "(Header)")
                entryName = string.Empty;

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
                    BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, searchEntry.Uuid, pNode.NodeType, pDisplayParent.Uuid, pDisplayParent.NodeType, pNode.LayoutVariant, pDisplayContext);
                }
                else
                {
                    BDSearchEntry searchEntry = entries.First<BDSearchEntry>();
                    // get matching search association records for search entry
                    IQueryable<BDSearchEntryAssociation> associations = (from entry in pDataContext.BDSearchEntryAssociations
                                                                         where (entry.searchEntryId == searchEntry.uuid
                                                                         && entry.displayParentId == pDisplayParent.Uuid
                                                                         && entry.displayParentType == (int)pDisplayParent.NodeType)
                                                                         select entry);

                    if (associations.Count() == 0)
                    {
                        BDSearchEntryAssociation.CreateBDSearchEntryAssociation(pDataContext, searchEntry.Uuid, pNode.NodeType, pDisplayParent.Uuid, pDisplayParent.NodeType, pNode.LayoutVariant, pDisplayContext);
                    }
                }
            }
        }
    }
}
