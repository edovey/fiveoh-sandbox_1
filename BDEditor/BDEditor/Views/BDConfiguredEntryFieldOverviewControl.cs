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
    public partial class BDConfiguredEntryFieldOverviewControl : BDConfiguredEntryFieldControl
    {
        private BDLinkedNote overviewLinkedNote;

        public BDConfiguredEntryFieldOverviewControl()
        {
            InitializeComponent();
            bdLinkedNoteControl1.DefaultLinkedNoteType = BDConstants.LinkedNoteType.Inline;
        }

        public BDConfiguredEntryFieldOverviewControl(Entities pDataContext, BDConfiguredEntry pConfiguredEntry, string pFieldName, Guid pScopeId): base(pDataContext,pConfiguredEntry, pFieldName, pScopeId)
        {
            InitializeComponent();
            bdLinkedNoteControl1.DefaultLinkedNoteType = BDConstants.LinkedNoteType.Inline;
        }

        public override void RefreshLayout()
        {
            base.RefreshLayout();

            string fieldNoteContextPropertyName = string.Format("{0}_fieldNote", fieldName);

            Boolean origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);
            bdLinkedNoteControl1.AssignDataContext(dataContext);
            bdLinkedNoteControl1.AssignContextPropertyName(fieldNoteContextPropertyName);
            bdLinkedNoteControl1.AssignScopeId(scopeId);
            bdLinkedNoteControl1.DefaultLinkedNoteType = BDConstants.LinkedNoteType.Inline;

            if (configuredEntry == null)
            {
                overviewLinkedNote = null;

                bdLinkedNoteControl1.CurrentLinkedNote = null;
                bdLinkedNoteControl1.AssignParentInfo(null, BDConstants.BDNodeType.BDConfiguredEntry);             
            }
            else
            {
                bdLinkedNoteControl1.AssignParentInfo(configuredEntry.Uuid, configuredEntry.NodeType);

                //BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentNode.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
                List<BDLinkedNoteAssociation> associationList = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(dataContext, configuredEntry.Uuid, fieldNoteContextPropertyName);
                if ((null != associationList) && (associationList.Count > 0))
                {
                    BDLinkedNoteAssociation association = associationList[0];
                    overviewLinkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(dataContext, association.linkedNoteId);
                    bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                }
            }

            //System.Drawing.Size textControlSize;

            //textControlSize = new System.Drawing.Size(870, 467);
            //this.bdLinkedNoteControl1.Size = textControlSize;
            //base.pnlOverview.Size = textControlSize;

            bdLinkedNoteControl1.RefreshLayout();

            ControlHelper.ResumeDrawing(this);

            BDCommon.Settings.IsUpdating = origState;
        }

    }
}
