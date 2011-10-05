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
    public partial class BDDiseaseControl : UserControl
    {
        private BDDisease currentDisease;

        public BDDisease CurrentDisease
        {
            get
            {
                return currentDisease;
            }
            set
            {
                currentDisease = value;
                if (currentDisease == null)
                {
                    tbDiseaseName.Text = @"";
                    tbDiseaseOverview.Text = @"";
                }
                else
                {
                    tbDiseaseName.Text = currentDisease.name;
                    tbDiseaseOverview.Text = currentDisease.overview;
                }
            }
        }

        public BDDiseaseControl()
        {
            InitializeComponent();
        }

        private void BDDiseaseControl_Load(object sender, EventArgs e)
        {
            if (currentDisease != null)
            {
                if(tbDiseaseName.Text != currentDisease.name) tbDiseaseName.Text = currentDisease.name;
                if(tbDiseaseOverview.Text != currentDisease.overview) tbDiseaseOverview.Text = currentDisease.overview;
            }
        }

        private void tbDiseaseName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
