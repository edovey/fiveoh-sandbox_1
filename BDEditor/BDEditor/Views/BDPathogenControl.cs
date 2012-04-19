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
    public partial class BDPathogenControl : UserControl, IBDControl
    {
        #region Class properties

        private Entities dataContext;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private BDNode currentPathogen;
        public BDConstants.LayoutVariantType DefaultLayoutVariantType;
        private Guid? scopeId;
        public int? DisplayOrder { get; set; }

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

        public BDNode CurrentPathogen
        {
            get { return currentPathogen; }
            set { currentPathogen = value; }
        }

         #endregion

        public BDPathogenControl()
        {
            InitializeComponent();

            btnLink.Tag = BDNode.PROPERTYNAME_NAME;
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

        public bool Save()
        {
            bool result = false;

            if ((null == currentPathogen) && (tbPathogenName.Text != string.Empty))
            {
                CreateCurrentObject();
            }
            if (null != currentPathogen)
            {
                if(currentPathogen.name != tbPathogenName.Text) currentPathogen.name = tbPathogenName.Text;
                if (currentPathogen.displayOrder != DisplayOrder) currentPathogen.displayOrder = DisplayOrder;
                BDNode.Save(dataContext, currentPathogen);
                result = true;

                Typeahead.AddToCollection(BDConstants.BDNodeType.BDPathogen, BDNode.PROPERTYNAME_NAME, currentPathogen.name);
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

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentPathogen)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    this.currentPathogen = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPathogen);
                    this.currentPathogen.SetParent(BDConstants.BDNodeType.BDPathogenGroup, parentId);
                    this.currentPathogen.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentPathogen.LayoutVariant = DefaultLayoutVariantType;
                    
                    BDNode.Save(dataContext, currentPathogen);
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            ControlHelper.SuspendDrawing(this);
            if (currentPathogen == null)
            {
                this.tbPathogenName.Text = @"";
            }
            else
            {
                this.tbPathogenName.Text = currentPathogen.name;
                DisplayOrder = currentPathogen.displayOrder;
            }
            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);
        }
        

        private void BDPathogenControl_Leave(object sender, EventArgs e)
        {
            Save();
        }        
        
        #endregion

        #region Class methods
        private void CreateLink()
        {
            if (CreateCurrentObject())
            {
                Save();
                BDLinkedNoteView noteView = new BDLinkedNoteView();
                noteView.AssignDataContext(dataContext);
                noteView.AssignParentInfo(currentPathogen.Uuid, currentPathogen.NodeType);
                noteView.AssignContextPropertyName(BDNode.PROPERTYNAME_NAME);
                noteView.AssignScopeId(scopeId);
                noteView.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                noteView.ShowDialog(this);
                noteView.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource)
        {
            tbPathogenName.AutoCompleteCustomSource = pSource;
            tbPathogenName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbPathogenName.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        #endregion

        private void btnLink_Click(object sender, EventArgs e)
        {
            if(this.Enabled)
                CreateLink();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentPathogen) ? this.currentPathogen.uuid : Guid.Empty);
            btnLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }  

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            this.btnLink.Enabled = (null != textBox);
        }

        public override string ToString()
        {
            return (null == this.currentPathogen) ? "No Pathogen" : this.currentPathogen.name;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDPathogen, DefaultLayoutVariantType));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new NodeEventArgs(dataContext, CurrentPathogen.Uuid));
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, CurrentPathogen.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, CurrentPathogen.Uuid));
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void notesChanged_Action(object sender, NodeEventArgs e)
        {
            //ShowLinksInUse(true);
            OnNotesChanged(e);
        }

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPathogenName, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPathogenName, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPathogenName, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPathogenName, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPathogenName, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPathogenName, "±");
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPathogenName, "¹");
        }


        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        BDConstants.LayoutVariantType IBDControl.DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                return CurrentPathogen;
            }
            set
            {
                CurrentPathogen = value as BDNode;
            }
        }

        public bool ShowAsChild { get; set; }
    }
}
