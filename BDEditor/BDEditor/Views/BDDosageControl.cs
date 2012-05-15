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
    public partial class BDDosageControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDDosage currentDosage;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private string currentControlName;

        private const string COST_TEXTBOX = "Cost";
        private const string DOSAGE_TEXTBOX = "Dosage";

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

        public BDDosageControl()
        {
            InitializeComponent();

            tbDosage.Tag = btnDosageLink;
            tbCost.Tag = btnCostLink;

            btnDosageLink.Tag = BDDosage.PROPERTYNAME_DOSAGE;
            btnCostLink.Tag = BDTherapy.PROPERTYNAME_DURATION;
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentDosage) ? this.currentDosage.uuid : Guid.Empty);
            btnDosageLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosageLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnCostLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnCostLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource, string pProperty)
        {
            if (pProperty == string.Empty || pProperty == BDDosage.PROPERTYNAME_DOSAGE)
            {
                tbDosage.AutoCompleteCustomSource = pSource;
                tbDosage.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDosage.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
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
                tbDosage.Text = @"";
                tbCost.Text = @"";
                noneRadioButton.Checked = true;
            }
            else
            {
                this.BackColor = SystemColors.Control;

                tbDosage.Text = currentDosage.dosage;
                tbCost.Text = currentDosage.cost;
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
                    (tbDosage.Text != string.Empty) ||
                    (tbCost.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentDosage)
                {
                    if (currentDosage.dosage != tbDosage.Text) currentDosage.dosage = tbDosage.Text;
                    if (currentDosage.cost != tbCost.Text) currentDosage.cost = tbCost.Text;
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

                    if (currentDosage.name.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, currentDosage.name);
                    if (currentDosage.dosage.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, currentDosage.dosage);
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
            if (currentControlName == COST_TEXTBOX)
            {
                int position = tbCost.SelectionStart;
                tbCost.Text = tbCost.Text.Insert(tbCost.SelectionStart, textToInsert);
                tbCost.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DOSAGE_TEXTBOX)
            {
                int position = tbDosage.SelectionStart;
                tbDosage.Text = tbDosage.Text.Insert(tbDosage.SelectionStart, textToInsert);
                tbDosage.SelectionStart = textToInsert.Length + position;
            }
        }

        private void toggleLinkButtonEnablement()
        {
            bool enabled = ( (tbDosage.Text.Length > 0) || (tbCost.Text.Length > 0) );

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

        private void BDTherapyControl_Leave(object sender, EventArgs e)
        {
            Save();
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
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDTherapy, DefaultLayoutVariantType));
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

        private void tbDosage_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_TEXTBOX;
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

        private void BDTherapyControl_Load(object sender, EventArgs e)
        {
            pnlMain.Refresh();
        }
    }
}