namespace BDEditor.Views
{
    partial class BDLayoutColumnMetadataEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDLayoutColumnMetadataEditor));
            this.txtColumnLabel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSelectedLayout = new System.Windows.Forms.Label();
            this.listBoxLayoutColumns = new System.Windows.Forms.ListBox();
            this.btnLinkedNote = new System.Windows.Forms.Button();
            this.btnMoveColumnNext = new System.Windows.Forms.Button();
            this.btnMoveColumnPrevious = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sTwoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trademarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtColumnLabel
            // 
            this.txtColumnLabel.ContextMenuStrip = this.contextMenuStrip1;
            this.txtColumnLabel.Location = new System.Drawing.Point(12, 279);
            this.txtColumnLabel.Name = "txtColumnLabel";
            this.txtColumnLabel.Size = new System.Drawing.Size(371, 20);
            this.txtColumnLabel.TabIndex = 42;
            this.txtColumnLabel.Leave += new System.EventHandler(this.txtColumnLabel_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 263);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 43;
            this.label3.Text = "Column Title";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "Columns";
            // 
            // lblSelectedLayout
            // 
            this.lblSelectedLayout.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSelectedLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedLayout.Location = new System.Drawing.Point(12, 9);
            this.lblSelectedLayout.Name = "lblSelectedLayout";
            this.lblSelectedLayout.Size = new System.Drawing.Size(405, 24);
            this.lblSelectedLayout.TabIndex = 39;
            this.lblSelectedLayout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listBoxLayoutColumns
            // 
            this.listBoxLayoutColumns.FormattingEnabled = true;
            this.listBoxLayoutColumns.Location = new System.Drawing.Point(12, 82);
            this.listBoxLayoutColumns.Name = "listBoxLayoutColumns";
            this.listBoxLayoutColumns.Size = new System.Drawing.Size(405, 134);
            this.listBoxLayoutColumns.TabIndex = 38;
            this.listBoxLayoutColumns.SelectedIndexChanged += new System.EventHandler(this.listBoxLayoutColumns_SelectedIndexChanged);
            // 
            // btnLinkedNote
            // 
            this.btnLinkedNote.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNote.Image")));
            this.btnLinkedNote.Location = new System.Drawing.Point(389, 274);
            this.btnLinkedNote.Name = "btnLinkedNote";
            this.btnLinkedNote.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNote.TabIndex = 44;
            this.btnLinkedNote.UseVisualStyleBackColor = true;
            this.btnLinkedNote.Click += new System.EventHandler(this.btnLinkedNote_Click);
            // 
            // btnMoveColumnNext
            // 
            this.btnMoveColumnNext.Image = global::BDEditor.Properties.Resources.next_16;
            this.btnMoveColumnNext.Location = new System.Drawing.Point(42, 222);
            this.btnMoveColumnNext.Name = "btnMoveColumnNext";
            this.btnMoveColumnNext.Size = new System.Drawing.Size(24, 24);
            this.btnMoveColumnNext.TabIndex = 46;
            this.btnMoveColumnNext.UseVisualStyleBackColor = true;
            this.btnMoveColumnNext.Click += new System.EventHandler(this.btnMoveColumnNext_Click);
            // 
            // btnMoveColumnPrevious
            // 
            this.btnMoveColumnPrevious.Image = global::BDEditor.Properties.Resources.previous_16;
            this.btnMoveColumnPrevious.Location = new System.Drawing.Point(12, 222);
            this.btnMoveColumnPrevious.Name = "btnMoveColumnPrevious";
            this.btnMoveColumnPrevious.Size = new System.Drawing.Size(24, 24);
            this.btnMoveColumnPrevious.TabIndex = 45;
            this.btnMoveColumnPrevious.UseVisualStyleBackColor = true;
            this.btnMoveColumnPrevious.Click += new System.EventHandler(this.btnMoveColumnPrevious_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(342, 316);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 47;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(261, 316);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 48;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.sTwoToolStripMenuItem,
            this.trademarkToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(128, 224);
            this.contextMenuStrip1.Text = "Insert Symbol";
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
            // sOneToolStripMenuItem
            // 
            this.sOneToolStripMenuItem.Name = "sOneToolStripMenuItem";
            this.sOneToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.sOneToolStripMenuItem.Text = "¹";
            this.sOneToolStripMenuItem.ToolTipText = "Insert ¹";
            this.sOneToolStripMenuItem.Click += new System.EventHandler(this.sOneToolStripMenuItem_Click);
            // 
            // sTwoToolStripMenuItem
            // 
            this.sTwoToolStripMenuItem.Name = "sTwoToolStripMenuItem";
            this.sTwoToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.sTwoToolStripMenuItem.Text = "²";
            this.sTwoToolStripMenuItem.ToolTipText = "Insert ²";
            this.sTwoToolStripMenuItem.Click += new System.EventHandler(this.sTwoToolStripMenuItem_Click);
            // 
            // trademarkToolStripMenuItem
            // 
            this.trademarkToolStripMenuItem.Name = "trademarkToolStripMenuItem";
            this.trademarkToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.trademarkToolStripMenuItem.Text = "®";
            this.trademarkToolStripMenuItem.ToolTipText = "Insert ®";
            this.trademarkToolStripMenuItem.DoubleClick += new System.EventHandler(this.trademarkToolStripMenuItem_Click);
            // 
            // BDLayoutColumnMetadataEditor
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(436, 351);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnLinkedNote);
            this.Controls.Add(this.txtColumnLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnMoveColumnNext);
            this.Controls.Add(this.btnMoveColumnPrevious);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblSelectedLayout);
            this.Controls.Add(this.listBoxLayoutColumns);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BDLayoutColumnMetadataEditor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "BDLayoutColumnMetadataEditor";
            this.Load += new System.EventHandler(this.BDLayoutColumnMetadataEditor_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLinkedNote;
        private System.Windows.Forms.TextBox txtColumnLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnMoveColumnNext;
        private System.Windows.Forms.Button btnMoveColumnPrevious;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSelectedLayout;
        private System.Windows.Forms.ListBox listBoxLayoutColumns;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem µToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sTwoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trademarkToolStripMenuItem;
    }
}