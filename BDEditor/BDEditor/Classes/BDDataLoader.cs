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
            chapter2c,
            chapter2d
        }

        char[] delimiters = new char[] { '\t' };

        private Entities dataContext;

        BDNode chapter;
        BDNode section;
        BDNode category;
        BDNode subcategory;
        BDNode disease;
        BDNode presentation;

        string uuidData = null;
        string chapterData = null;
        string sectionData = null;
        string categoryData = null;
        string subcategoryData = null;
        string diseaseData = null;
        string presentationData = null;

        short idxChapter = 0;
        short idxSection = 0;
        short idxCategory = 0;
        short idxSubcategory = 0;
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

            List<BDNode> sections = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDSection);
            if (sections.Count > 0)
                idxSection = (short)(sections.Count - 1);
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
                            case baseDataDefinitionType.chapter2b:
                                ProcessChapter2bInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter2c:
                                ProcessChapter2cInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter2d:
                                ProcessChapter2dInputLine(input);
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

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            //subCategoryData = string.Empty;
            diseaseData = string.Empty;
            presentationData = string.Empty;

            if(elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            //if(elements.Length > 4) subCategoryData = elements[4];
            if (elements.Length > 5) diseaseData = elements[5];
            if(elements.Length > 6) presentationData = elements[6];

            if( (chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                chapter = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDChapter,Guid.Parse(uuidData));
                chapter.name = chapterData;
                chapter.displayOrder = idxChapter++;
                chapter.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation00;
                chapter.SetParent(null);

                BDNode.Save(dataContext, chapter);

                section = null;
                category = null;
                disease = null;
                presentation = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                section = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                section.name = sectionData;
                section.SetParent(chapter);
                section.displayOrder = idxSection++;
                section.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01;
                BDNode.Save(dataContext, section);

                category = null;
                disease = null;
                presentation = null;

            }
            if ((categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                category = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                category.name = categoryData;
                category.SetParent(section);
                category.displayOrder = idxCategory++;
                category.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01;
                BDNode.Save(dataContext, category);

                disease = null;
                presentation = null;
            }

            if ((diseaseData != string.Empty) && ((null == disease) || (disease.name != diseaseData)))
            {
                disease = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                disease.name = diseaseData;
                disease.SetParent(category);
                disease.displayOrder = idxDisease++;
                disease.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01;
                BDNode.Save(dataContext, disease);

                presentation = null;
            }

            if ((presentationData != string.Empty) && ((null == presentation) || (presentation.name != presentationData)))
            {
                presentation = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.Parse(uuidData));
                presentation.name = presentationData;
                presentation.SetParent(disease);
                presentation.displayOrder = idxPresentation++;
                presentation.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01;
                BDNode.Save(dataContext, presentation);
            }
        }

        private void ProcessChapter2bInputLine(string pInputLine)
        {
            processChapter2bInputLine(pInputLine, baseDataDefinitionType.chapter2b);
        }

        private void processChapter2bInputLine(string pInputLine, baseDataDefinitionType pDataDefinitionType)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            BDConstants.LayoutVariantType layoutVariant = BDConstants.LayoutVariantType.Undefined;

            if (pDataDefinitionType == baseDataDefinitionType.chapter2b)
                layoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic;
            else if (pDataDefinitionType == baseDataDefinitionType.chapter2c)
                layoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal;


            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            diseaseData = string.Empty;
            presentationData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) diseaseData = elements[3];
            if (elements.Length > 4) presentationData = elements[4];

            if ((chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                chapter = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                chapter.name = chapterData;
                chapter.displayOrder = idxChapter++;
                chapter.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation00;
                chapter.SetParent(null);

                BDNode.Save(dataContext, chapter);

                section = null;
                disease = null;
                presentation = null;
            }
            else
                chapter = BDNode.RetrieveNodeWithId(dataContext, new Guid(@"f92fec3a-379d-41ef-a981-5ddf9c9a9f0e"));

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = layoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                    section = sectionNode;
                disease = null;
                presentation = null;

            }

            if ((null != diseaseData && diseaseData != string.Empty) && ((null == disease) || (disease.name != diseaseData)))
            {
                disease = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                disease.name = diseaseData;
                disease.SetParent(section);
                disease.displayOrder = idxDisease++;
                disease.LayoutVariant = layoutVariant;
                BDNode.Save(dataContext, disease);

                presentation = null;
            }

            if ((null != presentationData && presentationData != string.Empty) && ((null == presentation) || (presentation.name != presentationData)))
            {
                presentation = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.Parse(uuidData));
                presentation.name = presentationData;
                presentation.SetParent(disease);
                presentation.displayOrder = idxPresentation++;
                presentation.LayoutVariant = layoutVariant;
                BDNode.Save(dataContext, presentation);
            }
        }

        private void ProcessChapter2cInputLine(string pInputLine)
        {
            processChapter2bInputLine(pInputLine, baseDataDefinitionType.chapter2c);
        }

        private void ProcessChapter2dInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            diseaseData = string.Empty;
            presentationData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];
            if (elements.Length > 5) diseaseData = elements[5];
            if (elements.Length > 6) presentationData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                chapter = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                chapter.name = chapterData;
                chapter.displayOrder = idxChapter++;
                chapter.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation00;
                chapter.SetParent(null);

                BDNode.Save(dataContext, chapter);

                section = null;
                category = null;
                subcategory = null;
                disease = null;
                presentation = null;
            }
            else
                chapter = BDNode.RetrieveNodeWithId(dataContext, new Guid(@"f92fec3a-379d-41ef-a981-5ddf9c9a9f0e"));

            if ((null != sectionData && sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                section = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                section.name = sectionData;
                section.SetParent(chapter);
                section.displayOrder = idxSection++;
                section.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic;
                BDNode.Save(dataContext, section);

                category = null;
                disease = null;
                presentation = null;

            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                category = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                category.name = categoryData;
                category.SetParent(section);
                category.displayOrder = idxCategory++;
                category.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic;
                BDNode.Save(dataContext, category);

                disease = null;
                presentation = null;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                subcategory = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDSubCategory, Guid.Parse(uuidData));
                subcategory.name = subcategoryData;
                subcategory.SetParent(category);
                subcategory.displayOrder = idxSubcategory++;
                subcategory.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic;
                BDNode.Save(dataContext, subcategory);

                disease = null;
                presentation = null;
            }

            if ((null != diseaseData && diseaseData != string.Empty) && ((null == disease) || (disease.name != diseaseData)))
            {
                disease = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                disease.name = diseaseData;
                disease.SetParent(subcategory);
                disease.displayOrder = idxDisease++;
                disease.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic;
                BDNode.Save(dataContext, disease);

                presentation = null;
            }

            if ((null != presentationData && presentationData != string.Empty) && ((null == presentation) || (presentation.name != presentationData)))
            {
                presentation = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.Parse(uuidData));
                presentation.name = presentationData;
                presentation.SetParent(disease);
                presentation.displayOrder = idxPresentation++;
                presentation.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic;
                BDNode.Save(dataContext, presentation);
            }
        }
    }
}
