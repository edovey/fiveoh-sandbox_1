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
    public partial class BDObjectAssociation: IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_objectAssociation";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_objectAssociation";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif
        public const string ENTITYNAME = @"BDObjectAssociations";
        public const string ENTITYNAME_FRIENDLY = @"ObjectAssociations";
        public const string KEY_NAME = @"BDObjectAssociation";
        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"oa_uuid";
        private const string SCHEMAVERSION = @"oa_schemaVersion";
        private const string CREATEDDATE = @"oa_createdDate";

        private const string OBJECTID = @"oa_objectId";

        private const string OBJECTKEYNAME = @"oa_objectKeyName";
        private const string CHILDKEYNAME = @"oa_childKeyName";

        public static void CreateObjectAssociation(Entities pContext, Guid pObjectId, string pObjectKeyName, string pChildKeyName)
        {
            CreateObjectAssociation(pContext, pObjectId, pObjectKeyName, pChildKeyName, Guid.NewGuid());
        }

        public static void CreateObjectAssociation(Entities pContext, Guid pObjectId, string pObjectKeyName, string pChildKeyName, Guid pUuid)
        {
            if (!Exists(pContext, pObjectId, pObjectKeyName, pChildKeyName, pUuid))
            {
                BDObjectAssociation association = CreateBDObjectAssociation(pUuid);
                association.objectId = pObjectId;
                association.objectKeyName = pObjectKeyName;
                association.childKeyName = pChildKeyName;
                association.schemaVersion = ENTITY_SCHEMAVERSION;

                pContext.AddObject(ENTITYNAME, association);
            }
        }

        public static Boolean Exists(Entities pContext, Guid pObjectId, string pObjectKeyName, string pChildKeyName, Guid pUuid)
        {
            IQueryable<BDObjectAssociation> entries = (from entry in pContext.BDObjectAssociations
                                                            where (entry.objectId == pObjectId) && (entry.objectKeyName == pObjectKeyName) && (entry.childKeyName == pChildKeyName)
                                                            select entry);

            Boolean result = (entries.Count<BDObjectAssociation>() > 0);

            return result;
        }

        public static BDObjectAssociation RetrieveWithId(Entities pContext, Guid pUuid)
        {
            BDObjectAssociation result = null;

            if (null != pUuid)
            {
                IQueryable<BDObjectAssociation> entries = (from entry in pContext.BDObjectAssociations
                                                 where entry.uuid == pUuid
                                                 select entry);
                if (entries.Count<BDObjectAssociation>() > 0)
                    result = entries.AsQueryable().First<BDObjectAssociation>();
            }

            return result;
        }

        public static List<BDObjectAssociation> RetrieveList(Entities pContext, Guid pObjectId)
        {
            List<BDObjectAssociation> resultList = new List<BDObjectAssociation>();

            IQueryable<BDObjectAssociation> entries = (from entry in pContext.BDObjectAssociations
                                                       where (entry.objectId == pObjectId) 
                                                       select entry);

            foreach (BDObjectAssociation item in entries)
            {
                resultList.Add(item);
            }

            return resultList;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return string.Format("[{0}][{1}]", this.objectKeyName, this.childKeyName); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("[{0}][{1}]", this.objectKeyName, this.childKeyName); }
        }

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<IBDObject> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<IBDObject> entryList = new List<IBDObject>();
            IQueryable<BDObjectAssociation> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDObjectAssociations
                           select entry);
            }
            else
            {
                entries = pContext.BDObjectAssociations.Where(x => x.createdDate > pUpdateDateTime);
            }
            if (entries.Count() > 0)
            {
                entryList = new List<IBDObject>(entries.ToList<BDObjectAssociation>());
            }
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDObjectAssociation.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;

            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }

            return syncInfo;
        }

        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);

            BDObjectAssociation entry = BDObjectAssociation.RetrieveWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDObjectAssociation.CreateBDObjectAssociation(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);

            entry.objectId = Guid.Parse(pAttributeDictionary[OBJECTID]);
            entry.objectKeyName = pAttributeDictionary[OBJECTKEYNAME];

            entry.childKeyName = pAttributeDictionary[CHILDKEYNAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDObjectAssociation.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDObjectAssociation.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDObjectAssociation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDObjectAssociation.OBJECTKEYNAME).WithValue((null == objectKeyName) ? string.Empty : objectKeyName).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDObjectAssociation.CHILDKEYNAME).WithValue((null == childKeyName) ? string.Empty : childKeyName).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDObjectAssociation.OBJECTID).WithValue((null == objectId) ? Guid.Empty.ToString() : objectId.ToString().ToUpper()).WithReplace(true));

            return putAttributeRequest;
        }
    }
}
