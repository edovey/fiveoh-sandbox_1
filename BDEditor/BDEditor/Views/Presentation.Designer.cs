namespace BDEditor
{
    partial class Presentation
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
            this.components = new System.ComponentModel.Container();
            this.rtbOverview = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPathogen1 = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.entitiesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnGet = new System.Windows.Forms.Button();
            this.btnAddSection = new System.Windows.Forms.Button();
            this.tbSectionName = new System.Windows.Forms.TextBox();
            this.btnSaveSection = new System.Windows.Forms.Button();
            this.cbSection = new System.Windows.Forms.ComboBox();
            this.tbCategoryName = new System.Windows.Forms.TextBox();
            this.btnNewCategory = new System.Windows.Forms.Button();
            this.btnSaveCategory = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbOverview
            // 
            this.rtbOverview.Location = new System.Drawing.Point(13, 54);
            this.rtbOverview.Name = "rtbOverview";
            this.rtbOverview.Size = new System.Drawing.Size(1053, 126);
            this.rtbOverview.TabIndex = 0;
            this.rtbOverview.Text = "";
            this.rtbOverview.TextChanged += new System.EventHandler(this.rtbOverview_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Overview";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Pathogens";
            // 
            // tbPathogen1
            // 
            this.tbPathogen1.Location = new System.Drawing.Point(16, 225);
            this.tbPathogen1.Name = "tbPathogen1";
            this.tbPathogen1.Size = new System.Drawing.Size(280, 20);
            this.tbPathogen1.TabIndex = 3;
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(16, 13);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(100, 20);
            this.tbName.TabIndex = 4;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1013, 491);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(981, 225);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(246, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // entitiesBindingSource
            // 
            this.entitiesBindingSource.DataSource = typeof(BDEditor.DataModel.Entities);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(315, 222);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(75, 23);
            this.btnGet.TabIndex = 8;
            this.btnGet.Text = "Get By Key";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // btnAddSection
            // 
            this.btnAddSection.Location = new System.Drawing.Point(246, 313);
            this.btnAddSection.Name = "btnAddSection";
            this.btnAddSection.Size = new System.Drawing.Size(125, 25);
            this.btnAddSection.TabIndex = 9;
            this.btnAddSection.Text = "New Section";
            this.btnAddSection.UseVisualStyleBackColor = true;
            this.btnAddSection.Click += new System.EventHandler(this.btnAddSection_Click);
            // 
            // tbSectionName
            // 
            this.tbSectionName.Location = new System.Drawing.Point(28, 313);
            this.tbSectionName.Name = "tbSectionName";
            this.tbSectionName.Size = new System.Drawing.Size(195, 20);
            this.tbSectionName.TabIndex = 10;
            // 
            // btnSaveSection
            // 
            this.btnSaveSection.Location = new System.Drawing.Point(409, 314);
            this.btnSaveSection.Name = "btnSaveSection";
            this.btnSaveSection.Size = new System.Drawing.Size(125, 25);
            this.btnSaveSection.TabIndex = 11;
            this.btnSaveSection.Text = "Save Section";
            this.btnSaveSection.UseVisualStyleBackColor = true;
            this.btnSaveSection.Click += new System.EventHandler(this.btnSaveSection_Click);
            // 
            // cbSection
            // 
            this.cbSection.FormattingEnabled = true;
            this.cbSection.Location = new System.Drawing.Point(28, 364);
            this.cbSection.Name = "cbSection";
            this.cbSection.Size = new System.Drawing.Size(195, 21);
            this.cbSection.TabIndex = 12;
            this.cbSection.SelectedIndexChanged += new System.EventHandler(this.cbSection_SelectedIndexChanged);
            // 
            // tbCategoryName
            // 
            this.tbCategoryName.Location = new System.Drawing.Point(246, 364);
            this.tbCategoryName.Name = "tbCategoryName";
            this.tbCategoryName.Size = new System.Drawing.Size(144, 20);
            this.tbCategoryName.TabIndex = 13;
            this.tbCategoryName.TextChanged += new System.EventHandler(this.tbCategoryName_TextChanged);
            // 
            // btnNewCategory
            // 
            this.btnNewCategory.Location = new System.Drawing.Point(409, 364);
            this.btnNewCategory.Name = "btnNewCategory";
            this.btnNewCategory.Size = new System.Drawing.Size(125, 23);
            this.btnNewCategory.TabIndex = 14;
            this.btnNewCategory.Text = "New Category";
            this.btnNewCategory.UseVisualStyleBackColor = true;
            this.btnNewCategory.Click += new System.EventHandler(this.btnNewCategory_Click);
            // 
            // btnSaveCategory
            // 
            this.btnSaveCategory.Location = new System.Drawing.Point(558, 360);
            this.btnSaveCategory.Name = "btnSaveCategory";
            this.btnSaveCategory.Size = new System.Drawing.Size(123, 23);
            this.btnSaveCategory.TabIndex = 15;
            this.btnSaveCategory.Text = "Save Category";
            this.btnSaveCategory.UseVisualStyleBackColor = true;
            this.btnSaveCategory.Click += new System.EventHandler(this.btnSaveCategory_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 415);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Existing Categories:";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(134, 415);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(120, 95);
            this.listBox1.TabIndex = 17;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(763, 313);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(209, 145);
            this.button1.TabIndex = 18;
            this.button1.Text = "go baby";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Presentation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 526);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSaveCategory);
            this.Controls.Add(this.btnNewCategory);
            this.Controls.Add(this.tbCategoryName);
            this.Controls.Add(this.cbSection);
            this.Controls.Add(this.btnSaveSection);
            this.Controls.Add(this.tbSectionName);
            this.Controls.Add(this.btnAddSection);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.tbPathogen1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbOverview);
            this.Name = "Presentation";
            this.Text = "Presentation";
            this.Load += new System.EventHandler(this.Presentation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.entitiesBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbOverview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPathogen1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.BindingSource entitiesBindingSource;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.Button btnAddSection;
        private System.Windows.Forms.TextBox tbSectionName;
        private System.Windows.Forms.Button btnSaveSection;
        private System.Windows.Forms.ComboBox cbSection;
        private System.Windows.Forms.TextBox tbCategoryName;
        private System.Windows.Forms.Button btnNewCategory;
        private System.Windows.Forms.Button btnSaveCategory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
    }
}

