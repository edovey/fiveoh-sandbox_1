﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class BDFabrik
    {
        private static volatile BDFabrik instance;
        private static object syncRoot = new object();

        private BDFabrik() { }

        #region Singleton
        public static BDFabrik Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (syncRoot)
                    {
                        if (null == instance)
                        {
                            instance = new BDFabrik();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        public static void Save(Entities pDataContext, IBDNode pNode)
        {
            if (null != pNode)
            {
                switch (pNode.NodeType)
                {
                    case BDConstants.BDNodeType.BDCategory:
                    case BDConstants.BDNodeType.BDChapter:
                    case BDConstants.BDNodeType.BDDisease:
                    case BDConstants.BDNodeType.BDPathogen:
                    case BDConstants.BDNodeType.BDPathogenGroup:
                    case BDConstants.BDNodeType.BDPresentation:
                    case BDConstants.BDNodeType.BDSection:
                    case BDConstants.BDNodeType.BDSubCategory:
                        BDNode node = pNode as BDNode;
                        BDNode.Save(pDataContext, node);
                        break;

                    case BDConstants.BDNodeType.BDTherapy:
                        BDTherapy therapy = pNode as BDTherapy;
                        BDTherapy.Save(pDataContext, therapy);
                        break;
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        BDTherapyGroup therapyGroup = pNode as BDTherapyGroup;
                        BDTherapyGroup.Save(pDataContext, therapyGroup);
                        break;

                    case BDConstants.BDNodeType.None:
                    default:
                        break;
                }
            }
        }

        public static List<BDConstants.BDNodeType> ChildTypeDefinitionListForNode(IBDNode pNode)
        {
            if (null == pNode)
                return null;

            if ((pNode.LayoutVariant == BDConstants.LayoutVariantType.Undefined) || (null == pNode.LayoutVariant))
            {
                string message = string.Format("Undefined Layout Variant for node parameter in ChildTypeDefinitionListForNode method call. [{0}]", BDUtilities.GetEnumDescription(pNode.NodeType));
                throw new BDException(message, pNode);
            }

            return ChildTypeDefinitionListForNode(pNode.NodeType, pNode.LayoutVariant);
        }

        public static List<BDConstants.BDNodeType> ChildTypeDefinitionListForNode(BDConstants.BDNodeType pNodeType, BDConstants.LayoutVariantType pLayoutVariant)
        {
            List<BDConstants.BDNodeType> definitionList = new List<BDConstants.BDNodeType>();

            BDConstants.LayoutVariantType layoutVariant = pLayoutVariant;
            switch (pNodeType)
            {
                case BDConstants.BDNodeType.BDCategory:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            definitionList.Add(BDConstants.BDNodeType.BDDisease);
                            //definitionList.Add(BDConstants.BDNodeType.BDSubCategory);  //Test of multiple child types in nav tree
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDChapter:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            definitionList.Add(BDConstants.BDNodeType.BDSection);
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            definitionList.Add(BDConstants.BDNodeType.BDPresentation);
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogen:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPathogenGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            definitionList.Add(BDConstants.BDNodeType.BDPathogen);
                            definitionList.Add(BDConstants.BDNodeType.BDTherapyGroup);
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPresentation:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            definitionList.Add(BDConstants.BDNodeType.BDPathogenGroup);
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSection:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            definitionList.Add(BDConstants.BDNodeType.BDCategory);
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubCategory:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTherapy:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    switch (layoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                            definitionList.Add(BDConstants.BDNodeType.BDTherapy);
                            break;
                        default:
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.None:
                default:
                    definitionList = null;
                    break;
            }

            return definitionList;
        }

        public static List<IBDNode> GetAllForNodeType(Entities pDataContext, BDConstants.BDNodeType pNodeType)
        {
            List<IBDNode> entryList = new List<IBDNode>();

            switch (pNodeType)
            {
                case BDConstants.BDNodeType.None:
                    // do nothing
                    break;
                case BDConstants.BDNodeType.BDTherapy:
                    IQueryable<BDTherapy> tEntries = (from entry in pDataContext.BDTherapies
                                                                  orderby entry.displayOrder
                                                                  select entry);
                    if (tEntries.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(tEntries.ToList<BDTherapy>());
                        entryList.AddRange(workingList);
                    }
                    break;
                case BDConstants.BDNodeType.BDTherapyGroup:
                    IQueryable<BDTherapyGroup> tgEntries = (from entry in pDataContext.BDTherapyGroups
                                                                  orderby entry.displayOrder
                                                                  select entry);
                    if (tgEntries.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(tgEntries.ToList<BDTherapyGroup>());
                        entryList.AddRange(workingList);
                    }
                    break;
                default:
                    IQueryable<BDNode> nodeEntries = (from entry in pDataContext.BDNodes
                                                where entry.nodeType == (int)pNodeType
                                                orderby entry.displayOrder
                                                select entry);

                    if (nodeEntries.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(nodeEntries.ToList<BDNode>());
                        entryList.AddRange(workingList);
                    }
                    break;
            }

            return entryList;
        }

        public static List<IBDNode> GetChildrenForParentId(Entities pContext, Guid pParentId)
        {
            List<IBDNode> entryList = new List<IBDNode>();

            if (null != pParentId)
            {
                List<BDNodeAssociation> childNodeAssociationList = BDNodeAssociation.RetrieveList(pContext, pParentId);

                foreach (BDNodeAssociation association in childNodeAssociationList)
                {
                    if (null != association.childNodeType)
                    {
                        entryList.AddRange(GetChildrenForParentIdAndChildType(pContext, pParentId, association.ChildNodeType));
                    }
                }
            }
            return entryList;
        }

        public static List<IBDNode> GetChildrenForParentIdAndChildType(Entities pContext, Guid pParentId, BDConstants.BDNodeType pChildNodeType)
        {
            List<IBDNode> entryList = new List<IBDNode>();

            if (null != pParentId)
            {
                BDConstants.BDNodeType childNodeType = pChildNodeType;

                switch (childNodeType)
                {
                    case BDConstants.BDNodeType.BDTherapyGroup:
                        IQueryable<BDTherapyGroup> tgEntries = (from entry in pContext.BDTherapyGroups
                                                                where entry.parentId == pParentId
                                                                orderby entry.displayOrder ascending
                                                                select entry);
                        if (tgEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(tgEntries.ToList<BDTherapyGroup>());
                            entryList.AddRange(workingList);
                        }
                        break;

                    case BDConstants.BDNodeType.BDTherapy:
                        IQueryable<BDTherapy> tEntries = (from entry in pContext.BDTherapies
                                                          where entry.parentId == pParentId
                                                          orderby entry.displayOrder ascending
                                                          select entry);
                        if (tEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(tEntries.ToList<BDTherapy>());
                            entryList.AddRange(workingList);
                        }
                        break;

                    default:
                        IQueryable<BDNode> nodeEntries = (from entry in pContext.BDNodes
                                                          where (entry.parentId == pParentId) && (entry.nodeType == (int)pChildNodeType)
                                                          orderby entry.displayOrder ascending
                                                          select entry);

                        if (nodeEntries.Count() > 0)
                        {
                            List<IBDNode> workingList = new List<IBDNode>(nodeEntries.ToList<BDNode>());
                            entryList.AddRange(workingList);
                        }
                        break;
                }
            }
            return entryList;
        }

        public static List<IBDNode> RepairSiblingNodeDisplayOrder(Entities pContext, Guid pUuid, BDConstants.BDNodeType pNodeType)
        {
            List<IBDNode> siblingList = GetChildrenForParentIdAndChildType(pContext, pUuid, pNodeType);
            for (int idx = 0; idx < siblingList.Count; idx++)
            {
                siblingList[idx].DisplayOrder = idx;
                Save(pContext, siblingList[idx]);
            }

            return siblingList;
        }

        public static void ReorderNode(Entities pContext, IBDNode pNode, int pOffset)
        {
            if (!((null != pNode) && (null != pNode.ParentId) && (null != pNode.DisplayOrder)) ) return;

            List<IBDNode> siblingList = RepairSiblingNodeDisplayOrder(pContext, pNode.ParentId.Value, pNode.NodeType);

            int currentIndex = siblingList.FindIndex(t => t == pNode);
            if (currentIndex != pNode.DisplayOrder)
            {
                throw new BDException("Attempt to reorder a node without a valid display order", pNode);
            }

            if (currentIndex >= 0)
            {
                int requestedIndex = currentIndex + pOffset;
                if ((requestedIndex >= 0) && (requestedIndex < siblingList.Count))
                {
                    siblingList[requestedIndex].DisplayOrder = currentIndex;
                    Save(pContext, siblingList[requestedIndex]);

                    siblingList[currentIndex].DisplayOrder = requestedIndex;
                    Save(pContext, siblingList[currentIndex]);
                }
            }
        }

        public static IBDNode CreateChildNode(Entities pContext, IBDNode pParentNode, BDConstants.BDNodeType pChildType)
        {
            IBDNode result = null;

            // get the existing siblings to the new node

            List<IBDNode> siblingList = RepairSiblingNodeDisplayOrder(pContext, pParentNode.Uuid, pChildType);

            BDNodeAssociation.CreateAssociationsForNode(pContext, pParentNode);

            switch (pChildType)
            {
                case BDConstants.BDNodeType.BDTherapyGroup:
                    BDTherapyGroup therapyGroup = BDTherapyGroup.CreateTherapyGroup(pContext, pParentNode.Uuid);
                    therapyGroup.displayOrder = siblingList.Count;
                    therapyGroup.SetParent(pParentNode);
                    therapyGroup.LayoutVariant = pParentNode.LayoutVariant;
                    therapyGroup.Name = String.Format("New Therapy Group {0}", therapyGroup.displayOrder + 1);
                    BDTherapyGroup.Save(pContext, therapyGroup);
                    BDNodeAssociation.CreateAssociationsForNode(pContext, therapyGroup);
                    break;

                case BDConstants.BDNodeType.BDTherapy:
                    BDTherapy therapy = BDTherapy.CreateTherapy(pContext, pParentNode.Uuid);
                    therapy.displayOrder = siblingList.Count;
                    therapy.SetParent(pParentNode);
                    therapy.LayoutVariant = pParentNode.LayoutVariant;
                    therapy.Name = String.Format("New Therapy {0}", therapy.displayOrder + 1);
                    BDTherapy.Save(pContext, therapy);
                    BDNodeAssociation.CreateAssociationsForNode(pContext, therapy);
                    break;

                default:
                    BDNode node = BDNode.CreateNode(pContext, pChildType);
                    node.displayOrder = siblingList.Count;
                    node.SetParent(pParentNode);
                    node.LayoutVariant = pParentNode.LayoutVariant;
                    node.Name = String.Format("New {0}-{1}", BDUtilities.GetEnumDescription(pChildType), node.displayOrder + 1);
                    BDNode.Save(pContext, node);
                    BDNodeAssociation.CreateAssociationsForNode(pContext, node);
                    break;
            }
            return result;
        }

        public static void DeleteNode(Entities pContext, IBDNode pNode)
        {
            Guid parentId = pNode.ParentId.Value;
            BDConstants.BDNodeType nodeType = pNode.NodeType;

            switch (nodeType)
            {
                case BDConstants.BDNodeType.BDTherapyGroup:
                    BDTherapyGroup therapyGroup = pNode as BDTherapyGroup;
                    BDTherapyGroup.Delete(pContext, therapyGroup, true);
                    break;

                case BDConstants.BDNodeType.BDTherapy:

                    BDTherapy therapy = pNode as BDTherapy;
                    BDTherapy.Delete(pContext, therapy, true);
                    break;

                default:
                    BDNode node = pNode as BDNode;
                    BDNode.Delete(pContext, node, true);
                    break;
            }

            List<IBDNode> siblingList = RepairSiblingNodeDisplayOrder(pContext, parentId, nodeType);
        }
    }
}
