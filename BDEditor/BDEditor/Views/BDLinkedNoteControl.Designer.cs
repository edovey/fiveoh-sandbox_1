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
            this.rtfLinkNoteText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtfLinkNoteText
            // 
            this.rtfLinkNoteText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfLinkNoteText.Location = new System.Drawing.Point(3, 3);
            this.rtfLinkNoteText.Name = "rtfLinkNoteText";
            this.rtfLinkNoteText.Size = new System.Drawing.Size(620, 86);
            this.rtfLinkNoteText.TabIndex = 0;
            this.rtfLinkNoteText.Text = "";
            // 
            // BDLinkedNoteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtfLinkNoteText);
            this.Name = "BDLinkedNoteControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(626, 92);
            this.Leave += new System.EventHandler(this.BDLinkedNoteControl_Leave);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtfLinkNoteText;

    }
}
