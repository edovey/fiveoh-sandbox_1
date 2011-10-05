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
                    //this.BackColor = SystemColors.ControlDark;

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

            tbName.Tag = btnTherapyLink;
            tbDosage.Tag = btnDosageLink;
            tbDuration.Tag = btnDurationLink;
        }

        private void btnTherapyLink_Click(object sender, EventArgs e)
        {
            CreateLink(@"Therapy");
        }

        private void btnDosageLink_Click(object sender, EventArgs e)
        {
            CreateLink(@"Dosage");
        }

        private void btnDurationLink_Click(object sender, EventArgs e)
        {
            CreateLink(@"Duration");
        }
        private void CreateLink(string pProperty)
        {
            BDLinkedNoteView view = new BDLinkedNoteView();
            view.AssignDataContext(dataContext);
            view.AssignContextPropertyName(pProperty);

            if (null != currentTherapy)
            {
                BDLinkedNote note = BDLinkedNote.GetLinkedNoteForParentIdAndPropertyName(dataContext, currentTherapy.uuid, pProperty);
                view.AssignParentId(currentTherapy.uuid);
                view.CurrentLinkNote = note;
            }
            else
            {
                view.AssignParentId(null);
                view.CurrentLinkNote = null;
            }

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
                    if(currentTherapy.name != tbName.Text) currentTherapy.name = tbName.Text;
                    if(currentTherapy.dosage != tbDosage.Text) currentTherapy.dosage = tbDosage.Text;
                    if(currentTherapy.duration != tbDuration.Text) currentTherapy.duration = tbDuration.Text;

                    if (andRadioButton.Checked)
                    {
                        if(currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.AndWithNext) 
                            currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if(currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.OrWithNext) 
                            currentTherapy.therapyJoinType = (int)BDTherapy.TherapyJoinType.OrWithNext;
                    }
                    else
                    {
                        if(currentTherapy.therapyJoinType != (int)BDTherapy.TherapyJoinType.None) 
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
                //this.BackColor = (textBox.Text.Trim() != string.Empty) ? SystemColors.Control : SystemColors.ControlDark;

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

        #endregion
    }
}
