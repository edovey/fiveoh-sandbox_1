using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDTherapyGroupControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private BDTherapyGroup currentTherapyGroup;
        public BDConstants.LayoutVariantType DefaultLayoutVariantType;
        private IBDControl parentControl;
        private Guid? scopeId;
        public int? DisplayOrder { get; set; }

        private List<BDTherapyControl> therapyControlList = new List<BDTherapyControl>();

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;

        public event EventHandler ReorderToPrevious;
        public event EventHandler ReorderToNext;

        public event EventHandler NotesChanged;

        protected virtual void OnNotesChanged(EventArgs e)
        {
            if (null != NotesChanged) { NotesChanged(this, e); }
        }

        protected virtual void OnItemAddRequested(EventArgs e)
        {
            if (null != RequestItemAdd) { RequestItemAdd(this, e); }
        }

        protected virtual void OnItemDeleteRequested(EventArgs e)
        {
            if (null != RequestItemDelete) { RequestItemDelete(this, e); }
        }

        protected virtual void OnReorderToPrevious(EventArgs e)
        {
            if (null != ReorderToPrevious) { ReorderToPrevious(this, e); }
        }

        protected virtual void OnReorderToNext(EventArgs e)
        {
            if (null != ReorderToNext) { ReorderToNext(this, e); }
        }

        public BDTherapyGroupControl()
        {
            InitializeComponent();

            btnTherapyGroupLink.Tag = BDTherapyGroup.PROPERTYNAME_NAME;
        }

        public BDTherapyGroup CurrentTherapyGroup
        {
            get { return currentTherapyGroup; }
            set { currentTherapyGroup = value; }
        }

        public void RefreshLayout()
        {
            this.SuspendLayout();

            // This is generic for Constants.LayoutVariantType.TreatmentRecommendation01

            for (int idx = 0; idx < therapyControlList.Count; idx++)
            {
                BDTherapyControl control = therapyControlList[idx];
                removeTherapyControl(control, false);
            }

            therapyControlList.Clear();
            panelTherapies.Controls.Clear();

            if (null == currentTherapyGroup)
            {
                tbName.Text = @"";
                noneRadioButton.Checked = true;
            }
            else
            {
                tbName.Text = currentTherapyGroup.name;
                switch ((BDTherapyGroup.TherapyGroupJoinType)currentTherapyGroup.therapyGroupJoinType)
                {
                    case BDTherapyGroup.TherapyGroupJoinType.None:
                        noneRadioButton.Checked = true;
                        break;
                    case BDTherapyGroup.TherapyGroupJoinType.AndWithNext:
                        andRadioButton.Checked = true;
                        break;
                    case BDTherapyGroup.TherapyGroupJoinType.OrWithNext:
                        orRadioButton.Checked = true;
                        break;
                    default:
                        noneRadioButton.Checked = true;
                        break;
                }

                List<BDTherapy> list = BDTherapy.GetTherapiesForTherapyParentId(dataContext, currentTherapyGroup.uuid);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    BDTherapy entry = list[idx];
                    addTherapyControl(entry, idx);
                }
            }

            ShowLinksInUse(false);

            this.ResumeLayout();
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
            bool result = false;
            if (null != parentId)
            {
                foreach (BDTherapyControl control in therapyControlList)
                {
                    result = control.Save() || result;
                }
                // If zero therapies are defined then this is a valid test
                if ((result && (null == currentTherapyGroup)) || (null == currentTherapyGroup) && (tbName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentTherapyGroup)
                {
                    if(currentTherapyGroup.name != tbName.Text) currentTherapyGroup.name = tbName.Text;
                    if (currentTherapyGroup.displayOrder != DisplayOrder) currentTherapyGroup.displayOrder = DisplayOrder;

                    if (andRadioButton.Checked)
                    {
                        if(currentTherapyGroup.therapyGroupJoinType != (int)BDTherapyGroup.TherapyGroupJoinType.AndWithNext) 
                            currentTherapyGroup.therapyGroupJoinType = (int)BDTherapyGroup.TherapyGroupJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if(currentTherapyGroup.therapyGroupJoinType != (int)BDTherapyGroup.TherapyGroupJoinType.OrWithNext) 
                            currentTherapyGroup.therapyGroupJoinType = (int)BDTherapyGroup.TherapyGroupJoinType.OrWithNext;
                    }
                    else
                    {
                        if(currentTherapyGroup.therapyGroupJoinType != (int)BDTherapyGroup.TherapyGroupJoinType.None) 
                            currentTherapyGroup.therapyGroupJoinType = (int)BDTherapyGroup.TherapyGroupJoinType.None;
                    }
                    
                    BDTherapyGroup.Save(dataContext, currentTherapyGroup);
                    Typeahead.AddToCollection(BDConstants.BDNodeType.BDTherapyGroup, BDTherapyGroup.PROPERTYNAME_NAME, currentTherapyGroup.name);

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

            if (null == this.currentTherapyGroup)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentTherapyGroup = BDTherapyGroup.CreateTherapyGroup(dataContext, this.parentId.Value);
                    currentTherapyGroup.SetParent(parentType, parentId);
                    currentTherapyGroup.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentTherapyGroup.LayoutVariant = DefaultLayoutVariantType;
                }
            }

            return result;
        }

        #endregion

        private BDTherapyControl addTherapyControl(BDTherapy pTherapy, int pTabIndex)
        {
            BDTherapyControl therapyControl = null;

            if (CreateCurrentObject())
            {
                therapyControl = new BDTherapyControl();

                therapyControl.Dock = DockStyle.Top;
                therapyControl.TabIndex = pTabIndex;
                therapyControl.DisplayOrder = pTabIndex;
                therapyControl.AssignParentInfo(currentTherapyGroup.Uuid, currentTherapyGroup.NodeType);
                therapyControl.AssignDataContext(dataContext);
                therapyControl.AssignScopeId(scopeId);
                therapyControl.AssignTypeaheadSource(Typeahead.TherapyNames, BDTherapy.PROPERTYNAME_THERAPY);
                therapyControl.AssignTypeaheadSource(Typeahead.TherapyDosages, BDTherapy.PROPERTYNAME_DOSAGE);
                therapyControl.AssignTypeaheadSource(Typeahead.TherapyDurations, BDTherapy.PROPERTYNAME_DURATION);
                therapyControl.CurrentTherapy = pTherapy;
                therapyControl.DefaultLayoutVariantType = this.DefaultLayoutVariantType;
                therapyControl.RequestItemAdd += new EventHandler(Therapy_RequestItemAdd);
                therapyControl.RequestItemDelete += new EventHandler(Therapy_RequestItemDelete);
                therapyControl.ReorderToNext += new EventHandler(Therapy_ReorderToNext);
                therapyControl.ReorderToPrevious += new EventHandler(Therapy_ReorderToPrevious);
                therapyControl.NotesChanged += new EventHandler(notesChanged_Action);

                therapyControlList.Add(therapyControl);

                panelTherapies.Controls.Add(therapyControl);
                therapyControl.BringToFront();

                therapyControl.RefreshLayout();
            }

            return therapyControl;
        }

        private void removeTherapyControl(BDTherapyControl pTherapyControl, bool pDeleteRecord)
        {
            panelTherapies.Controls.Remove(pTherapyControl);

            pTherapyControl.RequestItemAdd -= new EventHandler(Therapy_RequestItemAdd);
            pTherapyControl.RequestItemDelete -= new EventHandler(Therapy_RequestItemDelete);
            pTherapyControl.ReorderToNext -= new EventHandler(Therapy_ReorderToNext);
            pTherapyControl.ReorderToPrevious -= new EventHandler(Therapy_ReorderToPrevious);
            pTherapyControl.NotesChanged -= new EventHandler(notesChanged_Action);
            
            therapyControlList.Remove(pTherapyControl);

            if (pDeleteRecord)
            {
                BDTherapy entry = pTherapyControl.CurrentTherapy;
                if (null != entry)
                {
                    BDTherapy.Delete(dataContext, entry, pDeleteRecord);
                    for (int idx = 0; idx < therapyControlList.Count; idx++)
                    {
                        therapyControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pTherapyControl.Dispose();
            pTherapyControl = null;
        }

        private void BDTherapyGroupControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                CreateLink(control.Tag as string);
            }
        }

        private void CreateLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();
                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentTherapyGroup.Uuid, currentTherapyGroup.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentTherapyGroup) ? this.currentTherapyGroup.uuid : Guid.Empty);
            btnTherapyGroupLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnTherapyGroupLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;

            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < therapyControlList.Count; idx++)
                {
                    therapyControlList[idx].ShowLinksInUse(true);
                }
            }
        }

        private void ReorderTherapyControl(BDTherapyControl pTherapyControl, int pOffset)
        {
            int currentPosition = therapyControlList.FindIndex(t => t == pTherapyControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < therapyControlList.Count))
                {
                    therapyControlList[requestedPosition].CreateCurrentObject();
                    therapyControlList[requestedPosition].DisplayOrder = currentPosition;

                    therapyControlList[requestedPosition].CurrentTherapy.displayOrder = currentPosition;
                    BDTherapy.Save(dataContext, therapyControlList[requestedPosition].CurrentTherapy);


                    therapyControlList[currentPosition].CreateCurrentObject();
                    therapyControlList[currentPosition].DisplayOrder = requestedPosition;

                    therapyControlList[currentPosition].CurrentTherapy.displayOrder = requestedPosition;
                    BDTherapy.Save(dataContext, therapyControlList[currentPosition].CurrentTherapy);

                    BDTherapyControl temp = therapyControlList[requestedPosition];
                    therapyControlList[requestedPosition] = therapyControlList[currentPosition];
                    therapyControlList[currentPosition] = temp;

                    int zOrder = panelTherapies.Controls.GetChildIndex(pTherapyControl);
                    zOrder = zOrder + (pOffset * -1);
                    panelTherapies.Controls.SetChildIndex(pTherapyControl, zOrder);
                }
            }
        }

        private void TherapyGroup_RequestItemAdd(object sender, EventArgs e)
        {
            OnItemAddRequested(new EventArgs());
        }

        private void TherapyGroup_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
        }

        private void Therapy_RequestItemAdd(object sender, EventArgs e)
        {
            BDTherapyControl control = addTherapyControl(null, therapyControlList.Count);
            if (null != control)
            {
                control.Focus();
            }
        }

        private void Therapy_RequestItemDelete(object sender, EventArgs e)
        {
            BDTherapyControl control = sender as BDTherapyControl;
            if (null != control)
            {
                if(MessageBox.Show("Delete Therapy?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removeTherapyControl(control, true);
            }
        }

        private void Therapy_ReorderToNext(object sender, EventArgs e)
        {
            BDTherapyControl control = sender as BDTherapyControl;
            if (null != control)
            {
                ReorderTherapyControl(control, 1);
            }
        }

        private void Therapy_ReorderToPrevious(object sender, EventArgs e)
        {
            BDTherapyControl control = sender as BDTherapyControl;
            if (null != control)
            {
                ReorderTherapyControl(control, -1);
            }
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new EventArgs());
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new EventArgs());
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

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        public override string ToString()
        {
            return (null == this.currentTherapyGroup) ? "No Therapy Group" : this.currentTherapyGroup.name;
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void notesChanged_Action(object sender, EventArgs e)
        {
            //ShowLinksInUse(true);
            OnNotesChanged(new EventArgs());
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
    }
}
