using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportGeometry.UnitsApp.Helpers
{
    class CallingFileBrowser
    {
        // Get file
        public static string GetFile()
        {
            string file = "";

            OpenFileDialog ofd = new OpenFileDialog();

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                file = ofd.FileName;
            }

            return file;
        }
    }
}
