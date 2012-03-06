namespace BDEditor.Views
{
    partial class BDSearchView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDSearchView));
            this.tbSearchTerm = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbDisease = new System.Windows.Forms.CheckBox();
            this.cbTherapy = new System.Windows.Forms.CheckBox();
            this.cbTherapyGroup = new System.Windows.Forms.CheckBox();
            this.cbPathogen = new System.Windows.Forms.CheckBox();
            this.cbPathogenGroup = new System.Windows.Forms.CheckBox();
            this.cbPresentation = new System.Windows.Forms.CheckBox();
            this.cbCategory = new System.Windows.Forms.CheckBox();
            this.cbSubcategory = new System.Windows.Forms.CheckBox();
            this.cbSection = new System.Windows.Forms.CheckBox();
            this.cbChapter = new System.Windows.Forms.CheckBox();
            this.rbNodes = new System.Windows.Forms.RadioButton();
            this.rbLinkedNotes = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSearchTerm
            // 
            this.tbSearchTerm.Location = new System.Drawing.Point(13, 13);
            this.tbSearchTerm.Name = "tbSearchTerm";
            this.tbSearchTerm.Size = new System.Drawing.Size(403, 20);
            this.tbSearchTerm.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(423, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.Location = new System.Drawing.Point(0, 168);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(515, 448);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbDisease);
            this.groupBox1.Controls.Add(this.cbTherapy);
            this.groupBox1.Controls.Add(this.cbTherapyGroup);
            this.groupBox1.Controls.Add(this.cbPathogen);
            this.groupBox1.Controls.Add(this.cbPathogenGroup);
            this.groupBox1.Controls.Add(this.cbPresentation);
            this.groupBox1.Controls.Add(this.cbCategory);
            this.groupBox1.Controls.Add(this.cbSubcategory);
            this.groupBox1.Controls.Add(this.cbSection);
            this.groupBox1.Controls.Add(this.cbChapter);
            this.groupBox1.Location = new System.Drawing.Point(13, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(485, 96);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search for:";
            // 
            // cbDisease
            // 
            this.cbDisease.AutoSize = true;
            this.cbDisease.Checked = true;
            this.cbDisease.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDisease.Location = new System.Drawing.Point(100, 42);
            this.cbDisease.Name = "cbDisease";
            this.cbDisease.Size = new System.Drawing.Size(64, 17);
            this.cbDisease.TabIndex = 9;
            this.cbDisease.Text = "Disease";
            this.cbDisease.UseVisualStyleBackColor = true;
            // 
            // cbTherapy
            // 
            this.cbTherapy.AutoSize = true;
            this.cbTherapy.Checked = true;
            this.cbTherapy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTherapy.Location = new System.Drawing.Point(319, 19);
            this.cbTherapy.Name = "cbTherapy";
            this.cbTherapy.Size = new System.Drawing.Size(65, 17);
            this.cbTherapy.TabIndex = 8;
            this.cbTherapy.Text = "Therapy";
            this.cbTherapy.UseVisualStyleBackColor = true;
            // 
            // cbTherapyGroup
            // 
            this.cbTherapyGroup.AutoSize = true;
            this.cbTherapyGroup.Checked = true;
            this.cbTherapyGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTherapyGroup.Location = new System.Drawing.Point(209, 65);
            this.cbTherapyGroup.Name = "cbTherapyGroup";
            this.cbTherapyGroup.Size = new System.Drawing.Size(97, 17);
            this.cbTherapyGroup.TabIndex = 7;
            this.cbTherapyGroup.Text = "Therapy Group";
            this.cbTherapyGroup.UseVisualStyleBackColor = true;
            // 
            // cbPathogen
            // 
            this.cbPathogen.AutoSize = true;
            this.cbPathogen.Checked = true;
            this.cbPathogen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPathogen.Location = new System.Drawing.Point(209, 42);
            this.cbPathogen.Name = "cbPathogen";
            this.cbPathogen.Size = new System.Drawing.Size(72, 17);
            this.cbPathogen.TabIndex = 6;
            this.cbPathogen.Text = "Pathogen";
            this.cbPathogen.UseVisualStyleBackColor = true;
            // 
            // cbPathogenGroup
            // 
            this.cbPathogenGroup.AutoSize = true;
            this.cbPathogenGroup.Checked = true;
            this.cbPathogenGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPathogenGroup.Location = new System.Drawing.Point(209, 19);
            this.cbPathogenGroup.Name = "cbPathogenGroup";
            this.cbPathogenGroup.Size = new System.Drawing.Size(104, 17);
            this.cbPathogenGroup.TabIndex = 5;
            this.cbPathogenGroup.Text = "Pathogen Group";
            this.cbPathogenGroup.UseVisualStyleBackColor = true;
            // 
            // cbPresentation
            // 
            this.cbPresentation.AutoSize = true;
            this.cbPresentation.Checked = true;
            this.cbPresentation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPresentation.Location = new System.Drawing.Point(100, 65);
            this.cbPresentation.Name = "cbPresentation";
            this.cbPresentation.Size = new System.Drawing.Size(85, 17);
            this.cbPresentation.TabIndex = 4;
            this.cbPresentation.Text = "Presentation";
            this.cbPresentation.UseVisualStyleBackColor = true;
            // 
            // cbCategory
            // 
            this.cbCategory.AutoSize = true;
            this.cbCategory.Checked = true;
            this.cbCategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCategory.Location = new System.Drawing.Point(7, 68);
            this.cbCategory.Name = "cbCategory";
            this.cbCategory.Size = new System.Drawing.Size(68, 17);
            this.cbCategory.TabIndex = 3;
            this.cbCategory.Text = "Category";
            this.cbCategory.UseVisualStyleBackColor = true;
            // 
            // cbSubcategory
            // 
            this.cbSubcategory.AutoSize = true;
            this.cbSubcategory.Checked = true;
            this.cbSubcategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSubcategory.Location = new System.Drawing.Point(100, 19);
            this.cbSubcategory.Name = "cbSubcategory";
            this.cbSubcategory.Size = new System.Drawing.Size(86, 17);
            this.cbSubcategory.TabIndex = 2;
            this.cbSubcategory.Text = "Subcategory";
            this.cbSubcategory.UseVisualStyleBackColor = true;
            // 
            // cbSection
            // 
            this.cbSection.AutoSize = true;
            this.cbSection.Checked = true;
            this.cbSection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSection.Location = new System.Drawing.Point(7, 44);
            this.cbSection.Name = "cbSection";
            this.cbSection.Size = new System.Drawing.Size(62, 17);
            this.cbSection.TabIndex = 1;
            this.cbSection.Text = "Section";
            this.cbSection.UseVisualStyleBackColor = true;
            // 
            // cbChapter
            // 
            this.cbChapter.AutoSize = true;
            this.cbChapter.Checked = true;
            this.cbChapter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbChapter.Location = new System.Drawing.Point(7, 20);
            this.cbChapter.Name = "cbChapter";
            this.cbChapter.Size = new System.Drawing.Size(63, 17);
            this.cbChapter.TabIndex = 0;
            this.cbChapter.Text = "Chapter";
            this.cbChapter.UseVisualStyleBackColor = true;
            // 
            // rbNodes
            // 
            this.rbNodes.AutoSize = true;
            this.rbNodes.Checked = true;
            this.rbNodes.Location = new System.Drawing.Point(13, 39);
            this.rbNodes.Name = "rbNodes";
            this.rbNodes.Size = new System.Drawing.Size(71, 17);
            this.rbNodes.TabIndex = 6;
            this.rbNodes.TabStop = true;
            this.rbNodes.Text = "All Entries";
            this.rbNodes.UseVisualStyleBackColor = true;
            this.rbNodes.CheckedChanged += new System.EventHandler(this.rbNodes_CheckedChanged);
            // 
            // rbLinkedNotes
            // 
            this.rbLinkedNotes.AutoSize = true;
            this.rbLinkedNotes.Location = new System.Drawing.Point(113, 39);
            this.rbLinkedNotes.Name = "rbLinkedNotes";
            this.rbLinkedNotes.Size = new System.Drawing.Size(53, 17);
            this.rbLinkedNotes.TabIndex = 7;
            this.rbLinkedNotes.Text = "Notes";
            this.rbLinkedNotes.UseVisualStyleBackColor = true;
            this.rbLinkedNotes.CheckedChanged += new System.EventHandler(this.rbLinkedNotes_CheckedChanged);
            // 
            // BDSearchView
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 616);
            this.Controls.Add(this.rbLinkedNotes);
            this.Controls.Add(this.rbNodes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbSearchTerm);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BDSearchView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bugs & Drugs Search";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSearchTerm;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbNodes;
        private System.Windows.Forms.RadioButton rbLinkedNotes;
        private System.Windows.Forms.CheckBox cbTherapy;
        private System.Windows.Forms.CheckBox cbTherapyGroup;
        private System.Windows.Forms.CheckBox cbPathogen;
        private System.Windows.Forms.CheckBox cbPathogenGroup;
        private System.Windows.Forms.CheckBox cbPresentation;
        private System.Windows.Forms.CheckBox cbCategory;
        private System.Windows.Forms.CheckBox cbSubcategory;
        private System.Windows.Forms.CheckBox cbSection;
        private System.Windows.Forms.CheckBox cbChapter;
        private System.Windows.Forms.CheckBox cbDisease;

    }
}