﻿namespace BDEditor.Views
{
    partial class BDDosageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDDosageControl));
            this.tbDosage4 = new System.Windows.Forms.TextBox();
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.chkPreviousDose4 = new System.Windows.Forms.CheckBox();
            this.chkPreviousDose3 = new System.Windows.Forms.CheckBox();
            this.chkPreviousDose2 = new System.Windows.Forms.CheckBox();
            this.tbDosage3 = new System.Windows.Forms.TextBox();
            this.btnDosage3Link = new System.Windows.Forms.Button();
            this.btnAdultLink = new System.Windows.Forms.Button();
            this.tbAdultDosage = new System.Windows.Forms.TextBox();
            this.tbDosage2 = new System.Windows.Forms.TextBox();
            this.btnDosage2Link = new System.Windows.Forms.Button();
            this.btnDosage4Link = new System.Windows.Forms.Button();
            this.btnMenu = new System.Windows.Forms.Button();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addDosageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.delDosagetoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDose2 = new System.Windows.Forms.Label();
            this.lblDose3 = new System.Windows.Forms.Label();
            this.lblDose4 = new System.Windows.Forms.Label();
            this.pnlLabels = new System.Windows.Forms.Panel();
            this.contextMenuStripTextBox.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.pnlLabels.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbDosage4
            // 
            this.tbDosage4.ContextMenuStrip = this.contextMenuStripTextBox;
            this.tbDosage4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDosage4.Location = new System.Drawing.Point(661, 2);
            this.tbDosage4.Name = "tbDosage4";
            this.tbDosage4.ShortcutsEnabled = false;
            this.tbDosage4.Size = new System.Drawing.Size(135, 24);
            this.tbDosage4.TabIndex = 6;
            this.toolTip1.SetToolTip(this.tbDosage4, "< 10 (Anuric)");
            this.tbDosage4.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDosage4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDosage4_MouseDown);
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
            this.contextMenuStripTextBox.Size = new System.Drawing.Size(98, 286);
            this.contextMenuStripTextBox.Text = "Insert Symbol";
            this.contextMenuStripTextBox.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTextBox_Opening);
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.µToolStripMenuItem.Text = "µ";
            this.µToolStripMenuItem.Click += new System.EventHandler(this.µToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(94, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(94, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(94, 6);
            // 
            // selectAlloolStripMenuItem
            // 
            this.selectAlloolStripMenuItem.Name = "selectAlloolStripMenuItem";
            this.selectAlloolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.selectAlloolStripMenuItem.Text = "Select All";
            this.selectAlloolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.BackColor = System.Drawing.SystemColors.Control;
            this.pnlMain.Controls.Add(this.chkPreviousDose4);
            this.pnlMain.Controls.Add(this.chkPreviousDose3);
            this.pnlMain.Controls.Add(this.chkPreviousDose2);
            this.pnlMain.Controls.Add(this.tbDosage3);
            this.pnlMain.Controls.Add(this.btnDosage3Link);
            this.pnlMain.Controls.Add(this.btnAdultLink);
            this.pnlMain.Controls.Add(this.tbAdultDosage);
            this.pnlMain.Controls.Add(this.tbDosage2);
            this.pnlMain.Controls.Add(this.tbDosage4);
            this.pnlMain.Controls.Add(this.btnDosage2Link);
            this.pnlMain.Controls.Add(this.btnDosage4Link);
            this.pnlMain.Controls.Add(this.btnMenu);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMain.Location = new System.Drawing.Point(0, 47);
            this.pnlMain.MinimumSize = new System.Drawing.Size(866, 39);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(868, 39);
            this.pnlMain.TabIndex = 0;
            // 
            // chkPreviousDose4
            // 
            this.chkPreviousDose4.AutoSize = true;
            this.chkPreviousDose4.Location = new System.Drawing.Point(640, 6);
            this.chkPreviousDose4.Name = "chkPreviousDose4";
            this.chkPreviousDose4.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDose4.TabIndex = 11;
            this.toolTip1.SetToolTip(this.chkPreviousDose4, "Same as previous");
            this.chkPreviousDose4.UseVisualStyleBackColor = true;
            this.chkPreviousDose4.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkPreviousDose3
            // 
            this.chkPreviousDose3.AutoSize = true;
            this.chkPreviousDose3.Location = new System.Drawing.Point(432, 6);
            this.chkPreviousDose3.Name = "chkPreviousDose3";
            this.chkPreviousDose3.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDose3.TabIndex = 10;
            this.toolTip1.SetToolTip(this.chkPreviousDose3, "Same as previous");
            this.chkPreviousDose3.UseVisualStyleBackColor = true;
            this.chkPreviousDose3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // chkPreviousDose2
            // 
            this.chkPreviousDose2.AutoSize = true;
            this.chkPreviousDose2.Location = new System.Drawing.Point(224, 6);
            this.chkPreviousDose2.Name = "chkPreviousDose2";
            this.chkPreviousDose2.Size = new System.Drawing.Size(15, 14);
            this.chkPreviousDose2.TabIndex = 9;
            this.toolTip1.SetToolTip(this.chkPreviousDose2, "Same as previous");
            this.chkPreviousDose2.UseVisualStyleBackColor = true;
            this.chkPreviousDose2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // tbDosage3
            // 
            this.tbDosage3.ContextMenuStrip = this.contextMenuStripTextBox;
            this.tbDosage3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDosage3.Location = new System.Drawing.Point(453, 3);
            this.tbDosage3.Name = "tbDosage3";
            this.tbDosage3.ShortcutsEnabled = false;
            this.tbDosage3.Size = new System.Drawing.Size(135, 24);
            this.tbDosage3.TabIndex = 4;
            this.toolTip1.SetToolTip(this.tbDosage3, "10 - 50");
            this.tbDosage3.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDosage3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDosage3_MouseDown);
            // 
            // btnDosage3Link
            // 
            this.btnDosage3Link.Enabled = false;
            this.btnDosage3Link.Image = ((System.Drawing.Image)(resources.GetObject("btnDosage3Link.Image")));
            this.btnDosage3Link.Location = new System.Drawing.Point(594, 2);
            this.btnDosage3Link.Name = "btnDosage3Link";
            this.btnDosage3Link.Size = new System.Drawing.Size(28, 28);
            this.btnDosage3Link.TabIndex = 5;
            this.btnDosage3Link.UseVisualStyleBackColor = true;
            this.btnDosage3Link.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnAdultLink
            // 
            this.btnAdultLink.Enabled = false;
            this.btnAdultLink.Image = ((System.Drawing.Image)(resources.GetObject("btnAdultLink.Image")));
            this.btnAdultLink.Location = new System.Drawing.Point(185, 3);
            this.btnAdultLink.Name = "btnAdultLink";
            this.btnAdultLink.Size = new System.Drawing.Size(28, 28);
            this.btnAdultLink.TabIndex = 1;
            this.btnAdultLink.UseVisualStyleBackColor = true;
            this.btnAdultLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // tbAdultDosage
            // 
            this.tbAdultDosage.ContextMenuStrip = this.contextMenuStripTextBox;
            this.tbAdultDosage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAdultDosage.Location = new System.Drawing.Point(10, 2);
            this.tbAdultDosage.Name = "tbAdultDosage";
            this.tbAdultDosage.ShortcutsEnabled = false;
            this.tbAdultDosage.Size = new System.Drawing.Size(169, 24);
            this.tbAdultDosage.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbAdultDosage, "Adult");
            this.tbAdultDosage.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbAdultDosage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbAdultDosage_MouseDown);
            // 
            // tbDosage2
            // 
            this.tbDosage2.ContextMenuStrip = this.contextMenuStripTextBox;
            this.tbDosage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDosage2.Location = new System.Drawing.Point(245, 2);
            this.tbDosage2.Name = "tbDosage2";
            this.tbDosage2.ShortcutsEnabled = false;
            this.tbDosage2.Size = new System.Drawing.Size(135, 24);
            this.tbDosage2.TabIndex = 2;
            this.toolTip1.SetToolTip(this.tbDosage2, "< 50");
            this.tbDosage2.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tbDosage2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbDosage2_MouseDown);
            // 
            // btnDosage2Link
            // 
            this.btnDosage2Link.Enabled = false;
            this.btnDosage2Link.Image = ((System.Drawing.Image)(resources.GetObject("btnDosage2Link.Image")));
            this.btnDosage2Link.Location = new System.Drawing.Point(386, 2);
            this.btnDosage2Link.Name = "btnDosage2Link";
            this.btnDosage2Link.Size = new System.Drawing.Size(28, 28);
            this.btnDosage2Link.TabIndex = 3;
            this.btnDosage2Link.UseVisualStyleBackColor = true;
            this.btnDosage2Link.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnDosage4Link
            // 
            this.btnDosage4Link.Enabled = false;
            this.btnDosage4Link.Image = ((System.Drawing.Image)(resources.GetObject("btnDosage4Link.Image")));
            this.btnDosage4Link.Location = new System.Drawing.Point(802, 1);
            this.btnDosage4Link.Name = "btnDosage4Link";
            this.btnDosage4Link.Size = new System.Drawing.Size(28, 28);
            this.btnDosage4Link.TabIndex = 7;
            this.btnDosage4Link.UseVisualStyleBackColor = true;
            this.btnDosage4Link.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.ContextMenuStrip = this.contextMenuStripEvents;
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(837, 0);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 8;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(41, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Normal Adult Dose";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(397, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(295, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Dose and Interval Adjustment for Renal Impairment";
            // 
            // lblDose2
            // 
            this.lblDose2.AutoSize = true;
            this.lblDose2.Location = new System.Drawing.Point(308, 30);
            this.lblDose2.Name = "lblDose2";
            this.lblDose2.Size = new System.Drawing.Size(25, 13);
            this.lblDose2.TabIndex = 35;
            this.lblDose2.Text = ">50";
            // 
            // lblDose3
            // 
            this.lblDose3.AutoSize = true;
            this.lblDose3.Location = new System.Drawing.Point(511, 30);
            this.lblDose3.Name = "lblDose3";
            this.lblDose3.Size = new System.Drawing.Size(40, 13);
            this.lblDose3.TabIndex = 36;
            this.lblDose3.Text = "10 - 50";
            // 
            // lblDose4
            // 
            this.lblDose4.AutoSize = true;
            this.lblDose4.Location = new System.Drawing.Point(692, 30);
            this.lblDose4.Name = "lblDose4";
            this.lblDose4.Size = new System.Drawing.Size(67, 13);
            this.lblDose4.TabIndex = 37;
            this.lblDose4.Text = "< 10 (Anuric)";
            // 
            // pnlLabels
            // 
            this.pnlLabels.Controls.Add(this.label2);
            this.pnlLabels.Controls.Add(this.lblDose4);
            this.pnlLabels.Controls.Add(this.label1);
            this.pnlLabels.Controls.Add(this.lblDose3);
            this.pnlLabels.Controls.Add(this.lblDose2);
            this.pnlLabels.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLabels.Location = new System.Drawing.Point(0, 0);
            this.pnlLabels.MinimumSize = new System.Drawing.Size(860, 47);
            this.pnlLabels.Name = "pnlLabels";
            this.pnlLabels.Size = new System.Drawing.Size(868, 47);
            this.pnlLabels.TabIndex = 38;
            // 
            // BDDosageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlLabels);
            this.Name = "BDDosageControl";
            this.Size = new System.Drawing.Size(868, 89);
            this.Load += new System.EventHandler(this.BDDosageControl_Load);
            this.Leave += new System.EventHandler(this.BDDosageControl_Leave);
            this.contextMenuStripTextBox.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.contextMenuStripEvents.ResumeLayout(false);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnDosage2Link;
        private System.Windows.Forms.Button btnDosage4Link;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Button btnAdultLink;
        private System.Windows.Forms.TextBox tbAdultDosage;
        private System.Windows.Forms.TextBox tbDosage2;
        private System.Windows.Forms.TextBox tbDosage4;
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
        private System.Windows.Forms.TextBox tbDosage3;
        private System.Windows.Forms.Button btnDosage3Link;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDose2;
        private System.Windows.Forms.Label lblDose3;
        private System.Windows.Forms.Label lblDose4;
        private System.Windows.Forms.Panel pnlLabels;
        private System.Windows.Forms.CheckBox chkPreviousDose4;
        private System.Windows.Forms.CheckBox chkPreviousDose3;
        private System.Windows.Forms.CheckBox chkPreviousDose2;
    }
}
