﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDNodeControl : UserControl, IBDControl
    {
        //protected bool isUpdating = false;

        protected IBDNode currentNode;
        protected Entities dataContext;
        protected Guid? scopeId;
        protected Guid? parentId;
        protected BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;
        protected bool showAsChild = false;
        protected bool showSiblingAdd = false;
        protected bool showChildren = true;

        public BDConstants.BDNodeType DefaultNodeType { get; set; }
        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }
        public int? DisplayOrder{ get; set; }

        protected List<IBDControl> childNodeControlList = new List<IBDControl>();

        protected List<ToolStripMenuItem> addChildNodeToolStripMenuItemList = new List<ToolStripMenuItem>(); //List of possible children
        protected List<ToolStripMenuItem> addSiblingNodeToolStripMenuItemList = new List<ToolStripMenuItem>(); //Bubbles to parent for creation

        public event EventHandler<NodeEventArgs> RequestItemAdd;
        public event EventHandler<NodeEventArgs> RequestItemDelete;

        public event EventHandler<NodeEventArgs> ReorderToPrevious;
        public event EventHandler<NodeEventArgs> ReorderToNext;

        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

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

        public IBDNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public bool ShowAsChild
        {
            get { return showAsChild; }
            set { showAsChild = value; }
        }

        public bool ShowSiblingAdd
        {
            get { return showSiblingAdd; }
            set { showSiblingAdd = value; }
        }

        public bool ShowChildren
        {
            get { return showChildren; }
            set { showChildren = value; }
        }

        public BDNodeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize form with existing BDNode
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pNode"></param>
        public BDNodeControl(Entities pDataContext, IBDNode pNode)
        {
            dataContext = pDataContext;
            currentNode = pNode;
            parentId = pNode.ParentId;
            DefaultNodeType = pNode.NodeType;
            DefaultLayoutVariantType = pNode.LayoutVariant;
            InitializeComponent();
        }

        private void BDNodeControl_Load(object sender, EventArgs e)
        {
            bool origState = BDCommon.Settings.IsUpdating;

            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
            setFormLayoutState();
            lblInfo.Visible = true;
            //if (null != currentNode)
            //{
            //    if (tbName.Text != currentNode.Name) tbName.Text = currentNode.Name;
            //    tbName.Select();
            //}

#if DEBUG
            contextMenuStripDebug.Enabled = true;
            copyUUIDToolStripMenuItem.Enabled = true;
            publishToDatabaseToolStripMenuItem.Enabled = true;
            generateSearchEntriesToolStripMenuItem.Enabled = true;
#else
            contextMenuStripDebug.Enabled = false;
#endif

            BDCommon.Settings.IsUpdating = origState;
        }

        private void BDNodeControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource, string pProperty)
        {
            if (pProperty == string.Empty || pProperty == BDNode.PROPERTYNAME_NAME)
            {
                tbName.AutoCompleteCustomSource = pSource;
                tbName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }

        #region IBDControl

        public virtual void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public virtual void RefreshLayout(bool pShowChildren)
        {
            bool origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);
            if(null != CurrentNode)
            {
                string lblString = string.Format(@"{0} : {1}", CurrentNode.Uuid, BDUtilities.GetEnumDescription(CurrentNode.LayoutVariant));
                toolTip1.SetToolTip(lblInfo, lblString);
            }
            for (int idx = 0; idx < childNodeControlList.Count; idx++)
            {
                IBDControl control = childNodeControlList[idx];
                removeChildNodeControl(control, false);
            }
            childNodeControlList.Clear();
            pnlDetail.Controls.Clear();

            if (currentNode == null)
            {
                tbName.Text = @"";
                lblInfo.Text = "na";
            }
            else
            {
                lblInfo.Text = currentNode.DisplayOrder.ToString();
                tbName.Text = currentNode.Name;
                if (pShowChildren)
                {
                    List<IBDNode> list = BDFabrik.GetChildrenForParent(dataContext, currentNode);
                    int idxDetail = 0;
                    foreach (IBDNode entry in list)
                    {
                        addChildNodeControl(entry, idxDetail++);
                    }
                }
            }
            ShowLinksInUse(false);
            setFormLayoutState();
            ControlHelper.ResumeDrawing(this);

            BDCommon.Settings.IsUpdating = origState;
        }

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        public bool Save()
        {
            if (BDCommon.Settings.IsUpdating) return false;

            System.Diagnostics.Debug.WriteLine(@"Node Control Save");

            bool result = false;

            if (null != parentId)
            {
                if ((null == currentNode) && (tbName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentNode)
                {
                    foreach (IBDControl control in childNodeControlList)
                    {
                        result = control.Save() || result;
                    }

                    if (currentNode.Name != tbName.Text)
                    {
                        currentNode.Name = tbName.Text;
                        OnNameChanged(new NodeEventArgs(dataContext, currentNode.Uuid, currentNode.Name));
                    }

                    if (null == currentNode.ParentId || Guid.Empty == currentNode.ParentId)
                        currentNode.SetParent(parentType, parentId);
                    
                    BDFabrik.SaveNode(dataContext, currentNode);  

                    switch (currentNode.NodeType)
                    {
                        case BDConstants.BDNodeType.BDPathogen:
                        case BDConstants.BDNodeType.BDTherapyGroup:
                            BDTypeahead.AddToCollection(currentNode.NodeType, BDNode.PROPERTYNAME_NAME, currentNode.Name);
                            break;

                        default:
                            break;
                    }
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

            if (null == this.currentNode)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentNode = BDFabrik.CreateNode(dataContext, DefaultNodeType, parentId, parentType );

                    this.currentNode.DisplayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentNode.LayoutVariant = this.DefaultLayoutVariantType;
                }
            }

            return result;
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentNode) ? this.currentNode.Uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;

            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < childNodeControlList.Count; idx++)
                {
                    childNodeControlList[idx].ShowLinksInUse(true);
                }
            }
        }

        #endregion

        private void setFormLayoutState()
        {
            btnMenuLeft.Enabled = !showAsChild;
            btnMenuLeft.Visible = !showAsChild;
            lblNode.Text = BDUtilities.GetEnumDescription(DefaultNodeType);
            lblNode.Enabled = !showAsChild;
            lblNode.Visible = !showAsChild;
            toolTip1.SetToolTip(lblNode, BDUtilities.GetEnumDescriptionWithNumber(DefaultLayoutVariantType));
            
            btnMenuRight.Enabled = showAsChild;
            btnMenuRight.Visible = showAsChild;
            lblNodeAsChild.Text = BDUtilities.GetEnumDescription(DefaultNodeType);
            lblNodeAsChild.Enabled = showAsChild;
            lblNodeAsChild.Visible = showAsChild;
            toolTip1.SetToolTip(lblNodeAsChild, BDUtilities.GetEnumDescriptionWithNumber(DefaultLayoutVariantType));
        }

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private void createLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();

                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private IBDControl addChildNodeControl(IBDNode pNode, int pTabIndex)
        {
            IBDControl nodeControl = null;

            if (CreateCurrentObject())
            {
                nodeControl = BDFabrik.CreateControlForNode(dataContext, pNode);

                if(null != nodeControl)
                {
                    ((System.Windows.Forms.UserControl)nodeControl).Dock = DockStyle.Top;
                    ((System.Windows.Forms.UserControl)nodeControl).TabIndex = pTabIndex;
                    nodeControl.DisplayOrder = pTabIndex;
                    nodeControl.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
                    nodeControl.AssignDataContext(dataContext);
                    nodeControl.AssignScopeId(scopeId);
                    nodeControl.CurrentNode = pNode;
                    nodeControl.DefaultLayoutVariantType = pNode.LayoutVariant;
                    nodeControl.DefaultNodeType = pNode.NodeType;

                    nodeControl.ReorderToNext += new EventHandler<NodeEventArgs>(childNodeControl_ReorderToNext);
                    nodeControl.ReorderToPrevious += new EventHandler<NodeEventArgs>(childNodeControl_ReorderToPrevious);
                    nodeControl.RequestItemAdd += new EventHandler<NodeEventArgs>(childNodeControl_RequestItemAdd);
                    nodeControl.RequestItemDelete += new EventHandler<NodeEventArgs>(childNodeControl_RequestItemDelete);
                    nodeControl.NotesChanged += new EventHandler<NodeEventArgs>(childNodeControl_NotesChanged);
                    nodeControl.NameChanged += new EventHandler<NodeEventArgs>(childNodeControl_NameChanged);

                    childNodeControlList.Add(nodeControl);

                    pnlDetail.Controls.Add(((System.Windows.Forms.UserControl)nodeControl));

                    ((System.Windows.Forms.UserControl)nodeControl).BringToFront();
                    nodeControl.RefreshLayout();
                }
            }
            return nodeControl;
        }

        void notesChanged_Action(object sender, NodeEventArgs e) // Same as though child control originated event
        {
            OnNotesChanged(e);
        }

        void childNodeControl_NameChanged(object sender, NodeEventArgs e)
        {
            OnNameChanged(e);
        }

        void childNodeControl_NotesChanged(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        void childNodeControl_RequestItemDelete(object sender, NodeEventArgs e)
        {
            removeChildNodeControl(e.Uuid.Value, true);
        }

        void childNodeControl_RequestItemAdd(object sender, NodeEventArgs e)
        {
            addChildControl(e.DataContext, CurrentNode, e.NodeType, e.LayoutVariant);
        }

        void childNodeControl_ReorderToPrevious(object sender, NodeEventArgs e)
        {
            reorderChildNodeControl(e.Uuid.Value, -1);
        }

        void childNodeControl_ReorderToNext(object sender, NodeEventArgs e)
        {
            reorderChildNodeControl(e.Uuid.Value, 1);
        }

        private void removeChildNodeControl(Guid pChildNodeUuid, bool pDeleteRecord)
        {
            int position = childNodeControlList.FindIndex(t => t.CurrentNode.Uuid == pChildNodeUuid);
            if (position >= 0)
            {
                IBDControl control = childNodeControlList[position];
                removeChildNodeControl(control, pDeleteRecord);
            }
        }

        private void removeChildNodeControl(IBDControl pChildNodeControl, bool pDeleteRecord)
        {
            ControlHelper.SuspendDrawing(this);
            this.Controls.Remove(((System.Windows.Forms.UserControl)pChildNodeControl));

            pChildNodeControl.NameChanged -= new EventHandler<NodeEventArgs>(childNodeControl_NameChanged);
            pChildNodeControl.NotesChanged -= new EventHandler<NodeEventArgs>(childNodeControl_NotesChanged);
            pChildNodeControl.ReorderToNext -= new EventHandler<NodeEventArgs>(childNodeControl_ReorderToNext);
            pChildNodeControl.ReorderToPrevious -= new EventHandler<NodeEventArgs>(childNodeControl_ReorderToPrevious);
            pChildNodeControl.RequestItemAdd -= new EventHandler<NodeEventArgs>(childNodeControl_RequestItemAdd);
            pChildNodeControl.RequestItemDelete -= new EventHandler<NodeEventArgs>(childNodeControl_RequestItemDelete);

            childNodeControlList.Remove(pChildNodeControl);

            if (pDeleteRecord)
            {
                IBDNode node = pChildNodeControl.CurrentNode;
                if (null != node)
                {
                    BDNode.Delete(dataContext, node, pDeleteRecord);

                    for (int idx = 0; idx < childNodeControlList.Count; idx++)
                    {
                        childNodeControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            ((System.Windows.Forms.UserControl)pChildNodeControl).Dispose();
            pChildNodeControl = null;
            ControlHelper.ResumeDrawing(this);
        }

        private void reorderChildNodeControl(Guid pChildNodeUuid, int pOffset)
        {
            int position = childNodeControlList.FindIndex(t => t.CurrentNode.Uuid == pChildNodeUuid);
            if (position >= 0)
            {
                IBDControl control = childNodeControlList[position];
                reorderChildNodeControl(control, pOffset);
            }
        }

        private void reorderChildNodeControl(IBDControl pChildNodeControl, int pOffset)
        {
            ControlHelper.SuspendDrawing(this);
            int currentPosition = childNodeControlList.FindIndex(t => t == pChildNodeControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < childNodeControlList.Count))
                {
                    childNodeControlList[requestedPosition].CreateCurrentObject();
                    childNodeControlList[requestedPosition].DisplayOrder = currentPosition;
                    childNodeControlList[requestedPosition].CurrentNode.DisplayOrder = currentPosition;
                    BDFabrik.SaveNode(dataContext, childNodeControlList[requestedPosition].CurrentNode);

                    childNodeControlList[currentPosition].CreateCurrentObject();
                    childNodeControlList[currentPosition].DisplayOrder = requestedPosition;
                    childNodeControlList[currentPosition].CurrentNode.DisplayOrder = requestedPosition;
                    BDFabrik.SaveNode(dataContext, childNodeControlList[currentPosition].CurrentNode);

                    IBDControl temp = childNodeControlList[requestedPosition];
                    childNodeControlList[requestedPosition] = childNodeControlList[currentPosition];
                    childNodeControlList[currentPosition] = temp;

                    int zOrder = pnlDetail.Controls.GetChildIndex(((System.Windows.Forms.UserControl)pChildNodeControl));
                    zOrder = zOrder + (pOffset * -1);
                    pnlDetail.Controls.SetChildIndex(((System.Windows.Forms.UserControl)pChildNodeControl), zOrder);
                }
            }
            ControlHelper.ResumeDrawing(this);
        }

        private void bdLinkedNoteControl_SaveAttemptWithoutParent(object sender, EventArgs e)
        {
            throw new NotSupportedException();
        }

        private void btnLinkedNote_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                string tag = control.Tag as string;
                createLink(tag);
            }
        }

        // Context Menu events for children
        private void editIndexStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper) {
                    BDSearchEntryEditView indexEditView = new BDSearchEntryEditView();
                    indexEditView.AssignDataContext(dataContext);
                    indexEditView.AssignCurrentNode(nodeWrapper.Node);
                    string contextString = BDUtilities.BuildHierarchyString(dataContext, nodeWrapper.Node, " : ");
                    indexEditView.DisplayContext = contextString;
                    indexEditView.ShowDialog(this);

                    indexEditView.Dispose();
                }
            }
        }

        private void editLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentNode != null)
            {
                BDLayoutColumnMetadataEditor layoutEditView = new BDLayoutColumnMetadataEditor();
                layoutEditView.AssignDataContext(dataContext);
                layoutEditView.AssignCurrentLayout(currentNode.LayoutVariant);
                layoutEditView.ShowDialog(this);
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

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "±");
        }

        private void tbName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                undoToolStripMenuItem.Enabled = tbName.CanUndo;
                pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
                cutToolStripMenuItem.Enabled = (tbName.SelectionLength > 0);
                copyToolStripMenuItem.Enabled = (tbName.SelectionLength > 0);
                deleteToolStripMenuItem.Enabled = (tbName.SelectionLength > 0);
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            buildNavContextMenuStrip(CurrentNode);

            Control anchor = (showAsChild) ? btnMenuRight : btnMenuLeft;
            this.contextMenuStripEvents.Show(anchor, new System.Drawing.Point(0, anchor.Height));
        }

        private void tbName_Leave(object sender, EventArgs e)
        {
            if(tbName.Text != currentNode.Name)
                Save();
        }

        void addChildControl(Entities pDataContext, IBDNode pParentNode, BDConstants.BDNodeType pChildNodeType, BDConstants.LayoutVariantType pChildLayoutVariant )
        {
            IBDNode node = BDFabrik.CreateChildNode(dataContext, pParentNode, pChildNodeType, pChildLayoutVariant);
            BDNotification.SendNotification(new BDNotificationEventArgs(BDNotificationEventArgs.BDNotificationType.Addition, pDataContext, node.NodeType, node.LayoutVariant, node.Uuid));
            if (ShowChildren)
            {
                ControlHelper.SuspendDrawing(this);
                IBDControl control = addChildNodeControl(node, childNodeControlList.Count);
                ControlHelper.ResumeDrawing(this);
                if (null != control)
                    ((System.Windows.Forms.UserControl)control).Focus();
            }
        }

        void addChildNode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    addChildControl(dataContext, nodeWrapper.Node, nodeWrapper.TargetNodeType, nodeWrapper.TargetLayoutVariant);
                }
            }
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

        #region Context Menu

        private void buildNavContextMenuStrip(IBDNode pBDNode)
        {
            string subDescription = string.Empty;
            //remove event handlers and existing entries
            foreach (ToolStripMenuItem entry in addChildNodeToolStripMenuItemList)
            {
                entry.Click -= new System.EventHandler(this.addChildNode_Click);
            }
            addChildNodeToolStripMenuItemList.Clear();
            //TODO: Bug fix?? was addChildNode_Click but corrected to addSiblingNode_Click
            //addSiblingNodeToolStripMenuItem.Click -= new EventHandler(addSiblingNode_Click);

            foreach (ToolStripMenuItem entry in addSiblingNodeToolStripMenuItemList)
            {
                entry.Click -= new System.EventHandler(this.addSiblingNode_Click);
                foreach (ToolStripMenuItem child in entry.DropDownItems)
                {
                    child.Click -= new System.EventHandler(this.addSiblingNode_Click);

                }
            }
            addSiblingNodeToolStripMenuItemList.Clear();

            addChildNodeToolStripMenuItem.Click -= new System.EventHandler(this.addChildNode_Click);
            foreach (ToolStripMenuItem entry in addChildNodeToolStripMenuItem.DropDownItems)
            {
                entry.Click -= new System.EventHandler(this.addChildNode_Click);
                foreach (ToolStripMenuItem child in entry.DropDownItems)
                {
                    child.Click -= new System.EventHandler(this.addChildNode_Click);

                }
            }
            addChildNodeToolStripMenuItem.DropDownItems.Clear();
            addSiblingNodeToolStripMenuItem.DropDownItems.Clear();

            // build the entries
            editIndexStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, null);
            reorderNextToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, null);
            reorderPreviousToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, null);
            deleteNodeToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, null);

            //subDescription = (pBDNode.NodeType == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(pBDNode.LayoutVariant)) : string.Empty;
            subDescription = BDUtilities.GetEnumDescription(pBDNode.LayoutVariant);
            addSiblingNodeToolStripMenuItem.Text = string.Format("&Add {0} [{1}]", BDUtilities.GetEnumDescription(pBDNode.NodeType), subDescription);

            // *****
            addSiblingNodeToolStripMenuItem.Visible = ShowSiblingAdd;
            addSiblingNodeToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant);
            // *****
            string nodeTypeName = BDUtilities.GetEnumDescription(pBDNode.NodeType);

            EventHandler<NodeEventArgs> handler = RequestItemDelete;
            if (null == handler) 
                deleteNodeToolStripMenuItem.Visible = false;
            else
                deleteNodeToolStripMenuItem.Text = string.Format("Delete {0}: {1}", nodeTypeName, pBDNode.Name);

            List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> childTypeInfoList = BDFabrik.ChildTypeDefinitionListForNode(pBDNode);
            if (null == childTypeInfoList || childTypeInfoList.Count == 0)
            {
                addChildNodeToolStripMenuItem.Visible = false;
                //toolStripSeparator2.Visible = ShowSiblingAdd;
            }
            else
            {
                if (childTypeInfoList.Count == 1)
                {
                    string childNodeTypeName = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item1);
                    //subDescription = (childTypeInfoList[0].Item1 == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[0])) : string.Empty;
                    subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[0]);
                    addChildNodeToolStripMenuItem.Text = string.Format("Add {0} [{1}]", childNodeTypeName, subDescription);

                    if (childTypeInfoList[0].Item2.Length == 1)
                    {
                        addChildNodeToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[0], null);
                        addChildNodeToolStripMenuItem.Click += new EventHandler(addChildNode_Click);
                    }
                    else
                    {
                        // One child type, many layout variants
                        for (int idx = 0; idx < childTypeInfoList[0].Item2.Length; idx++)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem();

                            item.Image = global::BDEditor.Properties.Resources.add_16x16;
                            item.Name = string.Format("dynamicAddChildLayoutVariant{0}", idx);
                            item.Size = new System.Drawing.Size(179, 22);
                            //subDescription = (childTypeInfoList[0].Item1 == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[idx])) : string.Empty;
                            subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[idx]);
                            item.Text = string.Format("&Add {0} [{1}]", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item1), subDescription);
                            item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[idx], null);
                            item.Click += new System.EventHandler(this.addChildNode_Click);
                            addChildNodeToolStripMenuItem.DropDownItems.Add(item);
                        }
                    }
                }
                else if (childTypeInfoList.Count > 1)
                {
                    // Many child types
                    addChildNodeToolStripMenuItem.Text = string.Format("Add");

                    for (int idx = 0; idx < childTypeInfoList.Count; idx++)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem();

                        item.Image = global::BDEditor.Properties.Resources.add_16x16;
                        item.Name = string.Format("dynamicAddChild{0}", idx);
                        item.Size = new System.Drawing.Size(179, 22);
                        //subDescription = (pBDNode.NodeType == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(((BDTableRow)pBDNode).NodeType)) : string.Empty;
                        //subDescription = (childTypeInfoList[idx].Item1 == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[0])) : string.Empty;
                        subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[0]);
                        item.Text = string.Format("&Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1));
                        item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[0], null);
                        item.Click += new System.EventHandler(this.addChildNode_Click);

                        if (childTypeInfoList[idx].Item2.Length > 1)
                        {
                            // Many layout variants per child type
                            for (int idy = 0; idy < childTypeInfoList[idx].Item2.Length; idy++)
                            {
                                ToolStripMenuItem layoutItem = new ToolStripMenuItem();

                                layoutItem.Image = global::BDEditor.Properties.Resources.add_16x16;
                                layoutItem.Name = string.Format("dynamicAddChildLayoutVariant{0}", idy);
                                layoutItem.Size = new System.Drawing.Size(179, 22);
                                //subDescription = (childTypeInfoList[idx].Item1 == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[idy])) : string.Empty;
                                subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[idy]);
                                layoutItem.Text = string.Format("Add {0} [{1}]", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1), subDescription);
                                layoutItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[idy], null);
                                layoutItem.Click += new System.EventHandler(this.addChildNode_Click);
                                item.DropDownItems.Add(layoutItem);
                            }
                        }

                        addChildNodeToolStripMenuItem.DropDownItems.Add(item);
                    }
                }
            }
        }
        #endregion

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = tbName.SelectionStart;
            tbName.Text = tbName.Text.Substring(0, i) + tbName.Text.Substring(i + tbName.SelectionLength);
            tbName.SelectionStart = i;
            tbName.SelectionLength = 0;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbName.SelectionStart = 0;
            tbName.SelectionLength = tbName.Text.Length;
            tbName.Focus();
        }

        private void contextMenuStripTextBox_Opening(object sender, CancelEventArgs e)
        {
            undoToolStripMenuItem.Enabled = tbName.CanUndo;
            pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
            cutToolStripMenuItem.Enabled = (tbName.SelectionLength > 0);
            copyToolStripMenuItem.Enabled = (tbName.SelectionLength > 0);
            deleteToolStripMenuItem.Enabled = (tbName.SelectionLength > 0);
        }

        private void copyUUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(currentNode.Uuid.ToString());
        }

        private void publishToDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if DEBUG
            this.Cursor = Cursors.WaitCursor;
            Guid nodeUuid = Guid.Empty;
            BDHtmlPageGenerator generator = new BDHtmlPageGenerator();
            List<BDNode> nodeList = new List<BDNode>();
            nodeList.Add(currentNode as BDNode);

            generator.GenerateForDebug(dataContext, nodeList);
            this.Cursor = Cursors.Default;
            MessageBox.Show(this, "HTML page generation complete", "Notice", MessageBoxButtons.OK);
#endif
        }

        private void generateSearchEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if DEBUG
            this.Cursor = Cursors.WaitCursor;
            Guid nodeUuid = Guid.Empty;
            BDHtmlPageGenerator generator = new BDHtmlPageGenerator();
            List<BDNode> nodeList = new List<BDNode>();
            nodeList.Add(currentNode as BDNode);

            generator.Generate(dataContext, nodeList);
            Debug.WriteLine("HTML page generation complete");

            BDSearchEntryGenerator searchGenerator = new BDSearchEntryGenerator();
            searchGenerator.Generate(dataContext);
            
            this.Cursor = Cursors.Default;
            MessageBox.Show(this, "Search Entry generation complete", "Notice", MessageBoxButtons.OK);
#endif
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if DEBUG
            //this.Cursor = Cursors.WaitCursor;
            //Guid nodeUuid = Guid.Empty;
            //BDHtmlPageGenerator generator = new BDHtmlPageGenerator();
            //List<BDNode> nodeList = new List<BDNode>();
            //nodeList.Add(currentNode as BDNode);

            //string noteText = generator.GenerateForPreview(dataContext, nodeList);
            //this.Cursor = Cursors.Default;
           
           

            //BDWebPreviewView preview = new BDWebPreviewView();
            //preview.previewHtml = noteText;

            //preview.Show();

//            MessageBox.Show(this, "HTML page generation complete", "Notice", MessageBoxButtons.OK);
#endif
        }
    }
}
