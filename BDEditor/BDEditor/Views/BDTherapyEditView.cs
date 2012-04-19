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
    public partial class BDTherapyEditView : Form
    {
        private Entities dataContext;
        private string currentControlName;

        private const string NAME_TEXTBOX = "Name";
        private const string DOSAGE_TEXTBOX = "Dosage";
        private const string DOSAGE_1_TEXTBOX = "Dosage1";
        private const string DOSAGE_2_TEXTBOX = "Dosage2";
        private const string DURATION_TEXTBOX = "Duration";
        private const string DURATION_1_TEXTBOX = "Duration1";
        private const string DURATION_2_TEXTBOX = "Duration2";

        BDTherapy currentTherapy;
        
        public BDTherapyEditView()
        {
            InitializeComponent();
        }

        public BDTherapy CurrentTherapy
        {
            get { return currentTherapy; }
            set {currentTherapy = value; }
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        private void insertTextFromMenu(string textToInsert)
        {
            if (currentControlName == NAME_TEXTBOX)
            {
                tbName.Text = tbName.Text.Insert(tbName.SelectionStart, textToInsert);
                tbName.SelectionStart += textToInsert.Length;
            }
            else if (currentControlName == DOSAGE_TEXTBOX)
            {
                tbDosage.Text = tbName.Text.Insert(tbName.SelectionStart, textToInsert);
                tbDosage.SelectionStart += textToInsert.Length;
            }
            else if (currentControlName == DURATION_TEXTBOX)
            {
                tbDuration.Text = tbName.Text.Insert(tbName.SelectionStart, textToInsert);
                tbDuration.SelectionStart += textToInsert.Length;
            }
            else if (currentControlName == DOSAGE_1_TEXTBOX)
            {
                tbDosage1.Text = tbName.Text.Insert(tbName.SelectionStart, textToInsert);
                tbDosage1.SelectionStart += textToInsert.Length;
            }
            else if (currentControlName == DOSAGE_2_TEXTBOX)
            {
                tbDosage2.Text = tbName.Text.Insert(tbName.SelectionStart, textToInsert);
                tbDosage2.SelectionStart += textToInsert.Length;
            }
            else if (currentControlName == DURATION_1_TEXTBOX)
            {
                tbDuration1.Text = tbName.Text.Insert(tbName.SelectionStart, textToInsert);
                tbDuration1.SelectionStart += textToInsert.Length;
            }
            else if (currentControlName == DURATION_2_TEXTBOX)
            {
                tbDuration2.Text = tbName.Text.Insert(tbName.SelectionStart, textToInsert);
                tbDuration2.SelectionStart += textToInsert.Length;
            }
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "ß";
            insertTextFromMenu(newText);
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "°";
            insertTextFromMenu(newText);
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "µ";

            insertTextFromMenu(newText);
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≥";
            insertTextFromMenu(newText);
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≤";
            insertTextFromMenu(newText);
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "±";
            insertTextFromMenu(newText);
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "¹";
            insertTextFromMenu(newText);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            save();
            // TODO:  refresh tree click & view currently on editor

            this.Close();
        }

        private void BDTherapyEditView_Load(object sender, EventArgs e)
        {
            tbDosage.Text = currentTherapy.dosage;
            tbDuration.Text = currentTherapy.duration;
            tbName.Text = currentTherapy.name;
            if(!string.IsNullOrEmpty(currentTherapy.dosage1))
                tbDosage1.Text = currentTherapy.dosage1;
            if (!string.IsNullOrEmpty(currentTherapy.dosage2))
                tbDosage2.Text = currentTherapy.dosage2;
            if (!string.IsNullOrEmpty(currentTherapy.duration1))
                tbDuration1.Text = currentTherapy.duration1;
            if (!string.IsNullOrEmpty(currentTherapy.duration2))
                tbDuration2.Text = currentTherapy.duration1;
        }

        private bool save()
        {
            bool result = false;

            if (null != currentTherapy)
            {
                if (currentTherapy.name != tbName.Text) currentTherapy.name = tbName.Text.Trim();
                if (currentTherapy.dosage != tbDosage.Text) currentTherapy.dosage = tbDosage.Text.Trim();
                if (currentTherapy.duration != tbDuration.Text) currentTherapy.duration = tbDuration.Text.Trim();
                if (currentTherapy.dosage1 != tbDosage1.Text) currentTherapy.dosage1 = tbDosage1.Text.Trim();
                if (currentTherapy.dosage2 != tbDosage2.Text) currentTherapy.dosage2 = tbDosage2.Text.Trim();
                if (currentTherapy.duration1 != tbDuration1.Text) currentTherapy.duration1 = tbDuration1.Text.Trim();
                if (currentTherapy.duration2 != tbDuration2.Text) currentTherapy.duration2 = tbDuration2.Text.Trim();

                BDTherapy.Save(dataContext, currentTherapy);
                result = true;
            }

            return result;

        }

        private void tbName_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = NAME_TEXTBOX;
        }

        private void tbDosage_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_TEXTBOX;
        }

        private void tbDuration_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DURATION_TEXTBOX;
        }
        private void tbDosage1_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_1_TEXTBOX;
        }

        private void tbDosage2_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DOSAGE_2_TEXTBOX;
        }

        private void tbDuration1_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DURATION_1_TEXTBOX;
        }

        private void tbDuration2_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = DURATION_2_TEXTBOX;
        }
    }
}
