using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDTherapyGroup
    /// </summary>
    public partial class BDTherapyGroup
    {
        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDTherapyGroup CreateTherapyGroup()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDTherapyGroup therapyGroup = CreateBDTherapyGroup(Guid.NewGuid(), false);
                therapyGroup.createdBy = Guid.Empty;
                therapyGroup.createdDate = DateTime.Now;
                therapyGroup.schemaVersion = 0;

                context.AddObject("BDTherapyGroups", therapyGroup);

                return therapyGroup;
            }
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pTherapyGroup"></param>
        public static void SaveTherapyGroup(BDTherapyGroup pTherapyGroup)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                pTherapyGroup.modifiedBy = Guid.Empty;
                pTherapyGroup.modifiedDate = DateTime.Now;

                context.SaveChanges();
            }

        }


        /// <summary>
        /// Gets all Therapy Groups in the model with the specified Pathogen ID
        /// </summary>
        /// <param name="pPathogenId"></param>
        /// <returns>List of BDTherapyGroups</returns>
        public static List<BDTherapyGroup> getTherapyGroupsForPathogenId(Guid pPathogenId)
        {
            List<BDTherapyGroup> therapyGroupList = new List<BDTherapyGroup>();
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDTherapyGroup> therapyGroups = (from bdTherapyGroups in context.BDTherapyGroups
                                                            where bdTherapyGroups.pathogenId == pPathogenId
                                                            select bdTherapyGroups);
                foreach (BDTherapyGroup therapyGroup in therapyGroups)
                {
                    therapyGroupList.Add(therapyGroup);
                }
            }
            return therapyGroupList;
        }
    }
}
