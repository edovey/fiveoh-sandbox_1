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
        private BDDataLoader.baseDataDefinitionType seedDataType = BDDataLoader.baseDataDefinitionType.none;

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
                            item.Text = string.Format("&Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[idx]));
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

                    foreach (Control ctrl in splitContainer1.Panel2.Controls)
                    {
                        IBDControl nodeCtrl = ctrl as IBDControl;
                        if (null != nodeCtrl)
                        {
                            nodeCtrl.NameChanged -= new EventHandler<NodeEventArgs>(nodeControl_NameChanged);
                            nodeCtrl.RequestItemAdd -= new EventHandler<NodeEventArgs>(siblingNodeCreateRequest);
                        }
                    }

                    splitContainer1.Panel2.Controls.Clear();
                }

                switch (node.NodeType)
                {
                    case BDConstants.BDNodeType.BDSection:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation12_Endocarditis_BCNE:
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
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
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
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
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
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
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
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
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
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                                childTreeNode = BDAntibioticsTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
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
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
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
                                if(!pInterrogateOnly)
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

                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            case BDConstants.LayoutVariantType.Microbiology_GramStainInterpretation:
                            case BDConstants.LayoutVariantType.Microbiology_CommensalAndPathogenicOrganisms:
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
                            case BDConstants.LayoutVariantType.TreatmentRecommendation11_GenitalUlcers:
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
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_VaccineDetail:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
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
                        }
                        break;
                }
                if (!pInterrogateOnly)
                {
                    control_tr01 = BDFabrik.CreateControlForNode(dataContext, node);
                    if (null != control_tr01)
                    {
                        control_tr01.AssignScopeId((null != node) ? node.Uuid : Guid.Empty);
                        control_tr01.AssignParentInfo(node.ParentId, node.ParentType);
                        ((System.Windows.Forms.UserControl)control_tr01).Dock = DockStyle.Fill;
                        control_tr01.NameChanged += new EventHandler<NodeEventArgs>(nodeControl_NameChanged);
                        control_tr01.RequestItemAdd += new EventHandler<NodeEventArgs>(siblingNodeCreateRequest);
                        splitContainer1.Panel2.Controls.Add((System.Windows.Forms.UserControl)control_tr01);
                        control_tr01.RefreshLayout(showChildControls);
                    }

                    ControlHelper.ResumeDrawing(splitContainer1.Panel2);
                    splitContainer1.Panel2.ResumeLayout();
                }
                this.Cursor = Cursors.Default;
            }
            return childTreeNode;
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

            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1a.txt", BDDataLoader.baseDataDefinitionType.chapter1a);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_1b.txt", BDDataLoader.baseDataDefinitionType.chapter1b);
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
            dataLoader.ImportData(dataContext, @"Resources\Chapter_3j.txt", BDDataLoader.baseDataDefinitionType.chapter3j);
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

        [Obsolete("Use Archive/Restore instead", false)]
        private void SyncData()
        {
            this.Cursor = Cursors.WaitCursor;
            BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            SyncInfoDictionary syncResultList = RepositoryHandler.Aws.Sync(DataContext, lastSyncDate, BDConstants.SyncType.Default);

            string resultMessage = string.Empty;

            foreach (SyncInfo syncInfo in syncResultList.Values)
            {
                System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                if( (syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0) )
                    resultMessage = string.Format("{0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage)? "": "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
            }

            if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

            MessageBox.Show(resultMessage, "Synchronization");

            UpdateSyncLabel();
            LoadChapterDropDown();

            systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            lastSyncDate = systemSetting.settingDateTimeValue;
            loadSeedDataButton.Visible = isSeedDataLoadAvailable;

            this.Cursor = Cursors.Default;
        }

        private void BDEditView_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}" , "Bugs & Drugs Editor", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            // Loading Seed Data:  set the following variables
            isSeedDataLoadAvailable = true;

#if DEBUG
            this.Text = this.Text + @" < DEVELOPMENT >";
            this.btnImportFromProduction.Visible = true;
            this.btnPublish.Visible = true;
            this.btnMove.Visible = !isSeedDataLoadAvailable && false;
            this.btnMove.Enabled = !isSeedDataLoadAvailable && false;
#else
            this.btnImportFromProduction.Visible = false;
            this.btnPublish.Visible = false;

            this.btnMove.Visible = false;
            this.btnMove.Enabled = false;
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
            btnPublish.Enabled = BDCommon.Settings.SyncPushEnabled;
            btnImportFromProduction.Enabled = BDCommon.Settings.SyncPushEnabled;
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
                MessageBox.Show("Archive complete", "Achive to Repository", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnSyncWithReplaceLocal_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will DELETE all local data and replace it from the repository?", "Replace Local Data", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;

                chapterDropDown.Items.Clear();
                chapterDropDown.SelectedIndex = -1;

                RepositoryHandler.Aws.DeleteLocalData(dataContext);
                
                dataContext = null;
                dataContext = new Entities();

                BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                systemSetting.settingDateTimeValue = null;
                dataContext.SaveChanges();
                this.Cursor = Cursors.Default;

                SyncData();          
            }
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

        private void btnPublish_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            BDHtmlPageGenerator generator = new BDHtmlPageGenerator();
            generator.Generate();
            System.Diagnostics.Debug.WriteLine("HTML page generation complete.");
            BDSearchEntryGenerator.Generate();
            System.Diagnostics.Debug.WriteLine("Search entry generation complete.");

            BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            SyncInfoDictionary syncResultList = RepositoryHandler.Aws.Sync(DataContext, lastSyncDate, BDConstants.SyncType.Publish);

            string resultMessage = string.Empty;

            foreach (SyncInfo syncInfo in syncResultList.Values)
            {
                System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                if ((syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0))
                    resultMessage = string.Format("{0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage) ? "" : "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
            }

            if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

            MessageBox.Show(resultMessage, "Synchronization");
            this.Cursor = Cursors.Default;
        }

        private void btnImportFromProduction_Click(object sender, EventArgs e)
        {

#if DEBUG
            if (MessageBox.Show("Have the development respository domains (bd_dev_2*) and bdDevStore bucket been cleared?", "Import from Production", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;
                BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                DateTime? lastSyncDate = null;

                SyncInfoDictionary syncResultList = RepositoryHandler.Aws.ImportFromProduction(dataContext, null);

                string resultMessage = string.Empty;

                foreach (SyncInfo syncInfo in syncResultList.Values)
                {
                    System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                    if ((syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0))
                        resultMessage = string.Format("Production Import {0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage) ? "" : "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
                }

                if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

                MessageBox.Show(resultMessage, "Synchronization");

                UpdateSyncLabel();
                LoadChapterDropDown();

                systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                lastSyncDate = systemSetting.settingDateTimeValue;
                loadSeedDataButton.Visible = isSeedDataLoadAvailable;

                this.Cursor = Cursors.Default;
            }
#else
            MessageBox.Show(@"May not import in this environment" , "Import");
#endif
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = chapterTree.SelectedNode;

            // search for a term:  open the search window
            BDSearchView searchView = new BDSearchView();
            searchView.AssignDataContext(dataContext);
            searchView.ShowDialog(this);

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

                siblingList.Remove(sourceNode);
                int targetIdx = (targetNode.Uuid == sourceNode.ParentId) ? 0 : siblingList.IndexOf(targetNode) + 1;
                siblingList.Insert(targetIdx, sourceNode);

                for (int idx = 0; idx < siblingList.Count; idx++)
                {
                    siblingList[idx].DisplayOrder = idx;
                    BDFabrik.SaveNode(this.DataContext, siblingList[idx]);
                }

                // what if parent is null?
                if (null != parentTreeNode)
                {
                    parentTreeNode.Nodes.Remove(sourceTreeNode);
                    parentTreeNode.Nodes.Insert(targetIdx, sourceTreeNode);
                    Debug.WriteLine("---");
                }
            }
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

        private void btnMove_Click(object sender, EventArgs e)
        {
            // These operations are CUSTOM, ** BY REQUEST ONLY **
           
            // the following are complete, but left as samples.
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
            //BDUtilities.ResetLayoutVariantWithChildren(dataContext, startNode, BDConstants.LayoutVariantType.TreatmentRecommendation15_Pneumonia, true);
        }

        private void btnShowLayoutEditor_Click(object sender, EventArgs e)
        {
            BDLayoutMetadataEditor editor = new BDLayoutMetadataEditor(DataContext);
            Application.DoEvents();
            editor.ShowDialog();

        }


    }
}
