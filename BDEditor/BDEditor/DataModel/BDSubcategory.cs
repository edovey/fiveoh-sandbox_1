using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDSubcategory
    /// </summary>
    public partial class BDSubcategory
    {
        /// <summary>
        /// Extended Create method that sets created date and schema version.
        /// </summary>
        /// <returns>New BDSubcategory object</returns>
        public static BDSubcategory CreateSubcategory(Entities pContext)
        {
                BDSubcategory subcategory = CreateBDSubcategory(Guid.NewGuid(), false);
                subcategory.createdBy = Guid.Empty;
                subcategory.createdDate = DateTime.Now;
                subcategory.schemaVersion = 0;

                pContext.AddObject("BDSubcategories", subcategory);

                return subcategory;
        }

        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pSubcategory"></param>
        public static void SaveSubcategory(Entities pContext, BDSubcategory pSubcategory)
        {
                pSubcategory.modifiedBy = Guid.Empty;
                pSubcategory.modifiedDate = DateTime.Now;

                pContext.SaveChanges();
        }

        /// <summary>
        /// Gets all subcategories in the model with the specified category ID
        /// </summary>
        /// <param name="pCategoryId"></param>
        /// <returns>List of Subcategories</returns>
        public static List<BDSubcategory> GetSubcategoriesForCategoryId(Entities pContext, Guid pCategoryId)
        {
            List<BDSubcategory> subcategoryList = new List<BDSubcategory>();

                IQueryable<BDSubcategory> subcategories = (from bdSubcategories in pContext.BDSubcategories
                                                           where bdSubcategories.categoryId == pCategoryId
                                                           select bdSubcategories);
                foreach (BDSubcategory subcat in subcategories)
                {
                    subcategoryList.Add(subcat);
                }
            return subcategoryList;
        }


        /// <summary>
        /// Get Subcategory with the specified ID
        /// </summary>
        /// <param name="pSubcategoryId"></param>
        /// <returns>Subcategory object.</returns>
        public static BDSubcategory GetSubcategoryWithId(Entities pContext, Guid pSubcategoryId)
        {
            BDSubcategory subcategory;
            IQueryable<BDSubcategory> subcategories = (from bdSubcategories in pContext.BDSubcategories
                                                        where bdSubcategories.uuid == pSubcategoryId
                                                        select bdSubcategories);
            subcategory = subcategories.AsQueryable().First<BDSubcategory>();
            return subcategory;
        }
    }
}
