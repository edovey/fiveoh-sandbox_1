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
    //public enum BDNodeContextType
    //{
    //    Chapter = 1,
    //    Section = 2,
    //    Category = 3,
    //    SubCategory = 4,
    //    Disease = 5,
    //    Presentation = 6,
    //    Therapy = 7,
    //    Pathogen = 8
    //}

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

            // This will preload the control into memory. 
            // Startup will be slower, but the first selection from the dropdown will be snappier
            //BDLinkedNoteControl control = new BDLinkedNoteControl();

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

        //public void createEntry(BDNodeContextType context)
        //{            
        //    switch (context)
        //    {
        //        case BDNodeContextType.Section:
        //            BDSection section = BDSection.CreateSection(dataContext);
        //            section.name = @"New Section";
        //            BDSection.Save(dataContext,section);
        //            break;

        //        default:
        //            break;
        //    }
        //}

        public void rebuildTree(BDChapter pChapter)
        {
            chapterTree.Nodes.Clear();

            TreeNode rootNode = new TreeNode(pChapter.name);

            List<BDSection> sectionList = BDSection.GetSectionsForParentId(dataContext, pChapter.uuid);

            foreach (BDSection section in sectionList)
            {
                TreeNode sectionNode = new TreeNode(section.name);
                sectionNode.Tag = section;

                List<BDCategory> categoryList = BDCategory.GetCategoriesForParentId(dataContext, section.uuid);
                foreach (BDCategory category in categoryList)
                {
                    TreeNode categoryNode = new TreeNode(category.name);
                    categoryNode.Tag = category;

                    List<BDSubcategory> subCategoryList = BDSubcategory.GetSubcategoriesForParentId(dataContext, category.uuid);
                    foreach (BDSubcategory subCategory in subCategoryList)
                    {
                        TreeNode subCategoryNode = new TreeNode(subCategory.name);
                        subCategoryNode.Tag = subCategory;

                        List<BDDisease> diseaseList = BDDisease.GetDiseasesForParentId(dataContext, subCategory.uuid);
                        foreach (BDDisease disease in diseaseList)
                        {
                            TreeNode diseaseNode = new TreeNode(disease.name);
                            diseaseNode.Tag = disease;

                            List<BDPresentation> presentationList = BDPresentation.GetPresentationsForParentId(dataContext, disease.uuid);
                            foreach (BDPresentation presentation in presentationList)
                            {
                                TreeNode presentationNode = new TreeNode(presentation.name);
                                presentationNode.Tag = presentation;
                                diseaseNode.Nodes.Add(presentationNode);
                            }
                            subCategoryNode.Nodes.Add(diseaseNode);
                        }
                        categoryNode.Nodes.Add(subCategoryNode);
                    } // subCatg

                    List<BDDisease> categorydiseaseList = BDDisease.GetDiseasesForParentId(dataContext, category.uuid);
                    foreach (BDDisease disease in categorydiseaseList)
                    {
                        TreeNode diseaseNode = new TreeNode(disease.name);
                        diseaseNode.Tag = disease;

                        List<BDPresentation> presentationList = BDPresentation.GetPresentationsForParentId(dataContext, disease.uuid);
                        foreach (BDPresentation presentation in presentationList)
                        {
                            TreeNode presentationNode = new TreeNode(presentation.name);
                            presentationNode.Tag = presentation;
                            diseaseNode.Nodes.Add(presentationNode);
                        }

                        categoryNode.Nodes.Add(diseaseNode);
                    } // disease

                    sectionNode.Nodes.Add(categoryNode);
                } // category
                chapterTree.Nodes.Add(sectionNode);
            } // node
        }

        private void listDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            //BDSection node = sectionDropDown.SelectedValue as BDSection;
            //BDChapter entry = chapterDropDown.SelectedItem as BDChapter;
            //if (null != entry)
            //{
            //    this.Cursor = Cursors.WaitCursor;
            //    rebuildTree(entry);
            //    this.Cursor = Cursors.Default;
            //}

            BDNode listEntry = chapterDropDown.SelectedItem as BDNode;
            if ((null != listEntry) && (listEntry.NodeType == Constants.BDNodeType.BDChapter))
            {
                BDMetadata meta = BDMetadata.GetMetadataWithItemId(dataContext, listEntry.Uuid);
                if (null != meta)
                {
                    switch (meta.LayoutVariant)
                    {
                        case BDMetadata.LayoutVariantType.TreatmentRecommendation00:
                            break;
                    }
                }
            }
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
                    if(selectedNode.Tag is BDCategory)
                    {
                        BDCategoryControl categoryControl = new BDCategoryControl();
                        categoryControl.AssignDataContext(dataContext);
                        categoryControl.Dock = DockStyle.Fill;
                        categoryControl.CurrentCategory = selectedNode.Tag as BDCategory;

                        splitContainer1.Panel2.Controls.Add(categoryControl);
                    }
                    else if (selectedNode.Tag is BDSubcategory)
                    {
                        BDSubCategoryControl subCategoryControl = new BDSubCategoryControl();
                        subCategoryControl.Dock = DockStyle.Fill;
                        subCategoryControl.CurrentSubcategory = selectedNode.Tag as BDSubcategory;
                        splitContainer1.Panel2.Controls.Add(subCategoryControl);
                    }
                    else if (selectedNode.Tag is BDDisease)
                    {
                        BDDiseaseControl diseaseControl = new BDDiseaseControl();
                        diseaseControl.AssignDataContext(dataContext);
                        
                        diseaseControl.Dock = DockStyle.Fill;
                        diseaseControl.CurrentDisease = selectedNode.Tag as BDDisease;
                        BDCategory category = selectedNode.Tag as BDCategory;
                        if (null != category)
                        {
                            diseaseControl.AssignParentId(category.uuid);
                            diseaseControl.CategoryId = category.uuid;
                        }

                        splitContainer1.Panel2.Controls.Add(diseaseControl);
                        diseaseControl.RefreshLayout();

                    }
                    else if (selectedNode.Tag is BDPresentation)
                    {
                        BDPresentationControl presentationControl = new BDPresentationControl();
                        presentationControl.AssignDataContext(dataContext);
                        presentationControl.Dock = DockStyle.Fill;
                        presentationControl.CurrentPresentation = selectedNode.Tag as BDPresentation;
                        presentationControl.AssignScopeId((null != presentationControl.CurrentPresentation) ? presentationControl.CurrentPresentation.uuid : Guid.Empty);
                        BDDisease disease = selectedNode.Parent.Tag as BDDisease;
                        if (null != disease)
                        {
                            presentationControl.AssignParentId(disease.uuid);
                        }

                        splitContainer1.Panel2.Controls.Add(presentationControl);
                        presentationControl.RefreshLayout();
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
            loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDSections.Count() <= 0);

            this.Cursor = Cursors.Default;
            
        }

        private void LoadChapterDropDown()
        {
            chapterDropDown.Items.Clear();
            foreach (BDChapter entry in BDChapter.GetAll(dataContext))
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
            loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDSections.Count() <= 0);

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
            loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDSections.Count() <= 0);
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
            loadSeedDataButton.Visible = (null != lastSyncDate) && (dataContext.BDSections.Count() <= 0);

            this.Cursor = Cursors.Default;
#else
            MessageBox.Show(@"May not import in this environment" , "Import");
#endif
        }
    }
}
