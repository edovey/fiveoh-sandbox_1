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
            this.cbConfiguredEntry = new System.Windows.Forms.CheckBox();
            this.cbCombinedEntry = new System.Windows.Forms.CheckBox();
            this.cbImmuneResponse = new System.Windows.Forms.CheckBox();
            this.cbInfectionFrequency = new System.Windows.Forms.CheckBox();
            this.cbRegimen = new System.Windows.Forms.CheckBox();
            this.cbSurgeryClassification = new System.Windows.Forms.CheckBox();
            this.cbAntimicrobialGroup = new System.Windows.Forms.CheckBox();
            this.cbMicroorganism = new System.Windows.Forms.CheckBox();
            this.cbMicroorganismGroup = new System.Windows.Forms.CheckBox();
            this.cbAntimicrobialRisk = new System.Windows.Forms.CheckBox();
            this.cbCondition = new System.Windows.Forms.CheckBox();
            this.cbSubsection = new System.Windows.Forms.CheckBox();
            this.cbTopic = new System.Windows.Forms.CheckBox();
            this.cbTableGroup = new System.Windows.Forms.CheckBox();
            this.cbAttachment = new System.Windows.Forms.CheckBox();
            this.cbSurgeryGroup = new System.Windows.Forms.CheckBox();
            this.cbDosageGroup = new System.Windows.Forms.CheckBox();
            this.cbDosage = new System.Windows.Forms.CheckBox();
            this.cbAntimicrobial = new System.Windows.Forms.CheckBox();
            this.cbPrecaution = new System.Windows.Forms.CheckBox();
            this.cbSurgery = new System.Windows.Forms.CheckBox();
            this.cbTable = new System.Windows.Forms.CheckBox();
            this.cbTableSection = new System.Windows.Forms.CheckBox();
            this.cbTableSubsection = new System.Windows.Forms.CheckBox();
            this.cbPathogenResistance = new System.Windows.Forms.CheckBox();
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.rtbLocation = new System.Windows.Forms.RichTextBox();
            this.cbSubtopic = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.button1.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.Location = new System.Drawing.Point(0, 293);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(759, 509);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbSubtopic);
            this.groupBox1.Controls.Add(this.cbConfiguredEntry);
            this.groupBox1.Controls.Add(this.cbCombinedEntry);
            this.groupBox1.Controls.Add(this.cbImmuneResponse);
            this.groupBox1.Controls.Add(this.cbInfectionFrequency);
            this.groupBox1.Controls.Add(this.cbRegimen);
            this.groupBox1.Controls.Add(this.cbSurgeryClassification);
            this.groupBox1.Controls.Add(this.cbAntimicrobialGroup);
            this.groupBox1.Controls.Add(this.cbMicroorganism);
            this.groupBox1.Controls.Add(this.cbMicroorganismGroup);
            this.groupBox1.Controls.Add(this.cbAntimicrobialRisk);
            this.groupBox1.Controls.Add(this.cbCondition);
            this.groupBox1.Controls.Add(this.cbSubsection);
            this.groupBox1.Controls.Add(this.cbTopic);
            this.groupBox1.Controls.Add(this.cbTableGroup);
            this.groupBox1.Controls.Add(this.cbAttachment);
            this.groupBox1.Controls.Add(this.cbSurgeryGroup);
            this.groupBox1.Controls.Add(this.cbDosageGroup);
            this.groupBox1.Controls.Add(this.cbDosage);
            this.groupBox1.Controls.Add(this.cbAntimicrobial);
            this.groupBox1.Controls.Add(this.cbPrecaution);
            this.groupBox1.Controls.Add(this.cbSurgery);
            this.groupBox1.Controls.Add(this.cbTable);
            this.groupBox1.Controls.Add(this.cbTableSection);
            this.groupBox1.Controls.Add(this.cbTableSubsection);
            this.groupBox1.Controls.Add(this.cbPathogenResistance);
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
            this.groupBox1.Size = new System.Drawing.Size(542, 230);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search for:";
            // 
            // cbConfiguredEntry
            // 
            this.cbConfiguredEntry.AutoSize = true;
            this.cbConfiguredEntry.Checked = true;
            this.cbConfiguredEntry.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbConfiguredEntry.Location = new System.Drawing.Point(10, 191);
            this.cbConfiguredEntry.Name = "cbConfiguredEntry";
            this.cbConfiguredEntry.Size = new System.Drawing.Size(104, 17);
            this.cbConfiguredEntry.TabIndex = 34;
            this.cbConfiguredEntry.Text = "Configured Entry";
            this.cbConfiguredEntry.UseVisualStyleBackColor = true;
            // 
            // cbCombinedEntry
            // 
            this.cbCombinedEntry.AutoSize = true;
            this.cbCombinedEntry.Checked = true;
            this.cbCombinedEntry.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCombinedEntry.Location = new System.Drawing.Point(10, 149);
            this.cbCombinedEntry.Name = "cbCombinedEntry";
            this.cbCombinedEntry.Size = new System.Drawing.Size(97, 17);
            this.cbCombinedEntry.TabIndex = 33;
            this.cbCombinedEntry.Text = "CombinedEntry";
            this.cbCombinedEntry.UseVisualStyleBackColor = true;
            // 
            // cbImmuneResponse
            // 
            this.cbImmuneResponse.AutoSize = true;
            this.cbImmuneResponse.Checked = true;
            this.cbImmuneResponse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbImmuneResponse.Location = new System.Drawing.Point(132, 86);
            this.cbImmuneResponse.Name = "cbImmuneResponse";
            this.cbImmuneResponse.Size = new System.Drawing.Size(114, 17);
            this.cbImmuneResponse.TabIndex = 32;
            this.cbImmuneResponse.Text = "Immune Response";
            this.cbImmuneResponse.UseVisualStyleBackColor = true;
            // 
            // cbInfectionFrequency
            // 
            this.cbInfectionFrequency.AutoSize = true;
            this.cbInfectionFrequency.Checked = true;
            this.cbInfectionFrequency.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbInfectionFrequency.Location = new System.Drawing.Point(132, 107);
            this.cbInfectionFrequency.Name = "cbInfectionFrequency";
            this.cbInfectionFrequency.Size = new System.Drawing.Size(120, 17);
            this.cbInfectionFrequency.TabIndex = 31;
            this.cbInfectionFrequency.Text = "Infection Frequency";
            this.cbInfectionFrequency.UseVisualStyleBackColor = true;
            // 
            // cbRegimen
            // 
            this.cbRegimen.AutoSize = true;
            this.cbRegimen.Checked = true;
            this.cbRegimen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRegimen.Location = new System.Drawing.Point(268, 86);
            this.cbRegimen.Name = "cbRegimen";
            this.cbRegimen.Size = new System.Drawing.Size(68, 17);
            this.cbRegimen.TabIndex = 30;
            this.cbRegimen.Text = "Regimen";
            this.cbRegimen.UseVisualStyleBackColor = true;
            // 
            // cbSurgeryClassification
            // 
            this.cbSurgeryClassification.AutoSize = true;
            this.cbSurgeryClassification.Checked = true;
            this.cbSurgeryClassification.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSurgeryClassification.Location = new System.Drawing.Point(405, 44);
            this.cbSurgeryClassification.Name = "cbSurgeryClassification";
            this.cbSurgeryClassification.Size = new System.Drawing.Size(126, 17);
            this.cbSurgeryClassification.TabIndex = 29;
            this.cbSurgeryClassification.Text = "Surgery Classification";
            this.cbSurgeryClassification.UseVisualStyleBackColor = true;
            // 
            // cbAntimicrobialGroup
            // 
            this.cbAntimicrobialGroup.AutoSize = true;
            this.cbAntimicrobialGroup.Checked = true;
            this.cbAntimicrobialGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAntimicrobialGroup.Location = new System.Drawing.Point(10, 44);
            this.cbAntimicrobialGroup.Name = "cbAntimicrobialGroup";
            this.cbAntimicrobialGroup.Size = new System.Drawing.Size(117, 17);
            this.cbAntimicrobialGroup.TabIndex = 28;
            this.cbAntimicrobialGroup.Text = "Antimicrobial Group";
            this.cbAntimicrobialGroup.UseVisualStyleBackColor = true;
            // 
            // cbMicroorganism
            // 
            this.cbMicroorganism.AutoSize = true;
            this.cbMicroorganism.Checked = true;
            this.cbMicroorganism.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMicroorganism.Location = new System.Drawing.Point(132, 128);
            this.cbMicroorganism.Name = "cbMicroorganism";
            this.cbMicroorganism.Size = new System.Drawing.Size(94, 17);
            this.cbMicroorganism.TabIndex = 27;
            this.cbMicroorganism.Text = "Microorganism";
            this.cbMicroorganism.UseVisualStyleBackColor = true;
            // 
            // cbMicroorganismGroup
            // 
            this.cbMicroorganismGroup.AutoSize = true;
            this.cbMicroorganismGroup.Checked = true;
            this.cbMicroorganismGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMicroorganismGroup.Location = new System.Drawing.Point(132, 149);
            this.cbMicroorganismGroup.Name = "cbMicroorganismGroup";
            this.cbMicroorganismGroup.Size = new System.Drawing.Size(126, 17);
            this.cbMicroorganismGroup.TabIndex = 26;
            this.cbMicroorganismGroup.Text = "Microorganism Group";
            this.cbMicroorganismGroup.UseVisualStyleBackColor = true;
            // 
            // cbAntimicrobialRisk
            // 
            this.cbAntimicrobialRisk.AutoSize = true;
            this.cbAntimicrobialRisk.Checked = true;
            this.cbAntimicrobialRisk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAntimicrobialRisk.Location = new System.Drawing.Point(10, 86);
            this.cbAntimicrobialRisk.Name = "cbAntimicrobialRisk";
            this.cbAntimicrobialRisk.Size = new System.Drawing.Size(109, 17);
            this.cbAntimicrobialRisk.TabIndex = 25;
            this.cbAntimicrobialRisk.Text = "Antimicrobial Risk";
            this.cbAntimicrobialRisk.UseVisualStyleBackColor = true;
            // 
            // cbCondition
            // 
            this.cbCondition.AutoSize = true;
            this.cbCondition.Checked = true;
            this.cbCondition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCondition.Location = new System.Drawing.Point(10, 170);
            this.cbCondition.Name = "cbCondition";
            this.cbCondition.Size = new System.Drawing.Size(70, 17);
            this.cbCondition.TabIndex = 24;
            this.cbCondition.Text = "Condition";
            this.cbCondition.UseVisualStyleBackColor = true;
            // 
            // cbSubsection
            // 
            this.cbSubsection.AutoSize = true;
            this.cbSubsection.Checked = true;
            this.cbSubsection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSubsection.Location = new System.Drawing.Point(268, 149);
            this.cbSubsection.Name = "cbSubsection";
            this.cbSubsection.Size = new System.Drawing.Size(79, 17);
            this.cbSubsection.TabIndex = 23;
            this.cbSubsection.Text = "Subsection";
            this.cbSubsection.UseVisualStyleBackColor = true;
            // 
            // cbTopic
            // 
            this.cbTopic.AutoSize = true;
            this.cbTopic.Checked = true;
            this.cbTopic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTopic.Location = new System.Drawing.Point(405, 191);
            this.cbTopic.Name = "cbTopic";
            this.cbTopic.Size = new System.Drawing.Size(53, 17);
            this.cbTopic.TabIndex = 22;
            this.cbTopic.Text = "Topic";
            this.cbTopic.UseVisualStyleBackColor = true;
            // 
            // cbTableGroup
            // 
            this.cbTableGroup.AutoSize = true;
            this.cbTableGroup.Checked = true;
            this.cbTableGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTableGroup.Location = new System.Drawing.Point(405, 86);
            this.cbTableGroup.Name = "cbTableGroup";
            this.cbTableGroup.Size = new System.Drawing.Size(85, 17);
            this.cbTableGroup.TabIndex = 21;
            this.cbTableGroup.Text = "Table Group";
            this.cbTableGroup.UseVisualStyleBackColor = true;
            // 
            // cbAttachment
            // 
            this.cbAttachment.AutoSize = true;
            this.cbAttachment.Checked = true;
            this.cbAttachment.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAttachment.Location = new System.Drawing.Point(10, 65);
            this.cbAttachment.Name = "cbAttachment";
            this.cbAttachment.Size = new System.Drawing.Size(80, 17);
            this.cbAttachment.TabIndex = 20;
            this.cbAttachment.Text = "Attachment";
            this.cbAttachment.UseVisualStyleBackColor = true;
            // 
            // cbSurgeryGroup
            // 
            this.cbSurgeryGroup.AutoSize = true;
            this.cbSurgeryGroup.Checked = true;
            this.cbSurgeryGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSurgeryGroup.Location = new System.Drawing.Point(405, 23);
            this.cbSurgeryGroup.Name = "cbSurgeryGroup";
            this.cbSurgeryGroup.Size = new System.Drawing.Size(94, 17);
            this.cbSurgeryGroup.TabIndex = 19;
            this.cbSurgeryGroup.Text = "Surgery Group";
            this.cbSurgeryGroup.UseVisualStyleBackColor = true;
            // 
            // cbDosageGroup
            // 
            this.cbDosageGroup.AutoSize = true;
            this.cbDosageGroup.Checked = true;
            this.cbDosageGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDosageGroup.Location = new System.Drawing.Point(132, 65);
            this.cbDosageGroup.Name = "cbDosageGroup";
            this.cbDosageGroup.Size = new System.Drawing.Size(95, 17);
            this.cbDosageGroup.TabIndex = 18;
            this.cbDosageGroup.Text = "Dosage Group";
            this.cbDosageGroup.UseVisualStyleBackColor = true;
            // 
            // cbDosage
            // 
            this.cbDosage.AutoSize = true;
            this.cbDosage.Checked = true;
            this.cbDosage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDosage.Location = new System.Drawing.Point(132, 44);
            this.cbDosage.Name = "cbDosage";
            this.cbDosage.Size = new System.Drawing.Size(63, 17);
            this.cbDosage.TabIndex = 17;
            this.cbDosage.Text = "Dosage";
            this.cbDosage.UseVisualStyleBackColor = true;
            // 
            // cbAntimicrobial
            // 
            this.cbAntimicrobial.AutoSize = true;
            this.cbAntimicrobial.Checked = true;
            this.cbAntimicrobial.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAntimicrobial.Location = new System.Drawing.Point(10, 23);
            this.cbAntimicrobial.Name = "cbAntimicrobial";
            this.cbAntimicrobial.Size = new System.Drawing.Size(85, 17);
            this.cbAntimicrobial.TabIndex = 16;
            this.cbAntimicrobial.Text = "Antimicrobial";
            this.cbAntimicrobial.UseVisualStyleBackColor = true;
            // 
            // cbPrecaution
            // 
            this.cbPrecaution.AutoSize = true;
            this.cbPrecaution.Checked = true;
            this.cbPrecaution.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPrecaution.Location = new System.Drawing.Point(268, 44);
            this.cbPrecaution.Name = "cbPrecaution";
            this.cbPrecaution.Size = new System.Drawing.Size(77, 17);
            this.cbPrecaution.TabIndex = 15;
            this.cbPrecaution.Text = "Precaution";
            this.cbPrecaution.UseVisualStyleBackColor = true;
            // 
            // cbSurgery
            // 
            this.cbSurgery.AutoSize = true;
            this.cbSurgery.Checked = true;
            this.cbSurgery.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSurgery.Location = new System.Drawing.Point(268, 191);
            this.cbSurgery.Name = "cbSurgery";
            this.cbSurgery.Size = new System.Drawing.Size(62, 17);
            this.cbSurgery.TabIndex = 14;
            this.cbSurgery.Text = "Surgery";
            this.cbSurgery.UseVisualStyleBackColor = true;
            // 
            // cbTable
            // 
            this.cbTable.AutoSize = true;
            this.cbTable.Checked = true;
            this.cbTable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTable.Location = new System.Drawing.Point(405, 65);
            this.cbTable.Name = "cbTable";
            this.cbTable.Size = new System.Drawing.Size(53, 17);
            this.cbTable.TabIndex = 13;
            this.cbTable.Text = "Table";
            this.cbTable.UseVisualStyleBackColor = true;
            // 
            // cbTableSection
            // 
            this.cbTableSection.AutoSize = true;
            this.cbTableSection.Checked = true;
            this.cbTableSection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTableSection.Location = new System.Drawing.Point(405, 107);
            this.cbTableSection.Name = "cbTableSection";
            this.cbTableSection.Size = new System.Drawing.Size(92, 17);
            this.cbTableSection.TabIndex = 12;
            this.cbTableSection.Text = "Table Section";
            this.cbTableSection.UseVisualStyleBackColor = true;
            // 
            // cbTableSubsection
            // 
            this.cbTableSubsection.AutoSize = true;
            this.cbTableSubsection.Checked = true;
            this.cbTableSubsection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTableSubsection.Location = new System.Drawing.Point(405, 128);
            this.cbTableSubsection.Name = "cbTableSubsection";
            this.cbTableSubsection.Size = new System.Drawing.Size(109, 17);
            this.cbTableSubsection.TabIndex = 11;
            this.cbTableSubsection.Text = "Table Subsection";
            this.cbTableSubsection.UseVisualStyleBackColor = true;
            // 
            // cbPathogenResistance
            // 
            this.cbPathogenResistance.AutoSize = true;
            this.cbPathogenResistance.Checked = true;
            this.cbPathogenResistance.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPathogenResistance.Location = new System.Drawing.Point(268, 23);
            this.cbPathogenResistance.Name = "cbPathogenResistance";
            this.cbPathogenResistance.Size = new System.Drawing.Size(128, 17);
            this.cbPathogenResistance.TabIndex = 10;
            this.cbPathogenResistance.Text = "Pathogen Resistance";
            this.cbPathogenResistance.UseVisualStyleBackColor = true;
            // 
            // cbDisease
            // 
            this.cbDisease.AutoSize = true;
            this.cbDisease.Checked = true;
            this.cbDisease.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDisease.Location = new System.Drawing.Point(132, 23);
            this.cbDisease.Name = "cbDisease";
            this.cbDisease.Size = new System.Drawing.Size(64, 17);
            this.cbDisease.TabIndex = 4;
            this.cbDisease.Text = "Disease";
            this.cbDisease.UseVisualStyleBackColor = true;
            // 
            // cbTherapy
            // 
            this.cbTherapy.AutoSize = true;
            this.cbTherapy.Checked = true;
            this.cbTherapy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTherapy.Location = new System.Drawing.Point(405, 149);
            this.cbTherapy.Name = "cbTherapy";
            this.cbTherapy.Size = new System.Drawing.Size(65, 17);
            this.cbTherapy.TabIndex = 9;
            this.cbTherapy.Text = "Therapy";
            this.cbTherapy.UseVisualStyleBackColor = true;
            // 
            // cbTherapyGroup
            // 
            this.cbTherapyGroup.AutoSize = true;
            this.cbTherapyGroup.Checked = true;
            this.cbTherapyGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTherapyGroup.Location = new System.Drawing.Point(405, 170);
            this.cbTherapyGroup.Name = "cbTherapyGroup";
            this.cbTherapyGroup.Size = new System.Drawing.Size(97, 17);
            this.cbTherapyGroup.TabIndex = 8;
            this.cbTherapyGroup.Text = "Therapy Group";
            this.cbTherapyGroup.UseVisualStyleBackColor = true;
            // 
            // cbPathogen
            // 
            this.cbPathogen.AutoSize = true;
            this.cbPathogen.Checked = true;
            this.cbPathogen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPathogen.Location = new System.Drawing.Point(132, 170);
            this.cbPathogen.Name = "cbPathogen";
            this.cbPathogen.Size = new System.Drawing.Size(72, 17);
            this.cbPathogen.TabIndex = 7;
            this.cbPathogen.Text = "Pathogen";
            this.cbPathogen.UseVisualStyleBackColor = true;
            // 
            // cbPathogenGroup
            // 
            this.cbPathogenGroup.AutoSize = true;
            this.cbPathogenGroup.Checked = true;
            this.cbPathogenGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPathogenGroup.Location = new System.Drawing.Point(132, 191);
            this.cbPathogenGroup.Name = "cbPathogenGroup";
            this.cbPathogenGroup.Size = new System.Drawing.Size(104, 17);
            this.cbPathogenGroup.TabIndex = 6;
            this.cbPathogenGroup.Text = "Pathogen Group";
            this.cbPathogenGroup.UseVisualStyleBackColor = true;
            // 
            // cbPresentation
            // 
            this.cbPresentation.AutoSize = true;
            this.cbPresentation.Checked = true;
            this.cbPresentation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPresentation.Location = new System.Drawing.Point(268, 65);
            this.cbPresentation.Name = "cbPresentation";
            this.cbPresentation.Size = new System.Drawing.Size(85, 17);
            this.cbPresentation.TabIndex = 5;
            this.cbPresentation.Text = "Presentation";
            this.cbPresentation.UseVisualStyleBackColor = true;
            // 
            // cbCategory
            // 
            this.cbCategory.AutoSize = true;
            this.cbCategory.Checked = true;
            this.cbCategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCategory.Location = new System.Drawing.Point(10, 107);
            this.cbCategory.Name = "cbCategory";
            this.cbCategory.Size = new System.Drawing.Size(68, 17);
            this.cbCategory.TabIndex = 2;
            this.cbCategory.Text = "Category";
            this.cbCategory.UseVisualStyleBackColor = true;
            // 
            // cbSubcategory
            // 
            this.cbSubcategory.AutoSize = true;
            this.cbSubcategory.Checked = true;
            this.cbSubcategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSubcategory.Location = new System.Drawing.Point(268, 128);
            this.cbSubcategory.Name = "cbSubcategory";
            this.cbSubcategory.Size = new System.Drawing.Size(86, 17);
            this.cbSubcategory.TabIndex = 3;
            this.cbSubcategory.Text = "Subcategory";
            this.cbSubcategory.UseVisualStyleBackColor = true;
            // 
            // cbSection
            // 
            this.cbSection.AutoSize = true;
            this.cbSection.Checked = true;
            this.cbSection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSection.Location = new System.Drawing.Point(268, 107);
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
            this.cbChapter.Location = new System.Drawing.Point(10, 128);
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
            this.rbNodes.TabIndex = 2;
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
            this.rbLinkedNotes.TabIndex = 3;
            this.rbLinkedNotes.Text = "Notes";
            this.rbLinkedNotes.UseVisualStyleBackColor = true;
            this.rbLinkedNotes.CheckedChanged += new System.EventHandler(this.rbLinkedNotes_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.tbSearchTerm);
            this.panel1.Controls.Add(this.rbLinkedNotes);
            this.panel1.Controls.Add(this.rbNodes);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(759, 293);
            this.panel1.TabIndex = 0;
            // 
            // rtbLocation
            // 
            this.rtbLocation.Dock = System.Windows.Forms.DockStyle.Right;
            this.rtbLocation.Location = new System.Drawing.Point(550, 0);
            this.rtbLocation.Name = "rtbLocation";
            this.rtbLocation.Size = new System.Drawing.Size(209, 293);
            this.rtbLocation.TabIndex = 2;
            this.rtbLocation.Text = "";
            // 
            // cbSubtopic
            // 
            this.cbSubtopic.AutoSize = true;
            this.cbSubtopic.Checked = true;
            this.cbSubtopic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSubtopic.Location = new System.Drawing.Point(268, 170);
            this.cbSubtopic.Name = "cbSubtopic";
            this.cbSubtopic.Size = new System.Drawing.Size(68, 17);
            this.cbSubtopic.TabIndex = 35;
            this.cbSubtopic.Text = "Subtopic";
            this.cbSubtopic.UseVisualStyleBackColor = true;
            // 
            // BDSearchView
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 802);
            this.Controls.Add(this.rtbLocation);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BDSearchView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bugs & Drugs Search";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox rtbLocation;
        private System.Windows.Forms.CheckBox cbSubsection;
        private System.Windows.Forms.CheckBox cbTopic;
        private System.Windows.Forms.CheckBox cbTableGroup;
        private System.Windows.Forms.CheckBox cbAttachment;
        private System.Windows.Forms.CheckBox cbSurgeryGroup;
        private System.Windows.Forms.CheckBox cbDosageGroup;
        private System.Windows.Forms.CheckBox cbDosage;
        private System.Windows.Forms.CheckBox cbAntimicrobial;
        private System.Windows.Forms.CheckBox cbPrecaution;
        private System.Windows.Forms.CheckBox cbSurgery;
        private System.Windows.Forms.CheckBox cbTable;
        private System.Windows.Forms.CheckBox cbTableSection;
        private System.Windows.Forms.CheckBox cbTableSubsection;
        private System.Windows.Forms.CheckBox cbPathogenResistance;
        private System.Windows.Forms.CheckBox cbConfiguredEntry;
        private System.Windows.Forms.CheckBox cbCombinedEntry;
        private System.Windows.Forms.CheckBox cbImmuneResponse;
        private System.Windows.Forms.CheckBox cbInfectionFrequency;
        private System.Windows.Forms.CheckBox cbRegimen;
        private System.Windows.Forms.CheckBox cbSurgeryClassification;
        private System.Windows.Forms.CheckBox cbAntimicrobialGroup;
        private System.Windows.Forms.CheckBox cbMicroorganism;
        private System.Windows.Forms.CheckBox cbMicroorganismGroup;
        private System.Windows.Forms.CheckBox cbAntimicrobialRisk;
        private System.Windows.Forms.CheckBox cbCondition;
        private System.Windows.Forms.CheckBox cbSubtopic;

    }
}