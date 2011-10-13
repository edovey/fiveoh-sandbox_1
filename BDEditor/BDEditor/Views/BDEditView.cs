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
        Section = 1,
        Category = 2,
        SubCategory = 3,
        Disease = 4,
        Presentation = 5,
        Therapy = 6,
        Pathogen = 7
    }

    public partial class BDEditView : Form
    {
        BDEditor.DataModel.Entities dataContext;

        public BDEditView()
        {
            InitializeComponent();

            dataContext = new DataModel.Entities();

            LoadSectionDropDown();

            //sectionDropDown.DataSource = dataContext.BDSections;
            sectionDropDown.DisplayMember = "Name";
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

        public void rebuildTree(BDSection pSection)
        {
            sectionTree.Nodes.Clear();

            TreeNode rootNode = new TreeNode(pSection.name);
            List<BDCategory> categoryList = BDCategory.GetCategoriesForSectionId(dataContext, pSection.uuid);
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
                }

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
                }

                sectionTree.Nodes.Add(categoryNode);
            }
        }

        private void sectionDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BDSection section = sectionDropDown.SelectedValue as BDSection;
            BDSection section = sectionDropDown.SelectedItem as BDSection;
            if (null != section)
            {
                this.Cursor = Cursors.WaitCursor;
                rebuildTree(section);
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

        private void createTestData()
        {
            BDSection section = BDSection.CreateSection(dataContext);
            section.name = @"Selected Infection in Adult Patients";
            BDSection.SaveSection(dataContext, section);

            BDCategory category = BDCategory.CreateCategory(dataContext);
            category.name = @"Skin & Soft Tissue";
            category.sectionId = section.uuid;
            BDCategory.SaveCategory(dataContext, category);

            BDSubcategory subCategory = BDSubcategory.CreateSubcategory(dataContext);
            subCategory.name = @"Sub Category";
            subCategory.categoryId = category.uuid;
            BDSubcategory.SaveSubcategory(dataContext, subCategory);

            BDDisease disease = BDDisease.CreateDisease(dataContext);
            disease.name = @"Disease";
            disease.categoryId = category.uuid;
            BDDisease.SaveDisease(dataContext, disease);

            BDPresentation presentation = BDPresentation.CreatePresentation(dataContext);
            presentation.name = @"Presentation";
            presentation.diseaseId = disease.uuid;
            BDPresentation.SavePresentation(dataContext, presentation);

            category = BDCategory.CreateCategory(dataContext);
            category.name = @"Bone & Joint";
            category.sectionId = section.uuid;
            BDCategory.SaveCategory(dataContext, category);

            category = BDCategory.CreateCategory(dataContext);
            category.name = @"Respiratory";
            category.sectionId = section.uuid;
            BDCategory.SaveCategory(dataContext, category);

            disease = BDDisease.CreateDisease(dataContext);
            disease.name = @"Pharygtis";
            disease.categoryId = category.uuid;
            BDDisease.SaveDisease(dataContext, disease);

            presentation = BDPresentation.CreatePresentation(dataContext);
            presentation.name = @"Acute";
            presentation.diseaseId = disease.uuid;
            BDPresentation.SavePresentation(dataContext, presentation);

            presentation = BDPresentation.CreatePresentation(dataContext);
            presentation.name = @"Non-Responders";
            presentation.diseaseId = disease.uuid;
            BDPresentation.SavePresentation(dataContext, presentation);

            presentation = BDPresentation.CreatePresentation(dataContext);
            presentation.name = @"Late Relapse / Recurrent";
            presentation.diseaseId = disease.uuid;
            BDPresentation.SavePresentation(dataContext, presentation);

            dataContext.Refresh(System.Data.Objects.RefreshMode.StoreWins, dataContext.BDSections);
            sectionDropDown.DataSource = null;
            sectionDropDown.DataSource = dataContext.BDSections;
            sectionDropDown.SelectedIndex = -1;
        }

        private void createTestDataButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            BDDataLoader dataLoader = new BDDataLoader();
            dataLoader.ImportData(dataContext, @"Resources\BDEditorStructure.txt");

            LoadSectionDropDown();

            this.Cursor = Cursors.Default;
            
        }

        private void LoadSectionDropDown()
        {
            sectionDropDown.Items.Clear();
            foreach (BDSection section in dataContext.BDSections)
            {
                sectionDropDown.Items.Add(section);
            }
            if (sectionDropDown.Items.Count > 1)
                sectionDropDown.SelectedIndex = 1;
        }
    }
}
