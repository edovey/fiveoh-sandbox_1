using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.Classes;
using BDEditor.DataModel;

namespace BDEditor.Classes.Navigation
{
    public class BDAntibioticsTree
    {
        private BDAntibioticsTree() { }

        public static TreeNode BuildBranch(Entities pDataContext, IBDNode pNode)
        {
            if (null == pNode) return null;

            TreeNode branchTreeNode = new TreeNode(pNode.Name);
            List<IBDNode> childList = BDFabrik.GetChildrenForParent(pDataContext, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDChapter:
                case BDConstants.BDNodeType.BDSection:
                case BDConstants.BDNodeType.BDSubsection:
                case BDConstants.BDNodeType.BDTopic:
                case BDConstants.BDNodeType.BDCategory:
                case BDConstants.BDNodeType.BDSubcategory:
                case BDConstants.BDNodeType.BDDisease:
                case BDConstants.BDNodeType.BDAntimicrobial:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.Antibiotics:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines:
                        case BDConstants.LayoutVariantType.Antibiotics_ClinicalGuidelines_Spectrum:
                        case BDConstants.LayoutVariantType.Antibiotics_Pharmacodynamics:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Adult:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndCosts_Paediatric:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Conventional:
                        case BDConstants.LayoutVariantType.Antibiotics_DosingAndMonitoring_Vancomycin:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_RenalImpairment:
                        case BDConstants.LayoutVariantType.Antibiotics_Dosing_HepaticImpairment:
                        case BDConstants.LayoutVariantType.Antibiotics_NameListing:
                        case BDConstants.LayoutVariantType.Antibiotics_Stepdown:
                        case BDConstants.LayoutVariantType.Antibiotics_CSFPenetration:
                        case BDConstants.LayoutVariantType.Antibiotics_BLactamAllergy:
                            foreach (IBDNode childNode in childList)
                            {
                                string name = childNode.Name;
                                if ((null == name) || (name.Length == 0))
                                {
                                    name = @"< intentionally blank >";
                                }
                                TreeNode childTreeNode = new TreeNode(name);
                                childTreeNode.Tag = childNode;
                                branchTreeNode.Nodes.Add(childTreeNode);
                            }
                            break;
                    }
                    break;
            }

            return branchTreeNode;
        }
    }

}
