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
    public partial class BDPresentationControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? diseaseId;
        private BDPresentation currentPresentation;

        public BDPresentation CurrentPresentation
        {
            get
            {
                return currentPresentation;
            }
            set
            {
                currentPresentation = value;
                if (currentPresentation == null)
                {
                    tbPresentationName.Text = @"";

                    bdPathogenGroupControl1.CurrentPathogenGroup = null;
                    bdPathogenGroupControl1.AssignParentId(null);
                }
                else
                {
                    tbPresentationName.Text = currentPresentation.name;

                    List<BDPathogenGroup> pathogenGroupList = BDPathogenGroup.GetPathogenGroupsForPresentationId(dataContext, currentPresentation.uuid);
                    if (pathogenGroupList.Count <= 0)
                    {
                        bdPathogenGroupControl1.CurrentPathogenGroup = null;
                        bdPathogenGroupControl1.AssignParentId(currentPresentation.uuid);
                    }
                    else
                    {
                        bdPathogenGroupControl1.CurrentPathogenGroup = pathogenGroupList[0];
                        bdPathogenGroupControl1.AssignParentId(currentPresentation.uuid);
                    }
                }
            }
        }

        public BDPresentationControl()
        {
            InitializeComponent();
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdPathogenGroupControl1.AssignDataContext(dataContext);
        }

        public void AssignParentId(Guid? pParentId)
        {
            diseaseId = pParentId;

            this.Enabled = (null != diseaseId); 
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }


        public void AssignParentControl(IBDControl pControl)
        {
            throw new NotImplementedException();
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            throw new NotImplementedException();
        }
    }
}
