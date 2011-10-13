namespace BDEditor.Views
{
    partial class BDLinkedNoteControl
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
            this.textControl = new TXTextControl.TextControl();
            this.rulerBar = new TXTextControl.RulerBar();
            this.buttonBar = new TXTextControl.ButtonBar();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnBeta = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textControl
            // 
            this.textControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.textControl.Font = new System.Drawing.Font("Arial", 10F);
            this.textControl.IsSpellCheckingEnabled = true;
            this.textControl.Location = new System.Drawing.Point(3, 56);
            this.textControl.Name = "textControl";
            this.textControl.Padding = new System.Windows.Forms.Padding(3);
            this.textControl.Size = new System.Drawing.Size(801, 167);
            this.textControl.TabIndex = 0;
            this.textControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textControl_KeyUp);
            // 
            // rulerBar
            // 
            this.rulerBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.rulerBar.Location = new System.Drawing.Point(3, 31);
            this.rulerBar.Name = "rulerBar";
            this.rulerBar.Size = new System.Drawing.Size(801, 25);
            this.rulerBar.TabIndex = 2;
            this.rulerBar.Text = "rulerBar1";
            // 
            // buttonBar
            // 
            this.buttonBar.ButtonOffsets = new int[] {
        10,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        10,
        10,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0};
            this.buttonBar.ButtonPositions = new TXTextControl.Button[] {
        TXTextControl.Button.StyleComboBox,
        TXTextControl.Button.FontNameComboBox,
        TXTextControl.Button.FontSizeComboBox,
        TXTextControl.Button.ListBulletedButton,
        TXTextControl.Button.ListNumberedButton,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None,
        TXTextControl.Button.None};
            this.buttonBar.ButtonSeparators = new bool[] {
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        true,
        true,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false};
            this.buttonBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonBar.Location = new System.Drawing.Point(3, 3);
            this.buttonBar.Name = "buttonBar";
            this.buttonBar.Size = new System.Drawing.Size(801, 28);
            this.buttonBar.TabIndex = 4;
            this.buttonBar.Text = "buttonBar1";
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(724, 228);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 5;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(643, 228);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(75, 23);
            this.btnPaste.TabIndex = 6;
            this.btnPaste.Text = "Paste";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnBeta
            // 
            this.btnBeta.Location = new System.Drawing.Point(7, 230);
            this.btnBeta.Name = "btnBeta";
            this.btnBeta.Size = new System.Drawing.Size(30, 23);
            this.btnBeta.TabIndex = 7;
            this.btnBeta.Text = "ß";
            this.btnBeta.UseVisualStyleBackColor = true;
            this.btnBeta.Click += new System.EventHandler(this.btnBeta_Click);
            // 
            // BDLinkedNoteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnBeta);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.textControl);
            this.Controls.Add(this.rulerBar);
            this.Controls.Add(this.buttonBar);
            this.Name = "BDLinkedNoteControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(807, 258);
            this.Load += new System.EventHandler(this.BDLinkedNoteControl_Load);
            this.Leave += new System.EventHandler(this.BDLinkedNoteControl_Leave);
            this.ResumeLayout(false);

        }

        #endregion

        private TXTextControl.TextControl textControl;
        private TXTextControl.RulerBar rulerBar;
        private TXTextControl.ButtonBar buttonBar;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnBeta;

    }
}
