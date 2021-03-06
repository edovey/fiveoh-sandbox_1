﻿using System;
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
    public partial class BDTableCellControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private IBDNode currentNode;
        private Guid? parentId;
        private Guid? scopeId;
        private BDConstants.BDNodeType parentType;
        private BDConstants.TableCellAlignment alignment;

        public string RichText
        {
            get { return tbValue.Text; }
            set { tbValue.Text = value; }
        }

        public int? DisplayOrder { get; set; }

        private bool showChildren = true;
        public bool ShowChildren
        {
            get { return showChildren; }
            set { showChildren = value; }
        }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType;

        public event EventHandler<NodeEventArgs> RequestItemAdd;
        public event EventHandler<NodeEventArgs> RequestItemDelete;

        public event EventHandler<NodeEventArgs> ReorderToPrevious;
        public event EventHandler<NodeEventArgs> ReorderToNext;

        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            throw new NotImplementedException();
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

        public BDTableCell CurrentTableCell
        {
            get { return currentNode as BDTableCell; }
            set { currentNode = value; }
        }

        public IBDNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public BDTableCellControl()
        {
            InitializeComponent();
        }

        public BDTableCellControl(Entities pDataContext, BDTableCell pCell)
        {
            dataContext = pDataContext;
            currentNode = pCell;
            parentId = pCell.parentId;
            alignment = (BDConstants.TableCellAlignment) pCell.alignment;
            InitializeComponent();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentNode) ? this.currentNode.Uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }     
        
        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }


        #region IBDControl
        public void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            Boolean origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);

            tbValue.Text = CurrentTableCell.value;

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

            if (null == this.currentNode)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentNode = BDTableCell.CreateBDTableCell(this.dataContext);
                    currentNode = BDFabrik.CreateNode(dataContext, DefaultNodeType, parentId, parentType);
                    currentNode.DisplayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentNode.LayoutVariant = DefaultLayoutVariantType;
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
                if (result && (null == currentNode))
                    CreateCurrentObject();
                if (null != currentNode)
                {
                    BDTableCell currentTableCell  = currentNode as BDTableCell;
                    currentTableCell.value = tbValue.Text;
                    BDTableCell.Save(dataContext, currentTableCell);
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
            //parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        #endregion

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, CurrentTableCell.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, CurrentTableCell.Uuid));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDTherapy, DefaultLayoutVariantType));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Guid? uuid = null;
            if (null != this.currentNode) uuid = this.currentNode.Uuid;

            OnItemDeleteRequested(new NodeEventArgs(dataContext, uuid));
        }

        public override string ToString()
        {
            return (null == this.currentNode) ? "No Cell" : this.currentNode.Uuid.ToString();
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        BDConstants.LayoutVariantType IBDControl.DefaultLayoutVariantType { get; set; }

        public bool ShowAsChild { get; set; }

        private void BDTableCellControl_Leave(object sender, EventArgs e)
        {
            Save(); 
        }

        private void btnLinkedNote_Click(object sender, EventArgs e)
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
            insertText(tbValue, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbValue, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbValue, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbValue, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbValue, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbValue, "±");
        }

        private void checkmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbValue, "√");
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbValue.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbValue.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbValue.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbValue.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = tbValue.SelectionStart;
            tbValue.Text = tbValue.Text.Substring(0, i) + tbValue.Text.Substring(i + tbValue.SelectionLength);
            tbValue.SelectionStart = i;
            tbValue.SelectionLength = 0;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbValue.SelectionStart = 0;
            tbValue.SelectionLength = tbValue.Text.Length;
            tbValue.Focus();
        }

        private void contextMenuStripTextBox_Opening(object sender, CancelEventArgs e)
        {
            undoToolStripMenuItem.Enabled = tbValue.CanUndo;
            pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
            cutToolStripMenuItem.Enabled = (tbValue.SelectionLength > 0);
            copyToolStripMenuItem.Enabled = (tbValue.SelectionLength > 0);
            deleteToolStripMenuItem.Enabled = (tbValue.SelectionLength > 0);
        }

        private void BDTableCellControl_Load(object sender, EventArgs e)
        {
            btnLinkedNote.Tag = BDTableCell.PROPERTYNAME_CONTENTS;

            BDTableCell cell = currentNode as BDTableCell;
            if (cell != null)
                tbValue.Text = cell.value;
        }

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private void createLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();

                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentNode.Uuid, BDConstants.BDNodeType.None);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }
    }
}
