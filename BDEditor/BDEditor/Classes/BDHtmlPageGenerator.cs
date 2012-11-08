﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class BDHtmlPageGenerator
    {
        private const int maxNodeType = 6;
        private const string topHtml = @"<html><head><meta http-equiv=""Content-type"" content=""text/html;charset=UTF-8\""/><meta name=""viewport"" content=""width=device-width; initial-scale=1.0; maximum-scale=1.0;""/><link rel=""stylesheet"" type=""text/css"" href=""bdviewer.css"" /> </head><body>";
        private const string bottomHtml = @"</body></html>";
        private const int EMPTY_PARAGRAPH = 8;  // <p> </p>
        private const string imgFileTag = "<img src=\"images/{0}{1}\" alt=\"\" width=\"300\" height=\"300\" />";
        private const string paintChipTag = "<img class=\"paintChip\" src=\"{0}\" alt=\"\" />";
        private const string PAINT_CHIP_ANTIBIOTICS = "AntibioticYellow.png";
        private const string PAINT_CHIP_DENTISTRY = "DentistryPurple.png";
        private const string PAINT_CHIP_ORGANISMS = "OrganismGreen.png";
        private const string PAINT_CHIP_TREATMENT_PAEDIATRIC = "PaediatricTreatmentLightBlue.png";
        private const string PAINT_CHIP_TREATMENT_ADULT = "TreatmentBlue.png";
        private const string PAINT_CHIP_PREGNANCY = "PregnancyRed.png";
        private const string PAINT_CHIP_PROPHYLAXIS = "ProphylaxisOrange.png";

        private List<BDLayoutMetadataColumn> metadataLayoutColumns = new List<BDLayoutMetadataColumn>();
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
        bool therapiesHaveDosage = false;
        bool therapiesHaveDuration = false;

        public List<BDHtmlPageMap> PagesMap
        {
            get { return pagesMap; }
        }

        public void Generate(Entities pContext, IBDNode pNode)
        {
            // delete pages from the local store
            BDHtmlPage.DeleteAll();

            // reset index entries to false
            BDNodeToHtmlPageIndex.ResetForRegeneration(pContext);

            generatePages(pNode);

            List<BDHtmlPage> pages = BDHtmlPage.RetrieveAll(pContext);
            List<Guid> displayParentIds = BDHtmlPage.RetrieveAllDisplayParentIDs(pContext);
            List<Guid> pageIds = BDHtmlPage.RetrieveAllIds(pContext);

            Debug.WriteLine("Post-processing HTML pages");
            foreach (BDHtmlPage page in pages)
            {
                processTextForInternalLinks(pContext, page, displayParentIds, pageIds);
                processTextForSubscriptAndSuperscriptMarkup(pContext, page);
                processTextForCarriageReturn(pContext, page);
            }
        }

        private void generatePages(IBDNode pNode)
        {
            Entities dataContext = new Entities();
            List<BDNode> chapters = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDChapter);
            List<BDHtmlPage> allPages = new List<BDHtmlPage>();
            if (pNode == null)
            {
                List<BDHtmlPage> childDetailPages = new List<BDHtmlPage>();

                List<BDHtmlPage> childNavPages = new List<BDHtmlPage>();
                foreach (BDNode chapter in chapters)
                {
                    currentChapter = chapter;
                    generateOverviewAndChildrenForNode(dataContext, chapter, childDetailPages, childNavPages);
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
                    generateOverviewAndChildrenForNode(dataContext, pNode, childDetailPages, childNavPages);
                }
                    allPages.AddRange(childDetailPages);
                    allPages.AddRange(childNavPages);
            }
            List<BDHtmlPage> chapterPages = allPages.Distinct().ToList();
            Debug.WriteLine("Creating home page with filtered distinct list");
            if (chapterPages.Count > 0)
                generateNavigationPage(dataContext, null, chapterPages);
        }

        /// <summary>
        /// Recursive method that traverses the navigation tree
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        private void generateOverviewAndChildrenForNode(Entities pContext, IBDNode pNode, List<BDHtmlPage> pNodeDetailPages, List<BDHtmlPage> pNodeNavPages)
        {
            // pNodeDetailPages gathers the detail HTML pages generated by beginDetailPage
            // must be passed in to this method to be outside the loop & gather them all
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

            return writeBDHtmlPage(pContext, pNode as BDNode, pageHTML, BDConstants.BDHtmlPageType.Navigation, footnotesOnPage, objectsOnPage);
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
            metadataLayoutColumns.Clear();

            generatePageForParentAndPropertyReferences(pContext, BDNode.PROPERTYNAME_NAME, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsClinicalGuidelines(pContext, pNode as BDNode));
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
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsPharmacodynamics(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsDosingInRenalImpairment(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticDosingInHepaticImpairment(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForDentalMicroorganisms(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForPLAntimicrobialsInPregnancy(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForPLAntimicrobialsInLactation(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForPLPreventionPerinatalInfection(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForMicrobiologyGramStain(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForMicrobiologyOrganisms(pContext, pNode));
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
                                    nodeChildPages.Add(generatePageForEmpiricTherapyDisease(pContext, pNode as BDNode));
                                }
                                else
                                    isPageGenerated = false;
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                                currentPageMasterObject = pNode;
                                nodeChildPages.Add(generatePageForProphylaxisCommunicableDiseases(pContext, pNode));
                                isPageGenerated = true;
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                                currentPageMasterObject = pNode;
                                nodeChildPages.Add(generatePageForProphylaxisCommunicableDiseases(pContext, pNode));
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
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfBCNE(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfParasitic(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfCultureDirected(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfCultureDirectedEndocarditis(pContext, pNode as BDNode));
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
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfCultureDirectedPeritonitis(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPresentation:
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
                            nodeChildPages.Add(generatePageForEmpiricTherapyPresentation(pContext, pNode as BDNode));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfCellulitisInExtremities(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfVesicularLesions(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDResponse:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfVesicularLesions(pContext, pNode as BDNode));
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
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsStepdown(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsCSFPenetration(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageforAntibioticsBLactam(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForAntibioticsNameListing(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_IERecommendation:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForProphylaxisEndocarditis(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForProphylaxisInfectionPrevention(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForPLCommunicableDiseases(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForPLPerinatalHIVProtocol(pContext, pNode));
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
                            nodeChildPages.Add(generatePageForAntibioticsDosingAndDailyCosts(pContext, pNode as BDNode));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfPneumonia(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForDentalProphylaxisDrugRegimens(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
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
                            nodeChildPages.Add(generatePageForAntibiotics_HepaticImpairmentGrading(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfEndocarditisPaediatrics(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Risk:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForProphylaxisFluidExposureRiskOfInfection(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_I:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForProphylaxisFluidExposureFollowupProtocolI(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForProphylaxisFluidExposureFollowupProtocolII(pContext, pNode));
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
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfFungalInfections(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForDentalProphylaxis(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                            currentPageMasterObject = pNode;
                            nodeChildPages.Add(generatePageForProhylaxisSexualAssault(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;

                case BDConstants.BDNodeType.BDChapter:
                case BDConstants.BDNodeType.BDMicroorganism:
                case BDConstants.BDNodeType.BDSubsection:
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));
            
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }

        #region Antibiotics sections
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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);

            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, pNode, "h1", footnotes, objectsOnPage));

            // child nodes can either be pathogen groups or topics (node with overview)
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode childNode in childNodes)
            {
                if (childNode.NodeType == BDConstants.BDNodeType.BDTopic)
                {
                    if (childNode.Name.Length > 0)
                        bodyHTML.AppendFormat("<h2>{0}</h2>", childNode.Name);
                    string nodeOverviewHTML = retrieveNoteTextForOverview(pContext, childNode.Uuid, objectsOnPage);
                    if (nodeOverviewHTML.Length > EMPTY_PARAGRAPH)
                    {
                        bodyHTML.Append(nodeOverviewHTML);
                    }
                }
                objectsOnPage.Add(childNode.Uuid);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in childNodes)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, "h3", footnotes, objectsOnPage));
                objectsOnPage.Add(child.Uuid);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
}

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode node in childNodes)
            {
                if (node.NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
                {
                    List<BDLinkedNote> inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, node.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
                    List<BDLinkedNote> marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, node.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
                    List<BDLinkedNote> unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, node.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);
                    BDHtmlPage notePage = generatePageForLinkedNotes(pContext, node.Uuid, node.NodeType, inline, marked, unmarked, objectsOnPage);
                    if (notePage == null)
                        bodyHTML.AppendFormat(@"<b>{0}</b><br>", node.Name);
                    else
                        bodyHTML.AppendFormat(@"<a href=""{0}""><b>{1}</b></a><br>", notePage.Uuid.ToString().ToUpper(), node.Name);
                    objectsOnPage.Add(node.Uuid);
                }
            }
           return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (antimicrobials.Count > 0)
            {
                // build markers and list for column header linked notes
                string c1Label = retrieveMetadataLabelForPropertyName(pContext, pNode.NodeType, BDNode.PROPERTYNAME_NAME);
                string c2Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE);
                string c3Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_COST);

                List<BDLinkedNote> c1Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]);
                string c1footerMarker = buildFooterMarkerForList(c1Links, true, footnotes, objectsOnPage);
                List<BDLinkedNote> c2Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]);
                string c2FooterMarker = buildFooterMarkerForList(c2Links, true, footnotes, objectsOnPage);
                List<BDLinkedNote> c3Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]);
                string c3FooterMarker = buildFooterMarkerForList(c3Links, true, footnotes, objectsOnPage);
                bodyHTML.AppendFormat(@"<table><tr><th>{0}{1}</th><th>{2}{3}</th><th>{4}{5}</th></tr>", c1Label, c1footerMarker, c2Label, c2FooterMarker, c3Label, c3FooterMarker);

                foreach (IBDNode antimicrobial in antimicrobials)
                {
                    List<BDLinkedNote> amFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, antimicrobial.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
                    string amFooterMarker = buildFooterMarkerForList(amFooters, true, footnotes, objectsOnPage);
                    // build each row of table, with antimicrobial name in first column
                    bodyHTML.AppendFormat(@"<tr><td>{0}{1}</td>", antimicrobial.Name, amFooterMarker);

                    StringBuilder dosageHTML = new StringBuilder();
                    dosageHTML.Append("<td>");
                    StringBuilder costHTML = new StringBuilder();
                    costHTML.Append("<td>");

                    List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, antimicrobial);
                    foreach (IBDNode child in childNodes)
                    {
                        if (child.NodeType == BDConstants.BDNodeType.BDTherapyGroup)
                        {
                            dosageHTML.AppendFormat("<b><u>{0}</u></b><br>", buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false, footnotes, objectsOnPage));
                            costHTML.Append("<br>");
                            List<IBDNode> lvl1Children = BDFabrik.GetChildrenForParent(pContext, child);
                            foreach (IBDNode lvl1Child in lvl1Children)
                            {
                                // BDDosageGroup
                                dosageHTML.AppendFormat("<u>{0}</u>:", buildCellHTML(pContext, lvl1Child, BDNode.PROPERTYNAME_NAME, lvl1Child.Name, false, footnotes, objectsOnPage));
                                costHTML.Append("<br>");

                                List<IBDNode> lvl2Children = BDFabrik.GetChildrenForParent(pContext, lvl1Child);
                                string cellLineTag = (lvl2Children.Count > 0) ? "<br>" : "";
                                foreach (IBDNode lvl2Child in lvl2Children)
                                {
                                    // BDDosage
                                    BDDosage dosage = lvl2Child as BDDosage;
                                    dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), cellLineTag);
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
                            dosageHTML.AppendFormat("<u>{0}</u>:", buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false, footnotes, objectsOnPage));
                            costHTML.Append("<br>");

                            List<IBDNode> lvl2Children = BDFabrik.GetChildrenForParent(pContext, child);
                            string cellLineTag = (lvl2Children.Count > 0) ? "<br>" : "";
                            foreach (IBDNode lvl2Child in lvl2Children)
                            {
                                // BDDosage
                                BDDosage dosage = lvl2Child as BDDosage;
                                dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), cellLineTag);
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
                            dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false, footnotes, objectsOnPage), cellLineTag);
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
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

        private BDHtmlPage generatePageForAntibioticsDosingAndMonitoring(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection && pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return null;
#endif
            }

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
                            else if (topicChild.NodeType == BDConstants.BDNodeType.BDSubtopic)
                            {
                                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topicChild, "h4", footnotes, objectsOnPage));
                                string noteText = retrieveNoteTextForOverview(pContext, topicChild.Uuid, objectsOnPage);
                                if (noteText.Length > 0)
                                    bodyHTML.Append(noteText);
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
                        }
                    }
                    else if (node.NodeType == BDConstants.BDNodeType.BDAttachment)
                    {
                        bodyHTML.Append(buildAttachmentHTML(pContext, node, footnotes, objectsOnPage));
                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotes, objectsOnPage));
            
            // child nodes are BDAntimicrobial 
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                List<string> labels = new List<string>();
                // build markers and list for column header linked notes
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE2));
                List<string> footerMarkers = new List<string>();
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, footnotes, objectsOnPage));

                bodyHTML.AppendFormat(@"<table><tr><th rowspan=4>{0}{1}</th><th rowspan=4>{2}{3}</th>",labels[0],footerMarkers[0],labels[1],footerMarkers[1]);
                bodyHTML.Append(@"<th colspan=3><b>Dose and Interval Adjustment for Renal Impairment</b></th></tr>");
                bodyHTML.AppendFormat(@"<tr><th colspan=""3""><b>{0}{1}</b></th><tr>",labels[2],footerMarkers[2]);
                bodyHTML.Append(@"<tr><th>&gt50</th><th>10 - 50</th><th>&lt10(Anuric)</th></tr>");

                foreach (IBDNode antimicrobial in childNodes) 
                {
                    List<IBDNode> dosageNodes = BDFabrik.GetChildrenForParent(pContext, antimicrobial);

                    string dosageGroupName = string.Empty;
                    foreach (IBDNode dNode in dosageNodes)
                    {
                        bodyHTML.AppendFormat("<tr><td>{0}</td>", antimicrobial.Name);
                        if (dNode.NodeType == BDConstants.BDNodeType.BDDosage)
                        {
                            bodyHTML.Append(buildDosageHTML(pContext, dNode, dosageGroupName, footnotes, objectsOnPage));
                            dosageGroupName = string.Empty;
                        }
                        else // BDDosageGroup
                            dosageGroupName = dNode.Name;
                    bodyHTML.Append("</tr>");
                    }
                }
                bodyHTML.Append(@"</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            // build markers and list for column header linked notes
            string c1Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME);
            string c2Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01);
            string c3Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02);
            string c4Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03);

            List<BDLinkedNote> c1Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]);
            string c1footerMarker = buildFooterMarkerForList(c1Links, true, footnotes, objectsOnPage);
            List<BDLinkedNote> c2Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]);
            string c2FooterMarker = buildFooterMarkerForList(c2Links, true, footnotes, objectsOnPage);
            List<BDLinkedNote> c3Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]);
            string c3FooterMarker = buildFooterMarkerForList(c3Links, true, footnotes, objectsOnPage);
            List<BDLinkedNote> c4Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[3]);
            string c4FooterMarker = buildFooterMarkerForList(c4Links, true, footnotes, objectsOnPage);
            bodyHTML.AppendFormat(@"<table><tr><th>{0}{1}</th><th>{2}{3}</th><th>{4}{5}</th><th>{6}{7}</th></tr>", c1Label, c1footerMarker, c2Label, c2FooterMarker, c3Label, c3FooterMarker,c4Label, c4FooterMarker);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode childNode in childNodes)
            {
                // children are BDConfiguredEntry
                BDConfiguredEntry entry = childNode as BDConfiguredEntry;
                bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", entry.Name, entry.field01, entry.field02, entry.field03);
            }
            bodyHTML.Append("</table>");
            bodyHTML.Append(buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, pNode, BDConstants.LinkedNoteType.UnmarkedComment, objectsOnPage));
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
                                            bodyHTML.AppendFormat(@"<tr><td colspan=3><nbsp><nbsp>{0}<td></tr>", sectionChild.Name);
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
                            //TODO:  Make fontsize smaller than antimicrobialSection name
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan=3>{0}<td></tr>", node.Name);
                        }
                    }
                }
                    bodyHTML.Append(@"</table>");
            }

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode child in childNodes)
                {
                    if (child.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_CSFPenetration)
                    {
                        //Append HTML for Topic + overview
                        bodyHTML.AppendFormat(@"<h3>{0}</h3>", child.Name);
                        bodyHTML.Append(retrieveNoteTextForOverview(pContext, child.Uuid, objectsOnPage));

                        List<IBDNode> categories = BDFabrik.GetChildrenForParent(pContext, child);
                        foreach (IBDNode category in categories)
                        {
                            bodyHTML.AppendFormat(@"<h4>{0}</h4>", category.Name);
                            bodyHTML.Append("<table><tr><th>Excellent Penetration</th><th>Good Penetration</th><th>Poor Penetration</th></tr>");
                            objectsOnPage.Add(category.Uuid);
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
                                            objectsOnPage.Add(antimicrobial.Uuid);
                                            List<BDLinkedNote> itemFooter = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, antimicrobial.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
                                            if (itemFooter.Count == 0)
                                                colHTML.AppendFormat(@"{0}<br>", antimicrobial.Name);
                                            else
                                            {
                                                StringBuilder footerMarker = new StringBuilder();
                                                foreach (BDLinkedNote footer in itemFooter)
                                                {
                                                    if (!footnotes.Contains(footer))
                                                    {
                                                        footnotes.Add(footer);
                                                        objectsOnPage.Add(footer.Uuid);
                                                    }
                                                    footerMarker.AppendFormat(@"<sup>{0}</sup>,", footnotes.IndexOf(footer) + 1);
                                                }
                                                if (footerMarker.Length > 0)
                                                    footerMarker.Remove(footerMarker.Length - 1, 1);
                                                colHTML.AppendFormat(@"{0}{1}<br>", antimicrobial.Name, footerMarker);
                                            }
                                        }
                                        bodyHTML.Append(colHTML);
                                    }
                                    bodyHTML.Append(@"</td>");
                                }
                                bodyHTML.Append(@"</tr>");
                            }
                            bodyHTML.Append(@"</table>");
                        }
                    }
                    else
                        bodyHTML.Append(buildAntibioticsCSFPenetrationDosagesHTML(pContext,child, objectsOnPage));
            }

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
                                if(tableChild.Name.Length > 0)
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
                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }
        #endregion

        #region Treatment Recommendations sections
        /// <summary>
        /// Build HTML page at Disease level when only one Presentation is defined
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        private BDHtmlPage generatePageForEmpiricTherapyDisease(Entities pContext, BDNode pNode)
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> presentations = BDFabrik.GetChildrenForParent(pContext, pNode);

#if DEBUG
            if (presentations.Count > 1)
                throw new InvalidOperationException();
#endif
            foreach (IBDNode presentation in presentations)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, presentation as BDNode, "h2", footnotes, objectsOnPage));
                    List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, presentation);
                if (presentation.LayoutVariant == BDConstants.LayoutVariantType.Dental_RecommendedTherapy)
                {
                    foreach (IBDNode tGroup in childNodes)
                    {
                        BDTherapyGroup group = tGroup as BDTherapyGroup;
                        if (null != group) // bypass any pathogens that also appear at this level
                        {
                            bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, group, footnotes, objectsOnPage));
                            bodyHTML.Append(buildTherapyGroupHTML(pContext, group, footnotes, objectsOnPage));
                        }
                    }
                }
                else
                {
                    foreach (IBDNode pathogenGroup in childNodes)
                        bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footnotes, objectsOnPage));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

        /// <summary>
        /// Build HTML page to show Presentation and all it's children on one page
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        private BDHtmlPage generatePageForEmpiricTherapyPresentation(Entities pContext, BDNode pNode)
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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            // gastroenteritis:  get Topic as child of Presentation, then Pathogen Group
            if (pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis || pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected)
            {
                foreach (IBDNode topic in childNodes)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h3", footnotes, objectsOnPage));
                    List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
                    foreach (IBDNode pathogenGroup in pathogenGroups)
                        bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footnotes, objectsOnPage));
                }
            }
            else if (pNode.LayoutVariant == BDConstants.LayoutVariantType.Dental_RecommendedTherapy)
            {
                foreach (IBDNode tGroup in childNodes)
                {
                    BDTherapyGroup group = tGroup as BDTherapyGroup;
                    if (null != group)
                    {
                        bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, group, footnotes, objectsOnPage));
                        bodyHTML.Append(buildTherapyGroupHTML(pContext, group, footnotes, objectsOnPage));
                    }
                }
            }
            else
            {
                List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode pathogenGroup in childNodes)
                    bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footnotes, objectsOnPage));
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
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
                return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
            }
            else
            {
                return generatePageForEmpiricTherapyPresentation(pContext, pNode);
            }
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
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
                        bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, childNode as BDTherapyGroup, footnotes, objectsOnPage));
                        bodyHTML.Append(buildTherapyGroupHTML(pContext, childNode as BDTherapyGroup, footnotes, objectsOnPage));
                }
                else 
                {
                    string presentationOverview = retrieveNoteTextForOverview(pContext, childNode.Uuid, objectsOnPage);
                    if (presentationOverview.Length > EMPTY_PARAGRAPH)
                        bodyHTML.AppendFormat(@"<u><b>Comments</b></u><br>{0}", presentationOverview);
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

        /// <summary>
        /// Build page for CURB-65 - Pneumonia Severity of Illness Scoring System
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));
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
                            bodyHTML.Append(buildTableRowHtml(pContext, childNodes[0] as BDTableRow, true, false, footnotes, objectsOnPage));
                            for (int i = 1; i < childNodes.Count; i++)
                                bodyHTML.Append(buildTableRowHtml(pContext, childNodes[i] as BDTableRow, false, false, footnotes, objectsOnPage));
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
                            bodyHTML.Append(buildTableRowHtml(pContext, row as BDTableRow, false, false, footnotes, objectsOnPage));
                        else
                        {
                            List<IBDNode> sectionRows = BDFabrik.GetChildrenForParent(pContext, row);
                            foreach(IBDNode sectionRow in sectionRows)
                                bodyHTML.Append(buildTableRowHtml(pContext, sectionRow as BDTableRow, false, false, footnotes, objectsOnPage));
                        }
                    }
                }
            }
            bodyHTML.Append(@"</table>");

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

        /// <summary>
        /// For culture-proven pneumonia & meningitis
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            // show child nodes in a table
            List<IBDNode> resistances = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode resistance in resistances)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, resistance as BDNode, "h2", footnotes, objectsOnPage));
                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, resistance);
                foreach(IBDNode therapyGroup in therapyGroups) 
                {
                    bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, therapyGroup as BDTherapyGroup, footnotes, objectsOnPage));
                    bodyHTML.Append(buildTherapyGroupHTML(pContext, therapyGroup as BDTherapyGroup, footnotes, objectsOnPage));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode child in childNodes)
            {
                bodyHTML.Append(buildAttachmentHTML(pContext,child,footnoteList,objectsOnPage));
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }


        /// <summary>
        /// Build page at PathogenGroup downward
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode therapyGroup in therapyGroups)
            {
                bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, therapyGroup as BDTherapyGroup, footnotes, objectsOnPage));
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
                        therapyHTML.Append(buildTherapyWithTwoDosagesHtml(pContext, therapy, true, footnotes, objectsOnPage));

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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

        private BDHtmlPage generatePageForEmpiricTherapyOfCultureDirectedEndocarditis(Entities pContext, BDNode pNode)
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));
            List<IBDNode> resistances = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode resistance in resistances)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, resistance as BDNode, "h2", footnotes, objectsOnPage));
                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, resistance);
                foreach (IBDNode therapyGroup in therapyGroups)
                {
                    bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, therapyGroup as BDTherapyGroup, footnotes, objectsOnPage));
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
                            therapyHTML.Append(buildTherapyWithTwoDurationsHtml(pContext, therapy, footnotes, objectsOnPage));

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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

        private BDHtmlPage generatePageForEmpiricTherapyOfEndocarditisPaediatrics(Entities pContext, BDNode pNode)
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));
                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode therapyGroup in therapyGroups)
                {
                    bodyHTML.Append(buildNodeWithLinkedNotesInlineHtml(pContext, therapyGroup as BDTherapyGroup, footnotes, objectsOnPage));
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
                            therapyHTML.Append(buildTherapyHtml(pContext, therapy, footnotes, objectsOnPage));

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
                return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }
        
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();
            
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            List<string> labelsFromMetadata = new List<string>();
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapyGroup, BDTherapyGroup.PROPERTYNAME_NAME));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1));

            List<string> footnoteMarkersFromMetadata = new List<string>();
            List<BDLinkedNote> footnotesFromMetadata = new List<BDLinkedNote>();

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            List<BDHtmlPage> childPages = new List<BDHtmlPage>();
            List<BDLinkedNote> catFootnotes = new List<BDLinkedNote>();
            StringBuilder therapyGroupHTML = new StringBuilder();
            therapyGroupHTML.AppendFormat("<h2>{0}{1}</h2>", labelsFromMetadata[0], buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnotesFromMetadata, objectsOnPage));
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
                    childPages.Add(writeBDHtmlPage(pContext, child, categoryHTML, BDConstants.BDHtmlPageType.Data, catFootnotes, categoriesOnPage));
                }
                else
                {
                    // this is a TherapyGroup > therapy hierarchy
                    StringBuilder therapyHTML = new StringBuilder();
                    List<Guid> therapiesOnPage = new List<Guid>();
                    List<BDLinkedNote> therapyFootnotes = new List<BDLinkedNote>();
                    List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, child);
                    therapyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "h2", therapyFootnotes, therapiesOnPage));
                    therapyFootnotes.AddRange(footnotesFromMetadata);
                    string subtext = "given 30-60 minutes before the procedure";
                    therapyHTML.AppendFormat("<table><tr><th>{0}{1}</th><th>{2} {3}{4}/ROUTE</th><th>{5}{6}{7}/ROUTE</th></tr>",
                        labelsFromMetadata[1], buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, therapyFootnotes, objectsOnPage),
                        labelsFromMetadata[2], subtext, buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, therapyFootnotes, objectsOnPage),
                        labelsFromMetadata[3], subtext, buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[3]), true, therapyFootnotes, objectsOnPage));
                    foreach (IBDNode t in therapies)
                    {
                        BDTherapy therapy = t as BDTherapy;
                        therapyHTML.AppendFormat(buildTherapyWithTwoDosagesHtml(pContext, therapy, false, therapyFootnotes, therapiesOnPage));
                    }
                    therapyHTML.Append("</table>");
                    currentPageMasterObject = child;
                    childPages.Add(writeBDHtmlPage(pContext, child, therapyHTML, BDConstants.BDHtmlPageType.Data, therapyFootnotes, therapiesOnPage));
                }
            }
            for (int i = 0; i < childPages.Count; i++)
            {
                if (childNodes[i].LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_IERecommendation)
                    bodyHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", childPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
                else
                    therapyGroupHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", childPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
            }
            bodyHTML.Append(therapyGroupHTML);
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }

        private BDHtmlPage generatePageForProphylaxisFluidExposureRiskOfInfection(Entities pContext, IBDNode pNode)
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            List<string> labelsFromMetadata = new List<string>();
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02));
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode entry in childNodes)
            {
                bodyHTML.AppendFormat("<h2>{0}</h2>", retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Name_fieldNote", objectsOnPage, footnoteList));
                bodyHTML.AppendFormat("<table><tr><th>{0}</th></tr>", labelsFromMetadata[1]);
                bodyHTML.AppendFormat("<tr><td>{0}</td></tr></table>",
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field01_fieldNote", objectsOnPage, footnoteList));
                bodyHTML.AppendFormat("<table><tr><th>{0}</th></tr>", labelsFromMetadata[2]);
                bodyHTML.AppendFormat("<tr><td>{0}</td></tr></table>",
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field02_fieldNote", objectsOnPage, footnoteList));
                objectsOnPage.Add(entry.Uuid);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            List<string> labelsFromMetadata = new List<string>();
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02));
            
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

                bodyHTML.AppendFormat("<table><tr><th>{0}</th><th>{1}</th></tr>", labelsFromMetadata[1], labelsFromMetadata[2]);
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode entry in childNodes)
            {
                bodyHTML.AppendFormat("<tr><td>{0}{1}</td><td>{2}{3}</td></tr>",
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field01_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD01, objectsOnPage, true, footnoteList),
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD01, BDConstants.LinkedNoteType.Footnote), true, footnoteList, objectsOnPage),
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field02_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD02, objectsOnPage, true, footnoteList),
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD02, BDConstants.LinkedNoteType.Footnote), true, footnoteList, objectsOnPage));
                objectsOnPage.Add(entry.Uuid);
            }
            bodyHTML.Append("</table>");
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            List<string> labelsFromMetadata = new List<string>();
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03));
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            List<string> footnoteMarkersFromMetadata = new List<string>();
            footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnoteList, objectsOnPage));
            footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnoteList, objectsOnPage));
            footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, footnoteList, objectsOnPage));
            footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[3]), true, footnoteList, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode entry in childNodes)
            {
                bodyHTML.AppendFormat(@"<h4>SOURCE:  {0}</h4>", retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Name_fieldNote", BDConfiguredEntry.PROPERTYNAME_NAME, objectsOnPage, true, footnoteList));
                bodyHTML.AppendFormat(@"<table><tr colspan=3><th>RECIPIENT{0}</th></tr>",footnoteMarkersFromMetadata[0]);
                bodyHTML.AppendFormat("<tr><th>{0}{1}</th><th>{2}{3}</th><th>{4}{5}</th></tr>",
                    labelsFromMetadata[1], footnoteMarkersFromMetadata[1], labelsFromMetadata[2], footnoteMarkersFromMetadata[2], labelsFromMetadata[3], footnoteMarkersFromMetadata[3]);
                bodyHTML.AppendFormat("<tr><td>{0}{1}</td><td>{2}{3}</td><td>{4}{5}</td></tr>",
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field01_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD01, objectsOnPage, true, footnoteList), 
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD01, BDConstants.LinkedNoteType.Footnote), true, footnoteList, objectsOnPage),
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field02_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD02, objectsOnPage, true, footnoteList), 
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD02, BDConstants.LinkedNoteType.Footnote), true, footnoteList, objectsOnPage),
                    retrieveNoteTextForConfiguredEntryField(pContext, entry.Uuid, "Field03_fieldNote", BDConfiguredEntry.PROPERTYNAME_FIELD03, objectsOnPage, true, footnoteList), 
                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, entry.Uuid, BDConfiguredEntry.PROPERTYNAME_FIELD03, BDConstants.LinkedNoteType.Footnote), true, footnoteList, objectsOnPage));

                bodyHTML.Append("</table>");
                objectsOnPage.Add(entry.Uuid);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                foreach (IBDNode child in childNodes)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child, "h2", footnoteList, objectsOnPage));
                    bodyHTML.Append("<table>");
                    List<IBDNode> tableChildren = BDFabrik.GetChildrenForParent(pContext, child);
                    foreach (IBDNode tableChild in tableChildren)
                    {
                        if (tableChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                            bodyHTML.Append(buildTableRowHtml(pContext, tableChild as BDTableRow, true, true, footnoteList, objectsOnPage));
                        else if (tableChild.NodeType == BDConstants.BDNodeType.BDTableSection)
                        {
                            if (tableChild.Name.Length > 0)
                                bodyHTML.AppendFormat("<tr><td colspan=3>{0}</td></tr>", tableChild.Name);
                            List<IBDNode> tableRows = BDFabrik.GetChildrenForParent(pContext, tableChild);
                            foreach(IBDNode row in tableRows)
                                bodyHTML.Append(buildTableRowHtml(pContext, row as BDTableRow, false, true, footnoteList, objectsOnPage));
                        }
                    }
                    bodyHTML.Append("</table>");
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }

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
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
           foreach(IBDNode child in childNodes)
            {
                bool isAlternateLayout = false;

                if (child.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza && child.NodeType == BDConstants.BDNodeType.BDTable)
                    isAlternateLayout = true;

                bodyHTML.Append(buildNodeWithLinkedNotesAsFootnotesHTML(pContext, child, "h3", footnoteList, objectsOnPage));

                StringBuilder tableHTML = new StringBuilder();
               List<IBDNode> l2Children = BDFabrik.GetChildrenForParent(pContext, child);
               bool isFirstChild = true;
               bool isFirstConfiguredEntry = true;
                foreach (IBDNode l2Child in l2Children) // lvl2 (l2) can be topic or table.
                {
                    if (isFirstChild)
                    {
                        metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, l2Child.LayoutVariant);
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
                            tableHTML.AppendFormat("<tr><td rowspan=4>{0}{1}</td><td>{2}{3}</td><td>{4}{5}</td></tr>", 
                                cEntry.Name, 
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                cEntry.entryTitle01, 
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                cEntry.entryDetail01,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY01, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                            tableHTML.AppendFormat("<tr><td>{0}{1}</td><td>{2}{3}</td></tr>", 
                                cEntry.entryTitle02,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                cEntry.entryDetail02,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY02, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                            tableHTML.AppendFormat("<tr><td>{0}{1}</td><td>{2}{3}</td></tr>", 
                                cEntry.entryTitle03, 
                                 buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                cEntry.entryDetail03,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY03, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                            tableHTML.AppendFormat("<tr><td>{0}{1}</td><td>{2}{3}</td></tr>", 
                                cEntry.entryTitle04, 
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                cEntry.entryDetail04,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY04, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                        }
                        else if (l2Child.LayoutVariant == BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza_Amantadine_NoRenal)
                        {
                            bodyHTML.AppendFormat("<h4>{0}</h4>",cEntry.Name);
                            tableHTML.AppendFormat("<tr><td>{0}{1}</td><td colspan=3>{2}{3}</td></tr>",
                                cEntry.entryTitle01,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                cEntry.entryDetail01,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY01, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                            tableHTML.AppendFormat("<tr><td>{0}{1}</td><td colspan=3>{2}{3}</td></tr>",
                                cEntry.entryTitle02,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                cEntry.entryDetail02,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY02, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                            tableHTML.AppendFormat("<tr><td>{0}{1}</td><td colspan=3>{2}{3}</td></tr>",
                                cEntry.entryTitle03,
                                 buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                cEntry.entryDetail03,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY03, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                        }
                        else
                        {
                            writeCellsToRow = true;
                            if (isAlternateLayout && !string.IsNullOrEmpty(cEntry.groupTitle))
                                tableHTML.AppendFormat("<tr><td colspan={0}><b>{1}{2}</b> </td></tr>", metadataLayoutColumns.Count, cEntry.groupTitle,
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_GROUPTITLE, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                            else if (!string.IsNullOrEmpty(cEntry.groupTitle)) cell0HTML.AppendFormat("<u>{0}{1}</u><br>", cEntry.groupTitle,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_GROUPTITLE, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.Name))
                                cell0HTML.AppendFormat("<b>{0}{1}</b><br>{2}<br>", cEntry.Name,
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                    BDUtilities.GetEnumDescription(cEntry.GroupJoinType));

                            if (!string.IsNullOrEmpty(cEntry.entryTitle01)) cell1HTML.AppendFormat("<u>{0}{1}</u><br>", cEntry.entryTitle01,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.entryDetail01))
                                cell1HTML.AppendFormat("{0}{1}<br><b>{2}</b><br>", cEntry.entryDetail01,
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY01, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                    BDUtilities.GetEnumDescription(cEntry.JoinType01));

                            if (!string.IsNullOrEmpty(cEntry.entryTitle02)) cell1HTML.AppendFormat("<br><u>{0}{1}</u><br>", cEntry.entryTitle02,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.entryDetail02))
                                cell1HTML.AppendFormat("{0}{1}<br><b>{2}</b><br>", cEntry.entryDetail02,
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY02, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                    BDUtilities.GetEnumDescription(cEntry.JoinType02));

                            if (!string.IsNullOrEmpty(cEntry.entryTitle03)) cell2HTML.AppendFormat("<u>{0}{1}</u><br>", cEntry.entryTitle03,
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.entryDetail03))
                                cell2HTML.AppendFormat("{0}{1}<br><b>{2}</b><br>", cEntry.entryDetail03,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY03, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                BDUtilities.GetEnumDescription(cEntry.JoinType03));

                            if (!string.IsNullOrEmpty(cEntry.entryTitle04)) cell2HTML.AppendFormat("<br><u>{0}{1}</u><br>", cEntry.entryTitle04,
                                buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));

                            if (!string.IsNullOrEmpty(cEntry.entryDetail04))
                                cell2HTML.AppendFormat("{0}{1}<br><b>{2}</b><br>", cEntry.entryDetail04,
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDCombinedEntry.PROPERTYNAME_ENTRY04, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
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
                                    // handle configured entry for Amantadine with No renal impairment
                                    tableHTML.Append("</table><h4>Renal Impairment</h4><table>");
                                    tableHTML.AppendFormat("<tr><th rowspan=2>{0}</th><th colspan=2>Dosage with capsules</th><th>Daily dosage with solution (10mg/mL)</th></tr>", layoutColumns[0]);
                                    tableHTML.AppendFormat("<tr><th>{0}{1}</th><th>{2}{3}</th><th>{4}{5}</th></tr>", 
                                        layoutColumns[1],
                                        buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext,layoutColumns[1]),true,footnoteList,objectsOnPage),
                                        layoutColumns[2],
                                        buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, layoutColumns[2]), true, footnoteList, objectsOnPage),
                                        layoutColumns[3],
                                        buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext,layoutColumns[3]),true,footnoteList,objectsOnPage));
                                    isFirstConfiguredEntry = false;
                                }
                                tableHTML.AppendFormat("<tr><td>{0}{1}</td><td><{2}{3}</td><td>{4}{5}</td><td>{6}{7}</td></tr>", 
                                    cEntry.Name, 
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid, BDConfiguredEntry.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                    cEntry.field01, 
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid,BDConfiguredEntry.PROPERTYNAME_FIELD01, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                    cEntry.field02, 
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid,BDConfiguredEntry.PROPERTYNAME_FIELD02, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage),
                                    cEntry.field03,
                                    buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, cEntry.Uuid,BDConfiguredEntry.PROPERTYNAME_FIELD03, BDConstants.LinkedNoteType.MarkedComment), true, footnoteList, objectsOnPage));
                            }
                            objectsOnPage.Add(tChild.Uuid);
                        }
                    }
                    objectsOnPage.Add(l2Child.Uuid);
                }
                    bodyHTML.Append(tableHTML);
                bodyHTML.Append("</table>");
           }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnoteList, objectsOnPage));

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
                            buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, precaution.Uuid, BDPrecaution.PROPERTYNAME_ORGANISM_1, BDConstants.LinkedNoteType.Footnote), true, footnoteList, objectsOnPage));
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
                    mPages.Add(writeBDHtmlPage(pContext, microorganism, mHTML, BDConstants.BDHtmlPageType.Data, mFootnotes, mObjectsOnPage));
                }
                for (int i = 0; i < mPages.Count; i++)
                    mgHTML.AppendFormat(@"<p><a href=""{0}"">{1}</a></p>", mPages[i].Uuid.ToString().ToUpper(), mgTitles[i]);
            }
            bodyHTML.Append(mgHTML);
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                List<string> labels = new List<string>();
                // build markers and list for column header linked notes
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01));
                labels.Add( retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02));
                labels.Add( retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03));
                labels.Add( retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD04));
                labels.Add( retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD05));
                labels.Add( retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD06));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD07));

                List<string> footerMarkers = new List<string>();
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[3]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[4]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[5]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[6]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[7]), true, footnotes, objectsOnPage));
                bodyHTML.Append("<table><tr>");
                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}{1}</th>", labels[i], footerMarkers[i]);
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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotes, objectsOnPage));

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
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, table, "h3", footnotes, objectsOnPage));
                List<string> labels = new List<string>();
                // build markers and list for column header linked notes
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1));
                List<string> footerMarkers = new List<string>();
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, footnotes, objectsOnPage));

                bodyHTML.Append("<table><tr>");

                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}{1}</th>", labels[i], footerMarkers[i]);
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
                        string marker = buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, t.Uuid, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment), true, footnotes, objectsOnPage);
                        bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}{2}</td><td>{3}</td></tr>",
                            t.Name, t.dosage, marker, t.dosage1);
                        objectsOnPage.Add(t.Uuid);
                    }
                }
                bodyHTML.Append("</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));
            
                List<IBDNode> surgeries = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode surgery in surgeries)
                {
                    List<string> labels = new List<string>();
                    // build markers and list for column header linked notes
                    labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDSurgery, BDNode.PROPERTYNAME_NAME));
                    List<string> footerMarkers = new List<string>();
                    footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnotes, objectsOnPage));

                    bodyHTML.AppendFormat("{0}{1}", buildNodeWithReferenceAndOverviewHTML(pContext, surgery, "h3", footnotes, objectsOnPage), footerMarkers[0]);

                    List<IBDNode> surgeryChildren = BDFabrik.GetChildrenForParent(pContext, surgery); 
                    foreach (IBDNode surgeryChild in surgeryChildren)
                    {
                        // surgery classification - owns the child row
                        if (surgeryChild.Name.Length > 0 && !surgeryChild.Name.Contains(BDUtilities.GetEnumDescription(surgeryChild.NodeType)))
                            bodyHTML.AppendFormat("<ul><li>{0}</ul>", surgeryChild.Name);
                        objectsOnPage.Add(surgeryChild.Uuid);
                        // build markers and list for column header linked notes
                        labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE));
                        labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1));

                        footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnotes, objectsOnPage));
                        footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, footnotes, objectsOnPage));

                        List<IBDNode> tGroups = BDFabrik.GetChildrenForParent(pContext, surgeryChild);
                        if (tGroups.Count > 0)
                        {
                            StringBuilder adultDosageHTML = new StringBuilder();
                            StringBuilder pedsDosageHTML = new StringBuilder();
                            bodyHTML.Append(@"<table>");
                            bodyHTML.AppendFormat(@"<tr><th>{0}</th><th>{1}</th></tr>", labels[1], labels[2]);
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
                                        adultDosageHTML.AppendFormat("<li>{0}", buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, false, footnotes, objectsOnPage));
                                        pedsDosageHTML.AppendFormat("<li>{0}", buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, false, footnotes, objectsOnPage));
                                    }
                                    else
                                    {
                                        adultDosageHTML.AppendFormat("<li>{0}", buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, false, footnotes, objectsOnPage));
                                        pedsDosageHTML.AppendFormat("<li>{0}", buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY, false, footnotes, objectsOnPage));
                                    }
                                    // Dosage - adult dose
                                    if (therapy.dosageSameAsPrevious.Value == true)
                                        adultDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, false, footnotes, objectsOnPage));
                                    else
                                        adultDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, false, footnotes, objectsOnPage));

                                    // Dosage 1 - Paediatric dose
                                    if (therapy.dosage1SameAsPrevious.Value == true)
                                        pedsDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, previousTherapyId, previousTherapyDosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, false, footnotes, objectsOnPage));
                                    else
                                        pedsDosageHTML.Append(buildNodePropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, false, footnotes, objectsOnPage));

                                    // check for conjunctions and add a row for any that are found
                                    switch (therapy.therapyJoinType)
                                    {
                                        case (int)BDTherapy.TherapyJoinType.AndWithNext:
                                            adultDosageHTML.Append(@" + ");
                                            pedsDosageHTML.Append(@" + ");
                                            break;
                                        case (int)BDTherapy.TherapyJoinType.OrWithNext:
                                            adultDosageHTML.Append(@"<br>or<br>");
                                            pedsDosageHTML.Append(@"<br>or<br>");
                                            break;
                                        case (int)BDTherapy.TherapyJoinType.ThenWithNext:
                                            adultDosageHTML.Append(@" then ");
                                            pedsDosageHTML.Append(@" then ");
                                            break;
                                        case (int)BDTherapy.TherapyJoinType.WithOrWithoutWithNext:
                                            adultDosageHTML.Append(@" +/- ");
                                            pedsDosageHTML.Append(@" +/- ");
                                            break;
                                        default:
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
                return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode subcategory in childNodes)
            {
                bodyHTML.Append("<p>");
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, "h4", footnotes, objectsOnPage));
                List<IBDNode> mGroups = BDFabrik.GetChildrenForParent(pContext, subcategory);
                foreach (IBDNode group in mGroups)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, group, "b", footnotes, objectsOnPage));
                    bodyHTML.Append("<br>");
                    List<IBDNode> microorganisms = BDFabrik.GetChildrenForParent(pContext, group);
                    foreach (IBDNode microorganism in microorganisms)
                    {
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, microorganism, "", footnotes, objectsOnPage));
                        bodyHTML.Append("<br>");
                    }
                }
                bodyHTML.Append("</p>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }
        #endregion

        #region Pregnancy/Lactation Sections

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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", footnotes, objectsOnPage));
            List<string> labels = new List<string>();
            // build markers and list for column header linked notes
            labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK));
            labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_APPRATING));
            labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE));

            List<string> footerMarkers = new List<string>();
            List<BDLinkedNote> footersFromMetadata = new List<BDLinkedNote>();
            footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footersFromMetadata, objectsOnPage));
            footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, footersFromMetadata, objectsOnPage));
            footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[3]), true, footersFromMetadata, objectsOnPage));

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
                        amPages.AddRange(buildAntimicrobialWithRiskHTML(pContext, labels, footerMarkers, footersFromMetadata, antimicrobials));
                        for (int i = 0; i < amPages.Count; i++)
                            apHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", amPages[i].Uuid.ToString().ToUpper(), antimicrobials[i].Name);
                            subcatHTML.Append(apHTML);
                    }
                    for (int i = 0; i < apPages.Count; i++)
                        subcatHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", apPages[i].Uuid.ToString().ToUpper(), subcategoryChildNodes[i].Name);
                    if (subcategory.Name.Length > 0)
                    {
                        currentPageMasterObject = subcategory;
                        subcatPages.Add(writeBDHtmlPage(pContext, subcategory, subcatHTML, BDConstants.BDHtmlPageType.Navigation, subcatFootnotes, subcatObjectsOnPage));
                    }
                    else
                        bodyHTML.Append(subcatHTML);
                }
                else
                { // antimicrobial group does not exist - the child nodes are antimicrobials
                    List<BDHtmlPage> amPages = new List<BDHtmlPage>();
                    amPages.AddRange(buildAntimicrobialWithRiskHTML(pContext, labels, footerMarkers, footersFromMetadata, subcategoryChildNodes));
                    for (int i = 0; i < amPages.Count; i++)
                        apHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", amPages[i].Uuid.ToString().ToUpper(), subcategoryChildNodes[i].Name);
                    if (subcategory.Name.Length > 0)
                    {
                        currentPageMasterObject = subcategory;
                        apPages.Add(writeBDHtmlPage(pContext, subcategory, apHTML, BDConstants.BDHtmlPageType.Navigation, apFootnotes, apObjectsOnPage));
                    }
                    else
                        subcatHTML.Append(apHTML);
                }
            }
            for (int i = 0; i < subcatPages.Count; i++)
                bodyHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", subcatPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

        private BDHtmlPage generatePageForPLAntimicrobialsInPregnancy(Entities pContext, IBDNode pNode)
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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

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

                // build markers and list for column header linked notes
                List<string> labelsFromMetadata = new List<string>();
                labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK));
                labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION));

                List<string> footnoteMarkersFromMetadata = new List<string>();
                List<BDLinkedNote> footnotesFromMetadata = new List<BDLinkedNote>();
                footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnotesFromMetadata, objectsOnPage));
                footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, footnotesFromMetadata, objectsOnPage));

                foreach (IBDNode antimicrobial in antimicrobials)
                {
                    // write an HTML page for the antimicrobial, build a link for the name
                    StringBuilder antimicrobialHTMLBody = new StringBuilder();
                    List<Guid> antimicrobialsOnPage = new List<Guid>();
                    antimicrobialHTMLBody.Append(buildNodeWithReferenceAndOverviewHTML(pContext, antimicrobial as BDNode, "h4", amFootnotes, antimicrobialsOnPage));
                    
                    footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(footnotesFromMetadata, true, amFootnotes, antimicrobialsOnPage));

                    List<IBDNode> amRisks = BDFabrik.GetChildrenForParent(pContext, antimicrobial);
                    foreach (IBDNode amRisk in amRisks)
                    {
                        BDAntimicrobialRisk risk = amRisk as BDAntimicrobialRisk;
                        if (risk.riskFactor.Length > 0)
                            antimicrobialHTMLBody.AppendFormat("<b>{0}{1}</b>: {2}", labelsFromMetadata[0], footnoteMarkersFromMetadata[0], risk.riskFactor);
                        if (risk.recommendations.Length > 0)
                            antimicrobialHTMLBody.AppendFormat("<p><b>{0}{1}</b><br>{2}</p>", labelsFromMetadata[1], footnoteMarkersFromMetadata[1], risk.recommendations);
                        antimicrobialsOnPage.Add(amRisk.Uuid);
                        amFootnotes.AddRange(footnotes);

                        amFootnotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, amRisk.Uuid, BDAntimicrobialRisk.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));
                        amFootnotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, amRisk.Uuid, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, BDConstants.LinkedNoteType.Footnote));
                        amFootnotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, amRisk.Uuid, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, BDConstants.LinkedNoteType.Footnote));
                        amFootnotes.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, amRisk.Uuid, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, BDConstants.LinkedNoteType.Footnote));
                    }
                    string commentText = buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, antimicrobial, BDConstants.LinkedNoteType.UnmarkedComment, antimicrobialsOnPage);
                    if (commentText.Length > 0)
                        antimicrobialHTMLBody.AppendFormat("<p><b>Comments</b><br>{0}</p>", commentText);
                    currentPageMasterObject = antimicrobial;
                    amPages.Add(writeBDHtmlPage(pContext, pNode, antimicrobialHTMLBody, BDConstants.BDHtmlPageType.Data, amFootnotes, antimicrobialsOnPage));
                }
                for (int i = 0; i < amPages.Count; i++)
                    subcatHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", amPages[i].Uuid.ToString().ToUpper(), antimicrobials[i].Name);
                currentPageMasterObject = subcategory;
                subcatPages.Add(writeBDHtmlPage(pContext, pNode, subcatHTML, BDConstants.BDHtmlPageType.Navigation, subcatFootnotes, subcatObjectsOnPage));

            }
            for(int i = 0; i < subcatPages.Count; i++)
                bodyHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", subcatPages[i].Uuid.ToString().ToUpper(), childNodes[i].Name);
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, pNode);
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));
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
                pages.Add(writeBDHtmlPage(pContext, pathogen, pathogenHTML, BDConstants.BDHtmlPageType.Data, pFootnotes, objectsOnPathogenPage));
            }
            for (int i = 0; i < pages.Count; i++)
                bodyHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", pages[i].Uuid.ToString().ToUpper(), pathogens[i].Name);
            
            currentPageMasterObject = pNode;
            return writeBDHtmlPage(pContext, pNode as BDNode, bodyHTML, BDConstants.BDHtmlPageType.Navigation, footnotes, objectsOnPage);
        }

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            List<string> labelsFromMetadata = new List<string>();
            // build markers and list for column header linked notes
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY));
            labelsFromMetadata.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE));

            List<string> footnoteMarkersFromMetadata = new List<string>();
            List<BDLinkedNote> footnotesFromMetadata = new List<BDLinkedNote>();
            footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnotesFromMetadata, objectsOnPage));
            footnoteMarkersFromMetadata.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnotesFromMetadata, objectsOnPage));

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            bodyHTML.Append("<table><tr>");
            for (int i = 0; i < metadataLayoutColumns.Count; i++)
                bodyHTML.AppendFormat("<th>{0}{1}</th>", labelsFromMetadata[i], footnoteMarkersFromMetadata[i]);
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
                        bodyHTML.Append(buildTherapyWithCombinedColumnHtml(pContext, therapy, footnotes, objectsOnPage));

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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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
            List<BDLinkedNote> footnoteList = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnoteList, objectsOnPage));

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnoteList, objectsOnPage);
        }
        
        #endregion

        #region Microbiology/Organisms Sections

        private BDHtmlPage generatePageForMicrobiologyGramStain(Entities pContext, IBDNode pNode)
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
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            // gram positive or gram negative
            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0) // subcategory - column 1 values
            {
                List<string> labels = new List<string>();
                // build markers and list for column header linked notes
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDSubcategory, BDNode.PROPERTYNAME_NAME));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDMicroorganism, BDNode.PROPERTYNAME_NAME));

                List<string> footerMarkers = new List<string>();
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnotes, objectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnotes, objectsOnPage));
                
                bodyHTML.Append("<table><tr>");
                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}{1}</th>", labels[i], footerMarkers[i]);
                bodyHTML.Append("</tr>");
                
                foreach (IBDNode child in childNodes)
                {
                    BDNode subcategory = child as BDNode;
                    List<IBDNode> mos = BDFabrik.GetChildrenForParent(pContext, subcategory);
                    StringBuilder mString = new StringBuilder();
                    foreach (IBDNode microorganism in mos)
                    {
                        mString.AppendFormat("{0}<br>", microorganism.Name);
                        objectsOnPage.Add(microorganism.Uuid);
                    }
                    bodyHTML.AppendFormat("<tr><td><ul><li>{0}</ul></td><td>{1}</td></tr>", subcategory.Name, mString);
                    objectsOnPage.Add(child.Uuid);
                }
                bodyHTML.Append("</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

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
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode subcategory in childNodes)
            {
                // microorganism group
                bodyHTML.Append(buildNodeWithLinkedNotesAsFootnotesHTML(pContext, subcategory as BDNode, "h2", footnotes, objectsOnPage));


                List<IBDNode> mGroups = BDFabrik.GetChildrenForParent(pContext, subcategory);
                foreach (IBDNode mGroup in mGroups)
                {
                    bodyHTML.Append(buildNodeWithLinkedNotesAsFootnotesHTML(pContext, mGroup as BDNode, "h4", footnotes, objectsOnPage));

                    List<IBDNode> microorganisms = BDFabrik.GetChildrenForParent(pContext, mGroup);
                    foreach (IBDNode microorganism in microorganisms)
                        bodyHTML.AppendFormat("{0}<br>", buildNodeWithLinkedNotesAsFootnotesHTML(pContext, microorganism as BDNode, "", footnotes, objectsOnPage));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footnotes, objectsOnPage);
        }

        #endregion

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

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, objectsOnPage));

            bodyHTML.Append(buildAttachmentHTML(pContext, pNode, footnotes, objectsOnPage));
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data,footnotes, objectsOnPage);
        }

        #region Standalone HTML pages
        private BDHtmlPage generatePageForOverview(Entities pContext, IBDNode pNode)
        {
            List<Guid> objectsOnPage = new List<Guid>();
            BDNode parentNode = pNode as BDNode;
            string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid, objectsOnPage);
            if (null != parentNode && noteText.Length > EMPTY_PARAGRAPH)
            {
                return writeBDHtmlPage(pContext, parentNode, noteText, BDConstants.BDHtmlPageType.Overview, new List<BDLinkedNote>(), objectsOnPage);
            }
            return null;
        }

        private BDHtmlPage generatePageForLinkedNotes(Entities pContext, Guid pParentId, BDConstants.BDNodeType pParentType, List<BDLinkedNote> pInlineNotes, List<BDLinkedNote> pMarkedNotes, List<BDLinkedNote> pUnmarkedNotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder noteHtml = new StringBuilder();

            if (notesListHasContent(pContext, pInlineNotes))
            {
                foreach (BDLinkedNote iNote in pInlineNotes)
                {
                    if (iNote.documentText.Length > EMPTY_PARAGRAPH)
                    {
                        if (noteHtml.ToString().Length > EMPTY_PARAGRAPH)
                            noteHtml.Append(@"<br>");
                        noteHtml.Append(iNote.documentText);
                        pObjectsOnPage.Add(iNote.Uuid);
                    }
                }
            }

            if (notesListHasContent(pContext, pMarkedNotes))
            {
                foreach (BDLinkedNote mNote in pMarkedNotes)
                {
                    if (mNote.documentText.Length > EMPTY_PARAGRAPH)
                    {
                        if (noteHtml.ToString().Length > EMPTY_PARAGRAPH)
                            noteHtml.Append(@"<br>");
                        noteHtml.Append(mNote.documentText);
                        pObjectsOnPage.Add(mNote.Uuid);
                    }
                }
            }

            if (notesListHasContent(pContext, pUnmarkedNotes))
            {
                foreach (BDLinkedNote uNote in pUnmarkedNotes)
                {
                    if (uNote.documentText.Length > EMPTY_PARAGRAPH)
                    {
                        if (noteHtml.ToString().Length > EMPTY_PARAGRAPH)
                            noteHtml.Append(@"<br>");
                        noteHtml.Append(uNote.documentText);
                        pObjectsOnPage.Add(uNote.Uuid);
                    }
                }
            }

            return generatePageForLinkedNotes(pContext, pParentId, pParentType, noteHtml.ToString(), BDConstants.BDHtmlPageType.Comments,pObjectsOnPage);
        }

        private BDHtmlPage generatePageForLinkedNotes(Entities pContext, Guid pDisplayParentId, BDConstants.BDNodeType pDisplayParentType, string pPageHtml, BDConstants.BDHtmlPageType pPageType, List<Guid> pObjectsOnPage)
        {
            if (pPageHtml.Length > EMPTY_PARAGRAPH)
            {
                // the linked note being processed will have a parent that is a BDNode OR another linked note
                IBDNode node = BDFabrik.RetrieveNode(pContext, pDisplayParentId);
                BDLinkedNote linkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, pDisplayParentId);
                if (node != null)
                {
                    currentPageMasterObject = node;
                    return writeBDHtmlPage(pContext, node, pPageHtml, pPageType, new List<BDLinkedNote>(), pObjectsOnPage);
                }
                else if (linkedNote != null)
                {
                    currentPageMasterObject = linkedNote;
                    return writeBDHtmlPage(pContext, linkedNote, pPageHtml, pPageType, new List<BDLinkedNote>(), pObjectsOnPage);
                }
                else
                    throw new NotSupportedException();
            }
            else
                return null;
        }

        /// <summary>
        /// Create an HTML page for the footnote attached to a node & property
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNotePropertyName"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns>Guid of HTML page.</returns>
        private BDHtmlPage generatePageForParentAndPropertyFootnote(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            List<Guid> objectsOnPage = new List<Guid>();
            string footnoteText = buildTextForParentAndPropertyFromLinkedNotes(pContext, pPropertyName, pNode, BDConstants.LinkedNoteType.Footnote, objectsOnPage);
            BDHtmlPage footnote = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, footnoteText, BDConstants.BDHtmlPageType.Footnote, objectsOnPage);
            foreach(Guid id in objectsOnPage)
                pagesMap.Add(new BDHtmlPageMap(footnote.Uuid, id));
            return footnote;
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
                BDHtmlPage footnote = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, referenceText.ToString(), BDConstants.BDHtmlPageType.Reference, objectsOnPage);
               
                List<Guid> filteredObjects = objectsOnPage.Distinct().ToList();
                foreach (Guid id in filteredObjects)
                    pagesMap.Add(new BDHtmlPageMap(footnote.Uuid, id));

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
        private StringBuilder buildEmpiricTherapyHTML(Entities pContext, BDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
             StringBuilder bodyHtml = new StringBuilder();
                bodyHtml.Append(buildPathogenGroupHtml(pContext, pNode, pFootnotes, pObjectsOnPage));

                // process therapy groups
                List<BDTherapyGroup> therapyGroups = BDTherapyGroup.RetrieveTherapyGroupsForParentId(pContext, pNode.Uuid);

                foreach (BDTherapyGroup tGroup in therapyGroups)
                {
                    BDTherapyGroup group = tGroup as BDTherapyGroup;
                    if (null != group)
                    {
                        bodyHtml.Append(buildNodeWithLinkedNotesInlineHtml(pContext, group, pFootnotes, pObjectsOnPage));
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
                if (!therapiesHaveDosage && !therapiesHaveDuration)
                    therapyGroupHtml.Append(@"<tr><th>Recommended Empiric Therapy</th>");
                else
                    therapyGroupHtml.Append(@"<tr><th>Recommended<br>Empiric<br>Therapy</th>");
                if (therapiesHaveDosage)
                    therapyGroupHtml.Append(@"<th>Recommended<br>Dose</th>");
                else
                    therapyGroupHtml.Append(@"<th></th>");
                if (therapiesHaveDuration)
                    therapyGroupHtml.Append(@"<th>Recommended<br>Duration</th>");
                else
                    therapyGroupHtml.Append(@"<th></th>");

                therapyGroupHtml.Append(@"</tr>");

                therapyGroupHtml.Append(therapyHTML);
                therapyGroupHtml.Append(@"</table>");
            }
            return therapyGroupHtml;
        }

        /// <summary>
        /// Generate HTML for Pathogen Group and Pathogen
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns></returns>
        private string buildPathogenGroupHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder pathogenGroupHtml = new StringBuilder();

            BDNode pathogenGroup = pNode as BDNode;
            if (null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
            {
                // Get overview for Pathogen Group
                pathogenGroupHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2", pFootnotes, pObjectsOnPage));

                List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

                pathogenGroupHtml.Append(buildTextFromNotes(inlineNotes, pObjectsOnPage));
                pathogenGroupHtml.Append(buildTextFromNotes(markedNotes, pObjectsOnPage));
                pathogenGroupHtml.Append(buildTextFromNotes(unmarkedNotes, pObjectsOnPage));

                List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, pathogenGroup);
                if (pathogens.Count > 0)
                    pathogenGroupHtml.Append(@"<h3>Usual Pathogens</h3>");

                foreach (IBDNode pathogen in pathogens)
                    if (pathogen.NodeType == BDConstants.BDNodeType.BDPathogen) // bypass the therapy groups that appear at this level
                        pathogenGroupHtml.AppendFormat("{0}<br>", (buildNodeWithReferenceAndOverviewHTML(pContext, pathogen, "", pFootnotes, pObjectsOnPage)));
            }
            return pathogenGroupHtml.ToString();
        }

        private string buildTherapyHtml(Entities pContext, BDTherapy pTherapy, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder therapyHtml = new StringBuilder();
            string styleString = string.Empty;

            // check join type - if none, then draw the bottom border on the table row
            if (pTherapy.therapyJoinType == (int)BDTherapy.TherapyJoinType.None)
                styleString = @"class=""d0""";  // row has bottom border
            else
                styleString = @"class=""d1""";  // NO bottom border

            therapyHtml.AppendFormat(@"<tr {0}><td>", styleString);

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(@"&#91");

            if (pTherapy.nameSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<b>{0}</b>", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, false, pFootnotes, pObjectsOnPage));
            else
                therapyHtml.AppendFormat("<b>{0}</b>", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.Name, BDTherapy.PROPERTYNAME_THERAPY, false, pFootnotes, pObjectsOnPage));

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(@"&#93");

            therapyHtml.Append(@"</td>");

            // Dosage
            if (pTherapy.dosageSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<td>{0}</td>", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, false, pFootnotes, pObjectsOnPage));
            else
                therapyHtml.AppendFormat("<td>{0}</td>", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, false, pFootnotes, pObjectsOnPage));

            // Duration
            if (pTherapy.durationSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<td>{0}</td>", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDuration, BDTherapy.PROPERTYNAME_DURATION, false, pFootnotes, pObjectsOnPage));
            else
                therapyHtml.AppendFormat("<td>{0}</td>", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration, BDTherapy.PROPERTYNAME_DURATION, false, pFootnotes, pObjectsOnPage));

            therapyHtml.Append(@"</tr>");

            // check for conjunctions and add a row for any that are found
            switch (pTherapy.therapyJoinType)
            {
                case (int)BDTherapy.TherapyJoinType.AndWithNext:
                    therapyHtml.Append(@"<tr><td> + </td><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.OrWithNext:
                    therapyHtml.Append(@"<tr><td> or</td><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.ThenWithNext:
                    therapyHtml.Append(@"<tr><td> then</td><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.WithOrWithoutWithNext:
                    therapyHtml.Append(@"<tr><td> +/-</td><td /><td /></tr>");
                    break;
                default:
                    break;
            }

            return therapyHtml.ToString();
        }

        private string buildTherapyWithTwoDosagesHtml(Entities pContext, BDTherapy pTherapy, bool includeDuration, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder therapyHtml = new StringBuilder();
            string styleString = string.Empty;

            // check join type - if none, then draw the bottom border on the table row
            if (pTherapy.therapyJoinType == (int)BDTherapy.TherapyJoinType.None)
                styleString = @"class=""d0""";
            else
                styleString = @"class=""d1""";

            therapyHtml.AppendFormat(@"<tr {0}>", styleString);

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(@"&#91");

            if (pTherapy.nameSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<td>{0}", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, false, "b", pFootnotes, pObjectsOnPage));
            else
                therapyHtml.AppendFormat("<td>{0}", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.Name, BDTherapy.PROPERTYNAME_THERAPY, false, "b", pFootnotes, pObjectsOnPage));

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(@"&#93");

            therapyHtml.Append(@"</td>");

            // Dosage
            if (pTherapy.dosageSameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, false, "td", pFootnotes, pObjectsOnPage));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, false, "td", pFootnotes, pObjectsOnPage));

            // Dosage 1
            if (pTherapy.dosage1SameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, false, "td", pFootnotes, pObjectsOnPage));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1, false, "td", pFootnotes, pObjectsOnPage));

            // Duration
            if (includeDuration)
            {
                if (pTherapy.durationSameAsPrevious.Value == true)
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDuration, BDTherapy.PROPERTYNAME_DURATION, false, "td", pFootnotes, pObjectsOnPage));
                else
                    therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration, BDTherapy.PROPERTYNAME_DURATION, false, "td", pFootnotes, pObjectsOnPage));
            }
            therapyHtml.Append(@"</tr>");

            // check for conjunctions and add a row for any that are found
            switch (pTherapy.therapyJoinType)
            {
                case (int)BDTherapy.TherapyJoinType.AndWithNext:
                    therapyHtml.Append(@"<tr><td> + </td><td /><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.OrWithNext:
                    therapyHtml.Append(@"<tr><td> or</td><td /><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.ThenWithNext:
                    therapyHtml.Append(@"<tr><td> then</td><td /><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.WithOrWithoutWithNext:
                    therapyHtml.Append(@"<tr><td> +/-</td><td /><td /><td /></tr>");
                    break;
                default:
                    break;
            }
            return therapyHtml.ToString();
        }

        private string buildTherapyWithTwoDurationsHtml(Entities pContext, BDTherapy pTherapy, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder therapyHtml = new StringBuilder();
            string styleString = string.Empty;

            // check join type - if NONE, then draw the bottom border on the table row
            if (pTherapy.therapyJoinType == (int)BDTherapy.TherapyJoinType.None)
                styleString = @"class=""d0""";
            else
                styleString = @"class=""d1""";

            therapyHtml.AppendFormat(@"<tr {0}><td>", styleString);

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(@"&#91");

            // Name
            if (pTherapy.nameSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<td>{0}", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, false, "b", pFootnotes, pObjectsOnPage));
            else
                therapyHtml.AppendFormat("<td>{0}", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.Name, BDTherapy.PROPERTYNAME_THERAPY, false, "b", pFootnotes, pObjectsOnPage));

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(@"&#93");

            therapyHtml.Append(@"</td>");

            // Dosage
            if (pTherapy.dosageSameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, false, "td", pFootnotes, pObjectsOnPage));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, false, "td", pFootnotes, pObjectsOnPage));

            // Duration
            if (pTherapy.durationSameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDuration, BDTherapy.PROPERTYNAME_DURATION, false, "td", pFootnotes, pObjectsOnPage));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration, BDTherapy.PROPERTYNAME_DURATION, false, "td", pFootnotes, pObjectsOnPage));
            
            // Duration 1
            if (pTherapy.duration1SameAsPrevious.Value == true)
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDuration1, BDTherapy.PROPERTYNAME_DURATION_1, false, "td", pFootnotes, pObjectsOnPage));
            else
                therapyHtml.Append(buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.duration1, BDTherapy.PROPERTYNAME_DURATION_1, false, "td", pFootnotes, pObjectsOnPage));

            therapyHtml.Append(@"</tr>");

            // check for conjunctions and add a row for any that are found
            switch (pTherapy.therapyJoinType)
            {
                case (int)BDTherapy.TherapyJoinType.AndWithNext:
                    therapyHtml.Append(@"<tr><td> + </td><td /><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.OrWithNext:
                    therapyHtml.Append(@"<tr><td> or</td><td /><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.ThenWithNext:
                    therapyHtml.Append(@"<tr><td> then</td><td /><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.WithOrWithoutWithNext:
                    therapyHtml.Append(@"<tr><td> +/-</td><td /><td /><td /></tr>");
                    break;
                default:
                    break;
            }
            return therapyHtml.ToString();
        }

        private string buildTherapyWithCombinedColumnHtml(Entities pContext, BDTherapy pTherapy, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder therapyHtml = new StringBuilder();
            string styleString = string.Empty;

            // check join type - if none, then draw the bottom border on the table row
            if (pTherapy.therapyJoinType == (int)BDTherapy.TherapyJoinType.None)
                styleString = @"class=""d0""";  // row has bottom border
            else
                styleString = @"class=""d1""";  // NO bottom border

            therapyHtml.AppendFormat(@"<tr {0}><td>", styleString);

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(@"&#91");

            if (pTherapy.nameSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<b>{0}</b>", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY, false, pFootnotes, pObjectsOnPage));
            else
                therapyHtml.AppendFormat("<b>{0}</b>", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.Name, BDTherapy.PROPERTYNAME_THERAPY, false, pFootnotes, pObjectsOnPage));

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(@"&#93");

            therapyHtml.Append(@"</td>");

            // Dosage + Duration are entered into the Dosage property
            if (pTherapy.dosageSameAsPrevious.Value == true)
                therapyHtml.AppendFormat("<td>{0}</td>", buildNodePropertyHTML(pContext, pTherapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE, false, pFootnotes, pObjectsOnPage));
            else
                therapyHtml.AppendFormat("<td>{0}</td>", buildNodePropertyHTML(pContext, pTherapy, pTherapy.Uuid, pTherapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE, false, pFootnotes, pObjectsOnPage));

            therapyHtml.Append(@"</tr>");

            // check for conjunctions and add a row for any that are found
            switch (pTherapy.therapyJoinType)
            {
                case (int)BDTherapy.TherapyJoinType.AndWithNext:
                    therapyHtml.Append(@"<tr><td> + </td><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.OrWithNext:
                    therapyHtml.Append(@"<tr><td> or</td><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.ThenWithNext:
                    therapyHtml.Append(@"<tr><td> then</td><td /><td /></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.WithOrWithoutWithNext:
                    therapyHtml.Append(@"<tr><td> +/-</td><td /><td /></tr>");
                    break;
                default:
                    break;
            }

            return therapyHtml.ToString();
        }

        /// <summary>
        /// Generate table row HTML for BDDosage
        /// Includes generation of footer markers, and addition of footers to list
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns></returns>
        private string buildDosageWithCostHTML(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            BDDosage dosageNode = pNode as BDDosage;
            StringBuilder dosageHTML = new StringBuilder();
            string styleString = string.Empty;

            dosageHTML.AppendFormat("<tr>{0}", buildNodePropertyHTML(pContext, dosageNode, dosageNode.Uuid, dosageNode.dosage, BDDosage.PROPERTYNAME_DOSAGE, false, "td", pFootnotes, pObjectsOnPage));
            dosageHTML.Append(buildNodePropertyHTML(pContext, dosageNode, dosageNode.Uuid, dosageNode.cost, BDDosage.PROPERTYNAME_COST, false, "td", pFootnotes, pObjectsOnPage));
            dosageHTML.Append("</tr>");
            return dosageHTML.ToString();
        }

        private string buildCellHTML(Entities pContext, IBDNode pCellParentNode, string pPropertyName, string pPropertyValue, bool includeCellTags, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            string cellTag = includeCellTags == true ? "td" : string.Empty;

            return buildNodePropertyHTML(pContext, pCellParentNode, pCellParentNode.Uuid, pPropertyValue, pPropertyName, false, cellTag, pFootnotes, pObjectsOnPage);
        }

        private string buildDosageHTML(Entities pContext, IBDNode pNode, string pDosageGroupName, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            BDDosage dosageNode = pNode as BDDosage;
            StringBuilder dosageHTML = new StringBuilder();
            string styleString = string.Empty;
            string colSpanTag = @"colspan=1";
            // dosage group if it exists, then adult dose in cell
            if (pDosageGroupName.Length > 0)
                dosageHTML.AppendFormat("<td><ul>{0}</ul><br>{1}</td>", pDosageGroupName,dosageNode.dosage);
            else
                dosageHTML.AppendFormat(@"<td>{0}</td>",dosageNode.dosage);
            // 3 remaining doses in cells
            if (dosageNode.dosage2 == dosageNode.dosage3 && dosageNode.dosage2 == dosageNode.dosage4)
                colSpanTag = @"colspan=3";
            if (dosageNode.dosage2 == dosageNode.dosage3 && dosageNode.dosage2 != dosageNode.dosage4)
                colSpanTag = @"colspan=2";

            // Dosage 2
            List<BDLinkedNote> d2Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d2Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d2Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.UnmarkedComment);
            List<BDLinkedNote> d2Footers = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.Footnote));
            string d2FooterMarker = buildFooterMarkerForList(d2Footers, true, pFootnotes, pObjectsOnPage);

            BDHtmlPage d2NotePage = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d2Inline, d2Marked, d2Unmarked, pObjectsOnPage);
            if (d2NotePage != null)
            {
                if (dosageNode.dosage2.Length > 0)
                    dosageHTML.AppendFormat(@"<td {0}><a href=""{1}"">{2}</a{3}>", colSpanTag, d2NotePage.Uuid.ToString().ToUpper(), dosageNode.dosage2, d2FooterMarker);
                else
                    dosageHTML.AppendFormat(@"<td {0}><a href=""{1}"">See Notes.</a>{3}", colSpanTag, d2NotePage.Uuid.ToString().ToUpper(), d2FooterMarker);
            }
            else
                dosageHTML.AppendFormat(@"<td {0}>{1}{2}", colSpanTag, dosageNode.dosage2, d2FooterMarker);

            dosageHTML.Append(@"</td>");

            // Dosage 3
            if (dosageNode.dosage2 != dosageNode.dosage3)
            {
                List<BDLinkedNote> d3Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> d3Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> d3Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.UnmarkedComment);
                List<BDLinkedNote> d3Footers = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.Footnote));
                string d3FooterMarker = buildFooterMarkerForList(d3Footers, true, pFootnotes, pObjectsOnPage);

                BDHtmlPage d3NotePage = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d3Inline, d3Marked, d3Unmarked, pObjectsOnPage);
                if (d3NotePage != null)
                {
                    if (dosageNode.dosage3.Length > 0)
                        dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a{2}>", d3NotePage.Uuid.ToString().ToUpper(), dosageNode.dosage3, d3FooterMarker);
                    else
                        dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a>{3}", d3NotePage.Uuid.ToString().ToUpper(), d3FooterMarker);
                }
                else
                    dosageHTML.AppendFormat("<td>{0}{1}", dosageNode.dosage3, d3FooterMarker);
                dosageHTML.Append(@"</td>");
            }

            // Dosage 4
            if (dosageNode.dosage2 != dosageNode.dosage4)
            {
                List<BDLinkedNote> d4Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> d4Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> d4Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.UnmarkedComment);
                List<BDLinkedNote> d4Footers = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.Footnote));
                string d4FooterMarker = buildFooterMarkerForList(d4Footers, true,pFootnotes, pObjectsOnPage);

                BDHtmlPage d4NotePage = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d4Inline, d4Marked, d4Unmarked, pObjectsOnPage);
                if (d4NotePage != null)
                {
                    if (dosageNode.dosage4.Length > 0)
                        dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a>{2}", d4NotePage.Uuid.ToString().ToUpper(), dosageNode.dosage4);
                    else
                        dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a>{3}", d4NotePage.Uuid.ToString().ToUpper(), d4FooterMarker);
                }
                else
                    dosageHTML.AppendFormat("<td>{0}{1}", dosageNode.dosage4, d4FooterMarker);

                dosageHTML.Append(@"</td>");
            }
            return dosageHTML.ToString();
        }

        private string buildNodeWithReferenceAndOverviewHTML(Entities pContext, IBDNode pNode, string pHeaderTagLevel, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            // footnotes
            List<BDLinkedNote> itemFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
            string itemFooterMarker = buildFooterMarkerForList(itemFooters, true, pFootnotes, pObjectsOnPage);

            string generatedNodeName = String.Format("New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
            StringBuilder nodeHTML = new StringBuilder();
            if (pNode.Name.Length > 0 && !pNode.Name.Contains(generatedNodeName) && pNode.Name != "SINGLE PRESENTATION")
            {
                if (pHeaderTagLevel.Length > 0)
                    nodeHTML.AppendFormat(@"<{0}>{1}{2}</{3}>", pHeaderTagLevel, pNode.Name, itemFooterMarker, pHeaderTagLevel);
                else
                    nodeHTML.AppendFormat(@"{0}{1}",pNode.Name,itemFooterMarker);
            }
            nodeHTML.Append(buildReferenceHtml(pContext, pNode, pObjectsOnPage));

            // overview
            string overviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid, pObjectsOnPage);
            if (overviewHTML.Length > EMPTY_PARAGRAPH)
                nodeHTML.Append(overviewHTML);
            pObjectsOnPage.Add(pNode.Uuid);

            return nodeHTML.ToString();
        }

        private string buildNodeWithLinkedNotesAsFootnotesHTML(Entities pContext, IBDNode pNode, string pHeaderTagLevel, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            // footnotes - treat all types of linked notes as footnotes
            List<BDLinkedNote> itemFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
            itemFooters.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline));
            itemFooters.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment));
            itemFooters.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment));
            string itemFooterMarker = buildFooterMarkerForList(itemFooters, true, pFootnotes, pObjectsOnPage);

            string generatedNodeName = String.Format("New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
            StringBuilder nodeHTML = new StringBuilder();
            if (pNode.Name.Length > 0 && !pNode.Name.Contains(generatedNodeName) && pNode.Name != "SINGLE PRESENTATION")
            {
                if (pHeaderTagLevel.Length > 0)
                    nodeHTML.AppendFormat(@"<{0}>{1}{2}</{3}>", pHeaderTagLevel, pNode.Name, itemFooterMarker, pHeaderTagLevel);
                else
                    nodeHTML.AppendFormat(@"{0}{1}", pNode.Name, itemFooterMarker);
            }
            return nodeHTML.ToString();
        }

        private string buildNodeWithLinkedNotesInlineHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            return buildNodeWithLinkedNotesInlineHtml(pContext, pNode, "h4", pFootnotes, pObjectsOnPage);
        }

        /// <summary>
        /// Generate HTML for node, with all linked note types gathered into a paragraph that follows the name/overview
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <returns></returns>
        private string buildNodeWithLinkedNotesInlineHtml(Entities pContext, IBDNode pNode, string pHeaderTagLevel, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder nodeHTML = new StringBuilder();
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

            nodeHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, pHeaderTagLevel, pFootnotes, pObjectsOnPage));
            
            nodeHTML.Append(buildTextFromNotes(inlineNotes, pObjectsOnPage));
            nodeHTML.Append(buildTextFromNotes(markedNotes, pObjectsOnPage));
            nodeHTML.Append(buildTextFromNotes(unmarkedNotes, pObjectsOnPage));

            return nodeHTML.ToString();
        }

        private string buildTextFromNotes(List<BDLinkedNote> pNotes, List<Guid> pObjectsOnPage)
        {
            StringBuilder noteString = new StringBuilder();
            foreach (BDLinkedNote note in pNotes)
            {
                if (note.documentText.Length > 0)
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
                    
                    List<BDLinkedNote> itemFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, tableCell.Uuid, BDTableCell.PROPERTYNAME_CONTENTS, BDConstants.LinkedNoteType.Footnote);
                    if (itemFooters.Count == 0)
                        tableRowHTML.AppendFormat(@"{0}{1}{2}", startTag, tableCell.value, endCellTag);
                    else
                    {
                       string itemFooterMarker = buildFooterMarkerForList(itemFooters, true, pFootnotes, pObjectsOnPage);

                        if (markFooterAtEnd)
                            tableRowHTML.AppendFormat(@"{0}{1}{2}{3}", startTag, tableCell.value, itemFooterMarker, endCellTag);
                        else
                        {
                            int lineBreakIndex = tableCell.value.Length;
                            if (tableCell.value.Contains("\n"))
                                lineBreakIndex = tableCell.value.IndexOf("\n");
                            string cellTextWithFooterTag = tableCell.value.Insert(lineBreakIndex, itemFooterMarker);
                            tableRowHTML.AppendFormat(@"{0}{1}{2}", startTag, cellTextWithFooterTag, endCellTag);
                        }
                    }
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

        private string buildAntibioticsCSFPenetrationDosagesHTML(Entities pContext, IBDNode pNode, List<Guid> pObjectsOnPage)
        {
            // Table to ConfiguredEntry
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1", footnotes, pObjectsOnPage));  // BDTable

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                List<string> labels = new List<string>();
                // build markers and list for column header linked notes
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02));

                List<string> footerMarkers = new List<string>();
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true, footnotes, pObjectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true, footnotes, pObjectsOnPage));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true, footnotes, pObjectsOnPage));
                bodyHTML.Append("<table><tr>");
                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}{1}</th>", labels[i], footerMarkers[i]);
                bodyHTML.Append("</tr>");
                foreach (IBDNode child in childNodes)
                {
                    BDConfiguredEntry entry = child as BDConfiguredEntry;
                    bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", entry.Name, entry.field01, entry.field02);
                    pObjectsOnPage.Add(child.Uuid);
                }
                bodyHTML.Append("</table>");
            }
            return bodyHTML.ToString();
        }

        private List<BDHtmlPage> buildAntimicrobialWithRiskHTML(Entities pContext, List<string> labels, List<string> footerMarkers, List<BDLinkedNote> pFootersFromMetadata, List<IBDNode> antimicrobials)
        {
            List<BDHtmlPage> pages = new List<BDHtmlPage>();
            foreach (IBDNode antimicrobial in antimicrobials)
            {
                List<BDLinkedNote> pageFootnotes = new List<BDLinkedNote>();
                pageFootnotes.AddRange(pFootersFromMetadata);
                // write an HTML page for the antimicrobial, build a link for the name
                StringBuilder antimicrobialHTMLBody = new StringBuilder();
                List<Guid> antimicrobialsOnPage = new List<Guid>();
                antimicrobialHTMLBody.Append(buildNodeWithReferenceAndOverviewHTML(pContext, antimicrobial as BDNode, "h4", pageFootnotes, antimicrobialsOnPage));

                List<IBDNode> amRisks = BDFabrik.GetChildrenForParent(pContext, antimicrobial);
                foreach (IBDNode amRisk in amRisks)
                {
                    BDAntimicrobialRisk risk = amRisk as BDAntimicrobialRisk;
                    antimicrobialHTMLBody.AppendFormat("<b>{0}{1}</b>: {2}", labels[0], footerMarkers[0], risk.riskFactor);
                    antimicrobialHTMLBody.AppendFormat("<p><b>{0}{1}</b>: {2}</p>", labels[1], footerMarkers[1], risk.aapRating);
                    antimicrobialHTMLBody.AppendFormat("<p><b>{0}{1}</b>: {2}</p>", labels[2], footerMarkers[2], risk.relativeInfantDose);
                    antimicrobialsOnPage.Add(amRisk.Uuid);
                }
                antimicrobialHTMLBody.AppendFormat("<p><b>Comments</b><br>{0}</p>", buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, antimicrobial, BDConstants.LinkedNoteType.UnmarkedComment, antimicrobialsOnPage));
                pages.Add(writeBDHtmlPage(pContext, antimicrobial, antimicrobialHTMLBody, BDConstants.BDHtmlPageType.Data, pageFootnotes, antimicrobialsOnPage));
            }
            return pages;
        }

        /// <summary>
        /// Build HTML segment for a single property of a node, handling all linked note types
        /// as well as footer marker, and filtering out 'New' name value.
        /// No surrounding HTML tags are returned
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pTherapy"></param>
        /// <param name="pNoteParentId"></param>
        /// <param name="pPropertyValue"></param>
        /// <param name="pNotePropertyName"></param>
        /// <returns></returns>
        private string buildNodePropertyHTML(Entities pContext, IBDNode pNode, Guid pNoteParentId, string pPropertyValue, string pPropertyName, bool showNotesInline, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            return buildNodePropertyHTML(pContext, pNode, pNoteParentId, pPropertyValue, pPropertyName, showNotesInline, string.Empty, pFootnotes, pObjectsOnPage);
        }

        private string buildNodePropertyHTML(Entities pContext, IBDNode pNode, Guid pNoteParentId, string pPropertyValue, string pPropertyName, bool showNotesInline, string pHtmlTag, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            string startTag = (pHtmlTag.Length > 0) ? string.Format("<{0}>", pHtmlTag) : string.Empty;
            string endTag = (pHtmlTag.Length > 0) ? string.Format("</{0}>", pHtmlTag) : string.Empty;

            StringBuilder propertyHTML = new StringBuilder();
            List<BDLinkedNote> propertyFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Footnote));
            string footerMarker = buildFooterMarkerForList(propertyFooters, true, pFootnotes, pObjectsOnPage);

            List<BDLinkedNote> inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.UnmarkedComment);

            if (pPropertyValue.Contains(BDUtilities.GetEnumDescription(pNode.NodeType)))
                pPropertyValue = string.Empty;

            if (showNotesInline)
            {
                if (pPropertyValue.Length > 0)
                    propertyHTML.AppendFormat(@"{0}{1}{2}{3}<br>", startTag, pPropertyValue.Trim(), endTag, footerMarker);
                pObjectsOnPage.Add(pNoteParentId);

                List<BDLinkedNote> notesList = new List<BDLinkedNote>();
                notesList.AddRange(inline);
                notesList.AddRange(marked);
                notesList.AddRange(unmarked);
                foreach (BDLinkedNote note in notesList)
                {
                    propertyHTML.AppendFormat("{0}<br>", note.documentText);
                    pObjectsOnPage.Add(note.Uuid);
                }
            }
            else
            {
                BDHtmlPage notePage = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.NodeType, inline, marked, unmarked, pObjectsOnPage);

                if (notePage != null)
                {
                    if (pPropertyValue.Length > 0)
                        propertyHTML.AppendFormat(@"<a href=""{0}"">{1}{2}{3}</a>{4}", notePage.Uuid.ToString().ToUpper(), startTag, pPropertyValue.Trim(), endTag, footerMarker);
                    else
                        propertyHTML.AppendFormat(@"<a href=""{0}"">See Comments.</a>", notePage.Uuid.ToString().ToUpper());
                }
                else
                    propertyHTML.AppendFormat(@" {0}{1}{2}{3}", startTag, pPropertyValue.Trim(), endTag, footerMarker);
            }
            return propertyHTML.ToString();
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
            List<BDLinkedNote> notes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pParentId, pNotePropertyName, BDConstants.LinkedNoteType.MarkedComment);
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
            BDNodeToHtmlPageIndex index = BDNodeToHtmlPageIndex.RetrieveIndexEntryForHtmlPageId(pContext, pPage.Uuid);
            if (index != null)
                currentChapter = BDFabrik.RetrieveNode(pContext, index.chapterId);
            else
                currentChapter = null;

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
                                // none of the existing html pages has this guid so the existing link is invalid
                                // look up the linkedNoteAssociation with the provided guid in the'parentKeyPropertyName'
                                // if returned object is null, then its either not found or collection was > 1 entry
                                BDLinkedNoteAssociation linkTargetAssn = BDLinkedNoteAssociation.RetrieveLinkedNoteAssociationForParentKeyPropertyName(pContext, anchorGuid.ToString());
                                if (linkTargetAssn != null)
                                {
                                    if (linkTargetAssn.internalLinkNodeId.HasValue)
                                    {
                                        // this is an internal link - first check the pagesMap for the HTML page containing that object
                                        BDHtmlPageMap mapEntry = pagesMap.FirstOrDefault<BDHtmlPageMap>(x => x.OriginalIBDObjectId == linkTargetAssn.internalLinkNodeId.Value);

                                        if (null != mapEntry && pExistingPages.Contains(mapEntry.HtmlPageId))
                                        {
                                            // modify anchor tag to point to the html page generated for the targeted node
                                            string newText = pPage.documentText.Replace(anchorGuid.ToString(), mapEntry.HtmlPageId.ToString().ToUpper());
                                            pPage.documentText = newText;
                                            BDHtmlPage.Save(pContext, pPage);
                                        }
                                        else // no page exists for this - there should have been one if its an internal link.
                                            Debug.WriteLine("Unable to map link in {0} showing {1}", pPage.Uuid, anchorGuid);
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
                                                BDHtmlPage newPage = generatePageForLinkedNotes(pContext, linkTargetAssn.linkedNoteId.Value, BDConstants.BDNodeType.BDLinkedNote, targetNote.documentText, BDConstants.BDHtmlPageType.Data, objectsOnPage);

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

        private string retrieveMetadataLabelForPropertyName(Entities pContext, BDConstants.BDNodeType pNodeType,  string pPropertyName)
        {
            string propertyColumnLabel = string.Empty;
            foreach (BDLayoutMetadataColumn column in metadataLayoutColumns)
            {
                List<BDLayoutMetadataColumnNodeType> columnNodeTypes = BDLayoutMetadataColumnNodeType.RetrieveListForLayoutColumn(pContext, column.Uuid);
                foreach (BDLayoutMetadataColumnNodeType columnNodeType in columnNodeTypes)
                {
                    if (columnNodeType.propertyName == pPropertyName && columnNodeType.nodeType == (int)pNodeType)
                    {
                        propertyColumnLabel = column.label;
                        break;
                    }
                    if (propertyColumnLabel.Length > 0)
                        break;
                }
            }
            return propertyColumnLabel;
        }

        private List<BDLinkedNote> retrieveNotesForLayoutColumn(Entities pContext, BDLayoutMetadataColumn pColumn)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pColumn.Uuid);
            List<BDLinkedNote> notes = new List<BDLinkedNote>();
            foreach (BDLinkedNoteAssociation link in links)
                notes.Add(BDLinkedNote.RetrieveLinkedNoteWithId(pContext, link.linkedNoteId));
            
            return notes;
        }

        private BDHtmlPage writeBDHtmlPage(Entities pContext, IBDNode pDisplayParentNode, StringBuilder pBodyHTML, BDConstants.BDHtmlPageType pPageType, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
        {
            return writeBDHtmlPage(pContext, pDisplayParentNode, pBodyHTML.ToString(), pPageType, pFootnotes, pObjectsOnPage);
        }

        /// <summary>
        /// Append footnotes, then wrap HTML with outer tags and save to db
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentId"></param>
        /// <param name="pDisplayParentType"></param>
        /// <param name="pBodyHTML"></param>
        /// <param name="pPageType"></param>
        /// <returns></returns>
        private BDHtmlPage writeBDHtmlPage(Entities pContext, IBDObject pDisplayParent, string pBodyHTML, BDConstants.BDHtmlPageType pPageType, List<BDLinkedNote> pFootnotes, List<Guid> pObjectsOnPage)
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

            BDNodeToHtmlPageIndex indexEntry = BDNodeToHtmlPageIndex.RetrieveIndexEntryForIBDNodeId(pContext, masterGuid);
            
            BDHtmlPage newPage = null;
            if (indexEntry != null)
                newPage = BDHtmlPage.RetrieveWithId(pContext, indexEntry.htmlPageId); 
            if(newPage == null)
                newPage = BDHtmlPage.CreateBDHtmlPage(pContext, Guid.NewGuid());

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
            ;
            if (newPage.layoutVariant == -1 )
                Debug.WriteLine("Page has no layout assigned: {0}", newPage.Uuid);

            BDHtmlPage.Save(pContext, newPage);

            if (indexEntry == null)
            {
                Guid chapterId = Guid.Empty;
                if (currentChapter != null)
                    chapterId = currentChapter.Uuid;
                indexEntry = BDNodeToHtmlPageIndex.CreateBDNodeToHtmlPageIndex(pContext, masterGuid, newPage.Uuid, chapterId);
            }
            else
            {
                indexEntry.chapterId = currentChapter != null ? currentChapter.Uuid : Guid.Empty;
                indexEntry.wasGenerated = true;
            }
            BDNodeToHtmlPageIndex.Save(pContext, indexEntry);

            if (pObjectsOnPage.Count == 0 && pDisplayParent != null)
                Debug.WriteLine("no objects added for page {0}", newPage.Uuid);

            if(pDisplayParent != null)
                pagesMap.Add(new BDHtmlPageMap(newPage.Uuid, pDisplayParent.Uuid));


            List<Guid> filteredObjects = pObjectsOnPage.Distinct().ToList();
            foreach (Guid objectId in filteredObjects)
            {
                pagesMap.Add(new BDHtmlPageMap(newPage.Uuid, objectId));
            }
            return newPage;
        }
        #endregion
    }
}
