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
        public const string AWS_DOMAIN = @"bd_categories";
        public const string ENTITYNAME = @"BDCategories";
        public const string ENTITYNAME_FRIENDLY = @"Category";

        private const string UUID = @"ct_uuid";
        private const string SCHEMAVERSION = @"ct_schemaversion";
        private const string CREATEDBY = @"ct_createdby";
        private const string CREATEDDATE = @"ct_createddate";
        private const string MODIFIEDBY = @"ct_modifiedby";
        private const string MODIFIEDDATE = @"ct_modifieddate";
        private const string SECTIONID = @"ct_sectionid";
        private const string NAME = @"ct_name";
        private const string DEPRECATED = @"ct_deprecated";
        private const string DISPLAYORDER = @"ct_displayorder";

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
            category.displayOrder = -1;
            category.sectionId = Guid.Empty;
            category.name = string.Empty;

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
                //pCategory.modifiedBy = Guid.Empty;
                //pCategory.modifiedDate = DateTime.Now;

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
            List<BDCategory> entryList = new List<BDCategory>();
            IQueryable<BDCategory> entries = (from entry in pContext.BDCategories
                                                 where entry.sectionId == pSectionId
                                                 orderby entry.displayOrder
                                                 select entry);
            if (entries.Count<BDCategory>() > 0)
                entryList = entries.ToList<BDCategory>();
            return entryList;
        }


        /// <summary>
        /// Get Category with specified ID
        /// </summary>
        /// <param name="pCategoryId"></param>
        /// <returns>BDCategory object.</returns>
        public static BDCategory GetCategoryWithId(Entities pContext, Guid pCategoryId)
        {
            BDCategory category = null;
            if (null != pCategoryId)
            {
                IQueryable<BDCategory> entries = (from bdCategories in pContext.BDCategories
                                                  where bdCategories.uuid == pCategoryId
                                                  select bdCategories);

                if (entries.Count<BDCategory>() > 0)
                    category = entries.AsQueryable().First<BDCategory>();
            }
            return category;
        }

        protected override void OnPropertyChanged(string property)
        {
            switch (property)
            {
                case "createdBy":
                case "createdDate":
                case "modifiedBy":
                case "modifiedDate":
                    break;
                default:
                    {
                        modifiedBy = Guid.Empty;
                        modifiedDate = DateTime.Now;
                    }
                    break;
            }

            base.OnPropertyChanged(property);
        }

        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDCategory> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
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
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDCategory entry = BDCategory.GetCategoryWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDCategory.CreateBDCategory(uuid, deprecated);
                pDataContext.AddObject("BDCategories", entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.sectionId = Guid.Parse(pAttributeDictionary[SECTIONID]);
            entry.name = pAttributeDictionary[NAME];
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;

            if(pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.SECTIONID).WithValue((null == sectionId) ? Guid.Empty.ToString() : sectionId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.NAME).WithValue(name).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            return putAttributeRequest;
        }

        #endregion
    }
}
