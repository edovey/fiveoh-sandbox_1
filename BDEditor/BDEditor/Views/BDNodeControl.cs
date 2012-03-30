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

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;

        public event EventHandler ReorderToPrevious;
        public event EventHandler ReorderToNext;

        public event EventHandler NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NameChanged;

            if (null != handler) { handler(this, e); }
        }

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
            OnReorderToPrevious(new EventArgs());
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new EventArgs());
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
    }
}
