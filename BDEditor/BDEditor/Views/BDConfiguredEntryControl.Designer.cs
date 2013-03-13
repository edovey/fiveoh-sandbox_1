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
            this.btnMenu = new System.Windows.Forms.Button();
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
            this.label1 = new System.Windows.Forms.Label();
            this.editIndexStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStripEvents.SuspendLayout();
            this.panelFields.SuspendLayout();
            this.panelSideBar.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.contextMenuStripEvents.Size = new System.Drawing.Size(164, 176);
            // 
            // reorderPreviousToolStripMenuItem
            // 
            this.reorderPreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.previous_16;
            this.reorderPreviousToolStripMenuItem.Name = "reorderPreviousToolStripMenuItem";
            this.reorderPreviousToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.reorderPreviousToolStripMenuItem.Text = "Move &Previous";
            this.reorderPreviousToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // reorderNextToolStripMenuItem
            // 
            this.reorderNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.next_16;
            this.reorderNextToolStripMenuItem.Name = "reorderNextToolStripMenuItem";
            this.reorderNextToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.reorderNextToolStripMenuItem.Text = "Move &Next";
            this.reorderNextToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToNext_Click);
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
            this.addSiblingNodeToolStripMenuItem.Click += new System.EventHandler(this.addSiblingNode_Click);
            // 
            // addChildNodeToolStripMenuItem
            // 
            this.addChildNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.addChildNodeToolStripMenuItem.Name = "addChildNodeToolStripMenuItem";
            this.addChildNodeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.addChildNodeToolStripMenuItem.Text = "Add Child";
            this.addChildNodeToolStripMenuItem.Visible = false;
            this.addChildNodeToolStripMenuItem.Click += new System.EventHandler(this.addChildNode_Click);
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
            this.deleteNodeToolStripMenuItem.Click += new System.EventHandler(this.btnDeleteNode_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(5, 4);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 31;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // panelFields
            // 
            this.panelFields.AutoSize = true;
            this.panelFields.Controls.Add(this.bdConfiguredEntryFieldControl1);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFields.Location = new System.Drawing.Point(3, 3);
            this.panelFields.MinimumSize = new System.Drawing.Size(371, 51);
            this.panelFields.Name = "panelFields";
            this.panelFields.Padding = new System.Windows.Forms.Padding(3);
            this.panelFields.Size = new System.Drawing.Size(371, 57);
            this.panelFields.TabIndex = 33;
            // 
            // bdConfiguredEntryFieldControl1
            // 
            this.bdConfiguredEntryFieldControl1.AutoSize = true;
            this.bdConfiguredEntryFieldControl1.BackColor = System.Drawing.SystemColors.Control;
            this.bdConfiguredEntryFieldControl1.DisplayOrder = 0;
            this.bdConfiguredEntryFieldControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.bdConfiguredEntryFieldControl1.Location = new System.Drawing.Point(3, 3);
            this.bdConfiguredEntryFieldControl1.MinimumSize = new System.Drawing.Size(370, 50);
            this.bdConfiguredEntryFieldControl1.Name = "bdConfiguredEntryFieldControl1";
            this.bdConfiguredEntryFieldControl1.Padding = new System.Windows.Forms.Padding(1);
            this.bdConfiguredEntryFieldControl1.Size = new System.Drawing.Size(370, 51);
            this.bdConfiguredEntryFieldControl1.TabIndex = 32;
            // 
            // panelSideBar
            // 
            this.panelSideBar.Controls.Add(this.btnMenu);
            this.panelSideBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSideBar.Location = new System.Drawing.Point(374, 3);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(333, 18);
            this.label1.TabIndex = 35;
            this.label1.Text = "This LayoutVariant has not been configured";
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
            // BDConfiguredEntryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelSideBar);
            this.Name = "BDConfiguredEntryControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(416, 63);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.panelFields.ResumeLayout(false);
            this.panelFields.PerformLayout();
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
        private System.Windows.Forms.Button btnMenu;
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem editIndexStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}
