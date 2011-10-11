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
            this.therapyGroupName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.panelTherapies = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTherapyGroupLink = new System.Windows.Forms.Button();
            this.bdTherapyControl3 = new BDEditor.Views.BDTherapyControl();
            this.bdTherapyControl2 = new BDEditor.Views.BDTherapyControl();
            this.bdTherapyControl1 = new BDEditor.Views.BDTherapyControl();
            this.panelTherapies.SuspendLayout();
            this.SuspendLayout();
            // 
            // therapyGroupName
            // 
            this.therapyGroupName.AutoSize = true;
            this.therapyGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.therapyGroupName.Location = new System.Drawing.Point(6, 4);
            this.therapyGroupName.Name = "therapyGroupName";
            this.therapyGroupName.Size = new System.Drawing.Size(169, 18);
            this.therapyGroupName.TabIndex = 6;
            this.therapyGroupName.Text = "Therapy Group Name";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(9, 25);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(286, 20);
            this.tbName.TabIndex = 7;
            this.tbName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(572, 25);
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
            this.andRadioButton.Location = new System.Drawing.Point(471, 25);
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
            this.noneRadioButton.Location = new System.Drawing.Point(350, 25);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(121, 17);
            this.noneRadioButton.TabIndex = 12;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "Next Therapy Group";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // panelTherapies
            // 
            this.panelTherapies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTherapies.AutoScroll = true;
            this.panelTherapies.BackColor = System.Drawing.SystemColors.Control;
            this.panelTherapies.Controls.Add(this.bdTherapyControl3);
            this.panelTherapies.Controls.Add(this.bdTherapyControl2);
            this.panelTherapies.Controls.Add(this.bdTherapyControl1);
            this.panelTherapies.Location = new System.Drawing.Point(6, 76);
            this.panelTherapies.Name = "panelTherapies";
            this.panelTherapies.Size = new System.Drawing.Size(791, 179);
            this.panelTherapies.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(38, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 18);
            this.label1.TabIndex = 16;
            this.label1.Text = "Therapy";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(299, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 18);
            this.label2.TabIndex = 17;
            this.label2.Text = "Dosage";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(556, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 18);
            this.label3.TabIndex = 18;
            this.label3.Text = "Duration";
            // 
            // btnTherapyGroupLink
            // 
            this.btnTherapyGroupLink.Location = new System.Drawing.Point(302, 25);
            this.btnTherapyGroupLink.Name = "btnTherapyGroupLink";
            this.btnTherapyGroupLink.Size = new System.Drawing.Size(38, 23);
            this.btnTherapyGroupLink.TabIndex = 19;
            this.btnTherapyGroupLink.Text = "Link";
            this.btnTherapyGroupLink.UseVisualStyleBackColor = true;
            this.btnTherapyGroupLink.Click += new System.EventHandler(this.btnTherapyGroupLink_Click);
            // 
            // bdTherapyControl3
            // 
            this.bdTherapyControl3.CurrentTherapy = null;
            this.bdTherapyControl3.DisplayLeftBracket = false;
            this.bdTherapyControl3.DisplayRightBracket = false;
            this.bdTherapyControl3.Location = new System.Drawing.Point(4, 118);
            this.bdTherapyControl3.Name = "bdTherapyControl3";
            this.bdTherapyControl3.Padding = new System.Windows.Forms.Padding(3);
            this.bdTherapyControl3.Size = new System.Drawing.Size(783, 50);
            this.bdTherapyControl3.TabIndex = 2;
            // 
            // bdTherapyControl2
            // 
            this.bdTherapyControl2.CurrentTherapy = null;
            this.bdTherapyControl2.DisplayLeftBracket = false;
            this.bdTherapyControl2.DisplayRightBracket = false;
            this.bdTherapyControl2.Location = new System.Drawing.Point(4, 61);
            this.bdTherapyControl2.Name = "bdTherapyControl2";
            this.bdTherapyControl2.Padding = new System.Windows.Forms.Padding(3);
            this.bdTherapyControl2.Size = new System.Drawing.Size(783, 50);
            this.bdTherapyControl2.TabIndex = 1;
            // 
            // bdTherapyControl1
            // 
            this.bdTherapyControl1.CurrentTherapy = null;
            this.bdTherapyControl1.DisplayLeftBracket = false;
            this.bdTherapyControl1.DisplayRightBracket = false;
            this.bdTherapyControl1.Location = new System.Drawing.Point(4, 4);
            this.bdTherapyControl1.Name = "bdTherapyControl1";
            this.bdTherapyControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdTherapyControl1.Size = new System.Drawing.Size(783, 50);
            this.bdTherapyControl1.TabIndex = 0;
            // 
            // BDTherapyGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnTherapyGroupLink);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelTherapies);
            this.Controls.Add(this.orRadioButton);
            this.Controls.Add(this.andRadioButton);
            this.Controls.Add(this.noneRadioButton);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.therapyGroupName);
            this.Name = "BDTherapyGroupControl";
            this.Size = new System.Drawing.Size(800, 261);
            this.Leave += new System.EventHandler(this.BDTherapyGroupControl_Leave);
            this.panelTherapies.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label therapyGroupName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.Panel panelTherapies;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTherapyGroupLink;
        private BDTherapyControl bdTherapyControl3;
        private BDTherapyControl bdTherapyControl2;
        private BDTherapyControl bdTherapyControl1;
    }
}
