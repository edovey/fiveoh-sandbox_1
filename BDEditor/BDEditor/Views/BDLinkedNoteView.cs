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
        private BDConstants.BDNodeType parentType;
        private string contextPropertyName;
        private Guid? displayContextParentUuid;
        private Guid? parentId;
        private Guid? scopeId;
        private bool isRendering = false;
        private bool hasNewLink = false;
        private List<BDLinkedNoteAssociation> parentPropertyNoteAssociationList;
        private BDLinkedNoteAssociation currentNoteAssociation;
        private List<BDLinkedNoteAssociation> currentNoteAssociationsList;
        private List<BDLinkedNote> existingNotesInScopeList;

        private BDConstants.BDNodeType internalLinkNodeType = BDConstants.BDNodeType.None;
        private Guid? internalLinkNodeId = null;

        public event EventHandler<NodeEventArgs> NotesChanged;

        protected virtual void OnNotesChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NotesChanged;
            if (null != handler) { handler(this, e); }
        }

        public bool HasNewLink
        {
            get { return hasNewLink; }
            set { }
        }

        public BDLinkedNoteView()
        {
            InitializeComponent();
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

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
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

        public void AssignDisplayContextParent(Guid pDisplayContextParentUuid)
        {
            displayContextParentUuid = pDisplayContextParentUuid;
        }

        private void RefreshListOfAssociatedLinks()
        {
            this.Cursor = Cursors.WaitCursor;
            isRendering = true;

            chListLinks.Items.Clear();

            if (null != currentNoteAssociation)
            {
                currentNoteAssociationsList = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForLinkedNoteId(dataContext, currentNoteAssociation.linkedNoteId.Value);

                if (currentNoteAssociationsList.Count > 0)
                {
                    for (int i = 0; i < currentNoteAssociationsList.Count; i++)
                    {
                        bool isCurrent = (currentNoteAssociationsList[i].uuid == currentNoteAssociation.uuid);
                        string description = string.Format("{0} #{1}", currentNoteAssociationsList[i].GetDescription(dataContext), currentNoteAssociationsList[i].displayOrder.ToString());
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
                    if (null != bdLinkedNoteControl1.CurrentLinkedNote)
                    {
                        isCurrent = (existingNotesInScopeList[i].uuid == bdLinkedNoteControl1.CurrentLinkedNote.uuid);
                    }

                    BDLinkedNote note = existingNotesInScopeList[i];

                    string description = string.Format("{0} [{1}]", @"-", note.previewText);
                    chListNotes.Items.Add(description, isCurrent);
                }
            }

            isRendering = false;

            if (null != scopeId)
            {
                IBDNode scopeNode = BDFabrik.RetrieveNode(dataContext, scopeId);
                lblScopeTitle.Text = scopeNode.Name;
            }
            else
            {
                lblScopeTitle.Text = "";
            }
            this.Cursor = Cursors.Default;
        }

        private void DisplayLinkedNote(BDLinkedNoteAssociation pAssociation, bool withRefresh)
        {
            if (null == pAssociation)
            {
                this.linkedNoteTypeCombo.SelectedIndex = (int)BDConstants.LinkedNoteType.MarkedComment;

                bdLinkedNoteControl1.CurrentLinkedNote = null;
                btnPrevious.Enabled = (this.parentPropertyNoteAssociationList.Count > 0);
                btnNext.Enabled = false;
                lblNoteCounter.Text = string.Empty;
            }
            else
            {
                this.internalLinkNodeId = pAssociation.internalLinkNodeId;
                this.internalLinkNodeType = pAssociation.InternalLinkNodeType;

                this.linkedNoteTypeCombo.SelectedIndex = pAssociation.linkedNoteType.Value;

                BDLinkedNote linkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(dataContext, pAssociation.linkedNoteId);
                bdLinkedNoteControl1.CurrentLinkedNote = linkedNote;

                int idx = this.parentPropertyNoteAssociationList.IndexOf(pAssociation);
                btnPrevious.Enabled = (idx > 0);
                btnNext.Enabled = (idx < this.parentPropertyNoteAssociationList.Count - 1);
                lblNoteCounter.Text = pAssociation.displayOrder.ToString();
            }

            //RefreshListOfAssociatedLinks();
            RefreshSelectedTab();

            if (withRefresh)
                bdLinkedNoteControl1.RefreshLayout();
        }

        private void SaveCurrentNote()
        {
            hasNewLink = false;

            BDConstants.LinkedNoteType linkedNoteType = (BDConstants.LinkedNoteType)Enum.Parse(typeof(BDConstants.LinkedNoteType), this.linkedNoteTypeCombo.GetItemText(this.linkedNoteTypeCombo.SelectedItem));

            if (bdLinkedNoteControl1.Save(linkedNoteType))  // BDLinkedNoteControl will create the association as required
            {
                // If the currentNoteAssociation is null, the note *should* also have been null and the note control should have created a new note + association
                if (null == currentNoteAssociation)
                    currentNoteAssociation = bdLinkedNoteControl1.CreatedLinkedNoteAssociation();

                if (null != currentNoteAssociation)
                {
                    currentNoteAssociation.linkedNoteType = (int)Enum.Parse(typeof(BDConstants.LinkedNoteType), this.linkedNoteTypeCombo.GetItemText(this.linkedNoteTypeCombo.SelectedItem));
                    currentNoteAssociation.internalLinkNodeId = internalLinkNodeId;
                    currentNoteAssociation.internalLinkNodeType = (int)internalLinkNodeType;

                    dataContext.SaveChanges();
                    lblNoteCounter.Text = currentNoteAssociation.displayOrder.ToString();


                }
                hasNewLink = true;
            }
            else
            {
                if (null != bdLinkedNoteControl1.CurrentLinkedNote)
                {
                    // DELETE linked notes & associations if they exist
                    BDLinkedNote.Delete(dataContext, bdLinkedNoteControl1.CurrentLinkedNote, true);
                    dataContext.SaveChanges();
                }
            }

            OnNotesChanged(new NodeEventArgs());
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
           SaveCurrentNote();
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
            if (!isRendering)
                e.NewValue = e.CurrentValue;
        }

        private void btnAssignNote_Click(object sender, EventArgs e)
        {
            if ((chListNotes.SelectedIndex >= 0) && chListNotes.SelectedIndex < existingNotesInScopeList.Count())
            {
                BDLinkedNote selectedNote = existingNotesInScopeList[chListNotes.SelectedIndex];

                if (null == this.currentNoteAssociation)
                {
                    this.currentNoteAssociation = BDLinkedNoteAssociation.CreateBDLinkedNoteAssociation(dataContext);

                    BDConstants.LinkedNoteType noteType = (BDConstants.LinkedNoteType)Enum.Parse(typeof(BDConstants.LinkedNoteType), this.linkedNoteTypeCombo.GetItemText(this.linkedNoteTypeCombo.SelectedItem));
                    this.currentNoteAssociation = BDLinkedNoteAssociation.CreateBDLinkedNoteAssociation(dataContext, noteType, selectedNote.uuid, parentType, parentId.Value, contextPropertyName);
                }

                this.currentNoteAssociation.linkedNoteId = selectedNote.uuid;

                BDLinkedNoteAssociation.Save(dataContext, this.currentNoteAssociation);

                DisplayLinkedNote(this.currentNoteAssociation, true);

                tabControl1.SelectedIndex = 0;
            }
        }

        private void chListNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enabled = true;
            btnAssignNote.Text = @"Assign";
            if ((null != currentNoteAssociation) && ((chListNotes.SelectedIndex >= 0) && chListNotes.SelectedIndex < existingNotesInScopeList.Count()))
            {
                BDLinkedNote selectedNote = existingNotesInScopeList[chListNotes.SelectedIndex];
                enabled = (selectedNote.uuid != currentNoteAssociation.linkedNoteId);
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
            if (null != currentNoteAssociation)
            {
                BDLinkedNoteAssociation.Delete(dataContext, currentNoteAssociation);
                this.currentNoteAssociation = null;
                DisplayLinkedNote(null, true);
                RefreshSelectedTab();
            }
        }

        private void DeleteCurrentNote_Action(object sender, EventArgs e)
        {
            if (null != currentNoteAssociation)
            {
                if (MessageBox.Show("This will also delete all linked associations.", "Delete Note", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    BDLinkedNote.Delete(dataContext, currentNoteAssociation.linkedNoteId.Value, true);

                    this.currentNoteAssociation = null;
                    DisplayLinkedNote(null, true);
                    RefreshSelectedTab();
                    OnNotesChanged(new NodeEventArgs());
                }
            }
        }

        private void linkedNoteType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BDConstants.LinkedNoteType linkedNoteType = (BDConstants.LinkedNoteType)Enum.Parse(typeof(BDConstants.LinkedNoteType), this.linkedNoteTypeCombo.GetItemText(this.linkedNoteTypeCombo.SelectedItem));
            if (null != currentNoteAssociation)
                currentNoteAssociation.LinkedNoteType = linkedNoteType;

            if (linkedNoteType == BDConstants.LinkedNoteType.InternalLink)
            {
                showInternalLinkInfo();
                panelInternalLink.Visible = true;
            }
            else
            {
                panelInternalLink.Visible = false;
            }
        }

        private void BDLinkedNoteView_Load(object sender, EventArgs e)
        {
            List<BDConstants.LinkedNoteType> noteTypes = Enum.GetValues(typeof(BDConstants.LinkedNoteType)).Cast<BDConstants.LinkedNoteType>().ToList<BDConstants.LinkedNoteType>();
            this.linkedNoteTypeCombo.DataSource = noteTypes;

            bdLinkedNoteControl1.CurrentLinkedNote = null;

            this.parentPropertyNoteAssociationList = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(dataContext, parentId, contextPropertyName);
            // Since this is the form load, populate with the first (if exists) entry

            this.currentNoteAssociation = null;
            if ((null != this.parentPropertyNoteAssociationList) && (this.parentPropertyNoteAssociationList.Count > 0))
            {
                this.currentNoteAssociation = this.parentPropertyNoteAssociationList[0];
                if ( (null != this.currentNoteAssociation) && (this.currentNoteAssociation.displayOrder < 0) )
                {
                    int displayOrder = 1;
                    for (int idx = 0; idx < parentPropertyNoteAssociationList.Count; idx++)
                    {
                        BDLinkedNoteAssociation entry = parentPropertyNoteAssociationList[idx];
                        entry.displayOrder = displayOrder++;
                        BDLinkedNoteAssociation.Save(dataContext, entry);
                    }
                    this.currentNoteAssociation = this.parentPropertyNoteAssociationList[0];
                }

                rtfContextInfo.Text = BDLinkedNoteAssociation.GetDescription(dataContext, parentId, parentType, contextPropertyName);
            }
            
            DisplayLinkedNote(this.currentNoteAssociation, true);
        }

        private void addAnotherNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentNote();
            this.parentPropertyNoteAssociationList = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(dataContext, parentId, contextPropertyName);
            currentNoteAssociation = null;
            DisplayLinkedNote(this.currentNoteAssociation, true);
        }

        private void movePreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void moveNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            SaveCurrentNote();
            this.parentPropertyNoteAssociationList = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(dataContext, parentId, contextPropertyName);

            int idx = this.parentPropertyNoteAssociationList.IndexOf(currentNoteAssociation);
            if (idx > 0)
            {
                currentNoteAssociation = this.parentPropertyNoteAssociationList[idx - 1];
            }
            DisplayLinkedNote(this.currentNoteAssociation, true);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentNote();
            this.parentPropertyNoteAssociationList = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(dataContext, parentId, contextPropertyName);

            int idx = this.parentPropertyNoteAssociationList.IndexOf(currentNoteAssociation);
            if (idx < this.parentPropertyNoteAssociationList.Count - 1)
            {
                currentNoteAssociation = this.parentPropertyNoteAssociationList[idx + 1];
            }
            DisplayLinkedNote(this.currentNoteAssociation, true);
        }

        private void btnInternalLink_Click(object sender, EventArgs e)
        {
            BDInternalLinkChooserDialog dialog = new BDInternalLinkChooserDialog(dataContext);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                internalLinkNodeType = dialog.SelectedNodeType;
                internalLinkNodeId = dialog.SelectedUuid;

                showInternalLinkInfo();
            }
        }

        private void showInternalLinkInfo()
        {
            if (null != internalLinkNodeId)
            {
                IBDNode node = BDFabrik.RetrieveNode(dataContext, internalLinkNodeType, internalLinkNodeId);
                if (null != node)
                {
                    lblInternalLinkDescription.Text = node.ToString();
                    lblInternalLinkDescription.ForeColor = Color.Black;
                }
                else
                {
                    lblInternalLinkDescription.Text = @"Target is no longer valid: Has it been deleted?";
                    lblInternalLinkDescription.ForeColor = Color.Red;
                }
            }
        }

        private void btnChangeScope_Click(object sender, EventArgs e)
        {
            Guid? selectedScopeUuid = scopeId;

            IBDNode scopeNode = BDFabrik.RetrieveNode(dataContext, selectedScopeUuid);
            BDInternalLinkChooserDialog dialog = new BDInternalLinkChooserDialog(dataContext);
            dialog.Setup(scopeNode);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedScopeUuid = dialog.SelectedUuid;
                AssignScopeId(selectedScopeUuid);
                RefreshListOfScopeNotes();
            }
        }

        private void editIndexStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentNote();

                // owner of the index/search entry at this point will be the linkedNoteAssociation
                // this will be adjusted in post processing to point to the page where the linked note was rendered
                // display Context in the index/search entry is always determined by the last IBDNode in the hierarchy
                BDLinkedNote currentNote = BDLinkedNote.RetrieveLinkedNoteWithId(dataContext, currentNoteAssociation.linkedNoteId.Value);
                if (currentNote != null)
                {
                    BDSearchEntryEditView iEditView = new BDSearchEntryEditView();
                    iEditView.AssignDataContext(dataContext);
                    iEditView.AssignCurrentLinkedNoteAssociation(currentNoteAssociation);
                    if (null != displayContextParentUuid)
                        iEditView.AssignDisplayContextParent(displayContextParentUuid.Value);
                    else
                        iEditView.AssignDisplayContextParent(parentId.Value);
                   // string contextString = BDUtilities.BuildHierarchyString(dataContext, BDFabrik.RetrieveNode(dataContext, parentId), " : ");
                    //iEditView.DisplayContext = contextString;
                    iEditView.ShowDialog(this);
                }
        }
    }
}
