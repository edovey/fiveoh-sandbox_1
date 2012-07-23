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
    public partial class BDCombinedEntryControl : UserControl, IBDControl
    {
        protected Entities dataContext;
        protected BDCombinedEntry currentEntry;
        protected Guid? scopeId;
        protected Guid parentId;
        protected BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;

        protected List<BDCombinedEntryFieldControl> fieldControlList = new List<BDCombinedEntryFieldControl>();

        private BDConstants.BDNodeType defaultNodeType;
        private BDConstants.LayoutVariantType defaultLayoutVariantType;
        private int? displayOrder;
        private bool showAsChild = false;

        public BDCombinedEntryControl()
        {
            InitializeComponent();
        }

        public BDCombinedEntryControl(Entities pDataContext, BDCombinedEntry pCombinedEntry, Guid? pScopeId)
        {
            InitializeComponent();

            if (null == pCombinedEntry) throw new NotSupportedException("May not create a CombinedEntry control without an existing entry");
            if (null == pCombinedEntry.ParentId) throw new NotSupportedException("May not create a CombinedEntry control without a supplied parent");

            dataContext = pDataContext;
            currentEntry = pCombinedEntry;
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

        public bool Gather(BDCombinedEntry pEntry)
        {
            bool result = false;

            if (null != pEntry)
            {
                result = true;
            }
            return result;
        }

        private BDCombinedEntryFieldControl addFieldEntryControl(BDCombinedEntry pConfiguredEntry, string pPropertyName, int pTabIndex)
        {
            BDCombinedEntryFieldControl control = new BDCombinedEntryFieldControl(dataContext, pConfiguredEntry, pPropertyName, scopeId.Value);

            //if (null != control)
            //{
            //    ((System.Windows.Forms.UserControl)control).Dock = DockStyle.Top;
            //    ((System.Windows.Forms.UserControl)control).TabIndex = pTabIndex;
            //    control.DisplayOrder = pTabIndex;

            //    control.NotesChanged += new EventHandler<NodeEventArgs>(fieldControl_NotesChanged);
            //    control.NameChanged += new EventHandler<NodeEventArgs>(fieldControl_NameChanged);

            //    fieldControlList.Add(control);

            //    panelFields.Controls.Add(((System.Windows.Forms.UserControl)control));

            //    ((System.Windows.Forms.UserControl)control).BringToFront();
            //    control.RefreshLayout();
            //}

            return control;
        }

        private void removeFieldEntryControl(BDCombinedEntryFieldControl pFieldEntryControl)
        {
            ControlHelper.SuspendDrawing(this);
            //panelFields.Controls.Remove(((System.Windows.Forms.UserControl)pFieldEntryControl));

            //pFieldEntryControl.NameChanged -= new EventHandler<NodeEventArgs>(fieldControl_NameChanged);
            //pFieldEntryControl.NotesChanged -= new EventHandler<NodeEventArgs>(fieldControl_NotesChanged);

            //fieldControlList.Remove(pFieldEntryControl);

            //((System.Windows.Forms.UserControl)pFieldEntryControl).Dispose();
            //pFieldEntryControl = null;
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
            if (null == pParentId) throw new NotSupportedException("May not assign a null parentId to a CombinedEntryControl");
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
            bool result = false;

            if (Gather(currentEntry))
            {
                BDCombinedEntry.Save(dataContext, currentEntry);
                result = true;
            }

            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool CreateCurrentObject()
        {
            throw new NotImplementedException();
        }

        public void RefreshLayout()
        {
            RefreshLayout(true);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            ControlHelper.SuspendDrawing(this);

            for (int idx = 0; idx < fieldControlList.Count; idx++)
            {
                BDCombinedEntryFieldControl control = fieldControlList[idx];
                removeFieldEntryControl(control);
            }
            fieldControlList.Clear();
            panelFields.Controls.Clear();

            List<BDLayoutMetadataColumn> metaDataColumnList = BDLayoutMetadataColumn.RetrieveListForLayout(dataContext, currentEntry.LayoutVariant);
            int tabIndex = 0;
            foreach (BDLayoutMetadataColumn columnDef in metaDataColumnList)
            {
                BDLayoutMetadataColumnNodeType propertyInfo = BDLayoutMetadataColumnNodeType.Retrieve(dataContext, currentEntry.LayoutVariant, currentEntry.NodeType, columnDef.Uuid);
                if (null != propertyInfo)
                {
                    addFieldEntryControl(currentEntry, propertyInfo.propertyName, tabIndex++);
                }
            }

            ControlHelper.ResumeDrawing(this);
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            
        }

        public BDConstants.BDNodeType DefaultNodeType
        {
            get { return defaultNodeType; }
            set { defaultNodeType = value; }
        }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType
        {
            get { return defaultLayoutVariantType; }
            set { defaultLayoutVariantType = value; }
        }

        public IBDNode CurrentNode
        {
            get { return currentEntry; }
            set { currentEntry = value as BDCombinedEntry; }
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
    }
}
