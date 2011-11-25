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
    public partial class BDPathogenGroupControl : UserControl, IBDControl
    {
        #region Class properties
        private Entities dataContext;
        private Guid? presentationId;
        private BDPathogenGroup currentPathogenGroup;
        private Guid? scopeId;
        public int? DisplayOrder { get; set; }

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;

        private List<BDTherapyGroupControl> therapyGroupControlList = new List<BDTherapyGroupControl>();

        protected virtual void OnItemAddRequested(EventArgs e)
        {
            if (null != RequestItemAdd) { RequestItemAdd(this, e); }
        }

        protected virtual void OnItemDeleteRequested(EventArgs e)
        {
            if (null != RequestItemDelete) { RequestItemDelete(this, e); }
        }

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
        }

        #region IBDControl
        
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
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

                foreach (BDTherapyGroupControl control in therapyGroupControlList)
                {
                    result = control.Save() || result;
                }

                if (result && (null == currentPathogenGroup)) 
                {
                    CreateCurrentObject();
                }

                if (null != currentPathogenGroup)
                {
                    System.Diagnostics.Debug.WriteLine(@"PathogenGroup Control Save");

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
            for (int idx = 0; idx < therapyGroupControlList.Count; idx++)
            {
                BDTherapyGroupControl control = therapyGroupControlList[idx];
                removeTherapyGroupControl(control, false);
            }
            therapyGroupControlList.Clear();
            panelTherapyGroups.Controls.Clear();

            if (null == currentPathogenGroup)
            {
                pathogenSet1.CurrentPathogenGroup = null;
                pathogenSet1.AssignParentId(null);
            }
            else
            {
                List<BDTherapyGroup> therapyGroupList = BDTherapyGroup.getTherapyGroupsForPathogenGroupId(dataContext, currentPathogenGroup.uuid);
                for (int idx = 0; idx < therapyGroupList.Count; idx++)
                {
                    BDTherapyGroup entry = therapyGroupList[idx];
                    addTherapyGroupControl(entry, idx);
                }

                pathogenSet1.CurrentPathogenGroup = currentPathogenGroup;
            }

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

        private BDTherapyGroupControl addTherapyGroupControl(BDTherapyGroup pTherapyGroup, int pTabIndex)
        {
            BDTherapyGroupControl therapyGroupControl = null;

            if (CreateCurrentObject())
            {
                therapyGroupControl = new BDTherapyGroupControl();
                int bottom = 0;
                foreach (Control control in panelTherapyGroups.Controls)
                {
                    bottom += control.Height;
                }
                therapyGroupControl.TabIndex = pTabIndex;
                therapyGroupControl.Top = bottom;
                therapyGroupControl.Left = 0;
                therapyGroupControl.AssignParentId(currentPathogenGroup.uuid);
                therapyGroupControl.AssignScopeId(scopeId);
                therapyGroupControl.AssignDataContext(dataContext);
                therapyGroupControl.CurrentTherapyGroup = pTherapyGroup;
                therapyGroupControl.RequestItemAdd += new EventHandler(TherapyGroup_RequestItemAdd);
                therapyGroupControl.RequestItemDelete += new EventHandler(TherapyGroup_RequestItemDelete);
                panelTherapyGroups.Controls.Add(therapyGroupControl);
                panelTherapyGroups.Controls.SetChildIndex(therapyGroupControl, pTabIndex);

                therapyGroupControl.RefreshLayout();
            }

            return therapyGroupControl;
        }

        private void removeTherapyGroupControl(BDTherapyGroupControl pTherapyGroupControl, bool pDeleteRecord)
        {
            if (pDeleteRecord)
            {
                BDTherapyGroup entry = pTherapyGroupControl.CurrentTherapyGroup;
                if (null != entry)
                {
                    // call to BDDeletion
                }
            }

            pTherapyGroupControl.RequestItemAdd -= new EventHandler(TherapyGroup_RequestItemAdd);
            pTherapyGroupControl.RequestItemDelete -= new EventHandler(TherapyGroup_RequestItemDelete);
            panelTherapyGroups.Controls.Remove(pTherapyGroupControl);

            therapyGroupControlList.Remove(pTherapyGroupControl);
            pTherapyGroupControl.Dispose();
            pTherapyGroupControl = null;

            int top = 0;
            for (int idx = 0; idx < panelTherapyGroups.Controls.Count; idx++)
            {
                Control control = panelTherapyGroups.Controls[idx];
                control.Top = top;
                top += control.Height;
            }
        }

        private void resizeTherapyGroupControlPanelHeight()
        {
            this.Height = panelTherapyGroups.Top + panelTherapyGroups.Height;
        }

        private void PathogenGroup_RequestItemAdd(object sender, EventArgs e)
        {
            OnItemAddRequested(new EventArgs());
        }

        private void PathogenGroup_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
        }

        private void TherapyGroup_RequestItemAdd(object sender, EventArgs e)
        {
            BDTherapyGroupControl control = addTherapyGroupControl(null, therapyGroupControlList.Count);
            if (null != control)
            {
                resizeTherapyGroupControlPanelHeight();
                control.Focus();
            }
        }

        private void TherapyGroup_RequestItemDelete(object sender, EventArgs e)
        {
            BDTherapyGroupControl control = sender as BDTherapyGroupControl;
            if (null != control)
            {
                removeTherapyGroupControl(control, true);
            }
        }

        public override string ToString()
        {
            return (null == this.currentPathogenGroup) ? "No Pathogen Group" : this.currentPathogenGroup.uuid.ToString();
        }
    }
}
