using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public partial class BDConfiguredEntry: IBDNode
    {
        public const string ENTITYNAME = @"BDConfiguredEntries";
        public const string ENTITYNAME_FRIENDLY = @"Configured Entry";
        public const string KEY_NAME = @"BDConfiguredEntry";

        public const int ENTITY_SCHEMAVERSION = 0;

        public const string PROPERTYNAME_NAME = @"Name";
        public const string PROPERTYNAME_FIELD01 = @"Field01";
        public const string PROPERTYNAME_FIELD02 = @"Field02";
        public const string PROPERTYNAME_FIELD03 = @"Field03";
        public const string PROPERTYNAME_FIELD04 = @"Field04";
        public const string PROPERTYNAME_FIELD05 = @"Field05";
        public const string PROPERTYNAME_FIELD06 = @"Field06";
        public const string PROPERTYNAME_FIELD07 = @"Field07";
        public const string PROPERTYNAME_FIELD08 = @"Field08";
        public const string PROPERTYNAME_FIELD09 = @"Field09";
        public const string PROPERTYNAME_FIELD10 = @"Field10";
        public const string PROPERTYNAME_FIELD11 = @"Field11";
        public const string PROPERTYNAME_FIELD12 = @"Field12";
        public const string PROPERTYNAME_FIELD13 = @"Field13";
        public const string PROPERTYNAME_FIELD14 = @"Field14";
        public const string PROPERTYNAME_FIELD15 = @"Field15";

        static public BDConfiguredEntry Create(Entities pDataContext, BDConstants.LayoutVariantType pLayoutVariant, Guid pParentUuid, BDConstants.BDNodeType pParentNodeType, string pName)
        {
            BDConfiguredEntry entry = BDConfiguredEntry.CreateBDConfiguredEntry(Guid.NewGuid());

            entry.LayoutVariant = pLayoutVariant;
            entry.SetParent(pParentNodeType, pParentUuid);
            entry.createdDate = DateTime.Now;
            entry.schemaVersion = ENTITY_SCHEMAVERSION;
            entry.name = pName;

            pDataContext.AddObject(ENTITYNAME, entry);
            pDataContext.SaveChanges();

            return entry;
        }

        static public void Save(Entities pDataContext, BDConfiguredEntry pEntry)
        {
            if (null != pEntry)
            {
                if (pEntry.EntityState != System.Data.EntityState.Unchanged)
                {
                    if (pEntry.schemaVersion != ENTITY_SCHEMAVERSION)
                        pEntry.schemaVersion = ENTITY_SCHEMAVERSION;

                    pDataContext.SaveChanges();
                }
            }
        }

        static public void Delete(Entities pContext, BDConfiguredEntry pEntry, bool pCreateDeletion)
        {
            if (null == pEntry) return;

            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntry.Uuid, pCreateDeletion);

            BDMetadata.DeleteForItemId(pContext, pEntry.Uuid, pCreateDeletion);
            // create BDDeletion record for the object to be deleted
            if (pCreateDeletion)
                BDDeletion.CreateBDDeletion(pContext, KEY_NAME, pEntry);
            // delete record from local data store
            pContext.DeleteObject(pEntry);
            pContext.SaveChanges();
        }

        static public List<BDConfiguredEntry> RetrieveForParentId(Entities pDataContext, Guid pParentId)
        {
            List<BDConfiguredEntry> entryList = new List<BDConfiguredEntry>();
            IQueryable<BDConfiguredEntry> entries = (from entry in pDataContext.BDConfiguredEntries
                                          where entry.parentId == pParentId
                                          orderby entry.displayOrder
                                          select entry);
            if (entries.Count() > 0)
                entryList = entries.ToList<BDConfiguredEntry>();

            return entryList;
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
                            //modifiedBy = Guid.Empty;
                            modifiedDate = DateTime.Now;
                        }
                        break;
                }

            base.OnPropertyChanged(property);
        }

        #region IBDNode

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
            set
            {
                layoutVariant = (int)value;
            }
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
            get { return uuid; }
        }

        public string Description
        {
            get { return name; }
        }

        public string DescriptionForLinkedNote
        {
            get { return name; }
        }

        public Amazon.SimpleDB.Model.PutAttributesRequest PutAttributes()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
