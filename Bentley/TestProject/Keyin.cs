using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TestProject
{
    public sealed class Keyin
    {
        public static void CmdTest(string unparsed)
        {
            new TestProject.TestUnits.OPMSchems();

            MessageBox.Show("Test ok");
        }
    }
}
