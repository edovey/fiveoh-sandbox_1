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

        public BDEditView()
        {
            InitializeComponent();

            dataContext = new DataModel.Entities();

            LoadChapterDropDown();

            //sectionDropDown.DataSource = dataContext.BDSections;
            chapterDropDown.DisplayMember = "Name";

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
            splitContainer1.Panel2.Controls.Clear();

            this.Cursor = Cursors.WaitCursor;

            chapterTree.Nodes.Clear();

            BDNode listEntry = chapterDropDown.SelectedItem as BDNode;
            if ((null != listEntry) && (listEntry.NodeType == Constants.BDNodeType.BDChapter))
            {
                switch (listEntry.LayoutVariant)
                    {
                        case Constants.LayoutVariantType.TreatmentRecommendation00:
                            TreeNode node = TreatmentRecommendationTree.BuildChapterTreeNode(dataContext, listEntry);
                            // this is only to prevent a single first node
                            TreeNode[] nodeList = new TreeNode[node.Nodes.Count];
                            node.Nodes.CopyTo(nodeList, 0);
                            chapterTree.Nodes.AddRange(nodeList);
                            // ---
                            break;
                    }
            }

            this.Cursor = Cursors.Default;
        }

        private void sectionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            switch (e.Action)
            {
                case TreeViewAction.ByKeyboard:
                case TreeViewAction.ByMouse:
                    splitContainer1.Panel2.SuspendLayout();
                    splitContainer1.Panel2.Controls.Clear();
                    TreeNode selectedNode = e.Node;

                    IBDNode node = selectedNode.Tag as IBDNode;

                    switch (node.NodeType)
                    {
                        case Constants.BDNodeType.BDSection:
                            switch (node.LayoutVariant)
                            {
                                case Constants.LayoutVariantType.TreatmentRecommendation01:
                                    BDNodeControl control_tr01 = null;
                                    control_tr01 = new BDNodeControl(dataContext, node);
                                    control_tr01.Dock = DockStyle.Fill;
                                    splitContainer1.Panel2.Controls.Add(control_tr01);
                                    control_tr01.RefreshLayout();
                                    break;
                            }
                            break;
                        case Constants.BDNodeType.BDCategory:
                            switch (node.LayoutVariant)
                            {
                                case Constants.LayoutVariantType.TreatmentRecommendation01:
                                    BDNodeControl control_tr01 = null;
                                    control_tr01 = new BDNodeControl(dataContext, node);
                                    control_tr01.Dock = DockStyle.Fill;
                                    splitContainer1.Panel2.Controls.Add(control_tr01);
                                    control_tr01.RefreshLayout();
                                    break;
                            }
                            break;
                        case Constants.BDNodeType.BDDisease:
                            switch (node.LayoutVariant)
                            {
                                case Constants.LayoutVariantType.TreatmentRecommendation01:
                                    BDNodeControl control_tr01 = null;
                                    control_tr01 = new BDNodeControl(dataContext, node);
                                    control_tr01.Dock = DockStyle.Fill;
                                    splitContainer1.Panel2.Controls.Add(control_tr01);
                                    control_tr01.RefreshLayout();
                                    break;
                            }
                            break;
                        case Constants.BDNodeType.BDPresentation:
                            BDNode presentation = node as BDNode;
                            switch (node.LayoutVariant)
                            {
                                case Constants.LayoutVariantType.TreatmentRecommendation01:
                                    BDPresentationControl control_tr01 = new BDPresentationControl(dataContext, presentation);
                                    control_tr01.Dock = DockStyle.Fill;
                                    control_tr01.CurrentPresentation = presentation;
                                    control_tr01.AssignScopeId((null != presentation) ? presentation.Uuid : Guid.Empty);
                                    control_tr01.AssignParentId(presentation.ParentId);

                                    splitContainer1.Panel2.Controls.Add(control_tr01);
                                    control_tr01.RefreshLayout();
                                    break;
                            }
                            break;
                    }

                    splitContainer1.Panel2.ResumeLayout();
                    break;
                case TreeViewAction.Collapse:
                case TreeViewAction.Expand:
                case TreeViewAction.Unknown:
                default:
                    break;
            }
            this.Cursor = Cursors.Default;
        }

  
        private void loadSeedData_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            BDDataLoader dataLoader = new BDDataLoader();

            dataLoader.ImportData(dataContext, @"Resources\Chapter_2a.txt", BDDataLoader.baseDataDefinitionType.chapter2a);

            LoadChapterDropDown();
            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDNodes.Count() <= 0);

            this.Cursor = Cursors.Default;
        }

        private void LoadChapterDropDown()
        {
            chapterDropDown.Items.Clear();
            foreach(IBDNode entry in BDFabrik.GetAllForNodeType(dataContext, Constants.BDNodeType.BDChapter))
            {
                chapterDropDown.Items.Add(entry);
            }

            if (chapterDropDown.Items.Count > 2)
                chapterDropDown.SelectedIndex = 2;
            else if (chapterDropDown.Items.Count > 1)
                chapterDropDown.SelectedIndex = 1;
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
                lbLastSyncDateTime.Text = lastSyncDate.Value.ToString(Constants.DATETIMEFORMAT);
            }
        }

        private void SyncData()
        {
            this.Cursor = Cursors.WaitCursor;
            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            SyncInfoDictionary syncResultList = RepositoryHandler.Aws.Sync(DataContext, lastSyncDate);

            string resultMessage = string.Empty;

            foreach (SyncInfo syncInfo in syncResultList.Values)
            {
                System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                if( (syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0) )
                    resultMessage = string.Format("{0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage)? "": "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
            }

            if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

            MessageBox.Show(resultMessage, "Snchronization");

            UpdateSyncLabel();
            LoadChapterDropDown();

            systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            lastSyncDate = systemSetting.settingDateTimeValue;
            loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDNodes.Count() <= 0);

            this.Cursor = Cursors.Default;
        }

        private void BDEditView_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}" , "Bugs & Drugs Editor", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

#if DEBUG
            this.Text = this.Text + @" < DEVELOPMENT >";
            this.btnImportFromProduction.Visible = true;
#else
            this.btnImportFromProduction.Visible = false;
#endif

            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;
            loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDNodes.Count() <= 0);
            UpdateSyncLabel();
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
                //BDSystemSetting.SaveTimestamp(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP, null);
                this.Cursor = Cursors.Default;

                SyncData();          
            }
        }

        private void splitContainer1_Leave(object sender, EventArgs e)
        {
            save();
        }

        private void lbLastSyncDateTime_Click(object sender, EventArgs e)
        {

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

        private void brewButton_Click(object sender, EventArgs e)
        {
            SearchEntryGenerator.Generate();
        }

        private void btnImportFromProduction_Click(object sender, EventArgs e)
        {

#if DEBUG
            this.Cursor = Cursors.WaitCursor;
            BDSystemSetting systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            DateTime? lastSyncDate = systemSetting.settingDateTimeValue;

            SyncInfoDictionary syncResultList = RepositoryHandler.Aws.ImportFromProduction(dataContext, null);

            string resultMessage = string.Empty;

            foreach (SyncInfo syncInfo in syncResultList.Values)
            {
                System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                if ((syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0))
                    resultMessage = string.Format("Procustion Import {0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage) ? "" : "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
            }

            if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

            MessageBox.Show(resultMessage, "Snchronization");

            UpdateSyncLabel();
            LoadChapterDropDown();

            systemSetting = BDSystemSetting.GetSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            lastSyncDate = systemSetting.settingDateTimeValue;
            loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDNodes.Count() <= 0);

            this.Cursor = Cursors.Default;
#else
            MessageBox.Show(@"May not import in this environment" , "Import");
#endif
        }
    }
}
