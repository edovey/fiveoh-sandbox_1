using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDRegimenGroupControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private BDRegimenGroup currentRegimenGroup;
        private IBDControl parentControl;
        private Guid? scopeId;

        public int? DisplayOrder { get; set; }
        public int? ColumnOrder { get; set; }

        private List<BDRegimenControl> regimenControlList = new List<BDRegimenControl>();

        private bool showChildren = true;
        public bool ShowChildren
        {
            get { return showChildren; }
            set { showChildren = value; }
        }

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

        protected virtual void OnNotesChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NotesChanged;
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

        public BDRegimenGroupControl()
        {
            InitializeComponent();
        }

        public BDRegimenGroup CurrentRegimenGroup
        {
            get { return currentRegimenGroup; }
            set { currentRegimenGroup = value; }
        }

        public void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            bool origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            string lblString = string.Format(@"u={0} : p={1} : {2}", CurrentNode.Uuid, CurrentNode.ParentId, BDUtilities.GetEnumDescription(CurrentNode.LayoutVariant));
            toolTip1.SetToolTip(regimenGroupName, lblString);

            ControlHelper.SuspendDrawing(this);

            // This is generic for Constants.LayoutVariantType.TreatmentRecommendation01

            for (int idx = 0; idx < regimenControlList.Count; idx++)
            {
                BDRegimenControl control = regimenControlList[idx];
                removeRegimenControl(control, false);
            }

            regimenControlList.Clear();
            panelRegimens.Controls.Clear();

            if (null == currentRegimenGroup)
            {
                tbName.Text = @"";
                noneRadioButton.Checked = true;
                lblInfo.Text = "na";
            }
            else
            {
                lblInfo.Text = currentRegimenGroup.DisplayOrder.ToString();
                tbName.Text = currentRegimenGroup.name;
                switch ((BDRegimenGroup.RegimenGroupJoinType)currentRegimenGroup.regimenGroupJoinType)
                {
                    case BDRegimenGroup.RegimenGroupJoinType.None:
                        noneRadioButton.Checked = true;
                        break;
                    case BDRegimenGroup.RegimenGroupJoinType.AndWithNext:
                        andRadioButton.Checked = true;
                        break;
                    case BDRegimenGroup.RegimenGroupJoinType.OrWithNext:
                        orRadioButton.Checked = true;
                        break;
                    default:
                        noneRadioButton.Checked = true;
                        break;
                }

                if(currentRegimenGroup.regimenOfChoice.Value)
                    cbRegimenOfChoice.Checked = true;

                if (currentRegimenGroup.alternativeRegimen.Value)
                    cbAlternativeRegimen.Checked = true;

                if (pShowChildren)
                {
                    List<BDRegimen> list = BDRegimen.RetrieveBDRegimensForParentUuid(dataContext, currentRegimenGroup.uuid);
                    for (int idx = 0; idx < list.Count; idx++)
                    {
                        BDRegimen entry = list[idx];
                        addRegimenControl(entry, idx);
                    }
                }
            }

            ShowLinksInUse(false);

            ControlHelper.ResumeDrawing(this);

            BDCommon.Settings.IsUpdating = origState;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource)
        {
            tbName.AutoCompleteCustomSource = pSource;
            tbName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbName.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentRegimenGroup) ? this.currentRegimenGroup.uuid : Guid.Empty);
            btnRegimenGroupLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnRegimenGroupLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnRegimenGroupConjunctionLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnRegimenGroupConjunctionLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < regimenControlList.Count; idx++)
                {
                    regimenControlList[idx].ShowLinksInUse(true);
                }
            }
        }

        #region IBDControl

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        public bool Save()
        {
            if (BDCommon.Settings.IsUpdating) return false;

            bool result = false;
            if (null != parentId)
            {
                foreach (BDRegimenControl control in regimenControlList)
                {
                    result = control.Save() || result;
                }
                // If zero regimens are defined then this is a valid test
                if ((result && (null == currentRegimenGroup)) || (null == currentRegimenGroup) && (tbName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentRegimenGroup)
                {
                    if (currentRegimenGroup.name != tbName.Text) currentRegimenGroup.name = tbName.Text;
                    if (null != DisplayOrder)
                    {
                        if (currentRegimenGroup.displayOrder != DisplayOrder) currentRegimenGroup.displayOrder = DisplayOrder;
                    }

                    if (andRadioButton.Checked)
                    {
                        if (currentRegimenGroup.regimenGroupJoinType != (int)BDRegimenGroup.RegimenGroupJoinType.AndWithNext)
                            currentRegimenGroup.regimenGroupJoinType = (int)BDRegimenGroup.RegimenGroupJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if (currentRegimenGroup.regimenGroupJoinType != (int)BDRegimenGroup.RegimenGroupJoinType.OrWithNext)
                            currentRegimenGroup.regimenGroupJoinType = (int)BDRegimenGroup.RegimenGroupJoinType.OrWithNext;
                    }
                    else
                    {
                        if (currentRegimenGroup.regimenGroupJoinType != (int)BDRegimenGroup.RegimenGroupJoinType.None)
                            currentRegimenGroup.regimenGroupJoinType = (int)BDRegimenGroup.RegimenGroupJoinType.None;
                    }

                    if (cbRegimenOfChoice.Checked != currentRegimenGroup.regimenOfChoice)
                        currentRegimenGroup.regimenOfChoice = cbRegimenOfChoice.Checked;

                    if (cbAlternativeRegimen.Checked != currentRegimenGroup.alternativeRegimen)
                        currentRegimenGroup.alternativeRegimen = cbAlternativeRegimen.Checked;

                    BDRegimenGroup.Save(dataContext, currentRegimenGroup);
                    BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDRegimenGroup, BDRegimenGroup.PROPERTYNAME_NAME, currentRegimenGroup.name);

                    result = true;
                }
            }

            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentRegimenGroup)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentRegimenGroup = BDRegimenGroup.CreateBDRegimenGroup(dataContext, this.parentId.Value);
                    currentRegimenGroup.SetParent(parentType, parentId);
                    currentRegimenGroup.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentRegimenGroup.LayoutVariant = DefaultLayoutVariantType;
                }
            }

            return result;
        }

        #endregion

        private void CreateLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();
                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentRegimenGroup.Uuid, currentRegimenGroup.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private void toggleLinkButton()
        {
            bool enabled = (tbName.Text.Length > 0);
        }

        public override string ToString()
        {
            return (null == this.currentRegimenGroup) ? "No Regimen Group" : this.currentRegimenGroup.name;
        }

        private BDRegimenControl addRegimenControl(BDRegimen pRegimen, int pTabIndex)
        {
            BDRegimenControl regimenControl = null;

            if (CreateCurrentObject())
            {
                regimenControl = new BDRegimenControl();

                regimenControl.Dock = DockStyle.Top;
                regimenControl.TabIndex = pTabIndex;
                regimenControl.DisplayOrder = pTabIndex;
                regimenControl.AssignParentInfo(currentRegimenGroup.Uuid, currentRegimenGroup.NodeType);
                regimenControl.AssignDataContext(dataContext);
                regimenControl.AssignScopeId(scopeId);
                regimenControl.AssignTypeaheadSource(BDTypeahead.TherapyNames, BDRegimen.PROPERTYNAME_NAME);
                regimenControl.AssignTypeaheadSource(BDTypeahead.TherapyDosages, BDRegimen.PROPERTYNAME_DOSAGE);
                regimenControl.AssignTypeaheadSource(BDTypeahead.TherapyDurations, BDRegimen.PROPERTYNAME_DURATION);
                regimenControl.CurrentRegimen = pRegimen;
                regimenControl.DefaultLayoutVariantType = this.DefaultLayoutVariantType;
                regimenControl.RequestItemAdd += new EventHandler<NodeEventArgs>(Regimen_RequestItemAdd);
                regimenControl.RequestItemDelete += new EventHandler<NodeEventArgs>(Regimen_RequestItemDelete);
                regimenControl.ReorderToNext += new EventHandler<NodeEventArgs>(Regimen_ReorderToNext);
                regimenControl.ReorderToPrevious += new EventHandler<NodeEventArgs>(Regimen_ReorderToPrevious);
                regimenControl.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);

                regimenControlList.Add(regimenControl);

                panelRegimens.Controls.Add(regimenControl);
                regimenControl.BringToFront();

                regimenControl.RefreshLayout();
            }

            return regimenControl;
        }

        private void removeRegimenControl(BDRegimenControl pRegimenControl, bool pDeleteRecord)
        {
            panelRegimens.Controls.Remove(pRegimenControl);

            pRegimenControl.RequestItemAdd -= new EventHandler<NodeEventArgs>(Regimen_RequestItemAdd);
            pRegimenControl.RequestItemDelete -= new EventHandler<NodeEventArgs>(Regimen_RequestItemDelete);
            pRegimenControl.ReorderToNext -= new EventHandler<NodeEventArgs>(Regimen_ReorderToNext);
            pRegimenControl.ReorderToPrevious -= new EventHandler<NodeEventArgs>(Regimen_ReorderToPrevious);
            pRegimenControl.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);

            regimenControlList.Remove(pRegimenControl);

            if (pDeleteRecord)
            {
                BDRegimen entry = pRegimenControl.CurrentRegimen;
                if (null != entry)
                {
                    BDRegimen.Delete(dataContext, entry, pDeleteRecord);
                    for (int idx = 0; idx < regimenControlList.Count; idx++)
                    {
                        regimenControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pRegimenControl.Dispose();
            pRegimenControl = null;
        }

        private void reorderRegimenControl(BDRegimenControl pRegimenControl, int pOffset)
        {
            int currentPosition = regimenControlList.FindIndex(t => t == pRegimenControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < regimenControlList.Count))
                {
                    regimenControlList[requestedPosition].CreateCurrentObject();
                    regimenControlList[requestedPosition].DisplayOrder = currentPosition;

                    regimenControlList[requestedPosition].CurrentRegimen.displayOrder = currentPosition;
                    BDRegimen.Save(dataContext, regimenControlList[requestedPosition].CurrentRegimen);


                    regimenControlList[currentPosition].CreateCurrentObject();
                    regimenControlList[currentPosition].DisplayOrder = requestedPosition;

                    regimenControlList[currentPosition].CurrentRegimen.displayOrder = requestedPosition;
                    BDRegimen.Save(dataContext, regimenControlList[currentPosition].CurrentRegimen);

                    BDRegimenControl temp = regimenControlList[requestedPosition];
                    regimenControlList[requestedPosition] = regimenControlList[currentPosition];
                    regimenControlList[currentPosition] = temp;

                    int zOrder = panelRegimens.Controls.GetChildIndex(pRegimenControl);
                    zOrder = zOrder + (pOffset * -1);
                    panelRegimens.Controls.SetChildIndex(pRegimenControl, zOrder);
                }
            }
        }

        private void RegimenGroup_RequestItemAdd(object sender, EventArgs e)
        {
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDRegimenGroup, DefaultLayoutVariantType));
        }

        private void RegimenGroup_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new NodeEventArgs(dataContext, CurrentRegimenGroup.Uuid));
        }

        private void Regimen_RequestItemAdd(object sender, EventArgs e)
        {
            if (CreateCurrentObject())
            {
                IBDNode node = BDFabrik.CreateChildNode(dataContext, this.currentRegimenGroup, BDConstants.BDNodeType.BDRegimen, this.currentRegimenGroup.LayoutVariant);
                BDRegimen regimen = node as BDRegimen;
                BDRegimenControl control = addRegimenControl(regimen, regimenControlList.Count);
                if (null != control)
                {
                    control.Focus();
                }
            }
        }

        private void Regimen_RequestItemDelete(object sender, EventArgs e)
        {
            BDRegimenControl control = sender as BDRegimenControl;
            if (null != control)
            {
                if (MessageBox.Show("Delete Regimen?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removeRegimenControl(control, true);
            }

        }

        private void Regimen_ReorderToNext(object sender, NodeEventArgs e)
        {
            BDRegimenControl control = sender as BDRegimenControl;
            if (null != control)
            {
                reorderRegimenControl(control, 1);
            }
        }

        private void Regimen_ReorderToPrevious(object sender, NodeEventArgs e)
        {
            BDRegimenControl control = sender as BDRegimenControl;
            if (null != control)
            {
                reorderRegimenControl(control, -1);
            }
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, CurrentRegimenGroup.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, CurrentRegimenGroup.Uuid));
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                CreateLink(control.Tag as string);
            }
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "±");
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "¹");
        }

        private void Menu_Copy(System.Object sender, System.EventArgs e)
        {
            // Ensure that text is selected in the text box.   
            if (tbName.SelectionLength > 0)
                // Copy the selected text to the Clipboard.
                tbName.Copy();
        }

        private void Menu_Cut(System.Object sender, System.EventArgs e)
        {
            // Ensure that text is currently selected in the text box.   
            if (tbName.SelectedText != "")
                // Cut the selected text in the control and paste it into the Clipboard.
                tbName.Cut();
        }

        private void Menu_Paste(System.Object sender, System.EventArgs e)
        {
            // Determine if there is any text in the Clipboard to paste into the text box.
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                // Paste current text in Clipboard into text box.
                tbName.Paste();
            }
        }

        private void Menu_Undo(System.Object sender, System.EventArgs e)
        {
            // Determine if last operation can be undone in text box.   
            if (tbName.CanUndo == true)
            {
                // Undo the last operation.
                tbName.Undo();
                // Clear the undo buffer to prevent last action from being redone.
                tbName.ClearUndo();
            }
        }

        private void Menu_Delete(System.Object sender, System.EventArgs e)
        {
            tbName.SelectedText = string.Empty;
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            editIndexStripMenuItem.Tag = new BDNodeWrapper(CurrentNode, CurrentNode.NodeType, CurrentNode.LayoutVariant, null);

            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void notesChanged_Action(object sender, NodeEventArgs e)
        {
            //ShowLinksInUse(true);
            OnNotesChanged(e);
        }

        private void tbName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                undoToolStripMenuItem.Enabled = tbName.CanUndo;
                pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
                cutToolStripMenuItem.Enabled = (tbName.SelectionLength > 0);
                copyToolStripMenuItem.Enabled = (tbName.SelectionLength > 0);
                deleteToolStripMenuItem1.Enabled = (tbName.SelectionLength > 0);
            }
        }

        private void BDRegimenGroupControl_Leave(object sender, EventArgs e)
        {
            Save();
        }


        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                return CurrentRegimenGroup;
            }
            set
            {
                CurrentRegimenGroup = value as BDRegimenGroup;
            }
        }

        public bool ShowAsChild { get; set; }

        private void BDRegimenGroupControl_Load(object sender, EventArgs e)
        {
            btnRegimenGroupLink.Tag = BDRegimenGroup.PROPERTYNAME_NAME;
            btnRegimenGroupConjunctionLink.Tag = BDRegimenGroup.PROPERTYNAME_CONJUNCTION;
            lblInfo.Visible = false;
#if DEBUG
            lblInfo.Visible = true;
#endif
        }

        private void editIndexStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    BDSearchEntryEditView indexEditView = new BDSearchEntryEditView();
                    indexEditView.AssignDataContext(dataContext);
                    indexEditView.AssignCurrentNode(nodeWrapper.Node);
                    string contextString = BDUtilities.BuildHierarchyString(dataContext, nodeWrapper.Node, " : ");
                    indexEditView.DisplayContext = contextString;
                    indexEditView.ShowDialog(this);

                    indexEditView.Dispose();
                }
            }
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            toggleLinkButton();
        }
    }
}
