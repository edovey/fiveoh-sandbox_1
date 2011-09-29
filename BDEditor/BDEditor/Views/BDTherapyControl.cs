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
    public partial class BDTherapyControl : UserControl
    {
        private BDTherapy currentTherapy;

        public BDTherapy CurrentTherapy
        {
            get
            {
                return currentTherapy;
            }
            set {
                currentTherapy = value;
                if(currentTherapy == null) {
                    tbName.Text = @"";
                    tbDosage.Text = @"";
                    tbDuration.Text = @"";}
                else {
                    tbName.Text = currentTherapy.name;
                    tbDosage.Text = currentTherapy.dosage;
                    tbDuration.Text = currentTherapy.duration;
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
    }
}
