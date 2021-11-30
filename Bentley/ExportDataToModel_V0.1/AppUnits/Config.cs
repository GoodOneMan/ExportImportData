using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;

namespace ExportDataToModel.AppUnits
{
    class Config
    {
        static string _path = @"C:\Program Files (x86)\Bentley\OpenPlantModeler V8i\OpenPlantModeler\mdlapps\OpenPlantV8IAddinCXPP\"; 
        //static string _path = @"C:\Program Files (x86)\Bentley\OpenPlantModeler V8i\OpenPlantModeler\mdlapps\ExportDataNW\";

        #region Log File
        static string _log_path = _path + "addin_CXPP.log";
        static StreamWriter _log = null;
        public static StreamWriter SWLog()
        {
            if (_log == null)
                _log = new StreamWriter(_log_path, true);

            _log.WriteLine(DateTime.Now);
            return _log;
        }

        public static void SWLogClose()
        {
            _log.Close();
            _log.Dispose();
            _log = null;
        }
        #endregion

        #region Fields Path
        static string _fields_file_path = null;
        static Dictionary<string, string> _fields = null;

        private static void ReadFields()
        {
            if (_fields_file_path == null || !File.Exists(_fields_file_path) || !Path.GetFileName(_fields_file_path).Equals("specified_fields.dat"))
            {
                SetPathFileFields();
                ReadFields();
            }
            else
            {
                using (StreamReader sr = new StreamReader(_fields_file_path))
                {
                    string line = null;
                    bool trigger = false;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("#START_FIELDS"))
                        {
                            trigger = true;
                            continue;
                        }
                        if (line.Contains("#END_FIELDS"))
                        {
                            trigger = false;
                            continue;
                        }

                        if (trigger)
                        {
                            string[] data = line.Split('=');
                            _fields[data[0].Trim(new char[] { ' ' })] = data[1].Trim(new char[] { ' ' });
                        }
                    }
                }
            }
        }

        private static void SetPathFileFields()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Файл с полями";
            dialog.Filter = "Файл с определенными полями (*.dat)|*.dat";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _fields_file_path = dialog.FileName;
            }
            else
            {
                _fields_file_path = _path + "specified_fields.dat";
            }
        }

        public static Dictionary<string, string> GetFields()
        {
            if(_fields == null)
            {
                _fields = new Dictionary<string, string>();
                ReadFields();
            }

            return _fields;
        }
        #endregion

    }
}
