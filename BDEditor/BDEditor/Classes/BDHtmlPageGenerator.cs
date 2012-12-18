using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

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

        private const int maxNodeType = 6;
        private const string topHtml = @"<html><head><meta http-equiv=""Content-type"" content=""text/html;charset=UTF-8\""/><meta name=""viewport"" content=""width=device-width; initial-scale=1.0; maximum-scale=1.0;""/><link rel=""stylesheet"" type=""text/css"" href=""bdviewer.css"" /> </head><body>";
        private const string bottomHtml = @"</body></html>";
        private const string anchorTag = @"<p><a href=""{0}""><b>{1}</b></a></p>";
        public const int EMPTY_PARAGRAPH = 8;  // <p> </p>
        private const string imgFileTag = "<img src=\"images/{0}{1}\" alt=\"\" width=\"300\" height=\"300\" />";
        private const string paintChipTag = "<img class=\"paintChip\" src=\"{0}\" alt=\"\" />";
        private const string PAINT_CHIP_ANTIBIOTICS = "AntibioticYellow.png";
        private const string PAINT_CHIP_DENTISTRY = "DentistryPurple.png";
        private const string PAINT_CHIP_ORGANISMS = "OrganismGreen.png";
        private const string PAINT_CHIP_TREATMENT_PAEDIATRIC = "PaediatricTreatmentLightBlue.png";
        private const string PAINT_CHIP_TREATMENT_ADULT = "TreatmentBlue.png";
        private const string PAINT_CHIP_PREGNANCY = "PregnancyRed.png";
        private const string PAINT_CHIP_PROPHYLAXIS = "ProphylaxisOrange.png";
        private const string LEFT_SQUARE_BRACKET = "&#91";
        private const string RIGHT_SQUARE_BRACKET = "&#93";

        //private List<BDLayoutMetadataColumn> metadataLayoutColumns = new List<BDLayoutMetadataColumn>();
        private List<BDHtmlPageMap> pagesMap = new List<BDHtmlPageMap>();
        private IBDNode currentChapter = null;
        private IBDObject currentPageMasterObject = null;
        private int? postProcessingPageLayoutVariant = (int)BDConstants.LayoutVariantType.Undefined;

        // create variables to hold data for 'same as previous' settings on Therapy
        string previousTherapyName = string.Empty;
        string previousTherapyDosage = string.Empty;
        string previousTherapyDosage1 = string.Empty;
        string previousTherapyDosage2 = string.Empty;
        string previousTherapyDuration = string.Empty;
        string previousTherapyDuration1 = string.Empty;
        string previousTherapyDuration2 = string.Empty;
        Guid previousTherapyId = Guid.Empty;
        bool therapiesHaveName = false;
        bool therapiesHaveDosage = false;
        bool therapiesHaveDuration = false;

        private OverrideType overrideType = OverrideType.Undefined;

        //ks
        private List<BDHtmlPageGeneratorLogEntry> referencePageUuidList = new List<BDHtmlPageGeneratorLogEntry>();

        public List<BDHtmlPageMap> PagesMap
        {
            get { return pagesMap; }
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
            BDHtmlPage.DeleteAll();

            // reset index entries to false
            BDNodeToHtmlPageIndex.ResetForRegeneration(pContext);

            generatePages(pContext, pNodeList);

            List<BDHtmlPage> pages = BDHtmlPage.RetrieveAll(pContext);
            List<Guid> displayParentIds = BDHtmlPage.RetrieveAllDisplayParentIDs(pContext);
            List<Guid> pageIds = BDHtmlPage.RetrieveAllIds(pContext);
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditPageMap.txt", "----------------------");
            foreach (BDHtmlPageMap pageMap in pagesMap)
            {
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditPageMap.txt", string.Format("{0}\t{1}", pageMap.OriginalIBDObjectId, pageMap.HtmlPageId));
            }

            Debug.WriteLine("Post-processing HTML pages");
            foreach (BDHtmlPage page in pages)
            {
                processTextForInternalLinks(pContext, page, displayParentIds, pageIds);
                processTextForSubscriptAndSuperscriptMarkup(pContext, page);
                processTextForCarriageReturn(pContext, page);
            }

            #region Output logs

            //ks: Write out all the verbose logs
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Reference Pages
            BDHtmlPageGeneratorLogEntry.WriteToFile(this.referencePageUuidList, mydocpath, @"ReferencePageUuidList.txt");

            #endregion

        }

        private void generatePages(Entities pContext, List<BDNode> pNodeList /* IBDNode pNode */)
        {
            //List<BDNode> chapters = BDNode.RetrieveNodesForType(pContext, BDConstants.BDNodeType.BDChapter);
            List<BDHtmlPage> allPages = new List<BDHtmlPage>();

            List<BDNode> chapters = pNodeList ?? BDNode.RetrieveNodesForType(pContext, BDConstants.BDNodeType.BDChapter);

            List<BDHtmlPage> childDetailPages = new List<BDHtmlPage>();
            List<BDHtmlPage> childNavPages = new List<BDHtmlPage>();

            foreach (BDNode chapter in chapters)
            {
                currentChapter = chapter;
                generateOverviewAndChildrenForNode(pContext, chapter, childDetailPages, childNavPages);
            }
            if (childDetailPages.Count > 0)
                allPages.AddRange(childDetailPages);
            if (childNavPages.Count > 0)
                allPages.AddRange(childNavPages);

            /*
            if (pNode == null)
            {
                List<BDHtmlPage> childDetailPages = new List<BDHtmlPage>();
                List<BDHtmlPage> childNavPages = new List<BDHtmlPage>();
                foreach (BDNode chapter in chapters)
                {
                    currentChapter = chapter;
                    generateOverviewAndChildrenForNode(pContext, chapter, childDetailPages, childNavPages);
                }
                if(childDetailPages.Count > 0)
                    allPages.AddRange(childDetailPages);
                if (childNavPages.Count > 0)
                    allPages.AddRange(childNavPages);
            }
            else
            {
                List<BDHtmlPage> childDetailPages = new List<BDHtmlPage>();
                List<BDHtmlPage> childNavPages = new List<BDHtmlPage>();
                if (pNode.NodeType == BDConstants.BDNodeType.BDChapter)
                {
                    currentChapter = pNode;
                    generateOverviewAndChildrenForNode(pContext, pNode, childDetailPages, childNavPages);
                }
                allPages.AddRange(childDetailPages);
                allPages.AddRange(childNavPages);
            }
             * */
            List<BDHtmlPage> chapterPages = allPages.Distinct().ToList();
            Debug.WriteLine("Creating home page with filtered distinct list");
            if (chapterPages.Count > 0)
                generateNavigationPage(pContext, null, chapterPages);
        }

        /// <summary>
        /// Recursive method that traverses the navigation tree
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        private void generateOverviewAndChildrenForNode(Entities pContext, IBDNode pNode, List<BDHtmlPage> pNodeDetailPages, List<BDHtmlPage> pNodeNavPages)
        {
            // hack to compensate for lack of differentiation in early layout variants
            if (pNode.Uuid == Guid.Parse("c0ecedc1-70cf-4422-b998-7e5f2bb986b1"))
            {
                overrideType = OverrideType.Paediatric;
            }
            else if (pNode.Uuid == Guid.Parse("757409a4-9446-4aa5-ac23-03fb7660759b"))
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
                   // if((int)child.NodeType < maxNodeType) - for debugging recursive call
                    generateOverviewAndChildrenForNode(pContext, child, childDetailPages, childNavPages);
                }
                // we are NOT on a leaf node, still on a navigation level
                // generate page for 'n' level, with list of navigation children that was returned
                if (childDetailPages.Count > 0)
                {
                    Debug.WriteLine("Detail page with {0} children for: {1}: {2}", childDetailPages.Count, pNode.NodeType.ToString(), pNode.Name);
                    pNodeDetailPages.Add(generateNavigationPage(pContext, pNode, childDetailPages));
                }
                if (childNavPages.Count > 0)
                {
                    Debug.WriteLine("Navigation page with {0} children for: {1}: {2}", childNavPages.Count, pNode.NodeType.ToString(), pNode.Name);
                    pNodeNavPages.Add(generateNavigationPage(pContext, pNode, childNavPages));
                }
            }
        }

        private BDHtmlPage generateNavigationPage(Entities pContext, IBDNode pNode, List<BDHtmlPage> pChildPages)
        {
            currentPageMasterObject = pNode;
            StringBuilder pageHTML = new StringBuilder();
            List<Guid> objectsOnPage = new List<Guid>();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            if (pNode != null)
            {
                pageHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(2), footnotesOnPage, objectsOnPage));

                //ks: consistently manage the title and notes
                /*
                footnotesOnPage.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));
                string footnoteMarkers = buildFooterMarkerForList(footnotesOnPage, true, footnotesOnPage, objectsOnPage);
                if (footnotesOnPage.Count > 0)
                    pageHTML.AppendFormat(@"<h2>{0}{1}</h2>", pNode.Name, footnoteMarkers);
                else
                    pageHTML.AppendFormat(@"<h2>{0}</h2>", pNode.Name);

                string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid, objectsOnPage);
                if (noteText.Length > EMPTY_PARAGRAPH)
                {
                    pageHTML.Append(noteText);
                }
                objectsOnPage.Add(pNode.Uuid);

                // add text for other linked note types
                List<BDLinkedNote> noteList = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
                noteList.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment));
                noteList.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment));
                foreach (BDLinkedNote note in noteList)
                {
                    pageHTML.Append(note.documentText);
                    objectsOnPage.Add(note.Uuid);
                }
                */

                // TODO:  build javascript blocks to expand/collapse overview
                foreach (BDHtmlPage page in pChildPages)
                {
                    if (page != null)
                    {
                        BDNode childNode = BDNode.RetrieveNodeWithId(pContext, page.displayParentId.Value);
                        if (childNode != null)
                            pageHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", page.Uuid.ToString().ToUpper(), childNode.Name);
                    }
                }
            }
            else  // this is the main page of the app
            {
                currentChapter = null;
                pageHTML.Append("<table>");
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
                                case BDConstants.LayoutVariantType.Microbiology:
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
                            pageHTML.Append("<table>");
                            pageHTML.AppendFormat(@"<tr class=""nav""><td>{0}</td><td><a href=""{1}""><b>{2}</b></a></td></tr>", paintChipHtml, childPage.Uuid.ToString().ToUpper(), childNode.Name);
                        }
                    }
                }
                pageHTML.Append("</table>");
            }

            return writeBDHtmlPage(pContext, pNode as BDNode, pageHTML, BDConstants.BDHtmlPageType.Navigation, footnotesOnPage, objectsOnPage, null);
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
                            //nodeChildPages.Add(generatePageForAntibioticsClinicalGuidelines(pContext, pNode as BDNode));
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
                            generatePageForAttachment(pContext, pNode);

                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsPharmacodynamics(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
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
                        case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForOrganismGramStain(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
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
                case BDConstants.BDNodeType.BDDisease:
                    {
                        switch (pNode.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
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
                                    //nodeChildPages.Add(generatePageForEmpiricTherapyDisease(pContext, pNode as BDNode));
                                    nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                                }
                                else
                                    isPageGenerated = false;
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                                currentPageMasterObject = pNode;
                                //nodeChildPages.Add(generatePageForProphylaxisCommunicableDiseases(pContext, pNode));
                                nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                                isPageGenerated = true;
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                                currentPageMasterObject = pNode;
                                //nodeChildPages.Add(generatePageForProphylaxisCommunicableDiseases(pContext, pNode));
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
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfBCNE(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfParasitic(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfCultureDirected(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                        //case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics: //ks: pathogen with this layout variant is not possible in BDFabrik 
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfCultureDirectedEndocarditis(pContext, pNode as BDNode));
                            nodeChildPages.Add((GenerateBDHtmlPage(pContext, pNode)));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogenGroup:
                    Debug.WriteLine("BONK - BDPathogenGroup");
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfCultureDirectedPeritonitis(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
/* DONE */     case BDConstants.BDNodeType.BDPresentation:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            // if the processing comes through here, then the Disease has > 1 Presentation 
                            isPageGenerated = true;
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyPresentation(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            break;
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
                    Debug.WriteLine("BONK - BDResponse");
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
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsStepdown(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageforAntibioticsBLactam(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsNameListing(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_IERecommendation:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForProphylaxisEndocarditis(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForProphylaxisInfectionPrevention(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForPLCommunicableDiseases(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForPLPerinatalHIVProtocol(pContext, pNode));
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
                            isPageGenerated = true;
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsDosingAndDailyCosts(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfPneumonia(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForDentalProphylaxisDrugRegimens(pContext, pNode));
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
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubtopic:
                    switch (pNode.LayoutVariant)
                    {
                        //case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
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
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgeryClassification:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSurgeryGroup:
                    switch (pNode.LayoutVariant)
                    {
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTable:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibiotics_HepaticImpairmentGrading(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsCSFPenetration(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            currentPageMasterObject = pNode;
                            //ks:
                            //nodeChildPages.Add(generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForEmpiricTherapyOfEndocarditisPaediatrics(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForProphylaxisFluidExposureRiskOfInfection(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForProphylaxisFluidExposureFollowupProtocolI(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForProphylaxisFluidExposureFollowupProtocolII(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntimicrobialAgentsForOralMicroorganisms(pContext, pNode));
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
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsClinicalGuidelinesSpectrum(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForAntibioticsCSFPenetration(pContext, pNode as BDNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfFungalInfections(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForDentalProphylaxis(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                            currentPageMasterObject = pNode;
                            //nodeChildPages.Add(generatePageForProhylaxisSexualAssault(pContext, pNode));
                            nodeChildPages.Add(GenerateBDHtmlPage(pContext, pNode));
                            isPageGenerated = true;
                            break;

                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;

                case BDConstants.BDNodeType.BDChapter:
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

        private BDHtmlPage generateStub(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));
            
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        #region Antibiotics sections
        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForAntibioticsClinicalGuidelines(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDAntimicrobial)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            // child nodes can either be pathogen groups or topics (node with overview)
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode childNode in childNodes)
            {
                if (childNode.NodeType == BDConstants.BDNodeType.BDTopic)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, childNode, "h2", footnotes, objectsOnPage));
                }
                objectsOnPage.Add(childNode.Uuid);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        private BDHtmlPage generatePageForAntibioticsClinicalGuidelinesSpectrum(Entities pContext, BDNode pNode)
{
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTopic)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in childNodes)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, "h3", footnotes, objectsOnPage));
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
}

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForAntibioticsPharmacodynamics(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode node in childNodes)
            {
                if (node.NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
                    bodyHTML.AppendFormat("{0}<br>",buildNodeWithReferenceAndOverviewHTML(pContext, node, string.Empty, footnotes, objectsOnPage));
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForAntibioticsDosingAndDailyCosts(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

                string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], pNode.NodeType, BDNode.PROPERTYNAME_NAME, footnotes, objectsOnPage);
                string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, footnotes, objectsOnPage);
                string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_COST, footnotes, objectsOnPage);

            List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (antimicrobials.Count > 0)
            {
                bodyHTML.AppendFormat(@"<table><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr>", c1Html, c2Html, c3Html);

                foreach (IBDNode antimicrobial in antimicrobials)
                {
                    string amHtml = buildNodePropertyHTML(pContext, antimicrobial, antimicrobial.Name, BDNode.PROPERTYNAME_NAME, footnotes, objectsOnPage);
                    List<BDLinkedNote> amFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, antimicrobial.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
                    string amFooterMarker = buildFooterMarkerForList(amFooters, true, footnotes, objectsOnPage);
                    // build each row of table, with antimicrobial name in first column
                    bodyHTML.AppendFormat(@"<tr class=v""{0}""><td>{1}</td>", (int)antimicrobial.LayoutVariant, amHtml);

                    StringBuilder dosageHTML = new StringBuilder();
                    dosageHTML.Append("<td>");
                    StringBuilder costHTML = new StringBuilder();
                    costHTML.Append("<td>");

                    List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, antimicrobial);
                    foreach (IBDNode child in childNodes)
                    {
                        if (child.NodeType == BDConstants.BDNodeType.BDTopic)
                        {
                            dosageHTML.AppendFormat("<u>{0}</u><br>", buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false, footnotes, objectsOnPage));
                            costHTML.Append("<br>");
                            List<IBDNode> lvl1Children = BDFabrik.GetChildrenForParent(pContext, child);
                            foreach (IBDNode lvl1Child in lvl1Children)
                            {
                                // BDDosageGroup
                                dosageHTML.AppendFormat("<b>{0}</b><br>", buildCellHTML(pContext, lvl1Child, BDNode.PROPERTYNAME_NAME, lvl1Child.Name, false, footnotes, objectsOnPage));
                                costHTML.Append("<br>");

                                List<IBDNode> lvl2Children = BDFabrik.GetChildrenForParent(pContext, lvl1Child);
                                string cellLineTag = (lvl2Children.Count > 0) ? "<br>" : "";
                                foreach (IBDNode lvl2Child in lvl2Children)
                                {
                                    // BDDosage
                                    BDDosage dosage = lvl2Child as BDDosage;
                                    if (dosage.joinType == (int)BDConstants.BDJoinType.Next)
                                        dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), cellLineTag);
                                    else
                                        dosageHTML.AppendFormat("{0} {1}{2}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), retrieveConjunctionString((int)dosage.joinType), cellLineTag);
                                     
                                    costHTML.Append(buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_COST, dosage.cost, false, footnotes, objectsOnPage));
                                    if (dosage.cost2.Length > 0) 
                                            costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_COST2, dosage.cost2, false, footnotes, objectsOnPage), cellLineTag);
                                    else
                                        costHTML.Append(cellLineTag);
                                }
                            }
                        }
                        else if (child.NodeType == BDConstants.BDNodeType.BDDosageGroup)
                        {
                            dosageHTML.AppendFormat("<b>{0}</b><br>", buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false, footnotes, objectsOnPage));
                            costHTML.Append("<br>");

                            List<IBDNode> lvl2Children = BDFabrik.GetChildrenForParent(pContext, child);
                            string cellLineTag = (lvl2Children.Count > 0) ? "<br>" : "";
                            foreach (IBDNode lvl2Child in lvl2Children)
                            {
                                // BDDosage
                                BDDosage dosage = lvl2Child as BDDosage;
                                if (dosage.joinType == (int)BDConstants.BDJoinType.Next)
                                    dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), cellLineTag);
                                else
                                    dosageHTML.AppendFormat("{0} {1}{2}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), retrieveConjunctionString((int)dosage.joinType), cellLineTag);
                                costHTML.Append(buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_COST, dosage.cost, false, footnotes, objectsOnPage));
                                if (dosage.cost2.Length > 0)
                                    costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_COST2, dosage.cost2, false, footnotes, objectsOnPage), cellLineTag);
                                else
                                    costHTML.Append(cellLineTag);
                            }
                        }
                        else if (child.NodeType == BDConstants.BDNodeType.BDDosage)
                        {
                            string cellLineTag = (childNodes.Count > 0) ? "<br>" : "";

                            BDDosage dosage = child as BDDosage;
                            if (dosage.joinType == (int)BDConstants.BDJoinType.Next)
                                dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), cellLineTag);
                            else
                                dosageHTML.AppendFormat("{0} {1}{2}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), retrieveConjunctionString((int)dosage.joinType), cellLineTag);
                            costHTML.Append(buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_COST, dosage.cost, false, footnotes, objectsOnPage));
                            if (dosage.cost2.Length > 0)
                                costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_COST2, dosage.cost2, false, footnotes, objectsOnPage), cellLineTag);
                            else
                                costHTML.Append(cellLineTag);
                        }
                    }
                    dosageHTML.Append("</td>");
                    costHTML.Append("</td>");
                    bodyHTML.AppendFormat(@"{0}{1}</tr>", dosageHTML, costHTML);
                }
                bodyHTML.Append(@"</table>");

                List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.parentId.Value, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                if (legendHTML.Length > EMPTY_PARAGRAPH)
                    bodyHTML.Append(legendHTML);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }


        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForAntibioticsDosingAndMonitoring(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubsection && pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                foreach (IBDNode node in childNodes)
                {
                    if (node.NodeType == BDConstants.BDNodeType.BDTopic)
                    {
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, node, "h3", footnotes, objectsOnPage));

                        List<IBDNode> topicChildren = BDFabrik.GetChildrenForParent(pContext, node);
                        foreach (IBDNode topicChild in topicChildren)
                        {
                            if (topicChild.NodeType == BDConstants.BDNodeType.BDTable)
                            {
                                // insert node name (table name)
                                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topicChild, "h4", footnotes, objectsOnPage));

                                int columnCount = BDFabrik.GetTableColumnCount(topicChild.LayoutVariant);
                                List<IBDNode> tableChildren = BDFabrik.GetChildrenForParent(pContext, topicChild);
                                if (tableChildren.Count > 0)
                                {
                                    bodyHTML.Append(@"<table>");
                                    foreach (IBDNode child in tableChildren)
                                    {
                                        if (child.NodeType == BDConstants.BDNodeType.BDTableRow)
                                        {
                                            BDTableRow row = child as BDTableRow;
                                            if (row != null)
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                                        }
                                        else
                                        {
                                            if (child.NodeType == BDConstants.BDNodeType.BDTableSection)
                                                bodyHTML.AppendFormat(@"<tr><td colspan={0}>{1}</td></tr>", columnCount, child.Name);
                                            List<IBDNode> sectionChildren = BDFabrik.GetChildrenForParent(pContext, child);
                                            foreach (IBDNode sectionChild in sectionChildren)
                                            {
                                                if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                                {
                                                    BDTableRow row = sectionChild as BDTableRow;
                                                    bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                                                }
                                                else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                                                {
                                                    bodyHTML.AppendFormat(@"<tr><td colspan={0}><u>{1}</u></td></tr>", columnCount, sectionChild.Name);
                                                    List<IBDNode> ssChildren = BDFabrik.GetChildrenForParent(pContext, sectionChild);
                                                    foreach (IBDNode ssChild in ssChildren)
                                                    {
                                                        BDTableRow ssRow = ssChild as BDTableRow;
                                                        if (ssRow != null)
                                                            bodyHTML.Append(buildTableRowHtml(pContext, ssRow, false, true, footnotes, objectsOnPage));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    bodyHTML.Append(@"</table>");

                                    List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, topicChild.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                                    string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                                    if (legendHTML.Length > EMPTY_PARAGRAPH)
                                        bodyHTML.Append(legendHTML);

                                }
                            }
                            else if (topicChild.NodeType == BDConstants.BDNodeType.BDSubtopic)
                            {
                                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topicChild, "h4", footnotes, objectsOnPage));
                                
                                List<IBDNode> subtopicChildren = BDFabrik.GetChildrenForParent(pContext, topicChild);
                                foreach (IBDNode subtopicChild in subtopicChildren)
                                {
                                    if (subtopicChild.NodeType == BDConstants.BDNodeType.BDTable)
                                    {
                                        int columnCount = BDFabrik.GetTableColumnCount(subtopicChild.LayoutVariant);
                                        // insert node name (table name)
                                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subtopicChild, "h4", footnotes, objectsOnPage));

                                        List<IBDNode> tableChildren = BDFabrik.GetChildrenForParent(pContext, subtopicChild);
                                        if (tableChildren.Count > 0)
                                        {
                                            bodyHTML.Append(@"<table>");
                                            foreach (IBDNode child in tableChildren)
                                            {
                                                if (child.NodeType == BDConstants.BDNodeType.BDTableRow)
                                                {
                                                    BDTableRow row = child as BDTableRow;
                                                    if (row != null)
                                                        bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                                                }
                                                else
                                                {
                                                    if (child.NodeType == BDConstants.BDNodeType.BDTableSection)
                                                        bodyHTML.AppendFormat(@"<tr><td colspan={0}>{1}</td></tr>", columnCount, child.Name);
                                                    List<IBDNode> sectionRows = BDFabrik.GetChildrenForParent(pContext, child);
                                                    foreach (IBDNode sectionRow in sectionRows)
                                                    {
                                                        if (sectionRow.NodeType == BDConstants.BDNodeType.BDTableRow)
                                                        {
                                                            BDTableRow row = sectionRow as BDTableRow;
                                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                                                        }
                                                    }
                                                }
                                            }
                                            bodyHTML.Append(@"</table>");
                                        }
                                    }
                                }
                            }
                            else if (topicChild.NodeType == BDConstants.BDNodeType.BDAttachment)
                            {
                                bodyHTML.Append(buildAttachmentHTML(pContext, topicChild, footnotes, objectsOnPage));

                                List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, topicChild.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                                string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                                if (legendHTML.Length > EMPTY_PARAGRAPH)
                                    bodyHTML.Append(legendHTML);
                            }
                        }
                    }
                    else if (node.NodeType == BDConstants.BDNodeType.BDAttachment)
                    {
                        bodyHTML.Append(buildAttachmentHTML(pContext, node, footnotes, objectsOnPage));
                        List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, node.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                        string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                        if (legendHTML.Length > EMPTY_PARAGRAPH)
                            bodyHTML.Append(legendHTML);

                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForAntibioticsDosingInRenalImpairment(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            // Category > Antimicrobial with Dosage in a table
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotes, objectsOnPage));
            
            // child nodes are BDAntimicrobial 
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], pNode.NodeType, BDNode.PROPERTYNAME_NAME, footnotes, objectsOnPage);
                string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, footnotes, objectsOnPage);
                string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE2, footnotes, objectsOnPage);

                bodyHTML.AppendFormat(@"<table class=""v{0}""><tr><th rowspan=4>{1}</th><th rowspan=4>{2}</th>", pNode.layoutVariant, c1Html, c2Html);
                bodyHTML.Append(@"<th colspan=3><b>Dose and Interval Adjustment for Renal Impairment</b></th></tr>");
                bodyHTML.AppendFormat(@"<tr><th colspan=""3""><b>{0}</b></th><tr>",c3Html);
                bodyHTML.Append(@"<tr><th>&gt50</th><th>10 - 50</th><th>&lt10(Anuric)</th></tr>");

                foreach (IBDNode antimicrobial in childNodes) 
                {
                    List<IBDNode> dosageNodes = BDFabrik.GetChildrenForParent(pContext, antimicrobial);
                    string antimicrobialHtml = buildNodePropertyHTML(pContext, antimicrobial, antimicrobial.Name, BDNode.PROPERTYNAME_NAME, footnotes, objectsOnPage);
                    bodyHTML.AppendFormat(@"<tr class=""v{0}""><td>{1}</td>", (int)antimicrobial.LayoutVariant, antimicrobialHtml);

                    string dosageGroupName = string.Empty;
                    string rowStartTag = string.Format(@"<tr class=""v{0}""><td />", (int)antimicrobial.LayoutVariant);
                    string rowEndTag = "</tr>";
                    bool isFirstRow = true;
                    foreach (IBDNode dNode in dosageNodes)
                    {
                        if (dNode.NodeType == BDConstants.BDNodeType.BDDosage)
                        {
                            if (!isFirstRow) bodyHTML.Append(rowStartTag);
                            bodyHTML.Append(buildDosageHTML(pContext, dNode, dosageGroupName, footnotes, objectsOnPage));
                            bodyHTML.Append(rowEndTag);
                            isFirstRow = false;
                        }
                        else // BDDosageGroup
                        {
                            if (!isFirstRow) bodyHTML.Append(rowStartTag);
                            dosageGroupName = buildNodePropertyHTML(pContext, dNode, dNode.Uuid, dNode.Name, BDNode.PROPERTYNAME_NAME, "u", footnotes, objectsOnPage);
                            List<IBDNode> dgChildren = BDFabrik.GetChildrenForParent(pContext, dNode);
                            foreach (IBDNode dgChild in dgChildren)
                            {
                                bodyHTML.Append(buildDosageHTML(pContext, dgChild, dosageGroupName, footnotes, objectsOnPage));
                            }
                            isFirstRow = false;
                            bodyHTML.Append(rowEndTag);
                        }
                    bodyHTML.Append("</tr>");
                    }
                }
                bodyHTML.Append(@"</table>");

                List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                if (legendHTML.Length > EMPTY_PARAGRAPH)
                    bodyHTML.Append(legendHTML);

            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        private BDHtmlPage generatePageForAntibioticDosingInHepaticImpairment(Entities pContext, BDNode pNode)
        {
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));
            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                // start table html
                bodyHTML.Append(@"<table><tr><th>Antimicrobial</th><th>Dosage Adjustment</th></tr>");
                foreach (IBDNode child in childNodes)
                {
                    //child is antimicrobial with overview:  add a row
                    BDNode node = child as BDNode;

                    bodyHTML.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>", child.Name, retrieveNoteTextForOverview(pContext, child.Uuid, objectsOnPage));
                }
                bodyHTML.Append(@"</table>");

                List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                if (legendHTML.Length > EMPTY_PARAGRAPH)
                    bodyHTML.Append(legendHTML);

            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForAntibiotics_HepaticImpairmentGrading(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            string c1Html = buildHtmlForMetadataColumn(pContext, pNode as BDNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, footnotes, objectsOnPage);
            string c2Html = buildHtmlForMetadataColumn(pContext, pNode as BDNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, footnotes, objectsOnPage);
            string c3Html = buildHtmlForMetadataColumn(pContext, pNode as BDNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, footnotes, objectsOnPage);
            string c4Html = buildHtmlForMetadataColumn(pContext, pNode as BDNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, footnotes, objectsOnPage);

            bodyHTML.AppendFormat(@"<table><tr><th>{0}</th><th>{1}</th><th>{2}</th><th>{3}</th></tr>", c1Html, c2Html, c3Html, c4Html);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode childNode in childNodes)
            {
                // children are BDConfiguredEntry
                BDConfiguredEntry entry = childNode as BDConfiguredEntry;
                bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", entry.Name, entry.field01, entry.field02, entry.field03);
            }
            bodyHTML.Append("</table>"); 
            
            List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
            string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
            if (legendHTML.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(legendHTML);

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForAntibioticsNameListing(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            //metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));
            List<IBDNode> tables = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode table in tables)
            {
                List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, table);
                if (childNodes.Count > 0)
                {
                    bodyHTML.Append(@"<table>");
                    foreach (IBDNode node in childNodes)
                    {
                        if (node.NodeType == BDConstants.BDNodeType.BDTableRow)
                        {
                            BDTableRow row = node as BDTableRow;
                            if (row != null)
                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                        }
                        else if (node.NodeType == BDConstants.BDNodeType.BDTableSection)
                        {
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan=3><b>{0}</b><td></tr>", node.Name);
                            List<IBDNode> sectionChildren = BDFabrik.GetChildrenForParent(pContext, node);
                            if (sectionChildren.Count > 0)
                            {
                                foreach (IBDNode sectionChild in sectionChildren)
                                {
                                    if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                                    {
                                        if (sectionChild.Name.Length > 0)
                                        {
                                            bodyHTML.AppendFormat(@"<tr><td colspan=3><nbsp><nbsp><u>{0}</u><td></tr>", sectionChild.Name);
                                            List<BDTableRow> rows = BDTableRow.RetrieveTableRowsForParentId(pContext, sectionChild.Uuid);
                                            foreach (BDTableRow row in rows)
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                                        }
                                    }
                                    else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = sectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                                    }
                                }
                            }

                        }
                        else if (node.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                        {
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan=3>{0}<td></tr>", node.Name);
                        }
                    }
                }
                bodyHTML.Append(@"</table>");

                List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, table.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                if (legendHTML.Length > EMPTY_PARAGRAPH)
                    bodyHTML.Append(legendHTML);

            }

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtml instead")]
        private BDHtmlPage generatePageForAntibioticsStepdown(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            //metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                bodyHTML.Append(@"<table>");
                foreach (IBDNode tableNode in childNodes)
                {
                    List<IBDNode> tableChildren = BDFabrik.GetChildrenForParent(pContext, tableNode);
                    foreach (IBDNode node in tableChildren)
                    {
                        if (node.NodeType == BDConstants.BDNodeType.BDTableRow)
                        {
                            BDTableRow row = node as BDTableRow;
                            if (row != null)
                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                        }
                        else if (node.NodeType == BDConstants.BDNodeType.BDTableSection)
                        {
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan=5><i>{0}</i><td></tr>", node.Name);
                            List<IBDNode> sectionChildren = BDFabrik.GetChildrenForParent(pContext, node);
                            if (sectionChildren.Count > 0)
                            {
                                foreach (IBDNode sectionChild in sectionChildren)
                                {
                                    if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                                    {
                                        if (sectionChild.Name.Length > 0)
                                        {
                                            bodyHTML.AppendFormat(@"<tr><td colspan=5>{0}<td></tr>", sectionChild.Name);
                                            List<BDTableRow> rows = BDTableRow.RetrieveTableRowsForParentId(pContext, sectionChild.Uuid);
                                            foreach (BDTableRow row in rows)
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                                        }
                                    }
                                    else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = sectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, false, footnotes, objectsOnPage));
                                    }
                                }
                            }

                        }
                        else if (node.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                        {
                            //TODO:  Make fontsize smaller than antimicrobialSection name
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan=5>{0}<td></tr>", node.Name);
                            List<IBDNode> subsectionChildren = BDFabrik.GetChildrenForParent(pContext, node);
                            if (subsectionChildren.Count > 0)
                            {
                                foreach (IBDNode subsectionChild in subsectionChildren)
                                {
                                    if (subsectionChild.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                                    {
                                        if (subsectionChild.Name.Length > 0)
                                        {
                                            bodyHTML.AppendFormat(@"<tr><td colspan=5>{0}<td></tr>", subsectionChild.Name);
                                            List<BDTableRow> rows = BDTableRow.RetrieveTableRowsForParentId(pContext, subsectionChild.Uuid);
                                            foreach (BDTableRow row in rows)
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true, footnotes, objectsOnPage));
                                        }
                                    }
                                    else if (subsectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = subsectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, false, footnotes, objectsOnPage));
                                    }
                                }
                            }
                        }
                    }
                }
                bodyHTML.Append(@"</table>");

                List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                if (legendHTML.Length > EMPTY_PARAGRAPH)
                    bodyHTML.Append(legendHTML);

            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtml instead")]
        private BDHtmlPage generatePageForAntibioticsCSFPenetration(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
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

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in childNodes)
            {
                if (child.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_CSFPenetration)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "h3", footnotesOnPage, objectsOnPage));

                    List<IBDNode> categories = BDFabrik.GetChildrenForParent(pContext, child);
                    foreach (IBDNode category in categories)
                    {
                        bodyHTML.Append(buildNodePropertyHTML(pContext, category, category.Name, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                        bodyHTML.Append("<table><tr><th>Excellent Penetration</th><th>Good Penetration</th><th>Poor Penetration</th></tr>");

                        List<IBDNode> subcategories = BDFabrik.GetChildrenForParent(pContext, category);
                        if (subcategories.Count > 0)
                        {
                            bodyHTML.Append(@"<tr>");
                            foreach (IBDNode column in subcategories)
                            {
                                objectsOnPage.Add(column.Uuid);
                                bodyHTML.Append(@"<td>");
                                // build columns
                                List<IBDNode> columnDetail = BDFabrik.GetChildrenForParent(pContext, column);
                                if (columnDetail.Count > 0)
                                {
                                    StringBuilder colHTML = new StringBuilder();
                                    foreach (IBDNode antimicrobial in columnDetail)
                                    {
                                        string antimicrobialHTML = buildNodePropertyHTML(pContext, antimicrobial, antimicrobial.Name, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage);
                                        colHTML.AppendFormat(@"{0}<br>", antimicrobialHTML);
                                    }
                                    bodyHTML.Append(colHTML);
                                }
                                bodyHTML.Append(@"</td>");
                            }
                            bodyHTML.Append(@"</tr>");
                        }
                        bodyHTML.Append(@"</table>");

                        List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, category.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                        string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                        if (legendHTML.Length > EMPTY_PARAGRAPH)
                            bodyHTML.Append(legendHTML);
                    }
                }
                else
                    bodyHTML.Append(buildAntibioticsCSFPenetrationDosagesHTML(pContext, child, footnotesOnPage, objectsOnPage));
                }

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageforAntibioticsBLactam(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            //metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            // show child nodes in a table
            List<IBDNode> subsections = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode subsection in subsections)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subsection as BDNode, "h3", footnotes, objectsOnPage)); 
                List<IBDNode> topics = BDFabrik.GetChildrenForParent(pContext, subsection);
                foreach (IBDNode topic in topics)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topic as BDNode, "h4", footnotes, objectsOnPage));
                    
                    List<IBDNode> tables = BDFabrik.GetChildrenForParent(pContext, topic);
                    foreach (IBDNode table in tables)
                    {
                        if (table.Name.Length > 0)
                            bodyHTML.AppendFormat(@"<b>{0}</b>", table.Name);

                        List<IBDNode> tableChildren = BDFabrik.GetChildrenForParent(pContext, table);
                        if (tableChildren.Count > 0)
                            bodyHTML.Append(@"<table>");

                         foreach (IBDNode tableChild in tableChildren)
                         {
                             if (tableChild.NodeType == BDConstants.BDNodeType.BDTableSection)
                             {
                                 if (tableChild.Name.Length > 0)
                                     bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, tableChild, "b", footnotes, objectsOnPage));

                                 List<IBDNode> rows = BDFabrik.GetChildrenForParent(pContext, tableChild);
                                 foreach (IBDNode row in rows)
                                 {
                                     BDTableRow tableRow = row as BDTableRow;
                                     if (row != null)
                                         bodyHTML.Append(buildTableRowHtml(pContext, tableRow, false, true, footnotes, objectsOnPage));
                                 }
                             }
                             else if (tableChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                             {
                                 BDTableRow headerRow = tableChild as BDTableRow;
                                 bodyHTML.Append(buildTableRowHtml(pContext, headerRow, false, true, footnotes, objectsOnPage));
                             }
                         }
                         bodyHTML.Append(@"</table>");
                         List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, table.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                         string legendHTML = buildTextFromNotes(legendNotes, objectsOnPage);
                         if (legendHTML.Length > EMPTY_PARAGRAPH)
                             bodyHTML.Append(legendHTML);

                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }
        #endregion


        #region Treatment Recommendations sections
        /// <summary>
        /// Build HTML page at Disease level when only one Presentation is defined
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        /// 
        [Obsolete("use GenerateBDHtmlPage instead")]
        public BDHtmlPage generatePageForEmpiricTherapyDisease(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDDisease)
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

            List<IBDNode> presentations = BDFabrik.GetChildrenForParent(pContext, pNode);

#if DEBUG
            if (presentations.Count > 1)
                throw new InvalidOperationException();
#endif
            foreach (IBDNode presentation in presentations)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, presentation as BDNode, "h2", footnotesOnPage, objectsOnPage));
                    List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, presentation);
                if (presentation.LayoutVariant == BDConstants.LayoutVariantType.Dental_RecommendedTherapy)
                {
                    foreach (IBDNode tGroup in childNodes)
                    {
                        BDTherapyGroup group = tGroup as BDTherapyGroup;
                        if (null != group) // bypass any pathogens that also appear at this level
                        {
                            bodyHTML.Append(buildNodePropertyHTML(pContext, group, group.Name, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                            bodyHTML.Append(buildTherapyGroupHTML(pContext, group, footnotesOnPage, objectsOnPage));
                        }
                    }
                }
                else
                {
                    foreach (IBDNode pathogenGroup in childNodes)
                        bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footnotesOnPage, objectsOnPage));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        /// <summary>
        /// Build HTML page to show Presentation and all it's children on one page
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        [Obsolete("use GenerateBDHtmlPage instead")]
        public BDHtmlPage generatePageForEmpiricTherapyPresentation(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
           // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotesOnPage, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);

            switch (pNode.LayoutVariant)
            {
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected:
                    foreach (IBDNode entry in childNodes)
                    {
                        //bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, entry, "h3", footnotesOnPage, objectsOnPage));

                        switch (entry.NodeType)
                        {
                            case BDConstants.BDNodeType.BDPathogenGroup:
                                bodyHTML.Append(BuildBDPathogenGroupHtml(pContext, entry, footnotesOnPage, objectsOnPage, 3, true));
                                break;
                            case BDConstants.BDNodeType.BDTopic:
                                // describe the topic
                                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, entry, "h3", footnotesOnPage, objectsOnPage));
                                List<IBDNode> topicPathogenGroups = BDFabrik.GetChildrenForParent(pContext, entry);

                                foreach (IBDNode pathogenGroup in topicPathogenGroups)
                                {
                                    //ks: casting pathogenGroup as BDNode may result in null objects
                                    //bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footnotesOnPage, objectsOnPage));
                                    bodyHTML.Append(BuildBDPathogenGroupHtml(pContext, entry, footnotesOnPage, objectsOnPage, 3, true));
                                }
                                break;
                            default:
                                throw new InvalidOperationException("Unhandled NodeType processing Gastroenteritis-esque child node");
                                break;
                        }
                    }
                    break;
                case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                    foreach (IBDNode tGroup in childNodes)
                    {
                        BDTherapyGroup group = tGroup as BDTherapyGroup;
                        if (null != group)
                        {
                            //ks: Therapy Layout override
                            bodyHTML.Append(BuildBDTherapyGroupHTML(pContext, group, footnotesOnPage, objectsOnPage, 3, BDConstants.LayoutVariantType.TreatmentRecommendation01));
                            //bodyHTML.Append(buildNodePropertyHTML(pContext, group, group.Name, BDNode.PROPERTYNAME_NAME, false, footnotesOnPage, objectsOnPage));
                            //bodyHTML.Append(buildTherapyGroupHTML(pContext, group, footnotesOnPage, objectsOnPage));
                        }
                    }
                    break;
                default:
                    List<IBDNode> pathogenGroupList = BDFabrik.GetChildrenForParent(pContext, pNode);
                    foreach (IBDNode pathogenGroup in childNodes)
                        bodyHTML.Append(BuildBDPathogenGroupHtml(pContext, pathogenGroup, footnotesOnPage, objectsOnPage, 3, true));
                        //bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footnotesOnPage, objectsOnPage));
                    break;
            }



            /*
            // gastroenteritis:  get Topic as child of Presentation, then Pathogen Group
            if (pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis || pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected)
            {
                foreach (IBDNode topic in childNodes)
                {
                    //ks: replaced pNode with topic
                     bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topic, "h3", footnotesOnPage, objectsOnPage));

                    //ks: These are NOT ALWAYS pathogen groups.  TreatmentRecommendation01_Gastroenteritis (10141) will return a collection of BDPathogens & BDTherapyGroup
                    List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, topic);
                    foreach (IBDNode pathogenGroup in pathogenGroups)
                    {
                        //replaced 'pathogenGroup as BDNode' with just 'pathogenGroup', refactored method to permit IBDNode as parameter instead of BDNode to prevent null references
                        bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footnotesOnPage, objectsOnPage));
                    }
                }
            }
             */
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        public BDHtmlPage GenerateBDHtmlPage(Entities pContext, IBDNode pNode)
        {
            if (null == pNode) return null;

            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    bodyHTML.Append(BuildBDAntimicrobialHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                    case BDConstants.BDNodeType.BDCategory:
                    bodyHTML.Append(BuildBDCategoryHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    bodyHTML.Append(BuildBDDiseaseHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDPathogen:
                    bodyHTML.Append(BuildBDPathogenHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDPathogenGroup:
                    bodyHTML.Append(BuildBDPathogenGroupHtml(pContext, pNode, footnotes, objectsOnPage, 1, true));
                    break;
                case BDConstants.BDNodeType.BDPresentation:
                    bodyHTML.Append(buildBDPresentationHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDResponse: //ks: BDFabrik doesn't show this nodeType to create a page...
                    bodyHTML.Append(BuildBDResponseHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSection:
                    bodyHTML.Append(BuildBDSectionHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSubcategory:
                    bodyHTML.Append(BuildBDSubcategoryHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSubsection:
                    bodyHTML.Append(BuildBDSubSectionHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDSubtopic:
                    bodyHTML.Append(BuildBDSubTopicHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDTable:
                    bodyHTML.Append(BuildBDTableHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
                case BDConstants.BDNodeType.BDTopic:
                    bodyHTML.Append(BuildBDTopicHtml(pContext, pNode, footnotes, objectsOnPage, 1));
                    break;
            }

            bodyHTML.Append(BuildBDLegendHtml(pContext, pNode, objectsOnPage));

            //TODO: Verify that this assignment is correct
            currentPageMasterObject = pNode;

            BDHtmlPage htmlPage = writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);

            return htmlPage;
        }

        public string BuildBDLegendHtml(Entities pContext, IBDNode pNode, List<Guid> pObjectsOnPage)
        {
            if (null == pNode) return string.Empty;

            StringBuilder html = new StringBuilder();
            List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
            foreach (BDLinkedNote note in legendNotes)
                pObjectsOnPage.Add(note.Uuid);
            string legendHTML = buildTextFromNotes(legendNotes, pObjectsOnPage);
            if (legendHTML.Length > EMPTY_PARAGRAPH)
                html.Append(legendHTML);

            return html.ToString();
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForEmpiricTherapyOfCellulitisInExtremities(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            //metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode child in childNodes)
            {
                if (child.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
                    bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pNode as BDNode, footnotes, objectsOnPage));
                else if (child.NodeType == BDConstants.BDNodeType.BDTable)
                {
                    // table
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode as BDNode, "h2", footnotes, objectsOnPage));
                    List<IBDNode> conditions = BDFabrik.GetChildrenForParent(pContext, pNode);
                    if (conditions.Count > 0)
                    {
                        bodyHTML.Append("<table><tr><th>Condition</th><th>Other Potential Pathogens</th></tr>");
                        foreach (IBDNode condition in conditions)
                        {
                            bodyHTML.AppendFormat(@"<tr><td>{0}</td><td>", condition.Name);
                            objectsOnPage.Add(condition.Uuid);
                            List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, condition);
                            foreach (IBDNode node in pathogens)
                            {
                                bodyHTML.AppendFormat("{0}<br>", node.Name);
                                objectsOnPage.Add(node.Uuid);
                            }
                        }
                        bodyHTML.Append("</td></tr></table>");
                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForEmpiricTherapyOfVesicularLesions(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            //metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in childNodes)   // response or pathogen group
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, "h2", footnotes, objectsOnPage));
                if (child.NodeType == BDConstants.BDNodeType.BDResponse)
                {
                    List<IBDNode> frequencies = BDFabrik.GetChildrenForParent(pContext, child);
                    foreach (IBDNode frequency in frequencies)
                    {
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, frequency as BDNode, "h4", footnotes, objectsOnPage));
                        List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, frequency);
                        foreach (IBDNode pathogenGroup in pathogenGroups)
                        {
                            bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footnotes, objectsOnPage));
                        }
                    }
                }
                else if (child.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
                {
                    List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, child);
                    foreach (IBDNode pathogen in pathogens)
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pathogen as BDNode, "h4", footnotes, objectsOnPage));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForEmpiricTherapyOfBCNE(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogen)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

           // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            StringBuilder diagnosis = new StringBuilder();
            StringBuilder clinical = new StringBuilder();
            StringBuilder therapy = new StringBuilder();
            diagnosis.Append(@"<h3>Diagnosis</h3>");
            clinical.Append(@"<h3>Clinical</h3>");
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode childNode in childNodes)
            {
                BDNode topic = childNode as BDNode;
                BDTherapyGroup therapyGroup = childNode as BDTherapyGroup;
                if (childNode.NodeType == BDConstants.BDNodeType.BDTopic && topic != null)
                {
                    // Inline contains the comments in the Clinical column
                    clinical.AppendFormat(@"<p><b>{0}</b><br>", topic.Name);
                    objectsOnPage.Add(topic.Uuid);
                    clinical.AppendFormat(@"{0}</p>", buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, topic, BDConstants.LinkedNoteType.Inline, objectsOnPage));
                    // overview contains the 'Diagnosis' column data
                    diagnosis.Append(retrieveNoteTextForOverview(pContext, topic.Uuid, objectsOnPage));
                }
                else if (therapyGroup != null)
                   therapy.Append(buildTherapyGroupHTML(pContext, therapyGroup, footnotes, objectsOnPage));

            }
            bodyHTML.AppendFormat(@"{0}<br>{1}<br>{2}", clinical, diagnosis, therapy);
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        //ks: REVIEW THIS
        private BDHtmlPage generatepageForEmpiricTherapyOfGenitalUlcers(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable && pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            //metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            if (pNode.NodeType == BDConstants.BDNodeType.BDTable)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode as BDNode, "h2", footnotes, objectsOnPage));
                List<IBDNode> topics = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode topic in topics)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topic as BDNode, "h3", footnotes, objectsOnPage));
                    List<IBDNode> subtopics = BDFabrik.GetChildrenForParent(pContext, topic);
                    foreach (IBDNode subtopic in subtopics)
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subtopic as BDNode, "h4", footnotes, objectsOnPage));
                }
                return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage, null);
            }
            else
            {
                return generatePageForEmpiricTherapyPresentation(pContext, pNode);
            }
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForEmpiricTherapyOfParasitic(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogen)
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

            if (pNode.Name.Length > 0 && !pNode.Name.Contains(@"New ") )
                bodyHTML.AppendFormat(@"<{0}>{1}</{2}>", "h1", pNode.Name, "h1");
                
            bodyHTML.Append(buildReferenceHtml(pContext, pNode, objectsOnPage));

            // overview
            string symptomsOverview = retrieveNoteTextForOverview(pContext, pNode.Uuid, objectsOnPage);
            if (symptomsOverview.Length > EMPTY_PARAGRAPH)
                bodyHTML.AppendFormat(@"<u><b>Symptoms</b></u><br>{0}",symptomsOverview);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode childNode in childNodes) 
            {
                if (childNode.NodeType == BDConstants.BDNodeType.BDTherapyGroup)
                {
                    bodyHTML.Append(buildNodePropertyHTML(pContext, childNode, childNode.Name, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                    bodyHTML.Append(buildTherapyGroupHTML(pContext, childNode as BDTherapyGroup, footnotesOnPage, objectsOnPage));
                }
                else 
                {
                    string presentationOverview = retrieveNoteTextForOverview(pContext, childNode.Uuid, objectsOnPage);
                    if (presentationOverview.Length > EMPTY_PARAGRAPH)
                        bodyHTML.AppendFormat(@"<u><b>Comments</b></u><br>{0}", presentationOverview);
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        /// <summary>
        /// Build page for CURB-65 - Pneumonia Severity of Illness Scoring System
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForEmpiricTherapyOfPneumonia(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
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
            bodyHTML.Append(@"<table>");

            List<IBDNode> tables = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode tbl in tables)
            {
                if (tbl.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I)
                {
                    List<IBDNode> tableSections = BDFabrik.GetChildrenForParent(pContext, tbl);
                    foreach (IBDNode section in tableSections)
                    {
                        List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, section);
                        if (childNodes.Count > 0)
                        {
                            bodyHTML.Append(buildTableRowHtml(pContext, childNodes[0] as BDTableRow, true, false, footnotesOnPage, objectsOnPage));
                            for (int i = 1; i < childNodes.Count; i++)
                                bodyHTML.Append(buildTableRowHtml(pContext, childNodes[i] as BDTableRow, false, false, footnotesOnPage, objectsOnPage));
                        }
                    }
                }
                else if (tbl.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II)
                {
                    // table row OR table section here
                    List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, tbl);
                    foreach (IBDNode row in childNodes)
                    {
                        if (row.NodeType == BDConstants.BDNodeType.BDTableRow)
                            bodyHTML.Append(buildTableRowHtml(pContext, row as BDTableRow, false, false, footnotesOnPage, objectsOnPage));
                        else
                        {
                            List<IBDNode> sectionRows = BDFabrik.GetChildrenForParent(pContext, row);
                            foreach(IBDNode sectionRow in sectionRows)
                                bodyHTML.Append(buildTableRowHtml(pContext, sectionRow as BDTableRow, false, false, footnotesOnPage, objectsOnPage));
                        }
                    }
                }
            }
            bodyHTML.Append(@"</table>");

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        /// <summary>
        /// For culture-proven pneumonia & meningitis
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForEmpiricTherapyOfCultureDirected(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogen)
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

            // show child nodes in a table
            List<IBDNode> resistances = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode resistance in resistances)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, resistance as BDNode, "h2", footnotesOnPage, objectsOnPage));
                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, resistance);
                foreach(IBDNode therapyGroup in therapyGroups) 
                {
                    bodyHTML.Append(buildNodePropertyHTML(pContext, therapyGroup, therapyGroup.Name, BDTherapyGroup.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                    bodyHTML.Append(buildTherapyGroupHTML(pContext, therapyGroup as BDTherapyGroup, footnotesOnPage, objectsOnPage));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        private BDHtmlPage generatePageForEmpiricTherapyOfFungalInfections(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTopic)
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

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode child in childNodes)
            {
                bodyHTML.Append(buildAttachmentHTML(pContext,child,footnotesOnPage,objectsOnPage));
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }


        /// <summary>
        /// Build page at PathogenGroup downward
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForEmpiricTherapyOfCultureDirectedPeritonitis(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogenGroup)
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

            List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode therapyGroup in therapyGroups)
            {
                bodyHTML.Append(buildNodePropertyHTML(pContext, therapyGroup, therapyGroup.Name, BDTherapyGroup.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                // custom-built - Therapy Group has 2 dosages and a custom header

                List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, therapyGroup.Uuid);
                if (therapies.Count > 0)
                {
                    bodyHTML.Append(@"<table>");

                    therapiesHaveDosage = false;
                    therapiesHaveDuration = false;
                    StringBuilder therapyHTML = new StringBuilder();
                    foreach (BDTherapy therapy in therapies)
                    {
                        therapyHTML.Append(buildTherapyWithTwoDosagesHtml(pContext, therapy, true, footnotesOnPage, objectsOnPage));

                        if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                            previousTherapyName = therapy.Name;
                        if (!string.IsNullOrEmpty(therapy.dosage))
                        {
                            if (therapy.dosageSameAsPrevious == false)
                                previousTherapyDosage = therapy.dosage;
                            therapiesHaveDosage = true;
                        }
                    }
                    if (!therapiesHaveDosage && !therapiesHaveDuration)
                        bodyHTML.Append(@"<tr><th>Recommended Empiric Therapy</th>");
                    else
                        bodyHTML.Append(@"<tr><th>Recommended<br>Empiric<br>Therapy</th>");

                    if (therapiesHaveDosage)
                    {
                        bodyHTML.Append(@"<th>Intermittent Dosing<br>Dwell time at least 6h</th>");
                        bodyHTML.Append(@"<th>Continuous dosing<br>(per L bag)</th>");
                    }
                    else
                        bodyHTML.Append(@"<th />");
                    
                    if (therapiesHaveDuration)
                        bodyHTML.Append(@"<th>Recommended<br>Duration</th>");
                    else
                        bodyHTML.Append(@"<th />");

                    bodyHTML.Append(@"</tr>");

                    bodyHTML.Append(therapyHTML);
                    bodyHTML.Append(@"</table>");
                }

                
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        public BDHtmlPage generatePageForEmpiricTherapyOfCultureDirectedEndocarditis(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogen)
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
            List<IBDNode> resistances = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode resistance in resistances)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, resistance as BDNode, "h2", footnotesOnPage, objectsOnPage));
                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, resistance);
                foreach (IBDNode therapyGroup in therapyGroups)
                {
                    bodyHTML.Append(buildNodePropertyHTML(pContext, therapyGroup, therapyGroup.Name, BDTherapyGroup.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                    
                    // custom-built - Therapy Group has 2 dosages and a custom header
                    List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, therapyGroup.Uuid);
                    if (therapies.Count > 0)
                    {
                        bodyHTML.Append(@"<table>");

                        therapiesHaveDosage = false;
                        therapiesHaveDuration = false;
                        StringBuilder therapyHTML = new StringBuilder();
                        foreach (BDTherapy therapy in therapies)
                        {
                            therapyHTML.Append(buildTherapyWithTwoDurationsHtml(pContext, therapy, footnotesOnPage, objectsOnPage));

                            if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                previousTherapyName = therapy.Name;
                            if (!string.IsNullOrEmpty(therapy.dosage))
                            {
                                if (therapy.dosageSameAsPrevious == false)
                                    previousTherapyDosage = therapy.dosage;
                                therapiesHaveDosage = true;
                            }
                            if (!string.IsNullOrEmpty(therapy.duration))
                            {
                                if (therapy.durationSameAsPrevious == false)
                                    previousTherapyDuration = therapy.duration;
                                therapiesHaveDuration = true;
                            }
                        }
                        if (!therapiesHaveDosage && !therapiesHaveDuration)
                            bodyHTML.Append(@"<tr><th>Recommended Empiric Therapy</th>");
                        else
                            bodyHTML.Append(@"<tr><th>Recommended<br>Empiric<br>Therapy</th>");
                        if (therapiesHaveDosage)
                        {
                            bodyHTML.Append(@"<th>Recommended<br>Dose</th>");
                        }
                        else
                            bodyHTML.Append(@"<th></th>");
                        if (therapiesHaveDuration)
                            bodyHTML.Append(@"<th colspan=2>Recommended<br>Duration</th>");
                        else
                            bodyHTML.Append(@"<th></th>");

                        bodyHTML.Append(@"</tr>");
                        bodyHTML.Append(@"<tr><th /><th /><th>Native</th><th>Prosthetic</th></tr>");

                        bodyHTML.Append(therapyHTML);
                        bodyHTML.Append(@"</table>");
                    }

                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        public BDHtmlPage generatePageForEmpiricTherapyOfEndocarditisPaediatrics(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable)
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

                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode therapyGroup in therapyGroups)
                {
                    bodyHTML.Append(buildNodePropertyHTML(pContext, therapyGroup, therapyGroup.Name, BDTherapyGroup.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                    
                    // custom-built - Therapy has one dosage and no duration
                    List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, therapyGroup.Uuid);
                    if (therapies.Count > 0)
                    {
                        bodyHTML.Append(@"<table>");

                        therapiesHaveDosage = false;
                        therapiesHaveDuration = false;
                        StringBuilder therapyHTML = new StringBuilder();
                        foreach (BDTherapy therapy in therapies)
                        {
                            therapyHTML.Append(buildTherapyHtml(pContext, therapy, footnotesOnPage, objectsOnPage));

                            if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                previousTherapyName = therapy.Name;
                            if (!string.IsNullOrEmpty(therapy.dosage))
                            {
                                if (therapy.dosageSameAsPrevious == false)
                                    previousTherapyDosage = therapy.dosage;
                                therapiesHaveDosage = true;
                            }
                        }
                        if (!therapiesHaveDosage && !therapiesHaveDuration)
                            bodyHTML.Append(@"<tr><th>Recommended Empiric Therapy</th>");
                        else
                            bodyHTML.Append(@"<tr><th>Antibiotics</th>");
                        if (therapiesHaveDosage)
                        {
                            bodyHTML.Append(@"<th>Dose</th>");
                        }
                        else
                            bodyHTML.Append(@"<th></th>");
                        if (therapiesHaveDuration)
                            bodyHTML.Append(@"<th colspan=2>Recommended<br>Duration</th>");
                        else
                            bodyHTML.Append(@"<th></th>");

                        bodyHTML.Append(@"</tr>");

                        bodyHTML.Append(therapyHTML);
                        bodyHTML.Append(@"</table>");
                    }
                }
                return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }
        #endregion

        #region Prophylaxis sections
        private BDHtmlPage generatePageforProphylaxisSurgical(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
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

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }
        
        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForProphylaxisEndocarditis(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();
            
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

            string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapyGroup, BDTherapyGroup.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage);
            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, footnotesOnPage, objectsOnPage);
            string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, footnotesOnPage, objectsOnPage);
            string c4Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, footnotesOnPage, objectsOnPage); 
            
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            List<BDHtmlPage> childPages = new List<BDHtmlPage>();
            List<BDLinkedNote> catFootnotes = new List<BDLinkedNote>();
            StringBuilder therapyGroupHTML = new StringBuilder();
            therapyGroupHTML.AppendFormat("<h2>{0}</h2>", c1Html);
            List<BDLinkedNote> therapyLinkedNotes = new List<BDLinkedNote>();
            foreach(IBDNode child in childNodes)
            {
                if (child.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_IERecommendation)
                // Category - build child pages and then links on the section page
                {
                    StringBuilder categoryHTML = new StringBuilder();
                    List<Guid> categoriesOnPage = new List<Guid>();
                    List<IBDNode> subcategories = BDFabrik.GetChildrenForParent(pContext, child);
                    categoryHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "h2", catFootnotes, categoriesOnPage));
                    foreach (IBDNode subcategory in subcategories)
                        categoryHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, "h4", catFootnotes, categoriesOnPage));
                    currentPageMasterObject = child;
                    childPages.Add(writeBDHtmlPage(pContext, child, categoryHTML, BDConstants.BDHtmlPageType.Data, catFootnotes, categoriesOnPage, null));
                }
                else
                {
                    // this is a TherapyGroup > therapy hierarchy
                    StringBuilder therapyHTML = new StringBuilder();
                    List<Guid> therapiesOnPage = new List<Guid>();
                    List<BDLinkedNote> therapyFootnotes = new List<BDLinkedNote>();
                    List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, child);
                    therapyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "h2", therapyFootnotes, therapiesOnPage));
                    string subtext = "given 30-60 minutes before the procedure";
                    therapyHTML.AppendFormat("<table><tr><th>{0}</th><th>{1} {2}/ROUTE</th><th>{3} {4}/ROUTE</th></tr>",
                        c2Html, c3Html, subtext, c4Html, subtext);
                    foreach (IBDNode t in therapies)
                    {
                        BDTherapy therapy = t as BDTherapy;
                        therapyHTML.AppendFormat(buildTherapyWithTwoDosagesHtml(pContext, therapy, false, therapyFootnotes, therapiesOnPage));
                    }
                    therapyHTML.Append("</table>");
                    currentPageMasterObject = child;
                    childPages.Add(writeBDHtmlPage(pContext, child, therapyHTML, BDConstants.BDHtmlPageType.Data, therapyFootnotes, therapiesOnPage, null));
                }
            }
            for (int i = 0; i < childPages.Count; i++)
            {
                if (childNodes[i].LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_IERecommendation)
                    bodyHTML.AppendFormat(anchorTag, childPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
                else
                    therapyGroupHTML.AppendFormat(anchorTag, childPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
            }
            bodyHTML.Append(therapyGroupHTML);
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        public BDHtmlPage generatePageForProphylaxisFluidExposureRiskOfInfection(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage);
            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, footnotesOnPage, objectsOnPage);
            string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, footnotesOnPage, objectsOnPage);

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode entry in childNodes)
            {
                bodyHTML.AppendFormat("<h2>{0}</h2>", retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Name_fieldNote", objectsOnPage, footnotesOnPage));
                bodyHTML.AppendFormat("<table><tr><th>{0}</th></tr>", c2Html);
                bodyHTML.AppendFormat("<tr><td>{0}</td></tr></table>",
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field01_fieldNote", objectsOnPage, footnotesOnPage));
                bodyHTML.AppendFormat("<table><tr><th>{0}</th></tr>",c3Html);
                bodyHTML.AppendFormat("<tr><td>{0}</td></tr></table>",
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field02_fieldNote", objectsOnPage, footnotesOnPage));
                objectsOnPage.Add(entry.Uuid);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForProphylaxisFluidExposureFollowupProtocolI(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, footnotesOnPage, objectsOnPage);
            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, footnotesOnPage, objectsOnPage);
           
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

                bodyHTML.AppendFormat("<table><tr><th>{0}</th><th>{1}</th></tr>", c1Html, c2Html);
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode entry in childNodes)
            {
                bodyHTML.AppendFormat("<tr><td>{0}{1}</td><td>{2}{3}</td></tr>",
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field01_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD01, objectsOnPage, true, footnotesOnPage),
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD01, BDConstants.LinkedNoteType.Footnote), true, footnotesOnPage, objectsOnPage),
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field02_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD02, objectsOnPage, true, footnotesOnPage),
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD02, BDConstants.LinkedNoteType.Footnote), true, footnotesOnPage, objectsOnPage));
                objectsOnPage.Add(entry.Uuid);
            }
            bodyHTML.Append("</table>");
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForProphylaxisFluidExposureFollowupProtocolII(Entities pContext, IBDNode pNode)
        {
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage);
            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, footnotesOnPage, objectsOnPage);
            string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, footnotesOnPage, objectsOnPage);
            string c4Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, footnotesOnPage, objectsOnPage);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode entry in childNodes)
            {
                bodyHTML.AppendFormat(@"<h4>SOURCE:  {0}</h4>", retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Name_fieldNote", BDConfiguredEntry.PROPERTYNAME_NAME, objectsOnPage, true, footnotesOnPage));
                bodyHTML.AppendFormat(@"<table><tr colspan=3><th>RECIPIENT{0}</th></tr>",c1Html);
                bodyHTML.AppendFormat("<tr><th>{0}</th><th>{1}</th><th>{2}</th></tr>", c2Html, c3Html, c4Html);
                bodyHTML.AppendFormat("<tr><td>{0}{1}</td><td>{2}{3}</td><td>{4}{5}</td></tr>",
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field01_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD01, objectsOnPage, true, footnotesOnPage), 
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD01, BDConstants.LinkedNoteType.Footnote), true, footnotesOnPage, objectsOnPage),
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field02_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD02, objectsOnPage, true, footnotesOnPage), 
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD02, BDConstants.LinkedNoteType.Footnote), true, footnotesOnPage, objectsOnPage),
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field03_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD03, objectsOnPage, true, footnotesOnPage), 
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD03, BDConstants.LinkedNoteType.Footnote), true, footnotesOnPage, objectsOnPage));

                bodyHTML.Append("</table>");
                objectsOnPage.Add(entry.Uuid);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForProhylaxisSexualAssault(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTopic)
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

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                foreach (IBDNode child in childNodes)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "h2", footnotesOnPage, objectsOnPage));
                    bodyHTML.Append("<table>");
                    List<IBDNode> tableChildren = BDFabrik.GetChildrenForParent(pContext, child);
                    foreach (IBDNode tableChild in tableChildren)
                    {
                        if (tableChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                            bodyHTML.Append(buildTableRowHtml(pContext, tableChild as BDTableRow, true, true, footnotesOnPage, objectsOnPage));
                        else if (tableChild.NodeType == BDConstants.BDNodeType.BDTableSection)
                        {
                            if (tableChild.Name.Length > 0)
                                bodyHTML.AppendFormat("<tr><td colspan=3>{0}</td></tr>", tableChild.Name);
                            objectsOnPage.Add(tableChild.Uuid);
                            List<IBDNode> tableRows = BDFabrik.GetChildrenForParent(pContext, tableChild);
                            foreach(IBDNode row in tableRows)
                                bodyHTML.Append(buildTableRowHtml(pContext, row as BDTableRow, false, true, footnotesOnPage, objectsOnPage));
                        }
                    }
                    bodyHTML.Append("</table>");
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        private BDHtmlPage generatePageForProhylaxisImmunization(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
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

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead", true)]
        private BDHtmlPage generatePageForProphylaxisCommunicableDiseases(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDDisease)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
           foreach(IBDNode child in childNodes)
            {
                bool isAlternateLayout = false;

                if (child.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza && child.NodeType == BDConstants.BDNodeType.BDTable)
                    isAlternateLayout = true;

                bodyHTML.Append(buildNodePropertyHTML(pContext, child, child.Uuid, child.Name, BDNode.PROPERTYNAME_NAME, "h3", footnotesOnPage, objectsOnPage));

                StringBuilder tableHTML = new StringBuilder();
               List<IBDNode> l2Children = BDFabrik.GetChildrenForParent(pContext, child);
               bool isFirstChild = true;
               bool isFirstConfiguredEntry = true;
                foreach (IBDNode l2Child in l2Children) // lvl2 (l2) can be topic or table.
                {
                        List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, l2Child.LayoutVariant);
                    if (isFirstChild)
                    {
                        tableHTML.Append("<table><tr>");
                        for (int i = 0; i < metadataLayoutColumns.Count; i++)
                            tableHTML.AppendFormat("<th>{0}</th>", metadataLayoutColumns[i]);
                        tableHTML.Append("</tr>");
                        isFirstChild = false;
                    }
                    if (l2Child.NodeType == BDConstants.BDNodeType.BDCombinedEntry)
                    {
                        StringBuilder cell0HTML = new StringBuilder();
                        StringBuilder cell1HTML = new StringBuilder();
                        StringBuilder cell2HTML = new StringBuilder();
                        BDCombinedEntry cEntry = l2Child as BDCombinedEntry;
                        bool writeCellsToRow = false;

                        if (l2Child.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Weight ||
                            l2Child.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Creatinine)
                        {
                            tableHTML.AppendFormat("<tr><td rowspan=4>{0}</td><td>{1}</td><td>{2}</td></tr>",
                                buildNodePropertyHTML(pContext, cEntry, cEntry.Name, BDCombinedEntry.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage),
                                buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, footnotesOnPage, objectsOnPage),
                                buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, footnotesOnPage, objectsOnPage));
                            if (cEntry.entryDetail02 != null && cEntry.entryTitle02 != null)
                            {
                                tableHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, footnotesOnPage, objectsOnPage),
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, footnotesOnPage, objectsOnPage));
                            }
                            if (cEntry.entryDetail03 != null && cEntry.entryTitle03 != null)
                            {
                                tableHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, footnotesOnPage, objectsOnPage),
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, footnotesOnPage, objectsOnPage));
                            }
                            if (cEntry.entryTitle04 != null && cEntry.entryDetail04 != null)
                            {
                                tableHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle04, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04, footnotesOnPage, objectsOnPage),
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail04, BDCombinedEntry.PROPERTYNAME_ENTRY04, footnotesOnPage, objectsOnPage));
                            }
                        }
                        else if (l2Child.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_NoRenal)
                        {
                            bodyHTML.AppendFormat("<h4>{0}</h4>", cEntry.Name);
                            tableHTML.AppendFormat("<tr><td>{0}</td><td colspan=3>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, footnotesOnPage, objectsOnPage),
                                buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, footnotesOnPage, objectsOnPage));
                            tableHTML.AppendFormat("<tr><td>{0}</td><td colspan=3>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, footnotesOnPage, objectsOnPage),
                                buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, footnotesOnPage, objectsOnPage));
                            tableHTML.AppendFormat("<tr><td>{0}</td><td colspan=3>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, footnotesOnPage, objectsOnPage),
                                buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, footnotesOnPage, objectsOnPage));
                        }
                        else
                        {
                            writeCellsToRow = true;
                            string groupTitleHtml = buildNodePropertyHTML(pContext, cEntry, cEntry.groupTitle, BDCombinedEntry.PROPERTYNAME_GROUPTITLE, footnotesOnPage, objectsOnPage);
                            if (isAlternateLayout && !string.IsNullOrEmpty(cEntry.groupTitle))
                                tableHTML.AppendFormat("<tr><td colspan={0}><b>{1}</b> </td></tr>", metadataLayoutColumns.Count, groupTitleHtml);
                            else if (!string.IsNullOrEmpty(cEntry.groupTitle)) cell0HTML.AppendFormat("<u>{0}</u><br>", groupTitleHtml);

                            if (!string.IsNullOrEmpty(cEntry.Name))
                                cell0HTML.AppendFormat("<b>{0}</b><br>{1}<br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.Name, BDCombinedEntry.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage),
                                    BDUtilities.GetEnumDescription(cEntry.GroupJoinType));

                            if (!string.IsNullOrEmpty(cEntry.entryTitle01))
                                cell1HTML.AppendFormat("<u>{0}</u><br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, footnotesOnPage, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.entryDetail01))
                                cell1HTML.AppendFormat("{0}<br><b>{1}</b><br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, footnotesOnPage, objectsOnPage),
                                    BDUtilities.GetEnumDescription(cEntry.JoinType01));

                            if (!string.IsNullOrEmpty(cEntry.entryTitle02))
                                cell1HTML.AppendFormat("<u>{0}</u><br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, footnotesOnPage, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.entryDetail02))
                                cell1HTML.AppendFormat("{0}<br><b>{1}</b><br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, footnotesOnPage, objectsOnPage),
                                    BDUtilities.GetEnumDescription(cEntry.JoinType02));

                            if (!string.IsNullOrEmpty(cEntry.entryTitle03))
                                cell2HTML.AppendFormat("<u>{0}</u><br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, footnotesOnPage, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.entryDetail03))
                                cell2HTML.AppendFormat("{0}<br><b>{1}</b><br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, footnotesOnPage, objectsOnPage),
                                    BDUtilities.GetEnumDescription(cEntry.JoinType03));

                            if (!string.IsNullOrEmpty(cEntry.entryTitle04))
                                cell2HTML.AppendFormat("<u>{0}</u><br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryTitle04, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04, footnotesOnPage, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.entryDetail04))
                                cell2HTML.AppendFormat("{0}<br><b>{1}</b><br>",
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.entryDetail04, BDCombinedEntry.PROPERTYNAME_ENTRY04, footnotesOnPage, objectsOnPage),
                                    BDUtilities.GetEnumDescription(cEntry.JoinType04));
                        }
                        if(writeCellsToRow)
                            tableHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", cell0HTML, cell1HTML, cell2HTML);
                    }
                    else if (l2Child.NodeType == BDConstants.BDNodeType.BDTopic)
                    {
                        List<IBDNode> topicChildren = BDFabrik.GetChildrenForParent(pContext, l2Child);
                        foreach (IBDNode tChild in topicChildren)
                        {
                            if (tChild.NodeType == BDConstants.BDNodeType.BDConfiguredEntry)
                            {
                                BDConfiguredEntry cEntry = tChild as BDConfiguredEntry;
                                if (isFirstConfiguredEntry)
                                {
                                    List<BDLayoutMetadataColumn> layoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, l2Child.LayoutVariant);
                                    string c1Html = buildHtmlForMetadataColumn(pContext, cEntry, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage);
                                    string c2Html = buildHtmlForMetadataColumn(pContext, cEntry, metadataLayoutColumns[1], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, footnotesOnPage, objectsOnPage);
                                    string c3Html = buildHtmlForMetadataColumn(pContext, cEntry, metadataLayoutColumns[2], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, footnotesOnPage, objectsOnPage);
                                    string c4Html = buildHtmlForMetadataColumn(pContext, cEntry, metadataLayoutColumns[3], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, footnotesOnPage, objectsOnPage);
                                    
                                    // handle configured entry for Amantadine with No renal impairment
                                    tableHTML.Append("</table><h4>Renal Impairment</h4><table>");
                                    tableHTML.AppendFormat("<tr><th rowspan=2>{0}</th><th colspan=2>Dosage with capsules</th><th>Daily dosage with solution (10mg/mL)</th></tr>", layoutColumns[0]);
                                    tableHTML.AppendFormat("<tr><th>{0}</th><th>{1}</th><th>{2}</th></tr>", c1Html, c2Html, c3Html);
                                    isFirstConfiguredEntry = false;
                                }
                                tableHTML.AppendFormat("<tr><td>{0}</td><td><{1}</td><td>{2}</td><td>{3}</td></tr>", 
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.Name, BDConfiguredEntry.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage),
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.field01, BDConfiguredEntry.PROPERTYNAME_FIELD01, footnotesOnPage, objectsOnPage),
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.field02, BDConfiguredEntry.PROPERTYNAME_FIELD02, footnotesOnPage, objectsOnPage),
                                    buildNodePropertyHTML(pContext, cEntry, cEntry.field03, BDConfiguredEntry.PROPERTYNAME_FIELD03, footnotesOnPage, objectsOnPage));
                            }
                            objectsOnPage.Add(tChild.Uuid);
                        }
                    }
                    objectsOnPage.Add(l2Child.Uuid);
                }
                    bodyHTML.Append(tableHTML);
                bodyHTML.Append("</table>");
           }
           return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForProphylaxisInfectionPrevention(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
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

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotesOnPage, objectsOnPage));

            List<string> mgTitles = new List<string>();
            List<IBDNode> mGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
            StringBuilder mgHTML = new StringBuilder();
            string previousGroupName = string.Empty;
            foreach (IBDNode mGroup in mGroups)
            {
                if (mGroup.Name != previousGroupName)
                {
                    mgHTML.AppendFormat("<h3>{0}</h3>", mGroup.Name);
                    previousGroupName = mGroup.Name;
                }
                List<BDHtmlPage> mPages = new List<BDHtmlPage>();
                List<IBDNode> microorganisms = BDFabrik.GetChildrenForParent(pContext, mGroup);
                foreach (IBDNode microorganism in microorganisms)
                {
                    StringBuilder mHTML = new StringBuilder();
                    List<Guid> mObjectsOnPage = new List<Guid>();
                    List<BDLinkedNote> mFootnotes = new List<BDLinkedNote>();
                    mHTML.AppendFormat("<h2>{0}</h2>", microorganism.Name);
                    mgTitles.Add(microorganism.Name);

                    List<IBDNode> precautions = BDFabrik.GetChildrenForParent(pContext, microorganism);
                    foreach (IBDNode precaution in precautions)
                    {
                        BDPrecaution p = precaution as BDPrecaution;
                        mHTML.AppendFormat("<h4>Infective Material</h4>{0}", p.infectiveMaterial);
                        mHTML.AppendFormat("<h4>Mode of Transmission</h4>{0}", p.modeOfTransmission);
                        // build table
                        mHTML.AppendFormat("<table><tr><th>Precautions{0}</th><th>Acute Care</th><th>Long Term Care</th></tr>", 
                            buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, precaution.Uuid, BDPrecaution.PROPERTYNAME_ORGANISM_1, BDConstants.LinkedNoteType.Footnote), true, footnotesOnPage, objectsOnPage));
                        mHTML.AppendFormat("<tr><td>Single Room</td><td>{0}</td><td>{1}</td></tr>", p.singleRoomAcute, p.singleRoomLongTerm);
                        mHTML.AppendFormat("<tr><td>Gloves</td><td>{0}</td><td>{1}</td></tr>", p.glovesAcute, p.glovesLongTerm);
                        mHTML.AppendFormat("<tr><td>Gowns</td><td>{0}</td><td>{1}</td></tr>", p.gownsAcute, p.gownsLongTerm);
                        mHTML.AppendFormat("<tr><td>Mask</td><td>{0}</td><td>{1}</td></tr>", p.maskAcute, p.maskLongTerm);
                        mHTML.Append("</table>");

                        List<BDLinkedNote> durationNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, p.Uuid, BDPrecaution.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
                        StringBuilder durationText = new StringBuilder();
                        foreach (BDLinkedNote note in durationNotes)
                            durationText.Append(note.documentText);
                        mHTML.AppendFormat("<h4>Duration of Precautions</h4>{0}", durationText);
                    }
                    currentPageMasterObject = microorganism;
                    mPages.Add(writeBDHtmlPage(pContext, microorganism, mHTML, BDConstants.BDHtmlPageType.Data, mFootnotes, mObjectsOnPage, null));
                }
                for (int i = 0; i < mPages.Count; i++)
                    mgHTML.AppendFormat(anchorTag, mPages[i].Uuid.ToString().ToUpper(), mgTitles[i]);
            }
            bodyHTML.Append(mgHTML);
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }
        #endregion

        #region Dental Sections
        private BDHtmlPage generatePageForAntimicrobialAgentsForOralMicroorganisms(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotesOnPage, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                List<string> colHtml = new List<string>();
                colHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                colHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, footnotesOnPage, objectsOnPage));
                colHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, footnotesOnPage, objectsOnPage));
                colHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, footnotesOnPage, objectsOnPage));
                colHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[4], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD04, footnotesOnPage, objectsOnPage));
                colHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[5], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD05, footnotesOnPage, objectsOnPage));
                colHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[6], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD06, footnotesOnPage, objectsOnPage));
                colHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[7], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD07, footnotesOnPage, objectsOnPage));
                
                bodyHTML.Append("<table><tr>");
                for (int i = 0; i < colHtml.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}</th>", colHtml[i]);
                bodyHTML.Append("</tr>");
                foreach (IBDNode child in childNodes)
                {
                    BDConfiguredEntry entry = child as BDConfiguredEntry;
                    bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>",
                        entry.Name, entry.field01, entry.field02, entry.field03, entry.field04, entry.field05, entry.field06, entry.field07);
                    objectsOnPage.Add(child.Uuid);

                }
                bodyHTML.Append("</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        private BDHtmlPage generatePageForDentalProphylaxis(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTopic)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            // Topic > Table > TherapyGroup, etc
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotesOnPage, objectsOnPage));

            // retrieve 'inline' type of linked note, draw in a box
            List<BDLinkedNote> topicNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME,BDConstants.LinkedNoteType.Inline);
            StringBuilder noteText = new StringBuilder();
            foreach (BDLinkedNote note in topicNotes)
            {
                noteText.Append(note.documentText);
                objectsOnPage.Add(note.Uuid);
            }
            if(topicNotes.Count > 0 && noteText.Length > 0)
                bodyHTML.AppendFormat("<div style=\"border:1px dotted black;padding:2em;\">{0}</div>", noteText);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode table in childNodes)
            {
                if(table.Name != pNode.Name)
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, table, "h3", footnotesOnPage, objectsOnPage));

                List<string> columnHtml = new List<string>();
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, footnotesOnPage, objectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, footnotesOnPage, objectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, footnotesOnPage, objectsOnPage));
                
                bodyHTML.Append("<table><tr>");

                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}</th>", columnHtml[i]);
                bodyHTML.Append("</tr>");

                List<IBDNode> tableChildren = BDFabrik.GetChildrenForParent(pContext, table);
                foreach (IBDNode therapyGroup in tableChildren)
                {
                    BDTherapyGroup entry = therapyGroup as BDTherapyGroup;
                    bodyHTML.AppendFormat("<tr><td colspan=3><b>{0}</b></td></tr>",entry.Name);
                    objectsOnPage.Add(therapyGroup.Uuid);
                    List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, entry);
                    foreach (IBDNode therapy in therapies)
                    {
                        BDTherapy t = therapy as BDTherapy;
                        string name = buildNodePropertyHTML(pContext, therapy, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, footnotesOnPage, objectsOnPage);
                        string dosage = buildNodePropertyHTML(pContext, therapy, t.dosage, BDTherapy.PROPERTYNAME_DOSAGE, footnotesOnPage, objectsOnPage);
                        string dosage1 = buildNodePropertyHTML(pContext, therapy, t.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, footnotesOnPage, objectsOnPage);
                        bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                            name,dosage,dosage1);
                        objectsOnPage.Add(t.Uuid);
                    }
                }
                bodyHTML.Append("</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForDentalProphylaxisDrugRegimens(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            // Subcategory > Surgery > SurgeryClassification > TherapyGroup > Therapy
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));
            
                List<IBDNode> surgeries = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode surgery in surgeries)
                {
                    string c1Html = buildHtmlForMetadataColumn(pContext, surgery, metadataLayoutColumns[0], BDConstants.BDNodeType.BDSurgery, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage);

                    bodyHTML.AppendFormat("{0}{1}", buildNodeWithReferenceAndOverviewHTML(pContext, surgery, "h3", footnotesOnPage, objectsOnPage), c1Html);

                    List<IBDNode> surgeryChildren = BDFabrik.GetChildrenForParent(pContext, surgery); 
                    foreach (IBDNode surgeryChild in surgeryChildren)
                    {
                        // surgery classification - owns the child row
                        if (surgeryChild.Name.Length > 0 && !surgeryChild.Name.Contains(BDUtilities.GetEnumDescription(surgeryChild.NodeType)))
                            bodyHTML.AppendFormat("<ul><li>{0}</ul>", surgeryChild.Name);
                        objectsOnPage.Add(surgeryChild.Uuid);

                        string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, footnotesOnPage, objectsOnPage);
                        string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, footnotesOnPage, objectsOnPage);

                        List<IBDNode> tGroups = BDFabrik.GetChildrenForParent(pContext, surgeryChild);
                        if (tGroups.Count > 0)
                        {
                            StringBuilder adultDosageHTML = new StringBuilder();
                            StringBuilder pedsDosageHTML = new StringBuilder();
                            bodyHTML.Append(@"<table>");
                            bodyHTML.AppendFormat(@"<tr><th>{0}</th><th>{1}</th></tr>", c2Html, c3Html);
                            foreach (IBDNode therapyGroup in tGroups)
                            {
                                if (therapyGroup.Name.Length > 0 && !therapyGroup.Name.Contains("New Therapy Group"))
                                {
                                    adultDosageHTML.AppendFormat("<u>{0}</u><ul>", therapyGroup.Name);
                                    pedsDosageHTML.AppendFormat("<u>{0}</u><ul>", therapyGroup.Name);
                                }
                                else
                                {
                                    adultDosageHTML.Append("<ul>");
                                    pedsDosageHTML.Append("<ul>");
                                }
                                objectsOnPage.Add(therapyGroup.Uuid);
                                List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, therapyGroup);
                                #region process therapies
                                foreach (IBDNode t in therapies)
                                {
                                    BDTherapy therapy = t as BDTherapy;
                                    // therapy name - add to both cells
                                    if (therapy.nameSameAsPrevious.Value == true)
                                    {
                                        adultDosageHTML.AppendFormat("<li>{0}", buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, footnotesOnPage, objectsOnPage));
                                        pedsDosageHTML.AppendFormat("<li>{0}", buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, footnotesOnPage, objectsOnPage));
                                    }
                                    else
                                    {
                                        adultDosageHTML.AppendFormat("<li>{0}", buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, footnotesOnPage, objectsOnPage));
                                        pedsDosageHTML.AppendFormat("<li>{0}", buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, footnotesOnPage, objectsOnPage));
                                    }
                                    // Dosage - adult dose
                                    if (therapy.dosageSameAsPrevious.Value == true)
                                        adultDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, footnotesOnPage, objectsOnPage));
                                    else
                                        adultDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, footnotesOnPage, objectsOnPage));

                                    // Dosage 1 - Paediatric dose
                                    if (therapy.dosage1SameAsPrevious.Value == true)
                                        pedsDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyDosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, footnotesOnPage, objectsOnPage));
                                    else
                                        pedsDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, footnotesOnPage, objectsOnPage));

                                    // check for conjunctions and add a row for any that are found
                                    switch (therapy.therapyJoinType)
                                    {
                                        case (int)BDConstants.BDJoinType.OrWithNext:
                                            adultDosageHTML.AppendFormat(@"<br>{0}<br> ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                            pedsDosageHTML.AppendFormat(@"<br>{0}<br>", retrieveConjunctionString((int)therapy.therapyJoinType));
                                            break;
                                        default:
                                            adultDosageHTML.AppendFormat(@" {0} ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                            pedsDosageHTML.AppendFormat(@" {0} ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                            break;
                                    }
                                    previousTherapyId = therapy.Uuid;
                                    previousTherapyName = therapy.Name;
                                    previousTherapyDosage = therapy.dosage;
                                    previousTherapyDosage1 = therapy.dosage1;
                                }
                                #endregion
                                adultDosageHTML.Append("</ul>");
                                pedsDosageHTML.Append("</ul>");
                            }
                                bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td</tr>", adultDosageHTML, pedsDosageHTML);
                            bodyHTML.Append(@"</table>");
                        }
                    }
                }
                return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForDentalMicroorganisms(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            //Subcategory>MicroorganismGroup>Microorganism

            //metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotesOnPage, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode subcategory in childNodes)
            {
                bodyHTML.Append("<p>");
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, "h4", footnotesOnPage, objectsOnPage));
                List<IBDNode> mGroups = BDFabrik.GetChildrenForParent(pContext, subcategory);
                foreach (IBDNode group in mGroups)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, group, "b", footnotesOnPage, objectsOnPage));
                    bodyHTML.Append("<br>");
                    List<IBDNode> microorganisms = BDFabrik.GetChildrenForParent(pContext, group);
                    foreach (IBDNode microorganism in microorganisms)
                    {
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, microorganism, "", footnotesOnPage, objectsOnPage));
                        bodyHTML.Append("<br>");
                    }
                }
                bodyHTML.Append("</p>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }
        #endregion

        #region Pregnancy/Lactation Sections

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForPLAntimicrobialsInLactation(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            // Category > Subcategory > subcategoryChild > antimicrobial > antimicrobialRisk
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotesOnPage, objectsOnPage));

            List<string> columnHtml = new List<string>();
            columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK, footnotesOnPage, objectsOnPage));
            columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, footnotesOnPage, objectsOnPage));
            columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, footnotesOnPage, objectsOnPage));

            List<BDHtmlPage> subcatPages = new List<BDHtmlPage>();
            List<Guid> subcatObjectsOnPage = new List<Guid>();
            List<BDLinkedNote> subcatFootnotes = new List<BDLinkedNote>();
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode subcategory in childNodes)
            {
                StringBuilder subcatHTML = new StringBuilder();

                subcatHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, "h2", subcatFootnotes, subcatObjectsOnPage));
                List<IBDNode> subcategoryChildNodes = BDFabrik.GetChildrenForParent(pContext, subcategory);
                List<BDHtmlPage> apPages = new List<BDHtmlPage>();
                List<BDLinkedNote> apFootnotes = new List<BDLinkedNote>();
                List<Guid> apObjectsOnPage = new List<Guid>();
                StringBuilder apHTML = new StringBuilder();
                //
                //the next layer can either be antimicrobial group or antimicrobial
                //
                if (subcategoryChildNodes[0].NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
                {
                    // then we need to loop over all the antimicrobial groups here & add the name to the page with the associated antimicrobials.
                    foreach (IBDNode antimicrobialGroup in subcategoryChildNodes)
                    {
                        apHTML.Clear();
                        apHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, antimicrobialGroup, "h4", apFootnotes, apObjectsOnPage));
                        List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(pContext, antimicrobialGroup);
                        List<BDHtmlPage> amPages = new List<BDHtmlPage>();
                        amPages.AddRange(buildAntimicrobialWithRiskHTML(pContext, columnHtml, antimicrobials));
                        for (int i = 0; i < amPages.Count; i++)
                            apHTML.AppendFormat(anchorTag, amPages[i].Uuid.ToString().ToUpper(), antimicrobials[i].Name);
                            subcatHTML.Append(apHTML);
                    }
                    for (int i = 0; i < apPages.Count; i++)
                        subcatHTML.AppendFormat(anchorTag, apPages[i].Uuid.ToString().ToUpper(), subcategoryChildNodes[i].Name);
                    if (subcategory.Name.Length > 0)
                    {
                        currentPageMasterObject = subcategory;
                        subcatPages.Add(writeBDHtmlPage(pContext, subcategory, subcatHTML, BDConstants.BDHtmlPageType.Navigation, subcatFootnotes, subcatObjectsOnPage, null));
                    }
                    else
                        bodyHTML.Append(subcatHTML);
                }
                else
                { // antimicrobial group does not exist - the child nodes are antimicrobials
                    List<BDHtmlPage> amPages = new List<BDHtmlPage>();
                    amPages.AddRange(buildAntimicrobialWithRiskHTML(pContext, columnHtml, subcategoryChildNodes));
                    for (int i = 0; i < amPages.Count; i++)
                        apHTML.AppendFormat(anchorTag, amPages[i].Uuid.ToString().ToUpper(), subcategoryChildNodes[i].Name);
                    if (subcategory.Name.Length > 0)
                    {
                        currentPageMasterObject = subcategory;
                        apPages.Add(writeBDHtmlPage(pContext, subcategory, apHTML, BDConstants.BDHtmlPageType.Navigation, apFootnotes, apObjectsOnPage, null));
                    }
                    else
                        subcatHTML.Append(apHTML);
                }
            }
            for (int i = 0; i < subcatPages.Count; i++)
                bodyHTML.AppendFormat(anchorTag, subcatPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        public BDHtmlPage generatePageForPLAntimicrobialsInPregnancy(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            // Category > Subcategory > antimicrobial > antimicrobialRisk
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            List<BDHtmlPage> subcatPages = new List<BDHtmlPage>();
            List<Guid> subcatObjectsOnPage = new List<Guid>();
            List<BDLinkedNote> subcatFootnotes = new List<BDLinkedNote>();
            foreach (IBDNode subcategory in childNodes)
            {
                StringBuilder subcatHTML = new StringBuilder();
                subcatHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, "h2", subcatFootnotes, subcatObjectsOnPage));
                List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(pContext, subcategory);
                List<BDHtmlPage> amPages = new List<BDHtmlPage>();
                List<BDLinkedNote> amFootnotes = new List<BDLinkedNote>();

                string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, footnotesOnPage, objectsOnPage);
                string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, footnotesOnPage, objectsOnPage);

                foreach (IBDNode antimicrobial in antimicrobials)
                {
                    // write an HTML page for the antimicrobial, build a link for the name
                    StringBuilder antimicrobialHTMLBody = new StringBuilder();
                    List<Guid> antimicrobialsOnPage = new List<Guid>();
                    antimicrobialHTMLBody.Append(buildNodeWithReferenceAndOverviewHTML(pContext, antimicrobial as BDNode, "h4", amFootnotes, antimicrobialsOnPage));
                    
                    List<IBDNode> amRisks = BDFabrik.GetChildrenForParent(pContext, antimicrobial);
                    foreach (IBDNode amRisk in amRisks)
                    {
                        BDAntimicrobialRisk risk = amRisk as BDAntimicrobialRisk;
                        if (risk.riskFactor.Length > 0)
                            antimicrobialHTMLBody.AppendFormat("<b>{0}</b>: {1}", c1Html, buildNodePropertyHTML(pContext, risk, risk.riskFactor, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, footnotesOnPage, objectsOnPage));
                        if (risk.recommendations.Length > 0)
                            antimicrobialHTMLBody.AppendFormat("<p><b>{0}</b><br>{1}</p>", c2Html, buildNodePropertyHTML(pContext, risk, risk.recommendations, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, footnotesOnPage, objectsOnPage));
                        antimicrobialsOnPage.Add(amRisk.Uuid);
                        amFootnotes.AddRange(footnotesOnPage);

                        amFootnotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, amRisk.Uuid, BDAntimicrobialRisk.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));
                        amFootnotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, amRisk.Uuid, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, BDConstants.LinkedNoteType.Footnote));
                    }
                    string commentText = buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, antimicrobial, BDConstants.LinkedNoteType.UnmarkedComment, antimicrobialsOnPage);
                    if (commentText.Length > 0)
                        antimicrobialHTMLBody.AppendFormat("<p><b>Comments</b><br>{0}</p>", commentText);
                    currentPageMasterObject = antimicrobial;
                    amPages.Add(writeBDHtmlPage(pContext, pNode, antimicrobialHTMLBody, BDConstants.BDHtmlPageType.Data, amFootnotes, antimicrobialsOnPage, null));
                }
                for (int i = 0; i < amPages.Count; i++)
                    subcatHTML.AppendFormat(anchorTag, amPages[i].Uuid.ToString().ToUpper(), antimicrobials[i].Name);
                currentPageMasterObject = subcategory;
                subcatPages.Add(writeBDHtmlPage(pContext, pNode, subcatHTML, BDConstants.BDHtmlPageType.Navigation, subcatFootnotes, subcatObjectsOnPage, null));

            }
            for(int i = 0; i < subcatPages.Count; i++)
                bodyHTML.AppendFormat(anchorTag, subcatPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForPLCommunicableDiseases(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            // Section > Pathogen > Topic with Overview
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, pNode);
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));
            bodyHTML.Append("<h2>Infectious Agent</h2>");
            List<BDHtmlPage> pages = new List<BDHtmlPage>();
            foreach (IBDNode pathogen in pathogens)
            {
                StringBuilder pathogenHTML = new StringBuilder();
                List<Guid> objectsOnPathogenPage = new List<Guid>();
                List<BDLinkedNote> pFootnotes = new List<BDLinkedNote>();
                pathogenHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pathogen, "h3", pFootnotes, objectsOnPathogenPage));
                pathogenHTML.Append(buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, pathogen, BDConstants.LinkedNoteType.UnmarkedComment, objectsOnPathogenPage));

                List<IBDNode> topics = BDFabrik.GetChildrenForParent(pContext, pathogen);
                foreach (IBDNode topic in topics)
                {
                    if (topic.Name != "Infectious Agent")
                    {
                        pathogenHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topic, "h4", pFootnotes, objectsOnPathogenPage));
                        objectsOnPathogenPage.Add(topic.Uuid);
                    }
                }
                currentPageMasterObject = pathogen;
                pages.Add(writeBDHtmlPage(pContext, pathogen, pathogenHTML, BDConstants.BDHtmlPageType.Data, pFootnotes, objectsOnPathogenPage, null));
            }
            for (int i = 0; i < pages.Count; i++)
                bodyHTML.AppendFormat(anchorTag, pages[i].Uuid.ToString().ToUpper(), pathogens[i].Name);
            
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode as BDNode, bodyHTML, BDConstants.BDHtmlPageType.Navigation, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForPLPreventionPerinatalInfection(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            List<string> columnHtml = new List<string>();
            columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, footnotesOnPage, objectsOnPage));
            columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, footnotesOnPage, objectsOnPage));

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

            bodyHTML.Append("<table><tr>");
            for (int i = 0; i < metadataLayoutColumns.Count; i++)
                bodyHTML.AppendFormat("<th>{0}</th>", columnHtml[i]);
            bodyHTML.Append("</tr>");

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode tGroup in childNodes)
            {
                if (tGroup.Name.Length > 0 && !tGroup.Name.Contains(BDUtilities.GetEnumDescription(tGroup.NodeType)))
                    bodyHTML.AppendFormat("<tr><td><b>{0}</b></td><td /></tr>", tGroup.Name);

                List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, tGroup);
                if (therapies.Count > 0)
                {
                    foreach (BDTherapy therapy in therapies)
                    {
                        bodyHTML.Append(buildTherapyWithCombinedColumnHtml(pContext, therapy, footnotesOnPage, objectsOnPage));

                        if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                            previousTherapyName = therapy.Name;
                        if (!string.IsNullOrEmpty(therapy.dosage))
                        {
                            if (therapy.dosageSameAsPrevious == false)
                                previousTherapyDosage = therapy.dosage;
                        }
                    }
                }
            }
            bodyHTML.Append("</table>");
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForPLPerinatalHIVProtocol(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
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

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }
        
        #endregion

        #region Microbiology/Organisms Sections

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForOrganismGramStain(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            // gram positive or gram negative
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0) // subcategory - column 1 values
            {
                List<string> columnHtml = new List<string>();
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDSubcategory, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDMicroorganism, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));

                bodyHTML.Append("<table><tr>");
                for (int i = 0; i < columnHtml.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}</th>", columnHtml[i]);
                bodyHTML.Append("</tr>");
                
                foreach (IBDNode child in childNodes)
                {
                    BDNode subcategory = child as BDNode;
                    List<IBDNode> mos = BDFabrik.GetChildrenForParent(pContext, subcategory);
                    StringBuilder mString = new StringBuilder();
                    foreach (IBDNode microorganism in mos)
                    {
                        mString.AppendFormat("{0}<br>", buildNodePropertyHTML(pContext, microorganism, microorganism.Name, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                    }
                    string scString = buildNodePropertyHTML(pContext, subcategory, subcategory.Name, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage);
                    bodyHTML.AppendFormat("<tr><td><ul><li>{0}</ul></td><td>{1}</td></tr>", scString, mString);
                    objectsOnPage.Add(child.Uuid);
                }
                bodyHTML.Append("</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        [Obsolete("use GenerateBDHtmlPage instead")]
        private BDHtmlPage generatePageForMicrobiologyOrganisms(Entities pContext, IBDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }
            // category > subcategory > microorganismGroup > microorganism
            // no metadata required // metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pDisplayParentNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotesOnPage, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode subcategory in childNodes)
            {
                // microorganism group
                bodyHTML.Append(buildNodePropertyHTML(pContext, subcategory, subcategory.Uuid, subcategory.Name, BDNode.PROPERTYNAME_NAME, "h2", footnotesOnPage, objectsOnPage));


                List<IBDNode> mGroups = BDFabrik.GetChildrenForParent(pContext, subcategory);
                foreach (IBDNode mGroup in mGroups)
                {
                    bodyHTML.Append(buildNodePropertyHTML(pContext, mGroup, mGroup.Uuid, mGroup.Name, BDNode.PROPERTYNAME_NAME, "h4", footnotesOnPage, objectsOnPage));

                    List<IBDNode> microorganisms = BDFabrik.GetChildrenForParent(pContext, mGroup);
                    foreach (IBDNode microorganism in microorganisms)
                        bodyHTML.AppendFormat("{0}<br>", buildNodePropertyHTML(pContext, microorganism, microorganism.Name, BDNode.PROPERTYNAME_NAME, footnotesOnPage, objectsOnPage));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotesOnPage, objectsOnPage, null);
        }

        #endregion

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
        /// <param name="pLayoutColumnId"></param>
        /// <param name="pLayoutType"></param>
        /// <param name="pInlineNotes"></param>
        /// <param name="pMarkedNotes"></param>
        /// <param name="pUnmarkedNotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <returns></returns>
        private BDHtmlPage generatePageForLinkedNotesLayoutColumn(Entities pContext, BDLayoutMetadataColumn pLayoutColumn, List<BDLinkedNote> pMarkedNotes, List<BDLinkedNote> pUnmarkedNotes)
        {
            StringBuilder noteHtml = new StringBuilder();

            List<Guid> objectsOnPage = new List<Guid>();

            if (null != pMarkedNotes && notesListHasContent(pContext, pMarkedNotes))
            {
                foreach (BDLinkedNote mNote in pMarkedNotes)
                {
                    if (mNote.documentText.Length > EMPTY_PARAGRAPH)
                    {
                        noteHtml.Append(mNote.documentText);
                       // objectsOnPage.Add(mNote.Uuid);
                    }
                }
            }

            if (null != pUnmarkedNotes && notesListHasContent(pContext, pUnmarkedNotes))
            {
                foreach (BDLinkedNote uNote in pUnmarkedNotes)
                {
                    if (uNote.documentText.Length > EMPTY_PARAGRAPH)
                    {
                        noteHtml.Append(uNote.documentText);
                        //objectsOnPage.Add(uNote.Uuid);
                    }
                }
            }

            List<BDHtmlPage> columnHtmlPages = BDHtmlPage.RetrieveHtmlPageForDisplayParentId(pContext, pLayoutColumn.Uuid);
            BDHtmlPage resultPage = null;
            foreach (BDHtmlPage page in columnHtmlPages)
                if (page.documentText.Contains(noteHtml.ToString()))
                    resultPage = page;

            if (noteHtml.ToString().Length > EMPTY_PARAGRAPH && resultPage == null)
            {
                resultPage = writeLayoutBDHtmlPage(pContext, pLayoutColumn, noteHtml.ToString(), objectsOnPage, BDConstants.BDHtmlPageType.Comments);
            }

            return resultPage;
        }

        private BDHtmlPage generatePageForLinkedNotes(Entities pContext, Guid pParentId, BDConstants.BDNodeType pParentType, List<BDLinkedNote> pMarkedNotes, List<BDLinkedNote> pUnmarkedNotes, string pParentKeyPropertyName)
        {
            StringBuilder noteHtml = new StringBuilder();
            List<Guid> objectsOnPage = new List<Guid>();

            if (null != pMarkedNotes && notesListHasContent(pContext, pMarkedNotes))
            {
                foreach (BDLinkedNote mNote in pMarkedNotes)
                {
                    if (mNote.documentText.Length > EMPTY_PARAGRAPH)
                    {
                        noteHtml.Append(mNote.documentText);
                       //objectsOnPage.Add(mNote.Uuid);
                    }
                }
            }

            if (null != pUnmarkedNotes && notesListHasContent(pContext, pUnmarkedNotes))
            {
                foreach (BDLinkedNote uNote in pUnmarkedNotes)
                {
                    if (uNote.documentText.Length > EMPTY_PARAGRAPH)
                    {
                        noteHtml.Append(uNote.documentText);
                        //objectsOnPage.Add(uNote.Uuid);
                    }
                }
            }

            return generatePageForLinkedNotes(pContext, pParentId, pParentType, noteHtml.ToString(), BDConstants.BDHtmlPageType.Comments, objectsOnPage, pParentKeyPropertyName);
        }

        private BDHtmlPage generatePageForLinkedNotes(Entities pContext, Guid pDisplayParentId, BDConstants.BDNodeType pDisplayParentType, string pPageHtml, BDConstants.BDHtmlPageType pPageType, List<Guid> pObjectsOnPage, string pParentKeyPropertyName)
        {
            if (pPageHtml.Length > EMPTY_PARAGRAPH)
            {
                // the linked note being processed will have a parent that is a BDNode OR another linked note
                IBDNode node = BDFabrik.RetrieveNode(pContext, pDisplayParentId);
                BDLinkedNote linkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, pDisplayParentId);
                if (node != null)
                {
                    currentPageMasterObject = node;
                    return writeBDHtmlPage(pContext, node, pPageHtml, pPageType, new List<BDLinkedNote>(), pObjectsOnPage, pParentKeyPropertyName);
                }
                else if (linkedNote != null)
                {
                    currentPageMasterObject = linkedNote;
                    return writeBDHtmlPage(pContext, linkedNote, pPageHtml, pPageType, new List<BDLinkedNote>(), pObjectsOnPage, pParentKeyPropertyName);
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
        /// <returns>Guid of HTML page.</returns>
        /// 
        private Guid generatePageForParentAndPropertyReferences(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            List<Guid> objectsOnPage = new List<Guid>();
            string reference = buildTextForParentAndPropertyFromLinkedNotes(pContext, pPropertyName, pNode, BDConstants.LinkedNoteType.Reference, objectsOnPage);
            if (reference.Length > EMPTY_PARAGRAPH)
            {
                StringBuilder referenceText = new StringBuilder();
                referenceText.AppendFormat(@"<h2>{0} References</h2>", pNode.Name);
                referenceText.Append(reference);
                BDHtmlPage footnote = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, referenceText.ToString(), BDConstants.BDHtmlPageType.Reference, objectsOnPage, pPropertyName);
               
                List<Guid> filteredObjects = objectsOnPage.Distinct().ToList();
                foreach (Guid id in filteredObjects)
                    pagesMap.Add(new BDHtmlPageMap(footnote.Uuid, id));

                //ks: keep track of the create pages.
                BDHtmlPageGeneratorLogEntry logEntry = new BDHtmlPageGeneratorLogEntry(footnote.Uuid, pNode.Name);
                this.referencePageUuidList.Add(logEntry);
                logEntry.AppendToFile(@"refPageUuidAppendLog.txt");

                return footnote.Uuid;
            }
            else
                return Guid.Empty;
        }
        #endregion

        #region Build HTML component parts
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
            List<BDLinkedNote> notes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, pNoteType);
            notesHTML.Append(buildTextFromNotes(notes,pObjectsOnPage));
            return notesHTML.ToString();
        }

        /// <summary>
        /// Build HTML for Empiric Therapy beginning at pathogenGroup level
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <param name="pFooterList"></param>
        /// <returns></returns>
        [Obsolete("Use buildBDPathogenGroupHtml instead")]
        private StringBuilder buildEmpiricTherapyHTML(Entities pContext, BDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            //ks: Consider refactoring to use buildBDPathogenGroupHtml

             StringBuilder bodyHtml = new StringBuilder();

                bodyHtml.Append(buildPathogenGroupHtml(pContext, pNode, pFootnotes, pObjectsOnPage));

                // process therapy groups
                List<BDTherapyGroup> therapyGroups = BDTherapyGroup.RetrieveTherapyGroupsForParentId(pContext, pNode.Uuid);

                foreach (BDTherapyGroup tGroup in therapyGroups)
                {
                    BDTherapyGroup group = tGroup as BDTherapyGroup;
                    if (null != group)
                    {
                        bodyHtml.Append(buildNodePropertyHTML(pContext, group, group.Name, BDTherapyGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                        bodyHtml.Append(buildTherapyGroupHTML(pContext, group, pFootnotes, pObjectsOnPage));
                }
            }
            return bodyHtml;
        }

        /// <summary>
        /// Build HTML for therapy group, and associated therapies enclosed in a table
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pTherapyGroup"></param>
        /// <returns></returns>
        /// 
        [Obsolete("Use BuildBDTherapyGroupHTML instead")]
        private StringBuilder buildTherapyGroupHTML(Entities pContext, BDTherapyGroup pTherapyGroup, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder therapyGroupHtml = new StringBuilder();
            List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, pTherapyGroup.Uuid);
            if (therapies.Count > 0)
            {
                therapyGroupHtml.Append(@"<table>");

                StringBuilder therapyHTML = new StringBuilder();
                therapiesHaveDosage = false;
                therapiesHaveDuration = false;
                foreach (BDTherapy therapy in therapies)
                {
                    therapyHTML.Append(buildTherapyHtml(pContext, therapy, pFootnotes, pObjectsOnPage));

                    if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                        previousTherapyName = therapy.Name;
                    if (!string.IsNullOrEmpty(therapy.dosage))
                    {
                        if (therapy.dosageSameAsPrevious == false)
                            previousTherapyDosage = therapy.dosage;
                        therapiesHaveDosage = true;
                    }
                    if (!string.IsNullOrEmpty(therapy.duration))
                    {
                        if (therapy.durationSameAsPrevious == false)
                            previousTherapyDuration = therapy.duration;
                        therapiesHaveDuration = true;
                    }
                }

                // zzz
                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, BDConstants.LayoutVariantType.TreatmentRecommendation01);
                string t0Html = buildHtmlForMetadataColumn(pContext, pTherapyGroup, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                string t1Html = buildHtmlForMetadataColumn(pContext, pTherapyGroup, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                string t2Html = buildHtmlForMetadataColumn(pContext, pTherapyGroup, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, pFootnotes, pObjectsOnPage);

                //if (!therapiesHaveDosage && !therapiesHaveDuration)
                //    therapyGroupHtml.Append(@"<tr><th>Recommended Empiric Therapy</th>");
                //else
                //    therapyGroupHtml.Append(@"<tr><th>Recommended<br>Empiric<br>Therapy</th>");

                therapyGroupHtml.AppendFormat(@"<tr><th>{0}</th>", t0Html);

                if (therapiesHaveDosage)
                    therapyGroupHtml.AppendFormat(@"<th>{0}</th>", t1Html);
                    //therapyGroupHtml.Append(@"<th>Recommended<br>Dose</th>");
                else
                    therapyGroupHtml.Append(@"<th />");

                if (therapiesHaveDuration)
                    therapyGroupHtml.AppendFormat(@"<th>{0}</th>", t2Html);
                    //therapyGroupHtml.Append(@"<th>Recommended<br>Duration</th>");
                else
                    therapyGroupHtml.Append(@"<th />");

                therapyGroupHtml.Append(@"</tr>");

                therapyGroupHtml.Append(therapyHTML);
                therapyGroupHtml.Append(@"</table>");
            }
            return therapyGroupHtml;
        }

        /// <summary>
        /// Build reference HTML for a therapyGroup. Renders the therapy Group and its child therapys
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pTherapyGroup"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <param name="pLevel"></param>
        /// <param name="pLayoutOverride">LayoutVariant to override pTherapyGroup.LayoutVarinat. Use null or Undefined otherwise</param>
        /// <returns></returns>
        public StringBuilder BuildBDTherapyGroupHTML(Entities pContext, BDTherapyGroup pTherapyGroup, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, BDConstants.LayoutVariantType? pLayoutOverride)
        {
            // Exception occurs in BuildBDPathogenResistanceHtml, BuildBDPathogenGroupHtml

            StringBuilder therapyGroupHtml = new StringBuilder();

            therapyGroupHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pTherapyGroup, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

            string therapyNameTitleHtml = string.Empty;
            string therapyDosageTitleHtml = string.Empty;
            string therapyDurationTitleHtml = string.Empty;
            // Because this is used across many layout variants without rendering differences, allow for an override
            BDConstants.LayoutVariantType layoutLayoutVariant = (pLayoutOverride.HasValue && pLayoutOverride != BDConstants.LayoutVariantType.Undefined) ? pLayoutOverride.Value : pTherapyGroup.LayoutVariant;

            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, layoutLayoutVariant, BDConstants.BDNodeType.BDTherapy);

            if (metadataLayoutColumns.Count > 0)
                therapyNameTitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
            if (metadataLayoutColumns.Count > 1)
                therapyDosageTitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
            if (metadataLayoutColumns.Count > 2)
                therapyDurationTitleHtml = buildHtmlForMetadataColumn(pContext, pTherapyGroup, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, pFootnotes, pObjectsOnPage);

            List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, pTherapyGroup.Uuid);
            if (therapies.Count > 0)
            {
                therapyGroupHtml.Append(@"<table>");

                StringBuilder therapyHTML = new StringBuilder();
                resetGlobalVariablesForTherapies();
                foreach (BDTherapy therapy in therapies)
                {
                    therapyHTML.Append(buildTherapyHtml(pContext, therapy, pFootnotes, pObjectsOnPage));

                    if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                        previousTherapyName = therapy.Name;
                    if (!string.IsNullOrEmpty(therapy.dosage))
                    {
                        if (therapy.dosageSameAsPrevious == false)
                            previousTherapyDosage = therapy.dosage;
                        therapiesHaveDosage = true;
                    }

                    if (!string.IsNullOrEmpty(therapy.duration))
                    {
                        if (therapy.durationSameAsPrevious == false)
                            previousTherapyDuration = therapy.duration;
                        therapiesHaveDuration = true;
                    }

                    previousTherapyId = therapy.Uuid;
                }

                // Build the header
                switch (pTherapyGroup.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        // use default titles if layout configuration data is misssing
                        if (therapyNameTitleHtml.Length == 0) therapyNameTitleHtml = "Recommended Empiric Therapy";
                        if (therapyDosageTitleHtml.Length == 0) therapyDosageTitleHtml = "Recommended Dose";
                        if (therapyDurationTitleHtml.Length == 0) therapyDurationTitleHtml = "Recommended Duration";

                        therapyGroupHtml.AppendFormat(@"<tr><th>{0}</th>", therapyNameTitleHtml);

                        if (therapiesHaveDosage)
                            therapyGroupHtml.AppendFormat(@"<th>{0}</th>", therapyDosageTitleHtml);
                            //therapyGroupHtml.Append(@"<th>Recommended<br>Dose</th>");
                        else
                            therapyGroupHtml.Append(@"<th />");

                        if (therapiesHaveDuration)
                            therapyGroupHtml.AppendFormat(@"<th>{0}</th>", therapyDurationTitleHtml);
                            //therapyGroupHtml.Append(@"<th>Recommended<br>Duration</th>");
                        else
                            therapyGroupHtml.Append(@"<th />");

                        therapyGroupHtml.Append(@"</tr>");
                        therapyGroupHtml.Append(therapyHTML);
                        therapyGroupHtml.Append(@"</table>");
                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                    case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                        therapyGroupHtml.AppendFormat(@"<tr><th>{0}</th><th>{1}</th><tr>",therapyNameTitleHtml,therapyDosageTitleHtml);
                        therapyGroupHtml.Append(therapyHTML);
                        therapyGroupHtml.Append(@"</table>");
                        break;

                    case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                        // not handled here: Category handler

                        break;

                    default:
                        if (therapyNameTitleHtml.Length == 0) therapyNameTitleHtml = "Therapy";
                        if (therapyDosageTitleHtml.Length == 0) therapyDosageTitleHtml = "Dose";
                        if (therapyDurationTitleHtml.Length == 0) therapyDurationTitleHtml = "Duration";

                        therapyGroupHtml.AppendFormat(@"<tr><th>{0}</th>", therapyNameTitleHtml);

                        if (therapiesHaveDosage)
                            therapyGroupHtml.AppendFormat(@"<th>{0}</th>", therapyDosageTitleHtml);
                        else
                            therapyGroupHtml.Append(@"<th />");

                        if (therapiesHaveDuration)
                            therapyGroupHtml.AppendFormat(@"<th>{0}</th>", therapyDurationTitleHtml);
                        else
                            therapyGroupHtml.Append(@"<th />");

                        therapyGroupHtml.Append(@"</tr>");
                        therapyGroupHtml.Append(therapyHTML);
                        therapyGroupHtml.Append(@"</table>");
                        break;
                }
            }
            return therapyGroupHtml;
        }

        /// <summary>
        /// Generate HTML for Pathogen Group and Pathogen
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns></returns>
         
        [Obsolete("Use buildBDPathogenGroupHtml instead")] 
        private string buildPathogenGroupHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder pathogenGroupHtml = new StringBuilder();

            BDNode pathogenGroup = pNode as BDNode;
            if (null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
            {
                pathogenGroupHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", pFootnotes, pObjectsOnPage));

                List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, pathogenGroup);
                if (pathogens.Count > 0)
                    pathogenGroupHtml.Append(@"<h3>Usual Pathogens</h3>");

                foreach (IBDNode pathogen in pathogens)
                    if (pathogen.NodeType == BDConstants.BDNodeType.BDPathogen) // bypass the therapy groups that appear at this level
                        pathogenGroupHtml.AppendFormat("{0}<br>", (buildNodeWithReferenceAndOverviewHTML(pContext, pathogen, "", pFootnotes, pObjectsOnPage)));
            }
            return pathogenGroupHtml.ToString();
        }

        /// <summary>
        /// Build reference HTML for a pathogenGroup. BDFabrik defines BDPathogen and BDTherapyGroup as possible children
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pFootnotes"></param>
        /// <param name="pObjectsOnPage"></param>
        /// <returns></returns>
        public string BuildBDPathogenGroupHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel, bool pUsualPathogenTitle)
        {
            // Exception: TherapyGroup & Therapy
            StringBuilder html = new StringBuilder();

            BDNode pathogenGroup = pNode as BDNode;
            if (null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
            {
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pathogenGroup);

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
                                html.Append(@"<table>");

                                resetGlobalVariablesForTherapies();
                                StringBuilder therapyHTML = new StringBuilder();
                                foreach (BDTherapy therapy in therapies)
                                {
                                    therapyHTML.Append(buildTherapyWithTwoDosagesHtml(pContext, therapy, true, pFootnotes, pObjectsOnPage));

                                    if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                        previousTherapyName = therapy.Name;

                                    previousTherapyId = therapy.Uuid;

                                }
                                html.AppendFormat(@"<tr><th>{0}</th>",therapyNameTitleHtml);
                                html.AppendFormat(@"<th>{0}</th><th>{1}</th>", therapyDosage1TitleHtml, therapyDosage2TitleHtml);
                                html.AppendFormat(@"<th>{0}</th>", therapyDurationTitleHtml);

                                html.Append(@"</tr>");

                                html.Append(therapyHTML);
                                html.Append(@"</table>");
                            }
                        }

                        List<BDLinkedNote> legendNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.ParentId.Value, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Legend);
                        string legendHTML = buildTextFromNotes(legendNotes, pObjectsOnPage);
                        if (legendHTML.Length > EMPTY_PARAGRAPH)
                            html.Append(legendHTML);
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
                        if (pUsualPathogenTitle && childrenHavePathogens)
                        {
                            string title = "Usual Pathogens";
                            if (pNode.ParentId == Guid.Parse("54f2fcf0-8cbb-42d0-b61e-494838b1920e")) title = "Potential Pathogens";
                            html.Append(string.Format("<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), title));
                        }

                        // describe the pathogen group
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPathogen:
                                    html.AppendFormat("{0}<br>", (buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage)));
                                    break;
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                    if (null != therapyGroup)
                                    {
                                        html.AppendFormat("{0}<br>", (BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 2, null)));
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            return html.ToString();
        }

        public string BuildBDPathogenHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel )
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
                                html.Append(buildNodePropertyHTML(pContext, therapyGroupChild, therapyGroupChild.Name, BDTherapyGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                                html.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                            }
                        }

                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        string namePlaceholderText = string.Format(@"New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
                        if (pNode.Name.Length > 0 && !pNode.Name.Contains(namePlaceholderText))
                            html.AppendFormat(@"<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), pNode.Name);

                        pObjectsOnPage.Add(pNode.Uuid);
                        html.Append(buildReferenceHtml(pContext, pNode, pObjectsOnPage));

                        foreach (IBDNode child in children) 
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDPresentation:

                                    string symptoms = retrieveNoteTextForOverview(pContext, child.Uuid, pObjectsOnPage);
                                    if (symptoms.Length > EMPTY_PARAGRAPH)
                                        html.AppendFormat(@"<u><b>Symptoms</b></u><br>{0}", symptoms);
                                    pObjectsOnPage.Add(child.Uuid);
                                   break;
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel + 2), pFootnotes, pObjectsOnPage));
                                    html.Append(BuildBDTherapyGroupHTML(pContext, child as BDTherapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                                    break;
                            }
                        }

                        // overview - contains 'Comments'
                        string comments = retrieveNoteTextForOverview(pContext, pNode.Uuid, pObjectsOnPage);
                        if (comments.Length > EMPTY_PARAGRAPH)
                            html.AppendFormat(@"<u><b>Comments</b></u><br>{0}", comments);
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        StringBuilder diagnosis = new StringBuilder();
                        StringBuilder clinical = new StringBuilder();
                        StringBuilder therapy = new StringBuilder();

                        diagnosis.Append(string.Format("<{0}>Diagnosis</{0}>", HtmlHeaderTagLevelString(pLevel + 1)));
                        clinical.Append(string.Format("<{0}>Clinical</{0}>", HtmlHeaderTagLevelString(pLevel + 1)));

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDTherapyGroup:
                                    BDTherapyGroup therapyGroup = child as BDTherapyGroup;
                                    therapy.Append(BuildBDTherapyGroupHTML(pContext, therapyGroup, pFootnotes, pObjectsOnPage, pLevel + 1, null));
                                    break;
                                case BDConstants.BDNodeType.BDTopic:
                                    clinical.AppendFormat(@"<p><b>{0}</b><br>", child.Name);
                                    pObjectsOnPage.Add(child.Uuid);
                                    clinical.AppendFormat(@"{0}</p>", buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, child, BDConstants.LinkedNoteType.Inline, pObjectsOnPage));
                                    // overview contains the 'Diagnosis' column data
                                    diagnosis.Append(retrieveNoteTextForOverview(pContext, child.Uuid, pObjectsOnPage));
                                    break;
                            }
                        }
                        html.AppendFormat(@"{0}<br>{1}<br>{2}", clinical, diagnosis, therapy);

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

                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        // custom-built - Therapy has 2 dosages and a custom header

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


                        foreach (IBDNode child in children)
                        {
                            List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, child.Uuid);
                            if (therapies.Count > 0)
                            {
                                html.Append(@"<table>");

                                resetGlobalVariablesForTherapies();
                                StringBuilder therapyHTML = new StringBuilder();
                                foreach (BDTherapy therapy in therapies)
                                {
                                    therapyHTML.Append(buildTherapyWithTwoDurationsHtml(pContext, therapy, pFootnotes, pObjectsOnPage));

                                    if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                        previousTherapyName = therapy.Name;
                                    if (!string.IsNullOrEmpty(therapy.dosage))
                                    {
                                        if (therapy.dosageSameAsPrevious == false)
                                            previousTherapyDosage = therapy.dosage;
                                        therapiesHaveDosage = true;
                                    }
                                    if (!string.IsNullOrEmpty(therapy.duration))
                                    {
                                        if (therapy.durationSameAsPrevious == false)
                                            previousTherapyDuration = therapy.duration;
                                        therapiesHaveDuration = true;
                                    }
                                }
                                    html.AppendFormat(@"<tr><th>{0}</th>",therapyNameTitleHtml);
                                if (therapiesHaveDosage)
                                {
                                    html.AppendFormat(@"<th>{0}</th>",therapyDosage1TitleHtml);
                                }
                                else
                                    html.Append(@"<th />");
                                if (therapiesHaveDuration)
                                    html.AppendFormat(@"<th colspan=2>{0}</th>", therapyDurationSpanTitleHtml);
                                else
                                    html.Append("<th colspan=2></th>");
                                html.AppendFormat(@"</tr><tr><th /><th /><th>{0}</th><th>{1}</th></tr>",therapyDuration1TitleHtml,therapyDuration2TitleHtml);

                                html.Append(therapyHTML);
                                html.Append(@"</table>");
                            }
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        // custom-built - Therapy has 2 dosages and a custom header

                        string t1therapyNameTitleHtml = string.Empty;
                        string t1therapyDosage1TitleHtml = string.Empty;
                        string t1therapyDurationSpanTitleHtml = string.Empty;

                        List<BDLayoutMetadataColumn> t1metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis);

                        if (t1metadataLayoutColumns.Count > 0)
                            t1therapyNameTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, t1metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                        if (t1metadataLayoutColumns.Count > 1)
                            t1therapyDosage1TitleHtml = buildHtmlForMetadataColumn(pContext, pNode, t1metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                        if (t1metadataLayoutColumns.Count > 2)
                            t1therapyDurationSpanTitleHtml = buildHtmlForMetadataColumn(pContext, pNode, t1metadataLayoutColumns[2], BDConstants.BDNodeType.BDMetaDecoration, pFootnotes, pObjectsOnPage);


                        foreach (IBDNode child in children)
                        {
                            List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, child.Uuid);
                            if (therapies.Count > 0)
                            {
                                html.Append(@"<table>");

                                resetGlobalVariablesForTherapies();
                                StringBuilder therapyHTML = new StringBuilder();
                                foreach (BDTherapy therapy in therapies)
                                {
                                    therapyHTML.Append(buildTherapyHtml(pContext, therapy, pFootnotes, pObjectsOnPage));

                                    if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                        previousTherapyName = therapy.Name;
                                    if (!string.IsNullOrEmpty(therapy.dosage))
                                    {
                                        if (therapy.dosageSameAsPrevious == false)
                                            previousTherapyDosage = therapy.dosage;
                                        therapiesHaveDosage = true;
                                    }
                                    if (!string.IsNullOrEmpty(therapy.duration))
                                    {
                                        if (therapy.durationSameAsPrevious == false)
                                            previousTherapyDuration = therapy.duration;
                                        therapiesHaveDuration = true;
                                    }
                                }
                                html.AppendFormat(@"<tr><th>{0}</th>", t1therapyNameTitleHtml);
                                if (therapiesHaveDosage)
                                {
                                    html.AppendFormat(@"<th>{0}</th>", t1therapyDosage1TitleHtml);
                                }
                                else
                                    html.Append(@"<th />");
                                if (therapiesHaveDuration)
                                    html.AppendFormat(@"<th colspan=2>{0}</th>", t1therapyDurationSpanTitleHtml);
                                else
                                    html.Append("<th colspan=2></th>");

                                html.Append(therapyHTML);
                                html.Append(@"</table>");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return html.ToString();
        }

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

                        //html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                        string amHtml = buildNodePropertyHTML(pContext, pNode, pNode.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                        List<BDLinkedNote> amFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
                        string amFooterMarker = buildFooterMarkerForList(amFooters, true, pFootnotes, pObjectsOnPage);
                        // build each row of table, with antimicrobial name in first column
                        html.AppendFormat(@"<tr class=v""{0}""><td>{1}</td>", (int)pNode.LayoutVariant, amHtml);

                        StringBuilder dosageHTML = new StringBuilder();
                        dosageHTML.Append("<td>");
                        StringBuilder costHTML = new StringBuilder();
                        costHTML.Append("<td>");
                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDTopic:
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));

                                    dosageHTML.AppendFormat("<u>{0}</u><br>", buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false, pFootnotes, pObjectsOnPage));
                                    costHTML.Append("<br>");
                                    List<IBDNode> topicChildren = BDFabrik.GetChildrenForParent(pContext, child);
                                    foreach (IBDNode topicChild in topicChildren)
                                    {
                                        switch (topicChild.NodeType)
                                        {
                                            case BDConstants.BDNodeType.BDDosageGroup:
                                                List<IBDNode> topicDosageGroupChildren = BDFabrik.GetChildrenForParent(pContext, topicChild);
                                                string topicDosageCellLineTag = (topicDosageGroupChildren.Count > 0) ? "<br>" : "";

                                                foreach (IBDNode topicDosageGroupChild in topicDosageGroupChildren)
                                                {
                                                    // BDDosage
                                                    BDDosage topicDosage = topicDosageGroupChild as BDDosage;

                                                    if (topicDosage.joinType == (int)BDConstants.BDJoinType.Next)
                                                        dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, topicDosage, BDDosage.PROPERTYNAME_DOSAGE, topicDosage.dosage, false, pFootnotes, pObjectsOnPage), topicDosageCellLineTag);
                                                    else
                                                        dosageHTML.AppendFormat("{0} {1}{2}", buildCellHTML(pContext, topicDosage, BDDosage.PROPERTYNAME_DOSAGE, topicDosage.dosage, false, pFootnotes, pObjectsOnPage), retrieveConjunctionString((int)topicDosage.joinType), topicDosageCellLineTag);

                                                    costHTML.Append(buildCellHTML(pContext, topicDosage, BDDosage.PROPERTYNAME_COST, topicDosage.cost, false, pFootnotes, pObjectsOnPage));
                                                    if (topicDosage.cost2.Length > 0)
                                                        costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, topicDosage, BDDosage.PROPERTYNAME_COST2, topicDosage.cost2, false, pFootnotes, pObjectsOnPage), topicDosageCellLineTag);
                                                    else
                                                        costHTML.Append(topicDosageCellLineTag);
                                                }
                                                break;
                                            case BDConstants.BDNodeType.BDDosage:
                                                BDDosage topicChildDosage = topicChild as BDDosage;

                                                if (topicChildDosage.joinType == (int)BDConstants.BDJoinType.Next)
                                                    dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, topicChildDosage, BDDosage.PROPERTYNAME_DOSAGE, topicChildDosage.dosage, false, pFootnotes, pObjectsOnPage), "<br>");
                                                else
                                                    dosageHTML.AppendFormat("{0} {1}{2}", buildCellHTML(pContext, topicChildDosage, BDDosage.PROPERTYNAME_DOSAGE, topicChildDosage.dosage, false, pFootnotes, pObjectsOnPage), retrieveConjunctionString((int)topicChildDosage.joinType), "<br>");

                                                costHTML.Append(buildCellHTML(pContext, topicChildDosage, BDDosage.PROPERTYNAME_COST, topicChildDosage.cost, false, pFootnotes, pObjectsOnPage));
                                                if (topicChildDosage.cost2.Length > 0)
                                                    costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, topicChildDosage, BDDosage.PROPERTYNAME_COST2, topicChildDosage.cost2, false, pFootnotes, pObjectsOnPage), "<br>");
                                                else
                                                    costHTML.Append("<br>");
                                                break;
                                        }
                                    }
                                    break;
                                case BDConstants.BDNodeType.BDDosageGroup:
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));

                                    dosageHTML.AppendFormat("<b>{0}</b><br>", buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false, pFootnotes, pObjectsOnPage));
                                    costHTML.Append("<br>");

                                    List<IBDNode> dosageGroupChildren = BDFabrik.GetChildrenForParent(pContext, child);
                                    string dosageGroupCellLineTag = (dosageGroupChildren.Count > 0) ? "<br>" : "";
                                    foreach (IBDNode dosageGroupChild in dosageGroupChildren)
                                    {
                                        // BDDosage
                                        BDDosage dosageChild = dosageGroupChild as BDDosage;
                                        if (dosageChild.joinType == (int)BDConstants.BDJoinType.Next)
                                            dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, dosageChild, BDDosage.PROPERTYNAME_DOSAGE, dosageChild.dosage, false, pFootnotes, pObjectsOnPage), dosageGroupCellLineTag);
                                        else
                                            dosageHTML.AppendFormat("{0} {1}{2}", buildCellHTML(pContext, dosageChild, BDDosage.PROPERTYNAME_DOSAGE, dosageChild.dosage, false, pFootnotes, pObjectsOnPage), retrieveConjunctionString((int)dosageChild.joinType), dosageGroupCellLineTag);
                                        costHTML.Append(buildCellHTML(pContext, dosageChild, BDDosage.PROPERTYNAME_COST, dosageChild.cost, false, pFootnotes, pObjectsOnPage));
                                        if (dosageChild.cost2.Length > 0)
                                            costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, dosageChild, BDDosage.PROPERTYNAME_COST2, dosageChild.cost2, false, pFootnotes, pObjectsOnPage), dosageGroupCellLineTag);
                                        else
                                            costHTML.Append(dosageGroupCellLineTag);
                                    }
                                    break;
                                case BDConstants.BDNodeType.BDDosage:
                                    string cellLineTag = (children.Count > 0) ? "<br>" : "";

                                    BDDosage dosage = child as BDDosage;
                                    if (dosage.joinType == (int)BDConstants.BDJoinType.Next)
                                        dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, pFootnotes, pObjectsOnPage), cellLineTag);
                                    else
                                        dosageHTML.AppendFormat("{0} {1}{2}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, pFootnotes, pObjectsOnPage), retrieveConjunctionString((int)dosage.joinType), cellLineTag);
                                    costHTML.Append(buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_COST, dosage.cost, false, pFootnotes, pObjectsOnPage));
                                    if (dosage.cost2.Length > 0)
                                        costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_COST2, dosage.cost2, false, pFootnotes, pObjectsOnPage), cellLineTag);
                                    else
                                        costHTML.Append(cellLineTag);
                                    break;
                            }
                        }
                        dosageHTML.Append("</td>");
                        costHTML.Append("</td>");
                        html.AppendFormat(@"{0}{1}</tr>", dosageHTML, costHTML);


                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosageGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDosage, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        string antimicrobialHtml = buildNodePropertyHTML(pContext, pNode, pNode.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                        html.AppendFormat(@"<tr class=""v{0}""><td>{1}</td>", (int)pNode.LayoutVariant, antimicrobialHtml);

                        string dosageGroupName = string.Empty;
                        string rowStartTag = string.Format(@"<tr class=""v{0}""><td />", (int)pNode.LayoutVariant);
                        const string ROWENDTAG = "</tr>";

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDDosage:
                                    if (!isFirstChild) html.Append(rowStartTag);
                                    html.Append(buildDosageHTML(pContext, child, dosageGroupName, pFootnotes, pObjectsOnPage));
                                    html.Append(ROWENDTAG);
                                    isFirstChild = false;
                                    break;
                                case BDConstants.BDNodeType.BDDosageGroup:
                                    if (!isFirstChild) html.Append(rowStartTag);
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
                            html.Append(ROWENDTAG);
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
                        pregnancyColumnsHtml.Add( buildHtmlForMetadataColumn(pContext, pNode, pregnancyColumns[2], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, pFootnotes, pObjectsOnPage));
                        foreach (IBDNode child in children)
                        {
                            BDAntimicrobialRisk amRisk = child as BDAntimicrobialRisk;
                            html.Append(BuildBDAntimicrobialRiskHtml(pContext, child, pFootnotes, pObjectsOnPage, pregnancyColumnsHtml, pLevel + 1));
                            string commentText = buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, pNode, BDConstants.LinkedNoteType.UnmarkedComment, pObjectsOnPage);
                            if (commentText.Length > 0)
                                html.AppendFormat("<p><b>Comments</b><br>{0}</p>", commentText);
                        }
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        html.Append(buildNodePropertyHTML(pContext, pNode, pNode.ParentId.Value, pNode.Name, BDNode.PROPERTYNAME_NAME, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
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
                                html.AppendFormat("<p><b>Comments</b><br>{0}</p>", commentText);
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
                        foreach (IBDNode child in children)
                        {
                            if (!string.IsNullOrEmpty(child.Name))
                            {
                                // create a page and add to collection
                                string amHtml = BuildBDAntimicrobialHtml(pContext, child, childFootnotes, childObjects, pLevel);
                                currentPageMasterObject = child;
                                childPages.Add(writeBDHtmlPage(pContext, child, amHtml, BDConstants.BDHtmlPageType.Navigation, childFootnotes, childObjects, null));
                            }
                        }
                        for (int i = 0; i < childPages.Count; i++)
                        {
                            html.AppendFormat(anchorTag, childPages[i].Uuid.ToString().ToUpper(), childPages[i].pageTitle);
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
                    html.AppendFormat("<b>{0}</b>: {1}", columnHtml[0], buildNodePropertyHTML(pContext, risk, risk.riskFactor, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, pFootnotes, pObjectsOnPage));
                    html.AppendFormat("<p><b>{0}</b>: {1}</p>", columnHtml[1], buildNodePropertyHTML(pContext, risk, risk.relativeInfantDose, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, pFootnotes, pObjectsOnPage));
                    break;
                case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                    BDAntimicrobialRisk lRisk = pNode as BDAntimicrobialRisk;
                    html.AppendFormat("<b>{0}</b>: {1}", columnHtml[0], buildNodePropertyHTML(pContext, lRisk, lRisk.riskFactor, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK, pFootnotes, pObjectsOnPage));
                    html.AppendFormat("<p><b>{0}</b>: {1}</p>", columnHtml[1], buildNodePropertyHTML(pContext, lRisk, lRisk.aapRating, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, pFootnotes, pObjectsOnPage));
                    html.AppendFormat("<p><b>{0}</b>: {1}</p>", columnHtml[2], buildNodePropertyHTML(pContext, lRisk, lRisk.relativeInfantDose, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, pFootnotes, pObjectsOnPage));
                    break;
                default:
                    break;
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
                    case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity_ContentRow }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass_HeaderRow }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                    case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                            html.Append("<table><tr><th>Condition</th><th>Other Potential Pathogens</th></tr>");
                            foreach (IBDNode child in children)
                            {
                                html.AppendFormat(@"<tr><td>{0}</td><td>", child.Name);
                                pObjectsOnPage.Add(child.Uuid);
                                List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, child);
                                foreach (IBDNode node in pathogens) // Assuming that these are pathogens...
                                {
                                    html.AppendFormat("{0}<br>", node.Name);
                                    pObjectsOnPage.Add(node.Uuid);
                                }
                            }
                            html.Append("</td></tr></table>");
                        }
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_PreOp:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_PreOp_HeaderRow }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;

                    // Common generic render
                    case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                    case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_Stepdown_HeaderRow }));
                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));

                    case BDConstants.LayoutVariantType.Table_2_Column:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_2_Column_HeaderRow }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));

                    case BDConstants.LayoutVariantType.Table_3_Column:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_3_Column_HeaderRow }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));

                    case BDConstants.LayoutVariantType.Table_4_Column:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_4_Column_HeaderRow }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));

                    case BDConstants.LayoutVariantType.Table_5_Column:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Table_5_Column_HeaderRow }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        int columnCount = BDFabrik.GetTableColumnCount(pNode.LayoutVariant);
                        if (children.Count > 0)
                        {
                            html.Append(@"<table>");
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
                            html.AppendFormat("<table><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr>", c2Html, c3Html, c4Html);
                            html.AppendFormat("<tr><td>{0}{1}</td><td>{2}{3}</td><td>{4}{5}</td></tr>",
                                retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field01_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD01, pObjectsOnPage, true, pFootnotes),
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD01, BDConstants.LinkedNoteType.Footnote), true, pFootnotes, pObjectsOnPage),
                                retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field02_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD02, pObjectsOnPage, true, pFootnotes),
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD02, BDConstants.LinkedNoteType.Footnote), true, pFootnotes, pObjectsOnPage),
                                retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field03_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD03, pObjectsOnPage, true, pFootnotes),
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD03, BDConstants.LinkedNoteType.Footnote), true, pFootnotes, pObjectsOnPage));

                            html.Append("</table>");
                            pObjectsOnPage.Add(entry.Uuid);
                        }
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk:
                        //foreach (IBDNode child in children)
                        //{
                        //    html.Append(BuildBDConfiguredEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, true));
                        //}
                        //break;

                    case BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading:
                    case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages:
                    case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        if (null != metadataLayoutColumns)
                        {
                            html.Append("<table><tr>");
                            foreach (BDLayoutMetadataColumn metadataColumn in metadataLayoutColumns)
                                html.AppendFormat("<th>{0}</th>", buildHtmlForMetadataColumn(pContext, pNode, metadataColumn, BDConstants.BDNodeType.BDConfiguredEntry, pFootnotes, pObjectsOnPage));
                            html.Append("</tr>");

                            bool firstColumnEmphasized = (pNode.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk);
                            foreach (IBDNode child in children)
                            {
                                html.Append(BuildBDConfiguredEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, false, firstColumnEmphasized));
                            }
                            html.Append("</table>");
                        }
                        break;

                    case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                    case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDConfiguredEntry, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                        html.Append("</table>");
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza: //312
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCombinedEntry, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_NoRenal }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_Renal }));

                        foreach (IBDNode child in children)
                        {
                            switch(child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDCombinedEntry:
                                    html.Append(BuildBDCombinedEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1, true));
                                    html.Append("</table>");
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


                html.AppendFormat(@"<tr><td colspan={0}>{1}</td></tr>", pColumnCount, buildNodeWithReferenceAndOverviewHTML(pContext, pNode as BDNode, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));

                foreach (IBDNode child in children)
                {
                    switch (child.NodeType)
                    {
                        case BDConstants.BDNodeType.BDTableRow:
                            BDTableRow tableRow = child as BDTableRow;
                            html.Append(buildTableRowHtml(pContext, tableRow, false, true, pFootnotes, pObjectsOnPage));
                            break;

                        case BDConstants.BDNodeType.BDTableSubsection:
                            html.AppendFormat(@"<tr><td colspan={0}>{1}</td></tr>", pColumnCount, buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, "u", pFootnotes, pObjectsOnPage));
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
            return BuildBDConfiguredEntryHtml(pContext, pNode, pFootnotes, pObjectsOnPage,pLevel, false, false);
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
                            // create the header with the note contents
                            string header = retrieveNoteTextForConfiguredEntryField(pContext, pNode.Uuid, BDConfiguredEntry.FieldNotePropertyNameForIndex(0) , pObjectsOnPage, pFootnotes);
                            if (header.StartsWith("<p>")) header = header.Substring(3);
                            if (header.EndsWith("</p>")) header = header.Substring(0, header.Length - 4);

                            html.AppendFormat("<{0}>{1}</{0}>",HtmlHeaderTagLevelString(pLevel) , header);
                            //Create a table with each field configured title and field note as rows (2)
                            
                            for (int idx = 1; idx < metadataLayoutColumns.Count; idx++)
                            {
                                string propertyName = metadataLayoutColumns[idx].FieldNameForColumnOfNodeType(pContext, BDConstants.BDNodeType.BDConfiguredEntry);
                                string title = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[idx], BDConstants.BDNodeType.BDConfiguredEntry, propertyName, pFootnotes, pObjectsOnPage);
                                string content = retrieveNoteTextForConfiguredEntryField(pContext, pNode.Uuid, BDConfiguredEntry.FieldNotePropertyNameForIndex(idx), pObjectsOnPage, pFootnotes);
                                html.AppendFormat("<{0}>{1}</{0}>",HtmlHeaderTagLevelString(pLevel + 2), title );
                                html.Append("<table>");
                                html.AppendFormat("<tr><td>{0}<td></tr>", content);
                                html.Append("</table>");
                            }                        
                        }
                    }
                    else
                    {
                         html.Append("<tr>");

                        if (null != metadataLayoutColumns)
                        {
                            for (int idx = 0; idx < metadataLayoutColumns.Count; idx++)
                            {
                                string propertyName = metadataLayoutColumns[idx].FieldNameForColumnOfNodeType(pContext, BDConstants.BDNodeType.BDConfiguredEntry);
                                string propertyValue = configuredEntry.PropertyValueForName(propertyName);

                                string propertyHtml = buildNodePropertyHTML(pContext, configuredEntry, propertyValue, propertyName, pFootnotes, pObjectsOnPage);
                                if(string.IsNullOrEmpty(propertyHtml))
                                    propertyHtml = buildNodePropertyHTML(pContext, configuredEntry, propertyValue, string.Format("{0}{1}", propertyName, BDConfiguredEntry.FIELDNOTE_SUFFIX), pFootnotes, pObjectsOnPage);

                                if(pFirstColumnEmphasized && (idx == 0))
                                    html.AppendFormat("<td><b>{0}</b></td>", propertyHtml);
                                else
                                    html.AppendFormat("<td>{0}</td>", propertyHtml);
                            }
                        }

                        html.Append("</tr>");                       
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

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDCombinedEntry))
            {
                BDCombinedEntry combinedEntry = pNode as BDCombinedEntry;
                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, combinedEntry.LayoutVariant);

                if (pRenderTableHeader)
                {
                    //html.AppendFormat("<h4>{0}</h4>", combinedEntry.Name);
                    //html.AppendFormat("<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), combinedEntry.Name);

                    //NOTE: This expects that a matching number of columns have been defined
                    html.Append("<table><tr><th></th>"); // insert a blank column header: Combined Entries layout columns, by definition, are offset by 1
                    for (int i = 0; i < metadataLayoutColumns.Count; i++)
                        html.AppendFormat("<th>{0}</th>", metadataLayoutColumns[i]);
                    html.Append("</tr>");
                }

                switch (pNode.LayoutVariant)
                {
                    // created from a table (Prophylaxis_Communicable_Influenza)
                    //case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:

                    //    break;

                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_NoRenal: //3121
                        prefixHtml.AppendFormat("<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel), combinedEntry.Name);
                        html.AppendFormat("<tr><td>{0}</td><td colspan=3>{1}</td></tr>",
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, pFootnotes, pObjectsOnPage));
                        html.AppendFormat("<tr><td>{0}</td><td colspan=3>{1}</td></tr>",
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, pFootnotes, pObjectsOnPage));
                        html.AppendFormat("<tr><td>{0}</td><td colspan=3>{1}</td></tr>",
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, pFootnotes, pObjectsOnPage));
                        break;

                    // created from a topic (Prophylaxis_Communicable_Influenza_Oseltamivir) 3123
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Creatinine: //3124
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Oseltamivir_Weight: //3125
                        html.AppendFormat("<tr><td rowspan=4>{0}</td><td>{1}</td><td>{2}</td></tr>",
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.Name, BDCombinedEntry.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, pFootnotes, pObjectsOnPage),
                            buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, pFootnotes, pObjectsOnPage));

                        if (combinedEntry.entryDetail02 != null && combinedEntry.entryTitle02 != null)
                        {
                            html.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, pFootnotes, pObjectsOnPage),
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, pFootnotes, pObjectsOnPage));
                        }
                        if (combinedEntry.entryDetail03 != null && combinedEntry.entryTitle03 != null)
                        {
                            html.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, pFootnotes, pObjectsOnPage),
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, pFootnotes, pObjectsOnPage));
                        }
                        if (combinedEntry.entryTitle04 != null && combinedEntry.entryDetail04 != null)
                        {
                            html.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle04, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04, pFootnotes, pObjectsOnPage),
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail04, BDCombinedEntry.PROPERTYNAME_ENTRY04, pFootnotes, pObjectsOnPage));
                        }
                        break;

                    // created from a table of the same layoutVariant
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive: //311
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae: //315
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis: //316
                    // created from a topic (Prophylaxis_Communicable_Influenza_Zanamivir) 3126
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Zanamivir: //3126
                    // everything else
                    default:
                        StringBuilder cell0HTML = new StringBuilder();
                        StringBuilder cell1HTML = new StringBuilder();
                        StringBuilder cell2HTML = new StringBuilder();   

                        // first column
                        if (!string.IsNullOrEmpty(combinedEntry.groupTitle))
                            cell0HTML.AppendFormat("<u>{0}</u>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.groupTitle, BDCombinedEntry.PROPERTYNAME_GROUPTITLE, pFootnotes, pObjectsOnPage));
                        cell0HTML.Append("<br>");

                        if (!string.IsNullOrEmpty(combinedEntry.Name))
                        {
                            cell0HTML.AppendFormat("<b>{0}</b><br>",
                                                   buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.Name, BDCombinedEntry.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.GroupJoinType) != string.Empty)
                                cell0HTML.AppendFormat("{0}<br>", retrieveConjunctionString(combinedEntry.GroupJoinType));
                        }

                        // second column
                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle01))
                            cell1HTML.AppendFormat("<u>{0}</u>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle01, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, pFootnotes, pObjectsOnPage));
                        cell1HTML.Append("<br>");

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail01))
                        {
                            cell1HTML.AppendFormat("{0}<br>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail01, BDCombinedEntry.PROPERTYNAME_ENTRY01, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType01) != string.Empty)
                                cell1HTML.AppendFormat("<b>{0}</b><br>", retrieveConjunctionString(combinedEntry.JoinType01));
                        }

                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle02))
                            cell1HTML.AppendFormat("<u>{0}</u><br>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle02, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, pFootnotes, pObjectsOnPage));

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail02))
                        {
                            cell1HTML.AppendFormat("{0}<br>", buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail02, BDCombinedEntry.PROPERTYNAME_ENTRY02, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType02) != string.Empty)
                            {
                                cell1HTML.AppendFormat("<b>{0}</b>", retrieveConjunctionString(combinedEntry.JoinType02));
                            }
                        }
                        
                        //third column
                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle03))
                            cell2HTML.AppendFormat("<u>{0}</u>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle03, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, pFootnotes, pObjectsOnPage));
                        cell2HTML.Append("<br>");

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail03))
                        {
                            cell2HTML.AppendFormat("{0}<br>", buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail03, BDCombinedEntry.PROPERTYNAME_ENTRY03, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType03) != string.Empty)
                                cell2HTML.AppendFormat("<b>{0}</b><br>", retrieveConjunctionString(combinedEntry.JoinType03));
                        }

                        if (!string.IsNullOrEmpty(combinedEntry.entryTitle04))
                            cell2HTML.AppendFormat("<u>{0}</u><br>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryTitle04, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04, pFootnotes, pObjectsOnPage));

                        if (!string.IsNullOrEmpty(combinedEntry.entryDetail04))
                        {
                            cell2HTML.AppendFormat("{0}<br>",
                                buildNodePropertyHTML(pContext, combinedEntry, combinedEntry.entryDetail04, BDCombinedEntry.PROPERTYNAME_ENTRY04, pFootnotes, pObjectsOnPage));
                            if (retrieveConjunctionString(combinedEntry.JoinType04) != string.Empty)
                                cell2HTML.AppendFormat("<b>{0}</b><br>", retrieveConjunctionString(combinedEntry.JoinType04));
                        }
                        
                        html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", cell0HTML, cell1HTML, cell2HTML);
                        break;
                }
            }

            html.Insert(0, prefixHtml);
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

        public string BuildBDSurgeryHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            StringBuilder suffixHtml = new StringBuilder();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSurgery))
            {
                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

                List<IBDNode> surgeryClassificationList = BDFabrik.GetChildrenForParent(pContext, pNode);
                // childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgeryClassification, new BDConstants.LayoutVariantType[] { layoutVariant }));
    
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                        foreach (IBDNode surgeryClassification in surgeryClassificationList) // SurgeryClassification
                        {
                            if (surgeryClassification.Name.Length > 0 && !surgeryClassification.Name.Contains(string.Format("New {0}", BDUtilities.GetEnumDescription(surgeryClassification.NodeType))))
                                html.AppendFormat("<ul><li>{0}</ul>", surgeryClassification.Name);
                            pObjectsOnPage.Add(surgeryClassification.Uuid);

                            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                            string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);

                            //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));

                            List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, surgeryClassification);
                            if (therapyGroups.Count > 0)
                            {
                                StringBuilder adultDosageHTML = new StringBuilder();
                                StringBuilder pedsDosageHTML = new StringBuilder();
                                html.Append(@"<table>");
                                html.AppendFormat(@"<tr><th>{0}</th><th>{1}</th></tr>", c2Html, c3Html);
                                foreach (IBDNode therapyGroup in therapyGroups)
                                {
                                    if (therapyGroup.Name.Length > 0 && !therapyGroup.Name.Contains("New Therapy Group"))
                                    {
                                        adultDosageHTML.AppendFormat("<u>{0}</u><ul>", therapyGroup.Name);
                                        pedsDosageHTML.AppendFormat("<u>{0}</u><ul>", therapyGroup.Name);
                                    }
                                    else
                                    {
                                        adultDosageHTML.Append("<ul>");
                                        pedsDosageHTML.Append("<ul>");
                                    }
                                    pObjectsOnPage.Add(therapyGroup.Uuid);
                                    List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, therapyGroup);
                                    #region process therapies
                                    resetGlobalVariablesForTherapies();
                                    foreach (IBDNode t in therapies)
                                    {
                                        BDTherapy therapy = t as BDTherapy;
                                        // therapy name - add to both cells
                                        if (therapy.nameSameAsPrevious.Value == true && previousTherapyId != Guid.Empty)
                                        {
                                            adultDosageHTML.AppendFormat("<li>{0} ", buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage));
                                            pedsDosageHTML.AppendFormat("<li>{0} ", buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage));
                                        }
                                        else
                                        {
                                            adultDosageHTML.AppendFormat("<li>{0} ", buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage));
                                            pedsDosageHTML.AppendFormat("<li>{0} ", buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage));
                                        }
                                        // Dosage - adult dose
                                        if (therapy.dosageSameAsPrevious.Value == true)
                                            adultDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage));
                                        else
                                            adultDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage));

                                        // Dosage 1 - Paediatric dose
                                        if (therapy.dosage1SameAsPrevious.Value == true)
                                            pedsDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyDosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage));
                                        else
                                            pedsDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage));

                                        // check for conjunctions and add a row for any that are found
                                        switch (therapy.therapyJoinType)
                                        {
                                            case (int)BDConstants.BDJoinType.OrWithNext:
                                                adultDosageHTML.AppendFormat(@"<br>{0}<br> ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                                pedsDosageHTML.AppendFormat(@"<br>{0}<br>", retrieveConjunctionString((int)therapy.therapyJoinType));
                                                break;
                                            default:
                                                adultDosageHTML.AppendFormat(@" {0} ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                                pedsDosageHTML.AppendFormat(@" {0} ", retrieveConjunctionString((int)therapy.therapyJoinType));
                                                break;
                                        }
                                        previousTherapyId = therapy.Uuid;
                                        previousTherapyName = therapy.Name;
                                        previousTherapyDosage = therapy.dosage;
                                        previousTherapyDosage1 = therapy.dosage1;
                                    }
                                    #endregion
                                    adultDosageHTML.Append("</ul>");
                                    pedsDosageHTML.Append("</ul>");
                                }
                                html.AppendFormat("<tr><td>{0}</td><td>{1}</td</tr>", adultDosageHTML, pedsDosageHTML);
                                html.Append(@"</table>");
                            }
                        }
                        break;
                }
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

                    case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDSubcategoryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;

                    case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        if (children.Count > 0) // subcategory - column 1 values
                        {
                            List<string> columnHtml601 = new List<string>();
                            columnHtml601.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDSubcategory, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                            columnHtml601.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDMicroorganism, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));

                            html.Append("<table><tr>");
                            for (int i = 0; i < columnHtml601.Count; i++)
                                html.AppendFormat("<th>{0}</th>", columnHtml601[i]);
                            html.Append("</tr>");

                            foreach (IBDNode child in children)
                            {
                                //BDNode subcategory = child as BDNode;
                                List<IBDNode> mos = BDFabrik.GetChildrenForParent(pContext, child);
                                StringBuilder mString = new StringBuilder();
                                foreach (IBDNode microorganism in mos)
                                {
                                    mString.AppendFormat("{0}<br>", buildNodePropertyHTML(pContext, microorganism, microorganism.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                                }
                                string scString = buildNodePropertyHTML(pContext, child, child.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                                html.AppendFormat("<tr><td><ul><li>{0}</ul></td><td>{1}</td></tr>", scString, mString);
                                pObjectsOnPage.Add(child.Uuid);
                            }
                            html.Append("</table>");
                        }
                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:

                        html.Append("<table><tr><th>Excellent Penetration</th><th>Good Penetration</th><th>Poor Penetration</th></tr>");
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
                                        string antimicrobialHTML = buildNodePropertyHTML(pContext, antimicrobial, antimicrobial.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);
                                        colHTML.AppendFormat(@"{0}<br>", antimicrobialHTML);
                                    }
                                    html.Append(colHTML);
                                }
                            }
                            html.Append(@"</tr>");
                        }
                        html.Append(@"</table>");

                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                    case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                    
                    
                    case BDConstants.LayoutVariantType.Prophylaxis_IERecommendation:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubcategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDSubcategoryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgeryClassification, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Microbiology_EmpiricTherapy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobialGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            if (child.NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
                                html.AppendFormat("{0}<br>", buildNodeWithReferenceAndOverviewHTML(pContext, child, string.Empty, pFootnotes, pObjectsOnPage));
                        }
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

                            html.AppendFormat(@"<table class=""v{0}""><tr><th rowspan=4>{1}</th><th rowspan=4>{2}</th>", (int)pNode.LayoutVariant, c1Html, c2Html);
                            html.Append(@"<th colspan=3><b>Dose and Interval Adjustment for Renal Impairment</b></th></tr>");
                            html.AppendFormat(@"<tr><th colspan=""3""><b>{0}</b></th><tr>", c3Html);
                            html.Append(@"<tr><th>&gt50</th><th>10 - 50</th><th>&lt10(Anuric)</th></tr>");

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
                            html.Append(@"<table><tr><th>Antimicrobial</th><th>Dosage Adjustment</th></tr>");
                            foreach (IBDNode child in children)
                            {
                                //child is antimicrobial with overview:  add a row
                                BDNode node = child as BDNode;

                                // this is more convenient than calling buildBDAntimicrobial
                                html.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>", child.Name, retrieveNoteTextForOverview(pContext, child.Uuid, pObjectsOnPage));
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
                        foreach (IBDNode child in children)
                        {
                            if (!string.IsNullOrEmpty(child.Name))
                            {
                                // need to create a page and add to collection for parent
                                string childHtml = BuildBDSubcategoryHtml(pContext, child, childFootnotes, childObjectsOnPage, pLevel);
                                currentPageMasterObject = child;
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
                                        // create a page and add to collection
                                        string gcHtml = BuildBDAntimicrobialHtml(pContext, gChild, gcFootnotes, gcObjects, pLevel);
                                        currentPageMasterObject = gChild;
                                        childPages.Add(writeBDHtmlPage(pContext, gChild, gcHtml, BDConstants.BDHtmlPageType.Navigation, gcFootnotes, gcObjects, null));
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < childPages.Count; i++)
                        {
                            html.AppendFormat(anchorTag, childPages[i].Uuid.ToString().ToUpper(), childPages[i].pageTitle);
                        }
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
                                // need to create a page and add to collection for parent
                                string childHtml = BuildBDSubcategoryHtml(pContext, child, l_childFootnotes, l_childObjectsOnPage, pLevel);
                                currentPageMasterObject = child;
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
                                        // create a page and add to collection
                                        string gcHtml = BuildBDAntimicrobialGroupHtmlAndPage(pContext, gChild, gcFootnotes, gcObjects, pLevel);
                                        currentPageMasterObject = gChild;
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
                                                // create a page and add to collection
                                                string ggHtml = BuildBDAntimicrobialHtml(pContext, ggChild, ggFootnotes, ggObjects, pLevel);
                                                currentPageMasterObject = ggChild;
                                                l_childPages.Add(writeBDHtmlPage(pContext, ggChild, ggHtml, BDConstants.BDHtmlPageType.Navigation, ggFootnotes, ggObjects, null));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < l_childPages.Count; i++)
                        {
                            html.AppendFormat(anchorTag, l_childPages[i].Uuid.ToString().ToUpper(), l_childPages[i].pageTitle);
                        }
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        // metadataLayoutColumns defined at root

                        // handles therapyGroups directly

                        List<string> columnHtml = new List<string>();
                        columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage));
                        columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage));

                        html.Append("<table><tr>");
                        for (int i = 0; i < metadataLayoutColumns.Count; i++)
                            html.AppendFormat("<th>{0}</th>", columnHtml[i]);
                        html.Append("</tr>");

                        List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
                        foreach(IBDNode tGroup in childNodes)
                        {
                            if (tGroup.Name.Length > 0 && !tGroup.Name.Contains(BDUtilities.GetEnumDescription(tGroup.NodeType)))
                                html.AppendFormat("<tr><td><b>{0}</b></td><td /></tr>", buildNodePropertyHTML(pContext, tGroup, tGroup.Name, BDTherapyGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));

                            List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, tGroup);
                            if (therapies.Count > 0)
                            {
                                foreach (BDTherapy therapy in therapies)
                                {
                                    html.Append(buildTherapyWithCombinedColumnHtml(pContext, therapy, pFootnotes, pObjectsOnPage));

                                    if (!string.IsNullOrEmpty(therapy.Name) && therapy.nameSameAsPrevious == false)
                                        previousTherapyName = therapy.Name;
                                    if (!string.IsNullOrEmpty(therapy.dosage))
                                    {
                                        if (therapy.dosageSameAsPrevious == false)
                                            previousTherapyDosage = therapy.dosage;
                                    }
                                }
                            }
                        }
                        html.Append("</table>");
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    default:
                        break;
                }
            }


            html.Append(suffixHtml);

            return html.ToString();
        }

        public string BuildBDCategoryPLAntimicrobialsInLactationHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotesOnPage, objectsOnPage));

            List<string> columnHtml = new List<string>();
            columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK, footnotesOnPage, objectsOnPage));
            columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, footnotesOnPage, objectsOnPage));
            columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, footnotesOnPage, objectsOnPage));

            List<BDHtmlPage> subcatPages = new List<BDHtmlPage>();
            List<Guid> subcatObjectsOnPage = new List<Guid>();
            List<BDLinkedNote> subcatFootnotes = new List<BDLinkedNote>();
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode subcategory in childNodes)
            {
                StringBuilder subcatHTML = new StringBuilder();

                subcatHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, "h2", subcatFootnotes, subcatObjectsOnPage));
                List<IBDNode> subcategoryChildNodes = BDFabrik.GetChildrenForParent(pContext, subcategory);
                List<BDHtmlPage> apPages = new List<BDHtmlPage>();
                List<BDLinkedNote> apFootnotes = new List<BDLinkedNote>();
                List<Guid> apObjectsOnPage = new List<Guid>();
                StringBuilder apHTML = new StringBuilder();
                //
                //the next layer can either be antimicrobial group or antimicrobial
                //
                if (subcategoryChildNodes[0].NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
                {
                    // then we need to loop over all the antimicrobial groups here & add the name to the page with the associated antimicrobials.
                    foreach (IBDNode antimicrobialGroup in subcategoryChildNodes)
                    {
                        apHTML.Clear();
                        apHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, antimicrobialGroup, "h4", apFootnotes, apObjectsOnPage));
                        List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(pContext, antimicrobialGroup);
                        List<BDHtmlPage> amPages = new List<BDHtmlPage>();
                        amPages.AddRange(buildAntimicrobialWithRiskHTML(pContext, columnHtml, antimicrobials));
                        for (int i = 0; i < amPages.Count; i++)
                            apHTML.AppendFormat(anchorTag, amPages[i].Uuid.ToString().ToUpper(), antimicrobials[i].Name);
                            subcatHTML.Append(apHTML);
                    }
                    for (int i = 0; i < apPages.Count; i++)
                        subcatHTML.AppendFormat(anchorTag, apPages[i].Uuid.ToString().ToUpper(), subcategoryChildNodes[i].Name);
                    if (subcategory.Name.Length > 0)
                    {
                        currentPageMasterObject = subcategory;
                        subcatPages.Add(writeBDHtmlPage(pContext, subcategory, subcatHTML, BDConstants.BDHtmlPageType.Navigation, subcatFootnotes, subcatObjectsOnPage, null));
                    }
                    else
                        bodyHTML.Append(subcatHTML);
                }
                else
                { // antimicrobial group does not exist - the child nodes are antimicrobials
                    List<BDHtmlPage> amPages = new List<BDHtmlPage>();
                    amPages.AddRange(buildAntimicrobialWithRiskHTML(pContext, columnHtml, subcategoryChildNodes));
                    for (int i = 0; i < amPages.Count; i++)
                        apHTML.AppendFormat(anchorTag, amPages[i].Uuid.ToString().ToUpper(), subcategoryChildNodes[i].Name);
                    if (subcategory.Name.Length > 0)
                    {
                        currentPageMasterObject = subcategory;
                        apPages.Add(writeBDHtmlPage(pContext, subcategory, apHTML, BDConstants.BDHtmlPageType.Navigation, apFootnotes, apObjectsOnPage, null));
                    }
                    else
                        subcatHTML.Append(apHTML);
                }
            }
            for (int i = 0; i < subcatPages.Count; i++)
                bodyHTML.AppendFormat(anchorTag, subcatPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
    
            return bodyHTML.ToString();
        }

        public string BuildBDCategoryPLAntimicrobialsInPregnancyHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // Category > Subcategory > antimicrobial > antimicrobialRisk
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotesOnPage = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            List<BDHtmlPage> subcatPages = new List<BDHtmlPage>();
            List<Guid> subcatObjectsOnPage = new List<Guid>();
            List<BDLinkedNote> subcatFootnotes = new List<BDLinkedNote>();
            foreach (IBDNode subcategory in childNodes)
            {
                StringBuilder subcatHTML = new StringBuilder();
                subcatHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, "h2", subcatFootnotes, subcatObjectsOnPage));
                List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(pContext, subcategory);
                List<BDHtmlPage> amPages = new List<BDHtmlPage>();
                List<BDLinkedNote> amFootnotes = new List<BDLinkedNote>();

                string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, footnotesOnPage, objectsOnPage);
                string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, footnotesOnPage, objectsOnPage);

                foreach (IBDNode antimicrobial in antimicrobials)
                {
                    // write an HTML page for the antimicrobial, build a link for the name
                    StringBuilder antimicrobialHTMLBody = new StringBuilder();
                    List<Guid> antimicrobialsOnPage = new List<Guid>();
                    antimicrobialHTMLBody.Append(buildNodeWithReferenceAndOverviewHTML(pContext, antimicrobial as BDNode, "h4", amFootnotes, antimicrobialsOnPage));
                    
                    List<IBDNode> amRisks = BDFabrik.GetChildrenForParent(pContext, antimicrobial);
                    foreach (IBDNode amRisk in amRisks)
                    {
                        BDAntimicrobialRisk risk = amRisk as BDAntimicrobialRisk;
                        if (risk.riskFactor.Length > 0)
                            antimicrobialHTMLBody.AppendFormat("<b>{0}</b>: {1}", c1Html, buildNodePropertyHTML(pContext, risk, risk.riskFactor, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, footnotesOnPage, objectsOnPage));
                        if (risk.recommendations.Length > 0)
                            antimicrobialHTMLBody.AppendFormat("<p><b>{0}</b><br>{1}</p>", c2Html, buildNodePropertyHTML(pContext, risk, risk.recommendations, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, footnotesOnPage, objectsOnPage));
                        antimicrobialsOnPage.Add(amRisk.Uuid);
                        amFootnotes.AddRange(footnotesOnPage);

                        amFootnotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, amRisk.Uuid, BDAntimicrobialRisk.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));
                        amFootnotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, amRisk.Uuid, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, BDConstants.LinkedNoteType.Footnote));
                    }
                    string commentText = buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, antimicrobial, BDConstants.LinkedNoteType.UnmarkedComment, antimicrobialsOnPage);
                    if (commentText.Length > 0)
                        antimicrobialHTMLBody.AppendFormat("<p><b>Comments</b><br>{0}</p>", commentText);
                    currentPageMasterObject = antimicrobial;
                    amPages.Add(writeBDHtmlPage(pContext, pNode, antimicrobialHTMLBody, BDConstants.BDHtmlPageType.Data, amFootnotes, antimicrobialsOnPage, null));
                }
                for (int i = 0; i < amPages.Count; i++)
                    subcatHTML.AppendFormat(anchorTag, amPages[i].Uuid.ToString().ToUpper(), antimicrobials[i].Name);
                currentPageMasterObject = subcategory;
                subcatPages.Add(writeBDHtmlPage(pContext, pNode, subcatHTML, BDConstants.BDHtmlPageType.Navigation, subcatFootnotes, subcatObjectsOnPage, null));

            }
            for(int i = 0; i < subcatPages.Count; i++)
                bodyHTML.AppendFormat(anchorTag, subcatPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
            return bodyHTML.ToString();
        }

        public string BuildBDSectionProphylaxisInfectionPreventionHtmlAndPages(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();

            //            childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage }));
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
                            mHTML.AppendFormat("<{0}>{1}</{0}>{2}", HtmlHeaderTagLevelString(pLevel + 3), columnHtml[0], p.infectiveMaterial);
                            mHTML.AppendFormat("<{0}>{1}</{0}>{2}", HtmlHeaderTagLevelString(pLevel + 3), columnHtml[1], p.modeOfTransmission);
                            // build table
                            mHTML.AppendFormat("<table><tr><th>{0}</th><th>Acute Care</th><th>Long Term Care</th></tr>", precautionTitle);
                            mHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", columnHtml[2], p.singleRoomAcute, p.singleRoomLongTerm);
                            mHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", columnHtml[3], p.glovesAcute, p.glovesLongTerm);
                            mHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", columnHtml[4], p.gownsAcute, p.gownsLongTerm);
                            mHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", columnHtml[5], p.maskAcute, p.maskLongTerm);
                            mHTML.Append("</table>");

                            List<BDLinkedNote> durationNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, p.Uuid, BDPrecaution.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
                            StringBuilder durationText = new StringBuilder();
                            foreach (BDLinkedNote note in durationNotes)
                            {
                                durationText.Append(note.documentText);
                                mObjectsOnPage.Add(note.Uuid);
                            }
                            mHTML.AppendFormat("<{0}{1}</{0}>{2}", HtmlHeaderTagLevelString(pLevel + 3), columnHtml[6], durationText);
                        }
                        currentPageMasterObject = microorganism;
                        mPages.Add(writeBDHtmlPage(pContext, microorganism, mHTML, BDConstants.BDHtmlPageType.Data, mFootnotes, mObjectsOnPage, null));
                    }
                    for (int i = 0; i < mPages.Count; i++)
                        mgHTML.AppendFormat(anchorTag, mPages[i].Uuid.ToString().ToUpper(), mPages[i].pageTitle);
                }
                html.Append(mgHTML);
                currentPageMasterObject = pNode;
            }

            return html.ToString();
        }

        public string BuildBDSectionProphylaxisEndocarditisHtmlAndPages(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            StringBuilder html = new StringBuilder();
            bool isFirstChildEntry = true;

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDSection))
            {
                //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IERecommendation }));
                //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage }));

                List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage);
                string c1Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDTherapyGroup, BDTherapyGroup.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage);

                StringBuilder therapyGroupHTML = new StringBuilder();
                List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
                List<BDHtmlPage> childPages = new List<BDHtmlPage>();
                foreach (IBDNode child in childNodes)
                {
                    switch (child.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Prophylaxis_IERecommendation:
                            List<BDLinkedNote> catFootnotes = new List<BDLinkedNote>();
                            StringBuilder categoryHTML = new StringBuilder();
                            List<Guid> categoriesOnPage = new List<Guid>();
                            List<IBDNode> subcategories = BDFabrik.GetChildrenForParent(pContext, child);
                            categoryHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel + 2), catFootnotes, categoriesOnPage));
                            foreach (IBDNode subcategory in subcategories)
                                categoryHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, HtmlHeaderTagLevelString(pLevel + 3), catFootnotes, categoriesOnPage));
                            currentPageMasterObject = child;
                            BDHtmlPage childPage = writeBDHtmlPage(pContext, child, categoryHTML, BDConstants.BDHtmlPageType.Data, catFootnotes, categoriesOnPage, null);
                            html.AppendFormat(anchorTag, childPage.Uuid.ToString().ToUpper(), child.Name);
                            break;

                        case BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage:
                        default:
                            // this is a TherapyGroup > therapy hierarchy
                            if (isFirstChildEntry)
                            {
                                therapyGroupHTML.AppendFormat("<{0}>{1}</{0}>", HtmlHeaderTagLevelString(pLevel + 1), c1Html);
                                isFirstChildEntry = false;
                            }
                            StringBuilder therapyHTML = new StringBuilder();

                            string c2Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, pFootnotes, pObjectsOnPage);
                            string c3Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, pFootnotes, pObjectsOnPage);
                            string c4Html = buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[3], BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, pFootnotes, pObjectsOnPage);

                            List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, child);
                            therapyGroupHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, HtmlHeaderTagLevelString(pLevel + 1), pFootnotes, pObjectsOnPage));
                            // the complexity of the column headers cannot be handled in the standard methods so the text is built manually
                            string subtext = "given 30-60 minutes before the procedure";
                            therapyHTML.AppendFormat("<table><tr><th>{0}</th><th><b>{1}</b> {2}<b>/ROUTE</b></th><th><b>{3}</b> {4}<b>/ROUTE</b></th></tr>",
                                c2Html, c3Html, subtext, c4Html, subtext);
                            therapyHTML.Append(@"<tr><th colspan=3>Dental, Oral, Respiratory Tract Procedures</td></tr>");
                            foreach (IBDNode t in therapies)
                            {
                                BDTherapy therapy = t as BDTherapy;
                                therapyHTML.AppendFormat(buildTherapyWithTwoDosagesHtml(pContext, therapy, false, pFootnotes, pObjectsOnPage));
                            }
                            therapyHTML.Append("</table>");
                            therapyGroupHTML.Append(therapyHTML);
                            break;
                    }
                }
                html.Append(therapyGroupHTML);
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
                    case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                        html.Append(buildNodePropertyHTML(pContext, pNode, pNode.Uuid, pNode.Name, BDNode.PROPERTYNAME_NAME, HtmlHeaderTagLevelString(pLevel + 2), pFootnotes, pObjectsOnPage));
                        foreach (IBDNode child in children)
                        {
                            html.AppendFormat("{0}<br>", buildNodePropertyHTML(pContext, child, child.Name, BDNode.PROPERTYNAME_NAME, pFootnotes, pObjectsOnPage));
                        }
                        break;

                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "b", pFootnotes, pObjectsOnPage));
                        html.Append("<ul>");

                        foreach (IBDNode child in children)
                        {
                            html.AppendFormat("<li>{0}</li>",buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage));
                        }
                        html.Append("</ul>");
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                    case BDConstants.LayoutVariantType.Microbiology_EmpiricTherapy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //break;
                    default:
                        html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                        html.Append("<br>");

                        foreach (IBDNode child in children)
                        {
                            html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "", pFootnotes, pObjectsOnPage));
                            html.Append("<br>");
                        }
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
                    case BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol:
                        //ks: Nothing in the editor: Zero child types defined.
                        break;
                    
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                    case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                    case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                    case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                    case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                    case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                    case BDConstants.LayoutVariantType.Microbiology_EmpiricTherapy:
                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum }));
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

                    case BDConstants.LayoutVariantType.TreatmentRecommendation16_CultureDirected:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia, BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis, BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis, BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubsection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy_CrossReactivity }));

                        foreach (IBDNode child in children)
                        {
                            switch (child.NodeType)
                            {
                                case BDConstants.BDNodeType.BDSubsection:
                                    html.Append(BuildBDSubSectionHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                                    break;
                                case BDConstants.BDNodeType.BDTable:
                                    //ks: No data/definition yet...
                                    break;
                            }
                        }

                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubsection, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens }));
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Immunization:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine, BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail }));
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_PreOp:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk, BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I, BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_SexualAssault }));
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Communicable:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive, BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDDisease, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza, BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis }));
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                        // complex: builds multiple pages
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage }));
                        html.Append(BuildBDSectionProphylaxisInfectionPreventionHtmlAndPages(pContext, pNode, pFootnotes, pObjectsOnPage, pLevel));
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis:
                    case BDConstants.LayoutVariantType.Prophylaxis_IERecommendation:
                        // Complex: Builds multiple pages
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IERecommendation }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTherapyGroup, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage }));
                        html.Append(BuildBDSectionProphylaxisEndocarditisHtmlAndPages(pContext, pNode, pFootnotes, pObjectsOnPage, pLevel));
                        break;
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPathogen, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        html.AppendFormat("<{0}>Infectious Agent</{0}>", HtmlHeaderTagLevelString(pLevel + 1));
                        List<BDHtmlPage> generatedPages = new List<BDHtmlPage>();
                        foreach (IBDNode child in children)
                        {
                            List<BDHtmlPage> childPages = null;
                            html.Append(BuildBDPathogenHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 2, out childPages));
                            // Only one page should be generated
                            if(null != childPages) generatedPages.AddRange(childPages);
                        }

                        //Expects that children.count == generatedPages.count: May it blow up real gud if it isn't
   
                        for (int i = 0; i < generatedPages.Count; i++)
                            html.AppendFormat(anchorTag, generatedPages[i].Uuid.ToString().ToUpper(), children[i].Name);

                        break;
                    case BDConstants.LayoutVariantType.Microbiology_Antibiogram:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAttachment, new BDConstants.LayoutVariantType[] { layoutVariant }));
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

                pObjectsOnPage.Add(pNode.Uuid);

                //html.Append("<p>"); // why is this here??  LD
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

                            html.AppendFormat(@"<table><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr>", c1Html, c2Html, c3Html);
                            foreach (IBDNode child in children)
                            {
                                html.Append(BuildBDAntimicrobialHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                            }

                            html.Append(@"</table>");
                        }
                        break;

                    case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDAntimicrobial, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;

                    case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I, BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II }));
                        html.Append(@"<table>");
                        foreach (IBDNode child in children) //tables
                        {
                            switch (child.LayoutVariant)
                            {
                                case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));

                                    List<IBDNode> tableSections = BDFabrik.GetChildrenForParent(pContext, child);
                                    foreach (IBDNode section in tableSections)
                                    {
                                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I_ContentRow }));

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
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableRow, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow }));
                                    //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTableSection, new BDConstants.LayoutVariantType[] { layoutVariant }));

                                    List<IBDNode> tableEntries = BDFabrik.GetChildrenForParent(pContext, child);
                                    foreach (IBDNode entry in tableEntries)
                                    {
                                        switch (entry.NodeType)
                                        {
                                            case BDConstants.BDNodeType.BDTableSection:
                                                List<IBDNode> sectionRows = BDFabrik.GetChildrenForParent(pContext, entry);
                                                foreach(IBDNode sectionRow in sectionRows)
                                                    html.Append(buildTableRowHtml(pContext, sectionRow as BDTableRow, false, false, pFootnotes, pObjectsOnPage));
                                                break;
                                            case BDConstants.BDNodeType.BDTableRow:
                                                html.Append(buildTableRowHtml(pContext, entry as BDTableRow, false, false, pFootnotes, pObjectsOnPage));
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                        html.Append(@"</table>");

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
                    case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDMicroorganismGroupHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }

                        break;
                    case BDConstants.LayoutVariantType.Microbiology_EmpiricTherapy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganismGroup, new BDConstants.LayoutVariantType[] { layoutVariant }));


                        break;
                    case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDMicroorganism, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSurgeryClassification, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                                amPages.Add(writeBDHtmlPage(pContext, child, amHtml, BDConstants.BDHtmlPageType.Navigation, amFootnotes, amObjects, null));
                            }
                        }
                        for (int i = 0; i < amPages.Count; i++)
                        {
                            html.AppendFormat(anchorTag, amPages[i].Uuid.ToString().ToUpper(), amPages[i].pageTitle);
                        }
                        break;
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        // child is BDAntimicrobialGroup: write a page for the child and add it as a link to this page
                        List<Guid> childObjects = new List<Guid>();
                        List<BDLinkedNote> childFootnotes = new List<BDLinkedNote>();
                        List<BDHtmlPage> childPages = new List<BDHtmlPage>();
                        foreach (IBDNode child in children)
                        {
                            if (!string.IsNullOrEmpty(child.Name))
                            {
                                // create a page and add to collection
                                string agHtml = BuildBDAntimicrobialGroupHtmlAndPage(pContext, child, childFootnotes, childObjects, pLevel);
                                currentPageMasterObject = child;
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
                                        // create a page and add to collection
                                        string gcHtml = BuildBDAntimicrobialHtml(pContext, gChild, gcFootnotes, gcObjects, pLevel);
                                        currentPageMasterObject = gChild;
                                        childPages.Add(writeBDHtmlPage(pContext, gChild, gcHtml, BDConstants.BDHtmlPageType.Navigation, gcFootnotes, gcObjects, null));
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < childPages.Count; i++)
                        {
                            html.AppendFormat(anchorTag, childPages[i].Uuid.ToString().ToUpper(), childPages[i].pageTitle);
                        }

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
                    case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { layoutVariant }));

                        foreach (IBDNode child in children)
                        {
                            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
                            List<Guid> objectsOnChildPage = new List<Guid>();

                            string childHtml = BuildBDTopicHtml(pContext, child, footnoteList, objectsOnChildPage, pLevel + 1);
                            currentPageMasterObject = child;
                            BDHtmlPage childPage = writeBDHtmlPage(pContext, child, childHtml, BDConstants.BDHtmlPageType.Navigation, footnoteList, objectsOnChildPage, null);
                            html.AppendFormat(anchorTag, childPage.Uuid.ToString().ToUpper(), childPage.pageTitle);
                        }

                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTopic, new BDConstants.LayoutVariantType[] { BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin }));
                        //foreach (IBDNode child in children)
                        //{
                        //    html.Append(BuildBDTopicHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        //}

                        foreach (IBDNode child in children)
                        {
                            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
                            List<Guid> objectsOnChildPage = new List<Guid>();

                            string childHtml = BuildBDTopicHtml(pContext, child, footnoteList, objectsOnChildPage, pLevel + 1);
                            currentPageMasterObject = child;
                            BDHtmlPage childPage = writeBDHtmlPage(pContext, child, childHtml, BDConstants.BDHtmlPageType.Navigation, footnoteList, objectsOnChildPage, null);
                            html.AppendFormat(anchorTag, childPage.Uuid.ToString().ToUpper(), childPage.pageTitle);
                        }

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

        public string BuildBDTopicHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // Gates on LayoutVariant to facilitate customization: Topics are too generic to do otherwise 

            StringBuilder html = new StringBuilder();
            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDTopic))
            {
                // describe the topic
                //BDNode topic = pNode as BDNode;
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));

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
                    case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDSubtopic, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDCategory, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(BuildBDCategoryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
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
                                html.AppendFormat("<table>");
                                html.AppendFormat("<tr><th rowspan=2>{0}</th><th colspan=2>Dosage with capsules</th><th>Daily dosage with solution (10mg/mL)</th></tr>", metadataLayoutColumns[0]);
                                html.AppendFormat("<tr><th>{0}</th><th>{1}</th><th>{2}</th></tr>", c1Html, c2Html, c3Html);
                            }
                            isFirstChild = false;

                            html.Append(BuildBDConfiguredEntryHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 2));
                        }
                        html.Append("</table>");

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
                        html.Append("</table>");
                        
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
                    case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDTable, new BDConstants.LayoutVariantType[] { layoutVariant }));
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
                        break;

                    default:
                        break;
                }
            }

            return html.ToString();
        }

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
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant, BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities, BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(buildBDPresentationHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
                        }
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                        //childDefinitionList.Add(new Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>(BDConstants.BDNodeType.BDPresentation, new BDConstants.LayoutVariantType[] { layoutVariant }));
                        foreach (IBDNode child in children)
                        {
                            html.Append(buildBDPresentationHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
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
                                    html.Append(buildBDPresentationHtml(pContext, child, pFootnotes, pObjectsOnPage, pLevel + 1));
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

        //ks:
        static public string HtmlHeaderTagLevelString(int pLevel)
        {
            string result = string.Format("h{0}", pLevel);
            return result;
        }

        public string buildBDPresentationHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, int pLevel)
        {
            // gated on child node type: very few variations and no identified customizations yet
            StringBuilder html = new StringBuilder();

            BDNode presentation = pNode as BDNode;
            if((null != presentation) && (presentation.NodeType == BDConstants.BDNodeType.BDPresentation))
            {
                bool isFirstChild = true;
                html.Append(buildNodeWithReferenceAndOverviewHTML(pContext, presentation, HtmlHeaderTagLevelString(pLevel), pFootnotes, pObjectsOnPage));
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, presentation);

                switch (presentation.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
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

        private string buildTherapyHtml(Entities pContext, BDTherapy pTherapy, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder therapyHtml = new StringBuilder();
            string styleString = string.Empty;

            // check join type - if none, then draw the bottom border on the table row
            if (pTherapy.therapyJoinType == (int)BDConstants.BDJoinType.Next)
                styleString = @"class=""d0""";  // row has bottom border
            else
                styleString = @"class=""d1""";  // NO bottom border

            therapyHtml.AppendFormat(@"<tr {0}><td>", styleString);

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(LEFT_SQUARE_BRACKET);

            string therapyNameHtml = string.Empty;

            string resolvedValue = null;

            if (pTherapy.nameSameAsPrevious.Value == true)
                therapyNameHtml = buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue);
            else
                therapyNameHtml = buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.Name, BDTherapy.PROPERTYNAME_THERAPY, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue);

            if (null != resolvedValue) therapiesHaveName = true;
            if(!string.IsNullOrEmpty(therapyNameHtml))
                therapyHtml.AppendFormat("<b>{0}</b>", therapyNameHtml);

            //if (pTherapy.rightBracket.Value == true)
            //    therapyHtml.Append(RIGHT_SQUARE_BRACKET);

            therapyHtml.Append(@"</td>");

            // Dosage
            resolvedValue = null;
            string dosageHtml = string.Empty;
            if (pTherapy.dosageSameAsPrevious.Value == true)
                dosageHtml = buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue);
            else
                dosageHtml = buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue);

            if (null != resolvedValue) therapiesHaveDosage = true;
            therapyHtml.AppendFormat("<td>{0}</td>", dosageHtml);

            // Duration
            resolvedValue = null;
            string durationHtml = string.Empty;
            if (pTherapy.durationSameAsPrevious.Value == true)
                durationHtml = buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDuration, BDTherapy.PROPERTYNAME_DURATION, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue);
            else
                durationHtml = buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration, BDTherapy.PROPERTYNAME_DURATION, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue);

            if (null != resolvedValue) therapiesHaveDuration = true;

            string rightBracket = "";
            if (pTherapy.rightBracket.Value == true)
                rightBracket = RIGHT_SQUARE_BRACKET;

            therapyHtml.AppendFormat("<td>{0}{1}</td>", durationHtml, rightBracket);

            therapyHtml.Append(@"</tr>");
            therapyHtml.AppendFormat(@"<tr><td> {0}</td><td /><td /></tr>", retrieveConjunctionString((int)pTherapy.therapyJoinType));
            return therapyHtml.ToString();
        }

        private string buildTherapyWithTwoDosagesHtml(Entities pContext, BDTherapy pTherapy, bool includeDuration, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder therapyHtml = new StringBuilder();
            string styleString = string.Empty;
            string resolvedValue = null;

            // check join type - if none, then draw the bottom border on the table row
            if (pTherapy.therapyJoinType == (int)BDConstants.BDJoinType.Next)
                styleString = @"class=""d0""";
            else
                styleString = @"class=""d1""";

            therapyHtml.AppendFormat(@"<tr {0}>", styleString);

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(LEFT_SQUARE_BRACKET);

            if (pTherapy.nameSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<td>{0}", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, "b", pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.AppendFormat("<td>{0}", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.Name, BDTherapy.PROPERTYNAME_THERAPY, "b", pFootnotes, pObjectsOnPage, out resolvedValue));

            if (null != resolvedValue) therapiesHaveName = true;

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(RIGHT_SQUARE_BRACKET);

            therapyHtml.Append(@"</td>");

            // Dosage
            resolvedValue = null;
            if (pTherapy.dosageSameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

            if (null != resolvedValue) therapiesHaveDosage = true;

            // Dosage 1
            if (pTherapy.dosage1SameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

            if (null != resolvedValue) therapiesHaveDosage = true;

            // Duration
            if (includeDuration)
            {
                resolvedValue = null;
                if (pTherapy.durationSameAsPrevious.Value == true)
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDuration, BDTherapy.PROPERTYNAME_DURATION, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
                else
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration, BDTherapy.PROPERTYNAME_DURATION, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

                if (null != resolvedValue) therapiesHaveDuration = true;
            }
            therapyHtml.Append(@"</tr>");
            therapyHtml.AppendFormat(@"<tr><td> {0}</td><td /><td /><td /></tr>", retrieveConjunctionString((int)pTherapy.therapyJoinType));
            
            return therapyHtml.ToString();
        }

        public string buildTherapyWithTwoDurationsHtml(Entities pContext, BDTherapy pTherapy, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            //string debugString = string.Format("Therapy {0} {1}", pTherapy.Uuid, pTherapy.Name);
            //Debug.WriteLine(debugString);

            string resolvedValue = null;

            StringBuilder therapyHtml = new StringBuilder();
            string styleString = string.Empty;

            // check join type - if NONE, then draw the bottom border on the table row
            if (pTherapy.therapyJoinType == (int)BDConstants.BDJoinType.Next)
                styleString = @"class=""d0""";
            else
                styleString = @"class=""d1""";

            therapyHtml.AppendFormat(@"<tr {0}>", styleString);

            // Name
            therapyHtml.Append(@"<td>");

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(LEFT_SQUARE_BRACKET);

            if (pTherapy.nameSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("{0}", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, "b", pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.AppendFormat("{0}", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.Name, BDTherapy.PROPERTYNAME_THERAPY, "b", pFootnotes, pObjectsOnPage, out resolvedValue));

            if (null != resolvedValue) therapiesHaveName = true;

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(RIGHT_SQUARE_BRACKET);

            therapyHtml.Append(@"</td>");

            // Dosage
            resolvedValue = null;
            if (pTherapy.dosageSameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

            if (null != resolvedValue) therapiesHaveDosage = true;

            // Duration
            resolvedValue = null;
            if (pTherapy.durationSameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDuration, BDTherapy.PROPERTYNAME_DURATION, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration, BDTherapy.PROPERTYNAME_DURATION, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

            if (null != resolvedValue) therapiesHaveDuration = true;

            // Duration 1
            resolvedValue = null;
            if (pTherapy.duration1SameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDuration1, BDTherapy.PROPERTYNAME_DURATION_1, "td", pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration1, BDTherapy.PROPERTYNAME_DURATION_1, "td", pFootnotes, pObjectsOnPage, out resolvedValue));

            if (null != resolvedValue) therapiesHaveDuration = true;

            therapyHtml.Append(@"</tr>");
            therapyHtml.AppendFormat(@"<tr><td> {0}</td><td /><td /><td /></tr>", retrieveConjunctionString((int)pTherapy.therapyJoinType));
            return therapyHtml.ToString();
        }

        private string buildTherapyWithCombinedColumnHtml(Entities pContext, BDTherapy pTherapy, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            string resolvedValue = null;

            StringBuilder therapyHtml = new StringBuilder();
            string styleString = string.Empty;

            // check join type - if none, then draw the bottom border on the table row
            if (pTherapy.therapyJoinType == (int)BDConstants.BDJoinType.Next)
                styleString = @"class=""d0""";  // row has bottom border
            else
                styleString = @"class=""d1""";  // NO bottom border

            therapyHtml.AppendFormat(@"<tr {0}><td>", styleString);

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(LEFT_SQUARE_BRACKET);

            if (pTherapy.nameSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<b>{0}</b>", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.AppendFormat("<b>{0}</b>", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.Name, BDTherapy.PROPERTYNAME_THERAPY, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue));

            if (null != resolvedValue) therapiesHaveName = true;

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(RIGHT_SQUARE_BRACKET);

            therapyHtml.Append(@"</td>");

            // Dosage + Duration are entered into the Dosage property
            resolvedValue = null;
            if (pTherapy.dosageSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<td>{0}</td>", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue));
            else
                therapyHtml.AppendFormat("<td>{0}</td>", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, string.Empty, pFootnotes, pObjectsOnPage, out resolvedValue));
            if (null != resolvedValue) therapiesHaveDosage = true;

            therapyHtml.Append(@"</tr>");
            therapyHtml.AppendFormat(@"<tr><td> {0}</td><td /><td /></tr>", retrieveConjunctionString((int)pTherapy.therapyJoinType));

            return therapyHtml.ToString();
        }

        /// <summary>
        /// Generate table row HTML for BDDosage
        /// Includes generation of footnote markers, and addition of footnotes to list
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns></returns>
        private string buildDosageWithCostHTML(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            BDDosage dosageNode = pNode as BDDosage;
            StringBuilder dosageHTML = new StringBuilder();
            string styleString = string.Empty;

            dosageHTML.AppendFormat("<tr>{0}", buildNodePropertyHTML(pContext, dosageNode, dosageNode.Uuid, dosageNode.dosage, BDDosage.PROPERTYNAME_DOSAGE, "td", pFootnotes, pObjectsOnPage));
            dosageHTML.Append(buildNodePropertyHTML(pContext, dosageNode, dosageNode.Uuid, dosageNode.cost, BDDosage.PROPERTYNAME_COST, "td", pFootnotes, pObjectsOnPage));
            dosageHTML.Append("</tr>");
            return dosageHTML.ToString();
        }

        private string buildCellHTML(Entities pContext, IBDNode pCellParentNode, string pPropertyName, string pPropertyValue, bool includeCellTags, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            string cellTag = includeCellTags == true ? "td" : string.Empty;

            return buildNodePropertyHTML(pContext, pCellParentNode, pCellParentNode.Uuid, pPropertyValue, pPropertyName, cellTag, pFootnotes, pObjectsOnPage);
        }

        private string buildDosageHTML(Entities pContext, IBDNode pNode, string pDosageGroupName, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            BDDosage dosageNode = pNode as BDDosage;
            StringBuilder dosageHTML = new StringBuilder();
            string styleString = string.Empty;
            string colSpanTag = string.Empty;  

            if (dosageNode.dosage2SameAsPrevious && dosageNode.dosage3SameAsPrevious && dosageNode.dosage4SameAsPrevious)
                colSpanTag = @" colspan=4";
            if (dosageNode.dosage2SameAsPrevious && dosageNode.dosage3SameAsPrevious && !dosageNode.dosage4SameAsPrevious)
                colSpanTag = @" colspan=3";
            if (dosageNode.dosage2SameAsPrevious && !dosageNode.dosage3SameAsPrevious && !dosageNode.dosage4SameAsPrevious)
                colSpanTag = @" colspan=2";

            // dosage group if it exists, then adult dose in cell
            if (!string.IsNullOrEmpty(pDosageGroupName))
                dosageHTML.AppendFormat("<td{0}>{1}<br>{2}</td>", colSpanTag, pDosageGroupName, buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage,BDDosage.PROPERTYNAME_DOSAGE,pFootnotes, pObjectsOnPage));
            else
                dosageHTML.AppendFormat(@"<td{0}>{1}</td>", colSpanTag, pDosageGroupName, buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage,BDDosage.PROPERTYNAME_DOSAGE,pFootnotes, pObjectsOnPage));

            colSpanTag = string.Empty;
            // 3 remaining doses in cells
            // Dosage 2
            if (dosageNode.dosage3SameAsPrevious && dosageNode.dosage4SameAsPrevious)
                colSpanTag = @" colspan=3";
            if (dosageNode.dosage3SameAsPrevious && !dosageNode.dosage4SameAsPrevious)
                colSpanTag = @" colspan=2";

            if (!dosageNode.dosage2SameAsPrevious)
            {
                string d2Html = buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage2, BDDosage.PROPERTYNAME_DOSAGE2, pFootnotes, pObjectsOnPage);
                if (!string.IsNullOrEmpty(d2Html))
                    dosageHTML.AppendFormat(@"<td{0}>{1}</td>", colSpanTag, d2Html);
                else
                    dosageHTML.Append(@"<td />");
            }
            colSpanTag = string.Empty;

            // Dosage 3
            if (dosageNode.dosage3SameAsPrevious)
                colSpanTag = @" colspan=2";

            if (!dosageNode.dosage3SameAsPrevious)
            {
                string d3Html = buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage3, BDDosage.PROPERTYNAME_DOSAGE3, pFootnotes, pObjectsOnPage);
                if (!string.IsNullOrEmpty(d3Html))
                    dosageHTML.AppendFormat(@"<td{0}>{1}</td>", colSpanTag, d3Html);
                else
                    dosageHTML.Append(@"<td />");
            }
            colSpanTag = string.Empty;

            // Dosage 4
            if (!dosageNode.dosage4SameAsPrevious)
            {
                string d4Html = buildNodePropertyHTML(pContext, dosageNode, dosageNode.dosage4, BDDosage.PROPERTYNAME_DOSAGE4, pFootnotes, pObjectsOnPage);
                if (!string.IsNullOrEmpty(d4Html))
                    dosageHTML.AppendFormat(@"<td{0}>{1}</td>", colSpanTag, d4Html);
                else
                    dosageHTML.Append(@"<td />");
            }
            return dosageHTML.ToString();
        }

        private string buildNodeWithReferenceAndOverviewHTML(Entities pContext, IBDNode pNode, string pHeaderTagLevel, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder nodeHTML = new StringBuilder();
            nodeHTML.Append(buildNodePropertyHTML(pContext, pNode, pNode.Uuid, pNode.Name, BDNode.PROPERTYNAME_NAME, pHeaderTagLevel, pFootnotes, pObjectsOnPage));

            nodeHTML.Append(buildReferenceHtml(pContext, pNode, pObjectsOnPage));

            return nodeHTML.ToString();
        }

        private string buildTextFromNotes(List<BDLinkedNote> pNotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder noteString = new StringBuilder();
            foreach (BDLinkedNote note in pNotes)
            {
                if (note.documentText.Length > EMPTY_PARAGRAPH)
                {
                    noteString.Append(note.documentText);
                    pObjectsOnPage.Add(note.Uuid);
                }
            }
            return noteString.ToString();
        }


        private string buildTableRowHtml(Entities pContext, BDTableRow pRow, bool forceHeaderRow, bool markFooterAtEnd, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder tableRowHTML = new StringBuilder();
            string startCellTag = @"<td>";
            string endCellTag = @"</td>";
            string firstCellStartTag = @"<td colspan=3>";
            if (pRow != null)
            {
                if (BDFabrik.RowIsHeaderRow(pRow) || forceHeaderRow)
                {
                    startCellTag = @"<th>";
                    endCellTag = @"</th>";
                    firstCellStartTag = @"<th colspan=3>";
                }
                pObjectsOnPage.Add(pRow.Uuid);
                tableRowHTML.Append(@"<tr>");
                List<BDTableCell> cells = BDTableCell.RetrieveTableCellsForParentId(pContext, pRow.Uuid);
                for (int i = 0; i < cells.Count; i++)
                {
                    pObjectsOnPage.Add(cells[i].Uuid);
                    BDTableCell tableCell = cells[i];
                    string startTag = startCellTag;
                    if (i == 0 && pRow.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I_ContentRow)
                        startTag = firstCellStartTag;

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
                if (refPage.documentText.Length > EMPTY_PARAGRAPH)
                {
                    refHTML.AppendFormat(@"<br><a href=""{0}"">References</a>", refPage.Uuid.ToString().ToUpper());
                    pObjectsOnPage.Add(refPage.Uuid);
                }
            }

            return refHTML.ToString();
        }

        private string buildFooterMarkerForList(List<BDLinkedNote> pItemFootnotes, bool addToPageFooter, List<BDLinkedNote> pPageFootnotes, List<Guid> pObjectsOnPage)
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
                    pObjectsOnPage.Add(footer.Uuid);
                }
                // trim last comma
                if(footerMarker.Length > 5)
                    footerMarker.Remove(footerMarker.Length - 1, 1);
                footerMarker.Append("</sup>");
                return footerMarker.ToString();
            }
            return string.Empty;
        }

        private string buildAttachmentHTML(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder attHtml = new StringBuilder();
            attHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h4", pFootnotes, pObjectsOnPage));

            BDAttachment attachmentNode = pNode as BDAttachment;

            attHtml.AppendFormat(imgFileTag, attachmentNode.uuid.ToString().ToUpper(), attachmentNode.MimeFileExtension());

            return attHtml.ToString();
        }

        private string buildAntibioticsCSFPenetrationDosagesHTML(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotesOnPage, List<Guid> pObjectsOnPage)
        {
            // Table to ConfiguredEntry
            List<BDLayoutMetadataColumn> metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", pFootnotesOnPage, pObjectsOnPage));  // BDTable

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                List<string> columnHtml = new List<string>();
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[0], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, pFootnotesOnPage, pObjectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[1], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, pFootnotesOnPage, pObjectsOnPage));
                columnHtml.Add(buildHtmlForMetadataColumn(pContext, pNode, metadataLayoutColumns[2], BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, pFootnotesOnPage, pObjectsOnPage));
                
                bodyHTML.Append("<table><tr>");
                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}</th>", columnHtml[i]);
                bodyHTML.Append("</tr>");
                foreach (IBDNode child in childNodes)
                {
                    BDConfiguredEntry entry = child as BDConfiguredEntry;
                    string ce1HTML = buildNodePropertyHTML(pContext, child, child.Uuid, child.Name, BDConfiguredEntry.PROPERTYNAME_NAME, pFootnotesOnPage, pObjectsOnPage);
                    string ce2HTML = buildNodePropertyHTML(pContext, child, child.Uuid, entry.field01, BDConfiguredEntry.PROPERTYNAME_FIELD01, pFootnotesOnPage, pObjectsOnPage);
                    string ce3HTML = buildNodePropertyHTML(pContext, child, child.Uuid, entry.field02, BDConfiguredEntry.PROPERTYNAME_FIELD02, pFootnotesOnPage, pObjectsOnPage);
                    bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", ce1HTML, ce2HTML, ce3HTML);
                }
                bodyHTML.Append("</table>");
            }
            return bodyHTML.ToString();
        }

        private List<BDHtmlPage> buildAntimicrobialWithRiskHTML(Entities pContext, List<string> columnHtml, List<IBDNode> antimicrobials)
        {
            List<BDHtmlPage> pages = new List<BDHtmlPage>();
            foreach (IBDNode antimicrobial in antimicrobials)
            {
                List<BDLinkedNote> pageFootnotes = new List<BDLinkedNote>();
                // write an HTML page for the antimicrobial, build a link for the name
                StringBuilder antimicrobialHTMLBody = new StringBuilder();
                List<Guid> antimicrobialsOnPage = new List<Guid>();
                antimicrobialHTMLBody.Append(buildNodeWithReferenceAndOverviewHTML(pContext, antimicrobial as BDNode, "h4", pageFootnotes, antimicrobialsOnPage));

                List<IBDNode> amRisks = BDFabrik.GetChildrenForParent(pContext, antimicrobial);
                foreach (IBDNode amRisk in amRisks)
                {
                    BDAntimicrobialRisk risk = amRisk as BDAntimicrobialRisk;
                    antimicrobialHTMLBody.AppendFormat("<b>{0}</b>: {1}", columnHtml[0], buildNodePropertyHTML(pContext, risk, risk.riskFactor, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, pageFootnotes, antimicrobialsOnPage));
                    antimicrobialHTMLBody.AppendFormat("<p><b>{0}</b>: {1}</p>", columnHtml[1], buildNodePropertyHTML(pContext, risk, risk.aapRating, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, pageFootnotes, antimicrobialsOnPage));
                    antimicrobialHTMLBody.AppendFormat("<p><b>{0}</b>: {1}</p>", columnHtml[2], buildNodePropertyHTML(pContext, risk, risk.relativeInfantDose, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, pageFootnotes, antimicrobialsOnPage));
                }
                antimicrobialHTMLBody.AppendFormat("<p><b>Comments:</b><br>{0}</p>", buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, antimicrobial, BDConstants.LinkedNoteType.UnmarkedComment, antimicrobialsOnPage));
                pages.Add(writeBDHtmlPage(pContext, antimicrobial, antimicrobialHTMLBody, BDConstants.BDHtmlPageType.Data, pageFootnotes, antimicrobialsOnPage, null));
            }
            return pages;
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
            string cLabel = retrieveMetadataLabelForPropertyName(pContext, pNodeType, pPropertyName, pMetadataColumn);

            List<BDLinkedNote> marked = retrieveNotesForLayoutColumn(pContext, pMetadataColumn, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarked = retrieveNotesForLayoutColumn(pContext, pMetadataColumn, BDConstants.LinkedNoteType.UnmarkedComment);
            List<BDLinkedNote> cFootnotes = retrieveNotesForLayoutColumn(pContext, pMetadataColumn, BDConstants.LinkedNoteType.Footnote);
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
            string footnoteMarker = buildFooterMarkerForList(cFootnotes, true, pFootnotes, pObjectsOnPage);
            
            //pObjectsOnPage.Add(pMetadataColumn.Uuid);

            BDHtmlPage notePage = generatePageForLinkedNotesLayoutColumn(pContext, pMetadataColumn, marked, unmarked);

            if (notePage != null)
                columnHtml.AppendFormat(@"<a href=""{0}"">{1}{2}</a>", notePage.Uuid.ToString().ToUpper(), cLabel, footnoteMarker);
            else
                columnHtml.AppendFormat(@"{0}{1}", cLabel, footnoteMarker);

            return columnHtml.ToString();
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
            return buildNodePropertyHTML(pContext, pNode, pNoteParentId, pPropertyValue, pPropertyName,pHtmlTag, pFootnotes, pObjectsOnPage, out resolvedValue);
        }

        private string buildNodePropertyHTML(Entities pContext, IBDNode pNode, Guid pNoteParentId, string pPropertyValue, string pPropertyName, string pHtmlTag, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage, out string pResolvedValue)
        {
            string startTag = (pHtmlTag.Length > 0) ? string.Format("<{0}>", pHtmlTag) : string.Empty;
            string endTag = (pHtmlTag.Length > 0) ? string.Format("</{0}>", pHtmlTag) : string.Empty;

            StringBuilder propertyHTML = new StringBuilder();
            List<BDLinkedNote> propertyFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.Footnote));
            string footerMarker = buildFooterMarkerForList(propertyFooters, true, pFootnotes, pObjectsOnPage);

            List<BDLinkedNote> inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.UnmarkedComment);

            //ks: added "New " prefix to permit the use of terms like "Table A" to appear in the name of a BDTable instance
            string namePlaceholderText = string.Format(@"New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
            if (pPropertyValue.Contains(namePlaceholderText) || pPropertyValue == "SINGLE PRESENTATION")
                pPropertyValue = string.Empty;

            // overview

            string overviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid, pObjectsOnPage);
            if (overviewHTML.Length <= EMPTY_PARAGRAPH)
                overviewHTML = @"";

            pObjectsOnPage.Add(pNode.Uuid);

            BDHtmlPage notePage = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.NodeType, marked, unmarked, pPropertyName);

            pResolvedValue = string.Format("{0}{1}{2}", pPropertyValue.Trim(), footerMarker, BDUtilities.buildTextFromInlineNotes(inline, pObjectsOnPage));

            if (pHtmlTag.ToLower() == "td")
            {
                if (notePage != null)
                {
                    if (pPropertyValue.Length > 0)
                        propertyHTML.AppendFormat(@"{1}<a href=""{0}"">{2}{3}</a>{4}{5}{6}", notePage.Uuid.ToString().ToUpper(), startTag, pPropertyValue.Trim(), footerMarker, BDUtilities.buildTextFromInlineNotes(inline, pObjectsOnPage), overviewHTML, endTag);
                    else
                    {
                        string inlineText = BDUtilities.buildTextFromInlineNotes(inline, pObjectsOnPage);
                        if (inlineText.Length > 0)
                        {
                            pResolvedValue = string.Format(@"<a href=""{0}"">{1}</a>", notePage.Uuid.ToString().ToUpper(), inlineText);
                        }
                        else
                        {
                            pResolvedValue = string.Format(@"<a href=""{0}"">See Comments.</a>", notePage.Uuid.ToString().ToUpper());
                        }
                        propertyHTML.AppendFormat(@"{0}{1}{2}{3}", startTag, pResolvedValue, overviewHTML, endTag);
                    }
                }
                else
                {
                    propertyHTML.AppendFormat(@" {0}{1}{2}{3}", startTag, pResolvedValue, overviewHTML, endTag);
                }
            }
            else
            {
                if (notePage != null)
                {
                    if (pPropertyValue.Length > 0)
                        propertyHTML.AppendFormat(@"{1}<a href=""{0}"">{2}{3}</a>{4}{5}{6}", notePage.Uuid.ToString().ToUpper(), startTag, pPropertyValue.Trim(), footerMarker, BDUtilities.buildTextFromInlineNotes(inline, pObjectsOnPage), endTag, overviewHTML);
                    else
                    {
                        string inlineText = BDUtilities.buildTextFromInlineNotes(inline, pObjectsOnPage);
                        if (inlineText.Length > 0)
                        {
                            pResolvedValue = string.Format(@"<a href=""{0}""{1}</a>", notePage.Uuid.ToString().ToUpper(), inlineText);
                        }
                        else
                        {
                            pResolvedValue = string.Format(@"<a href=""{0}"">See Comments.</a>", notePage.Uuid.ToString().ToUpper());
                        }
                        
                        propertyHTML.AppendFormat(@"{0}{1}{2}{3}", startTag, pResolvedValue, endTag, overviewHTML);
                    }
                }
                else
                {
                    propertyHTML.AppendFormat(@" {0}{1}{2}{3}", startTag, pResolvedValue, endTag, overviewHTML);
                }
            }

            if (pResolvedValue.Trim().Length == 0) pResolvedValue = null;

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
                    pObjectsOnPage.Add(linkedNote.Uuid);
                }
            }
            if (linkedNoteHtml.Length > EMPTY_PARAGRAPH)
                return linkedNoteHtml.ToString();
            else
                return "";
        }

        private string retrieveNoteTextForConfiguredEntryField(Entities pContext, Guid pParentId, string pNotePropertyName, List<Guid> pObjectsOnPage, List<BDLinkedNote> pFootnotesOnPage)
        {
            return retrieveNoteTextForConfiguredEntryField(pContext, pParentId, pNotePropertyName, string.Empty, pObjectsOnPage, false, pFootnotesOnPage);
        }

        private string retrieveNoteTextForConfiguredEntryField(Entities pContext, Guid pParentId, string pNotePropertyName, string pFieldPropertyName, List<Guid> pObjectsOnPage, bool trimTags, List<BDLinkedNote> pFootnotesOnPage)
        {
            StringBuilder noteText = new StringBuilder();
            List<BDLinkedNote> notes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pNotePropertyName, BDConstants.LinkedNoteType.Inline);
            foreach (BDLinkedNote note in notes)
            {
                if (null != note && note.documentText.Length > EMPTY_PARAGRAPH)
                {
                    string resultText = string.Empty;
                    if (trimTags) // trim the start and end paragraph tags
                    {
                        if (note.documentText.StartsWith("<p>"))
                            resultText = note.documentText.Substring(3);
                        else
                            resultText = note.documentText;
                        if(resultText.EndsWith("</p>"))
                            resultText = resultText.Substring(0,resultText.Length - 4);
                    }
                    else
                        resultText = note.documentText;
                    noteText.Append(resultText);
                }
                // retrieve any linked notes for the named property; add to footnote collection and mark the text
                if (pFieldPropertyName != string.Empty)
                {
                    List<BDLinkedNote> fieldNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pFieldPropertyName, BDConstants.LinkedNoteType.Footnote);
                    fieldNotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pFieldPropertyName, BDConstants.LinkedNoteType.Inline));
                    fieldNotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pFieldPropertyName, BDConstants.LinkedNoteType.MarkedComment));
                    fieldNotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pFieldPropertyName, BDConstants.LinkedNoteType.UnmarkedComment));
                    List<string> footnoteMarkers = new List<string>();
                    noteText.Append(buildFooterMarkerForList(fieldNotes, true, pFootnotesOnPage, pObjectsOnPage));
                }

            }
            return noteText.ToString();
        }

        private List<BDLinkedNote> retrieveNotesForParentAndPropertyOfLinkedNoteType(Entities pContext, Guid pParentId, string pPropertyName, BDConstants.LinkedNoteType pNoteType)
        {
            List<BDLinkedNote> noteList = new List<BDLinkedNote>();
            if (null != pPropertyName && pPropertyName.Length > 0)
            {
                List<BDLinkedNoteAssociation> list = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(pContext, pParentId, pPropertyName);
                foreach (BDLinkedNoteAssociation assn in list)
                {
                    if (assn.linkedNoteType == (int)pNoteType)
                    {
                        BDLinkedNote linkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, assn.linkedNoteId);
                        if (null != linkedNote)
                            noteList.Add(linkedNote);
                    }
                }
            }
            return noteList;
        }

        private void processTextForCarriageReturn(Entities pContext, BDHtmlPage pPage)
        {
            string pageText = pPage.documentText;
            pageText.Replace("\n", "<br>");
            BDHtmlPage.Save(pContext, pPage);
        }

        private void processTextForSubscriptAndSuperscriptMarkup(Entities pContext, BDHtmlPage pPage)
        {
            string superscriptStart = @"{";
            string superscriptEnd = @"}";
            string subscriptStart = @"{{";
            string subscriptEnd = @"}}";
            string htmlSuperscriptStart = @"<sup>";
            string htmlSuperscriptEnd = @"</sup>";
            string htmlSubscriptStart = @"<sub>";
            string htmlSubscriptEnd = @"</sub>";

            string newText = pPage.documentText;

            // do subscripts first because of double braces
            while (newText.Contains(subscriptStart))
            {
                int tStartIndex = newText.IndexOf(subscriptStart);
                newText = newText.Remove(tStartIndex, subscriptStart.Length);
                newText = newText.Insert(tStartIndex, htmlSubscriptStart);
                int tEndIndex = newText.IndexOf(subscriptEnd, tStartIndex);
                newText = newText.Remove(tEndIndex, subscriptEnd.Length);
                newText = newText.Insert(tEndIndex, htmlSubscriptEnd);
            }

            while (newText.Contains(superscriptStart))
            {
                int tStartIndex = newText.IndexOf(superscriptStart);
                newText = newText.Remove(tStartIndex, superscriptStart.Length);
                newText = newText.Insert(tStartIndex, htmlSuperscriptStart);
                int tEndIndex = newText.IndexOf(superscriptEnd, tStartIndex);
                newText = newText.Remove(tEndIndex, superscriptEnd.Length);
                newText = newText.Insert(tEndIndex, htmlSuperscriptEnd);
            }

            pPage.documentText = newText;
            BDHtmlPage.Save(pContext, pPage);
        }

        private void processTextForInternalLinks(Entities pContext, BDHtmlPage pPage, List<Guid> pRespresentedNodes, List<Guid> pExistingPages)
        {
            postProcessingPageLayoutVariant = pPage.layoutVariant;
            //BDNodeToHtmlPageIndex index = BDNodeToHtmlPageIndex.RetrieveIndexEntryForHtmlPageId(pContext, pPage.Uuid);
            //if (index != null)
            //    currentChapter = BDFabrik.RetrieveNode(pContext, index.chapterId);
            //else
            //    currentChapter = null;

            string compareString = @"<a href=";
            StringBuilder newString = new StringBuilder();
            if (pPage.documentText.Contains(compareString))
            {
                int startPosition = 0;

                while (startPosition < pPage.documentText.Length)
                {
                    // find the anchor tag
                    int tagLocation = pPage.documentText.IndexOf(compareString, startPosition);
                    if (tagLocation >= 0)
                    {
                        // inspect the 'guid'
                        int guidStart = tagLocation + 1 + compareString.Length;
                        string guidString = pPage.documentText.Substring(guidStart, 36);
                        // if the guid exists as an external URL, dont change it...
                        if (!guidString.Contains("http://www"))
                        {
                            Guid anchorGuid = new Guid(guidString);
                            if (!pExistingPages.Contains(anchorGuid))
                            {
                                // none of the existing html pages has this guid so the existing link is invalid as its currently written
                                // look up the linkedNoteAssociation with the provided guid in the'parentKeyPropertyName'
                                // if returned object is null, then its either not found or collection was > 1 entry
                                BDLinkedNoteAssociation linkTargetAssn = BDLinkedNoteAssociation.RetrieveLinkedNoteAssociationForParentKeyPropertyName(pContext, anchorGuid.ToString());
                                if (linkTargetAssn != null)
                                {
                                    if (linkTargetAssn.internalLinkNodeId.HasValue)
                                    {
                                        Guid htmlPageId = Guid.Empty;

                                        // this is an internal link - first check the pagesMap for the HTML page containing that object
                                        BDHtmlPageMap mapEntry = pagesMap.FirstOrDefault<BDHtmlPageMap>(x => x.OriginalIBDObjectId == linkTargetAssn.internalLinkNodeId.Value);

                                        if (null != mapEntry && pExistingPages.Contains(mapEntry.HtmlPageId))
                                            htmlPageId = mapEntry.HtmlPageId;
                                        else
                                        {
                                            //ks: Expectation that internal links will always link to "data" pages rather than "linked note" pages
                                            htmlPageId = BDNodeToHtmlPageIndex.RetrieveHtmlPageIdForIBDNodeId(pContext, linkTargetAssn.internalLinkNodeId.Value, BDConstants.BDHtmlPageType.Data);
                                            if (htmlPageId == Guid.Empty)
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
                                        }
                                        else // if this is an internal link there should be a page for it
                                        {
                                            Debug.WriteLine("Unable to map link in {0} showing {1}", pPage.Uuid, anchorGuid);
                                            BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkIssueLog.txt", string.Format("{0}\tHtml page Uuid {1}\tAnchor Uuid {2}\tLNA {3}", DateTime.Now, pPage.Uuid, anchorGuid.ToString(), linkTargetAssn.Uuid.ToString()));
                                        }
                                    }
                                    else if (linkTargetAssn.linkedNoteId.HasValue)
                                    {
                                        BDHtmlPageMap mapEntry = pagesMap.FirstOrDefault<BDHtmlPageMap>(x => x.OriginalIBDObjectId == linkTargetAssn.linkedNoteId.Value);

                                        if (null != mapEntry && pExistingPages.Contains(mapEntry.HtmlPageId))
                                        {
                                            // modify anchor tag to point to the html page generated for the targeted node
                                            string newText = pPage.documentText.Replace(anchorGuid.ToString(), mapEntry.HtmlPageId.ToString().ToUpper());
                                            pPage.documentText = newText;
                                            BDHtmlPage.Save(pContext, pPage);
                                        }

                                        else
                                        {
                                            // create an html page for the linked note - if its a note-in-note it may not have been created yet
                                            BDLinkedNote targetNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, linkTargetAssn.linkedNoteId);
                                            if (targetNote.documentText.Length > EMPTY_PARAGRAPH)
                                            {
                                                List<Guid> objectsOnPage = new List<Guid>();
                                                objectsOnPage.Add(linkTargetAssn.linkedNoteId.Value);
                                                BDHtmlPage newPage = generatePageForLinkedNotes(pContext, linkTargetAssn.linkedNoteId.Value, BDConstants.BDNodeType.BDLinkedNote, targetNote.documentText, BDConstants.BDHtmlPageType.Data, objectsOnPage, linkTargetAssn.parentKeyPropertyName);

                                                string newText = pPage.documentText.Replace(anchorGuid.ToString(), newPage.Uuid.ToString().ToUpper());
                                                pPage.documentText = newText;
                                                BDHtmlPage.Save(pContext, pPage);
                                            }
                                        }
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


        private bool notesListHasContent(Entities pContext, List<BDLinkedNote> pNotes)
        {
            bool hasContent = false;
            foreach (BDLinkedNote note in pNotes)
            {
                if (note.documentText.Length > EMPTY_PARAGRAPH)
                {
                    hasContent = true;
                    break;
                }
            }
            return hasContent;
        }

        private void resetGlobalVariablesForTherapies()
        {
            previousTherapyName = string.Empty;
            previousTherapyDosage = string.Empty;
            previousTherapyDosage1 = string.Empty;
            previousTherapyDosage2 = string.Empty;
            previousTherapyDuration = string.Empty;
            previousTherapyDuration1 = string.Empty;
            previousTherapyDuration2 = string.Empty;
            previousTherapyId = Guid.Empty;
            therapiesHaveName = false;
            therapiesHaveDosage = false;
            therapiesHaveDuration = false;
        }

        private string retrieveConjunctionString(BDConstants.BDJoinType pBDJoinType)
        {
            return retrieveConjunctionString((int) pBDJoinType, null);
        }

        private string retrieveConjunctionString(int pBDJoinType)
        {
            return retrieveConjunctionString(pBDJoinType, null);
        }

        private string retrieveConjunctionString(int pBDJoinType, IBDNode pNode)
        {
            string joinString = string.Empty;
            // check for conjunctions and add a row for any that are found

            switch (pBDJoinType)
            {
                case (int) BDConstants.BDJoinType.Next:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.Next);
                    break;
                case (int) BDConstants.BDJoinType.AndWithNext:
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
                case (int) BDConstants.BDJoinType.OrWithNext:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.OrWithNext);
                    break;
                case (int) BDConstants.BDJoinType.ThenWithNext:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.ThenWithNext);
                    break;
                case (int) BDConstants.BDJoinType.WithOrWithoutWithNext:
                    joinString = BDUtilities.GetEnumDescription(BDConstants.BDJoinType.WithOrWithoutWithNext);
                    break;
                case (int) BDConstants.BDJoinType.Other:
                    joinString = string.Empty;
                    break;
                case (int) BDConstants.BDJoinType.AndOr:
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
      private string retrieveMetadataLabelForPropertyName(Entities pContext, BDConstants.BDNodeType pNodeType,  string pPropertyName, BDLayoutMetadataColumn pColumn)
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
        /// <returns></returns>
        private List<BDLinkedNote> retrieveNotesForLayoutColumn(Entities pContext, BDLayoutMetadataColumn pColumn, BDConstants.LinkedNoteType? pNoteType)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pColumn.Uuid);
            List<BDLinkedNote> notes = new List<BDLinkedNote>();
            foreach (BDLinkedNoteAssociation link in links)
                if (pNoteType != null)
                {
                    if (link.LinkedNoteType == pNoteType)
                        notes.Add(BDLinkedNote.RetrieveLinkedNoteWithId(pContext, link.linkedNoteId));
                }
                else
                    notes.Add(BDLinkedNote.RetrieveLinkedNoteWithId(pContext, link.linkedNoteId));
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

            //BDNodeToHtmlPageIndex indexEntry = BDNodeToHtmlPageIndex.RetrieveIndexEntryForIBDNodeId(pContext, masterGuid, pPageType);
            Guid chapterId = (currentChapter == null) ? Guid.Empty : currentChapter.Uuid;
 
            BDNodeToHtmlPageIndex indexEntry = BDNodeToHtmlPageIndex.RetrieveOrCreateForIBDNodeId(pContext, masterGuid, pPageType, chapterId, pParentKeyPropertyName);

            BDHtmlPage newPage = null;
            if (indexEntry != null)
                newPage = BDHtmlPage.RetrieveWithId(pContext, indexEntry.htmlPageId);
            if (newPage == null)
                newPage = BDHtmlPage.CreateBDHtmlPage(pContext, indexEntry.htmlPageId.Value /* Guid.NewGuid() */);

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

            //if (indexEntry == null)
            //{
            //    Guid chapterId = Guid.Empty;
            //    if (currentChapter != null)
            //        chapterId = currentChapter.Uuid;
            //    indexEntry = BDNodeToHtmlPageIndex.CreateBDNodeToHtmlPageIndex(pContext, masterGuid, newPage.Uuid, chapterId, pPageType);
            //}
            //else
            //{
            //    indexEntry.chapterId = currentChapter != null ? currentChapter.Uuid : Guid.Empty;
            //    indexEntry.wasGenerated = true;
            //}
            //BDNodeToHtmlPageIndex.Save(pContext, indexEntry);

            //if (pObjectsOnPage.Count == 0 && pDisplayParent != null)
            //    Debug.WriteLine("no objects added for page {0}", newPage.Uuid);

            //if (pDisplayParent != null)
            //    pagesMap.Add(new BDHtmlPageMap(newPage.Uuid, pDisplayParent.Uuid));

            if (newPage.HtmlPageType == BDConstants.BDHtmlPageType.Data)
            {
                List<Guid> filteredObjects = pObjectsOnPage.Distinct().ToList();
                foreach (Guid objectId in filteredObjects)
                {
                    pagesMap.Add(new BDHtmlPageMap(newPage.Uuid, objectId));
                }
            }
            return newPage;
        }

        private BDHtmlPage writeLayoutBDHtmlPage(Entities pContext, BDLayoutMetadataColumn pLayoutColumn, string pBodyHTML, List<Guid> pObjectsOnPage, BDConstants.BDHtmlPageType pPageType)
        {
            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + pBodyHTML + bottomHtml;

            Guid chapterId = (currentChapter == null) ? Guid.Empty : currentChapter.Uuid;

            BDNodeToHtmlPageIndex indexEntry = BDNodeToHtmlPageIndex.RetrieveOrCreateForIBDNodeId(pContext, pLayoutColumn.Uuid, pPageType, chapterId, null);

            BDHtmlPage newPage = null;
            if (indexEntry != null)
                newPage = BDHtmlPage.RetrieveWithId(pContext, indexEntry.htmlPageId);
            if (newPage == null)
                newPage = BDHtmlPage.CreateBDHtmlPage(pContext, indexEntry.htmlPageId.Value);

            newPage.displayParentType = (int)BDConstants.BDNodeType.BDLayoutColumn;
            newPage.displayParentId = pLayoutColumn.Uuid;
            newPage.documentText = pageHtml;
            newPage.htmlPageType = (int)BDConstants.BDHtmlPageType.Undefined;
            newPage.layoutVariant = currentChapter != null ? (int)currentChapter.LayoutVariant : postProcessingPageLayoutVariant;

            newPage.pageTitle = pLayoutColumn.label;
            
            if (newPage.layoutVariant == -1)
                Debug.WriteLine("Page has no layout assigned: {0}", newPage.Uuid);

            BDHtmlPage.Save(pContext, newPage);

            /*
            if (indexEntry == null)
            {
                Guid chapterId = Guid.Empty;
                if (currentChapter != null)
                    chapterId = currentChapter.Uuid;
                indexEntry = BDNodeToHtmlPageIndex.CreateBDNodeToHtmlPageIndex(pContext, pLayoutColumn.Uuid, newPage.Uuid, chapterId, pPageType);
            }
            else
            {
                indexEntry.chapterId = currentChapter != null ? currentChapter.Uuid : Guid.Empty;
                indexEntry.wasGenerated = true;
            }
            BDNodeToHtmlPageIndex.Save(pContext, indexEntry);
            */

            //if (pObjectsOnPage.Count == 0 && pLayoutColumn != null)
            //    Debug.WriteLine("no objects added for page {0}", newPage.Uuid);

            //if (pLayoutColumn != null)
            //    pagesMap.Add(new BDHtmlPageMap(newPage.Uuid, pLayoutColumn.Uuid));

            if (newPage.HtmlPageType == BDConstants.BDHtmlPageType.Data)
            {
                List<Guid> filteredObjects = pObjectsOnPage.Distinct().ToList();
                foreach (Guid objectId in filteredObjects)
                {
                    pagesMap.Add(new BDHtmlPageMap(newPage.Uuid, objectId));
                }
            }
            return newPage;
        }

        #endregion
    }




    public class BDHtmlPageGeneratorLogEntry
    {
        public Guid Uuid { get; set;}
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

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", this.Uuid, this.Label, this.Tag);
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
