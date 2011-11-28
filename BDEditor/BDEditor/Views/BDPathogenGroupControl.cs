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

        public void Delete()
        {
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
            this.SuspendLayout();

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
                List<BDTherapyGroup> list = BDTherapyGroup.getTherapyGroupsForPathogenGroupId(dataContext, currentPathogenGroup.uuid);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    BDTherapyGroup entry = list[idx];
                    addTherapyGroupControl(entry, idx);
                }

                pathogenSet1.CurrentPathogenGroup = currentPathogenGroup;
            }

            pathogenSet1.RefreshLayout();

            this.ResumeLayout();
        }

        #endregion    

        private BDTherapyGroupControl addTherapyGroupControl(BDTherapyGroup pTherapyGroup, int pTabIndex)
        {
            BDTherapyGroupControl therapyGroupControl = null;

            if (CreateCurrentObject())
            {
                therapyGroupControl = new BDTherapyGroupControl();

                therapyGroupControl.Dock = DockStyle.Top;
                therapyGroupControl.TabIndex = pTabIndex;
                therapyGroupControl.DisplayOrder = pTabIndex;
                therapyGroupControl.AssignParentId(currentPathogenGroup.uuid);
                therapyGroupControl.AssignScopeId(scopeId);
                therapyGroupControl.AssignDataContext(dataContext);
                therapyGroupControl.CurrentTherapyGroup = pTherapyGroup;
                therapyGroupControl.RequestItemAdd += new EventHandler(TherapyGroup_RequestItemAdd);
                therapyGroupControl.RequestItemDelete += new EventHandler(TherapyGroup_RequestItemDelete);
                therapyGroupControl.ReorderToNext += new EventHandler(TherapyGroup_ReorderToNext);
                therapyGroupControl.ReorderToPrevious += new EventHandler(TherapyGroup_ReorderToPrevious);
                therapyGroupControlList.Add(therapyGroupControl);

                panelTherapyGroups.Controls.Add(therapyGroupControl);
                therapyGroupControl.BringToFront();

                therapyGroupControl.RefreshLayout();
            }

            return therapyGroupControl;
        }

        private void removeTherapyGroupControl(BDTherapyGroupControl pTherapyGroupControl, bool pDeleteRecord)
        {
            panelTherapyGroups.Controls.Remove(pTherapyGroupControl);

            pTherapyGroupControl.RequestItemAdd -= new EventHandler(TherapyGroup_RequestItemAdd);
            pTherapyGroupControl.RequestItemDelete -= new EventHandler(TherapyGroup_RequestItemDelete);
            pTherapyGroupControl.ReorderToNext -= new EventHandler(TherapyGroup_ReorderToNext);
            pTherapyGroupControl.ReorderToPrevious -= new EventHandler(TherapyGroup_ReorderToPrevious);

            therapyGroupControlList.Remove(pTherapyGroupControl);

            if (pDeleteRecord)
            {
                BDTherapyGroup entry = pTherapyGroupControl.CurrentTherapyGroup;
                if (null != entry)
                {
                    BDTherapyGroup.Delete(dataContext, entry);

                    for (int idx = 0; idx < therapyGroupControlList.Count; idx++)
                    {
                        therapyGroupControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pTherapyGroupControl.Dispose();
            pTherapyGroupControl = null;
        }

        private void ReorderTherapyGroupControl(BDTherapyGroupControl pTherapyGroupControl, int pOffset)
        {
            int currentPosition = therapyGroupControlList.FindIndex(t => t == pTherapyGroupControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < therapyGroupControlList.Count))
                {
                    therapyGroupControlList[requestedPosition].CreateCurrentObject();
                    therapyGroupControlList[requestedPosition].DisplayOrder = currentPosition;

                    therapyGroupControlList[requestedPosition].CurrentTherapyGroup .displayOrder = currentPosition;
                    BDTherapyGroup.SaveTherapyGroup(dataContext, therapyGroupControlList[requestedPosition].CurrentTherapyGroup);

                    therapyGroupControlList[currentPosition].CreateCurrentObject();
                    therapyGroupControlList[currentPosition].DisplayOrder = requestedPosition;

                    therapyGroupControlList[currentPosition].CurrentTherapyGroup.displayOrder = requestedPosition;
                    BDTherapyGroup.SaveTherapyGroup(dataContext, therapyGroupControlList[currentPosition].CurrentTherapyGroup);

                    BDTherapyGroupControl temp = therapyGroupControlList[requestedPosition];
                    therapyGroupControlList[requestedPosition] = therapyGroupControlList[currentPosition];
                    therapyGroupControlList[currentPosition] = temp;

                    int zOrder = panelTherapyGroups.Controls.GetChildIndex(pTherapyGroupControl);
                    zOrder = zOrder + (pOffset * -1);
                    panelTherapyGroups.Controls.SetChildIndex(pTherapyGroupControl, zOrder);
                }
            }
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

        private void TherapyGroup_ReorderToNext(object sender, EventArgs e)
        {
            BDTherapyGroupControl control = sender as BDTherapyGroupControl;
            if (null != control)
            {
                ReorderTherapyGroupControl(control, 1);
            }
        }

        private void TherapyGroup_ReorderToPrevious(object sender, EventArgs e)
        {
            BDTherapyGroupControl control = sender as BDTherapyGroupControl;
            if (null != control)
            {
                ReorderTherapyGroupControl(control, -1);
            }
        }

        public override string ToString()
        {
            return (null == this.currentPathogenGroup) ? "No Pathogen Group" : this.currentPathogenGroup.uuid.ToString();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }
    }
}
