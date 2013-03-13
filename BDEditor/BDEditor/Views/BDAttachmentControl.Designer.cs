namespace BDEditor.Views
{
    partial class BDAttachmentControl
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
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addSiblingNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.attachmentSurfaceContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.browseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.lblFilename = new System.Windows.Forms.Label();
            this.attachmentPictureBox = new System.Windows.Forms.PictureBox();
            this.editIndexStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.pnlOverview.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.attachmentSurfaceContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attachmentPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlOverview
            // 
            this.pnlOverview.Controls.Add(this.attachmentPictureBox);
            this.pnlOverview.Controls.Add(this.lblFilename);
            this.pnlOverview.Location = new System.Drawing.Point(0, 61);
            this.pnlOverview.Size = new System.Drawing.Size(870, 256);
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editIndexStripMenuItem,
            this.toolStripSeparator3,
            this.reorderPreviousToolStripMenuItem,
            this.reorderNextToolStripMenuItem,
            this.toolStripSeparator2,
            this.addSiblingNodeToolStripMenuItem,
            this.addChildNodeToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteNodeToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(164, 154);
            // 
            // reorderPreviousToolStripMenuItem
            // 
            this.reorderPreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.previous_16;
            this.reorderPreviousToolStripMenuItem.Name = "reorderPreviousToolStripMenuItem";
            this.reorderPreviousToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reorderPreviousToolStripMenuItem.Text = "Move &Previous";
            // 
            // reorderNextToolStripMenuItem
            // 
            this.reorderNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.next_16;
            this.reorderNextToolStripMenuItem.Name = "reorderNextToolStripMenuItem";
            this.reorderNextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reorderNextToolStripMenuItem.Text = "Move &Next";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // addSiblingNodeToolStripMenuItem
            // 
            this.addSiblingNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addSiblingNodeToolStripMenuItem.Name = "addSiblingNodeToolStripMenuItem";
            this.addSiblingNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addSiblingNodeToolStripMenuItem.Text = "Add Sibling";
            // 
            // addChildNodeToolStripMenuItem
            // 
            this.addChildNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.addChildNodeToolStripMenuItem.Name = "addChildNodeToolStripMenuItem";
            this.addChildNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addChildNodeToolStripMenuItem.Text = "Add Child";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // deleteNodeToolStripMenuItem
            // 
            this.deleteNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
            this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteNodeToolStripMenuItem.Text = "Delete";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // attachmentSurfaceContextMenuStrip
            // 
            this.attachmentSurfaceContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.browseToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.clearToolStripMenuItem});
            this.attachmentSurfaceContextMenuStrip.Name = "attachmentSurfaceContextMenuStrip";
            this.attachmentSurfaceContextMenuStrip.Size = new System.Drawing.Size(136, 76);
            // 
            // browseToolStripMenuItem
            // 
            this.browseToolStripMenuItem.Name = "browseToolStripMenuItem";
            this.browseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.browseToolStripMenuItem.Text = "Browse...";
            this.browseToolStripMenuItem.Click += new System.EventHandler(this.browseToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save as,,,";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // lblFilename
            // 
            this.lblFilename.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFilename.Location = new System.Drawing.Point(0, 0);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(870, 19);
            this.lblFilename.TabIndex = 0;
            this.lblFilename.Text = "filename";
            this.lblFilename.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // attachmentPictureBox
            // 
            this.attachmentPictureBox.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.attachmentPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.attachmentPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attachmentPictureBox.Location = new System.Drawing.Point(0, 19);
            this.attachmentPictureBox.Name = "attachmentPictureBox";
            this.attachmentPictureBox.Size = new System.Drawing.Size(870, 237);
            this.attachmentPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.attachmentPictureBox.TabIndex = 1;
            this.attachmentPictureBox.TabStop = false;
            this.attachmentPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.attachmentSurface_MouseDown);
            // 
            // editIndexStripMenuItem
            // 
            this.editIndexStripMenuItem.Image = global::BDEditor.Properties.Resources.edit_24x24;
            this.editIndexStripMenuItem.Name = "editIndexStripMenuItem";
            this.editIndexStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.editIndexStripMenuItem.Text = "&Edit Index Entries";
            this.editIndexStripMenuItem.Click += new System.EventHandler(this.editIndexStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(160, 6);
            // 
            // BDAttachmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "BDAttachmentControl";
            this.Size = new System.Drawing.Size(870, 317);
            this.Controls.SetChildIndex(this.pnlOverview, 0);
            this.pnlOverview.ResumeLayout(false);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.attachmentSurfaceContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.attachmentPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addSiblingNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addChildNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ContextMenuStrip attachmentSurfaceContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem browseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.PictureBox attachmentPictureBox;
        private System.Windows.Forms.ToolStripMenuItem editIndexStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}
