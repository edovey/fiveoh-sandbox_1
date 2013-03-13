namespace BDEditor.Views
{
    partial class BDTableRowControl
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
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnMenu = new System.Windows.Forms.Button();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editIndexStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addTherapyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlButton = new System.Windows.Forms.Panel();
            this.contextMenuStripEvents.SuspendLayout();
            this.pnlButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.AutoSize = true;
            this.pnlControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlControls.BackColor = System.Drawing.SystemColors.Control;
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.MinimumSize = new System.Drawing.Size(160, 41);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(160, 41);
            this.pnlControls.TabIndex = 0;
            // 
            // btnMenu
            // 
            this.btnMenu.BackColor = System.Drawing.SystemColors.Control;
            this.btnMenu.ContextMenuStrip = this.contextMenuStripEvents;
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(7, 5);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 15;
            this.btnMenu.UseVisualStyleBackColor = false;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
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
            this.contextMenuStripEvents.Size = new System.Drawing.Size(164, 154);
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
            // addTherapyToolStripMenuItem
            // 
            this.addTherapyToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addTherapyToolStripMenuItem.Name = "addTherapyToolStripMenuItem";
            this.addTherapyToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.addTherapyToolStripMenuItem.Text = "&Add Row";
            this.addTherapyToolStripMenuItem.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(160, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // pnlButton
            // 
            this.pnlButton.BackColor = System.Drawing.SystemColors.Control;
            this.pnlButton.Controls.Add(this.btnMenu);
            this.pnlButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlButton.Location = new System.Drawing.Point(160, 0);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Size = new System.Drawing.Size(39, 41);
            this.pnlButton.TabIndex = 16;
            // 
            // BDTableRowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.pnlButton);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(162, 41);
            this.Name = "BDTableRowControl";
            this.Size = new System.Drawing.Size(199, 41);
            this.Load += new System.EventHandler(this.BDTableRowControl_Load);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.pnlButton.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addTherapyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.ToolStripMenuItem editIndexStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;


    }
}
