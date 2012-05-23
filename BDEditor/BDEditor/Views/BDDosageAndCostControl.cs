using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using System.Diagnostics;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDDosageAndCostControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDDosage currentDosage;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private string currentControlName;

        private const string COST_TEXTBOX = "Cost";
        private const string DOSAGE_TEXTBOX = "Dosage";
        private const string NAME_TEXTBUX = "Name";

        public int? DisplayOrder { get; set; }

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

        public BDDosage CurrentDosage
        {
            get { return currentDosage; }
            set { currentDosage = value; }
        }

        public BDDosageAndCostControl()
        {
            InitializeComponent();
            rtbName.Tag = btnNameLink;
            rtbDosage.Tag = btnDosageLink;
            rtbCost.Tag = btnCostLink;

            btnDosageLink.Tag = BDDosage.PROPERTYNAME_DOSAGE;
            btnCostLink.Tag = BDDosage.PROPERTYNAME_COST;
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentDosage) ? this.currentDosage.uuid : Guid.Empty);
            btnNameLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnNameLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR; 
            btnDosageLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosageLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnCostLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnCostLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource, string pProperty)
        {
            throw new NotSupportedException();
        }

        #region IBDControl

        public void RefreshLayout()
        {
            RefreshLayout(true);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            ControlHelper.SuspendDrawing(this);
            if (currentDosage == null)
            {
                rtbName.Text = @"";
                rtbDosage.Text = @"";
                rtbCost.Text = @"";
                noneRadioButton.Checked = true;
            }
            else
            {
                this.BackColor = SystemColors.Control;

                rtbName.Text = currentDosage.name;
                rtbDosage.Text = currentDosage.dosage;
                rtbCost.Text = currentDosage.cost;
                DisplayOrder = currentDosage.displayOrder;

                switch ((BDDosage.DosageJoinType)currentDosage.joinType)
                {
                    case BDDosage.DosageJoinType.None:
                        noneRadioButton.Checked = true;
                        break;
                    case BDDosage.DosageJoinType.AndWithNext:
                        andRadioButton.Checked = true;
                        break;
                    case BDDosage.DosageJoinType.OrWithNext:
                        orRadioButton.Checked = true;
                        break;
                    case BDDosage.DosageJoinType.ThenWithNext:
                        thenRadioButton.Checked = true;
                        break;
                    case BDDosage.DosageJoinType.WithOrWithoutWithNext:
                        andOrRadioButton.Checked = true;
                        break;
                    default:
                        noneRadioButton.Checked = true;
                        break;
                }
            }
            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentDosage)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentDosage = BDDosage.CreateBDDosage(this.dataContext, this.parentId.Value);
                    currentDosage.SetParent(parentType, parentId);
                    currentDosage.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentDosage.LayoutVariant = DefaultLayoutVariantType;
                }
            }

            return result;
        }

        public bool Save()
        {
            bool result = false;
            if (null != parentId)
            {
                if ((null == currentDosage) &&
                    (rtbName.Text != string.Empty) ||
                    (rtbDosage.Text != string.Empty) ||
                    (rtbCost.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentDosage)
                {
                    if (currentDosage.Name != rtbName.Text) currentDosage.Name = rtbName.Text;
                    if (currentDosage.dosage != rtbDosage.Text) currentDosage.dosage = rtbDosage.Text;
                    if (currentDosage.cost != rtbCost.Text) currentDosage.cost = rtbCost.Text;
                    if (currentDosage.displayOrder != DisplayOrder) currentDosage.displayOrder = DisplayOrder;

                    if (andRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDDosage.DosageJoinType.AndWithNext)
                            currentDosage.joinType = (int)BDDosage.DosageJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDDosage.DosageJoinType.OrWithNext)
                            currentDosage.joinType = (int)BDDosage.DosageJoinType.OrWithNext;
                    }
                    else if (thenRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDDosage.DosageJoinType.ThenWithNext)
                            currentDosage.joinType = (int)BDDosage.DosageJoinType.ThenWithNext;
                    }
                    else if (andOrRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDDosage.DosageJoinType.WithOrWithoutWithNext)
                            currentDosage.joinType = (int)BDDosage.DosageJoinType.WithOrWithoutWithNext;
                    }
                    else
                    {
                        if (currentDosage.joinType != (int)BDDosage.DosageJoinType.None)
                            currentDosage.joinType = (int)BDDosage.DosageJoinType.None;
                    }

                    BDDosage.Save(dataContext, currentDosage);
                    result = true;
                }
            }

            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        #endregion

        private void createLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();

                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentDosage.Uuid, currentDosage.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private void insertTextFromMenu(string textToInsert)
        {
            if (currentControlName == NAME_TEXTBUX)
            {
                int position = rtbName.SelectionStart;
                rtbName.Text = rtbName.Text.Insert(rtbName.SelectionStart, textToInsert);
                rtbName.SelectionStart = textToInsert.Length + position;
            }
            if (currentControlName == COST_TEXTBOX)
            {
                int position = rtbCost.SelectionStart;
                rtbCost.Text = rtbCost.Text.Insert(rtbCost.SelectionStart, textToInsert);
                rtbCost.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DOSAGE_TEXTBOX)
            {
                int position = rtbDosage.SelectionStart;
                rtbDosage.Text = rtbDosage.Text.Insert(rtbDosage.SelectionStart, textToInsert);
                rtbDosage.SelectionStart = textToInsert.Length + position;
            }
        }

        private void toggleLinkButtonEnablement()
        {
            bool enabled = ( (rtbName.Text.Length > 0) || (rtbDosage.Text.Length > 0) || (rtbCost.Text.Length > 0) );

            btnNameLink.Enabled = enabled;
            btnDosageLink.Enabled = enabled;
            btnCostLink.Enabled = enabled;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            toggleLinkButtonEnablement();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            toggleLinkButtonEnablement();
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                string tag = control.Tag as string;
                createLink(tag);
            }
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "ß";
            insertTextFromMenu(newText);
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "°";
            insertTextFromMenu(newText);
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "µ";

            insertTextFromMenu(newText);
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≥";
            insertTextFromMenu(newText);
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≤";
            insertTextFromMenu(newText);
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "±";
            insertTextFromMenu(newText);
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "¹";
            insertTextFromMenu(newText);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDDosage, DefaultLayoutVariantType));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new NodeEventArgs(dataContext, currentDosage.Uuid));
        }

        public override string ToString()
        {
            return (null == this.currentDosage) ? "No Therapy" : this.currentDosage.name;
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, currentDosage.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, currentDosage.Uuid));
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        private void rtbDosage_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_TEXTBOX;
        }

        private void rtbName_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = NAME_TEXTBUX;
        }

        private void rtbCost_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = COST_TEXTBOX;
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                return CurrentDosage;
            }
            set
            {
                CurrentDosage = value as BDDosage;
            }
        }

        public bool ShowAsChild { get; set; }

        private void BDDosageControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void BDDosageControl_Load(object sender, EventArgs e)
        {
            rtbName.SelectAll();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbName.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbName.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbName.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbName.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = rtbName.SelectionStart;
            rtbName.Text = rtbName.Text.Substring(0, i) + rtbName.Text.Substring(i + rtbName.SelectionLength);
            rtbName.SelectionStart = i;
            rtbName.SelectionLength = 0;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbName.SelectAll();
        }

        private void contextMenuStripTextBox_Opening(object sender, CancelEventArgs e)
        {
            undoToolStripMenuItem.Enabled = rtbName.CanUndo;
            pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
            cutToolStripMenuItem.Enabled = (rtbName.SelectionLength > 0);
            copyToolStripMenuItem.Enabled = (rtbName.SelectionLength > 0);
            deleteToolStripMenuItem.Enabled = (rtbName.SelectionLength > 0);
        }
    }
}
