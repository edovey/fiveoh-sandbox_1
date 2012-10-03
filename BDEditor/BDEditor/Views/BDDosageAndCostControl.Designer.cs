namespace BDEditor.Views
{
    partial class BDDosageAndCostControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDDosageAndCostControl));
            this.rtbCost = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAlloolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.andOrRadioButton = new System.Windows.Forms.RadioButton();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.rtbCost2 = new System.Windows.Forms.RichTextBox();
            this.btnCost2Link = new System.Windows.Forms.Button();
            this.rtbDosage = new System.Windows.Forms.RichTextBox();
            this.btnDosageLink = new System.Windows.Forms.Button();
            this.btnCostLink = new System.Windows.Forms.Button();
            this.btnMenu = new System.Windows.Forms.Button();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addDosageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.delDosagetoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlRadioButtons = new System.Windows.Forms.Panel();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.thenRadioButton = new System.Windows.Forms.RadioButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripTextBox.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.pnlRadioButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbCost
            // 
            this.rtbCost.ContextMenuStrip = this.contextMenuStripTextBox;
            this.rtbCost.DetectUrls = false;
            this.rtbCost.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbCost.Location = new System.Drawing.Point(528, 2);
            this.rtbCost.Name = "rtbCost";
            this.rtbCost.ShortcutsEnabled = false;
            this.rtbCost.Size = new System.Drawing.Size(112, 50);
            this.rtbCost.TabIndex = 2;
            this.rtbCost.Text = "";
            this.toolTip1.SetToolTip(this.rtbCost, "Cost");
            this.rtbCost.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.rtbCost.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbCost_MouseDown);
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
            this.toolStripMenuItem3,
            this.undoToolStripMenuItem,
            this.toolStripMenuItem4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.selectAlloolStripMenuItem});
            this.contextMenuStripTextBox.Name = "contextMenuStrip1";
            this.contextMenuStripTextBox.ShowImageMargin = false;
            this.contextMenuStripTextBox.Size = new System.Drawing.Size(128, 308);
            this.contextMenuStripTextBox.Text = "Insert Symbol";
            this.contextMenuStripTextBox.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTextBox_Opening);
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
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // selectAlloolStripMenuItem
            // 
            this.selectAlloolStripMenuItem.Name = "selectAlloolStripMenuItem";
            this.selectAlloolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.selectAlloolStripMenuItem.Text = "Select All";
            this.selectAlloolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // andOrRadioButton
            // 
            this.andOrRadioButton.AutoSize = true;
            this.andOrRadioButton.Location = new System.Drawing.Point(426, 3);
            this.andOrRadioButton.Name = "andOrRadioButton";
            this.andOrRadioButton.Size = new System.Drawing.Size(93, 17);
            this.andOrRadioButton.TabIndex = 4;
            this.andOrRadioButton.TabStop = true;
            this.andOrRadioButton.Text = "+/-  (with next)";
            this.andOrRadioButton.UseVisualStyleBackColor = true;
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSize = true;
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.BackColor = System.Drawing.SystemColors.Control;
            this.pnlMain.Controls.Add(this.rtbCost2);
            this.pnlMain.Controls.Add(this.btnCost2Link);
            this.pnlMain.Controls.Add(this.rtbDosage);
            this.pnlMain.Controls.Add(this.rtbCost);
            this.pnlMain.Controls.Add(this.btnDosageLink);
            this.pnlMain.Controls.Add(this.btnCostLink);
            this.pnlMain.Controls.Add(this.btnMenu);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.MinimumSize = new System.Drawing.Size(866, 10);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(868, 55);
            this.pnlMain.TabIndex = 0;
            // 
            // rtbCost2
            // 
            this.rtbCost2.ContextMenuStrip = this.contextMenuStripTextBox;
            this.rtbCost2.DetectUrls = false;
            this.rtbCost2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbCost2.Location = new System.Drawing.Point(680, 2);
            this.rtbCost2.Name = "rtbCost2";
            this.rtbCost2.ShortcutsEnabled = false;
            this.rtbCost2.Size = new System.Drawing.Size(112, 50);
            this.rtbCost2.TabIndex = 4;
            this.rtbCost2.Text = "";
            this.toolTip1.SetToolTip(this.rtbCost2, "Cost");
            this.rtbCost2.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.rtbCost2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbCost2_MouseDown);
            // 
            // btnCost2Link
            // 
            this.btnCost2Link.Enabled = false;
            this.btnCost2Link.Image = ((System.Drawing.Image)(resources.GetObject("btnCost2Link.Image")));
            this.btnCost2Link.Location = new System.Drawing.Point(798, 2);
            this.btnCost2Link.Name = "btnCost2Link";
            this.btnCost2Link.Size = new System.Drawing.Size(28, 28);
            this.btnCost2Link.TabIndex = 5;
            this.btnCost2Link.UseVisualStyleBackColor = true;
            this.btnCost2Link.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // rtbDosage
            // 
            this.rtbDosage.ContextMenuStrip = this.contextMenuStripTextBox;
            this.rtbDosage.DetectUrls = false;
            this.rtbDosage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbDosage.Location = new System.Drawing.Point(185, 2);
            this.rtbDosage.Name = "rtbDosage";
            this.rtbDosage.ShortcutsEnabled = false;
            this.rtbDosage.Size = new System.Drawing.Size(303, 50);
            this.rtbDosage.TabIndex = 0;
            this.rtbDosage.Text = "";
            this.toolTip1.SetToolTip(this.rtbDosage, "Dosage");
            this.rtbDosage.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.rtbDosage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbDosage_MouseDown);
            // 
            // btnDosageLink
            // 
            this.btnDosageLink.Enabled = false;
            this.btnDosageLink.Image = ((System.Drawing.Image)(resources.GetObject("btnDosageLink.Image")));
            this.btnDosageLink.Location = new System.Drawing.Point(494, 2);
            this.btnDosageLink.Name = "btnDosageLink";
            this.btnDosageLink.Size = new System.Drawing.Size(28, 28);
            this.btnDosageLink.TabIndex = 1;
            this.btnDosageLink.UseVisualStyleBackColor = true;
            this.btnDosageLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnCostLink
            // 
            this.btnCostLink.Enabled = false;
            this.btnCostLink.Image = ((System.Drawing.Image)(resources.GetObject("btnCostLink.Image")));
            this.btnCostLink.Location = new System.Drawing.Point(646, 2);
            this.btnCostLink.Name = "btnCostLink";
            this.btnCostLink.Size = new System.Drawing.Size(28, 28);
            this.btnCostLink.TabIndex = 3;
            this.btnCostLink.UseVisualStyleBackColor = true;
            this.btnCostLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.ContextMenuStrip = this.contextMenuStripEvents;
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(832, 2);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 6;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reorderPreviousToolStripMenuItem,
            this.reorderNextToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addDosageToolStripMenuItem,
            this.toolStripMenuItem2,
            this.delDosagetoolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(153, 104);
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
            // addDosageToolStripMenuItem
            // 
            this.addDosageToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addDosageToolStripMenuItem.Name = "addDosageToolStripMenuItem";
            this.addDosageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addDosageToolStripMenuItem.Text = "&Add Dosage";
            this.addDosageToolStripMenuItem.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // delDosagetoolStripMenuItem
            // 
            this.delDosagetoolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.delDosagetoolStripMenuItem.Name = "delDosagetoolStripMenuItem";
            this.delDosagetoolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.delDosagetoolStripMenuItem.Text = "Delete";
            this.delDosagetoolStripMenuItem.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // pnlRadioButtons
            // 
            this.pnlRadioButtons.AutoSize = true;
            this.pnlRadioButtons.BackColor = System.Drawing.SystemColors.Control;
            this.pnlRadioButtons.Controls.Add(this.andOrRadioButton);
            this.pnlRadioButtons.Controls.Add(this.noneRadioButton);
            this.pnlRadioButtons.Controls.Add(this.andRadioButton);
            this.pnlRadioButtons.Controls.Add(this.orRadioButton);
            this.pnlRadioButtons.Controls.Add(this.thenRadioButton);
            this.pnlRadioButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRadioButtons.Location = new System.Drawing.Point(0, 80);
            this.pnlRadioButtons.Name = "pnlRadioButtons";
            this.pnlRadioButtons.Size = new System.Drawing.Size(868, 23);
            this.pnlRadioButtons.TabIndex = 1;
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(28, 3);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(87, 17);
            this.noneRadioButton.TabIndex = 0;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "Next Dosage";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(123, 3);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(95, 17);
            this.andRadioButton.TabIndex = 1;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "And (with next)";
            this.andRadioButton.UseVisualStyleBackColor = true;
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(224, 3);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(87, 17);
            this.orRadioButton.TabIndex = 2;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "Or (with next)";
            this.orRadioButton.UseVisualStyleBackColor = true;
            // 
            // thenRadioButton
            // 
            this.thenRadioButton.AutoSize = true;
            this.thenRadioButton.Location = new System.Drawing.Point(318, 3);
            this.thenRadioButton.Name = "thenRadioButton";
            this.thenRadioButton.Size = new System.Drawing.Size(101, 17);
            this.thenRadioButton.TabIndex = 3;
            this.thenRadioButton.TabStop = true;
            this.thenRadioButton.Text = "Then (with next)";
            this.thenRadioButton.UseVisualStyleBackColor = true;
            // 
            // BDDosageAndCostControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlRadioButtons);
            this.Name = "BDDosageAndCostControl";
            this.Size = new System.Drawing.Size(868, 103);
            this.Load += new System.EventHandler(this.BDDosageControl_Load);
            this.Leave += new System.EventHandler(this.BDDosageControl_Leave);
            this.contextMenuStripTextBox.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.pnlRadioButtons.ResumeLayout(false);
            this.pnlRadioButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton andOrRadioButton;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnDosageLink;
        private System.Windows.Forms.Button btnCostLink;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Panel pnlRadioButtons;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.RadioButton thenRadioButton;
        private System.Windows.Forms.RichTextBox rtbDosage;
        private System.Windows.Forms.RichTextBox rtbCost;
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
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addDosageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem delDosagetoolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAlloolStripMenuItem;
        private System.Windows.Forms.RichTextBox rtbCost2;
        private System.Windows.Forms.Button btnCost2Link;
    }
}
