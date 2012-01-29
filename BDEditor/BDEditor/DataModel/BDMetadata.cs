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
        public enum LayoutVariantType
        {
            Undefined = -1,
            TreatmentRecommendation00 = 100, // Chapter
            TreatmentRecommendation01 = 101, // format specific section within chapter
            TreatmentRecommendation02 = 102,
            TreatmentRecommendation03 = 103
        }
        //public const string AWS_DOMAIN = @"bd_1_metadatas";

        public const string AWS_PROD_DOMAIN = @"bd_2_metadatas";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_metadatas";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        //public const string AWS_BUCKET = @"bdDataStore";
        //public const string AWS_S3_PREFIX = @"bd~";
        //public const string AWS_S3_FILEEXTENSION = @".txt";

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
        private const string ITEMTYPE = @"md_itemType";
        private const string ITEMKEYNAME = @"md_itemKeyName";
        private const string DISPLAYPARENTID = @"md_displayParentId";
        private const string DISPLAYPARENTTYPE = @"md_displayParentType";
        private const string DISPLAYPARENTKEYNAME = @"md_displayParentKeyName";
        private const string DEMOGRAPHIC = @"md_demographic";
        private const string LAYOUTVARIANT = @"md_layoutVariant";

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDMetadata</returns>
        public static BDMetadata CreateMetadata(Entities pContext, LayoutVariantType pLayoutVariant, IBDNode pBDNodeObject)
        {
            if (null == pBDNodeObject) return null;

            return CreateMetadata(pContext, pLayoutVariant, pBDNodeObject.Uuid, pBDNodeObject.NodeType, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDMetadata</returns>
        public static BDMetadata CreateMetadata(Entities pContext, LayoutVariantType pLayoutVariant, IBDNode pBDNodeObject, Guid pUuid)
        {
            if (null == pBDNodeObject) return null;

            return CreateMetadata(pContext, pLayoutVariant, pBDNodeObject.Uuid, pBDNodeObject.NodeType, pUuid);
        }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <returns>BDMetadata</returns>
        public static BDMetadata CreateMetadata(Entities pContext, LayoutVariantType pLayoutVariant, Guid pItemId, Constants.BDNodeType pItemKeyType)
        {
            return CreateMetadata(pContext,pLayoutVariant, pItemId,pItemKeyType, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and schema version. Returns instance if already exists.
        /// </summary>
        /// <returns>BDMetadata</returns>
        public static BDMetadata CreateMetadata(Entities pContext, LayoutVariantType pLayoutVariant, Guid pItemId, Constants.BDNodeType pItemKeyType, Guid pUuid)
        {
            BDMetadata entry = GetMetadataWithItemId(pContext, pItemId);
            if (null == entry)
            {
                entry = CreateBDMetadata(pUuid);
                entry.createdBy = Guid.Empty;
                entry.createdDate = DateTime.Now;
                entry.schemaVersion = ENTITY_SCHEMAVERSION;
                entry.itemId = pItemId;
                entry.itemType = (int)pItemKeyType;
                entry.itemKeyName = pItemKeyType.ToString();
                entry.SetLayoutVariant(pLayoutVariant);

                pContext.AddObject(ENTITYNAME, entry);
            }
            return entry;
        }

        public static Boolean Exists(Entities pContext, Guid pItemId)
        {
            IQueryable<BDMetadata> entries = (from entry in pContext.BDMetadatas
                                                       where (entry.itemId == pItemId)
                                                       select entry);

            Boolean result = (entries.Count<BDMetadata>() > 0);

            return result;
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
                BDDeletion.CreateDeletion(pContext, KEY_NAME, pEntity.uuid);
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
                BDDeletion.CreateDeletion(pContext, KEY_NAME, pUuid);
                pContext.DeleteObject(entity);
                pContext.SaveChanges();
            }
        }

        public static void DeleteForItemId(Entities pContext, Guid? pItemId)
        {
            BDMetadata meta = GetMetadataWithItemId(pContext, pItemId);
            Delete(pContext, meta);
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
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
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
            entry.itemType = (null == pAttributeDictionary[ITEMTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[ITEMTYPE]);
            entry.itemKeyName = pAttributeDictionary[ITEMKEYNAME];

            entry.displayParentId = Guid.Parse(pAttributeDictionary[DISPLAYPARENTID]);
            entry.displayParentType = (null == pAttributeDictionary[DISPLAYPARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYPARENTTYPE]);
            entry.displayParentKeyName = pAttributeDictionary[DISPLAYPARENTKEYNAME];

            entry.layoutVariant = short.Parse(pAttributeDictionary[LAYOUTVARIANT]);

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public void SetLayoutVariant(LayoutVariantType pLayoutVariantType)
        {
            layoutVariant = (int)pLayoutVariantType;
        }

        public LayoutVariantType LayoutVariant
        {
            get
            {
                LayoutVariantType result = LayoutVariantType.Undefined;

                if (Enum.IsDefined(typeof(LayoutVariantType), layoutVariant))
                {
                    result = (LayoutVariantType)layoutVariant;
                }
                return result;
            }
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
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.ITEMTYPE).WithValue(string.Format(@"{0}", itemType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.ITEMKEYNAME).WithValue((null == itemKeyName) ? string.Empty : itemKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.DISPLAYPARENTID).WithValue((null == displayParentId) ? Guid.Empty.ToString() : displayParentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.DISPLAYPARENTTYPE).WithValue(string.Format(@"{0}", displayParentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.DISPLAYPARENTKEYNAME).WithValue((null == displayParentKeyName) ? string.Empty : displayParentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDMetadata.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

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

        public Constants.BDNodeType NodeType
        {
            get { throw new NotImplementedException(); }
        }
    }
}

