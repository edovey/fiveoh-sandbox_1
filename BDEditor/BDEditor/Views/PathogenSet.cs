
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
    public partial class PathogenSet : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? pathogenGroupId;
        private BDPathogenGroup currentPathogenGroup;
        private Guid? scopeId;
        private AutoCompleteStringCollection pathogenNameCollection;

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;

        private List<BDPathogenControl> pathogenControlList = new List<BDPathogenControl>();

        protected virtual void OnItemAddRequested(EventArgs e)
        {
            if (null != RequestItemAdd) { RequestItemAdd(this, e); }
        }

        protected virtual void OnItemDeleteRequested(EventArgs e)
        {
            if (null != RequestItemDelete) { RequestItemDelete(this, e); }
        }

        public PathogenSet()
        {
            InitializeComponent();

            //string[] pathogenNameList = BDPathogen.GetPathogenNames(dataContext);
            //AutoCompleteStringCollection nameCollection = new AutoCompleteStringCollection();
            //nameCollection.AddRange(pathogenNameList);
        }

        public BDPathogenGroup CurrentPathogenGroup
        {
            get {  return currentPathogenGroup; }
            set { currentPathogenGroup = value; }
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
            bdPathogenControl1.AssignScopeId(scopeId);
            //bdPathogenControl2.AssignScopeId(scopeId);
            //bdPathogenControl3.AssignScopeId(scopeId);
            //bdPathogenControl4.AssignScopeId(scopeId);
            //bdPathogenControl5.AssignScopeId(scopeId);
            //bdPathogenControl6.AssignScopeId(scopeId);
            //bdPathogenControl7.AssignScopeId(scopeId);
            //bdPathogenControl8.AssignScopeId(scopeId);
        }

        #region IBDControl
        
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignParentId(Guid? pParentId)
        {
            pathogenGroupId = pParentId;
        }

        public bool Save()
        {
            bool result = false;

            if (null != pathogenGroupId)
            {
                foreach (BDPathogenControl control in pathogenControlList)
                {
                    result = control.Save() || result;
                }               
            }
            return result;
        }

        public void Delete()
        {
        }

        public bool CreateCurrentObject()
        {
            throw new NotImplementedException();
        }

        public void RefreshLayout()
        {
            for (int idx = 0; idx < pathogenControlList.Count; idx++)
            {
                BDPathogenControl control = pathogenControlList[idx];
                removePathogenControl(control, false);
            }
            pathogenControlList.Clear();

            if (null == currentPathogenGroup)
            {
                throw new NotImplementedException();
            }
            else
            {
                List<BDPathogen> list = BDPathogen.GetPathogensForPathogenGroup(dataContext, currentPathogenGroup.uuid);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    BDPathogen entry = list[idx];
                    addPathogenControl(entry, idx);
                }
            }
        }

        #endregion    
    
        private BDPathogenControl addPathogenControl(BDPathogen pPathogen, int pTabIndex)
        {
            BDPathogenControl pathogenControl = null;

            if (CreateCurrentObject())
            {
                pathogenControl = new BDPathogenControl();

                pathogenControl.Dock = DockStyle.Top;
                pathogenControl.TabIndex = pTabIndex;
                pathogenControl.DisplayOrder = pTabIndex;
                pathogenControl.AssignParentId(currentPathogenGroup.uuid);
                pathogenControl.AssignDataContext(dataContext);
                pathogenControl.AssignScopeId(scopeId);
                pathogenControl.AssignTypeaheadSource(pathogenNameCollection);
                pathogenControl.CurrentPathogen = pPathogen;
                pathogenControl.RequestItemDelete += new EventHandler(Pathogen_RequestItemDelete);
                pathogenControl.ReorderToNext += new EventHandler(Pathogen_ReorderToNext);
                pathogenControl.ReorderToPrevious += new EventHandler(Pathogen_ReorderToPrevious);
                pathogenControlList.Add(pathogenControl);

                this.Controls.Add(pathogenControl);
                pathogenControl.BringToFront();
                pathogenControl.RefreshLayout();
            }
            return pathogenControl;
        }

        private void removePathogenControl(BDPathogenControl pPathogenControl, bool pDeleteRecord)
        {
            this.Controls.Remove(pPathogenControl);

            pPathogenControl.RequestItemDelete -= new EventHandler(Pathogen_RequestItemDelete);
            pPathogenControl.ReorderToNext -= new EventHandler(Pathogen_ReorderToNext);
            pPathogenControl.ReorderToPrevious -= new EventHandler(Pathogen_ReorderToPrevious);

            pathogenControlList.Remove(pPathogenControl);

            if (pDeleteRecord)
            {
                BDPathogen entry = pPathogenControl.CurrentPathogen;
                if (null != entry)
                {
                    BDPathogen.Delete(dataContext, entry);

                    for (int idx = 0; idx < pathogenControlList.Count; idx++)
                    {
                        pathogenControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pPathogenControl.Dispose();
            pPathogenControl = null;
        }

        private void ReorderPathogenControl(BDPathogenControl pPathogenControl, int pOffset)
        {
            int currentPosition = pathogenControlList.FindIndex(t => t == pPathogenControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition += pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < pathogenControlList.Count))
                {
                    pathogenControlList[requestedPosition].CreateCurrentObject();
                    pathogenControlList[requestedPosition].DisplayOrder = currentPosition;
                    pathogenControlList[requestedPosition].CurrentPathogen.displayOrder = currentPosition;
                    BDPathogen.SavePathogen(dataContext, pathogenControlList[requestedPosition].CurrentPathogen);

                    pathogenControlList[currentPosition].CreateCurrentObject();
                    pathogenControlList[currentPosition].DisplayOrder = requestedPosition;
                    pathogenControlList[currentPosition].CurrentPathogen.displayOrder = requestedPosition;
                    BDPathogen.SavePathogen(dataContext, pathogenControlList[currentPosition].CurrentPathogen);

                    BDPathogenControl temp = pathogenControlList[requestedPosition];
                    pathogenControlList[requestedPosition] = pathogenControlList[currentPosition];
                    pathogenControlList[currentPosition] = temp;

                    int zOrder = this.Controls.GetChildIndex(pPathogenControl);
                    zOrder = zOrder + (pOffset * -1);
                    this.Controls.SetChildIndex(pPathogenControl, zOrder);
                }
            }
        }

        private void Pathogen_RequestItemDelete(object sender, EventArgs e)
        {
            BDPathogenControl control = sender as BDPathogenControl;
            if (null != control)
            {
                removePathogenControl(control, true);
            }
        }

        private void Pathogen_ReorderToNext(object sender, EventArgs e)
        {
            BDPathogenControl control = sender as BDPathogenControl;
            if (null != control)
            {
                ReorderPathogenControl(control, 1);
            }
        }

        private void Pathogen_ReorderToPrevious(object sender, EventArgs e)
        {
            BDPathogenControl control = sender as BDPathogenControl;
            if (null != control)
            {
                ReorderPathogenControl(control, -1);
            }
        }
    }    
}
