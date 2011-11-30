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
    public enum LinkedNoteType
    {
        All = -1,
        Default = 0,
        Footnote = 1,
        EndNote = 2
    }

    /// <summary>
    /// Extension of generated BDLinkedNoteAssociation
    /// </summary>
    public partial class BDLinkedNoteAssociation
    {
        public const string AWS_DOMAIN = @"bd_1_linkedNoteAssociations";
        public const string ENTITYNAME = @"BDLinkedNoteAssociations";
        public const string ENTITYNAME_FRIENDLY = @"Linked Note Association";

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
        private const string PARENTENTITYNAME = @"la_parentEntityName";
        private const string PARENTENTITYPROPERTYNAME = @"la_parentEntityPropertyName";

        /// <summary>
        /// Extended Create method that sets the created data and schema version. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>BDLinkedNoteAssociation</returns>
        public static BDLinkedNoteAssociation CreateLinkedNoteAssociation(Entities pContext)
        {
            BDLinkedNoteAssociation linkedNoteAssociation = CreateBDLinkedNoteAssociation(Guid.NewGuid());
            linkedNoteAssociation.createdBy = Guid.Empty;
            linkedNoteAssociation.createdDate = DateTime.Now;
            linkedNoteAssociation.deprecated = false;
            linkedNoteAssociation.schemaVersion = 0;
            linkedNoteAssociation.linkedNoteType = (int)LinkedNoteType.Default;
            linkedNoteAssociation.displayOrder = -1;
            pContext.AddObject(@"BDLinkedNoteAssociations", linkedNoteAssociation);
            return linkedNoteAssociation;
        }

        /// <summary>
        /// Extended create method that includes parent information. Saves the instance.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteType"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <param name="pParentEntityName"></param>
        /// <param name="pParentId"></param>
        /// <param name="pParentEntityPropertyName"></param>
        /// <returns>BDLinkedNoteAssociation</returns>
        public static BDLinkedNoteAssociation CreateLinkedNoteAssociation(Entities pContext, 
                                                                            LinkedNoteType pLinkedNoteType, 
                                                                            Guid pLinkedNoteId, 
                                                                            string pParentEntityName, 
                                                                            Guid pParentId, 
                                                                            string pParentEntityPropertyName)
        {
            BDLinkedNoteAssociation linkedNoteAssociation = CreateBDLinkedNoteAssociation(Guid.NewGuid());
            linkedNoteAssociation.createdBy = Guid.Empty;
            linkedNoteAssociation.schemaVersion = 0;
            linkedNoteAssociation.linkedNoteType = (int)pLinkedNoteType;

            linkedNoteAssociation.linkedNoteId = pLinkedNoteId;
            linkedNoteAssociation.parentId = pParentId;
            linkedNoteAssociation.parentEntityName = pParentEntityName;
            linkedNoteAssociation.parentEntityPropertyName = pParentEntityPropertyName;

            pContext.AddObject(@"BDLinkedNoteAssociations", linkedNoteAssociation);

            SaveLinkedNoteAssociation(pContext, linkedNoteAssociation);

            return linkedNoteAssociation;
        }

        public static void SaveLinkedNoteAssociation(Entities pContext, BDLinkedNoteAssociation pLinkedNoteAssociation)
        {
            if (pLinkedNoteAssociation.EntityState != EntityState.Unchanged)
            {
                //pLinkedNoteAssociation.modifiedBy = Guid.Empty;
                //pLinkedNoteAssociation.modifiedDate = DateTime.Now;
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
            // Don't delete the note from here. Deletion of a note will delete all association entries
            if (null != pEntity)
            {
                if (pCreateDeletion)
                {
                    // create BDDeletion record for the object to be deleted
                    BDDeletion.CreateDeletion(pContext, ENTITYNAME_FRIENDLY, pEntity.uuid);
                }
                // delete record from local data store
                pContext.DeleteObject(pEntity);
                pContext.SaveChanges();
            }
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
            if (null != entity)
            {
                if (pCreateDeletion)
                {
                    BDLinkedNoteAssociation.Delete(pContext, entity);
                }
                else
                {
                    pContext.DeleteObject(entity);
                    pContext.SaveChanges();
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
                                                                          where bdLinkedNoteAssociations.parentId == pParentId && bdLinkedNoteAssociations.parentEntityPropertyName == pContextPropertyName
                                                                          select bdLinkedNoteAssociations);

            BDLinkedNoteAssociation result = null;
            if (linkedNoteAssociations.Count() > 0)
            {
                result = linkedNoteAssociations.First<BDLinkedNoteAssociation>();
            }
            return result;
        }

        public static List<BDLinkedNoteAssociation> GetLinkedNoteAssociationForParentId(Entities pContext, Guid? pParentId)
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
                                                                          orderby bdLinkedNoteAssociations.parentEntityName ascending, bdLinkedNoteAssociations.parentEntityPropertyName ascending
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

        public static string GetDescription(Entities pDataContext, Guid? pParentId, string pParentEntityName, string pParentEntityPropertyName)
        {
            string result = string.Format("{0} [{1}]", pParentEntityName, pParentEntityPropertyName);

            if (null != pParentId)
            {
                switch (pParentEntityName)
                {
                    case BDTherapy.ENTITYNAME_FRIENDLY:
                        {
                            BDTherapy therapy = BDTherapy.GetTherapyWithId(pDataContext, pParentId.Value);
                            if (null != therapy)
                            {
                                result = string.Format("{0} [{1}]", therapy.DescriptionForLinkedNote, pParentEntityPropertyName);
                            }
                        }
                        break;
                    case BDTherapyGroup.ENTITYNAME_FRIENDLY:
                        {
                            BDTherapyGroup therapyGroup = BDTherapyGroup.GetTherapyGroupWithId(pDataContext, pParentId.Value);
                            if (null != therapyGroup)
                            {
                                result = string.Format("{0} [{1}]", therapyGroup.DescriptionForLinkedNote, pParentEntityPropertyName);
                            }
                        }
                        break;
                    case BDPathogen.ENTITYNAME_FRIENDLY:
                        {
                            BDPathogen pathogen = BDPathogen.GetPathogenWithId(pDataContext, pParentId.Value);
                            if (null != pathogen)
                            {
                                result = string.Format("{0} [{1}]", pathogen.DescriptionForLinkedNote, pParentEntityPropertyName);
                            }
                        }
                        break;
                    case BDPresentation.ENTITYNAME_FRIENDLY:
                        {
                            result = string.Format("{0} [{1}]", pParentEntityName, pParentEntityPropertyName);
                        }
                        break;

                    default:
                        result = string.Format("{0} [{1}]", pParentEntityName, pParentEntityPropertyName);
                        break;
                }
            }
            return result;
        }

        public string GetDescription(Entities pDataContext)
        {
            return GetDescription(pDataContext, this.parentId, this.parentEntityName, this.parentEntityPropertyName);
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
        public static List<BDLinkedNoteAssociation> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDLinkedNoteAssociation> entryList = new List<BDLinkedNoteAssociation>();
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
                entryList = entries.ToList<BDLinkedNoteAssociation>();
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
            bool deprecated = Boolean.Parse(pAttributeDictionary[DEPRECATED]);
            BDLinkedNoteAssociation entry = BDLinkedNoteAssociation.GetLinkedNoteAssociationWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDLinkedNoteAssociation.CreateBDLinkedNoteAssociation(uuid);
                pDataContext.AddObject("BDLinkedNoteAssociations", entry);
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
            entry.parentEntityName = pAttributeDictionary[PARENTENTITYNAME];
            entry.parentEntityPropertyName = pAttributeDictionary[PARENTENTITYPROPERTYNAME];
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
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.LINKEDNOTEID).WithValue((null == linkedNoteId) ? Guid.Empty.ToString() : linkedNoteId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.LINKEDNOTETYPE).WithValue(string.Format(@"{0}", linkedNoteType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.PARENTENTITYNAME).WithValue((null == parentEntityName) ? string.Empty : parentEntityName).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDLinkedNoteAssociation.PARENTENTITYPROPERTYNAME).WithValue((null == parentEntityPropertyName) ? string.Empty : parentEntityPropertyName).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion
    }

}
