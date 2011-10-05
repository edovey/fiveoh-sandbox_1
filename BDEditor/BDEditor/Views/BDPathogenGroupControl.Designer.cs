namespace BDEditor.Views
{
    partial class BDPathogenGroupControl
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
            this.pathogenSet1 = new BDEditor.Views.PathogenSet();
            this.bdTherapyGroupControl1 = new BDEditor.Views.BDTherapyGroupControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Pathogens";
            // 
            // pathogenSet1
            // 
            this.pathogenSet1.AutoScroll = true;
            this.pathogenSet1.CurrentPathogenGroup = null;
            this.pathogenSet1.Location = new System.Drawing.Point(8, 30);
            this.pathogenSet1.Name = "pathogenSet1";
            this.pathogenSet1.Size = new System.Drawing.Size(308, 64);
            this.pathogenSet1.TabIndex = 2;
            // 
            // bdTherapyGroupControl1
            // 
            this.bdTherapyGroupControl1.BackColor = System.Drawing.SystemColors.Control;
            this.bdTherapyGroupControl1.CurrentTherapyGroup = null;
            this.bdTherapyGroupControl1.Location = new System.Drawing.Point(3, 119);
            this.bdTherapyGroupControl1.Name = "bdTherapyGroupControl1";
            this.bdTherapyGroupControl1.Size = new System.Drawing.Size(880, 270);
            this.bdTherapyGroupControl1.TabIndex = 3;
            // 
            // BDPathogenGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.pathogenSet1);
            this.Controls.Add(this.bdTherapyGroupControl1);
            this.Controls.Add(this.label1);
            this.Name = "BDPathogenGroupControl";
            this.Size = new System.Drawing.Size(898, 396);
            this.Load += new System.EventHandler(this.BDPathogenGroupControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BDTherapyGroupControl bdTherapyGroupControl1;
        private System.Windows.Forms.Label label1;
        private PathogenSet pathogenSet1;
    }
}
