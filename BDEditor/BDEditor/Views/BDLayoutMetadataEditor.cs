using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDLayoutMetadataEditor : Form
    {
        public Entities DataContext;

        public BDLayoutMetadataEditor()
        {
            InitializeComponent();
        }

        public BDLayoutMetadataEditor(Entities pDataContext)
        {
            InitializeComponent();
            DataContext = pDataContext;
        }

        private void BDLayoutMetadataEditor_Load(object sender, EventArgs e)
        {
            BDLayoutMetadata.Rebuild(DataContext);

            listBox1.Items.Clear();
            List<BDLayoutMetadata> allEntries = BDLayoutMetadata.RetrieveAll(DataContext, null);
            listBox1.Items.AddRange(allEntries.ToArray());

        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
