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
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageLinks.SuspendLayout();
            this.tabPageNotes.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 136);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(486, 210);
            this.panel1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageLinks);
            this.tabControl1.Controls.Add(this.tabPageNotes);
            this.tabControl1.Location = new System.Drawing.Point(12, 6);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(462, 169);
            this.tabControl1.TabIndex = 9;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageLinks
            // 
            this.tabPageLinks.Controls.Add(this.chListLinks);
            this.tabPageLinks.Location = new System.Drawing.Point(4, 22);
            this.tabPageLinks.Name = "tabPageLinks";
            this.tabPageLinks.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLinks.Size = new System.Drawing.Size(454, 143);
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
            this.chListLinks.Size = new System.Drawing.Size(448, 137);
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
            this.tabPageNotes.Size = new System.Drawing.Size(454, 143);
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
            this.chListNotes.Size = new System.Drawing.Size(448, 109);
            this.chListNotes.TabIndex = 7;
            this.chListNotes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.linkedNoteView_ItemCheck);
            this.chListNotes.SelectedIndexChanged += new System.EventHandler(this.chListNotes_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(399, 181);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
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
            this.panel2.Size = new System.Drawing.Size(486, 53);
            this.panel2.TabIndex = 3;
            // 
            // rtfContextInfo
            // 
            this.rtfContextInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfContextInfo.Location = new System.Drawing.Point(5, 5);
            this.rtfContextInfo.Name = "rtfContextInfo";
            this.rtfContextInfo.ReadOnly = true;
            this.rtfContextInfo.Size = new System.Drawing.Size(476, 43);
            this.rtfContextInfo.TabIndex = 0;
            this.rtfContextInfo.Text = "";
            // 
            // bdLinkedNoteControl1
            // 
            this.bdLinkedNoteControl1.CurrentLinkedNote = null;
            this.bdLinkedNoteControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bdLinkedNoteControl1.Location = new System.Drawing.Point(0, 53);
            this.bdLinkedNoteControl1.Name = "bdLinkedNoteControl1";
            this.bdLinkedNoteControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdLinkedNoteControl1.SaveOnLeave = false;
            this.bdLinkedNoteControl1.SelectedLinkedNoteType = BDEditor.DataModel.LinkedNoteType.Default;
            this.bdLinkedNoteControl1.Size = new System.Drawing.Size(486, 83);
            this.bdLinkedNoteControl1.TabIndex = 2;
            // 
            // BDLinkedNoteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 346);
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

    }
}