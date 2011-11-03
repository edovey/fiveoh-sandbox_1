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
            this.lblOverview = new System.Windows.Forms.Label();
            this.bdLinkedNoteControl1 = new BDEditor.Views.BDLinkedNoteControl();
            this.SuspendLayout();
            // 
            // lblDisease
            // 
            this.lblDisease.AutoSize = true;
            this.lblDisease.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisease.Location = new System.Drawing.Point(12, 14);
            this.lblDisease.Name = "lblDisease";
            this.lblDisease.Size = new System.Drawing.Size(77, 24);
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
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOverview.Location = new System.Drawing.Point(16, 73);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(89, 24);
            this.lblOverview.TabIndex = 3;
            this.lblOverview.Text = "Overview";
            // 
            // bdLinkedNoteControl1
            // 
            this.bdLinkedNoteControl1.CurrentLinkedNote = null;
            this.bdLinkedNoteControl1.Location = new System.Drawing.Point(15, 97);
            this.bdLinkedNoteControl1.Name = "bdLinkedNoteControl1";
            this.bdLinkedNoteControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdLinkedNoteControl1.SaveOnLeave = true;
            this.bdLinkedNoteControl1.SelectedLinkedNoteType = BDEditor.DataModel.LinkedNoteType.Default;
            this.bdLinkedNoteControl1.Size = new System.Drawing.Size(807, 258);
            this.bdLinkedNoteControl1.TabIndex = 4;
            // 
            // BDDiseaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bdLinkedNoteControl1);
            this.Controls.Add(this.lblOverview);
            this.Controls.Add(this.tbDiseaseName);
            this.Controls.Add(this.lblDisease);
            this.Name = "BDDiseaseControl";
            this.Size = new System.Drawing.Size(826, 357);
            this.Load += new System.EventHandler(this.BDDiseaseControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDisease;
        private System.Windows.Forms.TextBox tbDiseaseName;
        private System.Windows.Forms.Label lblOverview;
        private BDLinkedNoteControl bdLinkedNoteControl1;
    }
}
