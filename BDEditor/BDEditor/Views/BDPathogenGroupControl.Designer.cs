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
            this.panelTherapyGroups = new System.Windows.Forms.Panel();
            this.btnAddTherapyGroup = new System.Windows.Forms.Button();
            this.pathogenSet1 = new BDEditor.Views.PathogenSet();
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
            // panelTherapyGroups
            // 
            this.panelTherapyGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTherapyGroups.AutoScroll = true;
            this.panelTherapyGroups.AutoSize = true;
            this.panelTherapyGroups.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelTherapyGroups.Location = new System.Drawing.Point(8, 100);
            this.panelTherapyGroups.Name = "panelTherapyGroups";
            this.panelTherapyGroups.Size = new System.Drawing.Size(0, 0);
            this.panelTherapyGroups.TabIndex = 5;
            // 
            // btnAddTherapyGroup
            // 
            this.btnAddTherapyGroup.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.btnAddTherapyGroup.Location = new System.Drawing.Point(849, 66);
            this.btnAddTherapyGroup.Name = "btnAddTherapyGroup";
            this.btnAddTherapyGroup.Size = new System.Drawing.Size(28, 28);
            this.btnAddTherapyGroup.TabIndex = 6;
            this.btnAddTherapyGroup.UseVisualStyleBackColor = true;
            this.btnAddTherapyGroup.Click += new System.EventHandler(this.TherapyGroup_RequestItemAdd);
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
            // BDPathogenGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.btnAddTherapyGroup);
            this.Controls.Add(this.panelTherapyGroups);
            this.Controls.Add(this.pathogenSet1);
            this.Controls.Add(this.label1);
            this.Name = "BDPathogenGroupControl";
            this.Size = new System.Drawing.Size(977, 331);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private PathogenSet pathogenSet1;
        private System.Windows.Forms.Panel panelTherapyGroups;
        private System.Windows.Forms.Button btnAddTherapyGroup;
    }
}
