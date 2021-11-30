using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoupdateModels.Source
{
    class Config
    {
        #region Property

        // lock object
        public static object locker = new object();
        
        // Locked file copy to server
        public static List<string> locked_file = new List<string>();

        // the initialization flag
        public bool InitFlag { get; private set; }

        // initialization message
        public string InitMessage { get; private set; }

        // main configuration file
        public string ConfigurationFile { get; private set; }

        // single file storage folder
        private string folder;

        // folder for storing a single group of files
        private Dictionary<string, string> group_folder;

        // delay on re-update
        public int LoopPause { get; private set; }

        // the delay in re-copying the file
        public int CopyFilePause { get; private set; }

        // single object set
        public List<App.Structure.Files> files_list { get; set; }

        // a set of group objects
        public List<App.Structure.GroupFiles> group_files_list { get; set; }

        // config instance
        private static readonly Lazy<Config> lazy = new Lazy<Config>(() => new Config());

        #endregion

        // get config method
        public static Config Instance
        {
            get
            {
                lock (locker)
                {
                    return lazy.Value;
                }
            }
        }

        // construct
        private Config()
        {
            //ConfigurationFile = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Config\\Set.d";
            ConfigurationFile = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Config\\Set.d.loc";

            group_folder = new Dictionary<string, string>();
            files_list = new List<App.Structure.Files>();
            group_files_list = new List<App.Structure.GroupFiles>();

            Init();
            SetFolderEnd();
        }

        // Init config
        #region Init config     
                  
        private void Init()
        {
            if (File.Exists(ConfigurationFile))
            {
                using (StreamReader sr = new StreamReader(ConfigurationFile))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {

                        #region Pause
                        // Set time
                        if (line.Contains("pause loop") && !line.Contains("##"))
                        {
                            // Pause Loop
                            LoopPause = SetPause(line);
                        }
                        if (line.Contains("pause copy file") && !line.Contains("##"))
                        {
                            // Pause Copy file to server
                            CopyFilePause = SetPause(line);
                        }
                        #endregion

                        #region Files
                        // Set file
                        if (line.Contains("file link") && !line.Contains("##"))
                        {
                            // All files
                            string file = line.Replace("file link","").Trim();
                            string type = Path.GetExtension(file);
                            SetFiles(type, file);
                        }

                        if (line.Contains("folder save") && !line.Contains("##"))
                        {
                            // Path one folder save
                            folder = line.Replace("folder save", "").Trim();
                        }
                        #endregion

                        #region Group
                        // Set group files
                        if (line.Contains("group name") && !line.Contains("##"))
                        {
                            // Add group name
                            SetGroupData("group name", line);
                        }

                        if (line.Contains("group file") && !line.Contains("##"))
                        {
                            // File to group
                            SetGroupData("group file", line);
                        }

                        if (line.Contains("group folder") && !line.Contains("##"))
                        {
                            // Path group folder
                            SetGroupData("group folder", line);
                        }
                        #endregion
                    }
                }

                InitFlag = true;
                InitMessage = "initialization was successful";
            }
            else
            {
                InitFlag = false;
                InitMessage = "initialization failed";
            }
        }

        #endregion

        // Handler line
        #region Handler line init

        // Pause
        private int SetPause(string line)
        {
            int time = 0;
            if (line.Contains("sec"))
            {
                int sec = 0;
                try { sec = Convert.ToInt32(line.Replace("pause loop", "").Replace("pause copy file", "").Replace("sec", "").Trim()); }
                catch { }
                time = sec * 1000;
            }
            if (line.Contains("min"))
            {
                int min = 0;
                try { min = Convert.ToInt32(line.Replace("pause loop", "").Replace("pause copy file", "").Replace("min", "").Trim()); }
                catch { }
                time = min * 1000 * 60;
            }
            if (line.Contains("hour"))
            {
                int hour = 0;
                try { hour = Convert.ToInt32(line.Replace("pause loop", "").Replace("pause copy file", "").Replace("hour", "").Trim()); }
                catch { }
                time = hour * 1000 * 60 * 60;
            }
            if (line.Contains(":"))
            {
                string str = line.Replace("pause loop", "").Replace("pause copy file", "").Trim();

                var arr = str.Split(':');

                int sec = 0;
                int min = 0;
                int hour = 0;

                try { sec = Convert.ToInt32(arr[2]); }
                catch { }
                try { min = Convert.ToInt32(arr[1]); }
                catch { }
                try { hour = Convert.ToInt32(arr[0]); }
                catch { }

                time = sec * 1000 * min * 60 * hour * 60;
            }
            return time;
        }

        // Files
        private void SetFiles(string type, string file)
        {
            int count = files_list.Count;

            bool flag_type = true;

            App.Structure.Files Files = new App.Structure.Files();

            for (int i = 0; i < count; i++)
            {
                if (files_list[i].extension == type)
                {
                    Files = files_list[i];
                    flag_type = false;
                }
            }

            if (flag_type)
            {
                Files.extension = type;
                Files.folder = "";
                Files.files.Add(file);

                files_list.Add(Files);
            }
            else
            {
                bool flag_file = true;
                foreach(string f in Files.files)
                {
                    if (f.Contains(file))
                    {
                        flag_file = false;
                    }
                }

                if (flag_file)
                {
                    Files.files.Add(file);
                }
            }
        }

        // Facad group
        private void SetGroupData(string flag, string line)
        {
            switch (flag)
            {
                case "group name":
                    SetGroupName("group name",line);
                    break;
                case "group file":
                    SetGroupFiles("group file", line); 
                    break;
                case "group folder":
                    SetGroupFolder("group folder", line);
                    break;
            }
        }

        // Group name
        private void SetGroupName(string replace, string line)
        {
            line = line.Replace(replace, "").Trim();
            string name_group = line.Split('>')[0].Trim();
            string save_file = line.Split('>')[1].Trim();

            int count = group_files_list.Count;

            App.Structure.GroupFiles group = new App.Structure.GroupFiles();
            List<App.Structure.GroupFiles> list = new List<App.Structure.GroupFiles>();

            bool flag_group = true;
            bool flag_save_file = false;

            for (int i = 0; i < count; i++)
            {
                if(group_files_list[i].save_file != "")
                {
                    if (group_files_list[i].name == name_group && group_files_list[i].save_file == save_file)
                    {
                        flag_group = false;
                    }
                }
                else
                {
                    if (group_files_list[i].name == name_group)
                    {
                        flag_save_file = true;
                        flag_group = false;
                        list.Add(group_files_list[i]);
                    }
                }
            }

            if (flag_group)
            {
                group.name = name_group;
                group.save_file = save_file;
                group.folder = "";
                group.files = new List<string>();
                group_files_list.Add(group);
            }

            if (flag_save_file)
            {
                foreach(App.Structure.GroupFiles g in list)
                {
                    if (g.name == name_group)
                        g.save_file = save_file;
                }
            }
        }

        // Group files
        private void SetGroupFiles(string replace, string line)
        {
            line = line.Replace(replace, "").Trim();
            string name_group = line.Split('>')[0].Trim();
            string link_file = line.Split('>')[1].Trim();

            int count = group_files_list.Count;

            int count_group = 0;
            bool flag_group = false;

            App.Structure.GroupFiles group = new App.Structure.GroupFiles();
            List<App.Structure.GroupFiles> list = new List<App.Structure.GroupFiles>();

            for (int i = 0; i < count; i++)
            {
                if (group_files_list[i].name == name_group)
                {
                    count_group++;
                    flag_group = true;
                    list.Add(group_files_list[i]);
                }
            }

            if (flag_group)
            {
                foreach(App.Structure.GroupFiles g in list)
                {
                    if (!g.files.Contains(link_file))
                    {
                        g.files.Add(link_file);
                    }
                }
            }
            else
            {
                group.name = name_group;
                group.save_file = "";
                group.folder = "";
                group.files.Add(link_file);
                group_files_list.Add(group);
            }
        }

        // Group folder
        private void SetGroupFolder(string replace, string line)
        {
            line = line.Replace(replace, "").Trim();
            string group = line.Split('>')[0].Trim();
            string folder = line.Split('>')[1].Trim();

            bool flag = false;

            foreach(KeyValuePair<string, string> data in group_folder)
            {
                if(data.Key == group)
                {
                    flag = true;
                }
            }

            if (!flag)
            {
                group_folder[group] = folder;
            }
        }

        // To perform at the end of reading the file
        private void SetFolderEnd()
        {
            int count = files_list.Count;
            for (int i = 0; i < count; i++)
            {
                files_list[i].folder = folder;
            }

            foreach(App.Structure.GroupFiles g_files in group_files_list)
            {
                if (group_folder.ContainsKey(g_files.name))
                {
                    g_files.folder = group_folder[g_files.name];
                }
            }

            foreach (App.Structure.GroupFiles g_files in group_files_list)
            {
                if (g_files.folder == "")
                {
                    g_files.folder = folder;
                }
            }
        }
        #endregion
    }
}
