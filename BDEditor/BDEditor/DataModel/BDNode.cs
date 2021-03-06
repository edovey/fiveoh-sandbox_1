﻿using System;
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
        private const string MODIFIEDBY = @"no_modifiedBy";
        private const string MODIFIEDDATE = @"no_modifiedDate";
        private const string NAME = @"no_name";
        private const string DISPLAYORDER = @"no_displayOrder";
        private const string LAYOUTVARIANT = @"no_layoutVariant";

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
        public static BDNode CreateBDNode(Entities pContext, BDConstants.BDNodeType pNodeType)
        {
            return CreateBDNode(pContext, pNodeType, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDNode CreateBDNode(Entities pContext, BDConstants.BDNodeType pNodeType, Guid pUuid)
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
        /// Extended Save method 
        /// </summary>
        /// <param name="pNode"></param>
        public static void Save(Entities pContext, BDNode pNode)
        {
            if (null != pNode)
            {
                if (pNode.EntityState != EntityState.Unchanged)
                {
                    if (pNode.schemaVersion != ENTITY_SCHEMAVERSION)
                        pNode.schemaVersion = ENTITY_SCHEMAVERSION;

                    System.Diagnostics.Debug.WriteLine(@"Node Save");
                    pContext.SaveChanges();
                }
            }
            //if (pNode.Name == string.Empty)
            //    throw new NotSupportedException();
        }

        /// <summary>
        /// Extended Delete method that deletes the requested record as well as children and associated search, linked note records.
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pNode">the entry to be deleted</param>
        public static void Delete(Entities pContext, IBDNode pNode, bool pCreateDeletion)
        {
            if (null == pNode) return;

            BDLinkedNoteAssociation.DeleteForParentId(pContext, pNode.Uuid);
            BDSearchEntryAssociation.DeleteForAnchorNodeUuid(pContext, pNode.Uuid);

            List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in children)
            {
                switch (child.NodeType)
                {
                    case BDConstants.BDNodeType.BDTherapy:
                        BDTherapy therapyChild = child as BDTherapy;
                        BDTherapy.Delete(pContext, therapyChild, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        BDTherapyGroup therapyGroupChild = child as BDTherapyGroup;
                        BDTherapyGroup.Delete(pContext, therapyGroupChild, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDTableRow:
                        BDTableRow row = child as BDTableRow;
                        BDTableRow.Delete(pContext, row, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDTableCell:
                        BDTableCell cell = child as BDTableCell;
                        BDTableCell.Delete(pContext, cell, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDDosage:
                        BDDosage dosage = child as BDDosage;
                        BDDosage.Delete(pContext, dosage, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDPrecaution:
                        BDPrecaution precaution = child as BDPrecaution;
                        BDPrecaution.Delete(pContext, precaution, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDAttachment:
                        BDAttachment attachment = child as BDAttachment;
                        BDAttachment.Delete(pContext, attachment, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDAntimicrobialRisk:
                        BDAntimicrobialRisk risk = child as BDAntimicrobialRisk;
                        BDAntimicrobialRisk.Delete(pContext, risk, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDRegimenGroup:
                        BDRegimenGroup rGroup = child as BDRegimenGroup;
                        BDRegimenGroup.Delete(pContext, rGroup, pCreateDeletion);
                        break;
                    case BDConstants.BDNodeType.BDRegimen:
                        BDRegimen regimen = child as BDRegimen;
                        BDRegimen.Delete(pContext, regimen, pCreateDeletion);
                        break;
                    default:
                        BDNode.Delete(pContext, child, pCreateDeletion);
                        break;
                }
            }

            // delete record from local data store
            pContext.DeleteObject(pNode);
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
            BDNode entity = BDNode.RetrieveNodeWithId(pContext, pUuid);
            BDNode.Delete(pContext, entity, pCreateDeletion);
        }

        public static bool HasHtmlPage(Entities pContext, BDNode pNode)
        {
            Guid parentPageId = Guid.Empty;
            if (pNode != null)
                parentPageId = pNode.uuid;
            bool returnValue = false;
            IQueryable<BDHtmlPage> entries = (from entry in pContext.BDHtmlPages
                                          where entry.displayParentId == parentPageId
                                          select entry);
            if (entries.Count<BDHtmlPage>() > 0)
                returnValue = true;
            return returnValue;

        }

        public static bool HasAttachedNotes(Entities pContext, BDNode pNode)
        {
            bool returnValue = false;
            // find associations for node
            IQueryable<BDLinkedNoteAssociation> linkedNoteAssociations = (from bdLinkedNoteAssociations in pContext.BDLinkedNoteAssociations
                                                                          where bdLinkedNoteAssociations.parentId == pNode.uuid
                                                                          && (bdLinkedNoteAssociations.linkedNoteType == 0 || bdLinkedNoteAssociations.linkedNoteType == 9)
                                                                          select bdLinkedNoteAssociations);

            if (linkedNoteAssociations.Count<BDLinkedNoteAssociation>() > 0)
                returnValue = true;
            return returnValue;
        }

        public static List<BDNode> RetrieveAll(Entities pContext)
        {
            IQueryable<BDNode> nodes = (from entry in pContext.BDNodes
                                        select entry);
            return nodes.ToList<BDNode>();
        }

        /// <summary>
        /// Retrieve Node with the specified ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>BDNode object.</returns>
        public static BDNode RetrieveNodeWithId(Entities pContext, Guid pUuid)
        {
            BDNode entry = null;

            if (null != pUuid)
            {
                IQueryable<BDNode> entries = (from bdNodes in pContext.BDNodes
                                                  where bdNodes.uuid == pUuid
                                                  select bdNodes);
                if (entries.Count<BDNode>() > 0)
                    entry = entries.AsQueryable().First<BDNode>();
            }
            return entry;
        }

        public static List<BDNode> RetrieveNodesForParentIdAndChildNodeType(Entities pContext, Guid pParentId, BDConstants.BDNodeType pChildNodeType)
        {
            IQueryable<BDNode> nodeEntries = (from entry in pContext.BDNodes
                                              where (entry.parentId == pParentId) && (entry.nodeType == (int)pChildNodeType)
                                              orderby entry.displayOrder ascending
                                              select entry);
            return nodeEntries.ToList<BDNode>();
        }

        /// <summary>
        /// Retrieve a string array of distinct names for all  nodes of the specified BDNodeType
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNodeType"></param>
        /// <returns></returns>
        public static string[] RetrieveNodeNamesForType(Entities pContext, BDConstants.BDNodeType pNodeType)
        {
            var nodeNames = pContext.BDNodes.Where(x => (!string.IsNullOrEmpty(x.name) && x.nodeType == (int)pNodeType)).Select(node => node.name).Distinct();
            return nodeNames.ToArray();
        }

        /// <summary>
        /// Retrieve all nodes of a specified type in display order
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNodeType"></param>
        /// <returns></returns>
        public static List<BDNode> RetrieveNodesForType(Entities pContext, BDConstants.BDNodeType pNodeType)
        {
            List<BDNode> entryList = new List<BDNode>();
            IQueryable<BDNode> entries = (from entry in pContext.BDNodes
                                          where entry.nodeType == (int)pNodeType
                                          orderby entry.displayOrder
                                          select entry);
            if (entries.Count() > 0)
                entryList = entries.ToList<BDNode>();
            return entryList;
        }

        public static List<BDNode> RetrieveNodesWithNameContainingStringOfType(Entities pContext, string pString, BDConstants.BDNodeType pNodeType)
        {
            List<BDNode> returnList = new List<BDNode>();
            if (null != pString && pString.Length > 0)
            {
                IQueryable<BDNode> entries = (from entry in pContext.BDNodes
                                                 where (entry.name.Contains(pString) && entry.nodeType == (int)pNodeType)
                                                 select entry);
                returnList = entries.ToList<BDNode>();
            }
            return returnList;
        }

        /// <summary>
        /// Return the maximum value of the display order found in the children of the specified parent
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParent"></param>
        /// <returns></returns>
        public static int? RetrieveMaximumDisplayOrderForChildren(Entities pContext, BDNode pParent)
        {
            var maxDisplayorder = pContext.BDNodes.Where(x => (x.parentId == pParent.Uuid)).Select(node => node.displayOrder).Max();
            return (null == maxDisplayorder)? 0 : maxDisplayorder.Value;
        }

        public static int? RetrieveChildCountForNode(Entities pContext, BDNode pParent)
        {
            if (null == pParent) return 0;
            int count = pContext.BDNodes.Where(x => (x.parentId == pParent.Uuid)).Count();
            return count;
        }

        protected override void OnPropertyChanged(string property)
        {
            if (!BDCommon.Settings.IsSyncLoad)
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

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDNode.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if ((null != pCurrentSyncDate) && (!BDCommon.Settings.RepositoryOverwriteEnabled))
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDNode)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
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
            Guid dataUuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDNode entry = BDNode.RetrieveNodeWithId(pDataContext, dataUuid);

            if (null == entry)
            {
                int nt = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
                BDConstants.BDNodeType nodeType = BDConstants.BDNodeType.None;

                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), nt))
                {
                    nodeType = (BDConstants.BDNodeType)nt;
                }

                entry = BDNode.CreateBDNode(dataUuid);
                entry.nodeType = nt;

                pDataContext.AddObject(ENTITYNAME, entry);
            }

            entry.nodeType = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
            entry.schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]); 
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
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
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.NODETYPE).WithValue(string.Format(@"{0}", nodeType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.NODEKEYNAME).WithValue((null == nodeKeyName) ? string.Empty : nodeKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDNode.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

            //attributeList.Add(new ReplaceableAttribute().WithName(BDNode.INUSEBY).WithValue(inUseBy.ToString().ToUpper()).WithReplace(true));

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

