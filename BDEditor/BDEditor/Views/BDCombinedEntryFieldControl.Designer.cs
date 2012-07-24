namespace BDEditor.Views
{
    partial class BDCombinedEntryFieldControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDCombinedEntryFieldControl));
            this.txtEntryTitle = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnLinkedNoteTitle = new System.Windows.Forms.Button();
            this.btnLinkedNoteDetail = new System.Windows.Forms.Button();
            this.txtEntryDetail = new System.Windows.Forms.TextBox();
            this.pnlRadioButtons = new System.Windows.Forms.Panel();
            this.andOrRadioButton = new System.Windows.Forms.RadioButton();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.thenRadioButton = new System.Windows.Forms.RadioButton();
            this.pnlRadioButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtEntryTitle
            // 
            this.txtEntryTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntryTitle.Location = new System.Drawing.Point(5, 11);
            this.txtEntryTitle.Name = "txtEntryTitle";
            this.txtEntryTitle.Size = new System.Drawing.Size(449, 20);
            this.txtEntryTitle.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtEntryTitle, "Entry Title");
            this.txtEntryTitle.Leave += new System.EventHandler(this.txtField_Leave);
            // 
            // btnLinkedNoteTitle
            // 
            this.btnLinkedNoteTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkedNoteTitle.Enabled = false;
            this.btnLinkedNoteTitle.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNoteTitle.Image")));
            this.btnLinkedNoteTitle.Location = new System.Drawing.Point(458, 7);
            this.btnLinkedNoteTitle.Name = "btnLinkedNoteTitle";
            this.btnLinkedNoteTitle.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNoteTitle.TabIndex = 37;
            this.toolTip1.SetToolTip(this.btnLinkedNoteTitle, "Entry Title Note");
            this.btnLinkedNoteTitle.UseVisualStyleBackColor = true;
            // 
            // btnLinkedNoteDetail
            // 
            this.btnLinkedNoteDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkedNoteDetail.Enabled = false;
            this.btnLinkedNoteDetail.Image = ((System.Drawing.Image)(resources.GetObject("btnLinkedNoteDetail.Image")));
            this.btnLinkedNoteDetail.Location = new System.Drawing.Point(458, 38);
            this.btnLinkedNoteDetail.Name = "btnLinkedNoteDetail";
            this.btnLinkedNoteDetail.Size = new System.Drawing.Size(28, 28);
            this.btnLinkedNoteDetail.TabIndex = 39;
            this.toolTip1.SetToolTip(this.btnLinkedNoteDetail, "Entry Detail Note");
            this.btnLinkedNoteDetail.UseVisualStyleBackColor = true;
            // 
            // txtEntryDetail
            // 
            this.txtEntryDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntryDetail.Location = new System.Drawing.Point(5, 42);
            this.txtEntryDetail.Name = "txtEntryDetail";
            this.txtEntryDetail.Size = new System.Drawing.Size(449, 20);
            this.txtEntryDetail.TabIndex = 38;
            this.toolTip1.SetToolTip(this.txtEntryDetail, "Entry Detail");
            this.txtEntryDetail.Leave += new System.EventHandler(this.txtField_Leave);
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
            this.pnlRadioButtons.Location = new System.Drawing.Point(0, 68);
            this.pnlRadioButtons.Name = "pnlRadioButtons";
            this.pnlRadioButtons.Size = new System.Drawing.Size(491, 23);
            this.pnlRadioButtons.TabIndex = 40;
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
            this.andOrRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
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
            this.noneRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
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
            this.andRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
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
            this.orRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
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
            this.thenRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // BDCombinedEntryFieldControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlRadioButtons);
            this.Controls.Add(this.btnLinkedNoteDetail);
            this.Controls.Add(this.txtEntryDetail);
            this.Controls.Add(this.btnLinkedNoteTitle);
            this.Controls.Add(this.txtEntryTitle);
            this.Name = "BDCombinedEntryFieldControl";
            this.Size = new System.Drawing.Size(491, 91);
            this.Load += new System.EventHandler(this.BDCombinedEntryFieldControl_Load);
            this.pnlRadioButtons.ResumeLayout(false);
            this.pnlRadioButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEntryTitle;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnLinkedNoteTitle;
        private System.Windows.Forms.Button btnLinkedNoteDetail;
        private System.Windows.Forms.TextBox txtEntryDetail;
        private System.Windows.Forms.Panel pnlRadioButtons;
        private System.Windows.Forms.RadioButton andOrRadioButton;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.RadioButton thenRadioButton;

    }
}
