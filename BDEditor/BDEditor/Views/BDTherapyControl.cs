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

        public BDTherapy CurrentTherapy
        {
            get
            {
                return currentTherapy;
            }
            set
            {
                currentTherapy = value;
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
            Save();
            BDLinkedNoteView view = new BDLinkedNoteView();
            view.AssignDataContext(dataContext);
            view.AssignContextPropertyName(pProperty);
            view.AssignParentControl(this);
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

        #region IBDControl

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
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
                    currentTherapy = BDTherapy.CreateTherapy(dataContext);
                    currentTherapy.therapyGroupId = therapyGroupId;
                    currentTherapy.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
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

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (null != textBox)
            {
                Button linkButton = textBox.Tag as Button;
                if (null != linkButton)
                    linkButton.Enabled = true;

                if (null == currentTherapy)
                {
                    parentControl.TriggerCreateAndAssignParentIdToChildControl(this);
                }
            }
        }

        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            if (null == currentTherapy)
            {
                currentTherapy = BDTherapy.CreateTherapy(dataContext);
                currentTherapy.therapyGroupId = therapyGroupId;
                currentTherapy.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                BDTherapy.SaveTherapy(dataContext, currentTherapy);
                pControl.AssignParentId(currentTherapy.uuid);
                pControl.Save();
            }
            else
            {
                pControl.AssignParentId(currentTherapy.uuid);
                pControl.Save();
            }
        }

        private void BDTherapyControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        #endregion
    }
}