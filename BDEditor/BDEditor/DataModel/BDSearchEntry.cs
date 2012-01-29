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
    /// Extension of generated BDSearchEntry
    /// </summary>
    public partial class BDSearchEntry: IBDObject
    {
        //public const string AWS_DOMAIN = @"bd_1_searchEntries";

        public const string AWS_PROD_DOMAIN = @"bd_2_searchEntries";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_searchEntries";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        //public const string AWS_BUCKET = @"bdDataStore";
        //public const string AWS_S3_PREFIX = @"bd~";
        //public const string AWS_S3_FILEEXTENSION = @".txt";

        public const string ENTITYNAME = @"BDSearchEntries";
        public const string ENTITYNAME_FRIENDLY = @"Search Entry";
        public const string KEY_NAME = @"BDSearchEntry";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"se_uuid";
        private const string SCHEMAVERSION = @"se_schemaVersion";
        private const string CREATEDBY = @"se_createdBy";
        private const string CREATEDDATE = @"se_createdDate";
        //private const string MODIFIEDBY = @"se_modifieddBy";
        //private const string MODIFIEDDATE = @"se_modifiedDate";
        private const string NAME = @"se_name";

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDLinkedNote</returns>
        public static BDSearchEntry CreateSearchEntry(Entities pContext)
        {
            return CreateSearchEntry(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDLinkedNote</returns>
        public static BDSearchEntry CreateSearchEntry(Entities pContext, Guid pUuid)
        {
            BDSearchEntry entry = CreateBDSearchEntry(pUuid);
            entry.createdBy = Guid.Empty;
            entry.createdDate = DateTime.Now;
            entry.schemaVersion = ENTITY_SCHEMAVERSION;
            entry.name = string.Empty;

            pContext.AddObject(ENTITYNAME, entry);
            return entry;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pSearchEntry"></param>
        public static void Save(Entities pContext, BDSearchEntry pSearchEntry)
        {
            if (pSearchEntry.EntityState != EntityState.Unchanged)
            {
                if (pSearchEntry.schemaVersion != ENTITY_SCHEMAVERSION)
                    pSearchEntry.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"SearchEntry Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDSearchEntry pEntity)
        {
            // delet the note associations
            DeleteSearchEntryAssociations(pContext, pEntity.uuid, true);
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
        /// <param name="pCreateDeletion">create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDSearchEntry entity = BDSearchEntry.GetSearchEntryWithId(pContext, pUuid);
            if (null != entity)
            {
                DeleteSearchEntryAssociations(pContext, pUuid, pCreateDeletion);

                if (pCreateDeletion)
                {
                    BDSearchEntry.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                    pContext.SaveChanges();
                }
            }
        }

        public static void DeleteAll()
        {
            BDEditor.DataModel.Entities dataContext = new BDEditor.DataModel.Entities();
            dataContext.ExecuteStoreCommand("DELETE FROM BDSearchEntries");
        }
        
        private static void DeleteSearchEntryAssociations(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            List<BDSearchEntryAssociation> children = BDSearchEntryAssociation.GetSearchEntryAssociationsForSearchEntryId(pContext, pUuid);
            foreach (BDSearchEntryAssociation t in children)
            {
                BDSearchEntryAssociation.Delete(pContext, t, pCreateDeletion);
            }
        }

        /// <summary>
        /// Return the LinkedNote for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static BDSearchEntry GetSearchEntryWithId(Entities pContext, Guid? pEntityId)
        {
            BDSearchEntry result = null;

            if (null != pEntityId)
            {
                IQueryable<BDSearchEntry> entries = (from searchEntries in pContext.BDSearchEntries
                                                     where searchEntries.uuid == pEntityId
                                                     select searchEntries);

                if (entries.Count<BDSearchEntry>() > 0)
                    result = entries.AsQueryable().First<BDSearchEntry>();
            }

            return result;
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
            IQueryable<BDSearchEntry> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDSearchEntries
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDSearchEntries
                           where entry.createdDate > pUpdateDateTime.Value
                           select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDSearchEntry>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDSearchEntry.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDSearchEntry)syncInfo.PushList[idx]).createdDate = pCurrentSyncDate;
            }
            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
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
            BDSearchEntry entry = BDSearchEntry.GetSearchEntryWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDSearchEntry.CreateBDSearchEntry(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
           // entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            //entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.name = pAttributeDictionary[NAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntry.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntry.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntry.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntry.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            //attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntry.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            //attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntry.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntry.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

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

        public override string ToString()
        {
            return this.uuid.ToString();
        }


        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }


        public Constants.BDObjectType NodeType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
