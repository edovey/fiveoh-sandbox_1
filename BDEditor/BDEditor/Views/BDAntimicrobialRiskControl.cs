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
    public partial class BDAntimicrobialRiskControl : UserControl, IBDControl
    {
        private Entities dataContext;
        private IBDNode currentNode;
        private Guid? parentId;
        private BDConstants.BDNodeType parentType;
        private Guid? scopeId;
        private List<IBDControl> propertyControlList = new List<IBDControl>();
        private string currentControlName;

        private const string RISK_PREGNANCY_RTB = "Risk - Pregnancy";
        private const string RISK_LACTATION_RTB = "Risk - Lactation";
        private const string RECOMMENDATION_RTB = "Recommendation";
        private const string AAPRATING_RTB = "AAP Rating";
        private const string RELATIVEDOSE_RTB = "Relative Dose";

        private bool showChildren = true;
        public bool ShowChildren 
        { 
            get { return showChildren; } 
            set { showChildren = value; } 
        }

        public int? DisplayOrder { get; set; }

        public event EventHandler<NodeEventArgs> EditIndexEntries;

        public event EventHandler<NodeEventArgs> RequestItemAdd;
        public event EventHandler<NodeEventArgs> RequestItemDelete;

        public event EventHandler<NodeEventArgs> ReorderToPrevious;
        public event EventHandler<NodeEventArgs> ReorderToNext;

        public event EventHandler<NodeEventArgs> NotesChanged;
        public event EventHandler<NodeEventArgs> NameChanged;

        protected virtual void OnNameChanged(NodeEventArgs e)
        {
            throw new NotImplementedException();
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

        public BDAntimicrobialRiskControl()
        {
            InitializeComponent();
        }

        public BDAntimicrobialRiskControl(Entities pDataContext, IBDNode pNode)
        {
            InitializeComponent();
            dataContext = pDataContext;
            currentNode = pNode;
            parentId = pNode.ParentId;
            DefaultNodeType = pNode.NodeType;
            DefaultLayoutVariantType = pNode.LayoutVariant;
        }

        public void ShowLinksInUse(bool pPropagateToChildren)
        {
            //BDAntimicrobialRisk currentRisk = currentNode as BDAntimicrobialRisk;
            List<BDLinkedNoteAssociation> links = BDLinkedNoteAssociation.GetLinkedNoteAssociationsForParentId(dataContext, (null != this.currentNode) ? this.currentNode.Uuid : Guid.Empty);
            if (this.Controls.Contains(pnlRtbP))
            {
                btnRiskPregnancy.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnRiskPregnancy.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
                btnRecommendation.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnRecommendation.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            }
            if (this.Controls.Contains(pnlRtbL))
            {
                btnRiskLactation.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnRiskLactation.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
                btnAapRating.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnAapRating.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
                btnRelativeDose.BackColor = links.Exists(x => x.parentKeyPropertyName == (string)btnRelativeDose.Tag) ? BDConstants.ACTIVELINK_COLOR : BDConstants.INACTIVELINK_COLOR;
            }
        }

        public void AssignScopeId(Guid? pScopeId)
        {
            scopeId = pScopeId;
        }

        #region IBDControl

        public void RefreshLayout()
        {
            RefreshLayout(ShowChildren);
        }

        public void RefreshLayout(bool pShowChildren)
        {
            bool origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            ControlHelper.SuspendDrawing(this);
            BDAntimicrobialRisk currentRisk = currentNode as BDAntimicrobialRisk;

            if (null != CurrentNode)
            {
                
                switch (CurrentNode.LayoutVariant)
                {
                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                        this.Controls.Remove(pnlRtbL);

                        rtbRiskPregnancy.Text = currentRisk.riskFactor;
                        rtbRecommendations.Text = currentRisk.recommendations;

                        break;

                    case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                        this.Controls.Remove(pnlRtbP);
                        pnlRtbL.Dock = DockStyle.Top;

                        rtbRiskLactation.Text = currentRisk.riskFactor;
                        rtbAapRating.Text = currentRisk.aapRating;
                        rtbRelativeDose.Text = currentRisk.relativeInfantDose;

                        break;
                    default:
                        break;
                }
            }
            else
            {
                rtbRiskPregnancy.Text = "";
                rtbRecommendations.Text = "";
                rtbRiskLactation.Text = "";
                rtbAapRating.Text = "";
                rtbRelativeDose.Text = "";
            }

            ShowLinksInUse(false);
            ControlHelper.ResumeDrawing(this);

            BDCommon.Settings.IsUpdating = origState;
        }

        public void AssignDataContext(Entities pDataContext)
        {
            dataContext = pDataContext;
        }

        public bool CreateCurrentObject()
        {
            bool result = true;

            if (null == this.currentNode)
            {
                if (null == this.parentId)
                {
                    result = false;
                }
                else
                {
                    this.currentNode = BDFabrik.CreateNode(dataContext, DefaultNodeType, parentId, parentType);

                    this.currentNode.DisplayOrder = (null == DisplayOrder) ? -1 : DisplayOrder;
                    this.currentNode.LayoutVariant = this.DefaultLayoutVariantType;
                }
            }

            return result;
        }

        public bool Save()
        {
            if (BDCommon.Settings.IsUpdating) return false;

            bool result = false;
            if (null != parentId)
            {
                BDAntimicrobialRisk currentRisk = currentNode as BDAntimicrobialRisk;

                if (null == currentRisk)
                {
                    CreateCurrentObject();
                }
                else
                {
                    switch (currentRisk.LayoutVariant)
                    {
                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Pregnancy:
                            if (currentRisk.riskFactor != rtbRiskPregnancy.Text)
                                currentRisk.riskFactor = rtbRiskPregnancy.Text;
                            if (currentRisk.recommendations != rtbRecommendations.Text)
                                currentRisk.recommendations = rtbRecommendations.Text;
                            break;

                        case BDConstants.LayoutVariantType.PregnancyLactation_Antimicrobials_Lactation:
                            if (currentRisk.riskFactor != rtbRiskLactation.Text)
                                currentRisk.riskFactor = rtbRiskLactation.Text;
                            if (currentRisk.aapRating != rtbAapRating.Text)
                                currentRisk.aapRating = rtbAapRating.Text;
                            if (currentRisk.relativeInfantDose != rtbRelativeDose.Text)
                                currentRisk.relativeInfantDose = rtbRelativeDose.Text;
                            break;
                        default:
                            break;
                    }

                    BDAntimicrobialRisk.Save(dataContext, currentRisk);
                    result = true;
                }
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

        private void BDAntimicrobialRiskControl_Load(object sender, EventArgs e)
        {
            bool origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            rtbRiskPregnancy.Tag = btnRiskPregnancy;
            rtbRecommendations.Tag = btnRecommendation;
            rtbRiskLactation.Tag = btnRiskLactation;
            rtbAapRating.Tag = btnAapRating;
            rtbRelativeDose.Tag = btnRelativeDose;

            btnRiskPregnancy.Tag = BDAntimicrobialRisk.PROPERTYNAME_PREGNANCYRISK;
            btnRecommendation.Tag = BDAntimicrobialRisk.PROPERTYNAME_RECOMMENDATION;
            btnRiskLactation.Tag = BDAntimicrobialRisk.PROPERTYNAME_LACTATIONRISK;
            btnAapRating.Tag = BDAntimicrobialRisk.PROPERTYNAME_APPRATING;
            btnRelativeDose.Tag = BDAntimicrobialRisk.PROPERTYNAME_RELATIVEDOSE;

            BDCommon.Settings.IsUpdating = origState;
        }

        private void BDAntimicrobialRiskControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        public override string ToString()
        {
            return (null == this.currentNode) ? "No AntimicrobialRisk" : this.currentNode.Uuid.ToString();
        }

        private void createLink(string pProperty)
        {
            if (CreateCurrentObject())
            {
                Save();

                BDLinkedNoteView view = new BDLinkedNoteView();
                view.AssignDataContext(dataContext);
                view.AssignContextPropertyName(pProperty);
                view.AssignParentInfo(currentNode.Uuid, currentNode.NodeType);
                view.AssignScopeId(scopeId);
                view.NotesChanged += new EventHandler<NodeEventArgs>(notesChanged_Action);
                view.ShowDialog(this);
                view.NotesChanged -= new EventHandler<NodeEventArgs>(notesChanged_Action);
                ShowLinksInUse(false);
            }
        }

        private void insertTextFromMenu(string textToInsert)
        {
            if (currentControlName == RISK_PREGNANCY_RTB)
            {
                int position = rtbRiskPregnancy.SelectionStart;
                rtbRiskPregnancy.Text = rtbRiskPregnancy.Text.Insert(rtbRiskPregnancy.SelectionStart, textToInsert);
                rtbRiskPregnancy.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == RECOMMENDATION_RTB)
            {
                int position = rtbRecommendations.SelectionStart;
                rtbRecommendations.Text = rtbRecommendations.Text.Insert(rtbRecommendations.SelectionStart, textToInsert);
                rtbRecommendations.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == RISK_LACTATION_RTB)
            {
                int position = rtbRiskLactation.SelectionStart;
                rtbRiskLactation.Text = rtbRiskLactation.Text.Insert(rtbRiskLactation.SelectionStart, textToInsert);
                rtbRiskLactation.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == AAPRATING_RTB)
            {
                int position = rtbAapRating.SelectionStart;
                rtbAapRating.Text = rtbAapRating.Text.Insert(rtbAapRating.SelectionStart, textToInsert);
                rtbAapRating.SelectionStart = textToInsert.Length + position;
            }
            else if (currentControlName == RELATIVEDOSE_RTB)
            {
                int position = rtbRelativeDose.SelectionStart;
                rtbRelativeDose.Text = rtbRelativeDose.Text.Insert(rtbRelativeDose.SelectionStart, textToInsert);
                rtbRelativeDose.SelectionStart = textToInsert.Length + position;
            }
        }

        private RichTextBox getActiveControl()
        {
            if (currentControlName == RISK_PREGNANCY_RTB)
                return rtbRiskPregnancy;
            else if (currentControlName == RECOMMENDATION_RTB)
                return rtbRecommendations;
            else if (currentControlName == RISK_LACTATION_RTB)
                return rtbRiskLactation;
            else if (currentControlName == AAPRATING_RTB)
                return rtbAapRating;
            else if (currentControlName == RELATIVEDOSE_RTB)
                return rtbRelativeDose;
            return null;
        }

        void notesChanged_Action(object sender, NodeEventArgs e)
        {
            OnNotesChanged(e);
        }

        public BDConstants.BDNodeType DefaultNodeType { get; set; }
        public BDConstants.LayoutVariantType DefaultLayoutVariantType { get; set; }

        public IBDNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public bool ShowAsChild { get; set; }


        private void btnAdd_Click(object sender, EventArgs e)
        {

            OnItemAddRequested(new NodeEventArgs(dataContext, BDConstants.BDNodeType.BDTableRow, DefaultLayoutVariantType));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Guid? uuid = null;
            if (null != this.currentNode) uuid = this.currentNode.Uuid;

            OnItemDeleteRequested(new NodeEventArgs(dataContext, uuid));
        }

        private void btnReorderToPrevious_Click(object sender, EventArgs e)
        {
            OnReorderToPrevious(new NodeEventArgs(dataContext, currentNode.Uuid));
        }

        private void btnReorderToNext_Click(object sender, EventArgs e)
        {
            OnReorderToNext(new NodeEventArgs(dataContext, currentNode.Uuid));
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.contextMenuStripEvents.Show(btnMenu, new System.Drawing.Point(0, btnMenu.Height));
        }

        private void btnLinkedNote_Click(object sender, EventArgs e)
        {
            Button control = sender as Button;
            if (null != control)
            {
                string tag = control.Tag as string;
                createLink(tag);
            }
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

        private void checkmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "√";
            insertTextFromMenu(newText);
        }

        private void trademarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newText = "®";
            insertTextFromMenu(newText);
        }

        private void BDStringControl_Leave(object sender, EventArgs e)
        {
            Save();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getActiveControl();
            if(rtb != null)
                rtb.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getActiveControl();
            if (rtb != null)
                rtb.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getActiveControl();
            if (rtb != null)
                rtb.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getActiveControl();
            if (rtb != null)
                rtb.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getActiveControl();
            if (rtb != null)
            {
                int i = rtb.SelectionStart;
                rtb.Text = rtb.Text.Substring(0, i) + rtb.Text.Substring(i + rtb.SelectionLength);
                rtb.SelectionStart = i;
                rtb.SelectionLength = 0;
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = getActiveControl();
            if (rtb != null)
            {
                rtb.SelectionStart = 0;
                rtb.SelectionLength = rtb.Text.Length;
                rtb.Focus();
            }
        }

        private void contextMenuStripTextBox_Opening(object sender, CancelEventArgs e)
        {
            RichTextBox rtb = getActiveControl();
            if (rtb != null)
                undoToolStripMenuItem.Enabled = rtb.CanUndo;
            pasteToolStripMenuItem.Enabled = (Clipboard.ContainsText());
            cutToolStripMenuItem.Enabled = (rtb.SelectionLength > 0);
            copyToolStripMenuItem.Enabled = (rtb.SelectionLength > 0);
            deleteToolStripMenuItem.Enabled = (rtb.SelectionLength > 0);
        }

        private void rtbRiskPregnancy_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = RISK_PREGNANCY_RTB;
        }

        private void rtbRecommendations_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = RECOMMENDATION_RTB;
        }

        private void rtbRiskLactation_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = RISK_LACTATION_RTB;
        }

        private void rtbAapRating_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = AAPRATING_RTB;
        }

        private void rtbRelativeDose_MouseDown(object sender, MouseEventArgs e)
        {
            currentControlName = RELATIVEDOSE_RTB;
        }
    }
}
