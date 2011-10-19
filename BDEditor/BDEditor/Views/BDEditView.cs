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
using System.Diagnostics;

namespace BDEditor.Views
{
    public enum BDNodeContextType
    {
        Chapter = 1,
        Section = 2,
        Category = 3,
        SubCategory = 4,
        Disease = 5,
        Presentation = 6,
        Therapy = 7,
        Pathogen = 8
    }

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
        public void createEntry(BDNodeContextType context)
        {            
            switch (context)
            {
                case BDNodeContextType.Section:
                    BDSection section = BDSection.CreateSection(dataContext);
                    section.name = @"New Section";
                    BDSection.SaveSection(dataContext,section);
                    break;

                default:
                    break;
            }
        }

        public void rebuildTree(BDChapter pChapter)
        {
            chapterTree.Nodes.Clear();

            TreeNode rootNode = new TreeNode(pChapter.name);

            List<BDSection> sectionList = BDSection.GetSectionsForChapterId(dataContext, pChapter.uuid);

            foreach (BDSection section in sectionList)
            {
                TreeNode sectionNode = new TreeNode(section.name);
                sectionNode.Tag = section;

                List<BDCategory> categoryList = BDCategory.GetCategoriesForSectionId(dataContext, section.uuid);
                foreach (BDCategory category in categoryList)
                {
                    TreeNode categoryNode = new TreeNode(category.name);
                    categoryNode.Tag = category;

                    List<BDSubcategory> subCategoryList = BDSubcategory.GetSubcategoriesForCategoryId(dataContext, category.uuid);
                    foreach (BDSubcategory subCategory in subCategoryList)
                    {
                        TreeNode subCategoryNode = new TreeNode(subCategory.name);
                        subCategoryNode.Tag = subCategory;

                        List<BDDisease> diseaseList = BDDisease.GetDiseasesForSubcategory(dataContext, subCategory.uuid);
                        foreach (BDDisease disease in diseaseList)
                        {
                            TreeNode diseaseNode = new TreeNode(disease.name);
                            diseaseNode.Tag = disease;

                            List<BDPresentation> presentationList = BDPresentation.GetPresentationsForDiseaseId(dataContext, disease.uuid);
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

                    List<BDDisease> categorydiseaseList = BDDisease.GetDiseasesForCategoryId(dataContext, category.uuid);
                    foreach (BDDisease disease in categorydiseaseList)
                    {
                        TreeNode diseaseNode = new TreeNode(disease.name);
                        diseaseNode.Tag = disease;

                        List<BDPresentation> presentationList = BDPresentation.GetPresentationsForDiseaseId(dataContext, disease.uuid);
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
            } // section
        }

        private void listDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            //BDSection section = sectionDropDown.SelectedValue as BDSection;
            BDChapter entry = chapterDropDown.SelectedItem as BDChapter;
            if (null != entry)
            {
                this.Cursor = Cursors.WaitCursor;
                rebuildTree(entry);
                this.Cursor = Cursors.Default;
            }
        }

        private void sectionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            switch (e.Action)
            {
                case TreeViewAction.ByKeyboard:
                case TreeViewAction.ByMouse:
                    splitContainer1.Panel2.Controls.Clear();
                    TreeNode selectedNode = e.Node;
                    if(selectedNode.Tag is BDCategory)
                    {
                        BDCategoryControl categoryControl = new BDCategoryControl();
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
                            diseaseControl.AssignParentId(category.uuid);

                        splitContainer1.Panel2.Controls.Add(diseaseControl);
                    }
                    else if (selectedNode.Tag is BDPresentation)
                    {
                        BDPresentationControl presentationControl = new BDPresentationControl();
                        presentationControl.AssignDataContext(dataContext);
                        presentationControl.Dock = DockStyle.Fill;
                        presentationControl.CurrentPresentation = selectedNode.Tag as BDPresentation;
                        BDDisease disease = selectedNode.Parent.Tag as BDDisease;
                        if (null != disease)
                        {
                            presentationControl.AssignParentId(disease.uuid);
                        }

                        splitContainer1.Panel2.Controls.Add(presentationControl);
                    }
                    break;
                case TreeViewAction.Collapse:
                case TreeViewAction.Expand:
                case TreeViewAction.Unknown:
                default:
                    break;
            }
            this.Cursor = Cursors.Default;
        }

  
        private void createTestDataButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            BDDataLoader dataLoader = new BDDataLoader();
            dataLoader.ImportData(dataContext, @"Resources\BDEditorStructure.txt");

            LoadChapterDropDown();

            DateTime? lastSyncDate = BDSystemSetting.GetTimestamp(DataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            createTestDataButton.Visible = (null != lastSyncDate) && (dataContext.BDSections.Count() <= 0);

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
            DateTime? lastSyncDate = BDSystemSetting.GetTimestamp(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
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
            DateTime? lastSyncDate = BDSystemSetting.GetTimestamp(DataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);

            SyncInfoDictionary syncResultList = RepositoryHandler.Aws.Sync(DataContext, lastSyncDate);

            foreach (SyncInfo syncInfo in syncResultList.Values)
            {
                System.Diagnostics.Debug.WriteLine(syncInfo.EntityName);
            }

            UpdateSyncLabel();
            LoadChapterDropDown();

            lastSyncDate = BDSystemSetting.GetTimestamp(DataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            createTestDataButton.Visible = (null != lastSyncDate) && (dataContext.BDSections.Count() <= 0);

            this.Cursor = Cursors.Default;
        }

        private void BDEditView_Load(object sender, EventArgs e)
        {
            DateTime? lastSyncDate = BDSystemSetting.GetTimestamp(DataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
            createTestDataButton.Visible = (null != lastSyncDate) && (dataContext.BDSections.Count() <= 0);
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

                BDSystemSetting.SaveTimestamp(dataContext, BDSystemSetting.LASTSYNC_TIMESTAMP, null);
                this.Cursor = Cursors.Default;

                SyncData();          
            }
        }
    }
}
