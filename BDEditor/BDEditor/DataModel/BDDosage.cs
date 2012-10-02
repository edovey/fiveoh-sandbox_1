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
    /// Extension of generated BDDosage
    /// </summary>
   public partial class BDDosage : IBDNode
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_dosages";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_dosages";

        public const string AWS_PROD_BUCKET = @"bdProdStore";
        public const string AWS_DEV_BUCKET = @"bdDevStore";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
        public const string AWS_BUCKET = AWS_DEV_BUCKET;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
        public const string AWS_BUCKET = AWS_PROD_BUCKET;
#endif

        public const string ENTITYNAME = @"BDDosages";
        public const string ENTITYNAME_FRIENDLY = @"Dosages";
        public const string KEY_NAME = @"BDDosages";

        public const int ENTITY_SCHEMAVERSION = 0;

        public const string PROPERTYNAME_DOSAGE = @"Dosage";
        public const string PROPERTYNAME_COST = @"Cost";
        public const string PROPERTYNAME_NAME = @"Name";
        public const string PROPERTYNAME_DOSAGE2 = @"Dosage 2";
        public const string PROPERTYNAME_DOSAGE3 = @"Dosage 3";
        public const string PROPERTYNAME_DOSAGE4 = @"Dosage 4";
        public const string PROPERTYNAME_COST2 = @"Cost 2";

       private const string UUID = @"do_uuid";
        private const string SCHEMAVERSION = @"do_schemaVersion";
        private const string MODIFIEDDATE = @"do_modifiedDate";
        private const string DISPLAYORDER = @"do_displayOrder";
        private const string LAYOUTVARIANT = @"do_layoutVariant";
        private const string PARENTID = @"do_parentId";
        private const string PARENTKEYNAME = @"do_parentKeyName";
        private const string PARENTTYPE = @"do_parentType";
        private const string NAME = @"do_name";
        private const string COST = @"do_cost";
        private const string DOSAGE = @"do_dosage";
        private const string DOSAGEJOINTYPE = @"do_joinType";
        
        public Guid? tempProductionUuid { get; set; }

        public enum DosageJoinType
        {
            None = 0,
            AndWithNext = 1,
            OrWithNext = 2,
            ThenWithNext = 3,
            WithOrWithoutWithNext = 4,
        }

        /// <summary>
        /// Extended Create method that sets the schema version
        /// </summary>
        /// <returns>BDDosage</returns>
        public static BDDosage CreateBDDosage(Entities pContext, Guid pParentId)
        {
            return CreateBDDosage(pContext, pParentId, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the schema version, and initializes variables
        /// </summary>
        /// <returns>BDPrecaution</returns>
        public static BDDosage CreateBDDosage(Entities pContext, Guid pParentId, Guid pUuid)
        {
            BDDosage entity = CreateBDDosage(pUuid);
            entity.schemaVersion = ENTITY_SCHEMAVERSION;
            entity.displayOrder = -1;
            entity.name = string.Empty;
            entity.dosage = string.Empty;
            entity.dosage2 = string.Empty;
            entity.dosage3 = string.Empty;
            entity.dosage4 = string.Empty;
            entity.cost = string.Empty;
            entity.cost2 = string.Empty;
            entity.joinType = 0;
            entity.layoutVariant = -1;
            entity.parentId = pParentId;

            pContext.AddObject(ENTITYNAME, entity);

            return entity;
        }

        /// <summary>
        /// Extended Save that sets the modified date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pEntity"></param>
        public static void Save(Entities pContext, BDDosage pEntity)
        {
            if (null != pEntity)
            {
                if (pEntity.EntityState != EntityState.Unchanged)
                {
                    if (pEntity.schemaVersion != ENTITY_SCHEMAVERSION)
                        pEntity.schemaVersion = ENTITY_SCHEMAVERSION;
                    pContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDDosage pEntity, bool pCreateDeletion)
        {
            if (null == pEntity) return;
            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntity.uuid, pCreateDeletion);
            BDMetadata.DeleteForItemId(pContext, pEntity.uuid, pCreateDeletion);
            // create BDDeletion record for the object to be deleted
            if (pCreateDeletion)
                BDDeletion.CreateBDDeletion(pContext, KEY_NAME, pEntity);

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
            BDDosage entity = BDDosage.RetrieveDosageWithId(pContext, pUuid);
            BDDosage.Delete(pContext, entity, pCreateDeletion);
        }

        public static void DeleteForParent(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            List<BDDosage> children = BDDosage.RetrieveDosagesForParentId(pContext, pUuid);
            foreach (BDDosage child in children)
            {
                BDDosage.Delete(pContext, child, pCreateDeletion);
            }
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
                BDDosage entry = BDDosage.RetrieveDosageWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Gets all Precautions in the model with the specified parent ID
        /// </summary>
        /// <param name="pParentId"></param>
        /// <returns>List of Precautions</returns>
        public static List<BDDosage> RetrieveDosagesForParentId(Entities pContext, Guid pParentId)
        {
            IQueryable<BDDosage> entries = (from entry in pContext.BDDosages
                                                where entry.parentId == pParentId
                                                orderby entry.displayOrder ascending
                                                select entry);
            return entries.ToList<BDDosage>();
        }

        public static BDDosage RetrieveDosageWithId(Entities pContext, Guid pEntityId)
        {
            BDDosage entity = null;

            if (null != pEntityId)
            {
                IQueryable<BDDosage> entries = (from entry in pContext.BDDosages
                                                    where entry.uuid == pEntityId
                                                    select entry);
                if (entries.Count<BDDosage>() > 0)
                    entity = entries.AsQueryable().First<BDDosage>();
            }
            return entity;
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
            get { return BDConstants.BDNodeType.BDDosage; }
        }

        public string Description
        {
            get { return string.Format(@"Dosage for {0}}", this.dosage); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format(@"Dosage for {0}", this.dosage); }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
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
            IQueryable<BDDosage> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDDosages
                           select entry);
            }
            else
            {
                entries = pContext.BDDosages.Where(x => x.modifiedDate > pUpdateDateTime);
            }
            if (entries.Count() > 0)
            {
                entryList = new List<IBDObject>(entries.ToList<BDDosage>());
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
        /// Create or update an existing BDDosage from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDDosage entry = BDDosage.RetrieveDosageWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDDosage.CreateBDDosage(uuid);
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

            entry.name = pAttributeDictionary[NAME];
            entry.dosage = pAttributeDictionary[DOSAGE];
            entry.cost = pAttributeDictionary[COST];
            entry.joinType = int.Parse(pAttributeDictionary[DOSAGEJOINTYPE]);

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.DISPLAYORDER).WithValue(displayOrder.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.DOSAGE).WithValue((null == dosage) ? string.Empty : dosage).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.COST).WithValue((null == cost) ? string.Empty : cost).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDDosage.DOSAGEJOINTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            
            return putAttributeRequest;
        }
        #endregion

        public override string ToString()
        {
            return this.name;
        }


        public int? DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }
    }
}
