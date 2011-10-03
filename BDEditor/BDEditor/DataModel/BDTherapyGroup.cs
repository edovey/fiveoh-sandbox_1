using System;
using System.Collections.Generic;
using System.Data;
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

        public enum TherapyGroupJoinType
        {
            None = 0,
            AndWithNext = 1,
            OrWithNext = 2
        }

        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDTherapyGroup CreateTherapyGroup(Entities pContext)
        {
                BDTherapyGroup therapyGroup = CreateBDTherapyGroup(Guid.NewGuid(), false);
                therapyGroup.createdBy = Guid.Empty;
                therapyGroup.createdDate = DateTime.Now;
                therapyGroup.schemaVersion = 0;
                therapyGroup.therapyGroupJoinType = (int)TherapyGroupJoinType.None;

                pContext.AddObject("BDTherapyGroups", therapyGroup);

                return therapyGroup;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pTherapyGroup"></param>
        public static void SaveTherapyGroup(Entities pContext, BDTherapyGroup pTherapyGroup)
        {
            if (pTherapyGroup.EntityState != EntityState.Unchanged)
            {
                pTherapyGroup.modifiedBy = Guid.Empty;
                pTherapyGroup.modifiedDate = DateTime.Now;

                pContext.SaveChanges();
            }
        }


        /// <summary>
        /// Gets all Therapy Groups in the model with the specified Pathogen ID
        /// </summary>
        /// <param name="pPathogenId"></param>
        /// <returns>List of BDTherapyGroups</returns>
        public static List<BDTherapyGroup> getTherapyGroupsForPathogenGroupId(Entities pContext, Guid pPathogenGroupId)
        {
            List<BDTherapyGroup> therapyGroupList = new List<BDTherapyGroup>();
                IQueryable<BDTherapyGroup> therapyGroups = (from bdTherapyGroups in pContext.BDTherapyGroups
                                                            where bdTherapyGroups.pathogenGroupId == pPathogenGroupId
                                                            select bdTherapyGroups);
                foreach (BDTherapyGroup therapyGroup in therapyGroups)
                {
                    therapyGroupList.Add(therapyGroup);
                }
            return therapyGroupList;
        }
    }
}
