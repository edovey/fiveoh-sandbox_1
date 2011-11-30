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
        public Guid? SubCategoryId { get; set; }
        public Guid? CategoryId { get; set; }
        public int? DisplayOrder { get; set; }

        public BDDisease CurrentDisease
        {
            get { return currentDisease; }
            set { currentDisease = value; }
        }

        public void RefreshLayout()
        {
            if (currentDisease == null)
            {
                tbDiseaseName.Text = @"";
                overviewLinkedNote = null;

                bdLinkedNoteControl1.CurrentLinkedNote = null;
                bdLinkedNoteControl1.AssignParentId(null);
                bdLinkedNoteControl1.AssignScopeId(null);
                bdLinkedNoteControl1.AssignContextEntityName(BDDisease.ENTITYNAME_FRIENDLY);
                bdLinkedNoteControl1.AssignContextPropertyName(BDDisease.PROPERTYNAME_OVERVIEW);
            }
            else
            {
                tbDiseaseName.Text = currentDisease.name;
                bdLinkedNoteControl1.AssignParentId(currentDisease.uuid);
                bdLinkedNoteControl1.AssignScopeId(currentDisease.uuid);
                bdLinkedNoteControl1.AssignContextEntityName(BDDisease.ENTITYNAME_FRIENDLY);
                bdLinkedNoteControl1.AssignContextPropertyName(BDDisease.PROPERTYNAME_OVERVIEW);

                BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentDisease.uuid, BDDisease.PROPERTYNAME_OVERVIEW);
                if (null != association)
                {
                    overviewLinkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, association.linkedNoteId);
                    bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                }
            }
            bdLinkedNoteControl1.RefreshLayout();
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

        #region IBDControl
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
                    CreateCurrentObject();
                }

                if(null != currentDisease)
                {
                    if(currentDisease.name != tbDiseaseName.Text) currentDisease.name = tbDiseaseName.Text;
                    bdLinkedNoteControl1.Save();
                }
                System.Diagnostics.Debug.WriteLine(@"Disease Control Save");
                BDDisease.Save(dataContext, currentDisease);
            }
            return result;
        }

        public void Delete()
        {
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentDisease)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    this.currentDisease = BDDisease.CreateDisease(this.dataContext);
                    this.currentDisease.subcategoryId = SubCategoryId;
                    this.currentDisease.categoryId = CategoryId;
                    this.currentDisease.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                }
            }

            return result;
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {

        }

        #endregion

        private void bdLinkedNoteControl_SaveAttemptWithoutParent(object sender, EventArgs e)
        {
            BDLinkedNoteControl control = sender as BDLinkedNoteControl;
            if (null != control)
            {
                if (CreateCurrentObject())
                {
                    control.AssignParentId(this.currentDisease.uuid);
                    control.Save();
                }
            }
        }
    }
}
