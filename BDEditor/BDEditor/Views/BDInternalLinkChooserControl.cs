using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.Classes;
using BDEditor.Classes.Navigation;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDInternalLinkChooserControl : UserControl
    {
        private bool isLoading = false;

        public Entities DataContext;
        public IBDNode SelectedIBDNode;

        public BDInternalLinkChooserControl()
        {
            InitializeComponent();
        }  
 
        
        private void BDInternalLinkChooserControl_Load(object sender, EventArgs e)
        {
        }

        public void Setup(Entities pDataContext, IBDNode pSelectedNode)
        {
            DataContext = pDataContext;
            SelectedIBDNode = pSelectedNode;
            loadTopLevelChooser();
        }

        private void loadTopLevelChooser()
        {
            cboTopLevel.Items.Clear();
            List<IBDNode> list = BDFabrik.GetAllForNodeType(DataContext, BDConstants.BDNodeType.BDChapter);
            cboTopLevel.Items.AddRange(list.ToArray());

            if (cboTopLevel.Items.Count >= 1)
                cboTopLevel.SelectedIndex = 0;
        }

        private void cboTopLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            IBDNode listEntry = cboTopLevel.SelectedItem as IBDNode;

            displayTopLevelTree(listEntry);
        }

        private void displayTopLevelTree(IBDNode pNode)
        {
            Cursor _cursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            tree.Nodes.Clear();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDChapter))
            {
                TreeNode node = buildChildBranch(DataContext, pNode);
                
                if (null != node)
                {
                    TreeNode[] nodeList = new TreeNode[node.Nodes.Count];
                    node.Nodes.CopyTo(nodeList, 0);
                    tree.Nodes.AddRange(nodeList);
                }
            }

            this.Cursor = _cursor;
        }

        private TreeNode buildChildBranch(Entities pDataContext, IBDNode pNode)
        {
            if (null == pNode) return null;

            TreeNode branchTreeNode = new TreeNode(pNode.Name);
            List<IBDNode> childList = BDFabrik.GetChildrenForParent(pDataContext, pNode);

            foreach (IBDNode childNode in childList)
            {
                string name = childNode.Name;
                if ((null == name) || (name.Length == 0))
                {
                    //name = childNode.Uuid.ToString();
                    name = @"< intentionally blank >";
                }
                TreeNode childTreeNode = new TreeNode(name);
                childTreeNode.Tag = childNode;
                branchTreeNode.Nodes.Add(childTreeNode);
            }

            return branchTreeNode;
        }
        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
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

        private void graftTreeNode(TreeNode pTree, TreeNode pBranch)
        {
            if ((null != pTree) && (null != pBranch))
            {
                TreeNode[] nodeList = new TreeNode[pBranch.Nodes.Count];
                pBranch.Nodes.CopyTo(nodeList, 0);
                pTree.Nodes.Clear();
                pTree.Nodes.AddRange(nodeList);
                pTree.Expand();
            }
        }

        private TreeNode showNavSelection(TreeNode pSelectedNode, Boolean pInterrogateOnly)
        {
            TreeNode childTreeNode = null;

            TreeNode selectedNode = pSelectedNode;
            if (null == selectedNode)
            {
                this.Cursor = Cursors.WaitCursor;

                IBDNode listEntry = cboTopLevel.SelectedItem as IBDNode;
                displayTopLevelTree(listEntry);

                SelectedIBDNode = null;

                this.Cursor = Cursors.Default;
            }
            else
            {
                IBDNode node = selectedNode.Tag as IBDNode;

                SelectedIBDNode = node;

                bool showChildControls = true;

                this.Cursor = Cursors.WaitCursor;

                switch (node.NodeType)
                {
                    case BDConstants.BDNodeType.BDSection:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.PregnancyLactation_Perinatal_HIVProtocol:
                                if (!pInterrogateOnly)
                                    showChildControls = true;
                                break;
                            //case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                            //    if (!pInterrogateOnly)
                            //        showChildControls = true;
                            //    break;
                            default:
                                if (!pInterrogateOnly)
                                {
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
                            case BDConstants.LayoutVariantType.Organisms_Therapy:
                            case BDConstants.LayoutVariantType.Prophylaxis_IE:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Prevention_PerinatalInfection:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                            default:
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
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = false;
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
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = false;
                                }
                                break;
                        }
                        break;
                    case BDConstants.BDNodeType.BDPresentation:
                        switch (node.LayoutVariant)
                        {
                            case BDConstants.LayoutVariantType.TreatmentRecommendation13_VesicularLesions:
                                if (!pInterrogateOnly)
                                {
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
                            case BDConstants.LayoutVariantType.Prophylaxis_Surgical:
                            case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = false;
                                }
                                break;

                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                            case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                            case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            case BDConstants.LayoutVariantType.Organisms_GramStainInterpretation:
                            case BDConstants.LayoutVariantType.Organisms_CommensalAndPathogenic:
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
                            case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                                if (!pInterrogateOnly)
                                {
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
                                if (!pInterrogateOnly)
                                {
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
                            case BDConstants.LayoutVariantType.TreatmentRecommendation05_CultureProvenPeritonitis:
                            default:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = false;
                                }
                                break;
                            //default:
                            //    if (!pInterrogateOnly)
                            //    {
                            //        showChildControls = true;
                            //    }
                            //    break;

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
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = true;
                                }
                                break;
                            case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                            case BDConstants.LayoutVariantType.Prophylaxis_SexualAssault_Prophylaxis:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_Routine:
                            case BDConstants.LayoutVariantType.Prophylaxis_Immunization_HighRisk:
                                if (!pInterrogateOnly)
                                {
                                    showChildControls = false;
                                }
                                break;
                        }
                        break;
                }

                if (!showChildControls)
                {
                    childTreeNode = buildChildBranch(DataContext, node);
                    graftTreeNode(selectedNode, childTreeNode);
                }
                this.Cursor = Cursors.Default;
            }
            return childTreeNode;
        }


    }



}
