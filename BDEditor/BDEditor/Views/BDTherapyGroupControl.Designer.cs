namespace BDEditor.Views
{
    partial class BDTherapyGroupControl
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
            this.lblTherapyGroupNote = new System.Windows.Forms.Label();
            this.therapyGroupName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.panelTherapies = new System.Windows.Forms.Panel();
            this.bdTherapyControl2 = new BDEditor.Views.BDTherapyControl();
            this.bdTherapyControl1 = new BDEditor.Views.BDTherapyControl();
            this.panelTherapies.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTherapyGroupNote
            // 
            this.lblTherapyGroupNote.AutoSize = true;
            this.lblTherapyGroupNote.Location = new System.Drawing.Point(343, 4);
            this.lblTherapyGroupNote.Name = "lblTherapyGroupNote";
            this.lblTherapyGroupNote.Size = new System.Drawing.Size(72, 13);
            this.lblTherapyGroupNote.TabIndex = 4;
            this.lblTherapyGroupNote.Text = "Therapy Note";
            // 
            // therapyGroupName
            // 
            this.therapyGroupName.AutoSize = true;
            this.therapyGroupName.Location = new System.Drawing.Point(16, 4);
            this.therapyGroupName.Name = "therapyGroupName";
            this.therapyGroupName.Size = new System.Drawing.Size(35, 13);
            this.therapyGroupName.TabIndex = 6;
            this.therapyGroupName.Text = "Name";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(16, 20);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(286, 20);
            this.tbName.TabIndex = 7;
            this.tbName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(215, 62);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(87, 17);
            this.orRadioButton.TabIndex = 14;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "Or (with next)";
            this.orRadioButton.UseVisualStyleBackColor = true;
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(114, 62);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(95, 17);
            this.andRadioButton.TabIndex = 13;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "And (with next)";
            this.andRadioButton.UseVisualStyleBackColor = true;
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(19, 62);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(89, 17);
            this.noneRadioButton.TabIndex = 12;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "Next Therapy";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // panelTherapies
            // 
            this.panelTherapies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTherapies.AutoScroll = true;
            this.panelTherapies.BackColor = System.Drawing.SystemColors.Control;
            this.panelTherapies.Controls.Add(this.bdTherapyControl2);
            this.panelTherapies.Controls.Add(this.bdTherapyControl1);
            this.panelTherapies.Location = new System.Drawing.Point(6, 91);
            this.panelTherapies.Name = "panelTherapies";
            this.panelTherapies.Size = new System.Drawing.Size(868, 178);
            this.panelTherapies.TabIndex = 15;
            // 
            // bdTherapyControl2
            // 
            this.bdTherapyControl2.BackColor = System.Drawing.SystemColors.Control;
            this.bdTherapyControl2.CurrentTherapy = null;
            this.bdTherapyControl2.Location = new System.Drawing.Point(3, 88);
            this.bdTherapyControl2.Name = "bdTherapyControl2";
            this.bdTherapyControl2.Size = new System.Drawing.Size(844, 84);
            this.bdTherapyControl2.TabIndex = 1;
            // 
            // bdTherapyControl1
            // 
            this.bdTherapyControl1.BackColor = System.Drawing.SystemColors.Control;
            this.bdTherapyControl1.CurrentTherapy = null;
            this.bdTherapyControl1.Location = new System.Drawing.Point(3, 3);
            this.bdTherapyControl1.Name = "bdTherapyControl1";
            this.bdTherapyControl1.Size = new System.Drawing.Size(844, 84);
            this.bdTherapyControl1.TabIndex = 0;
            // 
            // BDTherapyGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelTherapies);
            this.Controls.Add(this.orRadioButton);
            this.Controls.Add(this.andRadioButton);
            this.Controls.Add(this.noneRadioButton);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.therapyGroupName);
            this.Controls.Add(this.lblTherapyGroupNote);
            this.Name = "BDTherapyGroupControl";
            this.Size = new System.Drawing.Size(880, 272);
            this.Leave += new System.EventHandler(this.BDTherapyGroupControl_Leave);
            this.panelTherapies.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTherapyGroupNote;
        private System.Windows.Forms.Label therapyGroupName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.Panel panelTherapies;
        private BDTherapyControl bdTherapyControl2;
        private BDTherapyControl bdTherapyControl1;
    }
}
