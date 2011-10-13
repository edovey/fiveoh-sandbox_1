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
    /// Extension of generated BDDisease
    /// </summary>
    public partial class BDDisease
    {
        public const string ENTITYNAME_FRIENDLY = @"Disease";
        public const string PROPERTYNAME_OVERVIEW = @"Overview";

        /// <summary>
        /// Extended Create method that sets the create date and the schema version
        /// </summary>
        /// <returns>BDDisease object</returns>
        public static BDDisease CreateDisease(Entities pContext)
        {
            BDDisease disease = CreateBDDisease(Guid.NewGuid(), false);
            disease.createdBy = Guid.Empty;
            disease.createdDate = DateTime.Now;
            disease.schemaVersion = 0;

            pContext.AddObject("BDDiseases", disease);

            return disease;
        }

        /// <summary>
        /// Extended Save method that sets the modification date
        /// </summary>
        /// <param name="pDisease"></param>
        public static void SaveDisease(Entities pContext, BDDisease pDisease)
        {
            if (pDisease.EntityState != EntityState.Unchanged)
            {
                pDisease.modifiedBy = Guid.Empty;
                pDisease.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"Disease Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all diseases in the model with the specified category ID
        /// </summary>
        /// <param name="pCategoryId"></param>
        /// <returns>List of Diseases</returns>
        public static List<BDDisease> GetDiseasesForCategoryId(Entities pContext, Guid pCategoryId)
        {
            List<BDDisease> diseaseList = new List<BDDisease>();
  
            IQueryable<BDDisease> diseases = (from bdDiseases in pContext.BDDiseases
                                        where bdDiseases.categoryId == pCategoryId
                                        select bdDiseases);

            foreach (BDDisease disease in diseases)
            {
                diseaseList.Add(disease);
            }
            return diseaseList;
        }
     
        /// <summary>
        /// Gets all diseases in the model with the specified subcategory ID
        /// </summary>
        /// <param name="pSubcategoryId"></param>
        /// <returns>List of Diseases</returns>
        public static List<BDDisease> GetDiseasesForSubcategory(Entities pContext, Guid pSubcategoryId)
        {
            List<BDDisease> diseaseList = new List<BDDisease>();

        IQueryable<BDDisease> diseases = (from bdDiseases in pContext.BDDiseases
                                                where bdDiseases.subcategoryId == pSubcategoryId
                                                select bdDiseases);

            foreach (BDDisease disease in diseases)
            {
                diseaseList.Add(disease);
            }

            return diseaseList;
        }

        /// <summary>
        /// Get Disease with the specified ID
        /// </summary>
        /// <param name="pDiseaseId"></param>
        /// <returns>Disease object</returns>
        public static BDDisease GetDiseaseWithId(Entities pContext, Guid pDiseaseId)
        {
            BDDisease disease;

            IQueryable<BDDisease> diseases = (from bdDiseases in pContext.BDDiseases
                                                where bdDiseases.uuid == pDiseaseId
                                                select bdDiseases);
            disease = diseases.AsQueryable().First<BDDisease>();

            return disease;
        }
    }
}
