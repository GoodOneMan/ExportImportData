using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api;
using CAB = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using CA = Autodesk.Navisworks.Api.Interop.ComApi;


namespace ExportGeometry.App.ModelGeometry
{
    class DataRrobber
    {
        Structures.Model model;

        public DataRrobber()
        {
            model = new Structures.Model();
            model.items = new List<Structures.Item>();


        }

        private void Run()
        {
            
        }
    }
}
