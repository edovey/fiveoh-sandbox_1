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
    /// Extension of generated BDRegimenGroup
    /// </summary>
    public partial class BDRegimenGroup : IBDNode
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_regimenGroups";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_regimenGroups";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDRegimenGroups";
        public const string ENTITYNAME_FRIENDLY = @"Regimen Group";
        public const string KEY_NAME = @"BDRegimenGroup";

        public const int ENTITY_SCHEMAVERSION = 0;
        public const string PROPERTYNAME_NAME = @"Name";

        private const string UUID = @"rg_uuid";
        private const string SCHEMAVERSION = @"rg_schemaVersion";
        private const string CREATEDDATE = @"rg_createdDate";
        private const string MODIFIEDDATE = @"rg_modifiedDate";
        private const string DISPLAYORDER = @"rg_displayOrder";
        private const string REGIMENGROUPJOINTYPE = @"rg_regimenGroupJoinType";
        private const string NAME = @"rg_name";
        private const string REGIMENOFCHOICE = @"rg_regimenOfChoice";
        private const string ALTERNATIVEREGIMEN = @"rg_alternativeRegimen";

        private const string PARENTID = @"rg_parentId";
        private const string PARENTTYPE = @"rg_parentType";
        private const string PARENTKEYNAME = @"rg_parentKeyName";
        private const string LAYOUTVARIANT = @"rg_layoutVariant";

        public enum RegimenGroupJoinType
        {
            None = 0,
            AndWithNext = 1,
            OrWithNext = 2
        }

        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDRegimenGroup CreateBDRegimenGroup(Entities pContext, Guid pParentId)
        {
            return CreateBDRegimenGroup(pContext, pParentId, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDRegimenGroup CreateBDRegimenGroup(Entities pContext, Guid pParentId, Guid pUuid)
        {
            BDRegimenGroup regimenGroup = CreateBDRegimenGroup(pUuid);
            regimenGroup.createdDate = DateTime.Now;
            regimenGroup.schemaVersion = ENTITY_SCHEMAVERSION;
            regimenGroup.regimenGroupJoinType = (int)RegimenGroupJoinType.None;
            regimenGroup.displayOrder = -1;
            regimenGroup.name = string.Empty;
            regimenGroup.parentId = pParentId;
            regimenGroup.regimenOfChoice = false;
            regimenGroup.alternativeRegimen = false;

            pContext.AddObject(ENTITYNAME, regimenGroup);

            return regimenGroup;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pRegimenGroup"></param>
        public static void Save(Entities pContext, BDRegimenGroup pRegimenGroup)
        {
            if (null != pRegimenGroup)
            {
                if (pRegimenGroup.EntityState != EntityState.Unchanged)
                {
                    if (pRegimenGroup.schemaVersion != ENTITY_SCHEMAVERSION)
                        pRegimenGroup.schemaVersion = ENTITY_SCHEMAVERSION;

                    System.Diagnostics.Debug.WriteLine(@"RegimenGroup Save");
                    pContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Extended Delete method that associated objects as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDRegimenGroup pEntity, bool pCreateDeletion)
        {
            if (null == pEntity) return;

            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntity.Uuid);
            BDSearchEntryAssociation.DeleteForAnchorNodeUuid(pContext, pEntity.Uuid);

            BDRegimen.DeleteForParentId(pContext, pEntity.Uuid, pCreateDeletion);

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
            BDRegimenGroup entity = BDRegimenGroup.RetrieveRegimenGroupWithUuid(pContext, pUuid);
            BDRegimenGroup.Delete(pContext, entity, pCreateDeletion);
        }

        /// <summary>
        /// Gets all Regimen Groups in the model with the specified Regimen ID
        /// </summary>
        /// <param name="pPathogenId"></param>
        /// <returns>List of BDRegimenGroups</returns>
        public static List<BDRegimenGroup> RetrieveRegimenGroupsForParentUuid(Entities pContext, Guid pParentUuid)
        {
            IQueryable<BDRegimenGroup> regimenGroups = (from entry in pContext.BDRegimenGroups
                                                        where entry.parentId == pParentUuid
                                                        orderby entry.displayOrder ascending
                                                        select entry);
            return regimenGroups.ToList<BDRegimenGroup>();
        }

        public static BDRegimenGroup RetrieveRegimenGroupWithUuid(Entities pContext, Guid pUuid)
        {
            BDRegimenGroup entry = null;
            if (null != pUuid)
            {
                IQueryable<BDRegimenGroup> entryList = (from bdRegimenGroups in pContext.BDRegimenGroups
                                                        where bdRegimenGroups.uuid == pUuid
                                                        select bdRegimenGroups);

                if (entryList.Count<BDRegimenGroup>() > 0)
                    entry = entryList.AsQueryable().First<BDRegimenGroup>();
            }
            return entry;
        }

        /// <summary>
        /// Get a string array of all the Regimen Group name values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] RetrieveRegimenGroupNames(Entities pContext)
        {
            var regimenGroupNames = pContext.BDRegimenGroups.Where(x => (!string.IsNullOrEmpty(x.name))).Select(pg => pg.name).Distinct();

            return regimenGroupNames.ToArray();
        }

        public static List<BDRegimenGroup> RetrieveRegimenGroupsWithNameContainingString(Entities pContext, string pString)
        {
            List<BDRegimenGroup> returnList = new List<BDRegimenGroup>();
            if (null != pString && pString.Length > 0)
            {
                IQueryable<BDRegimenGroup> entries = (from entry in pContext.BDRegimenGroups
                                                      where entry.name.Contains(pString)
                                                      select entry);
                returnList = entries.ToList<BDRegimenGroup>();
            }
            return returnList;
        }

        /// <summary>
        /// Return the maximum value of the display order found in the children of the specified parent
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParent"></param>
        /// <returns></returns>
        public static int? RetrieveMaximumDisplayOrderForChildren(Entities pContext, BDRegimenGroup pParent)
        {
            var maxDisplayorder = pContext.BDRegimenGroups.Where(x => (x.parentId == pParent.Uuid)).Select(node => node.displayOrder).Max();
            return (null == maxDisplayorder) ? 0 : maxDisplayorder;
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
                            //System.Diagnostics.Debug.WriteLine(string.Format("TherapyGroup property change [{0}]", property));
                        }
                        break;
                }

            base.OnPropertyChanged(property);
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
            get { return BDConstants.BDNodeType.BDRegimenGroup; }
        }

        public string Description
        {
            get { return string.Format("[{0}]", this.name); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("Regimen Group - {0}", this.name); }
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

        public int? DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
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

        public override string ToString()
        {
            return this.name;
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
            IQueryable<BDRegimenGroup> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDRegimenGroups
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDRegimenGroups
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }
            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDRegimenGroup>());
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDRegimenGroup.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if ((null != pCurrentSyncDate) && (!BDCommon.Settings.RepositoryOverwriteEnabled))
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDRegimenGroup)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDRegimenGroup from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            //bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDRegimenGroup entry = BDRegimenGroup.RetrieveRegimenGroupWithUuid(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDRegimenGroup.CreateBDRegimenGroup(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]); ;
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.regimenGroupJoinType = int.Parse(pAttributeDictionary[REGIMENGROUPJOINTYPE]);
            entry.name = pAttributeDictionary[NAME];
            entry.regimenOfChoice = bool.Parse(pAttributeDictionary[REGIMENOFCHOICE]);
            entry.alternativeRegimen = bool.Parse(pAttributeDictionary[ALTERNATIVEREGIMEN]);

            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentType = (null == pAttributeDictionary[PARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[PARENTTYPE]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

            entry.layoutVariant = short.Parse(pAttributeDictionary[LAYOUTVARIANT]);

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.REGIMENGROUPJOINTYPE).WithValue(regimenGroupJoinType.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.REGIMENOFCHOICE).WithValue(regimenOfChoice.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.ALTERNATIVEREGIMEN).WithValue(alternativeRegimen.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDRegimenGroup.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion




    }
}
