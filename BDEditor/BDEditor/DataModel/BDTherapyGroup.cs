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
    public partial class BDTherapyGroup : IBDObject
    {
        public const string AWS_DOMAIN = @"bd_1_therapyGroups";
        public const string ENTITYNAME = @"BDTherapyGroups";
        public const string ENTITYNAME_FRIENDLY = @"Therapy Group";
        public const string PROPERTYNAME_DEFAULT = "TherapyGroup";
        public const string PROPERTYNAME_NAME = @"Name";

        private const string UUID = @"tg_uuid";
        private const string SCHEMAVERSION = @"tg_schemaVersion";
        private const string CREATEDBY = @"tg_createdBy";
        private const string CREATEDDATE = @"tg_createdDate";
        private const string MODIFIEDBY = @"tg_modifiedBy";
        private const string MODIFIEDDATE = @"tg_modifiedDate";
        private const string DISPLAYORDER = @"tg_displayOrder";
        private const string PATHOGENGROUPID = @"tg_pathogenGroupId";
        private const string THERAPYGROUPJOINTYPE = @"tg_therapyGroupJoinType";
        private const string NAME = @"tg_name";
        private const string DEPRECATED = @"tg_deprecated";

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
        public static BDTherapyGroup CreateTherapyGroup(Entities pContext, Guid pPathogenGroupId)
        {
            BDTherapyGroup therapyGroup = CreateBDTherapyGroup(Guid.NewGuid(), false);
            therapyGroup.createdBy = Guid.Empty;
            therapyGroup.createdDate = DateTime.Now;
            therapyGroup.schemaVersion = 0;
            therapyGroup.therapyGroupJoinType = (int)TherapyGroupJoinType.None;
            therapyGroup.displayOrder = -1;
            therapyGroup.name = string.Empty;
            therapyGroup.pathogenGroupId = pPathogenGroupId;

            pContext.AddObject("BDTherapyGroups", therapyGroup);

            return therapyGroup;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pTherapyGroup"></param>
        public static void SaveTherapyGroup(Entities pContext, BDTherapyGroup pTherapyGroup)
        {
            if (pTherapyGroup.EntityState != EntityState.Unchanged)
            {
                //pTherapyGroup.modifiedBy = Guid.Empty;
                //pTherapyGroup.modifiedDate = DateTime.Now;
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
            // find and delete child objects, then delete record from local data store
            List<BDTherapy> children = BDTherapy.GetTherapiesForTherapyGroupId(pContext, pEntity.uuid);
            foreach (BDTherapy t in children)
            {
                BDTherapy.Delete(pContext, t);
            }

            // create BDDeletion record for the object to be deleted
            BDDeletion.CreateDeletion(pContext, ENTITYNAME_FRIENDLY, pEntity.uuid);

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
        public static List<BDTherapyGroup> getTherapyGroupsForPathogenGroupId(Entities pContext, Guid pPathogenGroupId)
        {
            List<BDTherapyGroup> therapyGroupList = new List<BDTherapyGroup>();
                IQueryable<BDTherapyGroup> therapyGroups = (from entry in pContext.BDTherapyGroups
                                                            where entry.pathogenGroupId == pPathogenGroupId
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
                IQueryable<BDTherapyGroup> entryList = (from bdTherapGroups in pContext.BDTherapyGroups
                                                        where bdTherapGroups.uuid == pUuid
                                                        select bdTherapGroups);

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
                            //System.Diagnostics.Debug.WriteLine(string.Format("TherapyGroup property change [{0}]", property));
                        }
                        break;
                }

            base.OnPropertyChanged(property);
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return string.Format("[{0}]", this.name); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("Therapy Group - {0}", this.name); }
        }

        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDTherapyGroup> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDTherapyGroup> entryList = new List<BDTherapyGroup>();
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
                entryList = entries.ToList<BDTherapyGroup>();
            return entryList;
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
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
                pDataContext.AddObject("BDTherapyGroups", entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.pathogenGroupId = Guid.Parse(pAttributeDictionary[PATHOGENGROUPID]);
            entry.therapyGroupJoinType = int.Parse(pAttributeDictionary[THERAPYGROUPJOINTYPE]);
            entry.name = pAttributeDictionary[NAME];

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
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.PATHOGENGROUPID).WithValue((null == pathogenGroupId) ? Guid.Empty.ToString() : pathogenGroupId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.THERAPYGROUPJOINTYPE).WithValue(therapyGroupJoinType.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapyGroup.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

    }
}
