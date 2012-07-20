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
            none,
            chapter1a, // Antibiotics
            chapter1b,
            chapter1c,
            chapter1d,
            chapter1e,
            chapter1f,
            chapter1g,
            chapter1h,
            chapter1i,
            chapter1j,

            chapter2a,
            chapter2b,
            chapter2c,
            chapter2d,
            chapter2e,
            chapter2f,

            chapter3a, // Prophylaxis
            chapter3b,
            chapter3c,
            chapter3d,
            chapter3e,
            chapter3f,
            chapter3g,
            chapter3h,
            chapter3i,
            chapter3j,
            chapter3k,
            chapter3l,

            chapter4a, //Dental
            chapter4b,
            chapter4c,
            chapter4d,
            chapter4e,
            chapter4f,
            chapter4g,
            chapter4h,

            chapter5a, //Pregnancy
            chapter5b,
            chapter5c,
            chapter5d,
            chapter5e,

            chapter6a, //Microbiology
            chapter6b,
            chapter6c,
            chapter6d
        }

        char[] delimiters = new char[] { '\t' };

        private Entities dataContext;

        private Guid chapter1Uuid = new Guid("45e13826-aedb-48d0-baf6-2f06ff45017f");

        BDNode chapter;
        BDNode section;
        BDNode category;
        BDNode subcategory;
        BDNode disease;
        BDNode presentation;
        BDNode pathogenGroup;
        BDNode pathogen;
        BDNode table;
        BDNode tableSection;
        BDNode tableSubsection;
        BDTableRow tableRow;
        BDTableRow tableHeaderRow;
        BDTableCell tableCell;
        BDTableCell tableHeaderCell;
        BDNode antimicrobial;
        BDNode dosageGroup;
        BDDosage dosage;
        BDNode subsection;
        BDNode topic;
        BDNode subtopic;
        BDTherapyGroup therapyGroup;
        BDTherapy therapy;
        BDNode surgery;
        BDNode microorganism;
        BDNode microorganismGroup;
        BDNode antimicrobialGroup;
        BDNode surgeryClassification;
        BDNode surgeryGroup;

        string uuidData = null;
        string chapterData = null;
        string sectionData = null;
        string categoryData = null;
        string subcategoryData = null;
        string diseaseData = null;
        string presentationData = null;
        string pathogenGroupData = null;
        string pathogenData = null;
        string tableData = null;
        string tableSectionData = null;
        string tableSubsectionData = null;
        string tableHeaderRowData = null;
        string tableRowData = null;
        string tableCellData = null;
        string tableHeaderCellData = null;
        string antimicrobialData = null;
        string dosageGroupData = null;
        string dosageData = null;
        string subsectionData = null;
        string topicData = null;
        string subtopicData = null;
        string therapyGroupData = null;
        string therapyData = null;
        string surgeryData = null;
        string microorganismData = null;
        string microorganismGroupData = null;
        string antimicrobialGroupData = null;
        string surgeryClassificationData = null;
        string surgeryGroupData = null;

        short idxChapter = 0;
        short idxSection = 0;
        short idxCategory = 0;
        short idxSubcategory = 0;
        short idxDisease = 0;
        short idxPresentation = 0;
        short idxPathogenGroup = 0;
        short idxPathogen = 0;
        short idxTable = 0;
        short idxTableChildren = 0;
        short idxTableCell = 0;
        short idxTableHeaderCell = 0;
        short idxAntimicrobial = 0;
        short idxDosageGroup = 0;
        short idxDosage = 0;
        short idxSubsection = 0;
        short idxTopic = 0;
        short idxSubtopic = 0;
        short idxTherapyGroup = 0;
        short idxTherapy = 0;
        short idxSurgery = 0;
        short idxMicroorganism = 0;
        short idxMicroorganismGroup = 0;
        short idxAntimicrobialGroup = 0;
        short idxSurgeryClassification = 0;
        short idxSurgeryGroup = 0;

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
                idxSection = (short)(sections.Count);
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
                            case baseDataDefinitionType.chapter1a:
                                ProcessChapter1aInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1b:
                                ProcessChapter1bInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1c:
                                ProcessChapter1cInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1d:
                                ProcessChapter1dInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1e:
                                ProcessChapter1eInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1f:
                                ProcessChapter1fInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1g:
                                ProcessChapter1gInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1h:
                                ProcessChapter1hInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1i:
                                ProcessChapter1iInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter1j:
                                ProcessChapter1jInputLine(input);
                                break;

                                // treatment recommendations
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
                            case baseDataDefinitionType.chapter2e:
                                ProcessChapter2eInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter2f:
                                ProcessChapter2fInputLine(input);
                                break;

                                // prophylaxis
                            case baseDataDefinitionType.chapter3a:
                                ProcessChapter3aInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3b:
                                ProcessChapter3bInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3c:
                                ProcessChapter3cInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3d:
                                ProcessChapter3dInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3e:
                                ProcessChapter3eInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3f:
                                ProcessChapter3fInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3g:
                                ProcessChapter3gInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3h:
                                ProcessChapter3hInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3i:
                                ProcessChapter3iInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3j:
                                ProcessChapter3jInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3k:
                                ProcessChapter3kInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter3l:
                                ProcessChapter3lInputLine(input);
                                break;

                                //dental
                            case baseDataDefinitionType.chapter4a:
                                ProcessChapter4aInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter4b:
                                ProcessChapter4bInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter4c:
                                ProcessChapter4cInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter4d:
                                ProcessChapter4dInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter4e:
                                ProcessChapter4eInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter4f:
                                ProcessChapter4fInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter4g:
                                ProcessChapter4gInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter4h:
                                ProcessChapter4hInputLine(input);
                                break;

                            case baseDataDefinitionType.chapter5a:
                                ProcessChapter5aInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter5b:
                                ProcessChapter5bInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter5c:
                                ProcessChapter5cInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter5d:
                                ProcessChapter5dInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter5e:
                                ProcessChapter5eInputLine(input);
                                break;
                            // microbiology
                            case baseDataDefinitionType.chapter6a:
                                ProcessChapter6aInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter6b:
                                ProcessChapter6bInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter6c:
                                ProcessChapter6cInputLine(input);
                                break;
                            case baseDataDefinitionType.chapter6d:
                                ProcessChapter6dInputLine(input);
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
                chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter,Guid.Parse(uuidData));
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
                section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
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
                category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
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
                disease = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                disease.name = diseaseData;
                disease.SetParent(category);
                disease.displayOrder = idxDisease++;
                disease.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01;
                BDNode.Save(dataContext, disease);

                presentation = null;
            }

            if ((presentationData != string.Empty) && ((null == presentation) || (presentation.name != presentationData)))
            {
                presentation = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.Parse(uuidData));
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
                chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
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
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
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
                disease = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                disease.name = diseaseData;
                disease.SetParent(section);
                disease.displayOrder = idxDisease++;
                disease.LayoutVariant = layoutVariant;
                BDNode.Save(dataContext, disease);

                presentation = null;
            }

            if ((null != presentationData && presentationData != string.Empty) && ((null == presentation) || (presentation.name != presentationData)))
            {
                presentation = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.Parse(uuidData));
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
            pathogenGroupData = string.Empty;
            pathogenData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) pathogenGroupData = elements[4];
            if (elements.Length > 5) pathogenData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
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
                // retrieve chapter 2
                chapter = BDNode.RetrieveNodeWithId(dataContext, new Guid(@"f92fec3a-379d-41ef-a981-5ddf9c9a9f0e"));

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I; ;
                    BDNode.Save(dataContext, section);
                }
                else
                    section = sectionNode;
                disease = null;
                presentation = null;


            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                category.name = categoryData;
                category.SetParent(section);
                category.displayOrder = idxCategory++;
                category.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I;
                BDNode.Save(dataContext, category);

                disease = null;
                presentation = null;
            }

            if ((null != pathogenGroupData && pathogenGroupData != string.Empty) && ((null == pathogenGroup) || (pathogenGroup.name != pathogenGroupData)))
            {
                pathogenGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPathogenGroup, Guid.Parse(uuidData));
                pathogenGroup.name = pathogenGroupData;
                pathogenGroup.SetParent(category);
                pathogenGroup.displayOrder = idxPathogenGroup++;
                pathogenGroup.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I;
                BDNode.Save(dataContext, pathogenGroup);

                pathogen = null;
            }

            if ((null != pathogenData && pathogenData != string.Empty) && ((null == pathogen) || (pathogen.name != pathogenData)))
            {
                pathogen = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPathogen, Guid.Parse(uuidData));
                pathogen.name = pathogenData;
                pathogen.SetParent(pathogenGroup);
                pathogen.displayOrder = idxPathogen++;
                pathogen.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I;
                BDNode.Save(dataContext, pathogen);

            }
        }

        private void ProcessChapter2eInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            pathogenData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) pathogenData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
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
                // retrieve chapter 2
                chapter = BDNode.RetrieveNodeWithId(dataContext, new Guid(@"f92fec3a-379d-41ef-a981-5ddf9c9a9f0e"));

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II; ;
                    BDNode.Save(dataContext, section);
                }
                else
                    section = sectionNode; // assign Parasitic section
                category = null;
                pathogen = null;

            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode categoryNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == categoryNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II;
                    BDNode.Save(dataContext, category);
                }
                else
                    category = categoryNode;
                pathogen = null;
            }

            if ((null != pathogenData && pathogenData != string.Empty) && ((null == pathogen) || (pathogen.name != pathogenData)))
            {
                pathogen = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPathogen, Guid.Parse(uuidData));
                pathogen.name = pathogenData;
                pathogen.SetParent(category);
                pathogen.displayOrder = idxPathogen++;
                pathogen.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II;
                BDNode.Save(dataContext, pathogen);

            }
        }

        private void ProcessChapter2fInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation01;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType diseaseLayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE;
            BDConstants.LayoutVariantType pathogenLayoutVariant = diseaseLayoutVariant;
            BDConstants.LayoutVariantType topicLayoutVariant = diseaseLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            diseaseData = string.Empty;
            pathogenData = string.Empty;
            topicData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) diseaseData = elements[4];
            if (elements.Length > 5) pathogenData = elements[5];
            if (elements.Length > 6) topicData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                disease = null;
                pathogen = null;
                topic = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxDisease = 0;
                idxPathogen = 0;
                idxTopic = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                    disease = null;
                    pathogen = null;
                    topic = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxDisease = 0;
                idxPathogen = 0;
                idxTopic = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                disease = null;
                pathogen = null;
                topic = null;

                idxDisease = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxPathogen = 0;
                idxTopic = 0;
            }
            if ((null != diseaseData && diseaseData != string.Empty) && ((null == disease) || (disease.name != diseaseData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    disease = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                    disease.name = (diseaseData == @"<blank>") ? string.Empty : diseaseData;
                    disease.SetParent(category);
                    disease.displayOrder = idxDisease++;
                    disease.LayoutVariant = diseaseLayoutVariant;
                    BDNode.Save(dataContext, disease);
                }
                else
                {
                    disease = tmpNode;
                    idxDisease++;
                }
                pathogen = null;
                topic = null;

                idxPathogen = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, disease);
                idxTopic = 0;
            }
            if ((null != pathogenData && pathogenData != string.Empty) && ((null == pathogen) || (pathogen.name != pathogenData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    pathogen = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPathogen, Guid.Parse(uuidData));
                    pathogen.name = (pathogenData == @"<blank>") ? string.Empty : pathogenData;
                    pathogen.SetParent(disease);
                    pathogen.displayOrder = idxPathogen++;
                    pathogen.LayoutVariant = pathogenLayoutVariant;
                    BDNode.Save(dataContext, pathogen);
                }
                else
                {
                    pathogen = tmpNode;
                    idxPathogen++;
                }
                topic = null;

                idxTopic = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, disease);
            }
            if ((null != topicData && topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = (topicData == @"<blank>") ? string.Empty : topicData;
                    topic.SetParent(pathogen);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = pathogenLayoutVariant;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
            }
        }

        private void ProcessChapter1aInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            antimicrobialData = string.Empty;
            topicData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) antimicrobialData = elements[3];
            if (elements.Length > 4) topicData = elements[4];
            if (elements.Length > 5) pathogenGroupData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);

                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                }
                section = null;
                antimicrobial = null;
                topic = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxAntimicrobial = 0;
                idxTopic = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
                    BDNode.Save(dataContext, section);

                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }

                antimicrobial = null;
                topic = null;
                    idxAntimicrobial = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                    idxTopic = 0;
            }
            if ((null != antimicrobialData && antimicrobialData != string.Empty) && ((null == antimicrobial) || (antimicrobial.name != antimicrobialData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    antimicrobial = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial, Guid.Parse(uuidData));
                    antimicrobial.name = antimicrobialData;
                    antimicrobial.SetParent(section);
                    antimicrobial.displayOrder = idxAntimicrobial++;
                    antimicrobial.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
                    BDNode.Save(dataContext, antimicrobial);
                }
                else
                {
                    antimicrobial = tmpNode;
                    idxAntimicrobial++;
                }
                topic = null;
                idxTopic = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, antimicrobial);
            }

            if ((null != topicData && topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = topicData;
                    topic.SetParent(antimicrobial);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
                    BDNode.Save(dataContext, topic);
                }
            }
            if ((null != pathogenGroupData && pathogenGroupData != string.Empty) && ((null == pathogenGroup) || (pathogenGroup.name != pathogenGroupData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    pathogenGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPathogenGroup, Guid.Parse(uuidData));
                    pathogenGroup.name = pathogenGroupData;
                    pathogenGroup.SetParent(antimicrobial);
                    pathogenGroup.displayOrder = idxTopic++; // share display order indexing with Topic as they have the same parent
                    pathogenGroup.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
                    BDNode.Save(dataContext, pathogenGroup);
                }
            }
        }
        
        /// <summary>
        /// Pharmacodynamics
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1bInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data
            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType antimicrobialGroupLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            antimicrobialGroupData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) antimicrobialGroupData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                antimicrobialGroup = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxAntimicrobialGroup = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                antimicrobialGroup = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxAntimicrobialGroup = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                antimicrobialGroup = null;

                idxAntimicrobialGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
            }

            if ((null != antimicrobialGroupData && antimicrobialGroupData != string.Empty) && ((null == antimicrobialGroup) || (antimicrobialGroup.name != antimicrobialGroupData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    antimicrobialGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobialGroup, Guid.Parse(uuidData));
                    antimicrobialGroup.name = antimicrobialGroupData;
                    antimicrobialGroup.SetParent(category);
                    antimicrobialGroup.displayOrder = idxAntimicrobialGroup++;
                    antimicrobialGroup.LayoutVariant = antimicrobialGroupLayoutVariant;
                    BDNode.Save(dataContext, antimicrobialGroup);
                }
                else
                {
                    antimicrobialGroup = tmpNode;
                    idxAntimicrobialGroup++;
                }
            }
        }        
        
        /// <summary>
        /// Dosing and Costs
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1cInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if(elements.Length > 4) subcategoryData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);

                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                }
                section = null;
                category = null;
                subcategory = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts;
                    BDNode.Save(dataContext, section);

                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;

                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
            }
            if ((categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                        category.name = categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = tmpNode;
                    idxCategory++;
                }
                subcategory = null;
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
            }
            if ((subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = tmpNode;
                    idxSubcategory++;
                }
            }
        }

        /// <summary>
        /// Dosing and Monitoring
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1dInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            subsectionData = string.Empty;
            topicData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) topicData = elements[3];
            
            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);

                    idxSection = 0;
                    idxTopic = 0;
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                topic = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring;
                    BDNode.Save(dataContext, section);

                    idxTopic = 0;
                }
                else
                {
                    section = sectionNode; // assign Parasitic section
                    idxSection++;
                }
                topic = null;
            }
            
            if ((topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = topicData;
                    topic.SetParent(section);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
            }
        }

        /// <summary>
        /// Renal Impairment
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1eInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            antimicrobialData = string.Empty;
            dosageGroupData = string.Empty;
            dosageData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) antimicrobialData = elements[4];

            if ((chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }

                section = null;
                category = null;
                subcategory = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                
                category = null;
                antimicrobial = null;
                dosageGroup = null;
                dosage = null;

            }
            if ((categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = tmpNode;
                    idxCategory++;
                }
                antimicrobial = null;
                dosageGroup = null;
                dosage = null;
            }
            if ((antimicrobialData != string.Empty) && ((null == antimicrobial) || (antimicrobial.name != antimicrobialData)))
            {
                BDNode amNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == amNode)
                {
                    antimicrobial = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial, Guid.Parse(uuidData));
                    antimicrobial.name = antimicrobialData;
                    antimicrobial.SetParent(category);
                    antimicrobial.displayOrder = idxAntimicrobial++;
                    antimicrobial.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment;
                    BDNode.Save(dataContext, antimicrobial);
                }
                else
                {
                    antimicrobial = amNode;
                    idxAntimicrobial++;
                }
                dosageGroup = null;
                dosage = null;
            }
            if ((dosageGroupData != string.Empty) && ((null == dosageGroup) || (dosageGroup.name != dosageGroupData)))
            {
                BDNode dgNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == dgNode
                    )
                {
                    dosageGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDosageGroup, Guid.Parse(uuidData));
                    dosageGroup.name = dosageGroupData;
                    dosageGroup.SetParent(antimicrobial);
                    dosageGroup.displayOrder = idxDosageGroup++;
                    dosageGroup.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment;
                    BDNode.Save(dataContext, dosageGroup);
                }
                else
                {
                    dosageGroup = dgNode;
                    idxDosageGroup++;
                }
                dosage = null;
            }
            if ((dosageData != string.Empty) && ((null == dosage) || (dosage.name != dosageData)))
            {
                BDDosage tmpDosage = BDDosage.RetrieveDosageWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpDosage)
                {
                    dosage = BDDosage.CreateBDDosage(dataContext, Guid.Parse(uuidData));
                    dosage.name = dosageData;
                    dosage.SetParent(dosageGroup);
                    dosage.displayOrder = idxDosage++;
                    dosage.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment;
                    BDDosage.Save(dataContext, dosage);
                }
                else
                {
                    dosage = tmpDosage;
                    idxDosage++;
                }
            }
        }

        /// <summary>
        /// Hepatic impairment
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1fInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            antimicrobialData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) antimicrobialData = elements[4];

            if ((chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }

                section = null;
                category = null;
                antimicrobial = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == sNode)
                {
                section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                section.name = sectionData;
                section.SetParent(chapter);
                section.displayOrder = idxSection++;
                section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment;
                BDNode.Save(dataContext, section);
                }
                else
                                    {
                    section = sNode;
                    idxSection++;
                }

                category = null;
                antimicrobial = null;

            }
            if ((categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if(null == tmpNode)
                {
                category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                category.name = categoryData;
                category.SetParent(section);
                category.displayOrder = idxCategory++;
                category.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment;
                BDNode.Save(dataContext, category);
                }
                else
                                    {
                    category = tmpNode;
                    idxCategory++;
                }

                antimicrobial = null;
            }
            if ((antimicrobialData != string.Empty) && ((null == antimicrobial) || (antimicrobial.name != antimicrobialData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    antimicrobial = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial, Guid.Parse(uuidData));
                    antimicrobial.name = antimicrobialData;
                    antimicrobial.SetParent(category);
                    antimicrobial.displayOrder = idxAntimicrobial++;
                    antimicrobial.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment;
                    BDNode.Save(dataContext, antimicrobial);
                }
                else
                {
                    antimicrobial = tmpNode;
                    idxAntimicrobial++;
                }
            }
        }

        /// <summary>
        /// Name Listing
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1gInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;
            tableSectionData = string.Empty;
            tableSubsectionData = string.Empty;
            tableRowData = string.Empty;
            tableCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) tableData = elements[3];
            if (elements.Length > 4) tableHeaderRowData = elements[4];
            if (elements.Length > 5) tableHeaderCellData = elements[5];
            if (elements.Length > 6) tableSectionData = elements[6];
            if (elements.Length > 7) tableSubsectionData = elements[7];
            if (elements.Length > 8) tableCellData = elements[8];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);

                    idxSection = 0;
                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                    idxTableCell = 0;
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_NameListing;
                    BDNode.Save(dataContext, section);

                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                    idxTableCell = 0;
                }
                else
                {
                    section = sectionNode; // assign Parasitic section
                    idxSection++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }
            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {
                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(section);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column;
                    BDNode.Save(dataContext, table);

                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                    idxTableCell = 0;
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"<blank>") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
                    BDTableRow.Save(dataContext, tableHeaderRow);

                    idxTableHeaderCell = 0;
                    idxTableCell = 0;

                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
                        tableHeaderRow.SetParent(tableSection);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
                tableSection = null;
                tableRow = null;
                tableCell = null;

            }
            if ((null != tableSectionData && tableSectionData != string.Empty) && ((null == tableSection) || (tableSection.name != tableSectionData)))
            {
                BDNode tmpRow = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableSection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTableSection, Guid.Parse(uuidData));

                    tableSection.name = (tableSectionData == @"<blank>") ? string.Empty : tableSectionData;
                    tableSection.SetParent(table);
                    tableSection.displayOrder = idxTableChildren++;
                    tableSection.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column;
                    BDNode.Save(dataContext, tableSection);

                    // do not reset table row counter - otherwise child rows will not sort correctly with header row
                    idxTableCell = 0;
                }
                else
                {
                    tableSection = tmpRow;
                    idxTableChildren++;
                }

                tableRow = null;
                tableCell = null;
            }
            if ((null != tableSubsectionData && tableSubsectionData != string.Empty) && ((null == tableSubsection) || (tableSubsection.name != tableSubsectionData)))
            {
                BDNode tmpRow = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableSubsection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTableSubsection, Guid.Parse(uuidData));

                    tableSubsection.name = (tableSubsectionData == @"<blank>") ? string.Empty : tableSubsectionData;
                    tableSubsection.SetParent(tableSection);
                    tableSubsection.displayOrder = idxTableChildren++;
                    tableSubsection.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column;
                    BDNode.Save(dataContext, tableSubsection);

                    // do not reset table row counter - otherwise child rows will not sort correctly with header row
                    idxTableCell = 0;
                }
                else
                {
                    tableSubsection = tmpRow;
                    idxTableChildren++;
                }

                tableRow = null;
                tableCell = null;
            }
            if ((null != tableCellData && tableCellData != string.Empty) && ((null == tableCell) || tableCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableRow)
                    {
                        tableRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableRow.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_ContentRow;
                        if (null == tableSubsectionData || tableSubsectionData == string.Empty)
                            tableRow.SetParent(tableSection);
                        else
                            tableRow.SetParent(tableSubsection);
                        tableRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableRow);
                        idxTableCell = 0;
                    }
                    tableCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableCell.displayOrder = idxTableCell++;
                    tableCell.SetParent(tableRow);
                    tableCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;

                    BDTableCell.Save(dataContext, tableCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableCell.Uuid);
                    cellValue.value = tableCellData;
                    BDString.Save(dataContext, cellValue);

                    BDTableCell cell2 = BDTableCell.CreateBDTableCell(dataContext);
                    cell2.displayOrder = idxTableCell++;
                    cell2.SetParent(tableRow);
                    cell2.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                    BDTableCell.Save(dataContext, cell2);

                    BDString cell2Value = BDString.CreateBDString(dataContext);
                    cell2Value.displayOrder = 0;
                    cell2Value.SetParent(cell2.Uuid);
                    cell2Value.value = string.Empty;
                    BDString.Save(dataContext, cell2Value);

                    BDTableCell cell3 = BDTableCell.CreateBDTableCell(dataContext);
                    cell3.displayOrder = idxTableCell++;
                    cell3.SetParent(tableRow);
                    cell3.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                    BDTableCell.Save(dataContext, cell3);

                    BDString cell3Value = BDString.CreateBDString(dataContext);
                    cell3Value.displayOrder = 0;
                    cell3Value.SetParent(cell3.Uuid);
                    cell3Value.value = string.Empty;
                    BDString.Save(dataContext, cell3Value);
                }
                //tableRow = null;
                //tableCell = null;
            }
        }

        /// <summary>
        /// Stepdown recommendations
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1hInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) tableData = elements[3];
            if (elements.Length > 4) tableHeaderRowData = elements[4];
            if (elements.Length > 5) tableHeaderCellData = elements[5];
            
            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);

                    idxSection = 0;
                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Stepdown;
                    BDNode.Save(dataContext, section);

                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    section = sectionNode; // assign Parasitic section
                    idxSection++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
            }
            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {
                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(section);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = BDConstants.LayoutVariantType.Table_5_Column;
                    BDNode.Save(dataContext, table);

                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"(Header)") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = BDConstants.LayoutVariantType.Table_5_Column_HeaderRow;
                    BDTableRow.Save(dataContext, tableHeaderRow);

                    idxTableHeaderCell = 0;

                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;
            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = BDConstants.LayoutVariantType.Table_5_Column_HeaderRow;
                        tableHeaderRow.SetParent(table);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
            }
        }

        /// <summary>
        /// CSF Penetration
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1iInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data
            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Antibiotics_CSFPenetration;
            BDConstants.LayoutVariantType topicLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType antimicrobialLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            topicData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            antimicrobialData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) topicData = elements[3];
            if (elements.Length > 4) categoryData = elements[4];
            if (elements.Length > 5) subcategoryData = elements[5];
            if (elements.Length > 6) antimicrobialData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                topic = null;
                subcategory = null;
                antimicrobial = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTopic = 0;
                idxCategory = 0;
                idxSubcategory = 0;
                idxAntimicrobial = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                topic = null;
                category = null;
                subcategory = null;
                antimicrobial = null;
                idxTopic = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxCategory = 0;
                idxSubcategory = 0;
                idxAntimicrobial = 0;
            }
            if ((null != topicData && topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = (topicData == @"<blank>") ? string.Empty : topicData;
                    topic.SetParent(section);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = topicLayoutVariant;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
                category = null;
                subcategory = null;
                antimicrobial = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, topic);
                idxSubcategory = 0;
                idxAntimicrobial = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(topic);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                antimicrobial = null;
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxAntimicrobial = 0;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }

                antimicrobial = null;
                idxAntimicrobial = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, subcategory);
            }
            if ((null != antimicrobialData && antimicrobialData != string.Empty) && ((null == antimicrobial) || (antimicrobial.name != antimicrobialData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    antimicrobial = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial, Guid.Parse(uuidData));
                    antimicrobial.name = antimicrobialData;
                    antimicrobial.SetParent(subcategory);
                    antimicrobial.displayOrder = idxAntimicrobial++;
                    antimicrobial.LayoutVariant = antimicrobialLayoutVariant;
                    BDNode.Save(dataContext, antimicrobial);
                }
                else
                {
                    antimicrobial = tmpNode;
                    idxAntimicrobial++;
                }
            }
        }

        /// <summary>
        /// B-Lactam Allergy
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1jInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            subsectionData = string.Empty;
            topicData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;
            tableSectionData = string.Empty;
            tableSubsectionData = string.Empty;
            tableRowData = string.Empty;
            tableCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) subsectionData = elements[3];
            if (elements.Length > 4) topicData = elements[4];
            if (elements.Length > 5) tableData = elements[5];
            if (elements.Length > 6) tableHeaderRowData = elements[6];
            if (elements.Length > 7) tableHeaderCellData = elements[7];
            if (elements.Length > 8) tableSectionData = elements[8];
            if (elements.Length > 9) tableCellData = elements[9];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);

                    idxSection = 0;
                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                    idxTableCell = 0;
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy;
                    BDNode.Save(dataContext, section);

                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                    idxTableCell = 0;
                }
                else
                {
                    section = sectionNode; // assign Parasitic section
                    idxSection++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }
            if ((subsectionData != string.Empty) && ((null == subsection) || (subsection.name != subsectionData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    subsection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubsection, Guid.Parse(uuidData));
                    subsection.name = subsectionData;
                    subsection.SetParent(section);
                    subsection.displayOrder = idxSubsection++;
                    subsection.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy;
                    BDNode.Save(dataContext, subsection);

                    idxTopic = 0;
                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                    idxTableCell = 0;
                }
                else
                {
                    subsection = tmpNode; // assign Parasitic section
                    idxSubsection++;
                }
                topic = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }
            if ((topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = topicData;
                    topic.SetParent(subsection);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy;
                    BDNode.Save(dataContext, topic);

                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                    idxTableCell = 0;
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }
            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {
                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(topic);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column;
                    BDNode.Save(dataContext, table);

                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                    idxTableCell = 0;
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"<blank>") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
                    BDTableRow.Save(dataContext, tableHeaderRow);

                    idxTableHeaderCell = 0;
                    idxTableCell = 0;

                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
                        tableHeaderRow.SetParent(table);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
                tableSection = null;
                tableRow = null;
                tableCell = null;

            }
            if ((null != tableSectionData && tableSectionData != string.Empty) && ((null == tableSection) || (tableSection.name != tableSectionData)))
            {
                BDNode tmpRow = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableSection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTableSection, Guid.Parse(uuidData));

                    tableSection.name = (tableSectionData == @"<blank>") ? string.Empty : tableSectionData;
                    tableSection.SetParent(table);
                    tableSection.displayOrder = idxTableChildren++;
                    tableSection.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column;
                    BDNode.Save(dataContext, tableSection);

                    // do not reset table row counter - otherwise child rows will not sort correctly with header row
                    idxTableCell = 0;
                }
                else
                {
                    tableSection = tmpRow;
                    idxTableChildren++;
                }

                tableRow = null;
                tableCell = null;
            }
            if ((null != tableCellData && tableCellData != string.Empty) && ((null == tableCell) || tableCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableRow)
                    {
                        tableRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableRow.LayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_ContentRow;
                        tableRow.SetParent(tableSection);
                        tableRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableRow);
                        idxTableCell = 0;
                    }
                    tableCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableCell.displayOrder = idxTableCell++;
                    tableCell.SetParent(tableRow);
                    tableCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;

                    BDTableCell.Save(dataContext, tableCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableCell.Uuid);
                    cellValue.value = tableCellData;
                    BDString.Save(dataContext, cellValue);

                    BDTableCell cell2 = BDTableCell.CreateBDTableCell(dataContext);
                    cell2.displayOrder = idxTableCell++;
                    cell2.SetParent(tableRow);
                    cell2.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                    BDTableCell.Save(dataContext, cell2);

                    BDString cell2Value = BDString.CreateBDString(dataContext);
                    cell2Value.displayOrder = 0;
                    cell2Value.SetParent(cell2.Uuid);
                    cell2Value.value = string.Empty;
                    BDString.Save(dataContext, cell2Value);

                    BDTableCell cell3 = BDTableCell.CreateBDTableCell(dataContext);
                    cell3.displayOrder = idxTableCell++;
                    cell3.SetParent(tableRow);
                    cell3.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                    BDTableCell.Save(dataContext, cell3);

                    BDString cell3Value = BDString.CreateBDString(dataContext);
                    cell3Value.displayOrder = 0;
                    cell3Value.SetParent(cell3.Uuid);
                    cell3Value.value = string.Empty;
                    BDString.Save(dataContext, cell3Value);
                }
                tableRow = null;
                tableCell = null;
            }
        }

        /// <summary>
        /// Antimicrobial Prophylaxis in Dentistry
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter4aInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Dental;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_Prophylaxis;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant; //BDConstants.LayoutVariantType.Dental_Prophylaxis
            BDConstants.LayoutVariantType therapyGroupLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            therapyGroupData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) therapyGroupData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxTherapyGroup = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode; 
                    idxSection++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTherapyGroup = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }

                therapyGroup = null;
                idxTherapyGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
           }

            if ((null != therapyGroupData && therapyGroupData != string.Empty) && ((null == therapyGroup) || (therapyGroup.name != therapyGroupData)))
            {
                BDTherapyGroup tmpNode = BDTherapyGroup.RetrieveTherapyGroupWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    therapyGroup = BDTherapyGroup.CreateBDTherapyGroup(dataContext, Guid.Parse(uuidData));
                    therapyGroup.name = therapyGroupData;
                    therapyGroup.SetParent(category);
                    therapyGroup.displayOrder = idxTherapyGroup++;
                    therapyGroup.LayoutVariant = therapyGroupLayoutVariant;
                    BDTherapyGroup.Save(dataContext, therapyGroup);
                }
                else
                {
                    therapyGroup = tmpNode;
                    idxTherapyGroup++;
                }
            }
        }

        /// <summary>
        /// Dental Prophylaxis Endocarditis
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter4bInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Dental;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType topicLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType subtopicLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            topicData = string.Empty;
            subtopicData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) topicData = elements[4];
            if (elements.Length > 5) subtopicData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxTopic = 0;
                idxSubtopic = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;

                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTopic = 0;
                idxSubtopic = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }

                idxTopic = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxSubtopic = 0;
            }
            if ((null != topicData && topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = topicData;
                    topic.SetParent(category);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = topicLayoutVariant;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
            }

            if ((null != subtopicData && subtopicData != string.Empty) && ((null == subtopic) || (subtopic.name != subtopicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    subtopic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic, Guid.Parse(uuidData));
                    subtopic.name = subtopicData;
                    subtopic.SetParent(topic);
                    subtopic.displayOrder = idxSubtopic++;
                    subtopic.LayoutVariant = subtopicLayoutVariant;
                    BDNode.Save(dataContext, subtopic);
                }
                else
                {
                    subtopic = tmpNode;
                    idxSubtopic++;
                }
            }
        }

        /// <summary>
        /// Dental Prophylaxis Endocarditis - Antibiotic Regimen
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter4cInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Dental;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType therapyGroupLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            therapyGroupData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];
            if (elements.Length > 5) therapyGroupData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;
                therapyGroup = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
                idxTherapyGroup = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                therapyGroup = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
                idxTherapyGroup = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                therapyGroup = null;
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxTherapyGroup = 0;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }

                therapyGroup = null;
                idxTherapyGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, subcategory);
            }

            if ((null != therapyGroupData && therapyGroupData != string.Empty) && ((null == therapyGroup) || (therapyGroup.name != therapyGroupData)))
            {
                BDTherapyGroup tmpNode = BDTherapyGroup.RetrieveTherapyGroupWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    therapyGroup = BDTherapyGroup.CreateBDTherapyGroup(dataContext, Guid.Parse(uuidData));
                    therapyGroup.name = therapyGroupData;
                    therapyGroup.SetParent(subcategory);
                    therapyGroup.displayOrder = idxTherapyGroup++;
                    therapyGroup.LayoutVariant = therapyGroupLayoutVariant;
                    BDTherapyGroup.Save(dataContext, therapyGroup);
                }
                else
                {
                    therapyGroup = tmpNode;
                    idxTherapyGroup++;
                }
            }
        }

        /// <summary>
        /// dental prophylaxis - Prosthetics
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter4dInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Dental;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant; 
            BDConstants.LayoutVariantType subcategoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType therapyGroupLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType therapyLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            therapyGroupData = string.Empty;
            therapyData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];
            if (elements.Length > 5) therapyGroupData = elements[5];
            if (elements.Length > 6) therapyData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;
                therapyGroup = null;
                therapy = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
                idxTherapyGroup = 0;
                idxTherapy = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                therapyGroup = null;
                therapy = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
                idxTherapyGroup = 0;
                idxTherapy = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                therapyGroup = null;
                therapy = null;
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxTherapyGroup = 0;
                idxTherapy = 0;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }

                therapyGroup = null;
                therapy = null;
                idxTherapyGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, subcategory);
                idxTherapy = 0;
            }

            if ((null != therapyGroupData && therapyGroupData != string.Empty) && ((null == therapyGroup) || (therapyGroup.name != therapyGroupData)))
            {
                BDTherapyGroup tmpNode = BDTherapyGroup.RetrieveTherapyGroupWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    therapyGroup = BDTherapyGroup.CreateBDTherapyGroup(dataContext, Guid.Parse(uuidData));
                    therapyGroup.name = therapyGroupData;
                    therapyGroup.SetParent(subcategory);
                    therapyGroup.displayOrder = idxTherapyGroup++;
                    therapyGroup.LayoutVariant = therapyGroupLayoutVariant;
                    BDTherapyGroup.Save(dataContext, therapyGroup);
                }
                else
                {
                    therapyGroup = tmpNode;
                    idxTherapyGroup++;
                }
                therapy = null;
                idxTherapy = (short)BDTherapyGroup.RetrieveMaximumDisplayOrderForChildren(dataContext, therapyGroup);
            }
            if ((null != therapyData && therapyData != string.Empty) && ((null == therapy) || (therapy.name != therapyData)))
            {
                BDTherapy tmpNode = BDTherapy.RetrieveTherapyWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    therapy = BDTherapy.CreateBDTherapy(dataContext, Guid.Parse(uuidData));
                    therapy.name = therapyData;
                    therapy.SetParent(therapyGroup);
                    therapy.displayOrder = idxTherapy++;
                    therapy.LayoutVariant = therapyLayoutVariant;
                    BDTherapy.Save(dataContext, therapy);
                }
                else
                {
                    therapy = tmpNode;
                    idxTherapy++;
                }
            }
        }

        /// <summary>
        /// Dental Prophylaxis - Drug Regimens
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter4eInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Dental;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType surgeryLayoutVariant = sectionLayoutVariant;
            //BDConstants.LayoutVariantType therapyGroupLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            surgeryData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];
            if (elements.Length > 5) surgeryData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;
                surgery = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
                idxSurgery = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                surgery = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
                idxSurgery = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                surgery = null;
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxSurgery = 0;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }

                surgery = null;
                idxSurgery = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, subcategory);
            }

            if ((null != surgeryData && surgeryData != string.Empty) && ((null == surgery) || (surgery.name != surgeryData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    surgery = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSurgery, Guid.Parse(uuidData));
                    surgery.name = (surgeryData == @"<blank>") ? string.Empty : surgeryData;
                    surgery.SetParent(subcategory);
                    surgery.displayOrder = idxSurgery++;
                    surgery.LayoutVariant = surgeryLayoutVariant;
                    BDNode.Save(dataContext, surgery);
                }
                else
                {
                    surgery = tmpNode;
                    idxSurgery++;
                }
            }
        }

        /// <summary>
        /// Dental - Recommended Therapy - Antimicrobial Activity
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter4fInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Dental;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy;
            BDConstants.LayoutVariantType tableLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity;
            BDConstants.LayoutVariantType headerRowLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity_HeaderRow;
            BDConstants.LayoutVariantType contentRowLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity_ContentRow;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;
            tableSectionData = string.Empty;
            tableRowData = string.Empty;
            tableCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) tableData = elements[3];
            if (elements.Length > 4) tableHeaderRowData = elements[4];
            if (elements.Length > 5) tableHeaderCellData = elements[5];
            if (elements.Length > 6) tableRowData = elements[6];
            if (elements.Length > 7) tableCellData = elements[7];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);

                    idxSection = 0;
                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);

                    idxTable = 0;
                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }
            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {
                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(section);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, table);

                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"(Header)") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                    BDTableRow.Save(dataContext, tableHeaderRow);

                    idxTableHeaderCell = 0;
                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                        tableHeaderRow.SetParent(table);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
                tableSection = null;
                tableRow = null;
                tableCell = null;
            }

            if ((null != tableRowData && tableRowData != string.Empty) && ((null == tableRow) || (tableRow.name != tableRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableRow.name = (tableRowData == @"(Data)") ? string.Empty : tableRowData;
                    tableRow.SetParent(table);
                    tableRow.displayOrder = idxTableChildren++;
                    tableRow.LayoutVariant = contentRowLayoutVariant;
                    BDTableRow.Save(dataContext, tableRow);

                    idxTableCell = 0;
                }
                else
                {
                    tableRow = tmpRow;
                    idxTableChildren++;
                }

                tableCell = null;
            }

            if ((null != tableCellData && tableCellData != string.Empty) && ((null == tableCell) || tableCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableRow)
                    {
                        tableRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableRow.LayoutVariant = contentRowLayoutVariant;
                        if (null == tableSubsectionData || tableSubsectionData == string.Empty)
                            tableRow.SetParent(tableSection);
                        else
                            tableRow.SetParent(tableSubsection);
                        tableRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableRow);
                        idxTableCell = 0;
                    }
                    //BDFabrik.GetTableColumnCount() defines this row layoutVariant as having 8 cells
                    // 1
                    tableCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableCell.displayOrder = idxTableCell++;
                    tableCell.SetParent(tableRow);
                    tableCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                    BDTableCell.Save(dataContext, tableCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableCell.Uuid);
                    cellValue.value = tableCellData;
                    BDString.Save(dataContext, cellValue);

                    int columnCount = BDFabrik.GetTableColumnCount(contentRowLayoutVariant);
                    for (int i = 1; i < columnCount; i++)
                    {
                        BDTableCell idxCell = BDTableCell.CreateBDTableCell(dataContext);
                        idxCell.displayOrder = idxTableCell++;
                        idxCell.SetParent(tableRow);
                        idxCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                        BDTableCell.Save(dataContext, idxCell);

                        BDString idxCellValue = BDString.CreateBDString(dataContext);
                        idxCellValue.displayOrder = 0;
                        idxCellValue.SetParent(idxCell.Uuid);
                        idxCellValue.value = string.Empty;
                        BDString.Save(dataContext, idxCellValue);
                    }                  
                }
            }
        }

        /// <summary>
        /// Dental - Recommended Therapy - Microorganisms
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter4gInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Dental;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms;
            BDConstants.LayoutVariantType categoryLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }
            }
        }

        /// <summary>
        /// Dental - Recommended Therapy for selected infections
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter4hInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Dental;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType diseaseLayoutVariant = categoryLayoutVariant;
            BDConstants.LayoutVariantType presentationLayoutVariant = diseaseLayoutVariant;

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            diseaseData = string.Empty;
            presentationData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) diseaseData = elements[4];
            if (elements.Length > 5) presentationData = elements[5];

            if ((chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);

                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }

                section = null;
                category = null;
                disease = null;
                presentation = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                section.name = sectionData;
                section.SetParent(chapter);
                section.displayOrder = idxSection++;
                section.LayoutVariant = sectionLayoutVariant;
                BDNode.Save(dataContext, section);

                category = null;
                disease = null;
                presentation = null;

            }
            if ((categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                category.name = categoryData;
                category.SetParent(section);
                category.displayOrder = idxCategory++;
                category.LayoutVariant = categoryLayoutVariant;
                BDNode.Save(dataContext, category);

                disease = null;
                presentation = null;
            }

            if ((diseaseData != string.Empty) && ((null == disease) || (disease.name != diseaseData)))
            {
                disease = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                disease.name = diseaseData;
                disease.SetParent(category);
                disease.displayOrder = idxDisease++;
                disease.LayoutVariant = diseaseLayoutVariant;
                BDNode.Save(dataContext, disease);

                presentation = null;
            }

            if ((presentationData != string.Empty) && ((null == presentation) || (presentation.name != presentationData)))
            {
                presentation = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPresentation, Guid.Parse(uuidData));
                presentation.name = presentationData;
                presentation.SetParent(disease);
                presentation.displayOrder = idxPresentation++;
                presentation.LayoutVariant = presentationLayoutVariant;
                BDNode.Save(dataContext, presentation);
            }
        }

        /// <summary>
        /// Pregnancy - Antimicrobials
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter5aInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.PregancyLactation;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType antimicrobialLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            antimicrobialData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];
            if (elements.Length > 5) antimicrobialData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;
                antimicrobial = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
                idxAntimicrobial = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                antimicrobial = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
                idxAntimicrobial = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                antimicrobial = null;
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxAntimicrobial = 0;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }

                antimicrobial = null;
                idxAntimicrobial = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, subcategory);
            }
            if ((null != antimicrobialData && antimicrobialData != string.Empty) && ((null == antimicrobial) || (antimicrobial.name != antimicrobialData)))
            {
                if (null == subcategory)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.NewGuid());
                    subcategory.name = string.Empty;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    antimicrobial = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial, Guid.Parse(uuidData));
                    antimicrobial.name = antimicrobialData;

                    if (subcategory != null) antimicrobial.SetParent(subcategory);
                    else antimicrobial.SetParent(subcategory);
                    
                    antimicrobial.displayOrder = idxAntimicrobial++;
                    antimicrobial.LayoutVariant = antimicrobialLayoutVariant;
                    BDNode.Save(dataContext, antimicrobial);
                }
                else
                {
                    antimicrobial = tmpNode;
                    idxAntimicrobial++;
                }
            }
        }

        /// <summary>
        /// Lactation - Antimicrobials
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter5bInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.PregancyLactation;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType antimicrobialGroupLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType antimicrobialLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            antimicrobialGroupData = string.Empty;
            antimicrobialData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];
            if (elements.Length > 5) antimicrobialGroupData = elements[5];
            if (elements.Length > 6) antimicrobialData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;
                antimicrobialGroup = null;
                antimicrobial = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
                idxAntimicrobialGroup = 0;
                idxAntimicrobial = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                antimicrobialGroup = null;
                antimicrobial = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
                idxAntimicrobialGroup = 0;
                idxAntimicrobial = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                antimicrobialGroup = null;
                antimicrobial = null;
                
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxAntimicrobialGroup = 0;
                idxAntimicrobial = 0;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = string.Empty;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }
                antimicrobialGroup = null;
                antimicrobial = null;
                idxAntimicrobialGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, subcategory);
                idxAntimicrobial = 0;
            }
            if ((null != antimicrobialGroupData && antimicrobialGroupData != string.Empty) && ((null == antimicrobialGroup) || (antimicrobialGroup.name != antimicrobialGroupData)))
            {
                    if (null == subcategory)
                    {
                        subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.NewGuid());
                        subcategory.name = string.Empty;
                        subcategory.SetParent(category);
                        subcategory.displayOrder = idxSubcategory++;
                        subcategory.LayoutVariant = subcategoryLayoutVariant;
                        BDNode.Save(dataContext, subcategory);
                    }

                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    antimicrobialGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobialGroup, Guid.Parse(uuidData));
                    antimicrobialGroup.name = antimicrobialGroupData;
                    antimicrobialGroup.SetParent(subcategory);
                    antimicrobialGroup.displayOrder = idxAntimicrobialGroup++;
                    antimicrobialGroup.LayoutVariant = antimicrobialGroupLayoutVariant;
                    BDNode.Save(dataContext, antimicrobialGroup);
                }
                else
                {
                    antimicrobialGroup = tmpNode;
                    idxAntimicrobialGroup++;
                }
                antimicrobial = null;
                idxAntimicrobial = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, antimicrobialGroup);

            }
            if ((null != antimicrobialData && antimicrobialData != string.Empty) && ((null == antimicrobial) || (antimicrobial.name != antimicrobialData)))
            {
                if (null == subcategory)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.NewGuid());
                    subcategory.name = string.Empty;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                if (null == antimicrobialGroup)
                {
                    antimicrobialGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobialGroup, Guid.NewGuid());
                    antimicrobialGroup.name = string.Empty;
                    antimicrobialGroup.SetParent(subcategory);
                    antimicrobialGroup.displayOrder = idxAntimicrobialGroup++;
                    antimicrobialGroup.LayoutVariant = antimicrobialGroupLayoutVariant;
                    BDNode.Save(dataContext, antimicrobialGroup);
                }

                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    antimicrobial = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial, Guid.Parse(uuidData));
                    antimicrobial.name = antimicrobialData;
                    antimicrobial.SetParent(antimicrobialGroup);
                    antimicrobial.displayOrder = idxAntimicrobial++;
                    antimicrobial.LayoutVariant = antimicrobialLayoutVariant;
                    BDNode.Save(dataContext, antimicrobial);
                }
                else
                {
                    antimicrobial = tmpNode;
                    idxAntimicrobial++;
                }
            }
        }

        /// <summary>
        /// Pregnancy/Lactation - Exposure to Communicable Diseases
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter5cInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.PregancyLactation;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases;
            BDConstants.LayoutVariantType pathogenLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType topicLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            pathogenData = string.Empty;
            topicData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) pathogenData = elements[3];
            if (elements.Length > 4) topicData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                pathogen = null;
                topic = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxPathogen = 0;
                    idxTopic = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                pathogen = null;
                topic = null;

                idxPathogen = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                    idxTopic = 0;
            }
            if ((null != pathogenData && pathogenData != string.Empty) && ((null == pathogen) || (pathogen.name != pathogenData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    pathogen = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDPathogen, Guid.Parse(uuidData));
                    pathogen.name = (pathogenData == @"<blank>") ? string.Empty : pathogenData;
                    pathogen.SetParent(section);
                    pathogen.displayOrder = idxPathogen++;
                    pathogen.LayoutVariant = pathogenLayoutVariant;
                    BDNode.Save(dataContext, pathogen);
                }
                else
                {
                    pathogen = tmpNode;
                    idxPathogen++;
                }
            }
            if ((null != topicData && topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = (topicData == @"<blank>") ? string.Empty : topicData;
                    topic.SetParent(pathogen);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = topicLayoutVariant;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
            }
        }

        /// <summary>
        /// Prevention of Perniatal Infection
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter5dInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.PregancyLactation;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
            }
        }

        /// <summary>
        /// Perinatal HIV Protocol
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter5eInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.PregancyLactation;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
            }
        }

        /// <summary>
        /// Microbiology - Gram Stain Interpretation
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter6aInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Microbiology;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType microorganismLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            microorganismData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];
            if (elements.Length > 5) microorganismData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;
                microorganism = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
                idxMicroorganism = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                microorganism = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
                idxMicroorganism = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                microorganism = null;

                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxMicroorganism = 0;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = tmpNode;
                    idxSubcategory++;
                }
                microorganism = null;
                idxMicroorganism = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, subcategory);

            }
            if ((null != microorganismData && microorganismData != string.Empty) && ((null == microorganism) || (microorganism.name != microorganismData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    microorganism = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDMicroorganism, Guid.Parse(uuidData));
                    microorganism.name = microorganismData;
                    microorganism.SetParent(subcategory);
                    microorganism.displayOrder = idxMicroorganism++;
                    microorganism.LayoutVariant = microorganismLayoutVariant;
                    BDNode.Save(dataContext, microorganism);
                }
                else
                {
                    microorganism = tmpNode;
                    idxMicroorganism++;
                }
            }
        }

        /// <summary>
        /// Microbiology - commensal and pathogenic
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter6bInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Microbiology;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType microorganismGroupLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType microorganismLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;
            microorganismGroupData = string.Empty;
            microorganismData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];
            if (elements.Length > 5) microorganismGroupData = elements[5];
            if (elements.Length > 6) microorganismData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;
                microorganismGroup = null;
                microorganism = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
                idxMicroorganismGroup = 0;
                idxMicroorganism = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                microorganismGroup = null;
                microorganism = null;

                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
                idxMicroorganismGroup = 0;
                idxMicroorganism = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                microorganismGroup = null;
                microorganism = null;

                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxMicroorganismGroup = 0;
                idxMicroorganism = 0;
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }
                microorganismGroup = null;
                microorganism = null;
                idxMicroorganismGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, subcategory);
                idxMicroorganism = 0;
            }
            if ((null != microorganismGroupData && microorganismGroupData != string.Empty) && ((null == microorganismGroup) || (microorganismGroup.name != microorganismGroupData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    microorganismGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDMicroorganismGroup, Guid.Parse(uuidData));
                    microorganismGroup.name = microorganismGroupData;
                    microorganismGroup.SetParent(subcategory);
                    microorganismGroup.displayOrder = idxMicroorganismGroup++;
                    microorganismGroup.LayoutVariant = microorganismGroupLayoutVariant;
                    BDNode.Save(dataContext, microorganismGroup);
                }
                else
                {
                    microorganismGroup = tmpNode;
                    idxMicroorganismGroup++;
                }
                microorganism = null;
                idxMicroorganism = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, microorganismGroup);

            }
            if ((null != microorganismData && microorganismData != string.Empty) && ((null == microorganism) || (microorganism.name != microorganismData)))
            {
                if (null == microorganismGroup)
                {
                    microorganismGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDMicroorganismGroup, Guid.NewGuid());
                    microorganismGroup.name = string.Empty;
                    microorganismGroup.SetParent(subcategory);
                    microorganismGroup.displayOrder = idxMicroorganismGroup++;
                    microorganismGroup.LayoutVariant = microorganismGroupLayoutVariant;
                    BDNode.Save(dataContext, microorganismGroup);
                }
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    microorganism = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDMicroorganism, Guid.Parse(uuidData));
                    microorganism.name = microorganismData;
                    microorganism.SetParent(microorganismGroup);
                    microorganism.displayOrder = idxMicroorganism++;
                    microorganism.LayoutVariant = microorganismLayoutVariant;
                    BDNode.Save(dataContext, microorganism);
                }
                else
                {
                    microorganism = tmpNode;
                    idxMicroorganism++;
                }
            }
        }

        /// <summary>
        /// Microbiology - Empiric Therapy
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter6cInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Microbiology;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Microbiology_EmpiricTherapy;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType microorganismGroupLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType microorganismLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            microorganismGroupData = string.Empty;
            microorganismData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) microorganismGroupData = elements[4];
            if (elements.Length > 5) microorganismData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                microorganismGroup = null;
                microorganism = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxMicroorganismGroup = 0;
                idxMicroorganism = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                microorganismGroup = null;
                microorganism = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxMicroorganismGroup = 0;
                idxMicroorganism = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                microorganismGroup = null;
                microorganism = null;

                idxMicroorganismGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxMicroorganism = 0;
            }

            if ((null != microorganismGroupData && microorganismGroupData != string.Empty) && ((null == microorganismGroup) || (microorganismGroup.name != microorganismGroupData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    microorganismGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDMicroorganismGroup, Guid.Parse(uuidData));
                    microorganismGroup.name = (microorganismGroupData == @"<blank>") ? string.Empty : microorganismGroupData;
                    microorganismGroup.SetParent(category);
                    microorganismGroup.displayOrder = idxMicroorganismGroup++;
                    microorganismGroup.LayoutVariant = microorganismGroupLayoutVariant;
                    BDNode.Save(dataContext, microorganismGroup);
                }
                else
                {
                    microorganismGroup = tmpNode;
                    idxMicroorganismGroup++;
                }
                microorganism = null;
                idxMicroorganism = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, microorganismGroup);

            }
            if ((null != microorganismData && microorganismData != string.Empty) && ((null == microorganism) || (microorganism.name != microorganismData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    microorganism = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDMicroorganism, Guid.Parse(uuidData));
                    microorganism.name = microorganismData;
                    microorganism.SetParent(microorganismGroup);
                    microorganism.displayOrder = idxMicroorganism++;
                    microorganism.LayoutVariant = microorganismLayoutVariant;
                    BDNode.Save(dataContext, microorganism);
                }
                else
                {
                    microorganism = tmpNode;
                    idxMicroorganism++;
                }
            }
        }

        /// <summary>
        /// Microbiology - Antibiogram
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter6dInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Microbiology;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Microbiology_Antibiogram;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
            }
        }

        /// <summary>
        /// Pre-op antibiotic administration
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3aInputLine(string pInputLine) {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_PreOp;
            BDConstants.LayoutVariantType tableLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_PreOp;
            BDConstants.LayoutVariantType tableSectionLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType headerRowLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_PreOp_HeaderRow;
            BDConstants.LayoutVariantType contentRowLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_PreOp_ContentRow;

            uuidData = string.Empty;
            chapterData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;
            tableSectionData = string.Empty;
            tableRowData = string.Empty;
            tableCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) tableData = elements[2];
            if (elements.Length > 3) tableHeaderRowData = elements[3];
            if (elements.Length > 4) tableHeaderCellData = elements[4];
            if (elements.Length > 5) tableSectionData = elements[5];
            if (elements.Length > 6) tableRowData = elements[6];
            if (elements.Length > 7) tableCellData = elements[7];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;

                idxSection = 0;
                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {
                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    // this seed data was created without the section layer of the hierarchy
                    if (null == section)
                    {
                        section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
                        section.name = tableData;
                        section.SetParent(chapter);
                        section.displayOrder = idxSection++;
                        section.LayoutVariant = sectionLayoutVariant;
                        BDNode.Save(dataContext, section);
                    }
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(section);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, table);
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;

                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"(Header)") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                    BDTableRow.Save(dataContext, tableHeaderRow);
                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;
                tableSection = null;
                tableRow = null;
                tableCell = null;

                idxTableHeaderCell = 0;

            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                        tableHeaderRow.SetParent(table);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
                tableSection = null;
            }

            if ((null != tableSectionData && tableSectionData != string.Empty) && ((null == tableSection) || (tableSection.name != tableSectionData)))
            {
                BDNode tmpRow = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableSection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTableSection, Guid.Parse(uuidData));

                    tableSection.name = (tableSectionData == @"<blank>") ? string.Empty : tableSectionData;
                    tableSection.SetParent(table);
                    tableSection.displayOrder = idxTableChildren++;
                    tableSection.LayoutVariant = tableSectionLayoutVariant;
                    BDNode.Save(dataContext, tableSection);

                    // do not reset table row counter - otherwise child rows will not sort correctly with header row
                    idxTableCell = 0;
                }
                else
                {
                    tableSection = tmpRow;
                    idxTableChildren++;
                }
            }

            if ((null != tableRowData && tableRowData != string.Empty) && ((null == tableRow) || (tableRow.name != tableRowData)))
            {
                // ignore the seed data - will create a row below.
                //BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                //if (null == tmpRow)
                //{
                //    tableRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                //    tableRow.name = string.Empty;
                //    tableRow.SetParent(tableSection);
                //    tableRow.displayOrder = idxTableChildren++;
                //    tableRow.LayoutVariant = contentRowLayoutVariant;
                //    BDTableRow.Save(dataContext, tableRow);
                //}
                //else
                //{
                //    tableRow = tmpRow;
                //    idxTableChildren++;
                //}

            }

            if ((null != tableCellData && tableCellData != string.Empty) && ((null == tableCell) || tableCell.Uuid.ToString() != uuidData))
            {
                tableCell = null;
                idxTableCell = 0;

                tableRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.NewGuid());

                tableRow.name = string.Empty;
                tableRow.SetParent(tableSection);
                tableRow.displayOrder = idxTableChildren++;
                tableRow.LayoutVariant = contentRowLayoutVariant;
                BDTableRow.Save(dataContext, tableRow);

                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    //BDFabrik.GetTableColumnCount() defines the number of cells for the layout variant
                    tableCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableCell.displayOrder = idxTableCell++;
                    tableCell.SetParent(tableRow);
                    tableCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                    BDTableCell.Save(dataContext, tableCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableCell.Uuid);
                    cellValue.value = tableCellData;
                    BDString.Save(dataContext, cellValue);

                    int columnCount = BDFabrik.GetTableColumnCount(contentRowLayoutVariant);
                    for (int i = 1; i < columnCount; i++)
                    {
                        BDTableCell idxCell = BDTableCell.CreateBDTableCell(dataContext);
                        idxCell.displayOrder = idxTableCell++;
                        idxCell.SetParent(tableRow);
                        idxCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                        BDTableCell.Save(dataContext, idxCell);

                        BDString idxCellValue = BDString.CreateBDString(dataContext);
                        idxCellValue.displayOrder = 0;
                        idxCellValue.SetParent(idxCell.Uuid);
                        idxCellValue.value = string.Empty;
                        BDString.Save(dataContext, idxCellValue);
                    }
                }
            }
        }

        /// <summary>
        /// surgical prophylaxis
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3bInputLine(string pInputLine) {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType categoryLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            BDConstants.LayoutVariantType surgeryClassificationLayout = categoryLayoutVariant;
            BDConstants.LayoutVariantType surgeryGroupLayout = categoryLayoutVariant;
            BDConstants.LayoutVariantType surgeryLayout = categoryLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            surgeryClassificationData = string.Empty;
            surgeryGroupData = string.Empty;
            surgeryData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) surgeryClassificationData = elements[4];
            if (elements.Length > 5) surgeryGroupData = elements[5];
            if (elements.Length > 6) surgeryData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                surgeryClassification = null;
                surgeryGroup = null;
                surgery = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSurgeryClassification = 0;
                idxSurgeryGroup = 0;
                idxSurgery = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                surgeryClassification = null;
                surgeryGroup = null;
                surgery = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSurgeryClassification = 0;
                idxSurgeryGroup = 0;
                idxSurgery = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                surgeryClassification = null;
                surgeryGroup = null;
                surgery = null;
                idxSurgeryClassification = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
                idxSurgeryGroup = 0;
                idxSurgery = 0;
            }

            if ((null != surgeryClassificationData && surgeryClassificationData != string.Empty) && ((null == surgeryClassification) || (surgeryClassification.name != surgeryClassificationData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    surgeryClassification = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSurgeryClassification, Guid.Parse(uuidData));
                    surgeryClassification.name = (surgeryClassificationData == @"<blank>") ? string.Empty : surgeryClassificationData;
                    surgeryClassification.SetParent(category);
                    surgeryClassification.displayOrder = idxSurgeryClassification++;
                    surgeryClassification.LayoutVariant = surgeryClassificationLayout;
                    BDNode.Save(dataContext, surgeryClassification);
                }
                else
                {
                    surgeryClassification = tmpNode;
                    idxSurgeryClassification++;
                }
                surgeryGroup = null;
                surgery = null;
                idxSurgeryGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, surgeryClassification);
                idxSurgery = 0;
            }
            if ((null != surgeryGroupData && surgeryGroupData != string.Empty) && ((null == surgeryGroup) || (surgeryGroup.name != surgeryGroupData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    surgeryGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSurgeryGroup, Guid.Parse(uuidData));
                    surgeryGroup.name = (surgeryGroupData == @"<blank>") ? string.Empty : surgeryGroupData;
                    surgeryGroup.SetParent(surgeryClassification);
                    surgeryGroup.displayOrder = idxSurgeryGroup++;
                    surgeryGroup.LayoutVariant = surgeryGroupLayout;
                    BDNode.Save(dataContext, surgeryGroup);
                }
                else
                {
                    surgeryGroup = tmpNode;
                    idxSurgeryGroup++;
                }
                surgery = null;
                idxSurgery = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, surgeryGroup);
            }
            if ((null != surgeryData && surgeryData != string.Empty) && ((null == surgery) || (surgery.name != surgeryData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    surgery = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSurgery, Guid.Parse(uuidData));
                    surgery.name = (surgeryData == @"<blank>") ? string.Empty : surgeryData;
                    surgery.SetParent(surgeryGroup);
                    surgery.displayOrder = idxSurgery++;
                    surgery.LayoutVariant = surgeryLayout;
                    BDNode.Save(dataContext, surgery);
                }
                else
                {
                    surgery = tmpNode;
                    idxSurgery++;
                }
            }
        }

        /// <summary>
        /// Prophyaxis Infective Endocarditis
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3cInputLine(string pInputLine) {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType categoryLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_IERecommendation;
            BDConstants.LayoutVariantType subcategoryLayoutVariant = categoryLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            subcategoryData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) subcategoryData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                category = null;
                subcategory = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxCategory = 0;
                idxSubcategory = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                subcategory = null;
                idxCategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxSubcategory = 0;
            }
            if ((null != categoryData && categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = (categoryData == @"<blank>") ? string.Empty : categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = catNode;
                    idxCategory++;
                }
                subcategory = null;
                idxSubcategory = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, category);
            }

            if ((null != subcategoryData && subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                BDNode catNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == catNode)
                {
                    subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                    subcategory.name = (subcategoryData == @"<blank>") ? string.Empty : subcategoryData;
                    subcategory.SetParent(category);
                    subcategory.displayOrder = idxSubcategory++;
                    subcategory.LayoutVariant = subcategoryLayoutVariant;
                    BDNode.Save(dataContext, subcategory);
                }
                else
                {
                    subcategory = catNode;
                    idxSubcategory++;
                }
            }
        }

        /// <summary>
        /// Prophylaxis Infective Endocarditis
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3dInputLine(string pInputLine) {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage;
            BDConstants.LayoutVariantType therapyGroupLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage;
            BDConstants.LayoutVariantType therapyLayoutVariant = therapyGroupLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            therapyGroupData = string.Empty;
            therapyData = string.Empty;

            bool firstTherapyGroup = true;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) therapyGroupData = elements[3];
            if (elements.Length > 4) therapyData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                therapyGroup = null;
                therapy = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTherapyGroup = 0;
                idxTherapy = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                therapyGroup = null;
                therapy = null;
                idxTherapy = 0;
                firstTherapyGroup = true;
            }
            if ((null != therapyGroupData && therapyGroupData != string.Empty) && ((null == therapyGroup) || (therapyGroup.name != therapyGroupData)))
            {
                if (therapyGroupData == "Standard")
                {
                    idxTherapyGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                    idxTherapyGroup++;
                }
                BDTherapyGroup tmpNode = BDTherapyGroup.RetrieveTherapyGroupWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    therapyGroup = BDTherapyGroup.CreateBDTherapyGroup(dataContext, Guid.Parse(uuidData));
                    therapyGroup.name = therapyGroupData;
                    therapyGroup.SetParent(section);
                    therapyGroup.displayOrder = idxTherapyGroup++;
                    therapyGroup.LayoutVariant = therapyGroupLayoutVariant;
                    BDTherapyGroup.Save(dataContext, therapyGroup);
                }
                else
                {
                    therapyGroup = tmpNode;
                    idxTherapyGroup++;
                }
                firstTherapyGroup = false;
                therapy = null;
                idxTherapy = 0;
            }
            if ((null != therapyData && therapyData != string.Empty) && ((null == therapy) || (therapy.name != therapyData)))
            {
                BDTherapy tmpNode = BDTherapy.RetrieveTherapyWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    therapy = BDTherapy.CreateBDTherapy(dataContext, Guid.Parse(uuidData));
                    therapy.name = (therapyData == @"<blank>") ? string.Empty : therapyData;
                    therapy.SetParent(therapyGroup);
                    therapy.displayOrder = idxTherapy++;
                    therapy.LayoutVariant = therapyLayoutVariant;
                    BDTherapy.Save(dataContext, therapy);
                }
                else
                {
                    therapy = tmpNode;
                    idxTherapy++;
                }
            }
        }

        /// <summary>
        /// Blood / Body fluid exposure
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3eInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_FluidExposure;
            BDConstants.LayoutVariantType tableLayoutVariant = BDConstants.LayoutVariantType.Undefined;
            BDConstants.LayoutVariantType headerRowLayoutVariant = BDConstants.LayoutVariantType.Undefined;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;
            tableSectionData = string.Empty;
            tableRowData = string.Empty;
            tableCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) tableData = elements[3];
            if (elements.Length > 4) tableHeaderRowData = elements[4];
            if (elements.Length > 5) tableHeaderCellData = elements[5];
            if (elements.Length > 6) tableSectionData = elements[6];

            if (tableData != null && tableData != string.Empty)
            {
                if (uuidData == "2d36bf1c-a4d8-4d00-916e-cb9a29013361" || uuidData == "a2e97873-bdb0-430b-a379-c9f136b8f968")
                {
                    tableLayoutVariant = BDConstants.LayoutVariantType.Table_3_Column;
                    headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
                }
                if (uuidData == "7c06671f-9ec7-4d20-8a2e-2804001f2156" || uuidData == "4cab3c94-09e0-440d-a39f-71ceb08c1b04")
                {
                    tableLayoutVariant = BDConstants.LayoutVariantType.Table_2_Column;
                    headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_2_Column_HeaderRow;
                }
                if (uuidData == "4017bc6d-05fd-45d9-9119-3ab9062a71a5")
                {
                    tableLayoutVariant = BDConstants.LayoutVariantType.Table_5_Column;
                    headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_5_Column_HeaderRow;
                }
            }

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {


                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(section);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, table);

                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                if (table.LayoutVariant == BDConstants.LayoutVariantType.Table_3_Column)
                    headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
                else if (table.LayoutVariant == BDConstants.LayoutVariantType.Table_2_Column)
                    headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_2_Column_HeaderRow;
                else if (table.LayoutVariant == BDConstants.LayoutVariantType.Table_5_Column)
                    headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_5_Column_HeaderRow;

                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"(Header)") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                    BDTableRow.Save(dataContext, tableHeaderRow);
                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;

                idxTableHeaderCell = 0;
            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        if (table.LayoutVariant == BDConstants.LayoutVariantType.Table_3_Column)
                            headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
                        else if (table.LayoutVariant == BDConstants.LayoutVariantType.Table_2_Column)
                            headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_2_Column_HeaderRow;
                        else if (table.LayoutVariant == BDConstants.LayoutVariantType.Table_5_Column)
                            headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_5_Column_HeaderRow;

                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                        tableHeaderRow.SetParent(table);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData == "<blank>" ? string.Empty : tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
            }
        }

        /// <summary>
        /// Sexual Assault
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3fInputLine(string pInputLine) {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_SexualAssault;
            BDConstants.LayoutVariantType topicLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType tableLayoutVariant = BDConstants.LayoutVariantType.Table_3_Column;
            BDConstants.LayoutVariantType headerRowLayoutVariant = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
            BDConstants.LayoutVariantType diseaseLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            topicData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;
            diseaseData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) topicData = elements[3];
            if (elements.Length > 4) tableData = elements[4];
            if (elements.Length > 5) tableHeaderRowData = elements[5];
            if (elements.Length > 6) tableHeaderCellData = elements[6];
            if (elements.Length > 7) diseaseData = elements[7];

            if (topicData != null && topicData != string.Empty)
            {
                if (uuidData == "edbd26cc-5224-490c-b12d-41676d1e8fb4")
                    topicLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_SexualAssault;
                else if (uuidData == "79f3014b-faf2-442d-9e2b-9afddfcb4e36")
                    topicLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis;
            }

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                topic = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTopic = 0;
                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                topic = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTopic = 0;
                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = topicData;
                    topic.SetParent(section);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = topicLayoutVariant;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {
                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(topic);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, table);

                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;
            }
            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {
                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(section);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, table);
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"(Header)") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                    BDTableRow.Save(dataContext, tableHeaderRow);
                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;

                idxTableHeaderCell = 0;

            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                        tableHeaderRow.SetParent(table);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
                disease = null;
                idxDisease = 0;
            }
            if ((null != diseaseData && diseaseData != string.Empty) && ((null == disease) || (disease.name != diseaseData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    disease = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                    disease.name = diseaseData;
                    disease.SetParent(topic);
                    disease.displayOrder = idxDisease++;
                    disease.LayoutVariant = topic.LayoutVariant;
                    BDNode.Save(dataContext, disease);
                }
                else
                {
                    disease = tmpNode;
                    idxDisease++;
                }
            }
        }

        /// <summary>
        /// Routine Immunization
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3gInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine;
            BDConstants.LayoutVariantType tableLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType headerRowLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine_HeaderRow;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            topicData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) topicData = elements[3];
            if (elements.Length > 4) tableData = elements[4];
            if (elements.Length > 5) tableHeaderRowData = elements[5];
            if (elements.Length > 6) tableHeaderCellData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                topic = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                idxTopic = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = topicData;
                    topic.SetParent(section);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTable = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, topic);
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {
                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(topic);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, table);

                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"(Header)") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                    BDTableRow.Save(dataContext, tableHeaderRow);
                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;

                idxTableHeaderCell = 0;
            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData == "<blank>" ? string.Empty : tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
            }
        }

        /// <summary>
        /// Vaccines for High-Risk patients
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3hInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Immunization;
            BDConstants.LayoutVariantType tableLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk;
            BDConstants.LayoutVariantType headerRowLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk_HeaderRow;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            topicData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) topicData = elements[3];
            if (elements.Length > 4) tableData = elements[4];
            if (elements.Length > 5) tableHeaderRowData = elements[5];
            if (elements.Length > 6) tableHeaderCellData = elements[6];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                tableSection = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                topic = null;
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;
                idxTopic = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxTable = 0;
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = topicData;
                    topic.SetParent(section);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
                table = null;
                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTable = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, topic);
                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableData && tableData != string.Empty) && ((null == table) || (table.name != tableData)))
            {


                BDNode tableNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tableNode)
                {
                    table = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(topic);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = tableLayoutVariant;
                    BDNode.Save(dataContext, table);

                    idxTableChildren = 0;
                    idxTableHeaderCell = 0;
                }
                else
                {
                    table = tableNode;
                    idxTable++;
                }

                tableHeaderRow = null;
                tableHeaderCell = null;

                idxTableChildren = 0;
                idxTableHeaderCell = 0;
            }

            if ((null != tableHeaderRowData && tableHeaderRowData != string.Empty) && ((null == tableHeaderRow) || (tableHeaderRow.name != tableHeaderRowData)))
            {
                BDTableRow tmpRow = BDTableRow.RetrieveTableRowWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpRow)
                {
                    tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    tableHeaderRow.name = (tableHeaderRowData == @"(Header)") ? string.Empty : tableHeaderRowData;
                    tableHeaderRow.SetParent(table);
                    tableHeaderRow.displayOrder = idxTableChildren++;
                    tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                    BDTableRow.Save(dataContext, tableHeaderRow);
                }
                else
                {
                    tableHeaderRow = tmpRow;
                    idxTableChildren++;
                }
                tableHeaderCell = null;

                idxTableHeaderCell = 0;
            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = headerRowLayoutVariant;
                        tableHeaderRow.SetParent(table);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow);
                    tableHeaderCell.alignment = (int)BDConstants.TableCellAlignment.Centred;

                    BDTableCell.Save(dataContext, tableHeaderCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableHeaderCell.Uuid);
                    cellValue.value = tableHeaderCellData == "<blank>" ? string.Empty : tableHeaderCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                {
                    tableHeaderCell = tmpCell;
                    idxTableHeaderCell++;
                }
            }
        }

        /// <summary>
        /// Immunization Recommendations
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3iInputLine(string pInputLine) {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail;
            BDConstants.LayoutVariantType topicLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            topicData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) topicData = elements[3];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);
                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }
                section = null;
                topic = null;

                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxTopic = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                topic = null;

                idxTopic = 0;
            }

            if ((topicData != string.Empty) && ((null == topic) || (topic.name != topicData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.Parse(uuidData));
                    topic.name = topicData;
                    topic.SetParent(section);
                    topic.displayOrder = idxTopic++;
                    topic.LayoutVariant = topicLayoutVariant;
                    BDNode.Save(dataContext, topic);
                }
                else
                {
                    topic = tmpNode;
                    idxTopic++;
                }
            }
        }

        /// <summary>
        /// Prophylaxis - Communicable Diseases
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3jInputLine(string pInputLine) {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Dental_RecommendedTherapy;
            BDConstants.LayoutVariantType categoryLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType diseaseLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType therapyGroupLayoutVariant = sectionLayoutVariant;

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            diseaseData = string.Empty;
            therapyGroupData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if (elements.Length > 4) diseaseData = elements[4];
            if (elements.Length > 5) therapyGroupData = elements[5];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);

                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }

                section = null;
                category = null;
                disease = null;
                therapyGroup = null;
                idxSection = 0;
                idxCategory = 0;
                idxDisease = 0;
                idxTherapyGroup = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                category = null;
                disease = null;
                therapyGroup = null;
                idxCategory = 0;
                idxDisease = 0;
                idxTherapyGroup = 0;
            }

            if ((categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                    category.name = categoryData;
                    category.SetParent(section);
                    category.displayOrder = idxCategory++;
                    category.LayoutVariant = categoryLayoutVariant;
                    BDNode.Save(dataContext, category);
                }
                else
                {
                    category = tmpNode;
                    idxCategory++;
                }
                disease = null;
                therapyGroup = null;
                idxDisease = 0;
                idxTherapyGroup = 0;
            }

            if ((diseaseData != string.Empty) && ((null == disease) || (disease.name != diseaseData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    disease = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDDisease, Guid.Parse(uuidData));
                    disease.name = diseaseData;
                    disease.SetParent(category);
                    disease.displayOrder = idxDisease++;
                    disease.LayoutVariant = diseaseLayoutVariant;
                    BDNode.Save(dataContext, disease);
                }
                else
                {
                    disease = tmpNode;
                    idxDisease++;
                }
                therapyGroup = null;
                idxTherapyGroup = 0;
            }

            if ((therapyGroupData != string.Empty) && ((null == therapyGroup) || (therapyGroup.name != therapyGroupData)))
            {
                BDTherapyGroup tmpNode = BDTherapyGroup.RetrieveTherapyGroupWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    therapyGroup = BDTherapyGroup.CreateBDTherapyGroup(dataContext, Guid.Parse(uuidData));
                    therapyGroup.name = therapyGroupData;
                    therapyGroup.SetParent(disease);
                    therapyGroup.displayOrder = idxTherapyGroup++;
                    therapyGroup.LayoutVariant = therapyGroupLayoutVariant;
                    BDTherapyGroup.Save(dataContext, therapyGroup);
                }
                else
                {
                    therapyGroup = tmpNode;
                    idxTherapyGroup++;
                }
            }

        }

        /// <summary>
        /// Prophylaxis - Influenza
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3kInputLine(string pInputLine) { }

        /// <summary>
        /// Infection Prevention and control
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter3lInputLine(string pInputLine) {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            BDConstants.LayoutVariantType chapterLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis;
            BDConstants.LayoutVariantType sectionLayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions;
            BDConstants.LayoutVariantType microorganismGroupLayoutVariant = sectionLayoutVariant;
            BDConstants.LayoutVariantType microorganismLayoutVariant = sectionLayoutVariant;

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            microorganismGroupData = string.Empty;
            microorganismData = string.Empty;

            //Expectation that a row contains only one element with data
            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) microorganismGroupData = elements[3];
            if (elements.Length > 4) microorganismData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                    chapter.name = chapterData;
                    chapter.displayOrder = idxChapter++;
                    chapter.LayoutVariant = chapterLayoutVariant;
                    chapter.SetParent(null);

                    BDNode.Save(dataContext, chapter);
                }
                else
                {
                    chapter = tmpNode;
                    idxChapter++;
                }

                section = null;
                microorganismGroup = null;
                microorganism = null;
                idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                idxMicroorganismGroup = 0;
                idxMicroorganism = 0;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                BDNode sectionNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == sectionNode)
                {
                    section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = sectionLayoutVariant;
                    BDNode.Save(dataContext, section);
                }
                else
                {
                    section = sectionNode;
                    idxSection++;
                }
                microorganismGroup = null;
                microorganism = null;
                idxMicroorganismGroup = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, section);
                idxMicroorganism = 0;
            }

            if ((microorganismGroupData != string.Empty) && ((null == microorganismGroup) || (microorganismGroup.uuid != Guid.Parse(uuidData))))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    microorganismGroup = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDMicroorganismGroup, Guid.Parse(uuidData));
                    microorganismGroup.name = microorganismGroupData;
                    microorganismGroup.SetParent(section);
                    microorganismGroup.displayOrder = idxMicroorganismGroup++;
                    microorganismGroup.LayoutVariant = microorganismGroupLayoutVariant;
                    BDNode.Save(dataContext, microorganismGroup);
                }
                else
                {
                    microorganismGroup = tmpNode;
                    idxMicroorganismGroup++;
                }
                microorganism = null;
                idxMicroorganism = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, microorganismGroup);

            }

            if ((microorganismData != string.Empty) && ((null == microorganism) || (microorganism.name != microorganismData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    microorganism = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDMicroorganism, Guid.Parse(uuidData));
                    microorganism.name = microorganismData;
                    microorganism.SetParent(microorganismGroup);
                    microorganism.displayOrder = idxMicroorganism++;
                    microorganism.LayoutVariant = microorganismLayoutVariant;
                    BDNode.Save(dataContext, microorganism);
                }
                else
                {
                    microorganism = tmpNode;
                    idxMicroorganism++;
                }
            }
        }
    }
}
