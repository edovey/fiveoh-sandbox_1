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
    public partial class BDPresentationControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;
        private BDNode currentPresentation;
        private BDLinkedNote overviewLinkedNote;
        private Guid? scopeId;
        private BDConstants.LayoutVariantType defaultLayoutVariantType;

        public int? DisplayOrder { get; set; }

        private List<BDPathogenGroupControl> pathogenGroupControlList = new List<BDPathogenGroupControl>();

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

        public BDNode CurrentPresentation
        {
            get { return currentPresentation; }
            set { currentPresentation = value; }
        }

        public BDPresentationControl()
        {
            InitializeComponent();
            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
        }

        public BDPresentationControl(Entities pDataContext, BDNode pNode)
        {
            InitializeComponent();
            dataContext = pDataContext;
            defaultLayoutVariantType = pNode.LayoutVariant;
            
            currentPresentation = pNode;

            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
        }

        public BDPresentationControl(Entities pDataContext, BDConstants.LayoutVariantType pDefaultLayoutVariantType)
        {
            InitializeComponent();
            dataContext = pDataContext;
            defaultLayoutVariantType = pDefaultLayoutVariantType;

            btnLinkedNote.Tag = BDNode.PROPERTYNAME_NAME;
        }

        private void BDPresentationControl_Load(object sender, EventArgs e)
        {
            bdLinkedNoteControl1.AssignDataContext(dataContext);
            if(null != currentPresentation)
            {
                bdLinkedNoteControl1.AssignParentInfo(currentPresentation.Uuid, currentPresentation.NodeType);
            }
            
            bdLinkedNoteControl1.AssignScopeId(scopeId);
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
                if ((null == currentPresentation) && (tbPresentationName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }
                if (null != currentPresentation)
                {
                    if (currentPresentation.name != tbPresentationName.Text)
                    {
                        currentPresentation.name = tbPresentationName.Text;
                        OnNameChanged(new NodeEventArgs(dataContext, currentPresentation.Uuid, currentPresentation.Name));
                    }

                    bdLinkedNoteControl1.Save();

                    foreach (BDPathogenGroupControl control in pathogenGroupControlList)
                    {
                        result = control.Save() || result;
                    }
                  
                    System.Diagnostics.Debug.WriteLine(@"Presentation Control Save");
                    BDNode.Save(dataContext, currentPresentation);
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

            if (null == this.currentPresentation)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    BDMetadata parentMetaData = BDMetadata.GetMetadataWithItemId(dataContext, parentId);
                    this.currentPresentation = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPresentation);
                    this.currentPresentation.SetParent(BDConstants.BDNodeType.BDDisease, parentId.Value);
                    this.currentPresentation.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentPresentation.LayoutVariant = defaultLayoutVariantType;

                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            ControlHelper.SuspendDrawing(this);

            if (null == currentPresentation)
            {
                tbPresentationName.Text = @"";
                overviewLinkedNote = null;

                bdLinkedNoteControl1.AssignDataContext(dataContext);
                bdLinkedNoteControl1.AssignParentInfo(null, BDConstants.BDNodeType.BDPresentation);
                bdLinkedNoteControl1.AssignScopeId(null);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
            }
            else
            {
                tbPresentationName.Text = currentPresentation.name;
                List<IBDNode> list = BDFabrik.GetChildrenForParent(dataContext, currentPresentation);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    IBDNode listEntry = list[idx];
                    if (listEntry.NodeType == (BDConstants.BDNodeType.BDPathogenGroup))
                    {
                        BDNode node = listEntry as BDNode;
                        addPathogenGroupControl(node, idx);
                    }
                }

                bdLinkedNoteControl1.AssignDataContext(dataContext);
                bdLinkedNoteControl1.AssignParentInfo(currentPresentation.Uuid, BDConstants.BDNodeType.BDPresentation);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);


                BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentPresentation.uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
                if (null != association)
                {
                    overviewLinkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, association.linkedNoteId);
                    bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                }
            }

            bdLinkedNoteControl1.RefreshLayout();

            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);
        }


        #endregion

        private void BDPresentationControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private BDPathogenGroupControl addPathogenGroupControl(BDNode pNode, int pTabIndex)
        {
            BDPathogenGroupControl pathogenGroupControl = null;
            if (CreateCurrentObject())
            {
                pathogenGroupControl = new BDPathogenGroupControl();

                pathogenGroupControl.Dock = DockStyle.Top;
                pathogenGroupControl.TabIndex = pTabIndex;
                pathogenGroupControl.DisplayOrder = pTabIndex;
                pathogenGroupControl.AssignParentInfo(currentPresentation.Uuid, currentPresentation.NodeType);
                pathogenGroupControl.AssignDataContext(dataContext);
                pathogenGroupControl.AssignScopeId(scopeId);
                pathogenGroupControl.AssignTypeaheadSource(BDTypeahead.PathogenGroups);
                pathogenGroupControl.CurrentPathogenGroup = pNode;
                pathogenGroupControl.DefaultLayoutVariantType = this.defaultLayoutVariantType;

                pathogenGroupControl.RequestItemAdd += new EventHandler<NodeEventArgs>(PathogenGroup_RequestItemAdd);
                pathogenGroupControl.RequestItemDelete += new EventHandler<NodeEventArgs>(PathogenGroup_RequestItemDelete);
                pathogenGroupControl.ReorderToNext += new EventHandler<NodeEventArgs>(PathogenGroup_ReorderToNext);
                pathogenGroupControl.ReorderToPrevious += new EventHandler<NodeEventArgs>(PathogenGroup_ReorderToPrevious);
                pathogenGroupControl.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);

                pathogenGroupControlList.Add(pathogenGroupControl);

                panelPathogenGroups.Controls.Add(pathogenGroupControl);
                pathogenGroupControl.BringToFront();
                pathogenGroupControl.RefreshLayout();
            }
            return pathogenGroupControl;
        }

        private void removePathogenGroupControl(BDPathogenGroupControl pPathogenGroupControl, bool pDeleteRecord)
        {
            panelPathogenGroups.Controls.Remove(pPathogenGroupControl);

            pPathogenGroupControl.RequestItemAdd -= new EventHandler<NodeEventArgs>(PathogenGroup_RequestItemAdd);
            pPathogenGroupControl.RequestItemDelete -= new EventHandler<NodeEventArgs>(PathogenGroup_RequestItemDelete);
            pPathogenGroupControl.ReorderToNext -= new EventHandler<NodeEventArgs>(PathogenGroup_ReorderToNext);
            pPathogenGroupControl.ReorderToPrevious -= new EventHandler<NodeEventArgs>(PathogenGroup_ReorderToPrevious);
            pPathogenGroupControl.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);

            pathogenGroupControlList.Remove(pPathogenGroupControl);
            
            if (pDeleteRecord)
            {
                BDNode entry = pPathogenGroupControl.CurrentPathogenGroup;
                if (null != entry)
                {
                    BDNode.Delete(dataContext, entry, pDeleteRecord);
                    for (int idx = 0; idx < pathogenGroupControlList.Count; idx++)
                    {
                        pathogenGroupControlList[idx].DisplayOrder = idx;
                    }
                }
            }
            pPathogenGroupControl.Dispose();
            pPathogenGroupControl = null;
        }

        private void ReorderPathogenGroupControl(BDPathogenGroupControl pPathogenGroupControl, int pOffset)
        {
            int currentPosition = pathogenGroupControlList.FindIndex(t => t == pPathogenGroupControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < pathogenGroupControlList.Count))
                {
                    pathogenGroupControlList[requestedPosition].CreateCurrentObject();
                    pathogenGroupControlList[requestedPosition].DisplayOrder = currentPosition;

                    pathogenGroupControlList[requestedPosition].CurrentPathogenGroup.displayOrder = currentPosition;
                    BDNode.Save(dataContext, pathogenGroupControlList[requestedPosition].CurrentPathogenGroup);

                    pathogenGroupControlList[currentPosition].CreateCurrentObject();
                    pathogenGroupControlList[currentPosition].DisplayOrder = requestedPosition;

                    pathogenGroupControlList[currentPosition].CurrentPathogenGroup.displayOrder = requestedPosition;
                    BDNode.Save(dataContext, pathogenGroupControlList[currentPosition].CurrentPathogenGroup);

                    BDPathogenGroupControl temp = pathogenGroupControlList[requestedPosition];
                    pathogenGroupControlList[requestedPosition] = pathogenGroupControlList[currentPosition];
                    pathogenGroupControlList[currentPosition] = temp;

                    int zOrder = panelPathogenGroups.Controls.GetChildIndex(pPathogenGroupControl);
                    zOrder = zOrder + (pOffset * -1);
                    panelPathogenGroups.Controls.SetChildIndex(pPathogenGroupControl, zOrder);
                }
            }
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentPresentation) ? this.currentPresentation.uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < pathogenGroupControlList.Count; idx++)
                {
                    pathogenGroupControlList[idx].ShowLinksInUse(true);
                }
            }
        }

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private void CreateLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();

                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentPresentation.Uuid, currentPresentation.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private void PathogenGroup_RequestItemAdd(object sender, EventArgs e)
        {
            BDPathogenGroupControl control = addPathogenGroupControl(null, pathogenGroupControlList.Count);
            if (null != control)
            {
                control.Focus();
            }
        }

        private void PathogenGroup_RequestItemDelete(object sender, NodeEventArgs e)
        {
            BDPathogenGroupControl control = sender as BDPathogenGroupControl;
            if (null != control)
            {
                if (MessageBox.Show("Delete Pathogen Group?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removePathogenGroupControl(control, true);
            }
        }

        private void PathogenGroup_ReorderToNext(object sender, NodeEventArgs e)
        {
            BDPathogenGroupControl control = sender as BDPathogenGroupControl;
            if (null != control)
            {
                ReorderPathogenGroupControl(control, 1);
            }
        }

        private void PathogenGroup_ReorderToPrevious(object sender, NodeEventArgs e)
        {
            BDPathogenGroupControl control = sender as BDPathogenGroupControl;
            if (null != control)
            {
                ReorderPathogenGroupControl(control, -1);
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void notesChanged_Action(object sender, NodeEventArgs e)
        {
            ShowLinksInUse(true);
            OnNotesChanged(e);
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

        private void tbPresentationName_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPresentationName, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPresentationName, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPresentationName, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPresentationName, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPresentationName, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPresentationName, "±");
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbPresentationName, "¹");
        }

        private void tbPresentationName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                undoToolStripMenuItem.Enabled = tbPresentationName.CanUndo;
                pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
                cutToolStripMenuItem.Enabled = (tbPresentationName.SelectionLength > 0);
                copyToolStripMenuItem.Enabled = (tbPresentationName.SelectionLength > 0);
                deleteToolStripMenuItem.Enabled = (tbPresentationName.SelectionLength > 0);
            }
        }


        public void AssignDataContext(Entities pDataContext)
        {
            throw new NotImplementedException();
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }


        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                return CurrentPresentation;
            }
            set
            {
                CurrentPresentation = value as BDNode;
            }
        }

        public bool ShowAsChild { get; set; }

    }
}
