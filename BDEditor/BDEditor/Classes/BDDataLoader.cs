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
        string chapterUuidData = null;

        string sectionData = null;
        string sectionUuidData = null;

        string categoryData = null;
        string categoryUuidData = null;

        string subCategoryData = null;
        string subCategoryUuidData = null;

        string diseaseData = null;
        string diseaseUuidData = null;

        string presentationData = null;
        string presentationUuidData = null;

        short idxChapter = 0;
        short idxSection = 0;
        short idxCategory = 0;
        //short idxSubCategory = 0;
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
                int rowIdx = 0;
                const int titleRowIdx = 0;
                const int headerRowIdx = 1;

                while ((input = sr.ReadLine()) != null)
                {
                    if ( (rowIdx != titleRowIdx) && (rowIdx != headerRowIdx))
                        ProcessChapter2InputLine(input);
                    rowIdx++;
                }
                Console.WriteLine ("The end of the stream has been reached.");
            }
        }

        private void ProcessChapter2InputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            chapterData = elements[1];
            chapterUuidData = elements[0];

            sectionData = elements[2];
            sectionUuidData = elements[0];

            categoryData = elements[3];
            categoryUuidData = elements[0];

            subCategoryData = elements[4];
            subCategoryUuidData = elements[0];

            diseaseData = elements[5];
            diseaseUuidData = elements[0];

            presentationData = elements[6];
            presentationUuidData = elements[0];

            if( (chapterData != string.Empty) && ((null == bdChapter) || (bdChapter.name != chapterData)))
            {
                bdChapter = BDChapter.CreateChapter(dataContext, Guid.Parse(chapterUuidData));
                bdChapter.name = chapterData;
                bdChapter.displayOrder = idxChapter++;
                BDChapter.Save(dataContext,bdChapter);

                bdSection = null;
                bdCategory = null;
                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;

                BDObjectAssociation.CreateObjectAssociation(dataContext, bdChapter.uuid, BDChapter.KEY_NAME, BDSection.KEY_NAME);
            }

            if ( (sectionData != string.Empty) && ( (null == bdSection) || (bdSection.name != sectionData) ) )
            {
                bdSection = BDSection.CreateSection(dataContext, Guid.Parse(sectionUuidData));
                bdSection.name = sectionData;
                bdSection.parentId = bdChapter.uuid;
                bdSection.parentKeyName = BDChapter.KEY_NAME;
                bdSection.displayOrder = idxSection++;
                BDSection.Save(dataContext, bdSection);

                bdCategory = null;
                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;

                BDObjectAssociation.CreateObjectAssociation(dataContext, bdSection.uuid, BDSection.KEY_NAME, BDCategory.KEY_NAME);
            }

            if ((categoryData != string.Empty) && ((null == bdCategory) || (bdCategory.name != categoryData)))
            {
                bdCategory = BDCategory.CreateCategory(dataContext, Guid.Parse(categoryUuidData));
                bdCategory.name = categoryData;
                bdCategory.parentId = bdSection.uuid;
                bdCategory.parentKeyName = BDSection.KEY_NAME;
                bdCategory.displayOrder = idxCategory++;
                BDCategory.Save(dataContext, bdCategory);

                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;

                BDObjectAssociation.CreateObjectAssociation(dataContext, bdCategory.uuid, BDCategory.KEY_NAME, BDDisease.KEY_NAME);
            }

            if ((diseaseData != string.Empty) && ((null == bdDisease) || (bdDisease.name != diseaseData)))
            {
                bdDisease = BDDisease.CreateDisease(dataContext, Guid.Parse(diseaseUuidData));
                bdDisease.name = diseaseData;
                bdDisease.displayOrder = idxDisease++;
                if (null != bdSubCategory)
                {
                    bdDisease.parentId = bdSubCategory.uuid;
                    bdDisease.parentKeyName = BDSubcategory.KEY_NAME;
                }
                else
                {
                    bdDisease.parentId = bdCategory.uuid;
                    bdDisease.parentKeyName = BDCategory.KEY_NAME;
                }
                BDDisease.Save(dataContext, bdDisease);

                BDObjectAssociation.CreateObjectAssociation(dataContext, bdDisease.uuid, BDDisease.KEY_NAME, BDPresentation.KEY_NAME);

                bdPresentation = null;
            }

            if ((presentationData != string.Empty) && ((null == bdPresentation) || (bdPresentation.name != presentationData)))
            {
                bdPresentation = BDPresentation.CreatePresentation(dataContext, bdDisease.uuid, Guid.Parse(presentationUuidData));
                bdPresentation.parentKeyName = BDDisease.KEY_NAME;
                bdPresentation.name = presentationData;
                bdPresentation.displayOrder = idxPresentation++;
                BDPresentation.Save(dataContext, bdPresentation);

                BDObjectAssociation.CreateObjectAssociation(dataContext, bdPresentation.uuid, BDPresentation.KEY_NAME, BDPathogenGroup.KEY_NAME);
            }
        }
    }
}
