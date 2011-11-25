﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
namespace BDEditor.Views
{
    public partial class BDLinkedNoteControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? scopeId;
        private Guid? contextParentId;
        private string contextEntityName;
        private string contextPropertyName;
        private bool saveOnLeave = true;
        private LinkedNoteType selectedLinkNoteType;
        private BDLinkedNote currentLinkedNote;

        public event EventHandler SaveAttemptWithoutParent;

        protected virtual void OnSaveAttemptWithoutParent(EventArgs e)
        {
            if (null != SaveAttemptWithoutParent) { SaveAttemptWithoutParent(this, e); }
        }

        public BDLinkedNote CurrentLinkedNote
        {
            get { return currentLinkedNote; }
            set { currentLinkedNote = value;  }
        }

        public bool SaveOnLeave
        {
            get { return saveOnLeave; }
            set { saveOnLeave = value; }
        }

        public LinkedNoteType SelectedLinkedNoteType
        {
            get { return selectedLinkNoteType; }
            set { selectedLinkNoteType = value; }
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

        public void AssignContextEntityName(string pContextEntityName)
        {
            contextEntityName = pContextEntityName;
        }

        public void AssignContextPropertyName(string pContextPropertyName)
        {
            contextPropertyName = pContextPropertyName;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
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
        public void AssignParentId(Guid? pParentId)
        {
            contextParentId = pParentId;
        }
        public bool Save()
        {
            bool result = false;

            if (null == contextParentId)
            {
                System.Diagnostics.Debug.WriteLine(@"Linked Note OnSaveAttemptWithoutParent");
                OnSaveAttemptWithoutParent(new EventArgs());
            }
            else
            {
                if ((null == currentLinkedNote) && (textControl.Text != string.Empty)) 
                {
                    CreateCurrentObject();
                }
                if (null != currentLinkedNote)
                {
                    TXTextControl.SaveSettings ss = new TXTextControl.SaveSettings();
                    
                    string plainText;
                    textControl.Save(out plainText, TXTextControl.StringStreamType.PlainText,ss);
                    string htmltext;
                    textControl.Save(out htmltext, TXTextControl.StringStreamType.HTMLFormat, ss);
                    string cleanText = CleanDocumentText(htmltext);

                    if (currentLinkedNote.documentText != cleanText)
                    {
                        currentLinkedNote.documentText = cleanText;
                        if (plainText.Length > 127)
                            currentLinkedNote.previewText = plainText.Substring(0, 127);
                        else
                            currentLinkedNote.previewText = plainText;
                    }
                    BDLinkedNote.SaveLinkedNote(dataContext, currentLinkedNote);
                }
            }
            return result;
        }

        public void Delete()
        {
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == currentLinkedNote)
            {
                if (null == this.contextParentId)
                {
                    result = false;
                }
                else
                {
                    currentLinkedNote = BDLinkedNote.CreateLinkedNote(dataContext);
                    BDLinkedNoteAssociation association = BDLinkedNoteAssociation.CreateLinkedNoteAssociation(dataContext);
                    association.linkedNoteId = currentLinkedNote.uuid;
                    association.parentId = contextParentId;
                    association.parentEntityName = contextEntityName;
                    association.parentEntityPropertyName = contextPropertyName;
                    association.linkedNoteType = (int)selectedLinkNoteType;
                    currentLinkedNote.linkedNoteAssociationId = association.uuid;
                    currentLinkedNote.scopeId = scopeId;
                    BDLinkedNote.SaveLinkedNote(dataContext, currentLinkedNote);
                    BDLinkedNoteAssociation.SaveLinkedNoteAssociation(dataContext, association);
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            if (null == currentLinkedNote)
            {
                textControl.Text = @"";
                selectedLinkNoteType = LinkedNoteType.Default;
            }

            if (currentLinkedNote != null && currentLinkedNote.documentText != null && currentLinkedNote.documentText.Length > 0)
            {
                if (currentLinkedNote.documentText.Contains("<"))
                    textControl.Append(currentLinkedNote.documentText, TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
                else
                    textControl.Append(currentLinkedNote.documentText, TXTextControl.StringStreamType.RichTextFormat, TXTextControl.AppendSettings.None);
            }
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
        /// Remove requested tag from provided string to end tag
        /// </summary>
        /// <param name="pText">Text to clean</param>
        /// <param name="pTagStart">Tag that begins the string to remove</param>
        /// <param name="pTagEnd">Tag that ends the string to remove</param>
        /// <param name="removeTagEnd">Boolean that indicates whether to remove the end tag with the operation</param>
        /// <returns></returns>
        private string CleanTagFromText(string pText, string pTagStart, string pTagEnd, bool removeTagEnd)
        {
            if (pText.Contains(pTagStart))
            {
                int tagStartIndex = pText.IndexOf(pTagStart);
                int tagEndIndex = pText.IndexOf(pTagEnd, tagStartIndex);
                int tagLength = 0;
                if (removeTagEnd == true)
                    tagLength = tagEndIndex + pTagEnd.Length - tagStartIndex;
                else tagLength = tagEndIndex - tagStartIndex;
    
                string cleanString = pText.Remove(tagStartIndex, tagLength);
                return cleanString;
            }
            return pText;
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
        /// Strip the text of the formatting tags that may exist (usually from a Word document)
        /// </summary>
        /// <param name="pString">String to search and clean</param>
        /// <returns></returns>
        private string CleanClipboardText(string pString)
        {
            string stringToClean = GetBodyContents(pString);
            // remove table tags
            while (stringToClean.Contains("<td"))
                stringToClean = CleanTagFromText(stringToClean, "<td", ">", true);
            stringToClean = stringToClean.Replace("</td>", "");

            while (stringToClean.Contains("<tr"))
                stringToClean = CleanTagFromText(stringToClean, "<tr", ">", true);
            stringToClean = stringToClean.Replace("</tr>", "");

            while (stringToClean.Contains("<table"))
                stringToClean = CleanTagFromText(stringToClean, "<table", ">", true);
            stringToClean = stringToClean.Replace("</table>", "");

            // remove span tags
            while (stringToClean.Contains("<span"))
                stringToClean = CleanTagFromText(stringToClean, "<span", ">", true);
            stringToClean = stringToClean.Replace("</span>", "");

            // remove style tags
            while (stringToClean.Contains(" style="))
                stringToClean = CleanTagFromText(stringToClean, " style=", ">", false);

            // clean out extra line returns
            stringToClean = stringToClean.Replace("\r\n", "");

            // remove paragraph tags from inside list tags
            stringToClean = stringToClean.Replace("<li><p>", "<li>");
            stringToClean = stringToClean.Replace("</p></li>", "</li>");

            // remove xml tags
            stringToClean = CleanTagFromText(stringToClean, "<p>xmlns", "</p>", true);

           // TODO:  clean extra paragraph tags from end of string

            return stringToClean;
        }

        /// <summary>
        /// strip out the HTML inserted by the TXTextControl that we don't want to keep in the data.
        /// Called just before save.
        /// </summary>
        /// <param name="pString">String to parse.</param>
        /// <returns></returns>
        private string CleanDocumentText(string pString)
        {
            string stringToClean = GetBodyContents(pString);

            // remove style tags from ul, ol, li and p
            while (stringToClean.Contains("<ul style="))
                stringToClean = CleanInnerTag(stringToClean, "<ul style=", ">", " style");

            while (stringToClean.Contains("<ol style="))
                stringToClean = CleanInnerTag(stringToClean, "<ol style=", ">", " style");

            while (stringToClean.Contains("<li style=")) 
                stringToClean = CleanInnerTag(stringToClean, "<li style=", ">", " style");

            while (stringToClean.Contains("<p style=")) 
                stringToClean = CleanInnerTag(stringToClean, "<p style=", ">", " style");

            // remove paragraph tags from inside list tags
            stringToClean = stringToClean.Replace("<li><p>", "<li>");
            stringToClean = stringToClean.Replace("</p></li>", "</li>");

            // clean out extra line returns
            stringToClean = stringToClean.Replace("\r\n", "");

            // remove paragraph tags from inside list tags
            stringToClean = stringToClean.Replace("<li><p>", "<li>");
            stringToClean = stringToClean.Replace("</p></li>", "</li>");

            stringToClean = stringToClean.Replace("style=\"margin-top:6pt;margin-bottom:6pt;\"", "");

            return stringToClean;
        }

        /// <summary>
        /// Paste the text from the clipboard into the control, then retrieve it and clean it
        /// </summary>
        private void PasteCleanText()
        {
            if (Clipboard.ContainsText(TextDataFormat.Html))
            {
                textControl.Append(Clipboard.GetText(TextDataFormat.Html), TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
            }
            else if (Clipboard.ContainsText(TextDataFormat.Rtf))
            {
                textControl.Append(Clipboard.GetText(TextDataFormat.Rtf), TXTextControl.StringStreamType.RichTextFormat, TXTextControl.AppendSettings.None);
            }

            // retrieve new text as HTML
            TXTextControl.SaveSettings ss = new TXTextControl.SaveSettings();

            string htmltext;
            textControl.Save(out htmltext, TXTextControl.StringStreamType.HTMLFormat, ss);
            string cleanText = CleanClipboardText(htmltext);

            if (cleanText.Length > 0)
            {
                textControl.Text = @"";
                textControl.Append(cleanText, TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
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
            } if (e.KeyData == (Keys.Control | Keys.V))
            {
                PasteCleanText();
            }
            else if(e.KeyData == (Keys.Control | Keys.B))
            {
                textControl.Selection.Bold = !textControl.Selection.Bold;
            }
            else if (e.KeyData == (Keys.Control | Keys.U))
            {
                if(textControl.Selection.Underline != TXTextControl.FontUnderlineStyle.Single)
                    textControl.Selection.Underline = TXTextControl.FontUnderlineStyle.Single;
            }
            else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.R))
            {
                // retrieve new text as HTML
                TXTextControl.SaveSettings ss = new TXTextControl.SaveSettings();

                string htmltext;
                textControl.Save(out htmltext, TXTextControl.StringStreamType.HTMLFormat, ss);
                string cleanText = CleanClipboardText(htmltext);
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
            PasteCleanText();
        }
        #endregion


        
    }
}
