namespace ExportToExcel.GUInterface
{
    partial class FilterCheckBoxForm
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
            this.clbFilterStatus = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnFolderSave = new System.Windows.Forms.Button();
            this.tbFolderSave = new System.Windows.Forms.TextBox();
            this.btnReports = new System.Windows.Forms.Button();
            this.cbFilterDate = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // clbFilterStatus
            // 
            this.clbFilterStatus.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.clbFilterStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbFilterStatus.CheckOnClick = true;
            this.clbFilterStatus.FormattingEnabled = true;
            this.clbFilterStatus.Items.AddRange(new object[] {
            "Новый",
            "Активный",
            "Исправлено",
            "Утверждено"});
            this.clbFilterStatus.Location = new System.Drawing.Point(12, 74);
            this.clbFilterStatus.Name = "clbFilterStatus";
            this.clbFilterStatus.Size = new System.Drawing.Size(188, 60);
            this.clbFilterStatus.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Статус";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Дата";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Сохранить в";
            // 
            // btnFolderSave
            // 
            this.btnFolderSave.Location = new System.Drawing.Point(162, 162);
            this.btnFolderSave.Name = "btnFolderSave";
            this.btnFolderSave.Size = new System.Drawing.Size(38, 20);
            this.btnFolderSave.TabIndex = 5;
            this.btnFolderSave.Text = "...";
            this.btnFolderSave.UseVisualStyleBackColor = true;
            this.btnFolderSave.Click += new System.EventHandler(this.btnFolderSave_Click);
            // 
            // tbFolderSave
            // 
            this.tbFolderSave.Location = new System.Drawing.Point(12, 162);
            this.tbFolderSave.Name = "tbFolderSave";
            this.tbFolderSave.Size = new System.Drawing.Size(141, 20);
            this.tbFolderSave.TabIndex = 6;
            // 
            // btnReports
            // 
            this.btnReports.Location = new System.Drawing.Point(12, 198);
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(75, 23);
            this.btnReports.TabIndex = 7;
            this.btnReports.Text = "Отчет";
            this.btnReports.UseVisualStyleBackColor = true;
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            // 
            // cbFilterDate
            // 
            this.cbFilterDate.FormattingEnabled = true;
            this.cbFilterDate.Location = new System.Drawing.Point(12, 25);
            this.cbFilterDate.Name = "cbFilterDate";
            this.cbFilterDate.Size = new System.Drawing.Size(188, 21);
            this.cbFilterDate.TabIndex = 8;
            // 
            // FilterCheckBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 237);
            this.Controls.Add(this.cbFilterDate);
            this.Controls.Add(this.btnReports);
            this.Controls.Add(this.tbFolderSave);
            this.Controls.Add(this.btnFolderSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.clbFilterStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FilterCheckBoxForm";
            this.Text = "Настроить отчет";
            this.Load += new System.EventHandler(this.FilterCheckBoxForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbFilterStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnFolderSave;
        private System.Windows.Forms.TextBox tbFolderSave;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.ComboBox cbFilterDate;
    }
}