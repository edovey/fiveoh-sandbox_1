namespace BDEditor.Views
{
    partial class BDConfiguredEntryFieldOverviewControl
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
            this.bdLinkedNoteControl1 = new BDEditor.Views.BDLinkedNoteControl();
            this.SuspendLayout();
            // 
            // txtFieldData
            // 
            this.txtFieldData.Size = new System.Drawing.Size(148, 20);
            this.txtFieldData.Visible = false;
            // 
            // lblFieldLabel
            // 
            this.lblFieldLabel.Size = new System.Drawing.Size(650, 23);
            // 
            // btnLinkedNote
            // 
            this.btnLinkedNote.Location = new System.Drawing.Point(656, 27);
            // 
            // bdLinkedNoteControl1
            // 
            this.bdLinkedNoteControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bdLinkedNoteControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bdLinkedNoteControl1.CurrentLinkedNote = null;
            this.bdLinkedNoteControl1.DefaultLayoutVariantType = BDEditor.Classes.BDConstants.LayoutVariantType.Undefined;
            this.bdLinkedNoteControl1.DefaultNodeType = BDEditor.Classes.BDConstants.BDNodeType.None;
            this.bdLinkedNoteControl1.DisplayOrder = null;
            this.bdLinkedNoteControl1.Location = new System.Drawing.Point(0, 27);
            this.bdLinkedNoteControl1.Name = "bdLinkedNoteControl1";
            this.bdLinkedNoteControl1.Padding = new System.Windows.Forms.Padding(3);
            this.bdLinkedNoteControl1.SaveOnLeave = true;
            this.bdLinkedNoteControl1.ShowAsChild = false;
            this.bdLinkedNoteControl1.ShowChildren = true;
            this.bdLinkedNoteControl1.Size = new System.Drawing.Size(650, 146);
            this.bdLinkedNoteControl1.TabIndex = 38;
            // 
            // BDConfiguredEntryFieldOverviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bdLinkedNoteControl1);
            this.MinimumSize = new System.Drawing.Size(665, 180);
            this.Name = "BDConfiguredEntryFieldOverviewControl";
            this.Size = new System.Drawing.Size(688, 180);
            this.Controls.SetChildIndex(this.btnLinkedNote, 0);
            this.Controls.SetChildIndex(this.txtFieldData, 0);
            this.Controls.SetChildIndex(this.lblFieldLabel, 0);
            this.Controls.SetChildIndex(this.bdLinkedNoteControl1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BDLinkedNoteControl bdLinkedNoteControl1;
    }
}
