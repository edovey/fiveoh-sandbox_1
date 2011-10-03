namespace BDEditor.Views
{
    partial class BDPresentationControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbPresentationName = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.lblOverview = new System.Windows.Forms.Label();
            this.bdPathogenGroupControl1 = new BDEditor.Views.BDPathogenGroupControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Presentation";
            // 
            // tbPresentationName
            // 
            this.tbPresentationName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPresentationName.Location = new System.Drawing.Point(6, 33);
            this.tbPresentationName.Name = "tbPresentationName";
            this.tbPresentationName.Size = new System.Drawing.Size(475, 24);
            this.tbPresentationName.TabIndex = 2;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(3, 90);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(974, 93);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Location = new System.Drawing.Point(3, 74);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(52, 13);
            this.lblOverview.TabIndex = 4;
            this.lblOverview.Text = "Overview";
            // 
            // bdPathogenGroupControl1
            // 
            this.bdPathogenGroupControl1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.bdPathogenGroupControl1.CurrentPathogenGroup = null;
            this.bdPathogenGroupControl1.Location = new System.Drawing.Point(0, 189);
            this.bdPathogenGroupControl1.Name = "bdPathogenGroupControl1";
            this.bdPathogenGroupControl1.Size = new System.Drawing.Size(1164, 798);
            this.bdPathogenGroupControl1.TabIndex = 5;
            // 
            // BDPresentationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bdPathogenGroupControl1);
            this.Controls.Add(this.lblOverview);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.tbPresentationName);
            this.Controls.Add(this.label1);
            this.Name = "BDPresentationControl";
            this.Size = new System.Drawing.Size(1171, 938);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPresentationName;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label lblOverview;
        private BDPathogenGroupControl bdPathogenGroupControl1;
    }
}
