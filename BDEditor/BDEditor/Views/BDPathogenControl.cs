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
        private BDPathogen currentPathogen;
        private string title = @"Pathogen";

        public BDPathogen CurrentPathogen
        {
            get {
                return currentPathogen;
            }
            set {
                currentPathogen = value;

                if(currentPathogen == null) {
                    tbPathogenName.Text = @"";
                }
                else
                    tbPathogenName.Text = currentPathogen.name;
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

        public void Save()
        {
            Entities context = new Entities();
            BDPathogen.SavePathogen(dataContext, currentPathogen);
        }

        public void AssignParentId(Guid pParentId)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void btnLink_Click(object sender, EventArgs e)
        {
            // open context menu for linking to: existing linked note, new linked note, remove link to note
            MessageBox.Show("Will show context menu for working with a linked note");
        }


    }
}
