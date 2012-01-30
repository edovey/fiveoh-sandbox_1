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

            if ((null != pChapterNode) && (pChapterNode.NodeType == Constants.BDNodeType.BDChapter))
            {
                chapterTreeNode.Name = pChapterNode.Name;

                List<IBDNode> sectionList = BDFabrik.GetChildrenForParentId(pDataContext, pChapterNode.Uuid);
                foreach (IBDNode sectionNode in sectionList)
                {
                    switch (sectionNode.LayoutVariant)
                    {
                        case Constants.LayoutVariantType.TreatmentRecommendation01:
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

            if ((pSectionNode.NodeType == Constants.BDNodeType.BDSection) && (pSectionNode.LayoutVariant == Constants.LayoutVariantType.TreatmentRecommendation01))
            {
                sectionTreeNode = new TreeNode(pSectionNode.Name);
                sectionTreeNode.Tag = pSectionNode;

                List<IBDNode> categoryList = BDFabrik.GetChildrenForParentId(pDataContext, pSectionNode.Uuid);
                foreach (IBDNode categoryNode in categoryList)
                {
                    if (categoryNode.NodeType == Constants.BDNodeType.BDCategory)
                    {
                        TreeNode categoryTreeNode = new TreeNode(categoryNode.Name);
                        categoryTreeNode.Tag = categoryNode;
                        sectionTreeNode.Nodes.Add(categoryTreeNode);

                        List<IBDNode> diseaseList = BDFabrik.GetChildrenForParentId(pDataContext, categoryNode.Uuid);
                        foreach (IBDNode diseaseNode in diseaseList)
                        {
                            if (diseaseNode.NodeType == Constants.BDNodeType.BDDisease)
                            {
                                TreeNode diseaseTreeNode = new TreeNode(diseaseNode.Name);
                                diseaseTreeNode.Tag = diseaseNode;
                                categoryTreeNode.Nodes.Add(diseaseTreeNode);

                                List<IBDNode> presentationList = BDFabrik.GetChildrenForParentId(pDataContext, diseaseNode.Uuid);
                                foreach (IBDNode presentationNode in presentationList)
                                {
                                    if (presentationNode.NodeType == Constants.BDNodeType.BDPresentation)
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
    }

}
