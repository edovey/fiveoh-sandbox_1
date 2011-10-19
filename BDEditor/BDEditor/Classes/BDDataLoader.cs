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

        BDChapter bdChapter;
        BDSection bdSection;
        BDCategory bdCategory;
        BDSubcategory bdSubCategory;
        BDDisease bdDisease;
        BDPresentation bdPresentation;

        string chapterData = null;
        string sectionData = null;
        string categoryData = null;
        string subCategoryData = null;
        string diseaseData = null;
        string presentationData = null;

        short idxChapter = 0;
        short idxSection = 0;
        short idxCategory = 0;
        short idxSubCategory = 0;
        short idxDisease = 0;
        short idxPresentation = 0;

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
                bool isFirstRow = true;
                while ((input = sr.ReadLine()) != null)
                {
                    if(!isFirstRow)
                        ProcessInputLine(input);
                    isFirstRow = false;
                }
                Console.WriteLine ("The end of the stream has been reached.");
            }
        }

        private void ProcessInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            chapterData = elements[0];
            sectionData = elements[1];
            categoryData = elements[2];
            subCategoryData = elements[3];
            diseaseData = elements[4];
            // diseaseOverviewFlag = elements[4];
            presentationData = elements[6];

            if( (chapterData != string.Empty) && ((null == bdChapter) || (bdChapter.name != chapterData)))
            {
                bdChapter = BDChapter.CreateChapter(dataContext);
                bdChapter.name = chapterData;
                bdChapter.displayOrder = idxChapter++;
                BDChapter.SaveChapter(dataContext,bdChapter);

                bdSection = null;
                bdCategory = null;
                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;
            }

            if ( (sectionData != string.Empty) && ( (null == bdSection) || (bdSection.name != sectionData) ) )
            {
                bdSection = BDSection.CreateSection(dataContext);
                bdSection.name = sectionData;
                bdSection.chapterId = bdChapter.uuid;
                bdSection.displayOrder = idxSection++;
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
                bdCategory.displayOrder = idxCategory++;
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
                bdSubCategory.displayOrder = idxSubCategory++;
                BDSubcategory.SaveSubcategory(dataContext, bdSubCategory);

                bdDisease = null;
                bdPresentation = null;
            }

            if ((diseaseData != string.Empty) && ((null == bdDisease) || (bdDisease.name != diseaseData)))
            {
                bdDisease = BDDisease.CreateDisease(dataContext);
                bdDisease.name = diseaseData;
                bdDisease.displayOrder = idxDisease++;
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
                bdPresentation.displayOrder = idxPresentation++;
                BDPresentation.SavePresentation(dataContext, bdPresentation);
            }

        }
    }
}
