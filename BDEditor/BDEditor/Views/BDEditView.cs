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

            sectionDropDown.DataSource = dataContext.BDSections;
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
                        subCategoryNode.Nodes.Add(diseaseNode);
                    }
                    subCategoryNode.Nodes.Add(new TreeNode(@"<Add Disease Entry>"));
                    categoryNode.Nodes.Add(subCategoryNode);
                }
                categoryNode.Nodes.Add(new TreeNode(@"<Add SubCategory Entry>"));

                List<BDDisease> categorydiseaseList = BDDisease.GetDiseasesForCategoryId(dataContext, category.uuid);
                foreach (BDDisease disease in categorydiseaseList)
                {
                    TreeNode diseaseNode = new TreeNode(disease.name);
                    diseaseNode.Tag = disease;
                    categoryNode.Nodes.Add(diseaseNode);
                }

                categoryNode.Nodes.Add(new TreeNode(@"<Add Disease Entry>"));

                sectionTree.Nodes.Add(categoryNode);
            }
            sectionTree.Nodes.Add(new TreeNode(@"<Add Category>"));

        }

        private void sectionDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            BDSection section = sectionDropDown.SelectedValue as BDSection;
            if (null != section)
            {
                rebuildTree(section);
            }

        }

        private void sectionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
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
                        splitContainer1.Panel2.Controls.Add(subCategoryControl);
                    }
                    else if (selectedNode.Tag is BDDisease)
                    {
                        BDDiseaseControl diseaseControl = new BDDiseaseControl();
                        diseaseControl.Dock = DockStyle.Fill;
                        diseaseControl.CurrentDisease = selectedNode.Tag as BDDisease;
                        splitContainer1.Panel2.Controls.Add(diseaseControl);
                    }
                    else if (selectedNode.Tag is BDPresentation)
                    {
                        BDPresentationControl presentationControl = new BDPresentationControl();
                        presentationControl.Dock = DockStyle.Fill;

                        splitContainer1.Panel2.Controls.Add(presentationControl);
                    }
                    break;
                case TreeViewAction.Collapse:
                case TreeViewAction.Expand:
                case TreeViewAction.Unknown:
                default:
                    break;
            }
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
            subCategory.name = @"All";
            subCategory.categoryId = category.uuid;
            BDSubcategory.SaveSubcategory(dataContext, subCategory);

            category = BDCategory.CreateCategory(dataContext);
            category.name = @"Respiratory";
            category.sectionId = section.uuid;
            BDCategory.SaveCategory(dataContext, category);

            BDDisease disease = BDDisease.CreateDisease(dataContext);
            disease.name = @"Pharygtis";
            disease.categoryId = category.uuid;
            BDDisease.SaveDisease(dataContext, disease);

            BDPresentation presentation = BDPresentation.CreatePresentation(dataContext);
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
        }

        private void createTestDataButton_Click(object sender, EventArgs e)
        {
            createTestData();
        }
    }
}
