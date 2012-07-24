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
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addSiblingNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMenuLRight = new System.Windows.Forms.Button();
            this.panelFields = new System.Windows.Forms.Panel();
            this.bdConfiguredEntryFieldControl1 = new BDEditor.Views.BDConfiguredEntryFieldControl();
            this.panelSideBar = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trademarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripEvents.SuspendLayout();
            this.panelFields.SuspendLayout();
            this.panelSideBar.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // deleteNodeToolStripMenuItem
            // 
            this.deleteNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
            this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.deleteNodeToolStripMenuItem.Text = "Delete";
            // 
            // btnMenuLRight
            // 
            this.btnMenuLRight.Enabled = false;
            this.btnMenuLRight.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenuLRight.Location = new System.Drawing.Point(5, 4);
            this.btnMenuLRight.Name = "btnMenuLRight";
            this.btnMenuLRight.Size = new System.Drawing.Size(28, 28);
            this.btnMenuLRight.TabIndex = 31;
            this.btnMenuLRight.UseVisualStyleBackColor = true;
            this.btnMenuLRight.Visible = false;
            // 
            // panelFields
            // 
            this.panelFields.AutoSize = true;
            this.panelFields.Controls.Add(this.bdConfiguredEntryFieldControl1);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFields.Location = new System.Drawing.Point(1, 1);
            this.panelFields.MinimumSize = new System.Drawing.Size(371, 51);
            this.panelFields.Name = "panelFields";
            this.panelFields.Padding = new System.Windows.Forms.Padding(3);
            this.panelFields.Size = new System.Drawing.Size(376, 57);
            this.panelFields.TabIndex = 33;
            // 
            // bdConfiguredEntryFieldControl1
            // 
            this.bdConfiguredEntryFieldControl1.BackColor = System.Drawing.SystemColors.Control;
            this.bdConfiguredEntryFieldControl1.DisplayOrder = 0;
            this.bdConfiguredEntryFieldControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.bdConfiguredEntryFieldControl1.Location = new System.Drawing.Point(3, 3);
            this.bdConfiguredEntryFieldControl1.MinimumSize = new System.Drawing.Size(370, 50);
            this.bdConfiguredEntryFieldControl1.Name = "bdConfiguredEntryFieldControl1";
            this.bdConfiguredEntryFieldControl1.Padding = new System.Windows.Forms.Padding(1);
            this.bdConfiguredEntryFieldControl1.Size = new System.Drawing.Size(370, 50);
            this.bdConfiguredEntryFieldControl1.TabIndex = 32;
            // 
            // panelSideBar
            // 
            this.panelSideBar.Controls.Add(this.btnMenuLRight);
            this.panelSideBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSideBar.Location = new System.Drawing.Point(377, 1);
            this.panelSideBar.Name = "panelSideBar";
            this.panelSideBar.Size = new System.Drawing.Size(39, 57);
            this.panelSideBar.TabIndex = 34;
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(71, 180);
            this.contextMenuStrip1.Text = "Insert Symbol";
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(70, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(70, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(70, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(70, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(70, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(70, 22);
            this.µToolStripMenuItem.Text = "µ";
            // 
            // sOneToolStripMenuItem
            // 
            this.sOneToolStripMenuItem.Name = "sOneToolStripMenuItem";
            this.sOneToolStripMenuItem.Size = new System.Drawing.Size(70, 22);
            this.sOneToolStripMenuItem.Text = "¹";
            this.sOneToolStripMenuItem.ToolTipText = "Insert ¹";
            // 
            // trademarkToolStripMenuItem
            // 
            this.trademarkToolStripMenuItem.Name = "trademarkToolStripMenuItem";
            this.trademarkToolStripMenuItem.Size = new System.Drawing.Size(70, 22);
            this.trademarkToolStripMenuItem.Text = "®";
            this.trademarkToolStripMenuItem.ToolTipText = "Insert ®";
            // 
            // BDConfiguredEntryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.panelSideBar);
            this.Name = "BDConfiguredEntryControl";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(417, 59);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.panelFields.ResumeLayout(false);
            this.panelSideBar.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnMenuLRight;
        private BDConfiguredEntryFieldControl bdConfiguredEntryFieldControl1;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.Panel panelSideBar;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem µToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trademarkToolStripMenuItem;
    }
}
