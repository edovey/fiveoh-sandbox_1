using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDEditor.DataModel;
using System.Windows.Forms;

namespace BDEditor.Classes
{
    public class BDNodeWrapper
    {
        public IBDNode Node { get; set; }
        public BDConstants.BDNodeType TargetNodeType { get; set; }
        public TreeNode NodeTreeNode { get; set; }

        public BDNodeWrapper() { }
        public BDNodeWrapper(IBDNode pNode, BDConstants.BDNodeType pTargetNodeType)
        {
            Node = pNode;
            TargetNodeType = pTargetNodeType;
        }

        public BDNodeWrapper(IBDNode pNode, BDConstants.BDNodeType pTargetNodeType, TreeNode pTreeNode)
        {
            Node = pNode;
            TargetNodeType = pTargetNodeType;
            NodeTreeNode = pTreeNode;
        }
    }
}
