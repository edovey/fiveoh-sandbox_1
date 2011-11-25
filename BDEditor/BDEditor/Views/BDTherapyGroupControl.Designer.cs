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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAddTherapyGroup = new System.Windows.Forms.Button();
            this.btnDelTherapyGroup = new System.Windows.Forms.Button();
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
            this.panelTherapies.AutoSize = true;
            this.panelTherapies.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelTherapies.BackColor = System.Drawing.SystemColors.Control;
            this.panelTherapies.Location = new System.Drawing.Point(9, 81);
            this.panelTherapies.Name = "panelTherapies";
            this.panelTherapies.Size = new System.Drawing.Size(0, 0);
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
            this.btnTherapyGroupLink.Image = global::BDEditor.Properties.Resources.link_16;
            this.btnTherapyGroupLink.Location = new System.Drawing.Point(302, 23);
            this.btnTherapyGroupLink.Name = "btnTherapyGroupLink";
            this.btnTherapyGroupLink.Size = new System.Drawing.Size(31, 25);
            this.btnTherapyGroupLink.TabIndex = 19;
            this.btnTherapyGroupLink.UseVisualStyleBackColor = true;
            this.btnTherapyGroupLink.Click += new System.EventHandler(this.btnTherapyGroupLink_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.btnAdd.Location = new System.Drawing.Point(9, 47);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(28, 28);
            this.btnAdd.TabIndex = 20;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.Therapy_RequestItemAdd);
            // 
            // btnAddTherapyGroup
            // 
            this.btnAddTherapyGroup.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.btnAddTherapyGroup.Location = new System.Drawing.Point(765, 25);
            this.btnAddTherapyGroup.Name = "btnAddTherapyGroup";
            this.btnAddTherapyGroup.Size = new System.Drawing.Size(28, 28);
            this.btnAddTherapyGroup.TabIndex = 21;
            this.btnAddTherapyGroup.UseVisualStyleBackColor = true;
            this.btnAddTherapyGroup.Click += new System.EventHandler(this.TherapyGroup_RequestItemAdd);
            // 
            // btnDelTherapyGroup
            // 
            this.btnDelTherapyGroup.Image = global::BDEditor.Properties.Resources.del_16x16;
            this.btnDelTherapyGroup.Location = new System.Drawing.Point(799, 25);
            this.btnDelTherapyGroup.Name = "btnDelTherapyGroup";
            this.btnDelTherapyGroup.Size = new System.Drawing.Size(28, 28);
            this.btnDelTherapyGroup.TabIndex = 22;
            this.btnDelTherapyGroup.UseVisualStyleBackColor = true;
            this.btnDelTherapyGroup.Click += new System.EventHandler(this.TherapyGroup_RequestItemDelete);
            // 
            // BDTherapyGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.btnDelTherapyGroup);
            this.Controls.Add(this.btnAddTherapyGroup);
            this.Controls.Add(this.btnAdd);
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
            this.Size = new System.Drawing.Size(877, 158);
            this.Leave += new System.EventHandler(this.BDTherapyGroupControl_Leave);
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
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnAddTherapyGroup;
        private System.Windows.Forms.Button btnDelTherapyGroup;
    }
}
