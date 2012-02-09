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
            // Clear the data from the remote store
            RepositoryHandler.Aws.DeleteRemoteForSearch();

            // clear the data from the databse
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
                            List<IBDNode> sections = BDFabrik.GetChildrenForParentId(dataContext, chapter.Uuid);
                            foreach (IBDNode section in sections)
                            {
                                string sectionDisplayContext = chapterDisplayContext + " : " + section.Name;
                                switch (section.LayoutVariant)
                                {
                                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                                        {
                                            List<IBDNode> categories = BDFabrik.GetChildrenForParentId(dataContext, section.Uuid);
                                            foreach (IBDNode category in categories)
                                            {
                                                string categoryDisplayContext = sectionDisplayContext + " : " + category.Name;
                                                List<IBDNode> diseases = BDFabrik.GetChildrenForParentId(dataContext, category.Uuid);
                                                foreach (IBDNode disease in diseases)
                                                {
                                                    string diseaseDisplayContext = categoryDisplayContext + " : " + disease.Name;

                                                    //TODO:  how will we navigate to diseases in the viewer when a disease-search-entry is selected?
                                                    List<IBDNode> presentations = BDFabrik.GetChildrenForParentId(dataContext, disease.Uuid);
                                                    foreach (IBDNode presentation in presentations)
                                                    {
                                                        string presentationDisplayContext = diseaseDisplayContext + " : " + presentation.Name;

                                                        List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParentId(dataContext, presentation.Uuid);
                                                        foreach (IBDNode pathogenGroup in pathogenGroups)
                                                        {
                                                            List<IBDNode> childNodes = BDFabrik.GetChildrenForParentId(dataContext, pathogenGroup.Uuid);
                                                            foreach (IBDNode node in childNodes)
                                                            {
                                                                BDNode parentNode = presentation as BDNode;
                                                                switch (node.NodeType)
                                                                {
                                                                    case BDConstants.BDNodeType.BDPathogen:
                                                                        {
                                                                            if (null != parentNode)
                                                                                generateEntryWithDisplayParent(dataContext, parentNode, node, presentationDisplayContext.ToString());
                                                                        }
                                                                        break;
                                                                    case BDConstants.BDNodeType.BDTherapyGroup:
                                                                        {
                                                                            List<IBDNode> therapies = BDFabrik.GetChildrenForParentId(dataContext, node.Uuid);
                                                                            foreach (IBDNode therapy in therapies)
                                                                            {
                                                                                if (null != parentNode)
                                                                                    generateEntryWithDisplayParent(dataContext, parentNode, therapy, presentationDisplayContext.ToString());
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
            string entryName = pNode.Name;

            // get existing matching search entries
            IQueryable<BDSearchEntry> entries = (from entry in pDataContext.BDSearchEntries
                                                 where entry.name == entryName
                                                 select entry);

            // if matching search entry is not found, create one
            if (entries.Count() == 0)
            {
                // create and save new search entry
                BDSearchEntry searchEntry = BDSearchEntry.CreateSearchEntry(pDataContext, entryName);
                
                // Create search association record
                BDSearchEntryAssociation.CreateSearchEntryAssociation(pDataContext, pNode.Uuid, pNode.NodeType, pDisplayParent.Uuid, pDisplayParent.NodeType, pNode.LayoutVariant, pDisplayContext);
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
                    BDSearchEntryAssociation.CreateSearchEntryAssociation(pDataContext, pNode.Uuid, pNode.NodeType, pDisplayParent.Uuid, pDisplayParent.NodeType, pNode.LayoutVariant, pDisplayContext);
                }
            }
        }
    }
}
