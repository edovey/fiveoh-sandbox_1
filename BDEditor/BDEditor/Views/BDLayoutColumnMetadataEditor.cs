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
    public partial class BDLayoutColumnMetadataEditor : Form
    {
        private BDLayoutMetadata selectedLayout = null;
        private BDLayoutMetadataColumn currentColumn = null;
        private bool isDisplaying = false;
        private Guid scopeId = Guid.Empty;
        public Entities dataContext;

        public BDLayoutColumnMetadataEditor()
        {
            InitializeComponent();
        }

        private void BDLayoutColumnMetadataEditor_Load(object sender, EventArgs e)
        {
            btnLinkedNote.Tag = BDLayoutMetadataColumn.PROPERTYNAME_LABEL;
            if (selectedLayout != null)
                lblSelectedLayout.Text = selectedLayout.descrip;
            List<BDLayoutMetadataColumn> columnList = BDLayoutMetadataColumn.RetrieveListForLayout(dataContext, selectedLayout);
            listBoxLayoutColumns.Items.AddRange(columnList.ToArray());
            listBoxLayoutColumns.SelectedIndex = -1;
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void AssignCurrentLayout(BDConstants.LayoutVariantType pLayout)
        {
            selectedLayout = BDLayoutMetadata.Retrieve(dataContext,pLayout);
        }

        private void createLink(string pProperty)
        {
            if (null != this.currentColumn)
            {
                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(BDLayoutMetadataColumn.PROPERTYNAME_LABEL);
                view.AssignParentInfo(this.currentColumn.Uuid, BDConstants.BDNodeType.BDLayoutColumn);
                view.AssignScopeId(scopeId);
                view.ShowDialog(this);
                ShowLinksInUse(false);
            }
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentColumn) ? this.currentColumn.Uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
        }

        private void displayLayoutColumn(BDLayoutMetadataColumn pLayoutColumn)
        {
            this.currentColumn = pLayoutColumn;

            if (null == this.currentColumn)
            {
                txtColumnLabel.Text = "";
            }
            else
            {
                txtColumnLabel.Text = this.currentColumn.label;
                List<BDLayoutMetadataColumnNodeType> columnNodetypeList = BDLayoutMetadataColumnNodeType.RetrieveListForLayoutColumn(dataContext, this.currentColumn.uuid);
            }

            ShowLinksInUse(false);
        }

        private void btnMoveColumnPrevious_Click(object sender, EventArgs e)
        {
            if (null != this.currentColumn)
            {
                this.isDisplaying = true;
                int index = listBoxLayoutColumns.Items.IndexOf(this.currentColumn);
                if (index > 0)
                {
                    listBoxLayoutColumns.Items.Remove(this.currentColumn);
                    listBoxLayoutColumns.Items.Insert(index - 1, this.currentColumn);
                    for (int idx = 0; idx < listBoxLayoutColumns.Items.Count; idx++)
                    {
                        BDLayoutMetadataColumn entry = listBoxLayoutColumns.Items[idx] as BDLayoutMetadataColumn;
                        entry.displayOrder = idx + 1;
                    }
                    dataContext.SaveChanges();
                    listBoxLayoutColumns.SelectedItem = this.currentColumn;
                }
                this.isDisplaying = false;
            }
        }

        private void btnMoveColumnNext_Click(object sender, EventArgs e)
        {
            if (null != this.currentColumn)
            {
                this.isDisplaying = true;
                int index = listBoxLayoutColumns.Items.IndexOf(this.currentColumn);
                if (index < listBoxLayoutColumns.Items.Count - 1)
                {
                    listBoxLayoutColumns.Items.Remove(this.currentColumn);
                    listBoxLayoutColumns.Items.Insert(index + 1, this.currentColumn);
                    for (int idx = 0; idx < listBoxLayoutColumns.Items.Count; idx++)
                    {
                        BDLayoutMetadataColumn entry = listBoxLayoutColumns.Items[idx] as BDLayoutMetadataColumn;
                        entry.displayOrder = idx + 1;
                    }
                    dataContext.SaveChanges();
                    listBoxLayoutColumns.SelectedItem = this.currentColumn;
                }
                this.isDisplaying = false;
            }
        }

        private void btnLinkedNote_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                string tag = control.Tag as string;
                createLink(tag);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBoxLayoutColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            BDLayoutMetadataColumn selectedColumn = null;
            if (listBoxLayoutColumns.SelectedIndex >= 0)
            {
                selectedColumn = listBoxLayoutColumns.SelectedItem as BDLayoutMetadataColumn;
            }
            displayLayoutColumn(selectedColumn);
        }

        private void txtColumnLabel_Leave(object sender, EventArgs e)
        {
            if (null != this.currentColumn)
            {
                this.currentColumn.label = txtColumnLabel.Text;
                dataContext.SaveChanges();
                int index = listBoxLayoutColumns.Items.IndexOf(this.currentColumn);
                listBoxLayoutColumns.Items[index] = this.currentColumn;
            }

        }

        private void insertTextFromMenu(string textToInsert)
        {

            int position = txtColumnLabel.SelectionStart;
            txtColumnLabel.Text = txtColumnLabel.Text.Insert(txtColumnLabel.SelectionStart, textToInsert);
            txtColumnLabel.SelectionStart = textToInsert.Length + position;
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

        private void sTwoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "²";
            insertTextFromMenu(newText);
        }

        private void trademarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "®";
            insertTextFromMenu(newText);
        }
    }
}
