namespace BDEditor.Views
{
    partial class BDPathogenControl
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
            this.tbPathogenName = new System.Windows.Forms.TextBox();
            this.btnLink = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnReorderToPrevious = new System.Windows.Forms.Button();
            this.btnReorderToNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbPathogenName
            // 
            this.tbPathogenName.AutoCompleteCustomSource.AddRange(new string[] {
            "Streptococcus aureus\t",
            "S. aureus",
            "Pathogen B"});
            this.tbPathogenName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbPathogenName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbPathogenName.Dock = System.Windows.Forms.DockStyle.Left;
            this.tbPathogenName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPathogenName.Location = new System.Drawing.Point(0, 0);
            this.tbPathogenName.Name = "tbPathogenName";
            this.tbPathogenName.Size = new System.Drawing.Size(239, 20);
            this.tbPathogenName.TabIndex = 1;
            this.tbPathogenName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // btnLink
            // 
            this.btnLink.Enabled = false;
            this.btnLink.Image = global::BDEditor.Properties.Resources.link_16;
            this.btnLink.Location = new System.Drawing.Point(245, -1);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(28, 28);
            this.btnLink.TabIndex = 2;
            this.btnLink.UseVisualStyleBackColor = true;
            this.btnLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = global::BDEditor.Properties.Resources.remove;
            this.btnDelete.Location = new System.Drawing.Point(279, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(28, 28);
            this.btnDelete.TabIndex = 14;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnReorderToPrevious
            // 
            this.btnReorderToPrevious.Image = global::BDEditor.Properties.Resources.reorder_previous;
            this.btnReorderToPrevious.Location = new System.Drawing.Point(313, 0);
            this.btnReorderToPrevious.Name = "btnReorderToPrevious";
            this.btnReorderToPrevious.Size = new System.Drawing.Size(28, 28);
            this.btnReorderToPrevious.TabIndex = 15;
            this.btnReorderToPrevious.UseVisualStyleBackColor = true;
            this.btnReorderToPrevious.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // btnReorderToNext
            // 
            this.btnReorderToNext.Image = global::BDEditor.Properties.Resources.reorder_next;
            this.btnReorderToNext.Location = new System.Drawing.Point(347, -1);
            this.btnReorderToNext.Name = "btnReorderToNext";
            this.btnReorderToNext.Size = new System.Drawing.Size(28, 28);
            this.btnReorderToNext.TabIndex = 16;
            this.btnReorderToNext.UseVisualStyleBackColor = true;
            this.btnReorderToNext.Click += new System.EventHandler(this.btnReorderToNext_Click);
            // 
            // BDPathogenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.btnReorderToNext);
            this.Controls.Add(this.btnReorderToPrevious);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnLink);
            this.Controls.Add(this.tbPathogenName);
            this.Name = "BDPathogenControl";
            this.Size = new System.Drawing.Size(378, 31);
            this.Leave += new System.EventHandler(this.BDPathogenControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPathogenName;
        private System.Windows.Forms.Button btnLink;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnReorderToPrevious;
        private System.Windows.Forms.Button btnReorderToNext;
    }
}
