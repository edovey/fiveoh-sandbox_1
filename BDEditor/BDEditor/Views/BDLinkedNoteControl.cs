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
        //private Guid? linkedNoteAssociationId;
        private string contextEntityName;
        private string contextPropertyName;
        private IBDControl parentControl;
        private bool saveOnLeave = true;
        private LinkedNoteType selectedLinkNoteType;

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
                {
                    rtfLinkNoteText.Rtf = @"";
                    selectedLinkNoteType = LinkedNoteType.Default;
                }
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

        public LinkedNoteType SelectedLinkedNoteType
        {
            get { return selectedLinkNoteType; }
            set { selectedLinkNoteType = value; }
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

        public void AssignContextEntityName(string pContextEntityName)
        {
            contextEntityName = pContextEntityName;
        }

        public void AssignContextPropertyName(string pContextPropertyName)
        {
            contextPropertyName = pContextPropertyName;
        }

        //public void AssignLinkedNoteAssociationId(Guid? pLinkedNoteAssociationId)
        //{
        //    linkedNoteAssociationId = pLinkedNoteAssociationId;
        //}

        #region IBDControl

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignParentId(Guid? pParentId)
        {
            contextParentId = pParentId;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
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
                if ((null == currentLinkedNote) && (rtfLinkNoteText.Text != string.Empty) ) //Check the Text property because .rtf usually has formatting described
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
                    if (currentLinkedNote.documentText != rtfLinkNoteText.Rtf)
                    {
                        currentLinkedNote.documentText = rtfLinkNoteText.Rtf;
                        if (rtfLinkNoteText.Text.Length > 127)
                            currentLinkedNote.previewText = rtfLinkNoteText.Text.Substring(0, 127);
                        else
                            currentLinkedNote.previewText = rtfLinkNoteText.Text;
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
