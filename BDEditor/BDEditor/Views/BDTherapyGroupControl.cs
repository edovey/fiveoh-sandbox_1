using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDTherapyGroupControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? pathogenGroupId;
        private BDTherapyGroup currentTherapyGroup;
        private IBDControl parentControl;
        private Guid? scopeId;
        public int? DisplayOrder { get; set; }

        public event EventHandler RequestItemAdd;
        public event EventHandler RequestItemDelete;

        private List<BDTherapyControl> therapyControlList = new List<BDTherapyControl>();

        protected virtual void OnItemAddRequested(EventArgs e)
        {
            if (null != RequestItemAdd) { RequestItemAdd(this, e); }
        }

        protected virtual void OnItemDeleteRequested(EventArgs e)
        {
            if (null != RequestItemDelete) { RequestItemDelete(this, e); }
        }

        public BDTherapyGroupControl()
        {
            InitializeComponent();
        }

        public BDTherapyGroup CurrentTherapyGroup
        {
            get { return currentTherapyGroup; }
            set { currentTherapyGroup = value; }
        }

        public void RefreshLayout()
        {
            for (int idx = 0; idx < therapyControlList.Count; idx++)
            {
                BDTherapyControl control = therapyControlList[idx];
                removeTherapyControl(control, false);
            }
            therapyControlList.Clear();
            panelTherapies.Controls.Clear();

            if (null == currentTherapyGroup)
            {
                tbName.Text = @"";
                noneRadioButton.Checked = true;
            }
            else
            {
                tbName.Text = currentTherapyGroup.name;
                switch ((BDTherapyGroup.TherapyGroupJoinType)currentTherapyGroup.therapyGroupJoinType)
                {
                    case BDTherapyGroup.TherapyGroupJoinType.None:
                        noneRadioButton.Checked = true;
                        break;
                    case BDTherapyGroup.TherapyGroupJoinType.AndWithNext:
                        andRadioButton.Checked = true;
                        break;
                    case BDTherapyGroup.TherapyGroupJoinType.OrWithNext:
                        orRadioButton.Checked = true;
                        break;
                    default:
                        noneRadioButton.Checked = true;
                        break;
                }

                List<BDTherapy> list = BDTherapy.GetTherapiesForTherapyGroupId(dataContext, currentTherapyGroup.uuid);
                for (int idx = 0; idx < list.Count; idx++)
                {
                    BDTherapy entry = list[idx];
                    addTherapyControl(entry, idx);
                }
            }
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

        public void AssignParentId(Guid? pParentId)
        {
            pathogenGroupId = pParentId;
        }

        public bool Save()
        {
            bool result = false;
            if (null != pathogenGroupId)
            {
                foreach (BDTherapyControl control in therapyControlList)
                {
                    result = control.Save() || result;
                }
                // If zero therapies are defined then this is a valid test
                if ((result && (null == currentTherapyGroup)) || (null == currentTherapyGroup) && (tbName.Text != string.Empty))
                {
                    CreateCurrentObject();
                }

                if (null != currentTherapyGroup)
                {
                    if(currentTherapyGroup.name != tbName.Text) currentTherapyGroup.name = tbName.Text;

                    if (andRadioButton.Checked)
                    {
                        if(currentTherapyGroup.therapyGroupJoinType != (int)BDTherapyGroup.TherapyGroupJoinType.AndWithNext) 
                            currentTherapyGroup.therapyGroupJoinType = (int)BDTherapyGroup.TherapyGroupJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        if(currentTherapyGroup.therapyGroupJoinType != (int)BDTherapyGroup.TherapyGroupJoinType.OrWithNext) 
                            currentTherapyGroup.therapyGroupJoinType = (int)BDTherapyGroup.TherapyGroupJoinType.OrWithNext;
                    }
                    else
                    {
                        if(currentTherapyGroup.therapyGroupJoinType != (int)BDTherapyGroup.TherapyGroupJoinType.None) 
                            currentTherapyGroup.therapyGroupJoinType = (int)BDTherapyGroup.TherapyGroupJoinType.None;
                    }
                    
                    BDTherapyGroup.SaveTherapyGroup(dataContext, currentTherapyGroup);
                    result = true;
                }
            }

            return result;
        }

        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentTherapyGroup)
            {
                if (null == this.pathogenGroupId)
                {
                    result = false;
                }
                else
                {
                    this.currentTherapyGroup = BDTherapyGroup.CreateTherapyGroup(dataContext, this.pathogenGroupId.Value);
                    this.currentTherapyGroup.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                }
            }

            return result;
        }

        #endregion

        private BDTherapyControl addTherapyControl(BDTherapy pTherapy, int pTabIndex)
        {
            BDTherapyControl therapyControl = null;

            if (CreateCurrentObject())
            {
                therapyControl = new BDTherapyControl();

                therapyControl.Dock = DockStyle.Top;
                therapyControl.TabIndex = pTabIndex;
                therapyControl.DisplayOrder = pTabIndex;
                therapyControl.AssignParentId(currentTherapyGroup.uuid);
                therapyControl.AssignDataContext(dataContext);
                therapyControl.AssignScopeId(scopeId);
                therapyControl.CurrentTherapy = pTherapy;
                therapyControl.RequestItemAdd += new System.EventHandler(Therapy_RequestItemAdd);
                therapyControl.RequestItemDelete += new System.EventHandler(Therapy_RequestItemDelete);
                therapyControlList.Add(therapyControl);

                panelTherapies.Controls.Add(therapyControl);
                therapyControl.BringToFront();

                therapyControl.RefreshLayout();
            }

            return therapyControl;
        }

        private void removeTherapyControl(BDTherapyControl pTherapyControl, bool pDeleteRecord)
        {
            if (pDeleteRecord)
            {
                BDTherapy entry = pTherapyControl.CurrentTherapy;
                if (null != entry)
                {
                    // call to BDDeletion
                }
            }

            pTherapyControl.RequestItemAdd -= new System.EventHandler(Therapy_RequestItemAdd);
            pTherapyControl.RequestItemDelete -= new System.EventHandler(Therapy_RequestItemDelete);
            panelTherapies.Controls.Remove(pTherapyControl);

            therapyControlList.Remove(pTherapyControl);
            pTherapyControl.Dispose();
            pTherapyControl = null;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (null != textBox)
            {
                //this.BackColor = (textBox.Text.Trim() != string.Empty) ? SystemColors.Control : SystemColors.ControlDark;
            }
        }

        private void BDTherapyGroupControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void btnTherapyGroupLink_Click(object sender, EventArgs e)
        {
            CreateLink(@"TherapyGroup");
        }

        private void CreateLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();
                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignContextEntityName(BDTherapyGroup.ENTITYNAME_FRIENDLY);
                view.AssignScopeId(scopeId);

                if (null != currentTherapyGroup)
                {
                    view.AssignParentId(currentTherapyGroup.uuid);
                }
                else
                {
                    view.AssignParentId(null);
                }
                view.PopulateControl();
                view.ShowDialog(this);
            }
        }

        private void TherapyGroup_RequestItemAdd(object sender, EventArgs e)
        {
            OnItemAddRequested(new EventArgs());
        }

        private void TherapyGroup_RequestItemDelete(object sender, EventArgs e)
        {
            OnItemDeleteRequested(new EventArgs());
        }

        private void Therapy_RequestItemAdd(object sender, EventArgs e)
        {
            BDTherapyControl control = addTherapyControl(null, therapyControlList.Count);
            if (null != control)
            {
                control.Focus();
            }
        }

        private void Therapy_RequestItemDelete(object sender, EventArgs e)
        {
            BDTherapyControl control = sender as BDTherapyControl;
            if (null != control)
            {
                removeTherapyControl(control, true);
            }
        }

        public override string ToString()
        {
            return (null == this.currentTherapyGroup) ? "No Therapy Group" : this.currentTherapyGroup.name;
        }

        private void BDTherapyGroupControl_Load(object sender, EventArgs e)
        {

        }
    }
}
