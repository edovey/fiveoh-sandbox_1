using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Classes
{
    public class BDFormsUtilties
    {
        private BDFormsUtilties() { }

        #region Build Context Menu

        public static void buildNavContextMenuStrip(IBDNode pBDNode, 
                                                ToolStripMenuItem pChildAdd, 
                                                ToolStripMenuItem pSiblingAdd,
                                                ToolStripMenuItem pDelete,
            ToolStripMenuItem pEditIndexEntry,
                                                ToolStripMenuItem pReorderPrev, 
                                                ToolStripMenuItem pReorderNext,
                                                EventHandler<NodeEventArgs> pRequestItemDelete,
                                                EventHandler pEventHandlerAddChild,
                                                //Delegate pAddSiblingDelegate,
                                                bool pShowSiblingAdd)
        {
            string subDescription = string.Empty;
            //remove event handlers and existing entries
            //foreach (ToolStripMenuItem entry in addChildNodeToolStripMenuItemList)
            //{
            //    entry.Click -= new System.EventHandler(this.addChildNode_Click);
            //}
            //addChildNodeToolStripMenuItemList.Clear();
            //pSiblingAdd.Click -= new EventHandler(pAddSiblingDelegate);

            //foreach (ToolStripMenuItem entry in addSiblingNodeToolStripMenuItemList)
            //{
            //    entry.Click -= new System.EventHandler(this.addSiblingNode_Click);
            //    foreach (ToolStripMenuItem child in entry.DropDownItems)
            //    {
            //        child.Click -= new System.EventHandler(this.addSiblingNode_Click);

            //    }
            //}
            //addSiblingNodeToolStripMenuItemList.Clear();

            pChildAdd.Click -= pEventHandlerAddChild;
            foreach (ToolStripMenuItem entry in pChildAdd.DropDownItems)
            {
                entry.Click -= pEventHandlerAddChild;
                foreach (ToolStripMenuItem child in entry.DropDownItems)
                {
                    child.Click -= pEventHandlerAddChild;

                }
            }
            pChildAdd.DropDownItems.Clear();
            pSiblingAdd.DropDownItems.Clear();

            // build the entries
            pEditIndexEntry.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, null);
            pReorderNext.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, null);
            pReorderPrev.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, null);
            pDelete.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, null);

            //subDescription = (pBDNode.NodeType == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(pBDNode.LayoutVariant)) : string.Empty;
            subDescription = BDUtilities.GetEnumDescription(pBDNode.LayoutVariant);
            pSiblingAdd.Text = string.Format("&Add {0} [{1}]", BDUtilities.GetEnumDescription(pBDNode.NodeType), subDescription);

            // *****
            pSiblingAdd.Visible = pShowSiblingAdd;
            pSiblingAdd.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant);
            // *****
            string nodeTypeName = BDUtilities.GetEnumDescription(pBDNode.NodeType);

            EventHandler<NodeEventArgs> handler = pRequestItemDelete;
            if (null == handler)
                pDelete.Visible = false;
            else
                pDelete.Text = string.Format("Delete {0}: {1}", nodeTypeName, pBDNode.Name);

            List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> childTypeInfoList = BDFabrik.ChildTypeDefinitionListForNode(pBDNode);
            if (null == childTypeInfoList || childTypeInfoList.Count == 0)
            {
                pChildAdd.Visible = false;
                //toolStripSeparator2.Visible = ShowSiblingAdd;
            }
            else
            {
                if (childTypeInfoList.Count == 1)
                {
                    string childNodeTypeName = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item1);
                    //subDescription = (childTypeInfoList[0].Item1 == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[0])) : string.Empty;
                    subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[0]);
                    pChildAdd.Text = string.Format("Add {0} [{1}]", childNodeTypeName, subDescription);

                    if (childTypeInfoList[0].Item2.Length == 1)
                    {
                        pChildAdd.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[0], null);
                        pChildAdd.Click += pEventHandlerAddChild;
                    }
                    else
                    {
                        // One child type, many layout variants
                        for (int idx = 0; idx < childTypeInfoList[0].Item2.Length; idx++)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem();

                            item.Image = global::BDEditor.Properties.Resources.add_16x16;
                            item.Name = string.Format("dynamicAddChildLayoutVariant{0}", idx);
                            item.Size = new System.Drawing.Size(179, 22);
                            //subDescription = (childTypeInfoList[0].Item1 == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[idx])) : string.Empty;
                            subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[idx]);
                            item.Text = string.Format("&Add {0} [{1}]", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item1), subDescription);
                            item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[idx], null);
                            item.Click += pEventHandlerAddChild;
                            pChildAdd.DropDownItems.Add(item);
                        }
                    }
                }
                else if (childTypeInfoList.Count > 1)
                {
                    // Many child types
                    pChildAdd.Text = string.Format("Add");

                    for (int idx = 0; idx < childTypeInfoList.Count; idx++)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem();

                        item.Image = global::BDEditor.Properties.Resources.add_16x16;
                        item.Name = string.Format("dynamicAddChild{0}", idx);
                        item.Size = new System.Drawing.Size(179, 22);
                        //subDescription = (pBDNode.NodeType == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(((BDTableRow)pBDNode).NodeType)) : string.Empty;
                        //subDescription = (childTypeInfoList[idx].Item1 == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[0])) : string.Empty;
                        subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[0]);
                        item.Text = string.Format("&Add {0} [{1}]", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1), subDescription);
                        item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[0], null);
                        item.Click += pEventHandlerAddChild;

                        if (childTypeInfoList[idx].Item2.Length > 1)
                        {
                            // Many layout variants per child type
                            for (int idy = 0; idy < childTypeInfoList[idx].Item2.Length; idy++)
                            {
                                ToolStripMenuItem layoutItem = new ToolStripMenuItem();

                                layoutItem.Image = global::BDEditor.Properties.Resources.add_16x16;
                                layoutItem.Name = string.Format("dynamicAddChildLayoutVariant{0}", idy);
                                layoutItem.Size = new System.Drawing.Size(179, 22);
                                //subDescription = (childTypeInfoList[idx].Item1 == BDConstants.BDNodeType.BDTableRow) ? string.Format(" ({0})", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[idy])) : string.Empty;
                                subDescription = BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[idy]);
                                layoutItem.Text = string.Format("Add {0} [{1}]", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1), subDescription);
                                layoutItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[idy], null);
                                layoutItem.Click += pEventHandlerAddChild;
                                item.DropDownItems.Add(layoutItem);
                            }
                        }

                        pChildAdd.DropDownItems.Add(item);
                    }
                }
            }
        }

        #endregion
    }
}
