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
    public partial class BDPresentationControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? diseaseId;
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
                    rtbPresentationOverview.Rtf = @"";

                    bdPathogenGroupControl1.CurrentPathogenGroup = null;
                    bdPathogenGroupControl1.AssignParentId(null);
                }
                else
                {
                    tbPresentationName.Text = currentPresentation.name;
                    BDLinkedNote linkedNote = BDLinkedNote.GetLinkedNoteForParentIdAndPropertyName(dataContext, currentPresentation.uuid, BDPresentation.OVERVIEW_NOTE);
                    if (linkedNote != null)
                        rtbPresentationOverview.Rtf = linkedNote.documentText;
                    List<BDPathogenGroup> pathogenGroupList = BDPathogenGroup.GetPathogenGroupsForPresentationId(dataContext, currentPresentation.uuid);
                    if (pathogenGroupList.Count <= 0)
                    {
                        bdPathogenGroupControl1.CurrentPathogenGroup = null;
                        bdPathogenGroupControl1.AssignParentId(currentPresentation.uuid);
                    }
                    else
                    {
                        bdPathogenGroupControl1.CurrentPathogenGroup = pathogenGroupList[0];
                        bdPathogenGroupControl1.AssignParentId(currentPresentation.uuid);
                    }
                }
            }
        }

        public BDPresentationControl()
        {
            InitializeComponent();
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdPathogenGroupControl1.AssignDataContext(dataContext);
        }

        public void AssignParentId(Guid? pParentId)
        {
            diseaseId = pParentId;

            this.Enabled = (null != diseaseId); 
        }

        public bool Save()
        {
            bool result = false;

            if (null != diseaseId)
            {
                if ((null == currentPresentation) && (tbPresentationName.Text != string.Empty))
                {
                    currentPresentation = BDPresentation.CreatePresentation(dataContext);
                    currentPresentation.diseaseId = diseaseId;
                }
                if (null != currentPresentation)
                {
                    if(currentPresentation.name != tbPresentationName.Text) currentPresentation.name = tbPresentationName.Text;

                    BDLinkedNote overviewNote;
                    BDLinkedNote linkedNote = BDLinkedNote.GetLinkedNoteForParentIdAndPropertyName(dataContext, currentPresentation.uuid, BDPresentation.OVERVIEW_NOTE);
                    if (linkedNote != null)
                    {
                        overviewNote = linkedNote;
                    }
                    else
                    {
                        overviewNote = BDLinkedNote.CreateLinkedNote(dataContext, currentPresentation.uuid, BDPresentation.OVERVIEW_NOTE);
                    }

                    if(overviewNote.documentText != rtbPresentationOverview.Rtf) overviewNote.documentText = rtbPresentationOverview.Rtf;
                    BDLinkedNote.SaveLinkedNote(dataContext, overviewNote);

                    System.Diagnostics.Debug.WriteLine(@"Presentation Control Save");
                    BDPresentation.SavePresentation(dataContext, currentPresentation);

                    bdPathogenGroupControl1.Save();
                }
            }
           
            return result;
        }


        public void AssignParentControl(IBDControl pControl)
        {
            throw new NotImplementedException();
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            throw new NotImplementedException();
        }

        private void BDPresentationControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                rtbPresentationOverview.Font = fontDialog1.Font;
            }
        }
    }
}
