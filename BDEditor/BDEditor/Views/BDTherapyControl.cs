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

namespace BDEditor.Views
{
    public partial class BDTherapyControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDTherapy currentTherapy;
        private Guid? therapyGroupId;
        private IBDControl parentControl;
        private Guid? scopeId;
        private bool displayLeftBracket;
        private bool displayRightBracket;
        
        public int? DisplayOrder {get; set;}

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;

        protected virtual void OnItemAddRequested(EventArgs e)
        {
            if (null != RequestItemAdd) { RequestItemAdd(this, e); }
        }

        protected virtual void OnItemDeleteRequested(EventArgs e)
        {
            if (null != RequestItemDelete) { RequestItemDelete(this, e); }
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
            }
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
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        private void btnTherapyLink_Click(object sender, EventArgs e)
        {
            CreateLink(BDTherapy.PROPERTYNAME_THERAPY);
        }

        private void btnDosageLink_Click(object sender, EventArgs e)
        {
            CreateLink(BDTherapy.PROPERTYNAME_DOSAGE);
        }

        private void btnDurationLink_Click(object sender, EventArgs e)
        {
            CreateLink(BDTherapy.PROPERTYNAME_DURATION);
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
                view.AssignContextEntityName(BDTherapy.ENTITYNAME_FRIENDLY);
                view.AssignScopeId(scopeId);

                if (null != currentTherapy)
                {
                    view.AssignParentId(currentTherapy.uuid);
                }
                else
                {
                    view.AssignParentId(null);
                }
                view.PopulateControl();
                view.ShowDialog(this);
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
                if (null == this.therapyGroupId)
                {
                    result = false;
                }
                else
                {
                    this.currentTherapy = BDTherapy.CreateTherapy(this.dataContext, this.therapyGroupId.Value);
                    this.currentTherapy.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                }
            }

            return result;
        }

        public bool Save()
        {
            bool result = false;
            if (null != therapyGroupId)
            {
                if ((null == currentTherapy) &&
                    ((tbName.Text != string.Empty) ||
                    (tbDosage.Text != string.Empty) ||
                    (tbDuration.Text != string.Empty)))
                {
                    CreateCurrentObject();
                }

                if (null != currentTherapy)
                {
                    if (currentTherapy.name != tbName.Text) currentTherapy.name = tbName.Text;
                    if (currentTherapy.dosage != tbDosage.Text) currentTherapy.dosage = tbDosage.Text;
                    if (currentTherapy.duration != tbDuration.Text) currentTherapy.duration = tbDuration.Text;

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
                    else
                    {
                        if (currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.None)
                            currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.None;
                    }

                    if(currentTherapy.leftBracket != this.displayLeftBracket) currentTherapy.leftBracket = this.displayLeftBracket;
                    if(currentTherapy.rightBracket != this.displayRightBracket) currentTherapy.rightBracket = this.displayRightBracket;

                    BDTherapy.SaveTherapy(dataContext, currentTherapy);

                    result = true;
                }
            }

            return result;
        }

        public void AssignParentId(Guid? pParentId)
        {
            therapyGroupId = pParentId;
        }

        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        #endregion

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (null != textBox)
            {
                Button linkButton = textBox.Tag as Button;
                if (null != linkButton)
                    linkButton.Enabled = true;
            }
        }

        private void BDTherapyControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Text = tbName.Text.Insert(tbName.SelectionStart, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Text = tbName.Text.Insert(tbName.SelectionStart, "°");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Text = tbName.Text.Insert(tbName.SelectionStart, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Text = tbName.Text.Insert(tbName.SelectionStart, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Text = tbName.Text.Insert(tbName.SelectionStart, "±");
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
    }
}