namespace BDEditor.Views
{
    partial class BDIndexEntryEditView
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlSelectTerms = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDeleteAssociation = new System.Windows.Forms.Button();
            this.btnMoveAssnNext = new System.Windows.Forms.Button();
            this.btnMoveAssnPrevious = new System.Windows.Forms.Button();
            this.btnEditEntryName = new System.Windows.Forms.Button();
            this.btnDeleteSearchEntry = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnRemoveFromSelected = new System.Windows.Forms.Button();
            this.btnAddToSelected = new System.Windows.Forms.Button();
            this.pnlSelectTerms.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(25, 64);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(209, 329);
            this.listBox1.TabIndex = 0;
            // 
            // listBox2
            // 
            this.listBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(305, 64);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(233, 329);
            this.listBox2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Available Index Entries";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(305, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 13);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(150, 417);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "index entry name";
            // 
            // pnlSelectTerms
            // 
            this.pnlSelectTerms.Controls.Add(this.btnEditEntryName);
            this.pnlSelectTerms.Controls.Add(this.btnDeleteSearchEntry);
            this.pnlSelectTerms.Controls.Add(this.lblName);
            this.pnlSelectTerms.Controls.Add(this.btnAddNew);
            this.pnlSelectTerms.Controls.Add(this.label1);
            this.pnlSelectTerms.Controls.Add(this.label2);
            this.pnlSelectTerms.Controls.Add(this.btnRemoveFromSelected);
            this.pnlSelectTerms.Controls.Add(this.btnAddToSelected);
            this.pnlSelectTerms.Controls.Add(this.listBox1);
            this.pnlSelectTerms.Controls.Add(this.listBox2);
            this.pnlSelectTerms.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSelectTerms.Location = new System.Drawing.Point(0, 0);
            this.pnlSelectTerms.Name = "pnlSelectTerms";
            this.pnlSelectTerms.Size = new System.Drawing.Size(565, 414);
            this.pnlSelectTerms.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 421);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Locations associated with:";
            // 
            // listBox3
            // 
            this.listBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox3.FormattingEnabled = true;
            this.listBox3.Location = new System.Drawing.Point(17, 437);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(536, 290);
            this.listBox3.TabIndex = 12;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(478, 755);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 16;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(397, 755);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDeleteAssociation
            // 
            this.btnDeleteAssociation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteAssociation.Image = global::BDEditor.Properties.Resources.delete_record_16;
            this.btnDeleteAssociation.Location = new System.Drawing.Point(243, 748);
            this.btnDeleteAssociation.Name = "btnDeleteAssociation";
            this.btnDeleteAssociation.Size = new System.Drawing.Size(40, 30);
            this.btnDeleteAssociation.TabIndex = 15;
            this.toolTip1.SetToolTip(this.btnDeleteAssociation, "Delete selected association");
            this.btnDeleteAssociation.UseVisualStyleBackColor = true;
            this.btnDeleteAssociation.Click += new System.EventHandler(this.btnDeleteAssociation_Click);
            // 
            // btnMoveAssnNext
            // 
            this.btnMoveAssnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveAssnNext.Image = global::BDEditor.Properties.Resources.arrow_270;
            this.btnMoveAssnNext.Location = new System.Drawing.Point(173, 748);
            this.btnMoveAssnNext.Name = "btnMoveAssnNext";
            this.btnMoveAssnNext.Size = new System.Drawing.Size(40, 30);
            this.btnMoveAssnNext.TabIndex = 14;
            this.toolTip1.SetToolTip(this.btnMoveAssnNext, "Move Next");
            this.btnMoveAssnNext.UseVisualStyleBackColor = true;
            this.btnMoveAssnNext.Click += new System.EventHandler(this.btnMoveAssnNext_Click);
            // 
            // btnMoveAssnPrevious
            // 
            this.btnMoveAssnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveAssnPrevious.Image = global::BDEditor.Properties.Resources.arrow_090;
            this.btnMoveAssnPrevious.Location = new System.Drawing.Point(124, 748);
            this.btnMoveAssnPrevious.Name = "btnMoveAssnPrevious";
            this.btnMoveAssnPrevious.Size = new System.Drawing.Size(40, 30);
            this.btnMoveAssnPrevious.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btnMoveAssnPrevious, "Move Previous");
            this.btnMoveAssnPrevious.UseVisualStyleBackColor = true;
            this.btnMoveAssnPrevious.Click += new System.EventHandler(this.btnMoveAssnPrevious_Click);
            // 
            // btnEditEntryName
            // 
            this.btnEditEntryName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditEntryName.Image = global::BDEditor.Properties.Resources.edit_16x16;
            this.btnEditEntryName.Location = new System.Drawing.Point(249, 340);
            this.btnEditEntryName.Name = "btnEditEntryName";
            this.btnEditEntryName.Size = new System.Drawing.Size(40, 30);
            this.btnEditEntryName.TabIndex = 17;
            this.toolTip1.SetToolTip(this.btnEditEntryName, "Edit Index Entry");
            this.btnEditEntryName.UseVisualStyleBackColor = true;
            this.btnEditEntryName.Click += new System.EventHandler(this.btnEditEntryName_Click);
            // 
            // btnDeleteSearchEntry
            // 
            this.btnDeleteSearchEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSearchEntry.Image = global::BDEditor.Properties.Resources.del_16x16;
            this.btnDeleteSearchEntry.Location = new System.Drawing.Point(249, 277);
            this.btnDeleteSearchEntry.Name = "btnDeleteSearchEntry";
            this.btnDeleteSearchEntry.Size = new System.Drawing.Size(40, 30);
            this.btnDeleteSearchEntry.TabIndex = 16;
            this.toolTip1.SetToolTip(this.btnDeleteSearchEntry, "Remove Index Entry");
            this.btnDeleteSearchEntry.UseVisualStyleBackColor = true;
            // 
            // btnAddNew
            // 
            this.btnAddNew.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.btnAddNew.Location = new System.Drawing.Point(249, 237);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(40, 34);
            this.btnAddNew.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnAddNew, "New Index Entry");
            this.btnAddNew.UseVisualStyleBackColor = true;
            // 
            // btnRemoveFromSelected
            // 
            this.btnRemoveFromSelected.Image = global::BDEditor.Properties.Resources.arrow_180;
            this.btnRemoveFromSelected.Location = new System.Drawing.Point(249, 110);
            this.btnRemoveFromSelected.Name = "btnRemoveFromSelected";
            this.btnRemoveFromSelected.Size = new System.Drawing.Size(40, 30);
            this.btnRemoveFromSelected.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnRemoveFromSelected, "Remove from list");
            this.btnRemoveFromSelected.UseVisualStyleBackColor = true;
            this.btnRemoveFromSelected.Click += new System.EventHandler(this.btnRemoveFromSelected_Click);
            // 
            // btnAddToSelected
            // 
            this.btnAddToSelected.Image = global::BDEditor.Properties.Resources.arrow;
            this.btnAddToSelected.Location = new System.Drawing.Point(249, 74);
            this.btnAddToSelected.Name = "btnAddToSelected";
            this.btnAddToSelected.Size = new System.Drawing.Size(40, 30);
            this.btnAddToSelected.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnAddToSelected, "Add to list");
            this.btnAddToSelected.UseVisualStyleBackColor = true;
            this.btnAddToSelected.Click += new System.EventHandler(this.btnAddToSelected_Click);
            // 
            // IndexEntryEditView
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(565, 788);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnDeleteAssociation);
            this.Controls.Add(this.btnMoveAssnNext);
            this.Controls.Add(this.btnMoveAssnPrevious);
            this.Controls.Add(this.listBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pnlSelectTerms);
            this.Controls.Add(this.label3);
            this.Name = "IndexEntryEditView";
            this.Text = "Index Entry Editor";
            this.Load += new System.EventHandler(this.IndexEntryEditView_Load);
            this.pnlSelectTerms.ResumeLayout(false);
            this.pnlSelectTerms.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button btnAddToSelected;
        private System.Windows.Forms.Button btnRemoveFromSelected;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlSelectTerms;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.Button btnMoveAssnPrevious;
        private System.Windows.Forms.Button btnMoveAssnNext;
        private System.Windows.Forms.Button btnDeleteAssociation;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnEditEntryName;
        private System.Windows.Forms.Button btnDeleteSearchEntry;
    }
}