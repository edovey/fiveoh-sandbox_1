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
        protected Entities dataContext;
        protected BDCombinedEntry currentEntry;
        private string fieldName;
        protected Guid? scopeId;
        protected Guid parentId;
        protected BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;

        private bool isUpdating = false;
        private int? displayOrder;

        public BDCombinedEntryFieldControl()
        {
            InitializeComponent();
        }

        public BDCombinedEntryFieldControl(Entities pDataContext, BDCombinedEntry pCombinedEntry, string pPropertyName, Guid? pScopeId)
        {
            InitializeComponent();

            if (null == pCombinedEntry) throw new NotSupportedException("May not create a CombinedEntryField control without an existing entry");
            if (null == pCombinedEntry.ParentId) throw new NotSupportedException("May not create a CombinedEntryField control without a supplied parent");

            dataContext = pDataContext;
            currentEntry = pCombinedEntry;
            parentId = currentEntry.ParentId.Value;
            fieldName = pPropertyName;
            scopeId = pScopeId;
        }

        private void BDCombinedEntryFieldControl_Load(object sender, EventArgs e)
        {
            switch (fieldName)
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

            Populate();
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

            string originalDetailValue = detailValueFromField(currentEntry);
            string originalTitleValue = titleValueFromField(currentEntry);
            BDConstants.BDJoinType originalJoinType = joinValueFromField(currentEntry);

            string detailValue = txtEntryDetail.Text;
            string titleValue = txtEntryTitle.Text;
            BDConstants.BDJoinType joinType = gatherJoinType();

            switch (fieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    currentEntry.entryDetail01 = detailValue;
                    currentEntry.entryTitle01 = titleValue;
                    currentEntry.entryJoinType01 = (int)joinType;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    currentEntry.entryDetail02 = detailValue;
                    currentEntry.entryTitle02 = titleValue;
                    currentEntry.entryJoinType02 = (int)joinType;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    currentEntry.entryDetail03 = detailValue;
                    currentEntry.entryTitle03 = titleValue;
                    currentEntry.entryJoinType03 = (int)joinType;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    currentEntry.entryDetail04 = detailValue;
                    currentEntry.entryTitle04 = titleValue;
                    currentEntry.entryJoinType04 = (int)joinType;
                    break;
            }

            dataContext.SaveChanges();

            if (detailValue != originalDetailValue) OnNameChanged(new NodeEventArgs(dataContext, currentEntry.Uuid, detailValue, fieldName));

            return result;
        }

        public void Populate()
        {
            isUpdating = true;

            txtEntryTitle.Text = titleValueFromField(currentEntry);
            txtEntryDetail.Text = detailValueFromField(currentEntry);
            switch (joinValueFromField(currentEntry))
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

            isUpdating = false;
        }

        private string detailValueFromField(BDCombinedEntry pEntry)
        {
            string value = null;
            switch (fieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    value = currentEntry.entryDetail01;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    value = currentEntry.entryDetail02;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    value = currentEntry.entryDetail03;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    value = currentEntry.entryDetail04;
                    break;
            }
            return value;
        }

        private string titleValueFromField(BDCombinedEntry pEntry)
        {
            string value = null;

            switch (fieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    value = currentEntry.entryTitle01;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    value = currentEntry.entryTitle02;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    value = currentEntry.entryTitle03;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    value = currentEntry.entryTitle04;
                    break;
            }
            return value;
        }
        
        private BDConstants.BDJoinType joinValueFromField(BDCombinedEntry pEntry)
        {
            BDConstants.BDJoinType joinType = BDConstants.BDJoinType.None;

            switch (fieldName)
            {
                case BDCombinedEntry.PROPERTYNAME_ENTRY01:
                    joinType = (BDConstants.BDJoinType)currentEntry.entryJoinType01;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY02:
                    joinType = (BDConstants.BDJoinType)currentEntry.entryJoinType02;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY03:
                    joinType = (BDConstants.BDJoinType)currentEntry.entryJoinType03;
                    break;
                case BDCombinedEntry.PROPERTYNAME_ENTRY04:
                    joinType = (BDConstants.BDJoinType)currentEntry.entryJoinType04;
                    break;
            }
            return joinType;
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

        private void createLink(string pProperty)
        {
            Save();

            BDLinkedNoteView view = new BDLinkedNoteView();
            view.AssignDataContext(dataContext);
            view.AssignContextPropertyName(pProperty);
            view.AssignParentInfo(currentEntry.Uuid, currentEntry.NodeType);
            view.AssignScopeId(scopeId);
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
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentEntry) ? this.currentEntry.uuid : Guid.Empty);
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
    }
}
