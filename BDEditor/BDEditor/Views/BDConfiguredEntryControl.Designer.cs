namespace BDEditor.Views
{
    partial class BDConfiguredEntryControl
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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMenuLeft = new System.Windows.Forms.Button();
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSiblingNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bdConfiguredEntryFieldControl1 = new BDEditor.Views.BDConfiguredEntryFieldControl();
            this.panelFields = new System.Windows.Forms.Panel();
            this.panelSideBar = new System.Windows.Forms.Panel();
            this.contextMenuStripEvents.SuspendLayout();
            this.panelFields.SuspendLayout();
            this.panelSideBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reorderPreviousToolStripMenuItem,
            this.reorderNextToolStripMenuItem,
            this.toolStripSeparator2,
            this.addSiblingNodeToolStripMenuItem,
            this.addChildNodeToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteNodeToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(156, 126);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // btnMenuLeft
            // 
            this.btnMenuLeft.Enabled = false;
            this.btnMenuLeft.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenuLeft.Location = new System.Drawing.Point(5, 4);
            this.btnMenuLeft.Name = "btnMenuLeft";
            this.btnMenuLeft.Size = new System.Drawing.Size(28, 28);
            this.btnMenuLeft.TabIndex = 31;
            this.btnMenuLeft.UseVisualStyleBackColor = true;
            this.btnMenuLeft.Visible = false;
            // 
            // reorderPreviousToolStripMenuItem
            // 
            this.reorderPreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.previous_16;
            this.reorderPreviousToolStripMenuItem.Name = "reorderPreviousToolStripMenuItem";
            this.reorderPreviousToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.reorderPreviousToolStripMenuItem.Text = "Move &Previous";
            // 
            // reorderNextToolStripMenuItem
            // 
            this.reorderNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.next_16;
            this.reorderNextToolStripMenuItem.Name = "reorderNextToolStripMenuItem";
            this.reorderNextToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.reorderNextToolStripMenuItem.Text = "Move &Next";
            // 
            // addSiblingNodeToolStripMenuItem
            // 
            this.addSiblingNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addSiblingNodeToolStripMenuItem.Name = "addSiblingNodeToolStripMenuItem";
            this.addSiblingNodeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.addSiblingNodeToolStripMenuItem.Text = "Add Sibling";
            // 
            // addChildNodeToolStripMenuItem
            // 
            this.addChildNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.addChildNodeToolStripMenuItem.Name = "addChildNodeToolStripMenuItem";
            this.addChildNodeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.addChildNodeToolStripMenuItem.Text = "Add Child";
            // 
            // deleteNodeToolStripMenuItem
            // 
            this.deleteNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
            this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.deleteNodeToolStripMenuItem.Text = "Delete";
            // 
            // bdConfiguredEntryFieldControl1
            // 
            this.bdConfiguredEntryFieldControl1.Location = new System.Drawing.Point(3, 3);
            this.bdConfiguredEntryFieldControl1.Name = "bdConfiguredEntryFieldControl1";
            this.bdConfiguredEntryFieldControl1.Padding = new System.Windows.Forms.Padding(1);
            this.bdConfiguredEntryFieldControl1.Size = new System.Drawing.Size(349, 51);
            this.bdConfiguredEntryFieldControl1.TabIndex = 32;
            // 
            // panelFields
            // 
            this.panelFields.Controls.Add(this.bdConfiguredEntryFieldControl1);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFields.Location = new System.Drawing.Point(0, 0);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(362, 403);
            this.panelFields.TabIndex = 33;
            // 
            // panelSideBar
            // 
            this.panelSideBar.Controls.Add(this.btnMenuLeft);
            this.panelSideBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSideBar.Location = new System.Drawing.Point(362, 0);
            this.panelSideBar.Name = "panelSideBar";
            this.panelSideBar.Size = new System.Drawing.Size(39, 403);
            this.panelSideBar.TabIndex = 34;
            // 
            // BDConfiguredEntryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.panelSideBar);
            this.Name = "BDConfiguredEntryControl";
            this.Size = new System.Drawing.Size(401, 403);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.panelFields.ResumeLayout(false);
            this.panelSideBar.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Button btnMenuLeft;
        private BDConfiguredEntryFieldControl bdConfiguredEntryFieldControl1;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.Panel panelSideBar;
    }
}
