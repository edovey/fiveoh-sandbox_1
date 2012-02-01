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
    /// Extension of generated BDSearchEntryAssociation
    /// </summary>
    public partial class BDSearchEntryAssociation : IBDObject
    {
        //public const string AWS_DOMAIN = @"bd_1_searchEntryAssociations";

        public const string AWS_PROD_DOMAIN = @"bd_2_searchEntryAssociations";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_searchEntryAssociations";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif

        public const string ENTITYNAME = @"BDSearchEntryAssociations";
        public const string ENTITYNAME_FRIENDLY = @"Search Entry Association";
        public const string KEY_NAME = @"BDSearchEntryAssociation";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"sa_uuid";
        private const string SCHEMAVERSION = @"sa_schemaVersion";
        private const string CREATEDBY = @"sa_createdBy";
        private const string CREATEDDATE = @"sa_createdDate";
        private const string MODIFIEDBY = @"sa_modifiedBy";
        private const string MODIFIEDDATE = @"sa_modifiedDate";
        private const string DISPLAYORDER = @"sa_displayOrder";
        private const string SEARCHENTRYID = @"sa_searchEntryId";
        private const string DISPLAYPARENTID = @"sa_displayParentId";
        private const string DISPLAYPARENTKEYNAME = @"sa_displayParentKeyName";
        private const string PARENTENTITYPROPERTYNAME = @"sa_parentEntityPropertyName";
        private const string DISPLAYCONTEXT = @"sa_displayContext";

        /// <summary>
        /// Extended Create method that sets the created data and schema version. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>BDSearchEntryAssociation</returns>
        public static BDSearchEntryAssociation CreateSearchEntryAssociation(Entities pContext)
        {
            return CreateSearchEntryAssociation(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created data and schema version. Does not save.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns>BDSearchEntryAssociation</returns>
        public static BDSearchEntryAssociation CreateSearchEntryAssociation(Entities pContext, Guid pUuid)
        {
            BDSearchEntryAssociation entry = CreateBDSearchEntryAssociation(pUuid);
            entry.createdBy = Guid.Empty;
            entry.createdDate = DateTime.Now;
            entry.schemaVersion = ENTITY_SCHEMAVERSION;
            entry.displayOrder = -1;
            pContext.AddObject(ENTITYNAME, entry);
            return entry;
        }

        /// <summary>
        /// Extended create method that includes parent information. Saves the instance.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pSearchEntryId"></param>
        /// <param name="pDisplayParentEntityName"></param>
        /// <param name="pDisplayParentId"></param>
        /// <param name="pDisplayContext"></param>
        /// <returns>BDSearchEntryAssociation</returns>
        public static BDSearchEntryAssociation CreateSearchEntryAssociation(Entities pContext,
                                                                            Guid pSearchEntryId,
                                                                            string pDisplayParentKeyName,
                                                                            Guid pDisplayParentId,
                                                                            string pDisplayContext)
        {
            BDSearchEntryAssociation association = CreateBDSearchEntryAssociation(Guid.NewGuid());
            association.createdBy = Guid.Empty;
            association.schemaVersion = 0;

            association.searchEntryId = pSearchEntryId;
            association.displayParentId = pDisplayParentId;
            association.displayParentKeyName = pDisplayParentKeyName;
            association.displayContext = pDisplayContext;

            pContext.AddObject(ENTITYNAME, association);

            Save(pContext, association);

            return association;
        }

        public static void Save(Entities pContext, BDSearchEntryAssociation pAssociation)
        {
            if (pAssociation.EntityState != EntityState.Unchanged)
            {
                if (pAssociation.schemaVersion != ENTITY_SCHEMAVERSION)
                    pAssociation.schemaVersion = ENTITY_SCHEMAVERSION;

                System.Diagnostics.Debug.WriteLine(@"SearchEntryAssociation Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDSearchEntryAssociation pEntity)
        {
            Delete(pContext, pEntity, true);
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        /// <param name="pCreateDeletion">Create entry in the deletion table (bool)</param>
        public static void Delete(Entities pContext, BDSearchEntryAssociation pEntity, bool pCreateDeletion)
        {
            // Don't delete the note from here. Deletion of a note will delete all association entries
            if (null == pEntity) return;
            if (pCreateDeletion)
            {
                // create BDDeletion record for the object to be deleted
                BDDeletion.CreateDeletion(pContext, KEY_NAME, pEntity.uuid);
            }
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
            BDSearchEntryAssociation entity = BDSearchEntryAssociation.GetSearchEntryAssociationWithId(pContext, pUuid);
            BDSearchEntryAssociation.Delete(pContext, entity, pCreateDeletion);
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
                BDSearchEntryAssociation entry = BDSearchEntryAssociation.GetSearchEntryAssociationWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        public static void DeleteAll()
        {
            BDEditor.DataModel.Entities dataContext = new BDEditor.DataModel.Entities();
            dataContext.ExecuteStoreCommand("DELETE FROM BDSearchEntryAssociations");
        }

        public static void DeleteForSearchEntryId(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            List<BDSearchEntryAssociation> children = BDSearchEntryAssociation.GetSearchEntryAssociationsForSearchEntryId(pContext, pUuid);
            foreach (BDSearchEntryAssociation t in children)
            {
                BDSearchEntryAssociation.Delete(pContext, t, pCreateDeletion);
            }
        }

        public static List<BDSearchEntryAssociation> GetSearchEntryAssociationsForDisplayParentId(Entities pContext, Guid? pDisplayParentId)
        {
            List<BDSearchEntryAssociation> resultList = new List<BDSearchEntryAssociation>();
            if (pDisplayParentId != null)
            {
                IQueryable<BDSearchEntryAssociation> associations = (from entries in pContext.BDSearchEntryAssociations
                                                                              where entries.displayParentId == pDisplayParentId
                                                                              select entries);

                resultList = associations.ToList<BDSearchEntryAssociation>();
            }
            return resultList;
        }

        /// <summary>
        /// Returns all the SearchEntryAssociations for a searchEntry uuid 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static List<BDSearchEntryAssociation> GetSearchEntryAssociationsForSearchEntryId(Entities pContext, Guid pSearchEntryId)
        {
            IQueryable<BDSearchEntryAssociation> entries = (from entities in pContext.BDSearchEntryAssociations
                                                            where entities.searchEntryId == pSearchEntryId
                                                            orderby entities.displayParentKeyName ascending, entities.displayContext ascending
                                                            select entities);
            List<BDSearchEntryAssociation> resultList = entries.ToList<BDSearchEntryAssociation>();
            return resultList;
        }

        /// <summary>
        /// Return the LinkedNoteAssociation for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLinkedNoteId"></param>
        /// <returns></returns>
        public static BDSearchEntryAssociation GetSearchEntryAssociationWithId(Entities pContext, Guid? pSearchEntryAssociationId)
        {
            BDSearchEntryAssociation result = null;

            if (null != pSearchEntryAssociationId)
            {
                IQueryable<BDSearchEntryAssociation> entries = (from entities in pContext.BDSearchEntryAssociations
                                                               where entities.uuid == pSearchEntryAssociationId
                                                               select entities);
                if (entries.Count<BDSearchEntryAssociation>() > 0)
                    result = entries.AsQueryable().First<BDSearchEntryAssociation>();
            }

            return result;
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
            IQueryable<BDSearchEntryAssociation> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDSearchEntryAssociations
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDSearchEntryAssociations
                           where entry.createdDate > pUpdateDateTime.Value
                           select entry);
            }
            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDSearchEntryAssociation>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDSearchEntryAssociation.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if (null != pCurrentSyncDate)
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDSearchEntryAssociation)syncInfo.PushList[idx]).createdDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDSearchEntryAssociation from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDSearchEntryAssociation entry = BDSearchEntryAssociation.GetSearchEntryAssociationWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDSearchEntryAssociation.CreateBDSearchEntryAssociation(uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            //entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            //entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.searchEntryId = Guid.Parse(pAttributeDictionary[SEARCHENTRYID]);
            entry.displayParentId = Guid.Parse(pAttributeDictionary[DISPLAYPARENTID]);
            entry.displayParentKeyName = pAttributeDictionary[DISPLAYPARENTKEYNAME];
            entry.displayContext = pAttributeDictionary[DISPLAYCONTEXT];
            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            //attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            //attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.SEARCHENTRYID).WithValue((null == searchEntryId) ? Guid.Empty.ToString() : searchEntryId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.DISPLAYPARENTID).WithValue((null == displayParentId) ? Guid.Empty.ToString() : displayParentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.DISPLAYPARENTKEYNAME).WithValue((null == displayParentKeyName) ? string.Empty : displayParentKeyName).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDSearchEntryAssociation.DISPLAYCONTEXT).WithValue((null == displayContext) ? string.Empty : displayContext).WithReplace(true));

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

        public override string ToString()
        {
            return this.uuid.ToString();
        }

        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }

        public BDConstants.BDNodeType NodeType
        {
            get { throw new NotImplementedException(); }
        }
    }

}
