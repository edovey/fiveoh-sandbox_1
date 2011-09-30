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
        BDEditor.DataModel.Entities entities;

        public BDEditView()
        {
            InitializeComponent();

            entities = new DataModel.Entities();

            sectionDropDown.DataSource = entities.BDSections;
            sectionDropDown.DisplayMember = "Name";
        }

        public void createEntry(BDNodeContextType context)
        {            
            switch (context)
            {
                case BDNodeContextType.Section:
                    BDSection section = BDSection.CreateSection();
                    section.name = @"New Section";
                    BDSection.SaveSection(section);
                    break;

                default:
                    break;
            }
        }

        public void rebuildTree(BDSection pSection)
        {
            sectionTree.Nodes.Clear();

            TreeNode rootNode = new TreeNode(pSection.name);
            List<BDCategory> categoryList = BDCategory.GetCategoriesForSectionId(pSection.uuid);
            foreach (BDCategory category in categoryList)
            {
                TreeNode categoryNode = new TreeNode(category.name);
                categoryNode.Tag = category;

                List<BDSubcategory> subCategoryList = BDSubcategory.GetSubcategoriesForCategoryId(category.uuid);
                foreach (BDSubcategory subCategory in subCategoryList)
                {
                    TreeNode subCategoryNode = new TreeNode(subCategory.name);
                    subCategoryNode.Tag = subCategory;

                    List<BDDisease> diseaseList = BDDisease.GetDiseasesForSubcategory(subCategory.uuid);
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

                List<BDDisease> categorydiseaseList = BDDisease.GetDiseasesForCategoryId(category.uuid);
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
                    if (selectedNode.Tag is BDDisease)
                    {
                        BDDiseaseControl diseaseControl = new BDDiseaseControl();
                        diseaseControl.Dock = DockStyle.Fill;
                        diseaseControl.CurrentDisease = selectedNode.Tag as BDDisease;
                        splitContainer1.Panel2.Controls.Add(diseaseControl);
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
            BDSection section = BDSection.CreateSection();
            section.name = @"Selected Infection in Adult Patients";
            BDSection.SaveSection(section);

            BDCategory category = BDCategory.CreateCategory();
            category.name = @"Skin & Soft Tissue";
            category.sectionId = section.uuid;
            BDCategory.SaveCategory(category);

            BDSubcategory subCategory = BDSubcategory.CreateSubcategory();
            subCategory.name = @"All";
            subCategory.categoryId = category.uuid;
            BDSubcategory.SaveSubcategory(subCategory);

            category = BDCategory.CreateCategory();
            category.name = @"Respiratory";
            category.sectionId = section.uuid;
            BDCategory.SaveCategory(category);

            BDDisease disease = BDDisease.CreateDisease();
            disease.name = @"Pharygtis";
            disease.categoryId = category.uuid;
            BDDisease.SaveDisease(disease);

            BDPresentation presentation = BDPresentation.CreatePresentation();
            presentation.name = @"Acute";
            presentation.diseaseId = disease.uuid;
            BDPresentation.SavePresentation(presentation);

            presentation = BDPresentation.CreatePresentation();
            presentation.name = @"Non-Responders";
            presentation.diseaseId = disease.uuid;
            BDPresentation.SavePresentation(presentation);

            presentation = BDPresentation.CreatePresentation();
            presentation.name = @"Late Relapse / Recurrent";
            presentation.diseaseId = disease.uuid;
            BDPresentation.SavePresentation(presentation);
        }

        private void createTestDataButton_Click(object sender, EventArgs e)
        {
            createTestData();
        }
    }
}
