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

            entry.nodeType = (int)BDConstants.BDNodeType.BDConfiguredEntry;
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

        static public List<BDConfiguredEntry> RetrieveListForParentId(Entities pDataContext, Guid pParentId)
        {
            List<BDConfiguredEntry> entryList = new List<BDConfiguredEntry>();
            IQueryable<BDConfiguredEntry> entries = (from entry in pDataContext.BDConfiguredEntries
                                          where entry.parentId == pParentId
                                          orderby entry.displayOrder
                                          select entry);
            if (entries.Count<BDConfiguredEntry>() > 0)
                entryList = entries.ToList<BDConfiguredEntry>();

            return entryList;
        }

        public static List<BDConfiguredEntry> RetrieveConfiguredEntriesContainingString(Entities pContext, string pString)
        {
            List<BDConfiguredEntry> returnList = new List<BDConfiguredEntry>();
            if (null != pString && pString.Length > 0)
            {
                IQueryable<BDConfiguredEntry> entries = (from entry in pContext.BDConfiguredEntries
                                                 where entry.name.Contains(pString) || entry.field01.Contains(pString) || entry.field02.Contains(pString) ||
                                                 entry.field03.Contains(pString) || entry.field04.Contains(pString) || entry.field05.Contains(pString) ||
                                                 entry.field06.Contains(pString) || entry.field07.Contains(pString) || entry.field08.Contains(pString) ||
                                                 entry.field09.Contains(pString) || entry.field10.Contains(pString) || entry.field11.Contains(pString) ||
                                                 entry.field12.Contains(pString) || entry.field13.Contains(pString) || entry.field14.Contains(pString) ||
                                                 entry.field15.Contains(pString)
                                                 select entry);
                returnList = entries.ToList<BDConfiguredEntry>();
            }
            return returnList;
        }

        public static BDConfiguredEntry RetrieveConfiguredEntryWithId(Entities pContext, Guid pUuid)
        {
            List<BDConfiguredEntry> entryList = new List<BDConfiguredEntry>();
            IQueryable<BDConfiguredEntry> entries = (from entry in pContext.BDConfiguredEntries
                                                     where entry.uuid == pUuid
                                                     orderby entry.displayOrder
                                                     select entry);
            if (entries.Count<BDConfiguredEntry>() > 0)
                entryList = entries.ToList<BDConfiguredEntry>();

            return entryList.FirstOrDefault<BDConfiguredEntry>();

        }

        public string PropertyValueForIndex(int pIndex)
        {
            string result;

            switch (pIndex)
            {
                case 0:
                    result = this.Name;
                    break;
                case 1:
                    result = this.field01;
                    break;
                case 2:
                    result = this.field02;
                    break;
                case 3:
                    result = this.field03;
                    break;
                case 4:
                    result = this.field04;
                    break;
                case 5:
                    result = this.field05;
                    break;
                case 6:
                    result = this.field06;
                    break;
                case 7:
                    result = this.field07;
                    break;
                case 8:
                    result = this.field08;
                    break;
                case 9:
                    result = this.field09;
                    break;
                case 10:
                    result = this.field10;
                    break;
                case 11:
                    result = this.field11;
                    break;
                case 12:
                    result = this.field12;
                    break;
                case 13:
                    result = this.field13;
                    break;
                case 14:
                    result = this.field14;
                    break;
                case 15:
                    result = this.field15;
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }
        public static string PropertyNameForIndex(int pIndex)
        {
            string result;

            switch (pIndex)
            {
                case 0:
                    result = PROPERTYNAME_NAME;
                    break;
                case 1:
                    result = PROPERTYNAME_FIELD01;
                    break;
                case 2:
                    result = PROPERTYNAME_FIELD02;
                    break;
                case 3:
                    result = PROPERTYNAME_FIELD03;
                    break;
                case 4:
                    result = PROPERTYNAME_FIELD04;
                    break;
                case 5:
                    result = PROPERTYNAME_FIELD05;
                    break;
                case 6:
                    result = PROPERTYNAME_FIELD06;
                    break;
                case 7:
                    result = PROPERTYNAME_FIELD07;
                    break;
                case 8:
                    result = PROPERTYNAME_FIELD08;
                    break;
                case 9:
                    result = PROPERTYNAME_FIELD09;
                    break;
                case 10:
                    result = PROPERTYNAME_FIELD10;
                    break;
                case 11:
                    result = PROPERTYNAME_FIELD11;
                    break;
                case 12:
                    result = PROPERTYNAME_FIELD12;
                    break;
                case 13:
                    result = PROPERTYNAME_FIELD13;
                    break;
                case 14:
                    result = PROPERTYNAME_FIELD14;
                    break;
                case 15:
                    result = PROPERTYNAME_FIELD15;
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
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
                return BDConstants.BDNodeType.BDConfiguredEntry;
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
