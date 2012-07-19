using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public partial class BDLayoutMetadataColumn
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

        static public List<BDLayoutMetadataColumn> RetrieveForLayout(Entities pDataContext, BDLayoutMetadata pLayout)
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

        public override string ToString()
        {
            return this.label;
        }

        public Guid Uuid
        {
            get { return this.uuid; }
        }
    }
}
