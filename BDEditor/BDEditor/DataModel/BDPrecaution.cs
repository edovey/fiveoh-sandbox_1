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
    /// <summary>
    /// Extension of generated BDPrecaution
    /// </summary>
    public partial class BDPrecaution : IBDNode
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_precautions";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_precautions";

        public const string AWS_PROD_BUCKET = @"bdProdStore";
        public const string AWS_DEV_BUCKET = @"bdDevStore";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
        public const string AWS_BUCKET = AWS_DEV_BUCKET;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
        public const string AWS_BUCKET = AWS_PROD_BUCKET;
#endif

        public const string ENTITYNAME = @"BDPrecautions";
        public const string ENTITYNAME_FRIENDLY = @"Precautions";
        public const string KEY_NAME = @"BDPrecautions";

        public const int ENTITY_SCHEMAVERSION = 0;

        public const string PROPERTYNAME_ORGANISM_1 = @"Organism1";
        public const string PROPERTYNAME_ORGANISM_2 = @"Organism2";
        public const string PROPERTYNAME_INFECTIVEMATERIAL = @"InfectiveMaterial";
        public const string PROPERTYNAME_MODEOFTRANSMISSION = @"ModeOfTransmission";
        public const string PROPERTYNAME_SINGLEROOMACUTE = @"SingleRoomAcute";
        public const string PROPERTYNAME_SINGLEROOMLONGTERM = @"SingleRoomLongTerm";
        public const string PROPERTYNAME_GLOVESACUTE = @"GlovesAcute";
        public const string PROPERTYNAME_GLOVESLONGTERM = @"GlovesLongTerm";
        public const string PROPERTYNAME_GOWNSACUTE = @"GownsAcute";
        public const string PROPERTYNAME_GOWNSLONGTERM = @"GownsLongTerm";
        public const string PROPERTYNAME_MASKACUTE = @"MaskAcute";
        public const string PROPERTYNAME_MASKLONGTERM = @"MaskLongTerm";
        public const string PROPERTYNAME_DURATION = @"Duration";

        public const string VIRTUALPROPERTYNAME_PRECAUTIONS = @"Precautions";

        private const string UUID = @"pr_uuid";
        private const string SCHEMAVERSION = @"pr_schemaVersion";
        private const string MODIFIEDDATE = @"pr_modifiedDate";
        private const string DISPLAYORDER = @"pr_displayOrder";
        private const string LAYOUTVARIANT = @"pr_layoutVariant";
        private const string PARENTID = @"pr_parentId";
        private const string PARENTKEYNAME = @"pr_parentKeyName";
        private const string PARENTTYPE = @"pr_parentType";
        private const string ORGANISM_1 = @"pr_organism1";
        private const string ORGANISM_2 = @"pr_organism2";
        private const string INFECTIVEMATERIAL = @"pr_infectiveMaterial";
        private const string MODEOFTRANSMISSION = @"pr_modeOfTransmission";
        private const string SINGLEROOMACUTE = @"pr_singleRoomAcute";
        private const string SINGLEROOMLONGTERM = @"pr_singleRoomLongTerm";
        private const string GLOVESACUTE = @"pr_glovesAcute";
        private const string GLOVESLONGTERM = @"pr_glovesLongTerm";
        private const string GOWNSACUTE = @"pr_gownsAcute";
        private const string GOWNSLONGTERM = @"pr_gownsLongTerm";
        private const string MASKACUTE = @"pr_maskAcute";
        private const string MASKLONGTERM = @"pr_maskLongTerm";
        private const string DURATION = @"pr_duration";



        public Guid? tempProductionUuid { get; set; }

        /// <summary>
        /// Extended Create method that sets the schema version
        /// </summary>
        /// <returns>BDPrecaution</returns>
        public static BDPrecaution CreateBDPrecaution(Entities pContext, Guid pParentId)
        {
            return CreateBDPrecaution(pContext, pParentId, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets created date and schema version, and initializes variables
        /// </summary>
        /// <returns>BDPrecaution</returns>
        public static BDPrecaution CreateBDPrecaution(Entities pContext, Guid pParentId, Guid pUuid)
        {
            BDPrecaution entity = CreateBDPrecaution(pUuid);
            entity.schemaVersion = ENTITY_SCHEMAVERSION;
            entity.displayOrder = -1;
            entity.organism1 = string.Empty;
            entity.organism2 = string.Empty;
            entity.infectiveMaterial = string.Empty;
            entity.modeOfTransmission = string.Empty;
            entity.singleRoomAcute = string.Empty;
            entity.singleRoomLongTerm = string.Empty;
            entity.glovesAcute = string.Empty;
            entity.glovesLongTerm = string.Empty;
            entity.gownsAcute = string.Empty;
            entity.gownsLongTerm = string.Empty;
            entity.maskAcute = string.Empty;
            entity.maskLongTerm = string.Empty;
            entity.duration = string.Empty;
            entity.parentId = pParentId;

            pContext.AddObject(ENTITYNAME, entity);

            return entity;
        }

        /// <summary>
        /// Extended Save that sets the modified date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPrecaution"></param>
        public static void Save(Entities pContext, BDPrecaution pPrecaution)
        {
            if (null != pPrecaution)
            {
                if (pPrecaution.EntityState != EntityState.Unchanged)
                {
                    if (pPrecaution.schemaVersion != ENTITY_SCHEMAVERSION)
                        pPrecaution.schemaVersion = ENTITY_SCHEMAVERSION;
                    pContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDPrecaution pEntity, bool pCreateDeletion)
        {
            if (null == pEntity) return;
            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntity.uuid, pCreateDeletion);
            BDSearchEntryAssociation.DeleteForAnchorNodeUuid(pContext, pEntity.Uuid);

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
            BDPrecaution entity = BDPrecaution.RetrievePrecautionWithId(pContext, pUuid);
            BDPrecaution.Delete(pContext, entity, pCreateDeletion);
        }

        public static void DeleteForParent(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            List<BDPrecaution> children = BDPrecaution.RetrievePrecautionsForParentId(pContext, pUuid);
            foreach (BDPrecaution child in children)
            {
                BDPrecaution.Delete(pContext, child, pCreateDeletion);
            }
        }

        /// <summary>
        /// Gets all Precautions in the model with the specified parent ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>List of Precautions</returns>
        public static List<BDPrecaution> RetrievePrecautionsForParentId(Entities pContext, Guid pParentId)
        {
            List<BDPrecaution> entryList = new List<BDPrecaution>();

            IQueryable<BDPrecaution> entries = (from entry in pContext.BDPrecautions
                                               where entry.parentId == pParentId
                                               orderby entry.displayOrder ascending
                                               select entry);
            return entries.ToList<BDPrecaution>();
        }

        public static BDPrecaution RetrievePrecautionWithId(Entities pContext, Guid pEntityId)
        {
            BDPrecaution entity = null;

            if (null != pEntityId)
            {
                IQueryable<BDPrecaution> entries = (from entry in pContext.BDPrecautions
                                                 where entry.uuid == pEntityId
                                                 select entry);
                if (entries.Count<BDPrecaution>() > 0)
                    entity = entries.AsQueryable().First<BDPrecaution>();
            }
            return entity;
        }

        public static List<BDPrecaution> RetrievePrecautionsContainingString(Entities pContext, string pString)
        {
            List<BDPrecaution> returnList = new List<BDPrecaution>();
            if (null != pString && pString.Length > 0)
            {
                IQueryable<BDPrecaution> entries = (from entry in pContext.BDPrecautions
                                                 where entry.organism1.Contains(pString) || entry.organism2.Contains(pString) || 
                                                 entry.infectiveMaterial.Contains(pString) || entry.modeOfTransmission.Contains(pString) ||
                                                 entry.duration.Contains(pString)
                                                 select entry);
                returnList = entries.ToList<BDPrecaution>();
            }
            return returnList;
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

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public BDConstants.BDNodeType NodeType
        {
            get { return BDConstants.BDNodeType.BDPrecaution; }
        }

        public string Description
        {
            get { return string.Format(@"Precautions for {0}:{1}", this.organism1, this.organism2); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format(@"Precautions for {0}:{1}", this.organism1, this.organism2); }
        }

       public string Name
        {
            get { return this.organism1; }
            set { this.organism1 = value; }
        }

        public Guid? ParentId
        {
            get { return parentId; }
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
            IQueryable<BDPrecaution> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDPrecautions
                           select entry);
            }
            else
            {
                entries = pContext.BDPrecautions.Where(x => x.modifiedDate > pUpdateDateTime);
            }
            if (entries.Count() > 0)
            {
                entryList = new List<IBDObject>(entries.ToList<BDPrecaution>());
            }
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDPrecaution.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if ((null != pCurrentSyncDate) && (!BDCommon.Settings.RepositoryOverwriteEnabled))
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDPrecaution)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDPrecaution from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDPrecaution entry = BDPrecaution.RetrievePrecautionWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDPrecaution.CreateBDPrecaution(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);

            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentType = (null == pAttributeDictionary[PARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[PARENTTYPE]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

            entry.layoutVariant = short.Parse(pAttributeDictionary[LAYOUTVARIANT]);

            entry.organism1 = pAttributeDictionary[ORGANISM_1];
            entry.organism2 = pAttributeDictionary[ORGANISM_2];
            entry.infectiveMaterial = pAttributeDictionary[INFECTIVEMATERIAL];
            entry.modeOfTransmission = pAttributeDictionary[MODEOFTRANSMISSION];
            entry.singleRoomAcute = pAttributeDictionary[SINGLEROOMACUTE];
            entry.singleRoomLongTerm = pAttributeDictionary[SINGLEROOMLONGTERM];
            entry.glovesAcute = pAttributeDictionary[GLOVESACUTE];
            entry.glovesLongTerm = pAttributeDictionary[GLOVESLONGTERM];
            entry.gownsAcute = pAttributeDictionary[GOWNSACUTE];
            entry.gownsLongTerm = pAttributeDictionary[GOWNSLONGTERM];
            entry.maskAcute = pAttributeDictionary[MASKACUTE];
            entry.maskLongTerm = pAttributeDictionary[MASKLONGTERM];
            entry.duration = pAttributeDictionary[DURATION];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.DISPLAYORDER).WithValue(displayOrder.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.ORGANISM_1).WithValue((null == organism1) ? string.Empty : organism1).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.ORGANISM_2).WithValue((null == organism2) ? string.Empty : organism2).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.INFECTIVEMATERIAL).WithValue((null == infectiveMaterial) ? string.Empty : infectiveMaterial).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.MODEOFTRANSMISSION).WithValue((null == modeOfTransmission) ? string.Empty : modeOfTransmission).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.SINGLEROOMACUTE).WithValue((null == singleRoomAcute) ? string.Empty : singleRoomAcute).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.SINGLEROOMLONGTERM).WithValue((null == singleRoomLongTerm) ? string.Empty : singleRoomLongTerm).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.GLOVESACUTE).WithValue((null == glovesAcute) ? string.Empty : glovesAcute).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.GLOVESLONGTERM).WithValue((null == glovesLongTerm) ? string.Empty : glovesLongTerm).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.GOWNSACUTE).WithValue((null == gownsAcute) ? string.Empty : gownsAcute).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.GOWNSLONGTERM).WithValue((null == gownsLongTerm) ? string.Empty : gownsLongTerm).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.MASKACUTE).WithValue((null == maskAcute) ? string.Empty : maskAcute).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.MASKLONGTERM).WithValue((null == maskLongTerm) ? string.Empty : maskLongTerm).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPrecaution.DURATION).WithValue((null == duration) ? string.Empty : duration).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

        public override string ToString()
        {
            return this.organism1;
        }


        public int? DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }
    }
}
