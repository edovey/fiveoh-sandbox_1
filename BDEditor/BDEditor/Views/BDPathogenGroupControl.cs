﻿using System;
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
                    this.BackColor = SystemColors.ControlDark;
                    pathogenList = new List<BDPathogen>();
                    AssignPathogensToView(null);
                }
                else
                {
                    pathogenList = BDPathogen.GetPathogensForPathogenGroup(dataContext, currentPathogenGroup.uuid);
                    AssignPathogensToView(pathogenList);
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
                AssignPathogensToView(pathogenList);
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

        private void AssignPathogensToView(List<BDPathogen> pListPathogens)
        {
            bdPathogenControl1.CurrentPathogen = null;
            bdPathogenControl2.CurrentPathogen = null;
            bdPathogenControl3.CurrentPathogen = null;
            bdPathogenControl4.CurrentPathogen = null;
            bdPathogenControl5.CurrentPathogen = null;
            bdPathogenControl6.CurrentPathogen = null;
            bdPathogenControl7.CurrentPathogen = null;
            bdPathogenControl8.CurrentPathogen = null;

            if (null != pListPathogens)
            {
                bdPathogenControl1.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl2.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl3.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl4.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl5.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl6.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl7.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl8.AssignParentId(currentPathogenGroup.uuid);

                if (pListPathogens.Count >= 1) bdPathogenControl1.CurrentPathogen = pListPathogens[0];
                if (pListPathogens.Count >= 2) bdPathogenControl2.CurrentPathogen = pListPathogens[1];
                if (pListPathogens.Count >= 3) bdPathogenControl3.CurrentPathogen = pListPathogens[2];
                if (pListPathogens.Count >= 4) bdPathogenControl4.CurrentPathogen = pListPathogens[3];
                if (pListPathogens.Count >= 5) bdPathogenControl5.CurrentPathogen = pListPathogens[4];
                if (pListPathogens.Count >= 6) bdPathogenControl6.CurrentPathogen = pListPathogens[5];
                if (pListPathogens.Count >= 7) bdPathogenControl7.CurrentPathogen = pListPathogens[6];
                if (pListPathogens.Count >= 8) bdPathogenControl8.CurrentPathogen = pListPathogens[7];
            }
            else
            {
                bdPathogenControl1.AssignParentId(null);
                bdPathogenControl2.AssignParentId(null);
                bdPathogenControl3.AssignParentId(null);
                bdPathogenControl4.AssignParentId(null);
                bdPathogenControl5.AssignParentId(null);
                bdPathogenControl6.AssignParentId(null);
                bdPathogenControl7.AssignParentId(null);
                bdPathogenControl8.AssignParentId(null);
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

                if (result && (null == currentPathogenGroup)) // only create a group if any of the children exist
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
