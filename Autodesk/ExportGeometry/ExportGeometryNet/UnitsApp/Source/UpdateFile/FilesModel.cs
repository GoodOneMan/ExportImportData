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

namespace ExportGeometry.UnitsApp.Source.UpdateFile
{
    class FilesModel
    {
        List<string> files = new List<string>();

        #region Block handler files in folder
        public static string[] GetFilesInFolder()
        {
            List<string> files_in = new List<string>();

            string file_in = _GetFile();

            if (file_in.Equals("") || !file_in.Contains(".txt"))
                return null;

            using (StreamReader sr = new StreamReader(file_in))
            {
                string line = "";

                while((line = sr.ReadLine()) != null)
                {
                    if (_ValidFile(line))
                        files_in.Add(line);
                }
            }

            return files_in.ToArray();
        }

        //
        private static bool _ValidFile(string path)
        {
            bool trigger = false;

            string file = Path.GetFileName(path);

            if (file.Contains(".nwf"))
                trigger = true;

            //if (file.Contains(".dgn"))
            //    trigger = true;
            //if (file.Contains(".dwg"))
            //    trigger = true;
            //if (file.Contains(".rvt"))
            //    trigger = true;
            //if (file.Contains(".fbx"))
            //    trigger = true;

            //if (file.Contains(".i.dgn"))
            //    trigger = false;

            return trigger;
        }

        //
        private static string _GetFile()
        {
            string file = "";

            OpenFileDialog fileDialog = new OpenFileDialog();
            
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                file = fileDialog.FileName;
            }

            return file;
        }

        #endregion
    }
}
