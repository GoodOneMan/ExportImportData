using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject
{
    [Bentley.MicroStation.AddInAttribute (KeyinTree = "TestProject.commands.xml", MdlTaskID = "TestProject")]
    internal sealed class Runner : Bentley.MicroStation.AddIn
    {
        private Runner(System.IntPtr mdlDesc) : base(mdlDesc)
        {

        }

        protected override int Run(string[] commandLine)
        {
            return 0;
        }
    }
}