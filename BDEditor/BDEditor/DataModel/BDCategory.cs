using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDCategory
    /// </summary>
    public partial class BDCategory
    {
        public const string AWS_DOMAIN = @"bd_categories_test";

        private const string UUID = @"ct_uuid";
        private const string SCHEMAVERSION = @"ct_schemaversion";
        private const string CREATEDBY = @"ct_createdby";
        private const string CREATEDDATE = @"ct_createddate";
        private const string MODIFIEDBY = @"ct_modifiedby";
        private const string MODIFIEDDATE = @"ct_modifieddate";
        private const string SECTIONID = @"ct_sectionid";
        private const string NAME = @"ct_name";
        private const string DEPRECATED = @"ct_deprecated";

        /// <summary>
        /// Extended Create Method that includes setting creation date and schema version.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static BDCategory CreateCategory(Entities pContext)
        {
                BDCategory category = CreateBDCategory(Guid.NewGuid(), false);
                category.createdBy = Guid.Empty;
                category.createdDate = DateTime.Now;
                category.schemaVersion = 0;

                pContext.AddObject("BDCategories", category);
            
                return category;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pCategory"></param>
        public static void SaveCategory(Entities pContext, BDCategory pCategory)
        {
            if (pCategory.EntityState != System.Data.EntityState.Unchanged)
            {
                pCategory.modifiedBy = Guid.Empty;
                pCategory.modifiedDate = DateTime.Now;

                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all sections in the model with the specified section ID
        /// </summary>
        /// <param name="pSectionId"></param>
        /// <returns>List of Categories</returns>
        public static List<BDCategory> GetCategoriesForSectionId(Entities pContext, Guid pSectionId)
        {
            List<BDCategory> catList = new List<BDCategory>();
            IQueryable<BDCategory> categories = (from bdCategories in pContext.BDCategories
                                                 where bdCategories.sectionId == pSectionId
                                                 select bdCategories);
            foreach (BDCategory cat in categories)
            {
                catList.Add(cat);
            }
            return catList;
        }

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDCategory> GetCategoriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDCategory> entryList = new List<BDCategory>();
            IQueryable<BDCategory> categories;

            if (null == pUpdateDateTime)
            {
                categories = (from entry in pContext.BDCategories
                              select entry);
            }
            else
            {
                categories = (from entry in pContext.BDCategories
                              where entry.modifiedDate > pUpdateDateTime.Value
                              select entry);
            }

            if (categories.Count() > 0)
            {
                entryList = categories.ToList<BDCategory>();
            }

            return entryList;
        }

        /// <summary>
        /// Get Category with specified ID
        /// </summary>
        /// <param name="pCategoryId"></param>
        /// <returns>BDCategory object.</returns>
        public static BDCategory GetCategoryWithId(Entities pContext, Guid pCategoryId)
        {
            BDCategory category;
            IQueryable<BDCategory> categories = (from bdCategories in pContext.BDCategories
                                                 where bdCategories.uuid == pCategoryId
                                                 select bdCategories);
            category = categories.AsQueryable().First<BDCategory>();
            return category;
        }


        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
        }

        /// <summary>
        /// Create or update an existing BDCategory from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDCategory entry = BDCategory.GetCategoryWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDCategory.CreateBDCategory(uuid, deprecated);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.sectionId = Guid.Parse(pAttributeDictionary[SECTIONID]);
            entry.name = pAttributeDictionary[NAME];
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);

            BDCategory.SaveCategory(pDataContext, entry);

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.UUID).WithValue(uuid.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.CREATEDBY).WithValue( (null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper() ));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)));

            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.SECTIONID).WithValue((null == sectionId) ? Guid.Empty.ToString() : sectionId.ToString().ToUpper()));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.NAME).WithValue(name));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.DEPRECATED).WithValue(deprecated.ToString()));

            return putAttributeRequest;
        }
    }
}
