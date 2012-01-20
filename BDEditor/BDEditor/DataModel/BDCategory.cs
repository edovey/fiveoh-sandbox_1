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
    public partial class BDCategory: IBDObject
    {
        //public const string AWS_DOMAIN = @"bd_1_categories";

        public const string AWS_PROD_DOMAIN = @"bd_1_categories";
        public const string AWS_DEV_DOMAIN = @"bd_dev_1_categories";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDCategories";
        public const string ENTITYNAME_FRIENDLY = @"Category";
        public const string KEY_NAME = @"BDCategory";
        public const string PROPERTYNAME_NAME = @"Name";

        public const int ENTITY_SCHEMAVERSION = 1;

        private const string UUID = @"ct_uuid";
        private const string SCHEMAVERSION = @"ct_schemaVersion";
        private const string CREATEDBY = @"ct_createdBy";
        private const string CREATEDDATE = @"ct_createdDate";
        private const string MODIFIEDBY = @"ct_modifiedBy";
        private const string MODIFIEDDATE = @"ct_modifiedDate";
        private const string PARENTID = @"ct_parentId";
        private const string PARENTKEYNAME = @"ct_parentKeyName";
        private const string NAME = @"ct_name";
        private const string DEPRECATED = @"ct_deprecated";
        private const string DISPLAYORDER = @"ct_displayOrder";

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
            category.schemaVersion = ENTITY_SCHEMAVERSION;
            category.displayOrder = -1;
            category.parentId = Guid.Empty;
            category.name = string.Empty;
            category.parentKeyName = string.Empty;

            pContext.AddObject(ENTITYNAME, category);
            
            return category;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pCategory"></param>
        public static void Save(Entities pContext, BDCategory pCategory)
        {
            if (pCategory.EntityState != System.Data.EntityState.Unchanged)
            {
                if (pCategory.schemaVersion != ENTITY_SCHEMAVERSION)
                    pCategory.schemaVersion = ENTITY_SCHEMAVERSION;

                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDCategory pEntity)
        {
            // delete linked note associations
            List<BDLinkedNoteAssociation> notes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in notes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }

            // delete children
            List<BDDisease> diseases = BDDisease.GetDiseasesForParentId(pContext, pEntity.uuid);
            foreach (BDDisease d in diseases)
            {
                BDDisease.Delete(pContext, d);
            }

            // create BDDeletion record for the object to be deleted
            BDDeletion.CreateDeletion(pContext, KEY_NAME, pEntity.uuid);
            // delete record from local data store
            pContext.DeleteObject(pEntity);
            pContext.SaveChanges();
            pEntity = null; 
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDCategory entity = BDCategory.GetCategoryWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDCategory.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Gets all sections in the model with the specified section ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>List of Categories</returns>
        public static List<BDCategory> GetCategoriesForParentId(Entities pContext, Guid pParentId)
        {
            List<BDCategory> entryList = new List<BDCategory>();
            IQueryable<BDCategory> entries = (from entry in pContext.BDCategories
                                                 where entry.parentId == pParentId
                                                 orderby entry.displayOrder
                                                 select entry);
            if (entries.Count<BDCategory>() > 0)
                entryList = entries.ToList<BDCategory>();
            return entryList;
        }


        /// <summary>
        /// Get Category with specified ID
        /// </summary>
        /// <param name="pParentId"></param>
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
            if(!Common.Settings.IsSyncLoad)
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
        public static List<IBDObject> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<IBDObject> entryList = new List<IBDObject>();
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
                entryList = new List<IBDObject>(categories.ToList<BDCategory>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            syncInfo.PushList = BDCategory.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDCategory)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
            }
            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            return syncInfo;
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
            bool deprecated = Boolean.Parse(pAttributeDictionary[DEPRECATED]);
            BDCategory entry = BDCategory.GetCategoryWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDCategory.CreateBDCategory(uuid, false);
                pDataContext.AddObject(ENTITYNAME, entry);
            }
            entry.deprecated = deprecated;
            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];
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

            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.NAME).WithValue((null == name)? string.Empty : name).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.PARENTKEYNAME).WithValue((null == parentKeyName)? string.Empty : parentKeyName).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDCategory.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            return putAttributeRequest;
        }

        #endregion

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return this.name; }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("{0}: {1}", ENTITYNAME_FRIENDLY, this.name); }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
