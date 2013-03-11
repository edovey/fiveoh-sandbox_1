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
        BDSortableBindingList<BDSearchEntry> selectedSearchEntries;
        BDSortableBindingList<BDSearchEntry> availableSearchEntries;
        List<BDSearchEntryAssociation> searchEntryAssociations;
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
            
            selectedSearchEntries = new BDSortableBindingList<BDSearchEntry>(BDSearchEntry.RetrieveSearchEntriesForDisplayParent(dataContext, currentNode.Uuid));
            List<BDSearchEntry> tmpEntry = new List<BDSearchEntry>(allSearchEntries);
            foreach (BDSearchEntry entry in selectedSearchEntries)
                tmpEntry.Remove(entry);
            availableSearchEntries = new BDSortableBindingList<BDSearchEntry>(tmpEntry);

            lblSelectedSearchEntry.Text = string.Empty;
            lblName.Text = !string.IsNullOrEmpty(currentNode.Name) ? currentNode.Name : "<intentionally blank>";

            lbAvailableIndexEntries.DataSource = availableSearchEntries;
            lbSelectedIndexEntries.DataSource = selectedSearchEntries;
        }

        private void reloadAssociatedLocations()
        {
            this.SuspendLayout();
            lbIndexEntryAssociations.BeginUpdate();
            searchEntryAssociations.Clear();

            currentSearchEntry = selectedSearchEntries[lbSelectedIndexEntries.SelectedIndices[0]];

            searchEntryAssociations = new List<BDSearchEntryAssociation>(BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, currentSearchEntry.Uuid));

            foreach(BDSearchEntryAssociation assn in searchEntryAssociations)
            {
                lbIndexEntryAssociations.Items.Add(assn);
            }
            lbIndexEntryAssociations.EndUpdate();
        }

        private void btnAddToSelected_Click(object sender, EventArgs e)
        {
            lbAvailableIndexEntries.BeginUpdate();
            lbSelectedIndexEntries.BeginUpdate();
            List<BDSearchEntry> selected = lbAvailableIndexEntries.SelectedItems.Cast<BDSearchEntry>().ToList();
            foreach(BDSearchEntry entry in selected)
            {
                selectedSearchEntries.Add(entry);
                availableSearchEntries.Remove(entry);
            }

            // Re-sort the list
            selectedSearchEntries.Sort("name", ListSortDirection.Ascending);
            
            lbAvailableIndexEntries.ClearSelected();
            lbSelectedIndexEntries.ClearSelected();
            lbAvailableIndexEntries.EndUpdate();
            lbSelectedIndexEntries.EndUpdate();
        }

        private void btnRemoveFromSelected_Click(object sender, EventArgs e)
        {
            lbAvailableIndexEntries.BeginUpdate();
            lbSelectedIndexEntries.BeginUpdate();
            List<BDSearchEntry> selected = lbSelectedIndexEntries.SelectedItems.Cast<BDSearchEntry>().ToList();
            foreach (BDSearchEntry entry in selected)
            {
                availableSearchEntries.Add(entry);
                selectedSearchEntries.Remove(entry);
            }
            availableSearchEntries.Sort("name", ListSortDirection.Ascending);

            lbAvailableIndexEntries.EndUpdate();
            lbSelectedIndexEntries.EndUpdate();
            lbAvailableIndexEntries.ClearSelected();
            lbSelectedIndexEntries.ClearSelected();
        }

        private void btnEditEntryName_Click(object sender, EventArgs e)
        {

        }


        private void btnMoveAssnPrevious_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveAssnNext_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteAssociation_Click(object sender, EventArgs e)
        {

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
            }
        }
    }    
}

