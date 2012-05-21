﻿using System;
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
            chapter1a,
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
            chapter2e
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

        private void ProcessChapter1aInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            antimicrobialData = string.Empty;
            topicData = string.Empty;

            idxTopic = 0;
            idxAntimicrobial = 0;
            idxSection = 0;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) antimicrobialData = elements[3];
            if (elements.Length > 4) topicData = elements[4];

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
                    //idxSection = (short)BDFabrik.GetChildrenForParent(dataContext, tmpNode).Count;
                    idxSection = (short)BDNode.RetrieveMaximumDisplayOrderForChildren(dataContext, chapter);
                }
                section = null;
                antimicrobial = null;
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
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
                    BDNode.Save(dataContext, section);
                }
                else
                    section = sectionNode; 

                antimicrobial = null;
                topic = null;

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
                    antimicrobial = tmpNode;

                topic = null;
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
        }
        
        /// <summary>
        /// Pharmacodynamics
        /// </summary>
        /// <param name="pInputLine"></param>
        private void ProcessChapter1bInputLine(string pInputLine)
        {
            string[] elements = pInputLine.Split(delimiters, StringSplitOptions.None);

            //Expectation that a row contains only one element with data

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            categoryData = string.Empty;
            pathogenData = string.Empty;
            tableData = string.Empty;
            tableSectionData = string.Empty;
            tableRowData = string.Empty;
            tableCellData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) tableData = elements[3];
            if (elements.Length > 4) tableRowData = elements[4];
            if (elements.Length > 5) tableCellData = elements[5];

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
                    chapter = tmpNode;
                section = null;
                table = null;
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
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics;
                    BDNode.Save(dataContext, section);
                }
                else
                    section = sectionNode; // assign Parasitic section
                table = null;
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
                }
                else
                    table = tableNode;

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

                    string name = String.Empty;
                    BDConstants.LayoutVariantType rowType = BDConstants.LayoutVariantType.Undefined;
                    if (tableRowData == @"(Header row)")
                        rowType = BDConstants.LayoutVariantType.Table_3_Column_HeaderRow;
                    else if (tableRowData == @"(Data row)")
                        rowType = BDConstants.LayoutVariantType.Table_3_Column_ContentRow;
                    else
                        name = tableRowData;

                    tableRow.name = name;
                    tableRow.SetParent(table);
                    tableRow.displayOrder = idxTableChildren++;
                    tableRow.LayoutVariant = rowType;
                    BDNode.Save(dataContext, table);
                }
                else
                    tableRow = tmpRow;

                tableCell = null;
            }

            if ((null != tableCellData && tableCellData != string.Empty) && ((null == tableCell) || tableCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    tableCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableCell.displayOrder = idxTableCell++;
                    tableCell.SetParent(tableRow.Uuid);
                    if (tableRow.layoutVariant == (int)BDConstants.LayoutVariantType.Table_3_Column_HeaderRow)
                        tableCell.alignment = (int)BDConstants.TableCellAlignment.Centred;
                    else
                        tableCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;

                    BDTableCell.Save(dataContext, tableCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableCell.Uuid);
                    cellValue.value = tableCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                    tableCell = tmpCell;
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
            diseaseData = string.Empty;
            presentationData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) categoryData = elements[3];
            if(elements.Length > 4) subcategoryData = elements[4];

            if ((chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                chapter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
                chapter.name = chapterData;
                chapter.displayOrder = idxChapter++;
                chapter.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics;
                chapter.SetParent(null);

                BDNode.Save(dataContext, chapter);

                section = null;
                category = null;
                subcategory = null;
            }

            if ((sectionData != string.Empty) && ((null == section) || (section.name != sectionData)))
            {
                section = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                section.name = sectionData;
                section.SetParent(chapter);
                section.displayOrder = idxSection++;
                section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts;
                BDNode.Save(dataContext, section);

                category = null;
                subcategory = null;

            }
            if ((categoryData != string.Empty) && ((null == category) || (category.name != categoryData)))
            {
                category = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
                category.name = categoryData;
                category.SetParent(section);
                category.displayOrder = idxCategory++;
                category.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts;
                BDNode.Save(dataContext, category);

                subcategory = null;
            }
            if ((subcategoryData != string.Empty) && ((null == subcategory) || (subcategory.name != subcategoryData)))
            {
                subcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory, Guid.Parse(uuidData));
                subcategory.name = subcategoryData;
                subcategory.SetParent(category);
                subcategory.displayOrder = idxSubcategory++;
                subcategory.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts;
                BDNode.Save(dataContext, subcategory);
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
            if (elements.Length > 5) dosageGroupData = elements[5];
            if (elements.Length > 6) dosageData = elements[6];

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
                    tableHeaderCell.SetParent(tableHeaderRow.Uuid);
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
                    tableCell.SetParent(tableRow.Uuid);
                    tableCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;

                    BDTableCell.Save(dataContext, tableCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableCell.Uuid);
                    cellValue.value = tableCellData;
                    BDString.Save(dataContext, cellValue);

                    BDTableCell cell2 = BDTableCell.CreateBDTableCell(dataContext);
                    cell2.displayOrder = idxTableCell++;
                    cell2.SetParent(tableRow.Uuid);
                    cell2.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                    BDTableCell.Save(dataContext, cell2);

                    BDString cell2Value = BDString.CreateBDString(dataContext);
                    cell2Value.displayOrder = 0;
                    cell2Value.SetParent(cell2.Uuid);
                    cell2Value.value = string.Empty;
                    BDString.Save(dataContext, cell2Value);

                    BDTableCell cell3 = BDTableCell.CreateBDTableCell(dataContext);
                    cell3.displayOrder = idxTableCell++;
                    cell3.SetParent(tableRow.Uuid);
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
                    tableHeaderCell.SetParent(tableHeaderRow.Uuid);
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

            uuidData = string.Empty;
            chapterData = string.Empty;
            sectionData = string.Empty;
            tableData = string.Empty;
            tableHeaderRowData = string.Empty;
            tableHeaderCellData = string.Empty;
            tableSectionData = string.Empty;

            if (elements.Length > 0) uuidData = elements[0];
            if (elements.Length > 1) chapterData = elements[1];
            if (elements.Length > 2) sectionData = elements[2];
            if (elements.Length > 3) tableData = elements[3];
            if (elements.Length > 4) tableHeaderRowData = elements[4];
            if (elements.Length > 5) tableHeaderCellData = elements[5];
            if (elements.Length > 6) tableSectionData = elements[6];

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
                tableSection = null;
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
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_CSFPenetration;
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
                tableSection = null;
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
                    table.LayoutVariant = BDConstants.LayoutVariantType.Table_4_Column;
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
                    tableHeaderRow.LayoutVariant = BDConstants.LayoutVariantType.Table_4_Column_HeaderRow;
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
            }

            if ((null != tableHeaderCellData && tableHeaderCellData != string.Empty) && ((null == tableHeaderCell) || tableHeaderCell.Uuid.ToString() != uuidData))
            {
                BDTableCell tmpCell = BDTableCell.RetrieveWithId(dataContext, Guid.Parse(uuidData));
                if (null == tmpCell)
                {
                    if (null == tableHeaderRow)
                    {
                        tableHeaderRow = BDTableRow.CreateBDTableRow(dataContext, BDConstants.BDNodeType.BDTableRow);
                        tableHeaderRow.LayoutVariant = BDConstants.LayoutVariantType.Table_4_Column_HeaderRow;
                        tableHeaderRow.SetParent(table);
                        tableHeaderRow.displayOrder = idxTableChildren++;
                        BDTableRow.Save(dataContext, tableHeaderRow);

                        idxTableCell = 0;
                    }

                    tableHeaderCell = BDTableCell.CreateBDTableCell(dataContext, Guid.Parse(uuidData));
                    tableHeaderCell.displayOrder = idxTableHeaderCell++;
                    tableHeaderCell.SetParent(tableHeaderRow.Uuid);
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
                    tableSection.LayoutVariant = BDConstants.LayoutVariantType.Table_4_Column;
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
                    tableHeaderCell.SetParent(tableHeaderRow.Uuid);
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
                    tableCell.SetParent(tableRow.Uuid);
                    tableCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;

                    BDTableCell.Save(dataContext, tableCell);

                    BDString cellValue = BDString.CreateBDString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableCell.Uuid);
                    cellValue.value = tableCellData;
                    BDString.Save(dataContext, cellValue);

                    BDTableCell cell2 = BDTableCell.CreateBDTableCell(dataContext);
                    cell2.displayOrder = idxTableCell++;
                    cell2.SetParent(tableRow.Uuid);
                    cell2.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;
                    BDTableCell.Save(dataContext, cell2);

                    BDString cell2Value = BDString.CreateBDString(dataContext);
                    cell2Value.displayOrder = 0;
                    cell2Value.SetParent(cell2.Uuid);
                    cell2Value.value = string.Empty;
                    BDString.Save(dataContext, cell2Value);

                    BDTableCell cell3 = BDTableCell.CreateBDTableCell(dataContext);
                    cell3.displayOrder = idxTableCell++;
                    cell3.SetParent(tableRow.Uuid);
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
    }
}
