using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExportDataToModel.AppUnits
{
    public partial class MessageForm : Form
    {
        public MessageForm()
        {
            InitializeComponent();
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {

        }

        public void SetCounter(int count)
        {
            lbCounter.Text = count.ToString();
            this.Update();
        }

        public void SetHeader(string header)
        {
            lbHeader.Text = header;
            this.Update();
        }

    }
}
