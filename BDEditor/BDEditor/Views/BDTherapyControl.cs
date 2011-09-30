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
                    tbName.Text = @"";
                    tbDosage.Text = @"";
                    tbDuration.Text = @"";
                    noneRadioButton.Checked = true;
                }
                else 
                {
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

        public void Save()
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

        }
    }
}
