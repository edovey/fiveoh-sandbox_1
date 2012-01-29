using System;
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

        public static List<IBDNode> GetAllForNodeType(Entities pDataContext, Constants.BDNodeType pNodeType)
        {
            List<IBDNode> entryList = new List<IBDNode>();

            switch (pNodeType)
            {
                case Constants.BDNodeType.None:
                    // do nothing
                    break;
                case Constants.BDNodeType.BDTherapy:
                    IQueryable<BDTherapy> tEntries = (from entry in pDataContext.BDTherapies
                                                                  orderby entry.displayOrder
                                                                  select entry);
                    if (tEntries.Count() > 0)
                    {
                        List<IBDNode> workingList = new List<IBDNode>(tEntries.ToList<BDTherapy>());
                        entryList.AddRange(workingList);
                    }
                    break;
                case Constants.BDNodeType.BDTherapyGroup:
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
                        Constants.BDNodeType childNodeType = association.ChildNodeType;
                        switch (childNodeType)
                        {
                            case Constants.BDNodeType.BDTherapyGroup:
                                IQueryable<BDTherapyGroup> tgEntries = (from entry in pContext.BDTherapyGroups
                                                                  where entry.parentId == pParentId
                                                                  orderby entry.displayOrder
                                                                  select entry);
                                if (tgEntries.Count() > 0)
                                {
                                    List<IBDNode> workingList = new List<IBDNode>(tgEntries.ToList<BDTherapyGroup>());
                                    entryList.AddRange(workingList);
                                }
                                break;

                            case Constants.BDNodeType.BDTherapy:
                                IQueryable<BDTherapy> tEntries = (from entry in pContext.BDTherapies
                                                                  where entry.parentId == pParentId
                                                                  orderby entry.displayOrder
                                                                  select entry);
                                if (tEntries.Count() > 0)
                                {
                                    List<IBDNode> workingList = new List<IBDNode>(tEntries.ToList<BDTherapy>());
                                    entryList.AddRange(workingList);
                                }
                                break;

                            default:
                                IQueryable<BDNode> nodeEntries = (from entry in pContext.BDNodes
                                                where entry.parentId == pParentId
                                                orderby entry.displayOrder
                                                select entry);

                                if (nodeEntries.Count() > 0)
                                {
                                    List<IBDNode> workingList = new List<IBDNode>(nodeEntries.ToList<BDNode>());
                                    entryList.AddRange(workingList);
                                }
                            break;
                        }
                    }
                }
            }
            return entryList;
        }
    }
}
