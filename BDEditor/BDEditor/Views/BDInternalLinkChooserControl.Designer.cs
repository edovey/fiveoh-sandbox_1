namespace BDEditor.Views
{
    partial class BDInternalLinkChooserControl
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
            this.tree = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboTopLevel = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tree
            // 
            this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tree.Location = new System.Drawing.Point(0, 31);
            this.tree.Name = "tree";
            this.tree.Size = new System.Drawing.Size(401, 170);
            this.tree.TabIndex = 0;
            this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cboTopLevel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(401, 31);
            this.panel1.TabIndex = 1;
            // 
            // cboTopLevel
            // 
            this.cboTopLevel.FormattingEnabled = true;
            this.cboTopLevel.Location = new System.Drawing.Point(5, 5);
            this.cboTopLevel.Name = "cboTopLevel";
            this.cboTopLevel.Size = new System.Drawing.Size(300, 21);
            this.cboTopLevel.TabIndex = 0;
            this.cboTopLevel.SelectedIndexChanged += new System.EventHandler(this.cboTopLevel_SelectedIndexChanged);
            // 
            // BDInternalLinkChooserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tree);
            this.Controls.Add(this.panel1);
            this.Name = "BDInternalLinkChooserControl";
            this.Size = new System.Drawing.Size(401, 201);
            this.Load += new System.EventHandler(this.BDInternalLinkChooserControl_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tree;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cboTopLevel;
    }
}
