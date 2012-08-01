namespace BDEditor.Views
{
    partial class BDLinkedNoteView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMenu = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLinks = new System.Windows.Forms.TabPage();
            this.chListLinks = new System.Windows.Forms.CheckedListBox();
            this.tabPageNotes = new System.Windows.Forms.TabPage();
            this.btnAssignNote = new System.Windows.Forms.Button();
            this.chListNotes = new System.Windows.Forms.CheckedListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.lblNoteCounter = new System.Windows.Forms.Label();
            this.noteTypeLabel = new System.Windows.Forms.Label();
            this.linkedNoteTypeCombo = new System.Windows.Forms.ComboBox();
            this.rtfContextInfo = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addAnotherNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.movePreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelInternalLink = new System.Windows.Forms.Panel();
            this.btnInternalLink = new System.Windows.Forms.Button();
            this.lblInternalLinkDescription = new System.Windows.Forms.Label();
            this.bdLinkedNoteControl1 = new BDEditor.Views.BDLinkedNoteControl();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageLinks.SuspendLayout();
            this.tabPageNotes.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.panelInternalLink.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMenu);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 309);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(810, 175);
            this.panel1.TabIndex = 1;
            // 
            // btnMenu
            // 
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(637, 139);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 17;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageLinks);
            this.tabControl1.Controls.Add(this.tabPageNotes);
            this.tabControl1.Location = new System.Drawing.Point(12, 2);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(619, 169);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageLinks
            // 
            this.tabPageLinks.Controls.Add(this.chListLinks);
            this.tabPageLinks.Location = new System.Drawing.Point(4, 22);
            this.tabPageLinks.Name = "tabPageLinks";
            this.tabPageLinks.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLinks.Size = new System.Drawing.Size(611, 143);
            this.tabPageLinks.TabIndex = 0;
            this.tabPageLinks.Text = "Links to Current Note";
            this.tabPageLinks.UseVisualStyleBackColor = true;
            // 
            // chListLinks
            // 
            this.chListLinks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chListLinks.FormattingEnabled = true;
            this.chListLinks.Location = new System.Drawing.Point(3, 3);
            this.chListLinks.Name = "chListLinks";
            this.chListLinks.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.chListLinks.Size = new System.Drawing.Size(605, 137);
            this.chListLinks.TabIndex = 6;
            this.chListLinks.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.linkedNoteView_ItemCheck);
            // 
            // tabPageNotes
            // 
            this.tabPageNotes.Controls.Add(this.btnAssignNote);
            this.tabPageNotes.Controls.Add(this.chListNotes);
            this.tabPageNotes.Location = new System.Drawing.Point(4, 22);
            this.tabPageNotes.Name = "tabPageNotes";
            this.tabPageNotes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNotes.Size = new System.Drawing.Size(611, 143);
            this.tabPageNotes.TabIndex = 1;
            this.tabPageNotes.Text = "Attached Notes";
            this.tabPageNotes.UseVisualStyleBackColor = true;
            // 
            // btnAssignNote
            // 
            this.btnAssignNote.Enabled = false;
            this.btnAssignNote.Location = new System.Drawing.Point(177, 117);
            this.btnAssignNote.Name = "btnAssignNote";
            this.btnAssignNote.Size = new System.Drawing.Size(75, 23);
            this.btnAssignNote.TabIndex = 8;
            this.btnAssignNote.Text = "Assign";
            this.btnAssignNote.UseVisualStyleBackColor = true;
            this.btnAssignNote.Visible = false;
            this.btnAssignNote.Click += new System.EventHandler(this.btnAssignNote_Click);
            // 
            // chListNotes
            // 
            this.chListNotes.Dock = System.Windows.Forms.DockStyle.Top;
            this.chListNotes.FormattingEnabled = true;
            this.chListNotes.Location = new System.Drawing.Point(3, 3);
            this.chListNotes.Name = "chListNotes";
            this.chListNotes.Size = new System.Drawing.Size(605, 109);
            this.chListNotes.TabIndex = 7;
            this.chListNotes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.linkedNoteView_ItemCheck);
            this.chListNotes.SelectedIndexChanged += new System.EventHandler(this.chListNotes_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(723, 146);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.btnNext);
            this.headerPanel.Controls.Add(this.btnPrevious);
            this.headerPanel.Controls.Add(this.lblNoteCounter);
            this.headerPanel.Controls.Add(this.noteTypeLabel);
            this.headerPanel.Controls.Add(this.linkedNoteTypeCombo);
            this.headerPanel.Controls.Add(this.rtfContextInfo);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(5);
            this.headerPanel.Size = new System.Drawing.Size(810, 53);
            this.headerPanel.TabIndex = 3;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Enabled = false;
            this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.Location = new System.Drawing.Point(786, 9);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 37);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.Enabled = false;
            this.btnPrevious.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrevious.Location = new System.Drawing.Point(716, 8);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(20, 37);
            this.btnPrevious.TabIndex = 4;
            this.btnPrevious.Text = "<";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // lblNoteCounter
            // 
            this.lblNoteCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNoteCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoteCounter.Location = new System.Drawing.Point(738, 9);
            this.lblNoteCounter.Name = "lblNoteCounter";
            this.lblNoteCounter.Size = new System.Drawing.Size(46, 36);
            this.lblNoteCounter.TabIndex = 3;
            this.lblNoteCounter.Text = "88";
            this.lblNoteCounter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // noteTypeLabel
            // 
            this.noteTypeLabel.AutoSize = true;
            this.noteTypeLabel.Location = new System.Drawing.Point(9, 9);
            this.noteTypeLabel.Name = "noteTypeLabel";
            this.noteTypeLabel.Size = new System.Drawing.Size(57, 13);
            this.noteTypeLabel.TabIndex = 2;
            this.noteTypeLabel.Text = "Note Type";
            // 
            // linkedNoteTypeCombo
            // 
            this.linkedNoteTypeCombo.FormattingEnabled = true;
            this.linkedNoteTypeCombo.Location = new System.Drawing.Point(9, 24);
            this.linkedNoteTypeCombo.Name = "linkedNoteTypeCombo";
            this.linkedNoteTypeCombo.Size = new System.Drawing.Size(135, 21);
            this.linkedNoteTypeCombo.TabIndex = 1;
            this.linkedNoteTypeCombo.SelectedIndexChanged += new System.EventHandler(this.linkedNoteType_SelectedIndexChanged);
            // 
            // rtfContextInfo
            // 
            this.rtfContextInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtfContextInfo.Location = new System.Drawing.Point(150, 5);
            this.rtfContextInfo.Name = "rtfContextInfo";
            this.rtfContextInfo.ReadOnly = true;
            this.rtfContextInfo.Size = new System.Drawing.Size(559, 43);
            this.rtfContextInfo.TabIndex = 0;
            this.rtfContextInfo.TabStop = false;
            this.rtfContextInfo.Text = "";
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addAnotherNoteToolStripMenuItem,
            this.deleteNoteToolStripMenuItem,
            this.toolStripMenuItem2,
            this.movePreviousToolStripMenuItem,
            this.moveNextToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(174, 126);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.delete_record_16;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.deleteToolStripMenuItem.Text = "Delete Association";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.RemoveCurrentAssociation_Action);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(170, 6);
            // 
            // addAnotherNoteToolStripMenuItem
            // 
            this.addAnotherNoteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addAnotherNoteToolStripMenuItem.Name = "addAnotherNoteToolStripMenuItem";
            this.addAnotherNoteToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.addAnotherNoteToolStripMenuItem.Text = "Add Another Note";
            this.addAnotherNoteToolStripMenuItem.Click += new System.EventHandler(this.addAnotherNoteToolStripMenuItem_Click);
            // 
            // deleteNoteToolStripMenuItem
            // 
            this.deleteNoteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteNoteToolStripMenuItem.Name = "deleteNoteToolStripMenuItem";
            this.deleteNoteToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.deleteNoteToolStripMenuItem.Text = "Delete Note";
            this.deleteNoteToolStripMenuItem.Click += new System.EventHandler(this.DeleteCurrentNote_Action);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(170, 6);
            this.toolStripMenuItem2.Visible = false;
            // 
            // movePreviousToolStripMenuItem
            // 
            this.movePreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.previous_16;
            this.movePreviousToolStripMenuItem.Name = "movePreviousToolStripMenuItem";
            this.movePreviousToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.movePreviousToolStripMenuItem.Text = "Move Previous";
            this.movePreviousToolStripMenuItem.Visible = false;
            this.movePreviousToolStripMenuItem.Click += new System.EventHandler(this.movePreviousToolStripMenuItem_Click);
            // 
            // moveNextToolStripMenuItem
            // 
            this.moveNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.next_16;
            this.moveNextToolStripMenuItem.Name = "moveNextToolStripMenuItem";
            this.moveNextToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.moveNextToolStripMenuItem.Text = "Move Next";
            this.moveNextToolStripMenuItem.Visible = false;
            this.moveNextToolStripMenuItem.Click += new System.EventHandler(this.moveNextToolStripMenuItem_Click);
            // 
            // panelInternalLink
            // 
            this.panelInternalLink.Controls.Add(this.lblInternalLinkDescription);
            this.panelInternalLink.Controls.Add(this.btnInternalLink);
            this.panelInternalLink.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInternalLink.Location = new System.Drawing.Point(0, 274);
            this.panelInternalLink.Name = "panelInternalLink";
            this.panelInternalLink.Padding = new System.Windows.Forms.Padding(4);
            this.panelInternalLink.Size = new System.Drawing.Size(810, 35);
            this.panelInternalLink.TabIndex = 4;
            // 
            // btnInternalLink
            // 
            this.btnInternalLink.Location = new System.Drawing.Point(4, 6);
            this.btnInternalLink.Name = "btnInternalLink";
            this.btnInternalLink.Size = new System.Drawing.Size(62, 23);
            this.btnInternalLink.TabIndex = 0;
            this.btnInternalLink.Text = "Link";
            this.btnInternalLink.UseVisualStyleBackColor = true;
            this.btnInternalLink.Click += new System.EventHandler(this.btnInternalLink_Click);
            // 
            // lblInternalLinkDescription
            // 
            this.lblInternalLinkDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblInternalLinkDescription.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblInternalLinkDescription.Location = new System.Drawing.Point(68, 4);
            this.lblInternalLinkDescription.Name = "lblInternalLinkDescription";
            this.lblInternalLinkDescription.Size = new System.Drawing.Size(738, 27);
            this.lblInternalLinkDescription.TabIndex = 1;
            this.lblInternalLinkDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bdLinkedNoteControl1
            // 
            this.bdLinkedNoteControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bdLinkedNoteControl1.CurrentLinkedNote = null;
            this.bdLinkedNoteControl1.DefaultLayoutVariantType = BDEditor.Classes.BDConstants.LayoutVariantType.Undefined;
            this.bdLinkedNoteControl1.DefaultNodeType = BDEditor.Classes.BDConstants.BDNodeType.None;
            this.bdLinkedNoteControl1.DisplayOrder = null;
            this.bdLinkedNoteControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bdLinkedNoteControl1.Location = new System.Drawing.Point(0, 53);
            this.bdLinkedNoteControl1.Name = "bdLinkedNoteControl1";
            this.bdLinkedNoteControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdLinkedNoteControl1.SaveOnLeave = false;
            this.bdLinkedNoteControl1.ShowAsChild = false;
            this.bdLinkedNoteControl1.Size = new System.Drawing.Size(810, 221);
            this.bdLinkedNoteControl1.TabIndex = 1;
            // 
            // BDLinkedNoteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 484);
            this.Controls.Add(this.bdLinkedNoteControl1);
            this.Controls.Add(this.panelInternalLink);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "BDLinkedNoteView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Linked Note Editor";
            this.Load += new System.EventHandler(this.BDLinkedNoteView_Load);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageLinks.ResumeLayout(false);
            this.tabPageNotes.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.contextMenuStripEvents.ResumeLayout(false);
            this.panelInternalLink.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOK;
        private BDLinkedNoteControl bdLinkedNoteControl1;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.RichTextBox rtfContextInfo;
        private System.Windows.Forms.CheckedListBox chListLinks;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLinks;
        private System.Windows.Forms.TabPage tabPageNotes;
        private System.Windows.Forms.CheckedListBox chListNotes;
        private System.Windows.Forms.Button btnAssignNote;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteNoteToolStripMenuItem;
        private System.Windows.Forms.ComboBox linkedNoteTypeCombo;
        private System.Windows.Forms.Label noteTypeLabel;
        private System.Windows.Forms.ToolStripMenuItem addAnotherNoteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem movePreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveNextToolStripMenuItem;
        private System.Windows.Forms.Label lblNoteCounter;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Panel panelInternalLink;
        private System.Windows.Forms.Label lblInternalLinkDescription;
        private System.Windows.Forms.Button btnInternalLink;

    }
}