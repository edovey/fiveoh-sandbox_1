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
            this.lblPathogenName = new System.Windows.Forms.Label();
            this.tbPathogenName = new System.Windows.Forms.TextBox();
            this.btnLink = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblPathogenName
            // 
            this.lblPathogenName.AutoSize = true;
            this.lblPathogenName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPathogenName.Location = new System.Drawing.Point(4, 4);
            this.lblPathogenName.Name = "lblPathogenName";
            this.lblPathogenName.Size = new System.Drawing.Size(71, 18);
            this.lblPathogenName.TabIndex = 0;
            this.lblPathogenName.Text = "Pathogen";
            // 
            // tbPathogenName
            // 
            this.tbPathogenName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPathogenName.Location = new System.Drawing.Point(7, 26);
            this.tbPathogenName.Name = "tbPathogenName";
            this.tbPathogenName.Size = new System.Drawing.Size(474, 24);
            this.tbPathogenName.TabIndex = 1;
            // 
            // btnLink
            // 
            this.btnLink.Location = new System.Drawing.Point(488, 26);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(37, 23);
            this.btnLink.TabIndex = 2;
            this.btnLink.Text = "Link";
            this.btnLink.UseVisualStyleBackColor = true;
            // 
            // BDPathogenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnLink);
            this.Controls.Add(this.tbPathogenName);
            this.Controls.Add(this.lblPathogenName);
            this.Name = "BDPathogenControl";
            this.Size = new System.Drawing.Size(1013, 55);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPathogenName;
        private System.Windows.Forms.TextBox tbPathogenName;
        private System.Windows.Forms.Button btnLink;
    }
}
