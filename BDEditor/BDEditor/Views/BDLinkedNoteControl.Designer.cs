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
            this.components = new System.ComponentModel.Container();
            this.textControl = new TXTextControl.TextControl();
            this.buttonBar = new TXTextControl.ButtonBar();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnBeta = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnGE = new System.Windows.Forms.Button();
            this.btnLE = new System.Windows.Forms.Button();
            this.btnPM = new System.Windows.Forms.Button();
            this.btnDegree = new System.Windows.Forms.Button();
            this.btnSuperscript = new System.Windows.Forms.Button();
            this.btnSubscript = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textControl
            // 
            this.textControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.textControl.Font = new System.Drawing.Font("Arial", 10F);
            this.textControl.IsSpellCheckingEnabled = true;
            this.textControl.Location = new System.Drawing.Point(3, 31);
            this.textControl.Name = "textControl";
            this.textControl.Padding = new System.Windows.Forms.Padding(3);
            this.textControl.Size = new System.Drawing.Size(801, 167);
            this.textControl.TabIndex = 0;
            this.textControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textControl_KeyUp);
            // 
            // buttonBar
            // 
            this.buttonBar.ButtonOffsets = new int[] {
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
        TXTextControl.Button.ListBulletedButton,
        TXTextControl.Button.ListNumberedButton,
        TXTextControl.Button.FontUnderlineButton,
        TXTextControl.Button.FontBoldButton,
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
        TXTextControl.Button.None,
        TXTextControl.Button.None};
            this.buttonBar.ButtonSeparators = new bool[] {
        true,
        true,
        true,
        true,
        true,
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
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(713, 205);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(75, 23);
            this.btnPaste.TabIndex = 6;
            this.btnPaste.Text = "Paste";
            this.toolTip1.SetToolTip(this.btnPaste, "Paste from Clipboard");
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnBeta
            // 
            this.btnBeta.Location = new System.Drawing.Point(7, 203);
            this.btnBeta.Name = "btnBeta";
            this.btnBeta.Size = new System.Drawing.Size(30, 27);
            this.btnBeta.TabIndex = 7;
            this.btnBeta.Text = "ß";
            this.toolTip1.SetToolTip(this.btnBeta, "Insert ß at cursor");
            this.btnBeta.UseVisualStyleBackColor = true;
            this.btnBeta.Click += new System.EventHandler(this.btnBeta_Click);
            // 
            // btnGE
            // 
            this.btnGE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGE.Location = new System.Drawing.Point(43, 201);
            this.btnGE.Name = "btnGE";
            this.btnGE.Size = new System.Drawing.Size(30, 29);
            this.btnGE.TabIndex = 8;
            this.btnGE.Text = "≥";
            this.toolTip1.SetToolTip(this.btnGE, "Insert ≥ at cursor");
            this.btnGE.UseVisualStyleBackColor = true;
            this.btnGE.Click += new System.EventHandler(this.btnGE_Click);
            // 
            // btnLE
            // 
            this.btnLE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLE.Location = new System.Drawing.Point(79, 201);
            this.btnLE.Name = "btnLE";
            this.btnLE.Size = new System.Drawing.Size(30, 29);
            this.btnLE.TabIndex = 9;
            this.btnLE.Text = "≤";
            this.toolTip1.SetToolTip(this.btnLE, "Insert ≤ at cursor");
            this.btnLE.UseVisualStyleBackColor = true;
            this.btnLE.Click += new System.EventHandler(this.btnLE_Click);
            // 
            // btnPM
            // 
            this.btnPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPM.Location = new System.Drawing.Point(115, 201);
            this.btnPM.Name = "btnPM";
            this.btnPM.Size = new System.Drawing.Size(30, 29);
            this.btnPM.TabIndex = 10;
            this.btnPM.Text = "±";
            this.toolTip1.SetToolTip(this.btnPM, "Insert ± at cursor");
            this.btnPM.UseVisualStyleBackColor = true;
            this.btnPM.Click += new System.EventHandler(this.btnPM_Click);
            // 
            // btnDegree
            // 
            this.btnDegree.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDegree.Location = new System.Drawing.Point(151, 201);
            this.btnDegree.Name = "btnDegree";
            this.btnDegree.Size = new System.Drawing.Size(30, 27);
            this.btnDegree.TabIndex = 11;
            this.btnDegree.Text = "°";
            this.toolTip1.SetToolTip(this.btnDegree, "Insert ° at cursor");
            this.btnDegree.UseVisualStyleBackColor = true;
            this.btnDegree.Click += new System.EventHandler(this.btnDegree_Click);
            // 
            // btnSuperscript
            // 
            this.btnSuperscript.Enabled = false;
            this.btnSuperscript.Location = new System.Drawing.Point(269, 202);
            this.btnSuperscript.Name = "btnSuperscript";
            this.btnSuperscript.Size = new System.Drawing.Size(82, 27);
            this.btnSuperscript.TabIndex = 12;
            this.btnSuperscript.Text = "Superscript";
            this.toolTip1.SetToolTip(this.btnSuperscript, "Superscript selection");
            this.btnSuperscript.UseVisualStyleBackColor = true;
            this.btnSuperscript.Click += new System.EventHandler(this.btnSuperscript_Click);
            // 
            // btnSubscript
            // 
            this.btnSubscript.Enabled = false;
            this.btnSubscript.Location = new System.Drawing.Point(357, 202);
            this.btnSubscript.Name = "btnSubscript";
            this.btnSubscript.Size = new System.Drawing.Size(82, 27);
            this.btnSubscript.TabIndex = 13;
            this.btnSubscript.Text = "Subscript";
            this.toolTip1.SetToolTip(this.btnSubscript, "Subscript selected text");
            this.btnSubscript.UseVisualStyleBackColor = true;
            this.btnSubscript.Click += new System.EventHandler(this.btnSubscript_Click);
            // 
            // BDLinkedNoteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSubscript);
            this.Controls.Add(this.btnSuperscript);
            this.Controls.Add(this.btnDegree);
            this.Controls.Add(this.btnPM);
            this.Controls.Add(this.btnLE);
            this.Controls.Add(this.btnGE);
            this.Controls.Add(this.btnBeta);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.textControl);
            this.Controls.Add(this.buttonBar);
            this.Name = "BDLinkedNoteControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(807, 230);
            this.Load += new System.EventHandler(this.BDLinkedNoteControl_Load);
            this.Leave += new System.EventHandler(this.BDLinkedNoteControl_Leave);
            this.ResumeLayout(false);

        }

        #endregion

        private TXTextControl.TextControl textControl;
        private TXTextControl.ButtonBar buttonBar;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnBeta;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnGE;
        private System.Windows.Forms.Button btnLE;
        private System.Windows.Forms.Button btnPM;
        private System.Windows.Forms.Button btnDegree;
        private System.Windows.Forms.Button btnSuperscript;
        private System.Windows.Forms.Button btnSubscript;

    }
}
