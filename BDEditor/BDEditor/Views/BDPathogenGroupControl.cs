using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;

namespace BDEditor.Views
{
    public partial class BDPathogenGroupControl : UserControl, IBDControl
    {
        #region Class properties
        private Entities dataContext;
        private BDPathogenGroup currentPathogenGroup;
        private List<BDPathogen> pathogenList;

        public BDPathogenGroup CurrentPathogenGroup
        {
            get
            {
                return currentPathogenGroup;
            }
            set
            {
                currentPathogenGroup = value;

                

            }
        }

        public List<BDPathogen> PathogenList
        {
            get
            {
                return pathogenList;
            }

            set
            {
                pathogenList = value;
                AssignPathogensToView();
            }
        }

        #endregion

        public BDPathogenGroupControl()
        {
            InitializeComponent();
            AssignPathogensToView();
        }

        private void BDPathogenGroupControl_Load(object sender, EventArgs e)
        {

        }

        private void AssignPathogensToView()
        {

        }

        private List<BDLinkedNote> GetLinkedNotesForPathogen(BDPathogen pPathogen)
        {
            List<BDLinkedNote> linkedNoteList = BDLinkedNote.GetLinkedNotesForParentId(dataContext, pPathogen.uuid);
            if (linkedNoteList.Count == 0)
                return null;
            else
                return linkedNoteList;
        }

        #region IBDControl
        
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public void Save()
        {

        }

        #endregion
    }
}
