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
        public const string AWS_PROD_DOMAIN = @"bd_2_searchEntries";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_searchEntries";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDSearchEntries";
        public const string ENTITYNAME_FRIENDLY = @"Search Entry";
        public const string KEY_NAME = @"BDSearchEntry";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"se_uuid";
        private const string SCHEMAVERSION = @"se_schemaVersion";
        private const string CREATEDBY = @"se_createdBy";
        private const string CREATEDDATE = @"se_createdDate";
        private const string NAME = @"se_name";
        private const string SHOW = @"se_show";


        /// <summary>
        /// Extended Create method that sets the created date, schema version and name.  Saves the instance.
        /// </summary>
        /// <returns>BDsearchEntry</returns>
        public static BDSearchEntry CreateBDSearchEntry(Entities pContext, string pName)
        {
            BDSearchEntry entry = CreateBDSearchEntry(pContext, Guid.NewGuid());
            entry.name = pName;

            Save(pContext, entry);
            return entry;
        }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDSearchEntry</returns>
        public static BDSearchEntry CreateBDSearchEntry(Entities pContext, Guid pUuid)
        {
            BDSearchEntry entry = CreateBDSearchEntry(pUuid);
            entry.createdBy = Guid.Empty;
            entry.createdDate = DateTime.Now;
            entry.schemaVersion = ENTITY_SCHEMAVERSION;
            entry.name = string.Empty;
            entry.show = false;

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

                // System.Diagnostics.Debug.WriteLine(@"SearchEntry Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDSearchEntry pEntity, bool pCreateDeletion)
        {
            if (null == pEntity) return;

            // delete the associations
            BDSearchEntryAssociation.DeleteForSearchEntryId(pContext, pEntity.Uuid, pCreateDeletion);
            // by design, no deletion record is kept
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
            BDSearchEntry entity = BDSearchEntry.RetrieveSearchEntryWithId(pContext, pUuid);
            BDSearchEntry.Delete(pContext, entity, pCreateDeletion);
        }

        public static void DeleteAll(Entities pContext)
        {
            pContext.ExecuteStoreCommand("DELETE FROM BDSearchEntries");
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
                BDSearchEntry entry = BDSearchEntry.RetrieveSearchEntryWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Retrieve all SearchEntry Nodes
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>List of IBDNode objects.</returns>
        public static List<BDSearchEntry> RetrieveAll(Entities pContext)
        {
            List<BDSearchEntry> entryList;
            IQueryable<BDSearchEntry> entries = (from bdNodes in pContext.BDSearchEntries
                                                    select bdNodes);
            entryList = new List<BDSearchEntry>(entries.ToList<BDSearchEntry>());
            return entryList;
        }

        /// <summary>
        /// Retrieve all SearchEntry nodes where show is true
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>List of IBDObjects</returns>
        public static List<IBDObject> RetrieveAllShowEntries(Entities pContext)
        {
            List<IBDObject> entryList;
            IQueryable<BDSearchEntry> entries = (from searchEntries in pContext.BDSearchEntries
                                                 where searchEntries.show == true
                                                 select searchEntries);
            entryList = new List<IBDObject>(entries.ToList<BDSearchEntry>());
            return entryList;
        }

        /// <summary>
        /// Return the SearchEntry for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns>BDSearchEntry</returns>
        public static BDSearchEntry RetrieveSearchEntryWithId(Entities pContext, Guid? pEntityId)
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

        /// <summary>
        /// Retrieve all search entries associated with the supplied parent ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentId"></param>
        /// <returns></returns>
        public static List<BDSearchEntry> RetrieveSearchEntriesForDisplayParent(Entities pContext, Guid pDisplayParentId)
        {
            List<BDSearchEntry> resultList;
            IQueryable<BDSearchEntry> entries = (from assns in pContext.BDSearchEntryAssociations
                                                 join searchEntries in pContext.BDSearchEntries
                                                 on assns.searchEntryId equals searchEntries.uuid
                                                 where assns.displayParentId == pDisplayParentId
                                                 select searchEntries );
            resultList = new List<BDSearchEntry>(entries.ToList<BDSearchEntry>());
            return resultList;
        }

        /// <summary>
        /// Get a string array of all the distinct Search Entry Name values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>List of strings</returns>
        public static List<string> RetrieveSearchEntryNames(Entities pContext)
        {
            var names = pContext.BDSearchEntries.Where(x => (!string.IsNullOrEmpty(x.name))).Select(pg => pg.name).Distinct();

            return names.ToList<string>();
        }

        public static BDSearchEntry RetrieveWithName(Entities pContext, string pName)
        {
            BDSearchEntry result = null;
            if (!string.IsNullOrEmpty(pName))
            {
                IQueryable<BDSearchEntry> entries = (from searchEntries in pContext.BDSearchEntries
                                                     where searchEntries.name == pName
                                                     select searchEntries);

                if (entries.Count<BDSearchEntry>() > 0)
                    result = entries.AsQueryable().First<BDSearchEntry>();

            }

            return result;
        }

        public static void ResetForRegeneration(Entities pContext)
        {
            List<BDSearchEntry> allEntries = new List<BDSearchEntry>();
            IQueryable<BDSearchEntry> entries = from entry in pContext.BDSearchEntries
                                                        select entry;
            allEntries = entries.ToList<BDSearchEntry>();

            foreach (BDSearchEntry entry in allEntries)
                entry.show = false;

            pContext.SaveChanges();
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

            // Get ALL entries - this is a full refresh.
            entries = (from entry in pContext.BDSearchEntries
                      select entry);
            
            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDSearchEntry>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDSearchEntry.RetrieveAllShowEntries(pDataContext);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
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
            BDSearchEntry entry = BDSearchEntry.RetrieveSearchEntryWithId(pDataContext, uuid);
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
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntry.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
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


        public BDConstants.BDNodeType NodeType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
