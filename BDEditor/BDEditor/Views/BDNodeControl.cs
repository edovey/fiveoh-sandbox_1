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
        private BDNode currentNode;
        private Entities dataContext;
        private BDLinkedNote overviewLinkedNote;
        private IBDNode node;
        private Guid? scopeId;
        private Guid? parentId;
        private Constants.BDNodeType parentType = Constants.BDNodeType.None;
        private Constants.LayoutVariantType defaultLayoutVariantType;
        private Constants.BDNodeType nodeType;

        public int? DisplayOrder { get; set; }

        public event EventHandler NotesChanged;

        protected virtual void OnNotesChanged(EventArgs e)
        {
            if (null != NotesChanged) { NotesChanged(this, e); }
        }

        public BDNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public void RefreshLayout()
        {
            if (currentNode == null)
            {
                tbName.Text = @"";
                overviewLinkedNote = null;

                bdLinkedNoteControl1.CurrentLinkedNote = null;
                bdLinkedNoteControl1.AssignParentId(null);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextNodeType(node.NodeType);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
            }
            else
            {
                tbName.Text = currentNode.name;
                bdLinkedNoteControl1.AssignParentId(currentNode.uuid);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextNodeType(node.NodeType);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);

                BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentNode.uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
                if (null != association)
                {
                    overviewLinkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, association.linkedNoteId);
                    bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                }
            }
            bdLinkedNoteControl1.RefreshLayout();
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
            node = pNode;             
            
            InitializeComponent();
}

        /// <summary>
        /// Initialize form without an existing BDNode
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pNodeType"></param>
        /// <param name="pDefaultLayoutType"></param>
        /// <param name="pParentId"></param>
        public BDNodeControl(Entities pDataContext, Constants.BDNodeType pNodeType, Constants.LayoutVariantType pDefaultLayoutType, Guid pParentId)
        {    
            dataContext = pDataContext;
            currentNode = null;
            defaultLayoutVariantType = pDefaultLayoutType;
            parentId = pParentId;
            nodeType = pNodeType;

            InitializeComponent();
        }

        private void BDNodeControl_Load(object sender, EventArgs e)
        {
            if (null != node)
            {
                this.nodeType = node.NodeType;

                switch (this.nodeType)
                {
                    case Constants.BDNodeType.BDSection:
                    case Constants.BDNodeType.BDCategory:
                    case Constants.BDNodeType.BDSubCategory:
                        lblOverview.Visible = false;
                        bdLinkedNoteControl1.Visible = false;
                        bdLinkedNoteControl1.Enabled = false;
                        break;
                    case Constants.BDNodeType.BDDisease:
                    default:
                        break;
                }

                currentNode = node as BDNode;
                parentId = currentNode.ParentId;

                this.defaultLayoutVariantType = node.LayoutVariant;
            }

            if (currentNode != null)
            {
                if(tbName.Text != currentNode.name) tbName.Text = currentNode.name;
            }
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {

        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        #region IBDControl

        public void AssignParentInfo(Guid? pParentId, Constants.BDNodeType pParentType)
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
                if ((null == currentNode) && (tbName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if(null != currentNode)
                {
                    if(currentNode.name != tbName.Text) currentNode.name = tbName.Text;
                    bdLinkedNoteControl1.Save();
                }
                System.Diagnostics.Debug.WriteLine(@"Node Control Save");
                BDNode.Save(dataContext, currentNode);
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
                    this.currentNode = BDNode.CreateNode(this.dataContext, this.nodeType);
                    this.currentNode.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentNode.LayoutVariant = this.defaultLayoutVariantType;
                }
            }

            return result;
        }

        private void CreateLink(string pProperty)
        {
            BDLinkedNoteView view = new BDLinkedNoteView();
            view.AssignDataContext(dataContext);
            view.AssignContextPropertyName(pProperty);
            view.AssignContextEntityNodeType(currentNode.NodeType);
            view.AssignScopeId(scopeId);
            view.AssignLinkedNoteType(LinkedNoteType.Footnote, true, true);
            view.NotesChanged += new EventHandler(notesChanged_Action);
            if (null != currentNode)
            {
                view.AssignParentId(currentNode.Uuid);
            }
            else
            {
                view.AssignParentId(null);
            }
            view.PopulateControl();
            view.ShowDialog(this);
            view.NotesChanged -= new EventHandler(notesChanged_Action);
            ShowLinksInUse(false);
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {

        }

        #endregion

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
                    control.AssignParentId(this.currentNode.Uuid);
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
    }
}
