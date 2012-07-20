﻿using System;
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
        private Entities dataContext;
        private BDConfiguredEntry configuredEntry;
        private string fieldName;
        private Guid scopeId;

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

        protected virtual void OnNotesChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NotesChanged;
            if (null != handler) { handler(this, e); }
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        private void BDConfiguredEntryFieldControl_Load(object sender, EventArgs e)
        {
            configureLabel();
            Populate();
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

        public void RefreshLayout()
        {
            Populate();
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
                }
            }
        }

        public void Populate()
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
            txtFieldData.Text = value;
        }

        public void Save()
        {
            string value = txtFieldData.Text;
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
        }

        private void txtFieldData_Leave(object sender, EventArgs e)
        {
            Save();
        }
    }
}
