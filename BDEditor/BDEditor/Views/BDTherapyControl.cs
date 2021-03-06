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
    public partial class BDTherapyControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDTherapy currentTherapy;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private bool displayLeftBracket;
        private bool displayRightBracket;
        private string currentControlName;

        private const string NAME_TEXTBOX = "Name";
        private const string DOSAGE_TEXTBOX = "Dosage";
        private const string DOSAGE_1_TEXTBOX = "Dosage1";
        private const string DOSAGE_2_TEXTBOX = "Dosage2";
        private const string DURATION_TEXTBOX = "Duration";
        private const string DURATION_1_TEXTBOX = "Duration1";
        private const string DURATION_2_TEXTBOX = "Duration2";

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

        public BDTherapy CurrentTherapy
        {
            get { return currentTherapy; }
            set { currentTherapy = value; }
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

        public BDTherapyControl()
        {
            InitializeComponent();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentTherapy) ? this.currentTherapy.uuid : Guid.Empty);
            btnTherapyLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnTherapyLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnDosageLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosageLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnDurationLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDurationLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;

            if (pnlMain.Controls.Contains(btnDosage1Link))
                btnDosage1Link.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosage1Link.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            if (pnlMain.Controls.Contains(btnDosage2Link))
                btnDosage2Link.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosage2Link.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            if (pnlMain.Controls.Contains(btnDuration1Link))
                btnDuration1Link.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDuration1Link.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            if (pnlMain.Controls.Contains(btnDuration2Link))
                btnDuration2Link.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDuration2Link.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }     
        
        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource, string pProperty)
        {
            if (pProperty == string.Empty || pProperty == BDTherapy.PROPERTYNAME_THERAPY)
            {
                tbName.AutoCompleteCustomSource = pSource;
                tbName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            } 
            else if (pProperty == BDTherapy.PROPERTYNAME_DOSAGE)
            {
                tbDosage.AutoCompleteCustomSource = pSource;
                tbDosage.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDosage.AutoCompleteSource = AutoCompleteSource.CustomSource;
            } 
            else if (pProperty == BDTherapy.PROPERTYNAME_DURATION)
            {
                tbDuration.AutoCompleteCustomSource = pSource;
                tbDuration.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDuration.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            else if (pProperty == BDTherapy.PROPERTYNAME_DOSAGE_1)
            {
                tbDosage1.AutoCompleteCustomSource = pSource;
                tbDosage1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDosage1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            else if (pProperty == BDTherapy.PROPERTYNAME_DOSAGE_2)
            {
                tbDosage2.AutoCompleteCustomSource = pSource;
                tbDosage2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDosage2.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            else if (pProperty == BDTherapy.PROPERTYNAME_DURATION_1)
            {
                tbDuration1.AutoCompleteCustomSource = pSource;
                tbDuration1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDuration1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            else if (pProperty == BDTherapy.PROPERTYNAME_DURATION_2)
            {
                tbDuration2.AutoCompleteCustomSource = pSource;
                tbDuration2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbDuration2.AutoCompleteSource = AutoCompleteSource.CustomSource;
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
            if (currentTherapy == null)
            {
                tbName.Text = @"";
                tbDosage.Text = @"";
                tbDuration.Text = @"";
                nextTherapyRadioButton.Checked = true;
                lblLeftBracket.ForeColor = SystemColors.ControlLight;
                lblRightBracket.ForeColor = SystemColors.ControlLight;
                chkPreviousName.Checked = false;
                chkPreviousDose.Checked = false;
                chkPreviousDuration.Checked = false;
            }
            else
            {
                this.BackColor = SystemColors.Control;

                tbName.Text = currentTherapy.name;
                tbDosage.Text = currentTherapy.dosage;
                tbDuration.Text = currentTherapy.duration;
                DisplayOrder = currentTherapy.displayOrder;

                if (pnlMain.Controls.Contains(tbDosage1))
                    tbDosage1.Text = currentTherapy.dosage1;
                if(pnlMain.Controls.Contains(tbDosage2))
                    tbDosage2.Text = currentTherapy.dosage2;
                if(pnlMain.Controls.Contains(tbDuration1))
                    tbDuration1.Text = currentTherapy.duration1;
                if(pnlMain.Controls.Contains(tbDuration2))
                    tbDuration2.Text = currentTherapy.duration2;

                switch ((BDConstants.BDJoinType)currentTherapy.therapyJoinType)
                {
                    case BDConstants.BDJoinType.Next:
                        nextTherapyRadioButton.Checked = true;
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
                        nextTherapyRadioButton.Checked = true;
                        break;
                }

                displayLeftBracket = currentTherapy.leftBracket.Value;
                lblLeftBracket.ForeColor = (displayLeftBracket) ? SystemColors.ControlText : SystemColors.ControlLight;

                displayRightBracket = currentTherapy.rightBracket.Value;
                lblRightBracket.ForeColor = (displayRightBracket) ? SystemColors.ControlText : SystemColors.ControlLight;

                chkPreviousName.Checked = currentTherapy.nameSameAsPrevious.Value;
                chkPreviousDose.Checked = currentTherapy.dosageSameAsPrevious.Value;
                chkPreviousDuration.Checked = currentTherapy.durationSameAsPrevious.Value;

                if (pnlMain.Controls.Contains(chkPreviousDose1) && currentTherapy.dosage1SameAsPrevious.HasValue)
                    chkPreviousDose1.Checked = currentTherapy.dosage1SameAsPrevious.Value;
                if (pnlMain.Controls.Contains(chkPreviousDose2) && currentTherapy.dosage2SameAsPrevious.HasValue)
                    chkPreviousDose2.Checked = currentTherapy.dosage2SameAsPrevious.Value;

                if (pnlMain.Controls.Contains(chkPreviousDuration1) && currentTherapy.duration1SameAsPrevious.HasValue)
                    chkPreviousDuration1.Checked = currentTherapy.duration1SameAsPrevious.Value;
                if (pnlMain.Controls.Contains(chkPreviousDuration2) && currentTherapy.duration2SameAsPrevious.HasValue)
                    chkPreviousDuration2.Checked = currentTherapy.duration2SameAsPrevious.Value;

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

            if (null == this.currentTherapy)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentTherapy = BDTherapy.CreateBDTherapy(this.dataContext, this.parentId.Value);
                    currentTherapy.SetParent(parentType, parentId);
                    currentTherapy.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentTherapy.LayoutVariant = DefaultLayoutVariantType;
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
                if ((null == currentTherapy) &&
                    ((tbName.Text != string.Empty) ||
                    (tbDosage.Text != string.Empty) ||
                    (tbDuration.Text != string.Empty) ||
                    chkPreviousName.Checked ||
                    chkPreviousDose.Checked ||
                    chkPreviousDuration.Checked))
                {
                    CreateCurrentObject();
                }

                if (null != currentTherapy)
                {
                    if (currentTherapy.name != tbName.Text) currentTherapy.name = tbName.Text;
                    if (currentTherapy.dosage != tbDosage.Text) currentTherapy.dosage = tbDosage.Text;
                    if (currentTherapy.duration != tbDuration.Text) currentTherapy.duration = tbDuration.Text;
                    if (currentTherapy.displayOrder != DisplayOrder) currentTherapy.displayOrder = DisplayOrder;

                    if (pnlMain.Controls.Contains(tbDosage1) && currentTherapy.dosage1 != tbDosage1.Text)
                        currentTherapy.dosage1 = tbDosage1.Text;
                    if (pnlMain.Controls.Contains(tbDosage2) && currentTherapy.dosage2 != tbDosage2.Text)
                        currentTherapy.dosage2 = tbDosage2.Text;
                    if (pnlMain.Controls.Contains(tbDuration1) && currentTherapy.duration1 != tbDuration1.Text)
                        currentTherapy.duration1 = tbDuration1.Text;
                    if (pnlMain.Controls.Contains(tbDuration2) && currentTherapy.duration2 != tbDuration2.Text)
                        currentTherapy.duration2 = tbDuration2.Text;

                    if (andRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDConstants.BDJoinType.AndWithNext)
                            currentTherapy.therapyJoinType = (int)BDConstants.BDJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDConstants.BDJoinType.OrWithNext)
                            currentTherapy.therapyJoinType = (int)BDConstants.BDJoinType.OrWithNext;
                    }
                    else if (thenRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDConstants.BDJoinType.ThenWithNext)
                            currentTherapy.therapyJoinType = (int)BDConstants.BDJoinType.ThenWithNext;
                    }
                    else if (andOrRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDConstants.BDJoinType.WithOrWithoutWithNext)
                            currentTherapy.therapyJoinType = (int)BDConstants.BDJoinType.WithOrWithoutWithNext;
                    }
                    else if (otherRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDConstants.BDJoinType.Other)
                            currentTherapy.therapyJoinType = (int)BDConstants.BDJoinType.Other;
                    }
                    else if (aoRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDConstants.BDJoinType.AndOr)
                            currentTherapy.therapyJoinType = (int)BDConstants.BDJoinType.AndOr;
                    }
                   else 
                    {
                        if (currentTherapy.therapyJoinType != (int)BDConstants.BDJoinType.Next)
                            currentTherapy.therapyJoinType = (int)BDConstants.BDJoinType.Next;
                    }

                    if(currentTherapy.leftBracket != this.displayLeftBracket) currentTherapy.leftBracket = this.displayLeftBracket;
                    if(currentTherapy.rightBracket != this.displayRightBracket) currentTherapy.rightBracket = this.displayRightBracket;

                    if (currentTherapy.nameSameAsPrevious != this.chkPreviousName.Checked) currentTherapy.nameSameAsPrevious = this.chkPreviousName.Checked;
                    if (currentTherapy.dosageSameAsPrevious != this.chkPreviousDose.Checked) currentTherapy.dosageSameAsPrevious = this.chkPreviousDose.Checked;
                    if (currentTherapy.durationSameAsPrevious != this.chkPreviousDuration.Checked) currentTherapy.durationSameAsPrevious = this.chkPreviousDuration.Checked;

                    if (pnlMain.Controls.Contains(chkPreviousDose1) && currentTherapy.dosage1SameAsPrevious != chkPreviousDose1.Checked)
                        currentTherapy.dosage1SameAsPrevious = this.chkPreviousDose1.Checked;
                    if (pnlMain.Controls.Contains(chkPreviousDose2) && currentTherapy.dosage2SameAsPrevious != chkPreviousDose2.Checked)
                        currentTherapy.dosage2SameAsPrevious = this.chkPreviousDose2.Checked;
                    if (pnlMain.Controls.Contains(chkPreviousDuration1) && currentTherapy.duration1SameAsPrevious != chkPreviousDuration1.Checked)
                        currentTherapy.duration1SameAsPrevious = chkPreviousDuration1.Checked;
                    if (pnlMain.Controls.Contains(chkPreviousDuration2) && currentTherapy.duration2SameAsPrevious != chkPreviousDuration2.Checked)
                        currentTherapy.duration2SameAsPrevious = chkPreviousDuration2.Checked;

                    BDTherapy.Save(dataContext, currentTherapy);
                    result = true;

                    if(currentTherapy.name.Length > 0) 
                       BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, currentTherapy.name);
                   if(currentTherapy.dosage.Length > 0)
                       BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, currentTherapy.dosage);
                    if(currentTherapy.duration.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, currentTherapy.duration);

                    if (pnlMain.Controls.Contains(tbDosage1) && currentTherapy.dosage1.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, currentTherapy.dosage1);
                    if (pnlMain.Controls.Contains(tbDosage2) && currentTherapy.dosage2.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, currentTherapy.dosage2);
                    if (pnlMain.Controls.Contains(tbDuration1) && currentTherapy.duration1.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, currentTherapy.duration1);
                    if (pnlMain.Controls.Contains(tbDuration2) && currentTherapy.duration2.Length > 0)
                        BDTypeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, currentTherapy.duration2);

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
                view.AssignParentInfo(currentTherapy.Uuid, currentTherapy.NodeType);
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
            else if (currentControlName == DOSAGE_1_TEXTBOX)
            {
                int position = tbDosage1.SelectionStart;
                tbDosage1.Text = tbDosage1.Text.Insert(tbDosage1.SelectionStart, textToInsert);
                tbDosage1.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DOSAGE_2_TEXTBOX)
            {
                int position = tbDosage2.SelectionStart;
                tbDosage2.Text = tbDosage2.Text.Insert(tbDosage2.SelectionStart, textToInsert);
                tbDosage2.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DURATION_1_TEXTBOX)
            {
                int position = tbDuration1.SelectionStart;
                tbDuration1.Text = tbDuration1.Text.Insert(tbDuration1.SelectionStart, textToInsert);
                tbDuration1.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == DURATION_2_TEXTBOX)
            {
                int position = tbDuration2.SelectionStart;
                tbDuration2.Text = tbDuration2.Text.Insert(tbDuration2.SelectionStart, textToInsert);
                tbDuration2.SelectionStart = textToInsert.Length + position;
            }
        }

        private void toggleLinkButtonEnablement()
        {
            bool enabled = ((chkPreviousDose.Checked || chkPreviousDuration.Checked || chkPreviousName.Checked) ||
                chkPreviousDose1.Checked || chkPreviousDose2.Checked || chkPreviousDuration1.Checked || chkPreviousDuration2.Checked ||
                ((tbDosage.Text.Length > 0) || (tbDuration.Text.Length > 0) || (tbName.Text.Length > 0) ||
                (tbDosage1.Text.Length > 0) || (tbDosage2.Text.Length > 0) || (tbDuration1.Text.Length > 0) || (tbDuration2.Text.Length > 0) ));

            btnTherapyLink.Enabled = enabled;
            btnDosageLink.Enabled = enabled;
            btnDurationLink.Enabled = enabled;
            btnDosage1Link.Enabled = enabled;
            btnDosage2Link.Enabled = enabled;
            btnDuration1Link.Enabled = enabled;
            btnDuration2Link.Enabled = enabled;
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
            string newText =  "¹";
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
            if (null != this.currentTherapy) uuid = this.currentTherapy.Uuid;

            OnItemDeleteRequested(new NodeEventArgs(dataContext, uuid));
        }

        public override string ToString()
        {
            return (null == this.currentTherapy) ? "No Therapy" : this.currentTherapy.name;
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, currentTherapy.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, currentTherapy.Uuid));
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

        private void tbDosage1_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_1_TEXTBOX;
        }

        private void tbDosage2_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_2_TEXTBOX;
        }

        private void tbDuration1_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DURATION_1_TEXTBOX;
        }

        private void tbDuration2_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DURATION_2_TEXTBOX; 
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                return CurrentTherapy;
            }
            set
            {
                CurrentTherapy = value as BDTherapy;
            }
        }

        public bool ShowAsChild { get; set; }

        private void BDTherapyControl_Load(object sender, EventArgs e)
        {
            bool origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;


            tbName.Tag = btnTherapyLink;
            tbDosage.Tag = btnDosageLink;
            tbDuration.Tag = btnDurationLink;
            tbDosage1.Tag = btnDosage1Link;
            tbDosage2.Tag = btnDosage2Link;
            tbDuration1.Tag = btnDuration1Link;
            tbDuration2.Tag = btnDuration2Link;

            chkPreviousName.Tag = btnTherapyLink;
            chkPreviousDose.Tag = btnDosageLink;
            chkPreviousDuration.Tag = btnDurationLink;
            chkPreviousDose1.Tag = btnDosage1Link;
            chkPreviousDose2.Tag = btnDosage2Link;
            chkPreviousDuration1.Tag = btnDuration1Link;
            chkPreviousDuration2.Tag = btnDuration2Link;

            btnTherapyLink.Tag = BDTherapy.PROPERTYNAME_THERAPY;
            btnDosageLink.Tag = BDTherapy.PROPERTYNAME_DOSAGE;
            btnDurationLink.Tag = BDTherapy.PROPERTYNAME_DURATION;
            btnDosage1Link.Tag = BDTherapy.PROPERTYNAME_DOSAGE_1;
            btnDosage2Link.Tag = BDTherapy.PROPERTYNAME_DOSAGE_2;
            btnDuration1Link.Tag = BDTherapy.PROPERTYNAME_DURATION_1;
            btnDuration2Link.Tag = BDTherapy.PROPERTYNAME_DURATION_2;

            switch (DefaultLayoutVariantType)
            {
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult:
                case BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric:
                    toolTip1.SetToolTip(tbDosage, "Intermittent Dose");
                    toolTip1.SetToolTip(tbDosage1, "Continuous Dose (per L bag)");
                    break;
                case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                    toolTip1.SetToolTip(tbDosage, "Adult Dose");
                    toolTip1.SetToolTip(tbDosage1, "Paediatric Dose");
                    toolTip1.SetToolTip(tbDuration, "Adult Route/Duration");
                    toolTip1.SetToolTip(tbDuration1, "Paediatric Route/Duration");
                    break;
                case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
                    toolTip1.SetToolTip(tbDosage, "Adult");
                    toolTip1.SetToolTip(tbDosage1, "Paediatric");
                    break;
                case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                    toolTip1.SetToolTip(tbDosage, "Adult Dose");
                    toolTip1.SetToolTip(tbDosage1, "Paediatric Dose");
                    break;
                default:
                    // no changes
                    break;
            }

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
            if(!BDFabrik.TherapyLayoutHasSecondDosage(DefaultLayoutVariantType))
            {
                pnlMain.Controls.Remove(tbDosage1);
                pnlMain.Controls.Remove(chkPreviousDose1);
                pnlMain.Controls.Remove(btnDosage1Link);
            }
            if (!BDFabrik.TherapyLayoutHasSecondDuration(DefaultLayoutVariantType))
            {
                pnlMain.Controls.Remove(tbDuration1);
                pnlMain.Controls.Remove(chkPreviousDuration1);
                pnlMain.Controls.Remove(btnDuration1Link);
            }
            if (!BDFabrik.TherapyLayoutHasThirdDuration(DefaultLayoutVariantType))
            {
                pnlMain.Controls.Remove(tbDuration2);
                pnlMain.Controls.Remove(chkPreviousDuration2);
                pnlMain.Controls.Remove(btnDuration2Link);

            }
            if (!BDFabrik.TherapyLayoutHasThirdDosage(DefaultLayoutVariantType))
            {
                pnlMain.Controls.Remove(tbDosage2);
                pnlMain.Controls.Remove(chkPreviousDose2);
                pnlMain.Controls.Remove(btnDosage2Link);

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
    }
}