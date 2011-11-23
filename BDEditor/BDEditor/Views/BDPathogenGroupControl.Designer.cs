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
            this.bdTherapyGroupControl2 = new BDEditor.Views.BDTherapyGroupControl();
            this.therapyGroupPanel = new System.Windows.Forms.Panel();
            this.therapyGroupPanel.SuspendLayout();
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
            this.bdTherapyGroupControl1.AutoScroll = true;
            this.bdTherapyGroupControl1.BackColor = System.Drawing.SystemColors.Control;
            this.bdTherapyGroupControl1.CurrentTherapyGroup = null;
            this.bdTherapyGroupControl1.DisplayOrder = null;
            this.bdTherapyGroupControl1.Location = new System.Drawing.Point(4, 10);
            this.bdTherapyGroupControl1.Name = "bdTherapyGroupControl1";
            this.bdTherapyGroupControl1.Size = new System.Drawing.Size(877, 254);
            this.bdTherapyGroupControl1.TabIndex = 3;
            // 
            // bdTherapyGroupControl2
            // 
            this.bdTherapyGroupControl2.AutoScroll = true;
            this.bdTherapyGroupControl2.BackColor = System.Drawing.SystemColors.Control;
            this.bdTherapyGroupControl2.CurrentTherapyGroup = null;
            this.bdTherapyGroupControl2.DisplayOrder = null;
            this.bdTherapyGroupControl2.Location = new System.Drawing.Point(4, 270);
            this.bdTherapyGroupControl2.Name = "bdTherapyGroupControl2";
            this.bdTherapyGroupControl2.Size = new System.Drawing.Size(865, 254);
            this.bdTherapyGroupControl2.TabIndex = 4;
            // 
            // therapyGroupPanel
            // 
            this.therapyGroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.therapyGroupPanel.AutoScroll = true;
            this.therapyGroupPanel.Controls.Add(this.bdTherapyGroupControl1);
            this.therapyGroupPanel.Controls.Add(this.bdTherapyGroupControl2);
            this.therapyGroupPanel.Location = new System.Drawing.Point(8, 100);
            this.therapyGroupPanel.Name = "therapyGroupPanel";
            this.therapyGroupPanel.Size = new System.Drawing.Size(885, 549);
            this.therapyGroupPanel.TabIndex = 5;
            // 
            // BDPathogenGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.therapyGroupPanel);
            this.Controls.Add(this.pathogenSet1);
            this.Controls.Add(this.label1);
            this.Name = "BDPathogenGroupControl";
            this.Size = new System.Drawing.Size(900, 652);
            this.therapyGroupPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BDTherapyGroupControl bdTherapyGroupControl1;
        private System.Windows.Forms.Label label1;
        private PathogenSet pathogenSet1;
        private BDTherapyGroupControl bdTherapyGroupControl2;
        private System.Windows.Forms.Panel therapyGroupPanel;
    }
}
