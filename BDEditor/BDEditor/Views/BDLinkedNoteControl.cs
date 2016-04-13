using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;

using TXTextControl;

namespace BDEditor.Views
{
    public partial class BDLinkedNoteControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? scopeId;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private IBDNode displayContextParent;
        private string contextPropertyName;
        private bool saveOnLeave = true;
        private string linkValue = string.Empty;
        private BDLinkedNote currentLinkedNote;
        private BDLinkedNoteView linkView;
      
        private BDLinkedNoteAssociation currentAssociation;

        public BDConstants.LinkedNoteType? DefaultLinkedNoteType = null;

        private bool showChildren = true;
        public bool ShowChildren
        {
            get { return showChildren; }
            set { showChildren = value; }
        }

        //private bool newLinkSaved = false;

        public event EventHandler SaveAttemptWithoutParent;

        public event EventHandler<NodeEventArgs> RequestItemAdd;
        public event EventHandler<NodeEventArgs> RequestItemDelete;

        public event EventHandler<NodeEventArgs> ReorderToPrevious;
        public event EventHandler<NodeEventArgs> ReorderToNext;

        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NameChanged;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnItemAddRequested(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = RequestItemAdd;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnItemDeleteRequested(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = RequestItemDelete;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnReorderToPrevious(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = ReorderToPrevious;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnReorderToNext(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = ReorderToNext;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnSaveAttemptWithoutParent(EventArgs e)
        {
            if (null != SaveAttemptWithoutParent) { SaveAttemptWithoutParent(this, e); }
        }

        public BDLinkedNote CurrentLinkedNote
        {
            get { return currentLinkedNote; }
            set { currentLinkedNote = value; }
        }

        public bool SaveOnLeave
        {
            get { return saveOnLeave; }
            set { saveOnLeave = value; }
        }

        public BDLinkedNoteControl()
        {
            InitializeComponent();
            textControl.ButtonBar = buttonBar;
        }

        private void BDLinkedNoteControl_Leave(object sender, EventArgs e)
        {
            if (SaveOnLeave)
                Save();
        }

        public void AssignContextPropertyName(string pContextPropertyName)
        {
            contextPropertyName = pContextPropertyName;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public void AssignDisplayContextParent(IBDNode pDisplayContextParent)
        {
            displayContextParent = pDisplayContextParent;
        }

        private void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(new NodeEventArgs());
        }

        protected virtual void OnNotesChanged(NodeEventArgs e)
        {
            ControlHelper.SuspendDrawing(this);
            if (this.linkValue.Length > 0 && this.linkView.HasNewLink)
            {
                // if this was called as a result of clicking the button (as opposed to clicking an existing link) insert new hypertextlink
                TXTextControl.HypertextLink newLink = new HypertextLink(textControl.Selection.Text, this.linkValue);
                textControl.Selection.Text = string.Empty;
                textControl.HypertextLinks.Add(newLink);

                // highlight the link text 
                textControl.Selection.Start = newLink.Start - 1;
                textControl.Selection.Length = newLink.Length;
                textControl.Selection.ForeColor = Color.Blue;
                textControl.Selection.Underline = TXTextControl.FontUnderlineStyle.Single;

                // reset the caret position and turn off color / underline
                // (color and underline tags get removed anyway when we clean the html on save)
                int newPosition = newLink.Start + newLink.Length;
                textControl.Select(newPosition + 1, 0);
                textControl.Selection.ForeColor = Color.Black;
                textControl.Selection.Underline = TXTextControl.FontUnderlineStyle.None;
            }

            int currentLocation = textControl.Selection.Start;
            int currentLength = textControl.Selection.Length;

            // re-evaluate all the hypertextlinks to see if they are valid.
            textControl.SelectAll();
            for (int i = 0; i < textControl.HypertextLinks.Count; i++)
            {
                HypertextLink h = textControl.HypertextLinks.GetItem(i);
                if (null != h)
                {
                    List<BDLinkedNoteAssociation> associationList = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(dataContext, currentLinkedNote.Uuid, h.Target);
                    //BDLinkedNoteAssociation na = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentLinkedNote.Uuid, h.Target);
                    if ((null == associationList) || (associationList.Count == 0) )
                    {
                        textControl.Select(h.Start - 1, h.Length);
                        textControl.Selection.ForeColor = Color.Black;
                        textControl.Selection.Underline = TXTextControl.FontUnderlineStyle.None;
                        textControl.HypertextLinks.Remove(h, true);
                    }
                }
            }
            // reset caret back to user's location
            textControl.Select(currentLocation, 0);
            Save();

            EventHandler<NodeEventArgs> handler = NotesChanged;
            if (null != handler) { handler(this, e); }
            ControlHelper.ResumeDrawing(this);
        }

        private void BDLinkedNoteControl_Load(object sender, EventArgs e)
        {
            RefreshLayout();
        }

        /// <summary>
        /// The LinkedNoteAssociation created by a save operation. 
        /// </summary>
        /// <returns>An association only if newly created. Null otherwise.</returns>
        public BDLinkedNoteAssociation CreatedLinkedNoteAssociation()
        {
            return currentAssociation; 
        }

        #region IBDControl implementation

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
        }

        public bool Save()
        {
            return Save(DefaultLinkedNoteType);
        }

        public bool Save(BDConstants.LinkedNoteType? pLinkedNoteType)
        {
            if (BDCommon.Settings.IsUpdating) return false;

            bool result = false;

            if (null == parentId)
            {
                System.Diagnostics.Debug.WriteLine(@"Linked Note OnSaveAttemptWithoutParent");
                OnSaveAttemptWithoutParent(new EventArgs());
            }
            else
            {
                bool isValidInternalLink = ( (null != pLinkedNoteType) && (pLinkedNoteType.Value == BDConstants.LinkedNoteType.InternalLink) );
                bool hasText = !string.IsNullOrEmpty(textControl.Text);

                if ((null == currentLinkedNote) && (isValidInternalLink || hasText) )
                {
                    CreateCurrentObject(pLinkedNoteType);
                }
                
                if (null != currentLinkedNote)
                { 
                    TXTextControl.SaveSettings ss = new TXTextControl.SaveSettings();

                    string plainText;
                    textControl.Save(out plainText, TXTextControl.StringStreamType.PlainText, ss);
                    string htmltext;
                    textControl.Save(out htmltext, TXTextControl.StringStreamType.HTMLFormat, ss);

                    string cleanText = WashText(htmltext);
                    if (currentLinkedNote.documentText != cleanText)
                    {
                        currentLinkedNote.documentText = cleanText;
                        if (plainText.Length > 127)
                            currentLinkedNote.previewText = plainText.Substring(0, 127);
                        else
                            currentLinkedNote.previewText = plainText;
                    }
                    BDLinkedNote.Save(dataContext, currentLinkedNote);
                    result = true;
                }
            }
            return result;
        }

        public void Delete()
        {
            //throw new NotImplementedException();
            System.Diagnostics.Debug.WriteLine("Delete Not implemented");
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            //throw new NotImplementedException();
            System.Diagnostics.Debug.WriteLine("'Show Links in Use' Not implemented");
        }

        public bool CreateCurrentObject()
        {
            return CreateCurrentObject(null);
        }

        public bool CreateCurrentObject(BDConstants.LinkedNoteType? pLinkedNoteType)
        {
            bool result = true;

            if (null == currentLinkedNote)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentLinkedNote = BDLinkedNote.CreateBDLinkedNote(dataContext);
                    BDConstants.LinkedNoteType linkedNoteType = BDConstants.LinkedNoteType.MarkedComment;
                    if (null != pLinkedNoteType)
                    {
                        linkedNoteType = pLinkedNoteType.Value;
                    }
                    currentAssociation = BDLinkedNoteAssociation.CreateBDLinkedNoteAssociation(dataContext, linkedNoteType, currentLinkedNote.Uuid, parentType, parentId.Value, contextPropertyName);
                    
                    currentLinkedNote.scopeId = scopeId;
                    BDLinkedNote.Save(dataContext, currentLinkedNote);
                    BDLinkedNoteAssociation.Save(dataContext, currentAssociation);
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            Boolean origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);
            if (textControl.Visible)
            {
                textControl.Text = @"";

                if (currentLinkedNote != null && currentLinkedNote.documentText != null && currentLinkedNote.documentText.Length > 0)
                {
                    if (currentLinkedNote.documentText.Contains("<"))
                        textControl.Append(currentLinkedNote.documentText, TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
                    else
                        textControl.Append(currentLinkedNote.documentText, TXTextControl.StringStreamType.RichTextFormat, TXTextControl.AppendSettings.None);
                }
                else
                {
                    textControl.Append(@"", TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
                }
            }
            ControlHelper.ResumeDrawing(this);
            BDCommon.Settings.IsUpdating = origState;
        }

        #endregion

        #region text cleaning / manipulation
        /// <summary>
        /// Get the HTML body from the text control contents
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        private string GetBodyContents(string pText)
        {
            // extract body contents from text
            string bodyText;
            int bodyStartIndex = (pText.IndexOf("<body"));
            bodyStartIndex = pText.IndexOf(">", bodyStartIndex);
            int bodyEndIndex = pText.IndexOf("</body>");
            if (bodyStartIndex > 0)
                bodyText = pText.Substring(bodyStartIndex + 1, (bodyEndIndex - bodyStartIndex) - 1);
            else bodyText = pText;

            return bodyText;
        }

        /// <summary>
        /// Remove all occurences of requested tag from provided string to end tag
        /// </summary>
        /// <param name="pString">Text to clean</param>
        /// <param name="pTagStart">Tag that begins the string to remove</param>
        /// <param name="pTagEnd">Tag that ends the string to remove</param>
        /// <param name="removeTagEnd">Boolean that indicates whether to remove the end tag with the operation</param>
        /// <returns></returns>
        private string CleanTagFromText(string pText, string pTagStart, string pTagEnd, bool removeTagEnd)
        {
            string cleanString = pText;
            while (cleanString.Contains(pTagStart))
            {
                int tagStartIndex = cleanString.IndexOf(pTagStart);
                int tagEndIndex = cleanString.IndexOf(pTagEnd, tagStartIndex);
                int tagLength = 0;
                if (removeTagEnd == true)
                    tagLength = tagEndIndex + pTagEnd.Length - tagStartIndex;
                else tagLength = tagEndIndex - tagStartIndex;

                cleanString = cleanString.Remove(tagStartIndex, tagLength);
            }
            return cleanString;
        }

        /// <summary>
        /// Remove extra markup from within a tag's boundaries but retain the tag itself
        /// </summary>
        /// <param name="pString"></param>
        /// <param name="pTagStart"></param>
        /// <param name="pTagEnd"></param>
        /// <param name="pTrimStart"></param>
        /// <returns></returns>
        private string CleanInnerTag(string pText, string pTagStart, string pTagEnd, string pTrimStart)
        {
            if (pText.Contains(pTagStart))
            {
                int tagStartIndex = pText.IndexOf(pTagStart);
                int trimStartIndex = pText.IndexOf(pTrimStart, tagStartIndex);
                int tagEndIndex = pText.IndexOf(pTagEnd, tagStartIndex);
                int tagLength = 0;

                tagLength = tagEndIndex - trimStartIndex;
                string cleanString = pText.Remove(trimStartIndex, tagLength);
                return cleanString;
            }
            return pText;
        }

        /// <summary>
        /// The Text Control inserts a span tag on every paragraph to control the font size.  This method strips it out.
        /// </summary>
        /// <param name="pString"></param>
        /// <param name="pTag"></param>
        /// <returns></returns>
        private string CleanFontStyleTag(string pText, string pTag)
        {
            if (pText.Contains(pTag))
            {
                string endTag = "</span>";
                int referenceIndex = pText.IndexOf(pTag);
                int tagStartIndex = pText.IndexOf(endTag, referenceIndex);

                string cleanString = pText.Remove(tagStartIndex, endTag.Length);
                cleanString = cleanString.Replace(pTag, "");
                return cleanString;
            }

            return pText;
        }

        private void ReformatTextControl(TXTextControl.TextControl pTextControl)
        {
            TXTextControl.SaveSettings ss = new TXTextControl.SaveSettings();
            string htmltext;
            pTextControl.Save(out htmltext, TXTextControl.StringStreamType.HTMLFormat, ss);
            string cleanText = WashText(htmltext);
            pTextControl.Text = @"";
            pTextControl.Append(cleanText, TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
        }

        /// <summary>
        /// For bold, italics and underline:  replace generated span tags with css span tags
        /// </summary>
        /// <param name="pStringToClean"></param>
        /// <returns></returns>
        private string resetSelectedTags(string pStringToClean)
        {
            string spanEnd = "</span>";
            string spanUnderlineTag = "<span style=\"text-decoration:underline ;\">";
            string spanBoldTag = "<span style=\"font-weight:bold;\">";
            string spanItalicsTag = "<span style=\"font-style:italic;\">";
            
            //  Not all combinations of the tags are coded specifically:  no matter what order the buttons are pressed,
            // the tags are always generated to the same span tag sequence.  For example pressing Underline then Bold
            // generates the same span tag as pressing Bold then Underline.

            string spanUnderlineBoldTag = "<span style=\"font-weight:bold;text-decoration:underline ;\">";
            string spanUnderlineItalicsTag = "<span style=\"font-style:italic;text-decoration:underline ;\">";
            string spanBoldItalicsTag = "<span style=\"font-weight:bold;font-style:italic;\">";
            string spanUnderlineBoldItalicsTag = "<span style=\"font-weight:bold;font-style:italic;text-decoration:underline ;\">";

            string cssSpanUnderlineTag = "<span class=\"underline\">";
            string cssSpanBoldTag = "<span class=\"body-bold\">";
            string cssSpanItalicsTag = "<span class=\"body-italic\">";
            string cssSpanUnderlineBoldTag = "<span class=\"body-underline-bold\">";
            string cssSpanUnderlineItalicsTag = "<span class=\"body-underline-italic\">";
            string cssSpanBoldItalicsTag = "<span class=\"body-bold-italic\">";
            string cssSpanUnderlineBoldItalicsTag = "<span class=\"body-underline-bold-italic\">";

            while (pStringToClean.Contains(spanUnderlineTag))
            {
                //int uStartIndex = pStringToClean.IndexOf(spanUnderlineTag);
                //pStringToClean = pStringToClean.Remove(uStartIndex, spanUnderlineTag.Length);
                //pStringToClean = pStringToClean.Insert(uStartIndex, @"<u>");
                //int uEndIndex = pStringToClean.IndexOf(spanEnd, uStartIndex);
                //pStringToClean = pStringToClean.Remove(uEndIndex, spanEnd.Length);
                //pStringToClean = pStringToClean.Insert(uEndIndex, @"</u>");
                pStringToClean = pStringToClean.Replace(spanUnderlineTag, cssSpanUnderlineTag);
            }

            while (pStringToClean.Contains(spanBoldTag))
            {
                pStringToClean = pStringToClean.Replace(spanBoldTag, cssSpanBoldTag);
                //int bStartIndex = pStringToClean.IndexOf(spanBoldTag);
                //pStringToClean = pStringToClean.Remove(bStartIndex, spanBoldTag.Length);
                //pStringToClean = pStringToClean.Insert(bStartIndex, @"<b>");
                //int bEndIndex = pStringToClean.IndexOf(spanEnd, bStartIndex);
                //pStringToClean = pStringToClean.Remove(bEndIndex, spanEnd.Length);
                //pStringToClean = pStringToClean.Insert(bEndIndex, @"</b>");
            }

            while (pStringToClean.Contains(spanItalicsTag))
            {
                pStringToClean = pStringToClean.Replace(spanItalicsTag, cssSpanItalicsTag);
                //int iStartIndex = pStringToClean.IndexOf(spanItalicsTag);
                //pStringToClean = pStringToClean.Remove(iStartIndex, spanItalicsTag.Length);
                //pStringToClean = pStringToClean.Insert(iStartIndex, @"<i>");
                //int iEndIndex = pStringToClean.IndexOf(spanEnd, iStartIndex);
                //pStringToClean = pStringToClean.Remove(iEndIndex, spanEnd.Length);
                //pStringToClean = pStringToClean.Insert(iEndIndex, @"</i>");
            }

            while (pStringToClean.Contains(spanUnderlineBoldTag))
            {
                pStringToClean = pStringToClean.Replace(spanUnderlineBoldTag, cssSpanUnderlineBoldTag);
                //int tStartIndex = pStringToClean.IndexOf(spanUnderlineBoldTag);
                //pStringToClean = pStringToClean.Remove(tStartIndex, spanUnderlineBoldTag.Length);
                //pStringToClean = pStringToClean.Insert(tStartIndex, @"<u><b>");
                //int tEndIndex = pStringToClean.IndexOf(spanEnd, tStartIndex);
                //pStringToClean = pStringToClean.Remove(tEndIndex, spanEnd.Length);
                //pStringToClean = pStringToClean.Insert(tEndIndex, @"</b></u>");
            }

            while (pStringToClean.Contains(spanUnderlineItalicsTag))
            {
                pStringToClean = pStringToClean.Replace(spanUnderlineItalicsTag, cssSpanUnderlineItalicsTag);
                //int tStartIndex = pStringToClean.IndexOf(spanUnderlineItalicsTag);
                //pStringToClean = pStringToClean.Remove(tStartIndex, spanUnderlineItalicsTag.Length);
                //pStringToClean = pStringToClean.Insert(tStartIndex, @"<u><i>");
                //int tEndIndex = pStringToClean.IndexOf(spanEnd, tStartIndex);
                //pStringToClean = pStringToClean.Remove(tEndIndex, spanEnd.Length);
                //pStringToClean = pStringToClean.Insert(tEndIndex, @"</i></u>");
            }

            while (pStringToClean.Contains(spanBoldItalicsTag))
            {
                pStringToClean = pStringToClean.Replace(spanBoldItalicsTag, cssSpanBoldItalicsTag);
                //int tStartIndex = pStringToClean.IndexOf(spanBoldItalicsTag);
                //pStringToClean = pStringToClean.Remove(tStartIndex, spanBoldItalicsTag.Length);
                //pStringToClean = pStringToClean.Insert(tStartIndex, @"<b><i>");
                //int tEndIndex = pStringToClean.IndexOf(spanEnd, tStartIndex);
                //pStringToClean = pStringToClean.Remove(tEndIndex, spanEnd.Length);
                //pStringToClean = pStringToClean.Insert(tEndIndex, @"</i></b>");
            }

            while (pStringToClean.Contains(spanUnderlineBoldItalicsTag))
            {
                pStringToClean = pStringToClean.Replace(spanUnderlineBoldItalicsTag, cssSpanUnderlineBoldItalicsTag);
                //int tStartIndex = pStringToClean.IndexOf(spanUnderlineBoldItalicsTag);
                //pStringToClean = pStringToClean.Remove(tStartIndex, spanUnderlineBoldItalicsTag.Length);
                //pStringToClean = pStringToClean.Insert(tStartIndex, @"<u><b><i>");
                //int tEndIndex = pStringToClean.IndexOf(spanEnd, tStartIndex);
                //pStringToClean = pStringToClean.Remove(tEndIndex, spanEnd.Length);
                //pStringToClean = pStringToClean.Insert(tEndIndex, @"</i></b></u>");
            }

            return pStringToClean;
        }

        /// <summary>
        /// Strip the text of the formatting tags that may exist (usually from a Word document) 
        ///  Also strip out the HTML inserted by the TXTextControl that we don't want to keep in the data.
        /// </summary>
        /// <param name="pString">String to search and clean</param>
        /// <returns></returns>
        private string WashText(string pText)
        {
            string stringToClean = GetBodyContents(pText);

            // clean out extra line returns
            stringToClean = stringToClean.Replace("\r\n", "");

            // replace span tags for bold and underline and italics >>  NOW HANDLED IN HTML GENERATION
            // stringToClean = resetSelectedTags(stringToClean);
            
            // remove style tags
            //stringToClean = CleanTagFromText(stringToClean, " style=", ">", false);

            // remove table tags
            stringToClean = CleanTagFromText(stringToClean, "<td", ">", true);
            stringToClean = stringToClean.Replace("</td>", "");

            stringToClean = CleanTagFromText(stringToClean, "<tr", ">", true);
            stringToClean = stringToClean.Replace("</tr>", "");

            stringToClean = CleanTagFromText(stringToClean, "<table", ">", true);
            stringToClean = stringToClean.Replace("</table>", "");


            // remove remaining span tags
           // stringToClean = CleanTagFromText(stringToClean, "<span", ">", true);
            //stringToClean = stringToClean.Replace("</span>", "");

            // remove paragraph tags from inside list tags
            //stringToClean = stringToClean.Replace("<li><p>", "<li>");
            //stringToClean = stringToClean.Replace("</p></li>", "</li>");

            //string listParagraphStart = @"<li><p>";
            //string listStart = @"<li>";
            //string paragraphEnd = @"</p>";

            //if(!string.IsNullOrEmpty(stringToClean))
            //{
            //    // do subscripts first because of double braces
            //    while (stringToClean.Contains(listParagraphStart))
            //    {
            //        int tStartIndex = stringToClean.IndexOf(listParagraphStart);
            //        int tEndIndex = stringToClean.IndexOf(paragraphEnd, tStartIndex);
            //        stringToClean = stringToClean.Remove(tStartIndex, listParagraphStart.Length);
            //        stringToClean = stringToClean.Insert(tStartIndex, listStart);
            //        stringToClean = stringToClean.Remove(tEndIndex, paragraphEnd.Length);
            //    }
            //}

            //Remove the "converted bullet" character sequence
            string bulletSequence = Char.ConvertFromUtf32(194) + Char.ConvertFromUtf32(183) + Char.ConvertFromUtf32(160);
            stringToClean = stringToClean.Replace(bulletSequence, "");
            // remove xml tags
            stringToClean = CleanTagFromText(stringToClean, "xmlns:", @"&quot;", true);
            stringToClean = CleanTagFromText(stringToClean, "xmlns=", @"&quot;", true);
            stringToClean = CleanTagFromText(stringToClean, "urn:", @"&quot;", true);
            stringToClean = CleanTagFromText(stringToClean, @"http://schemas.microsoft.com", @"&quot;", true);
            stringToClean = CleanTagFromText(stringToClean, @"http://www.w3.org", @"&quot;", true);
            stringToClean = CleanTagFromText(stringToClean, "<p>xmlns", "</p>", true);

            stringToClean = stringToClean.Replace("style=\"margin-top:6pt;margin-bottom:6pt;\"", "");

            // clean tags for font size
            string styleTag = "<span style=\"font-size:10pt;\">";
            while (stringToClean.Contains(styleTag))
                stringToClean = CleanFontStyleTag(stringToClean, styleTag);

            // cleanDocument code
            while (stringToClean.Contains("<ul style="))
                stringToClean = CleanInnerTag(stringToClean, "<ul style=", ">", " style");

            while (stringToClean.Contains("<ol style="))
                stringToClean = CleanInnerTag(stringToClean, "<ol style=", ">", " style");

            while (stringToClean.Contains("<li style="))
                stringToClean = CleanInnerTag(stringToClean, "<li style=", ">", " style");

            while (stringToClean.Contains("<p style="))
                stringToClean = CleanInnerTag(stringToClean, "<p style=", ">", " style");

            // TODO:  clean extra paragraph tags from end of string

            return stringToClean;
        }

        private void ShowDiagnostics()
        {
            System.Diagnostics.Debug.WriteLine("HTML>{0}", Clipboard.ContainsText(TextDataFormat.Html));
            System.Diagnostics.Debug.WriteLine("RTF>{0}", Clipboard.ContainsText(TextDataFormat.Rtf));
            System.Diagnostics.Debug.WriteLine("TEXT>{0}", Clipboard.ContainsText(TextDataFormat.Text));

            System.Diagnostics.Debug.WriteLine("Text");
            System.Diagnostics.Debug.WriteLine(Clipboard.GetText());

            if (Clipboard.ContainsText(TextDataFormat.Rtf))
            {
                System.Diagnostics.Debug.WriteLine("RTF");
                System.Diagnostics.Debug.WriteLine(Clipboard.GetText(TextDataFormat.Rtf));
            }
        }

        #endregion

        #region UI Events

        private void textControl_KeyUp(object sender, KeyEventArgs e)
        {
            // enable 'select all' so user can change font in one operation
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                textControl.SelectAll();
            }
            else if (e.KeyData == (Keys.Control | Keys.V))
            {
                System.Diagnostics.Debug.WriteLine("Ctrl-V");

                ReformatTextControl(textControl);
            }
            else if (e.KeyData == (Keys.Control | Keys.B))
            {
                textControl.Selection.Bold = !textControl.Selection.Bold;
            }
            else if (e.KeyData == (Keys.Control | Keys.U))
            {
                if (textControl.Selection.Underline != TXTextControl.FontUnderlineStyle.Single)
                    textControl.Selection.Underline = TXTextControl.FontUnderlineStyle.Single;
            }
            else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.R))
            {
                // retrieve new text as HTML
                TXTextControl.SaveSettings ss = new TXTextControl.SaveSettings();

                string htmltext;
                textControl.Save(out htmltext, TXTextControl.StringStreamType.HTMLFormat, ss);
                string cleanText = WashText(htmltext);
            }
            //NOTE:  Redo / Undo are automatically supported by the control.  Bold & underline are not.
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"ß"; // BDConstants.HTML_ENTITY_CODE_BETA;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"≥"; //BDConstants.HTML_ENTITY_CODE_GREATEROREQUALS;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"≤"; // BDConstants.HTML_ENTITY_CODE_LESSOREQUALS;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"±"; // BDConstants.HTML_ENTITY_CODE_PLUSMINUS;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"°"; // BDConstants.HTML_ENTITY_CODE_DEGREE;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            // superscript: move baseline of text up, reduce fontsize.
            textControl.Selection.Baseline = 100;
            textControl.Selection.FontSize -= 40;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            // subscript: move baseline of text down, reduce fontsize.
            textControl.Selection.Baseline = -100;
            textControl.Selection.FontSize -= 40;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            // Paste text without formatting
            textControl.Paste(ClipboardFormat.PlainText);
            ReformatTextControl(textControl);
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"µ"; //  BDConstants.HTML_ENTITY_CODE_MU;
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"®"; // BDConstants.HTML_ENTITY_CODE_REG_MARK;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (textControl.Selection.Length > 0)
                openLinkEditor(string.Empty);
            else
                MessageBox.Show("Please select the text to be linked.", "No Text Selected");
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"↑"; // BDConstants.HTML_ENTITY_CODE_ARROW_UP;
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"↓"; // BDConstants.HTML_ENTITY_CODE_ARROW_DOWN;
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"→"; // BDConstants.HTML_ENTITY_CODE_ARROW_RIGHT;
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = @"←"; // BDConstants.HTML_ENTITY_CODE_ARROW_LEFT
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = "√"; // BDConstants.HTML_ENTITY_CODE_CHECKMARK
        }

        private void textControl_HypertextLinkClicked(object sender, TXTextControl.HypertextLinkEventArgs e)
        {
            TXTextControl.HypertextLink link = textControl.HypertextLinks.GetItem();
            if (null != link)
            {
                if (!link.Target.Contains(@"http://"))
                    openLinkEditor(link.Target);
            }
        }
        #endregion

        /// <summary>
        /// Opening a new linked note editor:  this is a note-within-note situation
        /// The displayContextParent is assigned from the current note's parent.
        /// </summary>
        /// <param name="pProperty"></param>
        private void openLinkEditor(string pProperty)
        {
            Save();

            this.linkValue = (pProperty.Length > 0) ? pProperty : Guid.NewGuid().ToString();
            bool linkExists = (pProperty.Length > 0) ? true : false;

            linkView = new BDLinkedNoteView();
            linkView.AssignParentInfo(currentLinkedNote.Uuid, BDConstants.BDNodeType.BDLinkedNote);
            linkView.AssignDataContext(dataContext);
            linkView.AssignScopeId(scopeId);
            linkView.AssignContextPropertyName(linkValue);
            linkView.AssignDisplayContextParent(parentId.Value);

            linkView.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
            linkView.ShowDialog(this);
            linkView.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);

            this.linkValue = string.Empty;
        }


        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                // on purpose: should never be trying to get the node of a linked note.
                throw new NotSupportedException();
            }
            set
            {
                // on purpose
                if (null != value)
                {
                    throw new NotSupportedException();
                }
            }
        }

        public int? DisplayOrder { get; set; }

        public bool ShowAsChild { get; set; }

    }
}
