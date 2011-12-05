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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDisease
            // 
            this.lblDisease.AutoSize = true;
            this.lblDisease.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisease.Location = new System.Drawing.Point(3, 6);
            this.lblDisease.Name = "lblDisease";
            this.lblDisease.Size = new System.Drawing.Size(77, 24);
            this.lblDisease.TabIndex = 0;
            this.lblDisease.Text = "Disease";
            // 
            // tbDiseaseName
            // 
            this.tbDiseaseName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDiseaseName.Location = new System.Drawing.Point(39, 31);
            this.tbDiseaseName.Name = "tbDiseaseName";
            this.tbDiseaseName.ReadOnly = true;
            this.tbDiseaseName.Size = new System.Drawing.Size(475, 24);
            this.tbDiseaseName.TabIndex = 1;
            this.tbDiseaseName.TextChanged += new System.EventHandler(this.tbDiseaseName_TextChanged);
            // 
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOverview.Location = new System.Drawing.Point(39, 56);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(80, 20);
            this.lblOverview.TabIndex = 3;
            this.lblOverview.Text = "Overview";
            // 
            // bdLinkedNoteControl1
            // 
            this.bdLinkedNoteControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bdLinkedNoteControl1.CurrentLinkedNote = null;
            this.bdLinkedNoteControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.bdLinkedNoteControl1.Location = new System.Drawing.Point(0, 78);
            this.bdLinkedNoteControl1.Name = "bdLinkedNoteControl1";
            this.bdLinkedNoteControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdLinkedNoteControl1.SaveOnLeave = true;
            this.bdLinkedNoteControl1.SelectedLinkedNoteType = BDEditor.DataModel.LinkedNoteType.Inline;
            this.bdLinkedNoteControl1.Size = new System.Drawing.Size(826, 235);
            this.bdLinkedNoteControl1.TabIndex = 4;
            this.bdLinkedNoteControl1.SaveAttemptWithoutParent += new System.EventHandler(this.bdLinkedNoteControl_SaveAttemptWithoutParent);
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.lblOverview);
            this.panelHeader.Controls.Add(this.tbDiseaseName);
            this.panelHeader.Controls.Add(this.lblDisease);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(826, 78);
            this.panelHeader.TabIndex = 5;
            // 
            // BDDiseaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bdLinkedNoteControl1);
            this.Controls.Add(this.panelHeader);
            this.Name = "BDDiseaseControl";
            this.Size = new System.Drawing.Size(826, 357);
            this.Load += new System.EventHandler(this.BDDiseaseControl_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDisease;
        private System.Windows.Forms.TextBox tbDiseaseName;
        private System.Windows.Forms.Label lblOverview;
        private BDLinkedNoteControl bdLinkedNoteControl1;
        private System.Windows.Forms.Panel panelHeader;
    }
}
