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
    public partial class BDAttachment : IBDNode
    {
        public const string AWS_PROD_DOMAIN = @"bd_2_attachments";
        public const string AWS_DEV_DOMAIN = @"bd_dev_2_attachments";

        public const string AWS_PROD_BUCKET = @"bdProdStore";
        public const string AWS_DEV_BUCKET = @"bdDevStore";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
        public const string AWS_BUCKET = AWS_DEV_BUCKET;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
        public const string AWS_BUCKET = AWS_PROD_BUCKET;
#endif

        public const string AWS_S3_PREFIX = @"bdat~";
        public const string AWS_S3_FILEEXTENSION = @".data";

        public const string ENTITYNAME = @"BDAttachments";
        public const string ENTITYNAME_FRIENDLY = @"Attachments";
        public const string KEY_NAME = @"BDAttachment";

        public const int ENTITY_SCHEMAVERSION = 0;

        public const string PROPERTYNAME_DATA = @"Data";

        private const string UUID = @"at_uuid";
        private const string NODETYPE = @"at_nodeType";
        private const string NODEKEYNAME = @"at_nodeKeyName";
        private const string SCHEMAVERSION = @"at_schemaVersion";
        private const string MODIFIEDDATE = @"at_modifiedDate";
        private const string PARENTID = @"at_parentId";
        private const string PARENTTYPE = @"at_parentType";
        private const string PARENTKEYNAME = @"at_parentKeyName";
        private const string STORAGEKEY = @"at_storageKey";
        private const string ATTACHMENTDATA = @"at_attachmentData";
        private const string ATTACHMENTMIMETYPE = @"at_mimeType";
        private const string NAME = @"at_name";
        private const string DISPLAYORDER = @"at_displayOrder";
        private const string FILENAME = @"at_filename";

        //TODO: attachmentData, filename, filesize

        public Guid? tempProductionUuid { get; set; }

        /// <summary>
        /// Extension of generated BDAttachment
        /// </summary>
        public static BDAttachment CreateBDAttachment(Entities pContext)
        {
            return CreateBDAttachment(pContext, Guid.NewGuid());
        }

        /// <summary>
        /// Extended Create method that sets default values and schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUuid"></param>
        /// <returns></returns>
        public static BDAttachment CreateBDAttachment(Entities pContext, Guid pUuid)
        {
            BDAttachment attachment = CreateBDAttachment(pUuid);
            attachment.schemaVersion = ENTITY_SCHEMAVERSION;
            attachment.storageKey = GenerateStorageKey(attachment);
            attachment.attachmentMimeType = -1;
            attachment.nodeType = (int)BDConstants.BDNodeType.BDAttachment;

            pContext.AddObject(ENTITYNAME, attachment);
            Save(pContext, attachment);
            return attachment;
        }

        /// <summary>
        /// Extended Save that updates the schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pAttachment"></param>
        public static void Save(Entities pContext, BDAttachment pAttachment)
        {
            if (pAttachment.EntityState != EntityState.Unchanged)
            {
                pAttachment.storageKey = GenerateStorageKey(pAttachment);
                if (pAttachment.schemaVersion != ENTITY_SCHEMAVERSION)
                    pAttachment.schemaVersion = ENTITY_SCHEMAVERSION;
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Extended Delete method that created a deletion record as well as deleting the local record
        /// </summary>
        /// <param name="pContext">the data context</param>
        /// <param name="pEntity">the entry to be deleted</param>
        public static void Delete(Entities pContext, BDAttachment pEntity, bool pCreateDeletion)
        {
            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntity.uuid, pCreateDeletion);
            BDMetadata.DeleteForItemId(pContext, pEntity.uuid, pCreateDeletion);
            if (pCreateDeletion)
                BDDeletion.CreateBDDeletion(pContext, KEY_NAME, pEntity);
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
            BDAttachment entity = BDAttachment.RetrieveWithId(pContext, pUuid);
            Delete(pContext, entity, pCreateDeletion);
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
                BDAttachment entry = BDAttachment.RetrieveWithId(pContext, pUuid.Value);
                if (null != entry)
                {
                    pContext.DeleteObject(entry);
                }
            }
        }

        /// <summary>
        /// Return the Attachment for the uuid. Returns null if not found.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pEntryId"></param>
        /// <returns></returns>
        public static BDAttachment RetrieveWithId(Entities pContext, Guid? pEntryId)
        {
            BDAttachment result = null;

            if (null != pEntryId)
            {
                IQueryable<BDAttachment> entries = (from entry in pContext.BDAttachments
                                                  where entry.uuid == pEntryId
                                                  select entry);

                if (entries.Count<BDAttachment>() > 0)
                    result = entries.AsQueryable().First<BDAttachment>();
            }

            return result;
        }

        public static List<BDAttachment> RetrieveAttachmentForParentId(Entities pContext, Guid? pParentId)
        {
            IQueryable<BDAttachment> entries = (from entry in pContext.BDAttachments
                                              where entry.parentId == pParentId
                                                orderby entry.displayOrder
                                              select entry);

            return entries.ToList<BDAttachment>();
        }

        public static List<IBDObject> RetrieveAll(Entities pContext)
        {
            IQueryable<BDAttachment> entries = (from entry in pContext.BDAttachments
                       select entry);
            return entries.ToList<BDAttachment>().ToList<IBDObject>();
        }

        protected override void OnPropertyChanged(string property)
        {
            if (!BDCommon.Settings.IsSyncLoad)
            {
                switch (property)
                {
                    case "modifiedDate":
                        break;
                    default:
                        modifiedDate = DateTime.Now;
                        break;
                }
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
            IQueryable<BDAttachment> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDAttachments
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDAttachments
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }

            if (entries.Count() > 0)
                entryList = new List<IBDObject>(entries.ToList<BDAttachment>());

            return entryList;
        }

        public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDAttachment.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            if (null != pCurrentSyncDate)
            {
                for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
                {
                    ((BDAttachment)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
                }
                if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
            }
            return syncInfo;
        }

        public static SyncInfo SyncInfo(Entities pDataContext)
        {
            SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
            syncInfo.PushList = BDAttachment.RetrieveAll(pDataContext);
            syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
            return syncInfo;
        }

        /// <summary>
        /// Create or update an existing BDAttachment from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid dataUuid = Guid.Parse(pAttributeDictionary[UUID]);
            BDAttachment entry = BDAttachment.RetrieveWithId(pDataContext, dataUuid);

            if (null == entry)
            {
                int nt = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
                BDConstants.BDNodeType nodeType = BDConstants.BDNodeType.None;

                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), nt))
                {
                    nodeType = (BDConstants.BDNodeType)nt;
                }

                entry = BDAttachment.CreateBDAttachment(dataUuid);
                entry.nodeType = nt;

                pDataContext.AddObject(ENTITYNAME, entry);
            }

            entry.nodeType = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
            entry.schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.name = pAttributeDictionary[NAME];

            entry.nodeKeyName = pAttributeDictionary[NODEKEYNAME];
            entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
            entry.parentType = (null == pAttributeDictionary[PARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[PARENTTYPE]);
            entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

            entry.storageKey = pAttributeDictionary[STORAGEKEY];
            entry.attachmentMimeType = short.Parse(pAttributeDictionary[ATTACHMENTMIMETYPE]);

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return dataUuid;
        }

        public static string GenerateStorageKey(BDAttachment pEntity)
        {
            string extension = pEntity.MimeFileExtension();
            string result = GenerateStorageKey(pEntity.Uuid, extension);
            return result;
        }

        public static string GenerateStorageKey(Guid pUuid, string pExtension)
        {
            string result = string.Format("{0}{1}{2}", AWS_S3_PREFIX, pUuid.ToString().ToUpper(), pExtension);
            return result;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.NODETYPE).WithValue(string.Format(@"{0}", nodeType)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.NODEKEYNAME).WithValue((null == nodeKeyName) ? string.Empty : nodeKeyName).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.ATTACHMENTMIMETYPE).WithValue(string.Format(@"{0}", MimeType())).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.STORAGEKEY).WithValue((null == storageKey) ? Guid.Empty.ToString() : storageKey.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDAttachment.FILENAME).WithValue((null == filename) ? string.Empty : filename).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

        public Guid? ParentId
        {
            get { return parentId; }
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

        public BDConstants.BDNodeType NodeType
        {
            get
            {
                BDConstants.BDNodeType result = BDConstants.BDNodeType.None;

                if (Enum.IsDefined(typeof(BDConstants.BDNodeType), nodeType))
                {
                    result = (BDConstants.BDNodeType)nodeType;
                }
                return result;
            }
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
        { get; set; }

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
            get { return string.Format("{0}: {1}", BDUtilities.GetEnumDescription(NodeType), this.name); }
        }

        public override string ToString()
        {
            return this.name;
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public int? DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }

        public string MimeType()
        {
            BDConstants.BDAttachmentMimeType mt = BDConstants.BDAttachmentMimeType.unknown;
            if( (null != attachmentMimeType) && attachmentMimeType.HasValue)
                mt = (BDConstants.BDAttachmentMimeType)attachmentMimeType.Value;

            return BDUtilities.GetEnumDescription(mt);
        }

        public string MimeFileExtension()
        {
            string extension = ".data";

            if (attachmentMimeType.HasValue)
            {
                switch (attachmentMimeType.Value)
                {
                    case (int)BDConstants.BDAttachmentMimeType.bmp:
                        extension = ".bmp";
                        break;
                    case (int)BDConstants.BDAttachmentMimeType.gif:
                        extension = ".gif";
                        break;
                    case (int)BDConstants.BDAttachmentMimeType.jpeg:
                        extension = ".jpg";
                        break;
                    case (int)BDConstants.BDAttachmentMimeType.pdf:
                        extension = ".pdf";
                        break;
                    case (int)BDConstants.BDAttachmentMimeType.png:
                        extension = ".png";
                        break;
                    case (int)BDConstants.BDAttachmentMimeType.tiff:
                        extension = ".tif";
                        break;
                    default:
                        extension = ".data";
                        break;
                }
            }

            return extension;
        }
    }
}

