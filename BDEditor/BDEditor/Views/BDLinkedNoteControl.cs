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
    public partial class BDLinkedNoteControl : UserControl
    {
        private BDLinkedNote currentLinkedNote;

        public BDLinkedNote CurrentLinkedNote
        {
            get
            {
                return currentLinkedNote;
            }
            set
            {
                currentLinkedNote = value;
                if (currentLinkedNote == null)
                    tbLinkedNote.Text = @"";
                else
                    tbLinkedNote.Text = currentLinkedNote.documentText;
            }
        }

        public BDLinkedNoteControl()
        {
            InitializeComponent();
        }

    }
}
