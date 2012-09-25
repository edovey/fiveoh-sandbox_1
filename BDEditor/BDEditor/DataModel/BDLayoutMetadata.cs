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
    public partial class BDLayoutMetadata
    {
        public const string ENTITYNAME = @"BDLayoutMetadatas";
        public const string ENTITYNAME_FRIENDLY = @"Layout Metadata";
        public const string KEY_NAME = @"BDLayoutMetadata";

        static public void Rebuild(Entities pDataContext)
        {

            List<BDLayoutMetadata> existingEntryList = new List<BDLayoutMetadata>();

            IQueryable<BDLayoutMetadata> existingEntries = (from dbEntry in pDataContext.BDLayoutMetadatas
                                                            orderby dbEntry.layoutVariant
                                                            select dbEntry);

            int order = existingEntries.Count<BDLayoutMetadata>() == 0 ? 0 : existingEntries.Max<BDLayoutMetadata>(x => x.displayOrder) + 1;

            foreach (BDConstants.LayoutVariantType layoutVariant in Enum.GetValues(typeof(BDConstants.LayoutVariantType)))
            {
                BDLayoutMetadata entry = null;

                entry = existingEntries.FirstOrDefault<BDLayoutMetadata>(x => x.layoutVariant == (int)layoutVariant);

                if (null != entry)
                {
                    entry.descrip = BDUtilities.GetEnumDescription(layoutVariant);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("empty");
                    entry = BDLayoutMetadata.CreateBDLayoutMetadata((int)layoutVariant, false, order++);
                    entry.descrip = BDUtilities.GetEnumDescription(layoutVariant);
                    pDataContext.AddObject(ENTITYNAME, entry);
                }
            }

            pDataContext.SaveChanges();
        }

        static public List<BDLayoutMetadata> RetrieveAll(Entities pDataContext, Boolean? pIncluded)
        {
            List<BDLayoutMetadata> existingEntryList = new List<BDLayoutMetadata>();

            IQueryable<BDLayoutMetadata> existingEntries = null;

            if (pIncluded.HasValue)
            {
                existingEntries = (from dbEntry in pDataContext.BDLayoutMetadatas
                                        where (dbEntry.included == pIncluded.Value)
                                        orderby dbEntry.descrip
                                        select dbEntry);
            }
            else
            {
                existingEntries = (from dbEntry in pDataContext.BDLayoutMetadatas
                                        orderby dbEntry.descrip
                                        select dbEntry);
                
            }

            existingEntryList = existingEntries.ToList<BDLayoutMetadata>();
            return existingEntryList;
        }

        static public BDLayoutMetadata Retrieve(Entities pDataContext, BDConstants.LayoutVariantType pLayoutVariant)
        {
            IQueryable<BDLayoutMetadata> existingEntries = (from dbEntry in pDataContext.BDLayoutMetadatas
                                                            where (dbEntry.layoutVariant == (int)pLayoutVariant)
                                                            select dbEntry);

            BDLayoutMetadata entry = existingEntries.FirstOrDefault<BDLayoutMetadata>();
            return entry;
        }

        public static void Save(Entities pContext, BDLayoutMetadata pLayoutMetadata)
        {
            if (null != pLayoutMetadata)
            {
                if (pLayoutMetadata.EntityState != EntityState.Unchanged)
                {
                    pContext.SaveChanges();
                }
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", this.included ? "x" : " ", this.descrip );
        }
    }
}
