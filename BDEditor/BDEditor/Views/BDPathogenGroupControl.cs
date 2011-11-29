﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;

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
        private AutoCompleteStringCollection pathogenNameCollection;

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;
        public event EventHandler ReorderToPrevious;
        public event EventHandler ReorderToNext;

        private List<BDPathogenControl> pathogenControlList = new List<BDPathogenControl>();
        private List<BDTherapyGroupControl> therapyGroupControlList = new List<BDTherapyGroupControl>();

        protected virtual void OnItemAddRequested(EventArgs e)
        {
            if (null != RequestItemAdd) { RequestItemAdd(this, e); }
        }

        protected virtual void OnItemDeleteRequested(EventArgs e)
        {
            if (null != RequestItemDelete) { RequestItemDelete(this, e); }
        }

        protected virtual void OnReorderToPrevious(EventArgs e)
        {
            if (null != ReorderToPrevious) { ReorderToPrevious(this, e); }
        }

        protected virtual void OnReorderToNext(EventArgs e)
        {
            if (null != ReorderToNext) { ReorderToNext(this, e); }
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
            btnPathogenGroupLink.Tag = BDPathogenGroup.PROPERTYNAME_DEFAULT;
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
               foreach (BDPathogenControl control in pathogenControlList)
                {
                    result = control.Save() || result;
                }     

                foreach (BDTherapyGroupControl control in therapyGroupControlList)
                {
                    result = control.Save() || result;
                }

                // if zero pathogens are defined then this is a valid test
                if ((result && (null == currentPathogenGroup) || (null == currentPathogenGroup) && textBoxPathogenGroupName.Text != string.Empty)) 
                {
                    CreateCurrentObject();
                }

                if (null != currentPathogenGroup)
                {
                    System.Diagnostics.Debug.WriteLine(@"PathogenGroup Control Save");

                    if (currentPathogenGroup.name != textBoxPathogenGroupName.Text) currentPathogenGroup.name = textBoxPathogenGroupName.Text;
                    if (currentPathogenGroup.displayOrder != DisplayOrder) currentPathogenGroup.displayOrder = DisplayOrder;

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

            //for (int idx = 0; idx < pathogenControlList.Count; idx++)
            //{
            //    BDPathogenControl control = pathogenControlList[idx];
            //    removePathogenControl(control, false);
            //}
            pathogenControlList.Clear();

            if (null == currentPathogenGroup)
            {
                textBoxPathogenGroupName.Text = @"";
            }
            else
            {
                textBoxPathogenGroupName.Text = currentPathogenGroup.name;
                List<BDPathogen> list = BDPathogen.GetPathogensForPathogenGroup(dataContext, currentPathogenGroup.uuid);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    BDPathogen entry = list[idx];
                    addPathogenControl(entry, idx);
                }
            }

            for (int idx = 0; idx < therapyGroupControlList.Count; idx++)
            {
                BDTherapyGroupControl control = therapyGroupControlList[idx];
                removeTherapyGroupControl(control, false);
            }
            therapyGroupControlList.Clear();
            panelTherapyGroups.Controls.Clear();

            if (null != currentPathogenGroup)
            {
                List<BDTherapyGroup> list = BDTherapyGroup.getTherapyGroupsForPathogenGroupId(dataContext, currentPathogenGroup.uuid);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    BDTherapyGroup entry = list[idx];
                    addTherapyGroupControl(entry, idx);
                }
            }

            showLinksInUse();

            this.ResumeLayout();
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
                pathogenControl.RequestItemAdd += new EventHandler(Pathogen_RequestItemAdd);
                pathogenControl.RequestItemDelete += new EventHandler(Pathogen_RequestItemDelete);
                pathogenControl.ReorderToNext += new EventHandler(Pathogen_ReorderToNext);
                pathogenControl.ReorderToPrevious += new EventHandler(Pathogen_ReorderToPrevious);
                pathogenControlList.Add(pathogenControl);

                panelPathogens.Controls.Add(pathogenControl);
                pathogenControl.BringToFront();
                pathogenControl.RefreshLayout();
            }
            return pathogenControl;
        }

        private void removePathogenControl(BDPathogenControl pPathogenControl, bool pDeleteRecord)
        {
            this.Controls.Remove(pPathogenControl);

            pPathogenControl.RequestItemAdd -= new EventHandler(Pathogen_RequestItemAdd);
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

                    int zOrder = panelPathogens.Controls.GetChildIndex(pPathogenControl);
                    zOrder = zOrder + (pOffset * -1);
                    panelPathogens.Controls.SetChildIndex(pPathogenControl, zOrder);
                }
            }
        }



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

        private void PathogenGroup_ReorderToPrevious(object sender, EventArgs e)
        {
            OnReorderToPrevious(new EventArgs());
        }

        private void PathogenGroup_ReorderToNext(object sender, EventArgs e)
        {
            OnReorderToNext(new EventArgs());
        }

        private void Pathogen_RequestItemAdd(object sender, EventArgs e)
        {
            BDPathogenControl control = addPathogenControl(null, pathogenControlList.Count);
        }

        private void Pathogen_RequestItemDelete(object sender, EventArgs e)
        {
            BDPathogenControl control = sender as BDPathogenControl;
            if (null != control)
                removePathogenControl(control, true);
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

        private void btnLink_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                CreateLink(control.Tag as string);
            }
        }

        private void CreateLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();
                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignContextEntityName(BDPathogenGroup.ENTITYNAME_FRIENDLY);
                view.AssignScopeId(scopeId);

                if (null != currentPathogenGroup)
                {
                    view.AssignParentId(currentPathogenGroup.uuid);
                }
                else
                {
                    view.AssignParentId(null);
                }
                view.PopulateControl();
                view.ShowDialog(this);
                showLinksInUse();
            }
        }

        private void showLinksInUse()
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentId(dataContext, (null != this.currentPathogenGroup) ? this.currentPathogenGroup.uuid : Guid.Empty);
            btnPathogenGroupLink.BackColor = links.Exists(x => x.parentEntityPropertyName == (string)btnPathogenGroupLink.Tag) ? Constants.ACTIVELINK_COLOR : Constants.INACTIVELINK_COLOR;
        }   

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }
        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new EventArgs());
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new EventArgs());
        }

    }
}
