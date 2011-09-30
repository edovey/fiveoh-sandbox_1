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
        private BDPathogen currentPathogen;

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

        public BDPathogenControl()
        {
            InitializeComponent();
        }

        public void Save()
        {
            Entities context = new Entities();
            BDPathogen.SavePathogen(context, currentPathogen);
        }
    }
}
