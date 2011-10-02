using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDTherapyGroupControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? pathogenGroupId;
        private BDTherapyGroup currentTherapyGroup;

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
                    this.BackColor = SystemColors.ControlDark;

                    tbName.Text = @"";
                    noneRadioButton.Checked = true;

                    bdTherapyControl1.CurrentTherapy = null;
                    bdTherapyControl2.CurrentTherapy = null;
                    bdTherapyControl3.CurrentTherapy = null;
                    bdTherapyControl4.CurrentTherapy = null;
                    bdTherapyControl5.CurrentTherapy = null;
                    bdTherapyControl6.CurrentTherapy = null;
                }
                else
                {
                    this.BackColor = SystemColors.Control;

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
                    if (therapyList.Count > 1) bdTherapyControl1.CurrentTherapy = therapyList[1];
                    if (therapyList.Count > 2) bdTherapyControl1.CurrentTherapy = therapyList[2];
                    if (therapyList.Count > 3) bdTherapyControl1.CurrentTherapy = therapyList[3];
                    if (therapyList.Count > 4) bdTherapyControl1.CurrentTherapy = therapyList[4];
                    if (therapyList.Count > 5) bdTherapyControl1.CurrentTherapy = therapyList[5];
                   
                }
            }
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool Save()
        {
            bool result = false;
            if (null != pathogenGroupId)
            {
                result = bdTherapyControl1.Save() || result;
                result = bdTherapyControl2.Save() || result;
                result = bdTherapyControl3.Save() || result;
                result = bdTherapyControl4.Save() || result;
                result = bdTherapyControl5.Save() || result;
                result = bdTherapyControl6.Save() || result;

                if (result && (null == currentTherapyGroup))
                {
                    currentTherapyGroup = BDTherapyGroup.CreateTherapyGroup(dataContext);
                    currentTherapyGroup.pathogenGroupId = pathogenGroupId;
                }

                if (null != currentTherapyGroup)
                {
                    currentTherapyGroup.name = tbName.Text;

                    if (andRadioButton.Checked)
                    {
                        currentTherapyGroup.therapyGroupJoinType = (int)BDTherapyGroup.TherapyGroupJoinType.AndWithNext;
                    }
                    else if (orRadioButton.Checked)
                    {
                        currentTherapyGroup.therapyGroupJoinType = (int)BDTherapyGroup.TherapyGroupJoinType.OrWithNext;
                    }
                    else
                    {
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
            this.Enabled = (null != pathogenGroupId);

            bdTherapyControl1.AssignParentId(pathogenGroupId);
            bdTherapyControl2.AssignParentId(pathogenGroupId);
            bdTherapyControl3.AssignParentId(pathogenGroupId);
            bdTherapyControl4.AssignParentId(pathogenGroupId);
            bdTherapyControl5.AssignParentId(pathogenGroupId);
            bdTherapyControl6.AssignParentId(pathogenGroupId);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (null != textBox)
            {
                this.BackColor = (textBox.Text.Trim() != string.Empty) ? SystemColors.Control : SystemColors.ControlDark;
            }
        }
    }
}
