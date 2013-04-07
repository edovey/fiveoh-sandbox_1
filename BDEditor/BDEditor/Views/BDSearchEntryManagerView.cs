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

        private List<BDSearchEntry> entriesToDelete;
        private List<BDSearchEntryAssociation> associationsToDelete;

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
            set { 
                if(!string.IsNullOrEmpty(value))
                    editorContext = value; 
            }
        }

        private void BDSearchEntryEditView_Load(object sender, EventArgs e)
        {
            lbExistingSearchEntries.BeginUpdate();

            allSearchEntries = BDSearchEntry.RetrieveAll(dataContext);
            searchEntryAssociations = new BDSearchEntryAssociationBindingList();
            searchEntries = new BDSearchEntryBindingList();

            entriesToDelete = new List<BDSearchEntry>();
            associationsToDelete = new List<BDSearchEntryAssociation>();

            reloadAvailableEntries();

            lbSearchEntryAssociations.DataSource = searchEntryAssociations;
            lbSearchEntryAssociations.DisplayMember = "editorContext";

            lbExistingSearchEntries.SetSelected(0, true);
            lbExistingSearchEntries.EndUpdate();
        }

        private void reloadAvailableEntries()
        {
            searchEntries = new BDSearchEntryBindingList(allSearchEntries);
            lbExistingSearchEntries.DataSource = searchEntries;
        }

        private void reloadAssociatedLocations()
        {
            lbSearchEntryAssociations.BeginUpdate();
            searchEntryAssociations.Clear();

            if (null == currentSearchEntry)
            {
                if (lbExistingSearchEntries.SelectedIndices.Count > 0)
                {
                    currentSearchEntry = searchEntries[lbExistingSearchEntries.SelectedIndices[0]];
                }
            }

            if (null != currentSearchEntry)
            {
                currentSearchEntryAssociation = null;
                List<BDSearchEntryAssociation> tmpList = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, currentSearchEntry.Uuid);

                foreach (BDSearchEntryAssociation nodeAssn in tmpList)
                {
                    searchEntryAssociations.Add(nodeAssn);
                    if (nodeAssn.displayOrder == -1)
                    {
                        nodeAssn.displayOrder = searchEntryAssociations.IndexOf(nodeAssn);
                        BDSearchEntryAssociation.Save(dataContext, nodeAssn);
                    }
                    // repair if empty so there is something to display on the UI
                    if (string.IsNullOrEmpty(nodeAssn.editorContext))
                        nodeAssn.editorContext = nodeAssn.displayContext;
                }
                if (null != currentSearchEntryAssociation)
                {
                    if (currentSearchEntryAssociation.editorContext != this.editorContext)
                        currentSearchEntryAssociation.editorContext = this.editorContext; // update to current location

                    // repair any missing data : needed for converting from fully generated associations to managed associations
                    if (string.IsNullOrEmpty(currentSearchEntryAssociation.editorContext))
                        currentSearchEntryAssociation.editorContext = currentSearchEntryAssociation.displayContext;

                    // adjust editorContext for visibility
                    if (currentSearchEntryAssociation.editorContext.IndexOf("*") != 0)
                        currentSearchEntryAssociation.editorContext = currentSearchEntryAssociation.editorContext.Insert(0, "*");

                    BDSearchEntryAssociation.Save(dataContext, currentSearchEntryAssociation);
                    lbSearchEntryAssociations.SetSelected(searchEntryAssociations.IndexOf(currentSearchEntryAssociation), true);
                }
                else
                    lbSearchEntryAssociations.ClearSelected();
            }
            lbSearchEntryAssociations.EndUpdate();

            resetButtons();
        }

        private void resetButtons()
        {
            btnMoveAssnNext.Enabled = (lbSearchEntryAssociations.SelectedItems.Count > 0  && searchEntryAssociations.Count > 1)? true : false;
            btnMoveAssnPrevious.Enabled = (lbSearchEntryAssociations.SelectedItems.Count > 0 && searchEntryAssociations.Count > 1) ? true : false;

            cbFilterList.Enabled = tbEntryName.TextLength > 0 ? true : false;

            btnDeleteAssociation.Enabled = currentSearchEntryAssociation != null ? true : false; 

            btnDeleteSearchEntry.Enabled = (lbExistingSearchEntries.SelectedIndices.Count > 0) ? true : false;
            btnEditSearchEntry.Enabled = (lbExistingSearchEntries.SelectedIndices.Count > 0) ? true : false;
            
            btnOk.Enabled = formHasChanges;
            //btnCancel.Enabled = !formHasChanges;
        }

        private void btnDeleteSearchEntry_Click(object sender, EventArgs e)
        {
            if (null != currentSearchEntry)
            {
                bool okToDelete = false;
                List<BDSearchEntryAssociation> assns = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, currentSearchEntry.Uuid);
                if (assns.Count > 0)
                {
                    DialogResult confirm = MessageBox.Show("This entry has existing links.  Are you sure?", "Confirm Delete", MessageBoxButtons.OKCancel);
                    if (confirm == DialogResult.OK)
                        okToDelete = true;
                }
                else
                    okToDelete = true;

                if(okToDelete)
                {
                    entriesToDelete.Add(currentSearchEntry);
                    allSearchEntries.Remove(currentSearchEntry);
                    currentSearchEntry = null;
                    currentSearchEntryAssociation = null;

                    reloadAvailableEntries();
                    reloadAssociatedLocations();

                    formHasChanges = true;
                    resetButtons();
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
            CheckBox cb = sender as CheckBox;
            if (null != cb && cb.CheckState == CheckState.Checked)
            {
                string searchTerm = tbEntryName.Text;
                List<BDSearchEntry> filtered = searchEntries.Where(x => x.name.Contains(searchTerm)).ToList<BDSearchEntry>();
                if (filtered.Count <= 0)
                {
                    MessageBox.Show("No entries found with that name");
                    cbFilterList.CheckState = CheckState.Unchecked;
                    reloadAvailableEntries();
                }
                else
                {
                    BDSearchEntryBindingList filteredEntries = new BDSearchEntryBindingList(filtered);
                    lbExistingSearchEntries.DataSource = filteredEntries;
                }
            }
            else
            {
                lbSearchEntryAssociations.ClearSelected();
                lbExistingSearchEntries.ClearSelected();
                currentSearchEntryAssociation = null;
                currentSearchEntry = null;

                reloadAvailableEntries();
                resetButtons();

                tbEntryName.Text = string.Empty;
                cbFilterList.Enabled = false;
            }
            lbExistingSearchEntries.EndUpdate();
        }
        
        private void btnEditSearchEntry_Click(object sender, EventArgs e)
        {
            BDSearchEntry selected = searchEntries[lbExistingSearchEntries.SelectedIndex];
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
                lbSearchEntryAssociations.SetSelected(requestedPosition, true);
                lbSearchEntryAssociations.EndUpdate();

                formHasChanges = true;
                resetButtons();
            }
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
                lbSearchEntryAssociations.SetSelected(requestedPosition, true);
                lbSearchEntryAssociations.EndUpdate();

                formHasChanges = true;
                resetButtons();
            }
        }

        private void btnDeleteAssociation_Click(object sender, EventArgs e)
        {
            int index = lbSearchEntryAssociations.SelectedIndex;
            if (index >= 0)
            {
                BDSearchEntryAssociation selected = searchEntryAssociations[lbSearchEntryAssociations.SelectedIndex];
                associationsToDelete.Add(selected);
                searchEntryAssociations.Remove(selected);

                formHasChanges = true;
                resetButtons();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (currentSearchEntryAssociation != null && currentSearchEntryAssociation.editorContext.IndexOf("*") == 0)
                currentSearchEntryAssociation.editorContext = currentSearchEntryAssociation.editorContext.Substring(1);

            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (currentSearchEntryAssociation != null && currentSearchEntryAssociation.editorContext.IndexOf("*") == 0)
                currentSearchEntryAssociation.editorContext = currentSearchEntryAssociation.editorContext.Substring(1);

            // save from lists
            foreach (BDSearchEntry entry in entriesToDelete)
                BDSearchEntry.Delete(dataContext, entry, false);

            foreach (BDSearchEntryAssociation assn in associationsToDelete)
                BDSearchEntryAssociation.Delete(dataContext, assn, false);

            this.DialogResult = DialogResult.OK;
        }

        private void lbExistingIndexEntries_MouseDown(object sender, MouseEventArgs e)
        {
            lbSearchEntryAssociations.ClearSelected();

            if (lbExistingSearchEntries.SelectedIndices.Count > 0)
            {
                currentSearchEntry = searchEntries[lbExistingSearchEntries.SelectedIndices[0]];
                reloadAssociatedLocations();
            }
            resetButtons();
        }

        private void lbSearchEntryAssociations_MouseDown(object sender, MouseEventArgs e)
        {
            if (lbSearchEntryAssociations.SelectedItems.Count > 0)
                currentSearchEntryAssociation = searchEntryAssociations[lbSearchEntryAssociations.SelectedIndices[0]];
            resetButtons();
        }
    }    
}

