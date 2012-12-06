using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;
using BDEditor.Classes.Navigation;

using System.Diagnostics;

namespace BDEditor.Views
{
    public partial class BDEditView : Form
    {
        BDEditor.DataModel.Entities dataContext;

        List<ToolStripMenuItem> addNodeToolStripMenuItemList = new List<ToolStripMenuItem>();

        // reset the following on BDEditView_Load when adding seed data
        private bool isSeedDataLoadAvailable = false;
        private string seedDataFileName = string.Empty;
        // enable & show move button when data move is required
        private bool moveButtonVisible = false;

        public BDEditView()
        {
            InitializeComponent();

            dataContext = new DataModel.Entities();

            LoadChapterDropDown();

            chapterDropDown.DisplayMember = "Name";

            BDNotification.Notify += new EventHandler<BDNotificationEventArgs>(BDNotification_Notify);

        }

        void BDNotification_Notify(object sender, BDNotificationEventArgs e)
        {
            switch (e.NotificationType)
            {
                case BDNotificationEventArgs.BDNotificationType.Refresh:
                case BDNotificationEventArgs.BDNotificationType.Addition:
                case BDNotificationEventArgs.BDNotificationType.Deletion:
                    TreeNode childTreeNode = showNavSelection(chapterTree.SelectedNode, true);
                    if( (null != childTreeNode) && (chapterTree.SelectedNode.Nodes.Count != childTreeNode.Nodes.Count))
                    {
                        Debug.WriteLine("Refresh child tree nodes");
                        showNavSelection(chapterTree.SelectedNode, false);
                    }
                    break;
            }
        }
        
        public DataModel.Entities DataContext
        {
            get
            {
                if (null == dataContext)
                {
                    dataContext = new Entities();
                }
                return dataContext;
            }

            set
            {
                dataContext = value;
            }
        }

        private void listDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            IBDNode listEntry = chapterDropDown.SelectedItem as IBDNode;

            displayChapter(listEntry);
        }

        private void displayChapter(IBDNode pNode)
        {
            Cursor _cursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            
            splitContainer1.Panel2.Controls.Clear();

            chapterTree.Nodes.Clear();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDChapter))
            {
                TreeNode node = null;
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.FrontMatter:
                        node = BDFrontMatterTree.BuildBranch(dataContext, pNode);
                        break;
                    case BDConstants.LayoutVariantType.BackMatter:
                        node = BDBackMatterTree.BuildBranch(dataContext, pNode);
                        break;
                    case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        node = BDTreatmentRecommendationTree.BuildBranch(dataContext, pNode);
                        break;
                    case BDConstants.LayoutVariantType.Antibiotics:
                        node = BDAntibioticsTree.BuildBranch(dataContext, pNode);
                        break;
                    case BDConstants.LayoutVariantType.Prophylaxis:
                        node = BDProphylaxisTree.BuildBranch(dataContext, pNode);
                        break;
                    case BDConstants.LayoutVariantType.Dental:
                        node = BDDentalTree.BuildBranch(dataContext, pNode);
                        break;
                    case BDConstants.LayoutVariantType.PregancyLactation:
                        node = BDPregnancyLactationTree.BuildBranch(dataContext, pNode);
                        break;
                    case BDConstants.LayoutVariantType.Microbiology:
                        node = BDMicrobiologyTree.BuildBranch(dataContext, pNode);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (null != node)
                {
                    TreeNode[] nodeList = new TreeNode[node.Nodes.Count];
                    node.Nodes.CopyTo(nodeList, 0);
                    chapterTree.Nodes.AddRange(nodeList);
                }
            }

            this.Cursor = _cursor;
        }

        private void graftTreeNode(TreeNode pTree, TreeNode pBranch)
        {
            TreeNode[] nodeList = new TreeNode[pBranch.Nodes.Count];
            pBranch.Nodes.CopyTo(nodeList, 0);
            pTree.Nodes.Clear();
            pTree.Nodes.AddRange(nodeList);
            pTree.Expand();
        }

        private void buildNavContextMenuStrip(TreeNode pTreeNode, IBDNode pBDNode)
        {
            foreach (ToolStripMenuItem entry in addNodeToolStripMenuItemList)
            {
                entry.Click -= new System.EventHandler(this.addChildNode_Click);
            }
            addNodeToolStripMenuItemList.Clear();
            addChildNodeToolStripMenuItem.Visible = false;
            addChildNodeToolStripMenuItem.DropDownItems.Clear();
            toolStripMenuItem1.Visible = false;

            reorderNextToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);
            reorderPreviousToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);
            deleteToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);

            string nodeTypeName = BDUtilities.GetEnumDescription(pBDNode.NodeType);

            deleteToolStripMenuItem.Text = string.Format("Delete {0}: {1}", nodeTypeName, pBDNode.Name);

            List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> childTypeInfoList = BDFabrik.ChildTypeDefinitionListForNode(pBDNode);
            if (null != childTypeInfoList)
            {
                if (childTypeInfoList.Count == 1)
                {
                    addChildNodeToolStripMenuItem.Visible = true;
                    toolStripMenuItem1.Visible = true;

                    string childNodeTypeName = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item1);
                    

                    if (childTypeInfoList[0].Item2.Length == 1)
                    {
                        string subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[0]);
                        addChildNodeToolStripMenuItem.Text = string.Format("Add {0} [{1}]", childNodeTypeName, subDescription);
                        addChildNodeToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[0], pTreeNode);
                    }
                    else
                    {
                        
                        addChildNodeToolStripMenuItem.Text = string.Format("Add {0}", childNodeTypeName);

                        for (int idx = 0; idx < childTypeInfoList[0].Item2.Length; idx++)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem();

                            item.Image = global::BDEditor.Properties.Resources.add_16x16;
                            item.Name = string.Format("dynamicAddChildLayoutVariant{0}", idx);
                            item.Size = new System.Drawing.Size(179, 22);
                            item.Text = string.Format("&Add {0} [{1}]", childNodeTypeName, BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[idx]));
                            item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[idx], pTreeNode);
                            item.Click += new System.EventHandler(this.addChildNode_Click);
                            addChildNodeToolStripMenuItem.DropDownItems.Add(item);
                        }
                    }
                }
                else if (childTypeInfoList.Count > 1)
                {
                    toolStripMenuItem1.Visible = true;
                    addChildNodeToolStripMenuItem.Visible = true;
                    addChildNodeToolStripMenuItem.Text = string.Format("Add");

                    for (int idx = 0; idx < childTypeInfoList.Count; idx++)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem();

                        item.Image = global::BDEditor.Properties.Resources.add_16x16;
                        item.Name = string.Format("dynamicAddChild{0}", idx);
                        item.Size = new System.Drawing.Size(179, 22);
                        item.Text = string.Format("&Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1));
                        item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[0], pTreeNode);
                        item.Click += new System.EventHandler(this.addChildNode_Click);

                        if (childTypeInfoList[idx].Item2.Length == 1)
                        {
                            //include the layoutvariant info to the "Add" text if there is only one
                            string subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[0]);
                            item.Text = string.Format("&Add {0} [{1}]", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1), subDescription);
                        }
                        else
                        {
                            for (int idy = 0; idy < childTypeInfoList[idx].Item2.Length; idy++)
                            {
                                ToolStripMenuItem layoutItem = new ToolStripMenuItem();

                                layoutItem.Image = global::BDEditor.Properties.Resources.add_16x16;
                                layoutItem.Name = string.Format("dynamicAddChildLayoutVariant{0}", idy);
                                layoutItem.Size = new System.Drawing.Size(179, 22);
                                layoutItem.Text = string.Format("Add {0} [{1}]",BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1), BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[idy]));
                                layoutItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[idy], pTreeNode);
                                layoutItem.Click += new System.EventHandler(this.addChildNode_Click);
                                item.DropDownItems.Add(layoutItem);
                            }
                        }

                        addChildNodeToolStripMenuItem.DropDownItems.Add(item);
                    }
                }
            }
        }

        void reorderNodeToPrevious_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;

                BDFabrik.ReorderNode(DataContext, nodeWrapper.Node, -1);

                TreeNode parentNode = nodeWrapper.NodeTreeNode.Parent;
                if (null == parentNode)
                {
                    IBDNode listEntry = chapterDropDown.SelectedItem as IBDNode;

                    displayChapter(listEntry);
                }
                else
                {
                    chapterTree.SelectedNode = parentNode;
                    showNavSelection(parentNode, false);
                }
            }
        }

        void reorderNodeToNext_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;

                BDFabrik.ReorderNode(DataContext, nodeWrapper.Node, 1);

                TreeNode parentNode = nodeWrapper.NodeTreeNode.Parent;
                if (null == parentNode)
                {
                    IBDNode listEntry = chapterDropDown.SelectedItem as IBDNode;

                    displayChapter(listEntry);
                }
                else
                {
                    chapterTree.SelectedNode = parentNode;
                    showNavSelection(parentNode, false);
                }
            }
        }

        void addChildNode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    BDFabrik.CreateChildNode(DataContext, nodeWrapper.Node, nodeWrapper.TargetNodeType, nodeWrapper.TargetLayoutVariant);
                    showNavSelection(nodeWrapper.NodeTreeNode, false);
                }
            }
        }

        void deleteNode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    string nodeTypeName = BDUtilities.GetEnumDescription(nodeWrapper.Node.NodeType);
                    string message = string.Format("Are you sure you want to delete the {0} named [{1}]?\nThis will also delete all associated child information and cannot be undone", nodeTypeName, nodeWrapper.Node.Name);

                    if (MessageBox.Show(this, message, "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                    {
                        BDFabrik.DeleteNode(DataContext, nodeWrapper.Node);
                        showNavSelection(nodeWrapper.NodeTreeNode.Parent, false);
                    }
                }
            }
        }

        private void chapterTree_MouseUp(object sender, MouseEventArgs e)
        {
            TreeView tree = sender as TreeView;
            if (null == tree)
                return;

            // only need to change selected note during right-click - otherwise tree does fine by itself      
            if (e.Button == MouseButtons.Right)
            {
                Point pt = new Point(e.X, e.Y);
                tree.PointToClient(pt);
                TreeNode treeNode = tree.GetNodeAt(pt);
                if (null != treeNode)
                {
                    if (treeNode.Bounds.Contains(pt))
                    {
                        if (!treeNode.IsSelected)
                        {
                            chapterTree.SelectedNode = treeNode;
                            showNavSelection(treeNode, false);
                        }
                        IBDNode bdNode = treeNode.Tag as IBDNode;
                        buildNavContextMenuStrip(treeNode, bdNode);
                            navTreeContextMenuStrip.Show(tree, pt);
                    }
                }
            } 
        }
        
        private void sectionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Action)
            {
                case TreeViewAction.ByKeyboard:
                case TreeViewAction.ByMouse:
                    showNavSelection(e.Node, false);
                    break;
                case TreeViewAction.Collapse:
                case TreeViewAction.Expand:
                case TreeViewAction.Unknown:
                default:
                    break;
            }
        }

        private TreeNode showNavSelection(TreeNode pSelectedNode, Boolean pInterrogateOnly)
        {
            Debug.WriteLine("InterrogateOnly = {0}", pInterrogateOnly);
            TreeNode childTreeNode = null;

            IBDControl control_tr01 = null;
            TreeNode selectedNode = pSelectedNode;
            if (null == selectedNode)
            {
                this.Cursor = Cursors.WaitCursor;
                splitContainer1.Panel2.SuspendLayout();
                ControlHelper.SuspendDrawing(splitContainer1.Panel2);

                splitContainer1.Panel2.Controls.Clear();
                IBDNode listEntry = chapterDropDown.SelectedItem as IBDNode;
                displayChapter(listEntry);

                ControlHelper.ResumeDrawing(splitContainer1.Panel2);
                splitContainer1.Panel2.ResumeLayout();
                this.Cursor = Cursors.Default;
            }
            else
            {
                IBDNode node = selectedNode.Tag as IBDNode;
                bool showChildControls = true;

                this.Cursor = Cursors.WaitCursor;
                if (!pInterrogateOnly)
                {
                    splitContainer1.Panel2.SuspendLayout();
                    ControlHelper.SuspendDrawing(splitContainer1.Panel2);

                    for (int idx = 0; idx < splitContainer1.Panel2.Controls.Count; idx++)
                    {
                        {
                            Control ctrl = splitContainer1.Panel2.Controls[idx];
                            IBDControl nodeCtrl = ctrl as IBDControl;
                            if (null != nodeCtrl)
                            {
                                nodeCtrl.NameChanged -= new EventHandler<NodeEventArgs>(nodeControl_NameChanged);
                                nodeCtrl.RequestItemAdd -= new EventHandler<NodeEventArgs>(siblingNodeCreateRequest);
                                ((System.Windows.Forms.UserControl)nodeCtrl).Validated -= new EventHandler(BDEditView_Validated);
                                ctrl.Dispose();
                            }
                        }
                    }

                    splitContainer1.Panel2.Controls.Clear();
                }

                switch (node.NodeType)
                {
                    case BDConstants.BDNodeType.BDSection:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.FrontMatter:
                            case BDConstants.LayoutVariantType.BackMatter:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation16_CultureDirected:
                                childTreeNode = BDTreatmentRecommendationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;

                            case BDConstants.LayoutVariantType.Antibiotics:
                            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                            case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                            case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:

                                childTreeNode = BDAntibioticsTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis:
                            case BDConstants.LayoutVariantType.Prophylaxis_PreOp:
                            case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            case BDConstants.LayoutVariantType.Prophylaxis_IERecommendation:
                            case BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage:
                            case BDConstants.LayoutVariantType.Prophylaxis_FluidExposure:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Dental:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                                childTreeNode = BDDentalTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;

                            case BDConstants.LayoutVariantType.PregancyLactation:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                                childTreeNode = BDPregnancyLactationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol:
                                if (!pInterrogateOnly)
                                    showChildControls = true;
                                break;

                            case BDConstants.LayoutVariantType.Microbiology:
                            case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                            case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                            case BDConstants.LayoutVariantType.Microbiology_EmpiricTherapy:
                            case BDConstants.LayoutVariantType.Microbiology_Antibiogram:
                                childTreeNode = BDMicrobiologyTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDAntimicrobial:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                                childTreeNode = BDAntibioticsTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDAntimicrobialGroup:
                        switch (node.LayoutVariant)
                        {
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDCategory:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                            case BDConstants.LayoutVariantType.Microbiology_EmpiricTherapy:
                            case BDConstants.LayoutVariantType.Prophylaxis_IERecommendation:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                                childTreeNode = BDTreatmentRecommendationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                                childTreeNode = BDAntibioticsTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Dental:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                                childTreeNode = BDDentalTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                                childTreeNode = BDPregnancyLactationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
                            case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                                childTreeNode = BDMicrobiologyTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDDisease:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis:
                                childTreeNode = BDTreatmentRecommendationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Dental:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                                childTreeNode = BDDentalTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Invasive:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_HaemophiliusInfluenzae:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Influenza:
                            case BDConstants.LayoutVariantType.Prophylaxis_Communicable_Pertussis:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDMicroorganism:
                        switch(node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                                if (!pInterrogateOnly)
                                    showChildControls = true;
                                break;
                        }           
                        break;
                    case BDConstants.BDNodeType.BDMicroorganismGroup:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDPathogen:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Exposure_CommunicableDiseases:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDPathogenGroup:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                                childTreeNode = BDTreatmentRecommendationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDPresentation:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                                childTreeNode = BDTreatmentRecommendationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDResponse:
                        switch (node.LayoutVariant)
                        {
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSubcategory:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia:
                                childTreeNode = BDTreatmentRecommendationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                                childTreeNode = BDDentalTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;

                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSubsection:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                            case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                                childTreeNode = BDAntibioticsTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSubtopic:
                        switch (node.LayoutVariant)
                        {
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSurgery:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSurgeryClassification:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSurgeryGroup:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDTable:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation06_CultureProvenMeningitis:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia:
                                childTreeNode = BDTreatmentRecommendationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;

                        }
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        switch (node.LayoutVariant)
                        {
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDTopic:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                            case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                                childTreeNode = BDAntibioticsTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                }
                if (!pInterrogateOnly)
                {
                    BDCommon.Settings.WaitingForEvent = true;
                    BDCommon.Settings.IsUpdating = true;
                    control_tr01 = BDFabrik.CreateControlForNode(dataContext, node);
                    if (null != control_tr01)
                    {
                        ((System.Windows.Forms.UserControl)control_tr01).Validated += new EventHandler(BDEditView_Validated);
                        control_tr01.ShowChildren = showChildControls;
                        control_tr01.AssignScopeId((null != node) ? node.Uuid : Guid.Empty);
                        control_tr01.AssignParentInfo(node.ParentId, node.ParentType);
                        ((System.Windows.Forms.UserControl)control_tr01).Dock = DockStyle.Fill;
                        control_tr01.NameChanged += new EventHandler<NodeEventArgs>(nodeControl_NameChanged);
                        control_tr01.RequestItemAdd += new EventHandler<NodeEventArgs>(siblingNodeCreateRequest);
                        splitContainer1.Panel2.Controls.Add((System.Windows.Forms.UserControl)control_tr01);
                        control_tr01.RefreshLayout(showChildControls);

                    }
                    BDCommon.Settings.IsUpdating = false;
                    ControlHelper.ResumeDrawing(splitContainer1.Panel2);
                    splitContainer1.Panel2.ResumeLayout();
                }
                this.Cursor = Cursors.Default;
            }
            return childTreeNode;
        }

        void BDEditView_Validated(object sender, EventArgs e)
        {
            if (BDCommon.Settings.WaitingForEvent)
                BDCommon.Settings.IsUpdating = false;
            Debug.WriteLine("Validated");
        }

        void nodeControl_NameChanged(object sender, NodeEventArgs e)
        {
            IBDNode node = chapterTree.SelectedNode.Tag as IBDNode;
            if (node.Uuid == e.Uuid)
            {
                chapterTree.SelectedNode.Text = e.Text;
            }
        }

        void siblingNodeCreateRequest(object sender, NodeEventArgs e)
        {
            IBDNode siblingNode = BDFabrik.RetrieveNode(e.DataContext, e.NodeType, e.Uuid);
            if(null != siblingNode)
            {
                IBDNode parentNode = BDFabrik.RetrieveNode(e.DataContext, siblingNode.ParentType, siblingNode.ParentId);
                BDFabrik.CreateChildNode(DataContext, parentNode, e.NodeType, e.LayoutVariant);
                showNavSelection(chapterTree.SelectedNode, false);
            }
        }

        private void loadSeedData_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            BDDataLoader dataLoader = new BDDataLoader();

            dataLoader.ImportData(dataContext, @"Resources\Chapter_1a.txt", BDDataLoader.baseDataDefinitionType.chapter1a);
            dataLoader.ImportData(dataContext, @"Resources\Chapter_1b.txt", BDDataLoader.baseDataDefinitionType.chapter1b);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1c.txt", BDDataLoader.baseDataDefinitionType.chapter1c);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1d.txt", BDDataLoader.baseDataDefinitionType.chapter1d);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1e.txt", BDDataLoader.baseDataDefinitionType.chapter1e);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1f.txt", BDDataLoader.baseDataDefinitionType.chapter1f);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1g.txt", BDDataLoader.baseDataDefinitionType.chapter1g);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1h.txt", BDDataLoader.baseDataDefinitionType.chapter1h);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1i.txt", BDDataLoader.baseDataDefinitionType.chapter1i);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1j.txt", BDDataLoader.baseDataDefinitionType.chapter1j);

            //dataLoader.ImportData(dataContext, @"Resources\Chapter_4a.txt", BDDataLoader.baseDataDefinitionType.chapter4a);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_4b.txt", BDDataLoader.baseDataDefinitionType.chapter4b);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_4c.txt", BDDataLoader.baseDataDefinitionType.chapter4c);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_4d.txt", BDDataLoader.baseDataDefinitionType.chapter4d);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_4e.txt", BDDataLoader.baseDataDefinitionType.chapter4e);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_4f.txt", BDDataLoader.baseDataDefinitionType.chapter4f);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_4g.txt", BDDataLoader.baseDataDefinitionType.chapter4g);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_4h.txt", BDDataLoader.baseDataDefinitionType.chapter4h);
            
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_5a.txt", BDDataLoader.baseDataDefinitionType.chapter5a);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_5b.txt", BDDataLoader.baseDataDefinitionType.chapter5b);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_5c.txt", BDDataLoader.baseDataDefinitionType.chapter5c);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_5d.txt", BDDataLoader.baseDataDefinitionType.chapter5d);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_5e.txt", BDDataLoader.baseDataDefinitionType.chapter5e);

            //dataLoader.ImportData(dataContext, @"Resources\Chapter_6a.txt", BDDataLoader.baseDataDefinitionType.chapter6a);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_6b.txt", BDDataLoader.baseDataDefinitionType.chapter6b);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_6c.txt", BDDataLoader.baseDataDefinitionType.chapter6c);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_6d.txt", BDDataLoader.baseDataDefinitionType.chapter6d);

            //dataLoader.ImportData(dataContext, @"Resources\Chapter_2f.txt", BDDataLoader.baseDataDefinitionType.chapter2f);

           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3a.txt", BDDataLoader.baseDataDefinitionType.chapter3a);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3b.txt", BDDataLoader.baseDataDefinitionType.chapter3b);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3c.txt", BDDataLoader.baseDataDefinitionType.chapter3c);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3d.txt", BDDataLoader.baseDataDefinitionType.chapter3d);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3e.txt", BDDataLoader.baseDataDefinitionType.chapter3e);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3f.txt", BDDataLoader.baseDataDefinitionType.chapter3f);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3g.txt", BDDataLoader.baseDataDefinitionType.chapter3g);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_3h.txt", BDDataLoader.baseDataDefinitionType.chapter3h);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3i.txt", BDDataLoader.baseDataDefinitionType.chapter3i);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3j.txt", BDDataLoader.baseDataDefinitionType.chapter3j);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_3k.txt", BDDataLoader.baseDataDefinitionType.chapter3k);
           // dataLoader.ImportData(dataContext, @"Resources\Chapter_3l.txt", BDDataLoader.baseDataDefinitionType.chapter3l);
            LoadChapterDropDown();
            BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            loadSeedDataButton.Visible = isSeedDataLoadAvailable;
            loadSeedDataButton.Enabled = false;

            this.Cursor = Cursors.Default;
        }

        private void LoadChapterDropDown()
        {
            splitContainer1.Panel2.Controls.Clear();

            chapterTree.Nodes.Clear();

            chapterDropDown.Items.Clear();
            foreach(IBDNode entry in BDFabrik.GetAllForNodeType(dataContext, BDConstants.BDNodeType.BDChapter))
            {
                chapterDropDown.Items.Add(entry);
            }

            if (chapterDropDown.Items.Count >= 1)
                chapterDropDown.SelectedIndex = 0;

        }

        private void UpdateSyncLabel()
        {
            BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);

            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;
            if (null == lastSyncDate)
            {
                lbLastSyncDateTime.Text = @"<Never Archived>";
            }
            else
            {
                lbLastSyncDateTime.Text = lastSyncDate.Value.ToString(BDConstants.DATETIMEFORMAT);
            }
        }

        private void BDEditView_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}" , "Bugs & Drugs Editor", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            // Loading Seed Data:  set the following variables

            isSeedDataLoadAvailable = false;
            
#if DEBUG
            isSeedDataLoadAvailable = false;
            this.Text = this.Text + @" < DEVELOPMENT >";
            this.btnPublish.Visible = true;
            this.btnMove.Visible = !isSeedDataLoadAvailable && moveButtonVisible;
            this.btnMove.Enabled = !isSeedDataLoadAvailable && moveButtonVisible;
#else
            this.btnPublish.Visible = false;
            
            this.btnMove.Visible = moveButtonVisible;
            this.btnMove.Enabled = moveButtonVisible;

            this.btnDebug.Visible = false;
            this.btnDebug.Enabled = false;
#endif

            BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;
            loadSeedDataButton.Visible = isSeedDataLoadAvailable;
            UpdateSyncLabel();

            AuthenticationInputBox authenticationForm = new AuthenticationInputBox();
            authenticationForm.ShowDialog(this);

            if (!BDCommon.Settings.SyncPushEnabled)
            {
                //btnSync.Text = "Pull Only";
                btnSync.Enabled = false;
                this.Text = string.Format("{0} < CLOUD DATA IS READ ONLY >", this.Text);
            }
            if (BDCommon.Settings.RepositoryOverwriteEnabled)
            {
                btnSync.Text = "Overwrite";
                this.Text = string.Format("{0} < OVERWRITE REPOSITORY >", this.Text);
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            //SyncData();
            this.Cursor = Cursors.WaitCursor;
            BDArchiveDialog archiveDialog = new BDArchiveDialog();
            if (archiveDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                Application.DoEvents();
                RepositoryHandler.Aws.Archive(dataContext, archiveDialog.Username, archiveDialog.Comment);
                MessageBox.Show("Archive complete", "Archive to Repository", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateSyncLabel();
            }
            this.Cursor = Cursors.Default;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            List<BDArchiveRecord> archiveList = RepositoryHandler.Aws.ListArchives();
            BDRestoreDialog restoreDialog = new BDRestoreDialog();
            restoreDialog.ArchiveRecordList = archiveList;
            if (restoreDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                BDArchiveRecord archiveRecord = restoreDialog.SelectedArchiveRecord;
                if (null != archiveRecord)
                {
                    this.Cursor = Cursors.WaitCursor;
                    Application.DoEvents();
                    RepositoryHandler.Aws.Restore(dataContext, archiveRecord);
                    UpdateSyncLabel();
                    LoadChapterDropDown();
                }
            }
            this.Cursor = Cursors.Default;

        }

        private void splitContainer1_Leave(object sender, EventArgs e)
        {
            save();
        }

        private void BDEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            save();
        }

        private void save()
        {
            if (splitContainer1.Panel2.Controls.Count > 0)
            {
                IBDControl control = splitContainer1.Panel2.Controls[0] as IBDControl;
                if (null != control)
                {
                    control.Save();
                }
            }
        }

        public void setStatusText(string newText)
        {
            if (newText.Length > 0)
            toolStripStatusLabel1.Text = newText;
            toolStripStatusLabel1.ToolTipText = newText;
            }


        private void btnPublish_Click(object sender, EventArgs e)
        {
            IBDNode chapterNode = null;

            bool awsPushOnly = false;

            DialogResult pushChoice = MessageBox.Show("Push to Amazon only?", "AWS", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (pushChoice == DialogResult.Yes)
                awsPushOnly = true;
            else if (pushChoice == DialogResult.No)
                awsPushOnly = false;
            else
                return;

            if (!awsPushOnly)
            {
                DialogResult result = MessageBox.Show("Generate All Chapters?", "Publish", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                    chapterNode = null;
                else if (result == DialogResult.No)
                    chapterNode = (chapterDropDown.SelectedItem as BDNode);
                else
                    return;
            }

            this.Cursor = Cursors.WaitCursor;


            Debug.WriteLine(string.Format("Start {0}", DateTime.Now));

            BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Publish Start\t{0}", DateTime.Now));

            if (!awsPushOnly)
            {
                BDHtmlPageGenerator generator = new BDHtmlPageGenerator();
                generator.Generate(dataContext, chapterNode);
                System.Diagnostics.Debug.WriteLine("HTML page generation complete.");
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Generation Complete\t{0}", DateTime.Now));

                BDSearchEntryGenerator.Generate(dataContext, chapterNode, generator.PagesMap);
                System.Diagnostics.Debug.WriteLine("Search entry generation complete. {0}", DateTime.Now);

                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Search Generation Complete\t{0}", DateTime.Now));
            }

            if (BDCommon.Settings.SyncPushEnabled)
            {
                BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                DateTime? lastSyncDate = systemSetting.settingDateTimeValue;
                Debug.WriteLine("Begin sync with AWS");
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("AWS Push Start\t{0}", DateTime.Now));
                SyncInfoDictionary syncResultList = RepositoryHandler.Aws.Sync(DataContext, null, BDConstants.SyncType.Publish);

                string resultMessage = string.Empty;

                foreach (SyncInfo syncInfo in syncResultList.Values)
                {
                    System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                    if ((syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0))
                        resultMessage = string.Format("{0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage) ? "" : "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
                }

                Debug.WriteLine(string.Format("Syc Complete at {0}", DateTime.Now));
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("AWS Push (Publish) Complete\t{0}", DateTime.Now));

                if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

                MessageBox.Show(resultMessage, "Synchronization");
            }
            else MessageBox.Show("Synchronization Disabled", "Synchronization");

            BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Publish Complete\t{0}", DateTime.Now));
            this.Cursor = Cursors.Default;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = chapterTree.SelectedNode;

            // search for a term:  open the search window
            BDSearchView searchView = new BDSearchView();
            searchView.AssignDataContext(dataContext);
            searchView.ShowDialog(this);

            if (searchView.LocationString.Length > 0)
                this.toolStripStatusLabel1.Text = string.Format("Last Search: {0}\n{1}", searchView.SearchTerm, searchView.LocationString);

            if (null != chapterTree.SelectedNode)
            {
                this.Cursor = Cursors.WaitCursor;
                IBDNode newNode = chapterTree.SelectedNode.Tag as IBDNode;
                if (newNode.Name != treeNode.Text)
                    treeNode.Text = newNode.Name;
                showNavSelection(treeNode, false);
                this.Cursor = Cursors.Default;
            }
        }

        private void chapterTree_DragDrop(object sender, DragEventArgs e)
        {
            bool validTarget = false;
            IBDNode targetNode = null;
            IBDNode sourceNode = null;

            TreeNode targetTreeNode = null;
            TreeNode sourceTreeNode = null;
            TreeNode parentTreeNode = null;

            targetTreeNode = this.chapterTree.GetNodeAt(this.chapterTree.PointToClient(new Point(e.X, e.Y)));
            if (targetTreeNode != null)
            {
                targetNode = targetTreeNode.Tag as IBDNode;
                

                if (null != targetNode)
                {
                    if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
                    {
                        sourceTreeNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                        parentTreeNode = sourceTreeNode.Parent;
                        sourceNode = sourceTreeNode.Tag as IBDNode;
                        validTarget = (((sourceNode.Uuid != targetNode.Uuid) && (sourceNode.ParentId == targetNode.ParentId)) || (targetNode.Uuid == sourceNode.ParentId));
                    }
                }
            }

            if (validTarget)
            {
                IBDNode parentNode = BDFabrik.RetrieveNode(this.DataContext, sourceNode.ParentType, sourceNode.ParentId);
                List<IBDNode> siblingList = BDFabrik.GetChildrenForParent(this.DataContext, parentNode);
                Debug.WriteLine("sibling count is: {0}",siblingList.Count());

                siblingList.Remove(sourceNode);
                int targetIdx = (targetNode.Uuid == sourceNode.ParentId) ? 0 : siblingList.IndexOf(targetNode) + 1;
                siblingList.Insert(targetIdx, sourceNode);

                for (int idx = 0; idx < siblingList.Count; idx++)
                {
                    siblingList[idx].DisplayOrder = idx;
                    BDFabrik.SaveNode(this.DataContext, siblingList[idx]);
                }

                if (null != parentTreeNode)
                {
                    parentTreeNode.Nodes.Remove(sourceTreeNode);
                    parentTreeNode.Nodes.Insert(targetIdx, sourceTreeNode);
                    Debug.WriteLine("---");
                }
                else
                {
                    IBDNode listEntry = chapterDropDown.SelectedItem as IBDNode;
                    displayChapter(listEntry);
                }
            }
            else
                Debug.WriteLine("drop target is not valid");

        }

        private void chapterTree_DragEnter(object sender, DragEventArgs e)
        {
            //e.Effect = DragDropEffects.Move;
        }

        private void chapterTree_DragOver(object sender, DragEventArgs e)
        {
            bool validTarget = false;

            TreeNode nodeToDropIn = this.chapterTree.GetNodeAt(this.chapterTree.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn != null)
            {
                IBDNode targetNode = nodeToDropIn.Tag as IBDNode;
                if (null != targetNode)
                {
                    if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
                    {
                        TreeNode source = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                        IBDNode sourceNode = source.Tag as IBDNode;
                        validTarget = (((sourceNode.Uuid != targetNode.Uuid) &&  (sourceNode.ParentId == targetNode.ParentId)) || (targetNode.Uuid == sourceNode.ParentId) );
                    }
                }
            }

            if (validTarget)
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void chapterTree_DragLeave(object sender, EventArgs e)
        {

        }

        private void chapterTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void btnShowLayoutEditor_Click(object sender, EventArgs e)
        {
            BDLayoutMetadataEditor editor = new BDLayoutMetadataEditor(DataContext);
            Application.DoEvents();
            editor.ShowDialog();

        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            // These operations are CUSTOM, ** BY REQUEST ONLY **

            // the following are complete, but left as samples.

            #region earlier
            /*
            // move Pharyngitis to Respiratory
            BDNode node = BDNode.RetrieveNodeWithId(dataContext, new Guid("a55e8e97-16cf-4074-ae98-26627bb25143"));
            BDUtilities.MoveNode(dataContext, node, "Respiratory");

            // move Zoster subentries to Vesicular Lesions
            BDUtilities.MoveAllChildren(dataContext, new Guid("10dce9db-6b5a-4249-b156-c86b18635852"), new Guid("5ce93aff-137f-487b-8f8a-1e9076acc8cb"));

            // Move Gastroenteritis Mild-moderate to Gastroenteritis - Severe 
            //BDNode node = BDNode.RetrieveNodeWithId(dataContext, new Guid("f80d2660-66c0-4640-9d35-c4f90a369a97"));
            //BDUtilities.MoveNode(dataContext, node, "Gastroenteritis - Severe");
             */

            //BDNode node = BDNode.RetrieveNodeWithId(dataContext, new Guid("10dce9db-6b5a-4249-b156-c86b18635852"));
            //node.parentId = new Guid("5ce93aff-137f-487b-8f8a-1e9076acc8cb");
            //node.parentKeyName = "BDDisease";
            //node.nodeKeyName = "BDPresentation";
            //node.nodeType = (int)BDConstants.BDNodeType.BDPresentation;

            //BDNode.Save(dataContext, node);

            // reset node type for all table cells
            //BDUtilities.SetTableCellNodeType(dataContext);

            // reset layout variant for TreatmentRecommendation>Adult>Genital>GenitalUlcers
            //BDUtilities.SetDiseaseLayoutVariant(dataContext);

            // repair hierarchy for Genital Ulcers/ lesions in Treatment Recommendations > Adult > Genital...
            // BDUtilities.InjectNodeIntoHierarhy(dataContext);

            // reset all layoutVariants for children of Adults > SSTI > Vesicular Lesions
            // IBDNode startNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("5ce93aff-137f-487b-8f8a-1e9076acc8cb"));
            // BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode,BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions, false);

            // reset all layoutVariants for children of Peds > SSTI > Vesicular Lesions
            // IBDNode startNode2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a088c4d8-46d6-4b9f-a31b-bbf8a64f22e9"));
            // BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode2, BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions, false);

            // reset layoutVariant for Adults > Respiratory > Table xx Treatment of culture-proven pneumonia and all children 
            //IBDNode startNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("391bd1a4-daca-44e8-8fda-863f22128f1f"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode, BDConstants.LayoutVariantType.TreatmentRecommendation15_CultureProvenPneumonia, true);

            ////Gastroenteritis node: 8bb5b60e-68fd-4df2-bb77-f8f6e2f17c48 adult  d1c0f6ca-a501-4635-9241-aaeded1877be paediatric
            //// Change layoutVariant from TreatmentRecommendation01 (101) to TreatmentRecommendation01_Gastroenteritis (10141)
            //IBDNode startNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("8bb5b60e-68fd-4df2-bb77-f8f6e2f17c48"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis, true);

            //IBDNode startNode2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d1c0f6ca-a501-4635-9241-aaeded1877be"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode2, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis, true);

            //// Move children of Severe to culture-directed
            //// severe #1 e83a8e3f-10b4-4677-aa73-197aa8ce5c8c , target & exception: 879a8931-648f-4e16-9704-2d30ab0d76f9 exception: 14be53d4-a30a-4037-98a1-077445d78202
            //// severe #2 f80d2660-66c0-4640-9d35-c4f90a369a97 , target & exception: 874a2a14-c7c2-41dd-8799-80c13392441b exception: 62c6cfcc-cf6d-4640-9335-4327e937d415

            //List<Guid> exceptionList1 = new List<Guid>();
            //List<Guid> exceptionList2 = new List<Guid>();

            //exceptionList1.Add(Guid.Parse("879a8931-648f-4e16-9704-2d30ab0d76f9"));
            //exceptionList1.Add(Guid.Parse("14be53d4-a30a-4037-98a1-077445d78202"));

            //exceptionList2.Add(Guid.Parse("874a2a14-c7c2-41dd-8799-80c13392441b"));
            //exceptionList2.Add(Guid.Parse("62c6cfcc-cf6d-4640-9335-4327e937d415"));

            //BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("e83a8e3f-10b4-4677-aa73-197aa8ce5c8c"), Guid.Parse("879a8931-648f-4e16-9704-2d30ab0d76f9"),  exceptionList1);
            //BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("f80d2660-66c0-4640-9d35-c4f90a369a97"), Guid.Parse("874a2a14-c7c2-41dd-8799-80c13392441b"), exceptionList2);

            //IBDNode startNode = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("879a8931-648f-4e16-9704-2d30ab0d76f9"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected, true);

            // //IBDNode startNode2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("874a2a14-c7c2-41dd-8799-80c13392441b"));
            // //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode2, BDConstants.LayoutVariantType.TreatmentRecommendation01_Gastroenteritis_CultureDirected, true);
            // exceptionList1 = new List<Guid>();
            // //Move the "single presentation" children to the parent of the presentation
            // BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("2af20800-f237-4e7c-b270-a91ea35f3d13"), Guid.Parse("9e322d09-ae3d-41a2-b7c9-c6ef706bc986"), exceptionList1);
            // BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("77637ddd-81a6-4d9a-a92f-f7811c6e9446"), Guid.Parse("e10f7eb9-66d5-4225-b01e-06a0acefe34c"), exceptionList1);

            // //Move the children of (Adult) Necrotizing fasciitis/myositis down into the manually created subsection
            // exceptionList1 = new List<Guid>();
            // exceptionList1.Add(Guid.Parse("1323d38a-2cd4-4f38-a976-09ce67dae5ec"));
            // BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("9e2409c4-1ef0-49a9-ab20-887954f25ca0"), Guid.Parse("1323d38a-2cd4-4f38-a976-09ce67dae5ec"), exceptionList1);

            // //Move the children of (Child) Necrotizing fasciitis/myositis down into the manually created subsection
            // exceptionList1 = new List<Guid>();
            // exceptionList1.Add(Guid.Parse("91da8752-752a-43de-a315-159e3baeb838"));
            // BDUtilities.MoveAllChildrenExcept(dataContext, Guid.Parse("44e9114b-d3d5-4fa6-a56f-ce3eb346d6eb"), Guid.Parse("91da8752-752a-43de-a315-159e3baeb838"), exceptionList1);

            // // Manually change the nodeType and NodeKeyName from disease to presentation (5 - BDDisease) (6 - BDPresentation)
            // // Manually change parent info from (3 - BDCategory) to (5 - BDDisease)
            ////  9e322d09-ae3d-41a2-b7c9-c6ef706bc986	Synergistic necrotizing cellulitis, Fournier's gangrene	
            ////  e10f7eb9-66d5-4225-b01e-06a0acefe34c	Gas gangrene

            // delete orphan table
            //BDNode.DeleteLocal(dataContext, Guid.Parse(@"5c75848a-bf64-45e5-93fe-670e8790a13c"));

            //// create new antimicrobialSection for 'Vancomycin Dosing And Monitoring...'
            //BDNode chapter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"45e13826-aedb-48d0-baf6-2f06ff45017f"));
            //BDNode antimicrobialSection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection);
            //antimicrobialSection.name = @"Vancomycin Dosing and Monitoring Guidelines";
            //antimicrobialSection.SetParent(chapter);
            //antimicrobialSection.displayOrder = 0;
            //antimicrobialSection.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring;
            //BDNode.Save(dataContext, antimicrobialSection);

            //// retrieve Vancomycin Dosing - adjust type and make a child of the new antimicrobialSection 
            //BDNode vd = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"2dd4578a-a856-49e4-852a-e18eb43fb1b2"));
            //List<IBDNode> vdChildren = BDFabrik.GetChildrenForParent(dataContext, vd);
            //vd.SetParent(antimicrobialSection);
            //vd.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            //vd.nodeKeyName = vd.nodeType.ToString();
            //BDNode.Save(dataContext, vd);
            //foreach (IBDNode vdChild in vdChildren)
            //{   // reset parentType and parentKeyName or lookups will break
            //    vdChild.SetParent(vd);
            //    BDNode.Save(dataContext, vdChild as BDNode);
            //}
            //// retrieve Vancomycin Monitoring
            //BDNode vm = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"10a23f72-58ea-49c2-ad3f-d8d97d1846c6"));
            //List<IBDNode> vmChildren = BDFabrik.GetChildrenForParent(dataContext, vm);
            //vm.SetParent(antimicrobialSection);
            //vm.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            //vm.nodeKeyName = vm.nodeType.ToString();
            //BDNode.Save(dataContext, vm);
            //foreach (IBDNode vmChild in vmChildren)
            //{
            //    vmChild.SetParent(vm);
            //    BDNode.Save(dataContext, vmChild as BDNode);
            //}

            // move values from BDString object up to table cell as 'value'
            //List<BDTableCell> cells = BDTableCell.RetrieveAll(dataContext);
            //foreach (BDTableCell cell in cells)
            //{
            //    StringBuilder cellString = new StringBuilder();
            //    List<BDString> strings = BDString.RetrieveStringsForParentId(dataContext, cell.Uuid);
            //    foreach (BDString stringValue in strings)
            //        cellString.Append(stringValue.value);

            //    cell.value = cellString.ToString();
            //    BDTableCell.Save(dataContext, cell);
            //}

            //// fetch the antimicrobial name listing table
            //BDNode nameTable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"13acf8a8-6f81-4ba1-a5ca-620dc978596b"));
            //if (nameTable.name == "Antimicrobial Formulary and Generic / Trade Name Listing")
            //{
            //    List<BDNode> nameTableSections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, nameTable.Uuid, BDConstants.BDNodeType.BDSection);
            //    // Adjust layout variant for children
            //    foreach (BDNode tableSection in nameTableSections)
            //    {
            //        List<BDNode> subsections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, tableSection.Uuid, BDConstants.BDNodeType.BDSubsection);
            //        BDUtilities.ResetLayoutVariantInTable3RowsForParent(dataContext, tableSection, BDConstants.LayoutVariantType.Antibiotics_NameListing);
            //        tableSection.LayoutVariant = nameTable.LayoutVariant;
            //        BDNode.Save(dataContext, tableSection);
            //        foreach (BDNode subsection in subsections)
            //        {
            //            BDUtilities.ResetLayoutVariantInTable3RowsForParent(dataContext, subsection, BDConstants.LayoutVariantType.Antibiotics_NameListing);
            //            subsection.LayoutVariant = nameTable.LayoutVariant;
            //            BDNode.Save(dataContext, subsection);
            //        }
            //    }
            //    // reset header tableChildren
            //    BDUtilities.ResetLayoutVariantInTable3RowsForParent(dataContext, nameTable, BDConstants.LayoutVariantType.Antibiotics_NameListing);
            //}

            //// move Adults > SSTI > Fournier's Gangrene to presentation level of Rapidly progressive SSTI
            //BDUtilities.MoveNodeToParentSibling(dataContext, Guid.Parse(@"9e322d09-ae3d-41a2-b7c9-c6ef706bc986"), Guid.Parse(@"6e4c8849-ad12-4b5f-b5fc-5fc53288cfad"), "SINGLE PRESENTATION", BDConstants.BDNodeType.BDPresentation);

            //// move Adults > SSTI > Gas Gangrene to presentation level of Rapidly progressive SSTI
            //BDUtilities.MoveNodeToParentSibling(dataContext, Guid.Parse(@"e10f7eb9-66d5-4225-b01e-06a0acefe34c"), Guid.Parse(@"6e4c8849-ad12-4b5f-b5fc-5fc53288cfad"), "SINGLE PRESENTATION", BDConstants.BDNodeType.BDPresentation);
            #endregion
            #region v.1.6.12
            // fetch the antimicrobial stepdown table
            //BDNode nameTable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"45301fa9-55ac-4c8f-95f0-1008016635c4"));
            //if (nameTable.name == "Stepdown Recommendations")
            //{
            //    List<BDNode> nameTableSections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, nameTable.Uuid, BDConstants.BDNodeType.BDTableSection);
            //    // Adjust layout variant for children
            //    foreach (BDNode tableSection in nameTableSections)
            //    {
            //        List<BDNode> subsections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, tableSection.Uuid, BDConstants.BDNodeType.BDSubsection);
            //        BDUtilities.ResetLayoutVariantInTable5RowsForParent(dataContext, tableSection, BDConstants.LayoutVariantType.Antibiotics_Stepdown);
            //        tableSection.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Stepdown;
            //        BDNode.Save(dataContext, tableSection);
            //        foreach (BDNode subsection in subsections)
            //        {
            //            BDUtilities.ResetLayoutVariantInTable5RowsForParent(dataContext, subsection, BDConstants.LayoutVariantType.Antibiotics_Stepdown);
            //            subsection.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_Stepdown;
            //            BDNode.Save(dataContext, subsection);
            //        }
            //    }
            //    // reset header tableChildren
            //    BDUtilities.ResetLayoutVariantInTable5RowsForParent(dataContext, nameTable, BDConstants.LayoutVariantType.Antibiotics_Stepdown);
            //}

            //BDNode vancomycin = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse(@"c42a29c2-f5a1-48a4-b140-3e4dae56b445"));
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, vancomycin, BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin, true);

            #endregion
            #region v.1.6.13
            /*
            // move selected tables out of Treatment Recommendations > Adults into new antimicrobialSection Treatment recommendations > Culture Directed Infections in Adults
            // create new antimicrobialSection in TreatmentRecoomendations chapter
            BDNode chapter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("f92fec3a-379d-41ef-a981-5ddf9c9a9f0e"));
            BDNode newSection = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection);
            newSection.SetParent(chapter);
            newSection.name = "Recommended Empiric Therapy of Culture-Directed Infections in Adult Patients";
            newSection.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation16_CultureDirected;
            BDNode.Save(dataContext, newSection);

            // move related tables to new antimicrobialSection & assign new layout variant
            // get culture-proven pneumonia table
            BDNode table1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("391bd1a4-daca-44e8-8fda-863f22128f1f"));
            table1.SetParent(newSection);
            table1.DisplayOrder = 0;
            BDNode.Save(dataContext, table1);

            // get culture-proven PD Peritonitis table
            BDNode table2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("5b8548c8-0e54-4cb5-910e-2f55a41f9ecc"));
            table2.SetParent(newSection);
            table2.DisplayOrder = 1;
            BDNode.Save(dataContext, table2);

            // get culture-proven meningitis table
            BDNode table3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("fc25fee8-8ded-4d41-895d-b415bab02eb7"));
            table3.SetParent(newSection);
            table3.DisplayOrder = 2;
            BDNode.Save(dataContext, table3);

            // get culture-proven endocarditis table
            BDNode table4 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("8741d365-16ef-4de8-8d08-dabc574c010a"));
            table4.SetParent(newSection);
            table4.DisplayOrder = 3;
            BDNode.Save(dataContext, table4);

            // get culture-proven BCNE table
            BDNode table5 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("b38a1c03-2f74-4b08-b104-0da7f054c529"));
            table5.SetParent(newSection);
            table5.DisplayOrder = 4;
            BDNode.Save(dataContext, table5);

            // change pathogen groups in Antimicrobial_ClinicalGuidelines  to topics
            List<BDNode> therapyGroups = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDPathogenGroup);
            foreach(BDNode therapyGroup in therapyGroups)
            {
                if (therapyGroup.LayoutVariant == BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines)
                {
                    if (therapyGroup.Name == "Predictable Activity" || therapyGroup.Name == "Unpredictable Activity" || therapyGroup.Name == "No/insufficient Activity")
                    {
                        therapyGroup.nodeType = (int)BDConstants.BDNodeType.BDTopic;
                        therapyGroup.nodeKeyName = BDConstants.BDNodeType.BDTopic.ToString();
                        BDNode.Save(dataContext, therapyGroup);
                    }
                }
            }

            // add categories into Antimicrobial_ClinicalGuidelines, move child nodes of the antimicrobialSection into the categories.
            BDNode antimicrobialSection = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("01933c56-dedf-4c1a-9191-d541819000d8"));
            BDNode c1 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory);
            c1.SetParent(antimicrobialSection);
            c1.name = "ANTIMICROBIAL SPECTRUM";
            c1.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum;
            c1.DisplayOrder = 0;
            BDNode.Save(dataContext, c1);

            BDNode c2 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory);
            c2.SetParent(antimicrobialSection);
            c2.Name = "ANTIMICROBIAL AGENTS";
            c2.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            c2.DisplayOrder = 1;
            BDNode.Save(dataContext, c2);

            BDNode c3 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory);
            c3.SetParent(antimicrobialSection);
            c3.Name = "ANTIFUNGALS";
            c3.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            c3.DisplayOrder = 2;
            BDNode.Save(dataContext, c3);

            // move selected Antimicrobials to new category for Antifungals
            BDNode af1 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("bf1d8db1-7243-4139-90b9-60f10c2e4953"));
            af1.SetParent(c3);
            BDNode.Save(dataContext, af1);

            BDNode af2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("7a0fd527-3afd-4f57-9419-1d56eaf17110"));
            af2.SetParent(c3);
            BDNode.Save(dataContext, af2);

            BDNode af3 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("afe6f889-4289-4255-9dd4-be193568bbbf"));
            af3.SetParent(c3);
            BDNode.Save(dataContext, af3);

            BDNode af4 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial);
            af4.SetParent(c3);
            af4.Name = "ITRACONAZOLE";
            af4.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            BDNode.Save(dataContext, af4);

            BDNode af5 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial);
            af5.SetParent(c3);
            af5.Name = "MICAFUNGIN IV";
            af5.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            BDNode.Save(dataContext, af5);

            BDNode af6 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDAntimicrobial);
            af6.SetParent(c3);
            af6.Name = "POSACONAZOLE PO";
            af6.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines;
            BDNode.Save(dataContext, af6);

            BDNode af7 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ff32e382-13b6-4b36-8b6e-f40734bdcc1e"));
            af7.SetParent(c3);
            BDNode.Save(dataContext, af7);

            // get remaining children of original section, change all to children of new category for antimicrobial agents (c2)
            List<BDNode> antimicrobials = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, antimicrobialSection.Uuid, BDConstants.BDNodeType.BDAntimicrobial);
            foreach (BDNode am in antimicrobials)
            {
                am.SetParent(c2);
                BDNode.Save(dataContext, am);
            }

            // create new topic in c1 for 'Organism Groups'
            BDNode o1 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic);
            o1.SetParent(c1);
            o1.Name = "Organism Groups";
            o1.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum;
            BDNode.Save(dataContext, o1);

            // create new subtopics for 'Organism Groups'
            BDNode st1 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st1.SetParent(o1);
            st1.Name = "Viridans Group Streptococci";
            st1.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st1);

            BDNode st2 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st2.SetParent(o1);
            st2.Name = "Enterobacteriaceae";
            st2.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st2);

            BDNode st3 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st3.SetParent(o1);
            st3.Name = "Coryneform bacteria";
            st3.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st3);

            BDNode st4 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st4.SetParent(o1);
            st4.Name = "Enterobacteriaceae that produce inducible cephalosporinase";
            st4.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st4);

            BDNode st5 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st5.SetParent(o1);
            st5.Name = "Staphylococci spp";
            st5.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st5);

            BDNode st6 = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubtopic);
            st6.SetParent(o1);
            st6.Name = "Streptococcus pneumoniae";
            st6.LayoutVariant = o1.LayoutVariant;
            BDNode.Save(dataContext, st6);   */
            #endregion
            #region v.1.6.14
            /*
            // fix incorrect assignment of property 
            BDLinkedNoteAssociation lna = BDLinkedNoteAssociation.GetLinkedNoteAssociationWithId(dataContext, Guid.Parse("7cbbf37c-f7bd-4d75-b4a3-abe70a206c8f"));
            lna.parentKeyPropertyName = "Duration";
            BDLinkedNoteAssociation.Save(dataContext, lna);

            // Reset layout variant on Adults > SSTI > Cellulitis > Extremities
            BDNode ext = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d8e71b5c-2456-40b9-8886-d202d436f314"));
            ext.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation14_CellulitisExtremities;
            BDNode.Save(dataContext, ext);

            // Reset layout variant for Genital Ulcers nodes and children
            IBDNode startNode2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("96cbc7d0-c4ba-4593-a1d9-0e7908deeffd"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode2, BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers, true);

            // create new layer under Treatment Recommendations for Pneumonia CURB table sections
            BDNode respiratoryCategory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("0b65f9e2-7436-4c22-9785-93be2b7e4f5a"));
            BDNode curbSubcategory = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSubcategory);
            curbSubcategory.SetParent(respiratoryCategory);
            curbSubcategory.Name = "Table 3.  CURB-65 - Pneumonia Severity of Illness Scoring System";
            curbSubcategory.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation17_Pneumonia;
            BDNode partOne = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("9f5604d2-7c8c-4216-aba2-6a12fa3c58bd"));
            curbSubcategory.DisplayOrder = partOne.DisplayOrder;
            BDNode.Save(dataContext, curbSubcategory);

            partOne.SetParent(curbSubcategory);
            BDNode.Save(dataContext, partOne);

            BDNode partTwo = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("22437404-f46a-46c3-badb-dfe50aa6afd3"));
            partTwo.SetParent(curbSubcategory);
            BDNode.Save(dataContext, partTwo);

            // remove layer from hierarchy in CP Meningitis
            BDNode mTable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("fc25fee8-8ded-4d41-895d-b415bab02eb7"));
            BDNode pGroup = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d677df95-937a-41d9-b879-ef01af8ca99b"));
            List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(dataContext, pGroup);
            foreach (IBDNode pathogen in pathogens)
            {
                BDNode node = pathogen as BDNode;
                if (node != null)
                {
                    node.SetParent(mTable);
                    BDNode.Save(dataContext, node);
                }
            }
            BDNode.Delete(dataContext, pGroup, false);

            // remove layer from CP Pneumonia
            BDNode pTable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("391bd1a4-daca-44e8-8fda-863f22128f1f"));
            BDNode pGroup2 = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a39027d0-758f-45db-bc5f-97f73b6081d0"));
            List<IBDNode> nodes = BDFabrik.GetChildrenForParent(dataContext, pGroup2);
            foreach (IBDNode pathogen in nodes)
            {
                BDNode node = pathogen as BDNode;
                if (node != null)
                {
                    node.SetParent(pTable);
                    BDNode.Save(dataContext, node);
                }
            }
            BDNode.Delete(dataContext, pGroup2, false); */
            #endregion
            #region v.1.6.16
            /*
            // Set new layoutvariant in table TR > Paediatrics > Repiratory
            BDNode xxtable = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("c77ee0f1-e2f8-4e18-82c5-b4ab0bc917cc"));
            List<IBDNode> pgroups = BDFabrik.GetChildrenForParent(dataContext, xxtable);
            foreach (IBDNode pathogenGroup in pgroups)
            {
                List<IBDNode> pathogens = BDFabrik.GetChildrenForParent(dataContext, pathogenGroup);
                foreach (IBDNode pathogen in pathogens)
                {
                    List<IBDNode> pathogenResistances = BDFabrik.GetChildrenForParent(dataContext, pathogen);
                    foreach(IBDNode pathogenResistance in pathogenResistances)
                     {
                        List<IBDNode> therapyGroups = BDFabrik.GetChildrenForParent(dataContext, pathogenResistance);
                        foreach (IBDNode therapyGroup in therapyGroups)
                        {
                            List<IBDNode> therapies = BDFabrik.GetChildrenForParent(dataContext, therapyGroup);
                            foreach (IBDNode therapy in therapies)
                            {
                                therapy.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics;
                                BDFabrik.SaveNode(dataContext, therapy);
                            }
                            therapyGroup.Name = string.Empty;
                            therapyGroup.SetParent(xxtable);
                            therapyGroup.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics;
                            BDFabrik.SaveNode(dataContext, therapyGroup);
                        }
                        BDFabrik.DeleteNode(dataContext, pathogenResistance);
                    }
                    BDFabrik.DeleteNode(dataContext, pathogen);
                }
                BDFabrik.DeleteNode(dataContext, pathogenGroup);
            }
            xxtable.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation18_CultureProvenEndocarditis_Paediatrics;
            BDFabrik.SaveNode(dataContext, xxtable);

            // Delete the 'indicated' and 'not indicated' topics from Antibiotic Guidelines > Antimicrobials 
            BDNode category = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("1997c207-d754-4907-8e32-6722f7578641"));
            List<IBDNode> antimicrobials = BDFabrik.GetChildrenForParent(dataContext, category);
            foreach (IBDNode antimicrobial in antimicrobials)
            {
                List<IBDNode> topics = BDFabrik.GetChildrenForParent(dataContext, antimicrobial);
                foreach (IBDNode topic in topics)
                {
                    if (topic.Name == "Indicated" || topic.Name == "Not Indicated")
                        BDFabrik.DeleteNode(dataContext, topic);
                }
            }

            BDNode pdAdults = BDNode.RetrieveNodeWithId(dataContext,Guid.Parse("7366ae8d-03e9-4e3e-bf7d-8037e544deb4"));
            BDNode pdPeds = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("267febcb-f1da-431c-94df-1c612a6e7d3e"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, pdAdults, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Adult, true);
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, pdPeds, BDConstants.LayoutVariantType.TreatmentRecommendation19_Peritonitis_PD_Paediatric, true);

            // Reset layout variants for Antimicrobial generic/trade name listing
            BDNode table = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("13acf8a8-6f81-4ba1-a5ca-620dc978596b"));
            List<BDNode> tableSections = BDNode.RetrieveNodesForParentIdAndChildNodeType(dataContext, table.Uuid, BDConstants.BDNodeType.BDTableSection);
            foreach (BDNode section in tableSections)
            {
                section.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_NameListing;
                BDFabrik.SaveNode(dataContext, section);
            }
            BDTableCell cell3 = BDTableCell.RetrieveWithId(dataContext, Guid.Parse("d829e699-13a2-49e9-a2a8-3701c0784920"));
            BDTableCell.Delete(dataContext, cell3, false);
            */

            #endregion
            #region v.1.6.18
            /*
            // add layout variants for subset of Antibiotics Dosing Guide and Daily costs:  split to peds and adults
            // needed for unique titles in output
            BDNode adultSubcategory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("66a8dd87-98a7-4886-8d91-ab0d1e5457d8"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, adultSubcategory, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, true);
            BDNode pedsSubcategory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a99f114d-4bce-4389-922c-4396d1894c14"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, pedsSubcategory, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, true);

            // create new layouts : Hepatic Impairment Grading table
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading, 0, "Score",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading, 1, "1",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 0,"");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading, 2, "2",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_HepaticImpairment_Grading, 3, "3",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD03, 0, "");

            // create layout for CSF Penetration / Intrathecal and/or Intraventricular
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages, 0, "Drug",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages, 1, "Doses",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD01, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_CSFPenetration_Dosages, 2, "Reported severe adverse effects",
                BDConstants.BDNodeType.BDConfiguredEntry, BDConfiguredEntry.PROPERTYNAME_FIELD02, 0, "");
            
            // create footnote for Blood/Body Fluid Exposure
            BDUtilities.CreateFootnoteForLayoutColumn(dataContext, BDConstants.LayoutVariantType.Prophylaxis_FluidExposure_Followup_II, 3, "A non-responder is a person with inadequate response to vaccination (i.e anti-HBsAg <10IU/L)");

            // create layout for Antibiotics Dosing guide & daily costs - Peds
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 0, "Antimicrobial",
                BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 1, "Recommended Paediatric Dose",
                BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, 0, @"mg/kg/d = milligrams per kilogram per <u>day</u>.  Usual doses for paediatric patients with normal renal and hepatic function.  Paediatric dose should not exceed recommended adult dose (except for cefuroxime where maximum is 1.5g IV q8h).  For disease-specific dosing see Recommended Empiric Therapy in Neonatal/Paediatric Patients.");
            BDUtilities.CreateFootnoteForLayoutColumn(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 1, "These doses do not apply to neonates, except where noted.");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 2, "Cost ($)/Day",
                BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_COST, 0, "Based on a 20kg child.");
            BDUtilities.CreateFootnoteForLayoutColumn(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, 2, "Based on Alberta Health Drug Benefit List price, January 2006, or manufacturer's list price if drug not on AH DBL.  Prices in the hospital setting may be significantly different due to contract pricing.  Check with pharmacy for actual prices.  Does not include preparation, administration, supplies or serum level costs.");
            */
            #endregion
            #region v.1.6.20
            /*
            // adjust layout variant for Dosing Guide and Daily Costs - Paediatric
            BDNode category = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a99f114d-4bce-4389-922c-4396d1894c14"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, category, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric, true);
            */
            #endregion
            #region v.1.6.22
            /*
            // adjust layout variant for Dosing Guide and Daily Costs - Adult
            BDNode aCategory = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("66a8dd87-98a7-4886-8d91-ab0d1e5457d8"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, aCategory, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, true);

            // set value to 'Dosage'
            IQueryable<BDLinkedNoteAssociation> linkedNoteAssociations = (from bdLinkedNoteAssociations in dataContext.BDLinkedNoteAssociations
                                                                          where bdLinkedNoteAssociations.parentType == 18
                                                                          orderby bdLinkedNoteAssociations.displayOrder
                                                                          select bdLinkedNoteAssociations);

            List<BDLinkedNoteAssociation> resultList = linkedNoteAssociations.ToList<BDLinkedNoteAssociation>();
            foreach (BDLinkedNoteAssociation assn in resultList)
                assn.parentKeyPropertyName = BDDosage.PROPERTYNAME_DOSAGE;

            dataContext.SaveChanges();
            */
            #endregion
            #region v.1.6.23
            /*
            // create layout for Antibiotics Dosing guide & daily costs - Adults
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, 0, "Antimicrobial",
                BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, 1, "Normal Adult Dose",
                BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, 0, @"Based on a 70kg adult with normal renal and hepatic function.  For disease-specific dosing, see also Recommended Empiric Therapy in Adult Patients and/or Recommended Therapy of Culture-Directed Infections in Adult Patients.");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult, 2, "Cost ($)/Day",
                BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_COST, 0, "Based on Alberta Health Drug Benefit List (AH DBL) price, September 2012, or manufacturer's list price or wholesale price if drug not on AH DBL.  Prices in the hospital setting may be significantly different due to contract pricing.  Check with pharmacy for actual prices.  Does not include administration, supplies, or serum level costs.");
            */
            #endregion
            #region v.1.6.24
            /*
            // fix child layout variants for vancomycin section
            BDNode vancSection = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("c42a29c2-f5a1-48a4-b140-3e4dae56b445"));
            BDUtilities.ResetLayoutVariantWithChildren(dataContext, vancSection, BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin, true);

            // create layout metadata for Dental Prophylaxis
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Dental_Prophylaxis, 0, "Antimicrobial", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Dental_Prophylaxis, 1, "Adult Dose", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Dental_Prophylaxis, 2, "Paediatric Dose", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, 0, "");
            */
            #endregion
            #region v.1.6.26
            /*
            // layout metadata for Renal impairment
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment, 0, "Antimicrobial", BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment, 1, "Normal Adult<br>Dose", BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment, 2, "Creatinine Clearance (mL/min)", BDConstants.BDNodeType.BDDosage, BDDosage.PROPERTYNAME_DOSAGE2, 0, ">50mL/min = >0.83mL/s; 10-50mL/mon = 0.17-0.83mL/s; <10mL/min = <0.17mL/s");
            // reset layout variant for Aminoglycoside conventional dosing and monitoring to accommodate adult/peds category in hierarchy
            BDNode section = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ca7eefb1-81f8-465e-8818-34a35d92736a"));
            List<IBDNode> sectionChildren = BDFabrik.GetChildrenForParent(dataContext, section);

            BDNode categoryA = BDFabrik.CreateNode(dataContext, BDConstants.BDNodeType.BDCategory, section.Uuid, BDConstants.BDNodeType.BDSection) as BDNode;
            categoryA.DisplayOrder = 0;
            categoryA.Name = "ADULTS";
            categoryA.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional;
            BDNode.Save(dataContext, categoryA);

            BDNode categoryP = BDFabrik.CreateNode(dataContext, BDConstants.BDNodeType.BDCategory, section.Uuid, BDConstants.BDNodeType.BDSection) as BDNode;
            categoryP.DisplayOrder = 1;
            categoryP.Name = "PAEDIATRICS";
            categoryP.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional;
            BDNode.Save(dataContext, categoryP);

            foreach (IBDNode child in sectionChildren)
                child.SetParent(categoryA);
            dataContext.SaveChanges();

            BDUtilities.ResetLayoutVariantWithChildren(dataContext, section, BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional, true);
            dataContext.SaveChanges();
            */
            #endregion
            #region v.1.6.27
            /*
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation, 0, "Bacterial Morphology", BDConstants.BDNodeType.BDSubcategory, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation, 1, "Probable Organisms", BDConstants.BDNodeType.BDMicroorganism, BDNode.PROPERTYNAME_NAME, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 0, "DRUG", BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 1, "RISK CATEGORY", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 2, "AAP RATING", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_APPRATING, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation, 3, "RELATIVE INFANT DOSE", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 0, "DRUG", BDConstants.BDNodeType.BDAntimicrobial, BDNode.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 1, "FDA RISK CATEGORY", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy, 2, "RECOMMENDATION", BDConstants.BDNodeType.BDAntimicrobialRisk, BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION, 0, "");
            
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection, 0, "Antimicrobial Regimen", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection, 1, "Dose & Duration", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, 0, "");

            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage, 0, "SITUATION", BDConstants.BDNodeType.BDTherapyGroup, BDTherapyGroup.PROPERTYNAME_NAME, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage, 1, "DRUG", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_THERAPY, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage, 2, "ADULT DOSE", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE, 0, "");
            BDUtilities.ConfigureLayoutMetadata(dataContext, BDConstants.LayoutVariantType.Prophylaxis_IEDrugAndDosage, 3, "PAEDIATRIC DOSE", BDConstants.BDNodeType.BDTherapy, BDTherapy.PROPERTYNAME_DOSAGE_1, 0, "");

            BDNode ieProphylaxis = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("376b287e-1d80-40f5-bb0b-512e52720687"));
            ieProphylaxis.LayoutVariant = BDConstants.LayoutVariantType.Prophylaxis_IERecommendation;
            BDNode.Save(dataContext, ieProphylaxis);
            
            BDNode intro = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.NewGuid());
            intro.Name = "FrontMatter";
            intro.DisplayOrder = 0;
            intro.parentId = Guid.Empty;
            intro.parentKeyName = BDUtilities.GetEnumDescription(BDConstants.BDNodeType.None);
            intro.LayoutVariant = BDConstants.LayoutVariantType.FrontMatter;
            BDNode antibiotics = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("45e13826-aedb-48d0-baf6-2f06ff45017f"));
            antibiotics.DisplayOrder = 1;
            BDNode treatmentRecommendations = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("f92fec3a-379d-41ef-a981-5ddf9c9a9f0e"));
            treatmentRecommendations.DisplayOrder = 2;
            BDNode prophylaxis = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("76e2f45c-c8c5-45e4-a079-65e1a3908cde"));
            prophylaxis.DisplayOrder = 3;
            BDNode dental = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("cddcc760-d8f2-460f-8982-668f98b5404b"));
            dental.DisplayOrder = 4;
            BDNode pregnancyLactation = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("690c044e-72c4-4115-b90e-33c9807dfe50"));
            pregnancyLactation.DisplayOrder = 5;
            BDNode organisms = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("fc322f9f-7204-42a7-9e6a-322a0869e6aa"));
            organisms.DisplayOrder = 6;
            organisms.Name = "Organisms";
            dataContext.SaveChanges();

            BDNode preface = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
            preface.DisplayOrder = 0;
            preface.Name = "Preface";
            preface.LayoutVariant = BDConstants.LayoutVariantType.FrontMatter;
            preface.SetParent(intro);

            BDNode foreward = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
            foreward.SetParent(intro);
            foreward.DisplayOrder = 1;
            foreward.LayoutVariant = BDConstants.LayoutVariantType.FrontMatter;
            foreward.Name = "Foreward";
            dataContext.SaveChanges();
          
            // change parent of BDAttachment in Fungal infections
            BDNode section = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("40d92304-3224-4af0-8371-bcc27edad7dd"));
            BDNode topic = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDTopic, Guid.NewGuid());
            topic.Name = "Management of Adult Patients on Amphotericin B";
            topic.DisplayOrder = 12;
            topic.LayoutVariant = BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal;
            topic.SetParent(section);

            BDAttachment att1 = BDAttachment.RetrieveWithId(dataContext, Guid.Parse("a9f389cf-f134-4513-bba4-9896c5e17356"));
            att1.SetParent(topic);
            att1.DisplayOrder = 0;

            // for DEBUG only
            //BDAttachment att2 = BDAttachment.RetrieveWithId(dataContext, Guid.Parse("8875a8cf-6b7b-490b-9158-2a7948f21363"));
            //if (att2 != null)
            //{
            //    att2.SetParent(topic);
            //    att2.DisplayOrder = 1;
            //}
            dataContext.SaveChanges();
             */
            #endregion
            #region v.1.6.28
           /* BDNode sAssault = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("a6fdd2d3-96df-47b6-a598-8adbfa474c1b"));
            BDNode bbFluidExposure = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ec9b96ff-8744-4e27-82f6-9608fe22e4b4"));
            sAssault.SetParent(bbFluidExposure);
            sAssault.nodeType = (int)BDConstants.BDNodeType.BDCategory;
            BDNode.Save(dataContext, sAssault);
            */
            #endregion
            #region v.1.6.29 
            /*
            // move Organism groups up in the hierarchy
            BDNode legend = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("d013a04e-a653-4509-9ab3-907effef5ee6"));
            BDNode legendParent = BDNode.RetrieveNodeWithId(dataContext, legend.parentId.Value);
            List<IBDNode> children = BDFabrik.GetChildrenForParent(dataContext, legend);
            foreach (IBDNode child in children)
                child.SetParent(legendParent);
            dataContext.SaveChanges();
            
            BDNode antibioticChapter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("45e13826-aedb-48d0-baf6-2f06ff45017f"));
            BDNode admg = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
            admg.Name = "Aminoglycoside Dosing and Monitoring Guidelines";
            admg.DisplayOrder = 3;
            admg.SetParent(antibioticChapter);
            admg.LayoutVariant = BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring;

            BDNode highDose = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("147410a3-ca09-4b4a-af50-c5f8719f1729"));
            highDose.nodeType = (int)BDConstants.BDNodeType.BDSubsection;
            highDose.nodeKeyName = BDUtilities.GetEnumDescription(BDConstants.BDNodeType.BDSubsection);
            highDose.SetParent(BDConstants.BDNodeType.BDSection, admg.Uuid);
            highDose.DisplayOrder = 0;

            BDNode conventionalDose = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("ca7eefb1-81f8-465e-8818-34a35d92736a"));
            conventionalDose.nodeType = (int)BDConstants.BDNodeType.BDSubsection;
            conventionalDose.nodeKeyName = BDUtilities.GetEnumDescription(BDConstants.BDNodeType.BDSubsection);
            conventionalDose.SetParent(BDConstants.BDNodeType.BDSection, admg.Uuid);
            conventionalDose.DisplayOrder = 1;
            dataContext.SaveChanges();

            BDNode newParent = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDCategory, Guid.NewGuid());
            newParent.Name = "Monitoring";
            newParent.SetParent(conventionalDose);
            newParent.LayoutVariant = conventionalDose.LayoutVariant;

            BDNode monitoring = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("54aee8b1-9c6f-4f08-9491-622347f222fc"));
            monitoring.SetParent(newParent);
            BDNode serum = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("785e754c-6fb6-43ca-9cbd-b871ba2dc8ba"));
            serum.SetParent(newParent);
            BDNode frequency = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("9293d8bb-a55e-41fa-a941-7b1376715763"));
            frequency.SetParent(newParent);
            dataContext.SaveChanges();
            */
            #endregion

            #region v.1.6.34
            //BDNode backMatter = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDChapter, Guid.NewGuid());
            //backMatter.Name = "Back Matter";
            //backMatter.DisplayOrder = 7;
            //backMatter.parentId = Guid.Empty;
            //backMatter.parentKeyName = BDUtilities.GetEnumDescription(BDConstants.BDNodeType.None);
            //backMatter.LayoutVariant = BDConstants.LayoutVariantType.BackMatter;

            //BDNode afterword = BDNode.CreateBDNode(dataContext, BDConstants.BDNodeType.BDSection, Guid.NewGuid());
            //afterword.Name = "Afterword";
            //afterword.DisplayOrder = 0;
            //afterword.SetParent(backMatter);
            //afterword.LayoutVariant = BDConstants.LayoutVariantType.BackMatter;

            //BDNode frontMatter = BDNode.RetrieveNodeWithId(dataContext, Guid.Parse("5fe7fc06-5d76-438b-ac5b-0c3c483ea871"));
            //frontMatter.name = "Front Matter";

            //dataContext.SaveChanges();

            #endregion
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {

            Guid nodeUuid = Guid.Empty;
            BDNode node = null;
            BDHtmlPage htmlPage = null;
            BDHtmlPageGenerator generator = new BDHtmlPageGenerator();

            /*
            Debug.WriteLine("");
            nodeUuid = Guid.Parse("62fade9f-603c-4479-817f-737cb28f1d62");
            BDTherapy therapy = BDTherapy.RetrieveTherapyWithId(dataContext, nodeUuid);

            List<BDLinkedNote> footnotes = new List<BDLinkedNote>();
            List<Guid> objectsOnPage = new List<Guid>();

            

            //string htmlString = generator.buildTherapyWithTwoDurationsHtml(dataContext, therapy, footnotes, objectsOnPage);
            //Debug.WriteLine(htmlString);

            //nodeUuid = Guid.Parse("24DEE453-D880-4D8E-B869-CA89DBE13067");
            //BDNode node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            //BDHtmlPage htmlPage = generator.generatePageForEmpiricTherapyOfCultureDirectedEndocarditis(dataContext, node);
            //Debug.WriteLine(htmlPage.documentText);

            Debug.WriteLine("-- A --");
            nodeUuid = Guid.Parse("c77ee0f1-e2f8-4e18-82c5-b4ab0bc917cc");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.generatePageForEmpiricTherapyOfEndocarditisPaediatrics(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");

            Debug.WriteLine(" -- A2 --");
            htmlPage = null;
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");

            Debug.WriteLine("-- B --");
            htmlPage = null;
            nodeUuid = Guid.Parse("f63383e6-af5f-4f15-81a0-8f32cf058d01");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.generatePageForEmpiricTherapyPresentation(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");
            Debug.WriteLine("-- B2 --");
            htmlPage = null;
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");

            Debug.WriteLine("-- C --");
            htmlPage = null;
            nodeUuid = Guid.Parse("68f69d10-4d5f-4717-8815-843fbe6b5ed8");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.generatePageForEmpiricTherapyDisease(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");

            Debug.WriteLine("-- C2 --");
            htmlPage = null;
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");
            */
            /*
            Debug.WriteLine("-- D --");
            nodeUuid = Guid.Parse("4b476e8b-fa56-4d1a-b0c8-5fb29f820b6f");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");

            Debug.WriteLine("-- E --");
            nodeUuid = Guid.Parse("2a52c687-4fc6-4df8-bcc8-cdf38e678c52");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");

            Debug.WriteLine("-- F --");
            nodeUuid = Guid.Parse("3a5c4685-aa62-4235-a545-c919815a75e0");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");

            Debug.WriteLine("-- G --");
            nodeUuid = Guid.Parse("d2b46ef0-16a8-4217-b5c3-ca80531ab8b5");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");
            
            Debug.WriteLine("-- H --");
            nodeUuid = Guid.Parse("35879474-3d20-457d-9094-32cc46b838fc");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");
*/
            Debug.WriteLine("-- I2 --");
            nodeUuid = Guid.Parse("2569b75a-92cc-4a3a-9317-9125361f4785");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");

            Debug.WriteLine("-- Complete --");
           
        }

    }
}
