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
        List<BDSearchEntry> selectedSearchEntries;
        List<BDSearchEntry> availableSearchEntries;
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
            selectedSearchEntries = BDSearchEntry.RetrieveSearchEntriesForDisplayParent(dataContext, currentNode.Uuid);
            availableSearchEntries = new List<BDSearchEntry>();
            availableSearchEntries.AddRange(allSearchEntries);
            foreach (BDSearchEntry entry in selectedSearchEntries)
                availableSearchEntries.Remove(entry);

        }

        private void reloadAssociatedLocations()
        {
            this.SuspendLayout();
            searchEntryAssociations.Clear();

            currentSearchEntry = selectedSearchEntries[listBox2.SelectedIndices[0]];

            searchEntryAssociations = BDSearchEntryAssociation.RetrieveSearchEntryAssociationsForSearchEntryId(dataContext, currentSearchEntry.Uuid);

            if(searchEntryAssociations.Count > 0)
            {
                
            }

        }

        private void btnAddToSelected_Click(object sender, EventArgs e)
        {

        }

        private void btnRemoveFromSelected_Click(object sender, EventArgs e)
        {

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

        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }
    }    
}

