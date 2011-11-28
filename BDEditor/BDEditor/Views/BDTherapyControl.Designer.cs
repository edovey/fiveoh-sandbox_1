namespace BDEditor.Views
{
    partial class BDTherapyControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDTherapyControl));
            this.tbName = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbDosage = new System.Windows.Forms.TextBox();
            this.tbDuration = new System.Windows.Forms.TextBox();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.lblLeftBracket = new System.Windows.Forms.Label();
            this.lblRightBracket = new System.Windows.Forms.Label();
            this.thenRadioButton = new System.Windows.Forms.RadioButton();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnDurationLink = new System.Windows.Forms.Button();
            this.btnDosageLink = new System.Windows.Forms.Button();
            this.btnTherapyLink = new System.Windows.Forms.Button();
            this.btnReorderToPrevious = new System.Windows.Forms.Button();
            this.btnReorderToNext = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbName
            // 
            this.tbName.ContextMenuStrip = this.contextMenuStrip1;
            this.tbName.Location = new System.Drawing.Point(25, 3);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(218, 20);
            this.tbName.TabIndex = 0;
            this.tbName.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bToolStripMenuItem,
            this.geToolStripMenuItem,
            this.leToolStripMenuItem,
            this.plusMinusToolStripMenuItem,
            this.degreeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(69, 114);
            this.contextMenuStrip1.Text = "Insert Symbol";
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // tbDosage
            // 
            this.tbDosage.Location = new System.Drawing.Point(290, 3);
            this.tbDosage.Name = "tbDosage";
            this.tbDosage.Size = new System.Drawing.Size(211, 20);
            this.tbDosage.TabIndex = 2;
            this.tbDosage.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // tbDuration
            // 
            this.tbDuration.Location = new System.Drawing.Point(548, 3);
            this.tbDuration.Name = "tbDuration";
            this.tbDuration.Size = new System.Drawing.Size(174, 20);
            this.tbDuration.TabIndex = 4;
            this.tbDuration.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(30, 30);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(89, 17);
            this.noneRadioButton.TabIndex = 6;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "Next Therapy";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(125, 30);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(95, 17);
            this.andRadioButton.TabIndex = 7;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "And (with next)";
            this.andRadioButton.UseVisualStyleBackColor = true;
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(226, 30);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(87, 17);
            this.orRadioButton.TabIndex = 8;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "Or (with next)";
            this.orRadioButton.UseVisualStyleBackColor = true;
            // 
            // lblLeftBracket
            // 
            this.lblLeftBracket.AutoSize = true;
            this.lblLeftBracket.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeftBracket.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblLeftBracket.Location = new System.Drawing.Point(-6, -8);
            this.lblLeftBracket.Name = "lblLeftBracket";
            this.lblLeftBracket.Size = new System.Drawing.Size(38, 55);
            this.lblLeftBracket.TabIndex = 10;
            this.lblLeftBracket.Text = "[";
            this.lblLeftBracket.Click += new System.EventHandler(this.lblLeftBracket_Click);
            // 
            // lblRightBracket
            // 
            this.lblRightBracket.AutoSize = true;
            this.lblRightBracket.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRightBracket.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblRightBracket.Location = new System.Drawing.Point(755, -8);
            this.lblRightBracket.Name = "lblRightBracket";
            this.lblRightBracket.Size = new System.Drawing.Size(38, 55);
            this.lblRightBracket.TabIndex = 11;
            this.lblRightBracket.Text = "]";
            this.lblRightBracket.Click += new System.EventHandler(this.lblRightBracket_Click);
            // 
            // thenRadioButton
            // 
            this.thenRadioButton.AutoSize = true;
            this.thenRadioButton.Location = new System.Drawing.Point(320, 30);
            this.thenRadioButton.Name = "thenRadioButton";
            this.thenRadioButton.Size = new System.Drawing.Size(101, 17);
            this.thenRadioButton.TabIndex = 9;
            this.thenRadioButton.TabStop = true;
            this.thenRadioButton.Text = "Then (with next)";
            this.thenRadioButton.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Image = global::BDEditor.Properties.Resources.remove;
            this.btnDelete.Location = new System.Drawing.Point(783, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(28, 28);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnDurationLink
            // 
            this.btnDurationLink.Enabled = false;
            this.btnDurationLink.Image = ((System.Drawing.Image)(resources.GetObject("btnDurationLink.Image")));
            this.btnDurationLink.Location = new System.Drawing.Point(728, 2);
            this.btnDurationLink.Name = "btnDurationLink";
            this.btnDurationLink.Size = new System.Drawing.Size(28, 28);
            this.btnDurationLink.TabIndex = 5;
            this.btnDurationLink.UseVisualStyleBackColor = true;
            this.btnDurationLink.Click += new System.EventHandler(this.btnDurationLink_Click);
            // 
            // btnDosageLink
            // 
            this.btnDosageLink.Enabled = false;
            this.btnDosageLink.Image = ((System.Drawing.Image)(resources.GetObject("btnDosageLink.Image")));
            this.btnDosageLink.Location = new System.Drawing.Point(507, 2);
            this.btnDosageLink.Name = "btnDosageLink";
            this.btnDosageLink.Size = new System.Drawing.Size(28, 28);
            this.btnDosageLink.TabIndex = 3;
            this.btnDosageLink.UseVisualStyleBackColor = true;
            this.btnDosageLink.Click += new System.EventHandler(this.btnDosageLink_Click);
            // 
            // btnTherapyLink
            // 
            this.btnTherapyLink.Enabled = false;
            this.btnTherapyLink.Image = ((System.Drawing.Image)(resources.GetObject("btnTherapyLink.Image")));
            this.btnTherapyLink.Location = new System.Drawing.Point(249, 2);
            this.btnTherapyLink.Name = "btnTherapyLink";
            this.btnTherapyLink.Size = new System.Drawing.Size(28, 28);
            this.btnTherapyLink.TabIndex = 1;
            this.btnTherapyLink.UseVisualStyleBackColor = true;
            this.btnTherapyLink.Click += new System.EventHandler(this.btnTherapyLink_Click);
            // 
            // btnReorderToPrevious
            // 
            this.btnReorderToPrevious.Image = global::BDEditor.Properties.Resources.reorder_previous;
            this.btnReorderToPrevious.Location = new System.Drawing.Point(816, 3);
            this.btnReorderToPrevious.Name = "btnReorderToPrevious";
            this.btnReorderToPrevious.Size = new System.Drawing.Size(28, 28);
            this.btnReorderToPrevious.TabIndex = 14;
            this.btnReorderToPrevious.UseVisualStyleBackColor = true;
            this.btnReorderToPrevious.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // btnReorderToNext
            // 
            this.btnReorderToNext.Image = global::BDEditor.Properties.Resources.reorder_next;
            this.btnReorderToNext.Location = new System.Drawing.Point(816, 30);
            this.btnReorderToNext.Name = "btnReorderToNext";
            this.btnReorderToNext.Size = new System.Drawing.Size(28, 28);
            this.btnReorderToNext.TabIndex = 15;
            this.btnReorderToNext.UseVisualStyleBackColor = true;
            this.btnReorderToNext.Click += new System.EventHandler(this.btnReorderToNext_Click);
            // 
            // BDTherapyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnReorderToNext);
            this.Controls.Add(this.btnReorderToPrevious);
            this.Controls.Add(this.thenRadioButton);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.orRadioButton);
            this.Controls.Add(this.andRadioButton);
            this.Controls.Add(this.noneRadioButton);
            this.Controls.Add(this.btnDurationLink);
            this.Controls.Add(this.btnDosageLink);
            this.Controls.Add(this.btnTherapyLink);
            this.Controls.Add(this.tbDuration);
            this.Controls.Add(this.tbDosage);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lblLeftBracket);
            this.Controls.Add(this.lblRightBracket);
            this.Name = "BDTherapyControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(850, 60);
            this.Leave += new System.EventHandler(this.BDTherapyControl_Leave);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbDosage;
        private System.Windows.Forms.TextBox tbDuration;
        private System.Windows.Forms.Button btnTherapyLink;
        private System.Windows.Forms.Button btnDosageLink;
        private System.Windows.Forms.Button btnDurationLink;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.Label lblLeftBracket;
        private System.Windows.Forms.Label lblRightBracket;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.RadioButton thenRadioButton;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnReorderToPrevious;
        private System.Windows.Forms.Button btnReorderToNext;
    }
}
