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
        public enum baseDataLayoutType
        {
            chapter2a,
            chapter2b,
            chapter2c
        }

        char[] delimiters = new char[] { '\t' };

        private Entities dataContext;

        BDChapter bdChapter;
        BDSection bdSection;
        BDCategory bdCategory;
        BDSubcategory bdSubCategory;
        BDDisease bdDisease;
        BDPresentation bdPresentation;

        string uuidData = null;
        string chapterData = null;
        string sectionData = null;
        string categoryData = null;
        string subCategoryData = null;
        string diseaseData = null;
        string presentationData = null;

        short idxChapter = 0;
        short idxSection = 0;
        short idxCategory = 0;
        //short idxSubCategory = 0;
        short idxDisease = 0;
        short idxPresentation = 0;

        public void ImportData(Entities pDataContext, string pFilename, baseDataLayoutType pLayoutType)
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
                        switch (pLayoutType)
                        {
                            case baseDataLayoutType.chapter2a:
                                ProcessChapter2aInputLine(input);
                                break;
                        }
                        
                    rowIdx++;
                }
                Console.WriteLine ("The end of the stream has been reached.");
            }
        }

        private void ProcessChapter2aInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data
            uuidData = elements[0];

            chapterData = elements[1];
            sectionData = elements[2];
            categoryData = elements[3];
            subCategoryData = elements[4];
            diseaseData = elements[5];
            presentationData = elements[6];

            if( (chapterData != string.Empty) && ((null == bdChapter) || (bdChapter.name != chapterData)))
            {
                bdChapter = BDChapter.CreateChapter(dataContext, Guid.Parse(uuidData));
                bdChapter.name = chapterData;
                bdChapter.displayOrder = idxChapter++;
                BDChapter.Save(dataContext,bdChapter);

                bdSection = null;
                bdCategory = null;
                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;

                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, bdChapter.Uuid, BDChapter.KEY_NAME);
                meta.layoutVariant = 1;
                BDMetadata.Save(dataContext, meta);
            }

            if ( (sectionData != string.Empty) && ( (null == bdSection) || (bdSection.name != sectionData) ) )
            {
                bdSection = BDSection.CreateSection(dataContext, Guid.Parse(uuidData));
                bdSection.name = sectionData;
                bdSection.parentId = bdChapter.uuid;
                bdSection.parentKeyName = BDChapter.KEY_NAME;
                bdSection.displayOrder = idxSection++;
                BDSection.Save(dataContext, bdSection);

                bdCategory = null;
                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;

                BDNodeAssociation.CreateNodeAssociation(dataContext, bdChapter.uuid, BDChapter.KEY_NAME, BDSection.KEY_NAME);

                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, bdSection.Uuid, BDSection.KEY_NAME);
                meta.layoutVariant = 1;
                BDMetadata.Save(dataContext, meta);
            }

            if ((categoryData != string.Empty) && ((null == bdCategory) || (bdCategory.name != categoryData)))
            {
                bdCategory = BDCategory.CreateCategory(dataContext, Guid.Parse(uuidData));
                bdCategory.name = categoryData;
                bdCategory.parentId = bdSection.uuid;
                bdCategory.parentKeyName = BDSection.KEY_NAME;
                bdCategory.displayOrder = idxCategory++;
                BDCategory.Save(dataContext, bdCategory);

                bdSubCategory = null;
                bdDisease = null;
                bdPresentation = null;

                BDNodeAssociation.CreateNodeAssociation(dataContext, bdSection.uuid, BDSection.KEY_NAME, BDCategory.KEY_NAME);

                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, bdCategory.Uuid, BDCategory.KEY_NAME);
                meta.layoutVariant = 1;
                BDMetadata.Save(dataContext, meta);
            }

            if ((diseaseData != string.Empty) && ((null == bdDisease) || (bdDisease.name != diseaseData)))
            {
                bdDisease = BDDisease.CreateDisease(dataContext, Guid.Parse(uuidData));
                bdDisease.name = diseaseData;
                bdDisease.displayOrder = idxDisease++;
                if (null != bdSubCategory)
                {
                    BDNodeAssociation.CreateNodeAssociation(dataContext, bdSubCategory.uuid, BDSubcategory.KEY_NAME, BDDisease.KEY_NAME);
                    bdDisease.parentId = bdSubCategory.uuid;
                    bdDisease.parentKeyName = BDSubcategory.KEY_NAME;
                }
                else
                {
                    BDNodeAssociation.CreateNodeAssociation(dataContext, bdCategory.uuid, BDCategory.KEY_NAME, BDDisease.KEY_NAME);
                    bdDisease.parentId = bdCategory.uuid;
                    bdDisease.parentKeyName = BDCategory.KEY_NAME;
                }
                BDDisease.Save(dataContext, bdDisease);
      
                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, bdDisease.Uuid, BDDisease.KEY_NAME);
                meta.layoutVariant = 1;
                BDMetadata.Save(dataContext, meta);
                bdPresentation = null;
            }

            if ((presentationData != string.Empty) && ((null == bdPresentation) || (bdPresentation.name != presentationData)))
            {
                bdPresentation = BDPresentation.CreatePresentation(dataContext, bdDisease.uuid, Guid.Parse(uuidData));
                bdPresentation.parentKeyName = BDDisease.KEY_NAME;
                bdPresentation.name = presentationData;
                bdPresentation.displayOrder = idxPresentation++;
                BDPresentation.Save(dataContext, bdPresentation);

                BDNodeAssociation.CreateNodeAssociation(dataContext, bdDisease.uuid, BDDisease.KEY_NAME, BDPresentation.KEY_NAME);

                BDNodeAssociation.CreateNodeAssociation(dataContext, bdPresentation.uuid, BDPresentation.KEY_NAME, BDPathogenGroup.KEY_NAME);

                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, bdPresentation.Uuid, BDPresentation.KEY_NAME);
                meta.layoutVariant = 1;
                BDMetadata.Save(dataContext, meta);
            }
        }
    }
}
