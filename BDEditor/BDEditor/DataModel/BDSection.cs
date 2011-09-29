using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.DataModel;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDSection
    /// </summary>
    public partial class BDSection
    {
        public static BDSection CreateSection(Entities pContext)
        {
            Guid pGuid = Guid.NewGuid();
            BDSection section = CreateBDSection(pGuid, false);
            section.createdBy = Guid.Empty;
            section.createdDate = DateTime.Now;
            section.schemaVersion = 0;

            pContext.AddObject("BDSections", section);
            
            return section;
        }

        public static void SaveSection(Entities pContext, BDSection pSection)
        {
            pSection.modifiedBy = Guid.Empty;
            pSection.modifiedDate = DateTime.Now;

            pContext.SaveChanges();
        }
    }
}
