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
    /// Extension of generated class BDTherapy
    /// </summary>
    public partial class BDTherapy: IBDObject
    {
        public const string AWS_DOMAIN = @"bd_1_therapies";
        public const string ENTITYNAME = @"BDTherapies";
        public const string ENTITYNAME_FRIENDLY = @"Therapy";
        public const string KEY_NAME = @"BDTherapy";

        public const int ENTITY_SCHEMAVERSION = 0;
        public const string PROPERTYNAME_THERAPY = @"Therapy";
        public const string PROPERTYNAME_DOSAGE = @"Dosage"; 
        public const string PROPERTYNAME_DURATION = @"Duration";

        private const string UUID = @"th_uuid";
        private const string SCHEMAVERSION = @"th_schemaVersion";
        private const string CREATEDBY = @"th_createdBy";
        private const string CREATEDDATE = @"th_createdDate";
        private const string MODIFIEDBY = @"th_modifiedBy";
        private const string MODIFIEDDATE = @"th_modifiedDate";
        private const string DEPRECATED = @"th_deprecated";
        private const string DISPLAYORDER = @"th_displayOrder";
        private const string THERAPYGROUPID = @"th_therapyGroupId";
        private const string THERAPYJOINTYPE = @"th_therapyJoinType";
        private const string LEFTBRACKET = @"th_leftBracket";
        private const string RIGHTBRACKET = @"th_rightBracket";
        private const string NAME = @"th_name";
        private const string DOSAGE = @"th_dosage";
        private const string DURATION = @"th_duration";

        public enum TherapyJoinType
        {
            None = 0,
            AndWithNext = 1,
            OrWithNext = 2,
            ThenWithNext = 3,
            AndOrWithNext = 4,
        }

        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <returns></returns>
        public static BDTherapy CreateTherapy(Entities pContext, Guid pTherapyGroupId)
        {
            BDTherapy therapy = CreateBDTherapy(Guid.NewGuid(), false);
            therapy.createdBy = Guid.Empty;
            therapy.createdDate = DateTime.Now;
            therapy.schemaVersion = ENTITY_SCHEMAVERSION;
            therapy.therapyJoinType = (int)TherapyJoinType.None;
            therapy.leftBracket = false;
            therapy.rightBracket = false;
            therapy.displayOrder = -1;
            therapy.name = string.Empty;
            therapy.dosage = string.Empty;
            therapy.duration = string.Empty;
            therapy.therapyGroupId = pTherapyGroupId;

            pContext.AddObject(ENTITYNAME, therapy);

            return therapy;
        }

        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pTherapy"></param>
        public static void Save(Entities pContext, BDTherapy pTherapy)
        {
            if (pTherapy.EntityState != EntityState.Unchanged)
            {
                if (pTherapy.schemaVersion != ENTITY_SCHEMAVERSION)
                    pTherapy.schemaVersion = ENTITY_SCHEMAVERSION;
                
                System.Diagnostics.Debug.WriteLine(@"Therapy Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDTherapy pEntity)
        {
            // delete linked notes
            List<BDLinkedNoteAssociation> linkedNotes = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pEntity.uuid);
            foreach (BDLinkedNoteAssociation a in linkedNotes)
            {
                BDLinkedNoteAssociation.Delete(pContext, a);
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
            BDTherapy entity = BDTherapy.GetTherapyWithId(pContext, pUuid);
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDTherapy.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                }
            }
        }

        /// <summary>
        /// Gets all Therapies in the model with the specified Therapy Group ID
        /// </summary>
        /// <param name="pTherapyGroupId"></param>
        /// <returns>List of Therapies</returns>
        public static List<BDTherapy> GetTherapiesForTherapyGroupId(Entities pContext, Guid pTherapyGroupId)
        {
            List<BDTherapy> therapyList = new List<BDTherapy>();

            IQueryable<BDTherapy> therapies = (from entry in pContext.BDTherapies
                                                where entry.therapyGroupId == pTherapyGroupId
                                                orderby entry.displayOrder
                                                select entry);
            foreach (BDTherapy therapy in therapies)
            {
                therapyList.Add(therapy);
            }
            return therapyList;
        }

        public static BDTherapy GetTherapyWithId(Entities pContext, Guid pTherapyId)
        {
            BDTherapy therapy = null;

            if (null != pTherapyId)
            {
                IQueryable<BDTherapy> entries = (from bdTherapies in pContext.BDTherapies
                                                   where bdTherapies.uuid == pTherapyId
                                                   select bdTherapies);
                if (entries.Count<BDTherapy>() > 0)
                    therapy = entries.AsQueryable().First<BDTherapy>();
            }
            return therapy;
        }

        /// <summary>
        /// Get a string array of all the distinct Therapy Name values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] GetTherapyNames(Entities pContext)
        {
            var therapyNames = pContext.BDTherapies.Where(x => (!string.IsNullOrEmpty(x.name))).Select(pg => pg.name).Distinct();

            return therapyNames.ToArray();
        }

        /// <summary>
        /// Get a string array of all the distinct Therapy Dose values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] GetTherapyDosages(Entities pContext)
        {
            var dosages = pContext.BDTherapies.Where(x => (!string.IsNullOrEmpty(x.dosage))).Select(pg => pg.dosage).Distinct();

            return dosages.ToArray();
        }

        /// <summary>
        /// Get a string array of all the distinct Therapy Duration values in the database.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static string[] GetTherapyDurations(Entities pContext)
        {
            var durations = pContext.BDTherapies.Where(x => (!string.IsNullOrEmpty(x.duration))).Select(pg => pg.duration).Distinct();
            return durations.ToArray();
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return string.Format("[{0}][{1}][{2}]", this.name, this.dosage, this.duration); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("Therapy - {0} Dosage:{1} Duration:{2}", this.name, this.dosage, this.duration); }
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
            IQueryable<BDTherapy> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDTherapies
                            select entry);
            }
            else
            {
                entries = pContext.BDTherapies.Where(x => x.modifiedDate > pUpdateDateTime);
                //entries = (from entry in pContext.BDTherapies
                //           where (entry.modifiedDate > pUpdateDateTime) && (entry.modifiedDate != pUpdateDateTime)
                //            select entry);
            }
            if (entries.Count() > 0)
            {
                entryList = new List<IBDObject>(entries.ToList<BDTherapy>());
            }
            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
            syncInfo.PushList = BDTherapy.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;

            for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
            {
                ((BDTherapy)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
            }
            if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }

            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDTherapy from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDTherapy entry = BDTherapy.GetTherapyWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDTherapy.CreateBDTherapy(uuid, deprecated);
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
            entry.therapyGroupId = Guid.Parse(pAttributeDictionary[THERAPYGROUPID]);
            entry.therapyJoinType = int.Parse(pAttributeDictionary[THERAPYJOINTYPE]);
            entry.leftBracket = Boolean.Parse(pAttributeDictionary[LEFTBRACKET]);
            entry.rightBracket = Boolean.Parse(pAttributeDictionary[RIGHTBRACKET]);
            entry.name = pAttributeDictionary[NAME];
            entry.dosage = pAttributeDictionary[DOSAGE];
            entry.duration = pAttributeDictionary[DURATION];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.DISPLAYORDER).WithValue(displayOrder.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.THERAPYGROUPID).WithValue((null == therapyGroupId) ? Guid.Empty.ToString() : therapyGroupId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.THERAPYJOINTYPE).WithValue(string.Format(@"{0}", therapyJoinType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.LEFTBRACKET).WithValue(leftBracket.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.RIGHTBRACKET).WithValue(rightBracket.ToString()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.DOSAGE).WithValue((null == dosage) ? string.Empty : dosage).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTherapy.DURATION).WithValue((null == duration) ? string.Empty : duration).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

        public override string ToString()
        {
            return this.name;
        }
    }
}
