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
        private BDConstants.LayoutVariantType defaultLayoutVariantType;
        private BDConstants.BDNodeType defaultNodeType;
        private Guid? scopeId;

        public int? DisplayOrder { get; set; }

        private List<BDNodeControl> sectionControlList = new List<BDNodeControl>();

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

        public BDNode CurrentTable
        {
            get { return currentTable; }
            set { currentTable = value; }
        }

        #endregion

        public BDTableControl()
        {
            InitializeComponent();
            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
        }

        /// <summary>
        /// Initialize form with existing BDNode
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pNode"></param>
        public BDTableControl(Entities pDataContext, BDNode pNode)
        {
            InitializeComponent();
            dataContext = pDataContext;
            currentTable = pNode;
            defaultLayoutVariantType = pNode.LayoutVariant;
            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
        }

        public BDTableControl(Entities pDataContext, BDConstants.LayoutVariantType pDefaultLayoutVariantType)
        {
            InitializeComponent();
            dataContext = pDataContext;
            defaultLayoutVariantType = pDefaultLayoutVariantType;

            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
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
                // if zero nodes are defined then this is a valid test
                if ((result && (null == currentTable) || (null == currentTable) && tbTableName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentTable)
                {
                    foreach (BDNodeControl control in sectionControlList)
                    {
                        result = control.Save() || result;
                    }

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

            if (null == this.currentTable)
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

            for (int idx = 0; idx < sectionControlList.Count; idx++)
            {
                BDNodeControl control = sectionControlList[idx];
                removeNodeControlForTableSection(control, false);
            }
            sectionControlList.Clear();

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
                    addNodeControlForTableSection(node, idxSection++);
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

        private BDNodeControl addNodeControlForTableSection(BDNode pNode, int pTabIndex)
        {
            BDNodeControl nodeControl = null;

            if (CreateCurrentObject())
            {
                nodeControl = new BDNodeControl( );

                nodeControl.Dock = DockStyle.Top;
                nodeControl.TabIndex = pTabIndex;
                nodeControl.DisplayOrder = pTabIndex;
                nodeControl.AssignParentInfo(currentTable.Uuid, currentTable.NodeType);
                nodeControl.AssignDataContext(dataContext);
                nodeControl.AssignScopeId(scopeId);
                nodeControl.ShowAsChild = false;
                nodeControl.CurrentNode = pNode;
                nodeControl.DefaultLayoutVariantType = this.defaultLayoutVariantType;
                nodeControl.DefaultNodeType = BDConstants.BDNodeType.BDTableSection;

                nodeControl.RequestItemAdd += new EventHandler(TableSection_RequestItemAdd);
                nodeControl.RequestItemDelete += new EventHandler(TableSection_RequestItemDelete);
                nodeControl.ReorderToNext += new EventHandler(TableSection_ReorderToNext);
                nodeControl.ReorderToPrevious += new EventHandler(TableSection_ReorderToPrevious);
                nodeControl.NotesChanged += new EventHandler(notesChanged_Action);
                sectionControlList.Add(nodeControl);

                pnlSections.Controls.Add(nodeControl);
                nodeControl.BringToFront();
                nodeControl.RefreshLayout();
            }
            return nodeControl;
        }

        private void removeNodeControlForTableSection(BDNodeControl pNodeControl, bool pDeleteRecord)
        {
            this.Controls.Remove(pNodeControl);

            pNodeControl.RequestItemAdd -= new EventHandler(TableSection_RequestItemAdd);
            pNodeControl.RequestItemDelete -= new EventHandler(TableSection_RequestItemDelete);
            pNodeControl.ReorderToNext -= new EventHandler(TableSection_ReorderToNext);
            pNodeControl.ReorderToPrevious -= new EventHandler(TableSection_ReorderToPrevious);
            pNodeControl.NotesChanged -= new EventHandler(notesChanged_Action);

            sectionControlList.Remove(pNodeControl);

            if (pDeleteRecord)
            {
                BDNode node = (BDNode)pNodeControl.CurrentNode;
                if (null != node)
                {
                    BDNode.Delete(dataContext, node, pDeleteRecord);

                    for (int idx = 0; idx < sectionControlList.Count; idx++)
                    {
                        sectionControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pNodeControl.Dispose();
            pNodeControl = null;
        }

        private void reorderNodeControlForTableSection(BDNodeControl pNodeControl, int pOffset)
        {
            int currentPosition = sectionControlList.FindIndex(t => t == pNodeControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition += pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < sectionControlList.Count))
                {
                    sectionControlList[requestedPosition].CreateCurrentObject();
                    sectionControlList[requestedPosition].DisplayOrder = currentPosition;

                    sectionControlList[requestedPosition].CurrentNode.DisplayOrder = currentPosition;
                    BDNode.Save(dataContext, sectionControlList[requestedPosition].CurrentNode as BDNode);

                    sectionControlList[currentPosition].CreateCurrentObject();
                    sectionControlList[currentPosition].DisplayOrder = requestedPosition;
                    sectionControlList[currentPosition].CurrentNode.DisplayOrder = requestedPosition;
                    BDNode.Save(dataContext, sectionControlList[currentPosition].CurrentNode as BDNode);

                    BDNodeControl temp = sectionControlList[requestedPosition];
                    sectionControlList[requestedPosition] = sectionControlList[currentPosition];
                    sectionControlList[currentPosition] = temp;

                    int zOrder = pnlSections.Controls.GetChildIndex(pNodeControl);
                    zOrder = zOrder + (pOffset * -1);
                    pnlSections.Controls.SetChildIndex(pNodeControl, zOrder);
                }
            }
        }

        private void TableSection_RequestItemAdd(object sender, EventArgs e)
        {
            BDNodeControl control = addNodeControlForTableSection(null, sectionControlList.Count);
            if (null != control)
                control.Focus();
        }

        private void TableSection_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
            BDNodeControl control = sender as BDNodeControl;
            if(null != control)
            {
                if (MessageBox.Show("Delete Table Section?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removeNodeControlForTableSection(control, true);
            }
        }

        private void TableSection_ReorderToPrevious(object sender, EventArgs e)
        {
            BDNodeControl control = sender as BDNodeControl;
            if (null != control)
            {
                reorderNodeControlForTableSection(control, -1);
            }
        }

        private void TableSection_ReorderToNext(object sender, EventArgs e)
        {
            BDNodeControl control = sender as BDNodeControl;
            if (null != control)
            {
                reorderNodeControlForTableSection(control, 1);
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
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;

            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < sectionControlList.Count; idx++)
                {
                    sectionControlList[idx].ShowLinksInUse(true);
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
            ShowLinksInUse(true);
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
