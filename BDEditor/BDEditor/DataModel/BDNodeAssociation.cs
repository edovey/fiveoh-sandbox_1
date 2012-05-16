//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.EntityClient;
//using System.Data.Objects;
//using System.Linq;
//using System.Text;

//using Amazon.SimpleDB;
//using Amazon.SimpleDB.Model;

//using BDEditor.Classes;

//namespace BDEditor.DataModel
//{
//    public partial class BDNodeAssociation: IBDObject
//    {
//        public const string AWS_PROD_DOMAIN = @"bd_2_nodeAssociations";
//        public const string AWS_DEV_DOMAIN = @"bd_dev_2_nodeAssociations";

//#if DEBUG
//        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
//#else
//        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
//#endif
//        public const string ENTITYNAME = @"BDNodeAssociations";
//        public const string ENTITYNAME_FRIENDLY = @"NodeAssociations";
//        public const string KEY_NAME = @"BDNodeAssociation";
//        public const int ENTITY_SCHEMAVERSION = 0;

//        private const string UUID = @"na_uuid";
//        private const string SCHEMAVERSION = @"na_schemaVersion";
//        private const string CREATEDDATE = @"na_createdDate";
//        private const string NODEID = @"na_nodeId";
//        private const string NODETYPE = @"na_nodeType";
//        private const string NODEKEYNAME = @"na_nodeKeyName";
//        private const string CHILDNODETYPE = @"na_childNodeType";
//        private const string CHILDKEYNAME = @"na_childKeyName";

//        public static void CreateNodeAssociation(Entities pContext, IBDNode pNodeObject, BDConstants.BDNodeType pChildNodeType)
//        {
//            CreateNodeAssociation(pContext, pNodeObject.Uuid, pNodeObject.NodeType, pChildNodeType, Guid.NewGuid());
//        }

//        public static void CreateNodeAssociation(Entities pContext, IBDNode pNodeObject, BDConstants.BDNodeType pChildNodeType, Guid pUuid)
//        {
//            CreateNodeAssociation(pContext, pNodeObject.Uuid, pNodeObject.NodeType, pChildNodeType, pUuid);
//        }

//        private static void CreateNodeAssociation(Entities pContext, Guid pNodeId, BDConstants.BDNodeType pNodeType, BDConstants.BDNodeType pChildNodeType, Guid pUuid)
//        {
//            if (!Exists(pContext, pNodeId, pChildNodeType))
//            {
//                BDNodeAssociation association = CreateBDNodeAssociation(pUuid);
//                association.nodeId = pNodeId;
//                association.nodeType = (int)pNodeType;
//                association.nodeKeyName = pNodeType.ToString();
//                association.childNodeType = (int)pChildNodeType;
//                association.childKeyName = pChildNodeType.ToString();
//                association.schemaVersion = ENTITY_SCHEMAVERSION;
//                association.createdDate = DateTime.Now;

//                pContext.AddObject(ENTITYNAME, association);
//                Save(pContext, association);
//            }
//        }

//        public static void CreateAssociationsForNode(Entities pDataContext, IBDNode pNode)
//        {
//            List<BDConstants.BDNodeType> childNodeTypeDefinitionList = ChildTypeDefinitionListForNode(pNode);
//            foreach (BDConstants.BDNodeType childNodeType in childNodeTypeDefinitionList)
//            {
//                BDNodeAssociation.CreateNodeAssociation(pDataContext, pNode, childNodeType);
//            }
//        }

//        public static List<BDConstants.BDNodeType> ChildTypeDefinitionListForNode(IBDNode pNode)
//        {
//            if (null == pNode)
//                return null;

//            if ((pNode.LayoutVariant == BDConstants.LayoutVariantType.Undefined) || (null == pNode.LayoutVariant))
//            {
//                string message = string.Format("Undefined Layout Variant for node parameter in ChildTypeDefinitionListForNode method call. [{0}]", BDUtilities.GetEnumDescription(pNode.NodeType));
//                throw new BDException(message, pNode);
//            }

//            return ChildTypeDefinitionListForNode(pNode.NodeType, pNode.LayoutVariant);
//        }

//        public static List<BDConstants.BDNodeType> ChildTypeDefinitionListForNode(BDConstants.BDNodeType pNodeType, BDConstants.LayoutVariantType pLayoutVariant)
//        {
//            List<BDConstants.BDNodeType> definitionList = new List<BDConstants.BDNodeType>();

//            BDConstants.LayoutVariantType layoutVariant = pLayoutVariant;
//            switch (pNodeType)
//            {
//                case BDConstants.BDNodeType.BDCategory:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                            definitionList.Add(BDConstants.BDNodeType.BDDisease);
//                            //definitionList.Add(BDConstants.BDNodeType.BDSubcategory);  //Test of multiple child types in nav tree
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDChapter:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                            definitionList.Add(BDConstants.BDNodeType.BDSection);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDDisease:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                            definitionList.Add(BDConstants.BDNodeType.BDPresentation);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDPathogen:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDPathogenGroup:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                            definitionList.Add(BDConstants.BDNodeType.BDPathogen);
//                            definitionList.Add(BDConstants.BDNodeType.BDTherapyGroup);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDPresentation:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                            definitionList.Add(BDConstants.BDNodeType.BDPathogenGroup);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDSection:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                            definitionList.Add(BDConstants.BDNodeType.BDCategory);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDSubcategory:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDTherapy:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.BDTherapyGroup:
//                    switch (layoutVariant)
//                    {
//                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
//                            definitionList.Add(BDConstants.BDNodeType.BDTherapy);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case BDConstants.BDNodeType.None:
//                default:
//                    definitionList = null;
//                    break;
//            }

//            return definitionList;
//        }

//        /// <summary>
//        /// Extended Save method that sets the modified date
//        /// </summary>
//        /// <param name="pEntry"></param>
//        public static void Save(Entities pContext, BDNodeAssociation pEntry)
//        {
//            if (pEntry.EntityState != EntityState.Unchanged)
//            {
//                if (pEntry.schemaVersion != ENTITY_SCHEMAVERSION)
//                    pEntry.schemaVersion = ENTITY_SCHEMAVERSION;

//                System.Diagnostics.Debug.WriteLine(@"Node Association Save");
//                pContext.SaveChanges();
//            }
//        }

//        public static Boolean Exists(Entities pContext, Guid pObjectId, BDConstants.BDNodeType pChildNodeType)
//        {
//            IQueryable<BDNodeAssociation> entries = (from entry in pContext.BDNodeAssociations
//                                                     where (entry.nodeId == pObjectId) && (entry.childNodeType == (int)pChildNodeType)
//                                                            select entry);

//            Boolean result = (entries.Count<BDNodeAssociation>() > 0);

//            return result;
//        }

//        public static BDNodeAssociation RetrieveForNodeIdAndChildType(Entities pContext, Guid pNodeId, BDConstants.BDNodeType pChildNodeType)
//        {
//            BDNodeAssociation result = null;

//            IQueryable<BDNodeAssociation> entries = (from entry in pContext.BDNodeAssociations
//                                                     where (entry.nodeId == pNodeId) && (entry.childNodeType == (int)pChildNodeType)
//                                                     select entry);

//            if (entries.Count<BDNodeAssociation>() > 0)
//                result = entries.AsQueryable().First<BDNodeAssociation>();

//            return result;
//        } 

//        public static BDNodeAssociation RetrieveWithId(Entities pContext, Guid pUuid)
//        {
//            BDNodeAssociation result = null;

//            if (null != pUuid)
//            {
//                IQueryable<BDNodeAssociation> entries = (from entry in pContext.BDNodeAssociations
//                                                 where entry.uuid == pUuid
//                                                 select entry);
//                if (entries.Count<BDNodeAssociation>() > 0)
//                    result = entries.AsQueryable().First<BDNodeAssociation>();
//            }

//            return result;
//        }

//        public static List<BDNodeAssociation> RetrieveList(Entities pContext, Guid pNodeId)
//        {
//            List<BDNodeAssociation> resultList = new List<BDNodeAssociation>();

//            IQueryable<BDNodeAssociation> entries = (from entry in pContext.BDNodeAssociations
//                                                       where (entry.nodeId == pNodeId) 
//                                                       select entry);

//            foreach (BDNodeAssociation item in entries)
//            {
//                resultList.Add(item);
//            }

//            return resultList;
//        }

//        /// <summary>
//        /// Extended Delete method that creates a deletion record as well as deleting the local record. 
//        /// Deletes all assoications for the node
//        /// </summary>
//        /// <param name="pContext">the data context</param>
//        /// <param name="pNode">the node whose child associations are to be deleted</param>
//        public static void Delete(Entities pContext, IBDNode pNode, bool pCreateDeletion)
//        {
//            if (null == pNode) return;

//            List<BDNodeAssociation> nodeAssociationList = RetrieveList(pContext, pNode.Uuid);
//            foreach (BDNodeAssociation association in nodeAssociationList)
//            {
//                if(pCreateDeletion)
//                    BDDeletion.CreateDeletion(pContext, KEY_NAME, association.Uuid);
//                pContext.DeleteObject(association);
//            }
//            pContext.SaveChanges();
//        }

//        /// <summary>
//        /// Delete from the local datastore without creating a deletion record nor deleting any children. Does not save.
//        /// </summary>
//        /// <param name="pContext"></param>
//        /// <param name="pUuid"></param>
//        public static void DeleteLocal(Entities pContext, Guid? pUuid)
//        {
//            if (null != pUuid)
//            {
//                BDNodeAssociation entry = BDNodeAssociation.RetrieveWithId(pContext, pUuid.Value);
//                if (null != entry)
//                {
//                    pContext.DeleteObject(entry);
//                }
//            }
//        }

//        public BDConstants.BDNodeType NodeType
//        {
//            get
//            {
//                BDConstants.BDNodeType result = BDConstants.BDNodeType.None;

//                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), nodeType))
//                {
//                    result = (BDConstants.BDNodeType)nodeType;
//                }
//                return result;
//            }
//        }

//        public BDConstants.BDNodeType ChildNodeType
//        {
//            get
//            {
//                BDConstants.BDNodeType result = BDConstants.BDNodeType.None;

//                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), childNodeType))
//                {
//                    result = (BDConstants.BDNodeType)childNodeType;
//                }
//                return result;
//            }
//        }

//        public Guid Uuid
//        {
//            get { return this.uuid; }
//        }

//        public string Description
//        {
//            get { return string.Format("[{0}][{1}]", this.nodeKeyName, this.childKeyName); }
//        }

//        public string DescriptionForLinkedNote
//        {
//            get { return string.Format("[{0}][{1}]", this.nodeKeyName, this.childKeyName); }
//        }

//        /// <summary>
//        /// Retrieve all entries changed since a given date
//        /// </summary>
//        /// <param name="pContext"></param>
//        /// <param name="pUpdateDateTime">Null date will return all records</param>
//        /// <returns>List of entries. Empty list if none found.</returns>
//        public static List<IBDObject> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
//        {
//            List<IBDObject> entryList = new List<IBDObject>();
//            IQueryable<BDNodeAssociation> entries;

//            if (null == pUpdateDateTime)
//            {
//                entries = (from entry in pContext.BDNodeAssociations
//                           select entry);
//            }
//            else
//            {
//                entries = pContext.BDNodeAssociations.Where(x => x.createdDate > pUpdateDateTime);
//            }
//            if (entries.Count() > 0)
//            {
//                entryList = new List<IBDObject>(entries.ToList<BDNodeAssociation>());
//            }
//            return entryList;
//        }

//        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
//        {
//            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
//            syncInfo.PushList = BDNodeAssociation.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
//            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
//            if (null != pCurrentSyncDate)
//            {
//                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
//                {
//                    ((BDNodeAssociation)syncInfo.PushList[idx]).createdDate = pCurrentSyncDate;
//                }
//                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
//            }
//            return syncInfo;
//        }

//        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
//        {
//            Guid dataUuid = Guid.Parse(pAttributeDictionary[UUID]);
//            Guid dataNodeId = Guid.Parse(pAttributeDictionary[NODEID]);
//            int dataChildNodeInt = (null == pAttributeDictionary[CHILDNODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[CHILDNODETYPE]);

//            BDConstants.BDNodeType dataChildNodeType = BDConstants.BDNodeType.None;
//            if (Enum.IsDefined(typeof(BDConstants.BDNodeType), dataChildNodeInt))
//            {
//                dataChildNodeType = (BDConstants.BDNodeType)dataChildNodeInt;
//            }

//            BDNodeAssociation entry = BDNodeAssociation.RetrieveForNodeIdAndChildType(pDataContext, dataNodeId, dataChildNodeType);
//            if (null == entry)
//            {
//                entry = BDNodeAssociation.CreateBDNodeAssociation(dataUuid);
//                pDataContext.AddObject(ENTITYNAME, entry);
//            }

//            entry.schemaVersion = (null == pAttributeDictionary[SCHEMAVERSION]) ? (short)0 : short.Parse(pAttributeDictionary[SCHEMAVERSION]); ;
//            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);

//            entry.nodeId = Guid.Parse(pAttributeDictionary[NODEID]);
//            entry.nodeType = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
//            entry.nodeKeyName = pAttributeDictionary[NODEKEYNAME];

//            entry.childNodeType = (null == pAttributeDictionary[CHILDNODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[CHILDNODETYPE]);
//            entry.childKeyName = pAttributeDictionary[CHILDKEYNAME];

//            if (pSaveChanges)
//                pDataContext.SaveChanges();

//            return dataUuid;
//        }

//        public PutAttributesRequest PutAttributes()
//        {
//            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
//            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;

//            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
//            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
//            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

//            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.NODEID).WithValue((null == nodeId) ? Guid.Empty.ToString() : nodeId.ToString().ToUpper()).WithReplace(true));
//            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.NODETYPE).WithValue(string.Format(@"{0}", nodeType)).WithReplace(true));
//            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.NODEKEYNAME).WithValue((null == nodeKeyName) ? string.Empty : nodeKeyName).WithReplace(true));

//            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.CHILDNODETYPE).WithValue(string.Format(@"{0}", childNodeType)).WithReplace(true));
//            attributeList.Add(new ReplaceableAttribute().WithName(BDNodeAssociation.CHILDKEYNAME).WithValue((null == childKeyName) ? string.Empty : childKeyName).WithReplace(true));

//            return putAttributeRequest;
//        }
//    }
//}
