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
            chapter1a,
            chapter1b,
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
        BDTableRow tableRow;
        BDTableCell tableCell;

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
        string tableRowData = null;
        string tableCellData = null;

        short idxChapter = 0;
        short idxSection = 0;
        short idxCategory = 0;
        short idxSubcategory = 0;
        short idxDisease = 0;
        short idxPresentation = 0;
        short idxPathogenGroup = 0;
        short idxPathogen = 0;
        short idxTable = 0;
        short idxTableSection = 0;
        short idxTableRow = 0;
        short idxTableCell = 0;

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
                // retrieve chapter 2
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
                category = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
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
                pathogenGroup = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPathogenGroup, Guid.Parse(uuidData));
                pathogenGroup.name = pathogenGroupData;
                pathogenGroup.SetParent(category);
                pathogenGroup.displayOrder = idxPathogenGroup++;
                pathogenGroup.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I;
                BDNode.Save(dataContext, pathogenGroup);

                pathogen = null;
            }

            if ((null != pathogenData && pathogenData != string.Empty) && ((null == pathogen) || (pathogen.name != pathogenData)))
            {
                pathogen = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPathogen, Guid.Parse(uuidData));
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
                // retrieve chapter 2
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
                    category = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.Parse(uuidData));
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
                pathogen = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPathogen, Guid.Parse(uuidData));
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
            if (elements.Length > 4) tableSectionData = elements[4];

            if ((null != chapterData && chapterData != string.Empty) && ((null == chapter) || (chapter.name != chapterData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, chapter1Uuid);
                if (null == tmpNode)
                {
                    chapter = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
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
                    section = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
                    section.name = sectionData;
                    section.SetParent(chapter);
                    section.displayOrder = idxSection++;
                    section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
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
                    table = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = tableData;
                    table.SetParent(section);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
                    BDNode.Save(dataContext, table);
                }
                else
                    table = tableNode;

                tableSection = null;
                tableRow = null;
                tableCell = null;

            }

            if ((null != tableSectionData && tableSectionData != string.Empty) && ((null == tableSection) || (tableSection.name != tableSectionData)))
            {
                BDNode tmpNode = BDNode.RetrieveNodeWithId(dataContext, new Guid(uuidData));
                if (null == tmpNode)
                {
                    tableSection = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDTableSection, Guid.Parse(uuidData));
                    tableSection.name = tableSectionData;
                    tableSection.SetParent(table);
                    tableSection.displayOrder = idxTableSection++;
                    tableSection.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
                    BDNode.Save(dataContext, tableSection);

                    tableRow = null;
                    tableCell = null;
                }
            }
        }

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
                    chapter = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.Parse(uuidData));
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
                    section = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.Parse(uuidData));
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
                    table = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDTable, Guid.Parse(uuidData));
                    table.name = (tableData == @"<blank>") ? string.Empty : tableData;
                    table.SetParent(section);
                    table.displayOrder = idxTable++;
                    table.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics;
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
                    tableRow = BDTableRow.CreateTableRow(dataContext, BDConstants.BDNodeType.BDTableRow, Guid.Parse(uuidData));

                    string name = String.Empty;
                    BDConstants.LayoutVariantType rowType = BDConstants.LayoutVariantType.Undefined;
                    if (tableRowData == @"(Header row)")
                        rowType = BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics_HeaderRow;
                    else if (tableRowData == @"(Data row)")
                        rowType = BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics_ContentRow;
                    else
                        name = tableRowData;

                    tableRow.name = name;
                    tableRow.SetParent(table);
                    tableRow.displayOrder = idxTableRow++;
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
                    tableCell = BDTableCell.CreateTableCell(dataContext, Guid.Parse(uuidData));
                    tableCell.displayOrder = idxTableCell++;
                    tableCell.SetParent(tableRow.Uuid);
                    if (tableRow.layoutVariant == (int)BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics_HeaderRow)
                        tableCell.alignment = (int)BDConstants.TableCellAlignment.Centred;
                    else
                        tableCell.alignment = (int)BDConstants.TableCellAlignment.LeftJustified;

                    BDTableCell.Save(dataContext, tableCell);

                    BDString cellValue = BDString.CreateString(dataContext);
                    cellValue.displayOrder = 0;
                    cellValue.SetParent(tableCell.Uuid);
                    cellValue.value = tableCellData;
                    BDString.Save(dataContext, cellValue);
                }
                else
                    tableCell = tmpCell;
            }
        }
    }
}
