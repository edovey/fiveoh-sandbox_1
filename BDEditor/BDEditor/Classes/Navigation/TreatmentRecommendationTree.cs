﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.Classes;
using BDEditor.DataModel;

namespace BDEditor.Classes.Navigation
{
    public class TreatmentRecommendationTree
    {
        private TreatmentRecommendationTree() { }

        public static TreeNode BuildBranch(Entities pDataContext, IBDNode pNode)
        {
            if(null == pNode) return null;

            TreeNode branchTreeNode = new TreeNode(pNode.Name);
            List<IBDNode> childList = BDFabrik.GetChildrenForParent(pDataContext, pNode);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDChapter:
                case BDConstants.BDNodeType.BDSection:
                case BDConstants.BDNodeType.BDCategory:
                case BDConstants.BDNodeType.BDDisease:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            foreach (IBDNode childNode in childList)
                            {
                                string name = childNode.Name;
                                if ((null == name) || (name.Length == 0))
                                {
                                    name = childNode.Uuid.ToString();
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
