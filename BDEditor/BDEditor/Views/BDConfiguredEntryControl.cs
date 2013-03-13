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
    public partial class BDConfiguredEntryControl : UserControl, IBDControl
    {
        protected Entities dataContext;
        protected BDConfiguredEntry currentEntry;
        protected Guid? scopeId;
        protected Guid parentId;
        protected BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;

        public Boolean showOverview { get; set; }

        protected List<BDConfiguredEntryFieldControl> fieldControlList = new List<BDConfiguredEntryFieldControl>();

        private BDConstants.BDNodeType defaultNodeType;
        private BDConstants.LayoutVariantType defaultLayoutVariantType;
        private int? displayOrder;
        private bool showAsChild = false;

        private bool showChildren = true;
        public bool ShowChildren
        {
            get { return showChildren; }
            set { showChildren = value; }
        }

        public BDConfiguredEntryControl()
        {
            InitializeComponent();
        }

        public BDConfiguredEntryControl(Entities pDataContext, BDConfiguredEntry pConfiguredEntry, Guid? pScopeId)
        {
            InitializeComponent();
            
            if (null == pConfiguredEntry) throw new NotSupportedException("May not create a ConfiguredEntry control without an existing entry");
            if (null == pConfiguredEntry.ParentId) throw new NotSupportedException("May not create a ConfiguredEntry control without a supplied parent");

            dataContext = pDataContext;
            currentEntry = pConfiguredEntry;
            parentId = currentEntry.ParentId.Value;
            scopeId = pScopeId;
        }

        #region OnEvent handlers
        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NameChanged;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnNotesChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NotesChanged;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnItemAddRequested(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = RequestItemAdd;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnItemDeleteRequested(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = RequestItemDelete;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnReorderToPrevious(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = ReorderToPrevious;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnReorderToNext(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = ReorderToNext;
            if (null != handler) { handler(this, e); }
        }
        #endregion

        void fieldControl_NameChanged(object sender, NodeEventArgs e)
        {
            if (e.ContextInfo == BDConfiguredEntry.PROPERTYNAME_NAME)
            {
                OnNameChanged(e);
            }
        }

        void fieldControl_NotesChanged(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        private BDConfiguredEntryFieldControl addFieldEntryControl(BDConfiguredEntry pConfiguredEntry, string pPropertyName, int pTabIndex)
        {
            BDConfiguredEntryFieldControl control = null;
            if (showOverview)
            {
                control = new BDConfiguredEntryFieldOverviewControl(dataContext, pConfiguredEntry, pPropertyName, scopeId.Value);
            }
            else
            {
                control = new BDConfiguredEntryFieldControl(dataContext, pConfiguredEntry, pPropertyName, scopeId.Value);
            }

            if (null != control)
            {
                ((System.Windows.Forms.UserControl)control).Dock = DockStyle.Top;
                ((System.Windows.Forms.UserControl)control).TabIndex = pTabIndex;
                control.DisplayOrder = pTabIndex;

                control.NotesChanged += new EventHandler<NodeEventArgs>(fieldControl_NotesChanged);
                control.NameChanged += new EventHandler<NodeEventArgs>(fieldControl_NameChanged);

                fieldControlList.Add(control);

                panelFields.Controls.Add(((System.Windows.Forms.UserControl)control));

                ((System.Windows.Forms.UserControl)control).BringToFront();
                control.RefreshLayout();
            }

            return control;
        }

        private void removeFieldEntryControl(BDConfiguredEntryFieldControl pFieldEntryControl)
        {
            ControlHelper.SuspendDrawing(this);
            panelFields.Controls.Remove(((System.Windows.Forms.UserControl)pFieldEntryControl));

            pFieldEntryControl.NameChanged -= new EventHandler<NodeEventArgs>(fieldControl_NameChanged);
            pFieldEntryControl.NotesChanged -= new EventHandler<NodeEventArgs>(fieldControl_NotesChanged);

            fieldControlList.Remove(pFieldEntryControl);

            ((System.Windows.Forms.UserControl)pFieldEntryControl).Dispose();
            pFieldEntryControl = null;
            ControlHelper.ResumeDrawing(this);
        }

        #region IBDControl

        public event EventHandler<NodeEventArgs> RequestItemAdd;
        public event EventHandler<NodeEventArgs> RequestItemDelete;
        public event EventHandler<NodeEventArgs> ReorderToPrevious;
        public event EventHandler<NodeEventArgs> ReorderToNext;
        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            if (null == pParentId) throw new NotSupportedException("May not assign a null parentId to a ConfiguredEntryControl");
            parentId = pParentId.Value;
            parentType = pParentType;
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public bool Save()
        {
            if (BDCommon.Settings.IsUpdating) return false;

            bool result = false;

            foreach (BDConfiguredEntryFieldControl fieldControl in fieldControlList)
            {
                result = fieldControl.Save() || result;
            }

            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool CreateCurrentObject()
        {
            //do nothing
            return true;
        }

        public void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            Boolean origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);

            for (int idx = 0; idx < fieldControlList.Count; idx++)
            {
                BDConfiguredEntryFieldControl control = fieldControlList[idx];
                removeFieldEntryControl(control);
            }
            fieldControlList.Clear();
            panelFields.Controls.Clear();

            List<BDLayoutMetadataColumn> metaDataColumnList = BDLayoutMetadataColumn.RetrieveListForLayout(dataContext, currentEntry.LayoutVariant, BDConstants.BDNodeType.BDConfiguredEntry);
            int tabIndex = 0;
            foreach (BDLayoutMetadataColumn columnDef in metaDataColumnList)
            {
                BDLayoutMetadataColumnNodeType propertyInfo = BDLayoutMetadataColumnNodeType.Retrieve(dataContext, currentEntry.LayoutVariant, currentEntry.NodeType, columnDef.Uuid);
                if (null != propertyInfo)
                {
                    addFieldEntryControl(currentEntry, propertyInfo.propertyName, tabIndex++);
                }
            }

            if (fieldControlList.Count <= 0)
            {
                panelFields.Visible = false;
            }
            ControlHelper.ResumeDrawing(this);

            BDCommon.Settings.IsUpdating = origState;
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            for (int idx = 0; idx < fieldControlList.Count; idx++)
            {
                fieldControlList[idx].ShowLinksInUse(true);
            }
        }

        public BDConstants.BDNodeType DefaultNodeType
        {
            get {  return defaultNodeType; }
            set { defaultNodeType = value; }
        }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType
        {
            get { return defaultLayoutVariantType;  }
            set { defaultLayoutVariantType = value; }
        }

        public IBDNode CurrentNode
        {
            get { return currentEntry; }
            set {  currentEntry = value as BDConfiguredEntry; }
        }

        public int? DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }

        public bool ShowAsChild
        {
            get { return showAsChild; }
            set { showAsChild = value; }
        }

        #endregion

        #region Context Menu handlers

        private void btnMenu_Click(object sender, EventArgs e)
        {
            BDFormsUtilties.buildNavContextMenuStrip(CurrentNode, addChildNodeToolStripMenuItem, addSiblingNodeToolStripMenuItem, deleteNodeToolStripMenuItem, editIndexStripMenuItem, 
                reorderPreviousToolStripMenuItem, reorderNextToolStripMenuItem, new EventHandler<NodeEventArgs>(RequestItemDelete), new EventHandler(addChildNode_Click), true);

            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        void addChildNode_Click(object sender, EventArgs e)
        {

        }

        void addSiblingNode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    OnItemAddRequested(new NodeEventArgs(dataContext, nodeWrapper.TargetNodeType, nodeWrapper.TargetLayoutVariant));
                }
            }
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    OnReorderToPrevious(new NodeEventArgs(dataContext, nodeWrapper.Node.Uuid));
                }
            }
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    OnReorderToNext(new NodeEventArgs(dataContext, nodeWrapper.Node.Uuid));
                }
            }
        }

        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    string message = string.Format("Delete {0}?", BDUtilities.GetEnumDescription(nodeWrapper.TargetNodeType));
                    if (MessageBox.Show(message, "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        OnItemDeleteRequested(new NodeEventArgs(dataContext, nodeWrapper.Node.Uuid));
                    }
                }
            }
        }

        private void editIndexStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    BDSearchEntryEditView indexEditView = new BDSearchEntryEditView();
                    indexEditView.AssignDataContext(dataContext);
                    indexEditView.AssignCurrentNode(nodeWrapper.Node);
                    string contextString = BDUtilities.BuildHierarchyString(dataContext, nodeWrapper.Node, ":");
                    indexEditView.DisplayContext = contextString;
                    indexEditView.ShowDialog(this);

                }
            }
        }
        #endregion
    }
}
