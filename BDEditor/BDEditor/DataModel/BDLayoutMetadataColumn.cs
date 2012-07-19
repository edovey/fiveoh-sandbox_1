using System;
using System.Collections.Generic;
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

        public static void Delete(Entities pContext, BDLayoutMetadataColumn pEntry)
        {
            if (null == pEntry) return;

            BDLinkedNoteAssociation.DeleteForParentId(pContext, pEntry.Uuid, false);
            
            // delete record from local data store
            pContext.DeleteObject(pEntry);
            pContext.SaveChanges();
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


        public string Description
        {
            get { return this.label; }
        }

        public string DescriptionForLinkedNote
        {
            get { return this.label; }
        }

        public Amazon.SimpleDB.Model.PutAttributesRequest PutAttributes()
        {
            return null;
        }
    }
}
