namespace BDEditor.Views
{
    partial class BDTherapyControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDTherapyControl));
            this.tbName = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbDosage = new System.Windows.Forms.TextBox();
            this.tbDuration = new System.Windows.Forms.TextBox();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.lblLeftBracket = new System.Windows.Forms.Label();
            this.lblRightBracket = new System.Windows.Forms.Label();
            this.thenRadioButton = new System.Windows.Forms.RadioButton();
            this.btnDurationLink = new System.Windows.Forms.Button();
            this.btnDosageLink = new System.Windows.Forms.Button();
            this.btnTherapyLink = new System.Windows.Forms.Button();
            this.btnMenu = new System.Windows.Forms.Button();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addTherapyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.andOrRadioButton = new System.Windows.Forms.RadioButton();
            this.chkPreviousName = new System.Windows.Forms.CheckBox();
            this.chkPreviousDose = new System.Windows.Forms.CheckBox();
            this.chkPreviousDuration = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkPreviousDuration1 = new System.Windows.Forms.CheckBox();
            this.chkPreviousDose1 = new System.Windows.Forms.CheckBox();
            this.btnDuration1Link = new System.Windows.Forms.Button();
            this.btnDosage1Link = new System.Windows.Forms.Button();
            this.tbDuration1 = new System.Windows.Forms.TextBox();
            this.tbDosage1 = new System.Windows.Forms.TextBox();
            this.pnlRadioButtons = new System.Windows.Forms.Panel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnDuration2Link = new System.Windows.Forms.Button();
            this.tbDuration2 = new System.Windows.Forms.TextBox();
            this.chkPreviousDuration2 = new System.Windows.Forms.CheckBox();
            this.btnDosage2Link = new System.Windows.Forms.Button();
            this.tbDosage2 = new System.Windows.Forms.TextBox();
            this.chkPreviousDose2 = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.pnlRadioButtons.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbName
            // 
            this.tbName.ContextMenuStrip = this.contextMenuStrip1;
            this.tbName.Location = new System.Drawing.Point(46, 2);
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
            this.sOneToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(58, 158);
            this.contextMenuStrip1.Text = "Insert Symbol";
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(57, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(57, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(57, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(57, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(57, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(57, 22);
            this.µToolStripMenuItem.Text = "µ";
            this.µToolStripMenuItem.Click += new System.EventHandler(this.µToolStripMenuItem_Click);
            // 
            // sOneToolStripMenuItem
            // 
            this.sOneToolStripMenuItem.Name = "sOneToolStripMenuItem";
            this.sOneToolStripMenuItem.Size = new System.Drawing.Size(57, 22);
            this.sOneToolStripMenuItem.Text = "¹";
            this.sOneToolStripMenuItem.ToolTipText = "Insert ¹";
            this.sOneToolStripMenuItem.Click += new System.EventHandler(this.sOneToolStripMenuItem_Click);
            // 
            // tbDosage
            // 
            this.tbDosage.ContextMenuStrip = this.contextMenuStrip1;
            this.tbDosage.Location = new System.Drawing.Point(327, 2);
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
            this.tbDuration.Location = new System.Drawing.Point(605, 2);
            this.tbDuration.Name = "tbDuration";
            this.tbDuration.Size = new System.Drawing.Size(174, 20);
            this.tbDuration.TabIndex = 7;
            this.tbDuration.Tag = "";
            this.tbDuration.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDuration.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDuration_MouseDown);
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(28, 3);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(89, 17);
            this.noneRadioButton.TabIndex = 9;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "Next Therapy";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(123, 3);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(95, 17);
            this.andRadioButton.TabIndex = 10;
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
            this.orRadioButton.TabIndex = 11;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "Or (with next)";
            this.orRadioButton.UseVisualStyleBackColor = true;
            // 
            // lblLeftBracket
            // 
            this.lblLeftBracket.AutoSize = true;
            this.lblLeftBracket.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeftBracket.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblLeftBracket.Location = new System.Drawing.Point(-2, -10);
            this.lblLeftBracket.Name = "lblLeftBracket";
            this.lblLeftBracket.Size = new System.Drawing.Size(38, 55);
            this.lblLeftBracket.TabIndex = 15;
            this.lblLeftBracket.Text = "[";
            this.lblLeftBracket.Click += new System.EventHandler(this.lblLeftBracket_Click);
            // 
            // lblRightBracket
            // 
            this.lblRightBracket.AutoSize = true;
            this.lblRightBracket.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRightBracket.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblRightBracket.Location = new System.Drawing.Point(810, -9);
            this.lblRightBracket.Name = "lblRightBracket";
            this.lblRightBracket.Size = new System.Drawing.Size(38, 55);
            this.lblRightBracket.TabIndex = 16;
            this.lblRightBracket.Text = "]";
            this.lblRightBracket.Click += new System.EventHandler(this.lblRightBracket_Click);
            // 
            // thenRadioButton
            // 
            this.thenRadioButton.AutoSize = true;
            this.thenRadioButton.Location = new System.Drawing.Point(318, 3);
            this.thenRadioButton.Name = "thenRadioButton";
            this.thenRadioButton.Size = new System.Drawing.Size(101, 17);
            this.thenRadioButton.TabIndex = 12;
            this.thenRadioButton.TabStop = true;
            this.thenRadioButton.Text = "Then (with next)";
            this.thenRadioButton.UseVisualStyleBackColor = true;
            // 
            // btnDurationLink
            // 
            this.btnDurationLink.Enabled = false;
            this.btnDurationLink.Image = ((System.Drawing.Image)(resources.GetObject("btnDurationLink.Image")));
            this.btnDurationLink.Location = new System.Drawing.Point(785, 1);
            this.btnDurationLink.Name = "btnDurationLink";
            this.btnDurationLink.Size = new System.Drawing.Size(28, 28);
            this.btnDurationLink.TabIndex = 8;
            this.btnDurationLink.UseVisualStyleBackColor = true;
            this.btnDurationLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnDosageLink
            // 
            this.btnDosageLink.Enabled = false;
            this.btnDosageLink.Image = ((System.Drawing.Image)(resources.GetObject("btnDosageLink.Image")));
            this.btnDosageLink.Location = new System.Drawing.Point(544, 1);
            this.btnDosageLink.Name = "btnDosageLink";
            this.btnDosageLink.Size = new System.Drawing.Size(28, 28);
            this.btnDosageLink.TabIndex = 5;
            this.btnDosageLink.UseVisualStyleBackColor = true;
            this.btnDosageLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnTherapyLink
            // 
            this.btnTherapyLink.Enabled = false;
            this.btnTherapyLink.Image = ((System.Drawing.Image)(resources.GetObject("btnTherapyLink.Image")));
            this.btnTherapyLink.Location = new System.Drawing.Point(270, 1);
            this.btnTherapyLink.Name = "btnTherapyLink";
            this.btnTherapyLink.Size = new System.Drawing.Size(28, 28);
            this.btnTherapyLink.TabIndex = 2;
            this.btnTherapyLink.UseVisualStyleBackColor = true;
            this.btnTherapyLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(837, 0);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 14;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reorderPreviousToolStripMenuItem,
            this.reorderNextToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addTherapyToolStripMenuItem,
            this.toolStripMenuItem2,
            this.deleteToolStripMenuItem});
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
            // addTherapyToolStripMenuItem
            // 
            this.addTherapyToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addTherapyToolStripMenuItem.Name = "addTherapyToolStripMenuItem";
            this.addTherapyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addTherapyToolStripMenuItem.Text = "&Add Therapy";
            this.addTherapyToolStripMenuItem.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // andOrRadioButton
            // 
            this.andOrRadioButton.AutoSize = true;
            this.andOrRadioButton.Location = new System.Drawing.Point(426, 3);
            this.andOrRadioButton.Name = "andOrRadioButton";
            this.andOrRadioButton.Size = new System.Drawing.Size(93, 17);
            this.andOrRadioButton.TabIndex = 13;
            this.andOrRadioButton.TabStop = true;
            this.andOrRadioButton.Text = "+/-  (with next)";
            this.andOrRadioButton.UseVisualStyleBackColor = true;
            // 
            // chkPreviousName
            // 
            this.chkPreviousName.AutoSize = true;
            this.chkPreviousName.Location = new System.Drawing.Point(29, 6);
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
            this.chkPreviousDose.Location = new System.Drawing.Point(310, 6);
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
            this.chkPreviousDuration.Location = new System.Drawing.Point(587, 6);
            this.chkPreviousDuration.Name = "chkPreviousDuration";
            this.chkPreviousDuration.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDuration.TabIndex = 6;
            this.toolTip1.SetToolTip(this.chkPreviousDuration, "Same as previous");
            this.chkPreviousDuration.UseVisualStyleBackColor = true;
            this.chkPreviousDuration.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkPreviousDuration1
            // 
            this.chkPreviousDuration1.AutoSize = true;
            this.chkPreviousDuration1.Location = new System.Drawing.Point(587, 40);
            this.chkPreviousDuration1.Name = "chkPreviousDuration1";
            this.chkPreviousDuration1.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDuration1.TabIndex = 20;
            this.toolTip1.SetToolTip(this.chkPreviousDuration1, "Same as previous");
            this.chkPreviousDuration1.UseVisualStyleBackColor = true;
            this.chkPreviousDuration1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkPreviousDose1
            // 
            this.chkPreviousDose1.AutoSize = true;
            this.chkPreviousDose1.Location = new System.Drawing.Point(310, 40);
            this.chkPreviousDose1.Name = "chkPreviousDose1";
            this.chkPreviousDose1.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDose1.TabIndex = 17;
            this.toolTip1.SetToolTip(this.chkPreviousDose1, "Same as previous");
            this.chkPreviousDose1.UseVisualStyleBackColor = true;
            this.chkPreviousDose1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnDuration1Link
            // 
            this.btnDuration1Link.Enabled = false;
            this.btnDuration1Link.Image = ((System.Drawing.Image)(resources.GetObject("btnDuration1Link.Image")));
            this.btnDuration1Link.Location = new System.Drawing.Point(785, 35);
            this.btnDuration1Link.Name = "btnDuration1Link";
            this.btnDuration1Link.Size = new System.Drawing.Size(28, 28);
            this.btnDuration1Link.TabIndex = 22;
            this.btnDuration1Link.UseVisualStyleBackColor = true;
            this.btnDuration1Link.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnDosage1Link
            // 
            this.btnDosage1Link.Enabled = false;
            this.btnDosage1Link.Image = ((System.Drawing.Image)(resources.GetObject("btnDosage1Link.Image")));
            this.btnDosage1Link.Location = new System.Drawing.Point(544, 35);
            this.btnDosage1Link.Name = "btnDosage1Link";
            this.btnDosage1Link.Size = new System.Drawing.Size(28, 28);
            this.btnDosage1Link.TabIndex = 19;
            this.btnDosage1Link.UseVisualStyleBackColor = true;
            this.btnDosage1Link.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // tbDuration1
            // 
            this.tbDuration1.ContextMenuStrip = this.contextMenuStrip1;
            this.tbDuration1.Location = new System.Drawing.Point(605, 36);
            this.tbDuration1.Name = "tbDuration1";
            this.tbDuration1.Size = new System.Drawing.Size(174, 20);
            this.tbDuration1.TabIndex = 21;
            this.tbDuration1.Tag = "";
            this.tbDuration1.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDuration1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDuration1_MouseDown);
            // 
            // tbDosage1
            // 
            this.tbDosage1.ContextMenuStrip = this.contextMenuStrip1;
            this.tbDosage1.Location = new System.Drawing.Point(327, 36);
            this.tbDosage1.Name = "tbDosage1";
            this.tbDosage1.Size = new System.Drawing.Size(211, 20);
            this.tbDosage1.TabIndex = 18;
            this.tbDosage1.Tag = "";
            this.tbDosage1.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDosage1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDosage1_MouseDown);
            // 
            // pnlRadioButtons
            // 
            this.pnlRadioButtons.BackColor = System.Drawing.SystemColors.Control;
            this.pnlRadioButtons.Controls.Add(this.andOrRadioButton);
            this.pnlRadioButtons.Controls.Add(this.noneRadioButton);
            this.pnlRadioButtons.Controls.Add(this.andRadioButton);
            this.pnlRadioButtons.Controls.Add(this.orRadioButton);
            this.pnlRadioButtons.Controls.Add(this.thenRadioButton);
            this.pnlRadioButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRadioButtons.Location = new System.Drawing.Point(3, 103);
            this.pnlRadioButtons.Name = "pnlRadioButtons";
            this.pnlRadioButtons.Size = new System.Drawing.Size(866, 23);
            this.pnlRadioButtons.TabIndex = 29;
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSize = true;
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.BackColor = System.Drawing.SystemColors.Control;
            this.pnlMain.Controls.Add(this.tbName);
            this.pnlMain.Controls.Add(this.tbDosage);
            this.pnlMain.Controls.Add(this.chkPreviousDuration2);
            this.pnlMain.Controls.Add(this.tbDuration);
            this.pnlMain.Controls.Add(this.chkPreviousDose2);
            this.pnlMain.Controls.Add(this.btnTherapyLink);
            this.pnlMain.Controls.Add(this.btnDuration2Link);
            this.pnlMain.Controls.Add(this.btnDosageLink);
            this.pnlMain.Controls.Add(this.btnDosage2Link);
            this.pnlMain.Controls.Add(this.btnDurationLink);
            this.pnlMain.Controls.Add(this.tbDuration2);
            this.pnlMain.Controls.Add(this.tbDosage2);
            this.pnlMain.Controls.Add(this.chkPreviousName);
            this.pnlMain.Controls.Add(this.chkPreviousDuration1);
            this.pnlMain.Controls.Add(this.chkPreviousDose);
            this.pnlMain.Controls.Add(this.chkPreviousDose1);
            this.pnlMain.Controls.Add(this.chkPreviousDuration);
            this.pnlMain.Controls.Add(this.btnDuration1Link);
            this.pnlMain.Controls.Add(this.tbDosage1);
            this.pnlMain.Controls.Add(this.btnDosage1Link);
            this.pnlMain.Controls.Add(this.tbDuration1);
            this.pnlMain.Controls.Add(this.btnMenu);
            this.pnlMain.Controls.Add(this.lblRightBracket);
            this.pnlMain.Controls.Add(this.lblLeftBracket);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMain.Location = new System.Drawing.Point(3, 3);
            this.pnlMain.MinimumSize = new System.Drawing.Size(866, 10);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(866, 100);
            this.pnlMain.TabIndex = 30;
            // 
            // btnDuration2Link
            // 
            this.btnDuration2Link.Enabled = false;
            this.btnDuration2Link.Image = ((System.Drawing.Image)(resources.GetObject("btnDuration2Link.Image")));
            this.btnDuration2Link.Location = new System.Drawing.Point(785, 69);
            this.btnDuration2Link.Name = "btnDuration2Link";
            this.btnDuration2Link.Size = new System.Drawing.Size(28, 28);
            this.btnDuration2Link.TabIndex = 28;
            this.btnDuration2Link.UseVisualStyleBackColor = true;
            this.btnDuration2Link.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // tbDuration2
            // 
            this.tbDuration2.ContextMenuStrip = this.contextMenuStrip1;
            this.tbDuration2.Location = new System.Drawing.Point(605, 70);
            this.tbDuration2.Name = "tbDuration2";
            this.tbDuration2.Size = new System.Drawing.Size(174, 20);
            this.tbDuration2.TabIndex = 27;
            this.tbDuration2.Tag = "";
            this.tbDuration2.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDuration2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDuration2_MouseDown);
            // 
            // chkPreviousDuration2
            // 
            this.chkPreviousDuration2.AutoSize = true;
            this.chkPreviousDuration2.Location = new System.Drawing.Point(587, 74);
            this.chkPreviousDuration2.Name = "chkPreviousDuration2";
            this.chkPreviousDuration2.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDuration2.TabIndex = 26;
            this.toolTip1.SetToolTip(this.chkPreviousDuration2, "Same as previous");
            this.chkPreviousDuration2.UseVisualStyleBackColor = true;
            this.chkPreviousDuration2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnDosage2Link
            // 
            this.btnDosage2Link.Enabled = false;
            this.btnDosage2Link.Image = ((System.Drawing.Image)(resources.GetObject("btnDosage2Link.Image")));
            this.btnDosage2Link.Location = new System.Drawing.Point(544, 69);
            this.btnDosage2Link.Name = "btnDosage2Link";
            this.btnDosage2Link.Size = new System.Drawing.Size(28, 28);
            this.btnDosage2Link.TabIndex = 25;
            this.btnDosage2Link.UseVisualStyleBackColor = true;
            this.btnDosage2Link.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // tbDosage2
            // 
            this.tbDosage2.ContextMenuStrip = this.contextMenuStrip1;
            this.tbDosage2.Location = new System.Drawing.Point(327, 70);
            this.tbDosage2.Name = "tbDosage2";
            this.tbDosage2.Size = new System.Drawing.Size(211, 20);
            this.tbDosage2.TabIndex = 24;
            this.tbDosage2.Tag = "";
            this.tbDosage2.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDosage2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDosage2_MouseDown);
            // 
            // chkPreviousDose2
            // 
            this.chkPreviousDose2.AutoSize = true;
            this.chkPreviousDose2.Location = new System.Drawing.Point(310, 74);
            this.chkPreviousDose2.Name = "chkPreviousDose2";
            this.chkPreviousDose2.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDose2.TabIndex = 23;
            this.toolTip1.SetToolTip(this.chkPreviousDose2, "Same as previous");
            this.chkPreviousDose2.UseVisualStyleBackColor = true;
            this.chkPreviousDose2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // BDTherapyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlRadioButtons);
            this.MinimumSize = new System.Drawing.Size(872, 50);
            this.Name = "BDTherapyControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(872, 129);
            this.Load += new System.EventHandler(this.BDTherapyControl_Load);
            this.Leave += new System.EventHandler(this.BDTherapyControl_Leave);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.pnlRadioButtons.ResumeLayout(false);
            this.pnlRadioButtons.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbDosage;
        private System.Windows.Forms.TextBox tbDuration;
        private System.Windows.Forms.Button btnTherapyLink;
        private System.Windows.Forms.Button btnDosageLink;
        private System.Windows.Forms.Button btnDurationLink;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.Label lblLeftBracket;
        private System.Windows.Forms.Label lblRightBracket;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.RadioButton thenRadioButton;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addTherapyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.RadioButton andOrRadioButton;
        private System.Windows.Forms.CheckBox chkPreviousName;
        private System.Windows.Forms.CheckBox chkPreviousDose;
        private System.Windows.Forms.CheckBox chkPreviousDuration;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem µToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sOneToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkPreviousDuration1;
        private System.Windows.Forms.CheckBox chkPreviousDose1;
        private System.Windows.Forms.Button btnDuration1Link;
        private System.Windows.Forms.Button btnDosage1Link;
        private System.Windows.Forms.TextBox tbDuration1;
        private System.Windows.Forms.TextBox tbDosage1;
        private System.Windows.Forms.Panel pnlRadioButtons;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.CheckBox chkPreviousDuration2;
        private System.Windows.Forms.CheckBox chkPreviousDose2;
        private System.Windows.Forms.Button btnDuration2Link;
        private System.Windows.Forms.Button btnDosage2Link;
        private System.Windows.Forms.TextBox tbDuration2;
        private System.Windows.Forms.TextBox tbDosage2;
    }
}
