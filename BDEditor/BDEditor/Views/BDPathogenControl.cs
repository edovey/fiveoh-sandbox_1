﻿using System;
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
        private Guid? scopeId;
        public int? DisplayOrder { get; set; }

        public BDPathogen CurrentPathogen
        {
            get { return currentPathogen; }
            set { currentPathogen = value; }
        }

         #endregion

        public BDPathogenControl()
        {
            InitializeComponent();
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
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
                CreateCurrentObject();
            }
            if (null != currentPathogen)
            {
                if(currentPathogen.name != tbPathogenName.Text) currentPathogen.name = tbPathogenName.Text;
                BDPathogen.SavePathogen(dataContext, currentPathogen);
                result = true;
            }

            return result;
        }

        public void Delete()
        {
        }

        public void AssignParentId(Guid? pParentId)
        {
            pathogenGroupId = pParentId;
            this.Enabled = (null != pathogenGroupId);
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentPathogen)
            {
                if (null == this.pathogenGroupId)
                {
                    result = false;
                }
                else
                {
                    this.currentPathogen = BDPathogen.CreatePathogen(dataContext, pathogenGroupId.Value);
                    this.currentPathogen.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            if (currentPathogen == null)
            {
                this.tbPathogenName.Text = @"";
            }
            else
            {
                this.tbPathogenName.Text = currentPathogen.name;
            }
        }
        
        /*
        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            if (null == currentPathogen)
            {
                currentPathogen = BDPathogen.CreatePathogen(dataContext);
                currentPathogen.pathogenGroupId = pathogenGroupId;
                BDPathogen.SavePathogen(dataContext, currentPathogen);
                pControl.AssignParentId(currentPathogen.uuid);
                pControl.Save();
            }
            else
            {
                pControl.AssignParentId(currentPathogen.uuid);
                pControl.Save();
            }
        }
        */

        private void BDPathogenControl_Leave(object sender, EventArgs e)
        {
            Save();
        }        
        
        #endregion

        #region Class methods
        private void CreateLink()
        {
            CreateCurrentObject();
            Save();
            BDLinkedNoteView noteView = new BDLinkedNoteView();
            noteView.AssignDataContext(dataContext);
            noteView.AssignParentId(currentPathogen.uuid);
            noteView.AssignContextEntityName(BDPathogen.ENTITYNAME_FRIENDLY);
            noteView.AssignContextPropertyName(BDPathogen.PROPERTYNAME_NAME);
            noteView.AssignScopeId(scopeId);

            if (null != currentPathogen)
            {
                noteView.AssignParentId(currentPathogen.uuid);
            }
            else
            {
                noteView.AssignParentId(null);
            }

            noteView.PopulateControl();
            noteView.ShowDialog(this);
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource)
        {
            tbPathogenName.AutoCompleteCustomSource = pSource;
            tbPathogenName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbPathogenName.AutoCompleteSource = AutoCompleteSource.CustomSource;
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
            this.btnLink.Enabled = (null != textBox);
        }

        public override string ToString()
        {
            return (null == this.currentPathogen) ? "No Pathogen" : this.currentPathogen.name;
        }
        
    }
}
