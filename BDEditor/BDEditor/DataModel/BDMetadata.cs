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
    public partial class BDMetadata : IBDObject
    {
        public const string AWS_DOMAIN = @"bd_1_metadatas";
        public const string AWS_BUCKET = @"bdDataStore";
        public const string AWS_S3_PREFIX = @"bd~";
        public const string AWS_S3_FILEEXTENSION = @".txt";

        public const string ENTITYNAME = @"BDMetadatas";
        public const string ENTITYNAME_FRIENDLY = @"Metadata";
        public const string KEY_NAME = @"BDMetadata";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"md_uuid";
        private const string SCHEMAVERSION = @"md_schemaVersion";
        private const string CREATEDBY = @"md_createdBy";
        private const string CREATEDDATE = @"md_createdDate";
        private const string MODIFIEDBY = @"md_modifieddBy";
        private const string MODIFIEDDATE = @"md_modifiedDate";
        private const string ITEMID = @"md_itemId";
        private const string ITEMENTITYNAME = @"md_itemEntityName";
        private const string DISPLAYPARENTID = @"md_displayParentId";
        private const string DISPLAYPARENTENTITYNAME = @"md_displayParentEntityName";
        private const string DEMOGRAPHIC = @"md_demographic";


        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDMetadata</returns>
        public static BDMetadata CreateMetadata(Entities pContext, Guid pItemId, string pItemEntityName)
        {
            BDMetadata entry = CreateBDMetadata(Guid.NewGuid());
            entry.createdBy = Guid.Empty;
            entry.createdDate = DateTime.Now;
            entry.schemaVersion = ENTITY_SCHEMAVERSION;
            entry.itemId = pItemId;
            entry.itemEntityName = pItemEntityName;

            pContext.AddObject(ENTITYNAME, entry);
            return entry;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pEntity"></param>
        public static void Save(Entities pContext, BDMetadata pEntity)
        {
            if (pEntity.EntityState != EntityState.Unchanged)
            {
                if (pEntity.schemaVersion != ENTITY_SCHEMAVERSION)
                    pEntity.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"Metadata Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDMetadata pEntity)
        {
            if (pEntity != null)
            {
                // delete record from local data store
                pContext.DeleteObject(pEntity);
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        /// <param name="pCreateDeletion">create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDMetadata entity = BDMetadata.GetMetadataWithId(pContext, pUuid);
            if (null != entity)
            {
                    pContext.DeleteObject(entity);
                    pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Return the Metadata for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pMetadataId"></param>
        /// <returns></returns>
        public static BDMetadata GetMetadataWithId(Entities pContext, Guid? pMetadataId)
        {
            BDMetadata result = null;

            if (null != pMetadataId)
            {
                IQueryable<BDMetadata> entries = (from entity in pContext.BDMetadatas
                                                  where entity.uuid == pMetadataId
                                                     select entity);

                if (entries.Count<BDMetadata>() > 0)
                    result = entries.AsQueryable().First<BDMetadata>();
            }

            return result;
        }

        public static List<BDMetadata> GetAll(Entities pContext)
        {
            IQueryable<BDMetadata> entries = (from entities in pContext.BDMetadatas
                                              select entities);
            List<BDMetadata> resultList = entries.ToList<BDMetadata>();
            return resultList;
        }

        public static BDMetadata GetMetadataWithItemId(Entities pContext, Guid? pItemId)
        {
            BDMetadata result = null;

            if (null != pItemId)
            {
                IQueryable<BDMetadata> entries = (from entity in pContext.BDMetadatas
                                                  where entity.itemId == pItemId
                                                  select entity);

                if (entries.Count<BDMetadata>() > 0)
                    result = entries.AsQueryable().First<BDMetadata>();
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
            IQueryable<BDMetadata> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDMetadatas
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDMetadatas
                           where entry.createdDate > pUpdateDateTime.Value
                           select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDMetadata>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE);
            syncInfo.PushList = BDSearchEntry.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDMetadata)syncInfo.PushList[idx]).createdDate = pCurrentSyncDate;
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
            BDMetadata entry = BDMetadata.GetMetadataWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDMetadata.CreateBDMetadata(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);

            entry.itemId = Guid.Parse(pAttributeDictionary[ITEMID]);
            entry.itemEntityName = pAttributeDictionary[ITEMENTITYNAME];
            entry.displayParentId = Guid.Parse(pAttributeDictionary[DISPLAYPARENTID]);
            entry.displayParentEntityName = pAttributeDictionary[DISPLAYPARENTENTITYNAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.ITEMID).WithValue((null == itemId) ? Guid.Empty.ToString() : itemId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.ITEMENTITYNAME).WithValue((null == itemEntityName) ? string.Empty : itemEntityName).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.DISPLAYPARENTID).WithValue((null == displayParentId) ? Guid.Empty.ToString() : displayParentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.DISPLAYPARENTENTITYNAME).WithValue((null == displayParentEntityName) ? string.Empty : displayParentEntityName).WithReplace(true));

            return putAttributeRequest;
        }

        #endregion

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public override string ToString()
        {
            return this.uuid.ToString();
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }
    }
}

