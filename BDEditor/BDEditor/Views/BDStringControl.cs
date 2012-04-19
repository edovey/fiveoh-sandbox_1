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
    public partial class BDStringControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDString currentString;
        private Guid? parentId;
        private Guid? scopeId;

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

         public BDString CurrentString
        {
            get { return currentString; }
            set { currentString = value; }
        }

        BDConstants.LayoutVariantType IBDControl.DefaultLayoutVariantType { get; set; }

        public BDStringControl()
        {
            InitializeComponent();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentString) ? this.currentString.uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }


        private void insertText(RichTextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private void createLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();

                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentString.Uuid, BDConstants.BDNodeType.None);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

       public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            //pParentType = pParentType;
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        public bool Save()
        {
            bool result = false;
            if(null != parentId)
            {
                if(null == currentString && !string.IsNullOrEmpty(rtbValue.Text))
                    CreateCurrentObject();

                if(null != currentString)
                    if(currentString.value != rtbValue.Text) currentString.value = rtbValue.Text;

                BDString.Save(dataContext, currentString);
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
            bool result = true;
            if (null == this.currentString)
            {
                if (null == this.parentId)
                    result = false;
                else
                {
                    currentString = BDString.CreateString(this.dataContext, this.parentId.Value);
                    currentString.SetParent(parentId);
                    currentString.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                }
            }
            return result;
        }

        public void RefreshLayout()
        {
            ControlHelper.SuspendDrawing(this);
            if (currentString == null)
                rtbValue.Text = string.Empty;
            else
            {
                this.BackColor = SystemColors.Control;
                rtbValue.Text = currentString.value;
                DisplayOrder = currentString.displayOrder;
            }
            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);
        }

        public BDConstants.BDNodeType DefaultNodeType
        {
            get { return BDConstants.BDNodeType.None; }
            set { throw new NotImplementedException(); }
        }

        public IBDNode CurrentNode
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool ShowAsChild
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(rtbValue, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(rtbValue, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(rtbValue, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(rtbValue, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(rtbValue, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(rtbValue, "±");
        }

        private void rtbValue_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                undoToolStripMenuItem.Enabled = rtbValue.CanUndo;
                pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
                cutToolStripMenuItem.Enabled = (rtbValue.SelectionLength > 0);
                copyToolStripMenuItem.Enabled = (rtbValue.SelectionLength > 0);
                deleteToolStripMenuItem.Enabled = (rtbValue.SelectionLength > 0);
            }
        }

        private void BDStringControl_Leave(object sender, EventArgs e)
        {
            Save();
        }
    }
}
