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
    public partial class BDPathogenGroupControl : UserControl, IBDControl
    {
        #region Class properties
        private Entities dataContext;
        private Guid? presentationId;
        private BDPathogenGroup currentPathogenGroup;
        private Guid? scopeId;
        public int? DisplayOrder { get; set; }

        public BDPathogenGroup CurrentPathogenGroup
        {
            get { return currentPathogenGroup; }
            set { currentPathogenGroup = value; }
        }

        #endregion

        public BDPathogenGroupControl()
        {
            InitializeComponent();
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
            pathogenSet1.AssignScopeId(scopeId);
            bdTherapyGroupControl1.AssignScopeId(scopeId);
            bdTherapyGroupControl2.AssignScopeId(scopeId);
        }

        #region IBDControl
        
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdTherapyGroupControl1.AssignDataContext(dataContext);
            bdTherapyGroupControl2.AssignDataContext(dataContext);
            pathogenSet1.AssignDataContext(dataContext);
            pathogenSet1.AssignTypeaheadSource();
        }

        public void AssignParentId(Guid? pParentId)
        {
            presentationId = pParentId;
            this.Enabled = (null != presentationId);
        }

        public bool Save()
        {
            bool result = false;

            if (null != presentationId)
            {
                result = pathogenSet1.Save() || result;

                if (result && (null == currentPathogenGroup)) // only create a group if any of the children exist
                {
                    CreateCurrentObject();
                }

                if (null != currentPathogenGroup)
                {
                    System.Diagnostics.Debug.WriteLine(@"PathogenGroup Control Save");
                    bdTherapyGroupControl1.Save();
                    bdTherapyGroupControl2.Save();
                    BDPathogenGroup.SavePathogenGroup(dataContext, currentPathogenGroup);
                    result = true;
                }
            }
            return result;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentPathogenGroup)
            {
                if (null == this.presentationId)
                {
                    result = false;
                }
                else
                {
                    this.currentPathogenGroup = BDPathogenGroup.CreatePathogenGroup(this.dataContext, this.presentationId.Value);
                    this.currentPathogenGroup.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            if (null == currentPathogenGroup)
            {
                bdTherapyGroupControl1.AssignParentId(null);
                bdTherapyGroupControl1.CurrentTherapyGroup = null;
                bdTherapyGroupControl1.DisplayOrder = 1;

                bdTherapyGroupControl2.AssignParentId(null);
                bdTherapyGroupControl2.CurrentTherapyGroup = null;
                bdTherapyGroupControl2.DisplayOrder = 2;

                pathogenSet1.CurrentPathogenGroup = null;
                pathogenSet1.AssignParentId(null);
            }
            else
            {
                bdTherapyGroupControl1.AssignParentId(currentPathogenGroup.uuid);
                bdTherapyGroupControl2.AssignParentId(currentPathogenGroup.uuid);
                List<BDTherapyGroup> therapyGroupList = BDTherapyGroup.getTherapyGroupsForPathogenGroupId(dataContext, currentPathogenGroup.uuid);

                bdTherapyGroupControl1.CurrentTherapyGroup = null;
                bdTherapyGroupControl2.CurrentTherapyGroup = null;

                if (therapyGroupList.Count >= 1)
                {
                    bdTherapyGroupControl1.CurrentTherapyGroup = therapyGroupList[0];
                    //bdTherapyGroupControl1.RefreshLayout();
                }
                if (therapyGroupList.Count >= 2)
                {
                    bdTherapyGroupControl2.CurrentTherapyGroup = therapyGroupList[1];
                    //bdTherapyGroupControl2.RefreshLayout();
                }

                pathogenSet1.CurrentPathogenGroup = currentPathogenGroup;
            }

            bdTherapyGroupControl1.RefreshLayout();
            bdTherapyGroupControl2.RefreshLayout();
            pathogenSet1.RefreshLayout();
        }

        /*
        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            if (null == currentPathogenGroup)
            {
                currentPathogenGroup = BDPathogenGroup.CreatePathogenGroup(dataContext);
                currentPathogenGroup.presentationId = presentationId;
                BDPathogenGroup.SavePathogenGroup(dataContext, currentPathogenGroup);
                pControl.AssignParentId(currentPathogenGroup.uuid);
                pControl.Save();
            }
            else
            {
                pControl.AssignParentId(currentPathogenGroup.uuid);
                pControl.Save();
            }
        }
        */

        #endregion    

    }
}
