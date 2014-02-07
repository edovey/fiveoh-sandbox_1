namespace BDEditor.Views
{
    partial class BDRegimenControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDRegimenControl));
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.addTherapyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.rbColumnOrder_0 = new System.Windows.Forms.RadioButton();
            this.rbColumnOrder_1 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trademarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbDosage = new System.Windows.Forms.TextBox();
            this.tbDuration = new System.Windows.Forms.TextBox();
            this.btnNameLink = new System.Windows.Forms.Button();
            this.btnDosageLink = new System.Windows.Forms.Button();
            this.btnDurationLink = new System.Windows.Forms.Button();
            this.chkPreviousName = new System.Windows.Forms.CheckBox();
            this.chkPreviousDose = new System.Windows.Forms.CheckBox();
            this.chkPreviousDuration = new System.Windows.Forms.CheckBox();
            this.btnMenu = new System.Windows.Forms.Button();
            this.lblRightBracket = new System.Windows.Forms.Label();
            this.lblLeftBracket = new System.Windows.Forms.Label();
            this.andOrRadioButton = new System.Windows.Forms.RadioButton();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editIndexStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlRadioButtons = new System.Windows.Forms.Panel();
            this.aoRadioButton = new System.Windows.Forms.RadioButton();
            this.otherRadioButton = new System.Windows.Forms.RadioButton();
            this.nextRegimenRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.thenRadioButton = new System.Windows.Forms.RadioButton();
            this.pnlMain.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.pnlRadioButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(160, 6);
            this.toolStripMenuItem2.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // addTherapyToolStripMenuItem
            // 
            this.addTherapyToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addTherapyToolStripMenuItem.Name = "addTherapyToolStripMenuItem";
            this.addTherapyToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addTherapyToolStripMenuItem.Text = "&Add Regimen";
            this.addTherapyToolStripMenuItem.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSize = true;
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.BackColor = System.Drawing.SystemColors.Control;
            this.pnlMain.Controls.Add(this.rbColumnOrder_0);
            this.pnlMain.Controls.Add(this.rbColumnOrder_1);
            this.pnlMain.Controls.Add(this.label4);
            this.pnlMain.Controls.Add(this.lblInfo);
            this.pnlMain.Controls.Add(this.label3);
            this.pnlMain.Controls.Add(this.label1);
            this.pnlMain.Controls.Add(this.label2);
            this.pnlMain.Controls.Add(this.tbName);
            this.pnlMain.Controls.Add(this.tbDosage);
            this.pnlMain.Controls.Add(this.tbDuration);
            this.pnlMain.Controls.Add(this.btnNameLink);
            this.pnlMain.Controls.Add(this.btnDosageLink);
            this.pnlMain.Controls.Add(this.btnDurationLink);
            this.pnlMain.Controls.Add(this.chkPreviousName);
            this.pnlMain.Controls.Add(this.chkPreviousDose);
            this.pnlMain.Controls.Add(this.chkPreviousDuration);
            this.pnlMain.Controls.Add(this.btnMenu);
            this.pnlMain.Controls.Add(this.lblRightBracket);
            this.pnlMain.Controls.Add(this.lblLeftBracket);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.MinimumSize = new System.Drawing.Size(866, 10);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(866, 85);
            this.pnlMain.TabIndex = 32;
            // 
            // rbColumnOrder_0
            // 
            this.rbColumnOrder_0.AutoSize = true;
            this.rbColumnOrder_0.Location = new System.Drawing.Point(601, 7);
            this.rbColumnOrder_0.Name = "rbColumnOrder_0";
            this.rbColumnOrder_0.Size = new System.Drawing.Size(115, 17);
            this.rbColumnOrder_0.TabIndex = 27;
            this.rbColumnOrder_0.TabStop = true;
            this.rbColumnOrder_0.Text = "Regimen of Choice";
            this.rbColumnOrder_0.UseVisualStyleBackColor = true;
            this.rbColumnOrder_0.CheckedChanged += new System.EventHandler(this.rbColumnOrder_0_CheckedChanged);
            // 
            // rbColumnOrder_1
            // 
            this.rbColumnOrder_1.AutoSize = true;
            this.rbColumnOrder_1.Location = new System.Drawing.Point(722, 7);
            this.rbColumnOrder_1.Name = "rbColumnOrder_1";
            this.rbColumnOrder_1.Size = new System.Drawing.Size(120, 17);
            this.rbColumnOrder_1.TabIndex = 28;
            this.rbColumnOrder_1.TabStop = true;
            this.rbColumnOrder_1.Text = "Alternative Regimen";
            this.rbColumnOrder_1.UseVisualStyleBackColor = true;
            this.rbColumnOrder_1.CheckedChanged += new System.EventHandler(this.rbColumnOrder_1_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(41, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 18);
            this.label4.TabIndex = 31;
            this.label4.Text = "Regimen";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(3, 3);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(32, 13);
            this.lblInfo.TabIndex = 30;
            this.lblInfo.Text = "INFO";
            this.lblInfo.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(602, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 15);
            this.label3.TabIndex = 29;
            this.label3.Text = "Duration";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(43, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 27;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(324, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 15);
            this.label2.TabIndex = 28;
            this.label2.Text = "Dosage";
            // 
            // tbName
            // 
            this.tbName.ContextMenuStrip = this.contextMenuStrip1;
            this.tbName.Location = new System.Drawing.Point(46, 48);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(218, 20);
            this.tbName.TabIndex = 1;
            this.tbName.Tag = "";
            this.tbName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbName_MouseDown);
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
            // tbDosage
            // 
            this.tbDosage.ContextMenuStrip = this.contextMenuStrip1;
            this.tbDosage.Location = new System.Drawing.Point(327, 48);
            this.tbDosage.Name = "tbDosage";
            this.tbDosage.Size = new System.Drawing.Size(211, 20);
            this.tbDosage.TabIndex = 4;
            this.tbDosage.Tag = "";
            this.tbDosage.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDosage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDosage_MouseDown);
            // 
            // tbDuration
            // 
            this.tbDuration.ContextMenuStrip = this.contextMenuStrip1;
            this.tbDuration.Location = new System.Drawing.Point(605, 48);
            this.tbDuration.Name = "tbDuration";
            this.tbDuration.Size = new System.Drawing.Size(151, 20);
            this.tbDuration.TabIndex = 7;
            this.tbDuration.Tag = "";
            this.tbDuration.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDuration.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDuration_MouseDown);
            // 
            // btnNameLink
            // 
            this.btnNameLink.Enabled = false;
            this.btnNameLink.Image = ((System.Drawing.Image)(resources.GetObject("btnNameLink.Image")));
            this.btnNameLink.Location = new System.Drawing.Point(270, 47);
            this.btnNameLink.Name = "btnNameLink";
            this.btnNameLink.Size = new System.Drawing.Size(28, 28);
            this.btnNameLink.TabIndex = 2;
            this.btnNameLink.UseVisualStyleBackColor = true;
            this.btnNameLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnDosageLink
            // 
            this.btnDosageLink.Enabled = false;
            this.btnDosageLink.Image = ((System.Drawing.Image)(resources.GetObject("btnDosageLink.Image")));
            this.btnDosageLink.Location = new System.Drawing.Point(544, 47);
            this.btnDosageLink.Name = "btnDosageLink";
            this.btnDosageLink.Size = new System.Drawing.Size(28, 28);
            this.btnDosageLink.TabIndex = 5;
            this.btnDosageLink.UseVisualStyleBackColor = true;
            this.btnDosageLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnDurationLink
            // 
            this.btnDurationLink.Enabled = false;
            this.btnDurationLink.Image = ((System.Drawing.Image)(resources.GetObject("btnDurationLink.Image")));
            this.btnDurationLink.Location = new System.Drawing.Point(762, 47);
            this.btnDurationLink.Name = "btnDurationLink";
            this.btnDurationLink.Size = new System.Drawing.Size(28, 28);
            this.btnDurationLink.TabIndex = 8;
            this.btnDurationLink.UseVisualStyleBackColor = true;
            this.btnDurationLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // chkPreviousName
            // 
            this.chkPreviousName.AutoSize = true;
            this.chkPreviousName.Location = new System.Drawing.Point(29, 52);
            this.chkPreviousName.Name = "chkPreviousName";
            this.chkPreviousName.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousName.TabIndex = 0;
            this.toolTip1.SetToolTip(this.chkPreviousName, "Same as previous");
            this.chkPreviousName.UseVisualStyleBackColor = true;
            this.chkPreviousName.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkPreviousDose
            // 
            this.chkPreviousDose.AutoSize = true;
            this.chkPreviousDose.Location = new System.Drawing.Point(310, 52);
            this.chkPreviousDose.Name = "chkPreviousDose";
            this.chkPreviousDose.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDose.TabIndex = 3;
            this.toolTip1.SetToolTip(this.chkPreviousDose, "Same as previous");
            this.chkPreviousDose.UseVisualStyleBackColor = true;
            this.chkPreviousDose.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkPreviousDuration
            // 
            this.chkPreviousDuration.AutoSize = true;
            this.chkPreviousDuration.Location = new System.Drawing.Point(587, 52);
            this.chkPreviousDuration.Name = "chkPreviousDuration";
            this.chkPreviousDuration.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDuration.TabIndex = 6;
            this.toolTip1.SetToolTip(this.chkPreviousDuration, "Same as previous");
            this.chkPreviousDuration.UseVisualStyleBackColor = true;
            this.chkPreviousDuration.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnMenu
            // 
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(821, 48);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 14;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // lblRightBracket
            // 
            this.lblRightBracket.AutoSize = true;
            this.lblRightBracket.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRightBracket.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblRightBracket.Location = new System.Drawing.Point(787, 30);
            this.lblRightBracket.Name = "lblRightBracket";
            this.lblRightBracket.Size = new System.Drawing.Size(38, 55);
            this.lblRightBracket.TabIndex = 16;
            this.lblRightBracket.Text = "]";
            this.lblRightBracket.Click += new System.EventHandler(this.lblRightBracket_Click);
            // 
            // lblLeftBracket
            // 
            this.lblLeftBracket.AutoSize = true;
            this.lblLeftBracket.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeftBracket.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblLeftBracket.Location = new System.Drawing.Point(-2, 23);
            this.lblLeftBracket.Name = "lblLeftBracket";
            this.lblLeftBracket.Size = new System.Drawing.Size(38, 55);
            this.lblLeftBracket.TabIndex = 15;
            this.lblLeftBracket.Text = "[";
            this.lblLeftBracket.Click += new System.EventHandler(this.lblLeftBracket_Click);
            // 
            // andOrRadioButton
            // 
            this.andOrRadioButton.AutoSize = true;
            this.andOrRadioButton.Location = new System.Drawing.Point(426, 3);
            this.andOrRadioButton.Name = "andOrRadioButton";
            this.andOrRadioButton.Size = new System.Drawing.Size(84, 17);
            this.andOrRadioButton.TabIndex = 13;
            this.andOrRadioButton.TabStop = true;
            this.andOrRadioButton.Text = "+/-  (w/next)";
            this.andOrRadioButton.UseVisualStyleBackColor = true;
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editIndexStripMenuItem,
            this.toolStripSeparator1,
            this.reorderPreviousToolStripMenuItem,
            this.reorderNextToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addTherapyToolStripMenuItem,
            this.toolStripMenuItem2,
            this.deleteToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(164, 132);
            // 
            // editIndexStripMenuItem
            // 
            this.editIndexStripMenuItem.Image = global::BDEditor.Properties.Resources.edit_24x24;
            this.editIndexStripMenuItem.Name = "editIndexStripMenuItem";
            this.editIndexStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.editIndexStripMenuItem.Text = "&Edit Index Entries";
            this.editIndexStripMenuItem.Click += new System.EventHandler(this.editIndexStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(160, 6);
            // 
            // reorderPreviousToolStripMenuItem
            // 
            this.reorderPreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.reorder_previous;
            this.reorderPreviousToolStripMenuItem.Name = "reorderPreviousToolStripMenuItem";
            this.reorderPreviousToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.reorderPreviousToolStripMenuItem.Text = "Move &Previous";
            this.reorderPreviousToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // reorderNextToolStripMenuItem
            // 
            this.reorderNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.reorder_next;
            this.reorderNextToolStripMenuItem.Name = "reorderNextToolStripMenuItem";
            this.reorderNextToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.reorderNextToolStripMenuItem.Text = "Move &Next";
            this.reorderNextToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToNext_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(160, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // pnlRadioButtons
            // 
            this.pnlRadioButtons.BackColor = System.Drawing.SystemColors.Control;
            this.pnlRadioButtons.Controls.Add(this.aoRadioButton);
            this.pnlRadioButtons.Controls.Add(this.otherRadioButton);
            this.pnlRadioButtons.Controls.Add(this.andOrRadioButton);
            this.pnlRadioButtons.Controls.Add(this.nextRegimenRadioButton);
            this.pnlRadioButtons.Controls.Add(this.andRadioButton);
            this.pnlRadioButtons.Controls.Add(this.orRadioButton);
            this.pnlRadioButtons.Controls.Add(this.thenRadioButton);
            this.pnlRadioButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRadioButtons.Location = new System.Drawing.Point(0, 87);
            this.pnlRadioButtons.Name = "pnlRadioButtons";
            this.pnlRadioButtons.Size = new System.Drawing.Size(860, 23);
            this.pnlRadioButtons.TabIndex = 31;
            // 
            // aoRadioButton
            // 
            this.aoRadioButton.AutoSize = true;
            this.aoRadioButton.Location = new System.Drawing.Point(525, 3);
            this.aoRadioButton.Name = "aoRadioButton";
            this.aoRadioButton.Size = new System.Drawing.Size(102, 17);
            this.aoRadioButton.TabIndex = 15;
            this.aoRadioButton.TabStop = true;
            this.aoRadioButton.Text = "And/Or (w/next)";
            this.aoRadioButton.UseVisualStyleBackColor = true;
            // 
            // otherRadioButton
            // 
            this.otherRadioButton.AutoSize = true;
            this.otherRadioButton.Location = new System.Drawing.Point(630, 3);
            this.otherRadioButton.Name = "otherRadioButton";
            this.otherRadioButton.Size = new System.Drawing.Size(51, 17);
            this.otherRadioButton.TabIndex = 14;
            this.otherRadioButton.TabStop = true;
            this.otherRadioButton.Text = "Other";
            this.otherRadioButton.UseVisualStyleBackColor = true;
            // 
            // nextRegimenRadioButton
            // 
            this.nextRegimenRadioButton.AutoSize = true;
            this.nextRegimenRadioButton.Location = new System.Drawing.Point(28, 3);
            this.nextRegimenRadioButton.Name = "nextRegimenRadioButton";
            this.nextRegimenRadioButton.Size = new System.Drawing.Size(92, 17);
            this.nextRegimenRadioButton.TabIndex = 9;
            this.nextRegimenRadioButton.TabStop = true;
            this.nextRegimenRadioButton.Text = "Next Regimen";
            this.nextRegimenRadioButton.UseVisualStyleBackColor = true;
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(123, 3);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(86, 17);
            this.andRadioButton.TabIndex = 10;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "And (w/next)";
            this.andRadioButton.UseVisualStyleBackColor = true;
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(224, 3);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(78, 17);
            this.orRadioButton.TabIndex = 11;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "Or (w/next)";
            this.orRadioButton.UseVisualStyleBackColor = true;
            // 
            // thenRadioButton
            // 
            this.thenRadioButton.AutoSize = true;
            this.thenRadioButton.Location = new System.Drawing.Point(318, 3);
            this.thenRadioButton.Name = "thenRadioButton";
            this.thenRadioButton.Size = new System.Drawing.Size(92, 17);
            this.thenRadioButton.TabIndex = 12;
            this.thenRadioButton.TabStop = true;
            this.thenRadioButton.Text = "Then (w/next)";
            this.thenRadioButton.UseVisualStyleBackColor = true;
            // 
            // BDRegimenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlRadioButtons);
            this.Name = "BDRegimenControl";
            this.Size = new System.Drawing.Size(860, 110);
            this.Load += new System.EventHandler(this.BDRegimenControl_Load);
            this.Leave += new System.EventHandler(this.BDRegimenControl_Leave);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.pnlRadioButtons.ResumeLayout(false);
            this.pnlRadioButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem addTherapyToolStripMenuItem;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem µToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trademarkToolStripMenuItem;
        private System.Windows.Forms.TextBox tbDosage;
        private System.Windows.Forms.TextBox tbDuration;
        private System.Windows.Forms.Button btnNameLink;
        private System.Windows.Forms.Button btnDosageLink;
        private System.Windows.Forms.Button btnDurationLink;
        private System.Windows.Forms.CheckBox chkPreviousName;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkPreviousDose;
        private System.Windows.Forms.CheckBox chkPreviousDuration;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Label lblRightBracket;
        private System.Windows.Forms.Label lblLeftBracket;
        private System.Windows.Forms.RadioButton andOrRadioButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem editIndexStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Panel pnlRadioButtons;
        private System.Windows.Forms.RadioButton aoRadioButton;
        private System.Windows.Forms.RadioButton otherRadioButton;
        private System.Windows.Forms.RadioButton nextRegimenRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.RadioButton thenRadioButton;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbColumnOrder_0;
        private System.Windows.Forms.RadioButton rbColumnOrder_1;
    }
}
