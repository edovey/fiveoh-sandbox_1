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
        private Guid? presentationId;
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
                if (null == currentPathogenGroup)
                {
                    pathogenList = new List<BDPathogen>();

                    bdPathogenControl1.CurrentPathogen = null;
                    bdPathogenControl2.CurrentPathogen = null;
                    bdPathogenControl3.CurrentPathogen = null;
                    bdPathogenControl4.CurrentPathogen = null;
                    bdPathogenControl5.CurrentPathogen = null;
                    bdPathogenControl6.CurrentPathogen = null;
                    bdPathogenControl7.CurrentPathogen = null;
                    bdPathogenControl8.CurrentPathogen = null;
                }
                else
                {
                    pathogenList = BDPathogen.GetPathogensForPathogenGroup(dataContext, currentPathogenGroup.uuid);
                }
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
        }

        private void BDPathogenGroupControl_Load(object sender, EventArgs e)
        {

        }

        private void AssignPathogensToView()
        {
            //List<BDPathogen> list = BDPathogen.GetPathogensForPathogenGroup(dataContext, currentPathogenGroup.uuid);
            if (null != pathogenList)
            {
                if (pathogenList.Count >= 1)
                {
                    bdPathogenControl1.CurrentPathogen = pathogenList[0];
                    bdPathogenControl1.Title = @"Pathogen 1";
                    
                }
                if (pathogenList.Count >= 2)
                {
                    bdPathogenControl2.CurrentPathogen = pathogenList[1];
                    bdPathogenControl2.Title = @"Pathogen 2";
                }
                if (pathogenList.Count >= 3)
                {
                    bdPathogenControl3.CurrentPathogen = pathogenList[2];
                    bdPathogenControl3.Title = @"Pathogen 3";
                }
                if (pathogenList.Count >= 4)
                {
                    bdPathogenControl4.CurrentPathogen = pathogenList[3];
                    bdPathogenControl4.Title = @"Pathogen 4";
                }
                if (pathogenList.Count >= 5)
                {
                    bdPathogenControl5.CurrentPathogen = pathogenList[4];
                    bdPathogenControl5.Title = @"Pathogen 5";
                }
                if (pathogenList.Count >= 6)
                {
                    bdPathogenControl6.CurrentPathogen = pathogenList[5];
                    bdPathogenControl6.Title = @"Pathogen 6";
                }
                if (pathogenList.Count >= 7)
                {
                    bdPathogenControl7.CurrentPathogen = pathogenList[6];
                    bdPathogenControl7.Title = @"Pathogen 7";
                }
                if (pathogenList.Count >= 8)
                {
                    bdPathogenControl8.CurrentPathogen = pathogenList[7];
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

        public void AssignParentId(Guid? pParentId)
        {
            presentationId = pParentId;
            this.Enabled = (null != presentationId);
            bdPathogenControl1.AssignParentId(presentationId);
            bdPathogenControl2.AssignParentId(presentationId);
            bdPathogenControl3.AssignParentId(presentationId);
            bdPathogenControl4.AssignParentId(presentationId);
            bdPathogenControl5.AssignParentId(presentationId);
            bdPathogenControl6.AssignParentId(presentationId);
            bdPathogenControl7.AssignParentId(presentationId);
            bdPathogenControl8.AssignParentId(presentationId);
        }

        public bool Save()
        {
            bool result = false;

            if (null != presentationId)
            {
                result = bdPathogenControl1.Save() || result;
                result = bdPathogenControl2.Save() || result;
                result = bdPathogenControl3.Save() || result;
                result = bdPathogenControl4.Save() || result;
                result = bdPathogenControl5.Save() || result;
                result = bdPathogenControl6.Save() || result;
                result = bdPathogenControl7.Save() || result;
                result = bdPathogenControl8.Save() || result;

                if (result && (null == currentPathogenGroup))
                {
                    currentPathogenGroup = BDPathogenGroup.CreatePathogenGroup(dataContext);
                    currentPathogenGroup.presentationId = presentationId;
                }

                if (null != currentPathogenGroup)
                {
                    BDPathogenGroup.SavePathogenGroup(dataContext, currentPathogenGroup);
                    result = true;
                }
            }
            return result;
        }

        #endregion
    }
}
