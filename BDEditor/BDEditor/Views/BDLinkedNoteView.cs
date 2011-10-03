using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDLinkedNoteView : Form
    {
        Entities dataContext;
        
        public BDLinkedNoteView()
        {
            InitializeComponent();
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdLinkedNoteControl1.AssignDataContext(dataContext);
        }

        public void AssignContextPropertyName(string pContextPropertyName)
        {
            bdLinkedNoteControl1.AssignContextPropertyName(pContextPropertyName);
        }

        public void AssignParentId(Guid? pParentId)
        {
            bdLinkedNoteControl1.AssignParentId(pParentId);
        }

        public BDLinkedNote CurrentLinkNote
        {
            get
            {
                return bdLinkedNoteControl1.CurrentLinkedNote;
            }

            set
            {
                bdLinkedNoteControl1.CurrentLinkedNote = value;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bdLinkedNoteControl1.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
