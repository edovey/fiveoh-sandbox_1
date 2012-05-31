using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDLinkedNoteAssociation
    /// </summary>
    public partial class BDLinkedNoteAssociation: IBDObject
    {
        //public const string AWS_DOMAIN = @"bd_1_linkedNoteAssociations";

        public const string AWS_PROD_DOMAIN = @"bd_2_linkedNoteAssociations";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_linkedNoteAssociations";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDLinkedNoteAssociations";
        public const string ENTITYNAME_FRIENDLY = @"Linked Note Association";
        public const string KEY_NAME = @"BDLinkedNoteAssociation";

        public const int ENTITY_SCHEMAVERSION = 1;

        private const string UUID = @"la_uuid";
        private const string SCHEMAVERSION = @"la_schemaVersion";
        private const string CREATEDBY = @"la_createdBy";
        private const string CREATEDDATE = @"la_createdDate";
        private const string MODIFIEDBY = @"la_modifiedBy";
        private const string MODIFIEDDATE = @"la_modifiedDate";
        private const string DISPLAYORDER = @"la_displayOrder";
        private const string DEPRECATED = @"la_deprecated";
        private const string LINKEDNOTEID = @"la_linkedNoteId";
        private const string LINKEDNOTETYPE = @"la_linkedNoteType";
        private const string PARENTID = @"la_parentId";
        private const string PARENTKEYNAME = @"la_parentKeyName";
        private const string PARENTKEYPROPERTYNAME = @"la_parentKeyPropertyName";
        private const string PARENTTYPE = @"la_parentType";

        /// <summary>
        /// Extended Create method that sets the created data and schema version. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>BDLinkedNoteAssociation</returns>
        public static BDLinkedNoteAssociation CreateBDLinkedNoteAssociation(Entities pContext)
        {
            return CreateBDLinkedNoteAssociation(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created data and schema version. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>BDLinkedNoteAssociation</returns>
        public static BDLinkedNoteAssociation CreateBDLinkedNoteAssociation(Entities pContext, Guid pUuid)
        {
            BDLinkedNoteAssociation linkedNoteAssociation = CreateBDLinkedNoteAssociation(pUuid, false);
            linkedNoteAssociation.createdBy = Guid.Empty;
            linkedNoteAssociation.createdDate = DateTime.Now;
            linkedNoteAssociation.deprecated = false;
            linkedNoteAssociation.schemaVersion = ENTITY_SCHEMAVERSION;
            linkedNoteAssociation.linkedNoteType = (int)BDConstants.LinkedNoteType.Inline;
            linkedNoteAssociation.displayOrder = -1;
            pContext.AddObject(ENTITYNAME, linkedNoteAssociation);
            return linkedNoteAssociation;
        }

        /// <summary>
        /// Extended create method that includes parent information. Saves the instance.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteType"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <param name="pParentNodeType"></param>
        /// <param name="pParentId"></param>
        /// <param name="pParentPropertyName"></param>
        /// <returns>BDLinkedNoteAssociation</returns>
        public static BDLinkedNoteAssociation CreateBDLinkedNoteAssociation(Entities pContext, 
                                                                            BDConstants.LinkedNoteType pLinkedNoteType, 
                                                                            Guid pLinkedNoteId, 
                                                                            BDConstants.BDNodeType pParentType, 
                                                                            Guid pParentId, 
                                                                            string pParentPropertyName)
        {
            BDLinkedNoteAssociation linkedNoteAssociation = CreateBDLinkedNoteAssociation(Guid.NewGuid(), false);
            linkedNoteAssociation.createdBy = Guid.Empty;
            linkedNoteAssociation.schemaVersion = 0;
            linkedNoteAssociation.linkedNoteType = (int)pLinkedNoteType;
            linkedNoteAssociation.linkedNoteId = pLinkedNoteId;
            linkedNoteAssociation.parentId = pParentId;
            linkedNoteAssociation.parentType = (int)pParentType;
            linkedNoteAssociation.parentKeyName = pParentType.ToString();
            linkedNoteAssociation.parentKeyPropertyName = pParentPropertyName;

            pContext.AddObject(ENTITYNAME, linkedNoteAssociation);

            Save(pContext, linkedNoteAssociation);

            return linkedNoteAssociation;
        }

        public static void Save(Entities pContext, BDLinkedNoteAssociation pLinkedNoteAssociation)
        {
            if (pLinkedNoteAssociation.EntityState != EntityState.Unchanged)
            {
                if (pLinkedNoteAssociation.schemaVersion != ENTITY_SCHEMAVERSION)
                    pLinkedNoteAssociation.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"LinkedNoteAssociation Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDLinkedNoteAssociation pEntity)
        {
            Delete(pContext, pEntity, true);
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        /// <param name="pCreateDeletion">Create entry in the deletion table (bool)</param>
        public static void Delete(Entities pContext, BDLinkedNoteAssociation pEntity, bool pCreateDeletion)
        {
            // Don't delete the iNote from here. Deletion of a iNote will delete all association entries
            if (null == pEntity) return;
            if (pCreateDeletion)
            {
                // create BDDeletion record for the object to be deleted
                BDDeletion.CreateBDDeletion(pContext, KEY_NAME, pEntity.uuid);
            }

            BDMetadata.DeleteForItemId(pContext, pEntity.Uuid, pCreateDeletion);
            // delete record from local data store
            pContext.DeleteObject(pEntity);
            pContext.SaveChanges();
        }

        /// <summary>
        /// Get object to delete using provided uuid, call extended delete
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid">Guid of record to delete</param>
        /// <param name="pCreateDeletion">create entry in deletion table (bool)</param>
        public static void Delete(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            BDLinkedNoteAssociation entity = BDLinkedNoteAssociation.GetLinkedNoteAssociationWithId(pContext, pUuid);
            BDLinkedNoteAssociation.Delete(pContext, entity, pCreateDeletion);
        }

        public static void DeleteForNote(Entities pContext, BDLinkedNote pLinkedNote, bool pCreateDeletion)
        {
            if (null == pLinkedNote) return;

            List<BDLinkedNoteAssociation> associationList = GetLinkedNoteAssociationsForLinkedNoteId(pContext, pLinkedNote.Uuid);
            foreach (BDLinkedNoteAssociation association in associationList)
            {
                Delete(pContext, association, pCreateDeletion);
            }
        }

        public static void DeleteForParentId(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            List<BDLinkedNoteAssociation> associationList = GetLinkedNoteAssociationsForParentId(pContext, pUuid);
            foreach (BDLinkedNoteAssociation association in associationList)
            {
                Delete(pContext, association, pCreateDeletion);
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
                BDLinkedNoteAssociation entry = BDLinkedNoteAssociation.GetLinkedNoteAssociationWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Returns all the LinkedNoteAssociations for a parent uuid
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <returns></returns>
        public static List<BDLinkedNoteAssociation> GetLinkedNoteAssociationsFromParentIdAndProperty(Entities pContext, Guid? pParentId, string pContextPropertyName)
        {
            List<BDLinkedNoteAssociation> resultList = new List<BDLinkedNoteAssociation>();

            BDLinkedNoteAssociation existingAssociation = GetLinkedNoteAssociationForParentIdAndProperty(pContext, pParentId, pContextPropertyName);
            if (null != existingAssociation)
            {
                resultList = GetLinkedNoteAssociationsForLinkedNoteId(pContext, existingAssociation.linkedNoteId.Value);
            }

            return resultList;
        }

        public static BDLinkedNoteAssociation GetLinkedNoteAssociationForParentIdAndProperty(Entities pContext, Guid? pParentId, string pContextPropertyName)
        {
            IQueryable<BDLinkedNoteAssociation> linkedNoteAssociations = (from bdLinkedNoteAssociations in pContext.BDLinkedNoteAssociations
                                                                          where bdLinkedNoteAssociations.parentId == pParentId && bdLinkedNoteAssociations.parentKeyPropertyName == pContextPropertyName
                                                                          select bdLinkedNoteAssociations);

            BDLinkedNoteAssociation result = null;
            if (linkedNoteAssociations.Count() > 0)
            {
                result = linkedNoteAssociations.First<BDLinkedNoteAssociation>();
            }
            return result;
        }

        public static List<BDLinkedNoteAssociation> GetLinkedNoteAssociationsForParentId(Entities pContext, Guid? pParentId)
        {
            List<BDLinkedNoteAssociation> resultList = new List<BDLinkedNoteAssociation>();
            if (pParentId != null)
            {
                IQueryable<BDLinkedNoteAssociation> linkedNoteAssociations = (from bdLinkedNoteAssociations in pContext.BDLinkedNoteAssociations
                                                                              where bdLinkedNoteAssociations.parentId == pParentId
                                                                              select bdLinkedNoteAssociations);

                resultList = linkedNoteAssociations.ToList<BDLinkedNoteAssociation>();
            }
            return resultList;
        }

        public static List<string> GetAssignedPropertyNamesForParentId(Entities pContext, Guid? pParentId)
        {
            var nameList = pContext.BDLinkedNoteAssociations.Where(x => x.uuid == pParentId.Value)
                                                            .Select(pg => pg.parentKeyPropertyName).Distinct();

            return nameList.ToList<string>();
        }

        /// <summary>
        /// Returns all the LinkedNoteAssociations for a linkedNote uuid 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static List<BDLinkedNoteAssociation> GetLinkedNoteAssociationsForLinkedNoteId(Entities pContext, Guid pLinkedNoteId)
        {
            IQueryable<BDLinkedNoteAssociation> linkedNoteAssociations = (from bdLinkedNoteAssociations in pContext.BDLinkedNoteAssociations
                                                                          where bdLinkedNoteAssociations.linkedNoteId == pLinkedNoteId
                                                                          orderby bdLinkedNoteAssociations.parentKeyName ascending, bdLinkedNoteAssociations.parentKeyPropertyName ascending
                                                                          select bdLinkedNoteAssociations);
            List<BDLinkedNoteAssociation> resultList = linkedNoteAssociations.ToList<BDLinkedNoteAssociation>();
            return resultList;
        }

        /// <summary>
        /// Return the LinkedNoteAssociation for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static BDLinkedNoteAssociation GetLinkedNoteAssociationWithId(Entities pContext, Guid? pLinkedNoteAssociationId)
        {
            BDLinkedNoteAssociation result = null;

            if (null != pLinkedNoteAssociationId)
            {
                IQueryable<BDLinkedNoteAssociation> entries = (from entities in pContext.BDLinkedNoteAssociations
                                                        where entities.uuid == pLinkedNoteAssociationId
                                                        select entities);
                if (entries.Count<BDLinkedNoteAssociation>() > 0)
                    result = entries.AsQueryable().First<BDLinkedNoteAssociation>();
            }

            return result;
        }

        public static string GetDescription(Entities pDataContext, Guid? pParentId, BDConstants.BDNodeType pParentNodeType, string pParentEntityPropertyName)
        {
            string result = string.Format("{0} [{1}]", pParentNodeType.ToString(), pParentEntityPropertyName);

            if (null != pParentId)
            {
                switch (pParentNodeType)
                {
                    case BDConstants.BDNodeType.BDTherapy:
                        {
                            BDTherapy therapy = BDTherapy.RetrieveTherapyWithId(pDataContext, pParentId.Value);
                            if (null != therapy)
                            {
                                result = string.Format("{0} [{1}]", therapy.DescriptionForLinkedNote, pParentEntityPropertyName);
                            }
                        }
                        break;

                    case BDConstants.BDNodeType.BDTherapyGroup:
                        {
                            BDTherapyGroup therapyGroup = BDTherapyGroup.RetrieveTherapyGroupWithId(pDataContext, pParentId.Value);
                            if (null != therapyGroup)
                            {
                                result = string.Format("{0} [{1}]", therapyGroup.DescriptionForLinkedNote, pParentEntityPropertyName);
                            }
                        }
                        break;

                    case BDConstants.BDNodeType.BDPathogen:
                    case BDConstants.BDNodeType.BDPresentation:
                    default:
                        {
                            BDNode node = BDNode.RetrieveNodeWithId(pDataContext, pParentId.Value); //PathogenWithId(pDataContext, pParentId.Value);
                            if (null != node)
                            {
                                result = string.Format("{0} [{1}]", node.DescriptionForLinkedNote, pParentEntityPropertyName);
                            }
                        }
                        break;
                }
            }
            return result;
        }

        public string GetDescription(Entities pDataContext)
        {
            return GetDescription(pDataContext, this.parentId, this.ParentType, this.parentKeyPropertyName);
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
            IQueryable<BDLinkedNoteAssociation> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDLinkedNoteAssociations
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDLinkedNoteAssociations
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }
            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDLinkedNoteAssociation>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDLinkedNoteAssociation.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if (null != pCurrentSyncDate)
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDLinkedNoteAssociation)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
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
            bool deprecated = Boolean.Parse(pAttributeDictionary[DEPRECATED]);
            BDLinkedNoteAssociation entry = BDLinkedNoteAssociation.GetLinkedNoteAssociationWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDLinkedNoteAssociation.CreateBDLinkedNoteAssociation(uuid, false);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.linkedNoteId = Guid.Parse(pAttributeDictionary[LINKEDNOTEID]);
            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];
            entry.parentType = int.Parse(pAttributeDictionary[PARENTTYPE]);
            entry.parentKeyPropertyName = pAttributeDictionary[PARENTKEYPROPERTYNAME];
            entry.linkedNoteType = int.Parse(pAttributeDictionary[LINKEDNOTETYPE]);
            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.LINKEDNOTEID).WithValue((null == linkedNoteId) ? Guid.Empty.ToString() : linkedNoteId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.LINKEDNOTETYPE).WithValue(string.Format(@"{0}", linkedNoteType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.PARENTKEYPROPERTYNAME).WithValue((null == parentKeyPropertyName) ? string.Empty : parentKeyPropertyName).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { return this.uuid.ToString(); }
        }

        public string DescriptionForLinkedNote
        {
            get { return string.Format("{0}: {1}", ENTITYNAME_FRIENDLY, this.uuid); }
        }

        public override string ToString()
        {
            return this.uuid.ToString();
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
    }

}
