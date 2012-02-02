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
        private const string DURATION_TEXTBOX = "Duration";

        public int? DisplayOrder { get; set; }
        public BDConstants.LayoutVariantType DefaultLayoutVariantType;

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;
        public event EventHandler ReorderToPrevious;
        public event EventHandler ReorderToNext;
        public event EventHandler NotesChanged;

        protected virtual void OnNotesChanged(EventArgs e)
        {
            if (null != NotesChanged) { NotesChanged(this, e); }
        }

        protected virtual void OnItemAddRequested(EventArgs e)
        {
            if (null != RequestItemAdd) { RequestItemAdd(this, e); }
        }

        protected virtual void OnItemDeleteRequested(EventArgs e)
        {
            if (null != RequestItemDelete) { RequestItemDelete(this, e); }
        }

        protected virtual void OnReorderToPrevious(EventArgs e)
        {
            if (null != ReorderToPrevious) { ReorderToPrevious(this, e); }
        }

        protected virtual void OnReorderToNext(EventArgs e)
        {
            if (null != ReorderToNext) { ReorderToNext(this, e); }
        }

        public BDTherapy CurrentTherapy
        {
            get { return currentTherapy; }
            set { currentTherapy = value; }
        }

        public void RefreshLayout()
        {
            if (currentTherapy == null)
            {
                tbName.Text = @"";
                tbDosage.Text = @"";
                tbDuration.Text = @"";
                noneRadioButton.Checked = true;
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

                switch ((BDTherapy.TherapyJoinType)currentTherapy.therapyJoinType)
                {
                    case BDTherapy.TherapyJoinType.None:
                        noneRadioButton.Checked = true;
                        break;
                    case BDTherapy.TherapyJoinType.AndWithNext:
                        andRadioButton.Checked = true;
                        break;
                    case BDTherapy.TherapyJoinType.OrWithNext:
                        orRadioButton.Checked = true;
                        break;
                    case BDTherapy.TherapyJoinType.ThenWithNext:
                        thenRadioButton.Checked = true;
                        break;
                    default:
                        noneRadioButton.Checked = true;
                        break;
                }

                displayLeftBracket = currentTherapy.leftBracket.Value;
                lblLeftBracket.ForeColor = (displayLeftBracket) ? SystemColors.ControlText : SystemColors.ControlLight;

                displayRightBracket = currentTherapy.rightBracket.Value;
                lblRightBracket.ForeColor = (displayRightBracket) ? SystemColors.ControlText : SystemColors.ControlLight;

                chkPreviousName.Checked = currentTherapy.nameSameAsPrevious.Value;
                chkPreviousDose.Checked = currentTherapy.dosageSameAsPrevious.Value;
                chkPreviousDuration.Checked = currentTherapy.durationSameAsPrevious.Value;
            }
            ShowLinksInUse(false);
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

            tbName.Tag = btnTherapyLink;
            tbDosage.Tag = btnDosageLink;
            tbDuration.Tag = btnDurationLink;

            chkPreviousName.Tag = btnTherapyLink;
            chkPreviousDose.Tag = btnDosageLink;
            chkPreviousDuration.Tag = btnDurationLink;

            btnTherapyLink.Tag = BDTherapy.PROPERTYNAME_THERAPY;
            btnDosageLink.Tag = BDTherapy.PROPERTYNAME_DOSAGE;
            btnDurationLink.Tag = BDTherapy.PROPERTYNAME_DURATION;
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                string tag = control.Tag as string;
                CreateLink(tag);
            }
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentTherapy) ? this.currentTherapy.uuid : Guid.Empty);
            btnTherapyLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnTherapyLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnDosageLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDosageLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            btnDurationLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnDurationLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }     
        
        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        private void lblLeftBracket_Click(object sender, EventArgs e)
        {
            this.displayLeftBracket = !this.displayLeftBracket;
            lblLeftBracket.ForeColor = (this.displayLeftBracket) ? SystemColors.ControlText : SystemColors.ControlLight;
        }

        private void lblRightBracket_Click(object sender, EventArgs e)
        {
            this.displayRightBracket = !this.displayRightBracket;
            lblRightBracket.ForeColor = (this.displayRightBracket) ? SystemColors.ControlText : SystemColors.ControlLight;
        }

        private void CreateLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();

                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentTherapy.Uuid, currentTherapy.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler(notesChanged_Action);
                ShowLinksInUse(false);
            }
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
        }

        #region IBDControl

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
                    currentTherapy = BDTherapy.CreateTherapy(this.dataContext, this.parentId.Value);
                    currentTherapy.SetParent(parentType, parentId);
                    currentTherapy.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentTherapy.LayoutVariant = DefaultLayoutVariantType;
                }
            }

            return result;
        }

        public bool Save()
        {
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

                    if (andRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.AndWithNext)
                            currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.OrWithNext)
                            currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.OrWithNext;
                    }
                    else if (thenRadioButton.Checked)
                    {
                        if (currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.ThenWithNext)
                            currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.ThenWithNext;
                    }
                    else if (andOrRadioButton.Checked)
                    {
                       if (currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.AndOrWithNext)
                            currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.AndOrWithNext;
                    }
                   else 
                    {
                        if (currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.None)
                            currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.None;
                    }

                    if(currentTherapy.leftBracket != this.displayLeftBracket) currentTherapy.leftBracket = this.displayLeftBracket;
                    if(currentTherapy.rightBracket != this.displayRightBracket) currentTherapy.rightBracket = this.displayRightBracket;

                    if (currentTherapy.nameSameAsPrevious != this.chkPreviousName.Checked) currentTherapy.nameSameAsPrevious = this.chkPreviousName.Checked;
                    if (currentTherapy.dosageSameAsPrevious != this.chkPreviousDose.Checked) currentTherapy.dosageSameAsPrevious = this.chkPreviousDose.Checked;
                    if (currentTherapy.durationSameAsPrevious != this.chkPreviousDuration.Checked) currentTherapy.durationSameAsPrevious = this.chkPreviousDuration.Checked;

                    BDTherapy.Save(dataContext, currentTherapy);
                    result = true;

                    if(currentTherapy.name.Length > 0) 
                       Typeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, currentTherapy.name);
                   if(currentTherapy.dosage.Length > 0)
                       Typeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, currentTherapy.dosage);
                    if(currentTherapy.duration.Length > 0)
                        Typeahead.AddToCollection(BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DURATION, currentTherapy.duration);

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

        private void insertTextFromMenu(TextBox textbox, string textToInsert, int selectionStart)
        {
            textbox.Text = textbox.Text.Insert(selectionStart, textToInsert);
            textbox.SelectionStart = selectionStart + 1;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            //TextBox textBox = sender as TextBox;
            //if (null != textBox)
            //{
            //    Button linkButton = textBox.Tag as Button;
            //    if (null != linkButton)
            //        linkButton.Enabled = true;
            //}
            toggleLinkButtonEnablement();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            //CheckBox chkBox = sender as CheckBox;
            //if (null != chkBox)
            //{
            //    Button linkButton = chkBox.Tag as Button;
            //    if (null != linkButton)
            //        linkButton.Enabled = true;
            //}
            toggleLinkButtonEnablement();
        }

        private void toggleLinkButtonEnablement()
        {
            bool enabled = ((chkPreviousDose.Checked || chkPreviousDuration.Checked || chkPreviousName.Checked) ||
                ((tbDosage.Text.Length > 0) || (tbDuration.Text.Length > 0) || (tbName.Text.Length > 0)));

            btnTherapyLink.Enabled = enabled;
            btnDosageLink.Enabled = enabled;
            btnDurationLink.Enabled = enabled;
        }

        private void BDTherapyControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "ß";
            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "°";
            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "µ";

            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≥";

            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≤";

            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "±";

            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OnItemAddRequested(new EventArgs());
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
        }

        public override string ToString()
        {
            return (null == this.currentTherapy) ? "No Therapy" : this.currentTherapy.name;
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new EventArgs());
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new EventArgs());
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void notesChanged_Action(object sender, EventArgs e)
        {
            OnNotesChanged(new EventArgs());
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

    }
}