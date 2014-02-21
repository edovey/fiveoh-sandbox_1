namespace BDEditor.Views
{
    partial class BDRegimenGroupControl
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnMenu = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.regimenGroupName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.gbColumn = new System.Windows.Forms.GroupBox();
            this.cbAlternativeRegimen = new System.Windows.Forms.CheckBox();
            this.cbRegimenOfChoice = new System.Windows.Forms.CheckBox();
            this.gbJoin = new System.Windows.Forms.GroupBox();
            this.andRadioButton = new System.Windows.Forms.RadioButton();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            this.orRadioButton = new System.Windows.Forms.RadioButton();
            this.btnRegimenGroupLink = new System.Windows.Forms.Button();
            this.panelRegimens = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plusMinusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.µToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editIndexStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reorderPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorderNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addRegimenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRegimenToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnRegimenGroupConjunctionLink = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
            this.gbColumn.SuspendLayout();
            this.gbJoin.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStripEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.AutoSize = true;
            this.panelHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelHeader.Controls.Add(this.btnRegimenGroupConjunctionLink);
            this.panelHeader.Controls.Add(this.btnMenu);
            this.panelHeader.Controls.Add(this.lblInfo);
            this.panelHeader.Controls.Add(this.label1);
            this.panelHeader.Controls.Add(this.regimenGroupName);
            this.panelHeader.Controls.Add(this.tbName);
            this.panelHeader.Controls.Add(this.gbColumn);
            this.panelHeader.Controls.Add(this.gbJoin);
            this.panelHeader.Controls.Add(this.btnRegimenGroupLink);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(860, 85);
            this.panelHeader.TabIndex = 24;
            // 
            // btnMenu
            // 
            this.btnMenu.Image = global::BDEditor.Properties.Resources.apps_16;
            this.btnMenu.Location = new System.Drawing.Point(3, 4);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(28, 28);
            this.btnMenu.TabIndex = 25;
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(438, 16);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(32, 13);
            this.lblInfo.TabIndex = 33;
            this.lblInfo.Text = "INFO";
            this.lblInfo.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 31;
            this.label1.Text = "Name";
            // 
            // regimenGroupName
            // 
            this.regimenGroupName.AutoSize = true;
            this.regimenGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regimenGroupName.Location = new System.Drawing.Point(39, 4);
            this.regimenGroupName.Name = "regimenGroupName";
            this.regimenGroupName.Size = new System.Drawing.Size(126, 18);
            this.regimenGroupName.TabIndex = 6;
            this.regimenGroupName.Text = "Regimen Group";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(42, 52);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(394, 20);
            this.tbName.TabIndex = 7;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            this.tbName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbName_MouseDown);
            // 
            // gbColumn
            // 
            this.gbColumn.Controls.Add(this.cbAlternativeRegimen);
            this.gbColumn.Controls.Add(this.cbRegimenOfChoice);
            this.gbColumn.Location = new System.Drawing.Point(476, 1);
            this.gbColumn.Name = "gbColumn";
            this.gbColumn.Size = new System.Drawing.Size(315, 43);
            this.gbColumn.TabIndex = 29;
            this.gbColumn.TabStop = false;
            // 
            // cbAlternativeRegimen
            // 
            this.cbAlternativeRegimen.AutoSize = true;
            this.cbAlternativeRegimen.Location = new System.Drawing.Point(167, 15);
            this.cbAlternativeRegimen.Name = "cbAlternativeRegimen";
            this.cbAlternativeRegimen.Size = new System.Drawing.Size(121, 17);
            this.cbAlternativeRegimen.TabIndex = 30;
            this.cbAlternativeRegimen.Text = "Alternative Regimen";
            this.cbAlternativeRegimen.UseVisualStyleBackColor = true;
            // 
            // cbRegimenOfChoice
            // 
            this.cbRegimenOfChoice.AutoSize = true;
            this.cbRegimenOfChoice.Location = new System.Drawing.Point(29, 15);
            this.cbRegimenOfChoice.Name = "cbRegimenOfChoice";
            this.cbRegimenOfChoice.Size = new System.Drawing.Size(116, 17);
            this.cbRegimenOfChoice.TabIndex = 29;
            this.cbRegimenOfChoice.Text = "Regimen of Choice";
            this.cbRegimenOfChoice.UseVisualStyleBackColor = true;
            // 
            // gbJoin
            // 
            this.gbJoin.Controls.Add(this.andRadioButton);
            this.gbJoin.Controls.Add(this.noneRadioButton);
            this.gbJoin.Controls.Add(this.orRadioButton);
            this.gbJoin.Location = new System.Drawing.Point(476, 37);
            this.gbJoin.Name = "gbJoin";
            this.gbJoin.Size = new System.Drawing.Size(315, 45);
            this.gbJoin.TabIndex = 30;
            this.gbJoin.TabStop = false;
            // 
            // andRadioButton
            // 
            this.andRadioButton.AutoSize = true;
            this.andRadioButton.Location = new System.Drawing.Point(127, 18);
            this.andRadioButton.Name = "andRadioButton";
            this.andRadioButton.Size = new System.Drawing.Size(95, 17);
            this.andRadioButton.TabIndex = 13;
            this.andRadioButton.TabStop = true;
            this.andRadioButton.Text = "And (with next)";
            this.andRadioButton.UseVisualStyleBackColor = true;
            // 
            // noneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(6, 18);
            this.noneRadioButton.Name = "noneRadioButton";
            this.noneRadioButton.Size = new System.Drawing.Size(124, 17);
            this.noneRadioButton.TabIndex = 12;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "Next Regimen Group";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // orRadioButton
            // 
            this.orRadioButton.AutoSize = true;
            this.orRadioButton.Location = new System.Drawing.Point(226, 18);
            this.orRadioButton.Name = "orRadioButton";
            this.orRadioButton.Size = new System.Drawing.Size(87, 17);
            this.orRadioButton.TabIndex = 14;
            this.orRadioButton.TabStop = true;
            this.orRadioButton.Text = "Or (with next)";
            this.orRadioButton.UseVisualStyleBackColor = true;
            // 
            // btnRegimenGroupLink
            // 
            this.btnRegimenGroupLink.Image = global::BDEditor.Properties.Resources.link_16;
            this.btnRegimenGroupLink.Location = new System.Drawing.Point(442, 47);
            this.btnRegimenGroupLink.Name = "btnRegimenGroupLink";
            this.btnRegimenGroupLink.Size = new System.Drawing.Size(28, 28);
            this.btnRegimenGroupLink.TabIndex = 19;
            this.btnRegimenGroupLink.UseVisualStyleBackColor = false;
            this.btnRegimenGroupLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // panelRegimens
            // 
            this.panelRegimens.AutoScroll = true;
            this.panelRegimens.AutoSize = true;
            this.panelRegimens.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelRegimens.BackColor = System.Drawing.SystemColors.Control;
            this.panelRegimens.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRegimens.Location = new System.Drawing.Point(0, 85);
            this.panelRegimens.MinimumSize = new System.Drawing.Size(0, 5);
            this.panelRegimens.Name = "panelRegimens";
            this.panelRegimens.Size = new System.Drawing.Size(860, 5);
            this.panelRegimens.TabIndex = 27;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bToolStripMenuItem,
            this.geToolStripMenuItem,
            this.leToolStripMenuItem,
            this.plusMinusToolStripMenuItem,
            this.degreeToolStripMenuItem,
            this.µToolStripMenuItem,
            this.sOneToolStripMenuItem,
            this.toolStripMenuItem3,
            this.undoToolStripMenuItem,
            this.toolStripMenuItem4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(83, 280);
            this.contextMenuStrip1.Text = "Insert Symbol";
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.bToolStripMenuItem.Text = "ß";
            this.bToolStripMenuItem.ToolTipText = "Insert ß";
            this.bToolStripMenuItem.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // geToolStripMenuItem
            // 
            this.geToolStripMenuItem.Name = "geToolStripMenuItem";
            this.geToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.geToolStripMenuItem.Text = "≥";
            this.geToolStripMenuItem.ToolTipText = "Insert ≥";
            this.geToolStripMenuItem.Click += new System.EventHandler(this.geToolStripMenuItem_Click);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.leToolStripMenuItem.Text = "≤";
            this.leToolStripMenuItem.ToolTipText = "Insert ≤";
            this.leToolStripMenuItem.Click += new System.EventHandler(this.leToolStripMenuItem_Click);
            // 
            // plusMinusToolStripMenuItem
            // 
            this.plusMinusToolStripMenuItem.Name = "plusMinusToolStripMenuItem";
            this.plusMinusToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.plusMinusToolStripMenuItem.Text = "±";
            this.plusMinusToolStripMenuItem.ToolTipText = "Insert ±";
            this.plusMinusToolStripMenuItem.Click += new System.EventHandler(this.plusMinusToolStripMenuItem_Click);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.degreeToolStripMenuItem.Text = "°";
            this.degreeToolStripMenuItem.ToolTipText = "Insert °";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.degreeToolStripMenuItem_Click);
            // 
            // µToolStripMenuItem
            // 
            this.µToolStripMenuItem.Name = "µToolStripMenuItem";
            this.µToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.µToolStripMenuItem.Text = "µ";
            this.µToolStripMenuItem.Click += new System.EventHandler(this.µToolStripMenuItem_Click);
            // 
            // sOneToolStripMenuItem
            // 
            this.sOneToolStripMenuItem.Name = "sOneToolStripMenuItem";
            this.sOneToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.sOneToolStripMenuItem.Text = "¹";
            this.sOneToolStripMenuItem.ToolTipText = "Insert ¹";
            this.sOneToolStripMenuItem.Click += new System.EventHandler(this.sOneToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(79, 6);
            this.toolStripMenuItem3.Visible = false;
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Visible = false;
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.Menu_Undo);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(79, 6);
            this.toolStripMenuItem4.Visible = false;
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Visible = false;
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.Menu_Cut);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Visible = false;
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.Menu_Copy);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Visible = false;
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.Menu_Paste);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(82, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Visible = false;
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.Menu_Delete);
            // 
            // contextMenuStripEvents
            // 
            this.contextMenuStripEvents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editIndexStripMenuItem,
            this.toolStripSeparator1,
            this.reorderPreviousToolStripMenuItem,
            this.reorderNextToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addRegimenToolStripMenuItem,
            this.addRegimenToolStripMenuItem1,
            this.toolStripMenuItem2,
            this.deleteToolStripMenuItem});
            this.contextMenuStripEvents.Name = "contextMenuStripEvents";
            this.contextMenuStripEvents.Size = new System.Drawing.Size(183, 176);
            // 
            // editIndexStripMenuItem
            // 
            this.editIndexStripMenuItem.Image = global::BDEditor.Properties.Resources.edit_24x24;
            this.editIndexStripMenuItem.Name = "editIndexStripMenuItem";
            this.editIndexStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.editIndexStripMenuItem.Text = "&Edit Index Entries";
            this.editIndexStripMenuItem.Click += new System.EventHandler(this.editIndexStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // reorderPreviousToolStripMenuItem
            // 
            this.reorderPreviousToolStripMenuItem.Image = global::BDEditor.Properties.Resources.reorder_previous;
            this.reorderPreviousToolStripMenuItem.Name = "reorderPreviousToolStripMenuItem";
            this.reorderPreviousToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.reorderPreviousToolStripMenuItem.Text = "Move &Previous";
            this.reorderPreviousToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToPrevious_Click);
            // 
            // reorderNextToolStripMenuItem
            // 
            this.reorderNextToolStripMenuItem.Image = global::BDEditor.Properties.Resources.reorder_next;
            this.reorderNextToolStripMenuItem.Name = "reorderNextToolStripMenuItem";
            this.reorderNextToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.reorderNextToolStripMenuItem.Text = "Move &Next";
            this.reorderNextToolStripMenuItem.Click += new System.EventHandler(this.btnReorderToNext_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(179, 6);
            // 
            // addRegimenToolStripMenuItem
            // 
            this.addRegimenToolStripMenuItem.Image = global::BDEditor.Properties.Resources.add_16x16;
            this.addRegimenToolStripMenuItem.Name = "addRegimenToolStripMenuItem";
            this.addRegimenToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.addRegimenToolStripMenuItem.Text = "&Add Regimen Group";
            this.addRegimenToolStripMenuItem.Click += new System.EventHandler(this.RegimenGroup_RequestItemAdd);
            // 
            // addRegimenToolStripMenuItem1
            // 
            this.addRegimenToolStripMenuItem1.Image = global::BDEditor.Properties.Resources.add_record_16;
            this.addRegimenToolStripMenuItem1.Name = "addRegimenToolStripMenuItem1";
            this.addRegimenToolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
            this.addRegimenToolStripMenuItem1.Text = "Add Regimen";
            this.addRegimenToolStripMenuItem1.Click += new System.EventHandler(this.Regimen_RequestItemAdd);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(179, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BDEditor.Properties.Resources.remove;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.RegimenGroup_RequestItemDelete);
            // 
            // btnRegimenGroupConjunctionLink
            // 
            this.btnRegimenGroupConjunctionLink.Image = global::BDEditor.Properties.Resources.link_16;
            this.btnRegimenGroupConjunctionLink.Location = new System.Drawing.Point(797, 49);
            this.btnRegimenGroupConjunctionLink.Name = "btnRegimenGroupConjunctionLink";
            this.btnRegimenGroupConjunctionLink.Size = new System.Drawing.Size(28, 28);
            this.btnRegimenGroupConjunctionLink.TabIndex = 34;
            this.btnRegimenGroupConjunctionLink.UseVisualStyleBackColor = false;
            // 
            // BDRegimenGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panelRegimens);
            this.Controls.Add(this.panelHeader);
            this.MinimumSize = new System.Drawing.Size(860, 100);
            this.Name = "BDRegimenGroupControl";
            this.Size = new System.Drawing.Size(860, 100);
            this.Load += new System.EventHandler(this.BDRegimenGroupControl_Load);
            this.Leave += new System.EventHandler(this.BDRegimenGroupControl_Leave);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.gbColumn.ResumeLayout(false);
            this.gbColumn.PerformLayout();
            this.gbJoin.ResumeLayout(false);
            this.gbJoin.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStripEvents.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Label regimenGroupName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.RadioButton noneRadioButton;
        private System.Windows.Forms.RadioButton andRadioButton;
        private System.Windows.Forms.Button btnRegimenGroupLink;
        private System.Windows.Forms.RadioButton orRadioButton;
        private System.Windows.Forms.Panel panelRegimens;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plusMinusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem µToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEvents;
        private System.Windows.Forms.ToolStripMenuItem editIndexStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem reorderPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorderNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addRegimenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRegimenToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox gbColumn;
        private System.Windows.Forms.GroupBox gbJoin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.CheckBox cbAlternativeRegimen;
        private System.Windows.Forms.CheckBox cbRegimenOfChoice;
        private System.Windows.Forms.Button btnRegimenGroupConjunctionLink;
    }
}
