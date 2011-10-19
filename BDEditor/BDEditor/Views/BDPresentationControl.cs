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
        private BDLinkedNote overviewLinkedNote;
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
                    overviewLinkedNote = null;

                    bdPathogenGroupControl1.CurrentPathogenGroup = null;
                    bdPathogenGroupControl1.AssignParentId(null);
                    bdPathogenGroupControl1.AssignScopeId(null);

                    bdLinkedNoteControl1.AssignParentId(null);
                    bdLinkedNoteControl1.AssignScopeId(null);
                    bdLinkedNoteControl1.AssignParentControl(this);
                    bdLinkedNoteControl1.AssignContextEntityName(BDPresentation.ENTITYNAME_FRIENDLY);
                    bdLinkedNoteControl1.AssignContextPropertyName(BDPresentation.PROPERTYNAME_OVERVIEW);
                }
                else
                {
                    tbPresentationName.Text = currentPresentation.name;

                    bdLinkedNoteControl1.AssignParentId(currentPresentation.uuid);
                    bdLinkedNoteControl1.AssignScopeId(currentPresentation.uuid);
                    bdLinkedNoteControl1.AssignParentControl(this);
                    bdLinkedNoteControl1.AssignContextEntityName(BDPresentation.ENTITYNAME_FRIENDLY);
                    bdLinkedNoteControl1.AssignContextPropertyName(BDPresentation.PROPERTYNAME_OVERVIEW);

                    
                    BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentPresentation.uuid, BDPresentation.PROPERTYNAME_OVERVIEW);
                    if (null != association)
                    {
                        overviewLinkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, association.linkedNoteId);
                        bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                    }
                    
                    bdPathogenGroupControl1.AssignScopeId(currentPresentation.uuid);

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
            bdLinkedNoteControl1.AssignDataContext(dataContext);
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
                    bdLinkedNoteControl1.Save();
                  
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

    }
}
