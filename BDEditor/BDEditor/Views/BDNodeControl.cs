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
        private Constants.BDNodeType parentType = Constants.BDNodeType.None;
        private Constants.LayoutVariantType defaultLayoutVariantType;
        private Constants.BDNodeType defaultNodeType;

        public int? DisplayOrder { get; set; }

        public event EventHandler NotesChanged;

        protected virtual void OnNotesChanged(EventArgs e)
        {
            if (null != NotesChanged) { NotesChanged(this, e); }
        }

        public IBDNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public void RefreshLayout()
        {
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
        public BDNodeControl(Entities pDataContext, Constants.BDNodeType pDefaultNodeType, Constants.LayoutVariantType pDefaultLayoutType, Guid pParentId)
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
            btnLinkedNote.Tag = BDNode.VIRTUALPROPERTYNAME_OVERVIEW;

            if (null != currentNode)
            {
                switch (this.defaultNodeType)
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
            }

            if (null != currentNode)
            {
                if(tbName.Text != currentNode.Name) tbName.Text = currentNode.Name;
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
                    if (currentNode.Name != tbName.Text) currentNode.Name = tbName.Text;

                    bdLinkedNoteControl1.Save();

                    switch (currentNode.NodeType)
                    {
                        case Constants.BDNodeType.BDTherapy:
                            BDTherapy therapy = currentNode as BDTherapy;
                            if (null != therapy)
                            {
                                BDTherapy.Save(dataContext, therapy);
                            }
                            break;
                        case Constants.BDNodeType.BDTherapyGroup:
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
                        case Constants.BDNodeType.BDTherapy:
                            currentNode = BDTherapy.CreateTherapy(dataContext, parentId.Value);
                            break;
                        case Constants.BDNodeType.BDTherapyGroup:
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
                view.AssignLinkedNoteType(LinkedNoteType.Footnote, true, true);
                view.NotesChanged += new EventHandler(notesChanged_Action);
                view.PopulateControl();
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler(notesChanged_Action);
                ShowLinksInUse(false);
            }
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
    }
}
