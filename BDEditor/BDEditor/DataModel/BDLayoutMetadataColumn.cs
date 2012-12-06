using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;
using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public partial class BDLayoutMetadataColumn: IBDObject
    {
        public const string ENTITYNAME = @"BDLayoutMetadataColumns";
        public const string ENTITYNAME_FRIENDLY = @"Layout Metadata Column";
        public const string KEY_NAME = @"BDLayoutMetadataColumn";

        public const string PROPERTYNAME_LABEL = "Label";

        static public BDLayoutMetadataColumn Create(Entities pDataContext, BDLayoutMetadata pLayoutMetadata, int pDisplayOrder)
        {
            BDLayoutMetadataColumn entry = BDLayoutMetadataColumn.CreateBDLayoutMetadataColumn(Guid.NewGuid(), pLayoutMetadata.layoutVariant, pDisplayOrder);
            pDataContext.AddObject(ENTITYNAME, entry);
            pDataContext.SaveChanges();
            return entry;
        }

        static public BDLayoutMetadataColumn Create(Entities pDataContext, BDLayoutMetadata pLayoutMetadata, int pDisplayOrder, string pLabel)
        {
            BDLayoutMetadataColumn entry = BDLayoutMetadataColumn.Create(pDataContext, pLayoutMetadata, pDisplayOrder);
            entry.label = pLabel;
            pDataContext.SaveChanges();
            return entry;
        }

        public static void Save(Entities pContext, BDLayoutMetadataColumn pLayoutMetadataColumn)
        {
            if (null != pLayoutMetadataColumn)
            {
                if (pLayoutMetadataColumn.EntityState != EntityState.Unchanged)
                {
                    pContext.SaveChanges();
                }
            }
        }

        public static void Delete(Entities pContext, BDLayoutMetadataColumn pEntry)
        {
            if (null == pEntry) return;

            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntry.Uuid, false);
            
            // delete record from local data store
            pContext.DeleteObject(pEntry);
            pContext.SaveChanges();
        }

        static public List<BDLayoutMetadataColumn> RetrieveListForLayout(Entities pDataContext, BDLayoutMetadata pLayout)
        {
            List<BDLayoutMetadataColumn> existingEntryList = new List<BDLayoutMetadataColumn>();
            IQueryable<BDLayoutMetadataColumn> existingEntries = null;

            if (null != pLayout)
            {
                existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumns
                                   where (dbEntry.layoutVariant == pLayout.layoutVariant)
                                   orderby dbEntry.displayOrder
                                   select dbEntry);
                existingEntryList = existingEntries.ToList<BDLayoutMetadataColumn>();
            }

            return existingEntryList;
        }

        static public List<BDLayoutMetadataColumn> RetrieveListForLayout(Entities pDataContext, BDConstants.LayoutVariantType pLayoutVariant)
        {
            IQueryable<BDLayoutMetadataColumn> existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumns
                                                                    where (dbEntry.layoutVariant == (int)pLayoutVariant)
                                                                    orderby dbEntry.displayOrder
                                                                    select dbEntry);
            List<BDLayoutMetadataColumn> existingEntryList = existingEntries.ToList<BDLayoutMetadataColumn>();

            return existingEntryList;
        }

        static public BDLayoutMetadataColumn Retrieve(Entities pDataContext, Guid pUuid)
        {
            IQueryable<BDLayoutMetadataColumn> existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumns
                                                                   where (dbEntry.uuid == pUuid)
                                                                   select dbEntry);

            BDLayoutMetadataColumn entry = existingEntries.FirstOrDefault<BDLayoutMetadataColumn>();
            return entry;
        }

        static public BDLayoutMetadataColumn Retrieve(Entities pDataContext, BDConstants.LayoutVariantType pLayoutVariant, BDConstants.BDNodeType pNodeType, string pPropertyName)
        {
            BDLayoutMetadataColumn entry = null;

            BDLayoutMetadataColumnNodeType nodeInfo = BDLayoutMetadataColumnNodeType.Retrieve(pDataContext, pLayoutVariant, pNodeType, pPropertyName);
            if (null != nodeInfo)
            {
                entry = Retrieve(pDataContext, nodeInfo.columnId);
            }
            return entry;
        }

        public override string ToString()
        {
            return this.label;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
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

        public string Description
        {
            get { return this.label; }
        }

        public string DescriptionForLinkedNote
        {
            get 
            {
                string layoutDescription = BDUtilities.GetEnumDescription(LayoutVariant);
                return string.Format("{0} [{1}]", this.label, layoutDescription); 
            }
        }

        public List<string> FieldNameListForColumnOfNodeType(Entities pDataContext, BDConstants.BDNodeType pNodeType)
        {
            List<BDLayoutMetadataColumnNodeType> columnMappingList = BDLayoutMetadataColumnNodeType.RetrieveListForLayoutColumn(pDataContext, this.Uuid, pNodeType);
            return columnMappingList.Select(entry => entry.propertyName).ToList();
        }

        public string FieldNameForColumnOfNodeType(Entities pDataContext, BDConstants.BDNodeType pNodeType)
        {
            List<string> fieldnameList = FieldNameListForColumnOfNodeType(pDataContext, pNodeType);
            string result = ((null != fieldnameList) && (fieldnameList.Count > 0)) ? fieldnameList[0] : null;
            return result;
        }
        public Amazon.SimpleDB.Model.PutAttributesRequest PutAttributes()
        {
            return null;
        }
    }
}
