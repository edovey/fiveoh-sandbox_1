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
    public partial class BDNodeOverviewControl : BDNodeControl
    {
        private BDLinkedNote overviewLinkedNote;
        private BDLinkedNoteControl bdLinkedNoteControl1;

        public BDNodeOverviewControl()
        {
            InitializeComponent();
            LayoutControls();
        }

        public BDNodeOverviewControl(Entities pDataContext, IBDNode pNode): base(pDataContext, pNode)
        {
            LayoutControls();
        }

        private void LayoutControls()
        {
            this.bdLinkedNoteControl1 = new BDEditor.Views.BDLinkedNoteControl();
            this.bdLinkedNoteControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bdLinkedNoteControl1.CurrentLinkedNote = null;
            this.bdLinkedNoteControl1.DefaultNodeType = BDEditor.Classes.BDConstants.BDNodeType.None;
            this.bdLinkedNoteControl1.DisplayOrder = null;
            this.bdLinkedNoteControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bdLinkedNoteControl1.Location = new System.Drawing.Point(0, 0);
            this.bdLinkedNoteControl1.Name = "bdLinkedNoteControl1";
            this.bdLinkedNoteControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdLinkedNoteControl1.SaveOnLeave = true;
            this.bdLinkedNoteControl1.ShowAsChild = false;
            this.bdLinkedNoteControl1.Size = new System.Drawing.Size(870, 467);
            this.bdLinkedNoteControl1.TabIndex = 4;

            base.pnlOverview.Size = new System.Drawing.Size(870, 467);
            base.pnlOverview.Controls.Add(this.bdLinkedNoteControl1);
        }

        public override void RefreshLayout(bool pShowChildren)
        {
            base.RefreshLayout(pShowChildren);

            ControlHelper.SuspendDrawing(this);
            bdLinkedNoteControl1.AssignDataContext(dataContext);
            if (currentNode == null)
            {
                overviewLinkedNote = null;

                bdLinkedNoteControl1.CurrentLinkedNote = null;
                bdLinkedNoteControl1.AssignParentInfo(null, DefaultNodeType);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
            }
            else
            {
                bdLinkedNoteControl1.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextPropertyName(BDNode.VIRTUALPROPERTYNAME_OVERVIEW);

                BDLinkedNoteAssociation association = BDLinkedNoteAssociation.GetLinkedNoteAssociationForParentIdAndProperty(dataContext, currentNode.Uuid, BDNode.VIRTUALPROPERTYNAME_OVERVIEW);
                if (null != association)
                {
                    overviewLinkedNote = BDLinkedNote.GetLinkedNoteWithId(dataContext, association.linkedNoteId);
                    bdLinkedNoteControl1.CurrentLinkedNote = overviewLinkedNote;
                }
            }

            System.Drawing.Size textControlSize;
            if (base.showAsChild)
                textControlSize = new System.Drawing.Size(870, 225);
            else
                textControlSize = new System.Drawing.Size(870, 467);
            this.bdLinkedNoteControl1.Size = textControlSize;
            base.pnlOverview.Size = textControlSize;

            bdLinkedNoteControl1.RefreshLayout();

            ControlHelper.ResumeDrawing(this);
        }
    }
}
