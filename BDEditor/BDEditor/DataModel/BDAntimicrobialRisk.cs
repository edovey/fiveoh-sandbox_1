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
    /// Extension of generated BDAntimicrobialRisk
    /// </summary>
   public partial class BDAntimicrobialRisk : IBDNode
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_antimicrobialrisks";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_antimicrobialrisks";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDAntimicrobialRisks";
        public const string ENTITYNAME_FRIENDLY = @"AntimicrobialRisk";
        public const string KEY_NAME = @"BDAntimicrobialRisk";
        public const string PROPERTYNAME_NAME = @"Name";
        public const string PROPERTYNAME_PREGNANCYRISK = @"Risk Factor";
        public const string PROPERTYNAME_LACTATIONRISK = @"Risk Factor";
        public const string PROPERTYNAME_RECOMMENDATION = @"Recommendation";
        public const string PROPERTYNAME_APPRATING = @"AAP Rating";
        public const string PROPERTYNAME_RELATIVEDOSE = @"Relative Infant Dose";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"ar_uuid";
        private const string SCHEMAVERSION = @"ar_schemaVersion";
        private const string MODIFIEDDATE = @"ar_modifiedDate";
        private const string NAME = @"ar_name";
        private const string DISPLAYORDER = @"ar_displayOrder";
        private const string LAYOUTVARIANT = @"ar_layoutVariant";

        private const string PARENTID = @"ar_parentId";
        private const string PARENTTYPE = @"ar_parentType";
        private const string PARENTKEYNAME = @"ar_parentKeyName";

        private const string NODETYPE = @"ar_nodeType";
        private const string NODEKEYNAME = @"ar_nodeKeyName";

        private const string RISKFACTOR = @"ar_riskFactor";
        private const string RECOMMENDATIONS = @"ar_recommendations";
        private const string AAPRATING = @"ar_aapRating";
        private const string RELATIVEINFANTDOSE = @"ar_relativeInfantDose";

        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDAntimicrobialRisk CreateBDAntimicrobialRisk(Entities pContext, BDConstants.BDNodeType pNodeType)
        {
            return CreateBDAntimicrobialRisk(pContext, pNodeType, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets default values and schema version
        /// </summary>
        /// <returns></returns>
        public static BDAntimicrobialRisk CreateBDAntimicrobialRisk(Entities pContext, BDConstants.BDNodeType pNodeType, Guid pUuid)
        {
            BDAntimicrobialRisk node = CreateBDAntimicrobialRisk(pUuid);
            node.nodeType = (int)pNodeType;
            node.nodeKeyName = pNodeType.ToString();
            node.schemaVersion = ENTITY_SCHEMAVERSION;
            node.displayOrder = -1;
            node.name = string.Empty;
            node.parentType = -1;
            node.parentKeyName = string.Empty;
            node.nodeKeyName = string.Empty;
            node.layoutVariant = -1;
            node.riskFactor = string.Empty;
            node.recommendations = string.Empty;
            node.aapRating = string.Empty;
            node.relativeInfantDose = string.Empty;

            pContext.AddObject(ENTITYNAME, node);

            return node;
        }

        /// <summary>
        /// Extended Save method 
        /// </summary>
        /// <param name="pNode"></param>
        public static void Save(Entities pContext, BDAntimicrobialRisk pNode)
        {
            if (null != pNode)
            {
                if (pNode.EntityState != EntityState.Unchanged)
                {
                    if (pNode.schemaVersion != ENTITY_SCHEMAVERSION)
                        pNode.schemaVersion = ENTITY_SCHEMAVERSION;

                    System.Diagnostics.Debug.WriteLine(@"AntimicrobialRisk Save");
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
            if (null == pNode) return;

            BDLinkedNoteAssociation.DeleteForParentId(pContext, pNode.Uuid, pCreateDeletion);

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
                    default:
                        BDNode.Delete(pContext, child, pCreateDeletion);
                        break;
                }
            }

            BDMetadata.DeleteForItemId(pContext, pNode.Uuid, pCreateDeletion);
            // create BDDeletion record for the object to be deleted
            if (pCreateDeletion)
                BDDeletion.CreateBDDeletion(pContext, KEY_NAME, pNode.Uuid);
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

        /// <summary>
        /// Delete from the local datastore without creating a deletion record nor deleting any children. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid"></param>
        public static void DeleteLocal(Entities pContext, Guid? pUuid)
        {
            if (null != pUuid)
            {
                BDAntimicrobialRisk entry = BDAntimicrobialRisk.RetrieveAntimicrobialRiskWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Retrieve BDAntimicrobialRisk with the specified ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>BDAntimicrobialRisk object.</returns>
        public static BDAntimicrobialRisk RetrieveAntimicrobialRiskWithId(Entities pContext, Guid pUuid)
        {
            BDAntimicrobialRisk entry = null;

            if (null != pUuid)
            {
                IQueryable<BDAntimicrobialRisk> entries = (from risk in pContext.BDAntimicrobialRisks
                                              where risk.uuid == pUuid
                                              select risk);
                if (entries.Count<BDAntimicrobialRisk>() > 0)
                    entry = entries.AsQueryable().First<BDAntimicrobialRisk>();
            }
            return entry;
        }

        protected override void OnPropertyChanged(string property)
        {
            if (!BDCommon.Settings.IsSyncLoad)
                switch (property)
                {
                    case "modifiedDate":
                        break;
                    default:
                        {
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
            IQueryable<BDAntimicrobialRisk> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDAntimicrobialRisks
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDAntimicrobialRisks
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDAntimicrobialRisk>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDAntimicrobialRisk.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if (null != pCurrentSyncDate)
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDAntimicrobialRisk)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDAntimicrobialRisk from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid dataUuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDAntimicrobialRisk entry = BDAntimicrobialRisk.RetrieveAntimicrobialRiskWithId(pDataContext, dataUuid);

            if (null == entry)
            {
                int nt = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
                BDConstants.BDNodeType nodeType = BDConstants.BDNodeType.None;

                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), nt))
                {
                    nodeType = (BDConstants.BDNodeType)nt;
                }

                entry = BDAntimicrobialRisk.CreateBDAntimicrobialRisk(dataUuid);
                entry.nodeType = nt;

                pDataContext.AddObject(ENTITYNAME, entry);
            }

            entry.nodeType = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
            entry.schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.name = pAttributeDictionary[NAME];

            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentType = (null == pAttributeDictionary[PARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[PARENTTYPE]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

            entry.nodeKeyName = pAttributeDictionary[NODEKEYNAME];

            entry.layoutVariant = short.Parse(pAttributeDictionary[LAYOUTVARIANT]);

            entry.riskFactor = pAttributeDictionary[RISKFACTOR];
            entry.recommendations = pAttributeDictionary[RECOMMENDATIONS];
            entry.aapRating = pAttributeDictionary[AAPRATING];
            entry.relativeInfantDose = pAttributeDictionary[RELATIVEINFANTDOSE];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return dataUuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.NODETYPE).WithValue(string.Format(@"{0}", nodeType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.NODEKEYNAME).WithValue((null == nodeKeyName) ? string.Empty : nodeKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.RISKFACTOR).WithValue((null == riskFactor) ? string.Empty : riskFactor).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.RECOMMENDATIONS).WithValue((null == recommendations) ? string.Empty : recommendations).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.AAPRATING).WithValue((null == aapRating) ? string.Empty : aapRating).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAntimicrobialRisk.RELATIVEINFANTDOSE).WithValue((null == relativeInfantDose) ? string.Empty : relativeInfantDose).WithReplace(true));

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
