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
    /// Extension of generated BDTherapyGroup
    /// </summary>
    public partial class BDTherapyGroup : IBDNode
    {
        //public const string AWS_DOMAIN = @"bd_1_therapyGroups";

        public const string AWS_PROD_DOMAIN = @"bd_2_therapyGroups";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_therapyGroups";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDTherapyGroups";
        public const string ENTITYNAME_FRIENDLY = @"Therapy Group";
        public const string KEY_NAME = @"BDTherapyGroup";

        public const int ENTITY_SCHEMAVERSION = 0;
        //public const string PROPERTYNAME_DEFAULT = "TherapyGroup";
        public const string PROPERTYNAME_NAME = @"Name";

        private const string UUID = @"tg_uuid";
        private const string SCHEMAVERSION = @"tg_schemaVersion";
        private const string CREATEDBY = @"tg_createdBy";
        private const string CREATEDDATE = @"tg_createdDate";
        private const string MODIFIEDBY = @"tg_modifiedBy";
        private const string MODIFIEDDATE = @"tg_modifiedDate";
        private const string DISPLAYORDER = @"tg_displayOrder";
        private const string THERAPYGROUPJOINTYPE = @"tg_therapyGroupJoinType";
        private const string NAME = @"tg_name";
        private const string DEPRECATED = @"tg_deprecated";

        private const string PARENTID = @"tg_parentId";
        private const string PARENTTYPE = @"tg_parentType";
        private const string PARENTKEYNAME = @"tg_parentKeyName";
        private const string LAYOUTVARIANT = @"tg_layoutVariant";

        public enum TherapyGroupJoinType
        {
            None = 0,
            AndWithNext = 1,
            OrWithNext = 2
        }

        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDTherapyGroup CreateTherapyGroup(Entities pContext, Guid pParentId)
        {
            return CreateTherapyGroup(pContext, pParentId, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDTherapyGroup CreateTherapyGroup(Entities pContext, Guid pParentId, Guid pUuid)
        {
            BDTherapyGroup therapyGroup = CreateBDTherapyGroup(pUuid, false);
            therapyGroup.createdBy = Guid.Empty;
            therapyGroup.createdDate = DateTime.Now;
            therapyGroup.schemaVersion = ENTITY_SCHEMAVERSION;
            therapyGroup.therapyGroupJoinType = (int)TherapyGroupJoinType.None;
            therapyGroup.displayOrder = -1;
            therapyGroup.name = string.Empty;
            therapyGroup.parentId = pParentId;

            pContext.AddObject(ENTITYNAME, therapyGroup);

            return therapyGroup;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pTherapyGroup"></param>
        public static void Save(Entities pContext, BDTherapyGroup pTherapyGroup)
        {
            if (pTherapyGroup.EntityState != EntityState.Unchanged)
            {
                if (pTherapyGroup.schemaVersion != ENTITY_SCHEMAVERSION)
                    pTherapyGroup.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"TherapyGroup Save");
                pContext.SaveChanges();
            }
        }


        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDTherapyGroup pEntity)
        {
            // delete linked notes
            List<BDLinkedNoteAssociation> linkedNotes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in linkedNotes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }

            // find and delete child objects, then delete record from local data store
            List<BDTherapy> children = BDTherapy.GetTherapiesForTherapyParentId(pContext, pEntity.uuid);
            foreach (BDTherapy t in children)
            {
                BDTherapy.Delete(pContext, t);
            }

            // create BDDeletion record for the object to be deleted
            BDDeletion.CreateDeletion(pContext, KEY_NAME, pEntity.uuid);

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
            BDTherapyGroup entity = BDTherapyGroup.GetTherapyGroupWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDTherapyGroup.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Gets all Therapy Groups in the model with the specified Pathogen ID
        /// </summary>
        /// <param name="pPathogenId"></param>
        /// <returns>List of BDTherapyGroups</returns>
        public static List<BDTherapyGroup> getTherapyGroupsForParentId(Entities pContext, Guid pParentId)
        {
            List<BDTherapyGroup> therapyGroupList = new List<BDTherapyGroup>();
                IQueryable<BDTherapyGroup> therapyGroups = (from entry in pContext.BDTherapyGroups
                                                            where entry.parentId == pParentId
                                                            orderby entry.displayOrder
                                                            select entry);
                foreach (BDTherapyGroup therapyGroup in therapyGroups)
                {
                    therapyGroupList.Add(therapyGroup);
                }
            return therapyGroupList;
        }

        public static BDTherapyGroup GetTherapyGroupWithId(Entities pContext, Guid pUuid)
        {
            BDTherapyGroup entry = null;
            if (null != pUuid)
            {
                IQueryable<BDTherapyGroup> entryList = (from bdTherapyGroups in pContext.BDTherapyGroups
                                                        where bdTherapyGroups.uuid == pUuid
                                                        select bdTherapyGroups);

                if (entryList.Count<BDTherapyGroup>() > 0)
                    entry = entryList.AsQueryable().First<BDTherapyGroup>();
            }
            return entry;
        }

        /// <summary>
        /// Get a string array of all the Therapy Group name values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] GetTherapyGroupNames(Entities pContext)
        {
            var therapyGroupNames = pContext.BDTherapyGroups.Where(x => (!string.IsNullOrEmpty(x.name))).Select(pg => pg.name).Distinct();

            return therapyGroupNames.ToArray();
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
            get { return BDConstants.BDNodeType.BDTherapyGroup; }
        }

        public string Description
        {
            get { return string.Format("[{0}]", this.name); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("Therapy Group - {0}", this.name); }
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
            IQueryable<BDTherapyGroup> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDTherapyGroups
                            select entry);
            }
            else
            {
                entries = (from entry in pContext.BDTherapyGroups
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }
            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDTherapyGroup>());
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDTherapyGroup.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDTherapyGroup)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
            }
            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDDisease from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDTherapyGroup entry = BDTherapyGroup.GetTherapyGroupWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDTherapyGroup.CreateBDTherapyGroup(uuid, deprecated);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]); ;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.therapyGroupJoinType = int.Parse(pAttributeDictionary[THERAPYGROUPJOINTYPE]);
            entry.name = pAttributeDictionary[NAME];

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
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.THERAPYGROUPJOINTYPE).WithValue(therapyGroupJoinType.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion




    }
}
