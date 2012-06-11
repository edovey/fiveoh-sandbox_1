namespace BDDataArchiver
{
    partial class BDDataArchiverView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnChooseFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblSource = new System.Windows.Forms.Label();
            this.lblOutput = new System.Windows.Forms.Label();
            this.btnArchive = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnChooseFile
            // 
            this.btnChooseFile.Location = new System.Drawing.Point(12, 12);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(72, 23);
            this.btnChooseFile.TabIndex = 0;
            this.btnChooseFile.Text = "Locate";
            this.btnChooseFile.UseVisualStyleBackColor = true;
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblSource
            // 
            this.lblSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSource.Location = new System.Drawing.Point(90, 12);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(484, 23);
            this.lblSource.TabIndex = 1;
            // 
            // lblOutput
            // 
            this.lblOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOutput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblOutput.Location = new System.Drawing.Point(90, 41);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(484, 23);
            this.lblOutput.TabIndex = 2;
            // 
            // btnArchive
            // 
            this.btnArchive.Enabled = false;
            this.btnArchive.Location = new System.Drawing.Point(12, 41);
            this.btnArchive.Name = "btnArchive";
            this.btnArchive.Size = new System.Drawing.Size(72, 23);
            this.btnArchive.TabIndex = 3;
            this.btnArchive.Text = "Archive";
            this.btnArchive.UseVisualStyleBackColor = true;
            this.btnArchive.Click += new System.EventHandler(this.btnArchive_Click);
            // 
            // BDDataArchiverView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 78);
            this.Controls.Add(this.btnArchive);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.btnChooseFile);
            this.MaximizeBox = false;
            this.Name = "BDDataArchiverView";
            this.Text = "BD Data Archiver";
            this.Load += new System.EventHandler(this.BDDataArchiverView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnChooseFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Button btnArchive;
    }
}

