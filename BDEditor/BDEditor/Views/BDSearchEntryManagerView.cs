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
    public partial class BDSearchEntryManagerView : Form
    {
        private Entities dataContext;
        private BDSearchEntry currentSearchEntry;
        private BDSearchEntryAssociation currentSearchEntryAssociation;

        private List<BDSearchEntry> allSearchEntries;
        private BDSearchEntryBindingList searchEntries;
        private BDSearchEntryAssociationBindingList searchEntryAssociations;
        private List<BDSearchEntry> filteredSearchEntryList;

        //private List<BDSearchEntry> entriesToDelete;
        //private List<BDSearchEntryAssociation> associationsToDelete;

        private bool formHasChanges = false;

        string editorContext = string.Empty;

        public BDSearchEntryManagerView()
        {
            InitializeComponent();
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public string DisplayContext
        {
            get { return editorContext; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    editorContext = value;
            }
        }

        private void BDSearchEntryManagerView_Load(object sender, EventArgs e)
        {
            lbExistingSearchEntries.BeginUpdate();

            allSearchEntries = BDSearchEntry.RetrieveAll(dataContext);
            searchEntryAssociations = new BDSearchEntryAssociationBindingList();
            searchEntries = new BDSearchEntryBindingList();

            reloadAvailableEntries();

            lbSearchEntryAssociations.DataSource = searchEntryAssociations;
            lbSearchEntryAssociations.DisplayMember = "editorContext";

            lbExistingSearchEntries.ClearSelected();
            lbExistingSearchEntries.EndUpdate();
        }

        private void BDSearchEntryManagerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            allSearchEntries.Clear();
            searchEntryAssociations.Clear();
            searchEntries.Clear();
        }

        private void reloadAvailableEntries()
        {
            searchEntries = new BDSearchEntryBindingList(allSearchEntries);
            lbExistingSearchEntries.DataSource = searchEntries;
        }

        private void reloadAssociatedLocations()
        {
            BDSearchEntryAssociation selectedAssn = lbSearchEntryAssociations.SelectedItems.Count > 0 ? (BDSearchEntryAssociation)lbSearchEntryAssociations.SelectedItem : null;
            lbSearchEntryAssociations.BeginUpdate();
            lbSearchEntryAssociations.ClearSelected();
            searchEntryAssociations.Clear();

            if (null == currentSearchEntry && lbExistingSearchEntries.SelectedItems.Count > 0)
            {
                currentSearchEntry = lbExistingSearchEntries.SelectedItems[0] as BDSearchEntry;
            }

            if (null != currentSearchEntry)
            {
                currentSearchEntryAssociation = null;
                List<BDSearchEntryAssociation> tmpList = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, currentSearchEntry.Uuid);

                foreach (BDSearchEntryAssociation nodeAssn in tmpList)
                {
                    searchEntryAssociations.Add(nodeAssn);
                    nodeAssn.displayOrder = searchEntryAssociations.IndexOf(nodeAssn);
                    // repair any missing data : needed for converting from fully generated associations to managed associations
                    if (string.IsNullOrEmpty(nodeAssn.editorContext))
                        nodeAssn.editorContext = nodeAssn.displayContext;

                    if (nodeAssn.editorContext.IndexOf("*") == 0)
                        nodeAssn.editorContext = nodeAssn.editorContext.Substring(1);

                    BDSearchEntryAssociation.Save(dataContext, nodeAssn);
                }
                if (null != selectedAssn && tmpList.Contains(selectedAssn))
                    lbSearchEntryAssociations.SetSelected(searchEntryAssociations.IndexOf(selectedAssn), true);
            }

            lbSearchEntryAssociations.EndUpdate();

            resetButtons();
        }

        private void resetButtons()
        {
            btnMoveAssnNext.Enabled = (lbSearchEntryAssociations.SelectedItems.Count > 0 && searchEntryAssociations.Count > 1) ? true : false;
            btnMoveAssnPrevious.Enabled = (lbSearchEntryAssociations.SelectedItems.Count > 0 && searchEntryAssociations.Count > 1) ? true : false;

            cbFilterList.Enabled = tbEntryName.TextLength > 0 ? true : false;

            btnDeleteAssociation.Enabled = currentSearchEntryAssociation != null ? true : false;

            btnDeleteSearchEntry.Enabled = (lbExistingSearchEntries.SelectedIndices.Count > 0) ? true : false;
            btnEditSearchEntry.Enabled = (lbExistingSearchEntries.SelectedIndices.Count > 0) ? true : false;
        }

        private void btnDeleteSearchEntry_Click(object sender, EventArgs e)
        {
            if (lbExistingSearchEntries.SelectedItems.Count > 0)
            {
                DialogResult confirm = MessageBox.Show("This will delete the selected search entries and all existing links.  Are you sure?", "Confirm Delete", MessageBoxButtons.OKCancel);
                if (confirm == DialogResult.OK)
                {
                    List<BDSearchEntry> selectedSearchEntries = new List<BDSearchEntry>();
                    selectedSearchEntries.AddRange(lbExistingSearchEntries.SelectedItems.Cast<BDSearchEntry>());
                    foreach (BDSearchEntry entry in selectedSearchEntries)
                    {

                        List<BDSearchEntryAssociation> assns = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, entry.Uuid);
                        foreach (BDSearchEntryAssociation assn in assns)
                            BDSearchEntryAssociation.Delete(dataContext, assn);

                        allSearchEntries.Remove(entry);
                        BDSearchEntry.Delete(dataContext, entry, false);
                        currentSearchEntry = null;
                        currentSearchEntryAssociation = null;

                        reloadAvailableEntries();
                        reloadAssociatedLocations();

                        formHasChanges = true;
                        resetButtons();
                    }
                }
            }
        }

        private void btnAddNewSearchEntry_Click(object sender, EventArgs e)
        {
            using (BDEditNameDialog editDialog = new BDEditNameDialog())
            {
                DialogResult result = editDialog.ShowDialog();
                string newName = editDialog.IndexEntryName;
                if (result == DialogResult.OK && !string.IsNullOrEmpty(newName))
                {
                    BDSearchEntry existingMatch = BDSearchEntry.RetrieveWithName(dataContext, newName);
                    if (null == existingMatch)
                    {
                        BDSearchEntry newEntry = BDSearchEntry.CreateBDSearchEntry(dataContext, newName);
                        allSearchEntries.Add(newEntry);
                        reloadAvailableEntries();
                        searchEntries.Sort("name", ListSortDirection.Ascending);

                        currentSearchEntry = newEntry;
                        lbExistingSearchEntries.SetSelected(searchEntries.IndexOf(newEntry), true);
                        reloadAssociatedLocations();

                        formHasChanges = true;
                    }
                    else
                        MessageBox.Show("An Index Entry with that name already exists");
                }
            }
        }

        private void tbEntryName_TextChanged(object sender, EventArgs e)
        {
            resetButtons();
        }

        private void cbFilterList_CheckedChanged(object sender, EventArgs e)
        {
            lbExistingSearchEntries.BeginUpdate();
            currentSearchEntry = null;
            currentSearchEntryAssociation = null;
            lbSearchEntryAssociations.ClearSelected();
            lbExistingSearchEntries.ClearSelected();

            CheckBox cb = sender as CheckBox;
            if (null != cb && cb.CheckState == CheckState.Checked)
            {
                string searchTerm = tbEntryName.Text;
                if (!string.IsNullOrEmpty(tbEntryName.Text))
                {
                    filteredSearchEntryList = searchEntries.Where(x => x.name.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) != -1).ToList<BDSearchEntry>();
                    if (filteredSearchEntryList.Count <= 0)
                    {
                        MessageBox.Show("No entries found with that name");
                        cbFilterList.CheckState = CheckState.Unchecked;
                        reloadAvailableEntries();
                    }
                    else
                    {
                        BDSearchEntryBindingList filteredEntries = new BDSearchEntryBindingList(filteredSearchEntryList);
                        lbExistingSearchEntries.DataSource = filteredEntries;
                    }
                }
            }
            else
            {
                resetButtons();

                tbEntryName.Text = string.Empty;
                cbFilterList.Enabled = false;
            }
            lbExistingSearchEntries.EndUpdate();
        }

        private void btnEditSearchEntry_Click(object sender, EventArgs e)
        {
            BDSearchEntry selected = lbExistingSearchEntries.SelectedItems[0] as BDSearchEntry;
            using (BDEditNameDialog editDialog = new BDEditNameDialog())
            {
                editDialog.IndexEntryName = selected.name;
                DialogResult result = editDialog.ShowDialog();
                string newName = editDialog.IndexEntryName;
                if (result == DialogResult.OK && !string.IsNullOrEmpty(newName))
                {
                    BDSearchEntry existingMatch = BDSearchEntry.RetrieveWithName(dataContext, newName);
                    if (null == existingMatch || existingMatch.name != newName)
                    {
                        selected.name = newName;
                        BDSearchEntry.Save(dataContext, selected);
                        reloadAssociatedLocations();

                        searchEntries.Sort("name", ListSortDirection.Ascending);
                        formHasChanges = true;
                    }
                    else
                        MessageBox.Show("An entry with that name already exists.  Change was not saved.");
                }
            }
        }

        private void btnMoveAssnPrevious_Click(object sender, EventArgs e)
        {
            int selectedPosition = lbSearchEntryAssociations.SelectedIndex;
            int requestedPosition = selectedPosition - 1;

            if (requestedPosition >= 0)
            {
                searchEntryAssociations[requestedPosition].displayOrder = selectedPosition;
                searchEntryAssociations[selectedPosition].displayOrder = requestedPosition;

                BDSearchEntryAssociation.Save(dataContext, searchEntryAssociations[requestedPosition]);
                BDSearchEntryAssociation.Save(dataContext, searchEntryAssociations[selectedPosition]);

                searchEntryAssociations.Sort("displayOrder", ListSortDirection.Ascending);
                lbSearchEntryAssociations.BeginUpdate();
                lbSearchEntryAssociations.ClearSelected();
                lbSearchEntryAssociations.SetSelected(requestedPosition, true);
                lbSearchEntryAssociations.EndUpdate();

                formHasChanges = true;
                resetButtons();
            }
            btnOk.Focus();
        }

        private void btnMoveAssnNext_Click(object sender, EventArgs e)
        {
            int selectedPosition = lbSearchEntryAssociations.SelectedIndex;
            int requestedPosition = selectedPosition + 1;

            if (requestedPosition <= (searchEntryAssociations.Count - 1))
            {
                searchEntryAssociations[requestedPosition].displayOrder = selectedPosition;
                searchEntryAssociations[selectedPosition].displayOrder = requestedPosition;

                BDSearchEntryAssociation.Save(dataContext, searchEntryAssociations[requestedPosition]);
                BDSearchEntryAssociation.Save(dataContext, searchEntryAssociations[selectedPosition]);
                searchEntryAssociations.Sort("displayOrder", ListSortDirection.Ascending);

                lbSearchEntryAssociations.BeginUpdate();
                lbSearchEntryAssociations.ClearSelected();
                lbSearchEntryAssociations.SetSelected(requestedPosition, true);
                lbSearchEntryAssociations.EndUpdate();

                formHasChanges = true;
                resetButtons();
            }
            btnOk.Focus();
        }

        private void btnDeleteAssociation_Click(object sender, EventArgs e)
        {
            List<BDSearchEntryAssociation> associationsToDelete = new List<BDSearchEntryAssociation>();
            int index = lbSearchEntryAssociations.SelectedIndices[0];
            if (index >= 0)
            {
                associationsToDelete.AddRange(lbSearchEntryAssociations.SelectedItems.Cast<BDSearchEntryAssociation>());

                if (MessageBox.Show(string.Format("This will delete the selected search link.  Are you sure?"), "Confirm Delete",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    foreach (BDSearchEntryAssociation entry in associationsToDelete)
                    {
                        searchEntryAssociations.Remove(entry);
                        BDSearchEntryAssociation.Delete(dataContext, entry);
                    }
                    formHasChanges = true;
                    resetButtons();
                }
            }
            // renumber the display order
            foreach (BDSearchEntryAssociation nodeAssn in searchEntryAssociations)
            {
                nodeAssn.displayOrder = searchEntryAssociations.IndexOf(nodeAssn);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (currentSearchEntryAssociation != null && currentSearchEntryAssociation.editorContext.IndexOf("*") == 0)
            {
                currentSearchEntryAssociation.editorContext = currentSearchEntryAssociation.editorContext.Substring(1);
                BDSearchEntryAssociation.Save(dataContext, currentSearchEntryAssociation);
            }
            this.DialogResult = DialogResult.OK;
        }

        private void lbExistingIndexEntries_MouseDown(object sender, MouseEventArgs e)
        {
            lbSearchEntryAssociations.ClearSelected();
            currentSearchEntry = null;

            if (lbExistingSearchEntries.SelectedIndices.Count > 0)
            {
                currentSearchEntry = lbExistingSearchEntries.SelectedItems[0] as BDSearchEntry;
                reloadAssociatedLocations();
            }
            resetButtons();
        }

        private void lbSearchEntryAssociations_MouseDown(object sender, MouseEventArgs e)
        {
            if (lbSearchEntryAssociations.SelectedItems.Count > 0)
                currentSearchEntryAssociation = lbSearchEntryAssociations.SelectedItems[0] as BDSearchEntryAssociation;
            resetButtons();
        }

        private void tbEntryName_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbFilterList.CheckState != CheckState.Unchecked)
                cbFilterList.CheckState = CheckState.Unchecked;
        }
    }
}

