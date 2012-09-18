
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

        // create variables to hold data for 'same as previous' settings on Therapy
        string previousTherapyName = string.Empty;
        string previousTherapyDosage = string.Empty;
        string previousTherapyDosage1 = string.Empty;
        string previousTherapyDuration = string.Empty;
        string previousTherapyDuration1 = string.Empty;
        Guid previousTherapyId = Guid.Empty;
        bool therapiesHaveDosage = false;
        bool therapiesHaveDuration = false;

        public void Generate(BDNode pNode)
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

        private void generateOverviewAndChildrenForNode(Entities pContext, IBDNode pNode)
        {
            if (pNode.GetType() == typeof(BDNode))
                BDNavigationNode.CreateNavigationNodeFromBDNode(pContext, pNode as BDNode);
            if (!beginDetailPage(pContext, pNode))
            {
                string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid);
                if (noteText.Length > EMPTY_PARAGRAPH)
                    generatePageForOverview(pContext, pNode);


                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode child in children)
                {
                    generateOverviewAndChildrenForNode(pContext, child);
                }
            }
        }

        /// <summary>
        /// Check the node type and the layout variant to determine which page needs to be built.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns>Boolean to indicate that page is generated and thus to stop recursing through the node tree</returns>
        private bool beginDetailPage(Entities pContext, IBDNode pNode)
        {
            bool isPageGenerated = false;

                generatePageForParentAndPropertyReferences(pContext, BDNode.PROPERTYNAME_NAME, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            generatePageForAntibioticsDosingInRenalImpairment(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            generatePageForAntibioticsClinicalGuidelines(pContext, pNode as BDNode);
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
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            isPageGenerated = true;
                            // TODO:  build HTML wrapper around image tag?
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            generatePageForAntibioticsPharmacodynamics(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            generatePageForAntibioticDosingInHepaticImpairment(pContext, pNode as BDNode);
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
                                // if this disease has only one presentation, generate the HTML page at this level 
                                BDNode node = pNode as BDNode;
                                int childCount = BDNode.RetrieveChildCountForNode(pContext, node).Value;
                                if (node != null && childCount == 1)
                                {
                                    isPageGenerated = true;
                                    generatePageForEmpiricTherapyDisease(pContext, pNode as BDNode);
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
                            generatePageForEmpiricTherapyOfBCNE(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            generatePageForEmpiricTherapyOfParasitic(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                            generatePageForEmpiricTherapyOfCultureDirected(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                            generatePageForEmpiricTherapyOfCultureDirectedEndocarditis(pContext, pNode as BDNode);
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
                            generatePageForEmpiricTherapyOfCultureDirectedPeritonitis(pContext, pNode as BDNode);
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
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            // if the processing comes through here, then the Disease only has one Presentation 
                            // -> Disease and Presentation will be shown on the same HTML page
                            isPageGenerated = true;
                            generatePageForEmpiricTherapyPresentation(pContext, pNode as BDNode);
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                            generatePageForEmpiricTherapyOfCellulitisInExtremities(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            generatePageForEmpiricTherapyOfVesicularLesions(pContext, pNode as BDNode);
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
                            generatePageForEmpiricTherapyOfVesicularLesions(pContext, pNode as BDNode);
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
                            generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            //case BDConstants.LayoutVariantType.Table_5_Column:
                            generatePageForAntibioticsStepdown(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            generatePageForAntibioticsCSFPenetration(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            generatePageforAntibioticsBLactam(pContext, pNode as BDNode);
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
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                            isPageGenerated = true;
                            generatePageForAntibioticsDosingAndDailyCosts(pContext, pNode as BDNode);
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                            generatePageForEmpiricTherapyOfPneumonia(pContext, pNode as BDNode);
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
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            generatePageForAntibioticsNameListing(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        //case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                        //case BDConstants.LayoutVariantType.Table_5_Column:
                        //    generatePageForAntibioticsStepdown(pContext, pNode as BDNode);
                        //    isPageGenerated = true;
                        //    break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics:
                            generatePageForEmpiricTherapyOfEndocarditisPaediatrics(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                            // generate HTML
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
                            generatePageForAntibioticsClinicalGuidelinesSpectrum(pContext, pNode as BDNode);
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

        private void generatePages(BDNode pNode)
        {
            Entities dataContext = new Entities();
            List<BDNode> chapters = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDChapter);

            if (pNode == null)
            {
                foreach (BDNode chapter in chapters)
                {
                    generateOverviewAndChildrenForNode(dataContext, chapter);
                }
            }
            else
            {
                if (pNode.NodeType == BDConstants.BDNodeType.BDChapter)
                    generateOverviewAndChildrenForNode(dataContext, pNode);
            }

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

        private void generateStub(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));
            
            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        #region Antibiotics sections
        private void generatePageForAntibioticsClinicalGuidelines(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDAntimicrobial)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            // child nodes can either be pathogen groups or topics (node with overview)
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode node in childNodes)
            {
                if (node.NodeType == BDConstants.BDNodeType.BDTopic)
                {
                    if (node.Name.Length > 0)
                        bodyHTML.AppendFormat("<h2>{0}</h2>", node.Name);
                    string nodeOverviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid);
                    if (nodeOverviewHTML.Length > EMPTY_PARAGRAPH)
                    {
                        bodyHTML.Append(nodeOverviewHTML);
                    }
                }
            }
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForAntibioticsClinicalGuidelinesSpectrum(Entities pContext, BDNode pNode)
{
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTopic)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in childNodes)
            {
                bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, child as BDNode, "h3"));
            }
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
}

        private void generatePageForAntibioticsPharmacodynamics(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode node in childNodes)
            {
                if (node.NodeType == BDConstants.BDNodeType.BDAntimicrobialGroup)
                {
                    Guid footnoteGuid = generatePageForParentAndPropertyFootnote(pContext, BDNode.PROPERTYNAME_NAME, node as BDNode);
                    if (footnoteGuid == Guid.Empty)
                        bodyHTML.AppendFormat(@"{0}<br>", node.Name);
                    else
                        bodyHTML.AppendFormat(@"<a href=""{0}""><b>{1}</b></a><br>", footnoteGuid, node.Name);
                }
            }
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForAntibioticsDosingAndDailyCosts(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                bodyHTML.Append(@"<table><tr><th>Antimicrobial</th><th>Recommended Adult Dose</th><th>Cost ($)/Day</th></tr>");

                foreach (IBDNode node in childNodes)
                {
                    if (node.NodeType == BDConstants.BDNodeType.BDDosageGroup)
                    {
                        Guid footnoteGuid = generatePageForParentAndPropertyFootnote(pContext, BDNode.PROPERTYNAME_NAME, node);
                        if (footnoteGuid == Guid.Empty)
                            bodyHTML.AppendFormat(@"<tr><td>{0}</td><td /><td /></tr>", node.Name);
                        else
                            bodyHTML.AppendFormat(@"<tr><td><a href=""{0}""><b>{1}</b></a></td><td /><td /><tr>", footnoteGuid, node.Name);

                        List<IBDNode> dosages = BDFabrik.GetChildrenForParent(pContext, node);
                        foreach (IBDNode dosage in childNodes)
                            if (node.NodeType == BDConstants.BDNodeType.BDDosage)
                                bodyHTML.Append(buildDosageWithCostHTML(pContext, dosage));
                    }
                    else if (node.NodeType == BDConstants.BDNodeType.BDAntimicrobial)
                    {
                        Guid footnoteGuid = generatePageForParentAndPropertyFootnote(pContext, BDDosage.PROPERTYNAME_DOSAGE, node);
                        if (footnoteGuid == Guid.Empty)
                            bodyHTML.AppendFormat(@"<tr><td>{0}</td><td /><td /></tr>", node.Name);
                        else
                            bodyHTML.AppendFormat(@"<tr><td><a href=""{0}""><b>{1}</b></a></td><td /><td /><tr>", footnoteGuid, node.Name);

                        List<IBDNode> dosages = BDFabrik.GetChildrenForParent(pContext, node);
                        foreach (IBDNode dosage in dosages)
                            if (dosage.NodeType == BDConstants.BDNodeType.BDDosage)
                                bodyHTML.Append(buildDosageWithCostHTML(pContext, dosage));
                    }
                }
                bodyHTML.Append(@"</table>");
            }
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForAntibioticsDosingAndMonitoring(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection && pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
                        if (node.Name.Length > 0)
                            bodyHTML.AppendFormat(@"<h3>{0}</h3>", node.Name);

                        List<IBDNode> topicChildren = BDFabrik.GetChildrenForParent(pContext, node);
                        foreach (IBDNode topicChild in topicChildren)
                        {
                            if (topicChild.NodeType == BDConstants.BDNodeType.BDTable)
                            {
                                // insert node name (table name)
                                if (topicChild.Name.Length > 0)
                                    bodyHTML.AppendFormat(@"<h4>{0}</h4>", topicChild.Name);

                                List<IBDNode> rows = BDFabrik.GetChildrenForParent(pContext, topicChild);
                                if (rows.Count > 0)
                                {
                                    bodyHTML.Append(@"<table>");
                                    foreach (IBDNode child in rows)
                                    {
                                        BDTableRow row = child as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
                                    }
                                    bodyHTML.Append(@"</table>");
                                }
                            }
                            else if (topicChild.NodeType == BDConstants.BDNodeType.BDSubtopic)
                            {
                                if (topicChild.Name.Length > 0)
                                    bodyHTML.AppendFormat(@"<h4>{0}</h4>", topicChild.Name);
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
                                        if (subtopicChild.Name.Length > 0)
                                            bodyHTML.AppendFormat(@"<h4>{0}</h4>", subtopicChild.Name);

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
                                                        bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
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
                                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
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
                                // TODO how to build html, how to access picture from html
                            }
                        }
                    }
                }
            }
            // insert footer text
            // (footerList.Count > 0)
            //{
            //    footerHTML.Append(@"<h4>Footnotes</h4>");
            //    footerHTML.Append(@"<ol>");

            //    foreach (BDLinkedNote note in footerList)
            //    {
            //        footerHTML.AppendFormat(@"<li>{0}</li>", note.documentText);
            //    }
            //    footerHTML.Append(@"</ol>");
            //    bodyHTML.Append(footerHTML);
            //}
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForAntibioticsDosingInRenalImpairment(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDAntimicrobial)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));
            
            // child nodes are BDDosage 
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                BDDosage firstNode = childNodes[0] as BDDosage;
                List<BDLinkedNote> d1Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, firstNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> d1Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, firstNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> d1Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, firstNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

                bodyHTML.Append(@"<h3>Normal Adult Dose</h3><br>");
                Guid d1NotePageGuid = generatePageForLinkedNotes(pContext, firstNode.Uuid, BDConstants.BDNodeType.BDDosage, d1Inline, d1Marked, d1Unmarked);
                if (d1NotePageGuid != Guid.Empty)
                {
                    if (firstNode.dosage.Length > 0)
                        bodyHTML.AppendFormat(@"<a href=""{0}"">{1}</a>", d1NotePageGuid, firstNode.dosage);
                    else
                        bodyHTML.AppendFormat(@"<a href=""{0}"">See Notes.</a>", d1NotePageGuid);
                }
                else
                    bodyHTML.Append(firstNode.dosage);

                bodyHTML.Append(@"<table><tr><th colspan=""3""><b>Dose and Interval Adjustment for Renal Impairment</b></th></tr>");
                bodyHTML.Append(@"<tr><th>&gt50</th><th>10 - 50</th><th>&lt10(Anuric)</th></tr>");

                foreach (IBDNode node in childNodes)
                {
                    if (node.NodeType == BDConstants.BDNodeType.BDDosage)
                        bodyHTML.Append(buildDosageHTML(pContext, node));
                }
                bodyHTML.Append(@"</table>");

                writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
            }
        }

        private void generatePageForAntibioticDosingInHepaticImpairment(Entities pContext, BDNode pNode)
        {
            if (pNode.NodeType != BDConstants.BDNodeType.BDCategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForAntibioticsNameListing(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                bodyHTML.Append(@"<table>");
                foreach (IBDNode node in childNodes)
                {
                    if (node.NodeType == BDConstants.BDNodeType.BDTableRow)
                    {
                        BDTableRow row = node as BDTableRow;
                        if (row != null)
                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
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
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
                                    }
                                }
                                else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                {
                                    BDTableRow row = sectionChild as BDTableRow;
                                    if (row != null)
                                        bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForAntibioticsStepdown(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
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
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
                                        }
                                    }
                                    else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = sectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, false));
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
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, true));
                                        }
                                    }
                                    else if (subsectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = subsectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, false, footerList, false));
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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForAntibioticsCSFPenetration(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            // build data into a table; place footer at the bottom of the HTML page
            // TODO:  test creating a numbered link to the footer?

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                foreach (IBDNode child in childNodes)
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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageforAntibioticsBLactam(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSection)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
                        List<IBDNode> rows = BDFabrik.GetChildrenForParent(pContext, table);
                        if (rows.Count > 0)
                        {
                            bodyHTML.Append(@"<table>");
                            foreach (IBDNode row in rows)
                            {
                                BDTableRow tableRow = row as BDTableRow;
                                if (row != null)
                                    bodyHTML.Append(buildTableRowHtml(pContext, tableRow, false, footerList, true));
                            }
                            bodyHTML.Append(@"</table>");
                        }
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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }
        #endregion

        #region Treatment Recommendations sections
        /// <summary>
        /// Build HTML page at Disease level when only one Presentation is defined
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        private void generatePageForEmpiricTherapyDisease(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDDisease)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
                List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, presentation);
                foreach(IBDNode pathogenGroup in pathogenGroups)
                    bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footerList));

                writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
            }
        }

        /// <summary>
        /// Build HTML page to show Presentation and all it's children on one page
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pDisplayParentNode"></param>
        private void generatePageForEmpiricTherapyPresentation(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h2"));
            List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode pathogenGroup in pathogenGroups)
                bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footerList));

            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForEmpiricTherapyOfCellulitisInExtremities(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

            StringBuilder bodyHTML = new StringBuilder();
            StringBuilder footerHTML = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            bodyHTML.Append(buildNodeWithReferenceAndOverviewHTML(pContext, pNode, "h1"));

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach(IBDNode child in childNodes)
            {
                if(child.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
                    bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pNode as BDNode, footerList));
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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForEmpiricTherapyOfVesicularLesions(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
                            bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pathogenGroup as BDNode, footerList));
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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForEmpiricTherapyOfBCNE(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogen)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatepageForEmpiricTherapyOfGenitalUlcers(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable && pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
                writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
            }
            else if (pNode.NodeType == BDConstants.BDNodeType.BDPresentation)
            {
                generatePageForEmpiricTherapyPresentation(pContext, pNode);
            }
        }

        private void generatePageForEmpiricTherapyOfParasitic(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogen)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        /// <summary>
        /// Build page for CURB-65 - Pneumonia Severity of Illness Scoring System
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        private void generatePageForEmpiricTherapyOfPneumonia(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDSubcategory)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
                            bodyHTML.Append(buildTableRowHtml(pContext, childNodes[0] as BDTableRow, true, footerList, false));
                            for (int i = 1; i < childNodes.Count; i++)
                                bodyHTML.Append(buildTableRowHtml(pContext, childNodes[i] as BDTableRow, false, footerList, false));
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
                            bodyHTML.Append(buildTableRowHtml(pContext, row as BDTableRow, false, footerList, false));
                        else
                        {
                            List<IBDNode> sectionRows = BDFabrik.GetChildrenForParent(pContext, row);
                            foreach(IBDNode sectionRow in sectionRows)
                                bodyHTML.Append(buildTableRowHtml(pContext, sectionRow as BDTableRow, false, footerList, false));
                        }
                    }
                }
            }
            bodyHTML.Append(@"</table>");

            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        /// <summary>
        /// For culture-proven pneumonia & meningitis
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        private void generatePageForEmpiricTherapyOfCultureDirected(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogen)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        /// <summary>
        /// Build page at PathogenGroup downward
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        private void generatePageForEmpiricTherapyOfCultureDirectedPeritonitis(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogenGroup)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForEmpiricTherapyOfCultureDirectedEndocarditis(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPathogen)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }

        private void generatePageForEmpiricTherapyOfEndocarditisPaediatrics(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called from the wrong node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDTable)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }

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
            writeBDHtmlPage(pContext, pNode, bodyHTML, BDConstants.BDHtmlPageType.Data, footerList);
        }
        #endregion

        #region Standalone HTML pages
        private Guid generatePageForOverview(Entities pContext, IBDNode pNode)
        {
            BDNode parentNode = pNode as BDNode;
            string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (null != parentNode && noteText.Length > EMPTY_PARAGRAPH)
            {
                return writeBDHtmlPage(pContext, parentNode, noteText, BDConstants.BDHtmlPageType.Overview, new List<BDLinkedNote>());
            }
            return Guid.Empty;
        }

        private Guid generatePageForLinkedNotes(Entities pContext, Guid pParentId, BDConstants.BDNodeType pParentType, List<BDLinkedNote> pInlineNotes, List<BDLinkedNote> pMarkedNotes, List<BDLinkedNote> pUnmarkedNotes)
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

            Guid returnGuid = generatePageForLinkedNotes(pContext, pParentId, pParentType, noteHtml.ToString(), BDConstants.BDHtmlPageType.Comments);
            return returnGuid;
        }

        private Guid generatePageForLinkedNotes(Entities pContext, Guid pDisplayParentId, BDConstants.BDNodeType pDisplayParentType, string pPageHtml, BDConstants.BDHtmlPageType pPageType)
        {
            if (pPageHtml.Length > EMPTY_PARAGRAPH)
                return writeBDHtmlPage(pContext, pDisplayParentId, pDisplayParentType, pPageHtml, pPageType, new List<BDLinkedNote>());
            else
                return Guid.Empty;
        }

        /// <summary>
        /// Create an HTML page for the footnote attached to a node & property
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="pDisplayParentNode"></param>
        /// <returns>Buid of HTML page.</returns>
        private Guid generatePageForParentAndPropertyFootnote(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            string footnoteText = buildTextForParentAndPropertyFromLinkedNotes(pContext, pPropertyName, pNode, BDConstants.LinkedNoteType.Footnote);
            Guid footnoteId = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, footnoteText, BDConstants.BDHtmlPageType.Footnote);
            return footnoteId;
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
                Guid footnoteId = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, referenceText.ToString(), BDConstants.BDHtmlPageType.Reference);
                return footnoteId;
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
        private StringBuilder buildEmpiricTherapyHTML(Entities pContext, BDNode pNode, List<BDLinkedNote> pFooterList)
        {
             StringBuilder bodyHtml = new StringBuilder();
                bodyHtml.Append(buildPathogenGroupHtml(pContext, pNode, pFooterList));

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
            // therapy pTherapyGroup header
            // therapy pTherapyGroup linked allNotes:  inline, marked, unmarked
            // add footnote and endnote to footer
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
        private string buildPathogenGroupHtml(Entities pContext, IBDNode pNode, List<BDLinkedNote> pFooterList)
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

                pFooterList.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));

                if (pNode.Name != null && pNode.Name.Length > 0)
                    pathogenGroupHtml.AppendFormat(@"<h2>{0}</h2>", pNode.Name);

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
                        {
                            pathogenGroupHtml.Append(buildPathogenHtml(pContext, node));
                            pFooterList.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Footnote));
                        }
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

            if (pPathogen.Name.Length > 0)
                pathogenHtml.AppendFormat(@"{0}<br>", pPathogen.name);

            pathogenHtml.Append(buildTextFromNotes(inlineNotes));
            pathogenHtml.Append(buildTextFromNotes(markedNotes));
            pathogenHtml.Append(buildTextFromNotes(unmarkedNotes));

            // TODO:  DEAL WITH FOOTNOTES AND ENDNOTES

            return pathogenHtml.ToString();
        }

        private string buildTherapyGroupWithLinkedNotesHtml(Entities pContext, BDTherapyGroup pTherapyGroup)
        {
            StringBuilder therapyGroupHtml = new StringBuilder();
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

            // TODO:  DEAL WITH FOOTNOTES / ENDNOTES

            // MAY NEED THIS if we decide to generate a page for the allNotes rather than putting them in-line.
            //if (notesListHasContent(pContext, pNotes))
            //{
            //    Guid noteGuid = generatePageForLinkedNotes(pContext, pDisplayParentNode.Uuid, pDisplayParentNode.NodeType, pNotes, new List<BDLinkedNote>(), new List<BDLinkedNote>());
            //    therapyGroupHtml.AppendFormat(@"<h4><a href""{0}"">{1}</a></h4>", noteGuid, tgName);
            //}
            // else if (pDisplayParentNode.Name.Length > 0)

            if (pTherapyGroup.Name.Length > 0 && !pTherapyGroup.Name.Contains("New Therapy Group"))
                therapyGroupHtml.AppendFormat(@"<h4>{0}</h4>", pTherapyGroup.Name);
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

            // check join type - if none, then don't draw the bottom border on the table row
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

            List<BDLinkedNote> tNameInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tNameMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tNameUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tNameNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tNameInline, tNameMarked, tNameUnmarked);
            if (tNameNotePageGuid != Guid.Empty)
            {
                if (tName.Length > 0)
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>{1}</b></a>", tNameNotePageGuid, tName);
                else
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>See Notes.</b></a>", tNameNotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<b>{0}</b>", tName);

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

            List<BDLinkedNote> tDosageInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosageMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosageUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDosageNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosageInline, tDosageMarked, tDosageUnmarked);

            if (tDosageNotePageGuid != Guid.Empty)
            {
                if (pTherapy.dosage.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDosageNotePageGuid, tDosage.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See allNotes.</a></td>", tDosageNotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", tDosage.Trim());

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

            List<BDLinkedNote> tDurationInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDurationMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDurationUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDurationNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDurationInline, tDurationMarked, tDurationUnmarked);

            if (tDurationNotePageGuid != Guid.Empty)
            {
                if (pTherapy.duration.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDurationNotePageGuid, tDuration.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", tDurationNotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", tDuration.Trim());

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

            // check join type - if none, then don't draw the bottom border on the table row
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

           List<BDLinkedNote> inlineName = ( retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Inline));
            List<BDLinkedNote> notesForName = new List<BDLinkedNote>();
            notesForName.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.MarkedComment));
            notesForName.AddRange(retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.UnmarkedComment));

            therapyHtml.AppendFormat(@"<b>{0}</b>", tName);
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

            List<BDLinkedNote> tDosageInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosageMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosageUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDosageNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosageInline, tDosageMarked, tDosageUnmarked);

            if (tDosageNotePageGuid != Guid.Empty)
            {
                if (pTherapy.dosage.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDosageNotePageGuid, tDosage.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Comments.</a></td>", tDosageNotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", tDosage.Trim());

            // Dosage 2
            Guid dosage2NoteParentId = Guid.Empty;
            string tDosage1 = string.Empty;

            if (pTherapy.dosage1SameAsPrevious.Value == true)
            {
                dosage2NoteParentId = previousTherapyId;
                tDosage1 = previousTherapyDosage1;
            }
            else
            {
                dosage2NoteParentId = pTherapy.Uuid;
                tDosage1 = pTherapy.dosage1;
            }

            List<BDLinkedNote> tDosage1Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE_1, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosage1Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE_1, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosage1Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE_1, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDosage1NotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosage1Inline, tDosage1Marked, tDosage1Unmarked);

            if (tDosage1NotePageGuid != Guid.Empty)
            {
                if (pTherapy.dosage1.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDosage1NotePageGuid, tDosage1.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Comments.</a></td>", tDosage1NotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", tDosage1.Trim());

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

            List<BDLinkedNote> tDurationInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDurationMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDurationUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDurationNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDurationInline, tDurationMarked, tDurationUnmarked);

            if (tDurationNotePageGuid != Guid.Empty)
            {
                if (pTherapy.duration.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDurationNotePageGuid, tDuration.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", tDurationNotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", tDuration.Trim());

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

            // check join type - if none, then don't draw the bottom border on the table row
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

            List<BDLinkedNote> tNameInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tNameMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tNameUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tNameNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tNameInline, tNameMarked, tNameUnmarked);
            if (tNameNotePageGuid != Guid.Empty)
            {
                if (tName.Length > 0)
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>{1}</b></a>", tNameNotePageGuid, tName);
                else
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>See Notes.</b></a>", tNameNotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<b>{0}</b>", tName);

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

            List<BDLinkedNote> tDosageInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosageMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosageUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDosageNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosageInline, tDosageMarked, tDosageUnmarked);

            if (tDosageNotePageGuid != Guid.Empty)
            {
                if (pTherapy.dosage.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDosageNotePageGuid, tDosage.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Comments.</a></td>", tDosageNotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", tDosage.Trim());

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

            List<BDLinkedNote> tDurationInline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDurationMarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDurationUnmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDurationNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDurationInline, tDurationMarked, tDurationUnmarked);

            if (tDurationNotePageGuid != Guid.Empty)
            {
                if (pTherapy.duration.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDurationNotePageGuid, tDuration.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", tDurationNotePageGuid);
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

            List<BDLinkedNote> tDuration1Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION_1, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDuration1Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION_1, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDuration1Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION_1, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDuration1NotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDuration1Inline, tDuration1Marked, tDuration1Unmarked);

            if (tDuration1NotePageGuid != Guid.Empty)
            {
                if (pTherapy.duration1.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDuration1NotePageGuid, tDuration1.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", tDuration1NotePageGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", tDuration1.Trim());

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

            Guid d1NotePageGuid = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d1Inline, d1Marked, d1Unmarked);
            if (d1NotePageGuid != Guid.Empty)
            {
                if (dosageNode.dosage.Length > 0)
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", d1NotePageGuid, dosageNode.dosage);
                else
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", d1NotePageGuid);
            }
            else
                dosageHTML.AppendFormat("<td>{0}</td>", dosageNode.dosage);

            dosageHTML.AppendFormat(@"<td>{0}</td>", dosageNode.cost);

            return dosageHTML.ToString();
        }

        private string buildDosageHTML(Entities pContext, IBDNode pNode)
        {
            BDDosage dosageNode = pNode as BDDosage;
            StringBuilder dosageHTML = new StringBuilder();
            string styleString = string.Empty;
            string colSpanTag = @"colspan=1";

            dosageHTML.Append(@"<tr>");

            if (dosageNode.dosage.Contains(@"NO CHANGE NEEDED"))
            {
                if (dosageNode.dosage2 == dosageNode.dosage3 && dosageNode.dosage2 == dosageNode.dosage4)
                    colSpanTag = @"colspan=3";
                if (dosageNode.dosage2 == dosageNode.dosage3 && dosageNode.dosage2 != dosageNode.dosage4)
                    colSpanTag = @"colspan=2";
            }

            // Dosage 2
            List<BDLinkedNote> d2Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d2Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d2Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid d2NotePageGuid = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d2Inline, d2Marked, d2Unmarked);
            if (d2NotePageGuid != Guid.Empty)
            {
                if (dosageNode.dosage2.Length > 0)
                    dosageHTML.AppendFormat(@"<td {0}><a href=""{1}"">{2}</a>", colSpanTag, d2NotePageGuid, dosageNode.dosage2);
                else
                    dosageHTML.AppendFormat(@"<td {0}><a href=""{1}"">See Notes.</a>", colSpanTag, d2NotePageGuid);
            }
            else
                dosageHTML.AppendFormat(@"<td {0}>{1}", colSpanTag, dosageNode.dosage2);

            dosageHTML.Append(@"</td>");

            // Dosage 3
            List<BDLinkedNote> d3Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d3Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d3Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid d3NotePageGuid = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d3Inline, d3Marked, d3Unmarked);
            if (d3NotePageGuid != Guid.Empty && dosageNode.dosage2 != dosageNode.dosage3)
            {
                if (dosageNode.dosage3.Length > 0)
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a>", d3NotePageGuid, dosageNode.dosage3);
                else
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a>", d3NotePageGuid);
            }
            else
                dosageHTML.AppendFormat(@"<td>{0}", dosageNode.dosage3);

            dosageHTML.Append(@"</td>");

            // Dosage 4
            List<BDLinkedNote> d4Inline = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d4Marked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d4Unmarked = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid d4NotePageGuid = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d4Inline, d4Marked, d4Unmarked);
            if (d4NotePageGuid != Guid.Empty && dosageNode.dosage2 != dosageNode.dosage4)
            {
                if (dosageNode.dosage4.Length > 0)
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a>", d4NotePageGuid, dosageNode.dosage4);
                else
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a>", d4NotePageGuid);
            }
            else
                dosageHTML.AppendFormat(@"<td>{0}", dosageNode.dosage4);

            dosageHTML.Append(@"</td>");

            return dosageHTML.ToString();
        }

        private string buildNodeWithReferenceAndOverviewHTML(Entities pContext, BDNode pNode, string pHeaderTagLevel)
        {
            StringBuilder nodeHTML = new StringBuilder();
            if (pNode.Name.Length > 0 && !pNode.Name.Contains(@"New ") && pNode.Name != "SINGLE PRESENTATION")
                if(pHeaderTagLevel.Length > 0)
                    nodeHTML.AppendFormat(@"<{0}>{1}</{2}>",pHeaderTagLevel, pNode.Name,pHeaderTagLevel);
                else
                    nodeHTML.Append(pNode.Name);

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

        private string buildTableRowHtml(Entities pContext, BDTableRow pRow, bool forceHeaderRow, List<BDLinkedNote> pFooterList, bool markFooterAtEnd)
        {
            StringBuilder tableRowHTML = new StringBuilder();
            string startCellTag = @"<td>";
            string endCellTag = @"</td>";
            string firstCellStartTag = @"<td colspan=""3"">";
            if (pRow != null)
            {
                if (rowIsHeaderRow(pRow) || forceHeaderRow)
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
                    
                    List<BDLinkedNote> itemFooter = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, tableCell.Uuid, BDTableCell.PROPERTYNAME_CONTENTS, BDConstants.LinkedNoteType.Footnote);
                    if (itemFooter.Count == 0)
                        tableRowHTML.AppendFormat(@"{0}{1}{2}", startTag, tableCell.value, endCellTag);
                    else
                    {
                        StringBuilder footerMarker = new StringBuilder();
                        foreach (BDLinkedNote footer in itemFooter)
                        {
                            if (!pFooterList.Contains(footer))
                                pFooterList.Add(footer);

                            footerMarker.AppendFormat(@"<sup>{0}</sup>,", pFooterList.IndexOf(footer) + 1);
                        }
                        if (footerMarker.Length > 0)
                            footerMarker.Remove(footerMarker.Length - 1, 1);
                        if (markFooterAtEnd)
                            tableRowHTML.AppendFormat(@"{0}{1}{2}{3}", startTag, tableCell.value, footerMarker, endCellTag);
                        else
                        {
                            int lineBreakIndex = tableCell.value.Length;
                            if (tableCell.value.Contains("\n"))
                                lineBreakIndex = tableCell.value.IndexOf("\n");
                            string cellTextWithFooterTag = tableCell.value.Insert(lineBreakIndex, footerMarker.ToString());
                            tableRowHTML.AppendFormat(@"{0}{1}{2}", startTag, cellTextWithFooterTag, endCellTag);
                        }
                    }
                }
                tableRowHTML.Append(@"</tr>");
            }

            return tableRowHTML.ToString();
        }

        private string buildReferenceHtml(Entities pContext, BDNode pNode)
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

        private bool rowIsHeaderRow(IBDNode pNode)
        {
            bool isHeader = false;
            BDTableRow row = pNode as BDTableRow;
            if (row != null)
            {
                switch (row.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_NameListing_HeaderRow:
                    case BDConstants.LayoutVariantType.Antibiotics_Stepdown_HeaderRow:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass_HeaderRow:
                    case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II_HeaderRow:
                    case BDConstants.LayoutVariantType.Prophylaxis_PreOp_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_2_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_3_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_4_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_5_Column_HeaderRow:
                        isHeader = true;
                        break;
                    default:
                        isHeader = false;
                        break;
                }
            }
            return isHeader;
        }

        private static Guid writeBDHtmlPage(Entities pContext, BDNode pDisplayParentNode, StringBuilder pBodyHTML, BDConstants.BDHtmlPageType pPageType, List<BDLinkedNote> pFooterList)
        {
            return writeBDHtmlPage(pContext, pDisplayParentNode, pBodyHTML.ToString(), pPageType, pFooterList);
        }

        private static Guid writeBDHtmlPage(Entities pContext, BDNode pDisplayParentNode, string pBodyHTML, BDConstants.BDHtmlPageType pPageType, List<BDLinkedNote> pFooterList)
        {
            return writeBDHtmlPage(pContext, pDisplayParentNode.uuid, pDisplayParentNode.NodeType, pBodyHTML, pPageType, pFooterList);
        }

        private static Guid writeBDHtmlPage(Entities pContext, Guid pDisplayParentId, BDConstants.BDNodeType pDisplayParentType, string pBodyHTML, BDConstants.BDHtmlPageType pPageType, List<BDLinkedNote> pFooterList)
        {
            StringBuilder footerHTML = new StringBuilder();
            // insert footer text
            if (pFooterList.Count > 0)
            {
                footerHTML.Append(@"<h4>Footnotes</h4>");
                footerHTML.Append(@"<ol>");

                foreach (BDLinkedNote note in pFooterList)
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

            return newPage.Uuid;
        }
        #endregion
    }
}
