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
        private string contextPropertyName;
        private bool saveOnLeave = true;
        private string linkValue = string.Empty;
        private BDLinkedNote currentLinkedNote;
        private BDLinkedNoteView linkView;
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
                    BDLinkedNoteAssociation na = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentLinkedNote.Uuid, h.Target);
                    if (null == na)
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
            bool result = false;

            if (null == parentId)
            {
                System.Diagnostics.Debug.WriteLine(@"Linked Note OnSaveAttemptWithoutParent");
                OnSaveAttemptWithoutParent(new EventArgs());
            }
            else
            {
                if ((null == currentLinkedNote) && !string.IsNullOrEmpty(textControl.Text))
                {
                    CreateCurrentObject();
                }
                if (null != currentLinkedNote && !string.IsNullOrEmpty(textControl.Text))
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
            throw new NotImplementedException();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            throw new NotImplementedException();
        }

        public bool CreateCurrentObject()
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
                    BDLinkedNoteAssociation association = BDLinkedNoteAssociation.CreateBDLinkedNoteAssociation(dataContext);
                    association.linkedNoteId = currentLinkedNote.uuid;
                    association.parentId = parentId;
                    association.parentType = (int)parentType;
                    association.parentKeyPropertyName = contextPropertyName;
                    association.linkedNoteType =(int) BDConstants.LinkedNoteType.MarkedComment;
                    currentLinkedNote.scopeId = scopeId;
                    BDLinkedNote.Save(dataContext, currentLinkedNote);
                    BDLinkedNoteAssociation.Save(dataContext, association);
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            RefreshLayout(true);
        }

        public void RefreshLayout(bool pShowChildren)
        {
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
        }

        #endregion

        #region text cleaning / manipulation
        /// <summary>
        /// Get the HTML body from the text control contents
        /// </summary>
        /// <param name="pText"></param>
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
        /// <param name="pText">Text to clean</param>
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
        /// <param name="pText"></param>
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
        /// <param name="pText"></param>
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
        /// Strip the text of the formatting tags that may exist (usually from a Word document) also strip out the HTML inserted by the TXTextControl that we don't want to keep in the data.
        /// </summary>
        /// <param name="pText">String to search and clean</param>
        /// <returns></returns>
        private string WashText(string pText)
        {
            string stringToClean = GetBodyContents(pText);

            // remove table tags
            stringToClean = CleanTagFromText(stringToClean, "<td", ">", true);
            stringToClean = stringToClean.Replace("</td>", "");

            stringToClean = CleanTagFromText(stringToClean, "<tr", ">", true);
            stringToClean = stringToClean.Replace("</tr>", "");

            stringToClean = CleanTagFromText(stringToClean, "<table", ">", true);
            stringToClean = stringToClean.Replace("</table>", "");

            // remove span tags
            stringToClean = CleanTagFromText(stringToClean, "<span", ">", true);
            stringToClean = stringToClean.Replace("</span>", "");

            // remove style tags
            stringToClean = CleanTagFromText(stringToClean, " style=", ">", false);

            // clean out extra line returns
            stringToClean = stringToClean.Replace("\r\n", "");

            // remove paragraph tags from inside list tags
            stringToClean = stringToClean.Replace("<li><p>", "<li>");
            stringToClean = stringToClean.Replace("</p></li>", "</li>");

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
            textControl.Selection.Text = "ß";
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = "≥";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = "≤";
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = "±";
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = "°";
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
            textControl.Paste();
            ReformatTextControl(textControl);
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = "µ";
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (textControl.Selection.Length > 0)
                openLinkEditor(string.Empty);
            else
                MessageBox.Show("Please select the text to be linked.", "No Text Selected");
        }

        private void textControl_HypertextLinkClicked(object sender, TXTextControl.HypertextLinkEventArgs e)
        {
            TXTextControl.HypertextLink link = textControl.HypertextLinks.GetItem();
            if (!link.Target.Contains(@"http://"))
                openLinkEditor(link.Target);
        }
        #endregion

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
                throw new NotSupportedException();
            }
        }

        public int? DisplayOrder { get; set; }

        public bool ShowAsChild { get; set; }
    }
}
