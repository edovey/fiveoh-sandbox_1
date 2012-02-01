using System;
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

        public static TreeNode BuildChapterTreeNode(Entities pDataContext, IBDNode pChapterNode)
        {
            TreeNode chapterTreeNode = new TreeNode();

            if ((null != pChapterNode) && (pChapterNode.NodeType == BDConstants.BDNodeType.BDChapter))
            {
                chapterTreeNode.Name = pChapterNode.Name;

                List<IBDNode> sectionList = BDFabrik.GetChildrenForParentId(pDataContext, pChapterNode.Uuid);
                foreach (IBDNode sectionNode in sectionList)
                {
                    switch (sectionNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            TreeNode treeNode = BuildSectionLayout01TreeNode(pDataContext, sectionNode);
                            if (null != treeNode) chapterTreeNode.Nodes.Add(treeNode);
                            break;
                    }

                }
            }
            return chapterTreeNode;
        }

        public static TreeNode BuildSectionLayout01TreeNode(Entities pDataContext, IBDNode pSectionNode)
        {
            TreeNode sectionTreeNode = null;

            if ((pSectionNode.NodeType == BDConstants.BDNodeType.BDSection) && (pSectionNode.LayoutVariant == BDConstants.LayoutVariantType.TreatmentRecommendation01))
            {
                sectionTreeNode = new TreeNode(pSectionNode.Name);
                sectionTreeNode.Tag = pSectionNode;

                List<IBDNode> categoryList = BDFabrik.GetChildrenForParentId(pDataContext, pSectionNode.Uuid);
                foreach (IBDNode categoryNode in categoryList)
                {
                    if (categoryNode.NodeType == BDConstants.BDNodeType.BDCategory)
                    {
                        TreeNode categoryTreeNode = new TreeNode(categoryNode.Name);
                        categoryTreeNode.Tag = categoryNode;
                        sectionTreeNode.Nodes.Add(categoryTreeNode);

                        List<IBDNode> diseaseList = BDFabrik.GetChildrenForParentId(pDataContext, categoryNode.Uuid);
                        foreach (IBDNode diseaseNode in diseaseList)
                        {
                            if (diseaseNode.NodeType == BDConstants.BDNodeType.BDDisease)
                            {
                                TreeNode diseaseTreeNode = new TreeNode(diseaseNode.Name);
                                diseaseTreeNode.Tag = diseaseNode;
                                categoryTreeNode.Nodes.Add(diseaseTreeNode);

                                List<IBDNode> presentationList = BDFabrik.GetChildrenForParentId(pDataContext, diseaseNode.Uuid);
                                foreach (IBDNode presentationNode in presentationList)
                                {
                                    if (presentationNode.NodeType == BDConstants.BDNodeType.BDPresentation)
                                    {
                                        TreeNode presentationTreeNode = new TreeNode(presentationNode.Name);
                                        presentationTreeNode.Tag = presentationNode;
                                        diseaseTreeNode.Nodes.Add(presentationTreeNode);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return sectionTreeNode;
        }

        public static TreeNode BuildBranch(Entities pDataContext, IBDNode pNode)
        {
            if(null == pNode) return null;

            TreeNode branchTreeNode = new TreeNode(pNode.Name);
            List<IBDNode> childList = BDFabrik.GetChildrenForParentId(pDataContext, pNode.Uuid);

            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDChapter:
                case BDConstants.BDNodeType.BDSection:
                case BDConstants.BDNodeType.BDCategory:
                case BDConstants.BDNodeType.BDSubCategory:
                case BDConstants.BDNodeType.BDDisease:
                    switch (pNode.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            foreach (IBDNode childNode in childList)
                            {
                                TreeNode childTreeNode = new TreeNode(childNode.Name);
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
