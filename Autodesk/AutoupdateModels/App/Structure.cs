using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api.Automation;

namespace AutoupdateModels.App.Structure
{
    #region Files
    class Files
    {
        public string extension;
        public string folder;
        public List<string> files = new List<string>();
    }

    class GroupFiles
    {
        public string name;
        public string folder;
        public string save_file;
        public List<string> files = new List<string>();
    }
    #endregion
    
    #region Model
    class Model
    {
        // type model
        public string type;
        // created for the first time
        public bool flag_first = true;
        // RAM memory (default = 0)
        public long max_size_memory = 0;
        public long size_file = 0;
        // Time application
        public DateTime start_app_time;
        public DateTime end_app_time;
        public TimeSpan build_app_time;
        // id process roamer
        public int id_process = 0;
        // ref in navisworks application
        public NavisworksApplication roamer = null;
        // Files data
        public string out_file;
        public string out_folder;
    }
    
    class SingleModel : Model
    {
        // Files data
        public string in_file;
        public string in_folder;
    }

    class GroupModel : Model
    {
        public struct Files
        {
            // Files data
            public string in_file;
            public string in_folder;
        }

        public List<GroupModel.Files> files = new List<Files>();
    }
    #endregion
}
