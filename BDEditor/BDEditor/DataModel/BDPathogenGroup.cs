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
    /// Extension of generated BDPathogenGroup
    /// </summary>
    public partial class BDPathogenGroup: IBDObject
    {
        //public const string AWS_DOMAIN = @"bd_1_pathogenGroups";

        public const string AWS_PROD_DOMAIN = @"bd_1_pathogenGroups";
        public const string AWS_DEV_DOMAIN = @"bd_dev_1_pathogenGroups";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDPathogenGroups";
        public const string ENTITYNAME_FRIENDLY = @"Pathogen Group";
        public const string KEY_NAME = @"BDPathogenGroup";
        public const string PROPERTYNAME_NAME = @"Name";
        public const int ENTITY_SCHEMAVERSION = 2;

        private const string UUID = @"pg_uuid";
        private const string SCHEMAVERSION = @"pg_schemaVersion";
        private const string CREATEDBY = @"pg_createdBy";
        private const string CREATEDDATE = @"pg_createdDate";
        private const string MODIFIEDBY = @"pg_modifiedBy";
        private const string MODIFIEDDATE = @"pg_modifiedDate";
        private const string DEPRECATED = @"pg_deprecated";
        private const string DISPLAYORDER = @"pg_displayOrder";
        private const string NAME = @"pg_name";
        private const string PARENTID = @"pg_parentId";
        private const string PARENTKEYNAME = @"pg_parentKeyName";

        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static BDPathogenGroup CreatePathogenGroup(Entities pContext, Guid pParentId)
        {
            BDPathogenGroup pathogenGroup = CreateBDPathogenGroup(Guid.NewGuid(), false);
            pathogenGroup.createdBy = Guid.Empty;
            pathogenGroup.createdDate = DateTime.Now;
            pathogenGroup.schemaVersion = ENTITY_SCHEMAVERSION;
            pathogenGroup.displayOrder = -1;
            pathogenGroup.parentId = pParentId;
            pathogenGroup.name = string.Empty;
            pathogenGroup.parentKeyName = string.Empty;
            pContext.AddObject(ENTITYNAME, pathogenGroup);

            return pathogenGroup;
        }


        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPathogenGroup"></param>
        public static void Save(Entities pContext, BDPathogenGroup pPathogenGroup)
        {
            if (pPathogenGroup.EntityState != EntityState.Unchanged)
            {
                if (pPathogenGroup.schemaVersion != ENTITY_SCHEMAVERSION)
                    pPathogenGroup.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"PathogenGroup Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDPathogenGroup pEntity)
        {
            // delete linked notes
            List<BDLinkedNoteAssociation> linkedNotes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in linkedNotes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
            }

            // delete children
            List<BDPathogen> pathogens = BDPathogen.GetPathogensForParent(pContext, pEntity.uuid);
            foreach (BDPathogen p in pathogens)
            {
                BDPathogen.Delete(pContext, p);
            }

            List<BDTherapyGroup> therapyGroups = BDTherapyGroup.getTherapyGroupsForParentId(pContext, pEntity.uuid);
            foreach (BDTherapyGroup tg in therapyGroups)
            {
                BDTherapyGroup.Delete(pContext, tg);
            }

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
            BDPathogenGroup entity = BDPathogenGroup.GetPathogenGroupWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDPathogenGroup.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Gets all pathogen groups in the model with the specified parent ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <returns></returns>
        public static List<BDPathogenGroup> GetPathogenGroupsForParentId(Entities pContext, Guid pParentId)
        {
            List<BDPathogenGroup> pathogenGroupList = new List<BDPathogenGroup>();

            IQueryable<BDPathogenGroup> pathogenGroups = (from bdPathogenGroups in pContext.BDPathogenGroups
                                                          where bdPathogenGroups.parentId == pParentId
                                                          select bdPathogenGroups);
            foreach (BDPathogenGroup pathogenGroup in pathogenGroups)
            {
                pathogenGroupList.Add(pathogenGroup);
            }
            return pathogenGroupList;
        }

        public static BDPathogenGroup GetPathogenGroupWithId(Entities pContext, Guid pPathogenGroupId)
        {
            BDPathogenGroup pathogenGroup = null;

            if (null != pPathogenGroupId)
            {
                IQueryable<BDPathogenGroup> entries = (from bdPathogenGroups in pContext.BDPathogenGroups
                                                              where bdPathogenGroups.uuid == pPathogenGroupId
                                                              select bdPathogenGroups);
                if (entries.Count<BDPathogenGroup>() > 0)
                    pathogenGroup = entries.AsQueryable().First<BDPathogenGroup>();
            }
            return pathogenGroup;
        }

        /// <summary>
        /// Get a string array of all the Pathogen Group name values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] GetPathogenGroupNames(Entities pContext)
        {
            var pgroups = pContext.BDPathogenGroups.Where(x => (!string.IsNullOrEmpty(x.name))).Select(pg => pg.name).Distinct();

            return pgroups.ToArray();
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
            IQueryable<BDPathogenGroup> pathogenGroups;

            if (null == pUpdateDateTime)
            {
                pathogenGroups = (from entry in pContext.BDPathogenGroups
                            select entry);
            }
            else
            {
                pathogenGroups = (from entry in pContext.BDPathogenGroups
                            where entry.modifiedDate > pUpdateDateTime.Value
                            select entry);
            }
            if (pathogenGroups.Count() > 0)
                entryList = new List<IBDObject>( pathogenGroups.ToList<BDPathogenGroup>());
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDPathogenGroup.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDPathogenGroup)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
            }
            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDPathogenGroup from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDPathogenGroup entry = BDPathogenGroup.GetPathogenGroupWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDPathogenGroup.CreateBDPathogenGroup(uuid, deprecated);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            if(schemaVersion >= 1)
                entry.name = pAttributeDictionary[NAME];

            if (schemaVersion >= 2)
            {
                entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
                entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];
            }

            if (pSaveChanges)
                pDataContext.SaveChanges();
            
            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            if (schemaVersion >= 2)
            {
                attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
                attributeList.Add(new ReplaceableAttribute().WithName(BDPathogenGroup.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));
            }
            return putAttributeRequest;
        }
        #endregion

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
            get { return string.Format("Pathogen Group: {0}", this.name); }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
