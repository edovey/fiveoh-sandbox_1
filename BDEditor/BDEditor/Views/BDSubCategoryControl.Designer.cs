namespace BDEditor.DataModel
{
    partial class BDSubCategoryControl
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
            this.lblSubcategoryName = new System.Windows.Forms.Label();
            this.tbSubcategoryName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblSubcategoryName
            // 
            this.lblSubcategoryName.AutoSize = true;
            this.lblSubcategoryName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubcategoryName.Location = new System.Drawing.Point(14, 20);
            this.lblSubcategoryName.Name = "lblSubcategoryName";
            this.lblSubcategoryName.Size = new System.Drawing.Size(91, 18);
            this.lblSubcategoryName.TabIndex = 0;
            this.lblSubcategoryName.Text = "Subcategory";
            // 
            // tbSubcategoryName
            // 
            this.tbSubcategoryName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSubcategoryName.Location = new System.Drawing.Point(17, 51);
            this.tbSubcategoryName.Name = "tbSubcategoryName";
            this.tbSubcategoryName.ReadOnly = true;
            this.tbSubcategoryName.Size = new System.Drawing.Size(475, 24);
            this.tbSubcategoryName.TabIndex = 1;
            // 
            // BDSubCategoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbSubcategoryName);
            this.Controls.Add(this.lblSubcategoryName);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "BDSubCategoryControl";
            this.Size = new System.Drawing.Size(568, 156);
            this.Load += new System.EventHandler(this.BDSubCategoryControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSubcategoryName;
        private System.Windows.Forms.TextBox tbSubcategoryName;
    }
}
