using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.Classes;


//Some icons by Yusuke Kamiyamane. All rights reserved. Licensed under a Creative Commons Attribution 3.0 License.

namespace BDEditor.Views
{
    public partial class AuthenticationInputBox : Form
    {
        public AuthenticationInputBox()
        {
            InitializeComponent();
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            flagGreen.Visible = BDCommon.Settings.Validate(maskedTextBox1.Text);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!BDCommon.Settings.Authenticate(maskedTextBox1.Text))
            {
                MessageBox.Show("You may only retrieve data from the remote repository.", "Synchronization Permissions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!BDCommon.Settings.SyncPushEnabled)
            {
                MessageBox.Show("You may only retrieve data from the remote repository.", "Synchronization Permissions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void AuthenticationInputBox_Load(object sender, EventArgs e)
        {
            maskedTextBox1.Text = string.Empty;
            flagGreen.Visible = BDCommon.Settings.SyncPushEnabled;
        }
    }
}
