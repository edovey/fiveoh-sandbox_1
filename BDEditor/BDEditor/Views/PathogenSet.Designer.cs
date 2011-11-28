namespace BDEditor.Views
{
    partial class PathogenSet
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
            this.bdPathogenControl1 = new BDEditor.Views.BDPathogenControl();
            this.SuspendLayout();
            // 
            // bdPathogenControl1
            // 
            this.bdPathogenControl1.AutoSize = true;
            this.bdPathogenControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bdPathogenControl1.BackColor = System.Drawing.SystemColors.Control;
            this.bdPathogenControl1.CurrentPathogen = null;
            this.bdPathogenControl1.DisplayOrder = null;
            this.bdPathogenControl1.Location = new System.Drawing.Point(3, 2);
            this.bdPathogenControl1.Name = "bdPathogenControl1";
            this.bdPathogenControl1.Size = new System.Drawing.Size(378, 31);
            this.bdPathogenControl1.TabIndex = 0;
            // 
            // PathogenSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.bdPathogenControl1);
            this.Name = "PathogenSet";
            this.Size = new System.Drawing.Size(535, 43);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BDPathogenControl bdPathogenControl1;

    }
}
