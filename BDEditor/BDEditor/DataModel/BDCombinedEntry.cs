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

        public const string VIRTUALPROPERTYNAME_ENTRYDETAIL = @"Detail";
        public const string VIRTUALPROPERTYNAME_ENTRYTITLE = @"Title";

        public const string PROPERTYNAME_NAME = @"Name";
        public const string PROPERTYNAME_GROUPTITLE = @"GroupTitle";
        public const string PROPERTYNAME_ENTRYTITLE01 = @"EntryTitle01";
        public const string PROPERTYNAME_ENTRY01 = @"EntryDetail01";
        public const string PROPERTYNAME_ENTRYTITLE02 = @"EntryTitle02";
        public const string PROPERTYNAME_ENTRY02 = @"EntryDetail02";
        public const string PROPERTYNAME_ENTRYTITLE03 = @"EntryTitle03";
        public const string PROPERTYNAME_ENTRY03 = @"EntryDetail03";
        public const string PROPERTYNAME_ENTRYTITLE04 = @"EntryTitle04";
        public const string PROPERTYNAME_ENTRY04 = @"EntryDetail04";

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

        public static List<BDCombinedEntry> RetrieveCombinedEntryContainingString(Entities pContext, string pString)
        {
            List<BDCombinedEntry> returnList = new List<BDCombinedEntry>();
            if (null != pString && pString.Length > 0)
            {
                IQueryable<BDCombinedEntry> entries = (from entry in pContext.BDCombinedEntries
                                                 where entry.entryTitle01.Contains(pString) || entry.entryDetail01.Contains(pString) ||
                                                 entry.entryTitle02.Contains(pString) || entry.entryDetail02.Contains(pString) ||
                                                 entry.entryTitle03.Contains(pString) || entry.entryDetail03.Contains(pString) ||
                                                 entry.entryTitle04.Contains(pString) || entry.entryDetail04.Contains(pString)
                                                 select entry);
                returnList = entries.ToList<BDCombinedEntry>();
            }
            return returnList;
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

        public BDConstants.BDJoinType GroupJoinType
        {
            get
            {
                BDConstants.BDJoinType value = BDConstants.BDJoinType.None;
                if (null != groupJoinType) value = (BDConstants.BDJoinType)groupJoinType;
                return value;
            }
        }
        public BDConstants.BDJoinType JoinType01
        {
            get
            {
                BDConstants.BDJoinType value = BDConstants.BDJoinType.None;
                if (null != entryJoinType01) value = (BDConstants.BDJoinType)entryJoinType01;
                return value;
            }
        }
        public BDConstants.BDJoinType JoinType02
        {
            get
            {
                BDConstants.BDJoinType value = BDConstants.BDJoinType.None;
                if (null != entryJoinType02) value = (BDConstants.BDJoinType)entryJoinType02;
                return value;
            }
        }
        public BDConstants.BDJoinType JoinType03
        {
            get
            {
                BDConstants.BDJoinType value = BDConstants.BDJoinType.None;
                if (null != entryJoinType03) value = (BDConstants.BDJoinType)entryJoinType03;
                return value;
            }
        }
        public BDConstants.BDJoinType JoinType04
        {
            get
            {
                BDConstants.BDJoinType value = BDConstants.BDJoinType.None;
                if (null != entryJoinType04) value = (BDConstants.BDJoinType)entryJoinType04;
                return value;
            }
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
