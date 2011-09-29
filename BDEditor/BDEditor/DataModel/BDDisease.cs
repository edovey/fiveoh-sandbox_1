using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Extended Create method that sets the create date and the schema version
        /// </summary>
        /// <returns>BDDisease object</returns>
        public static BDDisease CreateDisease()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDDisease disease = CreateBDDisease(Guid.NewGuid(), false);
                disease.createdBy = Guid.Empty;
                disease.createdDate = DateTime.Now;
                disease.schemaVersion = 0;

                context.AddObject("BDDiseases", disease);

                return disease;
            }
        }

        /// <summary>
        /// Extended Save method that sets the modification date
        /// </summary>
        /// <param name="pDisease"></param>
        public static void SaveDisease(BDDisease pDisease)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                pDisease.modifiedBy = Guid.Empty;
                pDisease.modifiedDate = DateTime.Now;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all diseases in the model with the specified category ID
        /// </summary>
        /// <param name="pCategoryId"></param>
        /// <returns>List of Diseases</returns>
        public static List<BDDisease> GetDiseasesForCategoryId(Guid pCategoryId)
        {
            List<BDDisease> diseaseList = new List<BDDisease>();

            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {   
                IQueryable<BDDisease> diseases = (from bdDiseases in context.BDDiseases
                                          where bdDiseases.categoryId == pCategoryId
                                          select bdDiseases);

                foreach (BDDisease disease in diseases)
                {
                    diseaseList.Add(disease);
                }
            }
            return diseaseList;
        }
     
        /// <summary>
        /// Gets all diseases in the model with the specified subcategory ID
        /// </summary>
        /// <param name="pSubcategoryId"></param>
        /// <returns>List of Diseases</returns>
        public static List<BDDisease> GetDiseasesForSubcategory(Guid pSubcategoryId)
        {
            List<BDDisease> diseaseList = new List<BDDisease>();

            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDDisease> diseases = (from bdDiseases in context.BDDiseases
                                                  where bdDiseases.subcategoryId == pSubcategoryId
                                                  select bdDiseases);

                foreach (BDDisease disease in diseases)
                {
                    diseaseList.Add(disease);
                }
            }
            return diseaseList;
        }

        /// <summary>
        /// Get Disease with the specified ID
        /// </summary>
        /// <param name="pDiseaseId"></param>
        /// <returns>Disease object</returns>
        public static BDDisease GetDiseaseWithId(Guid pDiseaseId)
        {
            BDDisease disease;
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDDisease> diseases = (from bdDiseases in context.BDDiseases
                                                  where bdDiseases.uuid == pDiseaseId
                                                  select bdDiseases);
                disease = diseases.AsQueryable().First<BDDisease>();
            }
            return disease;
        }
    }
}
