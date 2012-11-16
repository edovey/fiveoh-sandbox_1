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
        /// <param name="pProcessPackage"></param>
        /// <param name="pOwnerUuid"></param>
        /// <returns></returns>
        static public void processForOwnerUuid(Entities pContext, ref BDHtmlProcessPackage pProcessPackage, Guid pOwnerUuid, string pPropertyName)
        {
            if (null == pProcessPackage) throw new System.ArgumentNullException("pProcessPackage");

            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(pContext, pOwnerUuid);
            foreach (BDLinkedNoteAssociation association in links)
            {
                processNoteAssociation(pContext, ref pProcessPackage, association);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pProcessPackage"></param>
        /// <param name="pNoteAssociation"></param>
        /// <returns>Guid of a BDHtmlPage to replace the propertyname (which is a uuid) embedded in a note-within-note link</returns>
        static private Guid? processNoteAssociation(Entities pContext, ref BDHtmlProcessPackage pProcessPackage, BDLinkedNoteAssociation pNoteAssociation)
        {
            Guid? htmlPageUuid = null; // guid that will replace the linked note association uuid embedded in a note-within-note link

            if (null == pProcessPackage) throw new System.ArgumentNullException("pProcessPackage");

            BDLinkedNote note = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, pNoteAssociation.linkedNoteId);

            // process any notes-within-note

            processNotesWithinNote(pContext, ref pProcessPackage, note);

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

            return htmlPageUuid;
        }

        /// <summary>
        /// Recursive method to walk the notes-within-note hierarchy
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pProcessPackage"></param>
        /// <param name="pNote"></param>
        /// <returns>Guid of a BDHtmlPage to replace the propertyname-uuid embedded in a note-within-note link</returns>
        static private Guid? processNotesWithinNote(Entities pContext, ref BDHtmlProcessPackage pProcessPackage, BDLinkedNote pNote)
        {
            Guid? replacementGuid = null;

            if (null == pNote) return replacementGuid;

            string documentText = pNote.documentText;

            string compareString = @"<a href=";
            StringBuilder newString = new StringBuilder();
            if (documentText.Contains(compareString))
            {
                int startPosition = 0;

                while (startPosition < documentText.Length)
                {
                    // find the anchor tag
                    int tagLocation = documentText.IndexOf(compareString, startPosition);
                    if (tagLocation >= 0)
                    {
                        // inspect the 'guid'
                        int guidStart = tagLocation + 1 + compareString.Length;
                        string guidString = documentText.Substring(guidStart, 36);
                        // if the guid exists as an external URL, dont change it...
                        if (!guidString.Contains("http://www"))
                        {
                            Guid anchorGuid = new Guid(guidString);

                            BDLinkedNoteAssociation linkedAssociation = BDLinkedNoteAssociation.RetrieveLinkedNoteAssociationWithId(pContext, anchorGuid);

                            BDLinkedNote linkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(pContext, linkedAssociation.linkedNoteId);

                            Guid? innerReplacementGuid = processNotesWithinNote(pContext, ref pProcessPackage, linkedNote);
                            documentText.Replace(anchorGuid.ToString(), innerReplacementGuid.ToString().ToUpper());


                        }
                        startPosition = guidStart;
                    }
                    else
                        startPosition = documentText.Length;
                }
            }

            return replacementGuid;
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
