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
        public static BDSubcategory CreateSubcategory()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDSubcategory subcategory = CreateBDSubcategory(Guid.NewGuid(), false);
                subcategory.createdBy = Guid.Empty;
                subcategory.createdDate = DateTime.Now;
                subcategory.schemaVersion = 0;

                context.AddObject("BDSubcategories", subcategory);

                return subcategory;
            }
        }

        /// <summary>
        /// Extended Save method that sets the modified date.
        /// </summary>
        /// <param name="pSubcategory"></param>
        public static void SaveSubcategory(BDSubcategory pSubcategory)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                pSubcategory.modifiedBy = Guid.Empty;
                pSubcategory.modifiedDate = DateTime.Now;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all subcategories in the model with the specified category ID
        /// </summary>
        /// <param name="pCategoryId"></param>
        /// <returns>List of Subcategories</returns>
        public static List<BDSubcategory> GetSubcategoriesForCategoryId(Guid pCategoryId)
        {
            List<BDSubcategory> subcategoryList = new List<BDSubcategory>();

            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDSubcategory> subcategories = (from bdSubcategories in context.BDSubcategories
                                                           where bdSubcategories.categoryId == pCategoryId
                                                           select bdSubcategories);
                foreach (BDSubcategory subcat in subcategories)
                {
                    subcategoryList.Add(subcat);
                }
            }
            return subcategoryList;
        }


        /// <summary>
        /// Get Subcategory with the specified ID
        /// </summary>
        /// <param name="pSubcategoryId"></param>
        /// <returns>Subcategory object.</returns>
        public static BDSubcategory GetSubcategoryWithId(Guid pSubcategoryId)
        {
            BDSubcategory subcategory;
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDSubcategory> subcategories = (from bdSubcategories in context.BDSubcategories
                                                           where bdSubcategories.uuid == pSubcategoryId
                                                           select bdSubcategories);
                subcategory = subcategories.AsQueryable().First<BDSubcategory>();
            }
            return subcategory;
        }
    }
}
