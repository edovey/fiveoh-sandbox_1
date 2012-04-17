﻿using System;
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
    /// Extension of BDTableCell class generated from model
    /// </summary>
    public partial class BDTableCell : IBDObject
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_tableCells";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_tableCells";

        public const string AWS_PROD_BUCKET = @"bdProdStore";
        public const string AWS_DEV_BUCKET = @"bdDevStore";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
        public const string AWS_BUCKET = AWS_DEV_BUCKET;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
        public const string AWS_BUCKET = AWS_PROD_BUCKET;
#endif
        public const string AWS_S3_PREFIX = @"bdhp~";
        public const string AWS_S3_FILEEXTENSION = @".html";

        public const string ENTITYNAME = @"BDTableCells";
        public const string ENTITYNAME_FRIENDLY = @"Table Cells";
        public const string KEY_NAME = @"BDTableCell";

        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"tc_uuid";
        private const string SCHEMAVERSION = @"tc_schemaVersion";
        private const string DISPLAYORDER = @"tc_displayOrder";
        private const string CREATEDBY = @"tc_createdBy";
        private const string CREATEDDATE = @"tc_createdDate";
        private const string MODIFIEDBY = @"tc_modifiedBy";
        private const string MODIFIEDDATE = @"tc_modifiedDate";
        private const string PARENTID = @"tc_parentId";
        private const string PARENTKEYNAME = @"tc_parentKeyName";
        private const string ALIGNMENT = @"tc_alignment";
        private const string VALUE = @"tc_value";

        public Guid? tempProductionUuid { get; set; }

        public static BDTableCell CreateTableCell(Entities pContext)
        {
            return CreateTableCell(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets the created date and schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid"></param>
        /// <returns></returns>
        public static BDTableCell CreateTableCell(Entities pContext, Guid pUuid)
        {
            BDTableCell cell = CreateBDTableCell(pUuid);
            cell.createdBy = Guid.Empty;
            cell.createdDate = DateTime.Now;
            cell.schemaVersion = ENTITY_SCHEMAVERSION;
            cell.displayOrder = -1;
            cell.alignment = -1;

            pContext.AddObject(ENTITYNAME, cell);
            Save(pContext, cell);
            return cell;
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
                BDTableCell entry = BDTableCell.RetrieveWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Delete all records from local store
        /// </summary>
        public static void DeleteAll()
        {
            BDEditor.DataModel.Entities dataContext = new BDEditor.DataModel.Entities();
            // check if table exists:

            int result = dataContext.ExecuteStoreQuery<int>(@"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME ='BDTableCells'").SingleOrDefault();

            if (result == 1)
                dataContext.ExecuteStoreCommand("DELETE FROM BDTableCells");
        }

        public static void DeleteForParentId(Entities pContext, Guid pUuid, bool pCreateDeletion)
        {
            List<BDTableCell> cells = RetrieveTableCellsForParentId(pContext, pUuid);
            foreach (BDTableCell cell in cells)
            {
                Delete(pContext, cell, pCreateDeletion);
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        /// <param name="pCreateDeletion">Create entry in the deletion table (bool)</param>
        public static void Delete(Entities pContext, BDTableCell pEntity, bool pCreateDeletion)
        {
            // Don't delete the iNote from here. Deletion of a iNote will delete all association entries
            if (null == pEntity) return;
            if (pCreateDeletion)
            {
                // create BDDeletion record for the object to be deleted
                BDDeletion.CreateDeletion(pContext, KEY_NAME, pEntity.uuid);
            }

            BDMetadata.DeleteForItemId(pContext, pEntity.Uuid, pCreateDeletion);
            // delete record from local data store
            pContext.DeleteObject(pEntity);
            pContext.SaveChanges();
        }

        public static void Save(Entities pContext, BDTableCell pCell)
        {
            if(pCell.EntityState != EntityState.Unchanged)
            {
                if(pCell.schemaVersion != ENTITY_SCHEMAVERSION)
                    pCell.schemaVersion = ENTITY_SCHEMAVERSION;
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Return the TableCell for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pEntryId"></param>
        /// <returns></returns>
        public static BDTableCell RetrieveWithId(Entities pContext, Guid? pEntryId)
        {
            BDTableCell result = null;

            if (null != pEntryId)
            {
                IQueryable<BDTableCell> entries = (from entry in pContext.BDTableCells
                                                  where entry.uuid == pEntryId
                                                  select entry);

                if (entries.Count<BDTableCell>() > 0)
                    result = entries.AsQueryable().First<BDTableCell>();
            }
            return result;
        }

        public static List<BDTableCell> RetrieveTableCellsForParentId(Entities pContext, Guid? pParentId)
        {
            IQueryable<BDTableCell> entries = (from entry in pContext.BDTableCells
                                               where entry.parentId == pParentId
                                               select entry);
            List<BDTableCell> resultList = entries.ToList<BDTableCell>();
            return resultList;
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
            IQueryable<BDTableCell> entries;

            // get ALL entries - this is a full refresh.
            entries = (from entry in pContext.BDTableCells
                       select entry);

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDTableCell>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, CREATEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDTableCell.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if (null != pCurrentSyncDate)
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDTableCell)syncInfo.PushList[idx]).createdDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDTableCell from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDTableCell entry = BDTableCell.RetrieveWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDTableCell.CreateTableCell(pDataContext, uuid);
                pDataContext.AddObject(ENTITYNAME, entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];
            entry.displayOrder = short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.value = pAttributeDictionary[VALUE];
            entry.alignment = (null == pAttributeDictionary[ALIGNMENT]) ? (short)-1 : short.Parse(pAttributeDictionary[ALIGNMENT]);
            
            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        /// <summary>
        /// Create a new TableCell (creating a new uuid in the process) from an existing TableCell from attributes in a dictionary. Saves the entry.
        /// The production uuid will be stored (by not persisted) in tempProductionUuid
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static BDTableCell CreateFromProdWithAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);

            BDTableCell entry = BDTableCell.CreateTableCell(pDataContext);
            entry.tempProductionUuid = uuid;

            entry.schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.displayOrder = short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.alignment = short.Parse(pAttributeDictionary[ALIGNMENT]);
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);

            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];
            entry.value = pAttributeDictionary[VALUE];

            pDataContext.SaveChanges();

            return entry;
        }

        public static string GenerateStorageKey(BDTableCell pCell)
        {
            string result = GenerateStorageKey(pCell.Uuid);
            return result;
        }

        public static string GenerateStorageKey(Guid pUuid)
        {
            string result = string.Format("{0}{1}{2}", AWS_S3_PREFIX, pUuid.ToString().ToUpper(), AWS_S3_FILEEXTENSION);
            return result;
        }

        #endregion

        #region IBDObject implementation
        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string DescriptionForLinkedNote
        {
            get { throw new NotImplementedException(); }
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.ALIGNMENT).WithValue(string.Format(@"{0}", alignment)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDTableCell.VALUE).WithValue((null == value) ? string.Empty : value).WithReplace(true));
            
            return putAttributeRequest;
        }
        #endregion

        public void SetParent(Guid? pParentId)
        {
            parentId = pParentId;
        }
        
        public override string ToString()
        {
            return this.uuid.ToString();
        }
    }
}