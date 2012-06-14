using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDRestoreDialog : Form
    {
        private List<BDArchiveRecord> _archiveRecordList = new List<BDArchiveRecord>();
        private BDArchiveRecord _selectedArchiveRecord = null;

        public BDRestoreDialog()
        {
            InitializeComponent();
        }

        public List<BDArchiveRecord> ArchiveRecordList
        {
            get { return _archiveRecordList; }
            set 
            { 
                _archiveRecordList = value;
                listBoxArchives.Items.Clear();
                listBoxArchives.Items.AddRange(_archiveRecordList.ToArray());
            }
        }

        public BDArchiveRecord SelectedArchiveRecord
        { 
            get { return _selectedArchiveRecord; }
            set { _selectedArchiveRecord = value; }
        }

        private void listBoxArchives_Click(object sender, EventArgs e)
        {
            btnRestore.Enabled = (listBoxArchives.SelectedIndex >= 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _selectedArchiveRecord = null;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (listBoxArchives.SelectedIndex >= 0)
            {
                _selectedArchiveRecord = listBoxArchives.SelectedItem as BDArchiveRecord;
            }
            else
            {
                _selectedArchiveRecord = null;
            }
        }
    }
}
