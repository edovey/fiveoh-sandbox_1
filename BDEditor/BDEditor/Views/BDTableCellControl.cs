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
    public partial class BDTableCellControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDTableCell currentTableCell;
        private Guid? parentId;
        private Guid? scopeId;
        private BDConstants.TableCellAlignment alignment;
        private List<BDStringControl> stringControlList = new List<BDStringControl>();

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

        public BDTableCell CurrentTableCell
        {
            get { return currentTableCell; }
            set { currentTableCell = value; }
        }

        public BDTableCellControl()
        {
            InitializeComponent();
        }

        public BDTableCellControl(Entities pDataContext, BDTableCell pCell)
        {
            dataContext = pDataContext;
            currentTableCell = pCell;
            parentId = pCell.parentId;
            alignment = (BDConstants.TableCellAlignment) pCell.alignment;
            InitializeComponent();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < stringControlList.Count; idx++)
                {
                    stringControlList[idx].ShowLinksInUse(true);
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
            RefreshLayout(true);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            ControlHelper.SuspendDrawing(this);

            for (int i = 0; i < stringControlList.Count; i++)
            {
                BDStringControl control = stringControlList[i];
                removeStringControl(control, false);
            }

            if (currentTableCell != null && pShowChildren)
            {
                List<BDString> list = BDString.RetrieveStringsForParentId(dataContext, currentTableCell.Uuid);
                int iDetail = 0;
                foreach (BDString entry in list)
                    addStringControl(entry, iDetail++);
            }

            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentTableCell)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentTableCell = BDTableCell.CreateTableCell(this.dataContext);
                    currentTableCell.SetParent(parentId);
                    currentTableCell.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    //currentTableCell.LayoutVariant = DefaultLayoutVariantType;
                }
            }

            return result;
        }

        private BDStringControl addStringControl(BDString pString, int pTabIndex)
        {
            BDStringControl stringControl = null;

            if (CreateCurrentObject())
            {
                stringControl = new BDStringControl();

                stringControl.Dock = DockStyle.Top;
                stringControl.TabIndex = pTabIndex;
                stringControl.DisplayOrder = pTabIndex;
                stringControl.AssignParentInfo(currentTableCell.Uuid, BDConstants.BDNodeType.None);
                stringControl.AssignDataContext(dataContext);
                stringControl.AssignScopeId(scopeId);
                stringControl.CurrentString = pString;
                stringControl.DefaultLayoutVariantType = this.DefaultLayoutVariantType;
                stringControl.RequestItemAdd += new EventHandler<NodeEventArgs>(String_RequestItemAdd);
                stringControl.RequestItemDelete += new EventHandler<NodeEventArgs>(String_RequestItemDelete);
                stringControl.ReorderToNext += new EventHandler<NodeEventArgs>(String_ReorderToNext);
                stringControl.ReorderToPrevious += new EventHandler<NodeEventArgs>(String_ReorderToPrevious);
                stringControl.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);

                stringControlList.Add(stringControl);

                pnlControls.Controls.Add(stringControl);
                stringControl.BringToFront();

                stringControl.RefreshLayout();
            }

            return stringControl;
        }

        /// <summary>
        /// Remove control from panel & from controls list.  Deregister event handlers.  Create delete record for entry if requested.
        /// </summary>
        /// <param name="pControl"></param>
        /// <param name="pDeleteRecord"></param>
        private void removeStringControl(BDStringControl pControl, bool pDeleteRecord)
        {
            pnlControls.Controls.Remove(pControl);

            pControl.RequestItemAdd -= new EventHandler<NodeEventArgs>(String_RequestItemAdd);
            pControl.RequestItemDelete -= new EventHandler<NodeEventArgs>(String_RequestItemDelete);
            pControl.ReorderToNext -= new EventHandler<NodeEventArgs>(String_ReorderToNext);
            pControl.ReorderToPrevious -= new EventHandler<NodeEventArgs>(String_ReorderToPrevious);
            pControl.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);

            stringControlList.Remove(pControl);

            if (pDeleteRecord)
            {
                BDString entry = pControl.CurrentString;
                if (null != entry)
                {
                    BDString.Delete(dataContext, entry, pDeleteRecord);
                    for (int idx = 0; idx < stringControlList.Count; idx++)
                    {
                        stringControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pControl.Dispose();
            pControl = null;
        }

        private void reorderStringControl(BDStringControl pControl, int pOffset)
        {
            int currentPosition = stringControlList.FindIndex(t => t == pControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < stringControlList.Count))
                {
                    stringControlList[requestedPosition].CreateCurrentObject();
                    stringControlList[requestedPosition].DisplayOrder = currentPosition;

                    stringControlList[requestedPosition].CurrentString.displayOrder = currentPosition;
                    BDString.Save(dataContext, stringControlList[requestedPosition].CurrentString);


                    stringControlList[currentPosition].CreateCurrentObject();
                    stringControlList[currentPosition].DisplayOrder = requestedPosition;

                    stringControlList[currentPosition].CurrentString.displayOrder = requestedPosition;
                    BDString.Save(dataContext, stringControlList[currentPosition].CurrentString);

                    BDStringControl temp = stringControlList[requestedPosition];
                    stringControlList[requestedPosition] = stringControlList[currentPosition];
                    stringControlList[currentPosition] = temp;

                    int zOrder = pnlControls.Controls.GetChildIndex(pControl);
                    zOrder = zOrder + (pOffset * -1);
                    pnlControls.Controls.SetChildIndex(pControl, zOrder);
                }
            }
        }
        
        public bool Save()
        {
            bool result = false;
            if (null != parentId)
            {
                foreach (BDStringControl control in stringControlList)
                {
                    result = control.Save() || result;
                }

                if(result && (null == currentTableCell))
                    CreateCurrentObject();
                if(null != currentTableCell)
                {
                    currentTableCell.alignment = (int)TableCellAlignment;
                    BDTableCell.Save(dataContext, currentTableCell);
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
            //parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        #endregion


        private void BDTherapyControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void String_RequestItemAdd(object sender, EventArgs e)
        {
            BDStringControl control = addStringControl(null, stringControlList.Count);
            if (null != control)
            {
                control.Focus();
            }
        }

        private void String_RequestItemDelete(object sender, EventArgs e)
        {
            BDStringControl control = sender as BDStringControl;
            if (null != control)
            {
                if (MessageBox.Show("Delete Text?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removeStringControl(control, true);
            }
        }

        private void String_ReorderToNext(object sender, NodeEventArgs e)
        {
            BDStringControl control = sender as BDStringControl;
            if (null != control)
            {
                reorderStringControl(control, 1);
            }
        }

        private void String_ReorderToPrevious(object sender, NodeEventArgs e)
        {
            BDStringControl control = sender as BDStringControl;
            if (null != control)
            {
                reorderStringControl(control, -1);
            }
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, CurrentTableCell.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, CurrentTableCell.Uuid));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDTherapy, DefaultLayoutVariantType));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new NodeEventArgs(dataContext, currentTableCell.Uuid));
        }

        public override string ToString()
        {
            return (null == this.currentTableCell) ? "No Cell" : this.currentTableCell.Uuid.ToString();
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }
        public BDConstants.TableCellAlignment TableCellAlignment { get; set; }

        BDConstants.LayoutVariantType IBDControl.DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool ShowAsChild { get; set; }

        private void BDTableCellControl_Leave(object sender, EventArgs e)
        {
            Save();
        }
    }
}
