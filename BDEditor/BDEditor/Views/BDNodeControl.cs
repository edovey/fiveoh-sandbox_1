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
        private BDLinkedNote overviewLinkedNote;
        private Guid? scopeId;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;
        private BDConstants.LayoutVariantType defaultLayoutVariantType;
        private BDConstants.BDNodeType defaultNodeType;

        public int? DisplayOrder { get; set; }

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

        public void RefreshLayout()
        {
            this.SuspendLayout();

            bdLinkedNoteControl1.AssignDataContext(dataContext);
            if (currentNode == null)
            {
                tbName.Text = @"";
                overviewLinkedNote = null;

                bdLinkedNoteControl1.CurrentLinkedNote = null;
                bdLinkedNoteControl1.AssignParentInfo(null, defaultNodeType);
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
            }
            bdLinkedNoteControl1.RefreshLayout();

            ShowLinksInUse(false);
            this.ResumeLayout();

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
            defaultLayoutVariantType = pNode.LayoutVariant;
            parentId = pNode.ParentId;
            defaultNodeType = pNode.NodeType;

            InitializeComponent();
        }

        /// <summary>
        /// Initialize form without an existing BDNode
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pDefaultNodeType"></param>
        /// <param name="pDefaultLayoutType"></param>
        /// <param name="pParentId"></param>
        public BDNodeControl(Entities pDataContext, BDConstants.BDNodeType pDefaultNodeType, BDConstants.LayoutVariantType pDefaultLayoutType, Guid pParentId)
        {    
            dataContext = pDataContext;
            currentNode = null;
            defaultLayoutVariantType = pDefaultLayoutType;
            parentId = pParentId;
            defaultNodeType = pDefaultNodeType;

            InitializeComponent();
        }

        private void BDNodeControl_Load(object sender, EventArgs e)
        {
            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
            lblNode.Text = BDUtilities.GetEnumDescription(defaultNodeType);

            if (null != currentNode)
            {
                if(tbName.Text != currentNode.Name) tbName.Text = currentNode.Name;
            }
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
                    switch (defaultNodeType)
                    {
                        case BDConstants.BDNodeType.BDTherapy:
                            currentNode = BDTherapy.CreateTherapy(dataContext, parentId.Value);
                            break;
                        case BDConstants.BDNodeType.BDTherapyGroup:
                            currentNode = BDTherapyGroup.CreateTherapyGroup(dataContext, parentId.Value);
                            break;
                        default:
                            currentNode = BDNode.CreateNode(this.dataContext, this.defaultNodeType);
                            break;
                    }
                    
                    this.currentNode.DisplayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentNode.LayoutVariant = this.defaultLayoutVariantType;
                }
            }

            return result;
        }

        private void CreateLink(string pProperty)
        {
            if(CreateCurrentObject())
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
                CreateLink(tag);
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
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void BDNodeControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void tbName_Leave(object sender, EventArgs e)
        {
            Save();
        }
    }
}
