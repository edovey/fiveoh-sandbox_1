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
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtfContextInfo = new System.Windows.Forms.RichTextBox();
            this.bdLinkedNoteControl1 = new BDEditor.Views.BDLinkedNoteControl();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageLinks.SuspendLayout();
            this.tabPageNotes.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
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
            this.btnOK.Location = new System.Drawing.Point(723, 146);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtfContextInfo);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(810, 53);
            this.panel2.TabIndex = 3;
            // 
            // rtfContextInfo
            // 
            this.rtfContextInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfContextInfo.Location = new System.Drawing.Point(5, 5);
            this.rtfContextInfo.Name = "rtfContextInfo";
            this.rtfContextInfo.ReadOnly = true;
            this.rtfContextInfo.Size = new System.Drawing.Size(800, 43);
            this.rtfContextInfo.TabIndex = 0;
            this.rtfContextInfo.TabStop = false;
            this.rtfContextInfo.Text = "";
            // 
            // bdLinkedNoteControl1
            // 
            this.bdLinkedNoteControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bdLinkedNoteControl1.CurrentLinkedNote = null;
            this.bdLinkedNoteControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bdLinkedNoteControl1.Location = new System.Drawing.Point(0, 53);
            this.bdLinkedNoteControl1.Name = "bdLinkedNoteControl1";
            this.bdLinkedNoteControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdLinkedNoteControl1.SaveOnLeave = false;
            this.bdLinkedNoteControl1.SelectedLinkedNoteType = BDEditor.DataModel.LinkedNoteType.Default;
            this.bdLinkedNoteControl1.Size = new System.Drawing.Size(810, 256);
            this.bdLinkedNoteControl1.TabIndex = 1;
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.deleteNoteToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(174, 76);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.delete_record_16;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.deleteToolStripMenuItem.Text = "Delete Association";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.RemoveCurrentAssociation_Action);
            // 
            // deleteNoteToolStripMenuItem
            // 
            this.deleteNoteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteNoteToolStripMenuItem.Name = "deleteNoteToolStripMenuItem";
            this.deleteNoteToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.deleteNoteToolStripMenuItem.Text = "Delete Note";
            this.deleteNoteToolStripMenuItem.Click += new System.EventHandler(this.DeleteCurrentNote_Action);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(170, 6);
            // 
            // BDLinkedNoteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 484);
            this.Controls.Add(this.bdLinkedNoteControl1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "BDLinkedNoteView";
            this.Text = "BDLinkedNoteView";
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageLinks.ResumeLayout(false);
            this.tabPageNotes.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOK;
        private BDLinkedNoteControl bdLinkedNoteControl1;
        private System.Windows.Forms.Panel panel2;
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

    }
}