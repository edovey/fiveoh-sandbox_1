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
    public partial class BDDosageControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDDosage currentDosage;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private string currentControlName;

        private const string ADULT_TEXTBOX = "Adult";
        private const string DOSAGE2_TEXTBOX = "< 50";
        private const string DOSAGE3_TEXTBOX = "10 - 50";
        private const string DOSAGE4_TEXTBOX = "< 10 (Anuric)";

        public int? DisplayOrder { get; set; }

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

        public BDDosage CurrentDosage
        {
            get { return currentDosage; }
            set { currentDosage = value; }
        }

        public BDDosageControl()
        {
            InitializeComponent();
        }

        public BDDosageControl(Entities pDataContext, IBDNode pNode)
        {
            dataContext = pDataContext;
            currentDosage = pNode as BDDosage;
            parentId = pNode.ParentId;
            DefaultNodeType = pNode.NodeType;
            DefaultLayoutVariantType = pNode.LayoutVariant;
            InitializeComponent();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentDosage) ? this.currentDosage.uuid : Guid.Empty);
            btnAdultLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnAdultLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR; 
            btnDosage2Link.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosage2Link.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnDosage3Link.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosage3Link.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnDosage4Link.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosage4Link.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
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
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            Boolean origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);
            if (currentDosage == null)
            {
                tbAdultDosage.Text = @"";
                tbDosage2.Text = @"";
                tbDosage3.Text = @"";
                tbDosage4.Text = @"";
            }
            else
            {
                this.BackColor = SystemColors.Control;

                tbAdultDosage.Text = currentDosage.dosage;
                tbDosage2.Text = currentDosage.dosage2;
                tbDosage3.Text = currentDosage.dosage3;
                tbDosage4.Text = currentDosage.dosage4;
                DisplayOrder = currentDosage.displayOrder;

                chkPreviousDose2.Checked = currentDosage.dosage2SameAsPrevious;
                chkPreviousDose3.Checked = currentDosage.dosage3SameAsPrevious;
                chkPreviousDose4.Checked = currentDosage.dosage4SameAsPrevious;
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
            if (BDCommon.Settings.IsUpdating) return false;

            bool result = false;
            if (null != parentId)
            {
                if ((null == currentDosage) &&
                    (tbAdultDosage.Text != string.Empty) ||
                    (tbDosage2.Text != string.Empty) ||
                    (tbDosage4.Text != string.Empty) ||
                    chkPreviousDose2.Checked ||
                    chkPreviousDose3.Checked ||
                    chkPreviousDose4.Checked )
                {
                    CreateCurrentObject();
                }

                if (null != currentDosage)
                {
                    if (currentDosage.dosage != tbAdultDosage.Text) currentDosage.dosage = tbAdultDosage.Text;
                    if (currentDosage.dosage2 != tbDosage2.Text) currentDosage.dosage2 = tbDosage2.Text;
                    if (currentDosage.dosage3 != tbDosage3.Text) currentDosage.dosage3 = tbDosage3.Text;
                    if (currentDosage.dosage4 != tbDosage4.Text) currentDosage.dosage4 = tbDosage4.Text;
                    if (currentDosage.displayOrder != DisplayOrder) currentDosage.displayOrder = DisplayOrder;

                    if (currentDosage.dosage2SameAsPrevious != this.chkPreviousDose2.Checked) currentDosage.dosage2SameAsPrevious = this.chkPreviousDose2.Checked;
                    if (currentDosage.dosage3SameAsPrevious != this.chkPreviousDose3.Checked) currentDosage.dosage3SameAsPrevious = this.chkPreviousDose3.Checked;
                    if (currentDosage.dosage4SameAsPrevious != this.chkPreviousDose4.Checked) currentDosage.dosage4SameAsPrevious = this.chkPreviousDose4.Checked;

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
            if (currentControlName == ADULT_TEXTBOX)
            {
                int position = tbAdultDosage.SelectionStart;
                tbAdultDosage.Text = tbAdultDosage.Text.Insert(tbAdultDosage.SelectionStart, textToInsert);
                tbAdultDosage.SelectionStart = textToInsert.Length + position;
            }
            if (currentControlName == DOSAGE4_TEXTBOX)
            {
                int position = tbDosage4.SelectionStart;
                tbDosage4.Text = tbDosage4.Text.Insert(tbDosage4.SelectionStart, textToInsert);
                tbDosage4.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DOSAGE2_TEXTBOX)
            {
                int position = tbDosage2.SelectionStart;
                tbDosage2.Text = tbDosage2.Text.Insert(tbDosage2.SelectionStart, textToInsert);
                tbDosage2.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DOSAGE3_TEXTBOX)
            {
                int position = tbDosage3.SelectionStart;
                tbDosage3.Text = tbDosage3.Text.Insert(tbDosage3.SelectionStart, textToInsert);
                tbDosage3.SelectionStart = textToInsert.Length + position;
            }
        }

        private void toggleLinkButtonEnablement()
        {
            bool enabled = ( (tbAdultDosage.Text.Length > 0) || (tbDosage2.Text.Length > 0) || (tbDosage3.Text.Length > 0) || (tbDosage4.Text.Length > 0) );

            btnAdultLink.Enabled = enabled;
            btnDosage2Link.Enabled = enabled;
            btnDosage3Link.Enabled = enabled;
            btnDosage4Link.Enabled = enabled;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
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
            Guid? uuid = null;
            if (null != this.currentDosage) uuid = this.currentDosage.Uuid;

            OnItemDeleteRequested(new NodeEventArgs(dataContext, uuid));
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

        private void tbAdultDosage_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = ADULT_TEXTBOX;
        }

        private void tbDosage2_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE2_TEXTBOX;
        }

        private void tbDosage3_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE3_TEXTBOX;
        }

        private void tbDosage4_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE4_TEXTBOX;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            toggleLinkButtonEnablement();
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
            Boolean origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            tbAdultDosage.Tag = btnAdultLink;
            tbDosage2.Tag = btnDosage2Link;
            tbDosage3.Tag = btnDosage3Link;
            tbDosage4.Tag = btnDosage4Link;

            chkPreviousDose2.Tag = btnDosage2Link;
            chkPreviousDose3.Tag = btnDosage3Link;
            chkPreviousDose4.Tag = btnDosage4Link;

            btnAdultLink.Tag = BDDosage.PROPERTYNAME_DOSAGE;
            btnDosage2Link.Tag = BDDosage.PROPERTYNAME_DOSAGE2;
            btnDosage3Link.Tag = BDDosage.PROPERTYNAME_DOSAGE3;
            btnDosage4Link.Tag = BDDosage.PROPERTYNAME_DOSAGE4;

            tbAdultDosage.SelectAll();

            BDCommon.Settings.IsUpdating = origState;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentControlName == ADULT_TEXTBOX)
                tbAdultDosage.Undo();
            else if (currentControlName == DOSAGE2_TEXTBOX)
                tbDosage2.Undo();
            else if (currentControlName == DOSAGE3_TEXTBOX)
                tbDosage3.Undo();
            else if (currentControlName == DOSAGE4_TEXTBOX)
                tbDosage4.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentControlName == ADULT_TEXTBOX)
                tbAdultDosage.Cut();
            else if (currentControlName == DOSAGE2_TEXTBOX)
                tbDosage2.Cut();
            else if (currentControlName == DOSAGE3_TEXTBOX)
                tbDosage3.Cut();
            else if (currentControlName == DOSAGE4_TEXTBOX)
                tbDosage4.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentControlName == ADULT_TEXTBOX)
                tbAdultDosage.Copy();
            else if (currentControlName == DOSAGE2_TEXTBOX)
                tbDosage2.Copy();
            else if (currentControlName == DOSAGE3_TEXTBOX)
                tbDosage3.Copy();
            else if (currentControlName == DOSAGE4_TEXTBOX)
                tbDosage4.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentControlName == ADULT_TEXTBOX)
                tbAdultDosage.Paste();
            else if (currentControlName == DOSAGE2_TEXTBOX)
                tbDosage2.Paste();
            else if (currentControlName == DOSAGE3_TEXTBOX)
                tbDosage3.Paste();
            else if (currentControlName == DOSAGE4_TEXTBOX)
                tbDosage4.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox tb = null;
            if (currentControlName == ADULT_TEXTBOX)
                tb = tbAdultDosage;
            else if (currentControlName == DOSAGE2_TEXTBOX)
                tb = tbDosage2;
            else if (currentControlName == DOSAGE3_TEXTBOX)
                tb = tbDosage3;
            else if (currentControlName == DOSAGE4_TEXTBOX)
                tb = tbDosage4;
            if (tb != null)
            {   
                int i = tb.SelectionStart;
                tb.Text = tb.Text.Substring(0, i) + tb.Text.Substring(i + tb.SelectionLength);
                tb.SelectionStart = i;
                tb.SelectionLength = 0;
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
                        TextBox tb = null;
            if (currentControlName == ADULT_TEXTBOX)
                tb = tbAdultDosage;
            else if (currentControlName == DOSAGE2_TEXTBOX)
                tb = tbDosage2;
            else if (currentControlName == DOSAGE3_TEXTBOX)
                tb = tbDosage3;
            else if (currentControlName == DOSAGE4_TEXTBOX)
                tb = tbDosage4;
            if (tb != null)
                tb.SelectAll();
        }

        private void contextMenuStripTextBox_Opening(object sender, CancelEventArgs e)
        {
            TextBox tb = null;
            if (currentControlName == ADULT_TEXTBOX)
                tb = tbAdultDosage;
            else if (currentControlName == DOSAGE2_TEXTBOX)
                tb = tbDosage2;
            else if (currentControlName == DOSAGE3_TEXTBOX)
                tb = tbDosage3;
            else if (currentControlName == DOSAGE4_TEXTBOX)
                tb = tbDosage4;
            if (tb != null)
            {
                undoToolStripMenuItem.Enabled = tb.CanUndo;
                pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
                cutToolStripMenuItem.Enabled = (tb.SelectionLength > 0);
                copyToolStripMenuItem.Enabled = (tb.SelectionLength > 0);
                deleteToolStripMenuItem.Enabled = (tb.SelectionLength > 0);
            }
        }
    }
}
