
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
        string previousTherapyDuration = string.Empty;
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
            generatePageForOverview(pContext, pNode);

            if (!beginDetailPage(pContext, pNode))
            {
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
                foreach (IBDNode child in children)
                {
                    generateOverviewAndChildrenForNode(pContext, child);
                }
            }
        }

        /// <summary>
        /// Check the node type and the layout variant to determine which page needs to be built.
        /// Closely resembles switch in EditView > showNavSelection.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <returns>Boolean to indicate that page is generated and thus to stop recursing through the node tree</returns>
        private bool beginDetailPage(Entities pContext, IBDNode pNode)
        {
            // create a navigationNode for each node that executes this method when it is a BDNode class
            // once the recursive calls have stopped, we are into an HTML page so we dont need the nav node
            // Non-BDNode classes are not navigation nodes (and should not execute this in any case)
            if (pNode.GetType() == typeof(BDNode))
            {
                BDNavigationNode.CreateNavigationNodeFromBDNode(pContext, pNode as BDNode);

                // create HTML page for References attached to this node
                generatePageForParentAndPropertyReferences(pContext, BDNode.PROPERTYNAME_NAME, pNode);
            }

            bool isPageGenerated = false;
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
                            generatePageForAntibioticsGuidelines(pContext, pNode as BDNode);
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
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            generatePageForEmpiricTherapyOfParasitic(pContext, pNode as BDNode);
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
                            case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                                generatepageForEmpiricTherapyOfGenitalUlcers(pContext, pNode as BDNode);
                                isPageGenerated = true;
                                break;
                            case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                                generatePageForEmpiricTherapyOfEndocarditis(pContext, pNode as BDNode);
                                isPageGenerated = true;
                                break;
                            case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                                generatePageForEmpiricTherapyOfVesicularLesions(pContext, pNode as BDNode);
                                isPageGenerated = true;
                                break;
                            case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                                generatePageForEmpiricTherapyOfCellulitisInExtremities(pContext, pNode as BDNode);
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
                            // if the processing comes through here, then the Disease only has one Presentation > each Presentation has a page
                            isPageGenerated = true;
                            generatePageForEmpiricTherapyPresentation(pContext, pNode as BDNode);
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
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                        case BDConstants.LayoutVariantType.Table_5_Column:
                            generatePageForAntibioticsStepdown(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                            generatePageForEmpiricTherapyOfPneumonia(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_Peritonitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_Meningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_Endocarditis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation15_Pneumonia:
                            generatePageForEmpiricTherapyOfCultureDirected(pContext, pNode as BDNode);
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
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;

                case BDConstants.BDNodeType.BDChapter:
                case BDConstants.BDNodeType.BDMicroorganism:
                case BDConstants.BDNodeType.BDPathogenGroup:
                case BDConstants.BDNodeType.BDResponse:
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
                        Guid pageGuid = generatePageForLinkedNotes(dataContext, a.linkedNoteId.Value, BDConstants.BDNodeType.BDLinkedNote, n.documentText);
                        break;
                    }
                }
            }
        }

        private void generatePageForAntibioticsGuidelines(Entities pContext, BDNode pNode)
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string antimicrobialOverviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (antimicrobialOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(antimicrobialOverviewHtml);

            // child nodes can either be pathogen groups or topics (node with overview)
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode node in childNodes)
            {
                if (node.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
                    bodyHTML.Append(buildPathogenGroupHtml(pContext, node));
                else if (node.NodeType == BDConstants.BDNodeType.BDTopic)
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

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)BDConstants.BDNodeType.BDAntimicrobial;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string antimicrobialOverviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (antimicrobialOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(antimicrobialOverviewHtml);

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

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string antimicrobialOverviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (antimicrobialOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(antimicrobialOverviewHtml);

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

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
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

            // begins at antimicrobialSection OR category
            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHTML.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHTML);

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
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
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
                                                        bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
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
                                                            bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
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

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        private void generateShell(Entities pContext, BDNode pNode)
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h2>{0}</h2>", pNode.Name);

            // insert overview text
            string antimicrobialOverviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (antimicrobialOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(antimicrobialOverviewHtml);

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

                // inject Html into page html & save as a page to the database.
                string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
                BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
                newPage.displayParentType = (int)BDConstants.BDNodeType.BDAntimicrobial;
                newPage.displayParentId = pNode.Uuid;
                newPage.documentText = pageHtml;
                BDHtmlPage.Save(pContext, newPage);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string nodeOverviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (nodeOverviewHTML.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(nodeOverviewHTML);

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

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string nodeOverviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (nodeOverviewHTML.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(nodeOverviewHTML);

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
                            bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
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
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
                                    }
                                }
                                else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                {
                                    BDTableRow row = sectionChild as BDTableRow;
                                    if (row != null)
                                        bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
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

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string nodeOverviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (nodeOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(nodeOverviewHtml);

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
                                bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
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
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
                                        }
                                    }
                                    else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = sectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, false));
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
                                                bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, true));
                                        }
                                    }
                                    else if (subsectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                    {
                                        BDTableRow row = subsectionChild as BDTableRow;
                                        if (row != null)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row, footerList, false));
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

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

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


            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> subsections = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode subsection in subsections)
            {
                if (subsection.Name.Length > 0)
                    bodyHTML.AppendFormat(@"<h3>{0}</h3>", subsection.Name);
                List<IBDNode> topics = BDFabrik.GetChildrenForParent(pContext, subsection);
                foreach (IBDNode topic in topics)
                {
                    if (topic.Name.Length > 0)
                        bodyHTML.AppendFormat(@"<h4>{0}</h4>", topic.Name);
                    string topicOverview = retrieveNoteTextForOverview(pContext, topic.Uuid);
                    if (topicOverview.Length > EMPTY_PARAGRAPH)
                        bodyHTML.Append(topicOverview);
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
                                    bodyHTML.Append(buildTableRowHtml(pContext, tableRow, footerList, true));
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

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        /// <summary>
        /// Build HTML page at Disease level when only one Presentation is defined
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
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

            StringBuilder bodyHtml = new StringBuilder();
            List<BDLinkedNote> footerList = new List<BDLinkedNote>();

            if (pNode.Name.Length > 0)
                bodyHtml.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            //List<BDHtmlPage> referencePages = BDHtmlPage.RetrieveHtmlPageForDisplayParentId(pContext, pNode.Uuid);
            //foreach(BDHtmlPage refPage in referencePages)
            //    if(refPage.

            // insert overview text from linkedNote
            string overviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHTML.Length > EMPTY_PARAGRAPH)
                bodyHtml.Append(overviewHTML);

            // process pathogen groups (and pathogens)
            List<IBDNode> presentations = BDFabrik.GetChildrenForParent(pContext, pNode);

#if DEBUG
            if (presentations.Count > 1)
                throw new InvalidOperationException();
#endif
            foreach (IBDNode presentation in presentations)
            {
                bodyHtml.Append(buildEmpiricTherapyHTML(pContext, presentation as BDNode, footerList));

                // inject Html into page html & save as a page to the database.
                string pageHtml = topHtml + bodyHtml.ToString() + bottomHtml;
                BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
                newPage.displayParentType = (int)BDConstants.BDNodeType.BDDisease;
                newPage.displayParentId = pNode.Uuid;
                newPage.documentText = pageHtml;
                BDHtmlPage.Save(pContext, newPage);
            }
        }

        /// <summary>
        /// Build HTML page to show Presentation and all it's children on one page
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
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

            bodyHTML.Append(buildEmpiricTherapyHTML(pContext, pNode, footerList));

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)BDConstants.BDNodeType.BDPresentation;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        private void generatePageForEmpiricTherapyOfCellulitisInExtremities(Entities pContext, BDNode pNode)
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        private void generatePageForEmpiricTherapyOfVesicularLesions(Entities pContext, BDNode pNode)
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        private void generatePageForEmpiricTherapyOfEndocarditis(Entities pContext, BDNode pNode)
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        private void generatepageForEmpiricTherapyOfGenitalUlcers(Entities pContext, BDNode pNode)
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        private void generatePageForEmpiricTherapyOfParasitic(Entities pContext, BDNode pNode)
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        private void generatePageForEmpiricTherapyOfCultureDirected(Entities pContext, BDNode pNode)
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string overviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(overviewHtml);

            // show child nodes in a table
            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                //Append HTML for child layout
            }

            // inject Html into page html & save as a page to the database.
            string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
            BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
            newPage.displayParentType = (int)pNode.NodeType;
            newPage.displayParentId = pNode.Uuid;
            newPage.documentText = pageHtml;
            BDHtmlPage.Save(pContext, newPage);
        }

        private Guid generatePageForOverview(Entities pContext, IBDNode pNode)
        {
            Guid returnGuid = Guid.Empty;
            string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (noteText.Length > EMPTY_PARAGRAPH)
            {
                BDHtmlPage notePage = BDHtmlPage.CreateBDHtmlPage(pContext);
                notePage.displayParentId = pNode.ParentId;
                notePage.displayParentType = (int)pNode.ParentType;
                notePage.documentText = topHtml + noteText + bottomHtml;

                BDHtmlPage.Save(pContext, notePage);

                returnGuid = notePage.Uuid;
            }
            return returnGuid;
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

            Guid returnGuid = generatePageForLinkedNotes(pContext, pParentId, pParentType, noteHtml.ToString());
            return returnGuid;
        }

        private Guid generatePageForLinkedNotes(Entities pContext, Guid pParentId, BDConstants.BDNodeType pParentType, string pPageHtml)
        {
            if (pPageHtml.Length > EMPTY_PARAGRAPH)
            {
                BDHtmlPage notePage = BDHtmlPage.CreateBDHtmlPage(pContext);
                notePage.displayParentId = pParentId;
                notePage.displayParentType = (int)pParentType;
                notePage.documentText = topHtml + pPageHtml + bottomHtml;

                BDHtmlPage.Save(pContext, notePage);

                return notePage.Uuid;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Create an HTML page for the footnote attached to a node & property
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="pNode"></param>
        /// <returns>Buid of HTML page.</returns>
        private Guid generatePageForParentAndPropertyFootnote(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            string footnoteText = buildTextForParentAndPropertyFromLinkedNotes(pContext, pPropertyName, pNode, BDConstants.LinkedNoteType.Footnote);
            Guid footnoteId = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, footnoteText);
            return footnoteId;
        }

        /// <summary>
        /// Create an HTML page for the references attached to a node & property
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="pNode"></param>
        /// <returns>Guid of HTML page.</returns>
        /// 
        private Guid generatePageForParentAndPropertyReferences(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            string referenceText = buildTextForParentAndPropertyFromLinkedNotes(pContext, pPropertyName, pNode, BDConstants.LinkedNoteType.Reference);
            Guid footnoteId = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, referenceText);
            return footnoteId;
        }

        /// <summary>
        /// Build HTML for linkedNotes attached to property in node, to inject into HTML page.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pPropertyName"></param>
        /// <param name="pNode"></param>
        /// <param name="pNoteType"></param>
        /// <returns>Text of linked note as HTML</returns>
        private string buildTextForParentAndPropertyFromLinkedNotes(Entities pContext, string pPropertyName, IBDNode pNode, BDConstants.LinkedNoteType pNoteType)
        {
            StringBuilder notesHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pNode.Uuid, pPropertyName, pNoteType);
            notesHTML.Append(buildTextFromNotes(footnotes));
            return notesHTML.ToString();
        }

        private StringBuilder buildEmpiricTherapyHTML(Entities pContext, BDNode pNode, List<BDLinkedNote> pFooterList)
        {
             StringBuilder bodyHtml = new StringBuilder();
             if (pNode.Name.Length > 0 && pNode.Name != @"SINGLE PRESENTATION")
                 bodyHtml.AppendFormat(@"<h2>{0}</h2>", pNode.Name);

            // insert overview text from linkedNote
            string overviewHTML = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (overviewHTML.Length > EMPTY_PARAGRAPH)
                bodyHtml.Append(overviewHTML);

            // process pathogen groups (and pathogens)
            List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode pathogenGroup in pathogenGroups)
            {
                bodyHtml.Append(buildPathogenGroupHtml(pContext, pathogenGroup));

                // process therapy groups
                List<BDTherapyGroup> therapyGroups = BDTherapyGroup.RetrieveTherapyGroupsForParentId(pContext, pathogenGroup.Uuid);
                //if (therapyGroups.Count > 0)
                //    bodyHtml.Append(@"<h3>Recommended Empiric Therapy</h3>");

                foreach (BDTherapyGroup tGroup in therapyGroups)
                {
                    BDTherapyGroup group = tGroup as BDTherapyGroup;
                    if (null != group)
                    {
                        // therapy group header
                        // therapy group linked allNotes:  inline, marked, unmarked
                        // add footnote and endnote to footer
                        List<BDTherapy> therapies = BDTherapy.RetrieveTherapiesForParentId(pContext, group.Uuid);
                        if (therapies.Count > 0)
                        {
                            bodyHtml.Append(buildTherapyGroupHtml(pContext, group));
                            bodyHtml.Append(@"<table>");

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

                            bodyHtml.Append(@"<tr><th>Recommended<br>Empiric<br>Therapy</th>");
                            if (therapiesHaveDosage)
                                bodyHtml.Append(@"<th>Recommended<br>Dose</th>");
                            else
                                bodyHtml.Append(@"<th></th>");
                            if (therapiesHaveDuration)
                                bodyHtml.Append(@"<th>Recommended<br>Duration</th>");
                            else
                                bodyHtml.Append(@"<th></th>");

                            bodyHtml.Append(@"</tr>");

                            bodyHtml.Append(therapyHTML);
                            bodyHtml.Append(@"</table>");
                        }
                    }
                }
            }
            return bodyHtml;
        }

        /// <summary>
        /// Generate HTML for Pathogen Group and Pathogen
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
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

                // TODO:  DEAL WITH FOOTNOTES & ENDNOTES - will need to tag the pathogen group name, manage symbols?

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

            if (pPathogen.Name.Length > 0)
                pathogenHtml.AppendFormat(@"{0}<br>", pPathogen.name);

            pathogenHtml.Append(buildTextFromNotes(inlineNotes));
            pathogenHtml.Append(buildTextFromNotes(markedNotes));
            pathogenHtml.Append(buildTextFromNotes(unmarkedNotes));

            // TODO:  DEAL WITH FOOTNOTES AND ENDNOTES

            return pathogenHtml.ToString();
        }

        private string buildTherapyGroupHtml(Entities pContext, BDTherapyGroup pTherapyGroup)
        {
            StringBuilder therapyGroupHtml = new StringBuilder();
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

            // TODO:  DEAL WITH FOOTNOTES / ENDNOTES

            // MAY NEED THIS if we decide to generate a page for the allNotes rather than putting them in-line.
            //if (notesListHasContent(pContext, pNotes))
            //{
            //    Guid noteGuid = generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.NodeType, pNotes, new List<BDLinkedNote>(), new List<BDLinkedNote>());
            //    therapyGroupHtml.AppendFormat(@"<h4><a href""{0}"">{1}</a></h4>", noteGuid, tgName);
            //}
            // else if (pNode.Name.Length > 0)

            if (pTherapyGroup.Name.Length > 0)
                therapyGroupHtml.AppendFormat(@"<h4>{0}</h4>", pTherapyGroup.Name);

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
                    therapyHtml.Append(@"<tr><td> + </td><td/><td></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.OrWithNext:
                    therapyHtml.Append(@"<tr><td> or</td><td/><td></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.ThenWithNext:
                    therapyHtml.Append(@"<tr><td> then</td><td/><td></tr>");
                    break;
                case (int)BDTherapy.TherapyJoinType.WithOrWithoutWithNext:
                    therapyHtml.Append(@"<tr><td> +/-</td><td/><td></tr>");
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

        private string buildTableRowHtml(Entities pContext, BDTableRow pRow, List<BDLinkedNote> pFooterList, bool markFooterAtEnd)
        {
            StringBuilder tableRowHTML = new StringBuilder();
            string startCellTag = @"<td>";
            string endCellTag = @"</td>";

            if (pRow != null)
            {
                if (rowIsHeaderRow(pRow))
                {
                    startCellTag = @"<th>";
                    endCellTag = @"</th>";
                }
                tableRowHTML.Append(@"<tr>");
                List<BDTableCell> cells = BDTableCell.RetrieveTableCellsForParentId(pContext, pRow.Uuid);
                foreach (BDTableCell tableCell in cells)
                {
                    List<BDLinkedNote> itemFooter = retrieveNotesForParentAndPropertyOfLinkedNoteType(pContext, tableCell.Uuid, BDTableCell.PROPERTYNAME_CONTENTS, BDConstants.LinkedNoteType.Footnote);
                    if (itemFooter.Count == 0)
                        tableRowHTML.AppendFormat(@"{0}{1}{2}", startCellTag, tableCell.value, endCellTag);
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
                            tableRowHTML.AppendFormat(@"{0}{1}{2}{3}", startCellTag, tableCell.value, footerMarker, endCellTag);
                        else
                        {
                            int lineBreakIndex = tableCell.value.Length;
                            if (tableCell.value.Contains("\n"))
                                lineBreakIndex = tableCell.value.IndexOf("\n");
                            string cellTextWithFooterTag = tableCell.value.Insert(lineBreakIndex, footerMarker.ToString());
                            tableRowHTML.AppendFormat(@"{0}{1}{2}", startCellTag, cellTextWithFooterTag, endCellTag);
                        }
                    }
                }
                tableRowHTML.Append(@"</tr>");
            }

            return tableRowHTML.ToString();
        }

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
            return linkedNoteHtml.ToString();
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
                    case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity_HeaderRow:
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
    }
}
