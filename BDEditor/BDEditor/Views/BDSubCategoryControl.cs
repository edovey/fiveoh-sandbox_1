using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;

namespace BDEditor.DataModel
{
    public partial class BDSubCategoryControl : UserControl
    {
        private BDSubcategory currentSubcategory;

        public BDSubcategory CurrentSubcategory
        {
            get
            {
                return currentSubcategory;
            }
            set
            {
                currentSubcategory = value;
                if (currentSubcategory == null)
                    tbSubcategoryName.Text = @"";
                else
                    tbSubcategoryName.Text = currentSubcategory.name;
            }
        }
        public BDSubCategoryControl()
        {
            InitializeComponent();
        }

        private void BDSubCategoryControl_Load(object sender, EventArgs e)
        {
            if (currentSubcategory != null)
                tbSubcategoryName.Text = currentSubcategory.name;
        }
    }
}
