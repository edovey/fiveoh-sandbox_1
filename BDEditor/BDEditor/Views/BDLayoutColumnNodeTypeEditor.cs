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
    public partial class BDLayoutColumnNodeTypeEditor : Form
    {
        private Entities dataContext;
        private BDLayoutMetadataColumn layoutColumn;

        public BDLayoutColumnNodeTypeEditor()
        {
            InitializeComponent();
        }

        public BDLayoutColumnNodeTypeEditor(Entities pDataContext, BDLayoutMetadataColumn pLayoutColumn)
        {
            InitializeComponent();

            this.dataContext = pDataContext;
            this.layoutColumn = pLayoutColumn;

        }

        private void BDLayoutColumnNodeTypeEditor_Load(object sender, EventArgs e)
        {
            lblColumnLabel.Text = "";
            if (null != this.layoutColumn)
            {
                lblColumnLabel.Text = this.layoutColumn.label;
            }

            listBoxNodetypes.Items.Clear();
            List<BDNodeTypeWrapper> list = new List<BDNodeTypeWrapper>();

            foreach (BDConstants.BDNodeType nodeType in Enum.GetValues(typeof(BDConstants.BDNodeType)))
            {
                BDNodeTypeWrapper entry = new BDNodeTypeWrapper(nodeType);
                list.Add(entry);
            }

            list.Sort();
            listBoxNodetypes.Items.AddRange(list.ToArray());
        }

        private void DisplayColumn(BDLayoutMetadataColumn pLayoutColumn)
        {
            listBoxColumnNodeTypes.Items.Clear();

            if (null != pLayoutColumn)
            {
                List<BDLayoutMetadataColumnNodeType> list = BDLayoutMetadataColumnNodeType.RetrieveForLayoutColumn(dataContext, pLayoutColumn.Uuid);    
                listBoxColumnNodeTypes.Items.AddRange(list.ToArray());
            }
        }

        private void DisplayPropertiesForNodeType(BDConstants.BDNodeType pNodeType)
        {

        }
    }

    public class BDNodeTypeWrapper: IComparable
    {
        public BDConstants.BDNodeType NodeType { get; set; }
        public string Description { get; set; }

        public BDNodeTypeWrapper(BDConstants.BDNodeType pNodeType)
        {
            NodeType = pNodeType;
            Description = BDUtilities.GetEnumDescription(pNodeType);
        }

        public override string ToString()
        {
            return Description;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            BDNodeTypeWrapper other = obj as BDNodeTypeWrapper;
            if (other != null)
                return this.Description.CompareTo(other.Description);
            else
                throw new ArgumentException("Object is not a BDNodeTypeWrapper");

        }
    }
}
