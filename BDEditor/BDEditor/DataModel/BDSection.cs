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
    public partial class BDSection
    {
        public const string AWS_DOMAIN = @"bd_1_sections";
        public const string ENTITYNAME = @"BDSections";
        public const string ENTITYNAME_FRIENDLY = @"Section";
        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"sn_uuid";
        private const string SCHEMAVERSION = @"sn_schemaVersion";
        private const string CREATEDBY = @"sn_createdBy";
        private const string CREATEDDATE = @"sn_createdDate";
        private const string MODIFIEDBY = @"sn_modifiedBy";
        private const string MODIFIEDDATE = @"sn_modifiedDate";
        private const string NAME = @"sn_name";
        private const string DEPRECATED = @"sn_deprecated";
        private const string DISPLAYORDER = @"sn_displayOrder";
        private const string CHAPTERID = @"sn_chapterId";

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

            pContext.AddObject("BDSections", section);

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
            // delete linked notes for section
            List<BDLinkedNoteAssociation> notes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsFromParentIdAndProperty(pContext, pEntity.uuid, ENTITYNAME_FRIENDLY);
            foreach (BDLinkedNoteAssociation a in notes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }

            // delete children
            List<BDCategory> categories = BDCategory.GetCategoriesForSectionId(pContext, pEntity.uuid);
            foreach (BDCategory c in categories)
            {
                BDCategory.Delete(pContext, c);
            }

            // create BDDeletion record for the object to be deleted
            BDDeletion.CreateDeletion(pContext, ENTITYNAME_FRIENDLY, pEntity.uuid);
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
        /// <param name="pSectionId"></param>
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

        public static List<BDSection> GetSectionsForChapterId(Entities pContext, Guid pChapterId)
        {
            List<BDSection> entryList = new List<BDSection>();

            if (null != pChapterId)
            {
                IQueryable<BDSection> entries = (from entry in pContext.BDSections
                                                 where entry.chapterId == pChapterId
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
        public static List<BDSection> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDSection> entryList = new List<BDSection>();
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
                entryList = entries.ToList<BDSection>();
            return entryList;
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
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
                pDataContext.AddObject("BDSections", entry);
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
            entry.chapterId = Guid.Parse(pAttributeDictionary[CHAPTERID]);

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
            attributeList.Add(new ReplaceableAttribute().WithName(BDSection.CHAPTERID).WithValue((null == chapterId) ? Guid.Empty.ToString() : chapterId.ToString().ToUpper()).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

    }
}
