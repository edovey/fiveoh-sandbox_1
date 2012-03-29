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
    public partial class BDTableControl : UserControl, IBDControl
    {    
        #region Class properties
        private Entities dataContext;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private BDNode currentTable;
        private BDNode currentSection;
        private BDConstants.LayoutVariantType defaultLayoutVariantType;
        private BDConstants.BDNodeType defaultNodeType;
        private Guid? scopeId;

        public int? DisplayOrder { get; set; }

        private List<BDNodeWithOverviewControl> nodeControlListSection = new List<BDNodeWithOverviewControl>();
        private List<BDNodeWithOverviewControl> nodeControlListSectionDetail = new List<BDNodeWithOverviewControl>();

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;
        public event EventHandler ReorderToPrevious;
        public event EventHandler ReorderToNext;
        public event EventHandler NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

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

        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NameChanged;

            if (null != handler) { handler(this, e); }
        }

        public BDNode CurrentTable
        {
            get { return currentTable; }
            set { currentTable = value; }
        }

        #endregion

        public BDTableControl()
        {
            InitializeComponent();
            btnTableNameLink.Tag = BDNode.PROPERTYNAME_NAME;
        }

                /// <summary>
        /// Initialize form with existing BDNode
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pNode"></param>
        public BDTableControl(Entities pDataContext, IBDNode pNode)
        {
            dataContext = pDataContext;
            currentTable = pNode as BDNode;
            defaultLayoutVariantType = pNode.LayoutVariant;
            parentId = pNode.ParentId;
            defaultNodeType = pNode.NodeType;

            InitializeComponent();
        }

        private void BDTableControl_Load(object sender, EventArgs e)
        {
            if (null != currentTable)
                tbTableName.Text = currentTable.Name;
        }

        private void BDTableControl_Leave(object sender, EventArgs e)
        {
            Save();
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

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        public bool Save()
        {
            bool result = false;

            if (null != parentId)
            {
                foreach (BDNodeWithOverviewControl control in nodeControlListSection)
                {
                    result = control.Save() || result;
                }

                // if zero nodes are defined then this is a valid test
                if ((result && (null == currentTable) || (null == currentTable) && tbTableName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentTable)
                {
                    System.Diagnostics.Debug.WriteLine(@"Table Control Save");

                    if (currentTable.name != tbTableName.Text) currentTable.name = tbTableName.Text;
                    if (currentTable.displayOrder != DisplayOrder) currentTable.displayOrder = DisplayOrder;

                    BDNode.Save(dataContext, currentTable);

                    result = true;
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

            if (null == this.currentTable )
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    this.currentTable = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDTable);
                    this.currentTable.SetParent(parentType, parentId);
                    this.currentTable.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentTable.LayoutVariant = defaultLayoutVariantType;

                    BDNode.Save(dataContext, currentTable);
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            this.SuspendLayout();

            for (int idx = 0; idx < nodeControlListSection.Count; idx++)
            {
                BDNodeWithOverviewControl control = nodeControlListSection[idx];
                removeNodeControlForSection(control, false);
            }
            nodeControlListSection.Clear();

            pnlSections.Controls.Clear();

            if (null == currentTable)
            {
                tbTableName.Text = @"";
            }
            else
            {
                // This is assuming Constants.LayoutVariantType.TreatmentRecommendation02
                tbTableName.Text = currentTable.name;
                List<IBDNode> list = BDFabrik.GetChildrenForParent(dataContext, currentTable);
                int idxSection = 0;
                foreach (IBDNode listEntry in list)
                {
                    BDNode node = listEntry as BDNode;
                    addNodeControlForSection(node, idxSection++);
                }
            }

            ShowLinksInUse(false);

            this.ResumeLayout();
        }

        #endregion

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private BDNodeWithOverviewControl addNodeControlForSection(BDNode pNode, int pTabIndex)
        {
            BDNodeWithOverviewControl nodeControl = null;

            if (CreateCurrentObject())
            {
                nodeControl = new BDNodeWithOverviewControl(dataContext, BDConstants.BDNodeType.BDTableRow, BDConstants.LayoutVariantType.TreatmentRecommendation02, currentTable.Uuid);

                nodeControl.Dock = DockStyle.Top;
                nodeControl.TabIndex = pTabIndex;
                nodeControl.DisplayOrder = pTabIndex;
                nodeControl.AssignParentInfo(currentTable.Uuid, currentTable.NodeType);
                nodeControl.AssignScopeId(scopeId);
                nodeControl.CurrentNode = pNode;
                nodeControl.RequestItemAdd += new EventHandler(SectionDetail_RequestItemAdd);
                nodeControl.RequestItemDelete += new EventHandler(SectionDetail_RequestItemDelete);
                nodeControl.ReorderToNext += new EventHandler(SectionDetail_ReorderToNext);
                nodeControl.ReorderToPrevious += new EventHandler(SectionDetail_ReorderToPrevious);
                nodeControl.NotesChanged += new EventHandler(notesChanged_Action);
                nodeControlListSection.Add(nodeControl);

                pnlSections.Controls.Add(nodeControl);
                nodeControl.BringToFront();
                nodeControl.RefreshLayout();
            }
            return nodeControl;
        }

        private void removeNodeControlForSection(BDNodeWithOverviewControl pNodeControl, bool pDeleteRecord)
        {
            this.Controls.Remove(pNodeControl);

            pNodeControl.RequestItemAdd -= new EventHandler(Section_RequestItemAdd);
            pNodeControl.RequestItemDelete -= new EventHandler(Section_RequestItemDelete);
            pNodeControl.ReorderToNext -= new EventHandler(Section_ReorderToNext);
            pNodeControl.ReorderToPrevious -= new EventHandler(Section_ReorderToPrevious);
            pNodeControl.NotesChanged -= new EventHandler(notesChanged_Action);

            nodeControlListSection.Remove(pNodeControl);

            if (pDeleteRecord)
            {
                BDNode node =(BDNode) pNodeControl.CurrentNode;
                if (null != node)
                {
                    BDNode.Delete(dataContext, node, pDeleteRecord);

                    for (int idx = 0; idx < nodeControlListSection.Count; idx++)
                    {
                        nodeControlListSection[idx].DisplayOrder = idx;
                    }
                }
            }

            pNodeControl.Dispose();
            pNodeControl = null;
        }

        private void reorderSectionNodeControl(BDNodeWithOverviewControl pNodeControl, int pOffset)
        {
            int currentPosition = nodeControlListSection.FindIndex(t => t == pNodeControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition += pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < nodeControlListSection.Count))
                {
                    nodeControlListSection[requestedPosition].CreateCurrentObject();
                    nodeControlListSection[requestedPosition].DisplayOrder = currentPosition;
                    nodeControlListSection[requestedPosition].CurrentNode.DisplayOrder = currentPosition;
                    BDNode.Save(dataContext, nodeControlListSection[requestedPosition].CurrentNode as BDNode);

                    nodeControlListSection[currentPosition].CreateCurrentObject();
                    nodeControlListSection[currentPosition].DisplayOrder = requestedPosition;
                    nodeControlListSection[currentPosition].CurrentNode.DisplayOrder = requestedPosition;
                    BDNode.Save(dataContext, nodeControlListSection[currentPosition].CurrentNode as BDNode);

                    BDNodeWithOverviewControl temp = nodeControlListSection[requestedPosition];
                    nodeControlListSection[requestedPosition] = nodeControlListSection[currentPosition];
                    nodeControlListSection[currentPosition] = temp;

                    int zOrder = pnlSections.Controls.GetChildIndex(pNodeControl);
                    zOrder = zOrder + (pOffset * -1);
                    pnlSections.Controls.SetChildIndex(pNodeControl, zOrder);
                }
            }
        }

        private BDNodeWithOverviewControl addNodeControlforSectionDetail(BDNode pNode, int pTabIndex)
        {
            BDNodeWithOverviewControl nodeControl = null;

            if (CreateCurrentObject())
            {
                nodeControl = new BDNodeWithOverviewControl(dataContext, BDConstants.BDNodeType.BDTableRow, BDConstants.LayoutVariantType.TreatmentRecommendation02, currentSection.Uuid);

                nodeControl.Dock = DockStyle.Top;
                nodeControl.TabIndex = pTabIndex;
                nodeControl.DisplayOrder = pTabIndex;
                nodeControl.AssignParentInfo(currentSection.Uuid, currentSection.NodeType);
                nodeControl.AssignScopeId(scopeId);
                nodeControl.CurrentNode = pNode;
                // nodeControl.DefaultLayoutVariantType = BDConstants.LayoutVariantType.TreatmentRecommendation02;
                nodeControl.RequestItemAdd += new EventHandler(SectionDetail_RequestItemAdd);
                nodeControl.RequestItemDelete += new EventHandler(SectionDetail_RequestItemDelete);
                nodeControl.ReorderToNext += new EventHandler(SectionDetail_ReorderToNext);
                nodeControl.ReorderToPrevious += new EventHandler(SectionDetail_ReorderToPrevious);
                nodeControl.NotesChanged += new EventHandler(notesChanged_Action);

                nodeControlListSectionDetail.Add(nodeControl);

                pnlSections.Controls.Add(nodeControl);
                nodeControl.BringToFront();

                nodeControl.RefreshLayout();
            }

            return nodeControl;
        }

        private void removeNodeControlForSectionDetail(BDNodeWithOverviewControl pNodeControl, bool pDeleteRecord)
        {
            pnlSections.Controls.Remove(pNodeControl);

            pNodeControl.RequestItemAdd -= new EventHandler(SectionDetail_RequestItemAdd);
            pNodeControl.RequestItemDelete -= new EventHandler(SectionDetail_RequestItemDelete);
            pNodeControl.ReorderToNext -= new EventHandler(SectionDetail_ReorderToNext);
            pNodeControl.ReorderToPrevious -= new EventHandler(SectionDetail_ReorderToPrevious);
            pNodeControl.NotesChanged -= new EventHandler(notesChanged_Action);

            nodeControlListSectionDetail.Remove(pNodeControl);

            if (pDeleteRecord)
            {
                BDNode entry = pNodeControl.CurrentNode as BDNode;
                if (null != entry)
                {
                    BDNode.Delete(dataContext, entry, pDeleteRecord);
                    for (int idx = 0; idx < nodeControlListSectionDetail.Count; idx++)
                    {
                        nodeControlListSectionDetail[idx].DisplayOrder = idx;
                    }
                }
            }

            pNodeControl.Dispose();
            pNodeControl = null;
        }

        private void reorderDetailNodeControl(BDNodeWithOverviewControl pNodeControl, int pOffset)
        {
            int currentPosition = nodeControlListSectionDetail.FindIndex(t => t == pNodeControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < nodeControlListSectionDetail.Count))
                {
                    nodeControlListSectionDetail[requestedPosition].CreateCurrentObject();
                    nodeControlListSectionDetail[requestedPosition].DisplayOrder = currentPosition;

                    nodeControlListSectionDetail[requestedPosition].CurrentNode.DisplayOrder = currentPosition;
                    BDNode.Save(dataContext, nodeControlListSectionDetail[requestedPosition].CurrentNode as BDNode);

                    nodeControlListSectionDetail[currentPosition].CreateCurrentObject();
                    nodeControlListSectionDetail[currentPosition].DisplayOrder = requestedPosition;

                    nodeControlListSectionDetail[currentPosition].CurrentNode.DisplayOrder = requestedPosition;
                    BDNode.Save(dataContext, nodeControlListSectionDetail[currentPosition].CurrentNode as BDNode);

                    BDNodeWithOverviewControl temp = nodeControlListSectionDetail[requestedPosition];
                    nodeControlListSectionDetail[requestedPosition] = nodeControlListSectionDetail[currentPosition];
                    nodeControlListSectionDetail[currentPosition] = temp;

                    int zOrder = pnlSections.Controls.GetChildIndex(pNodeControl);
                    zOrder = zOrder + (pOffset * -1);
                    pnlSections.Controls.SetChildIndex(pNodeControl, zOrder);
                }
            }
        }

        private void Section_RequestItemAdd(object sender, EventArgs e)
        {
            BDNodeWithOverviewControl control = addNodeControlForSection(null, nodeControlListSection.Count);
            if (null != control)
                control.Focus();
            //OnItemAddRequested(new EventArgs());
        }

        private void Section_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
        }

        private void Section_ReorderToPrevious(object sender, EventArgs e)
        {
            OnReorderToPrevious(new EventArgs());
        }

        private void Section_ReorderToNext(object sender, EventArgs e)
        {
            OnReorderToNext(new EventArgs());
        }

        private void SectionDetail_RequestItemAdd(object sender, EventArgs e)
        {
            BDNodeWithOverviewControl clickedControl = sender as BDNodeWithOverviewControl;
            currentSection = clickedControl.CurrentNode as BDNode;

            BDNodeWithOverviewControl control = addNodeControlforSectionDetail(null, nodeControlListSectionDetail.Count);
            if (null != control)
                control.Focus();
        }

        private void SectionDetail_RequestItemDelete(object sender, EventArgs e)
        {
            BDNodeWithOverviewControl control = sender as BDNodeWithOverviewControl;
            if (null != control)
                if (MessageBox.Show("Delete details?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removeNodeControlForSectionDetail(control, true);
        }

        private void SectionDetail_ReorderToNext(object sender, EventArgs e)
        {
            BDNodeWithOverviewControl control = sender as BDNodeWithOverviewControl;
            if (null != control)
            {
                reorderSectionNodeControl(control, 1);
            }
        }

        private void SectionDetail_ReorderToPrevious(object sender, EventArgs e)
        {
            BDNodeWithOverviewControl control = sender as BDNodeWithOverviewControl;
            if (null != control)
            {
                reorderSectionNodeControl(control, -1);
            }
        }

        public override string ToString()
        {
            return (null == this.currentTable) ? "No Table" : this.currentTable.uuid.ToString();
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
                view.AssignParentInfo(currentTable.Uuid, currentTable.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentTable) ? this.currentTable.uuid : Guid.Empty);
            btnTableNameLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnTableNameLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;

            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < nodeControlListSection.Count; idx++)
                {
                    nodeControlListSection[idx].ShowLinksInUse(true);
                }

                for (int idx = 0; idx < nodeControlListSectionDetail.Count; idx++)
                {
                    nodeControlListSectionDetail[idx].ShowLinksInUse(true);
                }
            }
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

        private void notesChanged_Action(object sender, EventArgs e)
        {
            OnNotesChanged(new EventArgs());
        }

        private void textBoxPathogenGroupName_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbTableName, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbTableName, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbTableName, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbTableName, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbTableName, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbTableName, "±");
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbTableName, "¹");
        }

        private void textBoxPathogenGroupName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                undoToolStripMenuItem.Enabled = tbTableName.CanUndo;
                pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
                cutToolStripMenuItem.Enabled = (tbTableName.SelectionLength > 0);
                copyToolStripMenuItem.Enabled = (tbTableName.SelectionLength > 0);
                deleteToolStripMenuItem.Enabled = (tbTableName.SelectionLength > 0);
            }
        }

    }
}
