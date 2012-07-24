namespace BDEditor.Views
{
    partial class BDCombinedEntryControl
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
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pnlRadioButtons = new System.Windows.Forms.Panel();
            this.andOrRadioButton = new System.Windows.Forms.RadioButton();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.thenRadioButton = new System.Windows.Forms.RadioButton();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.panelFields = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblVirtualColumnTwo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblVirtualColumnOne = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bdCombinedEntryFieldControl4 = new BDEditor.Views.BDCombinedEntryFieldControl();
            this.bdCombinedEntryFieldControl3 = new BDEditor.Views.BDCombinedEntryFieldControl();
            this.bdCombinedEntryFieldControl2 = new BDEditor.Views.BDCombinedEntryFieldControl();
            this.bdCombinedEntryFieldControl1 = new BDEditor.Views.BDCombinedEntryFieldControl();
            this.panelTop.SuspendLayout();
            this.pnlRadioButtons.SuspendLayout();
            this.panelFields.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.pnlRadioButtons);
            this.panelTop.Controls.Add(this.txtName);
            this.panelTop.Controls.Add(this.txtTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(500, 83);
            this.panelTop.TabIndex = 1;
            // 
            // pnlRadioButtons
            // 
            this.pnlRadioButtons.BackColor = System.Drawing.SystemColors.Control;
            this.pnlRadioButtons.Controls.Add(this.andOrRadioButton);
            this.pnlRadioButtons.Controls.Add(this.noneRadioButton);
            this.pnlRadioButtons.Controls.Add(this.andRadioButton);
            this.pnlRadioButtons.Controls.Add(this.orRadioButton);
            this.pnlRadioButtons.Controls.Add(this.thenRadioButton);
            this.pnlRadioButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRadioButtons.Location = new System.Drawing.Point(0, 60);
            this.pnlRadioButtons.Name = "pnlRadioButtons";
            this.pnlRadioButtons.Size = new System.Drawing.Size(500, 23);
            this.pnlRadioButtons.TabIndex = 41;
            // 
            // andOrRadioButton
            // 
            this.andOrRadioButton.AutoSize = true;
            this.andOrRadioButton.Location = new System.Drawing.Point(384, 3);
            this.andOrRadioButton.Name = "andOrRadioButton";
            this.andOrRadioButton.Size = new System.Drawing.Size(93, 17);
            this.andOrRadioButton.TabIndex = 13;
            this.andOrRadioButton.TabStop = true;
            this.andOrRadioButton.Text = "+/-  (with next)";
            this.andOrRadioButton.UseVisualStyleBackColor = true;
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(21, 3);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(47, 17);
            this.noneRadioButton.TabIndex = 9;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "Next";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(81, 3);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(95, 17);
            this.andRadioButton.TabIndex = 10;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "And (with next)";
            this.andRadioButton.UseVisualStyleBackColor = true;
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(182, 3);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(87, 17);
            this.orRadioButton.TabIndex = 11;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "Or (with next)";
            this.orRadioButton.UseVisualStyleBackColor = true;
            // 
            // thenRadioButton
            // 
            this.thenRadioButton.AutoSize = true;
            this.thenRadioButton.Location = new System.Drawing.Point(276, 3);
            this.thenRadioButton.Name = "thenRadioButton";
            this.thenRadioButton.Size = new System.Drawing.Size(101, 17);
            this.thenRadioButton.TabIndex = 12;
            this.thenRadioButton.TabStop = true;
            this.thenRadioButton.Text = "Then (with next)";
            this.thenRadioButton.UseVisualStyleBackColor = true;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(54, 30);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(407, 20);
            this.txtName.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtName, "Therapy Name");
            // 
            // txtTitle
            // 
            this.txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTitle.Location = new System.Drawing.Point(54, 3);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(407, 20);
            this.txtTitle.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtTitle, "Group Title");
            // 
            // panelFields
            // 
            this.panelFields.AutoSize = true;
            this.panelFields.Controls.Add(this.panel2);
            this.panelFields.Controls.Add(this.panel1);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFields.Location = new System.Drawing.Point(0, 83);
            this.panelFields.MinimumSize = new System.Drawing.Size(500, 447);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(500, 454);
            this.panelFields.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.bdCombinedEntryFieldControl4);
            this.panel2.Controls.Add(this.bdCombinedEntryFieldControl3);
            this.panel2.Controls.Add(this.lblVirtualColumnTwo);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 214);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(500, 214);
            this.panel2.TabIndex = 1;
            // 
            // lblVirtualColumnTwo
            // 
            this.lblVirtualColumnTwo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVirtualColumnTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVirtualColumnTwo.Location = new System.Drawing.Point(3, 3);
            this.lblVirtualColumnTwo.Name = "lblVirtualColumnTwo";
            this.lblVirtualColumnTwo.Size = new System.Drawing.Size(494, 22);
            this.lblVirtualColumnTwo.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.bdCombinedEntryFieldControl2);
            this.panel1.Controls.Add(this.bdCombinedEntryFieldControl1);
            this.panel1.Controls.Add(this.lblVirtualColumnOne);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(500, 214);
            this.panel1.TabIndex = 0;
            // 
            // lblVirtualColumnOne
            // 
            this.lblVirtualColumnOne.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVirtualColumnOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVirtualColumnOne.Location = new System.Drawing.Point(3, 3);
            this.lblVirtualColumnOne.Name = "lblVirtualColumnOne";
            this.lblVirtualColumnOne.Size = new System.Drawing.Size(494, 22);
            this.lblVirtualColumnOne.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Group";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Therapy";
            // 
            // bdCombinedEntryFieldControl4
            // 
            this.bdCombinedEntryFieldControl4.Dock = System.Windows.Forms.DockStyle.Top;
            this.bdCombinedEntryFieldControl4.Location = new System.Drawing.Point(3, 116);
            this.bdCombinedEntryFieldControl4.Name = "bdCombinedEntryFieldControl4";
            this.bdCombinedEntryFieldControl4.Size = new System.Drawing.Size(494, 91);
            this.bdCombinedEntryFieldControl4.TabIndex = 1;
            // 
            // bdCombinedEntryFieldControl3
            // 
            this.bdCombinedEntryFieldControl3.Dock = System.Windows.Forms.DockStyle.Top;
            this.bdCombinedEntryFieldControl3.Location = new System.Drawing.Point(3, 25);
            this.bdCombinedEntryFieldControl3.Name = "bdCombinedEntryFieldControl3";
            this.bdCombinedEntryFieldControl3.Size = new System.Drawing.Size(494, 91);
            this.bdCombinedEntryFieldControl3.TabIndex = 2;
            // 
            // bdCombinedEntryFieldControl2
            // 
            this.bdCombinedEntryFieldControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.bdCombinedEntryFieldControl2.Location = new System.Drawing.Point(3, 116);
            this.bdCombinedEntryFieldControl2.Name = "bdCombinedEntryFieldControl2";
            this.bdCombinedEntryFieldControl2.Size = new System.Drawing.Size(494, 91);
            this.bdCombinedEntryFieldControl2.TabIndex = 2;
            // 
            // bdCombinedEntryFieldControl1
            // 
            this.bdCombinedEntryFieldControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.bdCombinedEntryFieldControl1.Location = new System.Drawing.Point(3, 25);
            this.bdCombinedEntryFieldControl1.Name = "bdCombinedEntryFieldControl1";
            this.bdCombinedEntryFieldControl1.Size = new System.Drawing.Size(494, 91);
            this.bdCombinedEntryFieldControl1.TabIndex = 1;
            // 
            // BDCombinedEntryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.panelTop);
            this.MinimumSize = new System.Drawing.Size(500, 530);
            this.Name = "BDCombinedEntryControl";
            this.Size = new System.Drawing.Size(500, 537);
            this.Load += new System.EventHandler(this.BDCombinedEntryControl_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.pnlRadioButtons.ResumeLayout(false);
            this.pnlRadioButtons.PerformLayout();
            this.panelFields.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel2;
        private BDCombinedEntryFieldControl bdCombinedEntryFieldControl3;
        private BDCombinedEntryFieldControl bdCombinedEntryFieldControl4;
        private System.Windows.Forms.Label lblVirtualColumnTwo;
        private System.Windows.Forms.Panel panel1;
        private BDCombinedEntryFieldControl bdCombinedEntryFieldControl2;
        private BDCombinedEntryFieldControl bdCombinedEntryFieldControl1;
        private System.Windows.Forms.Label lblVirtualColumnOne;
        private System.Windows.Forms.Panel pnlRadioButtons;
        private System.Windows.Forms.RadioButton andOrRadioButton;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.RadioButton thenRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

    }
}
