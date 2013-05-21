namespace BDEditor.Views
{
    partial class BDSearchEntryManagerView
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
            this.lbExistingSearchEntries = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlSelectTerms = new System.Windows.Forms.Panel();
            this.cbFilterList = new System.Windows.Forms.CheckBox();
            this.tbEntryName = new System.Windows.Forms.TextBox();
            this.btnEditSearchEntry = new System.Windows.Forms.Button();
            this.btnDeleteSearchEntry = new System.Windows.Forms.Button();
            this.btnAddNewSearchEntry = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lbSearchEntryAssociations = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDeleteAssociation = new System.Windows.Forms.Button();
            this.btnMoveAssnNext = new System.Windows.Forms.Button();
            this.btnMoveAssnPrevious = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlSelectTerms.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbExistingSearchEntries
            // 
            this.lbExistingSearchEntries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbExistingSearchEntries.FormattingEnabled = true;
            this.lbExistingSearchEntries.Location = new System.Drawing.Point(19, 77);
            this.lbExistingSearchEntries.Name = "lbExistingSearchEntries";
            this.lbExistingSearchEntries.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbExistingSearchEntries.Size = new System.Drawing.Size(534, 290);
            this.lbExistingSearchEntries.TabIndex = 0;
            this.lbExistingSearchEntries.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbExistingIndexEntries_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Index Entries";
            // 
            // pnlSelectTerms
            // 
            this.pnlSelectTerms.Controls.Add(this.cbFilterList);
            this.pnlSelectTerms.Controls.Add(this.tbEntryName);
            this.pnlSelectTerms.Controls.Add(this.btnEditSearchEntry);
            this.pnlSelectTerms.Controls.Add(this.btnDeleteSearchEntry);
            this.pnlSelectTerms.Controls.Add(this.btnAddNewSearchEntry);
            this.pnlSelectTerms.Controls.Add(this.label1);
            this.pnlSelectTerms.Controls.Add(this.lbExistingSearchEntries);
            this.pnlSelectTerms.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSelectTerms.Location = new System.Drawing.Point(0, 0);
            this.pnlSelectTerms.Name = "pnlSelectTerms";
            this.pnlSelectTerms.Size = new System.Drawing.Size(565, 414);
            this.pnlSelectTerms.TabIndex = 0;
            // 
            // cbFilterList
            // 
            this.cbFilterList.AutoSize = true;
            this.cbFilterList.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFilterList.Image = global::BDEditor.Properties.Resources.filter_16x16;
            this.cbFilterList.Location = new System.Drawing.Point(220, 43);
            this.cbFilterList.Name = "cbFilterList";
            this.cbFilterList.Size = new System.Drawing.Size(31, 16);
            this.cbFilterList.TabIndex = 20;
            this.cbFilterList.UseVisualStyleBackColor = true;
            this.cbFilterList.CheckedChanged += new System.EventHandler(this.cbFilterList_CheckedChanged);
            // 
            // tbEntryName
            // 
            this.tbEntryName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEntryName.Location = new System.Drawing.Point(18, 39);
            this.tbEntryName.Name = "tbEntryName";
            this.tbEntryName.Size = new System.Drawing.Size(196, 23);
            this.tbEntryName.TabIndex = 18;
            this.tbEntryName.TextChanged += new System.EventHandler(this.tbEntryName_TextChanged);
            this.tbEntryName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbEntryName_MouseDown);
            // 
            // btnEditSearchEntry
            // 
            this.btnEditSearchEntry.Enabled = false;
            this.btnEditSearchEntry.Image = global::BDEditor.Properties.Resources.edit_24x24;
            this.btnEditSearchEntry.Location = new System.Drawing.Point(235, 373);
            this.btnEditSearchEntry.Name = "btnEditSearchEntry";
            this.btnEditSearchEntry.Size = new System.Drawing.Size(34, 34);
            this.btnEditSearchEntry.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnEditSearchEntry, "Edit Index Entry");
            this.btnEditSearchEntry.UseVisualStyleBackColor = true;
            this.btnEditSearchEntry.Click += new System.EventHandler(this.btnEditSearchEntry_Click);
            // 
            // btnDeleteSearchEntry
            // 
            this.btnDeleteSearchEntry.Enabled = false;
            this.btnDeleteSearchEntry.Image = global::BDEditor.Properties.Resources.del_16x16;
            this.btnDeleteSearchEntry.Location = new System.Drawing.Point(289, 373);
            this.btnDeleteSearchEntry.Name = "btnDeleteSearchEntry";
            this.btnDeleteSearchEntry.Size = new System.Drawing.Size(34, 34);
            this.btnDeleteSearchEntry.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnDeleteSearchEntry, "Remove Index Entry");
            this.btnDeleteSearchEntry.UseVisualStyleBackColor = true;
            this.btnDeleteSearchEntry.Click += new System.EventHandler(this.btnDeleteSearchEntry_Click);
            // 
            // btnAddNewSearchEntry
            // 
            this.btnAddNewSearchEntry.Enabled = false;
            this.btnAddNewSearchEntry.Image = global::BDEditor.Properties.Resources.new_24x24;
            this.btnAddNewSearchEntry.Location = new System.Drawing.Point(195, 373);
            this.btnAddNewSearchEntry.Name = "btnAddNewSearchEntry";
            this.btnAddNewSearchEntry.Size = new System.Drawing.Size(34, 34);
            this.btnAddNewSearchEntry.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnAddNewSearchEntry, "New Index Entry");
            this.btnAddNewSearchEntry.UseVisualStyleBackColor = true;
            this.btnAddNewSearchEntry.Visible = false;
            this.btnAddNewSearchEntry.Click += new System.EventHandler(this.btnAddNewSearchEntry_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 421);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "Linked Locations:";
            // 
            // lbSearchEntryAssociations
            // 
            this.lbSearchEntryAssociations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSearchEntryAssociations.FormattingEnabled = true;
            this.lbSearchEntryAssociations.HorizontalScrollbar = true;
            this.lbSearchEntryAssociations.Location = new System.Drawing.Point(17, 437);
            this.lbSearchEntryAssociations.Name = "lbSearchEntryAssociations";
            this.lbSearchEntryAssociations.Size = new System.Drawing.Size(536, 264);
            this.lbSearchEntryAssociations.TabIndex = 1;
            this.lbSearchEntryAssociations.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbSearchEntryAssociations_MouseDown);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(478, 755);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDeleteAssociation
            // 
            this.btnDeleteAssociation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteAssociation.Enabled = false;
            this.btnDeleteAssociation.Image = global::BDEditor.Properties.Resources.delete_record_16;
            this.btnDeleteAssociation.Location = new System.Drawing.Point(314, 711);
            this.btnDeleteAssociation.Name = "btnDeleteAssociation";
            this.btnDeleteAssociation.Size = new System.Drawing.Size(34, 34);
            this.btnDeleteAssociation.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnDeleteAssociation, "Delete selected association");
            this.btnDeleteAssociation.UseVisualStyleBackColor = true;
            this.btnDeleteAssociation.Click += new System.EventHandler(this.btnDeleteAssociation_Click);
            // 
            // btnMoveAssnNext
            // 
            this.btnMoveAssnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMoveAssnNext.Enabled = false;
            this.btnMoveAssnNext.Image = global::BDEditor.Properties.Resources.arrow_270;
            this.btnMoveAssnNext.Location = new System.Drawing.Point(260, 711);
            this.btnMoveAssnNext.Name = "btnMoveAssnNext";
            this.btnMoveAssnNext.Size = new System.Drawing.Size(34, 34);
            this.btnMoveAssnNext.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnMoveAssnNext, "Move Next");
            this.btnMoveAssnNext.UseVisualStyleBackColor = true;
            this.btnMoveAssnNext.Click += new System.EventHandler(this.btnMoveAssnNext_Click);
            // 
            // btnMoveAssnPrevious
            // 
            this.btnMoveAssnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMoveAssnPrevious.Enabled = false;
            this.btnMoveAssnPrevious.Image = global::BDEditor.Properties.Resources.arrow_090;
            this.btnMoveAssnPrevious.Location = new System.Drawing.Point(220, 711);
            this.btnMoveAssnPrevious.Name = "btnMoveAssnPrevious";
            this.btnMoveAssnPrevious.Size = new System.Drawing.Size(34, 34);
            this.btnMoveAssnPrevious.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnMoveAssnPrevious, "Move Previous");
            this.btnMoveAssnPrevious.UseVisualStyleBackColor = true;
            this.btnMoveAssnPrevious.Click += new System.EventHandler(this.btnMoveAssnPrevious_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(397, 755);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // BDSearchEntryManagerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(565, 788);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDeleteAssociation);
            this.Controls.Add(this.btnMoveAssnNext);
            this.Controls.Add(this.btnMoveAssnPrevious);
            this.Controls.Add(this.lbSearchEntryAssociations);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pnlSelectTerms);
            this.Name = "BDSearchEntryManagerView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Index Entry Manager";
            this.Load += new System.EventHandler(this.BDSearchEntryEditView_Load);
            this.pnlSelectTerms.ResumeLayout(false);
            this.pnlSelectTerms.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbExistingSearchEntries;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddNewSearchEntry;
        private System.Windows.Forms.Panel pnlSelectTerms;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbSearchEntryAssociations;
        private System.Windows.Forms.Button btnMoveAssnPrevious;
        private System.Windows.Forms.Button btnMoveAssnNext;
        private System.Windows.Forms.Button btnDeleteAssociation;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnEditSearchEntry;
        private System.Windows.Forms.Button btnDeleteSearchEntry;
        private System.Windows.Forms.TextBox tbEntryName;
        private System.Windows.Forms.CheckBox cbFilterList;
        private System.Windows.Forms.Button btnSave;
    }
}