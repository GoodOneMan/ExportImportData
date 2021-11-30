using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using System.Runtime.InteropServices;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using System.IO;

namespace ExportGeometry.UnitsApp.Source
{
    class UpdataFileCall
    {
        public UpdataFileCall()
        {

            // 1 get list of all files in folder
            // 2 filter by extension
            // 3 get a list of files in the model
            // 4 to find the path for the files from the model files in folders

            new UpdateFile.FilesModel();

            string[] file_mod = Source.UpdateFile.FilesModel.GetFilesInFolder();

            StreamWriter sw = new StreamWriter(@"D:\test_data_in.txt");

            foreach(string file in file_mod)
            {
                sw.WriteLine(file);
            }

            sw.Close();
        }
    }
}
