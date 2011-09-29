using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDPresentation
    /// </summary>
    public partial class BDPresentation
    {
        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDPresentation CreatePresentation()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDPresentation presentation = CreateBDPresentation(Guid.NewGuid(), false);
                presentation.createdBy = Guid.Empty;
                presentation.createdDate = DateTime.Now;
                presentation.schemaVersion = 0;

                context.AddObject("BDPresentations", presentation);

                return presentation;
            }
        }

        /// <summary>
        /// Extended Save method that sets modifiedDate.
        /// </summary>
        /// <param name="pPresentation"></param>
        public static void SavePresentation(BDPresentation pPresentation)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                pPresentation.modifiedBy = Guid.Empty;
                pPresentation.modifiedDate = DateTime.Now;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all Presentations in the model with the specified disease ID
        /// </summary>
        /// <param name="pDiseaseId"></param>
        /// <returns>List of Presentations</returns>
        public static List<BDPresentation> GetPresentationsForDiseaseId(Guid pDiseaseId)
        {
            List<BDPresentation> presentationList = new List<BDPresentation>();

            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDPresentation> presentations = (from bdPresentations in context.BDPresentations
                                                            where bdPresentations.diseaseId == pDiseaseId
                                                            select bdPresentations);
                foreach (BDPresentation presentation in presentations)
                {
                    presentationList.Add(presentation);
                }
            }
            return presentationList;
        }

        /// <summary>
        /// Get Presentation with the specified ID
        /// </summary>
        /// <param name="pPresentationId"></param>
        /// <returns>BDPresentation object</returns>
        public static BDPresentation GetPresentationWithId(Guid pPresentationId)
        {
            BDPresentation presentation;
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDPresentation> presentations = (from bdPresentations in context.BDPresentations
                                                            where bdPresentations.uuid == pPresentationId
                                                            select bdPresentations);
                presentation = presentations.AsQueryable().First<BDPresentation>();
            }
            return presentation;
        }
    }
}
