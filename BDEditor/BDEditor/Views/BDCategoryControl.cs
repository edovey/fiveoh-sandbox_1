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
    public partial class BDCategoryControl : UserControl
    {
        private BDCategory currentCategory;

        public BDCategory CurrentCategory
        {
            get
            {
                return currentCategory;
            }
            set
            {
                currentCategory = value;
                if (currentCategory != null)
                    tbCategoryName.Text = currentCategory.name;
            }
        }

        public BDCategoryControl()
        {
            InitializeComponent();
        }

        private void BDCategoryControl_Load(object sender, EventArgs e)
        {
            if (currentCategory != null)
                tbCategoryName.Text = currentCategory.name;
        }
    }
}
