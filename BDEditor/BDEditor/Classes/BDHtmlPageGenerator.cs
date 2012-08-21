
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

            generatePages(pNode);

            List<BDHtmlPage> pages = BDHtmlPage.RetrieveAll(dataContext);
            List<Guid> htmlPageIds = BDHtmlPage.RetrieveAllIDs(dataContext);

            foreach (BDHtmlPage page in pages)
            {
                processTextForInternalLinks(dataContext, page, htmlPageIds);
                processTextForSubscriptAndSuperscriptMarkup(dataContext, page);
            }
        }

        private void generateOverviewAndChildrenForNode( Entities pContext, IBDNode pNode)
        {
            if (!beginDetailPage(pContext, pNode))
            {
                string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid);
                if(noteText.Length > 0)
                    generatePageForOverview(pContext, pNode.Uuid,pNode.NodeType,noteText);

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
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            isPageGenerated = true;
                            break;
                        default:
                           isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            generatePageForAntibioticsPharmacodynamics(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            generatePageForAntibioticDosingInHepaticImpairment(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                           isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Microbiology_EmpiricTherapy:
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
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                                // create page
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
                        case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                            isPageGenerated = true;
                            break;
                        default:
                            isPageGenerated = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogen:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
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
                            isPageGenerated = true;
                            generatePresentationPagesForEmpiricTherapy(pContext, pNode as BDNode);
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
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
                        case BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol:
                            // build page
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Microbiology_Antibiogram:
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
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                            // generate page
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
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            // generate page
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
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            // generate page
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
                        case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            // generate page
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
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                        case BDConstants.LayoutVariantType.Table_3_Column:
                            generatePageForAntibioticsNameListing(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_PreOp:
                            // generate a page
                           isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
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
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            generatePageForAntibioticsDosingAndMonitoring(pContext, pNode as BDNode);
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail:
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                            // create page
                            isPageGenerated = true;
                            break;
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
                if(pNode.NodeType == BDConstants.BDNodeType.BDChapter)
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
                if(node.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
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
                    Guid footnoteGuid = buildFootnotePageForParentAndProperty(pContext, BDNode.PROPERTYNAME_NAME ,node as BDNode);
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
                        Guid footnoteGuid = buildFootnotePageForParentAndProperty(pContext, BDNode.PROPERTYNAME_NAME, node);
                        if (footnoteGuid == Guid.Empty)
                            bodyHTML.AppendFormat(@"<tr><td>{0}</td><td /><td /></tr>", node.Name);
                        else
                            bodyHTML.AppendFormat(@"<tr><td><a href=""{0}""><b>{1}</b></a></td><td /><td /><tr>", footnoteGuid, node.Name);

                        List<IBDNode> dosages = BDFabrik.GetChildrenForParent(pContext, node);
                        foreach (IBDNode dosage in childNodes)
                            if (node.NodeType == BDConstants.BDNodeType.BDDosage)
                                bodyHTML.Append(buildDosageWithCostHTML(pContext, dosage));
                    }
                    else if(node.NodeType == BDConstants.BDNodeType.BDAntimicrobial)
                    {
                        Guid footnoteGuid = buildFootnotePageForParentAndProperty(pContext, BDDosage.PROPERTYNAME_DOSAGE, node);
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

            if (pNode.Name.Length > 0)
                bodyHTML.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

            // insert overview text
            string antimicrobialOverviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (antimicrobialOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(antimicrobialOverviewHtml);

            List<IBDNode> childNodes = BDFabrik.GetChildrenForParent(pContext, pNode);
            if (childNodes.Count > 0)
            {
                

                foreach (IBDNode node in childNodes)
                {
                    if (node.NodeType == BDConstants.BDNodeType.BDTable)
                    {
                        // insert node name (table name)
                        if(node.Name.Length > 0)
                            bodyHTML.AppendFormat(@"<h0>{0}</h0>",node.Name);
                       
                        List<IBDNode> rows = BDFabrik.GetChildrenForParent(pContext, node);
                        if (rows.Count > 0)
                        {
                            bodyHTML.Append(@"<table>");
                            foreach (IBDNode child in rows)
                            {
                                BDTableRow row = child as BDTableRow;
                                if (row != null)
                                    buildTableRowHtml(pContext, row);
                            }
                            bodyHTML.Append(@"</table>");
                        }
                    }
                    else if(node.NodeType == BDConstants.BDNodeType.BDSubtopic)
                    {
                        if(node.Name.Length > 0)
                            bodyHTML.AppendFormat(@"<h0>{0}</h0>", node.Name);
                        string noteText = retrieveNoteTextForOverview(pContext, pNode.Uuid);
                        if (noteText.Length > 0)
                            bodyHTML.Append(noteText);
                    }
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

private void generateShell(Entities pContext, BDNode pNode) {
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
                List<BDLinkedNote> d1Inline = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, firstNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> d1Marked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, firstNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> d1Unmarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, firstNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

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
                            bodyHTML.Append(buildTableRowHtml(pContext, row));
                    }
                    else if (node.NodeType == BDConstants.BDNodeType.BDTableSection)
                    {
                        if (node.Name.Length > 0)
                            bodyHTML.AppendFormat(@"<tr><td colspan = 3>{0}<td></tr>", node.Name);
                        List<IBDNode> sectionChildren = BDFabrik.GetChildrenForParent(pContext, node);
                        if (sectionChildren.Count > 0)
                        {
                            foreach (IBDNode sectionChild in sectionChildren)
                            {
                                if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                                {
                                    if (sectionChild.Name.Length > 0)
                                    {
                                        bodyHTML.AppendFormat(@"<tr><td colspan = 3>{0}<td></tr>", sectionChild.Name);
                                        List<BDTableRow> rows = BDTableRow.RetrieveTableRowsForParentId(pContext, sectionChild.Uuid);
                                        foreach (BDTableRow row in rows)
                                            bodyHTML.Append(buildTableRowHtml(pContext, row));
                                    }
                                }
                                else if (sectionChild.NodeType == BDConstants.BDNodeType.BDTableRow)
                                {
                                    BDTableRow row = sectionChild as BDTableRow;
                                    if (row != null)
                                        bodyHTML.Append(buildTableRowHtml(pContext, row));
                                }
                            }
                        }

                    }
                    else if (node.NodeType == BDConstants.BDNodeType.BDTableSubsection)
                    {
                        //TODO:  Make fontsize smaller than section name
                        if(node.Name.Length > 0)
                            bodyHTML.AppendFormat(@"<tr><td colspan = 3>{0}<td></tr>", node.Name);
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

        private void generatePresentationPagesForEmpiricTherapy(Entities pContext, BDNode pNode)
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

            // insert overview text from linkedNote
            string presentationOverviewHtml = retrieveNoteTextForOverview(pContext, pNode.Uuid);
            if (presentationOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(presentationOverviewHtml);

            // process pathogen groups (and pathogens)
            List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode pathogenGroup in pathogenGroups)
            {
                bodyHTML.Append(buildPathogenGroupHtml(pContext, pathogenGroup));

                // process therapy groups
                List<BDTherapyGroup> therapyGroups = BDTherapyGroup.RetrieveTherapyGroupsForParentId(pContext, pathogenGroup.Uuid);
                if (therapyGroups.Count > 0)
                    bodyHTML.Append(@"<h2>Recommended Empiric Therapy</h2>");

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
                            bodyHTML.Append(buildTherapyGroupHtml(pContext, group));
                            bodyHTML.Append(@"<table>");

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
                                    if(therapy.dosageSameAsPrevious == false)
                                        previousTherapyDosage = therapy.dosage;
                                    therapiesHaveDosage = true;
                                }
                                if (!string.IsNullOrEmpty(therapy.duration))
                                {
                                    if(therapy.dosageSameAsPrevious == false)
                                        previousTherapyDosage = therapy.dosage;
                                    therapiesHaveDuration = true;
                                }
                            }

                            bodyHTML.Append(@"<tr><th>Therapy</th>");
                            if(therapiesHaveDosage)
                                bodyHTML.Append(@"<th>Dosage</th>");
                            else
                                bodyHTML.Append(@"<th></th>");
                            if(therapiesHaveDuration)
                                bodyHTML.Append(@"<th>Duration</th>");
                            else
                                bodyHTML.Append(@"<th></th>");

                            bodyHTML.Append(@"</tr>");

                            bodyHTML.Append(therapyHTML);
                            bodyHTML.Append(@"</table>");
                        }
                    }
                }

                // inject Html into page html & save as a page to the database.
                string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
                BDHtmlPage newPage = BDHtmlPage.CreateBDHtmlPage(pContext);
                newPage.displayParentType = (int)BDConstants.BDNodeType.BDPresentation;
                newPage.displayParentId = pNode.Uuid;
                newPage.documentText = pageHtml;
                BDHtmlPage.Save(pContext, newPage);
            }
        }

        private Guid generatePageForOverview(Entities pContext, Guid pParentId, BDConstants.BDNodeType pParentType, string pOverviewText)
        {
            Guid returnGuid = Guid.Empty;
            if (pOverviewText.Length > EMPTY_PARAGRAPH)
            {
                BDHtmlPage notePage = BDHtmlPage.CreateBDHtmlPage(pContext);
                notePage.displayParentId = pParentId;
                notePage.displayParentType = (int)pParentType;
                notePage.documentText = topHtml + pOverviewText + bottomHtml;

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

        private Guid buildFootnotePageForParentAndProperty(Entities pContext, string pPropertyName, IBDNode pNode)
        {
            StringBuilder nodeHTML = new StringBuilder();
            List<BDLinkedNote> footnotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pNode.Uuid, pPropertyName, BDConstants.LinkedNoteType.Footnote);

            // create HTML page for footnote
            nodeHTML.Append(buildTextFromNotes(footnotes));

            // create HTML with link to footnote
           Guid footnoteId =  generatePageForLinkedNotes(pContext, pNode.Uuid, pNode.ParentType, nodeHTML.ToString());
            return footnoteId;
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

                List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

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
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

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
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

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

            List<BDLinkedNote> tNameInline = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tNameMarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tNameUnmarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.UnmarkedComment);

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

            List<BDLinkedNote> tDosageInline = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosageMarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosageUnmarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

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

            List<BDLinkedNote> tDurationInline = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDurationMarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDurationUnmarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.UnmarkedComment);

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
            List<BDLinkedNote> d1Inline = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d1Marked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d1Unmarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid d1NotePageGuid = generatePageForLinkedNotes(pContext, dosageNode.Uuid, BDConstants.BDNodeType.BDDosage, d1Inline, d1Marked, d1Unmarked);
            if (d1NotePageGuid != Guid.Empty)
            {
                if (dosageNode.dosage.Length > 0)
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", d1NotePageGuid, dosageNode.dosage);
                else
                    dosageHTML.AppendFormat(@"<td><a href=""{0}"">See Notes.</a></td>", d1NotePageGuid);
            }
            else
                dosageHTML.AppendFormat("<td>{0}</td>",dosageNode.dosage);

            dosageHTML.AppendFormat(@"<td>{0}</td>",dosageNode.cost);

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
            List<BDLinkedNote> d2Inline = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d2Marked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d2Unmarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE2, BDConstants.LinkedNoteType.UnmarkedComment);

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
            List<BDLinkedNote> d3Inline = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d3Marked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d3Unmarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE3, BDConstants.LinkedNoteType.UnmarkedComment);

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
            List<BDLinkedNote> d4Inline = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> d4Marked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> d4Unmarked = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, dosageNode.Uuid, BDDosage.PROPERTYNAME_DOSAGE4, BDConstants.LinkedNoteType.UnmarkedComment);

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

        private string buildTableRowHtml(Entities pContext, BDTableRow pRow)
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
                    if (tableCell != null)
                        tableRowHTML.AppendFormat(@"{0}{1}{2}", startCellTag, tableCell.value, endCellTag);
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

        private List<BDLinkedNote> retrieveNotesForParentAndPropertyForLinkedNoteType(Entities pContext, Guid pParentId, string pPropertyName, BDConstants.LinkedNoteType pNoteType)
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

        private void processTextForInternalLinks(Entities pContext, BDHtmlPage pPage, List<Guid>pPageGuids)
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
