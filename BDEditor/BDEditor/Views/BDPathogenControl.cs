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
                BDPathogen.SavePathogen(dataContext, currentPathogen);
                result = true;
            }

            return result;
        }

        public void Delete()
        {
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
            showLinksInUse();
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
            noteView.AssignContextEntityName(BDPathogen.ENTITYNAME_FRIENDLY);
            noteView.AssignContextPropertyName(BDPathogen.PROPERTYNAME_NAME);
            noteView.AssignScopeId(scopeId);

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
            showLinksInUse();
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

        private void showLinksInUse()
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentId(dataContext, (null != this.currentPathogen) ? this.currentPathogen.uuid : Guid.Empty);
            btnLink.BackColor = links.Exists(x => x.parentEntityPropertyName == (string)btnLink.Tag) ? Constants.ACTIVELINK_COLOR : Constants.INACTIVELINK_COLOR;
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
    }
}
