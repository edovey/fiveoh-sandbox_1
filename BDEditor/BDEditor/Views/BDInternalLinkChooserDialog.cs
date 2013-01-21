using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.Classes;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDInternalLinkChooserDialog : Form
    {
        public Entities DataContext;
        public BDConstants.BDNodeType SelectedNodeType = BDConstants.BDNodeType.None;
        public Guid? SelectedUuid = null;

        public BDInternalLinkChooserDialog(Entities pDataContext)
        {
            InitializeComponent();
            DataContext = pDataContext;
        }

        public void Setup(IBDNode pInitialNode)
        {
            bdInternalLinkChooserControl1.Setup(DataContext, pInitialNode);    
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IBDNode selectedNode = bdInternalLinkChooserControl1.SelectedIBDNode;
            if (null != selectedNode)
            {
                SelectedNodeType = selectedNode.NodeType;
                SelectedUuid = selectedNode.Uuid;
            }
            else
            {
                SelectedNodeType = BDConstants.BDNodeType.None;
                SelectedUuid = null;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BDInternalLinkChooserDialog_Load(object sender, EventArgs e)
        {
            bdInternalLinkChooserControl1.Setup(DataContext, null);
        }
    }
}
