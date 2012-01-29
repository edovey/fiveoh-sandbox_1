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

        public static List<IBDObject> GetChildrenForParentId(Entities pContext, Guid pParentId)
        {
            List<IBDObject> entryList = new List<IBDObject>();

            if (null != pParentId)
            {
                List<BDNodeAssociation> childNodeAssociationList = BDNodeAssociation.RetrieveList(pContext, pParentId);

                foreach (BDNodeAssociation association in childNodeAssociationList)
                {
                    if (null != association.childNodeType)
                    {
                        Constants.BDObjectType childNodeType = association.ChildNodeType;
                        switch (childNodeType)
                        {
                            case Constants.BDObjectType.BDTherapyGroup:
                                IQueryable<BDTherapyGroup> tgEntries = (from entry in pContext.BDTherapyGroups
                                                                  where entry.parentId == pParentId
                                                                  select entry);
                                if (tgEntries.Count() > 0)
                                {
                                    List<IBDObject> workingList = new List<IBDObject>(tgEntries.ToList<BDTherapyGroup>());
                                    entryList.AddRange(workingList);
                                }
                                break;

                            case Constants.BDObjectType.BDTherapy:
                                IQueryable<BDTherapy> tEntries = (from entry in pContext.BDTherapies
                                                                  where entry.parentId == pParentId
                                                                  select entry);
                                if (tEntries.Count() > 0)
                                {
                                    List<IBDObject> workingList = new List<IBDObject>(tEntries.ToList<BDTherapy>());
                                    entryList.AddRange(workingList);
                                }
                                break;

                            default:
                                IQueryable<BDNode> nodeEntries = (from entry in pContext.BDNodes
                                                where entry.parentId == pParentId
                                                select entry);

                                if (nodeEntries.Count() > 0)
                                {
                                    List<IBDObject> workingList = new List<IBDObject>(nodeEntries.ToList<BDNode>());
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
