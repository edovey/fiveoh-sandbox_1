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
            this.rtbPresentationOverview = new System.Windows.Forms.RichTextBox();
            this.lblOverview = new System.Windows.Forms.Label();
            this.bdPathogenGroupControl1 = new BDEditor.Views.BDPathogenGroupControl();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.btnFont = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Presentation";
            // 
            // tbPresentationName
            // 
            this.tbPresentationName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPresentationName.Location = new System.Drawing.Point(6, 26);
            this.tbPresentationName.Name = "tbPresentationName";
            this.tbPresentationName.Size = new System.Drawing.Size(475, 20);
            this.tbPresentationName.TabIndex = 2;
            // 
            // rtbPresentationOverview
            // 
            this.rtbPresentationOverview.Location = new System.Drawing.Point(6, 68);
            this.rtbPresentationOverview.Name = "rtbPresentationOverview";
            this.rtbPresentationOverview.Size = new System.Drawing.Size(974, 93);
            this.rtbPresentationOverview.TabIndex = 3;
            this.rtbPresentationOverview.Text = "";
            // 
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Location = new System.Drawing.Point(6, 52);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(52, 13);
            this.lblOverview.TabIndex = 4;
            this.lblOverview.Text = "Overview";
            // 
            // bdPathogenGroupControl1
            // 
            this.bdPathogenGroupControl1.AutoScroll = true;
            this.bdPathogenGroupControl1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.bdPathogenGroupControl1.CurrentPathogenGroup = null;
            this.bdPathogenGroupControl1.Location = new System.Drawing.Point(0, 165);
            this.bdPathogenGroupControl1.Name = "bdPathogenGroupControl1";
            this.bdPathogenGroupControl1.Size = new System.Drawing.Size(1164, 798);
            this.bdPathogenGroupControl1.TabIndex = 5;
            // 
            // btnFont
            // 
            this.btnFont.Location = new System.Drawing.Point(987, 68);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(75, 23);
            this.btnFont.TabIndex = 6;
            this.btnFont.Text = "Set Font";
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // BDPresentationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.btnFont);
            this.Controls.Add(this.bdPathogenGroupControl1);
            this.Controls.Add(this.lblOverview);
            this.Controls.Add(this.rtbPresentationOverview);
            this.Controls.Add(this.tbPresentationName);
            this.Controls.Add(this.label1);
            this.Name = "BDPresentationControl";
            this.Size = new System.Drawing.Size(1103, 870);
            this.Leave += new System.EventHandler(this.BDPresentationControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPresentationName;
        private System.Windows.Forms.RichTextBox rtbPresentationOverview;
        private System.Windows.Forms.Label lblOverview;
        private BDPathogenGroupControl bdPathogenGroupControl1;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button btnFont;
    }
}
