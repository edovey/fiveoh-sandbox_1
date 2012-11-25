namespace BDEditor.Views
{
    partial class BDCombinedEntryFieldControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDCombinedEntryFieldControl));
            this.txtEntryTitle = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trademarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnLinkedNoteTitle = new System.Windows.Forms.Button();
            this.btnLinkedNoteDetail = new System.Windows.Forms.Button();
            this.txtEntryDetail = new System.Windows.Forms.TextBox();
            this.pnlRadioButtons = new System.Windows.Forms.Panel();
            this.andOrRadioButton = new System.Windows.Forms.RadioButton();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.thenRadioButton = new System.Windows.Forms.RadioButton();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDetail = new System.Windows.Forms.Label();
            this.otherRadioButton = new System.Windows.Forms.RadioButton();
            this.contextMenuStrip1.SuspendLayout();
            this.pnlRadioButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtEntryTitle
            // 
            this.txtEntryTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntryTitle.ContextMenuStrip = this.contextMenuStrip1;
            this.txtEntryTitle.Location = new System.Drawing.Point(81, 11);
            this.txtEntryTitle.Name = "txtEntryTitle";
            this.txtEntryTitle.Size = new System.Drawing.Size(560, 20);
            this.txtEntryTitle.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtEntryTitle, "Entry Title");
            this.txtEntryTitle.Leave += new System.EventHandler(this.txtField_Leave);
            this.txtEntryTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbTitle_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bToolStripMenuItem,
            this.geToolStripMenuItem,
            this.leToolStripMenuItem,
            this.plusMinusToolStripMenuItem,
            this.degreeToolStripMenuItem,
            this.µToolStripMenuItem,
            this.sOneToolStripMenuItem,
            this.trademarkToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 180);
            this.contextMenuStrip1.Text = "Insert Symbol";
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(60, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(60, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(60, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(60, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(60, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(60, 22);
            this.µToolStripMenuItem.Text = "µ";
            this.µToolStripMenuItem.Click += new System.EventHandler(this.µToolStripMenuItem_Click);
            // 
            // sOneToolStripMenuItem
            // 
            this.sOneToolStripMenuItem.Name = "sOneToolStripMenuItem";
            this.sOneToolStripMenuItem.Size = new System.Drawing.Size(60, 22);
            this.sOneToolStripMenuItem.Text = "¹";
            this.sOneToolStripMenuItem.ToolTipText = "Insert ¹";
            this.sOneToolStripMenuItem.Click += new System.EventHandler(this.sOneToolStripMenuItem_Click);
            // 
            // trademarkToolStripMenuItem
            // 
            this.trademarkToolStripMenuItem.Name = "trademarkToolStripMenuItem";
            this.trademarkToolStripMenuItem.Size = new System.Drawing.Size(60, 22);
            this.trademarkToolStripMenuItem.Text = "®";
            this.trademarkToolStripMenuItem.ToolTipText = "Insert ®";
            this.trademarkToolStripMenuItem.Click += new System.EventHandler(this.trademarkToolStripMenuItem_Click);
            // 
            // btnLinkedNoteTitle
            // 
            this.btnLinkedNoteTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkedNoteTitle.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNoteTitle.Image")));
            this.btnLinkedNoteTitle.Location = new System.Drawing.Point(645, 7);
            this.btnLinkedNoteTitle.Name = "btnLinkedNoteTitle";
            this.btnLinkedNoteTitle.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNoteTitle.TabIndex = 37;
            this.toolTip1.SetToolTip(this.btnLinkedNoteTitle, "Entry Title Note");
            this.btnLinkedNoteTitle.UseVisualStyleBackColor = true;
            this.btnLinkedNoteTitle.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnLinkedNoteDetail
            // 
            this.btnLinkedNoteDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkedNoteDetail.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNoteDetail.Image")));
            this.btnLinkedNoteDetail.Location = new System.Drawing.Point(645, 38);
            this.btnLinkedNoteDetail.Name = "btnLinkedNoteDetail";
            this.btnLinkedNoteDetail.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNoteDetail.TabIndex = 39;
            this.toolTip1.SetToolTip(this.btnLinkedNoteDetail, "Entry Detail Note");
            this.btnLinkedNoteDetail.UseVisualStyleBackColor = true;
            this.btnLinkedNoteDetail.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // txtEntryDetail
            // 
            this.txtEntryDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntryDetail.ContextMenuStrip = this.contextMenuStrip1;
            this.txtEntryDetail.Location = new System.Drawing.Point(81, 42);
            this.txtEntryDetail.Name = "txtEntryDetail";
            this.txtEntryDetail.Size = new System.Drawing.Size(560, 20);
            this.txtEntryDetail.TabIndex = 38;
            this.toolTip1.SetToolTip(this.txtEntryDetail, "Entry Detail");
            this.txtEntryDetail.Leave += new System.EventHandler(this.txtField_Leave);
            this.txtEntryDetail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDetail_MouseDown);
            // 
            // pnlRadioButtons
            // 
            this.pnlRadioButtons.BackColor = System.Drawing.SystemColors.Control;
            this.pnlRadioButtons.Controls.Add(this.otherRadioButton);
            this.pnlRadioButtons.Controls.Add(this.andOrRadioButton);
            this.pnlRadioButtons.Controls.Add(this.noneRadioButton);
            this.pnlRadioButtons.Controls.Add(this.andRadioButton);
            this.pnlRadioButtons.Controls.Add(this.orRadioButton);
            this.pnlRadioButtons.Controls.Add(this.thenRadioButton);
            this.pnlRadioButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRadioButtons.Location = new System.Drawing.Point(0, 76);
            this.pnlRadioButtons.Name = "pnlRadioButtons";
            this.pnlRadioButtons.Size = new System.Drawing.Size(678, 23);
            this.pnlRadioButtons.TabIndex = 40;
            // 
            // andOrRadioButton
            // 
            this.andOrRadioButton.AutoSize = true;
            this.andOrRadioButton.Location = new System.Drawing.Point(452, 3);
            this.andOrRadioButton.Name = "andOrRadioButton";
            this.andOrRadioButton.Size = new System.Drawing.Size(93, 17);
            this.andOrRadioButton.TabIndex = 13;
            this.andOrRadioButton.TabStop = true;
            this.andOrRadioButton.Text = "+/-  (with next)";
            this.andOrRadioButton.UseVisualStyleBackColor = true;
            this.andOrRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(89, 3);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(47, 17);
            this.noneRadioButton.TabIndex = 9;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "Next";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            this.noneRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(149, 3);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(95, 17);
            this.andRadioButton.TabIndex = 10;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "And (with next)";
            this.andRadioButton.UseVisualStyleBackColor = true;
            this.andRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(250, 3);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(87, 17);
            this.orRadioButton.TabIndex = 11;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "Or (with next)";
            this.orRadioButton.UseVisualStyleBackColor = true;
            this.orRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // thenRadioButton
            // 
            this.thenRadioButton.AutoSize = true;
            this.thenRadioButton.Location = new System.Drawing.Point(344, 3);
            this.thenRadioButton.Name = "thenRadioButton";
            this.thenRadioButton.Size = new System.Drawing.Size(101, 17);
            this.thenRadioButton.TabIndex = 12;
            this.thenRadioButton.TabStop = true;
            this.thenRadioButton.Text = "Then (with next)";
            this.thenRadioButton.UseVisualStyleBackColor = true;
            this.thenRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(4, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(71, 32);
            this.lblTitle.TabIndex = 41;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDetail
            // 
            this.lblDetail.Location = new System.Drawing.Point(4, 42);
            this.lblDetail.Name = "lblDetail";
            this.lblDetail.Size = new System.Drawing.Size(71, 31);
            this.lblDetail.TabIndex = 42;
            this.lblDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // otherRadioButton
            // 
            this.otherRadioButton.AutoSize = true;
            this.otherRadioButton.Location = new System.Drawing.Point(548, 3);
            this.otherRadioButton.Name = "otherRadioButton";
            this.otherRadioButton.Size = new System.Drawing.Size(51, 17);
            this.otherRadioButton.TabIndex = 14;
            this.otherRadioButton.TabStop = true;
            this.otherRadioButton.Text = "Other";
            this.otherRadioButton.UseVisualStyleBackColor = true;
            // 
            // BDCombinedEntryFieldControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDetail);
            this.Controls.Add(this.pnlRadioButtons);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnLinkedNoteDetail);
            this.Controls.Add(this.txtEntryDetail);
            this.Controls.Add(this.btnLinkedNoteTitle);
            this.Controls.Add(this.txtEntryTitle);
            this.Name = "BDCombinedEntryFieldControl";
            this.Size = new System.Drawing.Size(678, 99);
            this.Load += new System.EventHandler(this.BDCombinedEntryFieldControl_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.pnlRadioButtons.ResumeLayout(false);
            this.pnlRadioButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEntryTitle;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnLinkedNoteTitle;
        private System.Windows.Forms.Button btnLinkedNoteDetail;
        private System.Windows.Forms.TextBox txtEntryDetail;
        private System.Windows.Forms.Panel pnlRadioButtons;
        private System.Windows.Forms.RadioButton andOrRadioButton;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.RadioButton thenRadioButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem µToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trademarkToolStripMenuItem;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDetail;
        private System.Windows.Forms.RadioButton otherRadioButton;

    }
}
