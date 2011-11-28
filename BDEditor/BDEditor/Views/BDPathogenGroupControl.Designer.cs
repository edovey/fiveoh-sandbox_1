namespace BDEditor.Views
{
    partial class BDPathogenGroupControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.panelTherapyGroups = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnMenu = new System.Windows.Forms.Button();
            this.addTherapyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPathogenGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathogenSet1 = new BDEditor.Views.PathogenSet();
            this.panel1.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(-1, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Pathogens";
            // 
            // panelTherapyGroups
            // 
            this.panelTherapyGroups.AutoScroll = true;
            this.panelTherapyGroups.AutoSize = true;
            this.panelTherapyGroups.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelTherapyGroups.BackColor = System.Drawing.SystemColors.Control;
            this.panelTherapyGroups.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTherapyGroups.Location = new System.Drawing.Point(0, 102);
            this.panelTherapyGroups.MinimumSize = new System.Drawing.Size(870, 5);
            this.panelTherapyGroups.Name = "panelTherapyGroups";
            this.panelTherapyGroups.Size = new System.Drawing.Size(870, 5);
            this.panelTherapyGroups.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMenu);
            this.panel1.Controls.Add(this.pathogenSet1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(870, 102);
            this.panel1.TabIndex = 7;
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPathogenGroupToolStripMenuItem,
            this.addTherapyToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(186, 48);
            // 
            // btnMenu
            // 
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(789, 61);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 17;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // addTherapyToolStripMenuItem
            // 
            this.addTherapyToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.addTherapyToolStripMenuItem.Name = "addTherapyToolStripMenuItem";
            this.addTherapyToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.addTherapyToolStripMenuItem.Text = "&Add Therapy Group";
            this.addTherapyToolStripMenuItem.Click += new System.EventHandler(this.TherapyGroup_RequestItemAdd);
            // 
            // addPathogenGroupToolStripMenuItem
            // 
            this.addPathogenGroupToolStripMenuItem.Enabled = false;
            this.addPathogenGroupToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addPathogenGroupToolStripMenuItem.Name = "addPathogenGroupToolStripMenuItem";
            this.addPathogenGroupToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.addPathogenGroupToolStripMenuItem.Text = "Add Pathogen Group";
            this.addPathogenGroupToolStripMenuItem.Click += new System.EventHandler(this.PathogenGroup_RequestItemAdd);
            // 
            // pathogenSet1
            // 
            this.pathogenSet1.AutoScroll = true;
            this.pathogenSet1.CurrentPathogenGroup = null;
            this.pathogenSet1.Location = new System.Drawing.Point(3, 25);
            this.pathogenSet1.Name = "pathogenSet1";
            this.pathogenSet1.Size = new System.Drawing.Size(308, 64);
            this.pathogenSet1.TabIndex = 2;
            // 
            // BDPathogenGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panelTherapyGroups);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(870, 110);
            this.Name = "BDPathogenGroupControl";
            this.Size = new System.Drawing.Size(870, 110);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStripEvents.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private PathogenSet pathogenSet1;
        private System.Windows.Forms.Panel panelTherapyGroups;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ToolStripMenuItem addPathogenGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTherapyToolStripMenuItem;
    }
}
