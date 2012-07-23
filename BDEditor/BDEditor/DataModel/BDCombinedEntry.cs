using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public partial class BDCombinedEntry: IBDNode
    {
        public const string ENTITYNAME = @"BDCombinedEntries";
        public const string ENTITYNAME_FRIENDLY = @"Combined Entry";
        public const string KEY_NAME = @"BDCombinedEntry";

        public const int ENTITY_SCHEMAVERSION = 0;

        public const string PROPERTYNAME_NAME = @"Name";
        public const string PROPERTYNAME_GROUPTITLE = @"GroupTitle";
        public const string PROPERTYNAME_NAME = @"Name";
        public const string PROPERTYNAME_ENTRYTITLE01 = @"EntryTitle01";
        public const string PROPERTYNAME_ENTRYDETAIL01 = @"EntryDetail01";
        public const string PROPERTYNAME_ENTRYTITLE02 = @"EntryTitle02";
        public const string PROPERTYNAME_ENTRYDETAIL02 = @"EntryDetail02";
        public const string PROPERTYNAME_ENTRYTITLE03 = @"EntryTitle03";
        public const string PROPERTYNAME_ENTRYDETAIL03 = @"EntryDetail03";
        public const string PROPERTYNAME_ENTRYTITLE04 = @"EntryTitle04";
        public const string PROPERTYNAME_ENTRYDETAIL04 = @"EntryDetail04";

        static public BDCombinedEntry Create(Entities pDataContext, BDConstants.LayoutVariantType pLayoutVariant, Guid pParentUuid, BDConstants.BDNodeType pParentNodeType, string pName)
        {
            BDCombinedEntry entry = BDCombinedEntry.CreateBDCombinedEntry(Guid.NewGuid());

            entry.nodeType = (int)BDConstants.BDNodeType.BDCombinedEntry;
            entry.LayoutVariant = pLayoutVariant;
            entry.SetParent(pParentNodeType, pParentUuid);
            entry.createdDate = DateTime.Now;
            entry.schemaVersion = ENTITY_SCHEMAVERSION;
            entry.name = pName;

            pDataContext.AddObject(ENTITYNAME, entry);
            pDataContext.SaveChanges();

            return entry;
        }

        static public void Save(Entities pDataContext, BDCombinedEntry pEntry)
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

        static public void Delete(Entities pContext, BDCombinedEntry pEntry, bool pCreateDeletion)
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

        static public List<BDCombinedEntry> RetrieveListForParentId(Entities pDataContext, Guid pParentId)
        {
            List<BDCombinedEntry> entryList = new List<BDCombinedEntry>();
            IQueryable<BDCombinedEntry> entries = (from entry in pDataContext.BDCombinedEntries
                                                     where entry.parentId == pParentId
                                                     orderby entry.displayOrder
                                                     select entry);
            if (entries.Count<BDCombinedEntry>() > 0)
                entryList = entries.ToList<BDCombinedEntry>();

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
            get { return name; }
            set { name = value; }
        }

        public Classes.BDConstants.BDNodeType NodeType
        {
            get { return BDConstants.BDNodeType.BDCombinedEntry; }
        }

        public Classes.BDConstants.LayoutVariantType LayoutVariant
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
