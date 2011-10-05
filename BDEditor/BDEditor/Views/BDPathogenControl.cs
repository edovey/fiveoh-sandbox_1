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
        #region Class properties

        private Entities dataContext;
        private Guid? pathogenGroupId;
        private BDPathogen currentPathogen;
        //private string title;
        private IBDControl parentControl;

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
                    //this.BackColor = SystemColors.ControlDark;
                    this.tbPathogenName.Text = @"";
                    //this.lblTitle.ForeColor = SystemColors.HotTrack;
                }
                else
                {
                    //this.BackColor = SystemColors.Control;
                    this.tbPathogenName.Text = currentPathogen.name;
                }
            }
        }

        /*
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                if (title != null && title.Length > 0)
                    lblTitle.Text = title;
                else lblTitle.Text = @"Pathogen";
            }
        }
        */
         #endregion

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
                if(currentPathogen.name != tbPathogenName.Text) currentPathogen.name = tbPathogenName.Text;
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

        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            if(null == currentPathogen)
            {
                currentPathogen = BDPathogen.CreatePathogen(dataContext);
                currentPathogen.pathogenGroupId = pathogenGroupId;
                BDPathogen.SavePathogen(dataContext, currentPathogen);
                pControl.AssignParentId(currentPathogen.uuid);
                pControl.Save();

                //this.BackColor = SystemColors.Control;
            }
        }

        private void BDPathogenControl_Leave(object sender, EventArgs e)
        {
            Save();
        }        
        
        #endregion

        #region Class methods
        private void CreateLink()
        {
            BDLinkedNoteView noteView = new BDLinkedNoteView();
            noteView.AssignDataContext(dataContext);
            noteView.AssignParentId(currentPathogen.uuid);
            noteView.AssignParentControl(this);
            //noteView.AssignContextPropertyName(@"

            if (null != currentPathogen)
            {
                BDLinkedNote note = BDLinkedNote.GetLinkedNoteForParentId(dataContext, currentPathogen.uuid);
                if (note != null)
                    noteView.CurrentLinkNote = note;
                else
                    noteView.CurrentLinkNote = null;
            }
            else{
                noteView.CurrentLinkNote = null;
            }

            noteView.ShowDialog(this);
        }

        #endregion

        private void btnLink_Click(object sender, EventArgs e)
        {
            if(this.Enabled)
                CreateLink();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (null != textBox)
            {
                //this.BackColor = (textBox.Text.Trim() != string.Empty) ? SystemColors.Control : SystemColors.ControlDark;
                this.btnLink.Enabled = true;
                if (null == currentPathogen)
                {
                    currentPathogen = BDPathogen.CreatePathogen(dataContext);
                    currentPathogen.pathogenGroupId = pathogenGroupId;
                    BDPathogen.SavePathogen(dataContext, currentPathogen);
                }
            }
            else this.btnLink.Enabled = false;
        }
    }
}
