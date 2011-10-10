using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDLinkedNoteView : Form
    {
        Entities dataContext;
        string contextEntityName;
        string contextPropertyName;
        Guid? parentId;
        Guid? scopeId;
        bool isRendering = false;
        BDLinkedNoteAssociation existingAssociation;

        List<BDLinkedNoteAssociation> existingLinksList;
        List<BDLinkedNote> existingNotesInScopeList;
        
        public BDLinkedNoteView()
        {
            InitializeComponent();
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdLinkedNoteControl1.AssignDataContext(dataContext);
        }

        public void AssignContextEntityName(string pContextEntityName)
        {
            contextEntityName = pContextEntityName;
            bdLinkedNoteControl1.AssignContextEntityName(contextEntityName);
        }

        public void AssignContextPropertyName(string pContextPropertyName)
        {
            contextPropertyName = pContextPropertyName;
            bdLinkedNoteControl1.AssignContextPropertyName(contextPropertyName);
        }

        public void AssignParentId(Guid? pParentId)
        {
            parentId = pParentId;
            bdLinkedNoteControl1.AssignParentId(parentId);
        }

        public void AssignParentControl(IBDControl pParentControl)
        {
            bdLinkedNoteControl1.AssignParentControl(pParentControl);
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
            bdLinkedNoteControl1.AssignScopeId(pScopeId);
        }

        /// <summary>
        /// Fetch the LinkedNote associations related to the context attributes
        /// </summary>
        public void PopulateControl()
        {
            bdLinkedNoteControl1.CurrentLinkedNote = null;
            
            RefreshListOfAssociatedLinks();
            //RefreshListOfScopeNotes();
        }

        private void RefreshListOfAssociatedLinks()
        {
            this.Cursor = Cursors.WaitCursor;
            isRendering = true;

            chListLinks.Items.Clear();

            rtfContextInfo.Text = BDLinkedNoteAssociation.GetDescription(dataContext, parentId, contextEntityName, contextPropertyName);

            existingAssociation = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, parentId, contextPropertyName);
            if (null != existingAssociation)
            {
                existingLinksList = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForLinkedNoteId(dataContext, existingAssociation.linkedNoteId.Value);

                if (existingLinksList.Count > 0)
                {
                    for (int i = 0; i < existingLinksList.Count; i++)
                    {
                        bool isCurrent = (existingLinksList[i].uuid == existingAssociation.uuid);
                        string description = existingLinksList[i].GetDescription(dataContext);
                        chListLinks.Items.Add(description, isCurrent);
                        if (isCurrent)
                        {
                            rtfContextInfo.Text = description;
                            DisplayLinkedNote(existingLinksList[i]);
                        }
                    }
                }
            }

            isRendering = false;
            this.Cursor = Cursors.Default;
        }

        private void RefreshListOfScopeNotes()
        {
            this.Cursor = Cursors.WaitCursor;

            isRendering = true;

            btnAssignNote.Visible = false;
            chListNotes.Items.Clear();
            chListNotes.SelectedIndex = -1;

            existingNotesInScopeList = BDLinkedNote.GetLinkedNotesForScopeId(dataContext, scopeId);
            if (existingNotesInScopeList.Count > 0)
            {
                for (int i = 0; i < existingNotesInScopeList.Count; i++)
                {
                    bool isCurrent = false;
                    if(null != bdLinkedNoteControl1.CurrentLinkedNote)
                    {
                        isCurrent = (existingNotesInScopeList[i].uuid == bdLinkedNoteControl1.CurrentLinkedNote.uuid);
                    }

                    BDLinkedNote note = existingNotesInScopeList[i];

                    string description = string.Format("{0} [{1}]", @"-", note.previewText);
                    chListNotes.Items.Add(description, isCurrent);
                }
            }

            isRendering = false;

            this.Cursor = Cursors.Default;
        }

        private void DisplayLinkedNote(BDLinkedNoteAssociation pAssociation)
        {
            if (null == pAssociation)
            {
                bdLinkedNoteControl1.CurrentLinkedNote = null;
            }
            else
            {
                BDLinkedNote linkedNote = BDLinkedNote.GetLinkedNoteForId(dataContext, pAssociation.linkedNoteId);
                bdLinkedNoteControl1.CurrentLinkedNote = linkedNote;
                bdLinkedNoteControl1.SelectedLinkedNoteType = (LinkedNoteType)pAssociation.linkedNoteType;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bdLinkedNoteControl1.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    RefreshListOfAssociatedLinks();
                    break;
                case 1:
                    RefreshListOfScopeNotes();
                    break;
                default:
                    
                    break;
            }
        }

        private void linkedNoteView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(!isRendering)
                e.NewValue = e.CurrentValue;
        }

        private void btnAssignNote_Click(object sender, EventArgs e)
        {
            if ((chListNotes.SelectedIndex >= 0) && chListNotes.SelectedIndex < existingNotesInScopeList.Count())
            {
                BDLinkedNote selectedNote = existingNotesInScopeList[chListNotes.SelectedIndex];

                BDLinkedNoteAssociation assignedLink;
                if (null == existingAssociation)
                {
                    assignedLink = BDLinkedNoteAssociation.CreateLinkedNoteAssociation(dataContext);
                    assignedLink.parentEntityName = contextEntityName;
                    assignedLink.parentEntityPropertyName = contextPropertyName;
                    assignedLink.parentId = parentId;
                }
                else
                {
                    assignedLink = existingAssociation;
                }

                assignedLink.linkedNoteId = selectedNote.uuid;

                BDLinkedNoteAssociation.SaveLinkedNoteAssociation(dataContext, assignedLink);

                tabControl1.SelectedIndex = 0;
            }
        }

        private void chListNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enabled = true;
            btnAssignNote.Text = @"Assign";
            if ((null != existingAssociation) && ((chListNotes.SelectedIndex >= 0) && chListNotes.SelectedIndex < existingNotesInScopeList.Count()))
            {
                BDLinkedNote selectedNote = existingNotesInScopeList[chListNotes.SelectedIndex];
                enabled = (selectedNote.uuid != existingAssociation.linkedNoteId);
                if (enabled)
                {
                    btnAssignNote.Text = @"Reassign";
                    btnAssignNote.Visible = true;
                }
                else
                {
                    btnAssignNote.Visible = false;
                }
            }
            else
            {
                btnAssignNote.Visible = enabled && (chListNotes.SelectedIndex >= 0);
            }
            btnAssignNote.Enabled = enabled && (chListNotes.SelectedIndex >= 0);
        }
    }
}
