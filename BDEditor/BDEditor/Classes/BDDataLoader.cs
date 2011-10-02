using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.DataModel;
using System.IO;

namespace BDEditor.Classes
{
    public class BDDataLoader
    {
        char[] delimiters = new char[] { '\t' };

        private Entities dataContext;

        BDSection bdSection;
        BDCategory bdCategory;
        BDSubcategory bdSubCategory;
        BDDisease bdDisease;
        BDPresentation bdPresentation;

        string sectionData = null;
        string categoryData = null;
        string subCategoryData = null;
        string diseaseData = null;
        string presentationData = null;

        public void ImportData(Entities pDataContext, string pFilename)
        {
            dataContext = pDataContext;

            if (!File.Exists(pFilename))
            {
                Console.WriteLine("{0} does not exist.", pFilename);
                return;
            }
            using (StreamReader sr = File.OpenText(pFilename))
            {
                String input;
                
                while ((input = sr.ReadLine()) != null)
                {
                    ProcessInputLine(input);
                }
                Console.WriteLine ("The end of the stream has been reached.");
            }
        }

        private void ProcessInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            sectionData = elements[0];
            categoryData = elements[1];
            subCategoryData = elements[2];
            diseaseData = elements[3];
            presentationData = elements[4];

            if ( (sectionData != string.Empty) && ( (null == bdSection) || (bdSection.name != sectionData) ) )
            {
                bdSection = BDSection.CreateSection(dataContext);
                bdSection.name = sectionData;
                BDSection.SaveSection(dataContext, bdSection);

                bdCategory = null;
                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;
            }

            if ((categoryData != string.Empty) && ((null == bdCategory) || (bdCategory.name != categoryData)))
            {
                bdCategory = BDCategory.CreateCategory(dataContext);
                bdCategory.name = categoryData;
                bdCategory.sectionId = bdSection.uuid;
                BDCategory.SaveCategory(dataContext, bdCategory);

                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;
            }

            if ((subCategoryData != string.Empty) && ((null == bdSubCategory) || (bdSubCategory.name != subCategoryData)))
            {
                bdSubCategory = BDSubcategory.CreateSubcategory(dataContext);
                bdSubCategory.name = subCategoryData;
                bdSubCategory.categoryId = bdCategory.uuid;
                BDSubcategory.SaveSubcategory(dataContext, bdSubCategory);

                bdDisease = null;
                bdPresentation = null;
            }

            if ((diseaseData != string.Empty) && ((null == bdDisease) || (bdDisease.name != diseaseData)))
            {
                bdDisease = BDDisease.CreateDisease(dataContext);
                bdDisease.name = diseaseData;
                if (null != bdSubCategory)
                {
                    bdDisease.subcategoryId = bdSubCategory.uuid;
                }
                else
                {
                    bdDisease.categoryId = bdCategory.uuid;
                }
                BDDisease.SaveDisease(dataContext, bdDisease);

                bdPresentation = null;
            }

            if ((presentationData != string.Empty) && ((null == bdPresentation) || (bdPresentation.name != presentationData)))
            {
                bdPresentation = BDPresentation.CreatePresentation(dataContext);
                bdPresentation.name = presentationData;
                bdPresentation.diseaseId = bdDisease.uuid;
                BDPresentation.SavePresentation(dataContext, bdPresentation);
            }

        }
    }
}
