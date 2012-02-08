using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public static class BDHtmlPageGenerator
    {
        private const string headerHtml = @"<html><head><meta http-equiv=""Content-type"" content=""text/html;charset=UTF-8\""><link rel=""stylesheet"" type=""text/css"" href=""bdviewer.css"" /> </head><body>";
        private const string footerHtml = @"</body></html>";
        private const int EMPTY_PARAGRAPH = 8;  // <p> </p>
        public static void Generate()
        {
            BDHtmlPage.DeleteAll();

            generatePages();
        }

        private static void generatePages()
        {
            Entities dataContext = new Entities();
            List<BDNode> chapters = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDChapter);

            foreach (BDNode chapter in chapters)
            {
                switch (chapter.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        {
                            List<IBDNode> sections = BDFabrik.GetChildrenForParentId(dataContext, chapter.Uuid);
                            foreach (IBDNode section in sections)
                            {
                                switch (section.LayoutVariant)
                                {
                                    case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                                        {
                                            List<IBDNode> categories = BDFabrik.GetChildrenForParentId(dataContext, section.Uuid);
                                            foreach (IBDNode category in categories)
                                            {
                                                List<IBDNode> diseases = BDFabrik.GetChildrenForParentId(dataContext, category.Uuid);
                                                foreach (IBDNode disease in diseases)
                                                {
                                                    List<IBDNode> presentations = BDFabrik.GetChildrenForParentId(dataContext, disease.Uuid);
                                                    foreach (IBDNode presentation in presentations)
                                                    {
                                                        BDNode node = presentation as BDNode;
                                                        if (null != node && node.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01 && node.NodeType == BDConstants.BDNodeType.BDPresentation)
                                                        {
                                                            // in the case of a disease having a single presentation, the construction of the web page with the
                                                            // disease overview on top and the presentation page below will be handled by the viewer.  
                                                            // The presentation pages are built and saved here without consideration for that variant.
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

        public static void GeneratePresentationPagesForTreatmentRecommendation01(Entities pContext, BDNode pNode)
        {
            // in the case where this method is called without the check for node type 
            if (pNode.NodeType != BDConstants.BDNodeType.BDPresentation)
            {
#if DEBUG
                throw new InvalidOperationException();
#else
                return;
#endif
            }
            
            StringBuilder bodyHTML = new StringBuilder();

            // insert overview text from linked note
            string presentationOverviewHtml = retrieveNoteTextForParent(pContext, pNode.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
            if (presentationOverviewHtml.Length > EMPTY_PARAGRAPH)
                bodyHTML.Append(presentationOverviewHtml);
                
            // process pathogen groups (and pathogens)
            List<IBDNode> pathogenGroups = BDFabrik.GetChildrenForParentId(pContext, pNode.Uuid);
            foreach (IBDNode pathogenGroup in pathogenGroups)
            {
                bodyHTML.Append(buildPathogenGroupHtml(pContext, pathogenGroup));

                // process therapy groups
                List<BDTherapyGroup> therapyGroups = BDTherapyGroup.getTherapyGroupsForParentId(pContext, pathogenGroup.Uuid);
                if(therapyGroups.Count > 0)
                    bodyHTML.Append(@"<h2>Recommended Empiric Therapy</h2>");

                // process therapies
                foreach (BDTherapyGroup tGroup in therapyGroups)
                {
                    BDTherapyGroup group = tGroup as BDTherapyGroup;
                    if (null != group)
                    {
                        List<BDTherapy> therapies = BDTherapy.GetTherapiesForTherapyParentId(pContext, group.Uuid);
                        if (therapies.Count > 0)
                        {
                            bodyHTML.Append(buildTherapyGroupHtml(pContext, group));
                            bodyHTML.Append(@"<table>");
                            bodyHTML.Append(@"<tr><th>Therapy</th><th>Dosage</th><th>Duration</th></tr>");

                            foreach (BDTherapy therapy in therapies)
                            {
                                bodyHTML.Append(buildTherapyHtml(pContext, therapy));
                            }
                            bodyHTML.Append(@"</table>");
                        }
                    }
                }

                // inject Html into page html & save as a page to the database.
                string pageHtml = headerHtml + bodyHTML.ToString() + footerHtml;
                BDHtmlPage newPage = BDHtmlPage.CreateHtmlPage(pContext);
                newPage.displayParentType =(int) BDConstants.BDNodeType.BDPresentation;
                newPage.displayParentId = pNode.Uuid;
                newPage.documentText = pageHtml;
                BDHtmlPage.Save(pContext, newPage);
            }
        }

        private static string retrieveNoteTextForParent(Entities pContext, Guid pParentId, string pPropertyName)
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

        private static List<BDLinkedNote> retrieveNotesForParent(Entities pContext, Guid pParentId, string pPropertyName)
        {
            List<BDLinkedNote> noteList = new List<BDLinkedNote>();
            if (null != pPropertyName && pPropertyName.Length > 0)
            {
                List<BDLinkedNoteAssociation> list = BDLinkedNoteAssociation.GetLinkedNoteAssociationsFromParentIdAndProperty(pContext, pParentId, pPropertyName);
                foreach (BDLinkedNoteAssociation assn in list)
                {
                    BDLinkedNote linkedNote = BDLinkedNote.GetLinkedNoteWithId(pContext, assn.linkedNoteId);
                    if (null != linkedNote)
                        noteList.Add(linkedNote);
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
        private static string buildPathogenGroupHtml(Entities pContext, IBDNode pNode)
        {
            StringBuilder pathogenGroupHtml = new StringBuilder();

            BDNode pathogenGroup = pNode as BDNode;
            if(null != pNode && pNode.NodeType == BDConstants.BDNodeType.BDPathogenGroup)
            {
                List<BDLinkedNote> pgNotes = retrieveNotesForParent(pContext, pathogenGroup.Uuid, BDNode.PROPERTYNAME_NAME);

                string pName = string.Empty;

                if (null == pNode.Name || pNode.Name.Length == 0)
                    pName = @"See note.";
                else if(pNode.Name.Length > 0)
                    pName = pNode.Name;

                if (notesListHasContent(pgNotes))
                {
                    Guid notePageGuid = generatePageForLinkedNotes(pContext, pgNotes, pathogenGroup.Uuid, pathogenGroup.NodeType);
                    pathogenGroupHtml.AppendFormat(@"<h1><a href""{0}"">{1}</a></h1>", notePageGuid, pName);
                }
                else
                    pathogenGroupHtml.AppendFormat(@"<h1>{0}</h1>", pName);

                List<IBDNode> pathogens = BDFabrik.GetChildrenForParentId(pContext, pathogenGroup.Uuid);

                if (pathogens.Count > 0)
                    pathogenGroupHtml.Append(@"<h3>Usual Pathogens</h3>");

                foreach(IBDNode item in pathogens)
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

        private static string buildPathogenHtml(Entities pContext, BDNode pPathogen)
        {
            StringBuilder pathogenHtml = new StringBuilder();

            List<BDLinkedNote> pNotes = retrieveNotesForParent(pContext, pPathogen.Uuid, BDNode.PROPERTYNAME_NAME);

            if (notesListHasContent(pNotes))
            {
                Guid notePageGuid = generatePageForLinkedNotes(pContext, pNotes, pPathogen.Uuid, pPathogen.NodeType);
                pathogenHtml.AppendFormat(@"<a href""{0}"">{1}</a>", notePageGuid, pPathogen.Name);
            }
            else
                pathogenHtml.AppendFormat(@"{0}<br>", pPathogen.name);
            return pathogenHtml.ToString();
        }

        private static string buildTherapyGroupHtml(Entities pContext, BDTherapyGroup pTherapyGroup)
        {
            StringBuilder therapyGroupHtml = new StringBuilder();

            string tgName = string.Empty;
            if (null == pTherapyGroup.Name || pTherapyGroup.Name.Length == 0)
                tgName = @"See note.";
            else if (pTherapyGroup.Name.Length > 0)
                tgName = pTherapyGroup.Name;

            List<BDLinkedNote> pNotes = retrieveNotesForParent(pContext, pTherapyGroup.Uuid, BDTherapyGroup.PROPERTYNAME_NAME);

            if (notesListHasContent(pNotes))
            {
                Guid noteGuid = generatePageForLinkedNotes(pContext, pNotes, pTherapyGroup.Uuid, pTherapyGroup.NodeType);
                therapyGroupHtml.AppendFormat(@"<h4><a href""{0}"">{1}</a></h4>", noteGuid, tgName);
            }
            else if (pTherapyGroup.Name.Length > 0)
                therapyGroupHtml.AppendFormat(@"<h4>{0}</h4>", pTherapyGroup.Name);

            return therapyGroupHtml.ToString();
        }

        private static string buildTherapyHtml(Entities pContext, BDTherapy pTherapy)
        {
            StringBuilder therapyHtml = new StringBuilder();
            therapyHtml.Append(@"<tr><td>");

            if (pTherapy.leftBracket.Value == true)
                therapyHtml.Append(@"&#91");

            if (pTherapy.Name.Length > 0)
            {
                List<BDLinkedNote> tNameNotes = retrieveNotesForParent(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_THERAPY);

                if (notesListHasContent(tNameNotes))
                {
                   Guid noteGuid =  generatePageForLinkedNotes(pContext, tNameNotes, pTherapy.Uuid, pTherapy.NodeType);
                   therapyHtml.AppendFormat(@"<a href=""{0}""><b>{1}</b></a>", noteGuid, pTherapy.Name);
                }
                else
                    therapyHtml.AppendFormat(@"<b>{0}</b>", pTherapy.Name);
            }

            if (pTherapy.rightBracket.Value == true)
                therapyHtml.Append(@"&#93");

            // check for conjunctions and add any that are found
            switch (pTherapy.therapyJoinType)
            {
                case (int)BDTherapy.TherapyJoinType.AndWithNext:
                    therapyHtml.Append(@" +");
                    break;
                case (int)BDTherapy.TherapyJoinType.OrWithNext:
                    therapyHtml.Append(@" or");
                    break;
                case (int)BDTherapy.TherapyJoinType.ThenWithNext:
                    therapyHtml.Append(@" then");
                    break;
                case (int)BDTherapy.TherapyJoinType.WithOrWithoutWithNext:
                    therapyHtml.Append(@" +/-");
                    break;
                default:
                    break;
            }
            therapyHtml.Append(@"</td>");

            // Dosage
            List<BDLinkedNote> dosageNotes = retrieveNotesForParent(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DOSAGE);

            if (notesListHasContent(dosageNotes))
            {
                Guid dosageNoteGuid = generatePageForLinkedNotes(pContext, dosageNotes, pTherapy.Uuid, pTherapy.NodeType);
                if (pTherapy.dosage.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", dosageNoteGuid, pTherapy.dosage.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See note.</a></td>", dosageNoteGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", pTherapy.dosage);

            // Duration
            List<BDLinkedNote> durationNotes = retrieveNotesForParent(pContext, pTherapy.Uuid, BDTherapy.PROPERTYNAME_DURATION);

            if (notesListHasContent(durationNotes))
            {
                Guid durationNoteGuid = generatePageForLinkedNotes(pContext, durationNotes, pTherapy.Uuid, pTherapy.NodeType);
                if (pTherapy.duration.Length > 0)
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">{1}</a></td>", durationNoteGuid, pTherapy.duration.Trim());
                else
                    therapyHtml.AppendFormat(@"<td><a href=""{0}"">See note.</a></td>", durationNoteGuid);
            }
            else
                therapyHtml.AppendFormat(@"<td>{0}</td>", pTherapy.duration);

            therapyHtml.Append(@"</tr>");
            return therapyHtml.ToString();
        }

        private static Guid generatePageForLinkedNotes(Entities pContext, List <BDLinkedNote> pNotes, Guid pParentId, BDConstants.BDNodeType pParentType)
        {
            StringBuilder noteHtml = new StringBuilder();

            foreach (BDLinkedNote note in pNotes)
            {
                if (note.documentText.Length > EMPTY_PARAGRAPH)
                {
                    if (noteHtml.ToString().Length > EMPTY_PARAGRAPH)
                        noteHtml.Append(@"<br>");
                    noteHtml.Append(note.documentText);
                }
            }

            BDHtmlPage notePage = BDHtmlPage.CreateHtmlPage(pContext);
            notePage.displayParentId = pParentId;
            notePage.displayParentType = (int)pParentType;
            notePage.documentText = headerHtml + noteHtml.ToString() + footerHtml;

            BDHtmlPage.Save(pContext, notePage);

            return notePage.Uuid;
        }

        private static bool notesListHasContent(List<BDLinkedNote> pNotes)
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
    }
}
