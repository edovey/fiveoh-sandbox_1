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
    public partial class BDRegimen : IBDNode
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_regimens";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_regimens";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDRegimens";
        public const string ENTITYNAME_FRIENDLY = @"Regimen";
        public const string KEY_NAME = @"BDRegimen";

        public const int ENTITY_SCHEMAVERSION = 1;
        public const string PROPERTYNAME_NAME = @"Name";
        public const string PROPERTYNAME_DOSAGE = @"Dosage0";
        public const string PROPERTYNAME_DOSAGE_1 = @"Dosage1";
        // const string PROPERTYNAME_DOSAGE_2 = @"Dosage2";
        public const string PROPERTYNAME_DURATION = @"Duration0";
        public const string PROPERTYNAME_DURATION_1 = @"Duration1";
        //public const string PROPERTYNAME_DURATION_2 = @"Duration2";

        private const string UUID = @"re_uuid";
        private const string SCHEMAVERSION = @"re_schemaVersion";
        private const string CREATEDDATE = @"re_createdDate";
        private const string MODIFIEDDATE = @"re_modifiedDate";
        private const string DISPLAYORDER = @"re_displayOrder";
        private const string COLUMNORDER = @"re_columnOrder";

        private const string LAYOUTVARIANT = @"re_layoutVariant";
        private const string PARENTUUID = @"re_parentId";
        private const string PARENTTYPE = @"re_parentType";
        private const string PARENTKEYNAME = @"re_parentKeyName";

        private const string REGIMENJOINTYPE = @"re_regimenJoinType";
        private const string LEFTBRACKET = @"re_leftBracket";
        private const string RIGHTBRACKET = @"re_rightBracket";
        private const string NAME = @"re_name";
        private const string DOSAGE_0 = @"re_dosage0";
        private const string DOSAGE_1 = @"re_dosage1";
        private const string DURATION_0 = @"re_duration";
        private const string DURATION_1 = @"re_duration1";
        private const string NAMEPREVIOUS = @"re_nameSameAsPrevious";
        private const string DOSAGE_0_PREVIOUS = @"re_dosage0SameAsPrevious";
        private const string DOSAGE_1_PREVIOUS = @"re_dosage1SameAsPrevious";
        private const string DURATION_0_PREVIOUS = @"re_duration0SameAsPrevious";
        private const string DURATION_1_PREVIOUS = @"re_duration1SameAsPrevious";

        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <returns></returns>
        public static BDRegimen CreateBDRegimen(Entities pContext, Guid pParentId)
        {
            return CreateBDRegimen(pContext, pParentId, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <returns></returns>
        public static BDRegimen CreateBDRegimen(Entities pContext, Guid pParentId, Guid pUuid)
        {
            BDRegimen regimen = CreateBDRegimen(pUuid);
            regimen.createdDate = DateTime.Now;
            regimen.schemaVersion = ENTITY_SCHEMAVERSION;
            regimen.regimenJoinType = (int)BDConstants.BDJoinType.Next;
            regimen.leftBracket = false;
            regimen.rightBracket = false;
            regimen.displayOrder = -1;
            regimen.name = string.Empty;
            regimen.dosage0 = string.Empty;
            regimen.dosage1 = string.Empty;
            regimen.duration0 = string.Empty;
            regimen.duration1 = string.Empty;
            regimen.parentId = pParentId;
            regimen.nameSameAsPrevious = false;
            regimen.dosage0SameAsPrevious = false;
            regimen.dosage1SameAsPrevious = false;
            regimen.duration0SameAsPrevious = false;
            regimen.duration1SameAsPrevious = false;

            pContext.AddObject(ENTITYNAME, regimen);

            return regimen;
        }

        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pRegimen"></param>
        public static void Save(Entities pContext, BDRegimen pRegimen)
        {
            if (null != pRegimen)
            {
                if (pRegimen.EntityState != EntityState.Unchanged)
                {
                    if (pRegimen.schemaVersion != ENTITY_SCHEMAVERSION)
                        pRegimen.schemaVersion = ENTITY_SCHEMAVERSION;

                    System.Diagnostics.Debug.WriteLine(@"Regimen Save");
                    pContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Extended Delete method that deletes associated objects
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDRegimen pEntity, bool pCreateDeletion)
        {
            if (null == pEntity) return;

            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntity.Uuid);
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
            BDRegimen entity = BDRegimen.RetrieveBDRegimenWithUuid(pContext, pUuid);
            BDRegimen.Delete(pContext, entity, pCreateDeletion);
        }

        public static void DeleteForParentId(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            List<BDRegimen> children = BDRegimen.RetrieveBDRegimensForParentUuid(pContext, pUuid);
            foreach (BDRegimen child in children)
            {
                BDRegimen.Delete(pContext, child, pCreateDeletion);
            }
        }

        /// <summary>
        /// Gets all Therapies in the model with the specified Therapy Group ID
        /// </summary>
        /// <param name="pParentUuid"></param>
        /// <returns>List of Therapies</returns>
        public static List<BDRegimen> RetrieveBDRegimensForParentUuid(Entities pContext, Guid pParentUuid)
        {
            IQueryable<BDRegimen> regimens = (from entry in pContext.BDRegimens
                                               where entry.parentId == pParentUuid
                                               orderby entry.displayOrder ascending
                                               select entry);
            return regimens.ToList<BDRegimen>();
        }

        public static BDRegimen RetrieveBDRegimenWithUuid(Entities pContext, Guid pUuid)
        {
            BDRegimen regimen = null;

            if (null != pUuid)
            {
                IQueryable<BDRegimen> entries = (from bdRegimens in pContext.BDRegimens
                                                 where bdRegimens.uuid == pUuid
                                                 select bdRegimens);
                if (entries.Count<BDRegimen>() > 0)
                    regimen = entries.AsQueryable().First<BDRegimen>();
            }
            return regimen;
        }

        /// <summary>
        /// Get a string array of all the distinct Regimen Name values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] RetrieveBDRegimenNames(Entities pContext)
        {
            var regimenNames = pContext.BDRegimens.Where(x => (!string.IsNullOrEmpty(x.name))).Select(pg => pg.name).Distinct();

            return regimenNames.ToArray();
        }

        /// <summary>
        /// Get a string array of all the distinct Therapy Dose values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] RetrieveBDRegimenDosages(Entities pContext)
        {
            var dosages = pContext.BDRegimens.Where(x => (!string.IsNullOrEmpty(x.dosage0))).Select(pg => pg.dosage0).Distinct();
            var dosage1 = pContext.BDRegimens.Where(x => (!string.IsNullOrEmpty(x.dosage1))).Select(pg => pg.dosage1).Distinct();
            // return the results into a distinct array
            string[] dosageArray = dosage1.Distinct().ToArray();

            return dosageArray;
        }

        /// <summary>
        /// Get a string array of all the distinct Duration values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] RetrieveBDRegimenDurations(Entities pContext)
        {
            var durations = pContext.BDRegimens.Where(x => (!string.IsNullOrEmpty(x.duration0))).Select(pg => pg.duration0).Distinct();
            var duration1 = pContext.BDRegimens.Where(x => (!string.IsNullOrEmpty(x.duration1))).Select(pg => pg.duration1).Distinct();
            // return results into a distinct array
            string[] durationArray = duration1.Distinct().ToArray();
            return durationArray;
        }

        public static List<BDRegimen> RetrieveBDRegimensWithNameContainingString(Entities pContext, string pString)
        {
            List<BDRegimen> returnList = new List<BDRegimen>();
            if (null != pString && pString.Length > 0)
            {
                IQueryable<BDRegimen> entries = (from entry in pContext.BDRegimens
                                                 where entry.name.Contains(pString)
                                                 select entry);
                returnList = entries.ToList<BDRegimen>();
            }
            return returnList;
        }

        public static List<BDRegimen> RetrieveBDRegimensWithDosageContainingString(Entities pContext, string pString)
        {
            List<BDRegimen> returnList = new List<BDRegimen>();
            if (null != pString && pString.Length > 0)
            {
                IQueryable<BDRegimen> entries = (from entry in pContext.BDRegimens
                                                 where entry.dosage0.Contains(pString) || entry.dosage1.Contains(pString)
                                                 select entry);
                returnList = entries.ToList<BDRegimen>();
            }
            return returnList;
        }

        public static List<BDRegimen> RetrieveBDRegimensWithDurationContainingString(Entities pContext, string pString)
        {
            List<BDRegimen> returnList = new List<BDRegimen>();
            if (null != pString && pString.Length > 0)
            {
                IQueryable<BDRegimen> entries = (from entry in pContext.BDRegimens
                                                 where entry.duration0.Contains(pString) || entry.duration1.Contains(pString) 
                                                 select entry);
                returnList = entries.ToList<BDRegimen>();
            }
            return returnList;
        }

        /// <summary>
        /// Return the maximum value of the display order found in the children of the specified parent
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParent"></param>
        /// <returns></returns>
        public static int? RetrieveMaximumDisplayOrderForChildren(Entities pContext, BDRegimen pParent)
        {
            var maxDisplayorder = pContext.BDRegimens.Where(x => (x.parentId == pParent.Uuid)).Select(node => node.displayOrder).Max();
            return (null == maxDisplayorder) ? 0 : maxDisplayorder;
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
            get { return BDConstants.BDNodeType.BDRegimen; }
        }

        public string Description
        {
            get { return string.Format("[{0}][{1}][{2}]", this.name, this.dosage0, this.duration0); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("Regimen - {0} Dosage:{1} Duration:{2}", this.name, this.dosage0, this.duration0); }
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
                    case "createdBy":
                    case "createdDate":
                    case "modifiedBy":
                    case "modifiedDate":
                        break;
                    default:
                        {
                            modifiedDate = DateTime.Now;
                            //System.Diagnostics.Debug.WriteLine(string.Format("Therapy property change [{0}]", property));
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
            IQueryable<BDRegimen> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDRegimens
                           select entry);
            }
            else
            {
                entries = pContext.BDRegimens.Where(x => x.modifiedDate > pUpdateDateTime);
            }
            if (entries.Count() > 0)
            {
                entryList = new List<IBDObject>(entries.ToList<BDRegimen>());
            }
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDTherapy.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if ((null != pCurrentSyncDate) && (!BDCommon.Settings.RepositoryOverwriteEnabled))
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDRegimen)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDRegimen from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDRegimen entry = BDRegimen.RetrieveBDRegimenWithUuid(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDRegimen.CreateBDRegimen(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.columnOrder = (null == pAttributeDictionary[COLUMNORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[COLUMNORDER]);

            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTUUID]);
            entry.parentType = (null == pAttributeDictionary[PARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[PARENTTYPE]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

            entry.layoutVariant = short.Parse(pAttributeDictionary[LAYOUTVARIANT]);

            entry.regimenJoinType = int.Parse(pAttributeDictionary[REGIMENJOINTYPE]);
            entry.leftBracket = Boolean.Parse(pAttributeDictionary[LEFTBRACKET]);
            entry.rightBracket = Boolean.Parse(pAttributeDictionary[RIGHTBRACKET]);
            entry.name = pAttributeDictionary[NAME];
            entry.nameSameAsPrevious = Boolean.Parse(pAttributeDictionary[NAMEPREVIOUS]);
            entry.dosage0 = pAttributeDictionary[DOSAGE_0];
            entry.duration0 = pAttributeDictionary[DURATION_0];
            entry.dosage0SameAsPrevious = Boolean.Parse(pAttributeDictionary[DOSAGE_0_PREVIOUS]);
            entry.duration0SameAsPrevious = Boolean.Parse(pAttributeDictionary[DURATION_0_PREVIOUS]);

            if (entry.schemaVersion > 0)
            {
                entry.dosage1 = pAttributeDictionary[DOSAGE_1];
                entry.duration1 = pAttributeDictionary[DURATION_1];

                entry.dosage1SameAsPrevious = Boolean.Parse(pAttributeDictionary[DOSAGE_1_PREVIOUS]);
                entry.duration1SameAsPrevious = Boolean.Parse(pAttributeDictionary[DURATION_1_PREVIOUS]);
            }


            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DISPLAYORDER).WithValue(displayOrder.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.REGIMENJOINTYPE).WithValue(string.Format(@"{0}", regimenJoinType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.LEFTBRACKET).WithValue(leftBracket.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.RIGHTBRACKET).WithValue(rightBracket.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DOSAGE_0).WithValue((null == dosage0) ? string.Empty : dosage0).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DOSAGE_1).WithValue((null == dosage1) ? string.Empty : dosage1).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DURATION_0).WithValue((null == duration0) ? string.Empty : duration0).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DURATION_1).WithValue((null == duration1) ? string.Empty : duration1).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.NAMEPREVIOUS).WithValue(nameSameAsPrevious.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DOSAGE_0_PREVIOUS).WithValue(dosage0SameAsPrevious.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DOSAGE_1_PREVIOUS).WithValue(dosage1SameAsPrevious.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DURATION_0_PREVIOUS).WithValue(duration0SameAsPrevious.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.DURATION_1_PREVIOUS).WithValue(duration1SameAsPrevious.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.PARENTUUID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimen.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

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
