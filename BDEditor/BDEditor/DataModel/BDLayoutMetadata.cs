﻿using System;
using System.Collections.Generic;
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

        static public void Rebuild(Entities pContext)
        {

            List<BDLayoutMetadata> existingEntryList = new List<BDLayoutMetadata>();

            IQueryable<BDLayoutMetadata> existingEntries = (from dbEntry in pContext.BDLayoutMetadatas
                                                            orderby dbEntry.layoutVariant
                                                            select dbEntry);

            int order = existingEntries.Count<BDLayoutMetadata>() == 0 ? 0 : existingEntries.Max<BDLayoutMetadata>(x => x.displayOrder) + 1;

            foreach (BDConstants.LayoutVariantType layoutVariant in Enum.GetValues(typeof(BDConstants.LayoutVariantType)))
            {
                BDLayoutMetadata entry = null;

                //if (existingEntries.Count<BDLayoutMetadata>() > 0)
                //{
                    entry = existingEntries.FirstOrDefault<BDLayoutMetadata>(x => x.layoutVariant == (int)layoutVariant);
                //}

                if (null == entry)
                {
                    System.Diagnostics.Debug.WriteLine("empty");
                    entry = BDLayoutMetadata.CreateBDLayoutMetadata((int)layoutVariant, false, order++);
                    entry.descrip = BDUtilities.GetEnumDescription(layoutVariant);
                    pContext.AddObject(ENTITYNAME, entry);
                }
            }

            pContext.SaveChanges();

            //existingEntryList = existingEntries.ToList<BDLayoutMetadata>();
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

        public override string ToString()
        {
            return string.Format("[{0}] {1}", this.included ? "x" : " ", this.descrip );
        }
    }
}
