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

        private List<BDTherapyControl> therapyControlList = new List<BDTherapyControl>();

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
                BDTherapyControl therapyControl = therapyControlList[idx];
                removeTherapyControl(therapyControl, false);
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

                List<BDTherapy> therapyList = BDTherapy.GetTherapiesForTherapyGroupId(dataContext, currentTherapyGroup.uuid);
                for (int idx = 0; idx < therapyList.Count; idx++)
                {
                    BDTherapy therapy = therapyList[idx];
                    addTherapyControl(therapy, idx);
                }
            }

            resizeTherapyControlPanelHeight();
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

                if (result && (null == currentTherapyGroup))
                {
                    currentTherapyGroup = BDTherapyGroup.CreateTherapyGroup(dataContext, pathogenGroupId.Value);
                    currentTherapyGroup.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
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

        /*
        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            if (null == currentTherapyGroup)
            {
                currentTherapyGroup = BDTherapyGroup.CreateTherapyGroup(dataContext);
                currentTherapyGroup.pathogenGroupId = pathogenGroupId;
                currentTherapyGroup.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                BDTherapyGroup.SaveTherapyGroup(dataContext, currentTherapyGroup);
                pControl.AssignParentId(currentTherapyGroup.uuid);
                pControl.Save();
            }
            else
            {
                pControl.AssignParentId(currentTherapyGroup.uuid);
                pControl.Save();
            }
        }
        */
        #endregion

        private BDTherapyControl addTherapyControl(BDTherapy pTherapy, int pTabIndex)
        {
            BDTherapyControl therapyControl = null;

            if (CreateCurrentObject())
            {
                therapyControl = new BDTherapyControl();

                //TODO: if currentTherapyGroup is null, create one
                int bottom = 0;
                foreach (Control control in panelTherapies.Controls)
                {
                    bottom += control.Height;
                }
                therapyControl.TabIndex = pTabIndex;
                therapyControl.Top = bottom;
                therapyControl.Left = 0;
                therapyControl.AssignParentId(currentTherapyGroup.uuid);
                therapyControl.AssignParentControl(this);
                therapyControl.AssignDataContext(dataContext);
                therapyControl.AssignScopeId(scopeId);
                therapyControl.CurrentTherapy = pTherapy;
                therapyControl.RequestItemAdd += new System.EventHandler(RequestItemAdd);
                therapyControl.RequestItemDelete += new System.EventHandler(RequestItemDelete);
                therapyControlList.Add(therapyControl);

                panelTherapies.Controls.Add(therapyControl);
                panelTherapies.Controls.SetChildIndex(therapyControl, pTabIndex);

                therapyControl.RefreshLayout();
            }

            return therapyControl;
        }

        private void removeTherapyControl(BDTherapyControl pTherapyControl, bool pDeleteRecord)
        {
            if (pDeleteRecord)
            {
                BDTherapy therapy = pTherapyControl.CurrentTherapy;
                if (null != therapy)
                {
                    // call to BDDeleteRecord
                }
            }

            pTherapyControl.RequestItemAdd -= new System.EventHandler(RequestItemAdd);
            pTherapyControl.RequestItemDelete -= new System.EventHandler(RequestItemDelete);
            panelTherapies.Controls.Remove(pTherapyControl);

            therapyControlList.Remove(pTherapyControl);
            pTherapyControl.Dispose();
            pTherapyControl = null;
            int top = 0;
            for (int idx = 0; idx < panelTherapies.Controls.Count; idx++)
            {
                Control control = panelTherapies.Controls[idx];
                control.Top = top;
                top += control.Height;
            }
            //
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
                view.AssignParentControl(this);
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

        private void resizeTherapyControlPanelHeight()
        {
            this.Height = panelTherapies.Top + panelTherapies.Height;
        }

        private void RequestItemAdd(object sender, EventArgs e)
        {
            BDTherapyControl control = addTherapyControl(null, therapyControlList.Count);
            if (null != control)
            {
                resizeTherapyControlPanelHeight();
                control.Focus();
            }
        }

        private void RequestItemDelete(object sender, EventArgs e)
        {
            BDTherapyControl therapyControl = sender as BDTherapyControl;
            if (null != therapyControl)
            {
                removeTherapyControl(therapyControl, true);
            }
        }

        public override string ToString()
        {
            return (null == this.currentTherapyGroup) ? "No Therapy Group" : this.currentTherapyGroup.name;
        }
    }
}
