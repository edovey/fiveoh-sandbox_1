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
            this.lblLinkedNote = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tbLinkedNote = new System.Windows.Forms.TextBox();
            this.lblEndNote = new System.Windows.Forms.Label();
            this.tbEndNote = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblLinkedNote
            // 
            this.lblLinkedNote.AutoSize = true;
            this.lblLinkedNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLinkedNote.Location = new System.Drawing.Point(13, 14);
            this.lblLinkedNote.Name = "lblLinkedNote";
            this.lblLinkedNote.Size = new System.Drawing.Size(65, 13);
            this.lblLinkedNote.TabIndex = 0;
            this.lblLinkedNote.Text = "Linked Note";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(90, 10);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(169, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // tbLinkedNote
            // 
            this.tbLinkedNote.Location = new System.Drawing.Point(16, 35);
            this.tbLinkedNote.Multiline = true;
            this.tbLinkedNote.Name = "tbLinkedNote";
            this.tbLinkedNote.Size = new System.Drawing.Size(743, 72);
            this.tbLinkedNote.TabIndex = 2;
            // 
            // lblEndNote
            // 
            this.lblEndNote.AutoSize = true;
            this.lblEndNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEndNote.Location = new System.Drawing.Point(13, 117);
            this.lblEndNote.Name = "lblEndNote";
            this.lblEndNote.Size = new System.Drawing.Size(52, 13);
            this.lblEndNote.TabIndex = 3;
            this.lblEndNote.Text = "End Note";
            // 
            // tbEndNote
            // 
            this.tbEndNote.Location = new System.Drawing.Point(16, 136);
            this.tbEndNote.Multiline = true;
            this.tbEndNote.Name = "tbEndNote";
            this.tbEndNote.Size = new System.Drawing.Size(743, 41);
            this.tbEndNote.TabIndex = 4;
            // 
            // BDLinkedNoteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbEndNote);
            this.Controls.Add(this.lblEndNote);
            this.Controls.Add(this.tbLinkedNote);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.lblLinkedNote);
            this.Name = "BDLinkedNoteControl";
            this.Size = new System.Drawing.Size(764, 182);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLinkedNote;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox tbLinkedNote;
        private System.Windows.Forms.Label lblEndNote;
        private System.Windows.Forms.TextBox tbEndNote;
    }
}
