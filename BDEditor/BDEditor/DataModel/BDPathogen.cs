using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDPathogen
    /// </summary>
    public partial class BDPathogen
    {
        public static BDPathogen CreatePathogen(Entities pContext)
        {
                BDPathogen pathogen = CreateBDPathogen(Guid.NewGuid(), false);
                pathogen.createdBy = Guid.Empty;
                pathogen.createdDate = DateTime.Now;
                pathogen.schemaVersion = 0;

                pContext.AddObject("BDPathogens", pathogen);
                return pathogen;
        }

        /// <summary>
        /// Extend Save method that sets modified date
        /// </summary>
        /// <param name="pPathogen"></param>
        public static void SavePathogen(Entities pContext, BDPathogen pPathogen)
        {
                pPathogen.modifiedBy = Guid.Empty;
                pPathogen.modifiedDate = DateTime.Now;

                pContext.SaveChanges();
        }

        /// <summary>
        /// Get all pathogens in the model with the specified presentation ID
        /// </summary>
        /// <param name="pPresentationId"></param>
        /// <returns>List of Pathogens</returns>
        public static List<BDPathogen> GetPathogensForPresentationGroup(Entities pContext, Guid pPathogenGroupId)
        {
            List<BDPathogen> pathogenList = new List<BDPathogen>();
                IQueryable<BDPathogen> pathogens = (from bdPathogens in pContext.BDPathogens
                                                    where bdPathogens.pathogenGroupId == pPathogenGroupId
                                                    select bdPathogens);
                foreach (BDPathogen pathogen in pathogens)
                {
                    pathogenList.Add(pathogen);
                }
            return pathogenList;
        }

        public static BDPathogen GetPathogenWithId(Entities pContext, Guid pPathogenId)
        {
            BDPathogen pathogen;
                IQueryable<BDPathogen> pathogens = (from bdPathogens in pContext.BDPathogens
                                                    where bdPathogens.uuid == pPathogenId
                                                    select bdPathogens);
                pathogen = pathogens.AsQueryable().First<BDPathogen>();
            return pathogen;
        }
    }
}
