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
    public partial class BDNodeEditView : Form
    {
        private Entities dataContext;
        private IBDNode currentNode;

        public BDNodeEditView()
        {
            InitializeComponent();
        }

        private void BDNodeEditView_Load(object sender, EventArgs e)
        {
            tbName.Text = currentNode.Name;
        }

        public IBDNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            save();
            //TODO:  refresh the tree & main edit view
            this.Close();
        }
        private void insertText(TextBox pTextBox, string pText)
        {
            int x = pTextBox.SelectionStart;
            pTextBox.Text = pTextBox.Text.Insert(pTextBox.SelectionStart, pText);
            pTextBox.SelectionStart = x + 1;
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "ß");
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "°");
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "µ");
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "≥");
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "≤");
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "±");
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertText(tbName, "¹");
        }

        private bool save()
        {
            bool result = false;

            if (null != currentNode)
            {
                if (currentNode.Name != tbName.Text) currentNode.Name = tbName.Text.Trim();

                if (currentNode.NodeType == BDConstants.BDNodeType.BDTherapyGroup)
                    BDTherapyGroup.Save(dataContext, (currentNode as BDTherapyGroup));
                else
                    BDNode.Save(dataContext, (currentNode as BDNode));
                result = true;
            }

            return result;
        }
    }
}
