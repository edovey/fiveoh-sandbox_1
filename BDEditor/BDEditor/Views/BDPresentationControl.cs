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
    public partial class BDPresentationControl : UserControl
    {
        private BDPresentation currentPresentation;

        public BDPresentation CurrentPresentation
        {
            get
            {
                return currentPresentation;
            }
            set
            {
                currentPresentation = value;
                if (currentPresentation == null)
                {
                    tbPresentationName.Text = @"";
                }
                else
                {
                    tbPresentationName.Text = currentPresentation.name;
                }
            }
        }

        public BDPresentationControl()
        {
            InitializeComponent();
        }
    }
}
