﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net;

using System.Windows.Forms;  //hack
using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class BDHtmlPageGenerator
    {
        private enum OverrideType
        {
            Undefined = -1,
            Paediatric = 1,
            Adult = 2
        }

        private const bool sortData = false;
        private const int maxNodeType = 6;
        private const string topHtml = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd""><html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""en""><head><meta http-equiv=""content-type"" content=""text/html; charset=UTF-8""/><meta name=""viewport"" content=""width=device-width; initial-scale=1.0; maximum-scale=10.0;"" /><link rel=""stylesheet"" type=""text/css"" href=""ra_bd.base.css"" /><title>Bugs &amp; Drugs</title></head><body><div id=""ra_bd""><div class=""current"">";
        private const string bottomHtml = @"</div></div></body></html>";
        private const string navListDivPrefix = @"<div class=""scroll""><ul class=""rounded"">";
        private const string navListDivSuffix = @"</ul></div>";
        private const string navListAnchorTag = @"<li class=""arrow""><a class=""nav"" href=""{0}""><strong>{1}</strong></a></li>";
        private const string horizontalRuleTag = @"<hr />";
        public const int EMPTY_PARAGRAPH = 8;  // <p> </p>
        private const string imgFileTag = @"<img class=""{0}"" src=""images/{1}{2}"" alt="""" />";
        private const string paintChipTag = @"<img class=""paintChip"" src=""{0}"" alt="""" />";
        private const string PAINT_CHIP_ANTIBIOTICS = @"AntibioticYellow.png";
        private const string PAINT_CHIP_DENTISTRY = @"DentistryPurple.png";
        private const string PAINT_CHIP_ORGANISMS = @"OrganismGreen.png";
        private const string PAINT_CHIP_TREATMENT_PAEDIATRIC = @"PaediatricTreatmentLightBlue.png";
        private const string PAINT_CHIP_TREATMENT_ADULT = @"TreatmentBlue.png";
        private const string PAINT_CHIP_PREGNANCY = @"PregnancyRed.png";
        private const string PAINT_CHIP_PROPHYLAXIS = @"ProphylaxisOrange.png";
        private const string LEFT_SQUARE_BRACKET = @"&#91;";
        private const string RIGHT_SQUARE_BRACKET = @"&#93;";
        private const string TAG_SPAN_UNDERLINE = @"<span class=""underline"">";
        private const string TAG_SPAN_LEFTBRACKET = @"<span class=""leftBracket"">";
        private const string TAG_SPAN_RIGHTBRACKET = @"<span class=""rightBracket"">";
        private const string TAG_SPAN_END = @"</span>";
        private const string TAG_CELL_END = @"</td>";
        private const string CSS_INLINE_STYLE_INDENT_CELL = @"style=""padding-left:15px;""";

        private const string PUBLICATION_NOTES_UUID = @"a6d03c7e-a095-4c04-b0e7-ffe74bcfa8e6";
        private const string TREATMENT_RECOMMENDATION_PEDS_UUID = @"c0ecedc1-70cf-4422-b998-7e5f2bb986b1";
        private const string TREATMENT_RECOMMENDATION_ADULT_UUID = @"757409a4-9446-4aa5-ac23-03fb7660759b";
        private const string PROPHYLAXIS_IMMUNIZATION_SECTION_UUID = @"63a99294-dc8a-4ae3-be63-24b8eb7c578d";

        private IBDNode currentChapter = null;
        private IBDObject currentPageMasterObject = null;
        private int? postProcessingPageLayoutVariant = (int)BDConstants.LayoutVariantType.Undefined;

        // create variables to hold data for 'same as previous' settings on Therapy
        Guid previousTherapyNameId = Guid.Empty;
        Guid previousTherapyDosageId = Guid.Empty;
        Guid previousTherapyDosage1Id = Guid.Empty;
        Guid previousTherapyDosage2Id = Guid.Empty;
        Guid previousTherapyDurationId = Guid.Empty;
        Guid previousTherapyDuration1Id = Guid.Empty;
        Guid previousTherapyDuration2Id = Guid.Empty;

        bool therapiesHaveDosage = false;
        bool therapiesHaveDuration = false;

        private OverrideType overrideType = OverrideType.Undefined;

        //ks
        private List<BDHtmlPageGeneratorLogEntry> referencePageUuidList = new List<BDHtmlPageGeneratorLogEntry>();
        private List<Guid> ibdNodeWithSymbolUuidList = new List<Guid>();

        public void GenerateForDebug(Entities pContext, List<BDNode> pNodeList)
        {
            generatePages(pContext, pNodeList);

            List<BDHtmlPage> pages = BDHtmlPage.RetrieveAll(pContext);
            List<Guid> displayParentIds = BDHtmlPage.RetrieveAllDisplayParentIDs(pContext);
            List<Guid> pageIds = BDHtmlPage.RetrieveAllIds(pContext);
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditPageMap.txt", "----------------------");
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkToSelfLog.txt", "---------");
            BDHtmlPageGeneratorLogEntry.ClearFile("BDHTMLPageReview.txt");
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDHTMLPageReview.txt", "----------------------");

            Debug.WriteLine("Post-processing HTML pages");
            foreach (BDHtmlPage page in pages)
            {
                postProcessPage(pContext, page, pageIds);
            }

            #region Output logs

            //ks: Write out all the verbose logs
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Reference Pages
            BDHtmlPageGeneratorLogEntry.WriteToFile(this.referencePageUuidList, mydocpath, @"ReferencePageUuidList.txt");

            #endregion

            pages.Clear();
            displayParentIds.Clear();
            pageIds.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode">if the node is null,  all chapters are generated.  
        /// Otherwise only the passed node is generated with its children</param>
        public void Generate(Entities pContext, List<BDNode> pNodeList /* IBDNode pNode */)
        {
            // delete pages from the local store
            BDHtmlPage.DeleteAll(pContext);
            BDHtmlPageMap.DeleteAll(pContext);

            // reset index entries to false
            BDNodeToHtmlPageIndex.ResetForRegeneration(pContext);
            pContext.SaveChanges();

            generatePages(pContext, pNodeList);

            List<BDHtmlPage> pages = BDHtmlPage.RetrieveAll(pContext);
            List<Guid> displayParentIds = BDHtmlPage.RetrieveAllDisplayParentIDs(pContext);
            List<Guid> pageIds = BDHtmlPage.RetrieveAllIds(pContext);
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditPageMap.txt", "----------------------");
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkToSelfLog.txt", "---------");
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDPageChangeLog.txt", string.Format("{0} ------------", BDSystemSetting.RetrieveSettingValue(pContext, BDSystemSetting.CONTROL_NUMBER)));
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDHTMLSymbolChangeLog.txt",  string.Format("Start: {0} -------------------------------", DateTime.Now));
 
            // insert this line into the code where changes need to be tracked for review:
            // BDHtmlPageGeneratorLogEntry.AppendToFile("BDPageChangeLog.txt", string.Format("{0} {1}","Page UUID", "Page name"));

            Debug.WriteLine("Post-processing HTML pages");
            foreach (BDHtmlPage page in pages)
            {
                postProcessPage(pContext, page, pageIds);
            }

            #region Output logs

            //ks: Write out all the verbose logs
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Reference Pages
            BDHtmlPageGeneratorLogEntry.WriteToFile(this.referencePageUuidList, mydocpath, @"BDReferencePageUuidList.txt");

            // log HTML symbol changes
            foreach (Guid nodeId in ibdNodeWithSymbolUuidList)
            {
                IBDNode node = BDFabrik.RetrieveNode(pContext, nodeId);

                if (null != node)
                {
                    string htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForOriginalIBDNodeId(pContext, nodeId).ToString();
                    string message = string.Format("HTML page {0}: Symbol found in {1} {2}", htmlPageId, node.NodeType, node.Uuid);
                    BDHtmlPageGeneratorLogEntry.AppendToFile("BDHTMLSymbolChangeLog.txt", message);
                }
            }
            #endregion

            ibdNodeWithSymbolUuidList.Clear();
            pages.Clear();
            displayParentIds.Clear();
            pageIds.Clear();
        }

        private void generatePages(Entities pContext, List<BDNode> pNodeList /* IBDNode pNode */)
        {
            //List<BDNode> chapters = BDNode.RetrieveNodesForType(pContext, BDConstants.BDNodeType.BDChapter);
            List<BDHtmlPage> allPages = new List<BDHtmlPage>();

            List<BDNode> chapters = pNodeList ?? BDNode.RetrieveNodesForType(pContext, BDConstants.BDNodeType.BDChapter);

            List<BDHtmlPage> childDetailPages = new List<BDHtmlPage>();
            List<BDHtmlPage> childNavPages = new List<BDHtmlPage>();

            // remove the info chapter  from the collection so that there is no link to it on the main page
            chapters.Remove(BDNode.RetrieveNodeWithId(pContext, Guid.Parse(PUBLICATION_NOTES_UUID)));

            foreach (BDNode chapter in chapters)
            {
                currentChapter = chapter;
                generateOverviewAndChildrenForNode(pContext, chapter, childDetailPages, childNavPages);
            }
            if (childDetailPages.Count > 0)
                allPages.AddRange(childDetailPages);
            if (childNavPages.Count > 0)
                allPages.AddRange(childNavPages);

            List<BDHtmlPage> chapterPages = allPages.Distinct().ToList();

            Debug.WriteLine("Creating home page with filtered distinct list");
            if (chapterPages.Count > 0)
                generateNavigationPage(pContext, null, chapterPages);

            // generate info page
            Debug.WriteLine("Creating info pages");
            BDNode infoNode = BDNode.RetrieveNodeWithId(pContext, Guid.Parse("1400e49c-8e18-4571-aba5-b792ab9332f7"));
            List<BDHtmlPage> infoChildPages = new List<BDHtmlPage>();
            currentChapter = infoNode;
            generateOverviewAndChildrenForNode(pContext, infoNode, infoChildPages, infoChildPages);

            allPages.Clear();
            chapters.Clear();
            childDetailPages.Clear();
            childNavPages.Clear();
            chapterPages.Clear();
            infoChildPages.Clear();
        }

        /// <summary>
        /// Recursive method that traverses the navigation tree
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        private void generateOverviewAndChildrenForNode(Entities pContext, IBDNode pNode, List<BDHtmlPage> pNodeDetailPages, List<BDHtmlPage> pNodeNavPages)
        {
            // bypass incomplete sections 
            if (pNode.Uuid == Guid.Parse(PROPHYLAXIS_IMMUNIZATION_SECTION_UUID)) return;

            // hack to compensate for lack of differentiation in early layout variants
            if (pNode.Uuid == Guid.Parse(TREATMENT_RECOMMENDATION_PEDS_UUID))
            {
                overrideType = OverrideType.Paediatric;
            }
            else if (pNode.Uuid == Guid.Parse(TREATMENT_RECOMMENDATION_ADULT_UUID))
            {
                overrideType = OverrideType.Adult;
            }

            /*
            // Debug
            if ( (pNode.Uuid == Guid.Parse("01933c56-dedf-4c1a-9191-d541819000d8")) || (pNode.ParentId == Guid.Parse("01933c56-dedf-4c1a-9191-d541819000d8")) )
            {
                Debug.WriteLine("Watch node:{0}", pNode.Name);
            }
            */
            // pNodeDetailPages gathers the detail HTML pages generated by beginDetailPage
            // must be passed in to this method to be outside the loop & gather them all

            //All the "goodness" of building an individual page occurs here. (Here be dragons)
            if (!beginDetailPage(pContext, pNode, pNodeDetailPages))
            {
                List<BDHtmlPage> childNavPages = new List<BDHtmlPage>();
                List<BDHtmlPage> childDetailPages = new List<BDHtmlPage>();
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode child in children)
                {
                    generateOverviewAndChildrenForNode(pContext, child, childDetailPages, childNavPages);
                }
                // we are NOT on a leaf node, still on a navigation level
                // generate page for 'n' level, with list of navigation children that was returned
                if (childDetailPages.Count > 0)
                {
                    pNodeDetailPages.Add(generateNavigationPage(pContext, pNode, childDetailPages));
                }
                childNavPages.Clear();
                childDetailPages.Clear();
                children.Clear();
            }
        }

        private BDHtmlPage generateNavigationPage(Entities pContext, IBDNode pNode, List<BDHtmlPage> pChildPages)
        {
            currentPageMasterObject = pNode;
            bool logForReview = false;
            StringBuilder pageHTML = new StringBuilder();
            List<Guid> objectsOnPage = new List<Guid>();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            if (pNode != null)
            {
                if (sortData) pChildPages.Sort();

                pageHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(2), footnotesOnPage, objectsOnPage));
                pageHTML.Append(@"<div class=""scroll""><ul class=""rounded"">");
                // TODO:  build javascript blocks to expand/collapse overview
                foreach (BDHtmlPage page in pChildPages)
                {
                    if (page != null)
                    {
                        IBDNode childNode = BDFabrik.RetrieveNode(pContext, page.displayParentId.Value);
                        if (childNode != null)
                        {
                            string pageName = page.Name;
                            if (BDUtilities.HasSymbols(page.Name))
                            {
                                pageName = BDUtilities.ProcessTextForSymbols(pContext, page.Name);
                                logForReview = true;
                            }
                            pageHTML.AppendFormat(navListAnchorTag, page.Uuid.ToString().ToUpper(), pageName);
                        }
                    }
                }
                pageHTML.Append(@"</ul></div>");

                StringBuilder endNotesHTML = new StringBuilder();
                List<Guid> noteAssociations;
                List<BDLinkedNote> endnotes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Endnote, out noteAssociations);
                if (endnotes.Count > 0)
                    pageHTML.Append(buildTextFromNotes(endnotes));

                pageHTML.Append(BuildBDLegendHtml(pContext, pNode, objectsOnPage));
                
                if (pNode.NodeType == BDConstants.BDNodeType.BDChapter)
                    Debug.WriteLine("{1} Chapter generation complete", pNode.Name);
            }
            else  // this is the main page of the app
            {
                currentChapter = null;
                pageHTML.Append(@"<div class=""toolbar""><h1>Bugs &amp; Drugs</h1></div>");
                pageHTML.Append(@"<div class=""scroll""><ul id=""home"" class=""rounded"">");
                foreach (BDHtmlPage childPage in pChildPages)
                {
                    if (childPage != null)
                    {
                        BDNode childNode = BDNode.RetrieveNodeWithId(pContext, childPage.displayParentId.Value);
                        if (childNode != null)
                        {
                            string paintChipFileName = string.Empty;
                            switch (childNode.LayoutVariant)
                            {
                                case BDConstants.LayoutVariantType.Antibiotics:
                                    paintChipFileName = PAINT_CHIP_ANTIBIOTICS;
                                    break;
                                case BDConstants.LayoutVariantType.Dental:
                                    paintChipFileName = PAINT_CHIP_DENTISTRY;
                                    break;
                                case BDConstants.LayoutVariantType.Organisms:
                                    paintChipFileName = PAINT_CHIP_ORGANISMS;
                                    break;
                                case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                                    paintChipFileName = PAINT_CHIP_TREATMENT_ADULT;
                                    break;
                                case BDConstants.LayoutVariantType.PregancyLactation:
                                    paintChipFileName = PAINT_CHIP_PREGNANCY;
                                    break;
                                case BDConstants.LayoutVariantType.Prophylaxis:
                                    paintChipFileName = PAINT_CHIP_PROPHYLAXIS;
                                    break;
                                default:
                                    paintChipFileName = string.Empty;
                                    break;
                            }
                            string paintChipHtml = string.Format(paintChipTag, paintChipFileName);

                            pageHTML.AppendFormat(@"<li class=""arrow""><a class=""nav"" href=""{0}"">{1}{2}</a></li>", childPage.Uuid.ToString().ToUpper(), paintChipHtml, childPage.Name);
                        }
                    }
                }
                pageHTML.Append(@"</ul></div>");
            }

            BDHtmlPage navPage = writeBDHtmlPage(pContext, pNode as BDNode, pageHTML, BDConstants.BDHtmlPageType.Navigation, footnotesOnPage, objectsOnPage, null);

            // create a page map entry for search - only for the topmost node on this page.
            if (pNode != null)
            {
                BDHtmlPageMap nodePageMap = BDHtmlPageMap.CreateBDHtmlPageMap(pContext, navPage.Uuid, pNode.Uuid);
            }
            return navPage;
        }

        /// <summary>
        /// Check the node type and the layout variant to determine which page needs to be built.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns>Boolean to indicate that page is generated and thus to stop recursing through the node tree</returns>
        private bool beginDetailPage(Entities pContext, IBDNode pNode, List<BDHtmlPage> nodeChildPages)
        {
            bool isPageGenerated = false;

            generatePageForParentAndPropertyReferences(pContext, BDNode.PROPERTYNAME_NAME, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDAntimicrobialGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    isPageGenerated = true;
                    currentPageMasterObject = pNode;
                    nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));

                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsDosingInRenalImpairment(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticDosingInHepaticImpairment(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsCSFPenetration(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForDentalMicroorganisms(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForPLAntimicrobialsInPregnancy(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForPLAntimicrobialsInLactation(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForPLPreventionPerinatalInfection(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForPLPerinatalHIVProtocol(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_IE:
                        case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                            //case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForOrganismGramStain(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForMicrobiologyOrganisms(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDChapter:
                    {
                        switch (pNode.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.References:
                                currentPageMasterObject = pNode;
                                nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                                isPageGenerated = true;
                                break;
                            default:
                                isPageGenerated = false;
                                break;
                        }
                    }
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    {
                        switch (pNode.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                                // if this disease has only one presentation, generate the HTML page at this level 
                                // -> Disease and Presentation will be shown on the same HTML page
                                BDNode node = pNode as BDNode;
                                int childCount = BDNode.RetrieveChildCountForNode(pContext, node).Value;
                                if (node != null && childCount == 1)
                                {
                                    isPageGenerated = true;
                                    currentPageMasterObject = pNode;
                                    nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                                }
                                else
                                    isPageGenerated = false;
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                                currentPageMasterObject = pNode;
                                nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                                isPageGenerated = true;
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                                currentPageMasterObject = pNode;
                                nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                                isPageGenerated = true;
                                break;
                            default:
                                isPageGenerated = false;
                                break;
                        }
                    }
                    break;
                case BDConstants.BDNodeType.BDMicroorganismGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogen:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add((GenerateBDHtmlPage(pContext, pNode)));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogenGroup:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                /* DONE */
                case BDConstants.BDNodeType.BDPresentation:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            // if the processing comes through here, then the Disease has > 1 Presentation 
                            isPageGenerated = true;
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyPresentation(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfCellulitisInExtremities(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfVesicularLesions(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDResponse: //ks: BDFabrik doesn't show this to create a page
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            currentPageMasterObject = pNode;
                            // ks: NB
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfVesicularLesions(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSection:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                        case BDConstants.LayoutVariantType.References:
                        case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubcategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubsection:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.FrontMatter:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubtopic:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgery:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgeryClassification:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgeryGroup:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTable:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading:
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTopic:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsClinicalGuidelinesSpectrum(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_CNS_Meningitis_Table:
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Topic:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;

                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDMicroorganism:
                case BDConstants.BDNodeType.BDTableCell:
                case BDConstants.BDNodeType.BDTableRow:
                case BDConstants.BDNodeType.BDTherapyGroup:
                default:
                    isPageGenerated = false;
                    break;
            }
            return isPageGenerated;
        }

        public BDHtmlPage GenerateBDHtmlPage(Entities pContext, IBDNode pNode)
        {
            if (null == pNode) return null;

            // bypass specific sections of the data - sections not yet published
            if (pNode.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Immunization ||
                pNode.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk ||
                pNode.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine ||
                pNode.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail ||
                pNode.LayoutVariant == BDConstants.LayoutVariantType.Organisms_Antibiogram)
                return null;

            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();
            BDConstants.BDHtmlPageType htmlPageType = BDConstants.BDHtmlPageType.Data;

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    bodyHTML.Append(BuildBDAntimicrobialHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDAttachment:
                    bodyHTML.Append(BuildBDAttachmentHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDCategory:
                    bodyHTML.Append(BuildBDCategoryHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    // for the noted layout variants, the page being built is navigation due to empty layers in the hierarchy
                    if (pNode.LayoutVariant == BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation || pNode.LayoutVariant == BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy)
                        htmlPageType = BDConstants.BDHtmlPageType.Navigation;
                    break;
                case BDConstants.BDNodeType.BDChapter:
                    bodyHTML.Append(BuildBDChapterHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDConfiguredEntry:
                    bodyHTML.Append(BuildBDConfiguredEntryHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    bodyHTML.Append(BuildBDDiseaseHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDMicroorganismGroup:
                    bodyHTML.Append(BuildBDMicroorganismGroupHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDOrganism:
                    bodyHTML.Append(BuildBDOrganismHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDPathogen:
                    bodyHTML.Append(BuildBDPathogenHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDPathogenGroup:
                    bodyHTML.Append(BuildBDPathogenGroupHtml(pContext, pNode, footnotes, objectsOnPage, 1, true));
                    break;
                case BDConstants.BDNodeType.BDPresentation:
                    bodyHTML.Append(BuildBDPresentationHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDResponse: //ks: BDFabrik doesn't show this nodeType to create a page...
                    bodyHTML.Append(BuildBDResponseHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSection:
                    bodyHTML.Append(BuildBDSectionHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    // for the noted layout variants, the page being built is navigation
                    if (pNode.LayoutVariant == BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases ||
                        pNode.LayoutVariant == BDConstants.LayoutVariantType.Organisms_EmpiricTherapy)
                        htmlPageType = BDConstants.BDHtmlPageType.Navigation;
                    break;
                case BDConstants.BDNodeType.BDSubcategory:
                    bodyHTML.Append(BuildBDSubcategoryHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSubsection:
                    bodyHTML.Append(BuildBDSubSectionHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    // for the noted layout variant, the page being built is navigation
                    if (pNode.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring)
                        htmlPageType = BDConstants.BDHtmlPageType.Navigation;
                    break;
                case BDConstants.BDNodeType.BDSubtopic:
                    bodyHTML.Append(BuildBDSubTopicHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSurgery:
                    bodyHTML.Append(BuildBDSurgeryHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSurgeryClassification:
                    bodyHTML.Append(BuildBDSurgeryClassificationHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSurgeryGroup:
                    bodyHTML.Append(BuildBDSurgeryGroupHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDTable:
                    bodyHTML.Append(BuildBDTableHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDTopic:
                    bodyHTML.Append(BuildBDTopicHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                default:
                    break;
            }

            bodyHTML.Append(BuildBDLegendHtml(pContext, pNode, objectsOnPage));

            //TODO: Verify that this assignment is correct
            currentPageMasterObject = pNode;

            BDHtmlPage htmlPage = writeBDHtmlPage(pContext, pNode, bodyHTML, htmlPageType, footnotes, objectsOnPage, null);

            return htmlPage;
        }

        #region Standalone HTML pages
        private BDHtmlPage generatePageForAttachment(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDAttachment)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            //metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

            bodyHTML.Append(buildAttachmentHTML(pContext, pNode, footnotesOnPage, objectsOnPage));
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        /// <summary>
        /// Use for LayoutColumn Note Rendering
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pLayoutColumn"></param>
        /// <param name="pMarkedNotes"></param>
        /// <param name="pUnmarkedNotes"></param>
        /// <param name="pObjectsOnLinkedNotePage">List of LinkedNoteAssociations whose Uuid are represented by this page</param>
        /// <returns></returns>
        private BDHtmlPage generatePageForLinkedNotesOnLayoutColumn(Entities pContext, BDLayoutMetadataColumn pLayoutColumn, List<BDLinkedNote> pMarkedNotes, List<BDLinkedNote> pUnmarkedNotes, List<Guid> pObjectsOnLinkedNotePage)
        {
            StringBuilder noteHtml = new StringBuilder();

            if (null != pMarkedNotes && notesListHasContent(pContext, pMarkedNotes))
            {
                foreach (BDLinkedNote mNote in pMarkedNotes)
                {
                    string documentText = BDUtilities.CleanseStringOfEmptyTag(mNote.documentText, "p");
                    if (!string.IsNullOrEmpty(documentText))
                    {
                        noteHtml.Append(documentText);
                        pObjectsOnLinkedNotePage.Add(mNote.Uuid);
                    }
                }
            }

            if (null != pUnmarkedNotes && notesListHasContent(pContext, pUnmarkedNotes))
            {
                foreach (BDLinkedNote uNote in pUnmarkedNotes)
                {
                    string documentText = BDUtilities.CleanseStringOfEmptyTag(uNote.documentText, "p");
                    if (!string.IsNullOrEmpty(documentText))
                    {
                        noteHtml.Append(documentText);
                        pObjectsOnLinkedNotePage.Add(uNote.Uuid);
                    }
                }
            }

            List<BDHtmlPage> columnHtmlPages = BDHtmlPage.RetrieveHtmlPageForDisplayParentId(pContext, pLayoutColumn.Uuid);
            BDHtmlPage resultPage = null;
            foreach (BDHtmlPage page in columnHtmlPages)
            {
                if (page.documentText.Contains(noteHtml.ToString()))
                {
                    resultPage = page;
                    break;
                }
            }
            string noteHtmlText = BDUtilities.CleanseStringOfEmptyTag(noteHtml.ToString(), "p");
            if (!string.IsNullOrEmpty(noteHtmlText) && resultPage == null)
            {
                resultPage = writeLayoutBDHtmlPage(pContext, pLayoutColumn, noteHtml.ToString(), pObjectsOnLinkedNotePage, BDConstants.BDHtmlPageType.Comments);
            }

            return resultPage;
        }

        /// <summary>
        /// Generate an HMTL page for the specified notes
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <param name="pParentType"></param>
        /// <param name="pMarkedNotes"></param>
        /// <param name="pUnmarkedNotes"></param>
        /// <param name="pParentKeyPropertyName"></param>
        /// <param name="pObjectsOnPage">List of LinkedNoteAssociation Guids that are represented on the page</param>
        /// <returns></returns>
        private BDHtmlPage generatePageForLinkedNotes(Entities pContext, Guid pParentId, BDConstants.BDNodeType pParentType, List<BDLinkedNote> pMarkedNotes, List<BDLinkedNote> pUnmarkedNotes, string pParentKeyPropertyName, List<Guid> pObjectsOnPage)
        {
            StringBuilder noteHtml = new StringBuilder();

            if (null != pMarkedNotes && notesListHasContent(pContext, pMarkedNotes))
            {
                foreach (BDLinkedNote mNote in pMarkedNotes)
                {
                    string documentText = BDUtilities.CleanseStringOfEmptyTag(mNote.documentText, "p");
                    if (!string.IsNullOrEmpty(documentText))
                    {
                        noteHtml.Append(documentText);
                        pObjectsOnPage.Add(mNote.Uuid);
                    }
                }
            }

            if (null != pUnmarkedNotes && notesListHasContent(pContext, pUnmarkedNotes))
            {
                foreach (BDLinkedNote uNote in pUnmarkedNotes)
                {
                    string documentText = BDUtilities.CleanseStringOfEmptyTag(uNote.documentText, "p");
                    if (!String.IsNullOrEmpty(documentText))
                    {
                        noteHtml.Append(documentText);
                        pObjectsOnPage.Add(uNote.Uuid);
                    }
                }
            }

            return generatePageForLinkedNotes(pContext, pParentId, pParentType, noteHtml.ToString(), BDConstants.BDHtmlPageType.Comments, pObjectsOnPage, pParentKeyPropertyName);
        }

        private BDHtmlPage generatePageForLinkedNotes(Entities pContext, Guid pDisplayParentId, BDConstants.BDNodeType pDisplayParentType, string pPageHtml, BDConstants.BDHtmlPageType pPageType, List<Guid> pObjectsOnPage, string pParentKeyPropertyName)
        {
            // save the value of the parent page
            IBDObject parentPageMasterObject = currentPageMasterObject;
            string testValue = BDUtilities.CleanseStringOfEmptyTag(pPageHtml, "p");
            if (!string.IsNullOrEmpty(testValue))
            {
                // the linked note being processed will have a parent that is a BDNode OR another linked note
                IBDNode node = BDFabrik.RetrieveNode(pContext, pDisplayParentId);
                BDLinkedNote linkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, pDisplayParentId);

                if (node != null)
                {
                    // temporarily reset the master so the note page has a proper title
                    currentPageMasterObject = node;
                    pObjectsOnPage.Add(node.Uuid);
                    BDHtmlPage notePage = writeBDHtmlPage(pContext, node, pPageHtml, pPageType, new List<BDLinkedNote>(), pObjectsOnPage, pParentKeyPropertyName);
                    // Debug.WriteLine("Page title changed for linked note on page for:  {0}", parentPageMasterObject.Uuid);
                    // reset the master to the original (parent page) value
                    currentPageMasterObject = parentPageMasterObject;
                    return notePage;
                }
                else if (linkedNote != null)
                {
                    // temporarily reset the master so the note page has a proper title
                    currentPageMasterObject = linkedNote;
                    pObjectsOnPage.Add(linkedNote.Uuid);
                    BDHtmlPage notePage = writeBDHtmlPage(pContext, linkedNote, pPageHtml, pPageType, new List<BDLinkedNote>(), pObjectsOnPage, pParentKeyPropertyName);
                    // Debug.WriteLine("Page title changed for linked note on page for:  {0}", parentPageMasterObject.Uuid);
                    // reset the master to the original (parent page) value
                    currentPageMasterObject = parentPageMasterObject;
                    return notePage;
                }
                else
                    throw new NotSupportedException();
            }
            else
                return null;
        }

        /// <summary>
        /// Create an HTML page for the references attached to a node & property
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNotePropertyName"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns>Guid</returns>
        private Guid generatePageForParentAndPropertyReferences(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            List<Guid> objectsOnPage = new List<Guid>();
            string reference = buildTextForParentAndPropertyFromLinkedNotes(pContext, pPropertyName, pNode, BDConstants.LinkedNoteType.Reference, objectsOnPage);
            if (!string.IsNullOrEmpty(reference))
            {
                StringBuilder referenceText = new StringBuilder();
                string pageTitle = BDUtilities.ProcessTextForSymbols(pContext, pNode.Name);
                referenceText.AppendFormat(@"<h1>References</h1><h2>{0}</h2>", pageTitle);
                referenceText.Append(reference);
                BDHtmlPage footnotePage = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, referenceText.ToString(), BDConstants.BDHtmlPageType.Reference, objectsOnPage, pPropertyName);

                List<Guid> filteredObjects = objectsOnPage.Distinct().ToList();
                foreach (Guid objectId in filteredObjects)
                    BDHtmlPageMap.CreateOrRetrieveBDHtmlPageMap(pContext, Guid.NewGuid(), footnotePage.uuid, objectId);

                //ks: keep track of the create pages.
                BDHtmlPageGeneratorLogEntry logEntry = new BDHtmlPageGeneratorLogEntry(footnotePage.Uuid, pNode.Name);
                this.referencePageUuidList.Add(logEntry);
                logEntry.AppendToFile(@"refPageUuidAppendLog.txt");

                return footnotePage.Uuid;
            }
            else
                return Guid.Empty;
        }
        #endregion

        #region Build HTML component parts

        public string BuildBDAntimicrobialHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // This method gates on child node type rather than layout variant

            // PathogenGroups only describe Pathogens & TherapyGroups as children (BDFabrik)
            StringBuilder html = new StringBuilder();
            bool isFirstChild = true;

            BDNode node = pNode as BDNode;
            if (null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDAntimicrobial)
            {
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        // table tags are handled by the caller : BuildBDSubcategoryHTML
                        string amHtml = buildNodeWithReferenceAndOverviewHTML(pContext, pNode, string.Empty, pFootnotes, pObjectsOnPage);

                        bool isLastChild = false;

                        // build table rows
                        for (int idxChildren = 0; idxChildren < children.Count; idxChildren++)
                        {
                            if (idxChildren == children.Count - 1)
                                isLastChild = true;

                            IBDNode child = children[idxChildren];
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDTopic:
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));

                                    //add topic on its own tableRows
                                    // if this is the first tableRows, put antimicrobial in the first column
                                    string cellHtml = buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false, pFootnotes, pObjectsOnPage);
                                    if (isFirstChild)
                                        html.AppendFormat(@"<tr><td>{0}</td><td><b>{1}{2}{3}</b></td><td></td></tr>", amHtml, TAG_SPAN_UNDERLINE, cellHtml, TAG_SPAN_END);
                                    else
                                        html.AppendFormat(@"<tr><td></td><td><b>{0}{1}{2}</b></td><td></td></tr>", TAG_SPAN_UNDERLINE, cellHtml, TAG_SPAN_END);

                                    bool isLastTopicChild = false;

                                    // a tableRows has been written to the table for the topic, so isFirstRow is false
                                    bool isFirstRow = false;

                                    List<IBDNode> topicChildren = BDFabrik.GetChildrenForParent(pContext, child);
                                    for (int idxTopicChildren = 0; idxTopicChildren < topicChildren.Count; idxTopicChildren++)
                                    {
                                        if (idxTopicChildren == topicChildren.Count - 1)
                                            isLastTopicChild = true;

                                        IBDNode topicChild = topicChildren[idxTopicChildren];
                                        switch (topicChild.NodeType)
                                        {
                                            case BDConstants.BDNodeType.BDDosageGroup:
                                                html.Append(BuildBDDosageGroupHtml(pContext, topicChild, pFootnotes, pObjectsOnPage, pLevel, isFirstRow, isLastChild && isLastTopicChild, amHtml));
                                                break;
                                            case BDConstants.BDNodeType.BDDosage:
                                                html.Append(BuildBDDosageHtml(pContext, topicChild, pFootnotes, pObjectsOnPage, pLevel, isFirstRow, isLastChild && isLastTopicChild, amHtml));
                                                break;
                                        }
                                    }
                                    break;
                                case BDConstants.BDNodeType.BDDosageGroup:
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));
                                    html.Append(BuildBDDosageGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel, isFirstChild, isLastChild, amHtml));
                                    break;
                                case BDConstants.BDNodeType.BDDosage:
                                    html.Append(BuildBDDosageHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel, isFirstChild, isLastChild, amHtml));
                                    break;
                            }
                            isFirstChild = false;
                        }
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        string antimicrobialHtml = buildNodePropertyHTML(pContext, pNode, pNode.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                        html.AppendFormat(@"<tr><td rowspan=""{0}"" class=""v{1}"">{2}</td>", children.Count, (int)pNode.LayoutVariant, antimicrobialHtml);

                        string dosageGroupName = string.Empty;
                       //  string rowStartTag = string.Format(@"<tr><td class=""v{0}""></td>", (int)pNode.LayoutVariant);
                        string ROWSTARTTAG = @"<tr>";
                        const string ROWENDTAG = "</tr>";

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDDosage:
                                    if (!isFirstChild) html.Append(ROWSTARTTAG);

                                    html.Append(buildDosageHTML(pContext, child, dosageGroupName, pFootnotes, pObjectsOnPage));
                                    html.Append(ROWENDTAG);
                                    isFirstChild = false;
                                    break;
                                case BDConstants.BDNodeType.BDDosageGroup:
                                    if (!isFirstChild) html.Append(ROWSTARTTAG);
                                    
                                    dosageGroupName = buildNodePropertyHTML(pContext, child, child.Uuid, child.Name, BDNode.PROPERTYNAME_NAME, "u", pFootnotes, pObjectsOnPage);
                                    List<IBDNode> dgChildren = BDFabrik.GetChildrenForParent(pContext, child);
                                    foreach (IBDNode dgChild in dgChildren)
                                    {
                                        html.Append(buildDosageHTML(pContext, dgChild, dosageGroupName, pFootnotes, pObjectsOnPage));
                                    }
                                    isFirstChild = false;
                                    html.Append(ROWENDTAG);
                                    break;
                            }
                        }

                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                        foreach (IBDNode child in children)
                        {
                            if (child.NodeType == BDConstants.BDNodeType.BDTopic)
                            {
                                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));
                            }
                        }
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        html.Append(buildNodePropertyHTML(pContext, pNode, pNode.ParentId.Value, pNode.Name, BDNode.PROPERTYNAME_NAME, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                        List<BDLayoutMetadataColumn> pregnancyColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

                        List<string> pregnancyColumnsHtml = new List<string>();
                        pregnancyColumnsHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, pregnancyColumns[1], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, pFootnotes, pObjectsOnPage));
                        pregnancyColumnsHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, pregnancyColumns[2], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, pFootnotes, pObjectsOnPage));
                        foreach (IBDNode child in children)
                        {
                            BDAntimicrobialRisk amRisk = child as BDAntimicrobialRisk;
                            html.Append(BuildBDAntimicrobialRiskHtml(pContext, child, pFootnotes, pObjectsOnPage, pregnancyColumnsHtml, pLevel + 1));
                            string commentText = buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, pNode, BDConstants.LinkedNoteType.UnmarkedComment, pObjectsOnPage);
                            if (commentText.Length > 0)
                                html.AppendFormat(@"<p><b>Comments</b><br />{0}</p>", commentText);
                        }
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        //ks: Another hack that makes Karl grumpy
                        // Specific to Lactation
                        if (pNode.Name.Length > 0)
                        {
                            html.Append(buildNodePropertyHTML(pContext, pNode, pNode.ParentId.Value, pNode.Name, BDNode.PROPERTYNAME_NAME, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                        }
                        List<BDLayoutMetadataColumn> lactationColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

                        List<string> lactationColumnsHtml = new List<string>();
                        lactationColumnsHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, lactationColumns[1], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK, pFootnotes, pObjectsOnPage));
                        lactationColumnsHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, lactationColumns[2], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, pFootnotes, pObjectsOnPage));
                        lactationColumnsHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, lactationColumns[3], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, pFootnotes, pObjectsOnPage));
                        foreach (IBDNode child in children)
                        {
                            BDAntimicrobialRisk amRisk = child as BDAntimicrobialRisk;
                            html.Append(BuildBDAntimicrobialRiskHtml(pContext, child, pFootnotes, pObjectsOnPage, lactationColumnsHtml, pLevel + 1));
                            string commentText = buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, pNode, BDConstants.LinkedNoteType.UnmarkedComment, pObjectsOnPage);
                            if (commentText.Length > 0)
                                html.AppendFormat(@"<p><b>Comments</b><br />{0}</p>", commentText);
                        }
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDAntimicrobialGroupHtmlAndPage(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();

            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
            bool isFirstChild = true;

            BDNode node = pNode as BDNode;
            if (null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
            {
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        // build navigation page to child pages.
                        List<Guid> childObjects = new List<Guid>();
                        List<BDLinkedNote> childFootnotes = new List<BDLinkedNote>();
                        List<BDHtmlPage> childPages = new List<BDHtmlPage>();
                        if (children.Count > 1)
                        {
                            foreach (IBDNode child in children)
                            {
                                if (!string.IsNullOrEmpty(child.Name))
                                {
                                    string amHtml = BuildBDAntimicrobialHtml(pContext, child, childFootnotes, childObjects, pLevel);
                                    currentPageMasterObject = child;
                                    // create a page and add to collection : page has no children
                                    childPages.Add(writeBDHtmlPage(pContext, child, amHtml, BDConstants.BDHtmlPageType.Data, childFootnotes, childObjects, null));
                                }
                            }
                            html.Append(navListDivPrefix);

                            if (sortData) childPages.Sort();

                            for (int i = 0; i < childPages.Count; i++)
                            {
                                html.AppendFormat(navListAnchorTag, childPages[i].Uuid.ToString().ToUpper(), childPages[i].pageTitle);
                            }
                            html.Append(navListDivSuffix);
                        }
                        else
                        {
                            // data for single antimicrobial is listed on teh same page as the antimicrobial group
                            string amHtml = BuildBDAntimicrobialHtml(pContext, children[0], childFootnotes, childObjects, pLevel);
                            html.Append(amHtml);
                        }
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDAntimicrobialRiskHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, List<string> columnHtml, int pLevel)
        {
            // Gates on LayoutVariant to facilitate customization

            StringBuilder html = new StringBuilder();

            //   html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

            //List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant); // Here if needed

            // bool isFirstChild = true;

            List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
            switch (pNode.LayoutVariant)
            {
                case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                    BDAntimicrobialRisk risk = pNode as BDAntimicrobialRisk;
                    html.AppendFormat(@"<p><b>{0}</b>: {1}</p>", columnHtml[0], buildNodePropertyHTML(pContext, risk, risk.riskFactor, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, pFootnotes, pObjectsOnPage));
                    html.AppendFormat(@"<p><b>{0}</b>: {1}</p>", columnHtml[1], buildNodePropertyHTML(pContext, risk, risk.recommendations, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, pFootnotes, pObjectsOnPage));
                    break;
                case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                    BDAntimicrobialRisk lRisk = pNode as BDAntimicrobialRisk;
                    html.AppendFormat(@"<p><b>{0}</b>: {1}</p>", columnHtml[0], buildNodePropertyHTML(pContext, lRisk, lRisk.riskFactor, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK, pFootnotes, pObjectsOnPage));
                    html.AppendFormat(@"<p><b>{0}</b>: {1}</p>", columnHtml[1], buildNodePropertyHTML(pContext, lRisk, lRisk.aapRating, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, pFootnotes, pObjectsOnPage));
                    html.AppendFormat(@"<p><b>{0}</b>: {1}</p>", columnHtml[2], buildNodePropertyHTML(pContext, lRisk, lRisk.relativeInfantDose, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, pFootnotes, pObjectsOnPage));
                    break;
                default:
                    break;
            }

            return html.ToString();
        }

        public string BuildBDAttachmentHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDAttachment))
            {
                html.Append(buildAttachmentHTML(pContext, pNode, pFootnotes, pObjectsOnPage));
            }
            return html.ToString();
        }

        public string BuildBDCategoryHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // Gates on LayoutVariant to facilitate customization

            StringBuilder html = new StringBuilder();
            StringBuilder suffixHtml = new StringBuilder();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDCategory))
            {
                pObjectsOnPage.Add(pNode.Uuid);

                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                bool isFirstChild = true;

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        //DELTA: 1
                        ////childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis, BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics,
                        //    BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis,BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis, 
                        //    BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis, BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia}));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;

                    case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDSubcategoryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;

                    case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        if (children.Count > 0) // subcategory - column 1 values
                        {
                            List<string> columnHtml601 = new List<string>();
                            columnHtml601.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDSubcategory, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                            columnHtml601.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDMicroorganism, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));

                            html.AppendFormat(@"<table class=""v{0}""><tr>", (int)pNode.LayoutVariant);
                            for (int i = 0; i < columnHtml601.Count; i++)
                                html.AppendFormat(@"<th>{0}</th>", columnHtml601[i]);
                            html.Append(@"</tr>");

                            foreach (IBDNode child in children)
                            {
                                //BDNode subcategory = child as BDNode;
                                List<IBDNode> microList = BDFabrik.GetChildrenForParent(pContext, child);
                                StringBuilder mString = new StringBuilder();
                                foreach (IBDNode microorganism in microList)
                                {
                                    mString.AppendFormat(@"{0}<br />", buildNodePropertyHTML(pContext, microorganism, microorganism.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                                }
                                string subcatString = buildNodePropertyHTML(pContext, child, child.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                                html.AppendFormat(@"<tr><td><ul><li>{0}</ul></td><td>{1}</td></tr>", subcatString, mString);
                                pObjectsOnPage.Add(child.Uuid);
                            }
                            html.Append(@"</table>");
                        }
                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:

                        html.AppendFormat(@"<table class=""v{0}""><tr><th>Excellent Penetration</th><th>Good Penetration</th><th>Poor Penetration</th></tr>", (int)pNode.LayoutVariant);
                        if (children.Count > 0)
                        {
                            html.Append(@"<tr>");
                            foreach (IBDNode child in children)
                            {
                                pObjectsOnPage.Add(child.Uuid);
                                html.Append(@"<td>");
                                // build columns
                                List<IBDNode> columnDetail = BDFabrik.GetChildrenForParent(pContext, child);
                                if (columnDetail.Count > 0)
                                {
                                    StringBuilder colHTML = new StringBuilder();
                                    foreach (IBDNode antimicrobial in columnDetail)
                                    {
                                        colHTML.Append(buildNodePropertyHTML(pContext, antimicrobial, antimicrobial.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                                        if(antimicrobial != columnDetail.Last<IBDNode>())
                                            colHTML.Append(@"<br />");
                                    }
                                    html.Append(colHTML);
                                }
                                html.Append(@"</td>");
                            }
                            html.Append(@"</tr>");
                        }
                        html.Append(@"</table>");

                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                    case BDConstants.LayoutVariantType.Prophylaxis_IE:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDSubcategoryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                        html.Append(@"<h2>Dental, Oral, Respiratory Tract Procedures</h2>"); // there's no way to add this subtitle in the editor

                        foreach (IBDNode child in children)
                            html.Append(BuildBDTherapyGroupHTML(pContext, child as BDTherapyGroup, pFootnotes, pObjectsOnPage, pLevel + 2, BDConstants.LayoutVariantType.Undefined));
                        break;
                    case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobialGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        html.Append(@"<p>");

                        if (sortData) children.Sort((a, b) => String.Compare(a.Name, b.Name));

                        foreach (IBDNode child in children)
                        {
                            if (child.NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
                                html.AppendFormat(@"{0}<br />", buildNodeWithReferenceAndOverviewHTML(pContext, child, string.Empty, pFootnotes, pObjectsOnPage));
                        }
                        html.Append(@"</p>");
                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        if (children.Count > 0)
                        {
                            // metadataLayoutColumns defined at root
                            string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], pNode.NodeType, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                            string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE2, pFootnotes, pObjectsOnPage);

                            html.AppendFormat(@"<table class=""v{0}""><tr><th rowspan=""3"" class=""v{0} inner"">{1}</th><th rowspan=""3"" class=""v{0} inner"">{2}</th>", (int)pNode.LayoutVariant, c1Html, c2Html);
                            html.AppendFormat(@"<th class=""v{0}hw inner"" colspan=""3""><b>Dose and Interval Adjustment for Renal Impairment</b></th></tr>", (int)pNode.LayoutVariant);
                            html.AppendFormat(@"<tr><th class=""inner v{0}"" colspan=""3""><strong>{1}</strong></th></tr>", (int)pNode.LayoutVariant, c3Html);
                            html.AppendFormat(@"<tr><th class=""inner v{0}w"">&gt;50</th><th class=""inner v{0}w"">10 - 50</th><th class=""inner v{0}"">&lt;10 (Anuric)</th></tr>", (int)pNode.LayoutVariant);

                            foreach (IBDNode child in children)
                            {
                                html.Append(BuildBDAntimicrobialHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                            }

                            html.Append(@"</table>");
                        }
                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        if (children.Count > 0)
                        {
                            // start table html

                            html.AppendFormat(@"<table class=""v{0}""><tr><th>Antimicrobial</th><th>Dosage Adjustment</th></tr>", (int)pNode.LayoutVariant);
                            foreach (IBDNode child in children)
                            {
                                //child is antimicrobial with overview:  add a tableRows
                                BDNode node = child as BDNode;

                                // this is more convenient than calling buildBDAntimicrobial
                                string noteTextForCell = retrieveNoteTextForOverview(pContext, child.Uuid, pObjectsOnPage);
                                noteTextForCell = BDUtilities.CleanNoteText(noteTextForCell);
                                html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>", child.Name, noteTextForCell);
                                pObjectsOnPage.Add(child.Uuid);
                            }
                            html.Append(@"</table>");
                        }

                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }

                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDAttachment:
                                    html.Append(BuildBDAttachmentHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                case BDConstants.BDNodeType.BDTopic:
                                    html.Append(BuildBDTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                            }
                        }

                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        // create next navigation page manually - since the name of the subcategory is often blank
                        List<BDHtmlPage> childPages = new List<BDHtmlPage>();
                        List<Guid> childObjectsOnPage = new List<Guid>();
                        List<BDLinkedNote> childFootnotes = new List<BDLinkedNote>();
                        foreach (IBDNode child in children) // subcategories
                        {
                            if (!string.IsNullOrEmpty(child.Name))
                            {
                                // the next line builds child pages
                                string childHtml = BuildBDSubcategoryHtml(pContext, child, childFootnotes, childObjectsOnPage, pLevel);
                                currentPageMasterObject = child;
                                // create a page and add to collection for parent : page has children -> navigation type
                                childPages.Add(writeBDHtmlPage(pContext, child, childHtml, BDConstants.BDHtmlPageType.Navigation, childFootnotes, childObjectsOnPage, null));
                            }
                            else
                            {
                                // parent is blank - so links cannot be built.  Bypass this layer and build for next layer
                                List<IBDNode> grandchildren = BDFabrik.GetChildrenForParent(pContext, child);
                                List<Guid> gcObjects = new List<Guid>();
                                List<BDLinkedNote> gcFootnotes = new List<BDLinkedNote>();
                                foreach (IBDNode gChild in grandchildren)
                                {
                                    if (!string.IsNullOrEmpty(gChild.Name))
                                    {
                                        string gcHtml = BuildBDAntimicrobialHtml(pContext, gChild, gcFootnotes, gcObjects, pLevel);
                                        currentPageMasterObject = gChild;
                                        // create a page and add to collection:  gcHtml is a Data type of HTML page (has no children)
                                        childPages.Add(writeBDHtmlPage(pContext, gChild, gcHtml, BDConstants.BDHtmlPageType.Data, gcFootnotes, gcObjects, null));
                                    }
                                }
                            }
                        }
                        html.Append(navListDivPrefix);

                        if (sortData) childPages.Sort();

                        for (int i = 0; i < childPages.Count; i++)
                        {
                            html.AppendFormat(navListAnchorTag, childPages[i].Uuid.ToString().ToUpper(), childPages[i].pageTitle);
                        }
                        html.Append(navListDivSuffix);
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        // create next navigation page manually - since the name of the subcategory is often blank
                        List<BDHtmlPage> l_childPages = new List<BDHtmlPage>();
                        List<Guid> l_childObjectsOnPage = new List<Guid>();
                        List<BDLinkedNote> l_childFootnotes = new List<BDLinkedNote>();
                        foreach (IBDNode child in children)
                        {
                            if (!string.IsNullOrEmpty(child.Name))
                            {
                                string childHtml = BuildBDSubcategoryHtml(pContext, child, l_childFootnotes, l_childObjectsOnPage, pLevel);
                                currentPageMasterObject = child;
                                // create a page and add to collection for parent : page has children
                                l_childPages.Add(writeBDHtmlPage(pContext, child, childHtml, BDConstants.BDHtmlPageType.Navigation, l_childFootnotes, l_childObjectsOnPage, null));
                            }
                            else
                            {
                                // parent is blank - so links cannot be built.  Bypass this layer and build for next layer
                                List<IBDNode> grandchildren = BDFabrik.GetChildrenForParent(pContext, child);
                                List<Guid> gcObjects = new List<Guid>();
                                List<BDLinkedNote> gcFootnotes = new List<BDLinkedNote>();
                                foreach (IBDNode gChild in grandchildren)
                                {
                                    if (!string.IsNullOrEmpty(gChild.Name))
                                    {
                                        string gcHtml = BuildBDAntimicrobialGroupHtmlAndPage(pContext, gChild, gcFootnotes, gcObjects, pLevel);
                                        currentPageMasterObject = gChild;
                                        // create a page and add to collection : page has children
                                        l_childPages.Add(writeBDHtmlPage(pContext, gChild, gcHtml, BDConstants.BDHtmlPageType.Navigation, gcFootnotes, gcObjects, null));
                                    }
                                    else // name is empty - build pages and navigation for next layer
                                    {
                                        // antimicrobials
                                        List<IBDNode> ggchildren = BDFabrik.GetChildrenForParent(pContext, gChild);
                                        List<Guid> ggObjects = new List<Guid>();
                                        List<BDLinkedNote> ggFootnotes = new List<BDLinkedNote>();

                                        foreach (IBDNode ggChild in ggchildren)
                                        {
                                            if (!string.IsNullOrEmpty(ggChild.Name))
                                            {
                                                string ggHtml = BuildBDAntimicrobialHtml(pContext, ggChild, ggFootnotes, ggObjects, pLevel);
                                                currentPageMasterObject = ggChild;
                                                // create a page and add to collection : page has no children
                                                l_childPages.Add(writeBDHtmlPage(pContext, ggChild, ggHtml, BDConstants.BDHtmlPageType.Data, ggFootnotes, ggObjects, null));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        html.Append(navListDivPrefix);

                        if (sortData) l_childPages.Sort();

                        for (int i = 0; i < l_childPages.Count; i++)
                        {
                            html.AppendFormat(navListAnchorTag, l_childPages[i].Uuid.ToString().ToUpper(), l_childPages[i].pageTitle);
                        }
                        html.Append(navListDivSuffix);
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                        // BDTherapyGroup
                        // metadataLayoutColumns are retrieved at the beginning(ish) of this method
                        // therapy group is handled here as the layout differs from standard: the therapy group does NOT imply the start of a new table in this layout variant.

                        List<string> columnHtml = new List<string>();
                        string nameColumnTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                        string dosageColumnTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);

                        int columnCount = metadataLayoutColumns.Count + 2;

                        html.AppendFormat(@"<table class=""v{0}""><tr>", (int)pNode.LayoutVariant);
                        html.AppendFormat(@"<th>{0}</th><th>{1}</th></tr>", nameColumnTitle, dosageColumnTitle);

                        List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
                        StringBuilder conjunctionHTML = new StringBuilder();
                        BDTherapyGroup owner = null;
                        foreach (IBDNode tGroup in childNodes)
                        {
                            BDTherapyGroup therapyGroup = tGroup as BDTherapyGroup;
                            if (null != therapyGroup)
                            {
                                StringBuilder tgHTML = (BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                                html.AppendFormat(@"{0}{1}", conjunctionHTML, tgHTML);

                                conjunctionHTML.Clear();
                                if (therapyGroup.therapyGroupJoinType > 0)
                                {
                                    conjunctionHTML.AppendFormat(@"<tr><td>{0}</td><td /></tr>", retrieveConjunctionString((BDConstants.BDJoinType)therapyGroup.therapyGroupJoinType));
                                    owner = therapyGroup;
                                }
                            }
                        }

                        html.Append(@"</table>");
                        break;
                    default:
                        break;
                }
            }


            html.Append(suffixHtml);

            return html.ToString();
        }

        public string BuildBDChapterHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDChapter))
            {
                //html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.References:
                        foreach (IBDNode child in children)
                        {
                            BDNode section = child as BDNode;
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                        }
                        break;
                    default:
                        break;
                }
            }
            return html.ToString();
        }

        /// <summary>
        /// Expects that the "table /table" is managed by the caller 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <param name="pLevel"></param>
        /// <returns></returns>
        public string BuildBDConfiguredEntryHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            return BuildBDConfiguredEntryHtml(pContext, pNode, pFootnotes, pObjectsOnPage, pLevel, false, false);
        }

        /// <summary>
        /// Vertical Layout will create a header and table
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <param name="pLevel"></param>
        /// <param name="pVerticalLayout"></param>
        /// <returns></returns>
        public string BuildBDConfiguredEntryHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, bool pVerticalLayout, bool pFirstColumnEmphasized)
        {
            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDConfiguredEntry))
            {
                BDConfiguredEntry configuredEntry = pNode as BDConfiguredEntry;
                if (null != configuredEntry)
                {
                    List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

                    pObjectsOnPage.Add(configuredEntry.Uuid);
                    if (pVerticalLayout)
                    {
                        if (null != metadataLayoutColumns)
                        {
                            switch (pNode.LayoutVariant)
                            {
                                case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                                    // add header then paragraph for each configured field
                                    for (int idx = 0; idx < metadataLayoutColumns.Count; idx++)
                                    {
                                        string propertyName = metadataLayoutColumns[idx].FieldNameForColumnOfNodeType(pContext, BDConstants.BDNodeType.BDConfiguredEntry);
                                        string title = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[idx], BDConstants.BDNodeType.BDConfiguredEntry, propertyName, pFootnotes, pObjectsOnPage);
                                        string content = retrieveNoteTextForConfiguredEntryField(pContext, pNode.Uuid, BDConfiguredEntry.FieldNotePropertyNameForIndex(idx + 1), pObjectsOnPage, pFootnotes);

                                        if (content.Length == 0)
                                            title = null; // remove the title when there is no content

                                        if (!string.IsNullOrEmpty(title))
                                            html.AppendFormat(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title);

                                        if (!string.IsNullOrEmpty(content))
                                            html.Append(content);
                                    }
                                    break;
                                default:
                                    // create the header with the note contents
                                    string header = retrieveNoteTextForConfiguredEntryField(pContext, pNode.Uuid, BDConfiguredEntry.FieldNotePropertyNameForIndex(0), pObjectsOnPage, pFootnotes);
                                    if (header.StartsWith("<p>")) header = header.Substring(3);
                                    if (header.EndsWith("</p>")) header = header.Substring(0, header.Length - 4);

                                    html.AppendFormat(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), header);
                                    //Create a table with each field configured title and field note as rows (2)

                                    for (int idx = 1; idx < metadataLayoutColumns.Count; idx++)
                                    {
                                        string propertyName = metadataLayoutColumns[idx].FieldNameForColumnOfNodeType(pContext, BDConstants.BDNodeType.BDConfiguredEntry);
                                        string title = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[idx], BDConstants.BDNodeType.BDConfiguredEntry, propertyName, pFootnotes, pObjectsOnPage);
                                        string content = retrieveNoteTextForConfiguredEntryField(pContext, pNode.Uuid, BDConfiguredEntry.FieldNotePropertyNameForIndex(idx), pObjectsOnPage, pFootnotes);
                                        html.AppendFormat(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel + 2), title);
                                        html.AppendFormat(@"<table class=""v{0}"">", (int)pNode.LayoutVariant);
                                        html.AppendFormat(@"<tr><td>{0}<td></tr>", content);
                                        html.Append(@"</table>");
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        html.Append(@"<tr>");

                        switch (pNode.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                                if (null != metadataLayoutColumns)
                                {
                                    for (int idx = 0; idx < metadataLayoutColumns.Count; idx++)
                                    {
                                        string propertyName = metadataLayoutColumns[idx].FieldNameForColumnOfNodeType(pContext, BDConstants.BDNodeType.BDConfiguredEntry);
                                        string propertyValue = configuredEntry.PropertyValueForName(propertyName);

                                        string propertyHtml = buildNodePropertyHTML(pContext, configuredEntry, propertyValue, propertyName, pFootnotes, pObjectsOnPage);
                                        if (string.IsNullOrEmpty(propertyHtml))
                                            propertyHtml = buildNodePropertyHTML(pContext, configuredEntry, propertyValue, string.Format(@"{0}{1}", propertyName, BDConfiguredEntry.FIELDNOTE_SUFFIX), pFootnotes, pObjectsOnPage);

                                        if (pFirstColumnEmphasized && (idx == 0))
                                            html.AppendFormat(@"<td><b>{0}</b></td>", propertyHtml);
                                        else if (idx > 0 && idx < metadataLayoutColumns.Count - 1) // center all but the first and last columns
                                            html.AppendFormat(@"<td class=""ctrAlign"">{0}</td>", propertyHtml);
                                        else
                                            html.AppendFormat(@"<td>{0}</td>", propertyHtml);
                                    }
                                }

                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative:
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp:
                                // center all but first columns
                                if (null != metadataLayoutColumns)
                                {
                                    for (int idx = 0; idx < metadataLayoutColumns.Count; idx++)
                                    {
                                        string propertyName = metadataLayoutColumns[idx].FieldNameForColumnOfNodeType(pContext, BDConstants.BDNodeType.BDConfiguredEntry);
                                        string propertyValue = configuredEntry.PropertyValueForName(propertyName);

                                        string propertyHtml = buildNodePropertyHTML(pContext, configuredEntry, propertyValue, propertyName, pFootnotes, pObjectsOnPage);
                                        if (string.IsNullOrEmpty(propertyHtml))
                                            propertyHtml = buildNodePropertyHTML(pContext, configuredEntry, propertyValue, string.Format(@"{0}{1}", propertyName, BDConfiguredEntry.FIELDNOTE_SUFFIX), pFootnotes, pObjectsOnPage);

                                        if (idx == 0)
                                            html.AppendFormat(@"<td>{0}</td>", propertyHtml);
                                        else
                                            html.AppendFormat(@"<td class=""ctrAlign"">{0}</td>", propertyHtml);
                                    }
                                }
                                break;
                            default:
                                if (null != metadataLayoutColumns)
                                {
                                    for (int idx = 0; idx < metadataLayoutColumns.Count; idx++)
                                    {
                                        string propertyName = metadataLayoutColumns[idx].FieldNameForColumnOfNodeType(pContext, BDConstants.BDNodeType.BDConfiguredEntry);
                                        string propertyValue = configuredEntry.PropertyValueForName(propertyName);
                                       
                                        string propertyHtml = buildNodePropertyHTML(pContext, configuredEntry, configuredEntry.uuid, propertyValue, propertyName, "p", pFootnotes, pObjectsOnPage);

                                        if (string.IsNullOrEmpty(propertyHtml))
                                            propertyHtml = buildNodePropertyHTML(pContext, configuredEntry, propertyValue, string.Format(@"{0}{1}", propertyName, BDConfiguredEntry.FIELDNOTE_SUFFIX), pFootnotes, pObjectsOnPage);
                                        if (pFirstColumnEmphasized && (idx == 0))
                                            html.AppendFormat(@"<td><b>{0}</b></td>", propertyHtml);
                                        else
                                            html.AppendFormat(@"<td>{0}</td>", propertyHtml);
                                    }
                                }
                                break;
                        }

                        html.Append(@"</tr>");
                    }
                }
            }

            return html.ToString();
        }

        public string BuildBDCombinedEntryHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, bool pRenderTableHeader)
        {
            // Gates on LayoutVariant to facilitate customization

            StringBuilder html = new StringBuilder();
            StringBuilder prefixHtml = new StringBuilder();

            string col1Header = "";
            string col2Header = "";
            string col3Header = "";

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDCombinedEntry))
            {
                BDCombinedEntry combinedEntry = pNode as BDCombinedEntry;
                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, combinedEntry.LayoutVariant);

                if (pRenderTableHeader)
                {
                    //NOTE: This expects that a matching number of columns have been defined
                    html.AppendFormat(@"<table class=""v{0}""><tr>", (int)pNode.LayoutVariant);
                    int columnIndex = 0;
                    foreach (BDLayoutMetadataColumn layoutColumn in metadataLayoutColumns)
                    {
                        string definedColumnName = layoutColumn.FieldNameForColumnOfNodeType(pContext, BDConstants.BDNodeType.BDCombinedEntry);
                        switch (definedColumnName)
                        {
                            case BDCombinedEntry.PROPERTYNAME_NAME:
                                col1Header = layoutColumn.label;
                                break;
                            case BDCombinedEntry.VIRTUALPROPERTYNAME_ENTRYTITLE:
                                col2Header = layoutColumn.label;
                                break;
                            case BDCombinedEntry.VIRTUALPROPERTYNAME_ENTRYDETAIL:
                                col3Header = layoutColumn.label;
                                break;
                            case BDCombinedEntry.VIRTUALCOLUMNNAME_01:
                                col2Header = layoutColumn.label;
                                break;
                            case BDCombinedEntry.VIRTUALCOLUMNNAME_02:
                                col3Header = layoutColumn.label;
                                break;
                            case "":
                                switch (columnIndex)
                                {
                                    case 0:
                                        col2Header = layoutColumn.label;
                                        break;
                                    case 1:
                                        col3Header = layoutColumn.label;
                                        break;
                                }
                                break;
                        }
                    }
                }

                switch (pNode.LayoutVariant)
                {
                    // created from a table (Prophylaxis_Communicable_Influenza)
                    //case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:

                    //    break;

                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_NoRenal: //3121
                        if (pRenderTableHeader)
                        {
                            html.AppendFormat(@"<th>{0}</th><th>{1}</th></tr>", col2Header, col3Header);
                        }

                        prefixHtml.AppendFormat(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), combinedEntry.Name);
                        html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>",
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, pFootnotes, pObjectsOnPage));
                        html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>",
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, pFootnotes, pObjectsOnPage));
                        html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>",
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, pFootnotes, pObjectsOnPage));

                            break;

                    // created from a topic (Prophylaxis_Communicable_Influenza_Oseltamivir) 3123
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Creatinine: //3124
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Weight: //3125
                        if (pRenderTableHeader)
                        {
                            html.AppendFormat(@"<th>{0}</th><th>{1}</th><th>{2}</th></tr>", col1Header, col2Header, col3Header);
                        }
                        html.AppendFormat(@"<tr><td rowspan=""4"">{0}</td><td>{1}</td><td>{2}</td></tr>",
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.Name, BDCombinedEntry.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, pFootnotes, pObjectsOnPage));

                        if (combinedEntry.entryDetail02 != null && combinedEntry.entryTitle02 != null)
                        {
                            html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, pFootnotes, pObjectsOnPage),
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, pFootnotes, pObjectsOnPage));
                        }
                        if (combinedEntry.entryDetail03 != null && combinedEntry.entryTitle03 != null)
                        {
                            html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, pFootnotes, pObjectsOnPage),
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, pFootnotes, pObjectsOnPage));
                        }
                        if (combinedEntry.entryTitle04 != null && combinedEntry.entryDetail04 != null)
                        {
                            html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle04, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04, pFootnotes, pObjectsOnPage),
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail04, BDCombinedEntry.PROPERTYNAME_ENTRY04, pFootnotes, pObjectsOnPage));
                        }
                        break;

                    // created from a topic (Prophylaxis_Communicable_Influenza_Zanamivir) 3126
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Zanamivir: //3126
                        StringBuilder cell0Html = new StringBuilder();
                        StringBuilder cell1Html = new StringBuilder();

                        if (pRenderTableHeader)
                        {
                            html.AppendFormat(@"<th>{0}</th><th>{1}</th></tr>", col1Header, col3Header);
                        }

                        // first column
                        if (!string.IsNullOrEmpty(combinedEntry.groupTitle))
                        {
                            cell0Html.AppendFormat(@"{0}{1}{2}", TAG_SPAN_UNDERLINE,
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.groupTitle, BDCombinedEntry.PROPERTYNAME_GROUPTITLE, pFootnotes, pObjectsOnPage),
                                  TAG_SPAN_END);
                            cell0Html.Append(@"<br />");
                        }

                        if (!string.IsNullOrEmpty(combinedEntry.Name))
                        {
                            cell0Html.AppendFormat(@"{0}<br />",
                                                   buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.Name, BDCombinedEntry.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.GroupJoinType) != string.Empty)
                                cell0Html.AppendFormat(@"{0}<br />", retrieveConjunctionString(combinedEntry.GroupJoinType));
                        }

                        // second column
                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle01))
                        {
                            cell1Html.AppendFormat(@"{0}{1}{2}", TAG_SPAN_UNDERLINE,
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, pFootnotes, pObjectsOnPage),
                                TAG_SPAN_END);
                            cell1Html.Append(@"<br />");
                        }

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail01))
                        {
                            cell1Html.AppendFormat(@"{0}<br />",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType01) != string.Empty)
                                cell1Html.AppendFormat(@"<b>{0}</b><br />", retrieveConjunctionString(combinedEntry.JoinType01));
                        }

                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle02))
                            cell1Html.AppendFormat(@"{0}{1}{2}<br />", TAG_SPAN_UNDERLINE,
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, pFootnotes, pObjectsOnPage),
                                TAG_SPAN_END);

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail02))
                        {
                            cell1Html.AppendFormat(@"{0}<br />", buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType02) != string.Empty)
                            {
                                cell1Html.AppendFormat(@"<b>{0}</b>", retrieveConjunctionString(combinedEntry.JoinType02));
                            }
                        }

                        html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>", cell0Html, cell1Html);
                        break;
                    // created from a table of the same layoutVariant
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive: //311
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae: //315
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis: //316

                    // everything else
                    default:
                        StringBuilder cell0HTML = new StringBuilder();
                        StringBuilder cell1HTML = new StringBuilder();
                        StringBuilder cell2HTML = new StringBuilder();

                        if (pRenderTableHeader)
                        {
                            html.AppendFormat(@"<th>{0}</th><th>{1}</th><th>{2}</th></tr>", col1Header, col2Header, col3Header);
                        }

                        // first column
                        if (!string.IsNullOrEmpty(combinedEntry.groupTitle))
                        {
                            cell0HTML.AppendFormat(@"{0}{1}{2}", TAG_SPAN_UNDERLINE,
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.groupTitle, BDCombinedEntry.PROPERTYNAME_GROUPTITLE, pFootnotes, pObjectsOnPage),
                                  TAG_SPAN_END);
                            cell0HTML.Append(@"<br />");
                        }

                        if (!string.IsNullOrEmpty(combinedEntry.Name))
                        {
                            cell0HTML.AppendFormat(@"<b>{0}</b><br />",
                                                   buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.Name, BDCombinedEntry.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.GroupJoinType) != string.Empty)
                                cell0HTML.AppendFormat(@"{0}<br />", retrieveConjunctionString(combinedEntry.GroupJoinType));
                        }

                        // second column
                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle01))
                            cell1HTML.AppendFormat(@"{0}{1}{2}", TAG_SPAN_UNDERLINE,
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, pFootnotes, pObjectsOnPage),
                                TAG_SPAN_END);
                        cell1HTML.Append(@"<br />");

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail01))
                        {
                            cell1HTML.AppendFormat(@"{0}<br />",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType01) != string.Empty)
                                cell1HTML.AppendFormat(@"<b>{0}</b><br />", retrieveConjunctionString(combinedEntry.JoinType01));
                        }

                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle02))
                            cell1HTML.AppendFormat(@"{0}{1}{2}<br />", TAG_SPAN_UNDERLINE,
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, pFootnotes, pObjectsOnPage),
                                TAG_SPAN_END);

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail02))
                        {
                            cell1HTML.AppendFormat(@"{0}<br />", buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType02) != string.Empty)
                            {
                                cell1HTML.AppendFormat(@"<b>{0}</b>", retrieveConjunctionString(combinedEntry.JoinType02));
                            }
                        }

                        //third column
                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle03))
                            cell2HTML.AppendFormat(@"{0}{1}{2}", TAG_SPAN_UNDERLINE,
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, pFootnotes, pObjectsOnPage),
                                TAG_SPAN_END);
                        cell2HTML.Append(@"<br />");

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail03))
                        {
                            cell2HTML.AppendFormat(@"{0}<br />", buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType03) != string.Empty)
                                cell2HTML.AppendFormat(@"<b>{0}</b><br />", retrieveConjunctionString(combinedEntry.JoinType03));
                        }

                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle04))
                            cell2HTML.AppendFormat(@"{0}{1}{2}<br />", TAG_SPAN_UNDERLINE,
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle04, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04, pFootnotes, pObjectsOnPage),
                                TAG_SPAN_END);

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail04))
                        {
                            cell2HTML.AppendFormat(@"{0}<br />",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail04, BDCombinedEntry.PROPERTYNAME_ENTRY04, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType04) != string.Empty)
                                cell2HTML.AppendFormat(@"<b>{0}</b><br />", retrieveConjunctionString(combinedEntry.JoinType04));
                        }

                        html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", cell0HTML, cell1HTML, cell2HTML);
                        break;
                }
            }

            html.Insert(0, prefixHtml);
            return html.ToString();
        }

        public string BuildBDDiseaseHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // gated on layout variant

            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDDisease))
            {
                // describe the disease

                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities, BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDPresentationHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDPresentationHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPresentation:
                                    html.Append(BuildBDPresentationHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                case BDConstants.BDNodeType.BDTable:
                                    html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                        }
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDTherapyGroupHTML(pContext, child as BDTherapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                        }

                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir, BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Zanamivir }));
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDTable:
                                    html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                case BDConstants.BDNodeType.BDTopic:
                                    html.Append(BuildBDTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDDosageHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, bool pIsFirstChildRow, bool pIsLastDosage, string pParentHtml)
        {
            StringBuilder html = new StringBuilder();
            switch (pNode.LayoutVariant)
            {
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                    StringBuilder costHTML = new StringBuilder();
                    BDDosage dosage = pNode as BDDosage;

                    if (pIsFirstChildRow) // append parent html for first column
                        html.AppendFormat(@"<tr><td>{0}</td>", pParentHtml);
                    else // append empty column
                        html.Append(@"<tr><td></td>");

                    string dosageCellHtml = buildCellHTML(pContext, dosage, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, pFootnotes, pObjectsOnPage);
                    if (dosage.joinType == (int)BDConstants.BDJoinType.Next)
                        html.AppendFormat(@"<td>{0}</td>", dosageCellHtml);
                    else
                        html.AppendFormat(@"<td>{0} {1}</td>", dosageCellHtml, retrieveConjunctionString((int)dosage.joinType));

                    html.AppendFormat(@"<td>{0}", buildCellHTML(pContext, dosage, BDDosage.PROPERTYNAME_COST, dosage.cost, false, pFootnotes, pObjectsOnPage));
                    if (dosage.cost2.Length > 0)
                        html.AppendFormat(@" {0}</td>", buildCellHTML(pContext, dosage, BDDosage.PROPERTYNAME_COST2, dosage.cost2, false, pFootnotes, pObjectsOnPage));
                    else
                        html.Append(@"</td>");
                    html.Append(@"</tr>");
                    break;
                default:
                    break;
            }
            return html.ToString();
        }

        public string BuildBDDosageGroupHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, bool pIsFirstRow, bool pIsLastDosageGroup, string pParentHtml)
        {
            pObjectsOnPage.Add(pNode.Uuid);
            StringBuilder html = new StringBuilder();
            switch (pNode.LayoutVariant)
            {
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                    // append a tableRows for the DosageGroup
                    string dosageGroupHtml = buildCellHTML(pContext, pNode, BDNode.PROPERTYNAME_NAME, pNode.Name, false, pFootnotes, pObjectsOnPage);
                    if (pIsFirstRow) // add parent to first column (antimicrobial)
                        html.AppendFormat(@"<tr><td>{0}</td><td>{1}{2}{3}</td><td></td></tr>", pParentHtml, TAG_SPAN_UNDERLINE ,dosageGroupHtml, TAG_SPAN_END);
                    else
                        html.AppendFormat(@"<tr><td></td><td>{0}{1}{2}</td><td></td></tr>", TAG_SPAN_UNDERLINE, dosageGroupHtml, TAG_SPAN_END);

                    bool isLastDosage = false;
                    // a tableRows is now written to the table for this group, so anything following is not the first tableRows.
                    bool isFirstRow = false;
                    List<IBDNode> dosageGroupChildren = BDFabrik.GetChildrenForParent(pContext, pNode);
                    for (int i = 0; i < dosageGroupChildren.Count; i++)
                    {
                        if (i == dosageGroupChildren.Count - 1)
                            isLastDosage = true;
                        // add tableRows for BDDosage
                        html.Append(BuildBDDosageHtml(pContext, dosageGroupChildren[i], pFootnotes, pObjectsOnPage, pLevel, isFirstRow, (isLastDosage && pIsLastDosageGroup), pParentHtml));
                    }
                    break;
                default:
                    break;
            }
            return html.ToString();
        }

        /// <summary>
        /// Will return string.Empty if result contains only 'p /p' tags
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <returns></returns>
        public string BuildBDLegendHtml(Entities pContext, IBDNode pNode, List<Guid> pObjectsOnPage)
        {
            if (null == pNode) return string.Empty;

            StringBuilder html = new StringBuilder();
            List<Guid> legendAssociations;
            List<BDLinkedNote> legendNotes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend, out legendAssociations);

            bool hasLegend = false;
            foreach (Guid legendId in legendAssociations)
            {
                if (pObjectsOnPage.Contains(legendId))
                    hasLegend = true;
            }
            string legendHTML = buildTextFromNotes(legendNotes);
            if (!string.IsNullOrEmpty(legendHTML) && !hasLegend)
            {
                html.AppendFormat(@"<div class=""legend"">{0}</div>",legendHTML);
                pObjectsOnPage.AddRange(legendAssociations);
            }

            return html.ToString();
        }

        public string BuildBDMicroorganismGroupHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDMicroorganismGroup))
            {
                // describe the topic
                //BDNode topic = pNode as BDNode;

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                bool isFirstChild = true;

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        html.Append(buildNodePropertyHTML(pContext, pNode, pNode.Uuid, pNode.Name, BDNode.PROPERTYNAME_NAME, HtmlHeaderTagLevelString(pLevel + 2), pFootnotes, pObjectsOnPage));
                        html.Append(@"<p>");
                        foreach (IBDNode child in children)
                        {
                            html.AppendFormat(@"{0}<br />", buildNodePropertyHTML(pContext, child, child.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                        }
                        html.Append(@"</p>");
                        break;

                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "b", pFootnotes, pObjectsOnPage));
                        html.Append(@"<ul>");

                        foreach (IBDNode child in children)
                        {
                            html.AppendFormat(@"<li>{0}</li>", buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage));
                        }
                        html.Append(@"</ul>");
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                    //break;
                    default:
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                        html.Append(@"<p>");

                        foreach (IBDNode child in children)
                        {
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage));
                            html.Append(@"<br />");
                        }
                        html.Append(@"</p>");
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDPathogenHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            List<BDHtmlPage> generatedPages;
            return BuildBDPathogenHtml(pContext, pNode, pFootnotes, pObjectsOnPage, pLevel, out generatedPages);
        }

        public string BuildBDPathogenHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, out List<BDHtmlPage> pGeneratedPages)
        {
            StringBuilder html = new StringBuilder();
            StringBuilder suffixHtml = new StringBuilder();
            pGeneratedPages = null;

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDPathogen))
            {
                pObjectsOnPage.Add(pNode.Uuid);

                // moved to layout specific because of customizations
                //html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenResistance, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDPathogenResistanceHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }

                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenResistance, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                        foreach (IBDNode child in children)
                        {
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));
                            List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, child);
                            foreach (IBDNode therapyGroupChild in therapyGroups)
                            {
                                BDTherapyGroup therapyGroup = therapyGroupChild as BDTherapyGroup;
                                html.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                            }
                        }

                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:

                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                        
                        StringBuilder conjunctionHTML = new StringBuilder();
                        BDTherapyGroup owner = null;
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPresentation:

                                    string symptoms = retrieveNoteTextForOverview(pContext, child.Uuid, pObjectsOnPage);
                                    if (!string.IsNullOrEmpty(symptoms))
                                        html.AppendFormat(@"<p>{0}<b>Symptoms</b>{1}<br />{2}</p>", TAG_SPAN_UNDERLINE, TAG_SPAN_END, symptoms);
                                    pObjectsOnPage.Add(child.Uuid);
                                    break;
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    {
                                        BDTherapyGroup therapyGroup = child as BDTherapyGroup;

                                        StringBuilder tgHTML = (BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel, null));
                                        html.AppendFormat(@"{0}{1}", conjunctionHTML, tgHTML);

                                        conjunctionHTML.Clear();
                                        if (therapyGroup.therapyGroupJoinType > 0)
                                        {
                                            conjunctionHTML.AppendFormat(@"<p>{0}</p>", retrieveConjunctionString((BDConstants.BDJoinType)therapyGroup.therapyGroupJoinType));
                                            owner = therapyGroup;
                                        }
                                    }
                                    break;
                            }
                        }

                        StringBuilder endNotesHTML = new StringBuilder();
                        List<Guid> noteAssociations;
                        List<BDLinkedNote> endnotes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Endnote, out noteAssociations);
                        if (endnotes.Count > 0)
                            html.Append(buildTextFromNotes(endnotes));

                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        StringBuilder diagnosis = new StringBuilder();
                        StringBuilder clinical = new StringBuilder();
                        StringBuilder therapy = new StringBuilder();

                        diagnosis.Append(string.Format(@"<{0}>Diagnosis</{0}>", HtmlHeaderTagLevelString(pLevel + 1)));
                        clinical.Append(string.Format(@"<{0}>Clinical</{0}>", HtmlHeaderTagLevelString(pLevel + 1)));

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                    therapy.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                                    break;
                                case BDConstants.BDNodeType.BDTopic:
                                    //HACK
                                    List<Guid> immedAssociations;
                                    List<BDLinkedNote> immediate = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, child.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Immediate, out immedAssociations);
                                    string immediateText = BDUtilities.BuildTextFromInlineNotes(immediate);
                                    //end hack

                                    //clinical.AppendFormat(@"<p>{0}{1}</p>", child.Name, immediateText);
                                    pObjectsOnPage.AddRange(immedAssociations);
                                    pObjectsOnPage.Add(child.Uuid);

                                    string inlineText = buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, child, BDConstants.LinkedNoteType.Inline, pObjectsOnPage);
                                    inlineText = BDUtilities.CleanNoteText(inlineText);

                                    clinical.AppendFormat(@"<p>{0}{1} {2}</p>", child.Name, immediateText, inlineText);
                                    // overview contains the 'Diagnosis' column data
                                    diagnosis.Append(retrieveNoteTextForOverview(pContext, child.Uuid, pObjectsOnPage));
                                    break;
                            }
                        }

                        html.AppendFormat(@"{0}<br />{1}<br />{2}", clinical, diagnosis, therapy);

                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        pGeneratedPages = new List<BDHtmlPage>();
                        List<Guid> localObjectsOnPage = new List<Guid>();
                        List<BDLinkedNote> localFootnotes = new List<BDLinkedNote>();
                        StringBuilder localHtml = new StringBuilder();

                        localHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), localFootnotes, localObjectsOnPage));

                        List<IBDNode> topics = BDFabrik.GetChildrenForParent(pContext, pNode);
                        foreach (IBDNode topic in topics)
                        {
                            if (topic.Name != "Infectious Agent")
                            {
                                localHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topic, HtmlHeaderTagLevelString(pLevel + 2), localFootnotes, localObjectsOnPage));
                            }
                        }
                        currentPageMasterObject = pNode;
                        pGeneratedPages.Add(writeBDHtmlPage(pContext, pNode, localHtml, BDConstants.BDHtmlPageType.Data, localFootnotes, localObjectsOnPage, null));

                        break;
                    default:
                        break;
                }
            }

            html.Insert(0, suffixHtml);
            return html.ToString();
        }

        public string BuildBDOrganismHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDOrganism))
            {
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                        // HTML page assembled here
                        // add organism with overview to html
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                        StringBuilder configEntryHtml = new StringBuilder();
                        StringBuilder tableHtml = new StringBuilder();
                        tableHtml.AppendFormat(@"<table class=""v{0}"">", (int)pNode.LayoutVariant);

                        // get title from layout configuration
                        string therapyNameTitleHtml = string.Empty;
                        BDLayoutMetadataColumn nameColumn = BDLayoutMetadataColumn.Retrieve(pContext, pNode.LayoutVariant, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY);
                        if (null != nameColumn)
                            therapyNameTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, nameColumn, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);

                        // add table header row with configured title
                        if (!string.IsNullOrEmpty(therapyNameTitleHtml))
                            tableHtml.AppendFormat(@"<tr><th>{0}</th></tr>", therapyNameTitleHtml);

                        foreach (IBDNode child in children)
                        {
                            // add configured entry 
                            if (child.NodeType == BDConstants.BDNodeType.BDConfiguredEntry)
                            {
                                BDConfiguredEntry configuredEntry = child as BDConfiguredEntry;
                                configEntryHtml.Append(BuildBDConfiguredEntryHtml(pContext, configuredEntry, pFootnotes, pObjectsOnPage, pLevel + 1, true, false));
                            }
                            // add therapy group & therapy in 'Empiric Therapy' table
                            if (child.NodeType == BDConstants.BDNodeType.BDTherapyGroup)
                            {
                                BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                tableHtml.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                            }
                        }

                        tableHtml.Append(@"</table>");

                        html.Append(configEntryHtml);
                        html.Append(tableHtml);

                        StringBuilder endNotesHTML = new StringBuilder();

                        List<Guid> noteAssociations;
                        List<BDLinkedNote> endnotes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Endnote, out noteAssociations);

                        html.Append(buildTextFromNotes(endnotes));
                        pObjectsOnPage.AddRange(noteAssociations);

                        break;
                    default:
                        break;
                }
            }
            return html.ToString();
        }
        /// <summary>
        /// Build reference HTML for a pathogenGroup. BDFabrik defines BDPathogen and BDTherapyGroup as possible children
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <returns></returns>
        public string BuildBDPathogenGroupHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, bool pHasUsualPathogenTitle)
        {
            // Exception: TherapyGroup & Therapy
            StringBuilder html = new StringBuilder();

            BDNode pathogenGroup = pNode as BDNode;
            if (null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
            {
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pathogenGroup);

                StringBuilder pathogenHtml = new StringBuilder();
                StringBuilder therapyGroupHtml = new StringBuilder();

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:

                        // describe the pathogen group
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(buildNodePropertyHTML(pContext, child, child.Name, BDTherapyGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                            // custom-built - Therapy has 2 dosages and a custom header

                            string therapyNameTitleHtml = string.Empty;
                            string therapyDosage1TitleHtml = string.Empty;
                            string therapyDosage2TitleHtml = string.Empty;
                            string therapyDurationTitleHtml = string.Empty;

                            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, child.LayoutVariant);

                            if (metadataLayoutColumns.Count > 1)
                                therapyNameTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                            if (metadataLayoutColumns.Count > 2)
                                therapyDosage1TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                            if (metadataLayoutColumns.Count > 3)
                                therapyDosage2TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);
                            if (metadataLayoutColumns.Count > 4)
                                therapyDurationTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[4], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, pFootnotes, pObjectsOnPage);


                            List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, child.Uuid);
                            if (therapies.Count > 0)
                            {
                                html.AppendFormat(@"<table class=""v{0}"">", (int)child.LayoutVariant);

                                resetGlobalVariablesForTherapies();
                                StringBuilder therapyHTML = new StringBuilder();
                                bool isTherapyInBrackets = false;
                                foreach (BDTherapy therapy in therapies)
                                {
                                    if (therapy.leftBracket.HasValue && therapy.leftBracket.Value == true)
                                        isTherapyInBrackets = true;
                                    therapyHTML.Append(buildTherapyHtml(pContext, therapy, pFootnotes, pObjectsOnPage, isTherapyInBrackets));
                                    if (therapy.rightBracket.HasValue && therapy.rightBracket.Value == true)
                                        isTherapyInBrackets = false;

                                    if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                        previousTherapyNameId = therapy.Uuid;
                                }
                                html.AppendFormat(@"<tr><th>{0}</th>", therapyNameTitleHtml);
                                html.AppendFormat(@"<th>{0}</th><th>{1}</th>", therapyDosage1TitleHtml, therapyDosage2TitleHtml);
                                html.AppendFormat(@"<th>{0}</th>", therapyDurationTitleHtml);

                                html.Append(@"</tr>");

                                html.Append(therapyHTML);
                                html.Append(@"</table>");
                            }
                        }
                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                        if (pHasUsualPathogenTitle)
                        {
                            string title = "Usual Pathogens";
                            html.Append(string.Format(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title));
                        }

                        // describe the pathogen group
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));

                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogen:
                                    pathogenHtml.AppendFormat(@"{0}<br />", (buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage)));
                                    break;
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    string tgHtml = buildNodePropertyHTML(pContext, child, child.Name, BDTherapyGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                                    if (!string.IsNullOrEmpty(tgHtml))
                                        therapyGroupHtml.AppendFormat(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel + 1), tgHtml);

                                    // Therapy has 2 dosages, no duration and a custom header
                                    string therapyNameTitleHtml = string.Empty;
                                    string therapyDosageSpanTitle = string.Empty;
                                    string therapyDosage1TitleHtml = string.Empty;
                                    string therapyDosage2TitleHtml = string.Empty;

                                    List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, child.LayoutVariant);

                                    if (metadataLayoutColumns.Count > 0)
                                        therapyNameTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                                    if (metadataLayoutColumns.Count > 1)
                                        therapyDosageSpanTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDMetaDecoration, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                                    if (metadataLayoutColumns.Count > 2)
                                        therapyDosage1TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                                    if (metadataLayoutColumns.Count > 3)
                                        therapyDosage2TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);


                                    List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, child.Uuid);
                                    if (therapies.Count > 0)
                                    {
                                        therapyGroupHtml.AppendFormat(@"<table class=""v{0}"">", (int)child.LayoutVariant);

                                        resetGlobalVariablesForTherapies();
                                        StringBuilder therapyHTML = new StringBuilder();
                                        bool isTherapyInBrackets = false;
                                        foreach (BDTherapy therapy in therapies)
                                        {
                                            if (therapy.leftBracket.HasValue && therapy.leftBracket.Value == true)
                                                isTherapyInBrackets = true;
                                            therapyHTML.Append(buildTherapyHtml(pContext, therapy, pFootnotes, pObjectsOnPage, isTherapyInBrackets));
                                            if (therapy.rightBracket.HasValue && therapy.rightBracket.Value == true)
                                                isTherapyInBrackets = false;

                                            if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                                previousTherapyNameId = therapy.uuid;
                                            if (!string.IsNullOrEmpty(therapy.dosage) && therapy.dosageSameAsPrevious == false)
                                                previousTherapyDosageId = therapy.uuid;
                                            if (!string.IsNullOrEmpty(therapy.dosage1) && therapy.dosage1SameAsPrevious == false)
                                                previousTherapyDosage1Id = therapy.uuid;
                                            if (!string.IsNullOrEmpty(therapy.dosage2) && therapy.dosage2SameAsPrevious == false)
                                                previousTherapyDosage2Id = therapy.uuid;
                                            if (!string.IsNullOrEmpty(therapy.duration) && therapy.durationSameAsPrevious == false)
                                                previousTherapyDurationId = therapy.uuid;
                                            if (!string.IsNullOrEmpty(therapy.duration1) && therapy.duration1SameAsPrevious == false)
                                                previousTherapyDuration1Id = therapy.uuid;
                                            if (!string.IsNullOrEmpty(therapy.duration2) && therapy.duration2SameAsPrevious == false)
                                                previousTherapyDuration2Id = therapy.uuid;
                                        }
                                        therapyGroupHtml.AppendFormat(@"<tr><th rowspan=""2"">{0}</th>", therapyNameTitleHtml);
                                        therapyGroupHtml.AppendFormat(@"<th colspan=""2"">{0}</th></tr>", therapyDosageSpanTitle);
                                        therapyGroupHtml.AppendFormat(@"<tr><th class=""inner"">{0}</th><th class=""inner"">{1}</th></tr>", therapyDosage1TitleHtml, therapyDosage2TitleHtml);

                                        therapyGroupHtml.Append(therapyHTML);
                                        therapyGroupHtml.Append(@"</table>");
                                    }
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(pathogenHtml.ToString()))
                            html.AppendFormat(@"<p>{0}</p>", pathogenHtml);
                        if (!string.IsNullOrEmpty(therapyGroupHtml.ToString()))
                            html.Append(therapyGroupHtml);

                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:

                        bool childrenArePathogens = false;
                        foreach (IBDNode child in children)
                        {
                            if (child.NodeType == BDConstants.BDNodeType.BDPathogen)
                            {
                                childrenArePathogens = true;
                                break;
                            }
                        }
                        if (pHasUsualPathogenTitle && childrenArePathogens)
                        {
                            string title = "Usual Pathogens";
                            if (pNode.ParentId == Guid.Parse("54f2fcf0-8cbb-42d0-b61e-494838b1920e")) title = "Potential Pathogens";
                            html.Append(string.Format(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title));
                        }
                        else if (pHasUsualPathogenTitle && pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis)
                        {
                            // in the specific case of this layout, the title is required even if there are no pathogens.  <ld - Jan 2013>
                            string title = "Usual Pathogens";
                            html.Append(string.Format(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title));
                        }

                        // separate the pathogen groups' data with a line
                        html.Append(horizontalRuleTag);

                        // describe the pathogen group
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogen:
                                    pathogenHtml.AppendFormat(@"{0}", (buildNodeWithReferenceAndOverviewHTML(pContext, child, "span", pFootnotes, pObjectsOnPage)));
                                    break;
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                    if (null != therapyGroup)
                                    {
                                        therapyGroupHtml.AppendFormat(@"{0}", (BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 2, null)));
                                    }
                                    break;
                            }
                        }
                        int firstParagraphStartIndex = pathogenHtml.ToString().IndexOf(@"<p>");
                        if (!string.IsNullOrEmpty(pathogenHtml.ToString()))
                        {
                            if (firstParagraphStartIndex > 0)
                                html.AppendFormat(@"<p>{0}</p>", pathogenHtml);
                            else
                                html.Append(pathogenHtml);
                        }

                        if (!string.IsNullOrEmpty(therapyGroupHtml.ToString()))
                            html.Append(therapyGroupHtml);
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                        if (pHasUsualPathogenTitle)
                        {
                            string title = "Usual Pathogens";
                            if (pNode.ParentId == Guid.Parse("54f2fcf0-8cbb-42d0-b61e-494838b1920e")) title = "Potential Pathogens";
                            html.Append(string.Format(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title));
                        }

                        // separate the pathogen groups' data with a line
                        html.Append(horizontalRuleTag);

                        // describe the pathogen group
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogen:
                                    pathogenHtml.AppendFormat(@"{0}", (buildNodeWithReferenceAndOverviewHTML(pContext, child, "span", pFootnotes, pObjectsOnPage)));
                                    break;
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                    if (null != therapyGroup)
                                    {
                                        therapyGroupHtml.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 2, null));
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (!string.IsNullOrEmpty(pathogenHtml.ToString()))
                            html.Append(pathogenHtml);
                        if (!string.IsNullOrEmpty(therapyGroupHtml.ToString()))
                            html.AppendFormat(@"{0}<br />", therapyGroupHtml);
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        {
                            if (pHasUsualPathogenTitle)
                            {
                                string title = "Usual Pathogens";
                                html.Append(string.Format(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title));
                            }
                            // describe the pathogen group
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));

                            StringBuilder tgConjunctionHTML = new StringBuilder();

                            bool pathogenHasAttachedNotes = false;
                            foreach (IBDNode child in children)
                            {
                                switch (child.NodeType)
                                {
                                    case BDConstants.BDNodeType.BDPathogen:
                                        if(BDNode.HasAttachedNotes(pContext, child as BDNode)) {  // check if Pathogen has inline or immediate linked notes
                                            pathogenHasAttachedNotes = true;
                                            // build the pathogen HTML like a 'node with overview' - node name wrapped in <p>, with notes as entered added below node name
                                            pathogenHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));
                                        }
                                        else {
                                            // build Pathogen list the *normal Pathogen* way, inserting a <br> between pathogens, (later wrap in <p>)
                                            pathogenHtml.AppendFormat(@"{0}<br />", (buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage)));
                                        }
                                        break;
                                    case BDConstants.BDNodeType.BDTherapyGroup:
                                        BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                        if (null != therapyGroup)
                                        {
                                            int level = pLevel + 2;
                                            if (child.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic)
                                                level = pLevel + 1;
                                            StringBuilder tgHTML = (BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, level, null));
                                            therapyGroupHtml.AppendFormat(@"{0}{1}", tgConjunctionHTML, tgHTML);

                                            tgConjunctionHTML.Clear();
                                            if (therapyGroup.therapyGroupJoinType > 0)
                                            {
                                                tgConjunctionHTML.AppendFormat(@"<p>{0}</p>", retrieveConjunctionString((BDConstants.BDJoinType)therapyGroup.therapyGroupJoinType));
                                            }
                                        }

                                        break;
                                }
                            }
                            if (!string.IsNullOrEmpty(pathogenHtml.ToString()))
                            {
                                if (pathogenHasAttachedNotes)  // no further tags added
                                    html.Append(pathogenHtml);
                                else
                                    html.AppendFormat(@"<p>{0}</p>", pathogenHtml);
                            }
                            if (!string.IsNullOrEmpty(therapyGroupHtml.ToString()))
                                html.Append(therapyGroupHtml);
                        }
                        break;

                    default:
                        bool childrenHavePathogens = false;
                        foreach (IBDNode child in children)
                        {
                            if (child.NodeType == BDConstants.BDNodeType.BDPathogen)
                            {
                                childrenHavePathogens = true;
                                break;
                            }
                        }
                        if (pHasUsualPathogenTitle && childrenHavePathogens)
                        {
                            string title = "Usual Pathogens";
                            if (pNode.ParentId == Guid.Parse("54f2fcf0-8cbb-42d0-b61e-494838b1920e")) title = "Potential Pathogens";
                            html.Append(string.Format(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title));
                        }
                        else if (pHasUsualPathogenTitle && pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis)
                        {
                            // in the specific case of this layout, the title is required even if there are no pathogens.  <ld - Jan 2013>
                            string title = "Usual Pathogens";
                            html.Append(string.Format(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title));
                        }
                        // separate the pathogen groups' data with a line
                        if (pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis)
                        {
                            html.Append(horizontalRuleTag);
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel - 1), pFootnotes, pObjectsOnPage));
                        }
                        else
                            // describe the pathogen group
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));

                        StringBuilder conjunctionHTML = new StringBuilder();
                        BDTherapyGroup owner = null;
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogen:
                                    pathogenHtml.AppendFormat(@"{0}<br />", (buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage)));
                                    break;
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                    if (null != therapyGroup)
                                    {
                                        int level = pLevel + 2;
                                        if (child.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic)
                                            level = pLevel + 1;
                                       StringBuilder tgHTML = (BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, level, null));
                                       therapyGroupHtml.AppendFormat(@"{0}{1}", conjunctionHTML, tgHTML);

                                        conjunctionHTML.Clear();
                                        if (therapyGroup.therapyGroupJoinType > 0)
                                        {
                                            conjunctionHTML.AppendFormat(@"<p>{0}</p>", retrieveConjunctionString((BDConstants.BDJoinType)therapyGroup.therapyGroupJoinType));
                                            owner = therapyGroup;
                                        }
                                    }
                                    break;
                            }
                        }
                        if (!string.IsNullOrEmpty(pathogenHtml.ToString()))
                            html.AppendFormat(@"<p>{0}</p>", pathogenHtml);
                        if (!string.IsNullOrEmpty(therapyGroupHtml.ToString()))
                            html.Append(therapyGroupHtml);
                        break;
                }
                List<Guid> lgndAssociations;
                List<BDLinkedNote> lgndNotes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.ParentId.Value, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend, out lgndAssociations);
                string lgndHtml = buildTextFromNotes(lgndNotes);
                if (!string.IsNullOrEmpty(lgndHtml))
                {
                    html.Append(lgndHtml);
                    pObjectsOnPage.AddRange(lgndAssociations);
                }
            }
            return html.ToString();
        }

        public string BuildBDPathogenResistanceHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            bool isFirstChild = true;

            if (null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDPathogenResistance)
            {
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis: // [108]
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep: // [1082]
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        // custom-built - Therapy has 2 durations and a custom header

                        string therapyNameTitleHtml = string.Empty;
                        string therapyDosage1TitleHtml = string.Empty;
                        string therapyDurationSpanTitleHtml = string.Empty;
                        string therapyDuration1TitleHtml = string.Empty;
                        string therapyDuration2TitleHtml = string.Empty;

                        List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

                        if (metadataLayoutColumns.Count > 0)
                            therapyNameTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                        if (metadataLayoutColumns.Count > 1)
                            therapyDosage1TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                        if (metadataLayoutColumns.Count > 2)
                            therapyDurationSpanTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDMetaDecoration, pFootnotes, pObjectsOnPage);
                        if (metadataLayoutColumns.Count > 3)
                            therapyDuration1TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, pFootnotes, pObjectsOnPage);
                        if (metadataLayoutColumns.Count > 4)
                            therapyDuration2TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[4], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION_1, pFootnotes, pObjectsOnPage);

                        // all the information for one pathogen resistance is in the same table, so header is built outside the loops for therapy group and therapy

                        resetGlobalVariablesForTherapies();
                        StringBuilder endoTherapyHTML = new StringBuilder();

                        foreach (BDTherapyGroup endoTherapyGroup in children)
                        {
                            endoTherapyHTML.Append(BuildBDTherapyGroupHTML(pContext, endoTherapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                        }
                        // build table header
                        html.AppendFormat(@"<table class=""v{0}"">", (int)pNode.LayoutVariant);
                        html.AppendFormat(@"<tr><th>{0}</th>", therapyNameTitleHtml);

                        if (therapiesHaveDosage)
                            html.AppendFormat(@"<th>{0}</th>", therapyDosage1TitleHtml);
                        else
                            html.Append(@"<th></th>");

                        if (therapiesHaveDuration)
                            html.AppendFormat(@"<th colspan=""2"">{0}</th>", therapyDurationSpanTitleHtml);
                        else
                            html.Append(@"<th></th><th></th>");

                        html.AppendFormat(@"</tr><tr><th class=""inner""/><th class=""inner"" /><th class=""inner"">{0}</th><th class=""inner"">{1}</th></tr>", therapyDuration1TitleHtml, therapyDuration2TitleHtml);

                        // append child html
                        html.Append(endoTherapyHTML);
                        html.Append(@"</table>");
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration: // [1081]
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        // custom-built: Therapy groups all appear within the same table - 3 column table

                        string t1therapyNameTitleHtml = string.Empty;
                        string t1therapyDosage1TitleHtml = string.Empty;
                        string t1therapyDurationTitleHtml = string.Empty;

                        List<BDLayoutMetadataColumn> t1metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis);

                        if (t1metadataLayoutColumns.Count > 0)
                            t1therapyNameTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, t1metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                        if (t1metadataLayoutColumns.Count > 1)
                            t1therapyDosage1TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, t1metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                        if (t1metadataLayoutColumns.Count > 2)
                            t1therapyDurationTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, t1metadataLayoutColumns[2], BDConstants.BDNodeType.BDMetaDecoration, pFootnotes, pObjectsOnPage);

                        resetGlobalVariablesForTherapies();
                        StringBuilder therapyHTML = new StringBuilder();

                        foreach (BDTherapyGroup tGroup in children)
                            therapyHTML.Append(BuildBDTherapyGroupHTML(pContext, tGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));


                        // build table header
                        html.AppendFormat(@"<table class=""v{0}"">", (int)pNode.LayoutVariant);
                        html.AppendFormat(@"<tr><th>{0}</th>", t1therapyNameTitleHtml); 
                        if (therapiesHaveDosage)
                        {
                            html.AppendFormat(@"<th>{0}</th>", t1therapyDosage1TitleHtml);
                        }
                        else
                            html.Append(@"<th></th>");
                        if (therapiesHaveDuration)
                            html.AppendFormat(@"<th>{0}</th>", t1therapyDurationTitleHtml);
                        else
                            html.Append(@"<th></th>");

                        html.AppendFormat(@"</tr>{0}",therapyHTML);
                        html.Append(@"</table>");
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDPresentationHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // gated on child node type: very few variations and no identified customizations yet
            StringBuilder html = new StringBuilder();

            BDNode presentation = pNode as BDNode;
            if ((null != presentation) && (presentation.NodeType == BDConstants.BDNodeType.BDPresentation))
            {
                bool isFirstChild = true;
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, presentation, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, presentation);

                switch (presentation.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                        //PathogenGroup
                        //Topic
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogenGroup:
                                    html.Append(BuildBDPathogenGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                                    break;
                                case BDConstants.BDNodeType.BDTopic:
                                    html.Append(BuildBDTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                            }
                            isFirstChild = false;
                        }

                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                        //PathogenGroup
                        //Response

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogenGroup:
                                    html.Append(BuildBDPathogenGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                                    break;
                                case BDConstants.BDNodeType.BDResponse:
                                    html.Append(BuildBDResponseHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                            }
                            isFirstChild = false;
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                        //PathogenGroup
                        //Table
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogenGroup:
                                    html.Append(BuildBDPathogenGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                                    break;
                                case BDConstants.BDNodeType.BDTable:
                                    html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                            }
                            isFirstChild = false;
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        //PathogenGroup
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogenGroup:
                                    html.Append(BuildBDPathogenGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                                    break;
                            }
                            isFirstChild = false;
                        }
                        break;
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        //TherapyGroup
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                    html.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                                    break;
                            }
                            isFirstChild = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        /// <summary>
        /// Build reference HTML for a therapyGroup. Renders the therapy Group and its child therapies
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pTherapyGroup"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <param name="pLevel"></param>
        /// <param name="pLayoutOverride">LayoutVariant to override pTherapyGroup.LayoutVariant. Use null or Undefined otherwise</param>
        /// <returns></returns>
        public StringBuilder BuildBDTherapyGroupHTML(Entities pContext, BDTherapyGroup pTherapyGroup, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, BDConstants.LayoutVariantType? pLayoutOverride)
        {
            // Exception occurs in BuildBDPathogenResistanceHtml, BuildBDPathogenGroupHtml

            StringBuilder therapyGroupHtml = new StringBuilder();

            // represent TherapyGroup name wrapped in different tags for use in rows vs. headers
            string rowTGNameHtml = buildNodeWithReferenceAndOverviewHTML(pContext, pTherapyGroup, "u", pFootnotes, pObjectsOnPage);
            string headerTGNameHtml = buildNodeWithReferenceAndOverviewHTML(pContext, pTherapyGroup, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage);

            string therapyNameTitleHtml = string.Empty;
            string therapyDosageTitleHtml = string.Empty;
            string therapyDurationTitleHtml = string.Empty;
            string therapyDosage1TitleHtml = string.Empty;
            string therapyDosage2TitleHtml = string.Empty;
            string therapyDuration1TitleHtml = string.Empty;
            string therapyDuration2TitleHtml = string.Empty;
            // Because this is used across many layout variants without rendering differences, allow for an override
            BDConstants.LayoutVariantType layoutLayoutVariant = (pLayoutOverride.HasValue && pLayoutOverride != BDConstants.LayoutVariantType.Undefined) ? pLayoutOverride.Value : pTherapyGroup.LayoutVariant;

            //List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, layoutLayoutVariant, BDConstants.BDNodeType.BDTherapy);
            BDLayoutMetadataColumn nameColumn = BDLayoutMetadataColumn.Retrieve(pContext, pTherapyGroup.LayoutVariant, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY);
            if (null != nameColumn)
                therapyNameTitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, nameColumn, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);

            if (BDFabrik.TherapyLayoutHasFirstDosage(pTherapyGroup.LayoutVariant))
            {
                BDLayoutMetadataColumn dosageColumn = BDLayoutMetadataColumn.Retrieve(pContext, pTherapyGroup.LayoutVariant, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE);
                if (null != dosageColumn)
                    therapyDosageTitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, dosageColumn, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
            }

            if (BDFabrik.TherapyLayoutHasFirstDuration(pTherapyGroup.LayoutVariant))
            {
                BDLayoutMetadataColumn durationColumn = BDLayoutMetadataColumn.Retrieve(pContext, pTherapyGroup.LayoutVariant, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION);
                if (null != durationColumn)
                    therapyDurationTitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, durationColumn, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, pFootnotes, pObjectsOnPage);
            }

            if (BDFabrik.TherapyLayoutHasSecondDosage(pTherapyGroup.LayoutVariant))
            {
                BDLayoutMetadataColumn dosage1Column = BDLayoutMetadataColumn.Retrieve(pContext, pTherapyGroup.LayoutVariant, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1);
                if (null != dosage1Column)
                    therapyDosage1TitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, dosage1Column, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);
            }

            if (BDFabrik.TherapyLayoutHasSecondDuration(pTherapyGroup.LayoutVariant))
            {
                BDLayoutMetadataColumn duration1Column = BDLayoutMetadataColumn.Retrieve(pContext, pTherapyGroup.LayoutVariant, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION_1);
                if (null != duration1Column)
                    therapyDuration1TitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, duration1Column, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION_1, pFootnotes, pObjectsOnPage);
            }

            if (BDFabrik.TherapyLayoutHasThirdDosage(pTherapyGroup.LayoutVariant))
            {
                BDLayoutMetadataColumn dosage2Column = BDLayoutMetadataColumn.Retrieve(pContext, pTherapyGroup.LayoutVariant, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_2);
                if (null != dosage2Column)
                    therapyDosage2TitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, dosage2Column, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_2, pFootnotes, pObjectsOnPage);
            }

            if (BDFabrik.TherapyLayoutHasThirdDuration(pTherapyGroup.LayoutVariant))
            {
                BDLayoutMetadataColumn duration2Column = BDLayoutMetadataColumn.Retrieve(pContext, pTherapyGroup.LayoutVariant, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION_2);
                if (null != duration2Column)
                    therapyDuration2TitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, duration2Column, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION_2, pFootnotes, pObjectsOnPage);
            }

            List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, pTherapyGroup.Uuid);
            if (therapies.Count > 0)
            {
                StringBuilder therapyHTML = new StringBuilder();
                resetGlobalVariablesForTherapies();
                bool isTherapyInBrackets = false;
                string startHeaderCellTag = @"<th>";
                string endHeaderCellTag = @"</th>";
                string emptyHeaderCellTag = @"<th></th>";

                foreach (BDTherapy therapy in therapies)
                {
                    if (therapy.leftBracket.HasValue && therapy.leftBracket == true)
                        isTherapyInBrackets = true;

                    therapyHTML.Append(buildTherapyHtml(pContext, therapy, pFootnotes, pObjectsOnPage, isTherapyInBrackets));

                    if (therapy.rightBracket.HasValue && therapy.rightBracket.Value == true)
                        isTherapyInBrackets = false;

                    if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                        previousTherapyNameId = therapy.uuid;
                    if (!string.IsNullOrEmpty(therapy.dosage) && therapy.dosageSameAsPrevious == false)
                        previousTherapyDosageId = therapy.uuid;
                    if (!string.IsNullOrEmpty(therapy.dosage1) && therapy.dosage1SameAsPrevious == false)
                        previousTherapyDosage1Id = therapy.uuid;
                    if (!string.IsNullOrEmpty(therapy.dosage2) && therapy.dosage2SameAsPrevious == false)
                        previousTherapyDosage2Id = therapy.uuid;
                    if (!string.IsNullOrEmpty(therapy.duration) && therapy.durationSameAsPrevious == false)
                        previousTherapyDurationId = therapy.uuid;
                    if (!string.IsNullOrEmpty(therapy.duration1) && therapy.duration1SameAsPrevious == false)
                        previousTherapyDuration1Id = therapy.uuid;
                    if (!string.IsNullOrEmpty(therapy.duration2) && therapy.duration2SameAsPrevious == false)
                        previousTherapyDuration2Id = therapy.uuid;
                }

                // Add TherapyGroup and in some cases, HTML table 
                // some layouts have the TherapyGroup above the table, some have TherapyGroup in a table row.
                // where TherapyGroup is in a row, the table is being constructed by the caller to BuildBDTherapyGroup (this method)
                switch (pTherapyGroup.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        if (!string.IsNullOrEmpty(headerTGNameHtml))
                            therapyGroupHtml.Append(headerTGNameHtml);

                        therapyGroupHtml.AppendFormat(@"<table class=""v{0}"">", (int)pTherapyGroup.LayoutVariant);

                        // use default titles if layout configuration data is misssing
                        if (therapyNameTitleHtml.Length == 0) therapyNameTitleHtml = "Recommended Empiric Therapy";
                        if (therapyDosageTitleHtml.Length == 0) therapyDosageTitleHtml = "Recommended Dose";
                        if (therapyDurationTitleHtml.Length == 0) therapyDurationTitleHtml = "Recommended Duration";

                        therapyGroupHtml.AppendFormat(@"<tr><th>{0}</th>", therapyNameTitleHtml); 

                        if (therapiesHaveDosage)
                            therapyGroupHtml.AppendFormat(@"<th>{0}</th>", therapyDosageTitleHtml);
                        else
                            therapyGroupHtml.Append(@"<th></th>");

                        if (therapiesHaveDuration)
                            therapyGroupHtml.AppendFormat(@"<th>{0}</th>", therapyDurationTitleHtml);
                        else
                            therapyGroupHtml.Append(@"<th></th>");

                        therapyGroupHtml.Append(@"</tr>");
                        therapyGroupHtml.Append(therapyHTML);
                        therapyGroupHtml.Append(@"</table>");
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis: // [108]
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_ViridansStrep: // [1082]
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration: // [1081]
                        int columnCount = BDFabrik.GetTableColumnCount(pTherapyGroup.LayoutVariant);
                        if (!string.IsNullOrEmpty(rowTGNameHtml))
                        {  
                            therapyGroupHtml.AppendFormat(@"<tr><td>{0}</td><td /><td />", rowTGNameHtml);
                            // needs minimum 3, sometimes 4 columns in total
                            if (columnCount > 3)
                                therapyGroupHtml.Append(@"<td />");
                            therapyGroupHtml.Append(@"</tr>");
                        }
                        therapyGroupHtml.Append(therapyHTML);
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                    case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:  // 307
                        if (!string.IsNullOrEmpty(headerTGNameHtml))
                            therapyGroupHtml.Append(headerTGNameHtml);
                        therapyGroupHtml.AppendFormat(@"<table class=""v{0}"">", (int)pTherapyGroup.LayoutVariant);

                        therapyGroupHtml.AppendFormat(@"<tr><th>{0}</th><th>{1}</th></tr>", therapyNameTitleHtml, therapyDosageTitleHtml); 
                        therapyGroupHtml.Append(therapyHTML);
                        therapyGroupHtml.Append(@"</table>");
                        break;

                    case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                        if (!string.IsNullOrEmpty(rowTGNameHtml))
                            therapyGroupHtml.AppendFormat(@"<tr><td colspan=""2""><b>{0}</b></td></tr>", rowTGNameHtml);
                        therapyGroupHtml.Append(therapyHTML);
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:  // [304]
                        if (!string.IsNullOrEmpty(headerTGNameHtml))
                            therapyGroupHtml.Append(headerTGNameHtml);
                        therapyGroupHtml.AppendFormat(@"<table class=""v{0}"">", (int)pTherapyGroup.LayoutVariant);

                        // the column headers have not been added to the layout data so they are built manually here
                        string subtext = @"given 30-60 minutes before the procedure";
                        therapyGroupHtml.AppendFormat(@"<tr><th>{0}</th><th><b>{1}</b><br />{2}<b>/ROUTE</b></th><th><b>{3}</b><br />{4}<b>/ROUTE</b></th></tr>",
                            therapyNameTitleHtml, therapyDosageTitleHtml, subtext, therapyDosage1TitleHtml, subtext, (int)pTherapyGroup.LayoutVariant);
                        // therapyGroupHtml.Append(@"<tr><th class=""inner"" colspan=""3"">Dental, Oral, Respiratory Tract Procedures</th></tr>");  // removed October 2015 -- moved to Overview
                        therapyGroupHtml.Append(therapyHTML);
                        therapyGroupHtml.Append(@"</table>");
                        break;
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        if (!string.IsNullOrEmpty(rowTGNameHtml))
                            therapyGroupHtml.AppendFormat(@"<tr><td>{0}</td><td /><td /></tr>", rowTGNameHtml);
                        therapyGroupHtml.Append(therapyHTML);
                        break;
                    case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                        // therapy group is a row in the table
                        // could not use 'default' here due to lines that add the <th /> when dosage / duration are empty.
                        // cost of regression testing for the change is too high, even though that code looks like a bug.

                        if (!string.IsNullOrEmpty(rowTGNameHtml))
                            therapyGroupHtml.AppendFormat(@"<tr><td>{0}</td></tr>", rowTGNameHtml);
                        therapyGroupHtml.Append(therapyHTML);
                        break;                    
                    case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:  // 109
                    case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:     // 111
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:            // 401
                        string startHeaderCellTagWithWidth = string.Format( @"<th class=""v{0}"">", (int)pTherapyGroup.LayoutVariant);  // CSS th class to set width to 30%
                        if (!string.IsNullOrEmpty(headerTGNameHtml))
                            therapyGroupHtml.Append(headerTGNameHtml);

                        if (therapyNameTitleHtml.Length == 0) therapyNameTitleHtml = "Therapy";
                        if (therapyDosageTitleHtml.Length == 0) therapyDosageTitleHtml = "Dose";
                        if (therapyDurationTitleHtml.Length == 0) therapyDurationTitleHtml = "Duration";

                        //  when there is no header for the Dosage or the Duration column   
                        //  the Therapy and the Dosage column width is allowed to float 
                        //  and is not restricted to 30%
                        string activeCellTagForTherapy = therapiesHaveDosage ? startHeaderCellTagWithWidth : startHeaderCellTag;
                        string activeCellTagForDosage = therapiesHaveDuration ? startHeaderCellTagWithWidth : startHeaderCellTag;

                        therapyGroupHtml.AppendFormat(@"<table class=""v{0}"">", (int)pTherapyGroup.LayoutVariant);
                        therapyGroupHtml.AppendFormat(@"<tr>{0}{1}{2}", activeCellTagForTherapy, therapyNameTitleHtml, endHeaderCellTag);
                        
                        if (therapiesHaveDosage)
                            therapyGroupHtml.AppendFormat(@"{0}{1}{2}", activeCellTagForDosage, therapyDosageTitleHtml, endHeaderCellTag);
                        else
                            therapyGroupHtml.Append(emptyHeaderCellTag);

                        if (therapiesHaveDuration)
                            therapyGroupHtml.AppendFormat(@"<th>{0}</th>", therapyDurationTitleHtml);
                        else
                            therapyGroupHtml.Append(emptyHeaderCellTag);

                        therapyGroupHtml.Append(@"</tr>");
                        therapyGroupHtml.Append(therapyHTML);
                        therapyGroupHtml.Append(@"</table>");
                        break;
                    default:
                        if (!string.IsNullOrEmpty(headerTGNameHtml))
                            therapyGroupHtml.Append(headerTGNameHtml);

                        if (therapyNameTitleHtml.Length == 0) therapyNameTitleHtml = "Therapy";
                        if (therapyDosageTitleHtml.Length == 0) therapyDosageTitleHtml = "Dose";
                        if (therapyDurationTitleHtml.Length == 0) therapyDurationTitleHtml = "Duration";


                        therapyGroupHtml.AppendFormat(@"<table class=""v{0}"">", (int)pTherapyGroup.LayoutVariant);
                        therapyGroupHtml.AppendFormat(@"<tr>{0}{1}{2}", startHeaderCellTag, therapyNameTitleHtml, endHeaderCellTag);
                        if (therapiesHaveDosage)
                            therapyGroupHtml.AppendFormat(@"{0}{1}{2}", startHeaderCellTag, therapyDosageTitleHtml, endHeaderCellTag);
                        else
                            therapyGroupHtml.Append(emptyHeaderCellTag);

                        if (therapiesHaveDuration)
                            therapyGroupHtml.AppendFormat(@"<th>{0}</th>", therapyDurationTitleHtml);
                        else
                            therapyGroupHtml.Append(emptyHeaderCellTag);

                        therapyGroupHtml.Append(@"</tr>");
                        therapyGroupHtml.Append(therapyHTML);
                        therapyGroupHtml.Append(@"</table>");
                        break;
                }
            }
            else
                therapyGroupHtml.Append(headerTGNameHtml);

            return therapyGroupHtml;
        }

        #region obsolete method
        [Obsolete("Not in use",true)]
        public string BuildBDRegimenGroupHTML(Entities pContext, BDRegimenGroup pRegimenGroup, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            Guid previousRegimenNameUuid = Guid.Empty;
            Guid previousRegimenDosage1Uuid = Guid.Empty;
            Guid previousRegimenDuration1Uuid = Guid.Empty;

            if (null != pRegimenGroup)
            {
                string regimenGroupHtml = buildNodeWithReferenceAndOverviewHTML(pContext, pRegimenGroup, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage);

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pRegimenGroup);
                StringBuilder firstColumnHtml = new StringBuilder();
                StringBuilder secondColumnHtml = new StringBuilder();

                if (children.Count > 0)
                {
                    firstColumnHtml.Append(@"<td>");
                    secondColumnHtml.Append(@"<td>");
                    foreach (BDRegimen regimen in children)
                    {
                        if (regimen.columnOrder == 0)
                        {
                            firstColumnHtml.Append(buildNodePropertyHTML(pContext, regimen, regimen.Uuid, regimen.name, BDRegimen.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                        }
                        else if (regimen.columnOrder == 1)
                        {
                        }
                    }
                    firstColumnHtml.Append(@"</td>");
                    secondColumnHtml.Append(@"</td>");
                }
                html.AppendFormat(@"<tr>{0}{1}</tr>", firstColumnHtml, secondColumnHtml);
            }
            return html.ToString();
        }
        #endregion

        public string BuildBDResponseHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // gated on layout variant

            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDResponse))
            {
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                bool isFirstChild = true;

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:

                        foreach (IBDNode frequency in children)
                        {
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, frequency as BDNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));
                            List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, frequency); //assumes pathogenGroups are being returned (as defined in BDFabrik)
                            foreach (IBDNode pathogenGroup in pathogenGroups)
                            {
                                html.Append(BuildBDPathogenGroupHtml(pContext, pathogenGroup as BDNode, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                                isFirstChild = false;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDSectionProphylaxisInfectionPreventionHtmlAndPages(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();

            //            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen }));
            // Section > Microorganism Group > Microorganism

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSection))
            {
                //List<string> mPageTitles = new List<string>();
                List<IBDNode> mGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
                StringBuilder mgHTML = new StringBuilder();
                string previousGroupName = string.Empty;

                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
                List<string> columnHtml = new List<string>();
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDPrecaution, BDPrecaution.PROPERTYNAME_INFECTIVEMATERIAL, pFootnotes, pObjectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDPrecaution, BDPrecaution.PROPERTYNAME_MODEOFTRANSMISSION, pFootnotes, pObjectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDPrecaution, BDPrecaution.PROPERTYNAME_SINGLEROOMACUTE, pFootnotes, pObjectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[4], BDConstants.BDNodeType.BDPrecaution, BDPrecaution.PROPERTYNAME_GLOVESACUTE, pFootnotes, pObjectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[5], BDConstants.BDNodeType.BDPrecaution, BDPrecaution.PROPERTYNAME_GOWNSACUTE, pFootnotes, pObjectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[6], BDConstants.BDNodeType.BDPrecaution, BDPrecaution.PROPERTYNAME_MASKACUTE, pFootnotes, pObjectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[7], BDConstants.BDNodeType.BDPrecaution, BDPrecaution.PROPERTYNAME_DURATION, pFootnotes, pObjectsOnPage));

                foreach (IBDNode mGroup in mGroups)
                {
                    if (mGroup.Name != previousGroupName)
                    {
                        mgHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, mGroup as BDNode, HtmlHeaderTagLevelString(pLevel + 2), pFootnotes, pObjectsOnPage));
                        previousGroupName = mGroup.Name;
                    }
                    List<BDHtmlPage> mPages = new List<BDHtmlPage>();
                    List<IBDNode> microorganisms = BDFabrik.GetChildrenForParent(pContext, mGroup);
                    foreach (IBDNode microorganism in microorganisms)
                    {

                        StringBuilder mHTML = new StringBuilder();
                        List<Guid> mObjectsOnPage = new List<Guid>();
                        List<BDLinkedNote> mFootnotes = new List<BDLinkedNote>();
                        mHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, microorganism as BDNode, HtmlHeaderTagLevelString(pLevel + 2), mFootnotes, mObjectsOnPage));
                        string precautionTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDPrecaution, BDPrecaution.PROPERTYNAME_ORGANISM_1, mFootnotes, mObjectsOnPage);

                        List<IBDNode> precautions = BDFabrik.GetChildrenForParent(pContext, microorganism);
                        foreach (IBDNode precaution in precautions)
                        {
                            mObjectsOnPage.Add(precaution.Uuid);
                            BDPrecaution p = precaution as BDPrecaution;
                            mHTML.AppendFormat(@"<{0}>{1}</{0}><p>{2}</p>", HtmlHeaderTagLevelString(pLevel + 3), columnHtml[0], p.infectiveMaterial);
                            mHTML.AppendFormat(@"<{0}>{1}</{0}><p>{2}</p>", HtmlHeaderTagLevelString(pLevel + 3), columnHtml[1], p.modeOfTransmission);
                            // build table
                            mHTML.AppendFormat(@"<table class=""v{0}""><tr><th>{1}</th><th class=""v{0}"">Acute Care</th><th class=""v{0}"">Long Term Care</th></tr>", (int)precaution.LayoutVariant, precautionTitle);
                            mHTML.AppendFormat(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", columnHtml[2], p.singleRoomAcute, p.singleRoomLongTerm);
                            mHTML.AppendFormat(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", columnHtml[3], p.glovesAcute, p.glovesLongTerm);
                            mHTML.AppendFormat(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", columnHtml[4], p.gownsAcute, p.gownsLongTerm);
                            mHTML.AppendFormat(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", columnHtml[5], p.maskAcute, p.maskLongTerm);
                            mHTML.Append(@"</table>");

                            List<Guid> durationAssociations;
                            List<BDLinkedNote> durationNotes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, p.Uuid, BDPrecaution.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment, out durationAssociations);
                            StringBuilder durationText = new StringBuilder();
                            foreach (BDLinkedNote note in durationNotes)
                            {
                                durationText.Append(note.documentText);
                            }
                            if (durationText.Length > 0)
                                mObjectsOnPage.AddRange(durationAssociations);
                            mHTML.AppendFormat(@"<{0}>{1}</{0}>{2}", HtmlHeaderTagLevelString(pLevel + 3), columnHtml[6], durationText);
                        }
                        currentPageMasterObject = microorganism;
                        mPages.Add(writeBDHtmlPage(pContext, microorganism, mHTML, BDConstants.BDHtmlPageType.Data, mFootnotes, mObjectsOnPage, null));
                    }
                    mgHTML.Append(navListDivPrefix);

                    if (sortData) mPages.Sort();

                    for (int i = 0; i < mPages.Count; i++)
                        mgHTML.AppendFormat(navListAnchorTag, mPages[i].Uuid.ToString().ToUpper(), mPages[i].pageTitle);
                    mgHTML.Append(navListDivSuffix);
                }
                html.Append(mgHTML);
                currentPageMasterObject = pNode;
            }

            return html.ToString();
        }

        public string BuildBDSurgeryHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            StringBuilder suffixHtml = new StringBuilder();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSurgery))
            {
                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> surgeryChildren = BDFabrik.GetChildrenForParent(pContext, pNode);

                switch (pNode.LayoutVariant)
                {
                    #region Dental
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        foreach (IBDNode surgeryClassification in surgeryChildren) // SurgeryClassification
                        {
                            if (surgeryClassification.Name.Length > 0 && !surgeryClassification.Name.Contains(string.Format("New {0}", BDUtilities.GetEnumDescription(surgeryClassification.NodeType))))
                                html.AppendFormat(@"<ul><li>{0}</ul>", surgeryClassification.Name);
                            pObjectsOnPage.Add(surgeryClassification.Uuid);

                            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                            string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);

                            //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));

                            List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, surgeryClassification);
                            if (therapyGroups.Count > 0)
                            {
                                StringBuilder adultDosageHTML = new StringBuilder();
                                StringBuilder pedsDosageHTML = new StringBuilder();
                                html.AppendFormat(@"<table class=""v{0}"">", surgeryClassification.LayoutVariant);
                                html.AppendFormat(@"<tr><th>{0}</th><th>{1}</th></tr>", c2Html, c3Html);
                                foreach (IBDNode therapyGroup in therapyGroups)
                                {
                                    adultDosageHTML.Clear();
                                    pedsDosageHTML.Clear();
                                    StringBuilder therapyGroupHtml = new StringBuilder();
                                    therapyGroupHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, therapyGroup, "u", pFootnotes, pObjectsOnPage));

                                    if (therapyGroup.Name.Length > 0 && !therapyGroup.Name.Contains("New Therapy Group"))
                                    {
                                        adultDosageHTML.AppendFormat(@"{0}<ul>", therapyGroupHtml);
                                        pedsDosageHTML.AppendFormat(@"{0}<ul>", therapyGroupHtml);
                                    }
                                    else
                                    {
                                        adultDosageHTML.Append(@"<ul>");
                                        pedsDosageHTML.Append(@"<ul>");
                                    }
                                    pObjectsOnPage.Add(therapyGroup.Uuid);
                                    List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, therapyGroup);
                                    #region process therapies
                                    resetGlobalVariablesForTherapies();
                                    foreach (IBDNode t in therapies)
                                    {
                                        BDTherapy therapy = t as BDTherapy;
                                        string adultDosageString = string.Empty;
                                        string pedsDosageString = string.Empty;
                                        // Dosage - adult dose
                                        if (therapy.dosageSameAsPrevious.Value == true && previousTherapyDosageId != Guid.Empty)
                                            adultDosageString = buildNodePropertyHTML(pContext, therapy, previousTherapyDosageId,
                                                BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyDosageId).dosage,
                                                BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                                        else
                                            adultDosageString = buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);

                                        // Dosage 1 - Paediatric dose
                                        if (therapy.dosage1SameAsPrevious.Value == true)
                                            pedsDosageString = buildNodePropertyHTML(pContext, therapy, previousTherapyDosage1Id,
                                                BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyDosage1Id).dosage1,
                                                BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);
                                        else
                                            pedsDosageString = buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);

                                        // therapy name - add to both cells
                                        if (therapy.nameSameAsPrevious.Value == true && previousTherapyNameId != Guid.Empty)
                                        {
                                            if (!string.IsNullOrEmpty(adultDosageString))
                                                adultDosageHTML.AppendFormat(@"<li>{0} {1}", buildNodePropertyHTML(pContext, therapy, previousTherapyNameId,
                                                    BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyNameId).Name,
                                                    BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage), adultDosageString);

                                            if (!string.IsNullOrEmpty(pedsDosageString))
                                                pedsDosageHTML.AppendFormat(@"<li>{0} {1}", buildNodePropertyHTML(pContext, therapy, previousTherapyNameId,
                                                    BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyNameId).Name,
                                                    BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage), pedsDosageString);
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(adultDosageString))
                                                adultDosageHTML.AppendFormat(@"<li>{0} {1}", buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage), adultDosageString);
                                            if (!string.IsNullOrEmpty(pedsDosageString))
                                                pedsDosageHTML.AppendFormat(@"<li>{0} {1}", buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage), pedsDosageString);
                                        }
                                        // check for conjunctions and add a tableRows for any that are found
                                        switch (therapy.therapyJoinType)
                                        {
                                            case (int)BDConstants.BDJoinType.OrWithNext:
                                                adultDosageHTML.AppendFormat(@"<br />{0}<br /> ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                                pedsDosageHTML.AppendFormat(@"<br />{0}<br />", retrieveConjunctionString((int)therapy.therapyJoinType));
                                                break;
                                            default:
                                                adultDosageHTML.AppendFormat(@" {0} ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                                pedsDosageHTML.AppendFormat(@" {0} ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                                break;
                                        }

                                        if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                            previousTherapyNameId = therapy.uuid;
                                        if (!string.IsNullOrEmpty(therapy.dosage) && therapy.dosageSameAsPrevious == false)
                                            previousTherapyDosageId = therapy.uuid;
                                        if (!string.IsNullOrEmpty(therapy.dosage1) && therapy.dosage1SameAsPrevious == false)
                                            previousTherapyDosage1Id = therapy.uuid;
                                        if (!string.IsNullOrEmpty(therapy.dosage2) && therapy.dosage2SameAsPrevious == false)
                                            previousTherapyDosage2Id = therapy.uuid;
                                        if (!string.IsNullOrEmpty(therapy.duration) && therapy.durationSameAsPrevious == false)
                                            previousTherapyDurationId = therapy.uuid;
                                        if (!string.IsNullOrEmpty(therapy.duration1) && therapy.duration1SameAsPrevious == false)
                                            previousTherapyDuration1Id = therapy.uuid;
                                        if (!string.IsNullOrEmpty(therapy.duration2) && therapy.duration2SameAsPrevious == false)
                                            previousTherapyDuration2Id = therapy.uuid;
                                    }
                                    #endregion
                                    adultDosageHTML.Append(@"</ul>");
                                    pedsDosageHTML.Append(@"</ul>");
                                    html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>", adultDosageHTML, pedsDosageHTML);
                                }

                                html.Append(@"</table>");
                            }
                        }
                        break;
                    #endregion

                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery:
                        html.Append(buildRegimenHtml(pContext, pNode, pLevel, pFootnotes, pObjectsOnPage));
                        break; // page is created by caller
                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                        Debug.WriteLine("SURGERY CLASSIFICATION WHILE PROCESSING SURGERY");
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        /// <summary>
        /// Build an HTML page for the SurgeryClassification
        /// The caller will handle building the link and inserting the guid
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <param name="pLevel"></param>
        /// <returns>The generated HTML page</returns>
        public string BuildBDSurgeryClassificationHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSurgeryClassification))
            {
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                html.Append(buildRegimenHtml(pContext, pNode, pLevel, pFootnotes, pObjectsOnPage));
            }
            return html.ToString();
        }

        public string BuildBDSurgeryGroupHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSurgeryGroup))
            {
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                        {
                            StringBuilder pageTitleHtml = new StringBuilder();
                            StringBuilder surgeryHtml = new StringBuilder();

                            if (pNode.Name.Length > 0) // surgery group has a name
                                pageTitleHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                            List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);

                            foreach (IBDNode child in children)
                            {
                                if (child.NodeType == BDConstants.BDNodeType.BDSurgery)
                                {
                                    // add list of surgeries either as the title or a list that will follow the title
                                    if (surgeryHtml.Length > 0)
                                        surgeryHtml.Append(@"<br />");
                                    else
                                        surgeryHtml.Append(@"<p>");
                                    if (pNode.Name.Length > 0)
                                        surgeryHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "b", pFootnotes, pObjectsOnPage));
                                    else  // list of surgeries as the title on the page
                                        pageTitleHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                                }
                            }

                            html.Append(pageTitleHtml);

                            if (surgeryHtml.Length > 0)
                            {
                                surgeryHtml.Append("</p>");
                                html.Append(surgeryHtml);
                            }
                            html.Append(buildRegimenHtml(pContext, pNode, pLevel, pFootnotes, pObjectsOnPage));
                        }
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                        Debug.WriteLine("SURGERY WITH CLASSIFICATION PROCESSING WITH SURGERY GROUP");
                        if (pNode.Name.Length > 0) // surgery group has a name
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                        break;
                    default:
                        break;
                }
            }
            return html.ToString();
        }

        public string BuildBDSectionHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            StringBuilder prefixHtml = new StringBuilder();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSection))
            {
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                bool isFirstChild = true;

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                        foreach (IBDNode child in children)
                        {
                            string pageHtml = BuildBDCategoryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1);
                            html.Append(horizontalRuleTag);
                            html.Append(pageHtml);
                        }

                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                        // complex: builds multiple pages
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen }));
                        html.Append(BuildBDSectionProphylaxisInfectionPreventionHtmlAndPages(pContext, pNode, pFootnotes, pObjectsOnPage, pLevel));
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        html.AppendFormat(@"<{0}>Infectious Agent</{0}>", HtmlHeaderTagLevelString(pLevel + 1));
                        List<BDHtmlPage> generatedPages = new List<BDHtmlPage>();
                        foreach (IBDNode child in children)
                        {
                            List<BDHtmlPage> childPages = null;
                            html.Append(BuildBDPathogenHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 2, out childPages));
                            // Only one page should be generated
                            if (null != childPages) generatedPages.AddRange(childPages);
                        }

                        //Expects that children.count == generatedPages.count: May it blow up real gud if it isn't
                        html.Append(navListDivPrefix);

                        if (sortData) generatedPages.Sort();

                        for (int i = 0; i < generatedPages.Count; i++)
                            html.AppendFormat(navListAnchorTag, generatedPages[i].Uuid.ToString().ToUpper(), children[i].Name);
                        html.Append(navListDivSuffix);
                        break;
                    case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                        {

                            StringBuilder anchorTags = new StringBuilder();
                            StringBuilder subsectionHtml = new StringBuilder();
                            StringBuilder organismList = new StringBuilder();

                            // children as alphabet subsections
                            foreach (IBDNode subsection in children)
                            {
                                List<IBDNode> organisms = BDFabrik.GetChildrenForParent(pContext, subsection);

                                foreach (IBDNode organism in organisms)
                                {
                                    List<Guid> localObjectsOnPage = new List<Guid>();
                                    List<BDLinkedNote> localFootnotes = new List<BDLinkedNote>();
                                    StringBuilder localHtml = new StringBuilder();

                                    // generate the child page for the Organism
                                    localHtml.Append(BuildBDOrganismHtml(pContext, organism, localFootnotes, localObjectsOnPage, pLevel));
                                    currentPageMasterObject = organism;
                                    BDHtmlPage organismPage = (writeBDHtmlPage(pContext, organism, localHtml, BDConstants.BDHtmlPageType.Data, localFootnotes, localObjectsOnPage, null));

                                    string cleanChildName = organism.Name;
                                    if (BDUtilities.HasSymbols(cleanChildName))
                                        cleanChildName = BDUtilities.ProcessTextForSymbols(pContext, organism.Name);

                                    organismList.AppendFormat(navListAnchorTag, organismPage.Uuid.ToString().ToUpper(), cleanChildName);
                                }

                                // switch back
                                currentPageMasterObject = subsection;

                                subsectionHtml.AppendFormat(@"<h1>{0}</h1><h2>{1}</h2>{2}{3}{4}", pNode.Name, subsection.Name, navListDivPrefix, organismList, navListDivSuffix);

                                // create page for subsection
                                BDHtmlPage subsectionPage = (writeBDHtmlPage(pContext, subsection, subsectionHtml, BDConstants.BDHtmlPageType.Navigation, new List<BDLinkedNote>(), new List<Guid>(), null));

                                // add a link on section page to the subsection page 
                                anchorTags.AppendFormat(@"<a class=""bb"" href=""{0}"">{1}</a>", subsectionPage.Uuid.ToString().ToUpper(), subsection.Name);

                                // clear organism list and add new item
                                organismList.Clear();
                                subsectionHtml.Clear();
                            }

                            html.Append(anchorTags);
                            currentPageMasterObject = pNode;
                        }
                        break;
                    default:
                        break;
                }
            }

            html.Insert(0, prefixHtml);
            return html.ToString();
        }

        public string BuildBDSubcategoryHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSubcategory))
            {
                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                bool isFirstChild = true;

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        if (children.Count > 0)
                        {
                            string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                            string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_COST, pFootnotes, pObjectsOnPage);

                            html.AppendFormat(@"<table class=""v{3}""><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr>", c1Html, c2Html, c3Html, (int)pNode.LayoutVariant);
                            foreach (IBDNode child in children)
                            {
                                html.Append(BuildBDAntimicrobialHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                            }

                            html.Append(@"</table>");
                        }

                        html.Append(BuildBDLegendHtml(pContext, pNode, pObjectsOnPage));

                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                        //PathogenGroup
                        //Topic
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogenGroup:
                                    html.Append(BuildBDPathogenGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                                    break;
                            }
                            isFirstChild = false;
                        }

                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
                        // BDTopic
                        // BDSubtopic
                        foreach (IBDNode child in children)
                        {
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));
                            List<IBDNode> subtopics = BDFabrik.GetChildrenForParent(pContext, child);
                            foreach (IBDNode subtopic in subtopics)
                                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subtopic, HtmlHeaderTagLevelString(pLevel + 2), pFootnotes, pObjectsOnPage));
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                        //BDTable
                        html.AppendFormat(@"<table class=""v{0}"">", (int)pNode.LayoutVariant);
                        StringBuilder legendHTML = new StringBuilder();
                        foreach (IBDNode child in children) //tables
                        {
                            pObjectsOnPage.Add(child.Uuid);

                            switch (child.LayoutVariant)
                            {
                                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                                    //BDTableSection

                                    List<IBDNode> tableSections = BDFabrik.GetChildrenForParent(pContext, child);
                                    foreach (IBDNode section in tableSections)
                                    {
                                        //BDTableRow
                                        List<IBDNode> tableRows = BDFabrik.GetChildrenForParent(pContext, section);
                                        if (tableRows.Count > 0)
                                        {
                                            html.Append(buildTableRowHtml(pContext, tableRows[0] as BDTableRow, true, false, pFootnotes, pObjectsOnPage));
                                            for (int i = 1; i < tableRows.Count; i++)
                                                html.Append(buildTableRowHtml(pContext, tableRows[i] as BDTableRow, false, false, pFootnotes, pObjectsOnPage));
                                        }
                                    }
                                    break;
                                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                                    //BDTableRow
                                    //BDTableSection

                                    List<IBDNode> tableEntries = BDFabrik.GetChildrenForParent(pContext, child);
                                    foreach (IBDNode entry in tableEntries)
                                    {
                                        switch (entry.NodeType)
                                        {
                                            case BDConstants.BDNodeType.BDTableSection:
                                                List<IBDNode> sectionRows = BDFabrik.GetChildrenForParent(pContext, entry);
                                                foreach (IBDNode sectionRow in sectionRows)
                                                    html.Append(buildTableRowHtml(pContext, sectionRow as BDTableRow, false, false, pFootnotes, pObjectsOnPage));
                                                break;
                                            case BDConstants.BDNodeType.BDTableRow:
                                                html.Append(buildTableRowHtml(pContext, entry as BDTableRow, false, false, pFootnotes, pObjectsOnPage));
                                                break;
                                        }
                                    }
                                    break;
                            }
                            legendHTML.Append(BuildBDLegendHtml(pContext, child, pObjectsOnPage));
                        }
                        html.Append(@"</table>");
                        html.Append(legendHTML);
                        break;
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgery, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDSurgeryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }

                        break;
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDMicroorganismGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDMicroorganismGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        // child is BDAntimicrobial: write a page for the child and add it as a link to this page
                        List<Guid> amObjects = new List<Guid>();
                        List<BDLinkedNote> amFootnotes = new List<BDLinkedNote>();
                        List<BDHtmlPage> amPages = new List<BDHtmlPage>();
                        foreach (IBDNode child in children)
                        {
                            if (!string.IsNullOrEmpty(child.Name))
                            {
                                // create a page and add to collection
                                string amHtml = BuildBDAntimicrobialHtml(pContext, child, amFootnotes, amObjects, pLevel);
                                currentPageMasterObject = child;
                                amPages.Add(writeBDHtmlPage(pContext, child, amHtml, BDConstants.BDHtmlPageType.Data, amFootnotes, amObjects, null));
                            }
                        }
                        html.Append(navListDivPrefix);

                        if (sortData) amPages.Sort();

                        for (int i = 0; i < amPages.Count; i++)
                        {
                            html.AppendFormat(navListAnchorTag, amPages[i].Uuid.ToString().ToUpper(), amPages[i].pageTitle);
                        }
                        html.Append(navListDivSuffix);
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        //BDAntimicrobial
                        //BDAntimicrobialGroup
                        // CUSTOM HANDLING:  child is BDAntimicrobialGroup: write a page for the child and add it as a link to this page

                        List<BDHtmlPage> childPages = new List<BDHtmlPage>();
                        foreach (IBDNode child in children)
                        {
                            List<Guid> childObjects = new List<Guid>();
                            List<BDLinkedNote> childFootnotes = new List<BDLinkedNote>();

                            string childName = child.Name;
                            string namePlaceholderText = string.Format(@"New {0}", BDUtilities.GetEnumDescription(child.NodeType));
                            if (childName.Contains(namePlaceholderText))
                                childName = string.Empty;

                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDAntimicrobial:
                                    string childHtml = BuildBDAntimicrobialHtml(pContext, child, childFootnotes, childObjects, pLevel);
                                    currentPageMasterObject = child;
                                    // create a page and add to collection : page has no children
                                    childPages.Add(writeBDHtmlPage(pContext, child, childHtml, BDConstants.BDHtmlPageType.Data, childFootnotes, childObjects, null));

                                    break;
                                case BDConstants.BDNodeType.BDAntimicrobialGroup:
                                    if (!string.IsNullOrEmpty(childName))
                                    {
                                        string agHtml = BuildBDAntimicrobialGroupHtmlAndPage(pContext, child, childFootnotes, childObjects, pLevel);
                                        currentPageMasterObject = child;
                                        // create a page and add to collection : page has children
                                        childPages.Add(writeBDHtmlPage(pContext, child, agHtml, BDConstants.BDHtmlPageType.Navigation, childFootnotes, childObjects, null));
                                    }
                                    else // antimicrobial group name is empty - move to next layer and build pages + links
                                    {
                                        List<IBDNode> grandchildren = BDFabrik.GetChildrenForParent(pContext, child);
                                        List<Guid> gcObjects = new List<Guid>();
                                        List<BDLinkedNote> gcFootnotes = new List<BDLinkedNote>();
                                        foreach (IBDNode gChild in grandchildren)
                                        {
                                            if (!string.IsNullOrEmpty(gChild.Name))
                                            {
                                                string gcHtml = BuildBDAntimicrobialHtml(pContext, gChild, gcFootnotes, gcObjects, pLevel);
                                                currentPageMasterObject = gChild;
                                                // create a page and add to collection:  page has no children
                                                childPages.Add(writeBDHtmlPage(pContext, gChild, gcHtml, BDConstants.BDHtmlPageType.Data, gcFootnotes, gcObjects, null));
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        html.Append(navListDivPrefix);

                        if (sortData) childPages.Sort();

                        for (int i = 0; i < childPages.Count; i++)
                        {
                            html.AppendFormat(navListAnchorTag, childPages[i].Uuid.ToString().ToUpper(), childPages[i].pageTitle);
                        }
                        html.Append(navListDivSuffix);
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDSubSectionHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSubsection))
            {
                pObjectsOnPage.Add(pNode.Uuid);

                // describe the topic
                //BDNode topic = pNode as BDNode;
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                bool isFirstChild = true;

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin }));
                        html.Append(navListDivPrefix);

                        if (sortData) children.Sort((a, b) => String.Compare(a.Name, b.Name));

                        IBDObject subsectionMasterObject = currentPageMasterObject;
                        foreach (IBDNode child in children)
                        {
                            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
                            List<Guid> objectsOnChildPage = new List<Guid>();

                            string childHtml = BuildBDTopicHtml(pContext, child, footnoteList, objectsOnChildPage, pLevel + 1);
                            currentPageMasterObject = child;
                            BDHtmlPage childPage = writeBDHtmlPage(pContext, child, childHtml, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnChildPage, null);
                            html.AppendFormat(navListAnchorTag, childPage.Uuid.ToString().ToUpper(), childPage.pageTitle);
                        }
                        currentPageMasterObject = subsectionMasterObject;
                        html.Append(navListDivSuffix);
                        break;
                    case BDConstants.LayoutVariantType.FrontMatter:
                    case BDConstants.LayoutVariantType.BackMatter:
                        foreach (IBDNode child in children)
                            html.Append(BuildBDTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDCategoryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDSubTopicHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSubtopic))
            {
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                bool isFirstChild = true;

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column, BDConstants.LayoutVariantType.Table_3_Column, BDConstants.LayoutVariantType.Table_4_Column, BDConstants.LayoutVariantType.Table_5_Column }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    //case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDPathogenGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                            isFirstChild = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDTableHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // Gates on LayoutVariant to facilitate customization

            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDTable))
            {
                // Describe the table
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant); // Here if needed

                bool isFirstChild = true;

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                    case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:

                        foreach (IBDNode child in children)
                        {
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));
                            List<IBDNode> subtopics = BDFabrik.GetChildrenForParent(pContext, child);
                            foreach (IBDNode subtopic in subtopics)
                                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subtopic as BDNode, HtmlHeaderTagLevelString(pLevel + 2), pFootnotes, pObjectsOnPage));
                        }

                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:

                        //List<IBDNode> conditions = BDFabrik.GetChildrenForParent(pContext, pNode);
                        if (children.Count > 0)
                        {
                            html.AppendFormat(@"<table class=""v{0}""><tr><th>Condition</th><th>Other Potential Pathogens</th></tr>", (int)pNode.LayoutVariant);
                            foreach (IBDNode child in children)
                            {
                                html.AppendFormat(@"<tr><td>{0}</td><td>", buildNodePropertyHTML(pContext, child, child.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                                //pObjectsOnPage.Add(child.Uuid);
                                List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, child);
                                foreach (IBDNode node in pathogens) // Assuming that these are pathogens...
                                {
                                    html.Append(buildNodePropertyHTML(pContext, node, node.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                                    //pObjectsOnPage.Add(node.Uuid);
                                    if (node != pathogens.Last<IBDNode>())
                                        html.Append(@"<br />");
                                }
                                html.Append(@"</td></tr>");
                            }
                            html.Append(@"</table>");
                        }
                        break;

                    // Common generic render
                    case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                    case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                    case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                    case BDConstants.LayoutVariantType.Table_2_Column:
                    case BDConstants.LayoutVariantType.Table_3_Column:
                    case BDConstants.LayoutVariantType.Table_4_Column:
                    case BDConstants.LayoutVariantType.Table_5_Column:
                        {
                            int columnCount = BDFabrik.GetTableColumnCount(pNode.LayoutVariant);
                            if (children.Count > 0)
                            {
                                html.AppendFormat(@"<table class=""v{0}"">", (int)pNode.LayoutVariant);
                                foreach (IBDNode child in children)
                                {
                                    switch (child.NodeType)
                                    {
                                        case BDConstants.BDNodeType.BDTableRow:
                                            BDTableRow tableRow = child as BDTableRow;
                                            if (tableRow != null)
                                                html.Append(buildTableRowHtml(pContext, tableRow, false, true, pFootnotes, pObjectsOnPage));
                                            break;
                                        case BDConstants.BDNodeType.BDTableSection:
                                            html.Append(BuildBDTableSectionHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, columnCount));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                html.Append(@"</table>");
                            }
                        }
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II:
                        string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                        string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, pFootnotes, pObjectsOnPage);
                        string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, pFootnotes, pObjectsOnPage);
                        string c4Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, pFootnotes, pObjectsOnPage);
                        string c5Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[4], BDConstants.BDNodeType.BDMetaDecoration, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);

                        List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
                        foreach (IBDNode entry in childNodes)
                        {
                            html.AppendFormat(@"<{0}>SOURCE:  {1}</{0}>", HtmlHeaderTagLevelString(pLevel + 3), retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Name_fieldNote", BDConfiguredEntry.PROPERTYNAME_NAME, pObjectsOnPage, true, pFootnotes));
                            html.AppendFormat(@"<{0}>{1} {2}</{0}>", HtmlHeaderTagLevelString(pLevel + 3), c5Html, c1Html);
                            html.AppendFormat(@"<table class=""v{3}""><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr>", c2Html, c3Html, c4Html, (int)entry.LayoutVariant);

                            string fieldNoteText1 = retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field01_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD01, pObjectsOnPage, true, pFootnotes);
                            string fieldNoteText2 = retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field02_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD02, pObjectsOnPage, true, pFootnotes);
                            string fieldNoteText3 = retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field03_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD03, pObjectsOnPage, true, pFootnotes);

                            List<Guid> f1Associations;
                            List<Guid> f2Associations;
                            List<Guid> f3Associations;
                            List<BDLinkedNote> fieldFootnoteList1 = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD01, BDConstants.LinkedNoteType.Footnote, out f1Associations);
                            List<BDLinkedNote> fieldFootnoteList2 = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD02, BDConstants.LinkedNoteType.Footnote, out f2Associations);
                            List<BDLinkedNote> fieldFootnoteList3 = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD03, BDConstants.LinkedNoteType.Footnote, out f3Associations);

                            string fieldNoteFootnote1 = buildFooterMarkerForList(fieldFootnoteList1, true, pFootnotes);
                            string fieldNoteFootnote2 = buildFooterMarkerForList(fieldFootnoteList2, true, pFootnotes);
                            string fieldNoteFootnote3 = buildFooterMarkerForList(fieldFootnoteList3, true, pFootnotes);

                            html.AppendFormat(@"<tr><td>{0}{1}</td><td>{2}{3}</td><td>{4}{5}</td></tr>",
                                fieldNoteText1,
                                fieldNoteFootnote1,
                                fieldNoteText2,
                                fieldNoteFootnote2,
                                fieldNoteText3,
                                fieldNoteFootnote3);

                            html.Append(@"</table>");
                            pObjectsOnPage.Add(entry.Uuid);
                            pObjectsOnPage.AddRange(f1Associations);
                            pObjectsOnPage.AddRange(f2Associations);
                            pObjectsOnPage.AddRange(f3Associations);
                        }
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk:
                    case BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading:
                    case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages:
                    case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        if (null != metadataLayoutColumns)
                        {
                            html.AppendFormat(@"<table class=""v{0}""><tr>", (int)pNode.LayoutVariant);
                            foreach (BDLayoutMetadataColumn metadataColumn in metadataLayoutColumns)
                                html.AppendFormat(@"<th>{0}</th>", buildHtmlForMetadataColumn(pContext, pNode, metadataColumn, BDConstants.BDNodeType.BDConfiguredEntry, pFootnotes, pObjectsOnPage));
                            html.Append(@"</tr>");

                            bool firstColumnEmphasized = (pNode.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk);
                            foreach (IBDNode child in children)
                            {
                                html.Append(BuildBDConfiguredEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, false, firstColumnEmphasized));
                            }
                            html.Append(@"</table>");
                        }
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                    case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative:
                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        if (null != metadataLayoutColumns)
                        {
                            html.AppendFormat(@"<table class=""v{0}""><tr>", (int)pNode.LayoutVariant);
                            foreach (BDLayoutMetadataColumn metadataColumn in metadataLayoutColumns)
                                html.AppendFormat(@"<th>{0}</th>", buildHtmlForMetadataColumn(pContext, pNode, metadataColumn, BDConstants.BDNodeType.BDConfiguredEntry, pFootnotes, pObjectsOnPage));
                            html.Append(@"</tr>");

                            bool firstColumnEmphasized = (pNode.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk);
                            foreach (IBDNode child in children)
                            {
                                html.Append(BuildBDConfiguredEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, false, firstColumnEmphasized));
                            }
                            html.Append(@"</table>");
                        }

                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                            if (null != therapyGroup)
                            {
                                html.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                            }
                        }
                        break;
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis:  // [401]
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        string nameTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                        string adultDosageTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                        string pedsDosageTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);
                        string headerCellStartWithClass = @"<th class=""v401"">";  // sets width of column to 30% in CSS

                        html.AppendFormat(@"<table class=""v{0}""><tr>{1}{2}</th><th>{3}</th><th>{4}</th></tr>", (int)pNode.LayoutVariant, headerCellStartWithClass, nameTitle, adultDosageTitle, pedsDosageTitle);
                        //BDHtmlPageGeneratorLogEntry.AppendToFile("BDHTMLPageReview.txt", string.Format("{0}\t{1}\tCOL WIDTH: Table header cells at 30 ", DateTime.Now, pNode.Uuid.ToString()));

                        foreach (IBDNode therapyGroup in children)
                            html.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup as BDTherapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));

                        html.Append(@"</table>");

                        string legendHTML = BuildBDLegendHtml(pContext, pNode, pObjectsOnPage);

                        if (!string.IsNullOrEmpty(legendHTML))
                            html.Append(legendHTML);
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive: //311
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae: //315
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis: //316
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));                        

                        // render the table entries
                        isFirstChild = true;
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDCombinedEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                            isFirstChild = false;
                        }
                        html.Append(@"</table>");
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza: //312
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_NoRenal }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_Renal }));

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDCombinedEntry:
                                    html.Append(BuildBDCombinedEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, true));
                                    html.Append(@"</table>");
                                    break;
                                case BDConstants.BDNodeType.BDTopic:
                                    html.Append(BuildBDTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                            }

                        }
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

        public string BuildBDTableSectionHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, int pColumnCount)
        {
            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDTableSection))
            {
                bool isFirstChild = true;

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);

                string nodeHtml = buildNodeWithReferenceAndOverviewHTML(pContext, pNode as BDNode, "b", pFootnotes, pObjectsOnPage);
                if (nodeHtml.Length > 0)
                    html.AppendFormat(@"<tr><td colspan=""{0}"">{1}</td></tr>", pColumnCount,nodeHtml);

                foreach (IBDNode child in children)
                {
                    switch (child.NodeType)
                    {
                        case BDConstants.BDNodeType.BDTableRow:
                            BDTableRow tableRow = child as BDTableRow;
                            html.Append(buildTableRowHtml(pContext, tableRow, false, true, pFootnotes, pObjectsOnPage));
                            break;

                        case BDConstants.BDNodeType.BDTableSubsection:
                            string subsectionHtml = buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, "u", pFootnotes, pObjectsOnPage);
                            if(subsectionHtml.Length > 0)
                                html.AppendFormat(@"<tr><td colspan=""{0}"">{1}</td></tr>", pColumnCount, subsectionHtml);
                            List<IBDNode> subSectionChildren = BDFabrik.GetChildrenForParent(pContext, child);
                            foreach (IBDNode subSectionChild in subSectionChildren)
                            {
                                BDTableRow subSectionRow = subSectionChild as BDTableRow;
                                if (subSectionRow != null)
                                    html.Append(buildTableRowHtml(pContext, subSectionRow, false, true, pFootnotes, pObjectsOnPage));
                            }
                            break;
                    }

                }
            }
            return html.ToString();
        }

        public string BuildBDTopicHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // Gates on LayoutVariant to facilitate customization: Topics are too generic to do otherwise 

            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDTopic))
            {
                // describe the topic
                //BDNode topic = pNode as BDNode;
                string topicHtml = buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage);
                if(topicHtml.Length > 0)
                    html.Append(topicHtml);

                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                bool isFirstChild = true;

                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column, BDConstants.LayoutVariantType.Table_3_Column, BDConstants.LayoutVariantType.Table_4_Column, BDConstants.LayoutVariantType.Table_5_Column }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                    case BDConstants.LayoutVariantType.Prophylaxis_Immunization:
                    case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                    case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column, BDConstants.LayoutVariantType.Table_3_Column, BDConstants.LayoutVariantType.Table_4_Column, BDConstants.LayoutVariantType.Table_5_Column }));
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDDiseaseHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                    case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column, BDConstants.LayoutVariantType.Table_3_Column, BDConstants.LayoutVariantType.Table_4_Column, BDConstants.LayoutVariantType.Table_5_Column }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDAttachment:
                                    html.Append(BuildBDAttachmentHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                case BDConstants.BDNodeType.BDSubtopic:
                                    html.Append(BuildBDSubTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                case BDConstants.BDNodeType.BDTable:
                                    html.Append(BuildBDTableHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                            }
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    //case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                    //    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                    //    foreach (IBDNode child in children)
                    //    {
                    //        html.Append(BuildBDCategoryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                    //    }
                    //    break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_Renal:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_Renal }));

                        // According to BDFabrik, this will only ever have configured entries as children
                        //setup for child configured entries

                        // Disease (Influenza) > Table (Recommended Amantadine Dosage) > Topic (Renal Impairment)(You are here) > Configured Entries (children)
                        // Assumes that the 'page' is at the disease level
                        // These should be BDTable and NOT be null

                        IBDNode pciarImmediateParent = BDFabrik.RetrieveNode(pContext, pNode.ParentType, pNode.ParentId);
                        // this should be BDDisease
                        IBDNode pciarPageDisplayParent = BDFabrik.RetrieveNode(pContext, pciarImmediateParent.ParentType, pciarImmediateParent.ParentId);

                        isFirstChild = true;
                        foreach (IBDNode child in children)
                        {
                            if (isFirstChild)
                            {
                                // Build the table header

                                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, child.LayoutVariant);

                                string c1Html = buildHtmlForMetadataColumn(pContext, pciarPageDisplayParent, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                                string c2Html = buildHtmlForMetadataColumn(pContext, pciarPageDisplayParent, metadataLayoutColumns[1], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, pFootnotes, pObjectsOnPage);
                                string c3Html = buildHtmlForMetadataColumn(pContext, pciarPageDisplayParent, metadataLayoutColumns[2], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, pFootnotes, pObjectsOnPage);
                                string c4Html = buildHtmlForMetadataColumn(pContext, pciarPageDisplayParent, metadataLayoutColumns[3], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, pFootnotes, pObjectsOnPage);

                                // handle configured entry for Amantadine with No renal impairment
                                //html.AppendFormat("</table><{0}>Renal Impairment</{0}><table>", HtmlHeaderTagLevelString(pLevel + 1));
                                html.AppendFormat(@"<table class=""v{0}"">", (int)child.LayoutVariant);
                                html.AppendFormat(@"<tr><th rowspan=""2"">{0}</th><th colspan=""2"">Dosage with capsules</th><th class=""inner"">Daily dosage with solution (10mg/mL)</th></tr>", metadataLayoutColumns[0]);
                                html.AppendFormat(@"<tr><th class=""inner"">{0}</th><th class=""inner"">{1}</th><th class=""inner"">{2}</th></tr>", c2Html, c3Html, c4Html);
                            }
                            isFirstChild = false;

                            html.Append(BuildBDConfiguredEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 2));
                        }
                        html.Append(@"</table>");

                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir: //3123
                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Creatinine, BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Weight }));
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Zanamivir: //3126
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Zanamivir }));
                        isFirstChild = true;

                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDCombinedEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                            isFirstChild = false;
                        }
                        html.Append(@"</table>");

                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_CNS_Meningitis_Table:
                        bool tableOpen = false;
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDConfiguredEntry:
                                    if (!tableOpen)
                                    {
                                        List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, child.LayoutVariant);
                                        html.AppendFormat(@"<table class=""v{0}""><tr>", (int)child.LayoutVariant);
                                        foreach (BDLayoutMetadataColumn metadataColumn in metadataLayoutColumns)
                                            html.AppendFormat(@"<th>{0}</th>", buildHtmlForMetadataColumn(pContext, pNode, metadataColumn, BDConstants.BDNodeType.BDConfiguredEntry, pFootnotes, pObjectsOnPage));
                                        html.Append(@"</tr>");
                                    }
                                    tableOpen = true;
                                    html.Append(BuildBDConfiguredEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, false, false));
                                    break;
                                case BDConstants.BDNodeType.BDSubtopic:
                                    if (tableOpen) html.Append(@"</table>");
                                    tableOpen = false;
                                    html.Append(BuildBDSubTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                            }
                        }
                        if (tableOpen) html.Append(@"</table>");
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDPathogenGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                            isFirstChild = false;
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgery, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogenGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDSubtopic:
                                    html.Append(BuildBDSubTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                case BDConstants.BDNodeType.BDPathogenGroup:
                                    html.Append(BuildBDPathogenGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, isFirstChild));
                                    isFirstChild = false;
                                    break;
                            }
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDSubTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;

                    case BDConstants.LayoutVariantType.FrontMatter:
                    default:
                        break;
                }

                StringBuilder endNotesHTML = new StringBuilder();

                List<Guid> noteAssociations;
                List<BDLinkedNote> endnotes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Endnote, out noteAssociations);
                if(endnotes.Count > 0)
                html.Append(buildTextFromNotes(endnotes));
            }

            return html.ToString();
        }

        #region build therapy rows in table
        private string buildTherapyHtml(Entities pContext, BDTherapy pTherapy, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, bool pIsInBracketGroup)
        {
            return buildTherapyHtml(pContext, pTherapy, pFootnotes, pObjectsOnPage, false, pIsInBracketGroup);
        }

        private string buildTherapyHtml(Entities pContext, BDTherapy pTherapy, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, bool pAddEndBracket, bool pIsInBracketGroup)
        {

            StringBuilder therapyHtml = new StringBuilder();
            string resolvedValue = null;
            StringBuilder conjunctionRowHtml = new StringBuilder();

            // check join type - build a conjunction row if join is not 'Next'
            if (pTherapy.therapyJoinType > (int)BDConstants.BDJoinType.Next)
            {
                if (pIsInBracketGroup && !(pTherapy.rightBracket.HasValue && pTherapy.rightBracket.Value == true))
                    conjunctionRowHtml.AppendFormat(@"<tr><td {0}>{1}</td>", CSS_INLINE_STYLE_INDENT_CELL, retrieveConjunctionString((int)pTherapy.therapyJoinType));
                else
                    conjunctionRowHtml.AppendFormat(@"<tr><td>{0}</td>", retrieveConjunctionString((int)pTherapy.therapyJoinType));
            }
            therapyHtml.Append(@"<tr>");

            //Build row for Therapy:  first cell begins
            if (pIsInBracketGroup && !(pTherapy.leftBracket.HasValue && pTherapy.leftBracket.Value == true))
                therapyHtml.AppendFormat(@"<td {0}>", CSS_INLINE_STYLE_INDENT_CELL);
            else
                therapyHtml.Append(@"<td>");  // no brackets so therapy name is not indented
            
            //  add therapy name, (with brackets if they exist), end cell
            switch (pTherapy.LayoutVariant)
            {
                case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                default:
                    BDTherapy therapyInRow = null;

                    if (pTherapy.nameSameAsPrevious.Value == true && previousTherapyNameId != Guid.Empty)
                        therapyInRow = BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyNameId);
                    else
                        therapyInRow = pTherapy;
                    
                    string therapyNameHtml = buildNodePropertyHTML(pContext, pTherapy, therapyInRow.Uuid, therapyInRow.Name,
                         BDTherapy.PROPERTYNAME_THERAPY, "strong", pFootnotes, pObjectsOnPage, out resolvedValue);

                    if (!string.IsNullOrEmpty(therapyNameHtml) && (therapyNameHtml.Contains(@"<p>") || therapyNameHtml.Contains(@"<p >")))
                    {
                        //BDHtmlPageGeneratorLogEntry.AppendToFile("BDHTMLPageReview.txt", string.Format("{0}\t{1}\tLINKED NOTE: remove paragraph tag ", DateTime.Now, pTherapy.Uuid.ToString()));

                        // Fix Alignment in cells:  if text contains paragraph tags, replace with simple line return ('<br />') 2016-09
                        string lineBreakTag = @"<br />";

                        therapyNameHtml = therapyNameHtml.Replace("<li><p>", "<li>");
                        therapyNameHtml = therapyNameHtml.Replace("<li><p >", "<li>");
                        therapyNameHtml = therapyNameHtml.Replace("<p>", lineBreakTag);
                        therapyNameHtml = therapyNameHtml.Replace("<p >", lineBreakTag);
                        therapyNameHtml = therapyNameHtml.Replace("</p></li>", "</li>");
                        therapyNameHtml = therapyNameHtml.Replace("</p>", string.Empty);

                        if (therapyNameHtml.StartsWith(lineBreakTag))
                        {
                            therapyNameHtml = therapyNameHtml.Substring(lineBreakTag.Length);
                        }

                    }
                    therapyHtml.Append(therapyNameHtml);

                    therapyHtml.Append(TAG_CELL_END);
                    break;
            }

            // Dosage
            if (BDFabrik.TherapyLayoutHasFirstDosage(pTherapy.LayoutVariant))
            {
                resolvedValue = null;
                if (pTherapy.dosageSameAsPrevious.Value == true && previousTherapyDosageId != Guid.Empty)
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyDosageId,
                        BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyDosageId).dosage,
                        BDTherapy.PROPERTYNAME_DOSAGE, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
                else
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

                if (null != resolvedValue) therapiesHaveDosage = true;
                    conjunctionRowHtml.Append(@"<td></td>");
            }

            // Dosage 1
            if (BDFabrik.TherapyLayoutHasSecondDosage(pTherapy.LayoutVariant))
            {
                if (pTherapy.dosage1SameAsPrevious.Value == true && previousTherapyDosage1Id != Guid.Empty)
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyDosage1Id,
                        BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyDosage1Id).dosage1,
                        BDTherapy.PROPERTYNAME_DOSAGE_1, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
                else
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

                if (null != resolvedValue) therapiesHaveDosage = true;
                conjunctionRowHtml.Append(@"<td></td>");
            }

            // Dosage 2
            if (BDFabrik.TherapyLayoutHasThirdDosage(pTherapy.LayoutVariant))
            {
                if (pTherapy.dosage2SameAsPrevious.Value == true && previousTherapyDosage2Id != Guid.Empty)
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyDosage2Id,
                        BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyDosage2Id).dosage2,
                        BDTherapy.PROPERTYNAME_DOSAGE_2, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
                else
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage2, BDTherapy.PROPERTYNAME_DOSAGE_2, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

                if (null != resolvedValue) therapiesHaveDosage = true;
                conjunctionRowHtml.Append(@"<td></td>");
            }

            // Duration
            if (BDFabrik.TherapyLayoutHasFirstDuration(pTherapy.LayoutVariant))
            {
                resolvedValue = null;
                string durationHtml = string.Empty;
                if (pTherapy.durationSameAsPrevious.Value == true && previousTherapyDurationId != Guid.Empty)
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyDurationId,
                        BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyDurationId).duration,
                        BDTherapy.PROPERTYNAME_DURATION, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
                else
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration, BDTherapy.PROPERTYNAME_DURATION, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

                if (null != resolvedValue) therapiesHaveDuration = true;
                conjunctionRowHtml.Append(@"<td></td>");
            }

            // Duration 1
            if (BDFabrik.TherapyLayoutHasSecondDuration(pTherapy.LayoutVariant))
            {
                resolvedValue = null;
                if (pTherapy.duration1SameAsPrevious.Value == true && previousTherapyDuration1Id != Guid.Empty)
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyDuration1Id,
                        BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyDuration1Id).duration1,
                        BDTherapy.PROPERTYNAME_DURATION_1, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
                else
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration1, BDTherapy.PROPERTYNAME_DURATION_1, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

                if (null != resolvedValue) therapiesHaveDuration = true;
                conjunctionRowHtml.Append(@"<td></td>");
            }

            // Duration 2
            if (BDFabrik.TherapyLayoutHasThirdDuration(pTherapy.LayoutVariant))
            {
                resolvedValue = null;
                if (pTherapy.duration2SameAsPrevious.Value == true && previousTherapyDuration2Id != Guid.Empty)
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyDuration2Id,
                        BDTherapy.RetrieveTherapyWithId(pContext, previousTherapyDuration2Id).duration2,
                        BDTherapy.PROPERTYNAME_DURATION_2, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
                else
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration2, BDTherapy.PROPERTYNAME_DURATION_2, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

                conjunctionRowHtml.Append(@"<td></td>");
                if (null != resolvedValue) therapiesHaveDuration = true;
            }

            therapyHtml.Append(@"</tr>");
            conjunctionRowHtml.Append(@"</tr>");

            if (pTherapy.therapyJoinType != (int)BDConstants.BDJoinType.Next && pTherapy.therapyJoinType != (int)BDConstants.BDJoinType.Other)
            {
                therapyHtml.Append(conjunctionRowHtml);
            }
            return therapyHtml.ToString();
        }

        #endregion

        private string buildCellHTML(Entities pContext, IBDNode pCellParentNode, string pPropertyName, string pPropertyValue, bool includeCellTags, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            string cellTag = includeCellTags == true ? "td" : string.Empty;

            return buildNodePropertyHTML(pContext, pCellParentNode, pCellParentNode.Uuid, pPropertyValue, pPropertyName, cellTag, pFootnotes, pObjectsOnPage);
        }

        private string buildDosageHTML(Entities pContext, IBDNode pNode, string pDosageGroupName, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            // Used only for Adult Dosing in Renal Impairment

            // NOTE:  no changes here for colspan removal. 

            BDDosage dosageNode = pNode as BDDosage;
            StringBuilder dosageHTML = new StringBuilder();
            string styleString = string.Empty;
            string colSpanTag = string.Empty;

            int colSpanLength = 1;
            int colSpanStart = 0;

            for (int idx = 4; idx > 0; idx--)
            {
                switch (idx)
                {
                    case 4:
                        if (dosageNode.dosage4SameAsPrevious)
                        {
                            colSpanStart = 3;
                            colSpanLength++;
                        }
                        break;
                    case 3:
                        if (dosageNode.dosage3SameAsPrevious)
                        {
                            colSpanStart = 2;
                            colSpanLength++;
                        }
                        break;
                    case 2:
                        if (dosageNode.dosage2SameAsPrevious)
                        {
                            colSpanStart = 1;
                            colSpanLength++;
                        }
                        break;
                }
            }

            string breakTag = @"<br />";
            colSpanTag = (colSpanLength > 1) ? string.Format(@" class=""v{1}cs inner"" colspan=""{0}""", colSpanLength, (int)dosageNode.LayoutVariant) : string.Empty;
            string colCenterTag = string.Format(@" class=""v{0}cs inner""", (int)dosageNode.LayoutVariant);

            string dosage1Html = buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage, BDDosage.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);

            string spanTag = (colSpanStart == 1) ? colSpanTag : colCenterTag;
            string emptyCellTags = string.Format(@"<td class=""v{0}""></td>", (int)dosageNode.LayoutVariant);

            if (string.IsNullOrEmpty(pDosageGroupName))
            {
                dosageHTML.AppendFormat(@"<td{0}>{1}</td>", spanTag, dosage1Html);
                breakTag = string.Empty;
            } 
            else
                dosageHTML.AppendFormat(@"<td{0}>{1}<br />{2}</td>", spanTag, pDosageGroupName, dosage1Html);

            string dxHtml;

            if (!dosageNode.dosage2SameAsPrevious)
            {
                spanTag = (colSpanStart == 2) ? colSpanTag : colCenterTag;

                dxHtml = buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage2, BDDosage.PROPERTYNAME_DOSAGE2, pFootnotes, pObjectsOnPage);
                if (!string.IsNullOrEmpty(dxHtml))
                    dosageHTML.AppendFormat(@"<td{0}>{1}{2}</td>", spanTag, breakTag, dxHtml);
                else
                    dosageHTML.Append(emptyCellTags);
            }

            if (!dosageNode.dosage3SameAsPrevious)
            {
                spanTag = (colSpanStart == 3) ? colSpanTag : colCenterTag;

                dxHtml = buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage3, BDDosage.PROPERTYNAME_DOSAGE3, pFootnotes, pObjectsOnPage);
                if (!string.IsNullOrEmpty(dxHtml))
                    dosageHTML.AppendFormat(@"<td{0}>{1}{2}</td>", spanTag, breakTag, dxHtml);
                else
                    dosageHTML.Append(emptyCellTags);
            }

            if (!dosageNode.dosage4SameAsPrevious)
            {
                spanTag = (colSpanStart == 4) ? colSpanTag : colCenterTag;

                dxHtml = buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage4, BDDosage.PROPERTYNAME_DOSAGE4, pFootnotes, pObjectsOnPage);
                if (!string.IsNullOrEmpty(dxHtml))
                    dosageHTML.AppendFormat(@"<td{0}>{1}{2}</td>", spanTag, breakTag, dxHtml);
                else
                    dosageHTML.Append(emptyCellTags);
            }
            return dosageHTML.ToString();
        }

        private string buildRegimenHtml(Entities pContext, IBDNode pNode, int pLevel, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder html = new StringBuilder();

            int surgicalProphylaxisLayout = (int)BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            string pathogenTitle = @"";
            string choiceRegimenTitle = @"";
            string alternateRegimenTitle = @"";
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            if (metadataLayoutColumns.Count > 1)
            {
                pathogenTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDMetaDecoration, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                choiceRegimenTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDRegimenGroup, BDRegimenGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                alternateRegimenTitle = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDRegimenGroup, BDRegimenGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
            }
            StringBuilder pathogenHtml = new StringBuilder();
            StringBuilder regimenHtml = new StringBuilder();

            bool hasPathogens = false;
            bool hasChoiceRegimens = false;
            bool hasAlternateRegimens = false;

            List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
            StringBuilder choiceRegimenTable = new StringBuilder();
            StringBuilder alternateRegimenTable = new StringBuilder();

            if (children.Count > 0)
            {
                choiceRegimenTable.AppendFormat(@"<table class=""v{0}""><tr>", surgicalProphylaxisLayout);
                choiceRegimenTable.AppendFormat(@"<th colspan=""3"">{0}</th>", choiceRegimenTitle);
                choiceRegimenTable.Append(@"</tr>");
                alternateRegimenTable.AppendFormat(@"<table class=""v{0}""><tr>", surgicalProphylaxisLayout);
                alternateRegimenTable.AppendFormat(@"<th colspan=""3"">{0}</th>", alternateRegimenTitle);
                alternateRegimenTable.Append(@"</tr>");

                pathogenHtml.AppendFormat(@"<{0}>{1}</{0}><p>", "h3", pathogenTitle);
                foreach (IBDNode child in children)
                {
                    if (child.NodeType == BDConstants.BDNodeType.BDPathogen)
                    {
                        string childHtml = (buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage)).ToString();
                        if (childHtml.Length > 0)
                        {
                            hasPathogens = true;
                            pathogenHtml.AppendFormat(@"{0}<br />", childHtml);
                        }
                    }

                    else if (child.NodeType == BDConstants.BDNodeType.BDRegimenGroup)
                    {
                        BDRegimenGroup group = child as BDRegimenGroup;
                        // string grpConjunction = retrieveConjunctionString(group.regimenGroupJoinType.Value, group);

                        if (group.regimenOfChoice.Value == true)
                        {
                            string choiceRowsHtml = (buildRegimenGroupRowsHtml(pContext, group, 0, pFootnotes, pObjectsOnPage));
                            if (choiceRowsHtml.Length > 0)
                            {
                                choiceRegimenTable.Append(choiceRowsHtml);
                                hasChoiceRegimens = true;
                            }
                        }
                        if (group.alternativeRegimen.Value == true)
                        {
                            string altRowsHtml = (buildRegimenGroupRowsHtml(pContext, group, 1, pFootnotes, pObjectsOnPage));
                            if (altRowsHtml.Length > 0)
                            {
                                alternateRegimenTable.Append(altRowsHtml);
                                hasAlternateRegimens = true;
                            }
                        }
                    }
                    else if (child.NodeType == BDConstants.BDNodeType.BDRegimen)
                    {
                        // should not fall into here.
                        Console.WriteLine("Attempt to write Regimen data outside of Regimen Group");
                    }
                }
            }
            if (hasPathogens)
            {
                pathogenHtml.Append(@"</p>");
                html.Append(pathogenHtml.ToString());
            }

            if (hasChoiceRegimens)
            {
                choiceRegimenTable.Append(@"</table>");
                html.AppendFormat(@"<br />{0}", choiceRegimenTable);
            }
            if (hasAlternateRegimens)
            {
                alternateRegimenTable.Append(@"</table>");
                html.AppendFormat(@"<br />{0}", alternateRegimenTable);
            }

            return html.ToString();
        }

        private string buildRegimenGroupRowsHtml(Entities pContext, BDRegimenGroup pGroup, int pColumnType, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder groupHtml = new StringBuilder();
            StringBuilder groupConjunctionHtml = new StringBuilder();

            if (pGroup != null)
            {
                // add row for group name 
                string groupName = buildNodePropertyHTML(pContext, pGroup, pGroup.Name, BDRegimenGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                if (groupName.Length > 0)
                {
                    groupHtml.AppendFormat(@"<tr><td class=""v301rg"" colspan=""3"">{0}</td></tr>", groupName);
                }
                BDConstants.BDJoinType groupJoinType = (BDConstants.BDJoinType)pGroup.regimenGroupJoinType.Value;
                StringBuilder conjunctionHtml = new StringBuilder();
                conjunctionHtml.Append(buildConjunctionHTML(pContext, pGroup, groupJoinType, BDRegimenGroup.PROPERTYNAME_CONJUNCTION, pFootnotes, pObjectsOnPage));
                groupConjunctionHtml.AppendFormat(@"<tr><td>{0}</td><td /><td /></tr>", conjunctionHtml);

                // get children
                List<IBDNode> regimens = BDFabrik.GetChildrenForParent(pContext, pGroup);
                if (regimens.Count > 0)
                    groupHtml.Append(buildRegimenRowsHtml(pContext, regimens, pColumnType, pFootnotes, pObjectsOnPage));

                if (pGroup.regimenGroupJoinType != (int)BDConstants.BDJoinType.Next)
                    groupHtml.Append(groupConjunctionHtml);
            }

            return groupHtml.ToString();
        }

        private string buildRegimenRowsHtml(Entities pContext, List<IBDNode> regimens, int pColumnType, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            if (regimens.Count == 0)
                return string.Empty;

            int surgicalProphylaxisLayout = (int)BDConstants.LayoutVariantType.Prophylaxis_Surgical;
            bool isRegimenInBrackets = false;

            StringBuilder regimensHtml = new StringBuilder();

            // build a row for each child & for each child conjunction that is not 'Next'
            BDRegimen lastRegimen = (BDRegimen)regimens.Last();
            for (int i = 0; i < regimens.Count; i++)
            {
                if (regimens[i].NodeType != BDConstants.BDNodeType.BDRegimen)
                    break;

                BDRegimen regimen = (BDRegimen)regimens[i];
                if (regimen.leftBracket.HasValue && regimen.leftBracket.Value == true)
                    isRegimenInBrackets = true;

                BDConstants.BDJoinType regimenJoinType = (BDConstants.BDJoinType)regimen.regimenJoinType.Value;
                string conjunctionText = buildConjunctionHTML(pContext, regimen, regimenJoinType, BDRegimen.PROPERTYNAME_CONJUNCTION, pFootnotes, pObjectsOnPage);
                StringBuilder conjunctionRowHtml = new StringBuilder();

                // if the regimen belongs in the column / table that is being built
                // (it can be a child of the regimen group, but in the other table)
                if (pColumnType == regimen.columnOrder)
                {
                    string name = buildNodePropertyHTML(pContext, regimen, regimen.Uuid, regimen.name, BDRegimen.PROPERTYNAME_NAME, "b", pFootnotes, pObjectsOnPage);
                    string dosage = buildNodePropertyHTML(pContext, regimen, regimen.dosage, BDRegimen.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                    string duration = buildNodePropertyHTML(pContext, regimen, regimen.duration, BDRegimen.PROPERTYNAME_DURATION, pFootnotes, pObjectsOnPage);


                    // table cell has CSS class of 'v301' which sets the width of the cell to 30%
                    // it's used only on the last two columns and lets the remaining columns take up the remainder
                    
                    // if (regimen.leftBracket.HasValue && regimen.leftBracket.Value)
                    //    regimensHtml.AppendFormat(@"<tr><td>{0}{1}{2}",TAG_SPAN_LEFTBRACKET, LEFT_SQUARE_BRACKET, TAG_SPAN_END);
                    //else {

                    if (isRegimenInBrackets && !(regimen.leftBracket.HasValue && regimen.leftBracket.Value == true))
                        regimensHtml.AppendFormat(@"<tr><td {0}>", CSS_INLINE_STYLE_INDENT_CELL);
                    else
                        regimensHtml.Append(@"<tr><td>");
                    // }

                    //if (regimen.rightBracket.HasValue && regimen.rightBracket.Value)
                    //    regimensHtml.AppendFormat(@"{0}{1}{2}{3}</td><td class=""v{4}"">{5}</td><td class=""v{4}"">{6}</td></tr>", name, TAG_SPAN_RIGHTBRACKET, RIGHT_SQUARE_BRACKET, TAG_SPAN_END, surgicalProphylaxisLayout, dosage, duration);
                    // else
                        regimensHtml.AppendFormat(@"{0}</td><td class=""v{3}"">{1}</td><td class=""v{3}"">{2}</td></tr>", name, dosage, duration, surgicalProphylaxisLayout);

                    if (conjunctionText.Length > 0)
                    {
                        if (regimen.regimenJoinType > (int)BDConstants.BDJoinType.Next)  // build a conjunction row if join is not 'Next'
                        {
                            if (isRegimenInBrackets && !(regimen.rightBracket.HasValue && regimen.rightBracket.Value == true))
                                conjunctionRowHtml.AppendFormat(@"<tr><td {0}>{1}</td><td /><td /></tr>", CSS_INLINE_STYLE_INDENT_CELL, conjunctionText);
                            else
                                conjunctionRowHtml.AppendFormat(@"<tr><td>{0}</td><td /><td /></tr>", conjunctionText);
                        }
                        regimensHtml.Append(conjunctionRowHtml);
                    }
                    if (regimen.rightBracket.HasValue && regimen.rightBracket.Value == true)
                        isRegimenInBrackets = false;
                }
            }
            return regimensHtml.ToString();
        }


        private string buildNodeWithReferenceAndOverviewHTML(Entities pContext, IBDNode pNode, string pHeaderTagLevel, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder nodeHTML = new StringBuilder();
            nodeHTML.Append(buildNodePropertyHTML(pContext, pNode, pNode.Uuid, pNode.Name, BDNode.PROPERTYNAME_NAME, pHeaderTagLevel, pFootnotes, pObjectsOnPage));

            nodeHTML.Append(buildReferenceHtml(pContext, pNode, pObjectsOnPage));

            return nodeHTML.ToString();
        }

        /// <summary>
        /// Build HTML for linkedNotes attached to property in node, to inject into HTML page.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNotePropertyName"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <param name="pNoteType"></param>
        /// <returns>Text of linked note as HTML</returns>
        private string buildTextForParentAndPropertyFromLinkedNotes(Entities pContext, string pPropertyName, IBDNode pNode, BDConstants.LinkedNoteType pNoteType, List<Guid> pObjectsOnPage)
        {
            StringBuilder notesHTML = new StringBuilder();
            List<Guid> noteAssociations;
            List<BDLinkedNote> notes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, pNoteType, out noteAssociations);

            notesHTML.Append(buildTextFromNotes(notes));
            pObjectsOnPage.AddRange(noteAssociations);
            return notesHTML.ToString();
        }

        /// <summary>
        /// Will cleanse each note via BDUtilities.CleanseStringOfEmptyTag
        /// </summary>
        /// <param name="pNotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <returns></returns>
        private string buildTextFromNotes(List<BDLinkedNote> pNotes)
        {
            StringBuilder noteString = new StringBuilder();
            foreach (BDLinkedNote note in pNotes)
            {
                string documentText = BDUtilities.CleanseStringOfEmptyTag(note.documentText, "p");
                if (!string.IsNullOrEmpty(documentText))
                {
                    noteString.Append(note.documentText);
                }
            }
            return noteString.ToString();
        }

        private string buildTableRowHtml(Entities pContext, BDTableRow pRow, bool forceHeaderRow, bool markFooterAtEnd, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder tableRowHTML = new StringBuilder();
            string startCellTag = @"<td>";
            string endCellTag = @"</td>";
            string startCenteredCellTag = @"<td class=""ctrAlign"">";
            string startCenteredHeaderTag = @"<th class=""ctrAlign"">";
            string twoColumnCellTag = @"<td class=""v900c"">";
            if (pRow != null)
            {
                if (BDFabrik.RowIsHeaderRow(pRow) || forceHeaderRow)
                {
                    startCellTag = @"<th>";
                    endCellTag = @"</th>";
                }
                pObjectsOnPage.Add(pRow.Uuid);
                tableRowHTML.Append(@"<tr>");
                List<BDTableCell> cells = BDTableCell.RetrieveTableCellsForParentId(pContext, pRow.Uuid);

                if (cells.Count == 0)
                    return string.Empty;
                
                for (int i = 0; i < cells.Count; i++)
                {
                    pObjectsOnPage.Add(cells[i].Uuid);
                    BDTableCell tableCell = cells[i];
                    string startTag = startCellTag;

                    if (i == 0 && pRow.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I_ContentRow)
                        startTag = startCellTag;
                    if (i == 1 && pRow.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_HeaderRow)
                        startTag = startCenteredHeaderTag;
                    if (i == 1 && pRow.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_ContentRow)
                        startTag = startCenteredCellTag;
                    if (i == 1 && pRow.LayoutVariant == BDConstants.LayoutVariantType.Table_2_Column_ContentRow)
                        startTag = twoColumnCellTag;
                    if ((i == 1 || i == 2) && pRow.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional_HeaderRow)
                        startTag = startCenteredHeaderTag;
                    if ((i == 1 || i == 2) && pRow.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional_ContentRow)
                        startTag = startCenteredCellTag;
                    if (i == 4 && pRow.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_Stepdown_ContentRow)
                        startTag = startCenteredCellTag;

                    string cellHTML = buildNodePropertyHTML(pContext, tableCell, tableCell.Uuid, tableCell.value, BDTableCell.PROPERTYNAME_CONTENTS, pFootnotes, pObjectsOnPage);
                    tableRowHTML.AppendFormat(@"{0}{1}{2}", startTag, cellHTML, endCellTag);
                }
                tableRowHTML.Append(@"</tr>");
            }

            return tableRowHTML.ToString();
        }

        private string buildReferenceHtml(Entities pContext, IBDNode pNode, List<Guid> pObjectsOnPage)
        {
            StringBuilder refHTML = new StringBuilder();

            List<BDHtmlPage> referencePages = BDHtmlPage.RetrieveHtmlPageForDisplayParentIdOfPageType(pContext, pNode.Uuid, BDConstants.BDHtmlPageType.Reference);
            foreach (BDHtmlPage refPage in referencePages)
            {
                string documentText = BDUtilities.CleanseStringOfEmptyTag(refPage.documentText, "p");
                if (!string.IsNullOrEmpty(documentText))
                {
                    refHTML.AppendFormat(@"<br /><a class=""aa"" href=""{0}"">References</a>", refPage.Uuid.ToString().ToUpper());
                    pObjectsOnPage.Add(refPage.Uuid);
                }
            }

            return refHTML.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pItemFootnotes"></param>
        /// <param name="addToPageFooter"></param>
        /// <param name="pPageFootnotes"></param>
        /// <param name="pObjectsOnPage">if not null, add Uuid of note to objects on page </param>
        /// <returns></returns>
        private string buildFooterMarkerForList(List<BDLinkedNote> pItemFootnotes, bool addToPageFooter, List<BDLinkedNote> pPageFootnotes)
        {
            StringBuilder footerMarker = new StringBuilder();
            if (pItemFootnotes.Count > 0)
            {
                footerMarker.Append(@"<sup>");
                foreach (BDLinkedNote footer in pItemFootnotes)
                {
                    if (!pPageFootnotes.Contains(footer) && addToPageFooter)
                        pPageFootnotes.Add(footer);

                    footerMarker.AppendFormat(@"{0},", pPageFootnotes.IndexOf(footer) + 1);
                }
                // trim last comma
                if (footerMarker.Length > 5)
                    footerMarker.Remove(footerMarker.Length - 1, 1);
                footerMarker.Append(@"</sup>");
                return footerMarker.ToString();
            }
            return string.Empty;
        }

        private string buildAttachmentHTML(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder attHtml = new StringBuilder();
           
            BDAttachment attachmentNode = pNode as BDAttachment;
            pObjectsOnPage.Add(pNode.Uuid);

            string imgClass = @"";

            switch (pNode.LayoutVariant)
            {
                case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
                    imgClass = @"centred";
                    break;
                case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                default:
                     imgClass = @"embedded";
                    break;
            }

            attHtml.AppendFormat(imgFileTag, imgClass, attachmentNode.uuid.ToString().ToUpper(), attachmentNode.MimeFileExtension());
            attHtml.Append(BuildBDLegendHtml(pContext, pNode, pObjectsOnPage));

            return attHtml.ToString();
        }

        private string buildHtmlForMetadataColumn(Entities pContext, IBDNode pPageDisplayParent, BDLayoutMetadataColumn pMetadataColumn, BDConstants.BDNodeType pNodeType, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            string propertyName = pMetadataColumn.FieldNameForColumnOfNodeType(pContext, pNodeType);
            return buildHtmlForMetadataColumn(pContext, pPageDisplayParent, pMetadataColumn, pNodeType, propertyName, pFootnotes, pObjectsOnPage);
        }

        /// <summary>
        /// assumes that returned string will be placed within tags on table header
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPageDisplayParent"></param>
        /// <param name="pMetadataColumn"></param>
        /// <param name="pNodeType"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <returns></returns>
        private string buildHtmlForMetadataColumn(Entities pContext, IBDNode pPageDisplayParent, BDLayoutMetadataColumn pMetadataColumn, BDConstants.BDNodeType pNodeType, string pPropertyName, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder columnHtml = new StringBuilder();
            List<Guid> objectsOnLinkedNotePage = new List<Guid>();
            string cLabel = retrieveMetadataLabelForPropertyName(pContext, pNodeType, pPropertyName, pMetadataColumn);
            List<BDLinkedNote> immediate = retrieveNotesForLayoutColumn(pContext, pMetadataColumn, BDConstants.LinkedNoteType.Immediate, pObjectsOnPage);
            List<BDLinkedNote> inline = retrieveNotesForLayoutColumn(pContext, pMetadataColumn, BDConstants.LinkedNoteType.Inline, pObjectsOnPage);
            List<BDLinkedNote> cFootnotes = retrieveNotesForLayoutColumn(pContext, pMetadataColumn, BDConstants.LinkedNoteType.Footnote, pObjectsOnPage);

            // these have a new list for the objects represented as they are written to a separate page
            List<BDLinkedNote> marked = retrieveNotesForLayoutColumn(pContext, pMetadataColumn, BDConstants.LinkedNoteType.MarkedComment, objectsOnLinkedNotePage);
            List<BDLinkedNote> unmarked = retrieveNotesForLayoutColumn(pContext, pMetadataColumn, BDConstants.LinkedNoteType.UnmarkedComment, objectsOnLinkedNotePage);

            // Specific for Hack (Treatment recommendations)
            //Hack cont'd
            if (pPageDisplayParent.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01)
            {
                //yyy
                switch (overrideType)
                {
                    case OverrideType.Paediatric:
                        //Note with display order 1: remove other from list
                        if (cFootnotes.Count >= 2) { cFootnotes.RemoveAt(1); }
                        if (marked.Count >= 2) { marked.RemoveAt(1); }
                        if (unmarked.Count >= 2) { unmarked.RemoveAt(1); }
                        break;
                    case OverrideType.Adult:
                        // Note with display order 2: remove other from list
                        if (cFootnotes.Count >= 2) { cFootnotes.RemoveAt(0); }
                        if (marked.Count >= 2) { marked.RemoveAt(0); }
                        if (unmarked.Count >= 2) { unmarked.RemoveAt(0); }
                        break;
                }
            }

            // end hack

            pFootnotes.AddRange(cFootnotes);
            string footnoteMarker = buildFooterMarkerForList(cFootnotes, true, pFootnotes);

            string inlineText = BDUtilities.BuildTextFromNotes(inline); // In approx 50% of cases, "inline" notes have been used like an "overview"
            string immediateText = BDUtilities.BuildTextFromInlineNotes(immediate);

            cLabel = string.Format(@"{0}{1}{2}{3}", (retrieveMetadataLabelForPropertyName(pContext, pNodeType, pPropertyName, pMetadataColumn)).Trim(), footnoteMarker, immediateText, inlineText);

            string processedLabel = cLabel;
            
            if(!processedLabel.Contains("</")) 
                processedLabel = BDUtilities.ProcessTextForSymbols(pContext, cLabel);
            
            BDHtmlPage notePage = generatePageForLinkedNotesOnLayoutColumn(pContext, pMetadataColumn, marked, unmarked, objectsOnLinkedNotePage);

            if (notePage != null)
                columnHtml.AppendFormat(@"<a class=""aa"" href=""{0}"">{1}{2}</a>", notePage.Uuid.ToString().ToUpper(), processedLabel, footnoteMarker);
            else
                columnHtml.AppendFormat(@"{0}{1}", processedLabel, footnoteMarker);

            return columnHtml.ToString();
        }

        //ks:
        static public string HtmlHeaderTagLevelString(int pLevel)
        {
            string result = string.Format(@"h{0}", pLevel);
            return result;
        }

        /// <summary>
        /// Build HTML segment for a single property of a node, handling all linked note types
        /// as well as footer marker, and filtering out 'New' name value.
        /// No surrounding HTML tags are returned
        /// The note parent Id is passed separately to handle cases where data is marked 'same as previous'
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pTherapy"></param>
        /// <param name="pNoteParentId"></param>
        /// <param name="pPropertyValue"></param>
        /// <param name="pNotePropertyName"></param>
        /// <returns></returns>
        private string buildNodePropertyHTML(Entities pContext, IBDNode pNode, Guid pNoteParentId, string pPropertyValue, string pPropertyName, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            return buildNodePropertyHTML(pContext, pNode, pNoteParentId, pPropertyValue, pPropertyName, string.Empty, pFootnotes, pObjectsOnPage);
        }

        /// <summary>
        /// Build HTML segment for a single property of a node, handling all linked note types
        /// as well as footer marker, and filtering out 'New' name value.
        /// No surrounding HTML tags are returned
        /// Id of note parent is assumed from passed node
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pTherapy"></param>
        /// <param name="pPropertyValue"></param>
        /// <param name="pNotePropertyName"></param>
        /// <returns></returns>
        private string buildNodePropertyHTML(Entities pContext, IBDNode pNode, string pPropertyValue, string pPropertyName, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            return buildNodePropertyHTML(pContext, pNode, pNode.Uuid, pPropertyValue, pPropertyName, string.Empty, pFootnotes, pObjectsOnPage);
        }

        /// <summary>
        /// The note parent Id is passed separately to handle cases where data is marked 'same as previous'
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pNoteParentId"></param>
        /// <param name="pPropertyValue"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="showNotesInline"></param>
        /// <param name="pHtmlTag"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <returns></returns>
        private string buildNodePropertyHTML(Entities pContext, IBDNode pNode, Guid pNoteParentId, string pPropertyValue, string pPropertyName, string pHtmlTag, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            string resolvedValue = null;
            return buildNodePropertyHTML(pContext, pNode, pNoteParentId, pPropertyValue, pPropertyName, pHtmlTag, pFootnotes, pObjectsOnPage, out resolvedValue);
        }

        private string buildNodePropertyHTML(Entities pContext, IBDNode pNode, Guid pNoteParentId, string pPropertyValue, string pPropertyName, string pHtmlTag, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, out string pResolvedValue)
        {
            string startTag = (pHtmlTag.Length > 0) ? string.Format(@"<{0}>", pHtmlTag) : string.Empty;
            string endTag = (pHtmlTag.Length > 0) ? string.Format(@"</{0}>", pHtmlTag) : string.Empty;

            if (pHtmlTag == @"u")
            {
                startTag = TAG_SPAN_UNDERLINE;
                endTag = TAG_SPAN_END;
            }

            else if (pHtmlTag == "span")
            {
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation02_NecrotizingFasciitis:
                        {
                            startTag = @"<p><span class=""body-emphasis"">";
                            endTag = @"</span></p>";
                        }
                        break;
                    default:
                        break;
                }
            }

            else if (pHtmlTag == "td")
            {
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                    case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        {
                            startTag = @"<td class=""ctrAlign"">";
                        }
                        break;
                    default:
                        break;
                }

            }

            StringBuilder propertyHTML = new StringBuilder();
            List<Guid> footerAssociations;
            List<BDLinkedNote> propertyFooters = (BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.Footnote, out footerAssociations));
            string footerMarker = buildFooterMarkerForList(propertyFooters, true, pFootnotes);

            List<Guid> inlineAssociations;
            List<Guid> markedAssociations;
            List<Guid> unmarkedAssociations;
            List<Guid> immediateAssociations;

            List<BDLinkedNote> inline = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.Inline, out inlineAssociations);
            List<BDLinkedNote> marked = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.MarkedComment, out markedAssociations);
            List<BDLinkedNote> unmarked = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.UnmarkedComment, out unmarkedAssociations);
            List<BDLinkedNote> immediate = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.Immediate, out immediateAssociations);


            #region BF Hack that makes me grumpy

            // Antibiotics > Antimicrobial Spectrum of Activity > Antimicrobials | Antifungals > {Antimicrobial} > topic {Predictable | Unpredictable | no/Insufficient Activity

            // Layout must be created for layout Antibiotics_ClinicalGuidelines with at least 3 columns in order of the expected order of Pred, Unpred, Insuff.
            // Column names must have associated Marked Comment Notes that will be attached to existing marked comments here.

            // Required: pNode Display order = [0..2] corresponding to layout column order
            if ((pNode.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines) && (pNode.NodeType == BDConstants.BDNodeType.BDTopic) && (pNode.ParentType == BDConstants.BDNodeType.BDAntimicrobial))
            {
                List<BDLayoutMetadataColumn> legendList = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines);
                if ((null != legendList) && (legendList.Count >= 3))
                {
                    BDLayoutMetadataColumn column = legendList[pNode.DisplayOrder.Value];
                    List<BDLinkedNote> legendMarkedList = retrieveNotesForLayoutColumn(pContext, column, BDConstants.LinkedNoteType.MarkedComment, markedAssociations);
                    marked.AddRange(legendMarkedList);
                }
            }

            #endregion



            //ks: added "New " prefix to permit the use of terms like "Table A" to appear in the name of a BDTable instance
            string namePlaceholderText = string.Format(@"New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
            if (string.IsNullOrEmpty(pPropertyValue) || pPropertyValue.Contains(namePlaceholderText) || pPropertyValue == "SINGLE PRESENTATION")
                pPropertyValue = string.Empty;

            // overview
            string overviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid, pObjectsOnPage);

            // for one layout variant, the overview is added elsewhere
            if (pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II)
                overviewHTML = @"";

            if ((pNode.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines) && (pNode.NodeType == BDConstants.BDNodeType.BDTopic) && (pNode.ParentType == BDConstants.BDNodeType.BDAntimicrobial))
                if(overviewHTML.Contains("<p>"))
                    overviewHTML = overviewHTML.Replace("<p>", "<p class=\"v201\">");

            pObjectsOnPage.Add(pNode.Uuid);

            List<Guid> objectsOnChildPage = new List<Guid>();
            objectsOnChildPage.AddRange(markedAssociations);
            objectsOnChildPage.AddRange(unmarkedAssociations);
            BDHtmlPage notePage = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.NodeType, marked, unmarked, pPropertyName, objectsOnChildPage);

            string inlineOverviewText = BDUtilities.BuildTextFromNotes(inline); // In approx 50% of cases, "inline" notes have been used like an "overview"
            string immediateText = BDUtilities.BuildTextFromInlineNotes(immediate);

            // Replace >, < >= <= that may appear in text, entered from a text field (and not from the TXTextControl)
            string cleanPropertyValue = pPropertyValue;

            if (BDUtilities.HasSymbols(cleanPropertyValue))
            {
                cleanPropertyValue = BDUtilities.ProcessTextForSymbols(pContext, pPropertyValue.Trim());
                cleanPropertyValue = BDUtilities.ProcessTextForStyleMarkup(cleanPropertyValue);
                ibdNodeWithSymbolUuidList.Add(pNode.Uuid);
            }

            StringBuilder leftBracketHtml = new StringBuilder();
            StringBuilder rightBracketHtml = new StringBuilder();
            bool hasLeftBracket = false;
            bool hasRightBracket = false;

            if (pNode.NodeType == BDConstants.BDNodeType.BDTherapy && pPropertyName == BDTherapy.PROPERTYNAME_THERAPY)
            {
                BDTherapy therapy = pNode as BDTherapy;

                hasLeftBracket = (therapy.leftBracket.HasValue && therapy.leftBracket.Value == true);

                hasRightBracket = (therapy.rightBracket.HasValue && therapy.rightBracket.Value == true);
            }
            if (pNode.NodeType == BDConstants.BDNodeType.BDRegimen && pPropertyName == BDRegimen.PROPERTYNAME_NAME)
            {
                BDRegimen regimen = pNode as BDRegimen;

                hasLeftBracket = (regimen.leftBracket.HasValue && regimen.leftBracket.Value == true);

                hasRightBracket = (regimen.rightBracket.HasValue && regimen.rightBracket.Value == true);
            }

            if (hasLeftBracket)
            {
                leftBracketHtml.AppendFormat(@"{0}{1}{2}", TAG_SPAN_LEFTBRACKET, LEFT_SQUARE_BRACKET, TAG_SPAN_END);
            }

            if (hasRightBracket)
                rightBracketHtml.AppendFormat(@"{0}{1}{2}", TAG_SPAN_RIGHTBRACKET, RIGHT_SQUARE_BRACKET, TAG_SPAN_END);


            pResolvedValue = string.Format(@"{0}{1}{2}", cleanPropertyValue, footerMarker, immediateText);
            pObjectsOnPage.AddRange(immediateAssociations);

            if (pHtmlTag.ToLower() == "td")  // cell contents are being built:  replace what's in pResolvedValue
            {
                if (!string.IsNullOrEmpty(inlineOverviewText))
                {
                   // BDHtmlPageGeneratorLogEntry.AppendToFile("BDHTMLPageReview.txt", string.Format("{0}\t{1}\tLINKED NOTE: remove paragraph tag ", DateTime.Now, pNode.Uuid.ToString()));

                    string cleanString = BDUtilities.CleanNoteText(inlineOverviewText);
                    string nodeNamePlaceholderText = string.Format(@"New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
                    string nodeName = pNode.Name;
                    if (string.IsNullOrEmpty(nodeName) || nodeName.Contains(nodeNamePlaceholderText))
                        nodeName = string.Empty;

                    if (nodeName == string.Empty) {
                        // if node name is empty, inline note needs no line break
                        inlineOverviewText = cleanString;
                        //BDHtmlPageGeneratorLogEntry.AppendToFile("BDHTMLPageReview.txt", string.Format("{0}\t{1}\tLINKED NOTE: property name empty ", DateTime.Now, pNode.Uuid.ToString()));

                    } else {
                        // if pNode has a name, insert a break before the note text
                        inlineOverviewText = string.Format(@"<br />{0}", cleanString); 
                    }
                }

                if (notePage != null)
                {
                    // build a link to linked note page, with immediate notes behind the clean property value
                    if (!string.IsNullOrEmpty(pPropertyValue))
                        propertyHTML.AppendFormat(@"{0}{1}<a class=""aa"" href=""{2}"">{3}{4}</a>{5}{6}{7}{8}{9}", startTag, leftBracketHtml.ToString(), notePage.Uuid.ToString().ToUpper(), cleanPropertyValue, immediateText, footerMarker, rightBracketHtml.ToString(), inlineOverviewText, overviewHTML, endTag);
                    else
                    {
                        // property has no value
                        if (immediateText.Length > 0)
                        {
                            pResolvedValue = string.Format(@"<a class=""aa"" href=""{0}"">{1}</a>", notePage.Uuid.ToString().ToUpper(), immediateText);
                        }
                        else
                        {
                            pResolvedValue = string.Format(@"<a class=""aa"" href=""{0}"">See Comments.</a>", notePage.Uuid.ToString().ToUpper());
                        }
                        propertyHTML.AppendFormat(@"{0}{1}{2}{3}{4}{5}{6}", startTag, leftBracketHtml.ToString(), pResolvedValue, rightBracketHtml.ToString(), inlineOverviewText, overviewHTML, endTag);
                    }
                }
                else
                {
                    propertyHTML.AppendFormat(@"{0}{1}{2}{3}{4}{5}{6}", startTag, leftBracketHtml.ToString(), pResolvedValue, rightBracketHtml.ToString(), inlineOverviewText, overviewHTML, endTag);
                }
            }
            else  // html tag is not 'td'
            {
                if (notePage != null)
                {
                    if (!string.IsNullOrEmpty(pPropertyValue))
                        propertyHTML.AppendFormat(@"{0}{1}<a class=""aa"" href=""{2}"">{3}{4}</a>{5}{6}{7}{8}{9}", startTag, leftBracketHtml.ToString(), notePage.Uuid.ToString().ToUpper(), cleanPropertyValue, immediateText, footerMarker, rightBracketHtml.ToString(), endTag, inlineOverviewText, overviewHTML);
                    else
                    {
                        if (immediateText.Length > 0)
                        {
                            pResolvedValue = string.Format(@"<a class=""aa"" href=""{0}"">{1}</a>", notePage.Uuid.ToString().ToUpper(), immediateText);
                        }
                        else
                        {
                            pResolvedValue = string.Format(@"<a class=""aa"" href=""{0}"">See Comments.</a>", notePage.Uuid.ToString().ToUpper());
                        }
                        if (!string.IsNullOrEmpty(pResolvedValue))
                            propertyHTML.AppendFormat(@"{0}{1}{2}{3}{4}", startTag, pResolvedValue, endTag, inlineOverviewText, overviewHTML);
                        else
                            propertyHTML.AppendFormat(@"{0}{1}", inlineOverviewText, overviewHTML);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(pResolvedValue))
                        propertyHTML.AppendFormat(@"{0}{1}{2}{3}{4}{5}{6}", startTag, leftBracketHtml.ToString(), pResolvedValue, rightBracketHtml.ToString(), endTag, inlineOverviewText, overviewHTML);
                    else
                        propertyHTML.AppendFormat(@"{0}{1}", inlineOverviewText, overviewHTML);
                }
            }
            pObjectsOnPage.AddRange(inlineAssociations);
           // pResolvedValue = string.Format(@"{0}{1}", pResolvedValue, inlineOverviewText);

           // if (pResolvedValue.Trim().Length == 0) pResolvedValue = null;

            return propertyHTML.ToString().Trim();
        }

        private string buildConjunctionHTML(Entities pContext, IBDNode pNode, BDConstants.BDJoinType pJoinType, string pPropertyName, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            string resolvedValue = null;

            StringBuilder propertyHTML = new StringBuilder();
            List<Guid> footerAssociations;
            List<BDLinkedNote> propertyFooters = (BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Footnote, out footerAssociations));
            string footerMarker = buildFooterMarkerForList(propertyFooters, true, pFootnotes);

            List<Guid> inlineAssociations;
            List<Guid> markedAssociations;
            List<Guid> unmarkedAssociations;
            List<Guid> immediateAssociations;

            List<BDLinkedNote> inline = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Inline, out inlineAssociations);
            List<BDLinkedNote> marked = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.MarkedComment, out markedAssociations);
            List<BDLinkedNote> unmarked = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.UnmarkedComment, out unmarkedAssociations);
            List<BDLinkedNote> immediate = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Immediate, out immediateAssociations);

            string propertyValue = string.Empty;
            propertyValue = retrieveConjunctionString(pJoinType);

            // overview
            string overviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid, pObjectsOnPage);

            pObjectsOnPage.Add(pNode.Uuid);

            List<Guid> objectsOnChildPage = new List<Guid>();
            objectsOnChildPage.AddRange(markedAssociations);
            objectsOnChildPage.AddRange(unmarkedAssociations);
            BDHtmlPage notePage = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.NodeType, marked, unmarked, pPropertyName, objectsOnChildPage);

            string inlineOverviewText = BDUtilities.BuildTextFromInlineNotes(inline); // In approx 50% of cases, "inline" notes have been used like an "overview"
            string immediateText = BDUtilities.BuildTextFromInlineNotes(immediate);

            resolvedValue = string.Format(@"{0}{1}{2}", propertyValue, footerMarker, immediateText);
            pObjectsOnPage.AddRange(immediateAssociations);

            if (notePage != null)
            {
                if (!string.IsNullOrEmpty(propertyValue))
                    propertyHTML.AppendFormat(@"<a class=""aa"" href=""{0}"">{1}{2}</a>{3}{4}{5}", notePage.Uuid.ToString().ToUpper(), propertyValue, immediateText, footerMarker, inlineOverviewText, overviewHTML);
                else
                {
                    if (immediateText.Length > 0)
                    {
                        resolvedValue = string.Format(@"<a class=""aa"" href=""{0}"">{1}</a>", notePage.Uuid.ToString().ToUpper(), immediateText);
                    }
                    else
                    {
                        resolvedValue = string.Format(@"<a class=""aa"" href=""{0}"">See Comments.</a>", notePage.Uuid.ToString().ToUpper());
                    }
                    if (!string.IsNullOrEmpty(resolvedValue))
                        propertyHTML.AppendFormat(@"{0}{1}{2}", resolvedValue, inlineOverviewText, overviewHTML);
                    else
                        propertyHTML.AppendFormat(@"{0}{1}", inlineOverviewText, overviewHTML);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(resolvedValue))
                    propertyHTML.AppendFormat(@"{0}<br />{1}{2}", resolvedValue, inlineOverviewText, overviewHTML);
                else
                    propertyHTML.AppendFormat(@"{0}{1}", inlineOverviewText, overviewHTML);
            }
            //}
            pObjectsOnPage.AddRange(inlineAssociations);
            resolvedValue = string.Format(@"{0}{1}", resolvedValue, inlineOverviewText);

            if (resolvedValue.Trim().Length == 0) resolvedValue = null;

            return propertyHTML.ToString().Trim();
        }
        #endregion

        #region Utility methods
        /// <summary>
        /// Retrieve linked note text for Overview of a node
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <param name="pNotePropertyName"></param>
        /// <returns></returns>
        private string retrieveNoteTextForOverview(Entities pContext, Guid pParentId, List<Guid> pObjectsOnPage)
        {
            string propertyName = BDNode.VIRTUALPROPERTYNAME_OVERVIEW;
            StringBuilder linkedNoteHtml = new StringBuilder();
            List<BDLinkedNoteAssociation> list = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(pContext, pParentId, propertyName);
            foreach (BDLinkedNoteAssociation assn in list)
            {
                BDLinkedNote linkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, assn.linkedNoteId);
                if (null != linkedNote)
                {
                    linkedNoteHtml.Append(linkedNote.documentText);
                    pObjectsOnPage.Add(assn.Uuid);
                }
            }

            string resultValue = BDUtilities.CleanseStringOfEmptyTag(linkedNoteHtml.ToString(), "p");

            return resultValue;
        }

        private string retrieveNoteTextForConfiguredEntryField(Entities pContext, Guid pParentId, string pNotePropertyName, List<Guid> pObjectsOnPage, List<BDLinkedNote> pFootnotesOnPage)
        {
            return retrieveNoteTextForConfiguredEntryField(pContext, pParentId, pNotePropertyName, string.Empty, pObjectsOnPage, false, pFootnotesOnPage);
        }

        private string retrieveNoteTextForConfiguredEntryField(Entities pContext, Guid pParentId, string pNotePropertyName, string pFieldPropertyName, List<Guid> pObjectsOnPage, bool trimTags, List<BDLinkedNote> pFootnotesOnPage)
        {
            StringBuilder noteText = new StringBuilder();
            List<Guid> inlineAssociations;
            List<BDLinkedNote> notes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pNotePropertyName, BDConstants.LinkedNoteType.Inline, out inlineAssociations);
            foreach (BDLinkedNote note in notes)
            {
                string documentText = BDUtilities.CleanseStringOfEmptyTag(note.documentText, "p");
                if (!String.IsNullOrEmpty(documentText))
                {
                    string resultText = string.Empty;
                    if (trimTags) // trim the start and end paragraph tags
                    {
                        if (note.documentText.StartsWith("<p>"))
                            resultText = note.documentText.Substring(3);
                        else
                            resultText = note.documentText;
                        if (resultText.EndsWith("</p>"))
                            resultText = resultText.Substring(0, resultText.Length - 4);
                    }
                    else
                        resultText = note.documentText;
                    noteText.Append(resultText);
                    pObjectsOnPage.AddRange(inlineAssociations);
                }
                // retrieve any linked notes for the named property; add to footnote collection and mark the text
                if (pFieldPropertyName != string.Empty)
                {
                    //TODO:Remove connection of ALL notes as footnotes. UI must not allow linked note button when overview editor is visible on ConfiguredEntries
                    List<Guid> fieldNoteAssociations;
                    List<BDLinkedNote> fieldNotes = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pFieldPropertyName, BDConstants.LinkedNoteType.Footnote, out fieldNoteAssociations);
                    List<Guid> inlineFieldAssociations;
                    fieldNotes.AddRange(BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pFieldPropertyName, BDConstants.LinkedNoteType.Inline, out inlineFieldAssociations));
                    List<Guid> markedAssociations;
                    fieldNotes.AddRange(BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pFieldPropertyName, BDConstants.LinkedNoteType.MarkedComment, out markedAssociations));
                    List<Guid> unmarkedAssociations;
                    fieldNotes.AddRange(BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pFieldPropertyName, BDConstants.LinkedNoteType.UnmarkedComment, out unmarkedAssociations));

                    pObjectsOnPage.AddRange(fieldNoteAssociations);
                    pObjectsOnPage.AddRange(inlineFieldAssociations);
                    pObjectsOnPage.AddRange(markedAssociations);
                    pObjectsOnPage.AddRange(unmarkedAssociations);

                    List<string> footnoteMarkers = new List<string>();
                    noteText.Append(buildFooterMarkerForList(fieldNotes, true, pFootnotesOnPage));
                }

            }
            return noteText.ToString();
        }

        private string generateLinkNameForPage(Entities pContext, IBDNode pDisplayParent)
        {
            // in some cases there are nodes where the name is empty but link text is needed for the navigation page.
            StringBuilder titleText = new StringBuilder();
            if (null != pDisplayParent && pDisplayParent.NodeType == BDConstants.BDNodeType.BDSurgeryGroup && (
                pDisplayParent.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries ||
                  pDisplayParent.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification))
            {
                if (string.IsNullOrEmpty(pDisplayParent.Name))
                {
                    // find the child nodes
                    List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pDisplayParent);
                    // build a name from a list
                    foreach (IBDNode child in children)
                    {
                        if (child.NodeType == BDConstants.BDNodeType.BDSurgery)
                        {
                            if (titleText.Length > 0)
                                titleText.Append(@", ");
                            titleText.Append(child.Name);
                        }
                    }
                }
            }
            return titleText.ToString();
        }

        private void processTextForInternalLinks(Entities pContext, BDHtmlPage pPage, List<Guid> pExistingPages)
        {
            postProcessingPageLayoutVariant = pPage.layoutVariant;
            //BDNodeToHtmlPageIndex index = BDNodeToHtmlPageIndex.RetrieveIndexEntryForHtmlPageId(pContext, pPage.Uuid);
            //if (index != null)
            //    currentChapter = BDFabrik.RetrieveNode(pContext, index.chapterId);
            //else
            //    currentChapter = null;

            //TODO: convert to constants
            string compareString1 = @"<a class=""aa"" href=";
            string compareString2 = @"<a href=";

            string temp = pPage.documentText;
            temp = temp.Replace(compareString2, compareString1);
            if (pPage.documentText != temp)
                pPage.documentText = temp;

            if (pPage.documentText.Contains(compareString1) || pPage.documentText.Contains(compareString2))
            {
                int startPosition = 0;

                while (startPosition < pPage.documentText.Length)
                {
                    // find the anchor tag
                    int tagLocation = 0;
                    int compareStringLength = 0;
                    if (pPage.documentText.Contains(compareString1))
                    {
                        tagLocation = pPage.documentText.IndexOf(compareString1, startPosition);
                        compareStringLength = compareString1.Length;
                    }
                    if (pPage.documentText.Contains(compareString2))
                    {
                        tagLocation = pPage.documentText.IndexOf(compareString2, startPosition);
                        compareStringLength = compareString2.Length;
                    }

                    if (tagLocation >= 0)
                    {
                        // inspect the 'guid'
                        int guidStart = tagLocation + 1 + compareStringLength;
                        string guidString = pPage.documentText.Substring(guidStart, 36);
                        // if the guid exists as an external URL, or an email address, dont change it...
                        if (!guidString.Contains("http://www") && !guidString.Contains("mailto:"))
                        {
                            Guid anchorGuid = new Guid(guidString);
                            if (!pExistingPages.Contains(anchorGuid))
                            {
                                // none of the existing html pages has this guid so the existing link is invalid as its currently written
                                // look up the linkedNoteAssociation(s) with the provided guid in the'parentKeyPropertyName'
                                List<BDLinkedNoteAssociation> assnList = BDLinkedNoteAssociation.RetrieveLinkedNoteAssociationsForParentKeyPropertyName(pContext, anchorGuid.ToString());
                                if (null == assnList || assnList.Count == 0)
                                {
                                    // if we have null here, then we'll have a link that leads to a blank page.  Need to flag when this happens
                                    BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkIssueLog.txt", string.Format("Unresolved internal link - no match found:  {0}\tHtml page Uuid {1}\tAnchor Uuid {2}\tLNA LinkedNoteAssociation is NULL", DateTime.Now, pPage.Uuid, anchorGuid.ToString()));
                                }
                                else if (assnList.Count == 1)
                                {
                                    BDLinkedNoteAssociation linkTargetAssn = assnList[0];
                                    processLinkedNoteAssociation(pContext, anchorGuid, pPage, linkTargetAssn, pExistingPages);
                                } 

                                else if (assnList.Count > 1)
                                {
                                    // there are multiple matches in the database 
                                    // This is an error: check for indications that one of the assns is legitimate.
                                    foreach (BDLinkedNoteAssociation assn in assnList)
                                    {
                                        bool hasValidLink = false;
                                        
                                        if (assn.LinkedNoteType == BDConstants.LinkedNoteType.InternalLink && assn.internalLinkNodeId.HasValue)
                                        {
                                            IBDNode targetNode = BDFabrik.RetrieveNode(pContext, assn.internalLinkNodeId);
                                            if(null != targetNode)
                                                hasValidLink = true;
                                        }
                                        else
                                            hasValidLink = validOriginForLinkedNoteAssociation(pContext, assn, anchorGuid);

                                        if (hasValidLink == true)
                                            processLinkedNoteAssociation(pContext, anchorGuid, pPage, assn, pExistingPages);
                                        else
                                            Debug.WriteLine("*****CAUTION:  Internal link issue was not resolved.  Multiple matches found; Investigation recommended - see output log BDInternalLinkIssueLog.txt");
                                    }
                                }
                            }
                        }
                        startPosition = guidStart;
                    }
                    else
                        startPosition = pPage.documentText.Length;
                }
            }
        }

        private bool validOriginForLinkedNoteAssociation(Entities pContext, BDLinkedNoteAssociation linkTargetAssn, Guid anchorGuid)
        {
            /*
             *  c817f6bd-979f-4547-ae89-5682ddc6dcbf is the anchorId 
             *  c817f is the parentKeyPropertyName in 2 lna
             *  linkedNoteId in these 2 lna is used to look up a linkedNote: if linked note text does not contain anchorId
             *  lookup next layer using parentId, parentType
             */
            bool validLinkExists = false;
            bool emptyNote = false;
            if (linkTargetAssn.ParentType == BDConstants.BDNodeType.BDLinkedNote)
            {
                BDLinkedNote parentNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, linkTargetAssn.linkedNoteId.Value);
                if(linkTargetAssn.internalLinkNodeId.HasValue)
                {
                    IBDNode targetNode = BDFabrik.RetrieveNode(pContext, linkTargetAssn.internalLinkNodeId);
                    if (null != targetNode)
                        validLinkExists = true;
                }
                else if (parentNote.documentText.Length <= EMPTY_PARAGRAPH)
                {
                    emptyNote = true;
                    BDLinkedNoteAssociation.DeleteForNote(pContext, parentNote);
                    pContext.SaveChanges();

                    BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkIssueLog.txt", string.Format("{0} Empty LinkedNote Deleted:  {1}\tLinkedNoteAssociation Uuid {2}", DateTime.Now, linkTargetAssn.linkedNoteId, linkTargetAssn.Uuid));

                    return validLinkExists;  // false
                }
                else if (parentNote.documentText.Contains(anchorGuid.ToString()))
                {
                    validLinkExists = true;
                    return validLinkExists;
                }
                else
                {
                    // look up next layer of linked notes
                    List<BDLinkedNoteAssociation> parentAssnList = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForLinkedNoteId(pContext, linkTargetAssn.parentId.Value);
                    foreach (BDLinkedNoteAssociation assn in parentAssnList)
                    {
                        validLinkExists = validOriginForLinkedNoteAssociation(pContext, assn, anchorGuid);
                        if (validLinkExists == true)
                            return validLinkExists;
                    }
                }
            }
            if(emptyNote == true)
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkIssueLog.txt", string.Format("{0} Empty LinkedNote:  {1}\tLinkedNoteAssociation Uuid {2}", DateTime.Now, linkTargetAssn.linkedNoteId, linkTargetAssn.Uuid));
            else if(validLinkExists == false)
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkIssueLog.txt", string.Format("{0} Invalid LinkedNoteAssociation - link not found in original text \tLink Uuid {1}\tLinkedNoteAssociation Uuid {2}", DateTime.Now, anchorGuid, linkTargetAssn.Uuid));

            return validLinkExists;                                
        }

        private void processLinkedNoteAssociation(Entities pContext, Guid anchorGuid, BDHtmlPage pPage, BDLinkedNoteAssociation linkTargetAssn, List<Guid> pExistingPages)
        {
            if (linkTargetAssn != null)
            {

                #region Internal Link
                if (linkTargetAssn.internalLinkNodeId.HasValue)
                {
                    // this is an internal link - first check the pagesMap for the HTML page containing that object
                    Guid htmlPageId = Guid.Empty;

                    try
                    {
                        htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForInternalLinkByOriginalIBDNodeId(pContext, linkTargetAssn.internalLinkNodeId.Value);

                        if (htmlPageId == Guid.Empty) // page was not found - check the BDNodeToHtmlPageIndex table also
                        {
                            //ks: Expectation that internal links will always link to "data" pages rather than "linked note" pages
                            htmlPageId = BDNodeToHtmlPageIndex.RetrieveHtmlPageIdForIBDNodeId(pContext, linkTargetAssn.internalLinkNodeId.Value, BDConstants.BDHtmlPageType.Data);

                            if (htmlPageId == Guid.Empty)  // link points to a node on a 'navigation' page
                            {
                                htmlPageId = BDNodeToHtmlPageIndex.RetrieveHtmlPageIdForIBDNodeId(pContext, linkTargetAssn.internalLinkNodeId.Value, BDConstants.BDHtmlPageType.Navigation);
                            }
                        }

                        if (htmlPageId != Guid.Empty && null != BDHtmlPage.RetrieveWithId(pContext, htmlPageId))
                        {
                            // modify anchor tag to point to the html page generated for the targeted node
                            string newText = pPage.documentText.Replace(anchorGuid.ToString(), htmlPageId.ToString().ToUpper());
                            pPage.documentText = newText;
                            BDHtmlPage.Save(pContext, pPage);

                            if (htmlPageId == pPage.Uuid)
                            {
                                BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkToSelfLog.txt", string.Format("{0}\tHtml page Uuid {1}\tObjId={2}\tTitle={3}", DateTime.Now, pPage.Uuid, pPage.displayParentId, pPage.pageTitle));
                            }
                        }
                        else // if this is an internal link there should be a page for it
                        {
                            Debug.WriteLine("Unable to map link in {0} showing {1}", pPage.Uuid, anchorGuid);
                            BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkIssueLog.txt", string.Format("{0}\tHtml page Uuid {1}\tAnchor Uuid {2}\tLNA {3}", DateTime.Now, pPage.Uuid, anchorGuid.ToString(), linkTargetAssn.Uuid.ToString()));
                        }
                    }
                    catch (BDException bde)
                    {
                        BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkIssueLog.txt", string.Format("Unresolved internal link {0}:  {1}\tHtml page Uuid {2}\tAnchor Uuid {3}\tLNA {4}", DateTime.Now, bde.Message, pPage.Uuid, anchorGuid.ToString(), linkTargetAssn.Uuid.ToString()));
                        Debug.WriteLine("*****CAUTION:  Internal link issue was not resolved.  Multiple matches found; Investigation recommended - see output log BDInternalLinkIssueLog.txt");
                    }
                #endregion

                }
                //Standard Linked Note
                else if (linkTargetAssn.linkedNoteId.HasValue)
                {
                    #region External Link
                    if (linkTargetAssn.LinkedNoteType == BDConstants.LinkedNoteType.External)
                    {
                        BDLinkedNote targetNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, linkTargetAssn.linkedNoteId);
                        string documentText = BDUtilities.CleanseStringOfEmptyTag(targetNote.documentText, "p");
                        if (!String.IsNullOrEmpty(documentText))
                        {
                            //Remove all the paragraph markers and such.
                            string externalLinkText = targetNote.documentText.Replace("<p>", string.Empty);
                            externalLinkText = externalLinkText.Replace("<p >", string.Empty);
                            externalLinkText = externalLinkText.Replace("</p>", string.Empty);
                            externalLinkText = externalLinkText.Trim();

                            if (externalLinkText.StartsWith("<a"))  //Compensate for accidentally misentered external link
                            {
                                //strip the anchor tags
                                int anchorEndPos = externalLinkText.IndexOf(">");
                                externalLinkText = externalLinkText.Substring(anchorEndPos + 1);
                                externalLinkText = externalLinkText.Replace("</a>", "");
                            }
                            if (!externalLinkText.ToLower().StartsWith("mailto:"))
                            {

                                if (!externalLinkText.ToLower().StartsWith("http://"))
                                {
                                    externalLinkText = string.Format(@"http://{0}", externalLinkText);
                                }
                            }

                            string newDocumentText = pPage.documentText.Replace(anchorGuid.ToString(), externalLinkText);
                            pPage.documentText = newDocumentText;
                            BDHtmlPage.Save(pContext, pPage);
                        }
                    }
                    #endregion

                    else
                    {
                        Guid htmlPageId = BDHtmlPageMap.RetrieveHtmlPageIdForOriginalIBDNodeId(pContext, linkTargetAssn.linkedNoteId.Value);

                        if (htmlPageId != Guid.Empty && pExistingPages.Contains(htmlPageId))
                        {
                            // modify anchor tag to point to the html page generated for the targeted node
                            string newText = pPage.documentText.Replace(anchorGuid.ToString(), htmlPageId.ToString().ToUpper());
                            pPage.documentText = newText;
                            BDHtmlPage.Save(pContext, pPage);
                        }

                        else
                        {
                            // create an html page for the linked note - if its a note-in-note it may not have been created yet
                            BDLinkedNote targetNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, linkTargetAssn.linkedNoteId);
                            string documentText = BDUtilities.CleanseStringOfEmptyTag(targetNote.documentText, "p");
                            if (!String.IsNullOrEmpty(documentText))
                            {
                                List<Guid> objectsOnPage = new List<Guid>();
                                objectsOnPage.Add(linkTargetAssn.linkedNoteId.Value);
                                BDHtmlPage newPage = generatePageForLinkedNotes(pContext, linkTargetAssn.linkedNoteId.Value, BDConstants.BDNodeType.BDLinkedNote, targetNote.documentText, BDConstants.BDHtmlPageType.Comments, objectsOnPage, linkTargetAssn.parentKeyPropertyName);

                                string newText = pPage.documentText.Replace(anchorGuid.ToString(), newPage.Uuid.ToString().ToUpper());
                                pPage.documentText = newText;
                                BDHtmlPage.Save(pContext, pPage);

                                pExistingPages.Add(newPage.Uuid);

                                postProcessPage(pContext, newPage, pExistingPages);
                            }
                        }
                    }
                }
            }
        }

        private void postProcessPage(Entities pContext, BDHtmlPage pPage, List<Guid> pExistingPages)
        {
            processTextForInternalLinks(pContext, pPage, pExistingPages);
            BDHtmlPage.Save(pContext, pPage);
            pPage.documentText = BDUtilities.ProcessTextForSubscriptAndSuperscriptMarkup(pContext, pPage.documentText);
            BDHtmlPage.Save(pContext, pPage);
            pPage.documentText = BDUtilities.ProcessTextForCarriageReturn(pContext, pPage.documentText);
            BDHtmlPage.Save(pContext, pPage);

            //if (pPage.documentText.Contains("<li><p"))  // Sept 2016
            //{
            //    BDHtmlPageGeneratorLogEntry.AppendToFile("BDHTMLPageReview.txt", string.Format("{0}\t{1}\tBULLETED LIST: contains paragraph", DateTime.Now, pPage.Uuid.ToString()));
            //}
        }

        private bool notesListHasContent(Entities pContext, List<BDLinkedNote> pNotes)
        {
            bool hasContent = false;
            foreach (BDLinkedNote note in pNotes)
            {
                string documentText = BDUtilities.CleanseStringOfEmptyTag(note.documentText, "p");
                if (!String.IsNullOrEmpty(documentText))
                {
                    hasContent = true;
                    break;
                }
            }
            return hasContent;
        }

        private void resetGlobalVariablesForTherapies()
        {
            previousTherapyNameId = Guid.Empty;
            previousTherapyDosageId = Guid.Empty;
            previousTherapyDosage2Id = Guid.Empty;
            previousTherapyDurationId = Guid.Empty;
            previousTherapyDuration1Id = Guid.Empty;
            previousTherapyDuration2Id = Guid.Empty;
            therapiesHaveDosage = false;
            therapiesHaveDuration = false;
        }

        private string retrieveConjunctionString(BDConstants.BDJoinType pBDJoinType)
        {
            return retrieveConjunctionString((int)pBDJoinType, null);
        }

        private string retrieveConjunctionString(int pBDJoinType)
        {
            return retrieveConjunctionString(pBDJoinType, null);
        }

        private string retrieveConjunctionStringWithNote(Entities pContext, IBDNode pParent, string pNotePropertyName, List<Guid> pObjectsOnPage, List<BDLinkedNote> pFootnotesOnPage, BDConstants.BDJoinType pBDJoinType)
        {
            StringBuilder returnString = new StringBuilder();
            returnString.Append(retrieveConjunctionString((int)pBDJoinType, null));
            List<Guid> inlineAssociations = new List<Guid>();

            List<BDLinkedNote> inline = BDUtilities.RetrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParent.Uuid, pNotePropertyName, BDConstants.LinkedNoteType.Inline, out inlineAssociations);

            return returnString.ToString();
        }

        private string retrieveConjunctionString(int pBDJoinType, IBDNode pNode)
        {
            string joinString = string.Empty;
            // check for conjunctions and add a tableRows for any that are found

            switch (pBDJoinType)
            {
                case (int)BDConstants.BDJoinType.Next:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.Next);
                    break;
                case (int)BDConstants.BDJoinType.AndWithNext:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.AndWithNext);

                    if (null != pNode)
                    {
                        //switch (pNode.LayoutVariant)
                        //{
                        //    case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        //        joinString = "+";
                        //        break;
                        //}
                    }
                    break;
                case (int)BDConstants.BDJoinType.OrWithNext:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.OrWithNext);
                    break;
                case (int)BDConstants.BDJoinType.ThenWithNext:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.ThenWithNext);
                    break;
                case (int)BDConstants.BDJoinType.WithOrWithoutWithNext:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.WithOrWithoutWithNext);
                    break;
                case (int)BDConstants.BDJoinType.Other:
                    joinString = string.Empty;
                    break;
                case (int)BDConstants.BDJoinType.AndOr:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.AndOr);
                    break;
                default:
                    joinString = string.Empty;
                    break;
            }

            return joinString;
        }

        /// <summary>
        /// Validates that the returned label is associated with the specified property name and node type.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNodeType"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="pColumn"></param>
        /// <returns></returns>
        private string retrieveMetadataLabelForPropertyName(Entities pContext, BDConstants.BDNodeType pNodeType, string pPropertyName, BDLayoutMetadataColumn pColumn)
        {
            string propertyColumnLabel = string.Empty;
            List<BDLayoutMetadataColumnNodeType> columnNodeTypes = BDLayoutMetadataColumnNodeType.RetrieveListForLayoutColumn(pContext, pColumn.Uuid);
            foreach (BDLayoutMetadataColumnNodeType columnNodeType in columnNodeTypes)
            {
                if (columnNodeType.propertyName == pPropertyName && columnNodeType.nodeType == (int)pNodeType)
                {
                    propertyColumnLabel = pColumn.label;
                    break;
                }
                if (propertyColumnLabel.Length > 0)
                    break;
            }
            return propertyColumnLabel;
        }

        /// <summary>
        /// Retrieve either: notes of a specific type or all
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pColumn"></param>
        /// <param name="pNoteType">if null, will return all the notes</param>
        /// <param name="pObjectsOnPage">if null, no objects will be added to ObjectsOnPage for BDHtmlPageMap</param>
        /// <returns></returns>
        private List<BDLinkedNote> retrieveNotesForLayoutColumn(Entities pContext, BDLayoutMetadataColumn pColumn, BDConstants.LinkedNoteType? pNoteType, List<Guid> pObjectsOnPage)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pColumn.Uuid);
            List<BDLinkedNote> notes = new List<BDLinkedNote>();
            foreach (BDLinkedNoteAssociation link in links)
            {
                if (pNoteType != null)
                {
                    if (link.LinkedNoteType == pNoteType)
                    {
                        notes.Add(BDLinkedNote.RetrieveLinkedNoteWithId(pContext, link.linkedNoteId));
                        if (pObjectsOnPage != null)
                            pObjectsOnPage.Add(link.Uuid);
                    }
                }
                else
                    notes.Add(BDLinkedNote.RetrieveLinkedNoteWithId(pContext, link.linkedNoteId));
            }
            return notes;
        }

        private BDHtmlPage writeBDHtmlPage(Entities pContext, IBDNode pDisplayParentNode, StringBuilder pBodyHTML, BDConstants.BDHtmlPageType pPageType, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, string pParentKeyPropertyName)
        {
            return writeBDHtmlPage(pContext, pDisplayParentNode, pBodyHTML.ToString(), pPageType, pFootnotes, pObjectsOnPage, pParentKeyPropertyName);
        }

        /// <summary>
        /// Append footnotes, then wrap HTML with outer tags and save to db
        /// </summary>
        private BDHtmlPage writeBDHtmlPage(Entities pContext, IBDObject pDisplayParent, string pBodyHTML, BDConstants.BDHtmlPageType pPageType, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, string pParentKeyPropertyName)
        {
            StringBuilder footerHTML = new StringBuilder();
            // insert footer text
            if (pFootnotes.Count > 0)
            {
                footerHTML.Append(@"<h4>Footnotes</h4>");
                footerHTML.Append(@"<ol>");

                foreach (BDLinkedNote note in pFootnotes)
                {
                    footerHTML.AppendFormat(@"<li>{0}</li>", note.documentText);
                    pObjectsOnPage.Add(note.Uuid);
                }
                footerHTML.Append(@"</ol>");
                pBodyHTML = pBodyHTML + footerHTML.ToString();
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + pBodyHTML + bottomHtml;

            // the currentPageMasterObject will be null for the topmost page: this is expected as it has no parent
            Guid masterGuid = Guid.Empty;
            if (currentPageMasterObject != null)
                masterGuid = currentPageMasterObject.Uuid;

            Guid chapterId = (currentChapter == null) ? Guid.Empty : currentChapter.Uuid;

            BDNodeToHtmlPageIndex indexEntry = BDNodeToHtmlPageIndex.RetrieveOrCreateForIBDNodeId(pContext, masterGuid, pPageType, chapterId, pParentKeyPropertyName);

            string pageName = string.Empty;
            if (null != pDisplayParent)
            {
                IBDNode pageNode = BDFabrik.RetrieveNode(pContext, pDisplayParent.Uuid);
                if ((null != pageNode) && (!string.IsNullOrEmpty(pageNode.Name)))
                    pageName = pageNode.Name;
                else
                    pageName = generateLinkNameForPage(pContext, pageNode);
            }

            BDHtmlPage newPage = null;
            if (indexEntry != null)
                newPage = BDHtmlPage.RetrieveWithId(pContext, indexEntry.htmlPageId);
            if (newPage == null)
                newPage = BDHtmlPage.CreateBDHtmlPage(pContext, indexEntry.htmlPageId.Value /* Guid.NewGuid() */);

            newPage.name = pageName;

            newPage.displayParentType = pDisplayParent != null && pDisplayParent is IBDNode ? (int)((IBDNode)pDisplayParent).NodeType : (int)BDConstants.BDNodeType.Undefined;
            newPage.displayParentId = pDisplayParent != null ? pDisplayParent.Uuid : Guid.Empty;
            newPage.documentText = pageHtml;
            newPage.htmlPageType = (int)pPageType;
            newPage.layoutVariant = currentChapter != null ? (int)currentChapter.LayoutVariant : postProcessingPageLayoutVariant;
            if (currentPageMasterObject == null)
                newPage.pageTitle = string.Empty;
            else if (currentPageMasterObject is IBDNode)
                newPage.pageTitle = ((IBDNode)currentPageMasterObject).Name;
            else if (currentPageMasterObject is BDLinkedNote)
                newPage.pageTitle = currentPageMasterObject.DescriptionForLinkedNote;

            if ((currentChapter != null) && (newPage.layoutVariant == -1))
                Debug.WriteLine("Page has no layout assigned: {0}", newPage.Uuid);

            BDHtmlPage.Save(pContext, newPage);

            if (newPage.HtmlPageType == BDConstants.BDHtmlPageType.Data)
            {
                List<Guid> filteredObjects = pObjectsOnPage.Distinct().ToList();
                //         public static BDHtmlPageMap CreateOrRetrieveBDHtmlPageMap(Entities pContext, Guid pHtmlPageId, Guid pPageMapId, Guid pOriginalIBDObjectId)

                foreach (Guid objectId in filteredObjects)
                {
                    Guid dataPageMapGuid = Guid.NewGuid();
                    BDHtmlPageMap.CreateOrRetrieveBDHtmlPageMap(pContext, newPage.Uuid, dataPageMapGuid, objectId);
                }
            }
            else if (currentPageMasterObject != null && currentPageMasterObject.Uuid != Guid.Empty)
            {
                Guid navPageMapGuid = Guid.NewGuid();
                BDHtmlPageMap.CreateOrRetrieveBDHtmlPageMap(pContext, newPage.Uuid, navPageMapGuid, currentPageMasterObject.Uuid);
            }
            return newPage;
        }

        private BDHtmlPage writeLayoutBDHtmlPage(Entities pContext, BDLayoutMetadataColumn pLayoutColumn, string pBodyHTML, List<Guid> pObjectsOnPage, BDConstants.BDHtmlPageType pPageType)
        {
            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + pBodyHTML + bottomHtml;

            Guid chapterId = (currentChapter == null) ? Guid.Empty : currentChapter.Uuid;

            string pseudoPropertyName = null;
            if (pLayoutColumn.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01)
            {
                pseudoPropertyName = (overrideType == OverrideType.Paediatric) ? "Paediatric" : "Adult";
            }
            BDNodeToHtmlPageIndex indexEntry = BDNodeToHtmlPageIndex.RetrieveOrCreateForIBDNodeId(pContext, pLayoutColumn.Uuid, pPageType, chapterId, pseudoPropertyName);

            BDHtmlPage newPage = null;
            if (indexEntry != null)
                newPage = BDHtmlPage.RetrieveWithId(pContext, indexEntry.htmlPageId);
            if (newPage == null)
                newPage = BDHtmlPage.CreateBDHtmlPage(pContext, indexEntry.htmlPageId.Value);

            newPage.displayParentType = (int)BDConstants.BDNodeType.BDLayoutColumn;
            newPage.displayParentId = pLayoutColumn.Uuid;
            newPage.documentText = pageHtml;
            newPage.htmlPageType = (int)BDConstants.BDHtmlPageType.Comments;
            newPage.layoutVariant = currentChapter != null ? (int)currentChapter.LayoutVariant : postProcessingPageLayoutVariant;

            newPage.pageTitle = pLayoutColumn.label;
            newPage.name = pLayoutColumn.label;

            if (newPage.layoutVariant == -1)
                Debug.WriteLine("Page has no layout assigned: {0}", newPage.Uuid);

            BDHtmlPage.Save(pContext, newPage);

            // bypassing this filter for page types, since search entries can be created and should be honored for all output.
            //if (newPage.HtmlPageType == BDConstants.BDHtmlPageType.Data)
            //{
            List<Guid> filteredObjects = pObjectsOnPage.Distinct().ToList();
            foreach (Guid objectId in filteredObjects)
                BDHtmlPageMap.CreateOrRetrieveBDHtmlPageMap(pContext, Guid.NewGuid(), newPage.Uuid, objectId);
            //}
            return newPage;
        }

        #endregion
    }
    public class BDHtmlPageGeneratorLogEntry
    {
        public Guid Uuid { get; set; }
        public String Label { get; set; }
        public string Tag { get; set; }

        public BDHtmlPageGeneratorLogEntry(Guid? pUuid, String pLabel)
        {
            Uuid = (null == pUuid) ? Guid.Empty : pUuid.Value;
            Label = pLabel;
        }

        public BDHtmlPageGeneratorLogEntry(Guid? pUuid, String pLabel, string pTag)
        {
            Uuid = (null == pUuid) ? Guid.Empty : pUuid.Value;
            Label = pLabel;
            Tag = pTag;
        }

        public void AppendToFile(String pFilename)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outfile = new StreamWriter(Path.Combine(mydocpath, pFilename), true)) // overwrite the file if it exists
            {
                outfile.Write(this.ToString());
            }
        }

        static public void ClearFile(String pFilename)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullDocPath = Path.Combine(mydocpath, pFilename);
            File.WriteAllText(fullDocPath, string.Empty);
        }

        public override string ToString()
        {
            return string.Format(@"{0}\t{1}\t{2}", this.Uuid, this.Label, this.Tag);
        }

        static public void AppendToFile(string pFilename, string pStringData)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outfile = new StreamWriter(Path.Combine(mydocpath, pFilename), true)) // overwrite the file if it exists
            {
                outfile.WriteLine(pStringData);
            }
        }

        /// <summary>
        /// Write the log to disk, either appending to or overwriting existing
        /// </summary>
        /// <param name="pList"></param>
        /// <param name="pFolder"></param>
        /// <param name="pFilename"></param>
        /// <param name="pAppend">False: Overwrite</param>
        static public void WriteToFile(List<BDHtmlPageGeneratorLogEntry> pList, string pFolder, String pFilename, bool pAppend)
        {
            StringBuilder sbReferencePages = new StringBuilder();
            foreach (BDHtmlPageGeneratorLogEntry entry in pList)
            {
                sbReferencePages.AppendLine(entry.ToString());
            }

            using (StreamWriter outfile = new StreamWriter(Path.Combine(pFolder, pFilename), pAppend)) // overwrite the file if it exists
            {
                outfile.Write(sbReferencePages.ToString());
            }
        }

        /// <summary>
        /// Write the log to disk. Overwrite existing
        /// </summary>
        /// <param name="pList"></param>
        /// <param name="pFolder"></param>
        /// <param name="pFilename"></param>
        static public void WriteToFile(List<BDHtmlPageGeneratorLogEntry> pList, string pFolder, String pFilename)
        {
            BDHtmlPageGeneratorLogEntry.WriteToFile(pList, pFolder, pFilename, false);
        }

        /// <summary>
        /// Write list to filename. Use myDocuments folder
        /// </summary>
        /// <param name="pList"></param>
        /// <param name="pFilename"></param>
        static public void WriteToFile(List<BDHtmlPageGeneratorLogEntry> pList, String pFilename)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            BDHtmlPageGeneratorLogEntry.WriteToFile(pList, mydocpath, pFilename, false);
        }
    }
}
