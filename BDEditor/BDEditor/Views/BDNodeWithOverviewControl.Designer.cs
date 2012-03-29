﻿namespace BDEditor.Views
{
    partial class BDNodeWithOverviewControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDNodeWithOverviewControl));
            this.lblNode = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
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
            this.lblOverview = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblNodeDetail = new System.Windows.Forms.Label();
            this.btnMenu = new System.Windows.Forms.Button();
            this.btnLinkedNote = new System.Windows.Forms.Button();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.movePreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveNeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlEditor = new System.Windows.Forms.Panel();
            this.bdLinkedNoteControl1 = new BDEditor.Views.BDLinkedNoteControl();
            this.contextMenuStripTextBox.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.pnlEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNode
            // 
            this.lblNode.AutoSize = true;
            this.lblNode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNode.Location = new System.Drawing.Point(3, 6);
            this.lblNode.Name = "lblNode";
            this.lblNode.Size = new System.Drawing.Size(57, 24);
            this.lblNode.TabIndex = 0;
            this.lblNode.Text = "Node";
            // 
            // tbName
            // 
            this.tbName.ContextMenuStrip = this.contextMenuStripTextBox;
            this.tbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbName.Location = new System.Drawing.Point(39, 31);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(475, 24);
            this.tbName.TabIndex = 1;
            this.tbName.Leave += new System.EventHandler(this.tbName_Leave);
            this.tbName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbName_MouseDown);
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
            this.deleteToolStripMenuItem});
            this.contextMenuStripTextBox.Name = "contextMenuStrip1";
            this.contextMenuStripTextBox.ShowImageMargin = false;
            this.contextMenuStripTextBox.Size = new System.Drawing.Size(83, 258);
            this.contextMenuStripTextBox.Text = "Insert Symbol";
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.µToolStripMenuItem.Text = "µ";
            this.µToolStripMenuItem.Click += new System.EventHandler(this.µToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(79, 6);
            this.toolStripMenuItem3.Visible = false;
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Visible = false;
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(79, 6);
            this.toolStripMenuItem4.Visible = false;
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Visible = false;
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Visible = false;
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Visible = false;
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Visible = false;
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOverview.Location = new System.Drawing.Point(39, 56);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(80, 20);
            this.lblOverview.TabIndex = 3;
            this.lblOverview.Text = "Overview";
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.SystemColors.Control;
            this.panelHeader.Controls.Add(this.lblNodeDetail);
            this.panelHeader.Controls.Add(this.btnMenu);
            this.panelHeader.Controls.Add(this.btnLinkedNote);
            this.panelHeader.Controls.Add(this.lblOverview);
            this.panelHeader.Controls.Add(this.tbName);
            this.panelHeader.Controls.Add(this.lblNode);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(826, 78);
            this.panelHeader.TabIndex = 5;
            // 
            // lblNodeDetail
            // 
            this.lblNodeDetail.AutoSize = true;
            this.lblNodeDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNodeDetail.Location = new System.Drawing.Point(41, 10);
            this.lblNodeDetail.Name = "lblNodeDetail";
            this.lblNodeDetail.Size = new System.Drawing.Size(56, 20);
            this.lblNodeDetail.TabIndex = 31;
            this.lblNodeDetail.Text = "Detail";
            this.lblNodeDetail.Visible = false;
            // 
            // btnMenu
            // 
            this.btnMenu.Enabled = false;
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(7, 30);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 30;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Visible = false;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // btnLinkedNote
            // 
            this.btnLinkedNote.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNote.Image")));
            this.btnLinkedNote.Location = new System.Drawing.Point(520, 27);
            this.btnLinkedNote.Name = "btnLinkedNote";
            this.btnLinkedNote.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNote.TabIndex = 29;
            this.btnLinkedNote.UseVisualStyleBackColor = true;
            this.btnLinkedNote.Click += new System.EventHandler(this.btnLinkedNote_Click);
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.movePreviousToolStripMenuItem,
            this.moveNeToolStripMenuItem,
            this.toolStripSeparator2,
            this.addSectionToolStripMenuItem,
            this.addDetailToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(164, 120);
            // 
            // movePreviousToolStripMenuItem
            // 
            this.movePreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.previous_16;
            this.movePreviousToolStripMenuItem.Name = "movePreviousToolStripMenuItem";
            this.movePreviousToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.movePreviousToolStripMenuItem.Text = "Move &Previous";
            this.movePreviousToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // moveNeToolStripMenuItem
            // 
            this.moveNeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.next_16;
            this.moveNeToolStripMenuItem.Name = "moveNeToolStripMenuItem";
            this.moveNeToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.moveNeToolStripMenuItem.Text = "Move &Next";
            this.moveNeToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToNext_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(160, 6);
            // 
            // addSectionToolStripMenuItem
            // 
            this.addSectionToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addSectionToolStripMenuItem.Name = "addSectionToolStripMenuItem";
            this.addSectionToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addSectionToolStripMenuItem.Text = "Add Section";
            this.addSectionToolStripMenuItem.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // addDetailToolStripMenuItem
            // 
            this.addDetailToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.addDetailToolStripMenuItem.Name = "addDetailToolStripMenuItem";
            this.addDetailToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addDetailToolStripMenuItem.Text = "Add Detail (Row)";
            this.addDetailToolStripMenuItem.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // pnlEditor
            // 
            this.pnlEditor.Controls.Add(this.bdLinkedNoteControl1);
            this.pnlEditor.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEditor.Location = new System.Drawing.Point(0, 78);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Size = new System.Drawing.Size(826, 467);
            this.pnlEditor.TabIndex = 6;
            // 
            // bdLinkedNoteControl1
            // 
            this.bdLinkedNoteControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bdLinkedNoteControl1.CurrentLinkedNote = null;
            this.bdLinkedNoteControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bdLinkedNoteControl1.Location = new System.Drawing.Point(0, 0);
            this.bdLinkedNoteControl1.Name = "bdLinkedNoteControl1";
            this.bdLinkedNoteControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdLinkedNoteControl1.SaveOnLeave = true;
            this.bdLinkedNoteControl1.Size = new System.Drawing.Size(826, 467);
            this.bdLinkedNoteControl1.TabIndex = 4;
            this.bdLinkedNoteControl1.SaveAttemptWithoutParent += new System.EventHandler(this.bdLinkedNoteControl_SaveAttemptWithoutParent);
            // 
            // BDNodeWithOverviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnlEditor);
            this.Controls.Add(this.panelHeader);
            this.MinimumSize = new System.Drawing.Size(826, 550);
            this.Name = "BDNodeWithOverviewControl";
            this.Size = new System.Drawing.Size(826, 569);
            this.Load += new System.EventHandler(this.BDNodeControl_Load);
            this.Leave += new System.EventHandler(this.BDNodeControl_Leave);
            this.contextMenuStripTextBox.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.contextMenuStripEvents.ResumeLayout(false);
            this.pnlEditor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNode;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label lblOverview;
        private BDLinkedNoteControl bdLinkedNoteControl1;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Button btnLinkedNote;
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
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem movePreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveNeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDetailToolStripMenuItem;
        private System.Windows.Forms.Label lblNodeDetail;
        private System.Windows.Forms.Panel pnlEditor;
    }
}