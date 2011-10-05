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
    public partial class BDPathogenGroupControl : UserControl, IBDControl
    {
        #region Class properties
        private Entities dataContext;
        private Guid? presentationId;
        private BDPathogenGroup currentPathogenGroup;
        private IBDControl parentControl;

        public BDPathogenGroup CurrentPathogenGroup
        {
            get
            {
                return currentPathogenGroup;
            }
            set
            {
                currentPathogenGroup = value;
                if (null == currentPathogenGroup)
                {
                    //this.BackColor = SystemColors.ControlDark;
                    bdTherapyGroupControl1.AssignParentId(null);
                    bdTherapyGroupControl1.CurrentTherapyGroup = null;
                    pathogenSet1.CurrentPathogenGroup = null;
                    pathogenSet1.AssignParentId(null);
                }
                else
                {
                    //this.BackColor = SystemColors.Control;
                    bdTherapyGroupControl1.AssignParentId(currentPathogenGroup.uuid);
                    List<BDTherapyGroup> therapyGroupList = BDTherapyGroup.getTherapyGroupsForPathogenGroupId(dataContext, currentPathogenGroup.uuid);
                    if (therapyGroupList.Count <= 0)
                    {
                        bdTherapyGroupControl1.CurrentTherapyGroup = null;
                    }
                    else
                    {
                        bdTherapyGroupControl1.CurrentTherapyGroup = therapyGroupList[0];
                    }

                    pathogenSet1.CurrentPathogenGroup = currentPathogenGroup;
                }
            }
        }

        #endregion

        public BDPathogenGroupControl()
        {
            InitializeComponent();
        }

        private void BDPathogenGroupControl_Load(object sender, EventArgs e)
        {

        }

        private List<BDLinkedNote> GetLinkedNotesForPathogen(BDPathogen pPathogen)
        {
            List<BDLinkedNote> linkedNoteList = BDLinkedNote.GetLinkedNotesForParentId(dataContext, pPathogen.uuid);
            if (linkedNoteList.Count == 0)
                return null;
            else
                return linkedNoteList;
        }

        #region IBDControl
        
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdTherapyGroupControl1.AssignDataContext(dataContext);
            pathogenSet1.AssignDataContext(dataContext);
        }

        public void AssignParentId(Guid? pParentId)
        {
            presentationId = pParentId;
            this.Enabled = (null != presentationId);
        }

        public bool Save()
        {
            bool result = false;

            if (null != presentationId)
            {
                result = pathogenSet1.Save() || result;

                if (result && (null == currentPathogenGroup)) // only create a group if any of the children exist
                {
                    currentPathogenGroup = BDPathogenGroup.CreatePathogenGroup(dataContext);
                    currentPathogenGroup.presentationId = presentationId;
                }

                if (null != currentPathogenGroup)
                {
                    System.Diagnostics.Debug.WriteLine(@"PathogenGroup Control Save");
                    bdTherapyGroupControl1.Save();
                    BDPathogenGroup.SavePathogenGroup(dataContext, currentPathogenGroup);
                    result = true;
                }
            }
            return result;
        }

        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            if (null == currentPathogenGroup)
            {
                currentPathogenGroup = BDPathogenGroup.CreatePathogenGroup(dataContext);
                currentPathogenGroup.presentationId = presentationId;
                BDPathogenGroup.SavePathogenGroup(dataContext, currentPathogenGroup);
                pControl.AssignParentId(currentPathogenGroup.uuid);
                pControl.Save();
            }
            else
            {
                pControl.AssignParentId(currentPathogenGroup.uuid);
                pControl.Save();
            }
            //this.BackColor = SystemColors.Control;
        }
        #endregion    
    }
}
