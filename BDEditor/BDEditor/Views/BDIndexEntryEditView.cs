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
        List<BDSearchEntry> entriesToDelete;
        List<BDSearchEntryAssociation> assnsToDelete;
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
            entriesToDelete = new List<BDSearchEntry>();
            assnsToDelete = new List<BDSearchEntryAssociation>();

            selectedSearchEntries = new BDSearchEntryBindingList(BDSearchEntry.RetrieveSearchEntriesForDisplayParent(dataContext, currentNode.Uuid));
            List<BDSearchEntry> tmpEntry = new List<BDSearchEntry>(allSearchEntries);
            foreach (BDSearchEntry entry in selectedSearchEntries)
                tmpEntry.Remove(entry);
            availableSearchEntries = new BDSearchEntryBindingList(tmpEntry);

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

            currentSearchEntry = selectedSearchEntries[lbSelectedIndexEntries.SelectedIndices[0]];
            List<BDSearchEntryAssociation> tmpList = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, currentSearchEntry.Uuid);
            foreach (BDSearchEntryAssociation assn in tmpList)
                searchEntryAssociations.Add(assn);

            lbIndexEntryAssociations.EndUpdate();

            btnMoveAssnNext.Enabled = searchEntryAssociations.Count > 1 ? true : false;
            btnMoveAssnPrevious.Enabled = searchEntryAssociations.Count > 1 ? true : false;

            btnDeleteAssociation.Enabled = searchEntryAssociations.Count > 0 ? true : false;
        }

        private void btnAddToSelected_Click(object sender, EventArgs e)
        {
            lbExistingIndexEntries.BeginUpdate();
            lbSelectedIndexEntries.BeginUpdate();
            List<BDSearchEntry> selected = lbExistingIndexEntries.SelectedItems.Cast<BDSearchEntry>().ToList();
            foreach(BDSearchEntry entry in selected)
            {
                selectedSearchEntries.Add(entry);
                availableSearchEntries.Remove(entry);
            }
            currentSearchEntry = selected[0];
            
            selectedSearchEntries.Sort("name", ListSortDirection.Ascending);
            
            lbExistingIndexEntries.ClearSelected();
            lbSelectedIndexEntries.ClearSelected();
            lbExistingIndexEntries.EndUpdate();
            lbSelectedIndexEntries.EndUpdate();

            lbSelectedIndexEntries.SetSelected(selectedSearchEntries.IndexOf(currentSearchEntry), true);

            btnOk.Enabled = true;
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

            btnOk.Enabled = true;
        }

        private void btnEditEntryName_Click(object sender, EventArgs e)
        {

            btnOk.Enabled = true;
        }


        private void btnMoveAssnPrevious_Click(object sender, EventArgs e)
        {

            btnOk.Enabled = true;
        }

        private void btnMoveAssnNext_Click(object sender, EventArgs e)
        {

            btnOk.Enabled = true;
        }

        private void btnDeleteAssociation_Click(object sender, EventArgs e)
        {

            btnOk.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            this.DialogResult = DialogResult.OK;
        }

        private void lbSelectedIndexEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblSelectedSearchEntry.Text = string.Empty;
            if (lbSelectedIndexEntries.SelectedIndices.Count > 0)
            {
                BDSearchEntry entry = selectedSearchEntries[lbSelectedIndexEntries.SelectedIndices[0]];
                lblSelectedSearchEntry.Text = entry.name;

                reloadAssociatedLocations();
            }
        }
    }    
}

