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
    /// Extension of generated BDNavigationNode
    /// </summary>
    public partial class BDNavigationNode : IBDNode
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_navigationNodes";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_navigationNodes";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDNavigationNodes";
        public const string ENTITYNAME_FRIENDLY = @"NavigationNode";
        public const string KEY_NAME = @"BDNavigationNode";
        public const string PROPERTYNAME_NAME = @"Name";
        public const string VIRTUALPROPERTYNAME_OVERVIEW = @"Overview";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"na_uuid";
        private const string SCHEMAVERSION = @"na_schemaVersion";
        private const string NAME = @"na_name";
        private const string DISPLAYORDER = @"na_displayOrder";
        private const string LAYOUTVARIANT = @"na_layoutVariant";

        private const string PARENTID = @"na_parentId";
        private const string PARENTTYPE = @"na_parentType";
        private const string PARENTKEYNAME = @"na_parentKeyName";

        private const string NODETYPE = @"na_nodeType";
        private const string NODEKEYNAME = @"na_nodeKeyName";
        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDNavigationNode CreateBDNavigationNode(Entities pContext, BDConstants.BDNodeType pNodeType)
        {
            return CreateBDNavigationNode(pContext, pNodeType, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDNavigationNode CreateBDNavigationNode(Entities pContext, BDConstants.BDNodeType pNodeType, Guid pUuid)
        {
            BDNavigationNode node = CreateBDNavigationNode(pUuid);
            node.nodeType = (int)pNodeType;
            node.nodeKeyName = pNodeType.ToString();
            node.schemaVersion = ENTITY_SCHEMAVERSION;
            node.displayOrder = -1;
            node.name = string.Empty;

            pContext.AddObject(ENTITYNAME, node);

            return node;
        }

        public static BDNavigationNode CreateNavigationNodeFromBDNode(Entities pContext, BDNode pSourceNode)
        {
            BDNode parentNode = BDNode.RetrieveNodeWithId(pContext, pSourceNode.parentId.Value);
            BDNavigationNode newNode = CreateBDNavigationNode(pContext, pSourceNode.NodeType, pSourceNode.uuid);
            newNode.displayOrder = pSourceNode.displayOrder;
            newNode.SetParent(parentNode);
            newNode.name = pSourceNode.Name;
            newNode.LayoutVariant = pSourceNode.LayoutVariant;
            BDNavigationNode.Save(pContext, newNode);
            return newNode;
        }

        /// <summary>
        /// Extended Save method 
        /// </summary>
        /// <param name="pNode"></param>
        public static void Save(Entities pContext, BDNavigationNode pNode)
        {
            if (null != pNode)
            {
                if (pNode.EntityState != EntityState.Unchanged)
                {
                    if (pNode.schemaVersion != ENTITY_SCHEMAVERSION)
                        pNode.schemaVersion = ENTITY_SCHEMAVERSION;

                    System.Diagnostics.Debug.WriteLine(@"Navigation Node Save");
                    pContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Extended Delete method that creates a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pNode">the entry to be deleted</param>
        public static void Delete(Entities pContext, IBDNode pNode, bool pCreateDeletion)
        {
            //if (null == pNode) return;

            //BDLinkedNoteAssociation.DeleteForParentId(pContext, pNode.Uuid, pCreateDeletion);

            //List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
            //foreach (IBDNode child in children)
            //{
            //    switch (child.NodeType)
            //    {
            //        case BDConstants.BDNodeType.BDTherapy:
            //            BDTherapy therapyChild = child as BDTherapy;
            //            BDTherapy.Delete(pContext, therapyChild, pCreateDeletion);
            //            break;
            //        case BDConstants.BDNodeType.BDTherapyGroup:
            //            BDTherapyGroup therapyGroupChild = child as BDTherapyGroup;
            //            BDTherapyGroup.Delete(pContext, therapyGroupChild, pCreateDeletion);
            //            break;
            //        case BDConstants.BDNodeType.BDTableRow:
            //            BDTableRow row = child as BDTableRow;
            //            BDTableRow.Delete(pContext, row, pCreateDeletion);
            //            break;
            //        case BDConstants.BDNodeType.BDTableCell:
            //            BDTableCell cell = child as BDTableCell;
            //            BDTableCell.Delete(pContext, cell, pCreateDeletion);
            //            break;
            //        case BDConstants.BDNodeType.BDDosage:
            //            BDDosage dosage = child as BDDosage;
            //            BDDosage.Delete(pContext, dosage, pCreateDeletion);
            //            break;
            //        case BDConstants.BDNodeType.BDPrecaution:
            //            BDPrecaution precaution = child as BDPrecaution;
            //            BDPrecaution.Delete(pContext, precaution, pCreateDeletion);
            //            break;
            //        case BDConstants.BDNodeType.BDAttachment:
            //            BDAttachment attachment = child as BDAttachment;
            //            BDAttachment.Delete(pContext, attachment, pCreateDeletion);
            //            break;
            //        case BDConstants.BDNodeType.BDAntimicrobialRisk:
            //            BDAntimicrobialRisk risk = child as BDAntimicrobialRisk;
            //            BDAntimicrobialRisk.Delete(pContext, risk, pCreateDeletion);
            //            break;
            //        default:
            //            BDNode.Delete(pContext, child, pCreateDeletion);
            //            break;
            //    }
            //}

            //BDMetadata.DeleteForItemId(pContext, pNode.Uuid, pCreateDeletion);
            //// create BDDeletion record for the object to be deleted
            //if (pCreateDeletion)
            //    BDDeletion.CreateBDDeletion(pContext, KEY_NAME, pNode);
            //// delete record from local data store
            //pContext.DeleteObject(pNode);
            //pContext.SaveChanges();
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        /// <param name="pCreateDeletion"create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            //BDNode entity = BDNode.RetrieveNodeWithId(pContext, pUuid);
            //BDNode.Delete(pContext, entity, pCreateDeletion);
        }

        /// <summary>
        /// Delete from the local datastore without creating a deletion record nor deleting any children. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid"></param>
        public static void DeleteLocal(Entities pContext, Guid? pUuid)
        {
            //if (null != pUuid)
            //{
            //    BDNode entry = BDNode.RetrieveNodeWithId(pContext, pUuid.Value);
            //    if (null != entry)
            //    {
            //        pContext.DeleteObject(entry);
            //    }
            //}
        }

        /// <summary>
        /// Delete all records from local store
        /// </summary>
        public static void DeleteAll()
        {
            BDEditor.DataModel.Entities dataContext = new BDEditor.DataModel.Entities();
            // check if table exists:

            int result = dataContext.ExecuteStoreQuery<int>(@"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME ='BDNavigationNodes'").SingleOrDefault();

            if (result == 1)
                dataContext.ExecuteStoreCommand("DELETE FROM BDNavigationNodes");
        }

        /// <summary>
        /// Retrieve all Navigation Nodes
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>List of BDNavigationNode objects.</returns>
        public static List<IBDObject> RetrieveAll(Entities pContext)
        {
            List<IBDObject> entryList;
            IQueryable<BDNavigationNode> entries = (from bdNodes in pContext.BDNavigationNodes
                                                    select bdNodes);
            entryList = new List<IBDObject>(entries.ToList<BDNavigationNode>());
            return entryList;
        }


        /// <summary>
        /// Retrieve Node with the specified ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>BDNavigationNode object.</returns>
        public static BDNavigationNode RetrieveNodeWithId(Entities pContext, Guid pUuid)
        {
            BDNavigationNode entry = null;

            if (null != pUuid)
            {
                IQueryable<BDNavigationNode> entries = (from bdNodes in pContext.BDNavigationNodes
                                              where bdNodes.uuid == pUuid
                                              select bdNodes);
                if (entries.Count<BDNavigationNode>() > 0)
                    entry = entries.AsQueryable().First<BDNavigationNode>();
            }
            return entry;
        }

        public static List<BDNavigationNode> RetrieveNodesForParentIdAndChildNodeType(Entities pContext, Guid pParentId, BDConstants.BDNodeType pChildNodeType)
        {
            IQueryable<BDNavigationNode> nodeEntries = (from entry in pContext.BDNavigationNodes
                                              where (entry.parentId == pParentId) && (entry.nodeType == (int)pChildNodeType)
                                              orderby entry.displayOrder ascending
                                              select entry);
            return nodeEntries.ToList<BDNavigationNode>();
        }

        /// <summary>
        /// Retrieve a string array of distinct names for all  nodes of the specified BDNodeType
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNodeType"></param>
        /// <returns></returns>
        public static string[] RetrieveNodeNamesForType(Entities pContext, BDConstants.BDNodeType pNodeType)
        {
            var nodeNames = pContext.BDNavigationNodes.Where(x => (!string.IsNullOrEmpty(x.name) && x.nodeType == (int)pNodeType)).Select(node => node.name).Distinct();
            return nodeNames.ToArray();
        }

        /// <summary>
        /// Retrieve all nodes of a specified type in display order
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNodeType"></param>
        /// <returns></returns>
        public static List<BDNavigationNode> RetrieveNodesForType(Entities pContext, BDConstants.BDNodeType pNodeType)
        {
            List<BDNavigationNode> entryList = new List<BDNavigationNode>();
            IQueryable<BDNavigationNode> entries = (from entry in pContext.BDNavigationNodes
                                          where entry.nodeType == (int)pNodeType
                                          orderby entry.displayOrder
                                          select entry);
            if (entries.Count() > 0)
                entryList = entries.ToList<BDNavigationNode>();
            return entryList;
        }

        public static List<BDNavigationNode> RetrieveNodesNameWithTextForType(Entities pContext, string pText, BDConstants.BDNodeType pNodeType)
        {
            List<BDNavigationNode> returnList = new List<BDNavigationNode>();
            if (null != pText && pText.Length > 0)
            {
                IQueryable<BDNavigationNode> entries = (from entry in pContext.BDNavigationNodes
                                              where (entry.name.Contains(pText) && entry.nodeType == (int)pNodeType)
                                              select entry);
                returnList = entries.ToList<BDNavigationNode>();
            }
            return returnList;
        }

        ///// <summary>
        ///// Return the maximum value of the display order found in the children of the specified parent
        ///// </summary>
        ///// <param name="pContext"></param>
        ///// <param name="pParent"></param>
        ///// <returns></returns>
        //public static int? RetrieveMaximumDisplayOrderForChildren(Entities pContext, BDNode pParent)
        //{
        //    var maxDisplayorder = pContext.BDNodes.Where(x => (x.parentId == pParent.Uuid)).Select(node => node.displayOrder).Max();
        //    return (null == maxDisplayorder) ? 0 : maxDisplayorder;
        //}

        protected override void OnPropertyChanged(string property)
        {        }

        #region Repository

        /// <summary>
        /// Retrieve all entries 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">All records returned</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<IBDObject> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<IBDObject> entryList = new List<IBDObject>();
            IQueryable<BDNavigationNode> entries;

                entries = (from entry in pContext.BDNavigationNodes
                           select entry);
            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDNavigationNode>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDNavigationNode.RetrieveAll(pDataContext);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDNavigationNode from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid dataUuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDNavigationNode entry = BDNavigationNode.RetrieveNodeWithId(pDataContext, dataUuid);

            if (null == entry)
            {
                int nt = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
                BDConstants.BDNodeType nodeType = BDConstants.BDNodeType.None;

                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), nt))
                {
                    nodeType = (BDConstants.BDNodeType)nt;
                }

                entry = BDNavigationNode.CreateBDNavigationNode(dataUuid);
                entry.nodeType = nt;

                pDataContext.AddObject(ENTITYNAME, entry);
            }

            entry.nodeType = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
            entry.schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.name = pAttributeDictionary[NAME];

            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentType = (null == pAttributeDictionary[PARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[PARENTTYPE]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

            entry.nodeKeyName = pAttributeDictionary[NODEKEYNAME];

            entry.layoutVariant = short.Parse(pAttributeDictionary[LAYOUTVARIANT]);

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return dataUuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.NODETYPE).WithValue(string.Format(@"{0}", nodeType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.NODEKEYNAME).WithValue((null == nodeKeyName) ? string.Empty : nodeKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNavigationNode.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

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
                SetParent(BDConstants.BDNodeType.None, null);
            }
            else
            {
                SetParent(pParent.NodeType, pParent.Uuid);
            }
        }

        public void SetParent(BDConstants.BDNodeType pParentType, Guid? pParentId)
        {
            parentId = pParentId;
            parentType = (int)pParentType;
            parentKeyName = pParentType.ToString();
        }

        public BDConstants.BDNodeType NodeType
        {
            get
            {
                BDConstants.BDNodeType result = BDConstants.BDNodeType.None;

                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), nodeType))
                {
                    result = (BDConstants.BDNodeType)nodeType;
                }
                return result;
            }
        }

        public BDConstants.BDNodeType ParentType
        {
            get
            {
                BDConstants.BDNodeType result = BDConstants.BDNodeType.None;

                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), parentType))
                {
                    result = (BDConstants.BDNodeType)parentType;
                }
                return result;
            }
        }

        public BDConstants.LayoutVariantType LayoutVariant
        {
            get
            {
                BDConstants.LayoutVariantType result = BDConstants.LayoutVariantType.Undefined;

                if (Enum.IsDefined(typeof(BDConstants.LayoutVariantType), layoutVariant))
                {
                    result = (BDConstants.LayoutVariantType)layoutVariant;
                }
                return result;
            }
            set
            {
                layoutVariant = (int)value;
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
            get { return string.Format("{0}: {1}", BDUtilities.GetEnumDescription(NodeType), this.name); }
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

        public int? DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }
    }
}