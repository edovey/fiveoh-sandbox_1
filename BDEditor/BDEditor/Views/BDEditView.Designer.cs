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
            this.lbLastSyncDateTime = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.createTestDataButton = new System.Windows.Forms.Button();
            this.sectionDropDown = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sectionTree = new System.Windows.Forms.TreeView();
            this.entitiesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.entitiesBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.lbLastSyncDateTime);
            this.headerPanel.Controls.Add(this.btnSync);
            this.headerPanel.Controls.Add(this.createTestDataButton);
            this.headerPanel.Controls.Add(this.sectionDropDown);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1148, 39);
            this.headerPanel.TabIndex = 0;
            // 
            // lbLastSyncDateTime
            // 
            this.lbLastSyncDateTime.AutoSize = true;
            this.lbLastSyncDateTime.Location = new System.Drawing.Point(479, 15);
            this.lbLastSyncDateTime.Name = "lbLastSyncDateTime";
            this.lbLastSyncDateTime.Size = new System.Drawing.Size(83, 13);
            this.lbLastSyncDateTime.TabIndex = 3;
            this.lbLastSyncDateTime.Text = "<Never Sync\'d>";
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(398, 10);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 2;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // createTestDataButton
            // 
            this.createTestDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createTestDataButton.Location = new System.Drawing.Point(1061, 10);
            this.createTestDataButton.Name = "createTestDataButton";
            this.createTestDataButton.Size = new System.Drawing.Size(75, 23);
            this.createTestDataButton.TabIndex = 1;
            this.createTestDataButton.Text = "Test Data";
            this.createTestDataButton.UseVisualStyleBackColor = true;
            this.createTestDataButton.Click += new System.EventHandler(this.createTestDataButton_Click);
            // 
            // sectionDropDown
            // 
            this.sectionDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sectionDropDown.FormattingEnabled = true;
            this.sectionDropDown.Location = new System.Drawing.Point(3, 12);
            this.sectionDropDown.Name = "sectionDropDown";
            this.sectionDropDown.Size = new System.Drawing.Size(379, 21);
            this.sectionDropDown.TabIndex = 0;
            this.sectionDropDown.SelectedIndexChanged += new System.EventHandler(this.sectionDropDown_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 39);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.sectionTree);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Size = new System.Drawing.Size(1148, 661);
            this.splitContainer1.SplitterDistance = 378;
            this.splitContainer1.TabIndex = 1;
            // 
            // sectionTree
            // 
            this.sectionTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sectionTree.Location = new System.Drawing.Point(4, 4);
            this.sectionTree.Name = "sectionTree";
            this.sectionTree.Size = new System.Drawing.Size(370, 653);
            this.sectionTree.TabIndex = 0;
            this.sectionTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sectionTree_AfterSelect);
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
            this.ClientSize = new System.Drawing.Size(1148, 700);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.headerPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BDEditView";
            this.Text = "BDEditView";
            this.Load += new System.EventHandler(this.BDEditView_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ComboBox sectionDropDown;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView sectionTree;
        private System.Windows.Forms.BindingSource entitiesBindingSource;
        private System.Windows.Forms.BindingSource entitiesBindingSource1;
        private System.Windows.Forms.Button createTestDataButton;
        private System.Windows.Forms.Label lbLastSyncDateTime;
        private System.Windows.Forms.Button btnSync;

    }
}