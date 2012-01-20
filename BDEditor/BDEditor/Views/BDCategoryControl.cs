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
    public partial class BDCategoryControl : UserControl, IBDControl
    {
        private BDCategory currentCategory;
        private Entities dataContext;
        private Guid? scopeId;

        public event EventHandler NotesChanged;

        protected virtual void OnNotesChanged(EventArgs e)
        {
            if (null != NotesChanged) { NotesChanged(this, e); }
        }

        public BDCategory CurrentCategory
        {
            get
            {
                return currentCategory;
            }
            set
            {
                currentCategory = value;
                if (currentCategory == null)
                    tbCategoryName.Text = @"";
                else
                    tbCategoryName.Text = currentCategory.name;
            }
        }

        public BDCategoryControl()
        {
            InitializeComponent();
        }

        private void BDCategoryControl_Load(object sender, EventArgs e)
        {
            if (currentCategory != null)
                tbCategoryName.Text = currentCategory.name;
        }

        #region IBDControl
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignParentId(Guid? pParentId)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void RefreshLayout()
        {
            if (currentCategory == null)
            {
                tbCategoryName.Text = @"";
            }
            else
            {
                this.BackColor = SystemColors.Control;

                tbCategoryName.Text = currentCategory.name;
            }
            ShowLinksInUse(false);
        }

        public bool CreateCurrentObject()
        {
            throw new NotImplementedException();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentCategory) ? this.currentCategory.uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? Constants.ACTIVELINK_COLOR : Constants.INACTIVELINK_COLOR;
        }
        #endregion

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        private void CreateLink(string pProperty)
        {
                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignContextEntityKeyName(BDCategory.KEY_NAME);
                view.AssignScopeId(scopeId);
                view.AssignLinkedNoteType(LinkedNoteType.Footnote, true, true);
                view.NotesChanged += new EventHandler(notesChanged_Action);
                if (null != currentCategory)
                {
                    view.AssignParentId(currentCategory.uuid);
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

        private void notesChanged_Action(object sender, EventArgs e)
        {
            OnNotesChanged(new EventArgs());
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
