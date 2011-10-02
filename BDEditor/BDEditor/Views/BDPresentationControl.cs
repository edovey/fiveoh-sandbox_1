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
                }
                else
                {
                    tbPresentationName.Text = currentPresentation.name;
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
    }
}
