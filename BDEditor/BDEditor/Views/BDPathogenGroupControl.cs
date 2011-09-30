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
    public partial class BDPathogenGroupControl : UserControl, IBDControl
    {
        private BDPathogenGroup currentPathogenGroup;

        public BDPathogenGroup CurrentPathogenGroup
        {
            get
            {
                return currentPathogenGroup;
            }
            set
            {
                currentPathogenGroup = value;

                

            }
        }

        public BDPathogenGroupControl()
        {
            InitializeComponent();
        }

        private void BDPathogenGroupControl_Load(object sender, EventArgs e)
        {

        }

        public void Save()
        {

        }
    }
}
