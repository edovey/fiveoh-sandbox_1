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
    public partial class BDLayoutMetadataEditor : Form
    {
        private BDLayoutMetadata selectedLayout = null;
        private BDLayoutMetadataColumn currentColumn = null;
        private bool isDisplaying = false;

        
        public Entities DataContext;

        public BDLayoutMetadataEditor()
        {
            InitializeComponent();
        }

        public BDLayoutMetadataEditor(Entities pDataContext)
        {
            InitializeComponent();
            DataContext = pDataContext;
        }

        private void BDLayoutMetadataEditor_Load(object sender, EventArgs e)
        {
            btnLinkedNote.Tag = BDLayoutMetadataColumn.PROPERTYNAME_LABEL;

            BDLayoutMetadata.Rebuild(DataContext);

            listBoxLayoutVariants.Items.Clear();
            List<BDLayoutMetadata> allEntries = BDLayoutMetadata.RetrieveAll(DataContext, null);
            listBoxLayoutVariants.Items.AddRange(allEntries.ToArray());
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBoxLayoutVariants_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxLayoutVariants.SelectedIndex >= 0)
            {
                BDLayoutMetadata entry = listBoxLayoutVariants.SelectedItem as BDLayoutMetadata;
                DisplayLayout(entry);
            }
        }

        private void DisplayLayout(BDLayoutMetadata pLayout)
        {
            isDisplaying = true;

            this.selectedLayout = pLayout;

            listBoxLayoutColumns.Items.Clear();
            DisplayLayoutColumn(null);

            if (null == this.selectedLayout)
            {
                lblSelectedLayout.Text = "";
                chkLayoutIncluded.Checked = false;
                DisplayLayoutColumn(null);
            }
            else
            {
                lblSelectedLayout.Text = this.selectedLayout.descrip;
                chkLayoutIncluded.Checked = this.selectedLayout.included;
                List<BDLayoutMetadataColumn> columnList = BDLayoutMetadataColumn.RetrieveForLayout(DataContext, pLayout);
                listBoxLayoutColumns.Items.AddRange(columnList.ToArray());
                listBoxLayoutColumns.SelectedIndex = -1;
            }

            isDisplaying = false;
        }

        private void DisplayLayoutColumn(BDLayoutMetadataColumn pLayoutColumn)
        {
            this.currentColumn = pLayoutColumn;

            listBoxColumnNodeTypes.Items.Clear();

            if (null == this.currentColumn)
            {
                txtColumnLabel.Text = "";
            }
            else
            {
                txtColumnLabel.Text = this.currentColumn.label;
                List<BDLayoutMetadataColumnNodeType> columnNodetypeList = BDLayoutMetadataColumnNodeType.RetrieveForLayoutColumn(DataContext, this.currentColumn.uuid);
                listBoxColumnNodeTypes.Items.AddRange(columnNodetypeList.ToArray());
            }

            ShowLinksInUse(false);
        }

        private void listBoxLayoutColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isDisplaying)
            {
                BDLayoutMetadataColumn selectedColumn = null;
                if (listBoxLayoutColumns.SelectedIndex >= 0)
                {
                    selectedColumn = listBoxLayoutColumns.SelectedItem as BDLayoutMetadataColumn;
                }
                DisplayLayoutColumn(selectedColumn);
            }
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            if (null != this.selectedLayout)
            {
                int displayOrder = listBoxLayoutColumns.Items.Count + 1;
                BDLayoutMetadataColumn column = BDLayoutMetadataColumn.Create(DataContext, this.selectedLayout, displayOrder);
                column.label = string.Format("Column {0}", displayOrder);
                DataContext.SaveChanges();

                int index = listBoxLayoutColumns.Items.Add(column);
                listBoxLayoutColumns.SelectedIndex = index;
            }
        }

        private void btnRemoveColumn_Click(object sender, EventArgs e)
        {
            int index = listBoxLayoutColumns.SelectedIndex;

            if ((null != this.selectedLayout) && (index >= 0))
            {
                BDLayoutMetadataColumn column = listBoxLayoutColumns.SelectedItem as BDLayoutMetadataColumn;
                if (null != column)
                {
                    listBoxLayoutColumns.Items.Remove(column);
                    DataContext.DeleteObject(column);
                    DisplayLayoutColumn(null);
                    for (int idx = 0; idx < listBoxLayoutColumns.Items.Count; idx++)
                    {
                        BDLayoutMetadataColumn entry = listBoxLayoutColumns.Items[idx] as BDLayoutMetadataColumn;
                        entry.displayOrder = idx + 1;
                    }
                    DataContext.SaveChanges();
                }
            }
        }

        private void txtColumnLabel_Leave(object sender, EventArgs e)
        {
            if (null != this.currentColumn)
            {
                this.currentColumn.label = txtColumnLabel.Text;
                DataContext.SaveChanges();
                int index = listBoxLayoutColumns.Items.IndexOf(this.currentColumn);
                listBoxLayoutColumns.Items[index] = this.currentColumn;
            }
        }

        private void chkLayoutIncluded_CheckedChanged(object sender, EventArgs e)
        {
            if ( (!isDisplaying) && (null != this.selectedLayout))
            {
                this.selectedLayout.included = chkLayoutIncluded.Checked;
                DataContext.SaveChanges();

                int index = listBoxLayoutVariants.Items.IndexOf(this.selectedLayout);
                listBoxLayoutVariants.Items[index] = this.selectedLayout;
            }
        }

        private void btnLinkedNote_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                string tag = control.Tag as string;
                CreateLink(tag);
            }
        }

        private void CreateLink(string pProperty)
        {
            if (null != this.currentColumn)
            {
                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(DataContext);
                view.AssignContextPropertyName(BDLayoutMetadataColumn.PROPERTYNAME_LABEL);
                view.AssignParentInfo(this.currentColumn.Uuid, BDConstants.BDNodeType.BDLayoutColumn);
                view.AssignScopeId(null);
                //view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                //view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(DataContext, (null != this.currentColumn) ? this.currentColumn.Uuid : Guid.Empty);
            btnLinkedNote.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnLinkedNote.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;

        }

        private void btnColumnNoteTypeSetup_Click(object sender, EventArgs e)
        {
            if (null != this.currentColumn)
            {
                BDLayoutColumnNodeTypeEditor editor = new BDLayoutColumnNodeTypeEditor(DataContext, this.currentColumn);
                editor.ShowDialog();
                List<BDLayoutMetadataColumnNodeType> list = BDLayoutMetadataColumnNodeType.RetrieveForLayoutColumn(DataContext, this.currentColumn.Uuid);
                listBoxColumnNodeTypes.Items.Clear();
                listBoxColumnNodeTypes.Items.AddRange(list.ToArray());
            }
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
                    DataContext.SaveChanges();
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
                    DataContext.SaveChanges();
                    listBoxLayoutColumns.SelectedItem = this.currentColumn;
                }
                this.isDisplaying = false;
            }
        }
    }
}
