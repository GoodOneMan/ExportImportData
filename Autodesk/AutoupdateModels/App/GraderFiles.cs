using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoupdateModels.App
{
    class GraderFiles
    {
        string temp_one_dir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "_temp_one" + Path.DirectorySeparatorChar;
        string temp_group_dir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "_temp_group" + Path.DirectorySeparatorChar;
        Source.Config config = Source.Config.Instance;
        List<string> message = new List<string>();

        public GraderFiles()
        {
            CheckerFiles();
            CheckerGroupFiles();
        }

        #region Files
        private void CheckerFiles()
        {
            foreach(App.Structure.Files Files in config.files_list)
            {
                if(!Directory.Exists(Files.folder))
                {
                    if(IsLockedDirectory(Files.folder))
                    {
                        message.Add(Files.folder + " not found");
                        if(!IsLockedDirectory(temp_one_dir))
                        {
                            Files.folder = temp_one_dir;
                            message.Add("files will be moved to " + Files.folder);
                        }
                    }
                }

                List<string> list = new List<string>();
                foreach(string file in Files.files)
                {
                    if(File.Exists(file))
                    {
                        list.Add(file);
                    }
                    else
                    {
                        message.Add(file + " file not found");
                    }
                }

                Files.files = list;
            }
        }
        #endregion

        #region GroupFiles
        private void CheckerGroupFiles()
        {
            foreach (App.Structure.GroupFiles GroupFiles in config.group_files_list)
            {
                if (!Directory.Exists(GroupFiles.folder))
                {
                    if (IsLockedDirectory(GroupFiles.folder))
                    {
                        message.Add(GroupFiles.folder + " not found");
                        if (!IsLockedDirectory(temp_one_dir))
                        {
                            GroupFiles.folder = temp_group_dir;
                            message.Add("files will be moved to " + GroupFiles.folder);
                        }
                    }
                }

                List<string> list = new List<string>();
                foreach(string file in GroupFiles.files)
                {
                    if (File.Exists(file))
                    {
                        list.Add(file);
                    }
                    else
                    {
                        message.Add(file + " file not found");
                    }
                }

                GroupFiles.files = list;
            }
        }
        #endregion

        #region Helper method
        public List<string> GetMessage()
        {
            return message;
        }

        private bool IsLockedDirectory(string path)
        {
            bool flag = true;
            try
            {
                Directory.CreateDirectory(path);
                flag = false;
            }
            catch(Exception ex)
            {
                message.Add(ex.Message);
            }
            return flag;
        }
        #endregion
    }
}
