namespace BDEditor.Views
{
    partial class AuthenticationInputBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthenticationInputBox));
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.lblLabel = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.flagRed = new System.Windows.Forms.PictureBox();
            this.flagGreen = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.flagRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.flagGreen)).BeginInit();
            this.SuspendLayout();
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(16, 34);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.PasswordChar = '*';
            this.maskedTextBox1.Size = new System.Drawing.Size(225, 20);
            this.maskedTextBox1.TabIndex = 1;
            this.maskedTextBox1.TextChanged += new System.EventHandler(this.maskedTextBox1_TextChanged);
            // 
            // lblLabel
            // 
            this.lblLabel.AutoSize = true;
            this.lblLabel.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLabel.Location = new System.Drawing.Point(13, 13);
            this.lblLabel.Name = "lblLabel";
            this.lblLabel.Size = new System.Drawing.Size(145, 18);
            this.lblLabel.TabIndex = 2;
            this.lblLabel.Text = "Authentication Phrase";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(136, 66);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(64, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(206, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // flagRed
            // 
            this.flagRed.Image = global::BDEditor.Properties.Resources.Flag_Red;
            this.flagRed.Location = new System.Drawing.Point(248, 34);
            this.flagRed.Name = "flagRed";
            this.flagRed.Size = new System.Drawing.Size(22, 26);
            this.flagRed.TabIndex = 5;
            this.flagRed.TabStop = false;
            // 
            // flagGreen
            // 
            this.flagGreen.Image = ((System.Drawing.Image)(resources.GetObject("flagGreen.Image")));
            this.flagGreen.Location = new System.Drawing.Point(248, 34);
            this.flagGreen.Name = "flagGreen";
            this.flagGreen.Size = new System.Drawing.Size(22, 26);
            this.flagGreen.TabIndex = 6;
            this.flagGreen.TabStop = false;
            this.flagGreen.Visible = false;
            // 
            // AuthenticationInputBox
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(287, 101);
            this.ControlBox = false;
            this.Controls.Add(this.flagGreen);
            this.Controls.Add(this.flagRed);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblLabel);
            this.Controls.Add(this.maskedTextBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AuthenticationInputBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sync Authentication";
            this.Load += new System.EventHandler(this.AuthenticationInputBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.flagRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.flagGreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.Label lblLabel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox flagRed;
        private System.Windows.Forms.PictureBox flagGreen;
    }
}