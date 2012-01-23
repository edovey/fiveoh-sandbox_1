﻿using System;
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
    /// Extension of generated BDChapter
    /// </summary>
    public partial class BDChapter: IBDObject
    {
        //public const string AWS_DOMAIN = @"bd_1_chapters";

        public const string AWS_PROD_DOMAIN = @"bd_2_chapters";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_chapters";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDChapters";
        public const string ENTITYNAME_FRIENDLY = @"Chapter";
        public const string KEY_NAME = @"BDChapter";
        public const string PROPERTYNAME_NAME = @"Name";

        public const int ENTITY_SCHEMAVERSION = 1;

        private const string UUID = @"ch_uuid";
        private const string SCHEMAVERSION = @"ch_schemaVersion";
        private const string CREATEDBY = @"ch_createdBy";
        private const string CREATEDDATE = @"ch_createdDate";
        private const string MODIFIEDBY = @"ch_modifiedBy";
        private const string MODIFIEDDATE = @"ch_modifiedDate";
        private const string NAME = @"ch_name";
        private const string DEPRECATED = @"ch_deprecated";
        private const string DISPLAYORDER = @"ch_displayOrder";

        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static BDChapter CreateChapter(Entities pContext)
        {
            BDChapter chapter = CreateBDChapter(Guid.NewGuid(), false);
            chapter.createdBy = Guid.Empty;
            chapter.createdDate = DateTime.Now;
            chapter.schemaVersion = ENTITY_SCHEMAVERSION;
            chapter.displayOrder = -1;
            chapter.name = string.Empty;

            pContext.AddObject(ENTITYNAME, chapter);

            return chapter;
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDChapter pEntity)
        {
            // delete linked note associations
            List<BDLinkedNoteAssociation> notes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in notes)
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
            BDChapter entity = BDChapter.GetChapterWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDChapter.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pChapter"></param>
        public static void Save(Entities pContext, BDChapter pChapter)
        {
            if (pChapter.EntityState != EntityState.Unchanged)
            {
                if (pChapter.schemaVersion != ENTITY_SCHEMAVERSION)
                    pChapter.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"Chapter Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get the Chapter with the specified ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <returns>BDChapter object</returns>
        public static BDChapter GetChapterWithId(Entities pContext, Guid pChapterId)
        {
            BDChapter chapter = null;

            if(null != pChapterId)
            {
                IQueryable<BDChapter> entryList = (from entries in pContext.BDChapters
                                                  where entries.uuid == pChapterId
                                                  select entries);
                if (entryList.Count<BDChapter>() > 0)
                    chapter = entryList.AsQueryable().First<BDChapter>();
            }
            return chapter;
        }

        public static List<BDChapter> GetAll(Entities pContext)
        {
            List<BDChapter> entryList = new List<BDChapter>();
            IQueryable<BDChapter> entries = (from entry in pContext.BDChapters
                                             orderby entry.displayOrder
                                             select entry);
            if (entries.Count() > 0)
                entryList = entries.ToList<BDChapter>();
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
            IQueryable<BDChapter> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDChapters
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDChapters
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }
            if (entries.Count() > 0)
                entryList = new List<IBDObject>( entries.ToList<BDChapter>());
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDChapter.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDChapter)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
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
            BDChapter entry = BDChapter.GetChapterWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDChapter.CreateBDChapter(uuid, deprecated);
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

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

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
            get { return string.Format("{0}: {1}",ENTITYNAME_FRIENDLY, this.name); }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
