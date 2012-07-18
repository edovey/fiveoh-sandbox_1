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
    public partial class BDArchiveDialog : Form
    {
        string _username = string.Empty;
        string _comment = string.Empty;

        public BDArchiveDialog()
        {
            InitializeComponent();
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Comment
        {
            get { return _comment; }
        }

        private void textChanged(object sender, EventArgs e)
        {
            btnArchive.Enabled = ((txtName.Text.Trim() != string.Empty) && (txtComment.Text.Trim() != string.Empty)); 
        }

        private void btnArchive_Click(object sender, EventArgs e)
        {
            _comment = txtComment.Text;
            _username = txtName.Text;
            //this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _comment = string.Empty;
            //this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

    }
}
