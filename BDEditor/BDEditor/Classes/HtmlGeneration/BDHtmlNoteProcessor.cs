using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.Classes;
using BDEditor.DataModel;

namespace BDEditor.Classes.HtmlGeneration
{
    public class BDHtmlNoteProcessor
    {
        private BDHtmlNoteProcessor() { }

        /// <summary>
        /// Process all notes for a given owner/parent uuid, examining and processing all 'notes within note'
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pParentProcessPackage"></param>
        /// <param name="pOwnerUuid"></param>
        /// <returns></returns>
        static public void processForOwnerUuidAndProperty(Entities pContext, ref BDHtmlProcessPackage pParentProcessPackage, Guid pOwnerUuid, string pPropertyName)
        {
            if (null == pParentProcessPackage) throw new System.ArgumentNullException("pParentProcessPackage");

            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(pContext, pOwnerUuid, pPropertyName);

            BDHtmlProcessPackage forwardProcessPackage = new BDHtmlProcessPackage();

            // group by type
            foreach (BDLinkedNoteAssociation association in links)
            {
                BDLinkedNote note = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, association.linkedNoteId);
                BDHtmlLinkedNote noteWrapper = BDHtmlLinkedNote.CopyFromLinkedNote(note);
                noteWrapper.InternalLinkUuid = association.internalLinkNodeId;

                switch (association.LinkedNoteType)
                {
                    case BDConstants.LinkedNoteType.Endnote:
                    case BDConstants.LinkedNoteType.Footnote:
                        forwardProcessPackage.FootnoteNoteList.Add(noteWrapper);
                        break;
                    case BDConstants.LinkedNoteType.Inline:
                        forwardProcessPackage.InlineNoteList.Add(noteWrapper);
                        break;
                    case BDConstants.LinkedNoteType.InternalLink:
                        forwardProcessPackage.InternalLinkList.Add(noteWrapper);
                        break;
                    case BDConstants.LinkedNoteType.Legend:
                        forwardProcessPackage.LegendNoteList.Add(noteWrapper);
                        break;
                    case BDConstants.LinkedNoteType.Reference:
                    case BDConstants.LinkedNoteType.ReferenceEndnote:
                        forwardProcessPackage.ReferenceNoteList.Add(noteWrapper);
                        break;
                    case BDConstants.LinkedNoteType.MarkedComment:
                    case BDConstants.LinkedNoteType.UnmarkedComment:
                        forwardProcessPackage.CreatePageNoteList.Add(noteWrapper);
                        break;
                    default:
                        break;
                }
            }

            BDHtmlProcessPackage internalLinkPkg = processNoteList(pContext, forwardProcessPackage.InternalLinkList);
            BDHtmlProcessPackage mcPkg = null;
            if (forwardProcessPackage.InternalLinkList.Count <= 0) // An Internal-Link note takes precedence over other page producing notes
            {
                mcPkg = processNoteList(pContext, forwardProcessPackage.CreatePageNoteList);
            }

            BDHtmlProcessPackage footnotePkg = processNoteList(pContext, forwardProcessPackage.FootnoteNoteList);
            BDHtmlProcessPackage inlinePkg = processNoteList(pContext, forwardProcessPackage.InlineNoteList);
            
            BDHtmlProcessPackage referencePkg = processNoteList(pContext, forwardProcessPackage.ReferenceNoteList);
            BDHtmlProcessPackage legendPkg = processNoteList(pContext, forwardProcessPackage.LegendNoteList);
           
            // sew everything together here before returning results in the pParentProcessPackage

            pParentProcessPackage.AggregatePackages(footnotePkg, inlinePkg, internalLinkPkg, referencePkg, legendPkg, mcPkg);
        }

        static private BDHtmlProcessPackage processNoteList(Entities pContext, BDHtmlNoteList pNoteList)
        {
            BDHtmlProcessPackage returnPackage = new BDHtmlProcessPackage();

            if ((null == pNoteList) || (pNoteList.Count == 0)) return returnPackage;

            // create a single page with the aggregation of any "page-notes" (i.e. marked comments)
            // return back collections of fully processed notes grouped by type. (i.e. footnotes, legends)
            // return back inline html

            if (pNoteList.ListType == BDHtmlNoteList.BDHtmlNoteListType.InternalLink) // Internal-Links do not support Notes-Within-note nor more than one per link
            {
                if(pNoteList.Count > 0)
                {
                    BDHtmlLinkedNote note = pNoteList[0];
                    returnPackage.CreatedHtmlPageUuid = note.InternalLinkUuid;
                    if (note.DocumentText.Length > BDHtmlPageGenerator.EMPTY_PARAGRAPH)
                    {
                        returnPackage.InlineHtmlSegmentList.Add(new BDHtmlSegment(note.DocumentText));
                    }
                }
            }
            else
            {
                for (int idx = 0; idx < pNoteList.Count; idx++)
                {
                    BDHtmlLinkedNote note = pNoteList[idx];
                    BDHtmlProcessPackage pkg = processNotesWithinNote(pContext, note);
                    returnPackage.AppendPackageTransients(pkg);
                }
            }

            //
            switch (pNoteList.ListType)
            {
                case BDHtmlNoteList.BDHtmlNoteListType.Inline:

                case BDHtmlNoteList.BDHtmlNoteListType.Footnote:
                case BDHtmlNoteList.BDHtmlNoteListType.Legend:
                case BDHtmlNoteList.BDHtmlNoteListType.Reference:
                    // put together the 
                    break;
                case BDHtmlNoteList.BDHtmlNoteListType.Page:
                    // generate an Html page
                    break;
                case BDHtmlNoteList.BDHtmlNoteListType.InternalLink:
                    //do nothing
                    break;
            }

            // process any notes-within-note
            /*
            processNotesWithinNote(pContext, ref pProcessPackage, note);
            */
            /*
            // build pages, replace internal links, or pass along
            switch (pNoteAssociation.LinkedNoteType)
            {
                case BDConstants.LinkedNoteType.Endnote:
                case BDConstants.LinkedNoteType.Footnote:
                case BDConstants.LinkedNoteType.Inline:
                case BDConstants.LinkedNoteType.InternalLink:
                case BDConstants.LinkedNoteType.Legend:
                case BDConstants.LinkedNoteType.MarkedComment:
                case BDConstants.LinkedNoteType.Reference:
                case BDConstants.LinkedNoteType.ReferenceEndnote:
                case BDConstants.LinkedNoteType.UnmarkedComment:
                    break;
                default:
                    break;
            }
            */
            return returnPackage;
        }

        /// <summary>
        /// Recursive method to walk the notes-within-note hierarchy. Replace the propertyname-uuid embedded in the note-within-note links
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNote"></param>
        /// <returns>A processPackage containing the processed InlineHtmlSegments from the note documentText</returns>
        static private BDHtmlProcessPackage processNotesWithinNote(Entities pContext, BDHtmlLinkedNote pNote)
        {
            BDHtmlProcessPackage returnPackage = new BDHtmlProcessPackage();

            string documentText = pNote.DocumentText;

            string anchorStartString = @"<a href=";
            string anchorEndString = @"</a>";
            StringBuilder newString = new StringBuilder();
            if (documentText.Contains(anchorStartString))
            {
                int startPosition = 0;

                while (startPosition < documentText.Length)
                {
                    // find the anchor tag
                    int tagLocation = documentText.IndexOf(anchorStartString, startPosition);
                    if (tagLocation >= 0)
                    {
                        int endTagLocation = documentText.IndexOf(anchorEndString, tagLocation);
                        // inspect the 'guid'
                        int guidStart = tagLocation + 1 + anchorStartString.Length;
                        string guidString = documentText.Substring(guidStart, 36);
                        // if the guid exists as an external URL, dont change it...
                        if (guidString.Contains("http://www"))
                        {
                            startPosition = guidStart;
                        }
                        else
                        {
                            startPosition = endTagLocation + (anchorEndString.Length + 1);


                            // When finished this guid WILL be replaced with EITHER a generated Html page uuid OR a BDNode uuid
                            Guid abstractPropertyName = new Guid(guidString);

                            BDHtmlProcessPackage noteWithinNotePackage = new BDHtmlProcessPackage();

                            if (pNote.InternalLinkUuid.HasValue)
                            {
                                noteWithinNotePackage.CreatedHtmlPageUuid = pNote.InternalLinkUuid.Value;
                                
                            }
                            else
                            {
                                processForOwnerUuidAndProperty(pContext, ref noteWithinNotePackage, pNote.Uuid, abstractPropertyName.ToString());
                            }

                            string originalAnchor = string.Format("{0}\"{1}\">", anchorStartString, guidString);

                            //makes changes to the documentText based on what is returned in the processPackage

                            // if a new page was generated, replace the guid (which is an abstract-property) with the html page uuid
                            if (noteWithinNotePackage.CreatedHtmlPageUuid.HasValue)
                            {
                                documentText.Replace(guidString, noteWithinNotePackage.CreatedHtmlPageUuid.Value.ToString().ToUpper());
                            }

                            string inlineHtml = noteWithinNotePackage.InlineHtml;
                            if (inlineHtml.Length > 0)
                            {
                                documentText.Insert(startPosition, inlineHtml);
                                startPosition += inlineHtml.Length; // update the start position so that any embedded links (already handled) are bypassed in this pass              
                            }

                            returnPackage.AppendPackageTransients(noteWithinNotePackage); // everything else is handled by the caller
                        }
                    }
                    else
                    {
                        startPosition = documentText.Length;
                    }
                }

                pNote.DocumentText = documentText; // update the note wrapper with the cleaned up text
            }

            returnPackage.InlineHtmlSegmentList.Add(new BDHtmlSegment(documentText));

            return returnPackage;
        }

        static private BDHtmlPage writePage(Entities pContext, IBDObject pDisplayParent, BDHtmlSegment pHtmlSegment, BDHtmlProcessPackage pProcessPackage )
        {
            /*
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
            if (newPage == null)
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
            if (newPage.layoutVariant == -1)
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

            if (pDisplayParent != null)
                pagesMap.Add(new BDHtmlPageMap(newPage.Uuid, pDisplayParent.Uuid));

            List<Guid> filteredObjects = pObjectsOnPage.Distinct().ToList();
            foreach (Guid objectId in filteredObjects)
            {
                pagesMap.Add(new BDHtmlPageMap(newPage.Uuid, objectId));
            }

            */
            return null;
        }
    }
}
