using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDSection
    /// </summary>
    public partial class BDSection: IBDObject
    {
        //public const string AWS_DOMAIN = @"bd_1_sections";

        public const string AWS_PROD_DOMAIN = @"bd_2_sections";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_sections";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDSections";
        public const string ENTITYNAME_FRIENDLY = @"Section";
        public const string KEY_NAME = @"BDSection";
        public const string PROPERTYNAME_NAME = @"Name";

        public const int ENTITY_SCHEMAVERSION = 1;

        private const string UUID = @"sn_uuid";
        private const string SCHEMAVERSION = @"sn_schemaVersion";
        private const string CREATEDBY = @"sn_createdBy";
        private const string CREATEDDATE = @"sn_createdDate";
        private const string MODIFIEDBY = @"sn_modifiedBy";
        private const string MODIFIEDDATE = @"sn_modifiedDate";
        private const string NAME = @"sn_name";
        private const string DEPRECATED = @"sn_deprecated";
        private const string DISPLAYORDER = @"sn_displayOrder";
        private const string PARENTID = @"sn_parentId";
        private const string PARENTKEYNAME = @"sn_parentKeyName";

        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDSection CreateSection(Entities pContext)
        {

            BDSection section = CreateBDSection(Guid.NewGuid(), false);
            section.createdBy = Guid.Empty;
            section.createdDate = DateTime.Now;
            section.schemaVersion = ENTITY_SCHEMAVERSION;
            section.displayOrder = -1;
            section.name = string.Empty;

            pContext.AddObject(ENTITYNAME, section);

            return section;

        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pSection"></param>
        public static void Save(Entities pContext, BDSection pSection)
        {
            if (pSection.EntityState != EntityState.Unchanged)
            {
                if (pSection.schemaVersion != ENTITY_SCHEMAVERSION)
                    pSection.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"Section Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDSection pEntity)
        {
            // delete linked notes
            List<BDLinkedNoteAssociation> linkedNotes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in linkedNotes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }

            // delete children
            List<BDCategory> categories = BDCategory.GetCategoriesForParentId(pContext, pEntity.uuid);
            foreach (BDCategory c in categories)
            {
                BDCategory.Delete(pContext, c);
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
            BDSection entity = BDSection.GetSectionWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDSection.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Get Section with the specified ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>BDSection object.</returns>
        public static BDSection GetSectionWithId(Entities pContext, Guid pSectionId)
        {
            BDSection section = null;

            if (null != pSectionId)
            {
                IQueryable<BDSection> entries = (from bdSections in pContext.BDSections
                                                  where bdSections.uuid == pSectionId
                                                  select bdSections);
                if (entries.Count<BDSection>() > 0)
                    section = entries.AsQueryable().First<BDSection>();
            }
            return section;
        }

        public static List<BDSection> GetSectionsForParentId(Entities pContext, Guid pParentId)
        {
            List<BDSection> entryList = new List<BDSection>();

            if (null != pParentId)
            {
                IQueryable<BDSection> entries = (from entry in pContext.BDSections
                                                 where entry.parentId == pParentId
                                                 select entry);
                if (entries.Count<BDSection>() > 0)
                    entryList = entries.ToList<BDSection>();
            }
            return entryList;
        }

        public static List<BDSection> GetAll(Entities pContext)
        {
            List<BDSection> entryList = new List<BDSection>();
            IQueryable<BDSection> entries = (from entry in pContext.BDSections
                                                 orderby entry.displayOrder
                                                 select entry);
            if (entries.Count() > 0)
                entryList = entries.ToList<BDSection>();
            return entryList;
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
            IQueryable<BDSection> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDSections
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDSections
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDSection>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDSection.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDSection)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
            }
            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDSection from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDSection entry = BDSection.GetSectionWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDSection.CreateBDSection(uuid, deprecated);
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
            entry.name = pAttributeDictionary[NAME];
            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));
            if (schemaVersion > 0)
            {
                attributeList.Add(new ReplaceableAttribute().WithName(BDSection.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
                attributeList.Add(new ReplaceableAttribute().WithName(BDSection.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));
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

