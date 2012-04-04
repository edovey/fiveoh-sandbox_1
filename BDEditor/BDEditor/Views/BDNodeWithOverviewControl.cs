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
    public partial class BDNodeWithOverviewControl : UserControl, IBDControl
    {
        private IBDNode currentNode;
        private Entities dataContext;
        private BDLinkedNote overviewLinkedNote;
        private Guid? scopeId;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;
        private bool showAsChild = false;
        public BDConstants.LayoutVariantType DefaultLayoutVariantType;
        public BDConstants.BDNodeType DefaultNodeType;

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

        public bool ShowAsChild
        {
            get { return showAsChild; }
            set { showAsChild = value; }
        }

        public IBDNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public BDNodeWithOverviewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize form with existing BDNode
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pNode"></param>
        public BDNodeWithOverviewControl(Entities pDataContext, IBDNode pNode)
        {
            dataContext = pDataContext;
            currentNode = pNode;
            DefaultLayoutVariantType = pNode.LayoutVariant;
            parentId = pNode.ParentId;
            DefaultNodeType = pNode.NodeType;

            InitializeComponent();
        }

        private void BDNodeControl_Load(object sender, EventArgs e)
        {
            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
            setFormLayoutState();
            if (null != currentNode)
            {
                if(tbName.Text != currentNode.Name) tbName.Text = currentNode.Name;
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

            bdLinkedNoteControl1.AssignDataContext(dataContext);
            if (currentNode == null)
            {
                tbName.Text = @"";
                overviewLinkedNote = null;

                bdLinkedNoteControl1.CurrentLinkedNote = null;
                bdLinkedNoteControl1.AssignParentInfo(null, DefaultNodeType);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
            }
            else
            {
                tbName.Text = currentNode.Name;
                bdLinkedNoteControl1.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);

                BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentNode.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
                if (null != association)
                {
                    overviewLinkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, association.linkedNoteId);
                    bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                }
            bdLinkedNoteControl1.RefreshLayout();

            ShowLinksInUse(false);
            }
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
                    if (currentNode.Name != tbName.Text)
                    {
                        currentNode.Name = tbName.Text;
                        OnNameChanged( new NodeEventArgs(dataContext, currentNode.Uuid, currentNode.Name));
                    }

                    bdLinkedNoteControl1.Save();

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
            
            // Note:  this control contains no children
            /* if (pPropagateToChildren)
            {
                for (int idx = 0; idx < pathogenGroupControlList.Count; idx++)
                {
                    pathogenGroupControlList[idx].ShowLinksInUse(true);
                }
            } */

        }

        #endregion

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private void setFormLayoutState()
        {
            btnMenuLeft.Enabled = !showAsChild;
            btnMenuLeft.Visible = !showAsChild;
            lblNode.Text = BDUtilities.GetEnumDescription(DefaultNodeType);
            lblNode.Enabled = !showAsChild;
            lblNode.Visible = !showAsChild;
            lblOverview.Visible = !showAsChild;
            
            btnMenuRight.Enabled = showAsChild;
            btnMenuRight.Visible = showAsChild;
            lblNodeAsChild.Text = @"Table Row Title";
            lblNodeAsChild.Enabled = showAsChild;
            lblNodeAsChild.Visible = showAsChild;

        }

        private void createLink(string pProperty)
        {
            if(CreateCurrentObject())
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

        private void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OnItemAddRequested(new EventArgs());
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
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


        BDConstants.BDNodeType IBDControl.DefaultNodeType { get; set; }

        BDConstants.LayoutVariantType IBDControl.DefaultLayoutVariantType { get; set; }
    }
}
