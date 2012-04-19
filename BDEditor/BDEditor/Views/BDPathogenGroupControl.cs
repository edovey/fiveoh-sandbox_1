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
    public partial class BDPathogenGroupControl : UserControl, IBDControl
    {
        #region Class properties
        private Entities dataContext;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private BDNode currentPathogenGroup;
        private Guid? scopeId;
        public int? DisplayOrder { get; set; }

        private List<BDPathogenControl> pathogenControlList = new List<BDPathogenControl>();
        private List<BDTherapyGroupControl> therapyGroupControlList = new List<BDTherapyGroupControl>();

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

        public BDNode CurrentPathogenGroup
        {
            get { return currentPathogenGroup; }
            set { currentPathogenGroup = value; }
        }

        #endregion

        public BDPathogenGroupControl()
        {
            InitializeComponent();
            btnPathogenGroupLink.Tag = BDNode.PROPERTYNAME_NAME;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        public void AssignTypeaheadSource(AutoCompleteStringCollection pSource)
        {
            textBoxPathogenGroupName.AutoCompleteCustomSource = pSource;
            textBoxPathogenGroupName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxPathogenGroupName.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        #region IBDControl
        
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

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
               foreach (BDPathogenControl control in pathogenControlList)
                {
                    result = control.Save() || result;
                }     

                foreach (BDTherapyGroupControl control in therapyGroupControlList)
                {
                    result = control.Save() || result;
                }

                // if zero pathogens are defined then this is a valid test
                if ((result && (null == currentPathogenGroup) || (null == currentPathogenGroup) && textBoxPathogenGroupName.Text != string.Empty)) 
                {
                    CreateCurrentObject();
                }

                if (null != currentPathogenGroup)
                {
                    System.Diagnostics.Debug.WriteLine(@"PathogenGroup Control Save");

                    if (currentPathogenGroup.name != textBoxPathogenGroupName.Text) currentPathogenGroup.name = textBoxPathogenGroupName.Text;
                    if (currentPathogenGroup.displayOrder != DisplayOrder) currentPathogenGroup.displayOrder = DisplayOrder;

                    BDNode.Save(dataContext, currentPathogenGroup);

                    Typeahead.AddToCollection(BDConstants.BDNodeType.BDPathogenGroup, BDNode.PROPERTYNAME_NAME, currentPathogenGroup.name);

                    result = true;
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

            if (null == this.currentPathogenGroup)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    this.currentPathogenGroup = BDNode.CreateNode(dataContext, BDConstants.BDNodeType.BDPathogenGroup);
                    this.currentPathogenGroup.SetParent(parentType, parentId);
                    this.currentPathogenGroup.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentPathogenGroup.LayoutVariant = DefaultLayoutVariantType;

                    BDNode.Save(dataContext, currentPathogenGroup);
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
            this.SuspendLayout();

            for (int idx = 0; idx < pathogenControlList.Count; idx++)
            {
                BDPathogenControl control = pathogenControlList[idx];
                removePathogenControl(control, false);
            }
            pathogenControlList.Clear();

            for (int idx = 0; idx < therapyGroupControlList.Count; idx++)
            {
                BDTherapyGroupControl control = therapyGroupControlList[idx];
                removeTherapyGroupControl(control, false);
            }
            therapyGroupControlList.Clear();
            panelTherapyGroups.Controls.Clear();

            if (null == currentPathogenGroup)
            {
                textBoxPathogenGroupName.Text = @"";
            }
            else
            {
                // This is assuming Constants.LayoutVariantType.TreatmentRecommendation01
                textBoxPathogenGroupName.Text = currentPathogenGroup.name;
                List<IBDNode> list = BDFabrik.GetChildrenForParent(dataContext, currentPathogenGroup);
                int idxPathogen = 0;
                int idxTherapyGroup = 0;
                foreach(IBDNode listEntry in list)
                {
                    switch (listEntry.NodeType)
                    {
                        case BDConstants.BDNodeType.BDPathogen:
                            BDNode node = listEntry as BDNode;
                            addPathogenControl(node, idxPathogen++);
                            break;
                        case BDConstants.BDNodeType.BDTherapyGroup:
                            BDTherapyGroup therapyGroup = listEntry as BDTherapyGroup;
                            addTherapyGroupControl(therapyGroup, idxTherapyGroup++);
                            break;
                    }
                }
            }

            ShowLinksInUse(false);

            this.ResumeLayout();
        }

        #endregion    

        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private BDPathogenControl addPathogenControl(BDNode pNode, int pTabIndex)
        {
            BDPathogenControl pathogenControl = null;

            if (CreateCurrentObject())
            {
                pathogenControl = new BDPathogenControl();

                pathogenControl.Dock = DockStyle.Top;
                pathogenControl.TabIndex = pTabIndex;
                pathogenControl.DisplayOrder = pTabIndex;
                pathogenControl.AssignParentInfo(currentPathogenGroup.Uuid, currentPathogenGroup.NodeType);
                pathogenControl.AssignDataContext(dataContext);
                pathogenControl.AssignScopeId(scopeId);
                pathogenControl.AssignTypeaheadSource(Typeahead.Pathogens);
                pathogenControl.CurrentPathogen = pNode;
                pathogenControl.DefaultLayoutVariantType = this.DefaultLayoutVariantType;
                pathogenControl.RequestItemAdd += new EventHandler<NodeEventArgs>(Pathogen_RequestItemAdd);
                pathogenControl.RequestItemDelete += new EventHandler<NodeEventArgs>(Pathogen_RequestItemDelete);
                pathogenControl.ReorderToNext += new EventHandler<NodeEventArgs>(Pathogen_ReorderToNext);
                pathogenControl.ReorderToPrevious += new EventHandler<NodeEventArgs>(Pathogen_ReorderToPrevious);
                pathogenControl.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                pathogenControlList.Add(pathogenControl);

                panelPathogens.Controls.Add(pathogenControl);
                pathogenControl.BringToFront();
                pathogenControl.RefreshLayout();
            }
            return pathogenControl;
        }

        private void removePathogenControl(BDPathogenControl pPathogenControl, bool pDeleteRecord)
        {
            this.Controls.Remove(pPathogenControl);

            pPathogenControl.RequestItemAdd -= new EventHandler<NodeEventArgs>(Pathogen_RequestItemAdd);
            pPathogenControl.RequestItemDelete -= new EventHandler<NodeEventArgs>(Pathogen_RequestItemDelete);
            pPathogenControl.ReorderToNext -= new EventHandler<NodeEventArgs>(Pathogen_ReorderToNext);
            pPathogenControl.ReorderToPrevious -= new EventHandler<NodeEventArgs>(Pathogen_ReorderToPrevious);
            pPathogenControl.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);

            pathogenControlList.Remove(pPathogenControl);

            if (pDeleteRecord)
            {
                BDNode node = pPathogenControl.CurrentPathogen;
                if (null != node)
                {
                    BDNode.Delete(dataContext, node, pDeleteRecord);

                    for (int idx = 0; idx < pathogenControlList.Count; idx++)
                    {
                        pathogenControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pPathogenControl.Dispose();
            pPathogenControl = null;
        }

        private void ReorderPathogenControl(BDPathogenControl pPathogenControl, int pOffset)
        {
            int currentPosition = pathogenControlList.FindIndex(t => t == pPathogenControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < pathogenControlList.Count))
                {
                    pathogenControlList[requestedPosition].CreateCurrentObject();
                    pathogenControlList[requestedPosition].DisplayOrder = currentPosition;
                    pathogenControlList[requestedPosition].CurrentPathogen.displayOrder = currentPosition;
                    BDNode.Save(dataContext, pathogenControlList[requestedPosition].CurrentPathogen);

                    pathogenControlList[currentPosition].CreateCurrentObject();
                    pathogenControlList[currentPosition].DisplayOrder = requestedPosition;
                    pathogenControlList[currentPosition].CurrentPathogen.displayOrder = requestedPosition;
                    BDNode.Save(dataContext, pathogenControlList[currentPosition].CurrentPathogen);

                    BDPathogenControl temp = pathogenControlList[requestedPosition];
                    pathogenControlList[requestedPosition] = pathogenControlList[currentPosition];
                    pathogenControlList[currentPosition] = temp;

                    int zOrder = panelPathogens.Controls.GetChildIndex(pPathogenControl);
                    zOrder = zOrder + (pOffset * -1);
                    panelPathogens.Controls.SetChildIndex(pPathogenControl, zOrder);
                }
            }
        }


        private BDTherapyGroupControl addTherapyGroupControl(BDTherapyGroup pTherapyGroup, int pTabIndex)
        {
            BDTherapyGroupControl therapyGroupControl = null;

            if (CreateCurrentObject())
            {
                therapyGroupControl = new BDTherapyGroupControl();

                therapyGroupControl.Dock = DockStyle.Top;
                therapyGroupControl.TabIndex = pTabIndex;
                therapyGroupControl.DisplayOrder = pTabIndex;
                therapyGroupControl.AssignParentInfo(currentPathogenGroup.uuid, currentPathogenGroup.NodeType);
                therapyGroupControl.AssignScopeId(scopeId);
                therapyGroupControl.AssignDataContext(dataContext);
                therapyGroupControl.AssignTypeaheadSource(Typeahead.TherapyGroups);
                therapyGroupControl.CurrentTherapyGroup = pTherapyGroup;
                therapyGroupControl.DefaultLayoutVariantType = this.DefaultLayoutVariantType;
                therapyGroupControl.RequestItemAdd += new EventHandler<NodeEventArgs>(TherapyGroup_RequestItemAdd);
                therapyGroupControl.RequestItemDelete += new EventHandler<NodeEventArgs>(TherapyGroup_RequestItemDelete);
                therapyGroupControl.ReorderToNext += new EventHandler<NodeEventArgs>(TherapyGroup_ReorderToNext);
                therapyGroupControl.ReorderToPrevious += new EventHandler<NodeEventArgs>(TherapyGroup_ReorderToPrevious);
                therapyGroupControl.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);

                therapyGroupControlList.Add(therapyGroupControl);

                panelTherapyGroups.Controls.Add(therapyGroupControl);
                therapyGroupControl.BringToFront();

                therapyGroupControl.RefreshLayout();
            }

            return therapyGroupControl;
        }

        private void removeTherapyGroupControl(BDTherapyGroupControl pTherapyGroupControl, bool pDeleteRecord)
        {
            panelTherapyGroups.Controls.Remove(pTherapyGroupControl);

            pTherapyGroupControl.RequestItemAdd -= new EventHandler<NodeEventArgs>(TherapyGroup_RequestItemAdd);
            pTherapyGroupControl.RequestItemDelete -= new EventHandler<NodeEventArgs>(TherapyGroup_RequestItemDelete);
            pTherapyGroupControl.ReorderToNext -= new EventHandler<NodeEventArgs>(TherapyGroup_ReorderToNext);
            pTherapyGroupControl.ReorderToPrevious -= new EventHandler<NodeEventArgs>(TherapyGroup_ReorderToPrevious);
            pTherapyGroupControl.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);

            therapyGroupControlList.Remove(pTherapyGroupControl);

            if (pDeleteRecord)
            {
                BDTherapyGroup entry = pTherapyGroupControl.CurrentTherapyGroup;
                if (null != entry)
                {
                    BDTherapyGroup.Delete(dataContext, entry, pDeleteRecord);
                    for (int idx = 0; idx < therapyGroupControlList.Count; idx++)
                    {
                        therapyGroupControlList[idx].DisplayOrder = idx;
                    }
                }
            }

            pTherapyGroupControl.Dispose();
            pTherapyGroupControl = null;
        }

        private void ReorderTherapyGroupControl(BDTherapyGroupControl pTherapyGroupControl, int pOffset)
        {
            int currentPosition = therapyGroupControlList.FindIndex(t => t == pTherapyGroupControl);
            if (currentPosition >= 0)
            {
                int requestedPosition = currentPosition + pOffset;
                if ((requestedPosition >= 0) && (requestedPosition < therapyGroupControlList.Count))
                {
                    therapyGroupControlList[requestedPosition].CreateCurrentObject();
                    therapyGroupControlList[requestedPosition].DisplayOrder = currentPosition;

                    therapyGroupControlList[requestedPosition].CurrentTherapyGroup .displayOrder = currentPosition;
                    BDTherapyGroup.Save(dataContext, therapyGroupControlList[requestedPosition].CurrentTherapyGroup);

                    therapyGroupControlList[currentPosition].CreateCurrentObject();
                    therapyGroupControlList[currentPosition].DisplayOrder = requestedPosition;

                    therapyGroupControlList[currentPosition].CurrentTherapyGroup.displayOrder = requestedPosition;
                    BDTherapyGroup.Save(dataContext, therapyGroupControlList[currentPosition].CurrentTherapyGroup);

                    BDTherapyGroupControl temp = therapyGroupControlList[requestedPosition];
                    therapyGroupControlList[requestedPosition] = therapyGroupControlList[currentPosition];
                    therapyGroupControlList[currentPosition] = temp;

                    int zOrder = panelTherapyGroups.Controls.GetChildIndex(pTherapyGroupControl);
                    zOrder = zOrder + (pOffset * -1);
                    panelTherapyGroups.Controls.SetChildIndex(pTherapyGroupControl, zOrder);
                }
            }
        }

        private void PathogenGroup_RequestItemAdd(object sender, EventArgs e)
        {
            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDPathogenGroup, DefaultLayoutVariantType));
        }

        private void PathogenGroup_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new NodeEventArgs(dataContext, CurrentPathogenGroup.Uuid));
        }

        private void PathogenGroup_ReorderToPrevious(object sender, NodeEventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, CurrentPathogenGroup.Uuid));
        }

        private void PathogenGroup_ReorderToNext(object sender, NodeEventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, CurrentPathogenGroup.Uuid));
        }

        private void Pathogen_RequestItemAdd(object sender, EventArgs e)
        {
            BDPathogenControl control = addPathogenControl(null, pathogenControlList.Count);
        }

        private void Pathogen_RequestItemDelete(object sender, NodeEventArgs e)
        {
            BDPathogenControl control = sender as BDPathogenControl;
            if (null != control)
                if (MessageBox.Show("Delete Pathogen?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removePathogenControl(control, true);
        }

        private void Pathogen_ReorderToNext(object sender, NodeEventArgs e)
        {
            BDPathogenControl control = sender as BDPathogenControl;
            if (null != control)
            {
                ReorderPathogenControl(control, 1);
            }
        }

        private void Pathogen_ReorderToPrevious(object sender, NodeEventArgs e)
        {
            BDPathogenControl control = sender as BDPathogenControl;
            if (null != control)
            {
                ReorderPathogenControl(control, -1);
            }
        }

        private void TherapyGroup_RequestItemAdd(object sender, EventArgs e)
        {
            BDTherapyGroupControl control = addTherapyGroupControl(null, therapyGroupControlList.Count);
            if (null != control)
            {
                control.Focus();
            }
        }

        private void TherapyGroup_RequestItemDelete(object sender, NodeEventArgs e)
        {
            BDTherapyGroupControl control = sender as BDTherapyGroupControl;
            if (null != control)
            {
                if (MessageBox.Show("Delete Therapy Group?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    removeTherapyGroupControl(control, true);
            }
        }

        private void TherapyGroup_ReorderToNext(object sender, NodeEventArgs e)
        {
            BDTherapyGroupControl control = sender as BDTherapyGroupControl;
            if (null != control)
            {
                ReorderTherapyGroupControl(control, 1);
            }
        }

        private void TherapyGroup_ReorderToPrevious(object sender, NodeEventArgs e)
        {
            BDTherapyGroupControl control = sender as BDTherapyGroupControl;
            if (null != control)
            {
                ReorderTherapyGroupControl(control, -1);
            }
        }

        public override string ToString()
        {
            return (null == this.currentPathogenGroup) ? "No Pathogen Group" : this.currentPathogenGroup.uuid.ToString();
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                CreateLink(control.Tag as string);
            }
        }

        private void CreateLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();
                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentPathogenGroup.Uuid, currentPathogenGroup.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentPathogenGroup) ? this.currentPathogenGroup.uuid : Guid.Empty);
            btnPathogenGroupLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnPathogenGroupLink.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;

            if (pPropagateToChildren)
            {
                for (int idx = 0; idx < pathogenControlList.Count; idx++)
                {
                    pathogenControlList[idx].ShowLinksInUse(true);
                }

                for (int idx = 0; idx < therapyGroupControlList.Count; idx++)
                {
                    therapyGroupControlList[idx].ShowLinksInUse(true);
                }
            }
        }   

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, CurrentPathogenGroup.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, CurrentPathogenGroup.Uuid));
        }

        private void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        private void textBoxPathogenGroupName_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(textBoxPathogenGroupName, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(textBoxPathogenGroupName, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(textBoxPathogenGroupName, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(textBoxPathogenGroupName, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(textBoxPathogenGroupName, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(textBoxPathogenGroupName, "±");
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(textBoxPathogenGroupName, "¹");
        }

        private void textBoxPathogenGroupName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                undoToolStripMenuItem.Enabled = textBoxPathogenGroupName.CanUndo;
                pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
                cutToolStripMenuItem.Enabled = (textBoxPathogenGroupName.SelectionLength > 0);
                copyToolStripMenuItem.Enabled = (textBoxPathogenGroupName.SelectionLength > 0);
                deleteToolStripMenuItem.Enabled = (textBoxPathogenGroupName.SelectionLength > 0);
            }
        }


        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                return CurrentPathogenGroup;
            }
            set
            {
                CurrentPathogenGroup = value as BDNode;
            }
        }

        public bool ShowAsChild { get; set; }
    }
}
