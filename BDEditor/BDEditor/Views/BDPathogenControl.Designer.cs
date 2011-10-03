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
            this.lblTitle = new System.Windows.Forms.Label();
            this.tbPathogenName = new System.Windows.Forms.TextBox();
            this.btnLink = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(2, 3);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(53, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Pathogen";
            // 
            // tbPathogenName
            // 
            this.tbPathogenName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPathogenName.Location = new System.Drawing.Point(5, 17);
            this.tbPathogenName.Name = "tbPathogenName";
            this.tbPathogenName.Size = new System.Drawing.Size(239, 20);
            this.tbPathogenName.TabIndex = 1;
            this.tbPathogenName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // btnLink
            // 
            this.btnLink.Location = new System.Drawing.Point(246, 17);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(37, 23);
            this.btnLink.TabIndex = 2;
            this.btnLink.Text = "Link";
            this.btnLink.UseVisualStyleBackColor = true;
            this.btnLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // BDPathogenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnLink);
            this.Controls.Add(this.tbPathogenName);
            this.Controls.Add(this.lblTitle);
            this.Name = "BDPathogenControl";
            this.Size = new System.Drawing.Size(285, 44);
            this.Leave += new System.EventHandler(this.BDPathogenControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox tbPathogenName;
        private System.Windows.Forms.Button btnLink;
    }
}
