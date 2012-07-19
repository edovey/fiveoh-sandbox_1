namespace BDEditor.Views
{
    partial class BDLayoutMetadataEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDLayoutMetadataEditor));
            this.listBoxLayoutVariants = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxLayoutColumns = new System.Windows.Forms.ListBox();
            this.lblSelectedLayout = new System.Windows.Forms.Label();
            this.chkLayoutIncluded = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtColumnLabel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLinkedNote = new System.Windows.Forms.Button();
            this.btnMoveColumnPrevious = new System.Windows.Forms.Button();
            this.btnMoveColumnNext = new System.Windows.Forms.Button();
            this.btnAddColumn = new System.Windows.Forms.Button();
            this.btnRemoveColumn = new System.Windows.Forms.Button();
            this.listBoxColumnNodeTypes = new System.Windows.Forms.ListBox();
            this.btnColumnNoteTypeSetup = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxLayoutVariants
            // 
            this.listBoxLayoutVariants.FormattingEnabled = true;
            this.listBoxLayoutVariants.Location = new System.Drawing.Point(12, 27);
            this.listBoxLayoutVariants.Name = "listBoxLayoutVariants";
            this.listBoxLayoutVariants.Size = new System.Drawing.Size(316, 420);
            this.listBoxLayoutVariants.TabIndex = 0;
            this.listBoxLayoutVariants.SelectedIndexChanged += new System.EventHandler(this.listBoxLayoutVariants_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Layout Variants";
            // 
            // listBoxLayoutColumns
            // 
            this.listBoxLayoutColumns.FormattingEnabled = true;
            this.listBoxLayoutColumns.Location = new System.Drawing.Point(347, 101);
            this.listBoxLayoutColumns.Name = "listBoxLayoutColumns";
            this.listBoxLayoutColumns.Size = new System.Drawing.Size(319, 134);
            this.listBoxLayoutColumns.TabIndex = 3;
            this.listBoxLayoutColumns.SelectedIndexChanged += new System.EventHandler(this.listBoxLayoutColumns_SelectedIndexChanged);
            // 
            // lblSelectedLayout
            // 
            this.lblSelectedLayout.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSelectedLayout.Location = new System.Drawing.Point(347, 28);
            this.lblSelectedLayout.Name = "lblSelectedLayout";
            this.lblSelectedLayout.Size = new System.Drawing.Size(319, 24);
            this.lblSelectedLayout.TabIndex = 4;
            this.lblSelectedLayout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkLayoutIncluded
            // 
            this.chkLayoutIncluded.AutoSize = true;
            this.chkLayoutIncluded.Location = new System.Drawing.Point(347, 56);
            this.chkLayoutIncluded.Name = "chkLayoutIncluded";
            this.chkLayoutIncluded.Size = new System.Drawing.Size(67, 17);
            this.chkLayoutIncluded.TabIndex = 5;
            this.chkLayoutIncluded.Text = "Included";
            this.chkLayoutIncluded.UseVisualStyleBackColor = true;
            this.chkLayoutIncluded.CheckedChanged += new System.EventHandler(this.chkLayoutIncluded_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(344, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Columns";
            // 
            // txtColumnLabel
            // 
            this.txtColumnLabel.Location = new System.Drawing.Point(347, 298);
            this.txtColumnLabel.Name = "txtColumnLabel";
            this.txtColumnLabel.Size = new System.Drawing.Size(319, 20);
            this.txtColumnLabel.TabIndex = 7;
            this.txtColumnLabel.Leave += new System.EventHandler(this.txtColumnLabel_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(344, 282);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Column Label";
            // 
            // btnLinkedNote
            // 
            this.btnLinkedNote.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNote.Image")));
            this.btnLinkedNote.Location = new System.Drawing.Point(671, 293);
            this.btnLinkedNote.Name = "btnLinkedNote";
            this.btnLinkedNote.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNote.TabIndex = 30;
            this.btnLinkedNote.UseVisualStyleBackColor = true;
            this.btnLinkedNote.Click += new System.EventHandler(this.btnLinkedNote_Click);
            // 
            // btnMoveColumnPrevious
            // 
            this.btnMoveColumnPrevious.Image = global::BDEditor.Properties.Resources.previous_16;
            this.btnMoveColumnPrevious.Location = new System.Drawing.Point(347, 241);
            this.btnMoveColumnPrevious.Name = "btnMoveColumnPrevious";
            this.btnMoveColumnPrevious.Size = new System.Drawing.Size(24, 24);
            this.btnMoveColumnPrevious.TabIndex = 31;
            this.btnMoveColumnPrevious.UseVisualStyleBackColor = true;
            this.btnMoveColumnPrevious.Click += new System.EventHandler(this.btnMoveColumnPrevious_Click);
            // 
            // btnMoveColumnNext
            // 
            this.btnMoveColumnNext.Image = global::BDEditor.Properties.Resources.next_16;
            this.btnMoveColumnNext.Location = new System.Drawing.Point(377, 241);
            this.btnMoveColumnNext.Name = "btnMoveColumnNext";
            this.btnMoveColumnNext.Size = new System.Drawing.Size(24, 24);
            this.btnMoveColumnNext.TabIndex = 32;
            this.btnMoveColumnNext.UseVisualStyleBackColor = true;
            this.btnMoveColumnNext.Click += new System.EventHandler(this.btnMoveColumnNext_Click);
            // 
            // btnAddColumn
            // 
            this.btnAddColumn.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.btnAddColumn.Location = new System.Drawing.Point(642, 241);
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Size = new System.Drawing.Size(24, 24);
            this.btnAddColumn.TabIndex = 33;
            this.btnAddColumn.UseVisualStyleBackColor = true;
            this.btnAddColumn.Click += new System.EventHandler(this.btnAddColumn_Click);
            // 
            // btnRemoveColumn
            // 
            this.btnRemoveColumn.Image = global::BDEditor.Properties.Resources.del_16x16;
            this.btnRemoveColumn.Location = new System.Drawing.Point(612, 241);
            this.btnRemoveColumn.Name = "btnRemoveColumn";
            this.btnRemoveColumn.Size = new System.Drawing.Size(24, 24);
            this.btnRemoveColumn.TabIndex = 34;
            this.btnRemoveColumn.UseVisualStyleBackColor = true;
            this.btnRemoveColumn.Click += new System.EventHandler(this.btnRemoveColumn_Click);
            // 
            // listBoxColumnNodeTypes
            // 
            this.listBoxColumnNodeTypes.FormattingEnabled = true;
            this.listBoxColumnNodeTypes.Location = new System.Drawing.Point(347, 347);
            this.listBoxColumnNodeTypes.Name = "listBoxColumnNodeTypes";
            this.listBoxColumnNodeTypes.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxColumnNodeTypes.Size = new System.Drawing.Size(319, 69);
            this.listBoxColumnNodeTypes.TabIndex = 35;
            // 
            // btnColumnNoteTypeSetup
            // 
            this.btnColumnNoteTypeSetup.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnColumnNoteTypeSetup.Location = new System.Drawing.Point(347, 423);
            this.btnColumnNoteTypeSetup.Name = "btnColumnNoteTypeSetup";
            this.btnColumnNoteTypeSetup.Size = new System.Drawing.Size(24, 24);
            this.btnColumnNoteTypeSetup.TabIndex = 36;
            this.btnColumnNoteTypeSetup.UseVisualStyleBackColor = true;
            this.btnColumnNoteTypeSetup.Click += new System.EventHandler(this.btnColumnNoteTypeSetup_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(344, 331);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "NodeType Info";
            // 
            // BDLayoutMetadataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 465);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnColumnNoteTypeSetup);
            this.Controls.Add(this.btnLinkedNote);
            this.Controls.Add(this.listBoxColumnNodeTypes);
            this.Controls.Add(this.txtColumnLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnMoveColumnNext);
            this.Controls.Add(this.btnMoveColumnPrevious);
            this.Controls.Add(this.btnRemoveColumn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAddColumn);
            this.Controls.Add(this.chkLayoutIncluded);
            this.Controls.Add(this.lblSelectedLayout);
            this.Controls.Add(this.listBoxLayoutColumns);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxLayoutVariants);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BDLayoutMetadataEditor";
            this.Text = "Layout Virtual Column Editor";
            this.Load += new System.EventHandler(this.BDLayoutMetadataEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxLayoutVariants;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxLayoutColumns;
        private System.Windows.Forms.Label lblSelectedLayout;
        private System.Windows.Forms.CheckBox chkLayoutIncluded;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtColumnLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnLinkedNote;
        private System.Windows.Forms.Button btnMoveColumnNext;
        private System.Windows.Forms.Button btnMoveColumnPrevious;
        private System.Windows.Forms.Button btnRemoveColumn;
        private System.Windows.Forms.Button btnAddColumn;
        private System.Windows.Forms.ListBox listBoxColumnNodeTypes;
        private System.Windows.Forms.Button btnColumnNoteTypeSetup;
        private System.Windows.Forms.Label label4;
    }
}