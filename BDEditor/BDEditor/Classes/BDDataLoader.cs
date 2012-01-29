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
        public enum baseDataDefinitionType
        {
            chapter2a,
            chapter2b,
            chapter2c
        }

        char[] delimiters = new char[] { '\t' };

        private Entities dataContext;

        BDNode chapter;
        BDNode section;
        BDNode category;
        BDNode disease;
        BDNode presentation;

        string uuidData = null;
        string chapterData = null;
        string sectionData = null;
        string categoryData = null;
        string diseaseData = null;
        string presentationData = null;

        short idxChapter = 0;
        short idxSection = 0;
        short idxCategory = 0;
        short idxDisease = 0;
        short idxPresentation = 0;

        public void ImportData(Entities pDataContext, string pFilename, baseDataDefinitionType pDefinitionType)
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
                        switch (pDefinitionType)
                        {
                            case baseDataDefinitionType.chapter2a:
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
            //subCategoryData = elements[4];
            diseaseData = elements[5];
            presentationData = elements[6];

            if( (chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                chapter = BDNode.CreateNode(dataContext, Constants.BDObjectType.BDChapter,Guid.Parse(uuidData));
                chapter.name = chapterData;
                chapter.displayOrder = idxChapter++;

                chapter.SetParent(null);

                BDNode.Save(dataContext, chapter);

                section = null;
                category = null;
                disease = null;
                presentation = null;

                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, BDMetadata.LayoutVariantType.TreatmentRecommendation00, chapter);
                BDMetadata.Save(dataContext, meta);
            }

            if ( (sectionData != string.Empty) && ( (null == section) || (section.name != sectionData) ) )
            {
                section = BDNode.CreateNode(dataContext, Constants.BDObjectType.BDSection, Guid.Parse(uuidData));
                section.name = sectionData;
                section.SetParent(chapter);
                section.displayOrder = idxSection++;
                BDNode.Save(dataContext, section);

                category = null;
                disease = null;
                presentation = null;

                BDNodeAssociation.CreateNodeAssociation(dataContext, chapter, Constants.BDObjectType.BDSection);

                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, BDMetadata.LayoutVariantType.TreatmentRecommendation01, section);
                BDMetadata.Save(dataContext, meta);
            }

            if ((categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                category = BDNode.CreateNode(dataContext, Constants.BDObjectType.BDCategory, Guid.Parse(uuidData));
                category.name = categoryData;
                category.SetParent(section);
                category.displayOrder = idxCategory++;
                BDNode.Save(dataContext, category);

                disease = null;
                presentation = null;

                BDNodeAssociation.CreateNodeAssociation(dataContext, section, Constants.BDObjectType.BDChapter);

                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, BDMetadata.LayoutVariantType.TreatmentRecommendation01, category);
                BDMetadata.Save(dataContext, meta);
            }

            if ((diseaseData != string.Empty) && ((null == disease) || (disease.name != diseaseData)))
            {
                disease = BDNode.CreateNode(dataContext, Constants.BDObjectType.BDDisease, Guid.Parse(uuidData));
                disease.name = diseaseData;
                disease.SetParent(category);
                disease.displayOrder = idxDisease++;
                BDNode.Save(dataContext, disease);

                presentation = null;

                BDNodeAssociation.CreateNodeAssociation(dataContext, category, Constants.BDObjectType.BDDisease);
      
                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, BDMetadata.LayoutVariantType.TreatmentRecommendation01, disease);
                BDMetadata.Save(dataContext, meta);
            }

            if ((presentationData != string.Empty) && ((null == presentation) || (presentation.name != presentationData)))
            {
                presentation = BDNode.CreateNode(dataContext, Constants.BDObjectType.BDPresentation, Guid.Parse(uuidData));
                presentation.name = presentationData;
                presentation.SetParent(disease);
                presentation.displayOrder = idxPresentation++;
                BDNode.Save(dataContext, presentation);

                BDNodeAssociation.CreateNodeAssociation(dataContext, disease, Constants.BDObjectType.BDPresentation);
                BDNodeAssociation.CreateNodeAssociation(dataContext, presentation, Constants.BDObjectType.BDPathogenGroup);

                BDMetadata meta = BDMetadata.CreateMetadata(dataContext, BDMetadata.LayoutVariantType.TreatmentRecommendation01, presentation);
                BDMetadata.Save(dataContext, meta);
            }
        }
    }
}
