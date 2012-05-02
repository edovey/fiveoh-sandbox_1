using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.ComponentModel;
using BDEditor.DataModel;
using BDEditor.Classes;


namespace BDEditor.Classes
{
    public class BDUtilities
    {
        private BDUtilities() { }

        public static string GetEnumDescription(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return value.ToString();
        }

        /// <summary>
        /// Manual (non-UI) move of nodes from one parent to the parent's sibling
        /// Does not change the node type
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNodeId"></param>
        /// <param name="pNewParentId"></param>
        /// <returns></returns>
        public static void MoveNode(Entities pContext, Guid pNodeId, Guid pNewParentId)
        {
            BDNode nodeToMove = BDNode.RetrieveNodeWithId(pContext, pNodeId);
            BDNode currentParent = BDNode.RetrieveNodeWithId(pContext, nodeToMove.ParentId.Value);

            BDNode newParent = BDNode.RetrieveNodeWithId(pContext, pNewParentId);

            nodeToMove.SetParent(newParent);
            nodeToMove.LayoutVariant = newParent.LayoutVariant;
            BDNode.Save(pContext, nodeToMove);
        }

        /// <summary>
        /// Manual (non-UI) move of nodes from one parent to the parent's sibling
        /// Does not change the node type
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pNode"></param>
        /// <param name="pNewParentName"></param>
        /// <returns></returns>
        public static void MoveNode(Entities pContext, BDNode pNode, string pNewParentName)
        {
            BDNode currentParent = BDNode.RetrieveNodeWithId(pContext, pNode.ParentId.Value);

            List<IBDNode> parentSiblings = BDFabrik.GetChildrenForParent(pContext, BDNode.RetrieveNodeWithId(pContext, currentParent.parentId.Value));
            BDNode newParent = null;
            foreach (BDNode n in parentSiblings)
            {
                if (n.Name == pNewParentName)
                    newParent = n;
            }
            if (newParent != null)
                MoveNode(pContext, pNode.Uuid, newParent.Uuid);
        }

        /// <summary>
        /// Move all children from one parent node to another - parent nodes should be siblings
        /// Does not change the node type
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pCurrentParentId"></param>
        /// <param name="pNewParentId"></param>
        public static void MoveAllChildren(Entities pContext, Guid pCurrentParentId, Guid pNewParentId)
        {
            // check that parents are siblings
            BDNode currentParent = BDNode.RetrieveNodeWithId(pContext, pCurrentParentId);
            List<IBDNode> siblings = BDFabrik.GetChildrenForParent(pContext, BDNode.RetrieveNodeWithId(pContext, currentParent.parentId.Value));
            bool isSibling = false;
            foreach (BDNode s in siblings)
                if (s.Uuid == pNewParentId)
                    isSibling = true;

            if (isSibling)
            {
                List<IBDNode> children = BDFabrik.GetChildrenForParent(pContext, BDNode.RetrieveNodeWithId(pContext, pCurrentParentId));
                foreach (BDNode n in children)
                    MoveNode(pContext, n.Uuid, pNewParentId);
            }
        }
    }
}
