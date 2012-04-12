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
    /// Extension of generated BDString
    /// </summary>
    public partial class BDString : IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_strings";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_strings";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDStrings";
        public const string ENTITYNAME_FRIENDLY = @"String";
        public const string KEY_NAME = @"BDString";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"st_uuid";
        private const string SCHEMAVERSION = @"st_schemaVersion";
        private const string MODIFIEDDATE = @"st_modifiedDate";
        private const string DISPLAYORDER = @"st_displayOrder";
        private const string PARENTID = @"st_parentId";
        private const string VALUE = @"st_value";

        public static void CreateString(Entities pContext)
        {
            CreateString(pContext, Guid.NewGuid());
        }

        public static BDString CreateString(Entities pContext, Guid pUuid)
        {
            BDString str = CreateBDString(pUuid);
            str.modifiedDate = DateTime.Now;
            str.schemaVersion = ENTITY_SCHEMAVERSION;

            pContext.AddObject(ENTITYNAME, str);

            return str;
        }

        public static BDString RetrieveStringWithId(Entities pContext, Guid pUuid)
        {
            BDString str = null;
            if (null != pUuid)
            {
                IQueryable<BDString> entryList = (from entry in pContext.BDStrings
                                                        where entry.uuid == pUuid
                                                        select entry);

                if (entryList.Count<BDString>() > 0)
                    str = entryList.AsQueryable().First<BDString>();
            }
            return str;
        }

        #region Repository
        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries.  List is empty if none found.</returns>
        public static List<IBDObject> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<IBDObject> entryList = new List<IBDObject>();
            IQueryable<BDString> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDStrings
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDStrings
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDString>());
            return entryList;
        }

        /// <summary>
        /// Extended Delete method that creates a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pNode">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDString pEntity, bool pCreateDeletion)
        {
            if (null == pEntity) return;

            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntity.Uuid, pCreateDeletion);

            BDMetadata.DeleteForItemId(pContext, pEntity.Uuid, pCreateDeletion);
            // create BDDeletion record for the object to be deleted

            if (pCreateDeletion)
                BDDeletion.CreateDeletion(pContext, KEY_NAME, pEntity.uuid);

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
            BDString entity = BDString.RetrieveStringWithId(pContext, pUuid);
            BDString.Delete(pContext, entity, pCreateDeletion);
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
                BDString entry = BDString.RetrieveStringWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pString"></param>
        public static void Save(Entities pContext, BDString pString)
        {
            if (null != pString)
            {
                if (pString.EntityState != EntityState.Unchanged)
                {
                    if (pString.schemaVersion != ENTITY_SCHEMAVERSION)
                        pString.schemaVersion = ENTITY_SCHEMAVERSION;

                    System.Diagnostics.Debug.WriteLine(@"BDString Save");
                    pContext.SaveChanges();
                }
            }
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDString.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if (null != pCurrentSyncDate)
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDString)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDString entry = BDString.RetrieveStringWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDString.CreateBDString(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }
            entry.schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.displayOrder = short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.value = pAttributeDictionary[VALUE];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        #endregion

        public void SetParent(Guid? pParentId)
        {
            parentId = pParentId;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return this.uuid.ToString(); }
        }

        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDString.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDString.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDString.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDString.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDString.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDString.VALUE).WithValue((null == value) ? string.Empty : value).WithReplace(true));

            return putAttributeRequest;
        }
    }
}
