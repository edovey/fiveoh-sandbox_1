namespace BDEditor.Views
{
    partial class BDTherapyControl
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
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbDosage = new System.Windows.Forms.TextBox();
            this.tbDuration = new System.Windows.Forms.TextBox();
            this.lblTherapy = new System.Windows.Forms.Label();
            this.lblDosage = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.btnTherapyLink = new System.Windows.Forms.Button();
            this.bthDosageLink = new System.Windows.Forms.Button();
            this.btnDurationLink = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(17, 27);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(218, 20);
            this.tbName.TabIndex = 0;
            // 
            // tbDosage
            // 
            this.tbDosage.Location = new System.Drawing.Point(319, 25);
            this.tbDosage.Name = "tbDosage";
            this.tbDosage.Size = new System.Drawing.Size(211, 20);
            this.tbDosage.TabIndex = 1;
            // 
            // tbDuration
            // 
            this.tbDuration.Location = new System.Drawing.Point(606, 25);
            this.tbDuration.Name = "tbDuration";
            this.tbDuration.Size = new System.Drawing.Size(174, 20);
            this.tbDuration.TabIndex = 2;
            // 
            // lblTherapy
            // 
            this.lblTherapy.AutoSize = true;
            this.lblTherapy.Location = new System.Drawing.Point(17, 8);
            this.lblTherapy.Name = "lblTherapy";
            this.lblTherapy.Size = new System.Drawing.Size(46, 13);
            this.lblTherapy.TabIndex = 3;
            this.lblTherapy.Text = "Therapy";
            // 
            // lblDosage
            // 
            this.lblDosage.AutoSize = true;
            this.lblDosage.Location = new System.Drawing.Point(316, 8);
            this.lblDosage.Name = "lblDosage";
            this.lblDosage.Size = new System.Drawing.Size(44, 13);
            this.lblDosage.TabIndex = 4;
            this.lblDosage.Text = "Dosage";
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(603, 8);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(47, 13);
            this.lblDuration.TabIndex = 5;
            this.lblDuration.Text = "Duration";
            // 
            // btnTherapyLink
            // 
            this.btnTherapyLink.Location = new System.Drawing.Point(251, 22);
            this.btnTherapyLink.Name = "btnTherapyLink";
            this.btnTherapyLink.Size = new System.Drawing.Size(35, 23);
            this.btnTherapyLink.TabIndex = 6;
            this.btnTherapyLink.Text = "Link";
            this.btnTherapyLink.UseVisualStyleBackColor = true;
            this.btnTherapyLink.Click += new System.EventHandler(this.btnTherapyLink_Click);
            // 
            // bthDosageLink
            // 
            this.bthDosageLink.Location = new System.Drawing.Point(547, 23);
            this.bthDosageLink.Name = "bthDosageLink";
            this.bthDosageLink.Size = new System.Drawing.Size(35, 23);
            this.bthDosageLink.TabIndex = 7;
            this.bthDosageLink.Text = "Link";
            this.bthDosageLink.UseVisualStyleBackColor = true;
            // 
            // btnDurationLink
            // 
            this.btnDurationLink.Location = new System.Drawing.Point(786, 23);
            this.btnDurationLink.Name = "btnDurationLink";
            this.btnDurationLink.Size = new System.Drawing.Size(35, 23);
            this.btnDurationLink.TabIndex = 8;
            this.btnDurationLink.Text = "Link";
            this.btnDurationLink.UseVisualStyleBackColor = true;
            // 
            // BDTherapyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDurationLink);
            this.Controls.Add(this.bthDosageLink);
            this.Controls.Add(this.btnTherapyLink);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.lblDosage);
            this.Controls.Add(this.lblTherapy);
            this.Controls.Add(this.tbDuration);
            this.Controls.Add(this.tbDosage);
            this.Controls.Add(this.tbName);
            this.Name = "BDTherapyControl";
            this.Size = new System.Drawing.Size(855, 49);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbDosage;
        private System.Windows.Forms.TextBox tbDuration;
        private System.Windows.Forms.Label lblTherapy;
        private System.Windows.Forms.Label lblDosage;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Button btnTherapyLink;
        private System.Windows.Forms.Button bthDosageLink;
        private System.Windows.Forms.Button btnDurationLink;
    }
}
