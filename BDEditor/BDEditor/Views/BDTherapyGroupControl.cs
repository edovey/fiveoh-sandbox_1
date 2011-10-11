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

        public BDTherapyGroupControl()
        {
            InitializeComponent();
        }

        public BDTherapyGroup CurrentTherapyGroup
        {
            get
            {
                return currentTherapyGroup;
            }
            set
            {
                currentTherapyGroup = value;
                if (null == currentTherapyGroup)
                {
                    tbName.Text = @"";
                    noneRadioButton.Checked = true;

                    bdTherapyControl1.CurrentTherapy = null;
                    bdTherapyControl2.CurrentTherapy = null;
                    bdTherapyControl3.CurrentTherapy = null;

                    bdTherapyControl1.AssignParentId(null);
                    bdTherapyControl2.AssignParentId(null);
                    bdTherapyControl3.AssignParentId(null);
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
                    if (therapyList.Count > 0) bdTherapyControl1.CurrentTherapy = therapyList[0];
                    if (therapyList.Count > 1) bdTherapyControl2.CurrentTherapy = therapyList[1];
                    if (therapyList.Count > 2) bdTherapyControl3.CurrentTherapy = therapyList[2];

                    bdTherapyControl1.AssignParentId(currentTherapyGroup.uuid);
                    bdTherapyControl2.AssignParentId(currentTherapyGroup.uuid);
                    bdTherapyControl3.AssignParentId(currentTherapyGroup.uuid);
                }
            }
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
            bdTherapyControl1.AssignScopeId(scopeId);
            bdTherapyControl2.AssignScopeId(scopeId);
            bdTherapyControl3.AssignScopeId(scopeId);
        }

        #region IBDControl

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdTherapyControl1.AssignDataContext(dataContext);
            bdTherapyControl2.AssignDataContext(dataContext);
            bdTherapyControl3.AssignDataContext(dataContext);
        }

        public bool Save()
        {
            bool result = false;
            if (null != pathogenGroupId)
            {
                result = bdTherapyControl1.Save() || result;
                result = bdTherapyControl2.Save() || result;
                result = bdTherapyControl3.Save() || result;

                if (result && (null == currentTherapyGroup))
                {
                    currentTherapyGroup = BDTherapyGroup.CreateTherapyGroup(dataContext);
                    currentTherapyGroup.pathogenGroupId = pathogenGroupId;
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

        public void AssignParentId(Guid? pParentId)
        {
            pathogenGroupId = pParentId;

            bdTherapyControl1.AssignParentControl(this);
            bdTherapyControl2.AssignParentControl(this);
            bdTherapyControl3.AssignParentControl(this);
        }

        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        #endregion

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            if (null == currentTherapyGroup)
            {
                currentTherapyGroup = BDTherapyGroup.CreateTherapyGroup(dataContext);
                currentTherapyGroup.pathogenGroupId = pathogenGroupId;
                BDTherapyGroup.SaveTherapyGroup(dataContext, currentTherapyGroup);
                pControl.AssignParentId(currentTherapyGroup.uuid);
                pControl.Save();
            }
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
}
