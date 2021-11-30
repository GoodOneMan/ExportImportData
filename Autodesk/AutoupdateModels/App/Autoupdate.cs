using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.Navisworks.Api.Automation;
using System.Threading;
using System.Diagnostics;

namespace AutoupdateModels.App
{
    #region Autoupdate
    // To assemble a single file
    // step 1 initialization pool object Structure.AutoApp
    class Autoupdate
    {
        Source.Config _config = Source.Config.Instance;
        List<Structure.SingleModel> _pool_list = new List<Structure.SingleModel>();
        
        public Autoupdate()
        {
            foreach(App.Structure.Files Files in _config.files_list)
            {
                foreach(string file in Files.files)
                {
                    // Init structure
                    Structure.SingleModel _app = new Structure.SingleModel();

                    // type model
                    _app.type = "single_model";
                    // Set file and folder
                    _app.in_file = Path.GetFileName(file);
                    _app.out_file = Path.GetFileNameWithoutExtension(file) + ".nwd";
                    _app.in_folder = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar;
                    _app.out_folder = Files.folder;

                    // Add in pool
                    _pool_list.Add(_app);
                }
            }
        }
        
        public List<Structure.SingleModel> GetPool()
        {
            return _pool_list;
        }
    }
    #endregion

    #region AutoapdateGroup
    // To collect a group of files
    class AutoapdateGroup
    {
        Source.Config _config = Source.Config.Instance;
        List<Structure.GroupModel> _pool_list = new List<Structure.GroupModel>();

        public AutoapdateGroup()
        {
            foreach (App.Structure.GroupFiles GroupFiles in _config.group_files_list)
            {
                Structure.GroupModel _app = new Structure.GroupModel();
                // type model
                _app.type = "group_model";
                // Set file and folder
                _app.out_file = GroupFiles.save_file;
                _app.out_folder = GroupFiles.folder;

                foreach(string file in GroupFiles.files)
                {
                    Structure.GroupModel.Files f = new Structure.GroupModel.Files();

                    f.in_file = Path.GetFileName(file);
                    f.in_folder = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar;

                    _app.files.Add(f);
                }

                _pool_list.Add(_app);
            }
        }

        public List<Structure.GroupModel> GetPool()
        {
            return _pool_list;
        }
    }
    #endregion
}
