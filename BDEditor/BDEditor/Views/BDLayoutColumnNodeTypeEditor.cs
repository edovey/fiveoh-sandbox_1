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
        private bool isDisplaying = false;

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
            listBoxColumnNodeTypes.Items.Clear();

            if (null != this.layoutColumn)
            {
                lblColumnLabel.Text = this.layoutColumn.label;
                List<BDLayoutMetadataColumnNodeType> columnNodeList = BDLayoutMetadataColumnNodeType.RetrieveListForLayoutColumn(dataContext, layoutColumn.Uuid);
                listBoxColumnNodeTypes.Items.AddRange(columnNodeList.ToArray());
            }

            listBoxNodetypes.Items.Clear();
            List<BDNodeTypeWrapper> nodeTypelist = new List<BDNodeTypeWrapper>();

            foreach (BDConstants.BDNodeType nodeType in Enum.GetValues(typeof(BDConstants.BDNodeType)))
            {
                BDNodeTypeWrapper entry = new BDNodeTypeWrapper(nodeType);
                nodeTypelist.Add(entry);
            }

            nodeTypelist.Sort();
            listBoxNodetypes.Items.AddRange(nodeTypelist.ToArray());

        }

        private void DisplayColumn(BDLayoutMetadataColumn pLayoutColumn)
        {
            listBoxColumnNodeTypes.Items.Clear();

            if (null != pLayoutColumn)
            {
                List<BDLayoutMetadataColumnNodeType> list = BDLayoutMetadataColumnNodeType.RetrieveListForLayoutColumn(dataContext, pLayoutColumn.Uuid);    
                listBoxColumnNodeTypes.Items.AddRange(list.ToArray());
            }
        }

        private void DisplayPropertiesForNodeType(BDConstants.BDNodeType pNodeType)
        {
            listBoxNodetypeProperties.Items.Clear();
            List<String> propertyList = BDFabrik.GetPropertyNamesForNodeType(pNodeType);
            listBoxNodetypeProperties.Items.AddRange(propertyList.ToArray());
        }

        private void listBoxNodetypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.isDisplaying)
            {
                listBoxNodetypeProperties.Items.Clear();
                if (listBoxNodetypes.SelectedIndex >= 0)
                {
                    BDNodeTypeWrapper entry = listBoxNodetypes.SelectedItem as BDNodeTypeWrapper;
                    if (null != entry)
                    {
                        DisplayPropertiesForNodeType(entry.NodeType);
                    }
                }
            }
        }

        private void btnAssignNodeType_Click(object sender, EventArgs e)
        {
            if (null == this.layoutColumn) return;

            if ((listBoxNodetypes.SelectedIndex >= 0) && (listBoxNodetypeProperties.SelectedIndices.Count > 0))
            {
                BDNodeTypeWrapper nodeWrapper = listBoxNodetypes.SelectedItem as BDNodeTypeWrapper;

                bool hasChanged = false;

                foreach (Object obj in listBoxNodetypeProperties.SelectedItems)
                {
                    string propertyName = obj as string;
                    if (!BDLayoutMetadataColumnNodeType.ExistsForLayoutColumn(dataContext, this.layoutColumn.Uuid, nodeWrapper.NodeType, propertyName))
                    {
                        int orderOfPrecedence = listBoxColumnNodeTypes.Items.Count;
                        BDLayoutMetadataColumnNodeType entry = BDLayoutMetadataColumnNodeType.Create(this.dataContext, 
                                                                                                        this.layoutColumn.layoutVariant, 
                                                                                                        this.layoutColumn.Uuid, 
                                                                                                        nodeWrapper.NodeType, 
                                                                                                        propertyName, 
                                                                                                        orderOfPrecedence);
                        listBoxColumnNodeTypes.Items.Add(entry);
                        hasChanged = true;
                    }
                }

                if (hasChanged)
                {
                    for (int idx = 0; idx < listBoxColumnNodeTypes.Items.Count; idx++)
                    {
                        BDLayoutMetadataColumnNodeType entry = listBoxColumnNodeTypes.Items[idx] as BDLayoutMetadataColumnNodeType;
                        entry.orderOfPrecedence = idx + 1;
                    }
                    dataContext.SaveChanges();
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listBoxColumnNodeTypes.SelectedItems.Count > 0)
            {
                //for (int idx = 0; idx < listBoxColumnNodeTypes.SelectedItems.Count; idx++)
                Object[] list = new Object[listBoxColumnNodeTypes.SelectedItems.Count];
                listBoxColumnNodeTypes.SelectedItems.CopyTo(list, 0);
                foreach (Object obj in list)
                {
                    BDLayoutMetadataColumnNodeType entry = /*listBoxColumnNodeTypes.SelectedItems[idx] */ obj as BDLayoutMetadataColumnNodeType;
                    if (null != entry)
                    {
                        listBoxColumnNodeTypes.Items.Remove(entry);
                        dataContext.DeleteObject(entry);
                    }
                }
                dataContext.SaveChanges();
            }
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
