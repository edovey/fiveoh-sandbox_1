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
        private string title;

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

        #endregion
    }
}
