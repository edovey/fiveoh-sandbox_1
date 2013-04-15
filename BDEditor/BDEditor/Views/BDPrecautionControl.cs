using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BDEditor.DataModel;
using System.Diagnostics;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDPrecautionControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private BDPrecaution currentPrecaution;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private string currentControlName;
        private BDLinkedNote durationLinkedNote;
        private BDLinkedNoteControl bdLinkedNoteControl1;
        private bool isUpdating = false;

        private const string INFECTIVE_MATERIAL_RTB = "Infective Material";
        private const string MODE_OF_TRANSMISSION_RTB = "Mode of Transmission";
        private const string SINGLE_ROOM_ACUTE_RTB = "Single Room Acute";
        private const string SINGLE_ROOM_LONG_TERM_RTB = "Single Room Long Term";
        private const string GLOVES_ACUTE_RTB = "Gloves Acute";
        private const string GLOVES_LONG_TERM_RTB = "Gloves Long Term";
        private const string GOWNS_ACUTE_RTB = "Gowns Acute";
        private const string GOWNS_LONG_TERM_RTB = "Gowns Long Term";
        private const string MASK_ACUTE_RTB = "Mask Acute";
        private const string MASK_LONG_TERM_RTB = "Mask Long Term";

        public int? DisplayOrder { get; set; }

        private bool showChildren = true;
        public bool ShowChildren
        {
            get { return showChildren; }
            set { showChildren = value; }
        }

        public event EventHandler<NodeEventArgs> RequestItemAdd;
        public event EventHandler<NodeEventArgs> RequestItemDelete;

        public event EventHandler<NodeEventArgs> ReorderToPrevious;
        public event EventHandler<NodeEventArgs> ReorderToNext;

        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NameChanged;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnNotesChanged(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = NotesChanged;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnItemAddRequested(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = RequestItemAdd;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnItemDeleteRequested(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = RequestItemDelete;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnReorderToPrevious(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = ReorderToPrevious;
            if (null != handler) { handler(this, e); }
        }

        protected virtual void OnReorderToNext(NodeEventArgs e)
        {
            EventHandler<NodeEventArgs> handler = ReorderToNext;
            if (null != handler) { handler(this, e); }
        }

        public BDPrecaution CurrentPrecaution
        {
            get { return currentPrecaution; }
            set { currentPrecaution = value; }
        }

        public BDPrecautionControl()
        {
            InitializeComponent();
        }

        public BDPrecautionControl(Entities pDataContext, IBDNode pNode)
        {
            dataContext = pDataContext;
            currentPrecaution = pNode as BDPrecaution;
            parentId = pNode.ParentId;
            DefaultNodeType = pNode.NodeType;
            DefaultLayoutVariantType = BDConstants.LayoutVariantType.Prophylaxis_InfectionPrecautions;
            
            InitializeComponent();
            LayoutControls();
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentPrecaution) ? this.currentPrecaution.uuid : Guid.Empty);
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
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
            this.bdLinkedNoteControl1.AssignDataContext(dataContext);
            this.bdLinkedNoteControl1.AssignContextPropertyName(BDPrecaution.PROPERTYNAME_DURATION);

            this.pnlLinkedNote.Controls.Add(this.bdLinkedNoteControl1);
        }

        #region IBDControl

        public void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            isUpdating = true;
            ControlHelper.SuspendDrawing(this);
            if (currentPrecaution == null)
            {
                rtbInfectiveMaterial.Text = @"";
                rtbModeOfTransmission.Text = @"";
                rtbSingleRoomAcute.Text = @"";
                rtbSingleRoomLongTerm.Text = @"";
                rtbGlovesAcute.Text = @"";
                rtbGlovesLongTerm.Text = @"";
                rtbGownsAcute.Text = @"";
                rtbGownsLongTerm.Text = @"";
                rtbMaskAcute.Text = @"";
                rtbMaskLongTerm.Text = @"";

                durationLinkedNote = null;

                bdLinkedNoteControl1.CurrentLinkedNote = null;
                bdLinkedNoteControl1.AssignParentInfo(null, DefaultNodeType);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextPropertyName(BDPrecaution.PROPERTYNAME_DURATION);
                bdLinkedNoteControl1.AssignDataContext(dataContext);

            }
            else
            {
                this.BackColor = SystemColors.Control;

                rtbInfectiveMaterial.Text = currentPrecaution.infectiveMaterial;
                rtbModeOfTransmission.Text = currentPrecaution.modeOfTransmission;
                rtbSingleRoomAcute.Text = currentPrecaution.singleRoomAcute;
                rtbSingleRoomLongTerm.Text = currentPrecaution.singleRoomLongTerm;
                rtbGlovesAcute.Text = currentPrecaution.glovesAcute;
                rtbGlovesLongTerm.Text = currentPrecaution.glovesLongTerm;
                rtbGownsAcute.Text = currentPrecaution.gownsAcute;
                rtbGownsLongTerm.Text = currentPrecaution.gownsLongTerm;
                rtbMaskAcute.Text = currentPrecaution.maskAcute;
                rtbMaskLongTerm.Text = currentPrecaution.maskLongTerm;

                DisplayOrder = currentPrecaution.displayOrder;

                bdLinkedNoteControl1.AssignParentInfo(currentPrecaution.Uuid, currentPrecaution.NodeType);
                bdLinkedNoteControl1.AssignScopeId(scopeId);
                bdLinkedNoteControl1.AssignContextPropertyName(BDPrecaution.PROPERTYNAME_DURATION);

                List<BDLinkedNoteAssociation> associationList = BDLinkedNoteAssociation.GetLinkedNoteAssociationListForParentIdAndProperty(dataContext, currentPrecaution.Uuid, BDPrecaution.PROPERTYNAME_DURATION); ;
                if ((null != associationList) && (associationList.Count > 0))
                {
                    BDLinkedNoteAssociation association = associationList[0];
                    durationLinkedNote = BDLinkedNote.RetrieveLinkedNoteWithId(dataContext, association.linkedNoteId);
                    bdLinkedNoteControl1.CurrentLinkedNote = durationLinkedNote;
                }
                bdLinkedNoteControl1.RefreshLayout();
            }

            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);
            isUpdating = false;

        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentPrecaution)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    currentPrecaution = BDPrecaution.CreateBDPrecaution(this.dataContext, this.parentId.Value);
                    currentPrecaution.SetParent(parentType, parentId);
                    currentPrecaution.displayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    currentPrecaution.LayoutVariant = DefaultLayoutVariantType;
                }
            }

            return result;
        }

        public bool Save()
        {
            bool result = false;
            if (null != parentId)
            {
                if ((null == currentPrecaution) &&
                    (rtbInfectiveMaterial.Text != string.Empty) ||
                    (rtbModeOfTransmission.Text != string.Empty) ||
                    (rtbSingleRoomAcute.Text != string.Empty) ||
                    (rtbSingleRoomLongTerm.Text != string.Empty) ||
                    (rtbGlovesAcute.Text != string.Empty) ||
                    (rtbGlovesLongTerm.Text != string.Empty) ||
                    (rtbGownsAcute.Text != string.Empty) ||
                    (rtbGownsLongTerm.Text != string.Empty) ||
                    (rtbMaskAcute.Text != string.Empty) ||
                    (rtbMaskLongTerm.Text != string.Empty) 
                    )
                {
                    CreateCurrentObject();
                }

                if (null != currentPrecaution)
                {
                    if (currentPrecaution.infectiveMaterial != rtbInfectiveMaterial.Text) currentPrecaution.infectiveMaterial = rtbInfectiveMaterial.Text;
                    if (currentPrecaution.modeOfTransmission != rtbModeOfTransmission.Text) currentPrecaution.modeOfTransmission = rtbModeOfTransmission.Text;
                    if (currentPrecaution.singleRoomAcute != rtbSingleRoomAcute.Text) currentPrecaution.singleRoomAcute = rtbSingleRoomAcute.Text;
                    if (currentPrecaution.singleRoomLongTerm != rtbSingleRoomLongTerm.Text) currentPrecaution.singleRoomLongTerm = rtbSingleRoomLongTerm.Text;
                    if (currentPrecaution.glovesAcute != rtbGlovesAcute.Text) currentPrecaution.glovesAcute = rtbGlovesAcute.Text;
                    if (currentPrecaution.glovesLongTerm != rtbGlovesLongTerm.Text) currentPrecaution.glovesLongTerm = rtbGlovesLongTerm.Text;
                    if (currentPrecaution.gownsAcute != rtbGownsAcute.Text) currentPrecaution.gownsAcute = rtbGownsAcute.Text;
                    if (currentPrecaution.gownsLongTerm != rtbGownsLongTerm.Text) currentPrecaution.gownsLongTerm = rtbGownsLongTerm.Text;
                    if (currentPrecaution.maskAcute != rtbMaskAcute.Text) currentPrecaution.maskAcute = rtbMaskAcute.Text;
                    if (currentPrecaution.maskLongTerm != rtbMaskLongTerm.Text) currentPrecaution.maskLongTerm = rtbMaskLongTerm.Text;

                    BDPrecaution.Save(dataContext, currentPrecaution);
                    result = true;
                }
                bdLinkedNoteControl1.Save();
            }

            return result;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void AssignParentInfo(Guid? pParentId, BDConstants.BDNodeType pParentType)
        {
            parentId = pParentId;
            parentType = pParentType;
            this.Enabled = (null != parentId);
        }

        #endregion

        private void createLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();

                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(BDPrecaution.VIRTUALPROPERTYNAME_PRECAUTIONS);
                view.AssignParentInfo(currentPrecaution.Uuid, currentPrecaution.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private void insertTextFromMenu(string textToInsert)
        {
            RichTextBox rtb = getCurrentControl();

            int pos = rtb.SelectionStart;
            rtb.Text = rtb.Text.Insert(rtb.SelectionStart, textToInsert);
            rtb.SelectionStart = textToInsert.Length + pos;
        }

        private void toggleLinkButtonEnablement()
        {
            bool enabled = ((rtbInfectiveMaterial.Text.Length > 0) || (rtbModeOfTransmission.Text.Length > 0) || (rtbSingleRoomAcute.Text.Length > 0) || (rtbSingleRoomLongTerm.Text.Length > 0) ||
            (rtbGlovesAcute.Text.Length > 0) || (rtbGlovesLongTerm.Text.Length > 0) || (rtbGownsAcute.Text.Length > 0) || (rtbGownsLongTerm.Text.Length > 0) ||
            (rtbMaskAcute.Text.Length > 0) || (rtbMaskLongTerm.Text.Length > 0));
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            toggleLinkButtonEnablement();
        }

        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "ß";
            insertTextFromMenu(newText);
        }

        private void degreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "°";
            insertTextFromMenu(newText);
        }

        private void µToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "µ";

            insertTextFromMenu(newText);
        }

        private void geToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≥";
            insertTextFromMenu(newText);
        }

        private void leToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "≤";
            insertTextFromMenu(newText);
        }

        private void plusMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "±";
            insertTextFromMenu(newText);
        }

        private void sOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "¹";
            insertTextFromMenu(newText);
        }

        public override string ToString()
        {
            return (null == this.currentPrecaution) ? "No Infection Precaution" : this.currentPrecaution.infectiveMaterial;
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }

        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get
            {
                return CurrentPrecaution;
            }
            set
            {
                CurrentPrecaution = value as BDPrecaution;
            }
        }

        public bool ShowAsChild { get; set; }

        private void BDDosageControl_Leave(object sender, EventArgs e)
        {
            if (!isUpdating) { Save(); }
        }

        private void BDDosageControl_Load(object sender, EventArgs e)
        {
            isUpdating = true;
            rtbInfectiveMaterial.SelectAll();
            isUpdating = false;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getCurrentControl();
            if (rtb != null)
                rtb.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getCurrentControl();
            if (rtb != null)
                rtb.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getCurrentControl();
            if (rtb != null)
                rtb.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getCurrentControl();
            if (rtb != null)
                rtb.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getCurrentControl();
            if (rtb != null)
            {
                rtb.SelectAll();
                rtb.Cut();
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getCurrentControl();
            if (rtb != null)
                rtb.SelectAll();
        }

        private RichTextBox getCurrentControl()
        {
            if (currentControlName == INFECTIVE_MATERIAL_RTB)
                return rtbInfectiveMaterial;
            else if (currentControlName == MODE_OF_TRANSMISSION_RTB)
                return rtbModeOfTransmission;
            else if (currentControlName == SINGLE_ROOM_ACUTE_RTB)
                return rtbSingleRoomAcute;
            else if (currentControlName == SINGLE_ROOM_LONG_TERM_RTB)
                return rtbSingleRoomLongTerm;
            else if (currentControlName == GLOVES_ACUTE_RTB)
                return rtbGlovesAcute;
            else if (currentControlName == GLOVES_LONG_TERM_RTB)
                return rtbGlovesLongTerm;
            else if (currentControlName == GOWNS_ACUTE_RTB)
                return rtbGownsAcute;
            else if (currentControlName == GOWNS_LONG_TERM_RTB)
                return rtbGownsLongTerm;
            else if (currentControlName == MASK_ACUTE_RTB)
                return rtbMaskAcute;
            else if (currentControlName == MASK_LONG_TERM_RTB)
                return rtbMaskLongTerm;
            else return null;
        }

        private void contextMenuStripTextBox_Opening(object sender, CancelEventArgs e)
        {
            RichTextBox rtb = getCurrentControl();
            if (rtb != null)
            {
                undoToolStripMenuItem.Enabled = rtb.CanUndo;
                pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
                cutToolStripMenuItem.Enabled = (rtb.SelectionLength > 0);
                copyToolStripMenuItem.Enabled = (rtb.SelectionLength > 0);
                deleteToolStripMenuItem.Enabled = (rtb.SelectionLength > 0);
            }
        }

        private void rtbInfectiveMaterial_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = INFECTIVE_MATERIAL_RTB;
        }

        private void rtbModeOfTransmission_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = MODE_OF_TRANSMISSION_RTB;
        }

        private void rtbSingleRoomAcute_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = SINGLE_ROOM_ACUTE_RTB;
        }

        private void rtbSingleRoomLongTerm_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = SINGLE_ROOM_LONG_TERM_RTB;
        }

        private void rtbGlovesAcute_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = GLOVES_ACUTE_RTB;
        }

        private void rtbGlovesLongTerm_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = GLOVES_LONG_TERM_RTB;
        }

        private void rtbGownsAcute_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = GOWNS_ACUTE_RTB;
        }

        private void rtbGownsLongTerm_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = GOWNS_LONG_TERM_RTB;
        }

        private void rtbMaskAcute_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = MASK_ACUTE_RTB;
        }

        private void rtbMaskLongTerm_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = MASK_LONG_TERM_RTB;
        }
    }
}
