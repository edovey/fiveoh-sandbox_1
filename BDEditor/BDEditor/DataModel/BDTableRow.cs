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
    /// Extension of generated BDTableRow
    /// </summary>
   public partial class BDTableRow : IBDNode
   {
       public const string AWS_PROD_DOMAIN = @"bd_2_tableRows";
       public const string AWS_DEV_DOMAIN = @"bd_dev_2_tableRows";

       public const string AWS_PROD_BUCKET = @"bdProdStore";
       public const string AWS_DEV_BUCKET = @"bdDevStore";

#if DEBUG
       public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
       public const string AWS_BUCKET = AWS_DEV_BUCKET;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
        public const string AWS_BUCKET = AWS_PROD_BUCKET;
#endif

       public const string ENTITYNAME = @"BDTableRows";
       public const string ENTITYNAME_FRIENDLY = @"Table Rows";
       public const string KEY_NAME = @"BDTableRows";

       public const int ENTITY_SCHEMAVERSION = 0;

       private const string UUID = @"tr_uuid";
       private const string SCHEMAVERSION = @"tr_schemaVersion";
       private const string CREATEDBY = @"tr_createdBy";
       private const string CREATEDDATE = @"tr_createdDate";
       private const string MODIFIEDBY = @"tr_modifiedBy";
       private const string MODIFIEDDATE = @"tr_modifiedDate";
       private const string DISPLAYORDER = @"tr_displayOrder";
       private const string LAYOUTVARIANT = @"tr_layoutVariant";
       private const string ROWTYPE = @"tr_rowType";
       private const string PARENTID = @"tr_parentId";
       private const string PARENTKEYNAME = @"tr_parentKeyName";
       private const string PARENTTYPE = @"tr_parentType";
       private const string NODETYPE = @"tr_nodeType";
       private const string NODEKEYNAME = @"tr_nodeKeyName";
       private const string NAME = @"tr_name";

       public Guid? tempProductionUuid { get; set; }

       public static BDTableRow CreateBDTableRow(Entities pContext, BDConstants.BDNodeType pNodeType)
       {
           return CreateBDTableRow(pContext, pNodeType, Guid.NewGuid());
       }

       /// <summary>
       /// Extended Create method that sets Created date and schema version
       /// </summary>
       /// <param name="pContext"></param>
       /// <param name="pNodeType"></param>
       /// <returns></returns>
       public static BDTableRow CreateBDTableRow(Entities pContext, BDConstants.BDNodeType pNodeType, Guid pUuid)
       {
           BDTableRow row = CreateBDTableRow(pUuid);
           row.nodeType = (int)pNodeType;
           row.nodeKeyName = pNodeType.ToString();
           row.createdBy = Guid.Empty;
           row.createdDate = DateTime.Now;
           row.schemaVersion = ENTITY_SCHEMAVERSION;
           row.displayOrder = -1;
           row.name = string.Empty;

           pContext.AddObject(ENTITYNAME, row);
           return row;
       }

       /// <summary>
       /// Extended Save method
       /// </summary>
       /// <param name="pContext"></param>
       /// <param name="pTableRow"></param>
       public static void Save(Entities pContext, BDTableRow pTableRow)
       {
           if (null != pTableRow)
           {
               if (pTableRow.EntityState != EntityState.Unchanged)
               {
                   if (pTableRow.schemaVersion != ENTITY_SCHEMAVERSION)
                       pTableRow.schemaVersion = ENTITY_SCHEMAVERSION;
                   pContext.SaveChanges();
               }
           }
       }

       /// <summary>
       /// Extended Delete method that creates a deletion record as well as deleting the local record
       /// </summary>
       /// <param name="pContext">the data context</param>
       /// <param name="pNode">the entry to be deleted</param>
       public static void Delete(Entities pContext, IBDNode pNode, bool pCreateDeletion)
       {
           if (null == pNode) return;

           BDLinkedNoteAssociation.DeleteForParentId(pContext, pNode.Uuid, pCreateDeletion);

           BDTableCell.DeleteForParentId(pContext, pNode.Uuid, pCreateDeletion);

           List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
           foreach (IBDNode child in children)
           {
               switch (child.NodeType)
               {
                   case BDConstants.BDNodeType.BDTherapy:
                       BDTherapy therapyChild = child as BDTherapy;
                       BDTherapy.Delete(pContext, therapyChild, pCreateDeletion);
                       break;
                   case BDConstants.BDNodeType.BDTherapyGroup:
                       BDTherapyGroup therapyGroupChild = child as BDTherapyGroup;
                       BDTherapyGroup.Delete(pContext, therapyGroupChild, pCreateDeletion);
                       break;
                   default:
                       BDNode.Delete(pContext, child, pCreateDeletion);
                       break;
               }
           }

           BDMetadata.DeleteForItemId(pContext, pNode.Uuid, pCreateDeletion);
           // create BDDeletion record for the object to be deleted
           if (pCreateDeletion)
               BDDeletion.CreateBDDeletion(pContext, KEY_NAME, pNode.Uuid);
           // delete record from local data store
           pContext.DeleteObject(pNode);
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
           BDTableRow entity = BDTableRow.RetrieveTableRowWithId(pContext, pUuid);
           BDTableRow.Delete(pContext, entity, pCreateDeletion);
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
               BDTableRow entry = BDTableRow.RetrieveTableRowWithId(pContext, pUuid.Value);
               if (null != entry)
               {
                   pContext.DeleteObject(entry);
               }
           }
       }

       /// <summary>
       /// Retrieve Table Row with the specified ID
       /// </summary>
       /// <param name="pParentId"></param>
       /// <returns>BDTableRow object.</returns>
       public static BDTableRow RetrieveTableRowWithId(Entities pContext, Guid pUuid)
       {
           BDTableRow entry = null;

           if (null != pUuid)
           {
               IQueryable<BDTableRow> entries = (from entity in pContext.BDTableRows
                                             where entity.uuid == pUuid
                                             select entity);
               if (entries.Count<BDTableRow>() > 0)
                   entry = entries.AsQueryable().First<BDTableRow>();
           }
           return entry;
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
           IQueryable<BDTableRow> entries;

           if (null == pUpdateDateTime)
           {
               entries = (from entry in pContext.BDTableRows
                          select entry);
           }
           else
           {
               entries = (from entry in pContext.BDTableRows
                          where entry.modifiedDate > pUpdateDateTime.Value
                          select entry);
           }

           if (entries.Count() > 0)
               entryList = new List<IBDObject>(entries.ToList<BDTableRow>());

           return entryList;
       }

       public static SyncInfo SyncInfo(Entities pDataContext, DateTime? pLastSyncDate, DateTime? pCurrentSyncDate)
       {
           SyncInfo syncInfo = new SyncInfo(AWS_DOMAIN, MODIFIEDDATE, AWS_PROD_DOMAIN, AWS_DEV_DOMAIN);
           syncInfo.PushList = BDTableRow.GetEntriesUpdatedSince(pDataContext, pLastSyncDate);
           syncInfo.FriendlyName = ENTITYNAME_FRIENDLY;
           if ((null != pCurrentSyncDate) && (!BDCommon.Settings.RepositoryOverwriteEnabled))
           {
               for (int idx = 0; idx < syncInfo.PushList.Count; idx++)
               {
                   ((BDTableRow)syncInfo.PushList[idx]).modifiedDate = pCurrentSyncDate;
               }
               if (syncInfo.PushList.Count > 0) { pDataContext.SaveChanges(); }
           }
           return syncInfo;
       }

       /// <summary>
       /// Create or update an existing BDTableRow from attributes in a dictionary. Saves the entry.
       /// </summary>
       /// <param name="pDataContext"></param>
       /// <param name="pAttributeDictionary"></param>
       /// <returns>Uuid of the created/updated entry</returns>
       public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
       {
           Guid dataUuid = Guid.Parse(pAttributeDictionary[UUID]);
           BDTableRow entry = BDTableRow.RetrieveTableRowWithId(pDataContext, dataUuid);

           if (null == entry)
           {
               int nt = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
               BDConstants.BDNodeType nodeType = BDConstants.BDNodeType.None;

               if (Enum.IsDefined(typeof(BDConstants.BDNodeType), nt))
               {
                   nodeType = (BDConstants.BDNodeType)nt;
               }

               entry = BDTableRow.CreateBDTableRow(dataUuid);
               entry.nodeType = nt;

               pDataContext.AddObject(ENTITYNAME, entry);
           }
           
           entry.nodeType = (null == pAttributeDictionary[NODETYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[NODETYPE]);
           entry.schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
           entry.displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
           entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
           entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
           entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
           entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
           entry.name = pAttributeDictionary[NAME];

           entry.parentId = Guid.Parse(pAttributeDictionary[PARENTID]);
           entry.parentType = (null == pAttributeDictionary[PARENTTYPE]) ? (short)-1 : short.Parse(pAttributeDictionary[PARENTTYPE]);
           entry.parentKeyName = pAttributeDictionary[PARENTKEYNAME];

           entry.nodeKeyName = pAttributeDictionary[NODEKEYNAME];

           entry.layoutVariant = short.Parse(pAttributeDictionary[LAYOUTVARIANT]);
           entry.rowType = -1;

           if (pSaveChanges)
               pDataContext.SaveChanges();

           return dataUuid;
       }

       public PutAttributesRequest PutAttributes()
       {
           PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
           List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(BDConstants.DATETIMEFORMAT)).WithReplace(true));

           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.NAME).WithValue((null == name) ? string.Empty : name).WithReplace(true));

           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.PARENTID).WithValue((null == parentId) ? Guid.Empty.ToString() : parentId.ToString().ToUpper()).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.PARENTTYPE).WithValue(string.Format(@"{0}", parentType)).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.PARENTKEYNAME).WithValue((null == parentKeyName) ? string.Empty : parentKeyName).WithReplace(true));

           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.NODETYPE).WithValue(string.Format(@"{0}", nodeType)).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.NODEKEYNAME).WithValue((null == nodeKeyName) ? string.Empty : nodeKeyName).WithReplace(true));

           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.LAYOUTVARIANT).WithValue(string.Format(@"{0}", layoutVariant)).WithReplace(true));
           attributeList.Add(new ReplaceableAttribute().WithName(BDTableRow.ROWTYPE).WithValue(string.Format(@"{0}", rowType)).WithReplace(true));

           return putAttributeRequest;
       }
       #endregion


       #region IBDNode implementation
       public string Name
        {
            get { return this.name; }
            set { this.name = value; }
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
            set { layoutVariant = (int)value; }
        }

        public int? DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }

        public Guid? ParentId
        {
            get { return parentId; }
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

        public void SetParent(IBDNode pParent)
        {
            if (null == pParent)
                SetParent(BDConstants.BDNodeType.None, null);
            else
                SetParent(pParent.NodeType, pParent.Uuid);
        }

        public void SetParent(BDConstants.BDNodeType pParentType, Guid? pParentId)
        {
            parentId = pParentId;
            parentType = (int)pParentType;
            parentKeyName = pParentType.ToString();
        }

        #region IBDObject implementation
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

        #endregion
        #endregion
    }
}
