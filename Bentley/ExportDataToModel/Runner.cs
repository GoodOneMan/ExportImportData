using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportDataToModel
{
    [Bentley.MicroStation.AddInAttribute (KeyinTree = "ExportDataToModel.commands.xml", MdlTaskID = "ExportData")]
    internal sealed class Runner : Bentley.MicroStation.AddIn
    {
        private Runner(System.IntPtr mdlDesc) : base(mdlDesc)
        {
            // Load Assembly
            var loadResoutce = new AppUnits.LoadingResources();
        }

        protected override int Run(string[] commandLine)
        {
            return 0;
        }
    }
}