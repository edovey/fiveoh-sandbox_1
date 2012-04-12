using System;
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
        private BDTableRow currentTableRow;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private List<BDTableCellControl> cellControlList = new List<BDTableCellControl>();

        public int? DisplayOrder { get; set; }
        public BDConstants.LayoutVariantType DefaultLayoutVariantType;

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

        public BDTableRow CurrentTableRow
        {
            get { return currentTableRow; }
            set { currentTableRow = value; }
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
            this.SuspendLayout();

            /*
            for (int i = 0; i < cellControlList.Count; i++)
            {
                BDTableCellControl control = cellControlList[i];
                removeTableCellControl(control, false);
            }

            //cellControlList.Clear();
           // pnlControls.Controls.Clear();
            */
            ShowLinksInUse(false);
            this.ResumeLayout();
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentTableRow)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentTableRow = BDTableRow.CreateTableRow(this.dataContext,this.DefaultNodeType);
                    currentTableRow.SetParent(DefaultNodeType, parentId);
                    currentTableRow.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentTableRow.LayoutVariant = DefaultLayoutVariantType;
                }
            }

            return result;
        }

/*        private BDTableCellControl addTableCellControl(BDTableCell pCell, int pTabIndex)
        {
            BDTableCellControl stringControl = null;

            if (CreateCurrentObject())
            {
                stringControl = new BDTableCellControl();

                stringControl.Dock = DockStyle.Top;
                stringControl.TabIndex = pTabIndex;
                stringControl.DisplayOrder = pTabIndex;
                stringControl.AssignParentInfo(currentTableRow.Uuid, BDConstants.BDNodeType.None);
                stringControl.AssignDataContext(dataContext);
                stringControl.AssignScopeId(scopeId);
                stringControl.CurrentTableCell = pCell;
                stringControl.DefaultLayoutVariantType = this.DefaultLayoutVariantType;
                stringControl.RequestItemAdd += new EventHandler<NodeEventArgs>(TableCell_RequestItemAdd);
                stringControl.RequestItemDelete += new EventHandler<NodeEventArgs>(TableCell_RequestItemDelete);
                stringControl.ReorderToNext += new EventHandler<NodeEventArgs>(TableCell_ReorderToNext);
                stringControl.ReorderToPrevious += new EventHandler<NodeEventArgs>(TableCell_ReorderToPrevious);
                stringControl.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);

                cellControlList.Add(stringControl);

                this.Controls.Add(stringControl);
                stringControl.BringToFront();

                stringControl.RefreshLayout();
            }

            return stringControl;
        }

        private void removeTableCellControl(BDTableCellControl pControl, bool pDeleteRecord)
        {
           // this.Controls.Remove(pControl);

            pControl.RequestItemAdd -= new EventHandler<NodeEventArgs>(TableCell_RequestItemAdd);
            pControl.RequestItemDelete -= new EventHandler<NodeEventArgs>(TableCell_RequestItemDelete);
            pControl.ReorderToNext -= new EventHandler<NodeEventArgs>(TableCell_ReorderToNext);
            pControl.ReorderToPrevious -= new EventHandler<NodeEventArgs>(TableCell_ReorderToPrevious);
            pControl.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);

            cellControlList.Remove(pControl);

            if (pDeleteRecord)
            {
                BDTableCell entry = pControl.CurrentTableCell;
                if (null != entry)
                {
                    BDTableCell.Delete(dataContext, entry, pDeleteRecord);
                    for (int idx = 0; idx < cellControlList.Count; idx++)
                    {
                        cellControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pControl.Dispose();
            pControl = null;
        }

        private void reorderTableCellControl(BDTableCellControl pControl, int pOffset)
        {
            int currentPosition = cellControlList.FindIndex(t => t == pControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < cellControlList.Count))
                {
                    cellControlList[requestedPosition].CreateCurrentObject();
                    cellControlList[requestedPosition].DisplayOrder = currentPosition;

                    cellControlList[requestedPosition].CurrentTableCell.displayOrder = currentPosition;
                    BDTableCell.Save(dataContext, cellControlList[requestedPosition].CurrentTableCell);


                    cellControlList[currentPosition].CreateCurrentObject();
                    cellControlList[currentPosition].DisplayOrder = requestedPosition;

                    cellControlList[currentPosition].CurrentTableCell.displayOrder = requestedPosition;
                    BDTableCell.Save(dataContext, cellControlList[currentPosition].CurrentTableCell);

                    BDTableCellControl temp = cellControlList[requestedPosition];
                    cellControlList[requestedPosition] = cellControlList[currentPosition];
                    cellControlList[currentPosition] = temp;

                    int zOrder = this.Controls.GetChildIndex(pControl);
                    zOrder = zOrder + (pOffset * -1);
                    this.Controls.SetChildIndex(pControl, zOrder);
                }
            }
        } */
        
        public bool Save()
        {
            bool result = false;
            if (null != parentId)
            {
                foreach (BDTableCellControl control in cellControlList)
                {
                    result = control.Save() || result;
                }

                if(result && (null == currentTableRow))
                    CreateCurrentObject();
                if(null != currentTableRow)
                {
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

/*        private void TableCell_RequestItemAdd(object sender, EventArgs e)
        {
            BDTableCellControl control = addTableCellControl(null, cellControlList.Count);
            if (null != control)
            {
                control.Focus();
            }
        }

        private void TableCell_RequestItemDelete(object sender, EventArgs e)
        {
            BDTableCellControl control = sender as BDTableCellControl;
            if (null != control)
            {
                if (MessageBox.Show("Delete Cell?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removeTableCellControl(control, true);
            }
        }

        private void TableCell_ReorderToNext(object sender, NodeEventArgs e)
        {
            BDTableCellControl control = sender as BDTableCellControl;
            if (null != control)
            {
                reorderTableCellControl(control, 1);
            }
        }

        private void TableCell_ReorderToPrevious(object sender, NodeEventArgs e)
        {
            BDTableCellControl control = sender as BDTableCellControl;
            if (null != control)
            {
                reorderTableCellControl(control, -1);
            }
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, CurrentTableRow.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, CurrentTableRow.Uuid));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDTableRow, DefaultLayoutVariantType));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new NodeEventArgs(dataContext, currentTableRow.Uuid));
        } */

        public override string ToString()
        {
            return (null == this.currentTableRow) ? "No Table Row" : this.currentTableRow.Uuid.ToString();
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }
        public BDConstants.TableRowType TableRowType { get; set; }

        BDConstants.LayoutVariantType IBDControl.DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool ShowAsChild { get; set; }

        private void BDTableRowControl_Leave(object sender, EventArgs e)
        {
            Save();
        }
    }
}
