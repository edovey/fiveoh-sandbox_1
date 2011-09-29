using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using BDEditor.DataModel;
using BDEditor.Properties;
using BDEditor.Views;


namespace BDEditor
{
    public partial class Presentation : Form
    {
        public Presentation()
        {
            InitializeComponent();
             entities = new DataModel.Entities();

             comboBox1.DataSource = entities.BDPresentations;
             comboBox1.DisplayMember = "Name";

             cbSection.DataSource = entities.BDSections;
             cbSection.DisplayMember = "Name";
        }

        BDEditor.DataModel.Entities entities;
        BDPresentation selectedPresentation = null;
        BDSection selectedSection = null;
        BDCategory selectedCategory = null;

        private void btnExit_Click(object sender, EventArgs e)
        {
            // quit application
            Application.Exit();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (selectedPresentation == null)
            {
                Guid pGuid = System.Guid.NewGuid();

                selectedPresentation = BDPresentation.CreateBDPresentation(pGuid, false);
                entities.AddObject("BDPresentations", selectedPresentation);
            }

            selectedPresentation.name = tbName.Text;
            //selectedPresentation.overview = rtbOverview.Text;

            if (tbPathogen1.Text.Length > 0)
            // create new pathogen and set properties
            {
                BDPathogen pGen = BDPathogen.CreateBDPathogen(Guid.NewGuid(), false);
                pGen.name = tbPathogen1.Text;
                entities.AddObject("BDPathogens", pGen);
            }

            entities.SaveChanges();
           
        }       

        private void Presentation_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            BDPresentation p = (BDPresentation)this.comboBox1.SelectedItem;

            tbName.Text = p.name;
            rtbOverview.Text = p.overview;
            selectedPresentation = p;
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                Guid presentationId = selectedPresentation.uuid;
                string queryString = @"SELECT VALUE pathogen FROM Entities.BDPathogens AS pathogen";

                ObjectQuery<BDPathogen> pQuery = new ObjectQuery<BDPathogen>(queryString, context);
                pQuery.Where("presentationId = @presentationId");
                pQuery.Parameters.Add(new ObjectParameter("presentationId", presentationId));
            
            foreach (BDPathogen result in pQuery)
            {
                tbPathogen1.Text = tbPathogen1.Text + ":" + result.name;
            }
        }
    }

        private void rtbOverview_TextChanged(object sender, EventArgs e)
        {
            selectedPresentation.overview = (sender as RichTextBox).Text;
        }

        private void btnAddSection_Click(object sender, EventArgs e)
        {
            BDSection newSection = BDSection.CreateSection();
            newSection.name = tbSectionName.Text;
            selectedSection = newSection;
        }

        private void btnSaveSection_Click(object sender, EventArgs e)
        {
            selectedSection.name = tbSectionName.Text;
            BDSection.SaveSection(selectedSection);
            selectedSection = null;
            tbSectionName.Text = @"";
        }

        private void cbSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSection = (BDSection)this.cbSection.SelectedItem;
            this.listBox1.DataSource = BDCategory.GetCategoriesForSectionId(selectedSection.uuid);
            this.listBox1.DisplayMember = @"Name";
        }

        private void btnNewCategory_Click(object sender, EventArgs e)
        {
            BDCategory category = BDCategory.CreateCategory();
            category.name = tbCategoryName.Text;
            selectedCategory = category;
        }

        private void tbCategoryName_TextChanged(object sender, EventArgs e)
        {
            if (tbCategoryName.Text.Length > 0)
                this.btnNewCategory.Enabled = true;
            else
                this.btnNewCategory.Enabled = false;
        }

        private void btnSaveCategory_Click(object sender, EventArgs e)
        {
            selectedCategory.name = tbCategoryName.Text;
            selectedCategory.sectionId = selectedSection.uuid;
            BDCategory.SaveCategory(selectedCategory);
            selectedCategory = null;
            tbCategoryName.Text = @"";
           // this.listBox1.DataSource = BDCategory.GetCategoriesForSectionId(selectedSection.uuid);

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCategory = (BDCategory)this.listBox1.SelectedItem;
            tbCategoryName.Text = selectedCategory.name;
            btnNewCategory.Enabled = false;
            btnSaveCategory.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BDEditView view = new BDEditView();
            view.Show();
        }

    }
}
