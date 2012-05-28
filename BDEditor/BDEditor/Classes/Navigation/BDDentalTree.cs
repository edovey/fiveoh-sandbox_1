using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.Classes;
using BDEditor.DataModel;

namespace BDEditor.Classes.Navigation
{
    public class BDDentalTree
    {
        private BDDentalTree() { }

        public static TreeNode BuildBranch(Entities pDataContext, IBDNode pNode)
        {
            if (null == pNode) return null;

            TreeNode branchTreeNode = new TreeNode(pNode.Name);
            List<IBDNode> childList = BDFabrik.GetChildrenForParent(pDataContext, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDChapter:
                case BDConstants.BDNodeType.BDSection:
                case BDConstants.BDNodeType.BDCategory:
                case BDConstants.BDNodeType.BDSubcategory:
                case BDConstants.BDNodeType.BDDisease:
                case BDConstants.BDNodeType.BDTable:
                     switch (pNode.LayoutVariant)
                    {
                         case BDConstants.LayoutVariantType.Dental:
                         case BDConstants.LayoutVariantType.Dental_Prophylaxis:
                         case BDConstants.LayoutVariantType.Dental_Prophylaxis_DrugRegimens:
                         case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis:
                         case BDConstants.LayoutVariantType.Dental_Prophylaxis_Endocarditis_AntibioticRegimen:
                         case BDConstants.LayoutVariantType.Dental_Prophylaxis_Prosthetics:
                         case BDConstants.LayoutVariantType.Dental_RecommendedTherapy:
                         case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_AntimicrobialActivity:
                         case BDConstants.LayoutVariantType.Dental_RecommendedTherapy_Microorganisms:
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
