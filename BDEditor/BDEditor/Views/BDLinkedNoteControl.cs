using System;
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
        private IBDControl parentControl;
        private bool saveOnLeave = true;
        private LinkedNoteType selectedLinkNoteType;
        private BDLinkedNote currentLinkedNote;

        public BDLinkedNote CurrentLinkedNote
        {
            get { return currentLinkedNote; }
            set
            {
                currentLinkedNote = value;
                if (currentLinkedNote == null)
                {
                    textControl.Text = @"";
                    selectedLinkNoteType = LinkedNoteType.Default;
                }
            }
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

        private string CleanDocumentText(string pString)
        {
            string stringToClean = GetBodyContents(pString);
            // remove table tags
            while (stringToClean.Contains("<td"))
                stringToClean = CleanTagFromText(stringToClean, "<td", ">",true);
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
            stringToClean = stringToClean.Replace("</span>","");
            // remove style tags
            while (stringToClean.Contains(" style="))
                stringToClean = CleanTagFromText(stringToClean, " style=", ">", false);

            // clean out extra line returns
            stringToClean = stringToClean.Replace("\r\n", "");

            // remove paragraph tags from inside list tags
            stringToClean = stringToClean.Replace("<li><p>", "<li>");
            stringToClean = stringToClean.Replace("</p></li>", "</li>");
            return stringToClean;
        }

        #region IBDControl
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            //dataContext = new Entities();
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
                if (null != parentControl)
                {
                    System.Diagnostics.Debug.WriteLine(@"Triggering parent create");
                    parentControl.TriggerCreateAndAssignParentIdToChildControl(this);
                }
            }
            else
            {
                if ((null == currentLinkedNote) && (textControl.Text != string.Empty)) 
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
                if (null != currentLinkedNote)
                {
                    TXTextControl.SaveSettings ss = new TXTextControl.SaveSettings();
                    
                    string plainText;
                    textControl.Save(out plainText, TXTextControl.StringStreamType.PlainText,ss);
                    //string richText;
                    //textControl.Save(out richText, TXTextControl.StringStreamType.RichTextFormat);
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

        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void BDLinkedNoteControl_Load(object sender, EventArgs e)
        {
            if (currentLinkedNote != null && currentLinkedNote.documentText != null && currentLinkedNote.documentText.Length > 0)
            {
                if (currentLinkedNote.documentText.Contains("<"))
                    textControl.Append(currentLinkedNote.documentText, TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
                else
                    textControl.Append(currentLinkedNote.documentText, TXTextControl.StringStreamType.RichTextFormat, TXTextControl.AppendSettings.None);
            }
        }

        private void textControl_KeyUp(object sender, KeyEventArgs e)
        {
           // enable 'select all' so user can change font in one operation
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                textControl.SelectAll();
                e.Handled = true;
            }
            //NOTE:  Redo / Undo are automatically supported by the control.  Bold is not.
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            textControl.SelectAll();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if(Clipboard.ContainsText(TextDataFormat.Html))
            {
                textControl.Append(Clipboard.GetText(TextDataFormat.Html), TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
            }
            else if (Clipboard.ContainsText(TextDataFormat.Rtf))
            {
                textControl.Append(Clipboard.GetText(TextDataFormat.Rtf), TXTextControl.StringStreamType.RichTextFormat, TXTextControl.AppendSettings.None);
            }
        }

        private void btnBeta_Click(object sender, EventArgs e)
        {
            textControl.Selection.Text = "ß";
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            TXTextControl.SaveSettings ss = new TXTextControl.SaveSettings();

            string htmltext;
            textControl.Save(out htmltext, TXTextControl.StringStreamType.HTMLFormat, ss);
            string cleanText = CleanDocumentText(htmltext);

            if (cleanText.Length > 0)
            {
                textControl.Text = @"";
                textControl.Append(cleanText, TXTextControl.StringStreamType.HTMLFormat, TXTextControl.AppendSettings.None);
            }
        }

    }
}
