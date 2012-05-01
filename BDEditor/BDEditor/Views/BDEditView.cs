using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;
using BDEditor.Classes.Navigation;

using System.Diagnostics;

namespace BDEditor.Views
{
    public partial class BDEditView : Form
    {
        BDEditor.DataModel.Entities dataContext;

        List<ToolStripMenuItem> addNodeToolStripMenuItemList = new List<ToolStripMenuItem>();

        public BDEditView()
        {
            InitializeComponent();

            dataContext = new DataModel.Entities();

            LoadChapterDropDown();

            chapterDropDown.DisplayMember = "Name";

            BDNotification.Notify += new EventHandler<BDNotificationEventArgs>(BDNotification_Notify);

        }

        void BDNotification_Notify(object sender, BDNotificationEventArgs e)
        {
            switch (e.NotificationType)
            {
                case BDNotificationEventArgs.BDNotificationType.Refresh:
                case BDNotificationEventArgs.BDNotificationType.Addition:
                case BDNotificationEventArgs.BDNotificationType.Deletion:
                     showNavSelection(chapterTree.SelectedNode);
                    break;
            }
        }
        
        public DataModel.Entities DataContext
        {
            get
            {
                if (null == dataContext)
                {
                    dataContext = new Entities();
                }
                return dataContext;
            }

            set
            {
                dataContext = value;
            }
        }

        private void listDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            IBDNode listEntry = chapterDropDown.SelectedItem as IBDNode;

            displayChapter(listEntry);
        }

        private void displayChapter(IBDNode pNode)
        {
            Cursor _cursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            
            splitContainer1.Panel2.Controls.Clear();

            chapterTree.Nodes.Clear();

            if ((null != pNode) && (pNode.NodeType == BDConstants.BDNodeType.BDChapter))
            {
                switch (pNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.TreatmentRecommendation00:
                        TreeNode node = TreatmentRecommendationTree.BuildBranch(dataContext, pNode);
                        // this is only to prevent a single first node
                        TreeNode[] nodeList = new TreeNode[node.Nodes.Count];
                        node.Nodes.CopyTo(nodeList, 0);
                        chapterTree.Nodes.AddRange(nodeList);

                        // ---
                        break;
                }
            }

            this.Cursor = _cursor;
        }

        private void graftTreeNode(TreeNode pTree, TreeNode pBranch)
        {
            TreeNode[] nodeList = new TreeNode[pBranch.Nodes.Count];
            pBranch.Nodes.CopyTo(nodeList, 0);
            pTree.Nodes.Clear();
            pTree.Nodes.AddRange(nodeList);
            pTree.Expand();
        }

        private void buildNavContextMenuStrip(TreeNode pTreeNode, IBDNode pBDNode)
        {
            foreach (ToolStripMenuItem entry in addNodeToolStripMenuItemList)
            {
                entry.Click -= new System.EventHandler(this.addChildNode_Click);
            }
            addNodeToolStripMenuItemList.Clear();
            addChildNodeToolStripMenuItem.DropDownItems.Clear();

            reorderNextToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);
            reorderPreviousToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);
            deleteToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, pBDNode.NodeType, pBDNode.LayoutVariant, pTreeNode);

            string nodeTypeName = BDUtilities.GetEnumDescription(pBDNode.NodeType);

            deleteToolStripMenuItem.Text = string.Format("Delete {0}: {1}", nodeTypeName, pBDNode.Name);

            List<Tuple<BDConstants.BDNodeType, BDConstants.LayoutVariantType[]>> childTypeInfoList = BDFabrik.ChildTypeDefinitionListForNode(pBDNode);
            if (null != childTypeInfoList)
            {
                if (childTypeInfoList.Count == 1)
                {
                    string childNodeTypeName = BDUtilities.GetEnumDescription(childTypeInfoList[0].Item1);
                    addChildNodeToolStripMenuItem.Text = string.Format("Add {0}", childNodeTypeName);

                    if (childTypeInfoList[0].Item2.Length == 1)
                    {
                        addChildNodeToolStripMenuItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[0], pTreeNode);
                    }
                    else
                    {
                        for (int idx = 0; idx < childTypeInfoList[0].Item2.Length; idx++)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem();

                            item.Image = global::BDEditor.Properties.Resources.add_16x16;
                            item.Name = string.Format("dynamicAddChildLayoutVariant{0}", idx);
                            item.Size = new System.Drawing.Size(179, 22);
                            item.Text = string.Format("&Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[0].Item2[idx]));
                            item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[0].Item1, childTypeInfoList[0].Item2[idx], pTreeNode);
                            item.Click += new System.EventHandler(this.addChildNode_Click);
                            addChildNodeToolStripMenuItem.DropDownItems.Add(item);
                        }
                    }
                }
                else if (childTypeInfoList.Count > 1)
                {
                    addChildNodeToolStripMenuItem.Text = string.Format("Add");

                    for (int idx = 0; idx < childTypeInfoList.Count; idx++)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem();

                        item.Image = global::BDEditor.Properties.Resources.add_16x16;
                        item.Name = string.Format("dynamicAddChild{0}", idx);
                        item.Size = new System.Drawing.Size(179, 22);
                        item.Text = string.Format("&Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item1));
                        item.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[0], pTreeNode);
                        item.Click += new System.EventHandler(this.addChildNode_Click);

                        if (childTypeInfoList[idx].Item2.Length > 1)
                        {
                            for (int idy = 0; idy < childTypeInfoList[idx].Item2.Length; idy++)
                            {
                                ToolStripMenuItem layoutItem = new ToolStripMenuItem();

                                layoutItem.Image = global::BDEditor.Properties.Resources.add_16x16;
                                layoutItem.Name = string.Format("dynamicAddChildLayoutVariant{0}", idy);
                                layoutItem.Size = new System.Drawing.Size(179, 22);
                                layoutItem.Text = string.Format("Add {0}", BDUtilities.GetEnumDescription(childTypeInfoList[idx].Item2[idy]));
                                layoutItem.Tag = new BDNodeWrapper(pBDNode, childTypeInfoList[idx].Item1, childTypeInfoList[idx].Item2[idy], pTreeNode);
                                layoutItem.Click += new System.EventHandler(this.addChildNode_Click);
                                item.DropDownItems.Add(layoutItem);
                            }
                        }

                        addChildNodeToolStripMenuItem.DropDownItems.Add(item);
                    }
                }
            }
        }

        void reorderNodeToPrevious_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;

                BDFabrik.ReorderNode(DataContext, nodeWrapper.Node, -1);

                TreeNode parentNode = nodeWrapper.NodeTreeNode.Parent;
                if (null == parentNode)
                {
                    IBDNode listEntry = chapterDropDown.SelectedItem as IBDNode;

                    displayChapter(listEntry);
                }
                else
                {
                    chapterTree.SelectedNode = parentNode;
                    showNavSelection(parentNode);
                }
            }
        }

        void reorderNodeToNext_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;

                BDFabrik.ReorderNode(DataContext, nodeWrapper.Node, 1);

                TreeNode parentNode = nodeWrapper.NodeTreeNode.Parent;
                if (null == parentNode)
                {
                    IBDNode listEntry = chapterDropDown.SelectedItem as IBDNode;

                    displayChapter(listEntry);
                }
                else
                {
                    chapterTree.SelectedNode = parentNode;
                    showNavSelection(parentNode);
                }
            }
        }

        void addChildNode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    BDFabrik.CreateChildNode(DataContext, nodeWrapper.Node, nodeWrapper.TargetNodeType, nodeWrapper.TargetLayoutVariant);
                    showNavSelection(nodeWrapper.NodeTreeNode);
                }
            }
        }

        void deleteNode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    string nodeTypeName = BDUtilities.GetEnumDescription(nodeWrapper.Node.NodeType);
                    string message = string.Format("Are you sure you want to delete the {0} named [{1}]?\nThis will also delete all associated child information and cannot be undone", nodeTypeName, nodeWrapper.Node.Name);

                    if (MessageBox.Show(this, message, "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                    {
                        BDFabrik.DeleteNode(DataContext, nodeWrapper.Node);
                        showNavSelection(nodeWrapper.NodeTreeNode.Parent);
                    }
                }
            }
        }

        private void chapterTree_MouseUp(object sender, MouseEventArgs e)
        {
            TreeView tree = sender as TreeView;
            if (null == tree)
                return;

            // only need to change selected note during right-click - otherwise tree does fine by itself      
            if (e.Button == MouseButtons.Right)
            {
                Point pt = new Point(e.X, e.Y);
                tree.PointToClient(pt);
                TreeNode treeNode = tree.GetNodeAt(pt);
                if (null != treeNode)
                {
                    if (treeNode.Bounds.Contains(pt))
                    {
                        if (!treeNode.IsSelected)
                        {
                            chapterTree.SelectedNode = treeNode;
                            showNavSelection(treeNode);
                        }
                        IBDNode bdNode = treeNode.Tag as IBDNode;
                        buildNavContextMenuStrip(treeNode, bdNode);
                            navTreeContextMenuStrip.Show(tree, pt);
                    }
                }
            } 
        }
        
        private void sectionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Action)
            {
                case TreeViewAction.ByKeyboard:
                case TreeViewAction.ByMouse:
                    showNavSelection(e.Node);
                    break;
                case TreeViewAction.Collapse:
                case TreeViewAction.Expand:
                case TreeViewAction.Unknown:
                default:
                    break;
            }
        }

        private void showNavSelection(TreeNode pSelectedNode)
        {
            this.Cursor = Cursors.WaitCursor;

            splitContainer1.Panel2.SuspendLayout();
            ControlHelper.SuspendDrawing(splitContainer1.Panel2);

            foreach (Control ctrl in splitContainer1.Panel2.Controls)
            {
                IBDControl nodeCtrl = ctrl as IBDControl;
                if (null != nodeCtrl)
                {
                    nodeCtrl.NameChanged -= new EventHandler<NodeEventArgs>(nodeControl_NameChanged);
                    nodeCtrl.RequestItemAdd -= new EventHandler<NodeEventArgs>(siblingNodeCreateRequest);
                }
            }

            splitContainer1.Panel2.Controls.Clear();
            TreeNode selectedNode = pSelectedNode;

            IBDNode node = selectedNode.Tag as IBDNode;
            IBDControl control_tr01 = null;
            bool showChildControls = true;

            switch (node.NodeType)
            {
                case BDConstants.BDNodeType.BDSection:
                    switch (node.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            TreeNode childTreeNode = TreatmentRecommendationTree.BuildBranch(dataContext, node);
                            graftTreeNode(selectedNode, childTreeNode);

                            control_tr01 = new BDNodeOverviewControl(dataContext, node);
                            showChildControls = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDCategory:
                    switch (node.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic:
                            TreeNode childTreeNode = TreatmentRecommendationTree.BuildBranch(dataContext, node);
                            graftTreeNode(selectedNode, childTreeNode);

                            control_tr01 = new BDNodeOverviewControl(dataContext, node);
                            showChildControls = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDDisease:
                    switch (node.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            TreeNode childTreeNode = TreatmentRecommendationTree.BuildBranch(dataContext, node);
                            graftTreeNode(selectedNode, childTreeNode);

                            control_tr01 = new BDNodeOverviewControl(dataContext, node);
                            showChildControls = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDSubCategory:
                    switch (node.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic:
                            TreeNode childTreeNode = TreatmentRecommendationTree.BuildBranch(dataContext, node);
                            graftTreeNode(selectedNode, childTreeNode);

                            control_tr01 = new BDNodeOverviewControl(dataContext, node);
                            showChildControls = false;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDPresentation:
                    switch (node.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation01:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation08_Opthalmic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation09_Parasitic:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation10_Fungal:
                            control_tr01 = new BDNodeOverviewControl(dataContext, node);
                            showChildControls = true;
                            break;
                    }
                    break;
                case BDConstants.BDNodeType.BDTable:
                    switch (node.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.TreatmentRecommendation02_WoundMgmt:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation03_WoundClass:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_I:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation04_Pneumonia_II:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation05_Peritonitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation06_Meningitis:
                        case BDConstants.LayoutVariantType.TreatmentRecommendation07_Endocarditis:
                            control_tr01 = new BDNodeControl(dataContext, node);
                            showChildControls = true;
                            break;
                    }
                    break;
            }

            if (null != control_tr01)
            {
                control_tr01.AssignScopeId((null != node) ? node.Uuid : Guid.Empty);
                control_tr01.AssignParentInfo(node.ParentId, node.ParentType);
                ((System.Windows.Forms.UserControl)control_tr01).Dock = DockStyle.Fill;
                control_tr01.NameChanged += new EventHandler<NodeEventArgs>(nodeControl_NameChanged);
                control_tr01.RequestItemAdd += new EventHandler<NodeEventArgs>(siblingNodeCreateRequest);
                splitContainer1.Panel2.Controls.Add((System.Windows.Forms.UserControl)control_tr01);
                control_tr01.RefreshLayout(showChildControls);
            }

            ControlHelper.ResumeDrawing(splitContainer1.Panel2);
            splitContainer1.Panel2.ResumeLayout();

            this.Cursor = Cursors.Default;
        }

        void nodeControl_NameChanged(object sender, NodeEventArgs e)
        {
            IBDNode node = chapterTree.SelectedNode.Tag as IBDNode;
            if (node.Uuid == e.Uuid)
            {
                chapterTree.SelectedNode.Text = e.Text;
            }
        }

        void siblingNodeCreateRequest(object sender, NodeEventArgs e)
        {
            IBDNode siblingNode = BDFabrik.RetrieveNode(e.DataContext, e.NodeType, e.Uuid);
            if(null != siblingNode)
            {
                IBDNode parentNode = BDFabrik.RetrieveNode(e.DataContext, siblingNode.ParentType, siblingNode.ParentId);
                BDFabrik.CreateChildNode(DataContext, parentNode, e.NodeType, e.LayoutVariant);
                showNavSelection(chapterTree.SelectedNode);
            }
        }

        private void loadSeedData_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            BDDataLoader dataLoader = new BDDataLoader();

            //dataLoader.ImportData(dataContext, @"Resources\Chapter_2a.txt", BDDataLoader.baseDataDefinitionType.chapter2a);
            dataLoader.ImportData(dataContext, @"Resources\Chapter_2b.txt", BDDataLoader.baseDataDefinitionType.chapter2b);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_2c.txt", BDDataLoader.baseDataDefinitionType.chapter2c);
            //dataLoader.ImportData(dataContext, @"Resources\Chapter_2d.txt", BDDataLoader.baseDataDefinitionType.chapter2d);

            LoadChapterDropDown();
            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            //loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDNodes.Count() <= 0);

            this.Cursor = Cursors.Default;
        }

        private void LoadChapterDropDown()
        {
            splitContainer1.Panel2.Controls.Clear();

            chapterTree.Nodes.Clear();

            chapterDropDown.Items.Clear();
            foreach(IBDNode entry in BDFabrik.GetAllForNodeType(dataContext, BDConstants.BDNodeType.BDChapter))
            {
                chapterDropDown.Items.Add(entry);
            }

            if (chapterDropDown.Items.Count >= 1)
                chapterDropDown.SelectedIndex = 0;

        }

        private void UpdateSyncLabel()
        {
            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);

            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;
            if (null == lastSyncDate)
            {
                lbLastSyncDateTime.Text = @"<Never Sync'd>";
            }
            else
            {
                lbLastSyncDateTime.Text = lastSyncDate.Value.ToString(BDConstants.DATETIMEFORMAT);
            }
        }

        private void SyncData()
        {
            this.Cursor = Cursors.WaitCursor;
            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            SyncInfoDictionary syncResultList = RepositoryHandler.Aws.Sync(DataContext, lastSyncDate, BDConstants.SyncType.Default);

            string resultMessage = string.Empty;

            foreach (SyncInfo syncInfo in syncResultList.Values)
            {
                System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                if( (syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0) )
                    resultMessage = string.Format("{0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage)? "": "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
            }

            if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

            MessageBox.Show(resultMessage, "Synchronization");

            UpdateSyncLabel();
            LoadChapterDropDown();

            systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            lastSyncDate = systemSetting.settingDateTimeValue;
            //loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDNodes.Count() <= 0);

            this.Cursor = Cursors.Default;
        }

        private void BDEditView_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}" , "Bugs & Drugs Editor", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

#if DEBUG
            this.Text = this.Text + @" < DEVELOPMENT >";
            this.btnImportFromProduction.Visible = true;
            this.btnPublish.Visible = true;
#else
            this.btnImportFromProduction.Visible = false;
            this.btnPublish.Visible = false;
#endif


            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;
            //loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDNodes.Count() <= 0);
            UpdateSyncLabel();

            AuthenticationInputBox authenticationForm = new AuthenticationInputBox();
            authenticationForm.ShowDialog(this);

            if (!BDCommon.Settings.SyncPushEnabled)
            {
                btnSync.Text = "Pull Only";
            }
            btnPublish.Enabled = BDCommon.Settings.SyncPushEnabled;
            btnImportFromProduction.Enabled = BDCommon.Settings.SyncPushEnabled;
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            SyncData();
        }

        private void btnSyncWithReplaceLocal_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will DELETE all local data and replace it from the repository?", "Replace Local Data", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;

                chapterDropDown.Items.Clear();
                chapterDropDown.SelectedIndex = -1;

                RepositoryHandler.Aws.DeleteLocalData(dataContext);
                
                dataContext = null;
                dataContext = new Entities();

                BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                systemSetting.settingDateTimeValue = null;
                dataContext.SaveChanges();
                this.Cursor = Cursors.Default;

                SyncData();          
            }
        }

        private void splitContainer1_Leave(object sender, EventArgs e)
        {
            save();
        }

        private void BDEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            save();
        }

        private void save()
        {
            if (splitContainer1.Panel2.Controls.Count > 0)
            {
                IBDControl control = splitContainer1.Panel2.Controls[0] as IBDControl;
                if (null != control)
                {
                    control.Save();
                }
            }
        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            BDHtmlPageGenerator generator = new BDHtmlPageGenerator();
            generator.Generate();
            System.Diagnostics.Debug.WriteLine("HTML page generation complete.");
            BDSearchEntryGenerator.Generate();
            System.Diagnostics.Debug.WriteLine("Search entry generation complete.");

            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            SyncInfoDictionary syncResultList = RepositoryHandler.Aws.Sync(DataContext, lastSyncDate, BDConstants.SyncType.Publish);

            string resultMessage = string.Empty;

            foreach (SyncInfo syncInfo in syncResultList.Values)
            {
                System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                if ((syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0))
                    resultMessage = string.Format("{0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage) ? "" : "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
            }

            if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

            MessageBox.Show(resultMessage, "Synchronization");
            this.Cursor = Cursors.Default;
        }

        private void btnImportFromProduction_Click(object sender, EventArgs e)
        {

#if DEBUG
            if (MessageBox.Show("Have the development respository domains (bd_dev_2*) and bdDevStore bucket been cleared?", "Import from Production", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;
                BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                DateTime? lastSyncDate = null;

                SyncInfoDictionary syncResultList = RepositoryHandler.Aws.ImportFromProduction(dataContext, null);

                string resultMessage = string.Empty;

                foreach (SyncInfo syncInfo in syncResultList.Values)
                {
                    System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                    if ((syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0))
                        resultMessage = string.Format("Production Import {0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage) ? "" : "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
                }

                if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

                MessageBox.Show(resultMessage, "Synchronization");

                UpdateSyncLabel();
                LoadChapterDropDown();

                systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                lastSyncDate = systemSetting.settingDateTimeValue;
                loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDNodes.Count() <= 0);

                this.Cursor = Cursors.Default;
            }
#else
            MessageBox.Show(@"May not import in this environment" , "Import");
#endif
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = chapterTree.SelectedNode;

            // search for a term:  open the search window
            BDSearchView searchView = new BDSearchView();
            searchView.AssignDataContext(dataContext);
            searchView.ShowDialog(this);

            if (null != chapterTree.SelectedNode)
            {
                this.Cursor = Cursors.WaitCursor;
                IBDNode newNode = chapterTree.SelectedNode.Tag as IBDNode;
                if (newNode.Name != treeNode.Text)
                    treeNode.Text = newNode.Name;
                showNavSelection(treeNode);
                this.Cursor = Cursors.Default;
            }
        }
    }
}
