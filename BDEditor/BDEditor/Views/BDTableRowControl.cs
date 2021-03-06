﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using System.Diagnostics;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDTableRowControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private IBDNode currentNode;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private List<IBDControl> cellControlList = new List<IBDControl>();
        private TextBox textControl;

        public int? DisplayOrder { get; set; }

        private bool showChildren = true;
        public bool ShowChildren
        {
            get { return showChildren; }
            set { showChildren = value; }
        }

        public event EventHandler<NodeEventArgs> RequestItemAdd;
        public event EventHandler<NodeEventArgs> RequestItemDelete;

        public event EventHandler<NodeEventArgs> ReorderToPrevious;
        public event EventHandler<NodeEventArgs> ReorderToNext;

        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            throw new NotImplementedException();
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

        public BDTableRowControl()
        {
            InitializeComponent();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < cellControlList.Count; idx++)
                {
                    cellControlList[idx].ShowLinksInUse(true);
                }
            }
        }     
        
        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }


        #region IBDControl

        public void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            bool origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);

            pnlControls.Controls.Clear();
            addTableRowControls();

            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);

            BDCommon.Settings.IsUpdating = origState;
        }

        private void addTableRowControls()
        {
            if (null != currentNode)
            {
                int maxColumns = BDFabrik.GetTableColumnCount(currentNode.LayoutVariant);
                List<BDTableCell> list = BDTableCell.RetrieveTableCellsForParentId(dataContext, currentNode.Uuid);
                for (int i = 0; i < maxColumns; i++)
                {
                    IBDControl newControl = BDFabrik.CreateControlForNode(dataContext, list[i]);
                    if (null != newControl)
                    {
                        BDTableCell cell = list[i];
                        ((System.Windows.Forms.UserControl)newControl).Dock = DockStyle.Left;
                        ((System.Windows.Forms.UserControl)newControl).TabIndex = cell.displayOrder.Value;
                        newControl.DisplayOrder = cell.displayOrder;
                        newControl.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
                        newControl.AssignDataContext(dataContext);
                        newControl.AssignScopeId(scopeId);
                        newControl.CurrentNode = list[i];
                        newControl.DefaultLayoutVariantType = cell.LayoutVariant;
                        newControl.DefaultNodeType = cell.NodeType;
                        newControl.ShowAsChild = true;

                        pnlControls.Controls.Add((System.Windows.Forms.UserControl)newControl);
                        ((System.Windows.Forms.UserControl)newControl).BringToFront();
                        newControl.CurrentNode = cell;

                        cellControlList.Add(newControl);
                        newControl.RefreshLayout();
                    }
                }
           }
        }

        private BDNodeOverviewControl addNodeOverviewControl(BDTableCell cell)
        {
            BDNodeOverviewControl cellControl = new BDNodeOverviewControl();
            ((System.Windows.Forms.UserControl)cellControl).Dock = DockStyle.Right;
            ((System.Windows.Forms.UserControl)cellControl).TabIndex = cell.DisplayOrder.Value;
            cellControl.DisplayOrder = cell.DisplayOrder;
            cellControl.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
            cellControl.AssignDataContext(dataContext);
            cellControl.AssignScopeId(scopeId);
            cellControl.ShowAsChild = true;
            cellControl.CurrentNode = cell;
            cellControl.RefreshLayout();

            cellControlList.Add(cellControl);
            return cellControl;
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
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
                    this.currentNode = BDFabrik.CreateNode(dataContext, DefaultNodeType, parentId, parentType);

                    this.currentNode.DisplayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentNode.LayoutVariant = this.DefaultLayoutVariantType;
                }
            }

            return result;
        }

        public bool Save()
        {
            if (BDCommon.Settings.IsUpdating) return false;

            bool result = false;
            if (null != parentId)
            {
                foreach (BDTableCellControl control in cellControlList)
                {
                    result = control.Save() || result;
                }
                BDTableRow currentTableRow = currentNode as BDTableRow;
               
                if (result && (null == currentTableRow))
                    CreateCurrentObject();
                if (null != currentTableRow && null != textControl)
                {
                    if(currentTableRow.Name != textControl.Text)
                        currentTableRow.Name = textControl.Text;
                    BDTableRow.Save(dataContext, currentTableRow);
                    result = true;
                }
            }

            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        #endregion


        private void BDTherapyControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        public override string ToString()
        {
            return (null == this.currentNode) ? "No Table Row" : this.currentNode.Uuid.ToString();
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }
        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public bool ShowAsChild { get; set; }

        private void BDTableRowControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDTableRow, DefaultLayoutVariantType));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Guid? uuid = null;
            if (null != this.currentNode) uuid = this.currentNode.Uuid;

            OnItemDeleteRequested(new NodeEventArgs(dataContext, uuid));
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, currentNode.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, currentNode.Uuid));
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            editIndexStripMenuItem.Tag = new BDNodeWrapper(currentNode, currentNode.NodeType, currentNode.LayoutVariant, null);
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void BDTableRowControl_Load(object sender, EventArgs e)
        {
            if (null != CurrentNode)
            {
                switch (CurrentNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.Antibiotics_NameListing_HeaderRow:
                    case BDConstants.LayoutVariantType.Antibiotics_Stepdown_HeaderRow:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional_HeaderRow:
                    case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_2_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_3_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_4_Column_HeaderRow:
                    case BDConstants.LayoutVariantType.Table_5_Column_HeaderRow:
                        this.BackColor = SystemColors.ControlDark;
                        break;
                    default:
                        this.BackColor = SystemColors.Control;
                        break;
                }
            }
        }

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
    }
}
