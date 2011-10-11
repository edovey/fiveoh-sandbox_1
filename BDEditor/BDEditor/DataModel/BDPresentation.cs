using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDPresentation
    /// </summary>
    public partial class BDPresentation
    {
        public const string ENTITYNAME_FRIENDLY = @"Presentation";
        public const string PROPERTYNAME_OVERVIEW = @"Overview";

        /// <summary>
        /// Extended Create method that sets creation date and schema version.
        /// </summary>
        /// <returns></returns>
        public static BDPresentation CreatePresentation(Entities pContext)
        {
            BDPresentation presentation = CreateBDPresentation(Guid.NewGuid(), false);
            presentation.createdBy = Guid.Empty;
            presentation.createdDate = DateTime.Now;
            presentation.schemaVersion = 0;

            pContext.AddObject("BDPresentations", presentation);

            BDPathogenGroup pathogenGroup = BDPathogenGroup.CreatePathogenGroup(pContext);
            pathogenGroup.presentationId = presentation.uuid;
            BDPathogenGroup.SavePathogenGroup(pContext, pathogenGroup);

            return presentation;
        }

        /// <summary>
        /// Extended Save method that sets modifiedDate.
        /// </summary>
        /// <param name="pPresentation"></param>
        public static void SavePresentation(Entities pContext, BDPresentation pPresentation)
        {
            if (pPresentation.EntityState != EntityState.Unchanged)
            {
                pPresentation.modifiedBy = Guid.Empty;
                pPresentation.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"Presentation Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all Presentations in the model with the specified disease ID
        /// </summary>
        /// <param name="pDiseaseId"></param>
        /// <returns>List of Presentations</returns>
        public static List<BDPresentation> GetPresentationsForDiseaseId(Entities pContext, Guid pDiseaseId)
        {
            List<BDPresentation> presentationList = new List<BDPresentation>();

                IQueryable<BDPresentation> presentations = (from bdPresentations in pContext.BDPresentations
                                                            where bdPresentations.diseaseId == pDiseaseId
                                                            select bdPresentations);
                foreach (BDPresentation presentation in presentations)
                {
                    presentationList.Add(presentation);
                }
            return presentationList;
        }

        /// <summary>
        /// Get Presentation with the specified ID
        /// </summary>
        /// <param name="pPresentationId"></param>
        /// <returns>BDPresentation object</returns>
        public static BDPresentation GetPresentationWithId(Entities pContext, Guid pPresentationId)
        {
            BDPresentation presentation;
                IQueryable<BDPresentation> presentations = (from bdPresentations in pContext.BDPresentations
                                                            where bdPresentations.uuid == pPresentationId
                                                            select bdPresentations);
                presentation = presentations.AsQueryable().First<BDPresentation>();
            return presentation;
        }

    }
}
