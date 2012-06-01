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

        public BDSearchView()
        {
            InitializeComponent();
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

        private void srch()
        {
            this.SuspendLayout();
            nodeList.Clear();
            linkedNoteList.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;

            var nodesToSearch = loadTypesFromUI();

            if (rbNodes.Checked == true && tbSearchTerm.Text.Length > 0)
            {
                nodeList.AddRange(BDFabrik.SearchNodesForText(this.dataContext, nodesToSearch, tbSearchTerm.Text.Trim()));
                if (nodeList.Count > 0)
                {
                    dataGridView1.DataSource = nodeList;
                    // set up grid columns
                    DataGridViewTextBoxColumn nodeClass = new DataGridViewTextBoxColumn();
                    dataGridView1.Columns.Add(nodeClass);
                    nodeClass.DataPropertyName = "NodeType";
                    nodeClass.HeaderText = "Type";
                    nodeClass.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                    DataGridViewTextBoxColumn nodeName = new DataGridViewTextBoxColumn();
                    dataGridView1.Columns.Add(nodeName);
                    nodeName.DataPropertyName = "Name";
                    nodeName.HeaderText = "Name";

                    DataGridViewButtonColumn editButtons = new DataGridViewButtonColumn();
                    dataGridView1.Columns.Add(editButtons);
                    editButtons.Name = "Edit";
                    editButtons.Text = "Edit";
                    editButtons.HeaderText = "";
                    editButtons.UseColumnTextForButtonValue = true;
                    editButtons.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

                    dataGridView1.Tag = "Node";
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
                    DataGridViewTextBoxColumn nodeName = new DataGridViewTextBoxColumn();
                    nodeName.DataPropertyName = "DescriptionForLinkedNote";
                    nodeName.HeaderText = "Description";
                    dataGridView1.Columns.Add(nodeName);

                    DataGridViewButtonColumn editButtons = new DataGridViewButtonColumn();
                    editButtons.Name = "Edit";
                    editButtons.Text = "Edit";
                    editButtons.HeaderText = "";
                    editButtons.UseColumnTextForButtonValue = true;
                    editButtons.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    dataGridView1.Columns.Add(editButtons);

                    dataGridView1.Tag = "LinkedNote";
                }
                else
                    MessageBox.Show(this, "No matching entries found", "Not Found", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }

            dataGridView1.Refresh();
            this.ResumeLayout();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            srch();
        }

        void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex !=
                dataGridView1.Columns["Edit"].Index) return;

            // Retrieve the node ID.
            if (dataGridView1.Tag.ToString() == "LinkedNote")
            {
                Guid linkedNoteId = (Guid)linkedNoteList[e.RowIndex].Uuid;

                BDLinkedNote note = BDLinkedNote.GetLinkedNoteWithId(dataContext, linkedNoteId);
                List<BDLinkedNoteAssociation> assns = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForLinkedNoteId(dataContext, linkedNoteId);
                BDLinkedNoteView noteView = new BDLinkedNoteView();
                noteView.AssignDataContext(dataContext);
                noteView.AssignParentInfo(assns[0].parentId, assns[0].ParentType);
                noteView.AssignContextPropertyName(assns[0].parentKeyPropertyName);
                noteView.AssignScopeId(assns[0].parentId);
                noteView.ShowDialog(this);
                srch();
            }
            else
            {
                Guid nodeId = (Guid)nodeList[e.RowIndex].Uuid;
                if (((IBDNode)nodeList[e.RowIndex]).NodeType == BDConstants.BDNodeType.BDTherapy)
                {
                    // open BDTherapyEditView
                    BDTherapyEditView therapyEditView = new BDTherapyEditView();
                    therapyEditView.CurrentTherapy = BDTherapy.RetrieveTherapyWithId(dataContext,nodeId);
                    therapyEditView.AssignDataContext(dataContext);
                    therapyEditView.ShowDialog(this);

                }
                else
                {
                    BDNodeEditView nodeEditView = new BDNodeEditView();
                    if (((IBDNode)nodeList[e.RowIndex]).NodeType == BDConstants.BDNodeType.BDTherapyGroup)
                        nodeEditView.CurrentNode = BDTherapyGroup.RetrieveTherapyGroupWithId(dataContext, nodeId);
                    else
                        nodeEditView.CurrentNode = BDNode.RetrieveNodeWithId(dataContext, nodeId);
                    nodeEditView.AssignDataContext(dataContext);
                    nodeEditView.ShowDialog(this);
                }
                srch();
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
            }
            dataGridView1.Columns.Clear();
        }

        private List<int> loadTypesFromUI()
        {
            List<int> typeList = new List<int>();
            //new List<int>() {1,2,3,4,5,6,7,8,9,10};
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

            return typeList;
        }
    }
}
