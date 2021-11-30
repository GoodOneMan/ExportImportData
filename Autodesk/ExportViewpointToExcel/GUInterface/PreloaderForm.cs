using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportToExcel.GUInterface
{
    public partial class PreloaderForm : Form
    {
        public PreloaderForm()
        {
            InitializeComponent();
        }

        private void PreloaderForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }
    }
}
