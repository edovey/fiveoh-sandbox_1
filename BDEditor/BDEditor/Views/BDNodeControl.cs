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
    public partial class BDNodeControl : UserControl, IBDControl
    {
        private IBDNode currentNode;
        private Entities dataContext;
        private Guid? scopeId;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;
        private bool showAsChild = false;

        public BDConstants.BDNodeType DefaultNodeType;
        public BDConstants.LayoutVariantType DefaultLayoutVariantType;
        public int? DisplayOrder { get; set; }

        private List<BDNodeWithOverviewControl> detailControlList = new List<BDNodeWithOverviewControl>();

        private List<ToolStripMenuItem> addChildNodeToolStripMenuItemList = new List<ToolStripMenuItem>(); //List of possible children
        private List<ToolStripMenuItem> addSiblingNodeToolStripMenuItemList = new List<ToolStripMenuItem>(); //Bubbles to parent for creation
       
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
            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
            setFormLayoutState();
            if (null != currentNode)
            {
                if (tbName.Text != currentNode.Name) tbName.Text = currentNode.Name;
            }
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

        #region IBDControl

        public void RefreshLayout()
        {
            this.SuspendLayout();

            for (int idx = 0; idx < detailControlList.Count; idx++)
            {
                BDNodeWithOverviewControl control = detailControlList[idx];
                removeNodeControlForTableDetail(control, false);
            }
            detailControlList.Clear();
            pnlDetail.Controls.Clear();

            if (currentNode == null)
            {
                tbName.Text = @"";
            }
            else
            {
                tbName.Text = currentNode.Name;
                List<IBDNode> list = BDFabrik.GetChildrenForParent(dataContext, currentNode);
                int idxDetail = 0;
                foreach (IBDNode entry in list)
                {
                    BDNode node = entry as BDNode;
                    addNodeControlForTableDetail(node, idxDetail++);
                }
            }
            ShowLinksInUse(false);
            setFormLayoutState();
            this.ResumeLayout();
        }

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        public bool Save()
        {
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
                    foreach (BDNodeWithOverviewControl control in detailControlList)
                    {
                        result = control.Save() || result;
                    }

                    if (currentNode.Name != tbName.Text)
                    {
                        currentNode.Name = tbName.Text;
                        OnNameChanged(new NodeEventArgs(dataContext, currentNode.Uuid, currentNode.Name));
                    }

                    switch (currentNode.NodeType)
                    {
                        case BDConstants.BDNodeType.BDTherapy:
                            BDTherapy therapy = currentNode as BDTherapy;
                            if (null != therapy)
                            {
                                BDTherapy.Save(dataContext, therapy);
                            }
                            break;
                        case BDConstants.BDNodeType.BDTherapyGroup:
                            BDTherapyGroup therapyGroup = currentNode as BDTherapyGroup;
                            if (null != therapyGroup)
                            {
                                BDTherapyGroup.Save(dataContext, therapyGroup);
                            }
                            break;
                        default:
                            BDNode node = currentNode as BDNode;
                            if (null != node)
                            {
                                if (null == node.parentId || Guid.Empty == node.ParentId)
                                    node.SetParent(parentType, parentId);
                                BDNode.Save(dataContext, node);
                                result = true;
                            }
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
                    switch (DefaultNodeType)
                    {
                        case BDConstants.BDNodeType.BDTherapy:
                            currentNode = BDTherapy.CreateTherapy(dataContext, parentId.Value);
                            break;
                        case BDConstants.BDNodeType.BDTherapyGroup:
                            currentNode = BDTherapyGroup.CreateTherapyGroup(dataContext, parentId.Value);
                            break;
                        default:
                            currentNode = BDNode.CreateNode(this.dataContext, this.DefaultNodeType);
                            break;
                    }

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
                for (int idx = 0; idx < detailControlList.Count; idx++)
                {
                    detailControlList[idx].ShowLinksInUse(true);
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
            
            btnMenuRight.Enabled = showAsChild;
            btnMenuRight.Visible = showAsChild;
            lblNodeAsChild.Text = (DefaultNodeType > 0) ? BDUtilities.GetEnumDescription(DefaultNodeType) : @"Section";
            lblNodeAsChild.Enabled = showAsChild;
            lblNodeAsChild.Visible = showAsChild;
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
                view.NotesChanged += new EventHandler(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private BDNodeWithOverviewControl addNodeControlForTableDetail(BDNode pNode, int pTabIndex)
        {
            BDNodeWithOverviewControl nodeControl = null;

            if (CreateCurrentObject())
            {
                nodeControl = new BDNodeWithOverviewControl();

                nodeControl.Dock = DockStyle.Top;
                nodeControl.TabIndex = pTabIndex;
                nodeControl.DisplayOrder = pTabIndex;
                nodeControl.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
                nodeControl.AssignDataContext(dataContext);
                nodeControl.AssignScopeId(scopeId);
                nodeControl.ShowAsChild = true;
                nodeControl.CurrentNode = pNode;
                nodeControl.DefaultLayoutVariantType = this.DefaultLayoutVariantType;
                nodeControl.DefaultNodeType = BDConstants.BDNodeType.BDTableRow;

                nodeControl.RequestItemAdd += new EventHandler(TableDetail_RequestItemAdd);
                nodeControl.RequestItemDelete += new EventHandler(TableDetail_RequestItemDelete);
                nodeControl.ReorderToNext += new EventHandler(TableDetail_ReorderToNext);
                nodeControl.ReorderToPrevious += new EventHandler(TableDetail_ReorderToPrevious);
                nodeControl.NotesChanged += new EventHandler(notesChanged_Action);
                detailControlList.Add(nodeControl);

                pnlDetail.Controls.Add(nodeControl);
                nodeControl.BringToFront();
                nodeControl.RefreshLayout();
            }
            return nodeControl;
        }

        private void removeNodeControlForTableDetail(BDNodeWithOverviewControl pNodeControl, bool pDeleteRecord)
        {
            this.Controls.Remove(pNodeControl);

            pNodeControl.RequestItemAdd -= new EventHandler(TableDetail_RequestItemAdd);
            pNodeControl.RequestItemDelete -= new EventHandler(TableDetail_RequestItemDelete);
            pNodeControl.ReorderToNext -= new EventHandler(TableDetail_ReorderToNext);
            pNodeControl.ReorderToPrevious -= new EventHandler(TableDetail_ReorderToPrevious);
            pNodeControl.NotesChanged -= new EventHandler(notesChanged_Action);

            detailControlList.Remove(pNodeControl);

            if (pDeleteRecord)
            {
                BDNode node = (BDNode)pNodeControl.CurrentNode;
                if (null != node)
                {
                    BDNode.Delete(dataContext, node, pDeleteRecord);

                    for (int idx = 0; idx < detailControlList.Count; idx++)
                    {
                        detailControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pNodeControl.Dispose();
            pNodeControl = null;
        }

        private void reorderNodeControlForTableDetail(BDNodeWithOverviewControl pNodeControl, int pOffset)
        {
            int currentPosition = detailControlList.FindIndex(t => t == pNodeControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition += pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < detailControlList.Count))
                {
                    detailControlList[requestedPosition].CreateCurrentObject();
                    detailControlList[requestedPosition].DisplayOrder = currentPosition;

                    detailControlList[requestedPosition].CurrentNode.DisplayOrder = currentPosition;
                    BDNode.Save(dataContext, detailControlList[requestedPosition].CurrentNode as BDNode);

                    detailControlList[currentPosition].CreateCurrentObject();
                    detailControlList[currentPosition].DisplayOrder = requestedPosition;
                    detailControlList[currentPosition].CurrentNode.DisplayOrder = requestedPosition;
                    BDNode.Save(dataContext, detailControlList[currentPosition].CurrentNode as BDNode);

                    BDNodeWithOverviewControl temp = detailControlList[requestedPosition];
                    detailControlList[requestedPosition] = detailControlList[currentPosition];
                    detailControlList[currentPosition] = temp;

                    int zOrder = pnlDetail.Controls.GetChildIndex(pNodeControl);
                    zOrder = zOrder + (pOffset * -1);
                    pnlDetail.Controls.SetChildIndex(pNodeControl, zOrder);
                }
            }
        }

        private void TableSection_RequestItemAdd(object sender, EventArgs e)
        {
            OnItemAddRequested(new EventArgs());
        }

        private void TableSection_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
        }

        private void TableDetail_RequestItemAdd(object sender, EventArgs e)
        {
            BDNodeWithOverviewControl control = addNodeControlForTableDetail(null, detailControlList.Count);
            if (null != control)
                control.Focus();
        }

        private void TableDetail_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
            BDNodeWithOverviewControl control = sender as BDNodeWithOverviewControl;
            if (null != control)
            {
                if (MessageBox.Show("Delete Table Detail?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removeNodeControlForTableDetail(control, true);
            }
        }

        private void TableDetail_ReorderToPrevious(object sender, EventArgs e)
        {
            BDNodeWithOverviewControl control = sender as BDNodeWithOverviewControl;
            if (null != control)
            {
                reorderNodeControlForTableDetail(control, -1);
            }
        }

        private void TableDetail_ReorderToNext(object sender, EventArgs e)
        {
            BDNodeWithOverviewControl control = sender as BDNodeWithOverviewControl;
            if (null != control)
            {
                reorderNodeControlForTableDetail(control, 1);
            }
        }

        private void notesChanged_Action(object sender, EventArgs e)
        {
            OnNotesChanged(new EventArgs());
        }

        private void bdLinkedNoteControl_SaveAttemptWithoutParent(object sender, EventArgs e)
        {
            BDLinkedNoteControl control = sender as BDLinkedNoteControl;
            if (null != control)
            {
                if (CreateCurrentObject())
                {
                    control.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
                    control.Save();
                }
            }
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
            this.contextMenuStripEvents.Show(btnMenuLeft, new System.Drawing.Point(0, btnMenuLeft.Height));
        }

        private void tbName_Leave(object sender, EventArgs e)
        {
            Save();
        }

        void addChildNode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    BDFabrik.CreateChildNode(DataContext, nodeWrapper.Node, nodeWrapper.TargetNodeType, nodeWrapper.TargetLayoutVariant);
                    showNavSelection(nodeWrapper.NodeTreeNode);
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
            foreach (ToolStripMenuItem entry in addChildNodeToolStripMenuItemList)
            {
                entry.Click -= new System.EventHandler(this.addChildNode_Click);
            }
            addChildNodeToolStripMenuItemList.Clear();

            foreach (ToolStripMenuItem entry in addSiblingNodeToolStripMenuItemList)
            {
                entry.Click -= new System.EventHandler(this.addSiblingNode_Click);
            }
            addSiblingNodeToolStripMenuItemList.Clear();

            addChildNodeToolStripMenuItem.DropDownItems.Clear();
            addSiblingNodeToolStripMenuItem.DropDownItems.Clear();

            reorderNextToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);
            reorderPreviousToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);
            deleteToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);

            string nodeTypeName = BDUtilities.GetEnumDescription(pBDNode.NodeType);

            deleteToolStripMenuItem.Text = string.Format("Delete {0}: {1}", nodeTypeName, pBDNode.Name);

            //List<BDConstants.BDNodeType> childTypes = BDFabrik.ChildTypeDefinitionListForNode(pBDNode);
            List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> childTypeInfoList = BDFabrik.ChildTypeDefinitionListForNode(pBDNode);
            if (null != childTypeInfoList)
            {
                if (childTypeInfoList.Count == 1)
                {
                    string childNodeTypeName = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item1);
                    addChildNodeToolStripMenuItem.Text = string.Format("Add {0}", childNodeTypeName);

                    if (childTypeInfoList[0].Item2.Length == 1)
                    {
                        addChildNodeToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[0], pTreeNode);
                    }
                    else
                    {
                        for (int idx = 0; idx < childTypeInfoList[0].Item2.Length; idx++)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem();

                            item.Image = global::BDEditor.Properties.Resources.add_16x16;
                            item.Name = string.Format("dynamicAddChildLayoutVariant{0}", idx);
                            item.Size = new System.Drawing.Size(179, 22);
                            item.Text = string.Format("&Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[idx]));
                            item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[idx], pTreeNode);
                            item.Click += new System.EventHandler(this.addChildNode_Click);
                            addChildNodeToolStripMenuItem.DropDownItems.Add(item);
                        }
                    }
                }
                else if (childTypeInfoList.Count > 1)
                {
                    addChildNodeToolStripMenuItem.Text = string.Format("Add");

                    for (int idx = 0; idx < childTypeInfoList.Count; idx++)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem();

                        item.Image = global::BDEditor.Properties.Resources.add_16x16;
                        item.Name = string.Format("dynamicAddChild{0}", idx);
                        item.Size = new System.Drawing.Size(179, 22);
                        item.Text = string.Format("&Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1));
                        item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[0], pTreeNode);
                        item.Click += new System.EventHandler(this.addChildNode_Click);

                        if (childTypeInfoList[idx].Item2.Length > 1)
                        {
                            for (int idy = 0; idy < childTypeInfoList[idx].Item2.Length; idy++)
                            {
                                ToolStripMenuItem layoutItem = new ToolStripMenuItem();

                                layoutItem.Image = global::BDEditor.Properties.Resources.add_16x16;
                                layoutItem.Name = string.Format("dynamicAddChildLayoutVariant{0}", idy);
                                layoutItem.Size = new System.Drawing.Size(179, 22);
                                layoutItem.Text = string.Format("Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[idy]));
                                layoutItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[idy], pTreeNode);
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
    }
}
