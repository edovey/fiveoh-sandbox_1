﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class BDHtmlPageGenerator
    {
        private const string topHtml = @"<html><head><meta http-equiv=""Content-type"" content=""text/html;charset=UTF-8\""/><link rel=""stylesheet"" type=""text/css"" href=""bdviewer.css"" /> </head><body>";
        private const string bottomHtml = @"</body></html>";
        private const int EMPTY_PARAGRAPH = 8;  // <p> </p>
        private const string imgFileTag = "<img src=\"\\{0}{1}\" alt=\"\" width=\"100\" height=\"100\" />";
        private const string imgFileFolder = "images\\";  //note double slash in path (for ios viewer)

        private List<BDLayoutMetadataColumn> metadataLayoutColumns = new List<BDLayoutMetadataColumn>();
        private List<BDLinkedNote> pageFooterList = new List<BDLinkedNote>();

        // create variables to hold data for 'same as previous' settings on Therapy
        string previousTherapyName = string.Empty;
        string previousTherapyDosage = string.Empty;
        string previousTherapyDosage1 = string.Empty;
        string previousTherapyDuration = string.Empty;
        string previousTherapyDuration1 = string.Empty;
        Guid previousTherapyId = Guid.Empty;
        bool therapiesHaveDosage = false;
        bool therapiesHaveDuration = false;

        public void Generate(IBDNode pNode)
        {
            Entities dataContext = new Entities();

            // Clear the data from the local store.
            BDHtmlPage.DeleteAll();
            BDNavigationNode.DeleteAll();

            generatePages(pNode);

            List<BDHtmlPage> pages = BDHtmlPage.RetrieveAll(dataContext);
            List<Guid> htmlPageIds = BDHtmlPage.RetrieveAllIDs(dataContext);

            foreach (BDHtmlPage page in pages)
            {
                processTextForInternalLinks(dataContext, page, htmlPageIds);
                processTextForSubscriptAndSuperscriptMarkup(dataContext, page);
                processTextForCarriageReturn(dataContext, page);
            }
        }

        private void generatePages(IBDNode pNode)
        {
            Entities dataContext = new Entities();
            List<BDNode> chapters = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDChapter);
            List<BDHtmlPage> chapterPages = new List<BDHtmlPage>();
            if (pNode == null)
            {
                List<BDHtmlPage> childDetailPages = new List<BDHtmlPage>();

                List<BDHtmlPage> childNavPages = new List<BDHtmlPage>();
                foreach (BDNode chapter in chapters)
                {
                    generateOverviewAndChildrenForNode(dataContext, chapter, childDetailPages, childNavPages);
                    if(childDetailPages.Count > 0)
                        chapterPages.AddRange(childDetailPages);
                    if (childNavPages.Count > 0)
                        chapterPages.AddRange(childNavPages);
                }
            }
            else
            {
                List<BDHtmlPage> childDetailPages = new List<BDHtmlPage>();
                List<BDHtmlPage> childNavPages = new List<BDHtmlPage>();
                if (pNode.NodeType == BDConstants.BDNodeType.BDChapter)
                {
                    generateOverviewAndChildrenForNode(dataContext, pNode, childDetailPages, childNavPages);
                    chapterPages.AddRange(childDetailPages);
                    chapterPages.AddRange(childNavPages);
                }
            }
            Debug.WriteLine("Creating chapter page with returned list");
            if (chapterPages.Count > 0)
                generateNavigationPage(dataContext, null, chapterPages);
            

            // generate html pages for all linkedNotes that have at least one linkedAssociation where the parent type is 11
            List<BDLinkedNote> notesWithinNotes = BDLinkedNote.RetrieveLinkedNotesWithLinkedNoteParentType(dataContext);
            foreach (BDLinkedNote n in notesWithinNotes)
            {
                List<BDLinkedNoteAssociation> assns = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForLinkedNoteId(dataContext, n.uuid);
                foreach (BDLinkedNoteAssociation a in assns)
                {
                    if (a.parentType == (int)BDConstants.BDNodeType.BDLinkedNote)
                    {
                        BDConstants.BDHtmlPageType pageType = BDConstants.BDHtmlPageType.Undefined;
                        if (a.LinkedNoteType == BDConstants.LinkedNoteType.MarkedComment || a.LinkedNoteType == BDConstants.LinkedNoteType.UnmarkedComment || a.LinkedNoteType == BDConstants.LinkedNoteType.Inline)
                            pageType = BDConstants.BDHtmlPageType.Comments;
                        else if (a.LinkedNoteType == BDConstants.LinkedNoteType.Footnote)
                            pageType = BDConstants.BDHtmlPageType.Footnote;
                        else if (a.LinkedNoteType == BDConstants.LinkedNoteType.Reference)
                            pageType = BDConstants.BDHtmlPageType.Reference;
                        else
                            pageType = BDConstants.BDHtmlPageType.Data;

                        generatePageForLinkedNotes(dataContext, a.linkedNoteId.Value, BDConstants.BDNodeType.BDLinkedNote, n.documentText, pageType);
                    }
                }
            }
        }

        /// <summary>
        /// Recursive method that traverses the navigation tree
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
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
            StringBuilder pageHTML = new StringBuilder();
            if (pNode != null)
            {
                pageHTML.AppendFormat(@"<h2>{0}</h2>", pNode.Name);
                string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid);
                if (noteText.Length > EMPTY_PARAGRAPH)
                    pageHTML.Append(noteText);
            }
            // TODO:  build javascript blocks to expand/collapse overview
            foreach (BDHtmlPage page in pChildPages)
            {
                if (page != null)
                {
                    BDNode childNode = BDNode.RetrieveNodeWithId(pContext, page.displayParentId.Value);
                    if (childNode != null)
                    {
                        pageHTML.AppendFormat(@"<p><a href=""{0}""><b>{1}</b></a></p>", childNode.Uuid, childNode.Name);
                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode as BDNode, pageHTML, BDConstants.BDHtmlPageType.Navigation, true);
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
            pageFooterList.Clear();

            generatePageForParentAndPropertyReferences(pContext, BDNode.PROPERTYNAME_NAME, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
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
                            generatePageForAttachment(pContext, pNode);

                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            nodeChildPages.Add(generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            nodeChildPages.Add(generatePageForAntibioticsPharmacodynamics(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            nodeChildPages.Add(generatePageForAntibioticsDosingInRenalImpairment(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            nodeChildPages.Add(generatePageForAntibioticDosingInHepaticImpairment(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            nodeChildPages.Add(generatePageForDentalMicroorganisms(pContext, pNode));
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
                                    nodeChildPages.Add(generatePageForEmpiricTherapyDisease(pContext, pNode as BDNode));
                                }
                                else
                                    isPageGenerated = false;
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
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfBCNE(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfParasitic(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfCultureDirected(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
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
                            nodeChildPages.Add(generatePageForEmpiricTherapyPresentation(pContext, pNode as BDNode));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfCellulitisInExtremities(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            nodeChildPages.Add(generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
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
                            nodeChildPages.Add(generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            nodeChildPages.Add(generatePageForAntibioticsStepdown(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            nodeChildPages.Add(generatePageForAntibioticsCSFPenetration(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            nodeChildPages.Add(generatePageforAntibioticsBLactam(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            nodeChildPages.Add(generatePageForAntibioticsNameListing(pContext, pNode as BDNode));
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
                            nodeChildPages.Add(generatePageForAntibioticsDosingAndDailyCosts(pContext, pNode as BDNode));
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfPneumonia(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
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
                            nodeChildPages.Add(generatePageForAntibiotics_HepaticImpairmentGrading(pContext, pNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            nodeChildPages.Add(generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                            nodeChildPages.Add(generatePageForEmpiricTherapyOfEndocarditisPaediatrics(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
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
                            nodeChildPages.Add(generatePageForAntibioticsClinicalGuidelinesSpectrum(pContext, pNode as BDNode));
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            nodeChildPages.Add(generatePageForDentalProphylaxis(pContext, pNode));
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
            
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, true);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            // child nodes can either be pathogen groups or topics (node with overview)
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode childNode in childNodes)
            {
                if (childNode.NodeType == BDConstants.BDNodeType.BDTopic)
                {
                    if (childNode.Name.Length > 0)
                        bodyHTML.AppendFormat("<h2>{0}</h2>", childNode.Name);
                    string nodeOverviewHTML = retrieveNoteTextForOverview(pContext, childNode.Uuid);
                    if (nodeOverviewHTML.Length > EMPTY_PARAGRAPH)
                    {
                        bodyHTML.Append(nodeOverviewHTML);
                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in childNodes)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, "h3"));
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode node in childNodes)
            {
                if (node.NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
                {
                    BDHtmlPage footnote = generatePageForParentAndPropertyFootnote(pContext, BDNode.PROPERTYNAME_NAME, node as BDNode);
                    if (footnote == null)
                        bodyHTML.AppendFormat(@"<b>{0}</b><br>", node.Name);
                    else
                        bodyHTML.AppendFormat(@"<a href=""{0}""><b>{1}</b></a><br>", footnote.Uuid, node.Name);
                }
            }
           return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (antimicrobials.Count > 0)
            {
                // build markers and list for column header linked notes
                string c1Label = retrieveMetadataLabelForPropertyName(pContext, pNode.NodeType, BDNode.PROPERTYNAME_NAME);
                string c2Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE);
                string c3Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_COST);

                List<BDLinkedNote> c1Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]);
                string c1footerMarker = buildFooterMarkerForList(c1Links, true);
                List<BDLinkedNote> c2Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]);
                string c2FooterMarker = buildFooterMarkerForList(c2Links, true);
                List<BDLinkedNote> c3Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]);
                string c3FooterMarker = buildFooterMarkerForList(c3Links, true);
                bodyHTML.AppendFormat(@"<table><tr><th>{0}{1}</th><th>{2}{3}</th><th>{4}{5}</th></tr>", c1Label, c1footerMarker, c2Label, c2FooterMarker, c3Label, c3FooterMarker);

                foreach (IBDNode antimicrobial in antimicrobials)
                {
                    List<BDLinkedNote> amFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, antimicrobial.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
                    string amFooterMarker = buildFooterMarkerForList(amFooters, true);
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
                            dosageHTML.AppendFormat("<b><u>{0}</u></b><br>", buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false));
                            costHTML.Append("<br>");
                            List<IBDNode> lvl1Children = BDFabrik.GetChildrenForParent(pContext, child);
                            foreach (IBDNode lvl1Child in lvl1Children)
                            {
                                // BDDosageGroup
                                dosageHTML.AppendFormat("<u>{0}</u>:", buildCellHTML(pContext, lvl1Child, BDNode.PROPERTYNAME_NAME, lvl1Child.Name, false));
                                costHTML.Append("<br>");

                                List<IBDNode> lvl2Children = BDFabrik.GetChildrenForParent(pContext, lvl1Child);
                                string cellLineTag = (lvl2Children.Count > 0) ? "<br>" : "";
                                foreach (IBDNode lvl2Child in lvl2Children)
                                {
                                    // BDDosage
                                    BDDosage dosage = lvl2Child as BDDosage;
                                    dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false), cellLineTag);
                                    costHTML.Append(buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_COST, dosage.cost, false));
                                    if (dosage.cost2.Length > 0)
                                        costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_COST2, dosage.cost2,false), cellLineTag);
                                    else
                                        costHTML.Append(cellLineTag);
                                }
                            }
                        }
                        else if (child.NodeType == BDConstants.BDNodeType.BDDosageGroup)
                        {
                            dosageHTML.AppendFormat("<u>{0}</u>:", buildCellHTML(pContext, child, BDNode.PROPERTYNAME_NAME, child.Name, false));
                            costHTML.Append("<br>");

                            List<IBDNode> lvl2Children = BDFabrik.GetChildrenForParent(pContext, child);
                            string cellLineTag = (lvl2Children.Count > 0) ? "<br>" : "";
                            foreach (IBDNode lvl2Child in lvl2Children)
                            {
                                // BDDosage
                                BDDosage dosage = lvl2Child as BDDosage;
                                dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false), cellLineTag);
                                costHTML.Append(buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_COST, dosage.cost, false));
                                if (dosage.cost2.Length > 0)
                                    costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, lvl2Child, BDDosage.PROPERTYNAME_COST2, dosage.cost2, false), cellLineTag);
                                else
                                    costHTML.Append(cellLineTag);
                            }
                        }
                        else if (child.NodeType == BDConstants.BDNodeType.BDDosage)
                        {
                            string cellLineTag = (childNodes.Count > 0) ? "<br>" : "";

                            BDDosage dosage = child as BDDosage;
                            dosageHTML.AppendFormat("{0}{1}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_DOSAGE, dosage.dosage, false), cellLineTag);
                            costHTML.Append(buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_COST, dosage.cost, false));
                            if (dosage.cost2.Length > 0)
                                costHTML.AppendFormat("-{0}{1}", buildCellHTML(pContext, child, BDDosage.PROPERTYNAME_COST2, dosage.cost2, false), cellLineTag);
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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                foreach (IBDNode node in childNodes)
                {
                    if (node.NodeType == BDConstants.BDNodeType.BDTopic)
                    {
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, node, "h3"));

                        List<IBDNode> topicChildren = BDFabrik.GetChildrenForParent(pContext, node);
                        foreach (IBDNode topicChild in topicChildren)
                        {
                            if (topicChild.NodeType == BDConstants.BDNodeType.BDTable)
                            {
                                // insert node name (table name)
                                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topicChild, "h4"));

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
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
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
                                                    bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
                                                }
                                            }
                                        }
                                    }
                                    bodyHTML.Append(@"</table>");
                                }
                            }
                            else if (topicChild.NodeType == BDConstants.BDNodeType.BDSubtopic)
                            {
                                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topicChild, "h4"));
                                string noteText = retrieveNoteTextForOverview(pContext, topicChild.Uuid);
                                if (noteText.Length > 0)
                                    bodyHTML.Append(noteText);
                                List<IBDNode> subtopicChildren = BDFabrik.GetChildrenForParent(pContext, topicChild);
                                foreach (IBDNode subtopicChild in subtopicChildren)
                                {
                                    if (subtopicChild.NodeType == BDConstants.BDNodeType.BDTable)
                                    {
                                        int columnCount = BDFabrik.GetTableColumnCount(subtopicChild.LayoutVariant);
                                        // insert node name (table name)
                                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subtopicChild, "h4"));

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
                                                        bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
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
                                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
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
                        bodyHTML.Append(buildAttachmentHTML(pContext, node));
                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));
            
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
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true));

                bodyHTML.AppendFormat(@"<table><tr><th rowspan=""4"">{0}{1}</th><th rowspan=""4"">{2}{3}</th>",labels[0],footerMarkers[0],labels[1],footerMarkers[1]);
                bodyHTML.Append(@"<th colspan=""3""><b>Dose and Interval Adjustment for Renal Impairment</b></th></tr>");
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
                            bodyHTML.Append(buildDosageHTML(pContext, dNode, dosageGroupName));
                            dosageGroupName = string.Empty;
                        }
                        else // BDDosageGroup
                            dosageGroupName = dNode.Name;
                    bodyHTML.Append("</tr>");
                    }
                }
                bodyHTML.Append(@"</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
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

                    bodyHTML.AppendFormat(@"<tr><td>{0}</td><td>{1}</td></tr>", child.Name, retrieveNoteTextForOverview(pContext, child.Uuid));
                }
                bodyHTML.Append(@"</table>");
            }
           return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            // build markers and list for column header linked notes
            string c1Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME);
            string c2Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01);
            string c3Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02);
            string c4Label = retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03);

            List<BDLinkedNote> c1Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]);
            string c1footerMarker = buildFooterMarkerForList(c1Links, true);
            List<BDLinkedNote> c2Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]);
            string c2FooterMarker = buildFooterMarkerForList(c2Links, true);
            List<BDLinkedNote> c3Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]);
            string c3FooterMarker = buildFooterMarkerForList(c3Links, true);
            List<BDLinkedNote> c4Links = retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[3]);
            string c4FooterMarker = buildFooterMarkerForList(c4Links, true);
            bodyHTML.AppendFormat(@"<table><tr><th>{0}{1}</th><th>{2}{3}</th><th>{4}{5}</th><th>{6}{7}</th></tr>", c1Label, c1footerMarker, c2Label, c2FooterMarker, c3Label, c3FooterMarker,c4Label, c4FooterMarker);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode childNode in childNodes)
            {
                // children are BDConfiguredEntry
                BDConfiguredEntry entry = childNode as BDConfiguredEntry;
                bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", entry.Name, entry.field01, entry.field02, entry.field03);
            }
            bodyHTML.Append("</table>");
            bodyHTML.Append(buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, pNode, BDConstants.LinkedNoteType.UnmarkedComment));
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
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
                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
                        }
                        else if (node.NodeType == BDConstants.BDNodeType.BDTableSection)
                        {
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan = 3><b>{0}</b><td></tr>", node.Name);
                            List<IBDNode> sectionChildren = BDFabrik.GetChildrenForParent(pContext, node);
                            if (sectionChildren.Count > 0)
                            {
                                foreach (IBDNode sectionChild in sectionChildren)
                                {
                                    if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                                    {
                                        if (sectionChild.Name.Length > 0)
                                        {
                                            bodyHTML.AppendFormat(@"<tr><td colspan = 3><nbsp><nbsp>{0}<td></tr>", sectionChild.Name);
                                            List<BDTableRow> rows = BDTableRow.RetrieveTableRowsForParentId(pContext, sectionChild.Uuid);
                                            foreach (BDTableRow row in rows)
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
                                        }
                                    }
                                    else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = sectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
                                    }
                                }
                            }

                        }
                        else if (node.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                        {
                            //TODO:  Make fontsize smaller than antimicrobialSection name
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan = 3>{0}<td></tr>", node.Name);
                        }
                    }
                }
                    bodyHTML.Append(@"</table>");
            }

            //TODO:  implement cross-reactivity table

            // insert footer text
            if (footerList.Count > 0)
            {
                footerHTML.Append(@"<h4>Footnotes</h4>");
                footerHTML.Append(@"<ol>");

                foreach (BDLinkedNote note in footerList)
                {
                    footerHTML.AppendFormat(@"<li>{0}</li>", note.documentText);
                }
                footerHTML.Append(@"</ol>");
                bodyHTML.Append(footerHTML);
            }
           return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

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
                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
                        }
                        else if (node.NodeType == BDConstants.BDNodeType.BDTableSection)
                        {
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan = 5><i>{0}</i><td></tr>", node.Name);
                            List<IBDNode> sectionChildren = BDFabrik.GetChildrenForParent(pContext, node);
                            if (sectionChildren.Count > 0)
                            {
                                foreach (IBDNode sectionChild in sectionChildren)
                                {
                                    if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                                    {
                                        if (sectionChild.Name.Length > 0)
                                        {
                                            bodyHTML.AppendFormat(@"<tr><td colspan = 5>{0}<td></tr>", sectionChild.Name);
                                            List<BDTableRow> rows = BDTableRow.RetrieveTableRowsForParentId(pContext, sectionChild.Uuid);
                                            foreach (BDTableRow row in rows)
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
                                        }
                                    }
                                    else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = sectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, false));
                                    }
                                }
                            }

                        }
                        else if (node.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                        {
                            //TODO:  Make fontsize smaller than antimicrobialSection name
                            if (node.Name.Length > 0)
                                bodyHTML.AppendFormat(@"<tr><td colspan = 5>{0}<td></tr>", node.Name);
                            List<IBDNode> subsectionChildren = BDFabrik.GetChildrenForParent(pContext, node);
                            if (subsectionChildren.Count > 0)
                            {
                                foreach (IBDNode subsectionChild in subsectionChildren)
                                {
                                    if (subsectionChild.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                                    {
                                        if (subsectionChild.Name.Length > 0)
                                        {
                                            bodyHTML.AppendFormat(@"<tr><td colspan = 5>{0}<td></tr>", subsectionChild.Name);
                                            List<BDTableRow> rows = BDTableRow.RetrieveTableRowsForParentId(pContext, subsectionChild.Uuid);
                                            foreach (BDTableRow row in rows)
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, true));
                                        }
                                    }
                                    else if (subsectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = subsectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, false));
                                    }
                                }
                            }
                        }
                    }
                }
                bodyHTML.Append(@"</table>");
            }
            // insert footer text
            if (footerList.Count > 0)
            {
                footerHTML.Append(@"<h4>Footnotes</h4>");
                footerHTML.Append(@"<ol>");

                foreach (BDLinkedNote note in footerList)
                {
                    footerHTML.AppendFormat(@"<li>{0}</li>", note.documentText);
                }
                footerHTML.Append(@"</ol>");
                bodyHTML.Append(footerHTML);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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

            // build data into a table; place footer at the bottom of the HTML page
            // TODO:  test creating a numbered link to the footer?

            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode child in childNodes)
                {
                    if (child.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_CSFPenetration)
                    {
                        //Append HTML for Topic + overview
                        bodyHTML.AppendFormat(@"<h3>{0}</h3>", child.Name);
                        bodyHTML.Append(retrieveNoteTextForOverview(pContext, child.Uuid));

                        List<IBDNode> categories = BDFabrik.GetChildrenForParent(pContext, child);
                        foreach (IBDNode category in categories)
                        {
                            bodyHTML.AppendFormat(@"<h4>{0}</h4>", category.Name);
                            bodyHTML.Append("<table><tr><th>Excellent Penetration</th><th>Good Penetration</th><th>Poor Penetration</th></tr>");
                            List<IBDNode> subcategories = BDFabrik.GetChildrenForParent(pContext, category);
                            if (subcategories.Count > 0)
                            {
                                bodyHTML.Append(@"<tr>");
                                foreach (IBDNode column in subcategories)
                                {
                                    bodyHTML.Append(@"<td>");
                                    // build columns
                                    List<IBDNode> columnDetail = BDFabrik.GetChildrenForParent(pContext, column);
                                    if (columnDetail.Count > 0)
                                    {
                                        StringBuilder colHTML = new StringBuilder();
                                        foreach (IBDNode antimicrobial in columnDetail)
                                        {
                                            List<BDLinkedNote> itemFooter = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, antimicrobial.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
                                            if (itemFooter.Count == 0)
                                                colHTML.AppendFormat(@"{0}<br>", antimicrobial.Name);
                                            else
                                            {
                                                StringBuilder footerMarker = new StringBuilder();
                                                foreach (BDLinkedNote footer in itemFooter)
                                                {
                                                    if (!footerList.Contains(footer))
                                                        footerList.Add(footer);

                                                    footerMarker.AppendFormat(@"<sup>{0}</sup>,", footerList.IndexOf(footer) + 1);
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
                        bodyHTML.Append(buildAntibioticsCSFPenetrationDosagesHTML(pContext,child));
            }

            // insert footer text
            if (footerList.Count > 0)
            {
                footerHTML.Append(@"<h4>Footnotes</h4>");
                footerHTML.Append(@"<ol>");

                foreach (BDLinkedNote note in footerList)
                {
                    footerHTML.AppendFormat(@"<li>{0}</li>", note.documentText);
                }
                footerHTML.Append(@"</ol>");
                bodyHTML.Append(footerHTML);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            // show child nodes in a table
            List<IBDNode> subsections = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode subsection in subsections)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subsection as BDNode, "h3")); 
                List<IBDNode> topics = BDFabrik.GetChildrenForParent(pContext, subsection);
                foreach (IBDNode topic in topics)
                {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topic as BDNode, "h4"));
                    
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
                                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, tableChild, "b"));

                                List<IBDNode> rows = BDFabrik.GetChildrenForParent(pContext, tableChild);
                                foreach (IBDNode row in rows)
                                {
                                    BDTableRow tableRow = row as BDTableRow;
                                    if (row != null)
                                        bodyHTML.Append(buildTableRowHtml(pContext, tableRow, false, true));
                                }
                            }
                            else if (tableChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                            {
                                BDTableRow headerRow = tableChild as BDTableRow;
                                bodyHTML.Append(buildTableRowHtml(pContext, headerRow, false, true));
                            }
                        }
                            bodyHTML.Append(@"</table>");
                    }
                }
            }
            // insert footer text
            if (footerList.Count > 0)
            {
                footerHTML.Append(@"<h4>Footnotes</h4>");
                footerHTML.Append(@"<ol>");

                foreach (BDLinkedNote note in footerList)
                {
                    footerHTML.AppendFormat(@"<li>{0}</li>", note.documentText);
                }
                footerHTML.Append(@"</ol>");
                bodyHTML.Append(footerHTML);
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> presentations = BDFabrik.GetChildrenForParent(pContext, pNode);

#if DEBUG
            if (presentations.Count > 1)
                throw new InvalidOperationException();
#endif
            foreach (IBDNode presentation in presentations)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, presentation as BDNode, "h2"));
                    List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, presentation);
                if (presentation.LayoutVariant == BDConstants.LayoutVariantType.Dental_RecommendedTherapy)
                {
                    foreach (IBDNode tGroup in childNodes)
                    {
                        BDTherapyGroup group = tGroup as BDTherapyGroup;
                        if (null != group)
                        {
                            bodyHTML.Append(buildTherapyGroupWithLinkedNotesHtml(pContext, group));
                            bodyHTML.Append(buildTherapyGroupHTML(pContext, group));
                        }
                    }
                }
                else
                {
                    foreach (IBDNode pathogenGroup in childNodes)
                        bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            // gastroenteritis:  get Topic as child of Presentation, then Pathogen Group
            if (pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis || pNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected)
            {
                foreach (IBDNode topic in childNodes)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h3"));
                    List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
                    foreach (IBDNode pathogenGroup in pathogenGroups)
                        bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode));
                }
            }
            else if (pNode.LayoutVariant == BDConstants.LayoutVariantType.Dental_RecommendedTherapy)
            {
                foreach (IBDNode tGroup in childNodes)
                {
                    BDTherapyGroup group = tGroup as BDTherapyGroup;
                    if (null != group)
                    {
                        bodyHTML.Append(buildTherapyGroupWithLinkedNotesHtml(pContext, group));
                        bodyHTML.Append(buildTherapyGroupHTML(pContext, group));
                    }
                }
            }
            else
            {
                List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode pathogenGroup in childNodes)
                    bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode));
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode child in childNodes)
            {
                if(child.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
                    bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pNode as BDNode));
                else if (child.NodeType == BDConstants.BDNodeType.BDTable)
                {
                    // table
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode as BDNode, "h2"));
                    List<IBDNode> conditions = BDFabrik.GetChildrenForParent(pContext, pNode);
                    if (conditions.Count > 0)
                    {
                        bodyHTML.Append("<table><tr><th>Condition</th><th>Other Potential Pathogens</th></tr>");
                        foreach (IBDNode condition in conditions)
                        {
                            bodyHTML.AppendFormat(@"<tr><td>{0}</td><td>", condition.Name);
                            List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, condition);
                            foreach (IBDNode node in pathogens)
                            {
                                bodyHTML.AppendFormat("{0}<br>", node.Name);
                            }
                        }
                        bodyHTML.Append("</td></tr></table>");
                    }
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in childNodes)   // response or pathogen group
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, "h2"));
                if (child.NodeType == BDConstants.BDNodeType.BDResponse)
                {
                    List<IBDNode> frequencies = BDFabrik.GetChildrenForParent(pContext, child);
                    foreach (IBDNode frequency in frequencies)
                    {
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, frequency as BDNode, "h4"));
                        List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, frequency);
                        foreach (IBDNode pathogenGroup in pathogenGroups)
                        {
                            bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode));
                        }
                    }
                }
                else if (child.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
                {
                    List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, child);
                    foreach (IBDNode pathogen in pathogens)
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pathogen as BDNode, "h4"));
                }
            } 
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

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
                    clinical.AppendFormat(@"{0}</p>", buildTextForParentAndPropertyFromLinkedNotes(pContext, BDNode.PROPERTYNAME_NAME, topic, BDConstants.LinkedNoteType.Inline));
                    // overview contains the 'Diagnosis' column data
                    diagnosis.Append(retrieveNoteTextForOverview(pContext, topic.Uuid));
                }
                else if (therapyGroup != null)
                   therapy.Append(buildTherapyGroupHTML(pContext, therapyGroup));

            }
            bodyHTML.AppendFormat(@"{0}<br>{1}<br>{2}", clinical, diagnosis, therapy);
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            if (pNode.NodeType == BDConstants.BDNodeType.BDTable)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode as BDNode, "h2"));
                List<IBDNode> topics = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode topic in topics)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, topic as BDNode, "h3"));
                    List<IBDNode> subtopics = BDFabrik.GetChildrenForParent(pContext, topic);
                    foreach (IBDNode subtopic in subtopics)
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subtopic as BDNode, "h4"));
                }
                return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            if (pNode.Name.Length > 0 && !pNode.Name.Contains(@"New ") )
                bodyHTML.AppendFormat(@"<{0}>{1}</{2}>", "h1", pNode.Name, "h1");
                
            bodyHTML.Append(buildReferenceHtml(pContext, pNode));

            // overview
            string symptomsOverview = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (symptomsOverview.Length > EMPTY_PARAGRAPH)
                bodyHTML.AppendFormat(@"<u><b>Symptoms</b></u><br>{0}",symptomsOverview);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode childNode in childNodes) 
            {
                if (childNode.NodeType == BDConstants.BDNodeType.BDTherapyGroup)
                {
                        bodyHTML.Append(buildTherapyGroupWithLinkedNotesHtml(pContext, childNode as BDTherapyGroup));
                        bodyHTML.Append(buildTherapyGroupHTML(pContext, childNode as BDTherapyGroup));
                }
                else 
                {
                    string presentationOverview = retrieveNoteTextForOverview(pContext, childNode.Uuid);
                    if (presentationOverview.Length > EMPTY_PARAGRAPH)
                        bodyHTML.AppendFormat(@"<u><b>Comments</b></u><br>{0}", presentationOverview);
                }
            } 
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
        }

        /// <summary>
        /// Build page for CURB-65 - Pneumonia Severity of Illness Scoring System
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
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
                            bodyHTML.Append(buildTableRowHtml(pContext, childNodes[0] as BDTableRow, true, false));
                            for (int i = 1; i < childNodes.Count; i++)
                                bodyHTML.Append(buildTableRowHtml(pContext, childNodes[i] as BDTableRow, false, false));
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
                            bodyHTML.Append(buildTableRowHtml(pContext, row as BDTableRow, false, false));
                        else
                        {
                            List<IBDNode> sectionRows = BDFabrik.GetChildrenForParent(pContext, row);
                            foreach(IBDNode sectionRow in sectionRows)
                                bodyHTML.Append(buildTableRowHtml(pContext, sectionRow as BDTableRow, false, false));
                        }
                    }
                }
            }
            bodyHTML.Append(@"</table>");

            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
        }

        /// <summary>
        /// For culture-proven pneumonia & meningitis
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            // show child nodes in a table
            List<IBDNode> resistances = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode resistance in resistances)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, resistance as BDNode, "h2"));
                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, resistance);
                foreach(IBDNode therapyGroup in therapyGroups) 
                {
                    bodyHTML.Append(buildTherapyGroupWithLinkedNotesHtml(pContext, therapyGroup as BDTherapyGroup));
                    bodyHTML.Append(buildTherapyGroupHTML(pContext, therapyGroup as BDTherapyGroup));
                }
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
        }

        /// <summary>
        /// Build page at PathogenGroup downward
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode therapyGroup in therapyGroups)
            {
                bodyHTML.Append(buildTherapyGroupWithLinkedNotesHtml(pContext, therapyGroup as BDTherapyGroup));
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
                        therapyHTML.Append(buildTherapyWithTwoDosagesHtml(pContext, therapy));

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
                            if (therapy.dosageSameAsPrevious == false)
                                previousTherapyDosage = therapy.dosage;
                            therapiesHaveDuration = true;
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
                        bodyHTML.Append(@"<th></th>");
                    if (therapiesHaveDuration)
                        bodyHTML.Append(@"<th>Recommended<br>Duration</th>");
                    else
                        bodyHTML.Append(@"<th></th>");

                    bodyHTML.Append(@"</tr>");

                    bodyHTML.Append(therapyHTML);
                    bodyHTML.Append(@"</table>");
                }

                
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
            List<IBDNode> resistances = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode resistance in resistances)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, resistance as BDNode, "h2"));
                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, resistance);
                foreach (IBDNode therapyGroup in therapyGroups)
                {
                    bodyHTML.Append(buildTherapyGroupWithLinkedNotesHtml(pContext, therapyGroup as BDTherapyGroup));
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
                            therapyHTML.Append(buildTherapyWithTwoDurationsHtml(pContext, therapy));

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
                                if (therapy.dosageSameAsPrevious == false)
                                    previousTherapyDosage = therapy.dosage;
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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
                List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode therapyGroup in therapyGroups)
                {
                    bodyHTML.Append(buildTherapyGroupWithLinkedNotesHtml(pContext, therapyGroup as BDTherapyGroup));
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
                            therapyHTML.Append(buildTherapyHtml(pContext, therapy));

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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));

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
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[3]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[4]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[5]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[6]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[7]), true));
                bodyHTML.Append("<table><tr>");
                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}{1}</th>", labels[i], footerMarkers[i]);
                bodyHTML.Append("</tr>");
                foreach (IBDNode child in childNodes)
                {
                    BDConfiguredEntry entry = child as BDConfiguredEntry;
                    bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>",
                        entry.Name, entry.field01, entry.field02, entry.field03, entry.field04, entry.field05, entry.field06, entry.field07);

                }
                bodyHTML.Append("</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));

            // retrieve 'inline' type of linked note, draw in a box
            List<BDLinkedNote> topicNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME,BDConstants.LinkedNoteType.Inline);
            StringBuilder noteText = new StringBuilder();
            foreach(BDLinkedNote note in topicNotes)
                noteText.Append(note.documentText);
            if(topicNotes.Count > 0 && noteText.Length > 0)
                bodyHTML.AppendFormat("<div style=\"border:1px dotted black;padding:2em;\">{0}</div>", noteText);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode table in childNodes)
            {
                if(table.Name != pNode.Name)
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, table, "h3"));
                List<string> labels = new List<string>();
                // build markers and list for column header linked notes
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1));
                List<string> footerMarkers = new List<string>();
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true));

                bodyHTML.Append("<table><tr>");

                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}{1}</th>", labels[i], footerMarkers[i]);
                bodyHTML.Append("</tr>");

                List<IBDNode> tableChildren = BDFabrik.GetChildrenForParent(pContext, table);
                foreach (IBDNode therapyGroup in tableChildren)
                {
                    BDTherapyGroup entry = therapyGroup as BDTherapyGroup;
                    bodyHTML.AppendFormat("<tr><td colspan=\"3\"><b>{0}</b></td></tr>",entry.Name);
                    List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, entry);
                    foreach (IBDNode therapy in therapies)
                    {
                        BDTherapy t = therapy as BDTherapy;
                        string marker = buildFooterMarkerForList(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, t.Uuid, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment), true);
                        bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}{2}</td><td>{3}</td></tr>",
                            t.Name, t.dosage, marker, t.dosage1);
                    }
                }
                bodyHTML.Append("</table>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, true);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
            
                List<IBDNode> surgeries = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode surgery in surgeries)
                {
                    List<string> labels = new List<string>();
                    // build markers and list for column header linked notes
                    labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDSurgery, BDNode.PROPERTYNAME_NAME));
                    List<string> footerMarkers = new List<string>();
                    footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true));

                    bodyHTML.AppendFormat("{0}{1}",buildNodeWithReferenceAndOverviewHTML(pContext, surgery, "h3"),footerMarkers[0]);

                    List<IBDNode> surgeryChildren = BDFabrik.GetChildrenForParent(pContext, surgery); 
                    foreach (IBDNode surgeryChild in surgeryChildren)
                    {
                        // surgery classification - owns the table row
                        if (surgeryChild.Name.Length > 0 && !surgeryChild.Name.Contains(BDUtilities.GetEnumDescription(surgeryChild.NodeType)))
                            bodyHTML.AppendFormat("<ul><li>{0}</ul>", surgeryChild.Name);

                        // build markers and list for column header linked notes
                        labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE));
                        labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1));

                        footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true));
                        footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true));

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
                                List<IBDNode> therapies = BDFabrik.GetChildrenForParent(pContext, therapyGroup);
                                #region process therapies
                                foreach (IBDNode t in therapies)
                                {
                                    BDTherapy therapy = t as BDTherapy;
                                    // therapy name - add to both cells
                                    if (therapy.nameSameAsPrevious.Value == true)
                                    {
                                        adultDosageHTML.AppendFormat("<li>{0}", buildTherapyPropertyHTML(pContext, therapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY));
                                        pedsDosageHTML.AppendFormat("<li>{0}", buildTherapyPropertyHTML(pContext, therapy, previousTherapyId, previousTherapyName, BDTherapy.PROPERTYNAME_THERAPY));
                                    }
                                    else
                                    {
                                        adultDosageHTML.AppendFormat("<li>{0}", buildTherapyPropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY));
                                        pedsDosageHTML.AppendFormat("<li>{0}", buildTherapyPropertyHTML(pContext, therapy, therapy.Uuid, therapy.Name, BDTherapy.PROPERTYNAME_THERAPY));
                                    }
                                    // Dosage - adult dose
                                    if (therapy.dosageSameAsPrevious.Value == true)
                                        adultDosageHTML.Append(buildTherapyPropertyHTML(pContext, therapy, previousTherapyId, previousTherapyDosage, BDTherapy.PROPERTYNAME_DOSAGE));
                                    else
                                        adultDosageHTML.Append(buildTherapyPropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage, BDTherapy.PROPERTYNAME_DOSAGE));

                                    // Dosage 1 - Paediatric dose
                                    if (therapy.dosage1SameAsPrevious.Value == true)
                                        pedsDosageHTML.Append(buildTherapyPropertyHTML(pContext, therapy, previousTherapyId, previousTherapyDosage1, BDTherapy.PROPERTYNAME_DOSAGE_1));
                                    else
                                        pedsDosageHTML.Append(buildTherapyPropertyHTML(pContext, therapy, therapy.Uuid, therapy.dosage1, BDTherapy.PROPERTYNAME_DOSAGE_1));

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
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
        }

        private string buildTherapyPropertyHTML(Entities pContext, BDTherapy pTherapy, Guid pNoteParentId, string pPropertyValue, string pPropertyName)
        {
            StringBuilder propertyHTML = new StringBuilder();
            List<BDLinkedNote> propertyFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, pPropertyName, BDConstants.LinkedNoteType.Footnote));
            string footerMarker = buildFooterMarkerForList(propertyFooters, true);

            List<BDLinkedNote> inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNoteParentId, pPropertyName, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage notePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, inline, marked, unmarked);

            if (notePage != null)
            {
                if (pPropertyValue.Length > 0 && !pPropertyValue.Contains("New Therapy"))
                    propertyHTML.AppendFormat(@" <a href=""{0}"">{1}</a>{2}", notePage.Uuid, pPropertyValue.Trim(), footerMarker);
                else
                    propertyHTML.AppendFormat(@" <a href=""{0}"">See Comments.</a>", notePage.Uuid);
            }
            else
                propertyHTML.AppendFormat(@" {0}{1}", pPropertyValue.Trim(), footerMarker);
            return propertyHTML.ToString();
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode subcategory in childNodes)
            {
                bodyHTML.Append("<p>");
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, subcategory, "h4"));
                List<IBDNode> mGroups = BDFabrik.GetChildrenForParent(pContext, subcategory);
                foreach (IBDNode group in mGroups)
                {
                    bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, group, "b"));
                    bodyHTML.Append("<br>");
                    List<IBDNode> microorganisms = BDFabrik.GetChildrenForParent(pContext, group);
                    foreach (IBDNode microorganism in microorganisms)
                    {
                        bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, microorganism, ""));
                        bodyHTML.Append("<br>");
                    }
                }
                bodyHTML.Append("</p>");
            }
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
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
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            bodyHTML.Append(buildAttachmentHTML(pContext, pNode));
            return writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data);
        }

        #region Standalone HTML pages
        private BDHtmlPage generatePageForOverview(Entities pContext, IBDNode pNode)
        {
            BDNode parentNode = pNode as BDNode;
            string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (null != parentNode && noteText.Length > EMPTY_PARAGRAPH)
            {
                return writeBDHtmlPage(pContext, parentNode, noteText, BDConstants.BDHtmlPageType.Overview);
            }
            return null;
        }

        private BDHtmlPage generatePageForLinkedNotes(Entities pContext, Guid pParentId, BDConstants.BDNodeType pParentType, List<BDLinkedNote> pInlineNotes, List<BDLinkedNote> pMarkedNotes, List<BDLinkedNote> pUnmarkedNotes)
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
                    }
                }
            }

            return generatePageForLinkedNotes(pContext, pParentId, pParentType, noteHtml.ToString(), BDConstants.BDHtmlPageType.Comments);
        }

        private BDHtmlPage generatePageForLinkedNotes(Entities pContext, Guid pDisplayParentId, BDConstants.BDNodeType pDisplayParentType, string pPageHtml, BDConstants.BDHtmlPageType pPageType)
        {
            if (pPageHtml.Length > EMPTY_PARAGRAPH)
                return writeBDHtmlPage(pContext, pDisplayParentId, pDisplayParentType, pPageHtml, pPageType, true);
            else
                return null;
        }

        /// <summary>
        /// Create an HTML page for the footnote attached to a node & property
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns>Guid of HTML page.</returns>
        private BDHtmlPage generatePageForParentAndPropertyFootnote(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            string footnoteText = buildTextForParentAndPropertyFromLinkedNotes(pContext, pPropertyName, pNode, BDConstants.LinkedNoteType.Footnote);
            BDHtmlPage footnote = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, footnoteText, BDConstants.BDHtmlPageType.Footnote);
            return footnote;
        }

        /// <summary>
        /// Create an HTML page for the references attached to a node & property
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns>Guid of HTML page.</returns>
        /// 
        private Guid generatePageForParentAndPropertyReferences(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            string reference = buildTextForParentAndPropertyFromLinkedNotes(pContext, pPropertyName, pNode, BDConstants.LinkedNoteType.Reference);
            if (reference.Length > EMPTY_PARAGRAPH)
            {
                StringBuilder referenceText = new StringBuilder();
                referenceText.AppendFormat(@"<h2>{0} References</h2>", pNode.Name);
                referenceText.Append(reference);
                BDHtmlPage footnote = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, referenceText.ToString(), BDConstants.BDHtmlPageType.Reference);
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
        /// <param name="pPropertyName"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <param name="pNoteType"></param>
        /// <returns>Text of linked note as HTML</returns>
        private string buildTextForParentAndPropertyFromLinkedNotes(Entities pContext, string pPropertyName, IBDNode pNode, BDConstants.LinkedNoteType pNoteType)
        {
            StringBuilder notesHTML = new StringBuilder();
            List<BDLinkedNote> notes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, pNoteType);
            notesHTML.Append(buildTextFromNotes(notes));
            return notesHTML.ToString();
        }

        /// <summary>
        /// Build HTML for Empiric Therapy beginning at pathogenGroup level
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pFooterList"></param>
        /// <returns></returns>
        private StringBuilder buildEmpiricTherapyHTML(Entities pContext, BDNode pNode)
        {
             StringBuilder bodyHtml = new StringBuilder();
                bodyHtml.Append(buildPathogenGroupHtml(pContext, pNode));

                // process therapy groups
                List<BDTherapyGroup> therapyGroups = BDTherapyGroup.RetrieveTherapyGroupsForParentId(pContext, pNode.Uuid);

                foreach (BDTherapyGroup tGroup in therapyGroups)
                {
                    BDTherapyGroup group = tGroup as BDTherapyGroup;
                    if (null != group)
                    {
                        bodyHtml.Append(buildTherapyGroupWithLinkedNotesHtml(pContext, group));
                        bodyHtml.Append(buildTherapyGroupHTML(pContext, group));
                }
            }
            return bodyHtml;
        }

        private StringBuilder buildTherapyGroupHTML(Entities pContext, BDTherapyGroup pTherapyGroup)
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
                    therapyHTML.Append(buildTherapyHtml(pContext, therapy));

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
                        if (therapy.dosageSameAsPrevious == false)
                            previousTherapyDosage = therapy.dosage;
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
        private string buildPathogenGroupHtml(Entities pContext, IBDNode pNode)
        {
            StringBuilder pathogenGroupHtml = new StringBuilder();

            BDNode pathogenGroup = pNode as BDNode;
            if (null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
            {
                // Get overview for Pathogen Group
                pathogenGroupHtml.Append(retrieveNoteTextForOverview(pContext, pathogenGroup.Uuid));

                List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

                List<BDLinkedNote> itemFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));
                string footerMarker = buildFooterMarkerForList(itemFooters, true);

                if (pNode.Name != null && pNode.Name.Length > 0)
                    pathogenGroupHtml.AppendFormat(@"<h2>{0}</h2>{1}", pNode.Name, footerMarker);

                pathogenGroupHtml.Append(buildTextFromNotes(inlineNotes));
                pathogenGroupHtml.Append(buildTextFromNotes(markedNotes));
                pathogenGroupHtml.Append(buildTextFromNotes(unmarkedNotes));

                List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(pContext, pathogenGroup);
                if (pathogens.Count > 0)
                    pathogenGroupHtml.Append(@"<h3>Usual Pathogens</h3>");

                foreach (IBDNode item in pathogens)
                {
                    if (item.NodeType == BDConstants.BDNodeType.BDPathogen)
                    {
                        BDNode node = item as BDNode;
                        if (null != node)
                            pathogenGroupHtml.Append(buildPathogenHtml(pContext, node));
                    }
                }
            }
            return pathogenGroupHtml.ToString();
        }

        private string buildPathogenHtml(Entities pContext, BDNode pPathogen)
        {
            StringBuilder pathogenHtml = new StringBuilder();
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

            List<BDLinkedNote> itemFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));
            string footerMarker = buildFooterMarkerForList(itemFooters, true);

            if (pPathogen.Name.Length > 0)
                pathogenHtml.AppendFormat(@"{0}{1}<br>", pPathogen.name, footerMarker);

            pathogenHtml.Append(buildTextFromNotes(inlineNotes));
            pathogenHtml.Append(buildTextFromNotes(markedNotes));
            pathogenHtml.Append(buildTextFromNotes(unmarkedNotes));

            // TODO:  DEAL WITH ENDNOTES

            return pathogenHtml.ToString();
        }

        /// <summary>
        /// Generate HTML for Therapy Group Name, with all linked note types gathered into a paragraph that follows the name
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pTherapyGroup"></param>
        /// <returns></returns>
        private string buildTherapyGroupWithLinkedNotesHtml(Entities pContext, BDTherapyGroup pTherapyGroup)
        {
            StringBuilder therapyGroupHtml = new StringBuilder();
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

            List<BDLinkedNote> itemFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));
            string footerMarker = buildFooterMarkerForList(itemFooters, true);

            // MAY NEED THIS if we decide to generate a page for the allNotes rather than putting them in-line.
            //if (notesListHasContent(pContext, pNotes))
            //{
            //    Guid noteGuid = generatePageForLinkedNotes(pContext, pDisplayParentNode.Uuid, pDisplayParentNode.NodeType, pNotes, new List<BDLinkedNote>(), new List<BDLinkedNote>());
            //    therapyGroupHtml.AppendFormat(@"<h4><a href""{0}"">{1}</a></h4>", noteGuid, tgName);
            //}
            // else if (pDisplayParentNode.Name.Length > 0)

            if (pTherapyGroup.Name.Length > 0 && !pTherapyGroup.Name.Contains("New Therapy Group"))
                therapyGroupHtml.AppendFormat(@"<h4>{0}</h4>{1}", pTherapyGroup.Name, footerMarker);
            therapyGroupHtml.Append(retrieveNoteTextForOverview(pContext, pTherapyGroup.Uuid));
            therapyGroupHtml.Append(buildTextFromNotes(inlineNotes));
            therapyGroupHtml.Append(buildTextFromNotes(markedNotes));
            therapyGroupHtml.Append(buildTextFromNotes(unmarkedNotes));

            return therapyGroupHtml.ToString();
        }

        private string buildTherapyHtml(Entities pContext, BDTherapy pTherapy)
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

            Guid nameNoteParentId = Guid.Empty;
            string tName = string.Empty;

            if (pTherapy.nameSameAsPrevious.Value == true)
            {
                nameNoteParentId = previousTherapyId;
                tName = previousTherapyName;
            }
            else
            {
                nameNoteParentId = pTherapy.Uuid;
                tName = pTherapy.Name;
            }

            List<BDLinkedNote> tNameInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tNameMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tNameUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.UnmarkedComment);

            List<BDLinkedNote> therapyNameFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Footnote));
            string therapyNameFooterMarker = buildFooterMarkerForList(therapyNameFooters, true);

            BDHtmlPage tNameNotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tNameInline, tNameMarked, tNameUnmarked);
            if (tNameNotePage != null)
            {
                if (tName.Length > 0)
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>{1}</b></a>", tNameNotePage.Uuid, tName);
                else
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>See Notes.</b></a>", tNameNotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<b>{0}</b>", tName);

            therapyHtml.Append(therapyNameFooterMarker);
            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(@"&#93");

            therapyHtml.Append(@"</td>");

            // Dosage
            Guid dosageNoteParentId = Guid.Empty;
            string tDosage = string.Empty;

            if (pTherapy.dosageSameAsPrevious.Value == true)
            {
                dosageNoteParentId = previousTherapyId;
                tDosage = previousTherapyDosage;
            }
            else
            {
                dosageNoteParentId = pTherapy.Uuid;
                tDosage = pTherapy.dosage;
            }

            List<BDLinkedNote> dosageFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Footnote));
            string dosageFooterMarker = buildFooterMarkerForList(dosageFooters, true);

            List<BDLinkedNote> tDosageInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosageMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosageUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage tDosageNotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosageInline, tDosageMarked, tDosageUnmarked);

            if (tDosageNotePage != null)
            {
                if (pTherapy.dosage.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a>{1}</td>", tDosageNotePage.Uuid, tDosage.Trim(), dosageFooterMarker);
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See allNotes.</a></td>", tDosageNotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}{1}</td>", tDosage.Trim(), dosageFooterMarker);

            // Duration
            Guid durationNoteParentId = Guid.Empty;
            string tDuration = string.Empty;

            if (pTherapy.durationSameAsPrevious.Value == true)
            {
                durationNoteParentId = previousTherapyId;
                tDuration = previousTherapyDuration;
            }
            else
            {
                durationNoteParentId = pTherapy.Uuid;
                tDuration = pTherapy.duration;
            }

            List<BDLinkedNote> durationFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Footnote));
            string durationFooterMarker = buildFooterMarkerForList(durationFooters, true);

            List<BDLinkedNote> tDurationInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDurationMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDurationUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage tDurationNotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDurationInline, tDurationMarked, tDurationUnmarked);

            if (tDurationNotePage != null)
            {
                if (pTherapy.duration.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a>{1}</td>", tDurationNotePage.Uuid, tDuration.Trim(), durationFooterMarker);
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", tDurationNotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}{1}</td>", tDuration.Trim(), durationFooterMarker);

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

        private string buildTherapyWithTwoDosagesHtml(Entities pContext, BDTherapy pTherapy)
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

            Guid nameNoteParentId = Guid.Empty;
            string tName = string.Empty;

            if (pTherapy.nameSameAsPrevious.Value == true)
            {
                nameNoteParentId = previousTherapyId;
                tName = previousTherapyName;
            }
            else
            {
                nameNoteParentId = pTherapy.Uuid;
                tName = pTherapy.Name;
            }

            List<BDLinkedNote> therapyNameFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Footnote));
            string therapyNameFooterMarker = buildFooterMarkerForList(therapyNameFooters, true);

           List<BDLinkedNote> inlineName = ( retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Inline));
            List<BDLinkedNote> notesForName = new List<BDLinkedNote>();
            notesForName.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.MarkedComment));
            notesForName.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.UnmarkedComment));

            therapyHtml.AppendFormat(@"<b>{0}</b>{1}", tName, therapyNameFooterMarker);
            foreach (BDLinkedNote note in inlineName)
            {
                if (note.documentText.Length > EMPTY_PARAGRAPH)
                    therapyHtml.AppendFormat(@"<br>{0}", note.documentText);
            }

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(@"&#93");

            therapyHtml.Append(@"</td>");

            // Dosage 1
            Guid dosageNoteParentId = Guid.Empty;
            string tDosage = string.Empty;

            if (pTherapy.dosageSameAsPrevious.Value == true)
            {
                dosageNoteParentId = previousTherapyId;
                tDosage = previousTherapyDosage;
            }
            else
            {
                dosageNoteParentId = pTherapy.Uuid;
                tDosage = pTherapy.dosage;
            }

            List<BDLinkedNote> dosageFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Footnote));
            string dosageFooterMarker = buildFooterMarkerForList(dosageFooters, true);

            List<BDLinkedNote> tDosageInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosageMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosageUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage tDosageNotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosageInline, tDosageMarked, tDosageUnmarked);

            if (tDosageNotePage != null)
            {
                if (pTherapy.dosage.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a>{2}</td>", tDosageNotePage.Uuid, tDosage.Trim(), dosageFooterMarker);
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Comments.</a></td>", tDosageNotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}{1}</td>", tDosage.Trim(), dosageFooterMarker);

            // Dosage 1
            Guid dosage1NoteParentId = Guid.Empty;
            string tDosage1 = string.Empty;

            if (pTherapy.dosage1SameAsPrevious.Value == true)
            {
                dosage1NoteParentId = previousTherapyId;
                tDosage1 = previousTherapyDosage1;
            }
            else
            {
                dosage1NoteParentId = pTherapy.Uuid;
                tDosage1 = pTherapy.dosage1;
            }

            List<BDLinkedNote> dosage1Footers = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosage1NoteParentId, BDTherapy.PROPERTYNAME_DOSAGE_1, BDConstants.LinkedNoteType.Footnote));
            string dosage1FooterMarker = buildFooterMarkerForList(dosage1Footers, true);

            List<BDLinkedNote> tDosage1Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosage1NoteParentId, BDTherapy.PROPERTYNAME_DOSAGE_1, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosage1Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosage1NoteParentId, BDTherapy.PROPERTYNAME_DOSAGE_1, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosage1Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosage1NoteParentId, BDTherapy.PROPERTYNAME_DOSAGE_1, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage tDosage1NotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosage1Inline, tDosage1Marked, tDosage1Unmarked);

            if (tDosage1NotePage != null)
            {
                if (pTherapy.dosage1.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a>{2}</td>", tDosage1NotePage.Uuid, tDosage1.Trim(), dosage1FooterMarker);
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Comments.</a></td>", tDosage1NotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}{1}</td>", tDosage1.Trim(), dosage1FooterMarker);

            // Duration
            Guid durationNoteParentId = Guid.Empty;
            string tDuration = string.Empty;

            if (pTherapy.durationSameAsPrevious.Value == true)
            {
                durationNoteParentId = previousTherapyId;
                tDuration = previousTherapyDuration;
            }
            else
            {
                durationNoteParentId = pTherapy.Uuid;
                tDuration = pTherapy.duration;
            }
            if (tDuration.Length > 0)
            {
                List<BDLinkedNote> durationFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Footnote));
                string durationFooterMarker = buildFooterMarkerForList(durationFooters, true);

                List<BDLinkedNote> tDurationInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> tDurationMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> tDurationUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.UnmarkedComment);

                BDHtmlPage tDurationNotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDurationInline, tDurationMarked, tDurationUnmarked);

                if (tDurationNotePage != null)
                {
                    if (pTherapy.duration.Length > 0)
                        therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a>{2}</td>", tDurationNotePage.Uuid, tDuration.Trim(), durationFooterMarker);
                    else
                        therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", tDurationNotePage.Uuid);
                }
                else
                    therapyHtml.AppendFormat(@"<td>{0}{1}</td>", tDuration.Trim(), durationFooterMarker);
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

        private string buildTherapyWithTwoDurationsHtml(Entities pContext, BDTherapy pTherapy)
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

            Guid nameNoteParentId = Guid.Empty;
            string tName = string.Empty;

            if (pTherapy.nameSameAsPrevious.Value == true)
            {
                nameNoteParentId = previousTherapyId;
                tName = previousTherapyName;
            }
            else
            {
                nameNoteParentId = pTherapy.Uuid;
                tName = pTherapy.Name;
            }

            List<BDLinkedNote> therapyNameFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Footnote));
            string therapyNameFooterMarker = buildFooterMarkerForList(therapyNameFooters, true);

            List<BDLinkedNote> tNameInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tNameMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tNameUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage tNameNotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tNameInline, tNameMarked, tNameUnmarked);
            if (tNameNotePage != null)
            {
                if (tName.Length > 0)
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>{1}</b>{2}</a>", tNameNotePage.Uuid, tName, therapyNameFooterMarker);
                else
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>See Notes.</b></a>", tNameNotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<b>{0}{1}</b>", tName, therapyNameFooterMarker);

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(@"&#93");

            therapyHtml.Append(@"</td>");

            // Dosage
            Guid dosageNoteParentId = Guid.Empty;
            string tDosage = string.Empty;

            if (pTherapy.dosageSameAsPrevious.Value == true)
            {
                dosageNoteParentId = previousTherapyId;
                tDosage = previousTherapyDosage;
            }
            else
            {
                dosageNoteParentId = pTherapy.Uuid;
                tDosage = pTherapy.dosage;
            }

            List<BDLinkedNote> dosageFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Footnote));
            string dosageFooterMarker = buildFooterMarkerForList(dosageFooters, true);

            List<BDLinkedNote> tDosageInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosageMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosageUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage tDosageNotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosageInline, tDosageMarked, tDosageUnmarked);

            if (tDosageNotePage != null)
            {
                if (pTherapy.dosage.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a>{2}</td>", tDosageNotePage.Uuid, tDosage.Trim(), dosageFooterMarker);
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Comments.</a></td>", tDosageNotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}{1}</td>", tDosage.Trim(), dosageFooterMarker);

            // Duration
            Guid durationNoteParentId = Guid.Empty;
            string tDuration = string.Empty;

            if (pTherapy.durationSameAsPrevious.Value == true)
            {
                durationNoteParentId = previousTherapyId;
                tDuration = previousTherapyDuration;
            }
            else
            {
                durationNoteParentId = pTherapy.Uuid;
                tDuration = pTherapy.duration;
            }

            List<BDLinkedNote> durationFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Footnote));
            string durationFooterMarker = buildFooterMarkerForList(durationFooters, true);
            
            List<BDLinkedNote> tDurationInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDurationMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDurationUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage tDurationNotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDurationInline, tDurationMarked, tDurationUnmarked);

            if (tDurationNotePage != null)
            {
                if (pTherapy.duration.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDurationNotePage.Uuid, tDuration.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", tDurationNotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", tDuration.Trim());

            // Duration
            Guid duration1NoteParentId = Guid.Empty;
            string tDuration1 = string.Empty;

            if (pTherapy.duration1SameAsPrevious.Value == true)
            {
                duration1NoteParentId = previousTherapyId;
                tDuration1 = previousTherapyDuration1;
            }
            else
            {
                duration1NoteParentId = pTherapy.Uuid;
                tDuration1 = pTherapy.duration1;
            }

            List<BDLinkedNote> duration1Footers = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION_1, BDConstants.LinkedNoteType.Footnote));
            string duration1FooterMarker = buildFooterMarkerForList(duration1Footers, true);

            List<BDLinkedNote> tDuration1Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION_1, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDuration1Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION_1, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDuration1Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, durationNoteParentId, BDTherapy.PROPERTYNAME_DURATION_1, BDConstants.LinkedNoteType.UnmarkedComment);

            BDHtmlPage tDuration1NotePage = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDuration1Inline, tDuration1Marked, tDuration1Unmarked);

            if (tDuration1NotePage != null)
            {
                if (pTherapy.duration1.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a>{2}</td>", tDuration1NotePage.Uuid, tDuration1.Trim(), duration1FooterMarker);
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", tDuration1NotePage.Uuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}{1}</td>", tDuration1.Trim(),duration1FooterMarker);

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

        /// <summary>
        /// Generate table row HTML for BDDosage
        /// Includes generation of footer markers, and addition of footers to list
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <returns></returns>
        private string buildDosageWithCostHTML(Entities pContext, IBDNode pNode)
        {
            BDDosage dosageNode = pNode as BDDosage;
            StringBuilder dosageHTML = new StringBuilder();
            string styleString = string.Empty;

            dosageHTML.Append(@"<tr><td>");
            // dosageNode 1
            List<BDLinkedNote> d1Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d1Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d1Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            List<BDLinkedNote> d1Footers = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Footnote));
            string d1FooterMarker = buildFooterMarkerForList(d1Footers, true);
            
            BDHtmlPage d1NotePage = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d1Inline, d1Marked, d1Unmarked);
            if (d1NotePage != null)
            {
                if (dosageNode.dosage.Length > 0)
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a>{2}</td>", d1NotePage.Uuid, dosageNode.dosage, d1FooterMarker);
                else
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", d1NotePage.Uuid);
            }
            else
                dosageHTML.AppendFormat("<td>{0}{1}</td>", dosageNode.dosage,d1FooterMarker);

            List<BDLinkedNote> costFooters = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_COST, BDConstants.LinkedNoteType.Footnote));
            string costFooterMarker = buildFooterMarkerForList(costFooters, true);
            
            dosageHTML.AppendFormat(@"<td>{0}{1}</td>", dosageNode.cost, costFooterMarker);

            return dosageHTML.ToString();
        }

        private string buildCellHTML(Entities pContext, IBDNode pCellParentNode, string pPropertyName, string pPropertyValue, bool includeCellTags)
        {
            StringBuilder cellHTML = new StringBuilder();
            string styleString = string.Empty;

            if(includeCellTags)
                cellHTML.Append(@"<td>");
            
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pCellParentNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pCellParentNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pCellParentNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.UnmarkedComment);
            List<BDLinkedNote> footerNotes = (retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pCellParentNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Footnote));
            string cellFooterMarker = buildFooterMarkerForList(footerNotes, true);

            BDHtmlPage notePage = generatePageForLinkedNotes(pContext, pCellParentNode.Uuid, pCellParentNode.NodeType, inlineNotes, markedNotes, unmarkedNotes);
            if (notePage != null)
            {
                if (pPropertyValue.Length > 0)
                    cellHTML.AppendFormat(@"<a href=""{0}"">{1}</a>{2}", notePage.Uuid, pPropertyValue, cellFooterMarker);
                else
                    cellHTML.AppendFormat(@"<a href=""{0}"">See Notes.</a>", notePage.Uuid);
            }
            else
                cellHTML.AppendFormat("{0}{1}", pPropertyValue, cellFooterMarker);
            if (includeCellTags)
                cellHTML.Append("</td>");
            return cellHTML.ToString();
        }

        private string buildDosageHTML(Entities pContext, IBDNode pNode, string pDosageGroupName)
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
            string d2FooterMarker = buildFooterMarkerForList(d2Footers, true);

            BDHtmlPage d2NotePage = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d2Inline, d2Marked, d2Unmarked);
            if (d2NotePage != null)
            {
                if (dosageNode.dosage2.Length > 0)
                    dosageHTML.AppendFormat(@"<td {0}><a href=""{1}"">{2}</a{3}>", colSpanTag, d2NotePage.Uuid, dosageNode.dosage2, d2FooterMarker);
                else
                dosageHTML.AppendFormat(@"<td {0}><a href=""{1}"">See Notes.</a>{3}", colSpanTag, d2NotePage.Uuid,d2FooterMarker);
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
                string d3FooterMarker = buildFooterMarkerForList(d3Footers, true);

                BDHtmlPage d3NotePage = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d3Inline, d3Marked, d3Unmarked);
                if (d3NotePage != null)
                {
                    if (dosageNode.dosage3.Length > 0)
                        dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a{2}>", d3NotePage.Uuid, dosageNode.dosage3, d3FooterMarker);
                    else
                        dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a>{3}", d3NotePage.Uuid, d3FooterMarker);
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
                string d4FooterMarker = buildFooterMarkerForList(d4Footers, true);

                BDHtmlPage d4NotePage = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d4Inline, d4Marked, d4Unmarked);
                if (d4NotePage != null)
                {
                    if (dosageNode.dosage4.Length > 0)
                        dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a>{2}", d4NotePage.Uuid, dosageNode.dosage4);
                    else
                        dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a>{3}", d4NotePage.Uuid, d4FooterMarker);
                }
                else
                    dosageHTML.AppendFormat("<td>{0}{1}", dosageNode.dosage4, d4FooterMarker);

                dosageHTML.Append(@"</td>");
            }
            return dosageHTML.ToString();
        }

        private string buildNodeWithReferenceAndOverviewHTML(Entities pContext, IBDNode pNode, string pHeaderTagLevel)
        {
            // footnotes
            List<BDLinkedNote> itemFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote);
            string itemFooterMarker = buildFooterMarkerForList(itemFooters, true);

            string generatedNodeName = String.Format("New {0}", BDUtilities.GetEnumDescription(pNode.NodeType));
            StringBuilder nodeHTML = new StringBuilder();
            if (pNode.Name.Length > 0 && !pNode.Name.Contains(generatedNodeName) && pNode.Name != "SINGLE PRESENTATION")
            {
                if (pHeaderTagLevel.Length > 0)
                    nodeHTML.AppendFormat(@"<{0}>{1}</{2}>{3}", pHeaderTagLevel, pNode.Name, pHeaderTagLevel, itemFooterMarker);
                else
                    nodeHTML.AppendFormat(@"{0}{1}",pNode.Name,itemFooterMarker);
            }
            nodeHTML.Append(buildReferenceHtml(pContext, pNode));

            // overview
            string overviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHTML.Length > EMPTY_PARAGRAPH)
                nodeHTML.Append(overviewHTML);

            return nodeHTML.ToString();
        }

        private string buildTextFromNotes(List<BDLinkedNote> pNotes)
        {
            StringBuilder noteString = new StringBuilder();
            foreach (BDLinkedNote note in pNotes)
            {
                if (note.documentText.Length > 0)
                    noteString.Append(note.documentText);
            }
            return noteString.ToString();
        }

        private string buildTableRowHtml(Entities pContext, BDTableRow pRow, bool forceHeaderRow, bool markFooterAtEnd)
        {
            StringBuilder tableRowHTML = new StringBuilder();
            string startCellTag = @"<td>";
            string endCellTag = @"</td>";
            string firstCellStartTag = @"<td colspan=""3"">";
            if (pRow != null)
            {
                if (BDFabrik.RowIsHeaderRow(pRow) || forceHeaderRow)
                {
                    startCellTag = @"<th>";
                    endCellTag = @"</th>";
                    firstCellStartTag = @"<th colspan=""3"">";
                }
                tableRowHTML.Append(@"<tr>");
                List<BDTableCell> cells = BDTableCell.RetrieveTableCellsForParentId(pContext, pRow.Uuid);
                for (int i = 0; i < cells.Count; i++)
                {
                    BDTableCell tableCell = cells[i];
                    string startTag = startCellTag;
                    if (i == 0 && pRow.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I_ContentRow)
                        startTag = firstCellStartTag;
                    
                    List<BDLinkedNote> itemFooters = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, tableCell.Uuid, BDTableCell.PROPERTYNAME_CONTENTS, BDConstants.LinkedNoteType.Footnote);
                    if (itemFooters.Count == 0)
                        tableRowHTML.AppendFormat(@"{0}{1}{2}", startTag, tableCell.value, endCellTag);
                    else
                    {
                       string itemFooterMarker = buildFooterMarkerForList(itemFooters, true);

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

        private string buildReferenceHtml(Entities pContext, IBDNode pNode)
        {
            StringBuilder refHTML = new StringBuilder();

            List<BDHtmlPage> referencePages = BDHtmlPage.RetrieveHtmlPageForDisplayParentIdOfPageType(pContext, pNode.Uuid, BDConstants.BDHtmlPageType.Reference);
            foreach (BDHtmlPage refPage in referencePages)
            {
                if (refPage.documentText.Length > EMPTY_PARAGRAPH)
                    refHTML.AppendFormat(@"<br><a href=""{0}"">References</a>", refPage.Uuid);
            }

            return refHTML.ToString();
        }

        private string buildFooterMarkerForList(List<BDLinkedNote> pItemFooterList, bool addToPageFooter)
        {
            StringBuilder footerMarker = new StringBuilder();
            if (pItemFooterList.Count > 0)
            {
                footerMarker.Append(@"<sup>");
                foreach (BDLinkedNote footer in pItemFooterList)
                {
                    if (!pageFooterList.Contains(footer) && addToPageFooter)
                        pageFooterList.Add(footer);

                    footerMarker.AppendFormat(@"{0},", pItemFooterList.IndexOf(footer) + 1);
                }
                // trim last comma
                if(footerMarker.Length > 5)
                    footerMarker.Remove(footerMarker.Length - 1, 1);
                footerMarker.Append("</sup>");
                return footerMarker.ToString();
            }
            return string.Empty;
        }

        private string buildAttachmentHTML(Entities pContext, IBDNode pNode)
        {
            StringBuilder attHtml = new StringBuilder();
            attHtml.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h4"));

            BDAttachment attachmentNode = pNode as BDAttachment;

            attHtml.AppendFormat(imgFileTag, imgFileFolder, attachmentNode.filename);

            return attHtml.ToString();
        }

        private string buildAntibioticsCSFPenetrationDosagesHTML(Entities pContext, IBDNode pNode)
        {
            // Table to ConfiguredEntry
            metadataLayoutColumns = BDLayoutMetadataColumn.RetrieveListForLayout(pContext, pNode.LayoutVariant);
            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));  // BDTable

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                List<string> labels = new List<string>();
                // build markers and list for column header linked notes
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01));
                labels.Add(retrieveMetadataLabelForPropertyName(pContext, BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02));

                List<string> footerMarkers = new List<string>();
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[0]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[1]), true));
                footerMarkers.Add(buildFooterMarkerForList(retrieveNotesForLayoutColumn(pContext, metadataLayoutColumns[2]), true));
                bodyHTML.Append("<table><tr>");
                for (int i = 0; i < metadataLayoutColumns.Count; i++)
                    bodyHTML.AppendFormat("<th>{0}{1}</th>", labels[i], footerMarkers[i]);
                bodyHTML.Append("</tr>");
                foreach (IBDNode child in childNodes)
                {
                    BDConfiguredEntry entry = child as BDConfiguredEntry;
                    bodyHTML.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", entry.Name, entry.field01, entry.field02);
                }
                bodyHTML.Append("</table>");
            }
            return bodyHTML.ToString();
        }

        #endregion

        #region Utility methods
        /// <summary>
        /// Retrieve linked note text for Overview of a node
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentId"></param>
        /// <param name="pPropertyName"></param>
        /// <returns></returns>
        private string retrieveNoteTextForOverview(Entities pContext, Guid pParentId)
        {
            string propertyName = BDNode.VIRTUALPROPERTYNAME_OVERVIEW;
            StringBuilder linkedNoteHtml = new StringBuilder();
            List<BDLinkedNoteAssociation> list = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(pContext, pParentId, propertyName);
            foreach (BDLinkedNoteAssociation assn in list)
            {
                BDLinkedNote linkedNote = BDLinkedNote.GetLinkedNoteWithId(pContext, assn.linkedNoteId);
                if (null != linkedNote)
                    linkedNoteHtml.Append(linkedNote.documentText);
            }
            if (linkedNoteHtml.Length > EMPTY_PARAGRAPH)
                return linkedNoteHtml.ToString();
            else
                return "";
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
                        BDLinkedNote linkedNote = BDLinkedNote.GetLinkedNoteWithId(pContext, assn.linkedNoteId);
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

        private void processTextForInternalLinks(Entities pContext, BDHtmlPage pPage, List<Guid> pPageGuids)
        {
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
                        if (!guidString.Contains("http://www"))
                        {
                            Guid anchorGuid = new Guid(guidString);
                            // if the guid exists as an html page, dont change it...
                            if (!pPageGuids.Contains(anchorGuid))
                            {
                                // otherwise...   look up the guid as the property name in the linked note association table
                                Guid linkGuid = BDHtmlPage.RetrievePageIdForAnchorId(pContext, guidString);
                                if (linkGuid != Guid.Empty)
                                {
                                    string newText = pPage.documentText.Replace(anchorGuid.ToString(), linkGuid.ToString());
                                    pPage.documentText = newText;
                                }
                                BDHtmlPage.Save(pContext, pPage);
                            }
                            else
                            {
                                List<BDHtmlPage> pagesForNode = BDHtmlPage.RetrieveHtmlPageForDisplayParentId(pContext, pPage.Uuid);
                                if (pagesForNode.Count > 0)
                                {
                                    string newText = pPage.documentText.Replace(anchorGuid.ToString(), pagesForNode[0].Uuid.ToString());
                                    pPage.documentText = newText;
                                }
                                BDHtmlPage.Save(pContext, pPage);
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
                notes.Add(BDLinkedNote.GetLinkedNoteWithId(pContext, link.linkedNoteId));
            
            return notes;
        }

        private BDHtmlPage writeBDHtmlPage(Entities pContext, IBDNode pDisplayParentNode, StringBuilder pBodyHTML, BDConstants.BDHtmlPageType pPageType)
        {
            return writeBDHtmlPage(pContext, pDisplayParentNode, pBodyHTML.ToString(), pPageType, true);
        }

        private BDHtmlPage writeBDHtmlPage(Entities pContext, IBDNode pDisplayParentNode, StringBuilder pBodyHTML, BDConstants.BDHtmlPageType pPageType, bool pWithFootnotes)
        {
            return writeBDHtmlPage(pContext, pDisplayParentNode, pBodyHTML.ToString(), pPageType, pWithFootnotes);
        }

        private BDHtmlPage writeBDHtmlPage(Entities pContext, IBDNode pDisplayParentNode, string pBodyHTML, BDConstants.BDHtmlPageType pPageType)
        {
            return writeBDHtmlPage(pContext, pDisplayParentNode, pBodyHTML, pPageType, true);
        }
        private BDHtmlPage writeBDHtmlPage(Entities pContext, IBDNode pDisplayParentNode, string pBodyHTML, BDConstants.BDHtmlPageType pPageType, bool pWithFootnotes)
        {
            if(pDisplayParentNode == null)
                return writeBDHtmlPage(pContext, Guid.Empty, BDConstants.BDNodeType.Undefined, pBodyHTML, pPageType, pWithFootnotes);
            else
                return writeBDHtmlPage(pContext, pDisplayParentNode.Uuid, pDisplayParentNode.NodeType, pBodyHTML, pPageType, pWithFootnotes);
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
        private BDHtmlPage writeBDHtmlPage(Entities pContext, Guid pDisplayParentId, BDConstants.BDNodeType pDisplayParentType, string pBodyHTML, BDConstants.BDHtmlPageType pPageType, bool pWithFootnotes)
        {
            StringBuilder footerHTML = new StringBuilder();
            // insert footer text
            if (pageFooterList.Count > 0 && pWithFootnotes)
            {
                footerHTML.Append(@"<h4>Footnotes</h4>");
                footerHTML.Append(@"<ol>");

                foreach (BDLinkedNote note in pageFooterList)
                {
                    footerHTML.AppendFormat(@"<li>{0}</li>", note.documentText);
                }
                footerHTML.Append(@"</ol>");
                pBodyHTML = pBodyHTML + footerHTML.ToString();
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + pBodyHTML + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pDisplayParentType;
            newPage.displayParentId = pDisplayParentId;
            newPage.documentText = pageHtml;
            newPage.htmlPageType = (int)pPageType;
            BDHtmlPage.Save(pContext, newPage);

            return newPage;
        }
        #endregion
    }
}
