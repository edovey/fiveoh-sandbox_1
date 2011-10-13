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
    public partial class BDDiseaseControl : UserControl, IBDControl
    {
        private BDDisease currentDisease;
        private Entities dataContext;
        private BDLinkedNote overviewLinkedNote;
        private Guid? parentId;

        public BDDisease CurrentDisease
        {
            get
            {
                return currentDisease;
            }
            set
            {
                currentDisease = value;
                if (currentDisease == null)
                {
                    tbDiseaseName.Text = @"";
                    overviewLinkedNote = null;

                    bdLinkedNoteControl1.AssignParentId(null);
                    bdLinkedNoteControl1.AssignScopeId(null);
                    bdLinkedNoteControl1.AssignParentControl(this);
                    bdLinkedNoteControl1.AssignContextEntityName(BDDisease.ENTITYNAME_FRIENDLY);
                    bdLinkedNoteControl1.AssignContextPropertyName(BDDisease.PROPERTYNAME_OVERVIEW);
                }
                else
                {
                    tbDiseaseName.Text = currentDisease.name;
                    bdLinkedNoteControl1.AssignParentId(currentDisease.uuid);
                    bdLinkedNoteControl1.AssignScopeId(currentDisease.uuid);
                    bdLinkedNoteControl1.AssignParentControl(this);
                    bdLinkedNoteControl1.AssignContextEntityName(BDDisease.ENTITYNAME_FRIENDLY);
                    bdLinkedNoteControl1.AssignContextPropertyName(BDDisease.PROPERTYNAME_OVERVIEW);

                    BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentDisease.uuid, BDDisease.PROPERTYNAME_OVERVIEW);
                    if (null != association)
                    {
                        overviewLinkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, association.linkedNoteId);
                        bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                    }
                }
            }
        }

        public BDDiseaseControl()
        {
            InitializeComponent();
        }

        private void BDDiseaseControl_Load(object sender, EventArgs e)
        {
            if (currentDisease != null)
            {
                if(tbDiseaseName.Text != currentDisease.name) tbDiseaseName.Text = currentDisease.name;
            }
        }

        private void tbDiseaseName_TextChanged(object sender, EventArgs e)
        {

        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdLinkedNoteControl1.AssignDataContext(dataContext);
        }

        public void AssignParentId(Guid? pParentId)
        {
            parentId = pParentId;
            this.Enabled = (null != parentId);
        }

        public bool Save()
        {
            bool result = false;

            if (null != parentId)
            {
                if ((null == currentDisease) && (tbDiseaseName.Text != string.Empty))
                {
                    currentDisease.subcategoryId = parentId;
                    currentDisease.categoryId = parentId;
                }

                if(null != currentDisease)
                {
                    if(currentDisease.name != tbDiseaseName.Text)
                        currentDisease.name = tbDiseaseName.Text;
                    bdLinkedNoteControl1.Save();
                }
                System.Diagnostics.Debug.WriteLine(@"Disease Control Save");
                BDDisease.SaveDisease(dataContext, currentDisease);
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
    }
}
