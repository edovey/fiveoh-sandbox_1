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
        private Guid? pathogenGroupId;
        private BDPathogen currentPathogen;
        private Guid? scopeId;
        public int? DisplayOrder { get; set; }

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;
        public event EventHandler ReorderToPrevious;
        public event EventHandler ReorderToNext;
        public event EventHandler NotesChanged;
        public event EventHandler<SearchableItemEventArgs> SearchableItemAdded;

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

        protected virtual void OnSearchableItemAdded(SearchableItemEventArgs se)
        {
            // make a copy of the handler to avoid race condition
            EventHandler<SearchableItemEventArgs> handler = SearchableItemAdded;

            if (null != handler)
            {
                handler(this, se);
            }
        }

        public BDPathogen CurrentPathogen
        {
            get { return currentPathogen; }
            set { currentPathogen = value; }
        }

         #endregion

        public BDPathogenControl()
        {
            InitializeComponent();

            btnLink.Tag = BDPathogen.PROPERTYNAME_NAME;
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
                BDPathogen.Save(dataContext, currentPathogen);
                result = true;

                Typeahead.AddToCollection(BDPathogen.KEY_NAME, BDPathogen.PROPERTYNAME_NAME, currentPathogen.name);
            }

            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void AssignParentId(Guid? pParentId)
        {
            pathogenGroupId = pParentId;
            this.Enabled = (null != pathogenGroupId);
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentPathogen)
            {
                if (null == this.pathogenGroupId)
                {
                    result = false;
                }
                else
                {
                    this.currentPathogen = BDPathogen.CreatePathogen(dataContext, pathogenGroupId.Value);
                    this.currentPathogen.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;

                    // raise event to begin metadata entry creation
                    OnSearchableItemAdded(new SearchableItemEventArgs(this.currentPathogen.uuid, BDPathogen.KEY_NAME));
                }
            }

            return result;
        }

        public void RefreshLayout()
        {
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
        }
        

        private void BDPathogenControl_Leave(object sender, EventArgs e)
        {
            Save();
        }        
        
        #endregion

        #region Class methods
        private void CreateLink()
        {
            CreateCurrentObject();
            Save();
            BDLinkedNoteView noteView = new BDLinkedNoteView();
            noteView.AssignDataContext(dataContext);
            noteView.AssignParentId(currentPathogen.uuid);
            noteView.AssignContextEntityKeyName(BDPathogen.KEY_NAME);
            noteView.AssignContextPropertyName(BDPathogen.PROPERTYNAME_NAME);
            noteView.AssignScopeId(scopeId);
            noteView.NotesChanged += new EventHandler(notesChanged_Action);

            if (null != currentPathogen)
            {
                noteView.AssignParentId(currentPathogen.uuid);
            }
            else
            {
                noteView.AssignParentId(null);
            }

            noteView.PopulateControl();
            noteView.ShowDialog(this);
            noteView.NotesChanged -= new EventHandler(notesChanged_Action);
            ShowLinksInUse(false);
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
            btnLink.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLink.Tag) ? Constants.ACTIVELINK_COLOR : Constants.INACTIVELINK_COLOR;
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

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void notesChanged_Action(object sender, EventArgs e)
        {
            //ShowLinksInUse(true);
            OnNotesChanged(new EventArgs());
        }
    }
}
