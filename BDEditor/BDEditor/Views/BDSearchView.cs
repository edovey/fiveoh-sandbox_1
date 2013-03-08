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
    public partial class BDSearchView : Form
    {
        private BDEditor.DataModel.Entities dataContext;
        List<IBDNode> nodeList;
        List<BDLinkedNote> linkedNoteList;
        string locationString = string.Empty;
        string searchTerm = string.Empty;

        public BDSearchView()
        {
            InitializeComponent();
        }

        private void BDSearchView_Load(object sender, EventArgs e)
        {
            nodeList = new List<IBDNode>();
            linkedNoteList = new List<BDLinkedNote>();

            DataGridViewTextBoxColumn col0 = new DataGridViewTextBoxColumn();
            col0.DataPropertyName = "nodeType";
            col0.HeaderText = "Type";

            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            col1.DataPropertyName = "name";
            col1.HeaderText = "Name";
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public string LocationString
        {
            get { return locationString; }
        }

        public string SearchTerm
        {
            get { return searchTerm; }
        }

        private void executeSearch()
        {
            this.SuspendLayout();
            nodeList.Clear();
            linkedNoteList.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;

            var nodesToSearch = loadTypesFromUI();
            searchTerm = tbSearchTerm.Text;

            if (rbNodes.Checked == true && tbSearchTerm.Text.Length > 0)
            {
                nodeList.AddRange(BDFabrik.SearchNodesForText(this.dataContext, nodesToSearch, tbSearchTerm.Text.Trim()));
                if (nodeList.Count > 0)
                {
                    dataGridView1.DataSource = nodeList;
                    // column 1
                    DataGridViewButtonColumn editButtons = new DataGridViewButtonColumn();
                    dataGridView1.Columns.Add(editButtons);
                    editButtons.Name = "Edit";
                    editButtons.Text = "Edit";
                    editButtons.HeaderText = "";
                    editButtons.UseColumnTextForButtonValue = true;
                    editButtons.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    
                    // column 2
                    DataGridViewTextBoxColumn nodeName = new DataGridViewTextBoxColumn();
                    dataGridView1.Columns.Add(nodeName);
                    nodeName.DataPropertyName = "Name";
                    nodeName.HeaderText = "Name";

                    // column 3
                    DataGridViewTextBoxColumn nodeClass = new DataGridViewTextBoxColumn();
                    dataGridView1.Columns.Add(nodeClass);
                    nodeClass.DataPropertyName = "NodeType";
                    nodeClass.HeaderText = "Type";
                    nodeClass.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridView1.Tag = "Node";

                    // column 4
                    DataGridViewButtonColumn locateButtons = new DataGridViewButtonColumn();
                    dataGridView1.Columns.Add(locateButtons);
                    locateButtons.Name = "Location";
                    locateButtons.Text = "Location";
                    locateButtons.HeaderText = "";
                    locateButtons.UseColumnTextForButtonValue = true;
                    locateButtons.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                }
                else
                    MessageBox.Show(this, "No matching entries found", "Not Found", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            else if (rbLinkedNotes.Checked == true && tbSearchTerm.Text.Length > 0)
            {
                linkedNoteList.AddRange(BDLinkedNote.RetrieveLinkedNotesWithText(dataContext, tbSearchTerm.Text.Trim()));
                if (linkedNoteList.Count > 0)
                {
                    dataGridView1.DataSource = linkedNoteList;
                    // set up grid columns
                    DataGridViewButtonColumn editButtons = new DataGridViewButtonColumn();
                    editButtons.Name = "Edit";
                    editButtons.Text = "Edit";
                    editButtons.HeaderText = "";
                    editButtons.UseColumnTextForButtonValue = true;
                    editButtons.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    dataGridView1.Columns.Add(editButtons);

                    DataGridViewTextBoxColumn nodeName = new DataGridViewTextBoxColumn();
                    nodeName.DataPropertyName = "DescriptionForLinkedNote";
                    nodeName.HeaderText = "Description";
                    dataGridView1.Columns.Add(nodeName);

                    dataGridView1.Tag = "LinkedNote";
                }
                else
                    MessageBox.Show(this, "No matching entries found", "Not Found", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            rtbLocation.Text = string.Empty;
            dataGridView1.Refresh();
            this.ResumeLayout();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            executeSearch();
        }

        void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 ||( e.ColumnIndex !=
                dataGridView1.Columns["Edit"].Index  &&
            e.ColumnIndex !=
                dataGridView1.Columns["Location"].Index)) return;

            // Retrieve the node ID.
            if (dataGridView1.Tag.ToString() == "LinkedNote")
            {
                Guid linkedNoteId = (Guid)linkedNoteList[e.RowIndex].Uuid;

                BDLinkedNote note = BDLinkedNote.RetrieveLinkedNoteWithId(dataContext, linkedNoteId);
                List<BDLinkedNoteAssociation> assns = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForLinkedNoteId(dataContext, linkedNoteId);
                BDLinkedNoteView noteView = new BDLinkedNoteView();
                noteView.AssignDataContext(dataContext);
                noteView.AssignParentInfo(assns[0].parentId, assns[0].ParentType);
                noteView.AssignContextPropertyName(assns[0].parentKeyPropertyName);
                noteView.AssignScopeId(assns[0].parentId);
                noteView.ShowDialog(this);
                executeSearch();
            }
            else if(dataGridView1.Columns["Edit"].Index == e.ColumnIndex)
            {
                openEditorForNode(nodeList[e.RowIndex]); 
                locationString = string.Empty;
                executeSearch();
            }
            else if (dataGridView1.Columns["Location"].Index == e.ColumnIndex)
            {
                locationString = BDUtilities.BuildHierarchyString(dataContext, nodeList[e.RowIndex], "\n");
                rtbLocation.Text = locationString;
            }
        }

        private void rbLinkedNotes_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLinkedNotes.Checked == true)
            {
                groupBox1.Enabled = false;
                cbChapter.Checked = false;
                cbSection.Checked = false;
                cbSubcategory.Checked = false;
                cbCategory.Checked = false;
                cbDisease.Checked = false;
                cbPresentation.Checked = false;
                cbPathogenGroup.Checked = false;
                cbPathogen.Checked = false;
                cbTherapyGroup.Checked = false;
                cbTherapy.Checked = false;
                cbTable.Checked = false;
                cbTableSection.Checked = false;
                cbTableSubsection.Checked = false;
                cbPathogenResistance.Checked = false;
                cbDosageGroup.Checked = false;
                cbDosage.Checked = false;
                cbAntimicrobial.Checked = false;
                cbPrecaution.Checked = false;
                cbSubsection.Checked = false;
                cbTopic.Checked = false;
                cbTableGroup.Checked = false;
                cbAttachment.Checked = false;
                cbSurgeryGroup.Checked = false;
                cbSurgery.Checked = false;
                cbSurgeryClassification.Checked = false;
                cbSubtopic.Checked = false;
                cbAntimicrobialGroup.Checked = false;
                cbMicroorganism.Checked = false;
                cbMicroorganismGroup.Checked = false;
                cbInfectionFrequency.Checked = false;
                cbAntimicrobialRisk.Checked = false;
                cbCondition.Checked = false;
                cbImmuneResponse.Checked = false;
                cbRegimen.Checked = false;
                cbConfiguredEntry.Checked = false;
                cbCombinedEntry.Checked = false;
            }
        }

        private void rbNodes_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNodes.Checked == true)
            {
                groupBox1.Enabled = true;
                cbChapter.Checked = true;
                cbSection.Checked = true;
                cbSubcategory.Checked = true;
                cbCategory.Checked = true;
                cbDisease.Checked = true;
                cbPresentation.Checked = true;
                cbPathogenGroup.Checked = true;
                cbPathogen.Checked = true;
                cbTherapyGroup.Checked = true;
                cbTherapy.Checked = true;
                cbTable.Checked = true;
                cbTableSection.Checked = true;
                cbTableSubsection.Checked = true;
                cbPathogenResistance.Checked = true;
                cbDosageGroup.Checked = true;
                cbDosage.Checked = true;
                cbAntimicrobial.Checked = true;
                cbPrecaution.Checked = true;
                cbSubsection.Checked = true;
                cbTopic.Checked = true;
                cbTableGroup.Checked = true;
                cbAttachment.Checked = true;
                cbSurgeryGroup.Checked = true;
                cbSurgery.Checked = true;
                cbSurgeryClassification.Checked = true;
                cbSubtopic.Checked = true;
                cbAntimicrobialGroup.Checked = true;
                cbMicroorganism.Checked = true;
                cbMicroorganismGroup.Checked = true;
                cbInfectionFrequency.Checked = true;
                cbAntimicrobialRisk.Checked = true;
                cbCondition.Checked = true;
                cbImmuneResponse.Checked = true;
                cbRegimen.Checked = true;
                cbConfiguredEntry.Checked = true;
                cbCombinedEntry.Checked = true;
            }
            dataGridView1.Columns.Clear();
        }

        private List<int> loadTypesFromUI()
        {
            List<int> typeList = new List<int>();
            if (cbChapter.Checked == true)
                typeList.Add(1);
            if (cbSection.Checked == true)
                typeList.Add(2);
            if(cbCategory.Checked == true)
                typeList.Add(3);
            if(cbSubcategory.Checked == true)
                typeList.Add(4);
            if(cbDisease.Checked == true)
                typeList.Add(5);
            if(cbPresentation.Checked == true)
                typeList.Add(6);
            if(cbPathogenGroup.Checked == true)
                typeList.Add(7);
            if(cbPathogen.Checked == true)
                typeList.Add(8);
            if(cbTherapyGroup.Checked == true)
                typeList.Add(9);
            if(cbTherapy.Checked == true)
                typeList.Add(10);
            if (cbTable.Checked == true)
                typeList.Add(12);
            if (cbTableSection.Checked == true)
                typeList.Add(13);
            if (cbTableSubsection.Checked == true)
                typeList.Add(14);
            if (cbPathogenResistance.Checked == true)
                typeList.Add(16);
            if (cbDosageGroup.Checked == true)
                typeList.Add(17);
            if (cbDosage.Checked == true)
                typeList.Add(18);
            if (cbAntimicrobial.Checked == true)
                typeList.Add(19);
            if (cbPrecaution.Checked == true)
                typeList.Add(20);
            if (cbSubsection.Checked == true)
                typeList.Add(21);
            if (cbTopic.Checked == true)
                typeList.Add(22);
            if (cbTableGroup.Checked == true)
                typeList.Add(24);
            if (cbAttachment.Checked == true)
                typeList.Add(25);
            if (cbSurgeryGroup.Checked == true)
                typeList.Add(26);
            if (cbSurgery.Checked == true) 
                typeList.Add(27);
            if (cbSurgeryClassification.Checked == true)
                typeList.Add(28);
            if (cbSubtopic.Checked == true)
                typeList.Add(29);
            if (cbAntimicrobialGroup.Checked == true)
                typeList.Add(30);
            if (cbMicroorganism.Checked == true)
                typeList.Add(31);
            if (cbMicroorganismGroup.Checked == true)
                typeList.Add(32);
            if (cbAntimicrobialRisk.Checked == true)
                typeList.Add(33);
            if (cbCondition.Checked == true)
                typeList.Add(34);
            if (cbImmuneResponse.Checked == true)
                typeList.Add(35);
            if (cbInfectionFrequency.Checked == true)
                typeList.Add(36);
            if (cbRegimen.Checked == true) 
                typeList.Add(37);
            if (cbConfiguredEntry.Checked == true)
                typeList.Add(38);
            if (cbCombinedEntry.Checked == true)
                typeList.Add(39);

            return typeList;
        }

        private void showMessage(IBDNode pNode)
        {
            MessageBox.Show(this, "No editor is currently available for the selected row", "Edit Requested");
        }

        private void openEditorForNode(IBDNode pNode)
        {
            switch (pNode.NodeType)
            {
                case BDConstants.BDNodeType.BDTherapy:
                    // open BDTherapyEditView
                    BDTherapyEditView therapyEditView = new BDTherapyEditView();
                    therapyEditView.CurrentTherapy = pNode as BDTherapy;
                    therapyEditView.AssignDataContext(dataContext);
                    therapyEditView.StartPosition = FormStartPosition.CenterParent;
                    therapyEditView.ShowDialog(this);
                    break;
                case BDConstants.BDNodeType.BDDosage:
                    showMessage(pNode);
                    break;
                case BDConstants.BDNodeType.BDPrecaution:
                    showMessage(pNode);
                    break;
                case BDConstants.BDNodeType.BDAntimicrobialRisk:
                    showMessage(pNode);
                    break;
                case BDConstants.BDNodeType.BDConfiguredEntry:
                    showMessage(pNode);
                    break;
                case BDConstants.BDNodeType.BDCombinedEntry:
                    showMessage(pNode);
                    break;
                case BDConstants.BDNodeType.BDTableRow:
                    showMessage(pNode);
                    break;
                default:
                    BDNodeEditView nodeEditView = new BDNodeEditView();
                    if (pNode.NodeType == BDConstants.BDNodeType.BDTherapyGroup)
                        nodeEditView.CurrentNode = pNode as BDTherapyGroup;
                    else if (pNode.NodeType == BDConstants.BDNodeType.BDAttachment)
                        nodeEditView.CurrentNode = pNode as BDAttachment;
                    else if (pNode.NodeType == BDConstants.BDNodeType.BDTableCell)
                        nodeEditView.CurrentNode = pNode as BDTableCell;
                    else
                        nodeEditView.CurrentNode = pNode as BDNode;
                    nodeEditView.AssignDataContext(dataContext);
                    nodeEditView.StartPosition = FormStartPosition.CenterParent;
                    nodeEditView.ShowDialog(this);
                    break;
            }

        }
    }
}
