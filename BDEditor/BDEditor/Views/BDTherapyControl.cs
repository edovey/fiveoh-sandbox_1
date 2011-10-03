using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDTherapyControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDTherapy currentTherapy;
        private Guid? therapyGroupId;
        private IBDControl parentControl;

        public BDTherapy CurrentTherapy
        {
            get
            {
                return currentTherapy;
            }
            set 
            {
                currentTherapy = value;
                if(currentTherapy == null) 
                {
                    this.BackColor = SystemColors.ControlDark;

                    tbName.Text = @"";
                    tbDosage.Text = @"";
                    tbDuration.Text = @"";
                    noneRadioButton.Checked = true;
                }
                else 
                {
                    this.BackColor = SystemColors.Control;

                    tbName.Text = currentTherapy.name;
                    tbDosage.Text = currentTherapy.dosage;
                    tbDuration.Text = currentTherapy.duration;
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
                }
            }
        }

        public BDTherapyControl()
        {
            InitializeComponent();
        }

        private void btnTherapyLink_Click(object sender, EventArgs e)
        {
            CreateLink();
        }

        private void CreateLink()
        {
            // show context menu when button clicked for possible actions:
            // create new link, edit existing, delete, redirect to different?
        }

        // -- IBDControl

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool Save()
        {
            bool result = false;
            if (null != therapyGroupId)
            {
                if ( (null == currentTherapy) && 
                    ( (tbName.Text != string.Empty) ||
                    (tbDosage.Text != string.Empty) ||
                    (tbDuration.Text != string.Empty) ) )
                {
                    currentTherapy = BDTherapy.CreateTherapy(dataContext);
                    currentTherapy.therapyGroupId = therapyGroupId;
                }

                if (null != currentTherapy)
                {
                    currentTherapy.name = tbName.Text;
                    currentTherapy.dosage = tbDosage.Text;
                    currentTherapy.duration = tbDuration.Text;

                    if (andRadioButton.Checked)
                    {
                        currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.OrWithNext;
                    }
                    else
                    {
                        currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.None;
                    }

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
                this.BackColor = (textBox.Text.Trim() != string.Empty) ? SystemColors.Control : SystemColors.ControlDark;
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
                BDTherapy.SaveTherapy(dataContext, currentTherapy);
                pControl.AssignParentId(currentTherapy.uuid);
                pControl.Save();

                this.BackColor = SystemColors.Control;
            }
        }

        private void BDTherapyControl_Leave(object sender, EventArgs e)
        {
            Save();
        }
    }
}
