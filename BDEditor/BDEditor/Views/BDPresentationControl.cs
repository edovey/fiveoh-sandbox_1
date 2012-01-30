using System;
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
    public partial class BDPresentationControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? parentId;
        private BDNode currentPresentation;
        private BDLinkedNote overviewLinkedNote;
        private Guid? scopeId;
        private Constants.LayoutVariantType defaultLayoutVariantType;

        public int? DisplayOrder { get; set; }

        private List<BDPathogenGroupControl> pathogenGroupControlList = new List<BDPathogenGroupControl>();

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;
        public event EventHandler ReorderToPrevious;
        public event EventHandler ReorderToNext;

        public event EventHandler NotesChanged;

        protected virtual void OnNotesChanged(EventArgs e)
        {
            if (null != NotesChanged) { NotesChanged(this, e); }
        }

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

        public BDNode CurrentPresentation
        {
            get { return currentPresentation; }
            set { currentPresentation = value; }
        }

        public BDPresentationControl()
        {
            InitializeComponent();
            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
        }

        public BDPresentationControl(Entities pDataContext, BDNode pNode)
        {
            InitializeComponent();
            dataContext = pDataContext;
            defaultLayoutVariantType = pNode.LayoutVariant;
            
            currentPresentation = pNode;

            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
        }

        public BDPresentationControl(Entities pDataContext, Constants.LayoutVariantType pDefaultLayoutVariantType)
        {
            InitializeComponent();
            dataContext = pDataContext;
            defaultLayoutVariantType = pDefaultLayoutVariantType;

            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
        }

        private void BDPresentationControl_Load(object sender, EventArgs e)
        {
            bdLinkedNoteControl1.AssignDataContext(dataContext);
            bdLinkedNoteControl1.AssignContextNodeType(Constants.BDNodeType.BDPresentation);
            bdLinkedNoteControl1.AssignScopeId(scopeId);
            bdLinkedNoteControl1.AssignParentId(currentPresentation.Uuid); // This expects that currentPresentation is never null
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        #region IBDControl

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
                if ((null == currentPresentation) && (tbPresentationName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }
                if (null != currentPresentation)
                {
                    if(currentPresentation.name != tbPresentationName.Text) currentPresentation.name = tbPresentationName.Text;
                    bdLinkedNoteControl1.Save();

                    foreach (BDPathogenGroupControl control in pathogenGroupControlList)
                    {
                        result = control.Save() || result;
                    }
                  
                    System.Diagnostics.Debug.WriteLine(@"Presentation Control Save");
                    BDNode.Save(dataContext, currentPresentation);
                }
            }
           
            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentPresentation)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    BDMetadata parentMetaData = BDMetadata.GetMetadataWithItemId(dataContext, parentId);
                    this.currentPresentation = BDNode.CreateNode(dataContext, Constants.BDNodeType.BDPresentation);
                    this.currentPresentation.SetParent(Constants.BDNodeType.BDDisease, parentId.Value);
                    this.currentPresentation.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentPresentation.LayoutVariant = defaultLayoutVariantType;
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            this.SuspendLayout();

            if (null == currentPresentation)
            {
                tbPresentationName.Text = @"";
                overviewLinkedNote = null;

                bdLinkedNoteControl1.AssignParentId(null);
                bdLinkedNoteControl1.AssignScopeId(null);
                bdLinkedNoteControl1.AssignContextNodeType(currentPresentation.NodeType);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
            }
            else
            {
                tbPresentationName.Text = currentPresentation.name;
                List<IBDNode> list = BDFabrik.GetChildrenForParentId(dataContext, currentPresentation.uuid);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    IBDNode listEntry = list[idx];
                    if (listEntry.NodeType == (Constants.BDNodeType.BDPathogenGroup))
                    {
                        BDNode node = listEntry as BDNode;
                        addPathogenGroupControl(node, idx);
                    }
                }

                bdLinkedNoteControl1.AssignParentId(currentPresentation.uuid);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextNodeType(currentPresentation.NodeType);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);


                BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentPresentation.uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
                if (null != association)
                {
                    overviewLinkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, association.linkedNoteId);
                    bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                }
            }

            bdLinkedNoteControl1.RefreshLayout();

            ShowLinksInUse(false);
            this.ResumeLayout();
        }

        #endregion

        private void BDPresentationControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private BDPathogenGroupControl addPathogenGroupControl(BDNode pNode, int pTabIndex)
        {
            BDPathogenGroupControl pathogenGroupControl = null;
            if (CreateCurrentObject())
            {
                pathogenGroupControl = new BDPathogenGroupControl();

                pathogenGroupControl.Dock = DockStyle.Top;
                pathogenGroupControl.TabIndex = pTabIndex;
                pathogenGroupControl.DisplayOrder = pTabIndex;
                pathogenGroupControl.AssignParentId(currentPresentation.uuid);
                pathogenGroupControl.AssignDataContext(dataContext);
                pathogenGroupControl.AssignScopeId(scopeId);
                pathogenGroupControl.AssignTypeaheadSource(Typeahead.PathogenGroups);
                pathogenGroupControl.CurrentPathogenGroup = pNode;
                pathogenGroupControl.DefaultLayoutVariantType = this.defaultLayoutVariantType;

                pathogenGroupControl.RequestItemAdd += new EventHandler(PathogenGroup_RequestItemAdd);
                pathogenGroupControl.RequestItemDelete += new EventHandler(PathogenGroup_RequestItemDelete);
                pathogenGroupControl.ReorderToNext += new EventHandler(PathogenGroup_ReorderToNext);
                pathogenGroupControl.ReorderToPrevious += new EventHandler(PathogenGroup_ReorderToPrevious);
                pathogenGroupControl.NotesChanged += new EventHandler(notesChanged_Action);

                pathogenGroupControlList.Add(pathogenGroupControl);

                panelPathogenGroups.Controls.Add(pathogenGroupControl);
                pathogenGroupControl.BringToFront();
                pathogenGroupControl.RefreshLayout();
            }
            return pathogenGroupControl;
        }

        private void removePathogenGroupControl(BDPathogenGroupControl pPathogenGroupControl, bool pDeleteRecord)
        {
            panelPathogenGroups.Controls.Remove(pPathogenGroupControl);

            pPathogenGroupControl.RequestItemAdd -= new EventHandler(PathogenGroup_RequestItemAdd);
            pPathogenGroupControl.RequestItemDelete -= new EventHandler(PathogenGroup_RequestItemDelete);
            pPathogenGroupControl.ReorderToNext -= new EventHandler(PathogenGroup_ReorderToNext);
            pPathogenGroupControl.ReorderToPrevious -= new EventHandler(PathogenGroup_ReorderToPrevious);
            pPathogenGroupControl.NotesChanged -= new EventHandler(notesChanged_Action);

            pathogenGroupControlList.Remove(pPathogenGroupControl);
            
            if (pDeleteRecord)
            {
                BDNode entry = pPathogenGroupControl.CurrentPathogenGroup;
                if (null != entry)
                {
                    BDNode.Delete(dataContext, entry);
                    for (int idx = 0; idx < pathogenGroupControlList.Count; idx++)
                    {
                        pathogenGroupControlList[idx].DisplayOrder = idx;
                    }
                }
            }
            pPathogenGroupControl.Dispose();
            pPathogenGroupControl = null;
        }

        private void ReorderPathogenGroupControl(BDPathogenGroupControl pPathogenGroupControl, int pOffset)
        {
            int currentPosition = pathogenGroupControlList.FindIndex(t => t == pPathogenGroupControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < pathogenGroupControlList.Count))
                {
                    pathogenGroupControlList[requestedPosition].CreateCurrentObject();
                    pathogenGroupControlList[requestedPosition].DisplayOrder = currentPosition;

                    pathogenGroupControlList[requestedPosition].CurrentPathogenGroup.displayOrder = currentPosition;
                    BDNode.Save(dataContext, pathogenGroupControlList[requestedPosition].CurrentPathogenGroup);

                    pathogenGroupControlList[currentPosition].CreateCurrentObject();
                    pathogenGroupControlList[currentPosition].DisplayOrder = requestedPosition;

                    pathogenGroupControlList[currentPosition].CurrentPathogenGroup.displayOrder = requestedPosition;
                    BDNode.Save(dataContext, pathogenGroupControlList[currentPosition].CurrentPathogenGroup);

                    BDPathogenGroupControl temp = pathogenGroupControlList[requestedPosition];
                    pathogenGroupControlList[requestedPosition] = pathogenGroupControlList[currentPosition];
                    pathogenGroupControlList[currentPosition] = temp;

                    int zOrder = panelPathogenGroups.Controls.GetChildIndex(pPathogenGroupControl);
                    zOrder = zOrder + (pOffset * -1);
                    panelPathogenGroups.Controls.SetChildIndex(pPathogenGroupControl, zOrder);
                }
            }
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentPresentation) ? this.currentPresentation.uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? Constants.ACTIVELINK_COLOR : Constants.INACTIVELINK_COLOR;
            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < pathogenGroupControlList.Count; idx++)
                {
                    pathogenGroupControlList[idx].ShowLinksInUse(true);
                }
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
                view.AssignContextEntityNodeType(Constants.BDNodeType.BDPresentation);
                view.AssignScopeId(scopeId);
                view.AssignLinkedNoteType(LinkedNoteType.Footnote, true, true);
                view.NotesChanged += new EventHandler(notesChanged_Action);
                if (null != currentPresentation)
                {
                    view.AssignParentId(currentPresentation.uuid);
                }
                else
                {
                    view.AssignParentId(null);
                }
                view.PopulateControl();
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private void PathogenGroup_RequestItemAdd(object sender, EventArgs e)
        {
            BDPathogenGroupControl control = addPathogenGroupControl(null, pathogenGroupControlList.Count);
            if (null != control)
            {
                control.Focus();
            }
        }

        private void PathogenGroup_RequestItemDelete(object sender, EventArgs e)
        {
            BDPathogenGroupControl control = sender as BDPathogenGroupControl;
            if (null != control)
            {
                if (MessageBox.Show("Delete Pathogen Group?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removePathogenGroupControl(control, true);
            }
        }

        private void PathogenGroup_ReorderToNext(object sender, EventArgs e)
        {
            BDPathogenGroupControl control = sender as BDPathogenGroupControl;
            if (null != control)
            {
                ReorderPathogenGroupControl(control, 1);
            }
        }

        private void PathogenGroup_ReorderToPrevious(object sender, EventArgs e)
        {
            BDPathogenGroupControl control = sender as BDPathogenGroupControl;
            if (null != control)
            {
                ReorderPathogenGroupControl(control, -1);
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void notesChanged_Action(object sender, EventArgs e)
        {
            ShowLinksInUse(true);
            OnNotesChanged(new EventArgs());
        }

        private void btnLinkedNote_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                string tag = control.Tag as string;
                CreateLink(tag);
            }
        }

        private void tbPresentationName_Leave(object sender, EventArgs e)
        {
            Save();
        }


    }
}
