namespace BDEditor.Views
{
    partial class BDLayoutColumnNodeTypeEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BDLayoutColumnNodeTypeEditor));
            this.listBoxColumnNodeTypes = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblColumnLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxNodetypes = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxNodetypeProperties = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAssignNodeType = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxColumnNodeTypes
            // 
            this.listBoxColumnNodeTypes.FormattingEnabled = true;
            this.listBoxColumnNodeTypes.Location = new System.Drawing.Point(22, 94);
            this.listBoxColumnNodeTypes.Name = "listBoxColumnNodeTypes";
            this.listBoxColumnNodeTypes.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxColumnNodeTypes.Size = new System.Drawing.Size(319, 108);
            this.listBoxColumnNodeTypes.TabIndex = 36;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Column Label";
            // 
            // lblColumnLabel
            // 
            this.lblColumnLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblColumnLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColumnLabel.Location = new System.Drawing.Point(22, 34);
            this.lblColumnLabel.Name = "lblColumnLabel";
            this.lblColumnLabel.Size = new System.Drawing.Size(319, 23);
            this.lblColumnLabel.TabIndex = 39;
            this.lblColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Associated nodetypes and properties";
            // 
            // listBoxNodetypes
            // 
            this.listBoxNodetypes.FormattingEnabled = true;
            this.listBoxNodetypes.Location = new System.Drawing.Point(399, 94);
            this.listBoxNodetypes.Name = "listBoxNodetypes";
            this.listBoxNodetypes.Size = new System.Drawing.Size(316, 121);
            this.listBoxNodetypes.TabIndex = 41;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(396, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "Available Nodetypes";
            // 
            // listBoxNodetypeProperties
            // 
            this.listBoxNodetypeProperties.FormattingEnabled = true;
            this.listBoxNodetypeProperties.Location = new System.Drawing.Point(399, 251);
            this.listBoxNodetypeProperties.Name = "listBoxNodetypeProperties";
            this.listBoxNodetypeProperties.Size = new System.Drawing.Size(316, 173);
            this.listBoxNodetypeProperties.TabIndex = 44;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(396, 233);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Nodetype Properties";
            // 
            // btnRemove
            // 
            this.btnRemove.Image = global::BDEditor.Properties.Resources.arrow_split;
            this.btnRemove.Location = new System.Drawing.Point(357, 151);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(24, 24);
            this.btnRemove.TabIndex = 46;
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnAssignNodeType
            // 
            this.btnAssignNodeType.Image = global::BDEditor.Properties.Resources.arrow_join_180;
            this.btnAssignNodeType.Location = new System.Drawing.Point(357, 121);
            this.btnAssignNodeType.Name = "btnAssignNodeType";
            this.btnAssignNodeType.Size = new System.Drawing.Size(24, 24);
            this.btnAssignNodeType.TabIndex = 42;
            this.btnAssignNodeType.UseVisualStyleBackColor = true;
            // 
            // BDLayoutColumnNodeTypeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 440);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.listBoxNodetypeProperties);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAssignNodeType);
            this.Controls.Add(this.listBoxNodetypes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblColumnLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBoxColumnNodeTypes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BDLayoutColumnNodeTypeEditor";
            this.Text = "Virtual Column Nodetype Association Editor";
            this.Load += new System.EventHandler(this.BDLayoutColumnNodeTypeEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxColumnNodeTypes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblColumnLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxNodetypes;
        private System.Windows.Forms.Button btnAssignNodeType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxNodetypeProperties;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRemove;
    }
}