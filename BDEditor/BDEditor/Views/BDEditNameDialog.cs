using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BDEditor.Views
{
    public partial class BDEditNameDialog : Form
    {
        public BDEditNameDialog()
        {
            InitializeComponent();
        }

        public string IndexEntryName
        {
            set { tbEntryName.Text = value; }
            get { return tbEntryName.Text; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult =  DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
