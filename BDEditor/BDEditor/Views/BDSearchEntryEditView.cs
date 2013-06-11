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
    public partial class BDSearchEntryEditView : Form
    {
        private Entities dataContext;
        private IBDNode currentNode;
        private BDLinkedNoteAssociation currentLinkedNoteAssociation;
        private BDSearchEntry currentSearchEntry;
        private BDSearchEntryAssociation currentSearchEntryAssociation;
        private Guid displayContextParentUuid;

        private List<BDSearchEntry> allSearchEntries;
        private BDSearchEntryBindingList selectedSearchEntries;
        private BDSearchEntryBindingList availableSearchEntries;
        private BDSearchEntryAssociationBindingList searchEntryAssociations;
        
        private bool formHasChanges = false;
        private Guid htmlPageUuid;

        string editorContext = string.Empty;

        public BDSearchEntryEditView()
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

        public void AssignCurrentNode(IBDNode pCurrentNode)
        {
            currentNode = pCurrentNode;
        }

        /// <summary>
        /// Assign the node which is the lowest in the IBDNode hierarchy that is a parent to the note being edited
        /// This value is needed to build path for display context for search/index entries, esp where linked notes are nested.  
        /// (in such a case the heirarchy cannot be built from a lookup as we don't necessarily know which parent of a 
        /// linked note is the correct one for the current context.)
        /// </summary>
        /// <param name="pNode">IBDNode that is parent or grandparent of linked note</param>
        public void AssignDisplayContextParent(Guid pDisplayContextParentUuid)
        {
            displayContextParentUuid = pDisplayContextParentUuid;
        }

        public void AssignCurrentLinkedNoteAssociation(BDLinkedNoteAssociation pLinkedNoteAssociation)
        {
            currentLinkedNoteAssociation = pLinkedNoteAssociation;
        }

        private void BDSearchEntryEditView_Load(object sender, EventArgs e)
        {
            lbExistingSearchEntries.BeginUpdate();
            lbSelectedSearchEntries.BeginUpdate();

            allSearchEntries = BDSearchEntry.RetrieveAll(dataContext);
            searchEntryAssociations = new BDSearchEntryAssociationBindingList();
            selectedSearchEntries = new BDSearchEntryBindingList();
            availableSearchEntries = new BDSearchEntryBindingList();

            reloadSelectedEntries();
            reloadAvailableEntries();

            availableSearchEntries.Sort("name", ListSortDirection.Ascending);
            selectedSearchEntries.Sort("name", ListSortDirection.Ascending);

            lblSelectedSearchEntry.Text = string.Empty;
            if(null != currentNode)
            lblName.Text = !string.IsNullOrEmpty(currentNode.Name) ? currentNode.Name : "<intentionally blank>";
            if(null != currentLinkedNoteAssociation)
                lblName.Text = !string.IsNullOrEmpty(currentLinkedNoteAssociation.DescriptionForLinkedNote) ? currentLinkedNoteAssociation.DescriptionForLinkedNote : "<intentionally blank>";

            lbExistingSearchEntries.DataSource = availableSearchEntries;
            lbSelectedSearchEntries.DataSource = selectedSearchEntries;
            lbSearchEntryAssociations.DataSource = searchEntryAssociations;
            lbSearchEntryAssociations.DisplayMember = "editorContext";

            if (selectedSearchEntries.Count > 0)
            {
                lbSelectedSearchEntries.SetSelected(0, true);
                lbExistingSearchEntries.ClearSelected();
                currentSearchEntry = selectedSearchEntries[0];
                reloadAssociatedLocations();
            }
            else
            {
                lbExistingSearchEntries.ClearSelected();
            }
            lbExistingSearchEntries.EndUpdate();
            lbSelectedSearchEntries.EndUpdate();
        }

        private void BDSearchEntryEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            lbExistingSearchEntries.DataSource = null;
            lbSelectedSearchEntries.DataSource = null;
            lbSearchEntryAssociations.DataSource = null;

            allSearchEntries.Clear();
            searchEntryAssociations.Clear();
            selectedSearchEntries.Clear();
            availableSearchEntries.Clear();
        }

        private void reloadAvailableEntries()
        {
            List<BDSearchEntry> tmpAvailable = new List<BDSearchEntry>(allSearchEntries);
            foreach (BDSearchEntry entry in selectedSearchEntries)
                tmpAvailable.Remove(entry);
            availableSearchEntries = new BDSearchEntryBindingList(tmpAvailable);
            lbExistingSearchEntries.DataSource = availableSearchEntries;
        }

        private void reloadSelectedEntries()
        {
            selectedSearchEntries.Clear();
            if (null != currentNode)
            {
                List<BDSearchEntry> tmpSelected = BDSearchEntry.RetrieveSearchEntriesForAnchorNode(dataContext, currentNode.Uuid);
                selectedSearchEntries = new BDSearchEntryBindingList(tmpSelected.Distinct().ToList());
                lbSelectedSearchEntries.DataSource = selectedSearchEntries;
            }
            else if (null != currentLinkedNoteAssociation)
            {
                List<BDSearchEntry> tmpSelected = BDSearchEntry.RetrieveSearchEntriesForAnchorNode(dataContext, currentLinkedNoteAssociation.Uuid);
                selectedSearchEntries = new BDSearchEntryBindingList(tmpSelected.Distinct().ToList());
                lbSelectedSearchEntries.DataSource = selectedSearchEntries;
            }
        }

        private void reloadAssociatedLocations()
        {
            lbSearchEntryAssociations.BeginUpdate();
            searchEntryAssociations.Clear();

            if (null != currentSearchEntry)
            {
                currentSearchEntryAssociation = null;
                lblSelectedSearchEntry.Text = currentSearchEntry.name;
                List<BDSearchEntryAssociation> tmpList = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, currentSearchEntry.Uuid);

                Guid ownerUuid = (currentNode != null) ? currentNode.Uuid : currentLinkedNoteAssociation.Uuid;
                foreach (BDSearchEntryAssociation nodeAssn in tmpList)
                {
                    searchEntryAssociations.Add(nodeAssn);
                    if (nodeAssn.displayOrder == -1)
                    {
                        nodeAssn.displayOrder = searchEntryAssociations.IndexOf(nodeAssn);
                        BDSearchEntryAssociation.Save(dataContext, nodeAssn);
                    }
                    if (nodeAssn.anchorNodeId == ownerUuid)
                        currentSearchEntryAssociation = nodeAssn;
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
                    {
                        if (!string.IsNullOrEmpty(currentSearchEntryAssociation.displayContext))
                            currentSearchEntryAssociation.editorContext = currentSearchEntryAssociation.displayContext;
                        else
                        {
                            if (null != displayContextParentUuid && displayContextParentUuid != Guid.Empty)
                                currentSearchEntryAssociation.editorContext = BDUtilities.BuildHierarchyString(dataContext, displayContextParentUuid, ":");
                            else
                                currentSearchEntryAssociation.editorContext = "ERROR: current context could not be generated.";
                        }
                    }
                    if (currentSearchEntryAssociation.anchorNodeId == Guid.Empty)
                        currentSearchEntryAssociation.anchorNodeId = ownerUuid;

                   // adjust editorContext for visibility
                    if (currentSearchEntryAssociation.editorContext.IndexOf("*") != 0)
                        currentSearchEntryAssociation.editorContext = currentSearchEntryAssociation.editorContext.Insert(0, "*");

                    BDSearchEntryAssociation.Save(dataContext, currentSearchEntryAssociation);
                    lbSearchEntryAssociations.DataSource = searchEntryAssociations;
                    lbSearchEntryAssociations.DisplayMember = "editorContext";
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

            btnRemoveFromSelected.Enabled = lbSelectedSearchEntries.SelectedIndices.Count > 0 ? true : false;
            btnAddToSelected.Enabled = lbExistingSearchEntries.SelectedIndices.Count > 0 ? true : false;

            btnEditSearchEntry.Enabled = (lbSelectedSearchEntries.SelectedIndices.Count > 0) ? true : false;
        }

        private void btnAddToSelected_Click(object sender, EventArgs e)
        {
            lbExistingSearchEntries.BeginUpdate();
            lbSelectedSearchEntries.BeginUpdate();
            List<BDSearchEntry> selected = lbExistingSearchEntries.SelectedItems.Cast<BDSearchEntry>().ToList();

            foreach (BDSearchEntry entry in selected)
            {
                if (selectedSearchEntries.IndexOf(entry) < 0)
                {
                    selectedSearchEntries.Add(entry);
                    int assnCount = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, entry.Uuid).Count;
                    Guid ownerUuid = (currentNode != null) ? currentNode.Uuid : currentLinkedNoteAssociation.Uuid;
                    BDSearchEntryAssociation assnForEntry = BDSearchEntryAssociation.CreateBDSearchEntryAssociation(dataContext, entry.Uuid, ownerUuid, editorContext);
                    assnForEntry.displayOrder = assnCount;
                    BDSearchEntryAssociation.Save(dataContext, assnForEntry);

                    availableSearchEntries.Remove(entry);
                }
            }
        
            currentSearchEntry = selected[0];
            
            selectedSearchEntries.Sort("name", ListSortDirection.Ascending);
            
            lbExistingSearchEntries.ClearSelected();
            lbSelectedSearchEntries.ClearSelected();
            lbExistingSearchEntries.EndUpdate();
            lbSelectedSearchEntries.EndUpdate();

            lbSelectedSearchEntries.SetSelected(selectedSearchEntries.IndexOf(currentSearchEntry), true);
            reloadAssociatedLocations();
            formHasChanges = true;

            resetButtons();
        }

        private void btnRemoveFromSelected_Click(object sender, EventArgs e)
        {
            lbExistingSearchEntries.BeginUpdate();
            lbSelectedSearchEntries.BeginUpdate();
            List<BDSearchEntry> selected = lbSelectedSearchEntries.SelectedItems.Cast<BDSearchEntry>().ToList();
            foreach (BDSearchEntry entry in selected)
            {
                availableSearchEntries.Add(entry);
                selectedSearchEntries.Remove(entry);
                List<BDSearchEntryAssociation> assns = new List<BDSearchEntryAssociation>();
                if(null != currentNode)
                    assns.AddRange(BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryIdAndAnchorNodeId(dataContext, entry.Uuid, currentNode.Uuid));
                if (null != currentLinkedNoteAssociation)
                    assns.AddRange(BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryIdAndAnchorNodeId(dataContext, entry.Uuid, currentLinkedNoteAssociation.Uuid));
                foreach (BDSearchEntryAssociation assn in assns)
                {
                    searchEntryAssociations.Remove(assn);
                    BDSearchEntryAssociation.Delete(dataContext, assn);
                }
            }
            currentSearchEntry = selected[0];
            availableSearchEntries.Sort("name", ListSortDirection.Ascending);

            lbExistingSearchEntries.EndUpdate();
            lbSelectedSearchEntries.EndUpdate();
            lbExistingSearchEntries.ClearSelected();
            lbSelectedSearchEntries.ClearSelected();

            //lbExistingSearchEntries.SetSelected(availableSearchEntries.IndexOf(currentSearchEntry), true);
            if (lbExistingSearchEntries.Items.IndexOf(currentSearchEntry) >= 0)
            {
                lbExistingSearchEntries.SetSelected(lbExistingSearchEntries.Items.IndexOf(currentSearchEntry), true);
            }
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

                        lbExistingSearchEntries.BeginUpdate();
                        lbExistingSearchEntries.ClearSelected();
                        lbExistingSearchEntries.SetSelected(availableSearchEntries.IndexOf(newEntry), true);
                        lbExistingSearchEntries.EndUpdate();

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
            lbExistingSearchEntries.BeginUpdate();
            

            CheckBox cb = sender as CheckBox;
            if (null != cb && cb.CheckState == CheckState.Checked)
            {
                string searchTerm = tbEntryName.Text;
                if (!string.IsNullOrEmpty(tbEntryName.Text))
                {
                    List<BDSearchEntry> filtered = availableSearchEntries.Where(x => x.name.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) != -1).ToList<BDSearchEntry>();
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
            }
            else
            {
                lbSearchEntryAssociations.ClearSelected();
                //lbExistingSearchEntries.ClearSelected();

                reloadAvailableEntries();
                resetButtons();

                //tbEntryName.Text = string.Empty;
                //cbFilterList.Enabled = false;
            }
            lbExistingSearchEntries.ClearSelected();
            lbExistingSearchEntries.EndUpdate();
        }
        
        private void btnEditSearchEntry_Click(object sender, EventArgs e)
        {
            if (lbSelectedSearchEntries.SelectedIndex >= 0)
            {
                BDSearchEntry selected = lbSelectedSearchEntries.SelectedItems[0] as BDSearchEntry;
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

                            selectedSearchEntries.Sort("name", ListSortDirection.Ascending);
                            formHasChanges = true;
                        }
                        else
                            MessageBox.Show("An entry with that name already exists");
                    }
                }
            }
            else
                MessageBox.Show("Please select an entry to edit");
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
            //int index = lbSearchEntryAssociations.SelectedIndex;
            //if (index >= 0)
            //{
            //    BDSearchEntryAssociation selected = searchEntryAssociations[lbSearchEntryAssociations.SelectedIndex];
            //    BDSearchEntryAssociation.Delete(dataContext, selected);

            //    formHasChanges = true;
            //    resetButtons();
            //}
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

        private void lbSelectedSearchEntries_MouseDown(object sender, MouseEventArgs e)
        {
            lbExistingSearchEntries.ClearSelected();
            lbSearchEntryAssociations.ClearSelected();

            if (lbSelectedSearchEntries.SelectedItems.Count > 0)
            {
                currentSearchEntry = lbSelectedSearchEntries.SelectedItems[0] as BDSearchEntry;
                if (null != currentSearchEntryAssociation && currentSearchEntryAssociation.editorContext.IndexOf("*") == 0)
                {
                    currentSearchEntryAssociation.editorContext = currentSearchEntryAssociation.editorContext.Substring(1);
                    BDSearchEntryAssociation.Save(dataContext, currentSearchEntryAssociation);
                }
                reloadAssociatedLocations();
            }
            resetButtons();
        }

        private void lbExistingIndexEntries_MouseDown(object sender, MouseEventArgs e)
        {
            lbSelectedSearchEntries.ClearSelected();
            lbSearchEntryAssociations.ClearSelected();

            if (lbExistingSearchEntries.SelectedItems.Count > 0)
            {
                currentSearchEntry = lbExistingSearchEntries.SelectedItems[0] as BDSearchEntry;
                reloadAssociatedLocations();
            }
            resetButtons();
        }

        private void tbEntryName_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbFilterList.CheckState != CheckState.Unchecked)
                cbFilterList.CheckState = CheckState.Unchecked;
        }

    }    
}

