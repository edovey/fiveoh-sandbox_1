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
            if (currentPathogenGroup != null)
                AssignPathogensToView();
        }

        private void BDPathogenGroupControl_Load(object sender, EventArgs e)
        {

        }

        private void AssignPathogensToView()
        {
            List<BDPathogen> list = BDPathogen.GetPathogensForPathogenGroup(dataContext,currentPathogenGroup.uuid);
            if (list != null)
            {
                if (list.Count >= 1)
                    bdPathogenControl1.CurrentPathogen = list[0];
                if (list.Count >= 2)
                {
                    bdPathogenControl2.CurrentPathogen = list[1];
                    bdPathogenControl2.Title = @"Pathogen 2";
                }
                if (list.Count >= 3)
                {
                    bdPathogenControl3.CurrentPathogen = list[2];
                    bdPathogenControl3.Title = @"Pathogen 3";
                }
                if (list.Count >= 4)
                {
                    bdPathogenControl4.CurrentPathogen = list[3];
                    bdPathogenControl4.Title = @"Pathogen 4";
                }
                if (list.Count >= 5)
                {
                    bdPathogenControl5.CurrentPathogen = list[4];
                    bdPathogenControl5.Title = @"Pathogen 5";
                }
                if (list.Count >= 6)
                {
                    bdPathogenControl6.CurrentPathogen = list[5];
                    bdPathogenControl6.Title = @"Pathogen 6";
                }
                if (list.Count >= 7)
                {
                    bdPathogenControl7.CurrentPathogen = list[6];
                    bdPathogenControl7.Title = @"Pathogen 7";
                }
                if (list.Count >= 8)
                {
                    bdPathogenControl8.CurrentPathogen = list[7];
                    bdPathogenControl8.Title = @"Pathogen 8";
                }
            }
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
