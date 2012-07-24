using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDCombinedEntryControl : UserControl, IBDControl
    {
        protected Entities dataContext;
        protected BDCombinedEntry currentEntry;
        protected Guid? scopeId;
        protected Guid parentId;
        protected BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;
        private bool isUpdating = false;

        protected List<BDCombinedEntryFieldControl> fieldControlList = new List<BDCombinedEntryFieldControl>();

        private BDConstants.BDNodeType defaultNodeType;
        private BDConstants.LayoutVariantType defaultLayoutVariantType;
        private int? displayOrder;
        private bool showAsChild = false;

        private string currentControlName;

        private const string TEXTFIELD_TITLE = "titleField";
        private const string TEXTFIELD_NAME = "nameField";

        public BDCombinedEntryControl()
        {
            InitializeComponent();
        }

        public BDCombinedEntryControl(Entities pDataContext, BDCombinedEntry pCombinedEntry, Guid? pScopeId)
        {
            InitializeComponent();

            if (null == pCombinedEntry) throw new NotSupportedException("May not create a CombinedEntry control without an existing entry");
            if (null == pCombinedEntry.ParentId) throw new NotSupportedException("May not create a CombinedEntry control without a supplied parent");

            dataContext = pDataContext;
            currentEntry = pCombinedEntry;
            parentId = currentEntry.ParentId.Value;
            scopeId = pScopeId;
        }

        #region OnEvent handlers
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
        #endregion

        public bool Gather(BDCombinedEntry pEntry)
        {
            bool result = false;

            if (null != pEntry)
            {
                pEntry.name = txtName.Text;
                pEntry.groupTitle = txtTitle.Text;
                pEntry.groupJoinType = (int)gatherJoinType();

                //bdCombinedEntryFieldControl1.CurrentEntry = pEntry;
                //bdCombinedEntryFieldControl2.CurrentEntry = pEntry;
                //bdCombinedEntryFieldControl3.CurrentEntry = pEntry;
                //bdCombinedEntryFieldControl4.CurrentEntry = pEntry;

                //bdCombinedEntryFieldControl1.Save();
                //bdCombinedEntryFieldControl2.Save();
                //bdCombinedEntryFieldControl3.Save();
                //bdCombinedEntryFieldControl4.Save();

                result = true;
            }
            return result;
        }

        private BDConstants.BDJoinType gatherJoinType()
        {
            BDConstants.BDJoinType joinType = BDConstants.BDJoinType.None;

            if (andRadioButton.Checked) joinType = BDConstants.BDJoinType.AndWithNext;
            else if (orRadioButton.Checked) joinType = BDConstants.BDJoinType.OrWithNext;
            else if (thenRadioButton.Checked) joinType = BDConstants.BDJoinType.ThenWithNext;
            else if (andOrRadioButton.Checked) joinType = BDConstants.BDJoinType.WithOrWithoutWithNext;

            return joinType;
        }

        private void txtField_Leave(object sender, EventArgs e)
        {
            if (!isUpdating) Save();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!isUpdating) Save();
        }

        #region IBDControl

        public event EventHandler<NodeEventArgs> RequestItemAdd;
        public event EventHandler<NodeEventArgs> RequestItemDelete;
        public event EventHandler<NodeEventArgs> ReorderToPrevious;
        public event EventHandler<NodeEventArgs> ReorderToNext;
        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            if (null == pParentId) throw new NotSupportedException("May not assign a null parentId to a CombinedEntryControl");
            parentId = pParentId.Value;
            parentType = pParentType;
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public bool Save()
        {
            bool result = false;

            if (Gather(currentEntry))
            {
                BDCombinedEntry.Save(dataContext, currentEntry);
                result = true;
            }

            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool CreateCurrentObject()
        {
            throw new NotImplementedException();
        }

        public void RefreshLayout()
        {
            RefreshLayout(true);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            isUpdating = true;

            ControlHelper.SuspendDrawing(this);

            txtTitle.Text = currentEntry.groupTitle;
            txtName.Text = currentEntry.Name;

            List<BDLayoutMetadataColumn> metaDataColumnList = BDLayoutMetadataColumn.RetrieveListForLayout(dataContext, currentEntry.LayoutVariant);

            lblVirtualColumnOne.Text = "";
            lblVirtualColumnTwo.Text = "";

            int columnNumber = 0;
            foreach (BDLayoutMetadataColumn columnDef in metaDataColumnList)
            {
                switch (columnNumber)
                {
                    case 0:
                        lblVirtualColumnOne.Text = columnDef.label;
                        break;
                    case 1:
                        lblVirtualColumnTwo.Text = columnDef.label;
                        break;
                }
                columnNumber++;
            }

            switch (currentEntry.GroupJoinType)
            {
                case BDConstants.BDJoinType.None:
                    noneRadioButton.Checked = true;
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
                default:
                    noneRadioButton.Checked = true;
                    break;
            }

            bdCombinedEntryFieldControl1.Initialize(dataContext, currentEntry, BDCombinedEntry.PROPERTYNAME_ENTRY01, scopeId);
            bdCombinedEntryFieldControl2.Initialize(dataContext, currentEntry, BDCombinedEntry.PROPERTYNAME_ENTRY02, scopeId);
            bdCombinedEntryFieldControl3.Initialize(dataContext, currentEntry, BDCombinedEntry.PROPERTYNAME_ENTRY03, scopeId);
            bdCombinedEntryFieldControl4.Initialize(dataContext, currentEntry, BDCombinedEntry.PROPERTYNAME_ENTRY04, scopeId);
            bdCombinedEntryFieldControl1.Populate();
            bdCombinedEntryFieldControl2.Populate();
            bdCombinedEntryFieldControl3.Populate();
            bdCombinedEntryFieldControl4.Populate();
            ShowLinksInUse(false);

            ControlHelper.ResumeDrawing(this);
            isUpdating = false;
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            bdCombinedEntryFieldControl1.ShowLinksInUse(pPropagateToChildren);
            bdCombinedEntryFieldControl2.ShowLinksInUse(pPropagateToChildren);
            bdCombinedEntryFieldControl3.ShowLinksInUse(pPropagateToChildren);
            bdCombinedEntryFieldControl4.ShowLinksInUse(pPropagateToChildren);
        }

        public BDConstants.BDNodeType DefaultNodeType
        {
            get { return defaultNodeType; }
            set { defaultNodeType = value; }
        }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType
        {
            get { return defaultLayoutVariantType; }
            set { defaultLayoutVariantType = value; }
        }

        public IBDNode CurrentNode
        {
            get { return currentEntry; }
            set { currentEntry = value as BDCombinedEntry; }
        }

        public int? DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }

        public bool ShowAsChild
        {
            get { return showAsChild; }
            set { showAsChild = value; }
        }

        #endregion

        private void BDCombinedEntryControl_Load(object sender, EventArgs e)
        {

        }

        private void BDCombinedEntryControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void tbName_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = TEXTFIELD_NAME;
        }

        private void tbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = TEXTFIELD_TITLE;
        }

        private void insertTextFromMenu(string textToInsert)
        {
            if (currentControlName == TEXTFIELD_NAME)
            {
                int position = txtName.SelectionStart;
                txtName.Text = txtName.Text.Insert(txtName.SelectionStart, textToInsert);
                txtName.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == TEXTFIELD_TITLE)
            {
                int position = txtTitle.SelectionStart;
                txtTitle.Text = txtTitle.Text.Insert(txtTitle.SelectionStart, textToInsert);
                txtTitle.SelectionStart = textToInsert.Length + position;
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

        private void trademarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "®";
            insertTextFromMenu(newText);
        }
    }
}
