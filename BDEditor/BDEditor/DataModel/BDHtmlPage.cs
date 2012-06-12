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
    /// Extension of generated BDHtmlPage
    /// </summary>
    public partial class BDHtmlPage : IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_htmlPages";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_htmlPages";

        public const string AWS_PROD_BUCKET = @"bdProdStore";
        public const string AWS_DEV_BUCKET = @"bdDevStore";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
        public const string AWS_BUCKET = AWS_DEV_BUCKET;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
        public const string AWS_BUCKET = AWS_PROD_BUCKET;
#endif

        public const string AWS_S3_PREFIX = @"bdhp~";
        public const string AWS_S3_FILEEXTENSION = @".html";

        public const string ENTITYNAME = @"BDHtmlPages";
        public const string ENTITYNAME_FRIENDLY = @"HTML Pages";
        public const string KEY_NAME = @"BDHtmlPage";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"ht_uuid";
        private const string SCHEMAVERSION = @"ht_schemaVersion";
        private const string CREATEDBY = @"ht_createdBy";
        private const string CREATEDDATE = @"ht_createdDate";
        private const string DISPLAYPARENTID = @"ht_displayParentId";
        private const string DISPLAYPARENTTYPE = @"ht_displayParentType";
        private const string STORAGEKEY = @"ht_storageKey";
        private const string DOCUMENTTEXT = @"ht_documentText";

        public Guid? tempProductionUuid { get; set; }

        public static BDHtmlPage CreateBDHtmlPage(Entities pContext)
        {
            return CreateBDHtmlPage(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid"></param>
        /// <returns></returns>
        public static BDHtmlPage CreateBDHtmlPage(Entities pContext, Guid pUuid)
        {
            BDHtmlPage page = CreateBDHtmlPage(pUuid);
            page.createdBy = Guid.Empty;
            page.createdDate = DateTime.Now;
            page.schemaVersion = ENTITY_SCHEMAVERSION;
            page.documentText = string.Empty;
            page.storageKey = GenerateStorageKey(page);

            pContext.AddObject(ENTITYNAME, page);
            Save(pContext, page);
            return page;
        }

        public static void Save(Entities pContext, BDHtmlPage pPage)
        {
            if (pPage.EntityState != EntityState.Unchanged)
            {
                if (pPage.schemaVersion != ENTITY_SCHEMAVERSION)
                    pPage.schemaVersion = ENTITY_SCHEMAVERSION;

                pContext.SaveChanges();
            }
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
                BDHtmlPage entry = BDHtmlPage.RetrieveWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Delete all records from local store
        /// </summary>
        public static void DeleteAll()
        {
            BDEditor.DataModel.Entities dataContext = new BDEditor.DataModel.Entities();
            // check if table exists:
            
            int result = dataContext.ExecuteStoreQuery<int> (@"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME ='BDHtmlPages'").SingleOrDefault();
            
            if(result == 1) 
                dataContext.ExecuteStoreCommand("DELETE FROM BDHtmlPages");
        }

        /// <summary>
        /// Return the HtmlPage for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pEntryId"></param>
        /// <returns></returns>
        public static BDHtmlPage RetrieveWithId(Entities pContext, Guid? pEntryId)
        {
            BDHtmlPage result = null;

            if (null != pEntryId)
            {
                IQueryable<BDHtmlPage> entries = (from entry in pContext.BDHtmlPages
                                                  where entry.uuid == pEntryId
                                                    select entry);

                if (entries.Count<BDHtmlPage>() > 0)
                    result = entries.AsQueryable().First<BDHtmlPage>();
            }

            return result;
        }

        public static List<BDHtmlPage> RetrieveHtmlPageForDisplayParentId(Entities pContext, Guid? pDisplayParentId)
        {
            IQueryable<BDHtmlPage> entries = (from entry in pContext.BDHtmlPages
                                                    where entry.displayParentId == pDisplayParentId
                                                    select entry);

            List<BDHtmlPage> resultList = entries.ToList<BDHtmlPage>();
            return resultList;
        }

        public static List<BDHtmlPage> RetrieveAll(Entities pContext)
        {
            List<BDHtmlPage> entryList = new List<BDHtmlPage>();
            IQueryable<BDHtmlPage> entries;

            entries = (from entry in pContext.BDHtmlPages
                   select entry);
            if (entries.Count() > 0)
                return entries.ToList<BDHtmlPage>();

            return entryList;
        }

        public static List<Guid> RetrieveAllIDs(Entities pContext)
        {
            IQueryable<Guid> pages = (from entry in pContext.BDHtmlPages
                                            select entry.uuid);
            return pages.ToList<Guid>();
        }

        public static Guid RetrievePageIdForAnchorId(Entities pContext, string pAnchorIdAsText)
        {
            IQueryable<Guid> query = from lna in pContext.BDLinkedNoteAssociations
                                     join p in pContext.BDHtmlPages
                                     on lna.linkedNoteId equals p.displayParentId 
                                     where lna.parentKeyPropertyName == pAnchorIdAsText
                                     select p.uuid;
            return query.ToList<Guid>()[0];
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
            IQueryable<BDHtmlPage> entries;

            // get ALL entries - this is a full refresh.
            entries = (from entry in pContext.BDHtmlPages
                       select entry);

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDHtmlPage>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDHtmlPage.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if ((null != pCurrentSyncDate) && (!BDCommon.Settings.RepositoryOverwriteEnabled))
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDHtmlPage)syncInfo.PushList[idx]).createdDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDHtmlPage from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDHtmlPage entry = BDHtmlPage.RetrieveWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDHtmlPage.CreateBDHtmlPage(pDataContext, uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.displayParentId = Guid.Parse(pAttributeDictionary[DISPLAYPARENTID]);
            short displayParentType = short.Parse(pAttributeDictionary[DISPLAYPARENTTYPE]);
            entry.displayParentType = displayParentType;
            entry.storageKey = pAttributeDictionary[STORAGEKEY];
            //entry.documentText is loaded from S3 storage

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        /// <summary>
        /// Create a new HtmlPage (creating a new uuid in the process) from an existing BDHtmlPage from attributes in a dictionary. Saves the entry.
        /// The production uuid will be stored (by not persisted) in tempProductionUuid
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static BDHtmlPage CreateFromProdWithAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);

            BDHtmlPage entry = BDHtmlPage.CreateBDHtmlPage(pDataContext);
            entry.tempProductionUuid = uuid;

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.displayParentId = Guid.Parse(pAttributeDictionary[DISPLAYPARENTID]);
            short displayParentType = short.Parse(pAttributeDictionary[DISPLAYPARENTTYPE]);
            entry.displayParentType = displayParentType;
            entry.storageKey = pAttributeDictionary[STORAGEKEY]; // This is the storage key from the imported data: It will need to be rebuilt after the document text has been loaded.
            //entry.documentText is loaded from S3 storage

            pDataContext.SaveChanges();

            return entry;
        }

        public static string GenerateStorageKey(BDHtmlPage pNote)
        {
            string result = GenerateStorageKey(pNote.Uuid);
            return result;
        }

        public static string GenerateStorageKey(Guid pUuid)
        {
            string result = string.Format("{0}{1}{2}", AWS_S3_PREFIX, pUuid.ToString().ToUpper(), AWS_S3_FILEEXTENSION);
            return result;
        }

         #endregion

        #region IBDObject implementation
        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.DISPLAYPARENTID).WithValue((null == displayParentId) ? Guid.Empty.ToString() : displayParentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.STORAGEKEY).WithValue((null == storageKey) ? string.Empty : storageKey).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDHtmlPage.DISPLAYPARENTTYPE).WithValue(string.Format(@"{0}", displayParentType)).WithReplace(true));

            return putAttributeRequest;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
        public override string ToString()
        {
            return this.uuid.ToString();
        }
    }
}
