namespace BDEditor.Views
{
    partial class BDBuildView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDBuildView));
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbGeneratePages = new System.Windows.Forms.CheckBox();
            this.rbAllChapters = new System.Windows.Forms.RadioButton();
            this.rbSelectedChapters = new System.Windows.Forms.RadioButton();
            this.gbSelectHtmlPages = new System.Windows.Forms.GroupBox();
            this.clbChaptersToGenerate = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbUUIDsToGenerate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbSyncWithAws = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbGenerateSearch = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbSyncHtml = new System.Windows.Forms.CheckBox();
            this.cbSyncSearch = new System.Windows.Forms.CheckBox();
            this.gbSyncRecordTypes = new System.Windows.Forms.GroupBox();
            this.flagGreen = new System.Windows.Forms.PictureBox();
            this.flagRed = new System.Windows.Forms.PictureBox();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.cbSelectedUuids = new System.Windows.Forms.CheckBox();
            this.gbSelectHtmlPages.SuspendLayout();
            this.gbSyncRecordTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flagGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.flagRed)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_OK
            // 
            this.btn_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_OK.Enabled = false;
            this.btn_OK.Location = new System.Drawing.Point(323, 637);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 7;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(242, 637);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 8;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Generate HTML Pages";
            // 
            // cbGeneratePages
            // 
            this.cbGeneratePages.AutoSize = true;
            this.cbGeneratePages.Location = new System.Drawing.Point(12, 18);
            this.cbGeneratePages.Name = "cbGeneratePages";
            this.cbGeneratePages.Size = new System.Drawing.Size(15, 14);
            this.cbGeneratePages.TabIndex = 0;
            this.cbGeneratePages.UseVisualStyleBackColor = true;
            this.cbGeneratePages.CheckedChanged += new System.EventHandler(this.cbGeneratePages_CheckedChanged);
            // 
            // rbAllChapters
            // 
            this.rbAllChapters.AutoSize = true;
            this.rbAllChapters.Checked = true;
            this.rbAllChapters.Location = new System.Drawing.Point(18, 9);
            this.rbAllChapters.Name = "rbAllChapters";
            this.rbAllChapters.Size = new System.Drawing.Size(81, 17);
            this.rbAllChapters.TabIndex = 0;
            this.rbAllChapters.TabStop = true;
            this.rbAllChapters.Text = "All Chapters";
            this.rbAllChapters.UseVisualStyleBackColor = true;
            this.rbAllChapters.CheckedChanged += new System.EventHandler(this.rbAllChapters_CheckedChanged);
            // 
            // rbSelectedChapters
            // 
            this.rbSelectedChapters.AutoSize = true;
            this.rbSelectedChapters.Location = new System.Drawing.Point(122, 9);
            this.rbSelectedChapters.Name = "rbSelectedChapters";
            this.rbSelectedChapters.Size = new System.Drawing.Size(112, 17);
            this.rbSelectedChapters.TabIndex = 1;
            this.rbSelectedChapters.Text = "Selected Chapters";
            this.rbSelectedChapters.UseVisualStyleBackColor = true;
            this.rbSelectedChapters.CheckedChanged += new System.EventHandler(this.rbSelectedChapters_CheckedChanged);
            // 
            // gbSelectHtmlPages
            // 
            this.gbSelectHtmlPages.Controls.Add(this.rbAllChapters);
            this.gbSelectHtmlPages.Controls.Add(this.rbSelectedChapters);
            this.gbSelectHtmlPages.Enabled = false;
            this.gbSelectHtmlPages.Location = new System.Drawing.Point(20, 75);
            this.gbSelectHtmlPages.Name = "gbSelectHtmlPages";
            this.gbSelectHtmlPages.Size = new System.Drawing.Size(249, 34);
            this.gbSelectHtmlPages.TabIndex = 2;
            this.gbSelectHtmlPages.TabStop = false;
            // 
            // clbChaptersToGenerate
            // 
            this.clbChaptersToGenerate.CheckOnClick = true;
            this.clbChaptersToGenerate.Enabled = false;
            this.clbChaptersToGenerate.FormattingEnabled = true;
            this.clbChaptersToGenerate.Location = new System.Drawing.Point(49, 136);
            this.clbChaptersToGenerate.Name = "clbChaptersToGenerate";
            this.clbChaptersToGenerate.Size = new System.Drawing.Size(327, 124);
            this.clbChaptersToGenerate.TabIndex = 3;
            this.clbChaptersToGenerate.SelectedIndexChanged += new System.EventHandler(this.clbChaptersToGenerate_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Select the chapters to include:";
            // 
            // tbUUIDsToGenerate
            // 
            this.tbUUIDsToGenerate.Enabled = false;
            this.tbUUIDsToGenerate.Location = new System.Drawing.Point(49, 335);
            this.tbUUIDsToGenerate.Multiline = true;
            this.tbUUIDsToGenerate.Name = "tbUUIDsToGenerate";
            this.tbUUIDsToGenerate.Size = new System.Drawing.Size(327, 157);
            this.tbUUIDsToGenerate.TabIndex = 4;
            this.tbUUIDsToGenerate.TextChanged += new System.EventHandler(this.tbUUIDsToGenerate_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 312);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(305, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Paste the UUIDs of the Nodes to generate (one UUID per line):";
            // 
            // cbSyncWithAws
            // 
            this.cbSyncWithAws.AutoSize = true;
            this.cbSyncWithAws.Enabled = false;
            this.cbSyncWithAws.Location = new System.Drawing.Point(12, 520);
            this.cbSyncWithAws.Name = "cbSyncWithAws";
            this.cbSyncWithAws.Size = new System.Drawing.Size(15, 14);
            this.cbSyncWithAws.TabIndex = 5;
            this.cbSyncWithAws.UseVisualStyleBackColor = true;
            this.cbSyncWithAws.CheckedChanged += new System.EventHandler(this.cbSyncWithAws_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(33, 517);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "Sync with AWS";
            // 
            // cbGenerateSearch
            // 
            this.cbGenerateSearch.AutoSize = true;
            this.cbGenerateSearch.Location = new System.Drawing.Point(12, 46);
            this.cbGenerateSearch.Name = "cbGenerateSearch";
            this.cbGenerateSearch.Size = new System.Drawing.Size(15, 14);
            this.cbGenerateSearch.TabIndex = 1;
            this.cbGenerateSearch.UseVisualStyleBackColor = true;
            this.cbGenerateSearch.CheckedChanged += new System.EventHandler(this.cbGenerateSearch_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(33, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(188, 17);
            this.label5.TabIndex = 13;
            this.label5.Text = "Generate Search Entries";
            // 
            // cbSyncHtml
            // 
            this.cbSyncHtml.AutoSize = true;
            this.cbSyncHtml.Location = new System.Drawing.Point(29, 23);
            this.cbSyncHtml.Name = "cbSyncHtml";
            this.cbSyncHtml.Size = new System.Drawing.Size(89, 17);
            this.cbSyncHtml.TabIndex = 0;
            this.cbSyncHtml.Text = "HTML Pages";
            this.cbSyncHtml.UseVisualStyleBackColor = true;
            // 
            // cbSyncSearch
            // 
            this.cbSyncSearch.AutoSize = true;
            this.cbSyncSearch.Location = new System.Drawing.Point(29, 46);
            this.cbSyncSearch.Name = "cbSyncSearch";
            this.cbSyncSearch.Size = new System.Drawing.Size(95, 17);
            this.cbSyncSearch.TabIndex = 1;
            this.cbSyncSearch.Text = "Search Entries";
            this.cbSyncSearch.UseVisualStyleBackColor = true;
            // 
            // gbSyncRecordTypes
            // 
            this.gbSyncRecordTypes.Controls.Add(this.cbSyncHtml);
            this.gbSyncRecordTypes.Controls.Add(this.cbSyncSearch);
            this.gbSyncRecordTypes.Enabled = false;
            this.gbSyncRecordTypes.Location = new System.Drawing.Point(20, 540);
            this.gbSyncRecordTypes.Name = "gbSyncRecordTypes";
            this.gbSyncRecordTypes.Size = new System.Drawing.Size(356, 80);
            this.gbSyncRecordTypes.TabIndex = 6;
            this.gbSyncRecordTypes.TabStop = false;
            this.gbSyncRecordTypes.Text = "Include records for:";
            // 
            // flagGreen
            // 
            this.flagGreen.Image = ((System.Drawing.Image)(resources.GetObject("flagGreen.Image")));
            this.flagGreen.Location = new System.Drawing.Point(348, 517);
            this.flagGreen.Name = "flagGreen";
            this.flagGreen.Size = new System.Drawing.Size(22, 26);
            this.flagGreen.TabIndex = 14;
            this.flagGreen.TabStop = false;
            this.flagGreen.Visible = false;
            // 
            // flagRed
            // 
            this.flagRed.Image = global::BDEditor.Properties.Resources.Flag_Red;
            this.flagRed.Location = new System.Drawing.Point(348, 517);
            this.flagRed.Name = "flagRed";
            this.flagRed.Size = new System.Drawing.Size(22, 26);
            this.flagRed.TabIndex = 15;
            this.flagRed.TabStop = false;
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(154, 519);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.PasswordChar = '*';
            this.maskedTextBox1.Size = new System.Drawing.Size(188, 20);
            this.maskedTextBox1.TabIndex = 16;
            this.maskedTextBox1.TextChanged += new System.EventHandler(this.maskedTextBox1_TextChanged);
            // 
            // cbSelectedUuids
            // 
            this.cbSelectedUuids.AutoSize = true;
            this.cbSelectedUuids.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSelectedUuids.Location = new System.Drawing.Point(36, 281);
            this.cbSelectedUuids.Name = "cbSelectedUuids";
            this.cbSelectedUuids.Size = new System.Drawing.Size(144, 20);
            this.cbSelectedUuids.TabIndex = 17;
            this.cbSelectedUuids.Text = "Selected UUIDs";
            this.cbSelectedUuids.UseVisualStyleBackColor = true;
            this.cbSelectedUuids.CheckedChanged += new System.EventHandler(this.cbSelectedUuids_CheckedChanged);
            // 
            // BDBuildView
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(413, 672);
            this.Controls.Add(this.cbSelectedUuids);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.flagGreen);
            this.Controls.Add(this.cbGenerateSearch);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbSyncWithAws);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbUUIDsToGenerate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clbChaptersToGenerate);
            this.Controls.Add(this.cbGeneratePages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.gbSelectHtmlPages);
            this.Controls.Add(this.gbSyncRecordTypes);
            this.Controls.Add(this.flagRed);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BDBuildView";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Publish";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.BDBuildView_Load);
            this.gbSelectHtmlPages.ResumeLayout(false);
            this.gbSelectHtmlPages.PerformLayout();
            this.gbSyncRecordTypes.ResumeLayout(false);
            this.gbSyncRecordTypes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flagGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.flagRed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbGeneratePages;
        private System.Windows.Forms.RadioButton rbAllChapters;
        private System.Windows.Forms.RadioButton rbSelectedChapters;
        private System.Windows.Forms.GroupBox gbSelectHtmlPages;
        private System.Windows.Forms.CheckedListBox clbChaptersToGenerate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbUUIDsToGenerate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbSyncWithAws;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbGenerateSearch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbSyncHtml;
        private System.Windows.Forms.CheckBox cbSyncSearch;
        private System.Windows.Forms.GroupBox gbSyncRecordTypes;
        private System.Windows.Forms.PictureBox flagGreen;
        private System.Windows.Forms.PictureBox flagRed;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.CheckBox cbSelectedUuids;
    }
}