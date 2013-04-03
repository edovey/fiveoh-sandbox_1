namespace BDEditor.Views
{
    partial class BDEditView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDEditView));
            this.headerPanel = new System.Windows.Forms.Panel();
            this.btnDebug = new System.Windows.Forms.Button();
            this.btnShowLayoutEditor = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnPublish = new System.Windows.Forms.Button();
            this.lbLastSyncDateTime = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnIndexManager = new System.Windows.Forms.Button();
            this.chapterDropDown = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.chapterTree = new System.Windows.Forms.TreeView();
            this.navTreeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addChildNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entitiesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.entitiesBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.navTreeContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.btnDebug);
            this.headerPanel.Controls.Add(this.btnShowLayoutEditor);
            this.headerPanel.Controls.Add(this.btnRestore);
            this.headerPanel.Controls.Add(this.btnMove);
            this.headerPanel.Controls.Add(this.btnSearch);
            this.headerPanel.Controls.Add(this.btnPublish);
            this.headerPanel.Controls.Add(this.lbLastSyncDateTime);
            this.headerPanel.Controls.Add(this.btnSync);
            this.headerPanel.Controls.Add(this.btnIndexManager);
            this.headerPanel.Controls.Add(this.chapterDropDown);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1192, 39);
            this.headerPanel.TabIndex = 0;
            // 
            // btnDebug
            // 
            this.btnDebug.Location = new System.Drawing.Point(660, 10);
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(75, 23);
            this.btnDebug.TabIndex = 11;
            this.btnDebug.Text = "Debug";
            this.btnDebug.UseVisualStyleBackColor = true;
            this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
            // 
            // btnShowLayoutEditor
            // 
            this.btnShowLayoutEditor.Location = new System.Drawing.Point(741, 10);
            this.btnShowLayoutEditor.Name = "btnShowLayoutEditor";
            this.btnShowLayoutEditor.Size = new System.Drawing.Size(75, 23);
            this.btnShowLayoutEditor.TabIndex = 10;
            this.btnShowLayoutEditor.Text = "Layout";
            this.btnShowLayoutEditor.UseVisualStyleBackColor = true;
            this.btnShowLayoutEditor.Click += new System.EventHandler(this.btnShowLayoutEditor_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(516, 10);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(75, 23);
            this.btnRestore.TabIndex = 9;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(903, 10);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(75, 23);
            this.btnMove.TabIndex = 8;
            this.btnMove.Text = "Move";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(1094, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnPublish
            // 
            this.btnPublish.Location = new System.Drawing.Point(822, 10);
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Size = new System.Drawing.Size(75, 23);
            this.btnPublish.TabIndex = 5;
            this.btnPublish.Text = "Publish";
            this.btnPublish.UseVisualStyleBackColor = true;
            this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
            // 
            // lbLastSyncDateTime
            // 
            this.lbLastSyncDateTime.AutoSize = true;
            this.lbLastSyncDateTime.Location = new System.Drawing.Point(391, 15);
            this.lbLastSyncDateTime.Name = "lbLastSyncDateTime";
            this.lbLastSyncDateTime.Size = new System.Drawing.Size(83, 13);
            this.lbLastSyncDateTime.TabIndex = 3;
            this.lbLastSyncDateTime.Text = "<Never Sync\'d>";
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(310, 10);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 2;
            this.btnSync.Text = "Archive";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnIndexManager
            // 
            this.btnIndexManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIndexManager.Location = new System.Drawing.Point(995, 10);
            this.btnIndexManager.Name = "btnIndexManager";
            this.btnIndexManager.Size = new System.Drawing.Size(93, 23);
            this.btnIndexManager.TabIndex = 1;
            this.btnIndexManager.Text = "Index Manager";
            this.btnIndexManager.UseVisualStyleBackColor = true;
            this.btnIndexManager.Click += new System.EventHandler(this.btnIndexManager_Click);
            // 
            // chapterDropDown
            // 
            this.chapterDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chapterDropDown.FormattingEnabled = true;
            this.chapterDropDown.Location = new System.Drawing.Point(6, 12);
            this.chapterDropDown.Name = "chapterDropDown";
            this.chapterDropDown.Size = new System.Drawing.Size(298, 21);
            this.chapterDropDown.TabIndex = 0;
            this.chapterDropDown.SelectedIndexChanged += new System.EventHandler(this.listDropDown_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 39);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.statusStrip1);
            this.splitContainer1.Panel1.Controls.Add(this.chapterTree);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Panel1MinSize = 50;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Panel2MinSize = 800;
            this.splitContainer1.Size = new System.Drawing.Size(1192, 927);
            this.splitContainer1.SplitterDistance = 298;
            this.splitContainer1.SplitterIncrement = 10;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 1;
            this.splitContainer1.Leave += new System.EventHandler(this.splitContainer1_Leave);
            // 
            // statusStrip1
            // 
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(4, 878);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(286, 41);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(283, 0);
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chapterTree
            // 
            this.chapterTree.AllowDrop = true;
            this.chapterTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chapterTree.HideSelection = false;
            this.chapterTree.Location = new System.Drawing.Point(4, 4);
            this.chapterTree.Name = "chapterTree";
            this.chapterTree.Size = new System.Drawing.Size(286, 915);
            this.chapterTree.TabIndex = 0;
            this.chapterTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.chapterTree_ItemDrag);
            this.chapterTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sectionTree_AfterSelect);
            this.chapterTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.chapterTree_DragDrop);
            this.chapterTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.chapterTree_DragEnter);
            this.chapterTree.DragOver += new System.Windows.Forms.DragEventHandler(this.chapterTree_DragOver);
            this.chapterTree.DragLeave += new System.EventHandler(this.chapterTree_DragLeave);
            this.chapterTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chapterTree_MouseUp);
            // 
            // navTreeContextMenuStrip
            // 
            this.navTreeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reorderPreviousToolStripMenuItem,
            this.reorderNextToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addChildNodeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.deleteToolStripMenuItem});
            this.navTreeContextMenuStrip.Name = "contextMenuStripEvents";
            this.navTreeContextMenuStrip.Size = new System.Drawing.Size(153, 104);
            // 
            // reorderPreviousToolStripMenuItem
            // 
            this.reorderPreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.reorder_previous;
            this.reorderPreviousToolStripMenuItem.Name = "reorderPreviousToolStripMenuItem";
            this.reorderPreviousToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reorderPreviousToolStripMenuItem.Text = "Move &Previous";
            this.reorderPreviousToolStripMenuItem.Click += new System.EventHandler(this.reorderNodeToPrevious_Click);
            // 
            // reorderNextToolStripMenuItem
            // 
            this.reorderNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.reorder_next;
            this.reorderNextToolStripMenuItem.Name = "reorderNextToolStripMenuItem";
            this.reorderNextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reorderNextToolStripMenuItem.Text = "Move &Next";
            this.reorderNextToolStripMenuItem.Click += new System.EventHandler(this.reorderNodeToNext_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // addChildNodeToolStripMenuItem
            // 
            this.addChildNodeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addChildNodeToolStripMenuItem.Name = "addChildNodeToolStripMenuItem";
            this.addChildNodeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addChildNodeToolStripMenuItem.Text = "&Add Entry";
            this.addChildNodeToolStripMenuItem.Click += new System.EventHandler(this.addChildNode_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteNode_Click);
            // 
            // entitiesBindingSource
            // 
            this.entitiesBindingSource.DataSource = typeof(BDEditor.DataModel.Entities);
            // 
            // entitiesBindingSource1
            // 
            this.entitiesBindingSource1.DataSource = typeof(BDEditor.DataModel.Entities);
            // 
            // BDEditView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1192, 966);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.headerPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1280, 1024);
            this.Name = "BDEditView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bugs & Drugs Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BDEditView_FormClosing);
            this.Load += new System.EventHandler(this.BDEditView_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.navTreeContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ComboBox chapterDropDown;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView chapterTree;
        private System.Windows.Forms.BindingSource entitiesBindingSource;
        private System.Windows.Forms.BindingSource entitiesBindingSource1;
        private System.Windows.Forms.Button btnIndexManager;
        private System.Windows.Forms.Label lbLastSyncDateTime;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnPublish;
        private System.Windows.Forms.ContextMenuStrip navTreeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addChildNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnShowLayoutEditor;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnDebug;

    }
}