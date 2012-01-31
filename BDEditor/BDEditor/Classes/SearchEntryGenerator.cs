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
    public class SearchEntryGenerator
    {
        public SearchEntryGenerator() { }
        
        /// <summary>
        /// Generate new Search Entries from the data
        /// </summary>
        public static void Generate()
        {
            // clear the data from the databse
            BDSearchEntry.DeleteAll();
            BDSearchEntryAssociation.DeleteAll();

            GenerateSearchEntries();
        }

        private static void GenerateSearchEntries()
        {
            Entities dataContext = new Entities();
            List<BDMetadata> metadataList = BDMetadata.GetAll(dataContext);

            foreach (BDMetadata mdEntry in metadataList)
            {
                // get metadata entry entity to get search term
                string entryName = GetEntryName(dataContext, mdEntry);

                // get existing matching search entries
                IQueryable<BDSearchEntry> entries = (from entry in dataContext.BDSearchEntries
                                                     where entry.name == entryName
                                                     select entry);

                // if matching search entry is not found, create one
                if (entries.Count() == 0)
                {
                    BDSearchEntry searchEntry = BDSearchEntry.CreateSearchEntry(dataContext);
                    searchEntry.name = entryName;
                    // also create search association record
                    Guid assnGuid = CreateEntryAssociation(dataContext, mdEntry, searchEntry);
                    BDSearchEntry.Save(dataContext, searchEntry);
                }
                else
                {
                    BDSearchEntry searchEntry = entries.First<BDSearchEntry>();
                    // get matching search association records for search entry
                    IQueryable<BDSearchEntryAssociation> associations = (from entry in dataContext.BDSearchEntryAssociations
                                                                         where (entry.searchEntryId == searchEntry.uuid
                                                                         && entry.displayParentId == mdEntry.displayParentId
                                                                         && entry.displayParentKeyName == mdEntry.displayParentKeyName)
                                                                         select entry);

                    if (associations.Count() == 0)
                    {
                        Guid newAssociation = CreateEntryAssociation(dataContext, mdEntry, searchEntry);
                    }
                }
            }
        }

        private static string GetEntryName(Entities pContext, BDMetadata metadata)
        {
            string result = String.Empty;
            switch (metadata.itemKeyName)
            {
                case BDTherapy.ENTITYNAME:
                    result = BDTherapy.GetTherapyWithId(pContext, metadata.itemId.Value).name;
                    break;

                case BDNode.ENTITYNAME:
                    result = BDNode.GetNodeWithId(pContext, metadata.itemId.Value).name;
                    break;

                default:
                    break;
            }
            return result;
        }

        private static string GetDisplayContext(Entities pContext, BDMetadata pMetadata)
        {
            string result = String.Empty;

            switch (pMetadata.NodeType)
            {
                case BDConstants.BDNodeType.BDDisease:
                    {

                        BDNode disease = BDNode.GetNodeWithId(pContext, pMetadata.displayParentId.Value); 
                        BDNode category = BDNode.GetNodeWithId(pContext, disease.ParentId.Value);
                        result = string.Format("{0} : {1}", category.name, disease.name);
                    }
                    break;
                    
            }
            return result;
        }

        private static Guid CreateEntryAssociation(Entities pContext, BDMetadata pMetadata, BDSearchEntry pSearchEntry)
        {
            BDSearchEntryAssociation a = BDSearchEntryAssociation.CreateSearchEntryAssociation(pContext);
            a.searchEntryId = pSearchEntry.uuid;
            a.displayParentId = pMetadata.displayParentId;
            a.displayParentKeyName = pMetadata.displayParentKeyName;

            a.displayContext = GetDisplayContext(pContext, pMetadata);
            
            BDSearchEntryAssociation.Save(pContext, a);

            return a.uuid;
        }
    }
}
