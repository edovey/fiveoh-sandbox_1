namespace BDEditor.Views
{
    partial class BDNodeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDNodeControl));
            this.lblNodeAsChild = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblNode = new System.Windows.Forms.Label();
            this.btnMenuRight = new System.Windows.Forms.Button();
            this.btnMenuLeft = new System.Windows.Forms.Button();
            this.btnLinkedNote = new System.Windows.Forms.Button();
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
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addSiblingNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlDetail = new System.Windows.Forms.Panel();
            this.pnlOverview = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panelHeader.SuspendLayout();
            this.contextMenuStripTextBox.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNodeAsChild
            // 
            this.lblNodeAsChild.AutoSize = true;
            this.lblNodeAsChild.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNodeAsChild.Location = new System.Drawing.Point(41, 7);
            this.lblNodeAsChild.Name = "lblNodeAsChild";
            this.lblNodeAsChild.Size = new System.Drawing.Size(55, 20);
            this.lblNodeAsChild.TabIndex = 31;
            this.lblNodeAsChild.Text = "Name";
            this.lblNodeAsChild.Visible = false;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.SystemColors.Control;
            this.panelHeader.Controls.Add(this.lblNode);
            this.panelHeader.Controls.Add(this.btnMenuRight);
            this.panelHeader.Controls.Add(this.lblNodeAsChild);
            this.panelHeader.Controls.Add(this.btnMenuLeft);
            this.panelHeader.Controls.Add(this.btnLinkedNote);
            this.panelHeader.Controls.Add(this.tbName);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(870, 56);
            this.panelHeader.TabIndex = 7;
            // 
            // lblNode
            // 
            this.lblNode.AutoSize = true;
            this.lblNode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNode.Location = new System.Drawing.Point(3, 3);
            this.lblNode.Name = "lblNode";
            this.lblNode.Size = new System.Drawing.Size(57, 24);
            this.lblNode.TabIndex = 33;
            this.lblNode.Text = "Node";
            // 
            // btnMenuRight
            // 
            this.btnMenuRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMenuRight.Enabled = false;
            this.btnMenuRight.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenuRight.Location = new System.Drawing.Point(830, 26);
            this.btnMenuRight.Name = "btnMenuRight";
            this.btnMenuRight.Size = new System.Drawing.Size(28, 28);
            this.btnMenuRight.TabIndex = 32;
            this.btnMenuRight.UseVisualStyleBackColor = true;
            this.btnMenuRight.Visible = false;
            this.btnMenuRight.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // btnMenuLeft
            // 
            this.btnMenuLeft.Enabled = false;
            this.btnMenuLeft.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenuLeft.Location = new System.Drawing.Point(7, 26);
            this.btnMenuLeft.Name = "btnMenuLeft";
            this.btnMenuLeft.Size = new System.Drawing.Size(28, 28);
            this.btnMenuLeft.TabIndex = 30;
            this.btnMenuLeft.UseVisualStyleBackColor = true;
            this.btnMenuLeft.Visible = false;
            this.btnMenuLeft.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // btnLinkedNote
            // 
            this.btnLinkedNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkedNote.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNote.Image")));
            this.btnLinkedNote.Location = new System.Drawing.Point(796, 26);
            this.btnLinkedNote.Name = "btnLinkedNote";
            this.btnLinkedNote.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNote.TabIndex = 29;
            this.btnLinkedNote.UseVisualStyleBackColor = true;
            this.btnLinkedNote.Click += new System.EventHandler(this.btnLinkedNote_Click);
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.ContextMenuStrip = this.contextMenuStripTextBox;
            this.tbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbName.Location = new System.Drawing.Point(39, 28);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(751, 24);
            this.tbName.TabIndex = 1;
            this.tbName.Leave += new System.EventHandler(this.tbName_Leave);
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
            this.toolStripSeparator3,
            this.selectAllToolStripMenuItem1});
            this.contextMenuStripTextBox.Name = "contextMenuStrip1";
            this.contextMenuStripTextBox.ShowImageMargin = false;
            this.contextMenuStripTextBox.Size = new System.Drawing.Size(128, 308);
            this.contextMenuStripTextBox.Text = "Insert Symbol";
            this.contextMenuStripTextBox.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTextBox_Opening);
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.µToolStripMenuItem.Text = "µ";
            this.µToolStripMenuItem.Click += new System.EventHandler(this.µToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(124, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(124, 6);
            this.toolStripMenuItem4.Visible = false;
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(124, 6);
            // 
            // selectAllToolStripMenuItem1
            // 
            this.selectAllToolStripMenuItem1.Name = "selectAllToolStripMenuItem1";
            this.selectAllToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.selectAllToolStripMenuItem1.Text = "Select All";
            this.selectAllToolStripMenuItem1.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
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
            this.contextMenuStripEvents.Size = new System.Drawing.Size(153, 148);
            // 
            // reorderPreviousToolStripMenuItem
            // 
            this.reorderPreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.previous_16;
            this.reorderPreviousToolStripMenuItem.Name = "reorderPreviousToolStripMenuItem";
            this.reorderPreviousToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reorderPreviousToolStripMenuItem.Text = "Move &Previous";
            this.reorderPreviousToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // reorderNextToolStripMenuItem
            // 
            this.reorderNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.next_16;
            this.reorderNextToolStripMenuItem.Name = "reorderNextToolStripMenuItem";
            this.reorderNextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reorderNextToolStripMenuItem.Text = "Move &Next";
            this.reorderNextToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToNext_Click);
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
            this.addSiblingNodeToolStripMenuItem.Click += new System.EventHandler(this.addSiblingNode_Click);
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
            this.deleteNodeToolStripMenuItem.Click += new System.EventHandler(this.btnDeleteNode_Click);
            // 
            // pnlDetail
            // 
            this.pnlDetail.AutoSize = true;
            this.pnlDetail.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlDetail.BackColor = System.Drawing.SystemColors.Control;
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetail.Location = new System.Drawing.Point(0, 58);
            this.pnlDetail.MinimumSize = new System.Drawing.Size(600, 5);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(870, 5);
            this.pnlDetail.TabIndex = 8;
            // 
            // pnlOverview
            // 
            this.pnlOverview.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOverview.Location = new System.Drawing.Point(0, 56);
            this.pnlOverview.Name = "pnlOverview";
            this.pnlOverview.Size = new System.Drawing.Size(870, 2);
            this.pnlOverview.TabIndex = 10;
            // 
            // BDNodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.pnlDetail);
            this.Controls.Add(this.pnlOverview);
            this.Controls.Add(this.panelHeader);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(870, 50);
            this.Name = "BDNodeControl";
            this.Size = new System.Drawing.Size(870, 63);
            this.Load += new System.EventHandler(this.BDNodeControl_Load);
            this.Leave += new System.EventHandler(this.BDNodeControl_Leave);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.contextMenuStripTextBox.ResumeLayout(false);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNodeAsChild;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Button btnMenuLeft;
        private System.Windows.Forms.Button btnLinkedNote;
        private System.Windows.Forms.TextBox tbName;
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
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addChildNodeToolStripMenuItem;
        private System.Windows.Forms.Panel pnlDetail;
        private System.Windows.Forms.Button btnMenuRight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
        private System.Windows.Forms.Label lblNode;
        private System.Windows.Forms.ToolStripMenuItem addSiblingNodeToolStripMenuItem;
        protected System.Windows.Forms.Panel pnlOverview;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem1;
    }
}
