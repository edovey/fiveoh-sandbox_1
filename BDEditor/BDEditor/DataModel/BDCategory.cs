using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;
using BDEditor.DataModel;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDCategory
    /// </summary>
    public partial class BDCategory
    {
        /// <summary>
        /// Extended Create Method that includes setting creation date and schema version.
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static BDCategory CreateCategory(Entities pContext)
        {
            BDCategory category = CreateBDCategory(Guid.NewGuid(), false);
            category.createdBy = Guid.Empty;
            category.createdDate = DateTime.Now;
            category.schemaVersion = 0;

            pContext.AddObject("BDCategories", category);

            return category;
        }

        /// <summary>
        /// Extended Save method that includes setting the modification date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pCategory"></param>
        public static void SaveCategory(Entities pContext, BDCategory pCategory)
        {
            pCategory.modifiedBy = Guid.Empty;
            pCategory.modifiedDate = DateTime.Now;

            pContext.SaveChanges();
        }

        /// <summary>
        /// Gets all categories in the model with the indicated section ID
        /// </summary>
        /// <param name="pSectionId"></param>
        /// <returns>List of Categories</returns>
        public static List<BDCategory> GetCategoriesForSectionId(Guid pSectionId)
        {
            List<BDCategory> catList = new List<BDCategory>();
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDCategory> categories = (from bdCategories in context.BDCategories
                                  where bdCategories.sectionId == pSectionId
                                  select bdCategories);
                foreach (BDCategory cat in categories)
                {
                    catList.Add(cat);
                }
            }
            return catList;
        }
    }
}
