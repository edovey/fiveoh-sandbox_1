using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using BDEditor.Classes;
using BDEditor.Classes.Navigation;

using System.Diagnostics;

namespace BDEditor.Views
{
    public partial class BDBuildView : Form
    {
        private Entities dataContext;
        private string resultMessage;
        private List<BDNode> chapterList;
        private List<BDNode> selectedNodeList;
        
        private bool isPagesSelected = false;
        private bool isSearchSelected = false;
        private bool isSyncSelected = false;

        public BDBuildView()
        {
            InitializeComponent();
        }

        public BDBuildView(Entities pDataContext)
        {
            dataContext = pDataContext;
            InitializeComponent();
        }

        private void BDBuildView_Load(object sender, EventArgs e)
        {
            cbSyncWithAws.Enabled = BDCommon.Settings.SyncPushEnabled;
            flagGreen.Visible = BDCommon.Settings.SyncPushEnabled;

            maskedTextBox1.Text = string.Empty;

            chapterList = BDNode.RetrieveNodesForType(dataContext, BDConstants.BDNodeType.BDChapter);
            selectedNodeList = new List<BDNode>();
            selectedNodeList.AddRange(chapterList);
            foreach (BDNode chapter in chapterList)
                clbChaptersToGenerate.Items.Add(chapter.name, false);

        }

        private void cbGeneratePages_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGeneratePages.CheckState == CheckState.Checked)
            {
                gbSelectHtmlPages.Enabled = true;
                isPagesSelected = true;
            }
            else
            {
                if (cbGenerateSearch.CheckState != CheckState.Checked)
                    gbSelectHtmlPages.Enabled = false;

                isPagesSelected = false;

            }
            btn_OK.Enabled = (cbGeneratePages.CheckState == CheckState.Checked || cbGenerateSearch.CheckState == CheckState.Checked || cbSyncWithAws.CheckState == CheckState.Checked);
        }

        private void cbGenerateSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGenerateSearch.CheckState == CheckState.Checked)
            {
                gbSelectHtmlPages.Enabled = true;
                isSearchSelected = true;
            }
            else
            {
                if (cbGeneratePages.CheckState != CheckState.Checked)
                    gbSelectHtmlPages.Enabled = false;

                isSearchSelected = false;
            }
            btn_OK.Enabled = (cbGeneratePages.CheckState == CheckState.Checked || cbGenerateSearch.CheckState == CheckState.Checked || cbSyncWithAws.CheckState == CheckState.Checked);
        }

        private void rbSelectedUuids_CheckedChanged(object sender, EventArgs e)
        {
            selectedNodeList.Clear();
            clbChaptersToGenerate.Enabled = rbSelectedChapters.Checked;
        }

        private void rbSelectedChapters_CheckedChanged(object sender, EventArgs e)
        {
            selectedNodeList.Clear();
            clbChaptersToGenerate.Enabled = rbSelectedChapters.Checked;
        }

        private void rbAllChapters_CheckedChanged(object sender, EventArgs e)
        {
            selectedNodeList.Clear();
            clbChaptersToGenerate.Enabled = rbSelectedChapters.Checked;
            if (rbAllChapters.Checked)
                selectedNodeList.AddRange(chapterList);
        }

        private void clbChaptersToGenerate_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedNodeList.Clear();
            foreach (int idxChecked in clbChaptersToGenerate.CheckedIndices)
            {
                CheckState itemCheckState = clbChaptersToGenerate.GetItemCheckState(idxChecked);
                BDNode selectedNode = chapterList[idxChecked];
                if (itemCheckState == CheckState.Checked)
                    selectedNodeList.Add(selectedNode);
                else
                    selectedNodeList.Remove(selectedNode);
            }
        }

        private void tbUUIDsToGenerate_TextChanged(object sender, EventArgs e)
        {
            // parse the lines of the text box for UUIDs

            foreach (string textLine in tbUUIDsToGenerate.Lines)
            {
                Guid nodeUuid = Guid.Parse(textLine);
                if (null != nodeUuid)
                {
                    BDNode selectedNode = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
                    // if the Uuid is for something other than a BDNode, it won't be processed.
                    if(null != selectedNode) 
                        selectedNodeList.Add(selectedNode);
                }
            }
        }

        private void cbSyncWithAws_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSyncWithAws.CheckState == CheckState.Checked)
            {
                isSyncSelected = true;
                gbSyncRecordTypes.Enabled = true;
            }
            else
            {
                cbSyncHtml.CheckState = CheckState.Unchecked;
                cbSyncSearch.CheckState = CheckState.Unchecked;
                gbSyncRecordTypes.Enabled = false;
                isSyncSelected = false;
            }
            btn_OK.Enabled = (cbGeneratePages.CheckState == CheckState.Checked || cbGenerateSearch.CheckState == CheckState.Checked || cbSyncWithAws.CheckState == CheckState.Checked);
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            flagGreen.Visible = BDCommon.Settings.Validate(maskedTextBox1.Text);
            cbSyncWithAws.Enabled = flagGreen.Visible;
            BDCommon.Settings.Authenticate(maskedTextBox1.Text);
        }

        private void cbSelectedUuids_CheckedChanged(object sender, EventArgs e)
        {
            tbUUIDsToGenerate.Enabled = cbSelectedUuids.Checked;

            if (cbSelectedUuids.CheckState != CheckState.Checked)
            {
                foreach (string textLine in tbUUIDsToGenerate.Lines)
                {
                    Guid nodeUuid = Guid.Parse(textLine);
                    if (null != nodeUuid)
                    {
                        BDNode selectedNode = BDNode.RetrieveNodeWithId(dataContext, nodeUuid);
                        // if the Uuid is for something other than a BDNode, it won't be processed.
                        if (null != selectedNode && selectedNodeList.Contains(selectedNode))
                            selectedNodeList.Remove(selectedNode);
                    }
                }
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.publish();
            this.DialogResult = DialogResult.OK;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void publish()
        {
            this.Cursor = Cursors.WaitCursor;
            Debug.WriteLine(string.Format("Start {0}", DateTime.Now));

            BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Publish Start\t{0}", DateTime.Now));
            BDHtmlPageGeneratorLogEntry.AppendToFile("BDInternalLinkIssueLog.txt", string.Format("{0} -------------------------------", DateTime.Now));
            if (isPagesSelected)
            {
                BDHtmlPageGenerator generator = new BDHtmlPageGenerator();

                generator.Generate(dataContext, selectedNodeList);

                System.Diagnostics.Debug.WriteLine("HTML page generation complete.");
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Generation Complete\t{0}", DateTime.Now));
            }
            if (isSearchSelected)
            {
                Debug.WriteLine(string.Format("Search Gen Start {0}", DateTime.Now));
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Search Gen Start\t{0}", DateTime.Now));
                BDSearchEntryGenerator searchGenerator = new BDSearchEntryGenerator();
                searchGenerator.Generate(dataContext);
                Debug.WriteLine("Search entry generation complete. {0}", DateTime.Now);

                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Search Generation Complete\t{0}", DateTime.Now));
            }

            if (isSyncSelected && BDCommon.Settings.SyncPushEnabled)
            {
                RepositoryHandler.RepositoryHandlerPublishType publishTarget = RepositoryHandler.RepositoryHandlerPublishType.RepositoryHandlerPublishType_Development;
                if (rbPublishTypeProd.Checked)
                {
                    publishTarget = RepositoryHandler.RepositoryHandlerPublishType.RepositoryHandlerPublishType_Production;
                }

                BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                DateTime? lastSyncDate = systemSetting.settingDateTimeValue;
                Debug.WriteLine("Begin publish to AWS");
                BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("AWS Push Start\t{0}", DateTime.Now));
                SyncInfoDictionary syncResultList = null;
                if (cbSyncSearch.CheckState == CheckState.Checked && cbSyncHtml.CheckState != CheckState.Checked)
                {
                    Debug.WriteLine("Publish Search Only");
                    syncResultList = RepositoryHandler.Aws.Sync(dataContext, null, BDConstants.SyncType.SearchOnly, publishTarget);
                }
                else if (cbSyncSearch.CheckState != CheckState.Checked && cbSyncHtml.CheckState == CheckState.Checked)
                {
                    Debug.WriteLine("Publish Html Only");
                    syncResultList = RepositoryHandler.Aws.Sync(dataContext, null, BDConstants.SyncType.HtmlOnly, publishTarget);
                }
                else if (cbSyncSearch.CheckState == CheckState.Checked && cbSyncHtml.CheckState == CheckState.Checked)
                {
                    Debug.WriteLine("Publish All");
                    syncResultList = RepositoryHandler.Aws.Sync(dataContext, null, BDConstants.SyncType.All, publishTarget);
                }

                if (syncResultList != null)
                {
                    foreach (SyncInfo syncInfo in syncResultList.Values)
                    {
                        System.Diagnostics.Debug.WriteLine(syncInfo.FriendlyName);
                        if ((syncInfo.RowsPulled > 0) || (syncInfo.RowsPushed > 0))
                        {
                            resultMessage = string.Format("{0}{1}{4}: Pulled {2}, Pushed {3}", resultMessage, (string.IsNullOrEmpty(resultMessage) ? "" : "\n"), syncInfo.RowsPulled, syncInfo.RowsPushed, syncInfo.FriendlyName);
                        }
                    }

                    BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", resultMessage);
                    Debug.WriteLine(string.Format("Publish Complete at {0}", DateTime.Now));
                    BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("AWS Push (Publish) Complete\t{0}", DateTime.Now));

                    if (string.IsNullOrEmpty(resultMessage)) resultMessage = "No changes";

                    MessageBox.Show(resultMessage, "Publish");
                }
            }

            BDHtmlPageGeneratorLogEntry.AppendToFile("BDEditTimeLog.txt", string.Format("Publish Complete\t{0}", DateTime.Now));
            this.Cursor = Cursors.Default;
        }
    }
}
