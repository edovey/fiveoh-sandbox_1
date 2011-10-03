using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDSection
    /// </summary>
    public partial class BDSection
    {
        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDSection CreateSection(Entities pContext)
        {
            //using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            //{
                BDSection section = CreateBDSection(Guid.NewGuid(), false);
                section.createdBy = Guid.Empty;
                section.createdDate = DateTime.Now;
                section.schemaVersion = 0;

                pContext.AddObject("BDSections", section);

                return section;
            //}
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pSection"></param>
        public static void SaveSection(Entities pContext, BDSection pSection)
        {
            if (pSection.EntityState != EntityState.Unchanged)
            {
                pSection.modifiedBy = Guid.Empty;
                pSection.modifiedDate = DateTime.Now;

                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get Section with the specified ID
        /// </summary>
        /// <param name="pSectionId"></param>
        /// <returns>BDSection object.</returns>
        public static BDSection GetSectionWithId(Entities pContext, Guid pSectionId)
        {
            BDSection section;
            
                IQueryable<BDSection> sections = (from bdSections in pContext.BDSections
                                                     where bdSections.uuid == pSectionId
                                                     select bdSections);
                section = sections.AsQueryable().First<BDSection>();
            return section;
        }
    }
}
