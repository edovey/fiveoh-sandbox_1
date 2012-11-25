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
    public partial class BDCombinedEntryFieldControl : UserControl
    {
        public Entities DataContext;
        public BDCombinedEntry CurrentEntry;
        public string FieldName;
        public Guid? ScopeId;
        protected Guid parentId;
        protected BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;

        private bool isUpdating = false;
        private int? displayOrder;

        private string currentControlName;

        private const string TEXTFIELD_TITLE = "titleField";
        private const string TEXTFIELD_DETAIL = "detailField";

        public BDCombinedEntryFieldControl()
        {
            InitializeComponent();
        }

        public BDCombinedEntryFieldControl(Entities pDataContext, BDCombinedEntry pCombinedEntry, string pPropertyName, Guid? pScopeId)
        {
            InitializeComponent();

            Initialize(pDataContext, pCombinedEntry, pPropertyName, pScopeId);
        }

        public void Initialize(Entities pDataContext, BDCombinedEntry pCombinedEntry, string pPropertyName, Guid? pScopeId)
        {
            if (null == pCombinedEntry) throw new NotSupportedException("May not create a CombinedEntryField control without an existing entry");
            if (null == pCombinedEntry.ParentId) throw new NotSupportedException("May not create a CombinedEntryField control without a supplied parent");

            DataContext = pDataContext;
            CurrentEntry = pCombinedEntry;
            parentId = CurrentEntry.ParentId.Value;
            FieldName = pPropertyName;
            ScopeId = pScopeId;
        }

        private void BDCombinedEntryFieldControl_Load(object sender, EventArgs e)
        {
            //Populate();
        }

        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        protected virtual void OnNotesChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NotesChanged;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NameChanged;
            if (null != handler) { handler(this, e); }
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        public bool Save()
        {
            bool result = true;

            string originalDetailValue = detailValueFromField(CurrentEntry);
            string originalTitleValue = titleValueFromField(CurrentEntry);
            BDConstants.BDJoinType originalJoinType = joinValueFromField(CurrentEntry);

            string detailValue = txtEntryDetail.Text;
            string titleValue = txtEntryTitle.Text;
            BDConstants.BDJoinType joinType = gatherJoinType();

            switch (FieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    CurrentEntry.entryDetail01 = detailValue;
                    CurrentEntry.entryTitle01 = titleValue;
                    CurrentEntry.entryJoinType01 = (int)joinType;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    CurrentEntry.entryDetail02 = detailValue;
                    CurrentEntry.entryTitle02 = titleValue;
                    CurrentEntry.entryJoinType02 = (int)joinType;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    CurrentEntry.entryDetail03 = detailValue;
                    CurrentEntry.entryTitle03 = titleValue;
                    CurrentEntry.entryJoinType03 = (int)joinType;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    CurrentEntry.entryDetail04 = detailValue;
                    CurrentEntry.entryTitle04 = titleValue;
                    CurrentEntry.entryJoinType04 = (int)joinType;
                    break;
            }

            DataContext.SaveChanges();

            if (detailValue != originalDetailValue) OnNameChanged(new NodeEventArgs(DataContext, CurrentEntry.Uuid, detailValue, FieldName));

            return result;
        }

        public void Populate()
        {
            isUpdating = true;

            switch (FieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    btnLinkedNoteTitle.Tag = BDCombinedEntry.PROPERTYNAME_ENTRYTITLE01;
                    btnLinkedNoteDetail.Tag = BDCombinedEntry.PROPERTYNAME_ENTRY01;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    btnLinkedNoteTitle.Tag = BDCombinedEntry.PROPERTYNAME_ENTRYTITLE02;
                    btnLinkedNoteDetail.Tag = BDCombinedEntry.PROPERTYNAME_ENTRY02;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    btnLinkedNoteTitle.Tag = BDCombinedEntry.PROPERTYNAME_ENTRYTITLE03;
                    btnLinkedNoteDetail.Tag = BDCombinedEntry.PROPERTYNAME_ENTRY03;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    btnLinkedNoteTitle.Tag = BDCombinedEntry.PROPERTYNAME_ENTRYTITLE04;
                    btnLinkedNoteDetail.Tag = BDCombinedEntry.PROPERTYNAME_ENTRY04;
                    break;
            }

            BDLayoutMetadataColumn columnInfo = BDLayoutMetadataColumn.Retrieve(DataContext, CurrentEntry.LayoutVariant, CurrentEntry.NodeType, BDCombinedEntry.VIRTUALPROPERTYNAME_ENTRYTITLE);
            if (null != columnInfo)
            {
                lblTitle.Text = columnInfo.label;
            }
            columnInfo = BDLayoutMetadataColumn.Retrieve(DataContext, CurrentEntry.LayoutVariant, CurrentEntry.NodeType, BDCombinedEntry.VIRTUALPROPERTYNAME_ENTRYDETAIL);
            if (null != columnInfo)
            {
                lblDetail.Text = columnInfo.label;
            }

            txtEntryTitle.Text = titleValueFromField(CurrentEntry);
            txtEntryDetail.Text = detailValueFromField(CurrentEntry);
            switch (joinValueFromField(CurrentEntry))
            {
                case BDConstants.BDJoinType.Next:
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
                case BDConstants.BDJoinType.Other:
                    otherRadioButton.Checked = true;
                    break;
                default:
                    noneRadioButton.Checked = true;
                    break;
            }

            isUpdating = false;
        }

        private string detailValueFromField(BDCombinedEntry pEntry)
        {
            string value = null;
            switch (FieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    value = CurrentEntry.entryDetail01;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    value = CurrentEntry.entryDetail02;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    value = CurrentEntry.entryDetail03;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    value = CurrentEntry.entryDetail04;
                    break;
            }
            return value;
        }

        private string titleValueFromField(BDCombinedEntry pEntry)
        {
            string value = null;

            switch (FieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    value = CurrentEntry.entryTitle01;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    value = CurrentEntry.entryTitle02;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    value = CurrentEntry.entryTitle03;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    value = CurrentEntry.entryTitle04;
                    break;
            }
            return value;
        }
        
        private BDConstants.BDJoinType joinValueFromField(BDCombinedEntry pEntry)
        {
            BDConstants.BDJoinType joinType = BDConstants.BDJoinType.Next;

            switch (FieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    joinType = pEntry.JoinType01;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    joinType = pEntry.JoinType02;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    joinType = pEntry.JoinType03;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    joinType = pEntry.JoinType04;
                    break;
            }
            return joinType;
        }

        private BDConstants.BDJoinType gatherJoinType()
        {
            BDConstants.BDJoinType joinType = BDConstants.BDJoinType.Next;

            if (andRadioButton.Checked) joinType = BDConstants.BDJoinType.AndWithNext;
            else if (orRadioButton.Checked) joinType = BDConstants.BDJoinType.OrWithNext;
            else if (thenRadioButton.Checked) joinType = BDConstants.BDJoinType.ThenWithNext;
            else if (andOrRadioButton.Checked) joinType = BDConstants.BDJoinType.WithOrWithoutWithNext;
            else if (otherRadioButton.Checked) joinType = BDConstants.BDJoinType.Other;
            return joinType;
        }

        private void createLink(string pProperty)
        {
            Save();

            BDLinkedNoteView view = new BDLinkedNoteView();
            view.AssignDataContext(DataContext);
            view.AssignContextPropertyName(pProperty);
            view.AssignParentInfo(CurrentEntry.Uuid, CurrentEntry.NodeType);
            view.AssignScopeId(ScopeId);
            view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
            view.ShowDialog(this);
            view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
            ShowLinksInUse(false);

        }
        public void ShowLinksInUse()
        {
            ShowLinksInUse(true);
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(DataContext, (null != this.CurrentEntry) ? this.CurrentEntry.uuid : Guid.Empty);
            btnLinkedNoteDetail.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNoteDetail.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnLinkedNoteTitle.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNoteTitle.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }     

        private void txtField_Leave(object sender, EventArgs e)
        {
            if(!isUpdating) Save();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!isUpdating) Save();
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

        private void tbDetail_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = TEXTFIELD_DETAIL;
        }

        private void tbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = TEXTFIELD_TITLE;
        }

        private void insertTextFromMenu(string textToInsert)
        {
            if (currentControlName == TEXTFIELD_DETAIL)
            {
                int position = txtEntryDetail.SelectionStart;
                txtEntryDetail.Text = txtEntryDetail.Text.Insert(txtEntryDetail.SelectionStart, textToInsert);
                txtEntryDetail.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == TEXTFIELD_TITLE)
            {
                int position = txtEntryTitle.SelectionStart;
                txtEntryTitle.Text = txtEntryTitle.Text.Insert(txtEntryTitle.SelectionStart, textToInsert);
                txtEntryTitle.SelectionStart = textToInsert.Length + position;
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
