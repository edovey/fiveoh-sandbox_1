using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BDEditor.Views
{
    public partial class BDTextControl : UserControl
    {
        public BDTextControl()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
    }
}
