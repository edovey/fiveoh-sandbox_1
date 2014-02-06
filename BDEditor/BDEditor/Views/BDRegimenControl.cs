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
    public partial class BDRegimenControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDRegimen currentRegimen;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private bool displayLeftBracket;
        private bool displayRightBracket;
        private string currentControlName;

        private const string NAME_TEXTBOX = "Name";
        private const string DOSAGE_TEXTBOX = "Dosage";
        private const string DURATION_TEXTBOX = "Duration";

        public int? DisplayOrder { get; set; }

        public int? ColumnOrder { get; set; }

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

        public BDRegimen CurrentRegimen
        {
            get { return currentRegimen; }
            set { currentRegimen = value; }
        }

        public bool DisplayLeftBracket
        {
            get { return displayLeftBracket; }
            set { displayLeftBracket = value; }
        }

        public bool DisplayRightBracket
        {
            get { return displayRightBracket; }
            set { displayRightBracket = value; }
        }

        public BDRegimenControl()
        {
            InitializeComponent();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentRegimen) ? this.currentRegimen.uuid : Guid.Empty);
            btnNameLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnNameLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnDosageLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosageLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnDurationLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDurationLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource, string pProperty)
        {
            if (pProperty == string.Empty || pProperty == BDRegimen.PROPERTYNAME_NAME)
            {
                tbName.AutoCompleteCustomSource = pSource;
                tbName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            else if (pProperty == BDRegimen.PROPERTYNAME_DOSAGE)
            {
                tbDosage.AutoCompleteCustomSource = pSource;
                tbDosage.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDosage.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            else if (pProperty == BDRegimen.PROPERTYNAME_DURATION)
            {
                tbDuration.AutoCompleteCustomSource = pSource;
                tbDuration.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDuration.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }

        #region IBDControl

        public void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            bool origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);
            if (currentRegimen == null)
            {
                tbName.Text = @"";
                tbDosage.Text = @"";
                tbDuration.Text = @"";
                nextRegimenRadioButton.Checked = true;
                lblLeftBracket.ForeColor = SystemColors.ControlLight;
                lblRightBracket.ForeColor = SystemColors.ControlLight;
                chkPreviousName.Checked = false;
                chkPreviousDose.Checked = false;
                chkPreviousDuration.Checked = false;
            }
            else
            {
                this.BackColor = SystemColors.Control;

                tbName.Text = currentRegimen.name;
                tbDosage.Text = currentRegimen.dosage0;
                tbDuration.Text = currentRegimen.duration0;
                DisplayOrder = currentRegimen.displayOrder;
                ColumnOrder = currentRegimen.columnOrder;

                switch ((BDConstants.BDJoinType)currentRegimen.regimenJoinType)
                {
                    case BDConstants.BDJoinType.Next:
                        nextRegimenRadioButton.Checked = true;
                        break;
                    case BDConstants.BDJoinType.AndWithNext:
                        andRadioButton.Checked = true;
                        break;
                    case BDConstants.BDJoinType.OrWithNext:
                        orRadioButton.Checked = true;
                        break;
                    case BDConstants.BDJoinType.ThenWithNext:
                        thenRadioButton.Checked = true;
                        break;
                    case BDConstants.BDJoinType.WithOrWithoutWithNext:
                        andOrRadioButton.Checked = true;
                        break;
                    case BDConstants.BDJoinType.Other:
                        otherRadioButton.Checked = true;
                        break;
                    case BDConstants.BDJoinType.AndOr:
                        aoRadioButton.Checked = true;
                        break;
                    default:
                        nextRegimenRadioButton.Checked = true;
                        break;
                }

                displayLeftBracket = currentRegimen.leftBracket.Value;
                lblLeftBracket.ForeColor = (displayLeftBracket) ? SystemColors.ControlText : SystemColors.ControlLight;

                displayRightBracket = currentRegimen.rightBracket.Value;
                lblRightBracket.ForeColor = (displayRightBracket) ? SystemColors.ControlText : SystemColors.ControlLight;

                chkPreviousName.Checked = currentRegimen.nameSameAsPrevious.Value;
                chkPreviousDose.Checked = currentRegimen.dosage0SameAsPrevious.Value;
                chkPreviousDuration.Checked = currentRegimen.duration0SameAsPrevious.Value;
            }
            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);

            BDCommon.Settings.IsUpdating = origState;
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentRegimen)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentRegimen = BDRegimen.CreateBDRegimen(this.dataContext, this.parentId.Value);
                    currentRegimen.SetParent(parentType, parentId);
                    currentRegimen.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentRegimen.LayoutVariant = DefaultLayoutVariantType;
                }
            }

            return result;
        }

        public bool Save()
        {
            if (BDCommon.Settings.IsUpdating) return false;

            bool result = false;
            if (null != parentId)
            {
                if ((null == currentRegimen) &&
                    ((tbName.Text != string.Empty) ||
                    (tbDosage.Text != string.Empty) ||
                    (tbDuration.Text != string.Empty) ||
                    chkPreviousName.Checked ||
                    chkPreviousDose.Checked ||
                    chkPreviousDuration.Checked))
                {
                    CreateCurrentObject();
                }

                if (null != currentRegimen)
                {
                    if (currentRegimen.name != tbName.Text) currentRegimen.name = tbName.Text;
                    if (currentRegimen.dosage0 != tbDosage.Text) currentRegimen.dosage0 = tbDosage.Text;
                    if (currentRegimen.duration0 != tbDuration.Text) currentRegimen.duration0 = tbDuration.Text;
                    if (currentRegimen.displayOrder != DisplayOrder) currentRegimen.displayOrder = DisplayOrder;

                    if (rbColumnOrder_0.Checked)
                        currentRegimen.columnOrder = 0;
                    else if (rbColumnOrder_1.Checked)
                        currentRegimen.columnOrder = 1;

                    if (andRadioButton.Checked)
                    {
                        if (currentRegimen.regimenJoinType != (int)BDConstants.BDJoinType.AndWithNext)
                            currentRegimen.regimenJoinType = (int)BDConstants.BDJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if (currentRegimen.regimenJoinType != (int)BDConstants.BDJoinType.OrWithNext)
                            currentRegimen.regimenJoinType = (int)BDConstants.BDJoinType.OrWithNext;
                    }
                    else if (thenRadioButton.Checked)
                    {
                        if (currentRegimen.regimenJoinType != (int)BDConstants.BDJoinType.ThenWithNext)
                            currentRegimen.regimenJoinType = (int)BDConstants.BDJoinType.ThenWithNext;
                    }
                    else if (andOrRadioButton.Checked)
                    {
                        if (currentRegimen.regimenJoinType != (int)BDConstants.BDJoinType.WithOrWithoutWithNext)
                            currentRegimen.regimenJoinType = (int)BDConstants.BDJoinType.WithOrWithoutWithNext;
                    }
                    else if (otherRadioButton.Checked)
                    {
                        if (currentRegimen.regimenJoinType != (int)BDConstants.BDJoinType.Other)
                            currentRegimen.regimenJoinType = (int)BDConstants.BDJoinType.Other;
                    }
                    else if (aoRadioButton.Checked)
                    {
                        if (currentRegimen.regimenJoinType != (int)BDConstants.BDJoinType.AndOr)
                            currentRegimen.regimenJoinType = (int)BDConstants.BDJoinType.AndOr;
                    }
                    else
                    {
                        if (currentRegimen.regimenJoinType != (int)BDConstants.BDJoinType.Next)
                            currentRegimen.regimenJoinType = (int)BDConstants.BDJoinType.Next;
                    }

                    if (currentRegimen.leftBracket != this.displayLeftBracket) currentRegimen.leftBracket = this.displayLeftBracket;
                    if (currentRegimen.rightBracket != this.displayRightBracket) currentRegimen.rightBracket = this.displayRightBracket;

                    if (currentRegimen.nameSameAsPrevious != this.chkPreviousName.Checked) currentRegimen.nameSameAsPrevious = this.chkPreviousName.Checked;
                    if (currentRegimen.dosage0SameAsPrevious != this.chkPreviousDose.Checked) currentRegimen.dosage0SameAsPrevious = this.chkPreviousDose.Checked;
                    if (currentRegimen.duration0SameAsPrevious != this.chkPreviousDuration.Checked) currentRegimen.duration0SameAsPrevious = this.chkPreviousDuration.Checked;

                    BDRegimen.Save(dataContext, currentRegimen);
                    result = true;

                    if (currentRegimen.name.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDRegimen, BDRegimen.PROPERTYNAME_NAME, currentRegimen.name);
                    if (currentRegimen.dosage0.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDRegimen, BDRegimen.PROPERTYNAME_DOSAGE, currentRegimen.dosage0);
                    if (currentRegimen.duration0.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDRegimen, BDRegimen.PROPERTYNAME_DURATION, currentRegimen.duration0);
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
                view.AssignParentInfo(currentRegimen.Uuid, currentRegimen.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private void insertTextFromMenu(string textToInsert)
        {
            if (currentControlName == NAME_TEXTBOX)
            {
                int position = tbName.SelectionStart;
                tbName.Text = tbName.Text.Insert(tbName.SelectionStart, textToInsert);
                tbName.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DOSAGE_TEXTBOX)
            {
                int position = tbDosage.SelectionStart;
                tbDosage.Text = tbDosage.Text.Insert(tbDosage.SelectionStart, textToInsert);
                tbDosage.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DURATION_TEXTBOX)
            {
                int position = tbDuration.SelectionStart;
                tbDuration.Text = tbDuration.Text.Insert(tbDuration.SelectionStart, textToInsert);
                tbDuration.SelectionStart = textToInsert.Length + position;
            }
        }

        private void toggleLinkButtonEnablement()
        {
            bool enabled = ((chkPreviousDose.Checked || chkPreviousDuration.Checked || chkPreviousName.Checked) ||
                ((tbDosage.Text.Length > 0) || (tbDuration.Text.Length > 0) || (tbName.Text.Length > 0)));

            btnNameLink.Enabled = enabled;
            btnDosageLink.Enabled = enabled;
            btnDurationLink.Enabled = enabled;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            toggleLinkButtonEnablement();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            toggleLinkButtonEnablement();
        }

        private void BDRegimenControl_Leave(object sender, EventArgs e)
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

        private void lblLeftBracket_Click(object sender, EventArgs e)
        {
            this.Focus();
            this.displayLeftBracket = !this.displayLeftBracket;
            lblLeftBracket.ForeColor = (this.displayLeftBracket) ? SystemColors.ControlText : SystemColors.ControlLight;
        }

        private void lblRightBracket_Click(object sender, EventArgs e)
        {
            this.Focus();
            this.displayRightBracket = !this.displayRightBracket;
            lblRightBracket.ForeColor = (this.displayRightBracket) ? SystemColors.ControlText : SystemColors.ControlLight;
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

        private void trademarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "®";
            insertTextFromMenu(newText);

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDTherapy, DefaultLayoutVariantType));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Guid? uuid = null;
            if (null != this.currentRegimen) uuid = this.currentRegimen.Uuid;

            OnItemDeleteRequested(new NodeEventArgs(dataContext, uuid));
        }

        public override string ToString()
        {
            return (null == this.currentRegimen) ? "No Regimen" : this.currentRegimen.name;
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, currentRegimen.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, currentRegimen.Uuid));
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            editIndexStripMenuItem.Tag = new BDNodeWrapper(CurrentNode, CurrentNode.NodeType, CurrentNode.LayoutVariant, null);

            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        private void tbName_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = NAME_TEXTBOX;
        }

        private void tbDosage_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_TEXTBOX;
        }

        private void tbDuration_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DURATION_TEXTBOX;
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                return CurrentRegimen;
            }
            set
            {
                CurrentRegimen = value as BDRegimen;
            }
        }

        public bool ShowAsChild { get; set; }

        private void BDRegimenControl_Load(object sender, EventArgs e)
        {
            bool origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;


            tbName.Tag = btnNameLink;
            tbDosage.Tag = btnDosageLink;
            tbDuration.Tag = btnDurationLink;

            chkPreviousName.Tag = btnNameLink;
            chkPreviousDose.Tag = btnDosageLink;
            chkPreviousDuration.Tag = btnDurationLink;

            btnNameLink.Tag = BDRegimen.PROPERTYNAME_NAME;
            btnDosageLink.Tag = BDRegimen.PROPERTYNAME_DOSAGE;
            btnDurationLink.Tag = BDRegimen.PROPERTYNAME_DURATION;

            if (!BDFabrik.TherapyLayoutHasFirstDosage(DefaultLayoutVariantType))
            {
                pnlMain.Controls.Remove(tbDosage);
                pnlMain.Controls.Remove(chkPreviousDose);
                pnlMain.Controls.Remove(btnDosageLink);
            }
            if (!BDFabrik.TherapyLayoutHasFirstDuration(DefaultLayoutVariantType))
            {
                pnlMain.Controls.Remove(tbDuration);
                pnlMain.Controls.Remove(chkPreviousDuration);
                pnlMain.Controls.Remove(btnDurationLink);
            }
            pnlMain.Refresh();

            BDCommon.Settings.IsUpdating = origState;
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

        private void rbColumnOrder_0_CheckedChanged(object sender, EventArgs e)
        {
            if (rbColumnOrder_0.Checked)
                currentRegimen.columnOrder = 0;
        }

        private void rbColumnOrder_1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbColumnOrder_1.Checked)
                currentRegimen.columnOrder = 1;
        }
    }
}