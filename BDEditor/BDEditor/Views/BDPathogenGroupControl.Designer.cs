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
            this.labelPathogenGroupName = new System.Windows.Forms.Label();
            this.panelTherapyGroups = new System.Windows.Forms.Panel();
            this.panelpathogenGroups = new System.Windows.Forms.Panel();
            this.btnPathogenGroupLink = new System.Windows.Forms.Button();
            this.labelPathogen = new System.Windows.Forms.Label();
            this.textBoxPathogenGroupName = new System.Windows.Forms.TextBox();
            this.btnMenu = new System.Windows.Forms.Button();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.movePreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveNeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addPathogenGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPathogenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTherapyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deletePathogenGroupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelPathogens = new System.Windows.Forms.Panel();
            this.contextMenuStripTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelpathogenGroups.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.contextMenuStripTextBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPathogenGroupName
            // 
            this.labelPathogenGroupName.AutoSize = true;
            this.labelPathogenGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPathogenGroupName.Location = new System.Drawing.Point(39, 4);
            this.labelPathogenGroupName.Name = "labelPathogenGroupName";
            this.labelPathogenGroupName.Size = new System.Drawing.Size(180, 18);
            this.labelPathogenGroupName.TabIndex = 1;
            this.labelPathogenGroupName.Text = "Pathogen Group Name";
            // 
            // panelTherapyGroups
            // 
            this.panelTherapyGroups.AutoScroll = true;
            this.panelTherapyGroups.AutoSize = true;
            this.panelTherapyGroups.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelTherapyGroups.BackColor = System.Drawing.SystemColors.Control;
            this.panelTherapyGroups.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTherapyGroups.Location = new System.Drawing.Point(0, 79);
            this.panelTherapyGroups.MinimumSize = new System.Drawing.Size(870, 5);
            this.panelTherapyGroups.Name = "panelTherapyGroups";
            this.panelTherapyGroups.Size = new System.Drawing.Size(870, 5);
            this.panelTherapyGroups.TabIndex = 5;
            // 
            // panelpathogenGroups
            // 
            this.panelpathogenGroups.Controls.Add(this.btnPathogenGroupLink);
            this.panelpathogenGroups.Controls.Add(this.labelPathogen);
            this.panelpathogenGroups.Controls.Add(this.textBoxPathogenGroupName);
            this.panelpathogenGroups.Controls.Add(this.btnMenu);
            this.panelpathogenGroups.Controls.Add(this.labelPathogenGroupName);
            this.panelpathogenGroups.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelpathogenGroups.Location = new System.Drawing.Point(0, 0);
            this.panelpathogenGroups.Name = "panelpathogenGroups";
            this.panelpathogenGroups.Size = new System.Drawing.Size(870, 74);
            this.panelpathogenGroups.TabIndex = 7;
            // 
            // btnPathogenGroupLink
            // 
            this.btnPathogenGroupLink.Image = global::BDEditor.Properties.Resources.link_16;
            this.btnPathogenGroupLink.Location = new System.Drawing.Point(334, 24);
            this.btnPathogenGroupLink.Name = "btnPathogenGroupLink";
            this.btnPathogenGroupLink.Size = new System.Drawing.Size(31, 25);
            this.btnPathogenGroupLink.TabIndex = 20;
            this.btnPathogenGroupLink.UseVisualStyleBackColor = true;
            this.btnPathogenGroupLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // labelPathogen
            // 
            this.labelPathogen.AutoSize = true;
            this.labelPathogen.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPathogen.Location = new System.Drawing.Point(42, 52);
            this.labelPathogen.Name = "labelPathogen";
            this.labelPathogen.Size = new System.Drawing.Size(79, 18);
            this.labelPathogen.TabIndex = 19;
            this.labelPathogen.Text = "Pathogen";
            // 
            // textBoxPathogenGroupName
            // 
            this.textBoxPathogenGroupName.ContextMenuStrip = this.contextMenuStripTextBox;
            this.textBoxPathogenGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPathogenGroupName.Location = new System.Drawing.Point(42, 25);
            this.textBoxPathogenGroupName.Name = "textBoxPathogenGroupName";
            this.textBoxPathogenGroupName.Size = new System.Drawing.Size(286, 24);
            this.textBoxPathogenGroupName.TabIndex = 18;
            this.textBoxPathogenGroupName.Leave += new System.EventHandler(this.textBoxPathogenGroupName_Leave);
            this.textBoxPathogenGroupName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxPathogenGroupName_MouseDown);
            // 
            // btnMenu
            // 
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(6, 23);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 17;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.movePreviousToolStripMenuItem,
            this.moveNeToolStripMenuItem,
            this.toolStripSeparator2,
            this.addPathogenGroupToolStripMenuItem,
            this.addPathogenToolStripMenuItem,
            this.addTherapyToolStripMenuItem,
            this.toolStripSeparator1,
            this.deletePathogenGroupMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(187, 148);
            // 
            // movePreviousToolStripMenuItem
            // 
            this.movePreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.previous_16;
            this.movePreviousToolStripMenuItem.Name = "movePreviousToolStripMenuItem";
            this.movePreviousToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.movePreviousToolStripMenuItem.Text = "Move &Previous";
            this.movePreviousToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // moveNeToolStripMenuItem
            // 
            this.moveNeToolStripMenuItem.Image = global::BDEditor.Properties.Resources.next_16;
            this.moveNeToolStripMenuItem.Name = "moveNeToolStripMenuItem";
            this.moveNeToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.moveNeToolStripMenuItem.Text = "Move &Next";
            this.moveNeToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToNext_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(183, 6);
            // 
            // addPathogenGroupToolStripMenuItem
            // 
            this.addPathogenGroupToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addPathogenGroupToolStripMenuItem.Name = "addPathogenGroupToolStripMenuItem";
            this.addPathogenGroupToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.addPathogenGroupToolStripMenuItem.Text = "Add Pathogen Group";
            this.addPathogenGroupToolStripMenuItem.Click += new System.EventHandler(this.PathogenGroup_RequestItemAdd);
            // 
            // addPathogenToolStripMenuItem
            // 
            this.addPathogenToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.addPathogenToolStripMenuItem.Name = "addPathogenToolStripMenuItem";
            this.addPathogenToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.addPathogenToolStripMenuItem.Text = "Add Pathogen";
            this.addPathogenToolStripMenuItem.Click += new System.EventHandler(this.Pathogen_RequestItemAdd);
            // 
            // addTherapyToolStripMenuItem
            // 
            this.addTherapyToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.addTherapyToolStripMenuItem.Name = "addTherapyToolStripMenuItem";
            this.addTherapyToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.addTherapyToolStripMenuItem.Text = "&Add Therapy Group";
            this.addTherapyToolStripMenuItem.Click += new System.EventHandler(this.TherapyGroup_RequestItemAdd);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // deletePathogenGroupMenuItem
            // 
            this.deletePathogenGroupMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deletePathogenGroupMenuItem.Name = "deletePathogenGroupMenuItem";
            this.deletePathogenGroupMenuItem.Size = new System.Drawing.Size(186, 22);
            this.deletePathogenGroupMenuItem.Text = "Delete";
            this.deletePathogenGroupMenuItem.Click += new System.EventHandler(this.PathogenGroup_RequestItemDelete);
            // 
            // panelPathogens
            // 
            this.panelPathogens.AutoSize = true;
            this.panelPathogens.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelPathogens.BackColor = System.Drawing.SystemColors.Control;
            this.panelPathogens.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPathogens.Location = new System.Drawing.Point(0, 74);
            this.panelPathogens.MinimumSize = new System.Drawing.Size(870, 5);
            this.panelPathogens.Name = "panelPathogens";
            this.panelPathogens.Size = new System.Drawing.Size(870, 5);
            this.panelPathogens.TabIndex = 20;
            // 
            // contextMenuStripTextBox
            // 
            this.contextMenuStripTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bToolStripMenuItem,
            this.geToolStripMenuItem,
            this.leToolStripMenuItem,
            this.plusMinusToolStripMenuItem,
            this.degreeToolStripMenuItem,
            this.µToolStripMenuItem,
            this.toolStripMenuItem3,
            this.undoToolStripMenuItem,
            this.toolStripMenuItem4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStripTextBox.Name = "contextMenuStrip1";
            this.contextMenuStripTextBox.ShowImageMargin = false;
            this.contextMenuStripTextBox.Size = new System.Drawing.Size(83, 258);
            this.contextMenuStripTextBox.Text = "Insert Symbol";
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.µToolStripMenuItem.Text = "µ";
            this.µToolStripMenuItem.Click += new System.EventHandler(this.µToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(124, 6);
            this.toolStripMenuItem3.Visible = false;
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Visible = false;
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(124, 6);
            this.toolStripMenuItem4.Visible = false;
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Visible = false;
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Visible = false;
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Visible = false;
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Visible = false;
            // 
            // BDPathogenGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panelTherapyGroups);
            this.Controls.Add(this.panelPathogens);
            this.Controls.Add(this.panelpathogenGroups);
            this.MinimumSize = new System.Drawing.Size(870, 110);
            this.Name = "BDPathogenGroupControl";
            this.Size = new System.Drawing.Size(870, 110);
            this.panelpathogenGroups.ResumeLayout(false);
            this.panelpathogenGroups.PerformLayout();
            this.contextMenuStripEvents.ResumeLayout(false);
            this.contextMenuStripTextBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPathogenGroupName;
        private System.Windows.Forms.Panel panelTherapyGroups;
        private System.Windows.Forms.Panel panelpathogenGroups;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ToolStripMenuItem addPathogenGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTherapyToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxPathogenGroupName;
        private System.Windows.Forms.Panel panelPathogens;
        private System.Windows.Forms.Label labelPathogen;
        private System.Windows.Forms.Button btnPathogenGroupLink;
        private System.Windows.Forms.ToolStripMenuItem addPathogenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deletePathogenGroupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem movePreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveNeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTextBox;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem µToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}
