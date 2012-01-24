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
    /// Extension of generated BDSubcategory
    /// </summary>
    public partial class BDSubcategory: IBDObject
    {
        //public const string AWS_DOMAIN = @"bd_1_subcategories";

        public const string AWS_PROD_DOMAIN = @"bd_2_subcategories";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_subcategories";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDSubcategories";
        public const string ENTITYNAME_FRIENDLY = @"Subcategory";
        public const string KEY_NAME = @"BDSubCategory";

        public const int ENTITY_SCHEMAVERSION = 1;

        private const string UUID = @"sc_uuid";
        private const string SCHEMAVERSION = @"sc_schemaVersion";
        private const string CREATEDBY = @"sc_createdBy";
        private const string CREATEDDATE = @"sc_createdDate";
        private const string MODIFIEDBY = @"sc_modfiedBy";
        private const string MODIFIEDDATE = @"sc_modifiedDate";
        private const string DISPLAYORDER = @"sc_displayOrder";
        private const string PARENTID = @"sc_parentId";
        private const string PARENTKEYNAME = @"sc_parentKeyName";
        private const string NAME = @"sc_name";
        private const string DEPRECATED = @"sc_deprecated";
        
        /// <summary>
        /// Extended Create method that sets created date and schema version.
        /// </summary>
        /// <returns>New BDSubcategory object</returns>
        public static BDSubcategory CreateSubcategory(Entities pContext)
        {
            return CreateSubcategory(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets created date and schema version.
        /// </summary>
        /// <returns>New BDSubcategory object</returns>
        public static BDSubcategory CreateSubcategory(Entities pContext, Guid pUuid)
        {
            BDSubcategory subcategory = CreateBDSubcategory(pUuid, false);
            subcategory.createdBy = Guid.Empty;
            subcategory.createdDate = DateTime.Now;
            subcategory.schemaVersion = ENTITY_SCHEMAVERSION;
            subcategory.displayOrder = -1;
            subcategory.name = string.Empty;
            subcategory.parentId = Guid.Empty;

            pContext.AddObject(ENTITYNAME, subcategory);

            return subcategory;
        }

        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pSubcategory"></param>
        public static void Save(Entities pContext, BDSubcategory pSubcategory)
        {
            if (pSubcategory.EntityState != EntityState.Unchanged)
            {
                if (pSubcategory.schemaVersion != ENTITY_SCHEMAVERSION)
                    pSubcategory.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"SubCategory Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDSubcategory pEntity)
        {
            // delete linked notes
            List<BDLinkedNoteAssociation> linkedNotes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in linkedNotes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }

            // create BDDeletion record for the object to be deleted
            BDDeletion.CreateDeletion(pContext, KEY_NAME, pEntity.uuid);
            // delete record from local data store
            pContext.DeleteObject(pEntity);
            pContext.SaveChanges();
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        /// <param name="pCreateDeletion"create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDSubcategory entity = BDSubcategory.GetSubcategoryWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDSubcategory.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Gets all subcategories in the model with the specified category ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>List of Subcategories</returns>
        public static List<BDSubcategory> GetSubcategoriesForParentId(Entities pContext, Guid pParentId)
        {
            List<BDSubcategory> subcategoryList = new List<BDSubcategory>();

                IQueryable<BDSubcategory> subcategories = (from entry in pContext.BDSubcategories
                                                           where entry.parentId == pParentId
                                                           orderby entry.displayOrder
                                                           select entry);
                foreach (BDSubcategory subcat in subcategories)
                {
                    subcategoryList.Add(subcat);
                }
            return subcategoryList;
        }


        /// <summary>
        /// Get Subcategory with the specified ID
        /// </summary>
        /// <param name="pSubcategoryId"></param>
        /// <returns>Subcategory object.</returns>
        public static BDSubcategory GetSubcategoryWithId(Entities pContext, Guid pSubcategoryId)
        {
            BDSubcategory subcategory = null;
            if (null != pSubcategoryId)
            {
                IQueryable<BDSubcategory> entries = (from bdSubcategories in pContext.BDSubcategories
                                                           where bdSubcategories.uuid == pSubcategoryId
                                                           select bdSubcategories);
                if (entries.Count<BDSubcategory>() > 0)
                    subcategory = entries.AsQueryable().First<BDSubcategory>();
            }
            return subcategory;
        }

        protected override void OnPropertyChanged(string property)
        {
            if (!Common.Settings.IsSyncLoad)
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
            IQueryable<BDSubcategory> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDSubcategories
                            select entry);
            }
            else
            {
                entries = (from entry in pContext.BDSubcategories
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDSubcategory>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDSubcategory.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDSubcategory)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
            }
            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDSubcategory from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDSubcategory entry = BDSubcategory.GetSubcategoryWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDSubcategory.CreateBDSubcategory(uuid, deprecated);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];
            entry.name = pAttributeDictionary[NAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));
            if (schemaVersion > 0)
            {
                attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
                attributeList.Add(new ReplaceableAttribute().WithName(BDSubcategory.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));
            }

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
