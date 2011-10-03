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
    /// Extension of generated BDPathogenGroup
    /// </summary>
    public partial class BDPathogenGroup
    {
        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static BDPathogenGroup CreatePathogenGroup(Entities pContext)
        {
            BDPathogenGroup pathogenGroup = CreateBDPathogenGroup(Guid.NewGuid(), false);
            pathogenGroup.createdBy = Guid.Empty;
            pathogenGroup.createdDate = DateTime.Now;
            pathogenGroup.schemaVersion = 0;

            pContext.AddObject("BDPathogenGroups", pathogenGroup);

            return pathogenGroup;
        }


        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPathogenGroup"></param>
        public static void SavePathogenGroup(Entities pContext, BDPathogenGroup pPathogenGroup)
        {
            if (pPathogenGroup.EntityState != EntityState.Unchanged)
            {
                pPathogenGroup.modifiedBy = Guid.Empty;
                pPathogenGroup.modifiedDate = DateTime.Now;

                System.Diagnostics.Debug.WriteLine(@"PathogenGroup Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all pathogen groups in the model with the specified presentation ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPresentationId"></param>
        /// <returns></returns>
        public static List<BDPathogenGroup> GetPathogenGroupsForPresentationId(Entities pContext, Guid pPresentationId)
        {
            List<BDPathogenGroup> pathogenGroupList = new List<BDPathogenGroup>();

            IQueryable<BDPathogenGroup> pathogenGroups = (from bdPathogenGroups in pContext.BDPathogenGroups
                                                          where bdPathogenGroups.presentationId == pPresentationId
                                                          select bdPathogenGroups);
            foreach (BDPathogenGroup pathogenGroup in pathogenGroups)
            {
                pathogenGroupList.Add(pathogenGroup);
            }
            return pathogenGroupList;
        }

        public static BDPathogenGroup GetPathogenGroupWithId(Entities pContext, Guid pPathogenGroupId)
        {
            BDPathogenGroup pathogenGroup;

            IQueryable<BDPathogenGroup> pathogenGroups = (from bdPathogenGroups in pContext.BDPathogenGroups
                                                          where bdPathogenGroups.uuid == pPathogenGroupId
                                                          select bdPathogenGroups);
            pathogenGroup = pathogenGroups.AsQueryable().First<BDPathogenGroup>();
            return pathogenGroup;
        }
    }
}
