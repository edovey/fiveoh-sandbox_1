using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDLinkedNoteView : Form
    {
        private Entities dataContext;
        private Constants.BDNodeType parentType;
        private string contextPropertyName;
        private Guid? parentId;
        private Guid? scopeId;
        private bool isRendering = false;
        private BDLinkedNoteAssociation existingAssociation;
        private List<BDLinkedNoteAssociation> existingLinksList;
        private List<BDLinkedNote> existingNotesInScopeList;

        public event EventHandler NotesChanged;

        protected virtual void OnNotesChanged(EventArgs e)
        {
            if (null != NotesChanged) { NotesChanged(this, e); }
        }

        public BDLinkedNoteView()
        {
            InitializeComponent();
        }

        public void AssignLinkedNoteType(LinkedNoteType pType, bool isEditable, bool isRestrictedType)
        {
            this.linkedNoteTypeCombo.SelectedItem = pType;
            this.linkedNoteTypeCombo.Enabled = isEditable;
            if (isRestrictedType)
            {
                List<LinkedNoteType> noteTypes = Enum.GetValues(typeof(LinkedNoteType)).Cast<LinkedNoteType>().ToList<LinkedNoteType>();
                noteTypes.Remove(LinkedNoteType.Comment);
                // for disease and presentation, the 'inline' type is the overview which is already represented on the view
                noteTypes.Remove(LinkedNoteType.Inline);
                this.linkedNoteTypeCombo.DataSource = noteTypes;
            }
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdLinkedNoteControl1.AssignDataContext(dataContext);
        }

        public void AssignContextPropertyName(string pContextPropertyName)
        {
            contextPropertyName = pContextPropertyName;
            bdLinkedNoteControl1.AssignContextPropertyName(contextPropertyName);
        }

        public void AssignParentInfo(Guid? pParentId, Constants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
            bdLinkedNoteControl1.AssignParentInfo(pParentId, pParentType);
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
            //bdLinkedNoteControl1.CurrentLinkedNote = null;

            //this.existingAssociation = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, parentId, contextPropertyName);
            //rtfContextInfo.Text = BDLinkedNoteAssociation.GetDescription(dataContext, parentId, parentType, contextPropertyName);
            //if (null != this.existingAssociation)
            //{
            //    this.linkedNoteTypeCombo.SelectedIndex = this.existingAssociation.linkedNoteType.Value;
            //}

            //RefreshListOfAssociatedLinks();
            //DisplayLinkedNote(this.existingAssociation, false);
        }

        private void RefreshListOfAssociatedLinks()
        {
            this.Cursor = Cursors.WaitCursor;
            isRendering = true;

            chListLinks.Items.Clear();

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

        private void DisplayLinkedNote(BDLinkedNoteAssociation pAssociation, bool withRefresh)
        {
            if (null == pAssociation)
            {
                bdLinkedNoteControl1.CurrentLinkedNote = null;
            }
            else
            {
                BDLinkedNote linkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, pAssociation.linkedNoteId);
                bdLinkedNoteControl1.CurrentLinkedNote = linkedNote;
            }

            if (withRefresh)
                bdLinkedNoteControl1.RefreshLayout();
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
            RefreshSelectedTab();
        }

        private void RefreshSelectedTab()
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

                if (null == this.existingAssociation)
                {
                    this.existingAssociation = BDLinkedNoteAssociation.CreateLinkedNoteAssociation(dataContext);
                    this.existingAssociation.parentType = (int)parentType;
                    this.existingAssociation.parentKeyPropertyName = contextPropertyName;
                    this.existingAssociation.parentId = parentId;
                }

                this.existingAssociation.linkedNoteId = selectedNote.uuid;
                

                BDLinkedNoteAssociation.Save(dataContext, this.existingAssociation);

                DisplayLinkedNote(this.existingAssociation, true);

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

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void RemoveCurrentAssociation_Action(object sender, EventArgs e)
        {
            if (null != existingAssociation)
            {
                BDLinkedNoteAssociation.Delete(dataContext, existingAssociation);
                this.existingAssociation = null;
                DisplayLinkedNote(null, true);
                RefreshSelectedTab();
            }
        }

        private void DeleteCurrentNote_Action(object sender, EventArgs e)
        {
            if (null != existingAssociation)
            {
                if (MessageBox.Show("This will also delete all linked associations.", "Delete Note", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    BDLinkedNote.Delete(dataContext, existingAssociation.linkedNoteId.Value, true);

                    this.existingAssociation = null;
                    DisplayLinkedNote(null, true);
                    RefreshSelectedTab();
                    OnNotesChanged(new EventArgs());
                }
            }
        }

        private void linkedNoteType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(null != existingAssociation)
                existingAssociation.linkedNoteType = linkedNoteTypeCombo.SelectedIndex;
        }

        private void BDLinkedNoteView_Load(object sender, EventArgs e)
        {
            List<LinkedNoteType> noteTypes = Enum.GetValues(typeof(LinkedNoteType)).Cast<LinkedNoteType>().ToList<LinkedNoteType>();
            this.linkedNoteTypeCombo.DataSource = noteTypes;

            bdLinkedNoteControl1.CurrentLinkedNote = null;

            this.existingAssociation = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, parentId, contextPropertyName);
            rtfContextInfo.Text = BDLinkedNoteAssociation.GetDescription(dataContext, parentId, parentType, contextPropertyName);
            if (null != this.existingAssociation)
            {
                this.linkedNoteTypeCombo.SelectedIndex = this.existingAssociation.linkedNoteType.Value;
            }

            RefreshListOfAssociatedLinks();
            DisplayLinkedNote(this.existingAssociation, false);
        }
    }
}
