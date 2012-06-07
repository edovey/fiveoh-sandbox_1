namespace BDEditor.Views
{
    partial class BDAntimicrobialRiskControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDAntimicrobialRiskControl));
            this.pnlButton = new System.Windows.Forms.Panel();
            this.btnMenu = new System.Windows.Forms.Button();
            this.pnlRtbP = new System.Windows.Forms.Panel();
            this.rtbRecommendations = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rtbRiskPregnancy = new System.Windows.Forms.RichTextBox();
            this.btnRiskPregnancy = new System.Windows.Forms.Button();
            this.btnRecommendation = new System.Windows.Forms.Button();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlRtbL = new System.Windows.Forms.Panel();
            this.btnRelativeDose = new System.Windows.Forms.Button();
            this.btnAapRating = new System.Windows.Forms.Button();
            this.btnRiskLactation = new System.Windows.Forms.Button();
            this.rtbRiskLactation = new System.Windows.Forms.RichTextBox();
            this.rtbAapRating = new System.Windows.Forms.RichTextBox();
            this.rtbRelativeDose = new System.Windows.Forms.RichTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.trademarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlButton.SuspendLayout();
            this.pnlRtbP.SuspendLayout();
            this.contextMenuStripTextBox.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.pnlRtbL.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButton
            // 
            this.pnlButton.AutoSize = true;
            this.pnlButton.Controls.Add(this.btnMenu);
            this.pnlButton.Location = new System.Drawing.Point(680, 2);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Size = new System.Drawing.Size(39, 56);
            this.pnlButton.TabIndex = 2;
            // 
            // btnMenu
            // 
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(8, 2);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 0;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // pnlRtbP
            // 
            this.pnlRtbP.AllowDrop = true;
            this.pnlRtbP.Controls.Add(this.rtbRecommendations);
            this.pnlRtbP.Controls.Add(this.rtbRiskPregnancy);
            this.pnlRtbP.Controls.Add(this.btnRiskPregnancy);
            this.pnlRtbP.Controls.Add(this.btnRecommendation);
            this.pnlRtbP.Location = new System.Drawing.Point(0, 0);
            this.pnlRtbP.Name = "pnlRtbP";
            this.pnlRtbP.Size = new System.Drawing.Size(678, 58);
            this.pnlRtbP.TabIndex = 0;
            // 
            // rtbRecommendations
            // 
            this.rtbRecommendations.ContextMenuStrip = this.contextMenuStripTextBox;
            this.rtbRecommendations.Location = new System.Drawing.Point(264, 3);
            this.rtbRecommendations.Name = "rtbRecommendations";
            this.rtbRecommendations.Size = new System.Drawing.Size(380, 55);
            this.rtbRecommendations.TabIndex = 2;
            this.rtbRecommendations.Text = "";
            this.rtbRecommendations.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbRecommendations_MouseDown);
            // 
            // contextMenuStripTextBox
            // 
            this.contextMenuStripTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bToolStripMenuItem,
            this.geToolStripMenuItem,
            this.leToolStripMenuItem,
            this.plusMinusToolStripMenuItem,
            this.degreeToolStripMenuItem,
            this.µToolStripMenuItem,
            this.sOneToolStripMenuItem,
            this.trademarkToolStripMenuItem,
            this.toolStripMenuItem3,
            this.undoToolStripMenuItem,
            this.toolStripMenuItem4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripSeparator1,
            this.selectAllToolStripMenuItem});
            this.contextMenuStripTextBox.Name = "contextMenuStrip1";
            this.contextMenuStripTextBox.ShowImageMargin = false;
            this.contextMenuStripTextBox.Size = new System.Drawing.Size(128, 352);
            this.contextMenuStripTextBox.Text = "Insert Symbol";
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.µToolStripMenuItem.Text = "µ";
            this.µToolStripMenuItem.Click += new System.EventHandler(this.µToolStripMenuItem_Click);
            // 
            // sOneToolStripMenuItem
            // 
            this.sOneToolStripMenuItem.Name = "sOneToolStripMenuItem";
            this.sOneToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.sOneToolStripMenuItem.Text = "¹";
            this.sOneToolStripMenuItem.ToolTipText = "Insert ¹";
            this.sOneToolStripMenuItem.Click += new System.EventHandler(this.sOneToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(124, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(124, 6);
            this.toolStripMenuItem4.Visible = false;
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem2.Text = "Delete";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // rtbRiskPregnancy
            // 
            this.rtbRiskPregnancy.ContextMenuStrip = this.contextMenuStripTextBox;
            this.rtbRiskPregnancy.Location = new System.Drawing.Point(124, 3);
            this.rtbRiskPregnancy.Name = "rtbRiskPregnancy";
            this.rtbRiskPregnancy.Size = new System.Drawing.Size(100, 55);
            this.rtbRiskPregnancy.TabIndex = 0;
            this.rtbRiskPregnancy.Text = "";
            this.rtbRiskPregnancy.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbRiskPregnancy_MouseDown);
            // 
            // btnRiskPregnancy
            // 
            this.btnRiskPregnancy.Image = ((System.Drawing.Image)(resources.GetObject("btnRiskPregnancy.Image")));
            this.btnRiskPregnancy.Location = new System.Drawing.Point(230, 2);
            this.btnRiskPregnancy.Name = "btnRiskPregnancy";
            this.btnRiskPregnancy.Size = new System.Drawing.Size(28, 28);
            this.btnRiskPregnancy.TabIndex = 1;
            this.btnRiskPregnancy.UseVisualStyleBackColor = true;
            // 
            // btnRecommendation
            // 
            this.btnRecommendation.Image = ((System.Drawing.Image)(resources.GetObject("btnRecommendation.Image")));
            this.btnRecommendation.Location = new System.Drawing.Point(650, 2);
            this.btnRecommendation.Name = "btnRecommendation";
            this.btnRecommendation.Size = new System.Drawing.Size(28, 28);
            this.btnRecommendation.TabIndex = 3;
            this.btnRecommendation.UseVisualStyleBackColor = true;
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reorderPreviousToolStripMenuItem,
            this.reorderNextToolStripMenuItem,
            this.toolStripMenuItem1,
            this.deleteToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(153, 76);
            // 
            // reorderPreviousToolStripMenuItem
            // 
            this.reorderPreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.reorder_previous;
            this.reorderPreviousToolStripMenuItem.Name = "reorderPreviousToolStripMenuItem";
            this.reorderPreviousToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reorderPreviousToolStripMenuItem.Text = "Move &Previous";
            this.reorderPreviousToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // reorderNextToolStripMenuItem
            // 
            this.reorderNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.reorder_next;
            this.reorderNextToolStripMenuItem.Name = "reorderNextToolStripMenuItem";
            this.reorderNextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reorderNextToolStripMenuItem.Text = "Move &Next";
            this.reorderNextToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToNext_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // pnlRtbL
            // 
            this.pnlRtbL.Controls.Add(this.btnRelativeDose);
            this.pnlRtbL.Controls.Add(this.btnAapRating);
            this.pnlRtbL.Controls.Add(this.btnRiskLactation);
            this.pnlRtbL.Controls.Add(this.rtbRiskLactation);
            this.pnlRtbL.Controls.Add(this.rtbAapRating);
            this.pnlRtbL.Controls.Add(this.rtbRelativeDose);
            this.pnlRtbL.Location = new System.Drawing.Point(0, 76);
            this.pnlRtbL.Name = "pnlRtbL";
            this.pnlRtbL.Size = new System.Drawing.Size(678, 57);
            this.pnlRtbL.TabIndex = 1;
            // 
            // btnRelativeDose
            // 
            this.btnRelativeDose.Image = ((System.Drawing.Image)(resources.GetObject("btnRelativeDose.Image")));
            this.btnRelativeDose.Location = new System.Drawing.Point(650, 3);
            this.btnRelativeDose.Name = "btnRelativeDose";
            this.btnRelativeDose.Size = new System.Drawing.Size(28, 28);
            this.btnRelativeDose.TabIndex = 5;
            this.btnRelativeDose.UseVisualStyleBackColor = true;
            // 
            // btnAapRating
            // 
            this.btnAapRating.Image = ((System.Drawing.Image)(resources.GetObject("btnAapRating.Image")));
            this.btnAapRating.Location = new System.Drawing.Point(443, 3);
            this.btnAapRating.Name = "btnAapRating";
            this.btnAapRating.Size = new System.Drawing.Size(28, 28);
            this.btnAapRating.TabIndex = 3;
            this.btnAapRating.UseVisualStyleBackColor = true;
            // 
            // btnRiskLactation
            // 
            this.btnRiskLactation.Image = ((System.Drawing.Image)(resources.GetObject("btnRiskLactation.Image")));
            this.btnRiskLactation.Location = new System.Drawing.Point(230, 3);
            this.btnRiskLactation.Name = "btnRiskLactation";
            this.btnRiskLactation.Size = new System.Drawing.Size(28, 28);
            this.btnRiskLactation.TabIndex = 1;
            this.btnRiskLactation.UseVisualStyleBackColor = true;
            // 
            // rtbRiskLactation
            // 
            this.rtbRiskLactation.ContextMenuStrip = this.contextMenuStripTextBox;
            this.rtbRiskLactation.Location = new System.Drawing.Point(124, 0);
            this.rtbRiskLactation.Name = "rtbRiskLactation";
            this.rtbRiskLactation.Size = new System.Drawing.Size(100, 55);
            this.rtbRiskLactation.TabIndex = 0;
            this.rtbRiskLactation.Text = "";
            this.rtbRiskLactation.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbRiskLactation_MouseDown);
            // 
            // rtbAapRating
            // 
            this.rtbAapRating.ContextMenuStrip = this.contextMenuStripTextBox;
            this.rtbAapRating.Location = new System.Drawing.Point(337, 0);
            this.rtbAapRating.Name = "rtbAapRating";
            this.rtbAapRating.Size = new System.Drawing.Size(100, 55);
            this.rtbAapRating.TabIndex = 2;
            this.rtbAapRating.Text = "";
            this.rtbAapRating.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbAapRating_MouseDown);
            // 
            // rtbRelativeDose
            // 
            this.rtbRelativeDose.ContextMenuStrip = this.contextMenuStripTextBox;
            this.rtbRelativeDose.Location = new System.Drawing.Point(544, 0);
            this.rtbRelativeDose.Name = "rtbRelativeDose";
            this.rtbRelativeDose.Size = new System.Drawing.Size(100, 55);
            this.rtbRelativeDose.TabIndex = 4;
            this.rtbRelativeDose.Text = "";
            this.rtbRelativeDose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbRelativeDose_MouseDown);
            // 
            // trademarkToolStripMenuItem
            // 
            this.trademarkToolStripMenuItem.Name = "trademarkToolStripMenuItem";
            this.trademarkToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.trademarkToolStripMenuItem.Text = "®";
            this.trademarkToolStripMenuItem.ToolTipText = "Insert ®";
            this.trademarkToolStripMenuItem.Click += new System.EventHandler(this.trademarkToolStripMenuItem_Click);
            // 
            // BDAntimicrobialRiskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.pnlRtbP);
            this.Controls.Add(this.pnlRtbL);
            this.Controls.Add(this.pnlButton);
            this.Name = "BDAntimicrobialRiskControl";
            this.Size = new System.Drawing.Size(722, 141);
            this.Load += new System.EventHandler(this.BDAntimicrobialRiskControl_Load);
            this.Leave += new System.EventHandler(this.BDAntimicrobialRiskControl_Leave);
            this.pnlButton.ResumeLayout(false);
            this.pnlRtbP.ResumeLayout(false);
            this.contextMenuStripTextBox.ResumeLayout(false);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.pnlRtbL.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Panel pnlRtbP;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Panel pnlRtbL;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTextBox;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem µToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.RichTextBox rtbRiskLactation;
        private System.Windows.Forms.RichTextBox rtbAapRating;
        private System.Windows.Forms.RichTextBox rtbRelativeDose;
        private System.Windows.Forms.RichTextBox rtbRiskPregnancy;
        private System.Windows.Forms.Button btnRiskPregnancy;
        private System.Windows.Forms.Button btnRecommendation;
        private System.Windows.Forms.Button btnRelativeDose;
        private System.Windows.Forms.Button btnAapRating;
        private System.Windows.Forms.Button btnRiskLactation;
        private System.Windows.Forms.RichTextBox rtbRecommendations;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem sOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trademarkToolStripMenuItem;
    }
}
