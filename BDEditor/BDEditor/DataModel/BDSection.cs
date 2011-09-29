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
        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <returns></returns>
        public static BDSection CreateSection()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDSection section = CreateBDSection(Guid.NewGuid(), false);
                section.createdBy = Guid.Empty;
                section.createdDate = DateTime.Now;
                section.schemaVersion = 0;

                context.AddObject("BDSections", section);

                return section;
            }
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pSection"></param>
        public static void SaveSection(BDSection pSection)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                pSection.modifiedBy = Guid.Empty;
                pSection.modifiedDate = DateTime.Now;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get Section with the specified ID
        /// </summary>
        /// <param name="pSectionId"></param>
        /// <returns>BDSection object.</returns>
        public static BDSection GetSectionWithId(Guid pSectionId)
        {
            BDSection section;
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDSection> sections = (from bdSections in context.BDSections
                                                     where bdSections.uuid == pSectionId
                                                     select bdSections);
                section = sections.AsQueryable().First<BDSection>();
            }
            return section;
        }
    }
}
