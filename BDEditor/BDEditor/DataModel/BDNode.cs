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
    /// Extension of generated BDNode
    /// </summary>
    public partial class BDNode: IBDNode
    {
        //public const string AWS_DOMAIN = @"bd_1_nodes";

        public const string AWS_PROD_DOMAIN = @"bd_2_nodes";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_nodes";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDNodes";
        public const string ENTITYNAME_FRIENDLY = @"Node";
        public const string KEY_NAME = @"BDNode";
        public const string PROPERTYNAME_NAME = @"Name";
        public const string VIRTUALPROPERTYNAME_OVERVIEW = @"Overview";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"no_uuid";
        private const string SCHEMAVERSION = @"no_schemaVersion";
        private const string CREATEDBY = @"no_createdBy";
        private const string CREATEDDATE = @"no_createdDate";
        private const string MODIFIEDBY = @"nosn_modifiedBy";
        private const string MODIFIEDDATE = @"no_modifiedDate";
        private const string NAME = @"no_name";
        private const string DISPLAYORDER = @"no_displayOrder";

        private const string PARENTID = @"no_parentId";
        private const string PARENTTYPE = @"no_parentType";
        private const string PARENTKEYNAME = @"no_parentKeyName";

        private const string NODETYPE = @"no_nodeType";
        private const string NODEKEYNAME = @"no_nodeKeyName";

        private const string INUSEBY = @"no_inUseBy";


        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDNode CreateNode(Entities pContext, Constants.BDNodeType pNodeType)
        {
            return CreateNode(pContext, pNodeType, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDNode CreateNode(Entities pContext, Constants.BDNodeType pNodeType, Guid pUuid)
        {
            BDNode node = CreateBDNode(pUuid);
            node.nodeType = (int)pNodeType;
            node.nodeKeyName = pNodeType.ToString();
            node.createdBy = Guid.Empty;
            node.createdDate = DateTime.Now;
            node.schemaVersion = ENTITY_SCHEMAVERSION;
            node.displayOrder = -1;
            node.name = string.Empty;

            pContext.AddObject(ENTITYNAME, node);

            return node;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pNode"></param>
        public static void Save(Entities pContext, BDNode pNode)
        {
            if (pNode.EntityState != EntityState.Unchanged)
            {
                if (pNode.schemaVersion != ENTITY_SCHEMAVERSION)
                    pNode.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"Node Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDNode pEntity)
        {
            // delete linked notes
            List<BDLinkedNoteAssociation> linkedNotes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in linkedNotes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }

            // delete children
            //List<BDNode> children = BDNode.GetChildNodesForParentId(pContext, pEntity.uuid);
            //foreach (BDNode n in children)
            //{
            //    BDNode.Delete(pContext, n);
            //}

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
            BDNode entity = BDNode.GetNodeWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDNode.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Get Node with the specified ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>BDNode object.</returns>
        public static BDNode GetNodeWithId(Entities pContext, Guid pUuid)
        {
            BDNode section = null;

            if (null != pUuid)
            {
                IQueryable<BDNode> entries = (from bdNodes in pContext.BDNodes
                                                  where bdNodes.uuid == pUuid
                                                  select bdNodes);
                if (entries.Count<BDNode>() > 0)
                    section = entries.AsQueryable().First<BDNode>();
            }
            return section;
        }

        public static List<BDNode> GetAll(Entities pContext)
        {
            List<BDNode> entryList = new List<BDNode>();
            IQueryable<BDNode> entries = (from entry in pContext.BDNodes
                                                 orderby entry.displayOrder
                                                 select entry);
            if (entries.Count() > 0)
                entryList = entries.ToList<BDNode>();
            return entryList;
        }

        /// <summary>
        /// Get a string array of distinct names for all  nodes of the specified BDNodeType
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNodeType"></param>
        /// <returns></returns>
        public static string[] GetNodeNamesForType(Entities pContext, Constants.BDNodeType pNodeType)
        {
            var nodeNames = pContext.BDNodes.Where(x => (!string.IsNullOrEmpty(x.Name) && x.NodeType == pNodeType)).Select(node => node.Name).Distinct();
            return nodeNames.ToArray();
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
            IQueryable<BDNode> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDNodes
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDNodes
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDNode>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDNode.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDNode)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
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
            BDNode entry = BDNode.GetNodeWithId(pDataContext, uuid);

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.name = pAttributeDictionary[NAME];

            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentType = (null == pAttributeDictionary[PARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[PARENTTYPE]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

            entry.nodeType = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
            entry.nodeKeyName = pAttributeDictionary[NODEKEYNAME];

            entry.inUseBy = Guid.Parse(pAttributeDictionary[INUSEBY]);

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.NODETYPE).WithValue(string.Format(@"{0}", nodeType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.NODEKEYNAME).WithValue((null == nodeKeyName) ? string.Empty : nodeKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.INUSEBY).WithValue(inUseBy.ToString().ToUpper()).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

        public Guid? ParentId
        {
            get { return parentId; }
        }

        public void SetParent(IBDNode pParent)
        {
            if (null == pParent)
            {
                SetParent(Constants.BDNodeType.None, null);
            }
            else
            {
                SetParent(pParent.NodeType, pParent.Uuid);
            }
        }

        public void SetParent(Constants.BDNodeType pParentType, Guid? pParentId)
        {
            parentId = pParentId;
            parentType = (int)pParentType;
            parentKeyName = pParentType.ToString();
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

        public Constants.BDNodeType ParentType
        {
            get
            {
                Constants.BDNodeType result = Constants.BDNodeType.None;

                if (Enum.IsDefined(typeof(Constants.BDNodeType), parentType))
                {
                    result = (Constants.BDNodeType)parentType;
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
            get { return this.name; }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("{0}: {1}", ENTITYNAME_FRIENDLY, this.name); }
        }

        public override string ToString()
        {
            return this.name;
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}

