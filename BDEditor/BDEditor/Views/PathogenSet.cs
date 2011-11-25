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
    public partial class PathogenSet : UserControl, IBDControl
    {
        private Entities dataContext;
        private Guid? pathogenGroupId;
        private BDPathogenGroup currentPathogenGroup;
        private Guid? scopeId;

        public PathogenSet()
        {
            InitializeComponent();
        }

        public BDPathogenGroup CurrentPathogenGroup
        {
            get {  return currentPathogenGroup; }
            set { currentPathogenGroup = value; }
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
                if (pListPathogens.Count >= 1) bdPathogenControl1.CurrentPathogen = pListPathogens[0];
                if (pListPathogens.Count >= 2) bdPathogenControl2.CurrentPathogen = pListPathogens[1];
                if (pListPathogens.Count >= 3) bdPathogenControl3.CurrentPathogen = pListPathogens[2];
                if (pListPathogens.Count >= 4) bdPathogenControl4.CurrentPathogen = pListPathogens[3];
                if (pListPathogens.Count >= 5) bdPathogenControl5.CurrentPathogen = pListPathogens[4];
                if (pListPathogens.Count >= 6) bdPathogenControl6.CurrentPathogen = pListPathogens[5];
                if (pListPathogens.Count >= 7) bdPathogenControl6.CurrentPathogen = pListPathogens[6];
                if (pListPathogens.Count >= 8) bdPathogenControl6.CurrentPathogen = pListPathogens[7];
            }

            if (null == currentPathogenGroup)
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
            else
            {
                bdPathogenControl1.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl2.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl3.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl4.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl5.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl6.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl7.AssignParentId(currentPathogenGroup.uuid);
                bdPathogenControl8.AssignParentId(currentPathogenGroup.uuid);
            }

            bdPathogenControl1.RefreshLayout();
            bdPathogenControl2.RefreshLayout();
            bdPathogenControl3.RefreshLayout();
            bdPathogenControl4.RefreshLayout();
            bdPathogenControl5.RefreshLayout();
            bdPathogenControl6.RefreshLayout();
            bdPathogenControl7.RefreshLayout();
            bdPathogenControl8.RefreshLayout();
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
            bdPathogenControl1.AssignScopeId(scopeId);
            bdPathogenControl2.AssignScopeId(scopeId);
            bdPathogenControl3.AssignScopeId(scopeId);
            bdPathogenControl4.AssignScopeId(scopeId);
            bdPathogenControl5.AssignScopeId(scopeId);
            bdPathogenControl6.AssignScopeId(scopeId);
            bdPathogenControl7.AssignScopeId(scopeId);
            bdPathogenControl8.AssignScopeId(scopeId);
        }

        public void AssignTypeaheadSource()
        {
            string[] pathogenNameList = BDPathogen.GetPathogenNames(dataContext);
            AutoCompleteStringCollection nameCollection = new AutoCompleteStringCollection();
            nameCollection.AddRange(pathogenNameList);
            bdPathogenControl1.AssignTypeaheadSource(nameCollection);
            bdPathogenControl2.AssignTypeaheadSource(nameCollection);
            bdPathogenControl3.AssignTypeaheadSource(nameCollection);
            bdPathogenControl4.AssignTypeaheadSource(nameCollection);
            bdPathogenControl5.AssignTypeaheadSource(nameCollection);
            bdPathogenControl6.AssignTypeaheadSource(nameCollection);
            bdPathogenControl7.AssignTypeaheadSource(nameCollection);
            bdPathogenControl8.AssignTypeaheadSource(nameCollection);
        }

        #region IBDControl
        
        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
            bdPathogenControl1.AssignDataContext(dataContext);
            bdPathogenControl2.AssignDataContext(dataContext);
            bdPathogenControl3.AssignDataContext(dataContext);
            bdPathogenControl4.AssignDataContext(dataContext);
            bdPathogenControl5.AssignDataContext(dataContext);
            bdPathogenControl6.AssignDataContext(dataContext);
            bdPathogenControl7.AssignDataContext(dataContext);
            bdPathogenControl8.AssignDataContext(dataContext);
        }

        public void AssignParentId(Guid? pParentId)
        {
            pathogenGroupId = pParentId;
        }

        public bool Save()
        {
            bool result = false;

            if (null != pathogenGroupId)
            {
                result = bdPathogenControl1.Save() || result;
                result = bdPathogenControl2.Save() || result;
                result = bdPathogenControl3.Save() || result;
                result = bdPathogenControl4.Save() || result;
                result = bdPathogenControl5.Save() || result;
                result = bdPathogenControl6.Save() || result;
                result = bdPathogenControl7.Save() || result;
                result = bdPathogenControl8.Save() || result;

            }
            return result;
        }

        public void Delete()
        {
        }

        public bool CreateCurrentObject()
        {
            throw new NotImplementedException();
        }

        public void RefreshLayout()
        {
            if (null == currentPathogenGroup)
            {
                AssignPathogensToView(null);
            }
            else
            {
                List<BDPathogen> pathogenList = BDPathogen.GetPathogensForPathogenGroup(dataContext, currentPathogenGroup.uuid);
                AssignPathogensToView(pathogenList);
            }
        }

        /*
        public void AssignParentControl(IBDControl pControl)
        {
            parentControl = pControl;
        }

        public void TriggerCreateAndAssignParentIdToChildControl(IBDControl pControl)
        {
            if(null != parentControl)
            {
                parentControl.TriggerCreateAndAssignParentIdToChildControl(pControl);
            }
        }
        */

        #endregion    
    

        
    }    
}
