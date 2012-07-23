using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDCombinedEntryFieldControl : UserControl
    {
        protected Entities dataContext;
        protected BDCombinedEntry currentEntry;
        protected Guid? scopeId;
        protected Guid parentId;
        protected BDConstants.BDNodeType parentType = BDConstants.BDNodeType.None;

        private int? displayOrder;
        public BDCombinedEntryFieldControl()
        {
            InitializeComponent();
        }

        public BDCombinedEntryFieldControl(Entities pDataContext, BDCombinedEntry pCombinedEntry, string pPropertyName, Guid? pScopeId)
        {
            InitializeComponent();

            if (null == pCombinedEntry) throw new NotSupportedException("May not create a CombinedEntryField control without an existing entry");
            if (null == pCombinedEntry.ParentId) throw new NotSupportedException("May not create a CombinedEntryField control without a supplied parent");

            dataContext = pDataContext;
            currentEntry = pCombinedEntry;
            parentId = currentEntry.ParentId.Value;
            scopeId = pScopeId;
        }
    }
}
