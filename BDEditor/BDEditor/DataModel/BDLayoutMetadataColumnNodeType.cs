﻿using System;
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

        static public BDLayoutMetadataColumnNodeType Create(Entities pDataContext, int pLayoutVariant, Guid pLayoutColumnId, BDConstants.BDNodeType pNodeType, string pPropertyName)
        {
            BDLayoutMetadataColumnNodeType entry = BDLayoutMetadataColumnNodeType.CreateBDLayoutMetadataColumnNodeType(pLayoutVariant, pLayoutColumnId, (int)pNodeType, Guid.NewGuid());
            pDataContext.AddObject(ENTITYNAME, entry);
            pDataContext.SaveChanges();
            return entry;
        }

        static public List<BDLayoutMetadataColumnNodeType> RetrieveForLayoutColumn(Entities pDataContext, Guid pColumnId)
        {
            List<BDLayoutMetadataColumnNodeType> existingEntryList = new List<BDLayoutMetadataColumnNodeType>();
            IQueryable<BDLayoutMetadataColumnNodeType> existingEntries = null;

            existingEntries = (from dbEntry in pDataContext.BDLayoutMetadataColumnNodeTypes
                                where (dbEntry.columnId == pColumnId)
                                select dbEntry);
            existingEntryList = existingEntries.ToList<BDLayoutMetadataColumnNodeType>();

            return existingEntryList;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }

        public override string ToString()
        {
            BDConstants.BDNodeType theNodeType = (BDConstants.BDNodeType)this.nodeType;
            string description = BDUtilities.GetEnumDescription(theNodeType);
            return string.Format("{0} {1}", description, this.propertyName);
        }
    }
}
