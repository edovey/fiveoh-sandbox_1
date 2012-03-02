using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void Generate()
        {
            // Clear the data from the remote store.
            Entities dataContext = new Entities();
            RepositoryHandler.Aws.DeleteRemotePages(dataContext);

            // Clear the data from the local store.
            BDHtmlPage.DeleteAll();

            generatePages();
        }

        private void generatePages()
        {
            Entities dataContext = new Entities();
            List<BDNode> chapters = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDChapter);

            foreach (BDNode chapter in chapters)
            {
                switch (chapter.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        {
                            List<IBDNode> sections = BDFabrik.GetChildrenForParent(dataContext, chapter);
                            foreach (IBDNode section in sections)
                            {
                                switch (section.LayoutVariant)
                                {
                                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                                        {
                                            List<BDLinkedNote> sectionOverviewNotes = retrieveNotesForParentAndPropertyForType(dataContext, section.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW, BDConstants.LinkedNoteType.Inline);
                                            if(sectionOverviewNotes.Count > 0)
                                                generatePageForOverview(dataContext, section.Uuid, BDConstants.BDNodeType.BDSection, sectionOverviewNotes[0]);
                                            List<IBDNode> categories = BDFabrik.GetChildrenForParent(dataContext, section);
                                            foreach (IBDNode category in categories)
                                            {
                                                List<BDLinkedNote> categoryOverviewNotes = retrieveNotesForParentAndPropertyForType(dataContext, category.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW, BDConstants.LinkedNoteType.Inline);
                                                if(categoryOverviewNotes.Count > 0)
                                                    generatePageForOverview(dataContext, category.Uuid, BDConstants.BDNodeType.BDCategory, categoryOverviewNotes[0]);
                                                List<IBDNode> diseases = BDFabrik.GetChildrenForParent(dataContext, category);
                                                foreach (IBDNode disease in diseases)
                                                {
                                                    List<BDLinkedNote> diseaseOverviewNotes = retrieveNotesForParentAndPropertyForType(dataContext, disease.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW, BDConstants.LinkedNoteType.Inline);
                                                    if (diseaseOverviewNotes.Count > 0)
                                                        generatePageForOverview(dataContext, disease.Uuid, BDConstants.BDNodeType.BDDisease, diseaseOverviewNotes[0]);
                                                    List<IBDNode> presentations = BDFabrik.GetChildrenForParent(dataContext, disease);
                                                    foreach (IBDNode presentation in presentations)
                                                    {
                                                        BDNode node = presentation as BDNode;
                                                        if (null != node && node.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01 && node.NodeType == BDConstants.BDNodeType.BDPresentation)
                                                        {
                                                            GeneratePresentationPagesForTreatmentRecommendation01(dataContext, node);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
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
            string presentationOverviewHtml = retrieveNoteTextForParentAndProperty(pContext, pNode.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
            if (presentationOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(presentationOverviewHtml);

            // process pathogen groups (and pathogens)
            List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParent(pContext, pNode);
            foreach (IBDNode pathogenGroup in pathogenGroups)
            {
                bodyHTML.Append(buildPathogenGroupHtml(pContext, pathogenGroup));

                // process therapy groups
                List<BDTherapyGroup> therapyGroups = BDTherapyGroup.getTherapyGroupsForParentId(pContext, pathogenGroup.Uuid);
                if (therapyGroups.Count > 0)
                    bodyHTML.Append(@"<h2>Recommended Empiric Therapy</h2>");
                foreach (BDTherapyGroup tGroup in therapyGroups)
                {
                    BDTherapyGroup group = tGroup as BDTherapyGroup;
                    if (null != group)
                    {
                        // therapy group header
                        // therapy group linked notes:  inline, marked, unmarked
                        // add footnote and endnote to footer
                        List<BDTherapy> therapies = BDTherapy.GetTherapiesForTherapyParentId(pContext, group.Uuid);
                        if (therapies.Count > 0)
                        {
                            bodyHTML.Append(buildTherapyGroupHtml(pContext, group));
                            bodyHTML.Append(@"<table>");
                            bodyHTML.Append(@"<tr><th>Therapy</th><th>Dosage</th><th>Duration</th></tr>");

                            foreach (BDTherapy therapy in therapies)
                            {
                                bodyHTML.Append(buildTherapyHtml(pContext, therapy));

                                if (therapy.Name != null && therapy.Name.Length > 0 && therapy.nameSameAsPrevious == false)
                                    previousTherapyName = therapy.Name;
                                if (therapy.dosage != null && therapy.dosage.Length > 0 && therapy.dosageSameAsPrevious == false)
                                    previousTherapyDosage = therapy.dosage;
                                if (therapy.duration != null && therapy.duration.Length > 0 && therapy.dosageSameAsPrevious == false)
                                    previousTherapyDosage = therapy.dosage;

                            }
                            bodyHTML.Append(@"</table>");
                        }
                    }
                }

                // inject Html into page html & save as a page to the database.
                string pageHtml = topHtml + bodyHTML.ToString() + bottomHtml;
                BDHtmlPage newPage = BDHtmlPage.CreateHtmlPage(pContext);
                newPage.displayParentType = (int)BDConstants.BDNodeType.BDPresentation;
                newPage.displayParentId = pNode.Uuid;
                newPage.documentText = pageHtml;
                BDHtmlPage.Save(pContext, newPage);
            }
        }

        private string retrieveNoteTextForParentAndProperty(Entities pContext, Guid pParentId, string pPropertyName)
        {
            StringBuilder linkedNoteHtml = new StringBuilder();
            if (null != pPropertyName && pPropertyName.Length > 0)
            {
                List<BDLinkedNoteAssociation> list = BDLinkedNoteAssociation.GetLinkedNoteAssociationsFromParentIdAndProperty(pContext, pParentId, pPropertyName);
                foreach (BDLinkedNoteAssociation assn in list)
                {
                    BDLinkedNote linkedNote = BDLinkedNote.GetLinkedNoteWithId(pContext, assn.linkedNoteId);
                    if (null != linkedNote)
                        linkedNoteHtml.Append(linkedNote.documentText);
                }
            }
            return linkedNoteHtml.ToString();
        }

        private List<BDLinkedNote> retrieveNotesForParentAndPropertyForType(Entities pContext, Guid pParentId, string pPropertyName, BDConstants.LinkedNoteType pNoteType)
        {
            List<BDLinkedNote> noteList = new List<BDLinkedNote>();
            if (null != pPropertyName && pPropertyName.Length > 0)
            {
                List<BDLinkedNoteAssociation> list = BDLinkedNoteAssociation.GetLinkedNoteAssociationsFromParentIdAndProperty(pContext, pParentId, pPropertyName);
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
                pathogenGroupHtml.Append(retrieveNoteTextForParentAndProperty(pContext, pathogenGroup.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW));

                List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyForType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
                List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyForType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
                List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyForType(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

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
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyForType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyForType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyForType(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

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
            List<BDLinkedNote> inlineNotes = retrieveNotesForParentAndPropertyForType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> markedNotes = retrieveNotesForParentAndPropertyForType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> unmarkedNotes = retrieveNotesForParentAndPropertyForType(pContext, pTherapyGroup.Uuid, BDNode.PROPERTYNAME_NAME, BDConstants.LinkedNoteType.UnmarkedComment);

            // TODO:  DEAL WITH FOOTNOTES / ENDNOTES

            // MAY NEED THIS if we decide to generate a page for the notes rather than putting them in-line.
            //if (notesListHasContent(pContext, pNotes))
            //{
            //    Guid noteGuid = generatePageForLinkedNotes(pContext, pTherapyGroup.Uuid, pTherapyGroup.NodeType, pNotes, new List<BDLinkedNote>(), new List<BDLinkedNote>());
            //    therapyGroupHtml.AppendFormat(@"<h4><a href""{0}"">{1}</a></h4>", noteGuid, tgName);
            //}
            // else if (pTherapyGroup.Name.Length > 0)

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

            List<BDLinkedNote> tNameInline = retrieveNotesForParentAndPropertyForType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tNameMarked = retrieveNotesForParentAndPropertyForType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tNameUnmarked = retrieveNotesForParentAndPropertyForType(pContext, nameNoteParentId, BDTherapy.PROPERTYNAME_THERAPY, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tNameNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tNameInline, tNameMarked, tNameUnmarked);
            if (tNameNotePageGuid != Guid.Empty)
            {
                if (tName.Length > 0)
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>{1}</b></a>", tNameNotePageGuid, tName);
                else
                    therapyHtml.AppendFormat(@"<a href=""{0}""><b>See notes.</b></a>", tNameNotePageGuid);
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

            List<BDLinkedNote> tDosageInline = retrieveNotesForParentAndPropertyForType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDosageMarked = retrieveNotesForParentAndPropertyForType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDosageUnmarked = retrieveNotesForParentAndPropertyForType(pContext, dosageNoteParentId, BDTherapy.PROPERTYNAME_DOSAGE, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDosageNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDosageInline, tDosageMarked, tDosageUnmarked);

            if (tDosageNotePageGuid != Guid.Empty)
            {
                if (pTherapy.dosage.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDosageNotePageGuid, tDosage.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See notes.</a></td>", tDosageNotePageGuid);
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

            List<BDLinkedNote> tDurationInline = retrieveNotesForParentAndPropertyForType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.Inline);
            List<BDLinkedNote> tDurationMarked = retrieveNotesForParentAndPropertyForType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.MarkedComment);
            List<BDLinkedNote> tDurationUnmarked = retrieveNotesForParentAndPropertyForType(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION, BDConstants.LinkedNoteType.UnmarkedComment);

            Guid tDurationNotePageGuid = generatePageForLinkedNotes(pContext, pTherapy.Uuid, BDConstants.BDNodeType.BDTherapy, tDurationInline, tDurationMarked, tDurationUnmarked);

            if (tDurationNotePageGuid != Guid.Empty)
            {
                if (pTherapy.duration.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", tDurationNotePageGuid, tDuration.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See notes.</a></td>", tDurationNotePageGuid);
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
                BDHtmlPage notePage = BDHtmlPage.CreateHtmlPage(pContext);
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

            Guid returnGuid = Guid.Empty;
            if (noteHtml.Length > EMPTY_PARAGRAPH)
            {
                BDHtmlPage notePage = BDHtmlPage.CreateHtmlPage(pContext);
                notePage.displayParentId = pParentId;
                notePage.displayParentType = (int)pParentType;
                notePage.documentText = topHtml + noteHtml.ToString() + bottomHtml;

                BDHtmlPage.Save(pContext, notePage);

                returnGuid = notePage.Uuid;
            }
            return returnGuid;
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
    }
}
