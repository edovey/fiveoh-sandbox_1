using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        private bool auditButtonVisible = true;

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
                    if ((null != childTreeNode) && (chapterTree.SelectedNode.Nodes.Count != childTreeNode.Nodes.Count))
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
                    case BDConstants.LayoutVariantType.References:
                        node = BDReferencesTree.BuildBranch(dataContext, pNode);
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
                    case BDConstants.LayoutVariantType.Organisms:
                        node = BDOrganismsTree.BuildBranch(dataContext, pNode);
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

            if (pNode.NodeType == BDConstants.BDNodeType.BDChapter)
            {
                IBDControl control_tr01 = BDFabrik.CreateControlForNode(dataContext, pNode);
                if (null != control_tr01)
                {
                    ((System.Windows.Forms.UserControl)control_tr01).Validated += new EventHandler(BDEditView_Validated);
                    control_tr01.ShowChildren = false;
                    control_tr01.AssignScopeId((null != pNode) ? pNode.Uuid : Guid.Empty);
                    control_tr01.AssignParentInfo(pNode.ParentId, pNode.ParentType);
                    ((System.Windows.Forms.UserControl)control_tr01).Dock = DockStyle.Fill;
                    control_tr01.NameChanged += new EventHandler<NodeEventArgs>(nodeControl_NameChanged);
                    control_tr01.RequestItemAdd += new EventHandler<NodeEventArgs>(siblingNodeCreateRequest);
                    splitContainer1.Panel2.Controls.Add((System.Windows.Forms.UserControl)control_tr01);
                    control_tr01.RefreshLayout(false);
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
                                layoutItem.Text = string.Format("Add {0} [{1}]", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1), BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[idy]));
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
                                childTreeNode = BDFrontMatterTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.References:
                                childTreeNode = BDReferencesTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
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
                            case BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions:
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            case BDConstants.LayoutVariantType.Prophylaxis_IE:
                            case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
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

                            case BDConstants.LayoutVariantType.Organisms:
                            case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                            case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                            case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                            case BDConstants.LayoutVariantType.Organisms_Antibiogram:
                                childTreeNode = BDOrganismsTree.BuildBranch(dataContext, node);
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
                            case BDConstants.LayoutVariantType.Prophylaxis_IE:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_I:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic_II:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal_Amphotericin_B:
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
                            case BDConstants.LayoutVariantType.Prophylaxis_IE_AntibioticRegimen:
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
                            case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
                            case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                                childTreeNode = BDOrganismsTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Organisms_EmpiricTherapy:
                                childTreeNode = BDOrganismsTree.BuildBranch(dataContext, node);
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
                            case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
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
                        switch (node.LayoutVariant)
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
                            case BDConstants.LayoutVariantType.TreatmentRecommendation07_CultureProvenEndocarditis_SingleDuration:
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
                            case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopic:
                            case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
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
                            case BDConstants.LayoutVariantType.TreatmentRecommendation01_Sepsis_Without_Focus_WithRisk:
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
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
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
                            //case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                                childTreeNode = BDAntibioticsTree.BuildBranch(dataContext, node);
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
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSurgeryClassification:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgery_With_Classification:
                                //childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    //graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = true;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDSurgeryGroup:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Surgeries_With_Classification:
                                childTreeNode = BDProphylaxisTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
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
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_PreOp:
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical_Intraoperative:
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
                            case BDConstants.LayoutVariantType.TreatmentRecommendation20_Adult_WithTopicAndSubtopic:
                                childTreeNode = BDTreatmentRecommendationTree.BuildBranch(dataContext, node);
                                if (!pInterrogateOnly)
                                {
                                    graftTreeNode(selectedNode, childTreeNode);
                                    showChildControls = false;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault:
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
            if (null != siblingNode)
            {
                IBDNode parentNode = BDFabrik.RetrieveNode(e.DataContext, siblingNode.ParentType, siblingNode.ParentId);
                BDFabrik.CreateChildNode(DataContext, parentNode, e.NodeType, e.LayoutVariant);
                showNavSelection(chapterTree.SelectedNode, false);
            }
        }

        private void LoadChapterDropDown()
        {
            splitContainer1.Panel2.Controls.Clear();

            chapterTree.Nodes.Clear();

            chapterDropDown.Items.Clear();
            foreach (IBDNode entry in BDFabrik.GetAllForNodeType(dataContext, BDConstants.BDNodeType.BDChapter))
            {
                chapterDropDown.Items.Add(entry);
            }

            if (chapterDropDown.Items.Count >= 1)
                chapterDropDown.SelectedIndex = 0;

            // splitContainer1.Panel2
        }

        private void UpdateSyncLabel()
        {
            BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.ARCHIVE_TIMESTAMP);
            string controlNumberString = BDSystemSetting.RetrieveSettingValue(dataContext, BDSystemSetting.CONTROL_NUMBER);
            DateTime? archiveDate = systemSetting.settingDateTimeValue;

            if (null == archiveDate) // Determine if it is the legacy 'lastSync' entry
            {
                BDSystemSetting lastSyncEntry = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                archiveDate = lastSyncEntry.settingDateTimeValue;
                if (null == archiveDate)
                {
                    lbLastSyncDateTime.Text = @"<Not Archived>";
                }
                else
                {
                    lbLastSyncDateTime.Text = string.Format("{0} [{1}]", archiveDate.Value.ToString(BDConstants.DATETIMEFORMAT), "00000.000");
                }
            }
            else
            {
                lbLastSyncDateTime.Text = string.Format("{0} [{1}]", archiveDate.Value.ToString(BDConstants.DATETIMEFORMAT), controlNumberString);
                BDSystemSetting lastSyncEntry = BDSystemSetting.RetrieveRawSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                if (null != lastSyncEntry)
                {
                    dataContext.DeleteObject(lastSyncEntry);
                    dataContext.SaveChanges();
                }
            }
        }

        private void BDEditView_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}", "Bugs & Drugs Editor", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

#if DEBUG
            this.Text = this.Text + @" < DEVELOPMENT >";
            this.btnPublish.Visible = true;
            this.btnAudit.Visible = auditButtonVisible;
            this.btnAudit.Enabled = auditButtonVisible;
#else
            this.btnPublish.Visible = false;

            this.btnAudit.Visible = false;
            this.btnAudit.Enabled = false;

            this.btnDebug.Visible = false;
            this.btnDebug.Enabled = false;
#endif

            BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;
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
            authenticationForm.Dispose();
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
                try
                {
                    RepositoryControlNumber controlNumber = RepositoryHandler.Aws.Archive(dataContext, archiveDialog.Username, archiveDialog.Comment, false, RepositoryHandler.RepositoryHandlerPublishType.RepositoryHandlerPublishType_CompilerDirective);
                    string message = @"Archive to Repository";
                    if (null != controlNumber)
                        message = string.Format("{0}{1}Control Number:{2}", message, Environment.NewLine, controlNumber.ControlNumberText);
                    MessageBox.Show(message, "Archive complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Archive Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                    MessageBox.Show("Please restart the application", "Data Restore", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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

            BDBuildView build = new BDBuildView(dataContext);
            DialogResult result = build.ShowDialog(this);
            if (result == DialogResult.OK)
                MessageBox.Show(this, "Close the application before publishing again.", "Publish Is Complete", MessageBoxButtons.OK);
            UpdateSyncLabel();
        }

        private void btnIndexManager_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            BDSearchEntryManagerView mgrView = new BDSearchEntryManagerView();
            mgrView.AssignDataContext(dataContext);
            mgrView.ShowDialog(this);
            this.Cursor = Cursors.Default;
            mgrView.Dispose();
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
                Debug.WriteLine("sibling count is: {0}", siblingList.Count());

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
                        validTarget = (((sourceNode.Uuid != targetNode.Uuid) && (sourceNode.ParentId == targetNode.ParentId)) || (targetNode.Uuid == sourceNode.ParentId));
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

        private void btnAudit_Click(object sender, EventArgs e)
        {
            BDAuditReport.ReadAuditLog(dataContext);
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            Guid nodeUuid = Guid.Empty;
            BDNode node = null;
            BDHtmlPage htmlPage = null;
            BDHtmlPageGenerator generator = new BDHtmlPageGenerator();

            /*
            Debug.WriteLine("-- A --");
            Debug.Indent();
            nodeUuid = Guid.Parse("24dee453-d880-4d8e-b869-ca89dbe13067");
            node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            Debug.WriteLine(htmlPage.documentText);
            Debug.WriteLine("");
            Debug.WriteLine("HtmlPage Uuid= {0}", htmlPage.Uuid);
            Debug.WriteLine("");
            Debug.Unindent();

            //Debug.WriteLine("-- B --");
            //Debug.Indent();
            //nodeUuid = Guid.Parse("376b287e-1d80-40f5-bb0b-512e52720687");
            //node = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
            //htmlPage = generator.GenerateBDHtmlPage(dataContext, node);
            //Debug.WriteLine(htmlPage.documentText);
            //Debug.WriteLine("");
            //Debug.WriteLine("HtmlPage Uuid= {0}", htmlPage.Uuid);
            //Debug.WriteLine("");
            //Debug.Unindent();
            //Debug.WriteLine("-- Complete --");

                        Debug.WriteLine("-- DEBUG FORENSICS --");
                        List<IBDNode> fList = new List<IBDNode>(); 
                        string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        using (StreamReader sr = File.OpenText(Path.Combine(mydocpath,"NodeUuidList.txt")))
                        {
                            String input;

                
                            while ((input = sr.ReadLine()) != null)
                            {
                                IBDNode fNode = BDFabrik.RetrieveNode(dataContext, Guid.Parse(input));
                                fList.Add(fNode);
                            }

                        }
             */

            /// this section generates to HTML page in database, used for hierarchy of pages
            // simulates the publish operation for the nodes specified.
            Debug.WriteLine("-- DEBUG GENERATION --");
            List<BDNode> nodeList = new List<BDNode>();
            List<Guid> guidList = new List<Guid>();
            guidList.Add(Guid.Parse("0264e99b-20ad-4abf-b349-a8202582326e"));
            guidList.Add(Guid.Parse("9fe2fc95-d340-4586-9f1b-b7a6adcfb95b"));
            guidList.Add(Guid.Parse("17905052-f5e7-475e-987e-fc4dbe80bfbd"));
            //guidList.Add(Guid.Parse("12c6c370-b63b-4b3c-9dc1-5cb9fa988918"));
            //guidList.Add(Guid.Parse("c0cf6533-c4c9-480b-ad85-c7d187672849"));
            foreach (Guid guid in guidList)
            {
                nodeList.Add(BDNode.RetrieveNodeWithId(dataContext, guid));
            }

            generator.Generate(dataContext, nodeList);

            Debug.WriteLine("-- Complete --");
        }

    }
}
