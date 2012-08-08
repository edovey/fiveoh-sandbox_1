
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

        public void Generate()
        {
            Entities dataContext = new Entities();

            // Clear the data from the local store.
            BDHtmlPage.DeleteAll();

            generatePages();

            List<BDHtmlPage> pages = BDHtmlPage.RetrieveAll(dataContext);
            List<Guid> htmlPageIds = BDHtmlPage.RetrieveAllIDs(dataContext);

            foreach (BDHtmlPage page in pages)
                processTextForInternalLinks (dataContext, page, htmlPageIds);
        }

        private void generateOverviewAndChildrenForNode( Entities pContext, IBDNode pNode)
        {
            List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode child in children)
            {
                List<BDLinkedNote> chIldOverviewNotes = retrieveNotesForParentAndPropertyForLinkedNoteType(pContext, pNode.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW, BDConstants.LinkedNoteType.Inline);
                if (chIldOverviewNotes.Count > 0)
                    generatePageForOverview(pContext, pNode.Uuid, pNode.NodeType, chIldOverviewNotes[0]);

                if(!beginDetailPage(pContext, pNode))
                generateOverviewAndChildrenForNode(pContext, child);
               }
        }

        /// <summary>
        /// Check the node type and the layout variant to determine which page needs to be built.
        /// Closely resembles switch in EditView > showNavSelection.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        private bool beginDetailPage(Entities pContext, IBDNode pNode)
        {
            bool isPageGenerated = false;
            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDSection:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            // build page
                            isPageGenerated = true;
                            break;
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
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            // build html page
                            isPageGenerated = true;
                            break;
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                           // build html page
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
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            // check if we need a page here.
                            isPageGenerated = true;
                            break;
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
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
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
                        case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities:
                            // need to check the rest of the layout variants
//                            GeneratePresentationPagesForTreatmentRecommendation01(pContext, pNode as BDNode);
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
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                        case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                        case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
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
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                        case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                        case BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail:
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
                case BDConstants.BDNodeType.BDTherapyGroup:
               default:
                    isPageGenerated = false;
                            break;
            }
            return isPageGenerated;
        }
        
        
        private void generatePages()
        {
            Entities dataContext = new Entities();
            List<BDNode> chapters = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDChapter);

            foreach (BDNode chapter in chapters)
            {
                generateOverviewAndChildrenForNode(dataContext, chapter);
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

        public void GeneratePresentationPagesForTreatmentRecommendation01(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called directly from another class 
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

            // insert overview text from linked iNote
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
                    pathogenGroupHtml.AppendFormat(@"<h1>{0}</h1>", pNode.Name);

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

        private Guid generatePageForOverview(Entities pContext, Guid pParentId, BDConstants.BDNodeType pParentType, BDLinkedNote pOverviewNote)
        {
            Guid returnGuid = Guid.Empty;
           if (pOverviewNote.documentText.Length > EMPTY_PARAGRAPH)
            {
                BDHtmlPage notePage = BDHtmlPage.CreateBDHtmlPage(pContext);
                notePage.displayParentId = pParentId;
                notePage.displayParentType = (int)pParentType;
                notePage.documentText = topHtml + pOverviewNote.documentText + bottomHtml;

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
                                string newText = pPage.documentText.Replace(anchorGuid.ToString(), linkGuid.ToString());
                                pPage.documentText = newText;
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
    }
}
