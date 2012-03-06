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
        private const string DURATION_TEXTBOX = "Duration";

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

        private void insertTextFromMenu(TextBox textbox, string textToInsert, int selectionStart)
        {
            textbox.Text = textbox.Text.Insert(selectionStart, textToInsert);
            textbox.SelectionStart = selectionStart + 1;
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "ß";
            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "°";
            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "µ";

            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≥";

            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≤";

            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "±";

            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "¹";
            if (currentControlName == NAME_TEXTBOX)
                insertTextFromMenu(tbName, newText, tbName.SelectionStart);
            else if (currentControlName == DOSAGE_TEXTBOX)
                insertTextFromMenu(tbDosage, newText, tbDosage.SelectionStart);
            else if (currentControlName == DURATION_TEXTBOX)
                insertTextFromMenu(tbDuration, newText, tbDuration.SelectionStart);
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
        }

        private bool save()
        {
            bool result = false;

            if (null != currentTherapy)
            {
                if (currentTherapy.name != tbName.Text) currentTherapy.name = tbName.Text.Trim();
                if (currentTherapy.dosage != tbDosage.Text) currentTherapy.dosage = tbDosage.Text.Trim();
                if (currentTherapy.duration != tbDuration.Text) currentTherapy.duration = tbDuration.Text.Trim();
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
    }
}
