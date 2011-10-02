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
    public partial class BDPathogenControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? pathogenGroupId;
        private BDPathogen currentPathogen;
        private string title = @"Pathogen";

        public BDPathogen CurrentPathogen
        {
            get 
            {
                return currentPathogen;
            }
            set 
            {
                currentPathogen = value;

                if (currentPathogen == null)
                {
                    this.BackColor = SystemColors.ControlDark;
                    tbPathogenName.Text = @"";
                }
                else
                {
                    this.BackColor = SystemColors.Control;
                    tbPathogenName.Text = currentPathogen.name;
                }
            }

        }

        /// <summary>
        /// The title that appears on this control
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                lblTitle.Text = title;
            }
        }

        public BDPathogenControl()
        {
            InitializeComponent();
        }

        #region IBDControl

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool Save()
        {
            bool result = false;

            if ((null == currentPathogen) && (tbPathogenName.Text != string.Empty))
            {
                currentPathogen = BDPathogen.CreatePathogen(dataContext);
                currentPathogen.pathogenGroupId = pathogenGroupId;
            }
            if (null != currentPathogen)
            {
                currentPathogen.name = tbPathogenName.Text;
                BDPathogen.SavePathogen(dataContext, currentPathogen);
                result = true;
            }

            return result;
        }

        public void AssignParentId(Guid? pParentId)
        {
            pathogenGroupId = pParentId;
            this.Enabled = (null != pathogenGroupId);
        }

        #endregion

        private void btnLink_Click(object sender, EventArgs e)
        {
            // open context menu for linking to: existing linked note, new linked note, remove link to note
            MessageBox.Show("Will show context menu for working with a linked note");
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (null != textBox)
            {
                this.BackColor = (textBox.Text.Trim() != string.Empty) ? SystemColors.Control : SystemColors.ControlDark;
            }
        }
    }
}
