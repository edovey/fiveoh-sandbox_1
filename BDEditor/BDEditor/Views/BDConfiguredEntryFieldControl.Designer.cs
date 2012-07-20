namespace BDEditor.Views
{
    partial class BDConfiguredEntryFieldControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDConfiguredEntryFieldControl));
            this.lblFieldLabel = new System.Windows.Forms.Label();
            this.txtFieldData = new System.Windows.Forms.TextBox();
            this.btnLinkedNote = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFieldLabel
            // 
            this.lblFieldLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFieldLabel.Location = new System.Drawing.Point(0, 3);
            this.lblFieldLabel.Name = "lblFieldLabel";
            this.lblFieldLabel.Size = new System.Drawing.Size(309, 23);
            this.lblFieldLabel.TabIndex = 37;
            this.lblFieldLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFieldData
            // 
            this.txtFieldData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFieldData.Location = new System.Drawing.Point(0, 27);
            this.txtFieldData.Name = "txtFieldData";
            this.txtFieldData.Size = new System.Drawing.Size(309, 20);
            this.txtFieldData.TabIndex = 35;
            this.txtFieldData.Tag = "";
            this.txtFieldData.Leave += new System.EventHandler(this.txtFieldData_Leave);
            // 
            // btnLinkedNote
            // 
            this.btnLinkedNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkedNote.Enabled = false;
            this.btnLinkedNote.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNote.Image")));
            this.btnLinkedNote.Location = new System.Drawing.Point(315, 18);
            this.btnLinkedNote.Name = "btnLinkedNote";
            this.btnLinkedNote.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNote.TabIndex = 36;
            this.btnLinkedNote.UseVisualStyleBackColor = true;
            this.btnLinkedNote.Click += new System.EventHandler(this.btnLinkedNote_Click);
            // 
            // BDConfiguredEntryFieldControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblFieldLabel);
            this.Controls.Add(this.txtFieldData);
            this.Controls.Add(this.btnLinkedNote);
            this.Name = "BDConfiguredEntryFieldControl";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(350, 50);
            this.Load += new System.EventHandler(this.BDConfiguredEntryFieldControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFieldLabel;
        private System.Windows.Forms.TextBox txtFieldData;
        private System.Windows.Forms.Button btnLinkedNote;
    }
}
