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
    public partial class BDNodeAssociation: IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_nodeAssociation";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_nodeAssociation";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif
        public const string ENTITYNAME = @"BDNodeAssociations";
        public const string ENTITYNAME_FRIENDLY = @"NodeAssociations";
        public const string KEY_NAME = @"BDNodeAssociation";
        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"na_uuid";
        private const string SCHEMAVERSION = @"na_schemaVersion";
        private const string CREATEDDATE = @"na_createdDate";
        private const string NODEID = @"na_nodeId";
        private const string NODETYPE = @"na_nodeType";
        private const string NODEKEYNAME = @"na_nodeKeyName";
        private const string CHILDNODETYPE = @"na_childNodeType";
        private const string CHILDKEYNAME = @"na_childKeyName";

        public static void CreateNodeAssociation(Entities pContext, IBDObject pNodeObject, Constants.BDNodeType pChildNodeType)
        {
            CreateNodeAssociation(pContext, pNodeObject.Uuid, pNodeObject.NodeType, pChildNodeType, Guid.NewGuid());
        }

        public static void CreateNodeAssociation(Entities pContext, IBDObject pNodeObject, Constants.BDNodeType pChildNodeType, Guid pUuid)
        {
            CreateNodeAssociation(pContext, pNodeObject.Uuid, pNodeObject.NodeType, pChildNodeType, pUuid);
        }

        private static void CreateNodeAssociation(Entities pContext, Guid pNodeId, Constants.BDNodeType pNodeType, Constants.BDNodeType pChildNodeType, Guid pUuid)
        {
            if (!Exists(pContext, pNodeId, pChildNodeType))
            {
                BDNodeAssociation association = CreateBDNodeAssociation(pUuid);
                association.nodeId = pNodeId;
                association.nodeType = (int)pNodeType;
                association.nodeKeyName = pNodeType.ToString();
                association.childNodeType = (int)pChildNodeType;
                association.childKeyName = pChildNodeType.ToString();
                association.schemaVersion = ENTITY_SCHEMAVERSION;
                association.createdDate = DateTime.Now;

                pContext.AddObject(ENTITYNAME, association);
                Save(pContext, association);
            }
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pEntry"></param>
        public static void Save(Entities pContext, BDNodeAssociation pEntry)
        {
            if (pEntry.EntityState != EntityState.Unchanged)
            {
                if (pEntry.schemaVersion != ENTITY_SCHEMAVERSION)
                    pEntry.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"ObjectAssociation Save");
                pContext.SaveChanges();
            }
        }

        public static Boolean Exists(Entities pContext, Guid pObjectId, Constants.BDNodeType pChildNodeType)
        {
            IQueryable<BDNodeAssociation> entries = (from entry in pContext.BDNodeAssociations
                                                     where (entry.nodeId == pObjectId) && (entry.childNodeType == (int)pChildNodeType)
                                                            select entry);

            Boolean result = (entries.Count<BDNodeAssociation>() > 0);

            return result;
        }

        public static BDNodeAssociation RetrieveWithId(Entities pContext, Guid pUuid)
        {
            BDNodeAssociation result = null;

            if (null != pUuid)
            {
                IQueryable<BDNodeAssociation> entries = (from entry in pContext.BDNodeAssociations
                                                 where entry.uuid == pUuid
                                                 select entry);
                if (entries.Count<BDNodeAssociation>() > 0)
                    result = entries.AsQueryable().First<BDNodeAssociation>();
            }

            return result;
        }

        public static List<BDNodeAssociation> RetrieveList(Entities pContext, Guid pObjectId)
        {
            List<BDNodeAssociation> resultList = new List<BDNodeAssociation>();

            IQueryable<BDNodeAssociation> entries = (from entry in pContext.BDNodeAssociations
                                                       where (entry.nodeId == pObjectId) 
                                                       select entry);

            foreach (BDNodeAssociation item in entries)
            {
                resultList.Add(item);
            }

            return resultList;
        }

        public Constants.BDNodeType NodeType
        {
            get
            {
                Constants.BDNodeType result = Constants.BDNodeType.None;

                if (Enum.IsDefined(typeof(Constants.BDNodeType), nodeType))
                {
                    result = (Constants.BDNodeType)nodeType;
                }
                return result;
            }
        }

        public Constants.BDNodeType ChildNodeType
        {
            get
            {
                Constants.BDNodeType result = Constants.BDNodeType.None;

                if (Enum.IsDefined(typeof(Constants.BDNodeType), childNodeType))
                {
                    result = (Constants.BDNodeType)childNodeType;
                }
                return result;
            }
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return string.Format("[{0}][{1}]", this.nodeKeyName, this.childKeyName); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("[{0}][{1}]", this.nodeKeyName, this.childKeyName); }
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
            IQueryable<BDNodeAssociation> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDNodeAssociations
                           select entry);
            }
            else
            {
                entries = pContext.BDNodeAssociations.Where(x => x.createdDate > pUpdateDateTime);
            }
            if (entries.Count() > 0)
            {
                entryList = new List<IBDObject>(entries.ToList<BDNodeAssociation>());
            }
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDNodeAssociation.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;

            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }

            return syncInfo;
        }

        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);

            BDNodeAssociation entry = BDNodeAssociation.RetrieveWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDNodeAssociation.CreateBDNodeAssociation(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            entry.schemaVersion = (null == pAttributeDictionary[SCHEMAVERSION]) ? (short)0 : short.Parse(pAttributeDictionary[SCHEMAVERSION]); ;
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);

            entry.nodeId = Guid.Parse(pAttributeDictionary[NODEID]);
            entry.nodeType = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
            entry.nodeKeyName = pAttributeDictionary[NODEKEYNAME];

            entry.childNodeType = (null == pAttributeDictionary[CHILDNODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[CHILDNODETYPE]);
            entry.childKeyName = pAttributeDictionary[CHILDKEYNAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;

            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.NODEID).WithValue((null == nodeId) ? Guid.Empty.ToString() : nodeId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.NODETYPE).WithValue(string.Format(@"{0}", nodeType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.NODEKEYNAME).WithValue((null == nodeKeyName) ? string.Empty : nodeKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.CHILDNODETYPE).WithValue(string.Format(@"{0}", childNodeType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.CHILDKEYNAME).WithValue((null == childKeyName) ? string.Empty : childKeyName).WithReplace(true));

            return putAttributeRequest;
        }
    }
}
