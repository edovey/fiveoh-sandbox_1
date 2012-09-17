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
    public partial class BDConfiguredEntryFieldControl : UserControl
    {
        protected Entities dataContext;
        protected BDConfiguredEntry configuredEntry;
        protected string fieldName;
        protected Guid scopeId;
        protected bool isUpdating = false;

        public int DisplayOrder { get; set; } 

        public BDConfiguredEntryFieldControl()
        {
            InitializeComponent();
        }

        public BDConfiguredEntryFieldControl(Entities pDataContext, BDConfiguredEntry pConfiguredEntry, string pFieldName, Guid pScopeId)
        {
            InitializeComponent();
            dataContext = pDataContext;
            configuredEntry = pConfiguredEntry;
            fieldName = pFieldName;
            scopeId = pScopeId;
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

        private void BDConfiguredEntryFieldControl_Load(object sender, EventArgs e)
        {
            isUpdating = true;
            configureLabel();
            Populate();
            isUpdating = false;
        }

        private void createLink(string pProperty)
        {
            BDLinkedNoteView view = new BDLinkedNoteView();
            view.AssignDataContext(dataContext);
            view.AssignContextPropertyName(pProperty);
            view.AssignParentInfo(configuredEntry.Uuid, configuredEntry.NodeType);
            view.AssignScopeId(scopeId);
            view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
            view.ShowDialog(this);
            view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
            ShowLinksInUse(false);
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != configuredEntry) ? configuredEntry.Uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }

        public virtual void RefreshLayout()
        {
            isUpdating = true;
            Populate();
            isUpdating = false;
        }

        private void btnLinkedNote_Click(object sender, EventArgs e)
        {
            createLink(fieldName);
        }
        private void configureLabel()
        {
            if (null != configuredEntry)
            {
                BDLayoutMetadataColumn columnInfo = BDLayoutMetadataColumn.Retrieve(dataContext, configuredEntry.LayoutVariant, configuredEntry.NodeType, fieldName);
                if (null != columnInfo)
                {
                    lblFieldLabel.Text = columnInfo.label;

                    if (BDConfiguredEntry.PROPERTYNAME_NAME == fieldName)
                    {
                        lblFieldLabel.Font = new Font(lblFieldLabel.Font.FontFamily, 10);
                        lblFieldLabel.Font = new Font(lblFieldLabel.Font, FontStyle.Bold);
                        
                    }
                }
            }
        }

        public virtual void Populate()
        {
            txtFieldData.Text = valueFromField();
        }

        private string valueFromField()
        {
            string value = null;
            switch (fieldName)
            {
                case BDConfiguredEntry.PROPERTYNAME_FIELD01:
                    value = configuredEntry.field01;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD02:
                    value = configuredEntry.field02;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD03:
                    value = configuredEntry.field03;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD04:
                    value = configuredEntry.field04;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD05:
                    value = configuredEntry.field05;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD06:
                    value = configuredEntry.field06;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD07:
                    value = configuredEntry.field07;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD08:
                    value = configuredEntry.field08;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD09:
                    value = configuredEntry.field09;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD10:
                    value = configuredEntry.field10;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD11:
                    value = configuredEntry.field11;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD12:
                    value = configuredEntry.field12;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD13:
                    value = configuredEntry.field13;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD14:
                    value = configuredEntry.field14;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD15:
                    value = configuredEntry.field15;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_NAME:
                    value = configuredEntry.name;
                    break;
            }

            return value;
        }

        public virtual bool Save()
        {
            bool result = true; // always true because this is only saving a specific field within the instance
            string value = txtFieldData.Text;
            string orignalValue = valueFromField();

            switch (fieldName)
            {
                case BDConfiguredEntry.PROPERTYNAME_FIELD01:
                    configuredEntry.field01 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD02:
                    configuredEntry.field02 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD03:
                    configuredEntry.field03 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD04:
                    configuredEntry.field04 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD05:
                    configuredEntry.field05 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD06:
                    configuredEntry.field06 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD07:
                    configuredEntry.field07 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD08:
                    configuredEntry.field08 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD09:
                    configuredEntry.field09 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD10:
                    configuredEntry.field10 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD11:
                    configuredEntry.field11 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD12:
                    configuredEntry.field12 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD13:
                    configuredEntry.field13 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD14:
                    configuredEntry.field14 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_FIELD15:
                    configuredEntry.field15 = value;
                    break;
                case BDConfiguredEntry.PROPERTYNAME_NAME:
                    configuredEntry.name = value;
                    break;
            }
            dataContext.SaveChanges();

            if (value != orignalValue)
            {
                OnNameChanged(new NodeEventArgs(dataContext, configuredEntry.Uuid, value, fieldName));
            }
            return result;
        }

        private void txtFieldData_Leave(object sender, EventArgs e)
        {
            if (!isUpdating) { Save(); }
        }

        private void insertTextFromMenu(string textToInsert)
        {
            int position = txtFieldData.SelectionStart;
            txtFieldData.Text = txtFieldData.Text.Insert(txtFieldData.SelectionStart, textToInsert);
            txtFieldData.SelectionStart = textToInsert.Length + position;
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
