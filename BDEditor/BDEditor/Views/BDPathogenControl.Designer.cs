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
            this.SuspendLayout();
            // 
            // tbPathogenName
            // 
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
            this.btnLink.Location = new System.Drawing.Point(245, -1);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(37, 20);
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
            this.Name = "BDPathogenControl";
            this.Size = new System.Drawing.Size(285, 21);
            this.Leave += new System.EventHandler(this.BDPathogenControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPathogenName;
        private System.Windows.Forms.Button btnLink;
    }
}
