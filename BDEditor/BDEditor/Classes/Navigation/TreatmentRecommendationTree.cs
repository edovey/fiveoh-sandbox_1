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

        public static TreeNode BuildChapterTreeNode(BDNode pChapterNode)
        {
            TreeNode resultNode = new TreeNode();

            if ((null != pChapterNode) && (pChapterNode.NodeType == Constants.BDObjectType.BDChapter))
            {

            }
            return resultNode;
        }
    }
}
