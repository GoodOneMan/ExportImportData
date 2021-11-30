using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportDataOPM.AppTest.SelectionItem
{
    public partial class SetPropertyForm : Form
    {
        SearchItem searchItem = null;
        
        public SetPropertyForm(SearchItem _searchItem)
        {
            searchItem = _searchItem;

            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string userCategory = tbUserCat.Text;
            string internalCategory = tbInternalCat.Text;
            string userProperty = tbUserProp.Text;
            string intrenalProperty = tbInternalProp.Text;
            string propertyValue = tbValue.Text;


            if (userCategory.Equals(""))
            {
                userCategory = "Элемент";
            }
            if (internalCategory.Equals(""))
            {
                internalCategory = "LcOaNode";
            }
            if (userProperty.Equals(""))
            {
                userProperty = "Внутренний тип";
            }
            if (intrenalProperty.Equals(""))
            {
                intrenalProperty = "LcOaSceneBaseClassName";
            }
            if (propertyValue.Equals(""))
            {
                propertyValue = "LcDgnCone";
            }
            
            searchItem.RunAttribSearch(userCategory, internalCategory, userProperty, intrenalProperty, propertyValue);

        }

        private void btnSearchClassName_Click(object sender, EventArgs e)
        {
            string[] words = tbWordArray.Text.Split(';');
            bool wholeWord = cbWholeWord.Checked;

            searchItem.RunClassNameSearch(words, wholeWord);

        }
    }
}
