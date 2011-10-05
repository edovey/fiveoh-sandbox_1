namespace BDEditor.Views
{
    partial class BDDiseaseControl
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
            this.lblDisease = new System.Windows.Forms.Label();
            this.tbDiseaseName = new System.Windows.Forms.TextBox();
            this.tbDiseaseOverview = new System.Windows.Forms.TextBox();
            this.lblOverview = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblDisease
            // 
            this.lblDisease.AutoSize = true;
            this.lblDisease.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisease.Location = new System.Drawing.Point(12, 14);
            this.lblDisease.Name = "lblDisease";
            this.lblDisease.Size = new System.Drawing.Size(62, 18);
            this.lblDisease.TabIndex = 0;
            this.lblDisease.Text = "Disease";
            // 
            // tbDiseaseName
            // 
            this.tbDiseaseName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDiseaseName.Location = new System.Drawing.Point(15, 41);
            this.tbDiseaseName.Name = "tbDiseaseName";
            this.tbDiseaseName.ReadOnly = true;
            this.tbDiseaseName.Size = new System.Drawing.Size(475, 24);
            this.tbDiseaseName.TabIndex = 1;
            this.tbDiseaseName.TextChanged += new System.EventHandler(this.tbDiseaseName_TextChanged);
            // 
            // tbDiseaseOverview
            // 
            this.tbDiseaseOverview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDiseaseOverview.Location = new System.Drawing.Point(15, 96);
            this.tbDiseaseOverview.Multiline = true;
            this.tbDiseaseOverview.Name = "tbDiseaseOverview";
            this.tbDiseaseOverview.Size = new System.Drawing.Size(727, 104);
            this.tbDiseaseOverview.TabIndex = 2;
            // 
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOverview.Location = new System.Drawing.Point(16, 77);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(64, 16);
            this.lblOverview.TabIndex = 3;
            this.lblOverview.Text = "Overview";
            // 
            // BDDiseaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblOverview);
            this.Controls.Add(this.tbDiseaseOverview);
            this.Controls.Add(this.tbDiseaseName);
            this.Controls.Add(this.lblDisease);
            this.Name = "BDDiseaseControl";
            this.Size = new System.Drawing.Size(752, 220);
            this.Load += new System.EventHandler(this.BDDiseaseControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDisease;
        private System.Windows.Forms.TextBox tbDiseaseName;
        private System.Windows.Forms.TextBox tbDiseaseOverview;
        private System.Windows.Forms.Label lblOverview;
    }
}
