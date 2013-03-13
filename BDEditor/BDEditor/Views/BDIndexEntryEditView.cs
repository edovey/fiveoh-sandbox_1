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
    public partial class BDIndexEntryEditView : Form
    {
        private Entities dataContext;
        IBDNode currentNode;
        BDSearchEntry currentSearchEntry;
        List<BDSearchEntry> allSearchEntries;
        BDSearchEntryBindingList selectedSearchEntries;
        BDSearchEntryBindingList availableSearchEntries;
        BDSearchEntryAssociationBindingList searchEntryAssociations;
        bool formHasChanges = false;

        string displayContext = string.Empty;

        public BDIndexEntryEditView()
        {
            InitializeComponent();
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public string DisplayContext
        {
            get { return displayContext; }
            set { 
                if(!string.IsNullOrEmpty(value))
                    displayContext = value; 
            }
        }

        public void AssignCurrentNode(IBDNode pCurrentNode)
        {
            currentNode = pCurrentNode;
        }

        private void IndexEntryEditView_Load(object sender, EventArgs e)
        {
            allSearchEntries = BDSearchEntry.RetrieveAll(dataContext);
            searchEntryAssociations = new BDSearchEntryAssociationBindingList();

            selectedSearchEntries = new BDSearchEntryBindingList(BDSearchEntry.RetrieveSearchEntriesForDisplayParent(dataContext, currentNode.Uuid));
            List<BDSearchEntry> tmpEntry = new List<BDSearchEntry>(allSearchEntries);
            foreach (BDSearchEntry entry in selectedSearchEntries)
                tmpEntry.Remove(entry);
            availableSearchEntries = new BDSearchEntryBindingList(tmpEntry);

            availableSearchEntries.Sort("name", ListSortDirection.Ascending);
            selectedSearchEntries.Sort("name", ListSortDirection.Ascending);

            lblSelectedSearchEntry.Text = string.Empty;
            lblName.Text = !string.IsNullOrEmpty(currentNode.Name) ? currentNode.Name : "<intentionally blank>";

            lbExistingIndexEntries.DataSource = availableSearchEntries;
            lbSelectedIndexEntries.DataSource = selectedSearchEntries;
            lbIndexEntryAssociations.DataSource = searchEntryAssociations;
        }

        private void reloadAssociatedLocations()
        {
            this.SuspendLayout();
            lbIndexEntryAssociations.BeginUpdate();
            searchEntryAssociations.Clear();

            if (null != currentSearchEntry)
            {
                lblSelectedSearchEntry.Text = currentSearchEntry.name;
                List<BDSearchEntryAssociation> tmpList = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, currentSearchEntry.Uuid);
                foreach (BDSearchEntryAssociation assn in tmpList)
                    searchEntryAssociations.Add(assn);
            }
            lbIndexEntryAssociations.EndUpdate();

            resetButtons();
        }

        private void resetButtons()
        {
            btnMoveAssnNext.Enabled = (lbIndexEntryAssociations.SelectedItems.Count > 0  && searchEntryAssociations.Count > 1)? true : false;
            btnMoveAssnPrevious.Enabled = (lbIndexEntryAssociations.SelectedItems.Count > 0 && searchEntryAssociations.Count > 1) ? true : false;

            cbFilterList.Enabled = tbEntryName.TextLength > 0 ? true : false;

            //btnDeleteAssociation.Enabled = lbIndexEntryAssociations.SelectedItems.Count > 0 ? true : false;

            btnRemoveFromSelected.Enabled = lbSelectedIndexEntries.SelectedIndices.Count > 0 ? true : false;
            btnAddToSelected.Enabled = lbExistingIndexEntries.SelectedIndices.Count > 0 ? true : false;

            //btnDeleteSearchEntry.Enabled = (lbExistingIndexEntries.SelectedIndices.Count > 0 || lbSelectedIndexEntries.SelectedIndices.Count > 0) ? true : false;
            btnEditSearchEntry.Enabled = (lbExistingIndexEntries.SelectedIndices.Count > 0 || lbSelectedIndexEntries.SelectedIndices.Count > 0) ? true : false;

            btnOk.Enabled = formHasChanges;
        }

        private void btnAddToSelected_Click(object sender, EventArgs e)
        {
            lbExistingIndexEntries.BeginUpdate();
            lbSelectedIndexEntries.BeginUpdate();
            List<BDSearchEntry> selected = lbExistingIndexEntries.SelectedItems.Cast<BDSearchEntry>().ToList();
            
            foreach(BDSearchEntry entry in selected)
            {
                selectedSearchEntries.Add(entry);
                int assnCount = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, entry.Uuid).Count;
                BDSearchEntryAssociation assnForEntry = BDSearchEntryAssociation.CreateBDSearchEntryAssociation(dataContext, entry.Uuid, currentNode.Uuid, currentNode.LayoutVariant, displayContext);
                assnForEntry.displayOrder = assnCount;
                BDSearchEntryAssociation.Save(dataContext, assnForEntry);

                availableSearchEntries.Remove(entry);
            }
            currentSearchEntry = selected[0];
            
            selectedSearchEntries.Sort("name", ListSortDirection.Ascending);
            
            lbExistingIndexEntries.ClearSelected();
            lbSelectedIndexEntries.ClearSelected();
            lbExistingIndexEntries.EndUpdate();
            lbSelectedIndexEntries.EndUpdate();

            lbSelectedIndexEntries.SetSelected(selectedSearchEntries.IndexOf(currentSearchEntry), true);
            reloadAssociatedLocations();
            formHasChanges = true;

            resetButtons();
        }

        private void btnRemoveFromSelected_Click(object sender, EventArgs e)
        {
            lbExistingIndexEntries.BeginUpdate();
            lbSelectedIndexEntries.BeginUpdate();
            List<BDSearchEntry> selected = lbSelectedIndexEntries.SelectedItems.Cast<BDSearchEntry>().ToList();
            foreach (BDSearchEntry entry in selected)
            {
                availableSearchEntries.Add(entry);
                selectedSearchEntries.Remove(entry);
                List<BDSearchEntryAssociation> assns = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryIdAndDisplayParentid(dataContext, entry.Uuid, currentNode.Uuid);
                
                foreach (BDSearchEntryAssociation assn in assns)
                {
                    searchEntryAssociations.Remove(assn);
                    BDSearchEntryAssociation.Delete(dataContext, assn);
                }
            }
            currentSearchEntry = selected[0];
            availableSearchEntries.Sort("name", ListSortDirection.Ascending);

            lbExistingIndexEntries.EndUpdate();
            lbSelectedIndexEntries.EndUpdate();
            lbExistingIndexEntries.ClearSelected();
            lbSelectedIndexEntries.ClearSelected();

            lbExistingIndexEntries.SetSelected(availableSearchEntries.IndexOf(currentSearchEntry), true);
            searchEntryAssociations.Clear();
            lblSelectedSearchEntry.Text = string.Empty;

            reloadAssociatedLocations();
            formHasChanges = true;
            resetButtons();
        }

        private void btnDeleteSearchEntry_Click(object sender, EventArgs e)
        {
            //entriesToDelete.Add(currentSearchEntry);
            //availableSearchEntries.Remove(currentSearchEntry);

            //formHasChanges = true;
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
                        availableSearchEntries.Add(newEntry);
                        availableSearchEntries.Sort("name", ListSortDirection.Ascending);

                        currentSearchEntry = newEntry;
                        lbExistingIndexEntries.SetSelected(availableSearchEntries.IndexOf(newEntry), true);
                        reloadAssociatedLocations();

                        formHasChanges = true;
                    }
                    else
                        MessageBox.Show("Index entry already exists");
                }
            }
        }

        private void tbEntryName_TextChanged(object sender, EventArgs e)
        {
            resetButtons();
        }

        private void cbFilterList_CheckedChanged(object sender, EventArgs e)
        {
            lbExistingIndexEntries.BeginUpdate();
            CheckBox cb = sender as CheckBox;
            if (null != cb && cb.CheckState == CheckState.Checked)
            {
                string searchTerm = tbEntryName.Text;
                List<BDSearchEntry> filtered = availableSearchEntries.Where(x => x.name.Contains(searchTerm)).ToList<BDSearchEntry>();
                if (filtered.Count <= 0)
                {
                    MessageBox.Show("No entries found with that name");
                    cbFilterList.CheckState = CheckState.Unchecked;
                }
                else
                {
                    availableSearchEntries.Clear();
                    foreach (BDSearchEntry entry in filtered)
                        availableSearchEntries.Add(entry);
                }
            }
            else
            {
                availableSearchEntries.Clear();
                foreach (BDSearchEntry entry in allSearchEntries)
                {
                    if (!selectedSearchEntries.Contains(entry))
                        availableSearchEntries.Add(entry);
                }
                tbEntryName.Text = string.Empty;
                cbFilterList.Enabled = false;
            }
            lbExistingIndexEntries.EndUpdate();
        }
        
        private void btnEditSearchEntry_Click(object sender, EventArgs e)
        {
            BDSearchEntry selected = availableSearchEntries[lbExistingIndexEntries.SelectedIndex];
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

                        availableSearchEntries.Sort("name", ListSortDirection.Ascending);
                        formHasChanges = true;
                    }
                    else
                        MessageBox.Show("An entry with that name already exists");
                }
            }
        }

        private void btnMoveAssnPrevious_Click(object sender, EventArgs e)
        {
            int selectedPosition = lbIndexEntryAssociations.SelectedIndex;
            int requestedPosition = selectedPosition - 1;

            if (requestedPosition >= 0)
            {
                searchEntryAssociations[requestedPosition].displayOrder = selectedPosition;
                searchEntryAssociations[selectedPosition].displayOrder = requestedPosition;

                BDSearchEntryAssociation.Save(dataContext, searchEntryAssociations[requestedPosition]);
                BDSearchEntryAssociation.Save(dataContext, searchEntryAssociations[selectedPosition]);
            }

            searchEntryAssociations.Sort("displayOrder", ListSortDirection.Ascending);

            formHasChanges = true;
            resetButtons();
        }

        private void btnMoveAssnNext_Click(object sender, EventArgs e)
        {
            int selectedPosition = lbIndexEntryAssociations.SelectedIndex;
            int requestedPosition = selectedPosition + 1;

            if (requestedPosition <= (searchEntryAssociations.Count - 1))
            {
                searchEntryAssociations[requestedPosition].displayOrder = selectedPosition;
                searchEntryAssociations[selectedPosition].displayOrder = requestedPosition;

                BDSearchEntryAssociation.Save(dataContext, searchEntryAssociations[requestedPosition]);
                BDSearchEntryAssociation.Save(dataContext, searchEntryAssociations[selectedPosition]);
            }
            searchEntryAssociations.Sort("displayOrder", ListSortDirection.Ascending);

            formHasChanges = true;
            resetButtons();
        }

        private void btnDeleteAssociation_Click(object sender, EventArgs e)
        {
            int index = lbIndexEntryAssociations.SelectedIndex;
            if (index >= 0)
            {
                BDSearchEntryAssociation selected = searchEntryAssociations[lbIndexEntryAssociations.SelectedIndex];
                BDSearchEntryAssociation.Delete(dataContext, selected);

                formHasChanges = true;
                resetButtons();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void lbSelectedIndexEntries_MouseDown(object sender, MouseEventArgs e)
        {
            lbExistingIndexEntries.ClearSelected();
            lbIndexEntryAssociations.ClearSelected();

            if (lbSelectedIndexEntries.SelectedIndices.Count > 0)
            {
                currentSearchEntry = selectedSearchEntries[lbSelectedIndexEntries.SelectedIndices[0]];
                reloadAssociatedLocations();
            }
            resetButtons();
        }

        private void lbExistingIndexEntries_MouseDown(object sender, MouseEventArgs e)
        {
            lbSelectedIndexEntries.ClearSelected();
            lbIndexEntryAssociations.ClearSelected();

            if (lbExistingIndexEntries.SelectedIndices.Count > 0)
            {
                currentSearchEntry = availableSearchEntries[lbExistingIndexEntries.SelectedIndices[0]];
                reloadAssociatedLocations();
            }
            resetButtons();
        }

    }    
}

