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
        public int? DisplayOrder { get; set; }

        public BDPresentation CurrentPresentation
        {
            get { return currentPresentation; }
            set { currentPresentation = value; }
        }

        public BDPresentationControl()
        {
            InitializeComponent();
        }

        #region IBDControl

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
                    CreateCurrentObject();
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

        public void Delete()
        {
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentPresentation)
            {
                if (null == this.diseaseId)
                {
                    result = false;
                }
                else
                {
                    this.currentPresentation = BDPresentation.CreatePresentation(dataContext, diseaseId.Value);
                    this.currentPresentation.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            this.SuspendLayout();

            if (currentPresentation == null)
            {
                tbPresentationName.Text = @"";
                overviewLinkedNote = null;

                bdPathogenGroupControl1.CurrentPathogenGroup = null;
                bdPathogenGroupControl1.AssignParentId(null);
                bdPathogenGroupControl1.AssignScopeId(null);

                bdLinkedNoteControl1.AssignParentId(null);
                bdLinkedNoteControl1.AssignScopeId(null);
                bdLinkedNoteControl1.AssignContextEntityName(BDPresentation.ENTITYNAME_FRIENDLY);
                bdLinkedNoteControl1.AssignContextPropertyName(BDPresentation.PROPERTYNAME_OVERVIEW);
            }
            else
            {
                tbPresentationName.Text = currentPresentation.name;

                bdLinkedNoteControl1.AssignParentId(currentPresentation.uuid);
                bdLinkedNoteControl1.AssignScopeId(currentPresentation.uuid);
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

            bdPathogenGroupControl1.RefreshLayout();
            bdLinkedNoteControl1.RefreshLayout();
            this.ResumeLayout();
        }

        /*
        public void AssignParentControl(IBDControl pControl)
        {
            throw new NotImplementedException();
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            throw new NotImplementedException();
        }
        */
        #endregion

        private void BDPresentationControl_Leave(object sender, EventArgs e)
        {
            Save();
        }
    }
}
