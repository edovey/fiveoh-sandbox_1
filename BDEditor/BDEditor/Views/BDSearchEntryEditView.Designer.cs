namespace BDEditor.Views
{
    partial class BDSearchEntryEditView
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
            this.lbSelectedSearchEntries = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblSelectedSearchEntry = new System.Windows.Forms.Label();
            this.pnlSelectTerms = new System.Windows.Forms.Panel();
            this.cbFilterList = new System.Windows.Forms.CheckBox();
            this.tbEntryName = new System.Windows.Forms.TextBox();
            this.btnEditSearchEntry = new System.Windows.Forms.Button();
            this.btnDeleteSearchEntry = new System.Windows.Forms.Button();
            this.btnAddNewSearchEntry = new System.Windows.Forms.Button();
            this.btnRemoveFromSelected = new System.Windows.Forms.Button();
            this.btnAddToSelected = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lbSearchEntryAssociations = new System.Windows.Forms.ListBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDeleteAssociation = new System.Windows.Forms.Button();
            this.btnMoveAssnNext = new System.Windows.Forms.Button();
            this.btnMoveAssnPrevious = new System.Windows.Forms.Button();
            this.pnlSelectTerms.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbExistingSearchEntries
            // 
            this.lbExistingSearchEntries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbExistingSearchEntries.FormattingEnabled = true;
            this.lbExistingSearchEntries.Location = new System.Drawing.Point(19, 90);
            this.lbExistingSearchEntries.Name = "lbExistingSearchEntries";
            this.lbExistingSearchEntries.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbExistingSearchEntries.Size = new System.Drawing.Size(233, 303);
            this.lbExistingSearchEntries.TabIndex = 0;
            this.lbExistingSearchEntries.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbExistingIndexEntries_MouseDown);
            // 
            // lbSelectedSearchEntries
            // 
            this.lbSelectedSearchEntries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSelectedSearchEntries.FormattingEnabled = true;
            this.lbSelectedSearchEntries.Location = new System.Drawing.Point(316, 90);
            this.lbSelectedSearchEntries.Name = "lbSelectedSearchEntries";
            this.lbSelectedSearchEntries.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSelectedSearchEntries.Size = new System.Drawing.Size(233, 303);
            this.lbSelectedSearchEntries.TabIndex = 2;
            this.lbSelectedSearchEntries.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbSelectedSearchEntries_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Existing Index Entries";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(313, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(170, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Selected Index Entries";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(13, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(95, 20);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "nodeName";
            // 
            // lblSelectedSearchEntry
            // 
            this.lblSelectedSearchEntry.AutoSize = true;
            this.lblSelectedSearchEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedSearchEntry.Location = new System.Drawing.Point(118, 417);
            this.lblSelectedSearchEntry.Name = "lblSelectedSearchEntry";
            this.lblSelectedSearchEntry.Size = new System.Drawing.Size(132, 17);
            this.lblSelectedSearchEntry.TabIndex = 8;
            this.lblSelectedSearchEntry.Text = "index entry name";
            // 
            // pnlSelectTerms
            // 
            this.pnlSelectTerms.Controls.Add(this.cbFilterList);
            this.pnlSelectTerms.Controls.Add(this.tbEntryName);
            this.pnlSelectTerms.Controls.Add(this.btnEditSearchEntry);
            this.pnlSelectTerms.Controls.Add(this.btnDeleteSearchEntry);
            this.pnlSelectTerms.Controls.Add(this.lblName);
            this.pnlSelectTerms.Controls.Add(this.btnAddNewSearchEntry);
            this.pnlSelectTerms.Controls.Add(this.label1);
            this.pnlSelectTerms.Controls.Add(this.label2);
            this.pnlSelectTerms.Controls.Add(this.btnRemoveFromSelected);
            this.pnlSelectTerms.Controls.Add(this.btnAddToSelected);
            this.pnlSelectTerms.Controls.Add(this.lbExistingSearchEntries);
            this.pnlSelectTerms.Controls.Add(this.lbSelectedSearchEntries);
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
            this.cbFilterList.Location = new System.Drawing.Point(221, 67);
            this.cbFilterList.Name = "cbFilterList";
            this.cbFilterList.Size = new System.Drawing.Size(31, 16);
            this.cbFilterList.TabIndex = 20;
            this.cbFilterList.UseVisualStyleBackColor = true;
            this.cbFilterList.CheckedChanged += new System.EventHandler(this.cbFilterList_CheckedChanged);
            // 
            // tbEntryName
            // 
            this.tbEntryName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEntryName.Location = new System.Drawing.Point(19, 63);
            this.tbEntryName.Name = "tbEntryName";
            this.tbEntryName.Size = new System.Drawing.Size(196, 23);
            this.tbEntryName.TabIndex = 18;
            this.tbEntryName.TextChanged += new System.EventHandler(this.tbEntryName_TextChanged);
            this.tbEntryName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbEntryName_MouseDown);
            // 
            // btnEditSearchEntry
            // 
            this.btnEditSearchEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditSearchEntry.Enabled = false;
            this.btnEditSearchEntry.Image = global::BDEditor.Properties.Resources.edit_24x24;
            this.btnEditSearchEntry.Location = new System.Drawing.Point(267, 216);
            this.btnEditSearchEntry.Name = "btnEditSearchEntry";
            this.btnEditSearchEntry.Size = new System.Drawing.Size(34, 34);
            this.btnEditSearchEntry.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnEditSearchEntry, "Edit Index Entry");
            this.btnEditSearchEntry.UseVisualStyleBackColor = true;
            this.btnEditSearchEntry.Click += new System.EventHandler(this.btnEditSearchEntry_Click);
            // 
            // btnDeleteSearchEntry
            // 
            this.btnDeleteSearchEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSearchEntry.Enabled = false;
            this.btnDeleteSearchEntry.Image = global::BDEditor.Properties.Resources.del_16x16;
            this.btnDeleteSearchEntry.Location = new System.Drawing.Point(267, 330);
            this.btnDeleteSearchEntry.Name = "btnDeleteSearchEntry";
            this.btnDeleteSearchEntry.Size = new System.Drawing.Size(34, 34);
            this.btnDeleteSearchEntry.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnDeleteSearchEntry, "Remove Index Entry");
            this.btnDeleteSearchEntry.UseVisualStyleBackColor = true;
            this.btnDeleteSearchEntry.Visible = false;
            this.btnDeleteSearchEntry.Click += new System.EventHandler(this.btnDeleteSearchEntry_Click);
            // 
            // btnAddNewSearchEntry
            // 
            this.btnAddNewSearchEntry.Image = global::BDEditor.Properties.Resources.new_24x24;
            this.btnAddNewSearchEntry.Location = new System.Drawing.Point(267, 290);
            this.btnAddNewSearchEntry.Name = "btnAddNewSearchEntry";
            this.btnAddNewSearchEntry.Size = new System.Drawing.Size(34, 34);
            this.btnAddNewSearchEntry.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnAddNewSearchEntry, "New Index Entry");
            this.btnAddNewSearchEntry.UseVisualStyleBackColor = true;
            this.btnAddNewSearchEntry.Click += new System.EventHandler(this.btnAddNewSearchEntry_Click);
            // 
            // btnRemoveFromSelected
            // 
            this.btnRemoveFromSelected.Enabled = false;
            this.btnRemoveFromSelected.Image = global::BDEditor.Properties.Resources.arrow_180;
            this.btnRemoveFromSelected.Location = new System.Drawing.Point(267, 151);
            this.btnRemoveFromSelected.Name = "btnRemoveFromSelected";
            this.btnRemoveFromSelected.Size = new System.Drawing.Size(34, 34);
            this.btnRemoveFromSelected.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnRemoveFromSelected, "Remove from list");
            this.btnRemoveFromSelected.UseVisualStyleBackColor = true;
            this.btnRemoveFromSelected.Click += new System.EventHandler(this.btnRemoveFromSelected_Click);
            // 
            // btnAddToSelected
            // 
            this.btnAddToSelected.Enabled = false;
            this.btnAddToSelected.Image = global::BDEditor.Properties.Resources.arrow;
            this.btnAddToSelected.Location = new System.Drawing.Point(267, 115);
            this.btnAddToSelected.Name = "btnAddToSelected";
            this.btnAddToSelected.Size = new System.Drawing.Size(34, 34);
            this.btnAddToSelected.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnAddToSelected, "Add to list");
            this.btnAddToSelected.UseVisualStyleBackColor = true;
            this.btnAddToSelected.Click += new System.EventHandler(this.btnAddToSelected_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 421);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Locations linked to:";
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
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(478, 755);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnDeleteAssociation
            // 
            this.btnDeleteAssociation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteAssociation.Enabled = false;
            this.btnDeleteAssociation.Image = global::BDEditor.Properties.Resources.delete_record_16;
            this.btnDeleteAssociation.Location = new System.Drawing.Point(312, 711);
            this.btnDeleteAssociation.Name = "btnDeleteAssociation";
            this.btnDeleteAssociation.Size = new System.Drawing.Size(40, 30);
            this.btnDeleteAssociation.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnDeleteAssociation, "Delete selected association");
            this.btnDeleteAssociation.UseVisualStyleBackColor = true;
            this.btnDeleteAssociation.Visible = false;
            this.btnDeleteAssociation.Click += new System.EventHandler(this.btnDeleteAssociation_Click);
            // 
            // btnMoveAssnNext
            // 
            this.btnMoveAssnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveAssnNext.Enabled = false;
            this.btnMoveAssnNext.Image = global::BDEditor.Properties.Resources.arrow_270;
            this.btnMoveAssnNext.Location = new System.Drawing.Point(242, 711);
            this.btnMoveAssnNext.Name = "btnMoveAssnNext";
            this.btnMoveAssnNext.Size = new System.Drawing.Size(40, 30);
            this.btnMoveAssnNext.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnMoveAssnNext, "Move Next");
            this.btnMoveAssnNext.UseVisualStyleBackColor = true;
            this.btnMoveAssnNext.Click += new System.EventHandler(this.btnMoveAssnNext_Click);
            // 
            // btnMoveAssnPrevious
            // 
            this.btnMoveAssnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveAssnPrevious.Enabled = false;
            this.btnMoveAssnPrevious.Image = global::BDEditor.Properties.Resources.arrow_090;
            this.btnMoveAssnPrevious.Location = new System.Drawing.Point(193, 711);
            this.btnMoveAssnPrevious.Name = "btnMoveAssnPrevious";
            this.btnMoveAssnPrevious.Size = new System.Drawing.Size(40, 30);
            this.btnMoveAssnPrevious.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnMoveAssnPrevious, "Move Previous");
            this.btnMoveAssnPrevious.UseVisualStyleBackColor = true;
            this.btnMoveAssnPrevious.Click += new System.EventHandler(this.btnMoveAssnPrevious_Click);
            // 
            // BDSearchEntryEditView
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 788);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnDeleteAssociation);
            this.Controls.Add(this.btnMoveAssnNext);
            this.Controls.Add(this.btnMoveAssnPrevious);
            this.Controls.Add(this.lbSearchEntryAssociations);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pnlSelectTerms);
            this.Controls.Add(this.lblSelectedSearchEntry);
            this.Name = "BDSearchEntryEditView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Index Entry Editor";
            this.Load += new System.EventHandler(this.BDSearchEntryEditView_Load);
            this.pnlSelectTerms.ResumeLayout(false);
            this.pnlSelectTerms.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbExistingSearchEntries;
        private System.Windows.Forms.ListBox lbSelectedSearchEntries;
        private System.Windows.Forms.Button btnAddToSelected;
        private System.Windows.Forms.Button btnRemoveFromSelected;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddNewSearchEntry;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblSelectedSearchEntry;
        private System.Windows.Forms.Panel pnlSelectTerms;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbSearchEntryAssociations;
        private System.Windows.Forms.Button btnMoveAssnPrevious;
        private System.Windows.Forms.Button btnMoveAssnNext;
        private System.Windows.Forms.Button btnDeleteAssociation;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnEditSearchEntry;
        private System.Windows.Forms.Button btnDeleteSearchEntry;
        private System.Windows.Forms.TextBox tbEntryName;
        private System.Windows.Forms.CheckBox cbFilterList;
    }
}