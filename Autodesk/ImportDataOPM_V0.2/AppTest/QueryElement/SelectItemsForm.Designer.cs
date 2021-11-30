namespace ImportDataOPM.AppTest.QueryElement
{
    partial class SelectItemsForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbDGN = new System.Windows.Forms.CheckBox();
            this.cbRVT = new System.Windows.Forms.CheckBox();
            this.cbOther = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.listFamily = new System.Windows.Forms.ListBox();
            this.lbFamily = new System.Windows.Forms.Label();
            this.listCategory = new System.Windows.Forms.ListBox();
            this.lbCategory = new System.Windows.Forms.Label();
            this.lbECCLASS = new System.Windows.Forms.Label();
            this.listEcClass = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbOther);
            this.panel1.Controls.Add(this.cbRVT);
            this.panel1.Controls.Add(this.cbDGN);
            this.panel1.Location = new System.Drawing.Point(12, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(229, 37);
            this.panel1.TabIndex = 0;
            // 
            // cbDGN
            // 
            this.cbDGN.AutoSize = true;
            this.cbDGN.Location = new System.Drawing.Point(24, 12);
            this.cbDGN.Name = "cbDGN";
            this.cbDGN.Size = new System.Drawing.Size(50, 17);
            this.cbDGN.TabIndex = 0;
            this.cbDGN.Text = "DGN";
            this.cbDGN.UseVisualStyleBackColor = true;
            // 
            // cbRVT
            // 
            this.cbRVT.AutoSize = true;
            this.cbRVT.Location = new System.Drawing.Point(94, 12);
            this.cbRVT.Name = "cbRVT";
            this.cbRVT.Size = new System.Drawing.Size(48, 17);
            this.cbRVT.TabIndex = 1;
            this.cbRVT.Text = "RVT";
            this.cbRVT.UseVisualStyleBackColor = true;
            // 
            // cbOther
            // 
            this.cbOther.AutoSize = true;
            this.cbOther.Location = new System.Drawing.Point(161, 12);
            this.cbOther.Name = "cbOther";
            this.cbOther.Size = new System.Drawing.Size(52, 17);
            this.cbOther.TabIndex = 2;
            this.cbOther.Text = "Other";
            this.cbOther.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbCategory);
            this.groupBox1.Controls.Add(this.listCategory);
            this.groupBox1.Controls.Add(this.lbFamily);
            this.groupBox1.Controls.Add(this.listFamily);
            this.groupBox1.Location = new System.Drawing.Point(12, 75);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(578, 225);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "RVT";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listEcClass);
            this.groupBox2.Controls.Add(this.lbECCLASS);
            this.groupBox2.Location = new System.Drawing.Point(12, 306);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(271, 204);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "DGN";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(515, 472);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // listFamily
            // 
            this.listFamily.FormattingEnabled = true;
            this.listFamily.Location = new System.Drawing.Point(6, 40);
            this.listFamily.Name = "listFamily";
            this.listFamily.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listFamily.Size = new System.Drawing.Size(254, 173);
            this.listFamily.TabIndex = 0;
            // 
            // lbFamily
            // 
            this.lbFamily.AutoSize = true;
            this.lbFamily.Location = new System.Drawing.Point(3, 20);
            this.lbFamily.Name = "lbFamily";
            this.lbFamily.Size = new System.Drawing.Size(36, 13);
            this.lbFamily.TabIndex = 1;
            this.lbFamily.Text = "Family";
            // 
            // listCategory
            // 
            this.listCategory.FormattingEnabled = true;
            this.listCategory.Location = new System.Drawing.Point(295, 40);
            this.listCategory.Name = "listCategory";
            this.listCategory.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listCategory.Size = new System.Drawing.Size(277, 173);
            this.listCategory.TabIndex = 2;
            // 
            // lbCategory
            // 
            this.lbCategory.AutoSize = true;
            this.lbCategory.Location = new System.Drawing.Point(292, 20);
            this.lbCategory.Name = "lbCategory";
            this.lbCategory.Size = new System.Drawing.Size(49, 13);
            this.lbCategory.TabIndex = 3;
            this.lbCategory.Text = "Category";
            // 
            // lbECCLASS
            // 
            this.lbECCLASS.AutoSize = true;
            this.lbECCLASS.Location = new System.Drawing.Point(6, 20);
            this.lbECCLASS.Name = "lbECCLASS";
            this.lbECCLASS.Size = new System.Drawing.Size(58, 13);
            this.lbECCLASS.TabIndex = 0;
            this.lbECCLASS.Text = "EC CLASS";
            // 
            // listEcClass
            // 
            this.listEcClass.FormattingEnabled = true;
            this.listEcClass.Location = new System.Drawing.Point(9, 36);
            this.listEcClass.Name = "listEcClass";
            this.listEcClass.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listEcClass.Size = new System.Drawing.Size(251, 160);
            this.listEcClass.TabIndex = 1;
            // 
            // SelectItemsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 522);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "SelectItemsForm";
            this.Text = "Выбрать элементы";
            this.Load += new System.EventHandler(this.SelectItemsForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbOther;
        private System.Windows.Forms.CheckBox cbRVT;
        private System.Windows.Forms.CheckBox cbDGN;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbFamily;
        private System.Windows.Forms.ListBox listFamily;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lbCategory;
        private System.Windows.Forms.ListBox listCategory;
        private System.Windows.Forms.ListBox listEcClass;
        private System.Windows.Forms.Label lbECCLASS;
    }
}