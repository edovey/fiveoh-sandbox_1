﻿namespace BDEditor.Views
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
            this.btnImportFromProduction = new System.Windows.Forms.Button();
            this.btnPublish = new System.Windows.Forms.Button();
            this.btnSyncWithReplaceLocal = new System.Windows.Forms.Button();
            this.lbLastSyncDateTime = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.loadSeedDataButton = new System.Windows.Forms.Button();
            this.chapterDropDown = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chapterTree = new System.Windows.Forms.TreeView();
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
            this.headerPanel.Controls.Add(this.btnImportFromProduction);
            this.headerPanel.Controls.Add(this.btnPublish);
            this.headerPanel.Controls.Add(this.btnSyncWithReplaceLocal);
            this.headerPanel.Controls.Add(this.lbLastSyncDateTime);
            this.headerPanel.Controls.Add(this.btnSync);
            this.headerPanel.Controls.Add(this.loadSeedDataButton);
            this.headerPanel.Controls.Add(this.chapterDropDown);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1192, 39);
            this.headerPanel.TabIndex = 0;
            // 
            // btnImportFromProduction
            // 
            this.btnImportFromProduction.Location = new System.Drawing.Point(619, 10);
            this.btnImportFromProduction.Name = "btnImportFromProduction";
            this.btnImportFromProduction.Size = new System.Drawing.Size(103, 23);
            this.btnImportFromProduction.TabIndex = 6;
            this.btnImportFromProduction.Text = "AWS Prod Import";
            this.btnImportFromProduction.UseVisualStyleBackColor = true;
            this.btnImportFromProduction.Visible = false;
            this.btnImportFromProduction.Click += new System.EventHandler(this.btnImportFromProduction_Click);
            // 
            // btnPublish
            // 
            this.btnPublish.Location = new System.Drawing.Point(728, 10);
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Size = new System.Drawing.Size(75, 23);
            this.btnPublish.TabIndex = 5;
            this.btnPublish.Text = "Publish";
            this.btnPublish.UseVisualStyleBackColor = true;
            this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
            // 
            // btnSyncWithReplaceLocal
            // 
            this.btnSyncWithReplaceLocal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSyncWithReplaceLocal.Location = new System.Drawing.Point(919, 10);
            this.btnSyncWithReplaceLocal.Name = "btnSyncWithReplaceLocal";
            this.btnSyncWithReplaceLocal.Size = new System.Drawing.Size(261, 23);
            this.btnSyncWithReplaceLocal.TabIndex = 4;
            this.btnSyncWithReplaceLocal.Text = "(System Option) Replace Local from Repository";
            this.btnSyncWithReplaceLocal.UseVisualStyleBackColor = true;
            this.btnSyncWithReplaceLocal.Click += new System.EventHandler(this.btnSyncWithReplaceLocal_Click);
            // 
            // lbLastSyncDateTime
            // 
            this.lbLastSyncDateTime.AutoSize = true;
            this.lbLastSyncDateTime.Location = new System.Drawing.Point(391, 15);
            this.lbLastSyncDateTime.Name = "lbLastSyncDateTime";
            this.lbLastSyncDateTime.Size = new System.Drawing.Size(83, 13);
            this.lbLastSyncDateTime.TabIndex = 3;
            this.lbLastSyncDateTime.Text = "<Never Sync\'d>";
            this.lbLastSyncDateTime.Click += new System.EventHandler(this.lbLastSyncDateTime_Click);
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(310, 10);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 2;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // loadSeedDataButton
            // 
            this.loadSeedDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadSeedDataButton.Location = new System.Drawing.Point(809, 10);
            this.loadSeedDataButton.Name = "loadSeedDataButton";
            this.loadSeedDataButton.Size = new System.Drawing.Size(104, 23);
            this.loadSeedDataButton.TabIndex = 1;
            this.loadSeedDataButton.Text = "Load Seed Data";
            this.loadSeedDataButton.UseVisualStyleBackColor = true;
            this.loadSeedDataButton.Click += new System.EventHandler(this.loadSeedData_Click);
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
            // chapterTree
            // 
            this.chapterTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chapterTree.HideSelection = false;
            this.chapterTree.Location = new System.Drawing.Point(4, 4);
            this.chapterTree.Name = "chapterTree";
            this.chapterTree.Size = new System.Drawing.Size(286, 915);
            this.chapterTree.TabIndex = 0;
            this.chapterTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sectionTree_AfterSelect);
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.Button loadSeedDataButton;
        private System.Windows.Forms.Label lbLastSyncDateTime;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnSyncWithReplaceLocal;
        private System.Windows.Forms.Button btnPublish;
        private System.Windows.Forms.Button btnImportFromProduction;

    }
}