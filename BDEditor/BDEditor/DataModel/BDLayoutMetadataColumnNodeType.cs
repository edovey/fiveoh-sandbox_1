using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public partial class BDLayoutMetadataColumnNodeType
    {
        public const string ENTITYNAME = @"BDLayoutMetadataColumnNodeTypes";
        public const string ENTITYNAME_FRIENDLY = @"Layout Metadata Column NodeType";
        public const string KEY_NAME = @"BDLayoutMetadataColumnNodeType";

        static public BDLayoutMetadataColumnNodeType Create(Entities pDataContext, int pLayoutVariant, Guid pLayoutColumnId, BDConstants.BDNodeType pNodeType, string pPropertyName, int pOrderOfPrecedence)
        {
            BDLayoutMetadataColumnNodeType entry = BDLayoutMetadataColumnNodeType.CreateBDLayoutMetadataColumnNodeType(pLayoutVariant, pLayoutColumnId, (int)pNodeType, Guid.NewGuid());
            entry.propertyName = pPropertyName;
            entry.orderOfPrecedence = pOrderOfPrecedence;
            pDataContext.AddObject(ENTITYNAME, entry);
            pDataContext.SaveChanges();
            return entry;
        }

        static public List<BDLayoutMetadataColumnNodeType> RetrieveListForLayoutColumn(Entities pDataContext, Guid pColumnId)
        {
            List<BDLayoutMetadataColumnNodeType> existingEntryList = new List<BDLayoutMetadataColumnNodeType>();
            IQueryable<BDLayoutMetadataColumnNodeType> existingEntries = null;

            existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumnNodeTypes
                                where (dbEntry.columnId == pColumnId)
                                orderby dbEntry.orderOfPrecedence
                                select dbEntry);
            existingEntryList = existingEntries.ToList<BDLayoutMetadataColumnNodeType>();

            return existingEntryList;
        }

        static public List<BDLayoutMetadataColumnNodeType> RetrieveListForLayoutColumn(Entities pDataContext, Guid pColumnId, BDConstants.BDNodeType pNodeType)
        {
            List<BDLayoutMetadataColumnNodeType> existingEntryList = new List<BDLayoutMetadataColumnNodeType>();
            IQueryable<BDLayoutMetadataColumnNodeType> existingEntries = null;

            existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumnNodeTypes
                               where (dbEntry.columnId == pColumnId) && (dbEntry.nodeType == (int)pNodeType)
                               orderby dbEntry.orderOfPrecedence
                               select dbEntry);
            existingEntryList = existingEntries.ToList<BDLayoutMetadataColumnNodeType>();

            return existingEntryList;
        }

        static public BDLayoutMetadataColumnNodeType Retrieve(Entities pDataContext, BDConstants.LayoutVariantType pLayoutVariant, BDConstants.BDNodeType pNodeType, string pPropertyName)
        {
            BDLayoutMetadataColumnNodeType entry = null;

            IQueryable<BDLayoutMetadataColumnNodeType> existingEntries = null;

            existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumnNodeTypes
                               where (dbEntry.layoutVariant == (int)pLayoutVariant) && (dbEntry.nodeType == (int)pNodeType) && (dbEntry.propertyName == pPropertyName)
                               orderby dbEntry.orderOfPrecedence
                               select dbEntry);

            entry = existingEntries.FirstOrDefault<BDLayoutMetadataColumnNodeType>();
            return entry;
        }

        static public BDLayoutMetadataColumnNodeType Retrieve(Entities pDataContext, BDConstants.LayoutVariantType pLayoutVariant, BDConstants.BDNodeType pNodeType, Guid pColumnId)
        {
            BDLayoutMetadataColumnNodeType entry = null;

            IQueryable<BDLayoutMetadataColumnNodeType> existingEntries = null;

            existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumnNodeTypes
                               where (dbEntry.layoutVariant == (int)pLayoutVariant) && (dbEntry.nodeType == (int)pNodeType) && (dbEntry.columnId == pColumnId)
                               orderby dbEntry.orderOfPrecedence
                               select dbEntry);

            entry = existingEntries.FirstOrDefault<BDLayoutMetadataColumnNodeType>();
            return entry;
        }

        static public bool ExistsForLayoutColumn(Entities pDataContext, Guid pColumnId, BDConstants.BDNodeType pNodeType, string pPropertyName)
        {
            bool exists = false;

            IQueryable<BDLayoutMetadataColumnNodeType> existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumnNodeTypes
                                                                           where (dbEntry.columnId == pColumnId) 
                                                                           && (dbEntry.nodeType == (int)pNodeType) 
                                                                           && (dbEntry.propertyName == pPropertyName)
                                                                           select dbEntry);
            exists = (existingEntries.Count() > 0);
            return exists;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public override string ToString()
        {
            BDConstants.BDNodeType theNodeType = (BDConstants.BDNodeType)this.nodeType;
            string description = BDUtilities.GetEnumDescription(theNodeType);
            return string.Format("{0} [{1}]", description, this.propertyName);
        }
    }
}
