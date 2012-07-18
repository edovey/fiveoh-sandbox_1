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
    /// Extension of generated BDLinkedNote
    /// </summary>
    public partial class BDLinkedNote: IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_linkedNotes";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_linkedNotes";

        public const string AWS_PROD_BUCKET = @"bdProdStore";
        public const string AWS_DEV_BUCKET = @"bdDevStore";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
        public const string AWS_BUCKET = AWS_DEV_BUCKET;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
        public const string AWS_BUCKET = AWS_PROD_BUCKET;
#endif

        public const string AWS_S3_PREFIX = @"bd~";
        public const string AWS_S3_FILEEXTENSION = @".txt";

        public const string ENTITYNAME = @"BDLinkedNotes";
        public const string ENTITYNAME_FRIENDLY = @"Linked Note";
        public const string KEY_NAME = @"BDLinkedNote";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"ln_uuid";
        private const string SCHEMAVERSION = @"ln_schemaVersion";
        private const string CREATEDBY = @"ln_createdBy";
        private const string CREATEDDATE = @"ln_createdDate";
        private const string MODIFIEDBY = @"ln_modifieddBy";
        private const string MODIFIEDDATE = @"ln_modifiedDate";
        private const string LINKEDNOTEASSOCIATIONID = @"ln_linkedNoteAssociationId";
        private const string PREVIEWTEXT = @"ln_previewText";
        private const string SCOPEID = @"ln_scopeId";
        private const string DEPRECATED = @"ln_deprecated";
        private const string SINGLEUSE = @"ln_singleUse";
        private const string STORAGEKEY = @"ln_storageKey";
        private const string DOCUMENTTEXT = @"ln_documentText";

        public Guid? tempProductionUuid { get; set; }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDLinkedNote</returns>
        public static BDLinkedNote CreateBDLinkedNote(Entities pContext)
        {
            return CreateBDLinkedNote(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDLinkedNote</returns>
        public static BDLinkedNote CreateBDLinkedNote(Entities pContext, Guid pUuid)
        {
            BDLinkedNote linkedNote = CreateBDLinkedNote(pUuid, false);
            linkedNote.createdBy = Guid.Empty;
            linkedNote.createdDate = DateTime.Now;
            linkedNote.schemaVersion = ENTITY_SCHEMAVERSION;
            linkedNote.documentText = string.Empty;
            //linkedNote.storageKey = string.Format("{0}{1}{2}", AWS_S3_PREFIX, linkedNote.uuid.ToString().ToUpper(), AWS_S3_FILEEXTENSION);
            linkedNote.storageKey = GenerateStorageKey(linkedNote);
            linkedNote.singleUse = false;
            linkedNote.previewText = string.Empty;

            pContext.AddObject(ENTITYNAME, linkedNote);
            return linkedNote;
        }

        /// <summary>
        /// Extended Save method that resets the schema version
        /// </summary>
        /// <param name="pLinkedNote"></param>
        public static void Save(Entities pContext, BDLinkedNote pLinkedNote)
        {
            if (pLinkedNote.EntityState != EntityState.Unchanged)
            {
                if (pLinkedNote.schemaVersion != ENTITY_SCHEMAVERSION)
                    pLinkedNote.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"LinkedNote Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDLinkedNote pEntity, bool pCreateDeletion)
        {
            BDLinkedNoteAssociation.DeleteForNote(pContext, pEntity, pCreateDeletion);
            BDMetadata.DeleteForItemId(pContext, pEntity.Uuid, pCreateDeletion);
            if(pCreateDeletion)
                BDDeletion.CreateBDDeletion(pContext, KEY_NAME, pEntity);
            // delete record from local data store
            pContext.DeleteObject(pEntity);
            pContext.SaveChanges();
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        /// <param name="pCreateDeletion">create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDLinkedNote entity = BDLinkedNote.GetLinkedNoteWithId(pContext, pUuid);
            Delete(pContext, entity, pCreateDeletion);
        }

        /// <summary>
        /// Delete from the local datastore without creating a deletion record nor deleting any children. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid"></param>
        public static void DeleteLocal(Entities pContext, Guid? pUuid)
        {
            if (null != pUuid)
            {
                BDLinkedNote entry = BDLinkedNote.GetLinkedNoteWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Return the LinkedNote for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static BDLinkedNote GetLinkedNoteWithId(Entities pContext, Guid? pLinkedNoteId)
        {
            BDLinkedNote result = null;

            if (null != pLinkedNoteId)
            {
                IQueryable<BDLinkedNote> entries = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                        where bdLinkedNotes.uuid == pLinkedNoteId
                                                        select bdLinkedNotes);

                if (entries.Count<BDLinkedNote>() > 0)
                    result = entries.AsQueryable().First<BDLinkedNote>();
            }

            return result;
        }

        ///// <summary>
        ///// Get all linked allNotes with the specified association ID
        ///// </summary>
        ///// <param name="pContext"></param>
        ///// <param name="pParentId"></param>
        ///// <returns></returns>
        //public static List<BDLinkedNote> GetLinkedNotesForLinkedNoteAssociationId(Entities pContext, Guid pLinkedNoteAssociationId)
        //{
        //    IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
        //                                            where bdLinkedNotes.linkedNoteAssociationId == pLinkedNoteAssociationId
        //                                            select bdLinkedNotes);

        //    List<BDLinkedNote> linkedNoteList = linkedNotes.ToList<BDLinkedNote>();

        //    return linkedNoteList;
        //}

        public static List<BDLinkedNote> GetLinkedNotesForScopeId(Entities pContext, Guid? pScopeId)
        {
            IQueryable<BDLinkedNote> linkedNotes = (from bdLinkedNotes in pContext.BDLinkedNotes
                                                    where bdLinkedNotes.scopeId == pScopeId && bdLinkedNotes.singleUse == false
                                                    orderby bdLinkedNotes.previewText
                                                    select bdLinkedNotes);

            List<BDLinkedNote> resultList = linkedNotes.ToList<BDLinkedNote>();
            return resultList;
        }

        public static List<BDLinkedNote> RetrieveLinkedNotesWithText(Entities pContext, string pText)
        {
            List<BDLinkedNote> returnList = new List<BDLinkedNote>();
            if (null != pText && pText.Length > 0)
            {
                IQueryable<BDLinkedNote> entries = (from entry in pContext.BDLinkedNotes
                                                 where entry.documentText.Contains(pText)
                                                 select entry);
                returnList = entries.ToList<BDLinkedNote>();
            }
            return returnList;
        }

        public static List<BDLinkedNote> RetrieveLinkedNotesWithLinkedNoteParentType(Entities pContext)
        {
            List<BDLinkedNote> returnList = new List<BDLinkedNote>();
            IQueryable<BDLinkedNote> entries = (from assn in pContext.BDLinkedNoteAssociations
                                            from entry in pContext.BDLinkedNotes
                                            where assn.parentType == 11 && assn.linkedNoteId == entry.uuid
                                            select entry);
            returnList = entries.ToList<BDLinkedNote>();
            return returnList;
        }

        protected override void OnPropertyChanged(string property)
        {
            if (!BDCommon.Settings.IsSyncLoad)
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
            IQueryable<BDLinkedNote> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDLinkedNotes
                            select entry);
            }
            else
            {
                entries = (from entry in pContext.BDLinkedNotes
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }
            
            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDLinkedNote>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDLinkedNote.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if ((null != pCurrentSyncDate) && (!BDCommon.Settings.RepositoryOverwriteEnabled))
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDLinkedNote)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDLinkedNote from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDLinkedNote entry = BDLinkedNote.GetLinkedNoteWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDLinkedNote.CreateBDLinkedNote(uuid, deprecated);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            //entry.linkedNoteAssociationId = Guid.Parse(pAttributeDictionary[LINKEDNOTEASSOCIATIONID]);
            entry.previewText = pAttributeDictionary[PREVIEWTEXT];
            entry.scopeId = Guid.Parse(pAttributeDictionary[SCOPEID]);
            entry.singleUse = bool.Parse(pAttributeDictionary[SINGLEUSE]);
            entry.storageKey = pAttributeDictionary[STORAGEKEY];
            //entry.documentText is loaded from S3 storage

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        /// <summary>
        /// Create a new LinkedNote (creating a new uuid in the process) from an existing BDLinkedNote from attributes in a dictionary. Saves the entry.
        /// The production uuid will be stored (by not persisted) in tempProductionUuid
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static BDLinkedNote CreateFromProdWithAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);

            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);

            BDLinkedNote entry = BDLinkedNote.CreateBDLinkedNote(pDataContext);
            entry.tempProductionUuid = uuid;
            entry.deprecated = deprecated;

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            //entry.linkedNoteAssociationId = Guid.Parse(pAttributeDictionary[LINKEDNOTEASSOCIATIONID]);
            entry.previewText = pAttributeDictionary[PREVIEWTEXT];
            entry.scopeId = Guid.Parse(pAttributeDictionary[SCOPEID]);
            entry.singleUse = bool.Parse(pAttributeDictionary[SINGLEUSE]);
            entry.storageKey = pAttributeDictionary[STORAGEKEY]; // This is the storage key from the imported data: It will need to be rebuilt after the document text has been loaded.
            //entry.documentText is loaded from S3 storage

            pDataContext.SaveChanges();

            return entry;
        }

        public static string GenerateStorageKey(BDLinkedNote pNote)
        {
            string result = GenerateStorageKey(pNote.Uuid);
            return result;
        }

        public static string GenerateStorageKey(Guid pUuid)
        {
            string result = string.Format("{0}{1}{2}", AWS_S3_PREFIX, pUuid.ToString().ToUpper(), AWS_S3_FILEEXTENSION);
            return result;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            //attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.LINKEDNOTEASSOCIATIONID).WithValue((null == linkedNoteAssociationId) ? Guid.Empty.ToString() : linkedNoteAssociationId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.SCOPEID).WithValue((null == scopeId) ? Guid.Empty.ToString() : scopeId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.PREVIEWTEXT).WithValue((null == previewText) ? string.Empty : previewText).WithReplace(true));
            //attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.DOCUMENTTEXT).WithValue(documentText).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.STORAGEKEY).WithValue((null == storageKey) ? string.Empty : storageKey).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNote.SINGLEUSE).WithValue(singleUse.ToString()).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return this.previewText; }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("{0}: {1}", ENTITYNAME_FRIENDLY, this.previewText); }
        }

        public override string ToString()
        {
            return this.uuid.ToString();
        }
    }
}
