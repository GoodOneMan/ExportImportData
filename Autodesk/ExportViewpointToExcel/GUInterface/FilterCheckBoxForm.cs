using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportToExcel.GUInterface
{
    public partial class FilterCheckBoxForm : Form
    {
        App.Structure.Model Model = null;
        FilterCheckBoxForm InstanceForm = null;
        FilterCheckBoxFormModel InstanceModel = null;

        public FilterCheckBoxForm(App.Structure.Model Model)
        {
            InitializeComponent();

            this.Model = Model;
            InstanceForm = this;
            InstanceModel = new FilterCheckBoxFormModel(InstanceForm, Model);
        }

        //
        private void FilterCheckBoxForm_Load(object sender, EventArgs e)
        {
            // ComboBox Filter Init
            cbFilterDate.Items.Add("Все");
            cbFilterDate.SelectedIndex = 0;
            if (Model.RootItem.Count != 0)
            {
                foreach (var Item in Model.RootItem)
                {
                    cbFilterDate.Items.Add(Item.Name);
                }
            }

            // Folder save
            tbFolderSave.Text = @"D:\Отчет по точкам " + Path.GetFileNameWithoutExtension(Model.Name) + Model.Date.ToString(" dd.MM.yyyy");
        }

        //
        private void btnFolderSave_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbFolderSave.Text = fbd.SelectedPath;
            }
        }

        //
        private void btnReports_Click(object sender, EventArgs e)
        {
            App.Structure.Statement Statement = new App.Structure.Statement();

            //
            string root_folder = cbFilterDate.SelectedItem.ToString();
            if (root_folder != "Все")
            {
                Statement.Date = root_folder;
            }

            //
            CheckedListBox.CheckedItemCollection collection = clbFilterStatus.CheckedItems;
            if (collection.Count != 0)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    Statement.MultiStatus.Add(
                        App.Viewpoint.ModelViewpoints.GetStringToStatus(
                            collection[i].ToString()
                        )
                    );
                }
            }

            //
            Statement.FolderSave = tbFolderSave.Text;

            //
            InstanceModel.GenerationReports(Statement);
        }

        #region Public method
        //
        public string GetSaveFolder()
        {
            return tbFolderSave.Text;
        }
        #endregion
        
    }
}
