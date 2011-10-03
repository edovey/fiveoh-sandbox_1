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
        private Guid? parentId;
        private string contextPropertyName;
        private IBDControl parentControl;
        private bool saveOnLeave = true;

        private BDLinkedNote currentLinkedNote;

        public BDLinkedNote CurrentLinkedNote
        {
            get
            {
                return currentLinkedNote;
            }
            set
            {
                currentLinkedNote = value;
                if (currentLinkedNote == null)
                    rtfLinkNoteText.Rtf = @"";
                else
                    rtfLinkNoteText.Rtf = currentLinkedNote.documentText;
            }
        }

        public bool SaveOnLeave
        {
            get
            {
                return saveOnLeave;
            }
            set
            {
                saveOnLeave = value;
            }
        }

        public BDLinkedNoteControl()
        {
            InitializeComponent();
        }

        private void BDLinkedNoteControl_Leave(object sender, EventArgs e)
        {
            if (SaveOnLeave)
                Save();
        }

        #region IBDControl

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignParentId(Guid? pParentId)
        {
            parentId = pParentId;
        }

        public void AssignContextPropertyName(string pContextPropertyName)
        {
            contextPropertyName = pContextPropertyName;
        }

        public bool Save()
        {
            bool result = false;
            if (null != parentId)
            { 
                if ((null == currentLinkedNote) && (rtfLinkNoteText.Rtf != string.Empty) )
                {
                    currentLinkedNote = BDLinkedNote.CreateLinkedNote(dataContext, parentId.Value, contextPropertyName);
                }

                if (null != currentLinkedNote)
                {
                    if (currentLinkedNote.documentText != rtfLinkNoteText.Rtf)
                    {
                        currentLinkedNote.documentText = rtfLinkNoteText.Rtf;
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

        private void tbLinkedNote_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
