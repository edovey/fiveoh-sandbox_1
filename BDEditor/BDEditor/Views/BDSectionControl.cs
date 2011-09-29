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
    public partial class BDSectionControl : UserControl
    {
        private BDSection currentSection;

        public BDSection CurrentSection
        {
            get
            {
                return currentSection;
            }

            set
            {
                currentSection = value;
                tbSectionName.Text = currentSection.name;
            }
        }

        public BDSectionControl()
        {
            InitializeComponent();
        }

        private void BDSectionControl_Load(object sender, EventArgs e)
        {
            if (currentSection != null)
                tbSectionName.Text = currentSection.name;
        }
    }
}
