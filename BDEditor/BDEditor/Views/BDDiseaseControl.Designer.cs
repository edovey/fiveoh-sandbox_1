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
            this.tcPresentation = new System.Windows.Forms.TabControl();
            this.tpPresentation1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.tcPresentation.SuspendLayout();
            this.tpPresentation1.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.tbDiseaseOverview.Size = new System.Drawing.Size(1067, 104);
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
            // tcPresentation
            // 
            this.tcPresentation.Controls.Add(this.tpPresentation1);
            this.tcPresentation.Controls.Add(this.tabPage2);
            this.tcPresentation.Location = new System.Drawing.Point(15, 217);
            this.tcPresentation.Name = "tcPresentation";
            this.tcPresentation.SelectedIndex = 0;
            this.tcPresentation.Size = new System.Drawing.Size(1067, 269);
            this.tcPresentation.TabIndex = 4;
            // 
            // tpPresentation1
            // 
            this.tpPresentation1.Controls.Add(this.label1);
            this.tpPresentation1.Location = new System.Drawing.Point(4, 22);
            this.tpPresentation1.Name = "tpPresentation1";
            this.tpPresentation1.Padding = new System.Windows.Forms.Padding(3);
            this.tpPresentation1.Size = new System.Drawing.Size(1059, 243);
            this.tpPresentation1.TabIndex = 0;
            this.tpPresentation1.Text = "Presentation";
            this.tpPresentation1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1059, 243);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "additional presentations..";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "presentation control goes here";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(19, 544);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1059, 100);
            this.panel1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "linked notes control goes here";
            // 
            // BDDiseaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tcPresentation);
            this.Controls.Add(this.lblOverview);
            this.Controls.Add(this.tbDiseaseOverview);
            this.Controls.Add(this.tbDiseaseName);
            this.Controls.Add(this.lblDisease);
            this.Name = "BDDiseaseControl";
            this.Size = new System.Drawing.Size(1126, 681);
            this.Load += new System.EventHandler(this.BDDiseaseControl_Load);
            this.tcPresentation.ResumeLayout(false);
            this.tpPresentation1.ResumeLayout(false);
            this.tpPresentation1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDisease;
        private System.Windows.Forms.TextBox tbDiseaseName;
        private System.Windows.Forms.TextBox tbDiseaseOverview;
        private System.Windows.Forms.Label lblOverview;
        private System.Windows.Forms.TabControl tcPresentation;
        private System.Windows.Forms.TabPage tpPresentation1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
    }
}
