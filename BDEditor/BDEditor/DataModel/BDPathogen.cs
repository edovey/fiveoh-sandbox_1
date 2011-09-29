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
        public static BDPathogen CreatePathogen()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDPathogen pathogen = CreateBDPathogen(Guid.NewGuid(), false);
                pathogen.createdBy = Guid.Empty;
                pathogen.createdDate = DateTime.Now;
                pathogen.schemaVersion = 0;

                context.AddObject("BDPathogens", pathogen);
                return pathogen;
            }
        }

        /// <summary>
        /// Extend Save method that sets modified date
        /// </summary>
        /// <param name="pPathogen"></param>
        public static void SavePathogen(BDPathogen pPathogen)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                pPathogen.modifiedBy = Guid.Empty;
                pPathogen.modifiedDate = DateTime.Now;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get all pathogens in the model with the specified presentation ID
        /// </summary>
        /// <param name="pPresentationId"></param>
        /// <returns>List of Pathogens</returns>
        public static List<BDPathogen> GetPathogensForPresentationId(Guid pPresentationId)
        {
            List<BDPathogen> pathogenList = new List<BDPathogen>();
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDPathogen> pathogens = (from bdPathogens in context.BDPathogens
                                                    where bdPathogens.presentationId == pPresentationId
                                                    select bdPathogens);
                foreach (BDPathogen pathogen in pathogens)
                {
                    pathogenList.Add(pathogen);
                }
            }
            return pathogenList;
        }

        public static BDPathogen GetPathogenWithId(Guid pPathogenId)
        {
            BDPathogen pathogen;
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDPathogen> pathogens = (from bdPathogens in context.BDPathogens
                                                    where bdPathogens.uuid == pPathogenId
                                                    select bdPathogens);
                pathogen = pathogens.AsQueryable().First<BDPathogen>();
            }
            return pathogen;
        }
    }
}
