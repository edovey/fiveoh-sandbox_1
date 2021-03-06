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
        private const string COST2_TEXTBOX = "Cost 2";

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

        public BDDosageAndCostControl()
        {
            InitializeComponent();
        }

        public BDDosageAndCostControl(Entities pDataContext, IBDNode pNode)
        {
            InitializeComponent();
            dataContext = pDataContext;
            currentDosage = pNode as BDDosage;
            parentId = pNode.ParentId;
            DefaultNodeType = pNode.NodeType;
            DefaultLayoutVariantType = pNode.LayoutVariant;

            tbDosage.Tag = btnDosageLink;
            tbCost.Tag = btnCostLink;
            tbCost2.Tag = btnCost2Link;

            btnDosageLink.Tag = BDDosage.PROPERTYNAME_DOSAGE;
            btnCostLink.Tag = BDDosage.PROPERTYNAME_COST;
            btnCost2Link.Tag = BDDosage.PROPERTYNAME_COST2;

        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentDosage) ? this.currentDosage.uuid : Guid.Empty);
            btnDosageLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosageLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnCostLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnCostLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnCost2Link.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnCost2Link.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
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
                tbDosage.Text = @"";
                tbCost.Text = @"";
                tbCost2.Text = @"";
                noneRadioButton.Checked = true;
            }
            else
            {
                this.BackColor = SystemColors.Control;

                tbDosage.Text = currentDosage.dosage;
                tbCost.Text = currentDosage.cost;
                tbCost2.Text = currentDosage.cost2;
                DisplayOrder = currentDosage.displayOrder;

                switch ((BDConstants.BDJoinType)currentDosage.joinType)
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
                    (tbDosage.Text != string.Empty) ||
                    (tbCost.Text != string.Empty) ||
                    (tbCost2.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentDosage)
                {
                    if (currentDosage.dosage != tbDosage.Text) currentDosage.dosage = tbDosage.Text;
                    if (currentDosage.cost != tbCost.Text) currentDosage.cost = tbCost.Text;
                    if (currentDosage.cost2 != tbCost2.Text) currentDosage.cost2 = tbCost2.Text;
                    if (currentDosage.displayOrder != DisplayOrder) currentDosage.displayOrder = DisplayOrder;

                    if (andRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDConstants.BDJoinType.AndWithNext)
                            currentDosage.joinType = (int)BDConstants.BDJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDConstants.BDJoinType.OrWithNext)
                            currentDosage.joinType = (int)BDConstants.BDJoinType.OrWithNext;
                    }
                    else if (thenRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDConstants.BDJoinType.ThenWithNext)
                            currentDosage.joinType = (int)BDConstants.BDJoinType.ThenWithNext;
                    }
                    else if (andOrRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDConstants.BDJoinType.WithOrWithoutWithNext)
                            currentDosage.joinType = (int)BDConstants.BDJoinType.WithOrWithoutWithNext;
                    }
                    else if (otherRadioButton.Checked)
                    {
                        if (currentDosage.joinType != (int)BDConstants.BDJoinType.Other)
                            currentDosage.joinType = (int)BDConstants.BDJoinType.Other;
                    }
                    else
                    {
                        if (currentDosage.joinType != (int)BDConstants.BDJoinType.Next)
                            currentDosage.joinType = (int)BDConstants.BDJoinType.Next;
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
            else if (currentControlName == COST2_TEXTBOX)
            {
                int position = tbCost2.SelectionStart;
                tbCost2.Text = tbCost2.Text.Insert(tbCost2.SelectionStart, textToInsert);
                tbCost2.SelectionStart = textToInsert.Length + position;
            }
        }

        private void toggleLinkButtonEnablement()
        {
            bool enabled = ( (tbDosage.Text.Length > 0) || (tbCost.Text.Length > 0)  || (tbCost2.Text.Length > 0));

            btnDosageLink.Enabled = enabled;
            btnCostLink.Enabled = enabled;
            btnCost2Link.Enabled = enabled;
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


        private void superscriptTwoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "²";
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

        private void rtbDosage_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_TEXTBOX;
        }

        private void tbCost_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = COST_TEXTBOX;
        }

        private void tbCost2_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = COST2_TEXTBOX;
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
            tbDosage.SelectAll();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentControlName == DOSAGE_TEXTBOX)
                tbDosage.Undo();
            else if (currentControlName == COST_TEXTBOX)
                tbCost.Undo();
            else if (currentControlName == COST2_TEXTBOX)
                tbCost2.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentControlName == DOSAGE_TEXTBOX)
                tbDosage.Cut();
            else if (currentControlName == COST_TEXTBOX)
                tbCost.Cut();
            else if (currentControlName == COST2_TEXTBOX)
                tbCost2.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentControlName == DOSAGE_TEXTBOX)
                tbDosage.Copy();
            else if (currentControlName == COST_TEXTBOX)
                tbCost.Copy();
            else if (currentControlName == COST2_TEXTBOX)
                tbCost2.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentControlName == DOSAGE_TEXTBOX)
                tbDosage.Paste();
            else if (currentControlName == COST_TEXTBOX)
                tbCost.Paste();
            else if (currentControlName == COST2_TEXTBOX)
                tbCost2.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox tb = null;
            if (currentControlName == DOSAGE_TEXTBOX)
                tb = tbDosage;
            else if (currentControlName == COST_TEXTBOX)
                tb = tbCost;
            else if (currentControlName == COST2_TEXTBOX)
                tb = tbCost2;
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
            if (currentControlName == DOSAGE_TEXTBOX)
                tbDosage.SelectAll();
            else if (currentControlName == COST_TEXTBOX)
                tbCost.SelectAll();
            else if (currentControlName == COST2_TEXTBOX)
                tbCost2.SelectAll();
        }

        private void contextMenuStripTextBox_Opening(object sender, CancelEventArgs e)
        {
            TextBox tb = null;
            if (currentControlName == DOSAGE_TEXTBOX)
                tb = tbDosage;
            else if (currentControlName == COST_TEXTBOX)
                tb = tbCost;
            else if (currentControlName == COST2_TEXTBOX)
                tb = tbCost2;
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
