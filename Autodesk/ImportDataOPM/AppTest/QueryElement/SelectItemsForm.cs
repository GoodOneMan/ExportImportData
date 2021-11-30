using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportDataOPM.AppTest.QueryElement
{
    public partial class SelectItemsForm : Form
    {

        QueryElement queryElement = null;
        
        public SelectItemsForm()
        {
            InitializeComponent();

            queryElement = new QueryElement();
        }

        private void SelectItemsForm_Load(object sender, EventArgs e)
        {
            var queryEcClass = queryElement.GetEcClass();
            var queryFamily = queryElement.GetFamily();
            var queryCategory = queryElement.GetCategory();

            // ecClass
            foreach(string ec_class in queryEcClass)
            {
                listEcClass.Items.Add(ec_class);
            }

            // family
            foreach(string family in queryFamily)
            {
                listFamily.Items.Add(family);
            }

            // category
            foreach(string category in queryCategory)
            {
                listCategory.Items.Add(category);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // ListBox.SelectedObjectCollection lEcClass = listEcClass.SelectedItems;
            // ListBox.SelectedObjectCollection lFamily = listFamily.SelectedItems;
            // ListBox.SelectedObjectCollection lCategory = listCategory.SelectedItems;

            List<string> lEcClass = listEcClass.SelectedItems.Cast<string>().ToList();
            List<string> lFamily = listFamily.SelectedItems.Cast<string>().ToList();
            List<string> lCategory = listCategory.SelectedItems.Cast<string>().ToList();

            queryElement.SearchCriteries(lEcClass, lFamily, lCategory);
        }
    }
}
