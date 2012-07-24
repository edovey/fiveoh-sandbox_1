namespace BDEditor.Views
{
    partial class BDCombinedEntryFieldGroupControl
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
            this.lblVirtualColumnName = new System.Windows.Forms.Label();
            this.panelFields = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelFields.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblVirtualColumnName
            // 
            this.lblVirtualColumnName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblVirtualColumnName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVirtualColumnName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVirtualColumnName.Location = new System.Drawing.Point(3, 3);
            this.lblVirtualColumnName.Name = "lblVirtualColumnName";
            this.lblVirtualColumnName.Size = new System.Drawing.Size(410, 20);
            this.lblVirtualColumnName.TabIndex = 0;
            // 
            // panelFields
            // 
            this.panelFields.Controls.Add(this.label1);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFields.Location = new System.Drawing.Point(3, 23);
            this.panelFields.Name = "panelFields";
            this.panelFields.Padding = new System.Windows.Forms.Padding(3);
            this.panelFields.Size = new System.Drawing.Size(410, 49);
            this.panelFields.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(82, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "CombinedEntryFieldGroup";
            // 
            // BDCombinedEntryFieldGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.lblVirtualColumnName);
            this.MinimumSize = new System.Drawing.Size(200, 75);
            this.Name = "BDCombinedEntryFieldGroupControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(416, 75);
            this.panelFields.ResumeLayout(false);
            this.panelFields.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblVirtualColumnName;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.Label label1;
    }
}
