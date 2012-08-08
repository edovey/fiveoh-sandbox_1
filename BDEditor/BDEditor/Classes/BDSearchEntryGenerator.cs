using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static void Generate()
        {
            // clear the data from the database
            BDSearchEntry.DeleteAll();
            BDSearchEntryAssociation.DeleteAll();

            GenerateSearchEntries();
        }

        private static void GenerateSearchEntries()
        {
            Entities dataContext = new Entities();
            List<BDNode> chapters = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDChapter);

            foreach (BDNode chapter in chapters)
            {
                string chapterDisplayContext = chapter.Name;
                switch (chapter.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        {
                            List<IBDNode> sections = BDFabrik.GetChildrenForParent(dataContext, chapter);
                            foreach (IBDNode section in sections)
                            {
                                string sectionDisplayContext = chapterDisplayContext + " : " + section.Name;
                                switch (section.LayoutVariant)
                                {
                                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                                        {
                                            List<IBDNode> categories = BDFabrik.GetChildrenForParent(dataContext, section);
                                            foreach (IBDNode category in categories)
                                            {
                                                string categoryDisplayContext = sectionDisplayContext + " : " + category.Name;
                                                List<IBDNode> diseases = BDFabrik.GetChildrenForParent(dataContext, category);
                                                foreach (IBDNode disease in diseases)
                                                {
                                                    string diseaseDisplayContext = categoryDisplayContext + " : " + disease.Name;

                                                    List<IBDNode> presentations = BDFabrik.GetChildrenForParent(dataContext, disease);
                                                    foreach (IBDNode presentation in presentations)
                                                    {
                                                        string presentationDisplayContext = string.Empty;
                                                        if (presentation.Name != "SINGLE PRESENTATION")
                                                            presentationDisplayContext = diseaseDisplayContext + " : " + presentation.Name;
                                                        else
                                                            presentationDisplayContext = diseaseDisplayContext;

                                                        List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(dataContext, presentation);
                                                        foreach (IBDNode pathogenGroup in pathogenGroups)
                                                        {
                                                            string pathogenGroupDisplayContext = presentationDisplayContext + " : " + pathogenGroup.Name;
                                                            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(dataContext, pathogenGroup);
                                                            foreach (IBDNode node in childNodes)
                                                            {
                                                                BDNode displayParentNode = presentation as BDNode;
                                                                switch (node.NodeType)
                                                                {
                                                                    case BDConstants.BDNodeType.BDPathogen:
                                                                        {
                                                                            if (null != displayParentNode && node.Name.Length > 0)
                                                                                generateEntryWithDisplayParent(dataContext, displayParentNode, node, pathogenGroupDisplayContext);
                                                                        }
                                                                        break;
                                                                    case BDConstants.BDNodeType.BDTherapyGroup:
                                                                        {
                                                                            string therapyGroupDisplayContext = pathogenGroupDisplayContext + " : " + node.Name;
                                                                           List<IBDNode> therapies = BDFabrik.GetChildrenForParent(dataContext, node);
                                                                            foreach (IBDNode therapy in therapies)
                                                                            {
                                                                                if (null != displayParentNode && therapy.Name.Length > 0)
                                                                                    generateEntryWithDisplayParent(dataContext, displayParentNode, therapy, therapyGroupDisplayContext);
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


        private static void generateEntryWithDisplayParent(Entities pDataContext, BDNode pDisplayParent, IBDNode pNode, string pDisplayContext)
        {
            string entryName = pNode.Name.Trim();

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
