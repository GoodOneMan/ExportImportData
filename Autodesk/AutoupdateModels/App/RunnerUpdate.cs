using Autodesk.Navisworks.Api.Automation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoupdateModels.App
{
    // Start app
    class RunnerUpdate
    {
        // in data
        public List<Structure.SingleModel> _single_pool_list = new List<Structure.SingleModel>();
        public List<Structure.GroupModel> _group_pool_list = new List<Structure.GroupModel>();

        // out data
        public List<Structure.Model> _model_pool_list = new List<Structure.Model>();

        // Initialization pool models
        public RunnerUpdate()
        {
            App.Autoupdate _app_one = new Autoupdate();
            _single_pool_list = _app_one.GetPool();

            App.AutoapdateGroup _app_group = new AutoapdateGroup();
            _group_pool_list = _app_group.GetPool();
        }

        // Runner update
        public void Run()
        {
            #region display information on the screen
            //updating and primary collection of model data
            Source.Display.Show("updating and primary collection of model data", Source.DisplayColor.primary, 2, "... ", " ");
            #endregion

            if (_single_pool_list.Count != 0)
            {
                // single model
                RunSingleModelUpdate();
            }

            if(_group_pool_list.Count != 0)
            {
                // group model
                RunGroupModelUpdate();
            }

            #region display information on the screen
            //update the models in the competing streams
            Source.Display.Show("update the models in the competing streams", Source.DisplayColor.primary, 2, "... ", " ");
            #endregion

            if (_model_pool_list.Count != 0)
            {
                // !!!
                // running upgrade of models to the competing 
                // flows with calculation of possible load memory
                // { this code !!! }
                // 

                new CompetitorProcess(_model_pool_list);
            }
        }

        #region Run update
        // Single model
        private void RunSingleModelUpdate()
        {
            foreach(Structure.SingleModel model in _single_pool_list)
            {
                if (model.flag_first)
                {
                    model.flag_first = false;
                    new ModelUpdate(model).RunUpdate();
                }

                _model_pool_list.Add(model);
            }
        }

        // Group model
        private void RunGroupModelUpdate()
        {
            foreach (Structure.GroupModel model in _group_pool_list)
            {
                if (model.flag_first)
                {
                    model.flag_first = false;
                    new ModelUpdate(model).RunUpdate();
                }

                _model_pool_list.Add(model);
            }
        }
        #endregion
    }

    #region Class Model Update
    // Class for universal model
    class ModelUpdate
    {
        Structure.Model model;
        Source.Config config = Source.Config.Instance;

        public ModelUpdate(Structure.Model _model)
        {
            model = _model;
        }

        // Run update single model
        public void RunUpdate()
        {
            Task max_memory_task = null;

            try
            {
                // Time start
                model.start_app_time = DateTime.Now;

                #region display information on the screen
                Source.Display.Show("start the update of the model", Source.DisplayColor.primary, 0, "", " ");
                Source.Display.Show(model.out_file, Source.DisplayColor.success, 0, "", " ");
                Source.Display.Show(model.start_app_time.ToString("dd.MM HH:mm:ss"), Source.DisplayColor.secondary, 1);
                #endregion

                // Clear and set list process id
                Source.TrackingProcess.ResetListId();

                // Create new roamer
                model.roamer = new NavisworksApplication();
                // Get id process roamer
                model.id_process = Source.TrackingProcess.GetIdProcess();

                #region get max memory size

                // Run task for get max memory size
                Source.TrackingProcess roamer = new Source.TrackingProcess(model.id_process);
                roamer.TrackMaximumSizeEvent += GetMaxSize_TrackMaximumSizeEvent;
                //Task.Factory.StartNew(roamer.CalculateMemory);
                max_memory_task = new Task(roamer.CalculateMemory);
                max_memory_task.Start();
                #endregion

                // Disable progress
                model.roamer.DisableProgress();

                // Open file
                if (model.type == "single_model")
                {
                    var m = model as Structure.SingleModel;

                    #region display information on the screen
                    Source.Display.Show("file reference", Source.DisplayColor.primary, 0, "", " ");
                    Source.Display.Show(m.in_folder + m.in_file, Source.DisplayColor.secondary, 1);
                    #endregion

                    model.roamer.OpenFile(m.in_folder + m.in_file);
                }
                else if(model.type == "group_model")
                {
                    var m = model as Structure.GroupModel;
                    foreach (var f in m.files)
                    {
                        #region display information on the screen
                        Source.Display.Show("file reference", Source.DisplayColor.primary, 0, "", " ");
                        Source.Display.Show(f.in_folder + f.in_file, Source.DisplayColor.secondary, 1);
                        #endregion

                        model.roamer.AppendFile(f.in_folder + f.in_file);
                    }
                }

                // Calling plugin navisworks
                //try { new Source.PluginCall(_autoApp, null); }
                //catch (Exception ex) { _config.Display(ex.Message, "error", 2); }

                // Save file
                if (SaveFile(model))
                {
                    model.end_app_time = DateTime.Now;
                    model.size_file = new FileInfo(model.out_folder + model.out_file).Length;
                    model.build_app_time = DateTime.Now.Subtract(model.start_app_time);

                    #region display information on the screen
                    Source.Display.Show("the model", Source.DisplayColor.primary, 0, "", " ");
                    Source.Display.Show(model.out_file, Source.DisplayColor.success, 0, "", " ");
                    Source.Display.Show("is saved", Source.DisplayColor.primary, 0, "", " ");
                    Source.Display.Show(model.end_app_time.ToString("dd.MM HH:mm:ss"), Source.DisplayColor.secondary, 1);
                    // file size
                    // build time
                    Source.Display.Show("file size", Source.DisplayColor.primary, 0, "", ": ");
                    Source.Display.Show(Source.Helper.SizeMemory(model.size_file), Source.DisplayColor.warning, 0, "", "  ");
                    Source.Display.Show("build time", Source.DisplayColor.primary, 0, "", ": ");
                    Source.Display.Show(model.build_app_time.ToString(@"hh\:mm\:ss"), Source.DisplayColor.secondary, 1);
                    // the path to save the file
                    Source.Display.Show("the path to save the file", Source.DisplayColor.primary, 0, "", ": ");
                    Source.Display.Show(model.out_folder, Source.DisplayColor.warning, 1);
                    #endregion


                    // Broadcast message
                    // { Code this}
                }
                else
                {

                    #region display information on the screen

                    #endregion

                    // Process copy file to server
                    lock (Source.Config.locker)
                    {
                        if (!Source.Config.locked_file.Contains(model.out_file))
                        {
                            Source.Config.locked_file.Add(model.out_file);

                            // Task for copy files to server and callback responce
                        }
                    }
                }
            }
            catch (AutomationException ex)
            {
                Source.Display.Show("Error: " + ex.Message, Source.DisplayColor.danger, 2);
            }
            catch (AutomationDocumentFileException ex)
            {
                Source.Display.Show("Error: " + ex.Message, Source.DisplayColor.danger, 2);
            }
            catch (Exception ex)
            {
                Source.Display.Show("Error: " + ex.Message, Source.DisplayColor.danger, 2);
            }
            finally
            {
                if (model.roamer != null)
                {
                    model.roamer.Dispose();
                    model.roamer = null;
                }
            }

            // to set the maximum memory size
            //Thread.Sleep(1000);
            if(max_memory_task != null)
                max_memory_task.Wait();

            //null id
            model.id_process = 0;
            
            #region display information on the screen
            // show max memory size
            Source.Display.Show("the maximum size of used memory", Source.DisplayColor.primary, 0, "", ": ");
            Source.Display.Show(Source.Helper.SizeMemory(model.max_size_memory), Source.DisplayColor.warning, 2);
            #endregion
        }

        // Save file
        private bool SaveFile(Structure.Model model)
        {
            bool flag = false;
            string file = model.out_folder + model.out_file;

            try
            {
                if (File.Exists(file))
                {
                    // File is locked
                    if (!Source.Helper.FileIsLocked(file, FileAccess.Write))
                    {
                        flag = true;
                        model.roamer.SaveFile(file);
                    }
                    else
                    {
                        string tpm_file = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + model.out_file;
                        model.roamer.SaveFile(tpm_file);
                    }
                }
                else
                {
                    flag = true;
                    model.roamer.SaveFile(file);
                }
            }
            catch (Exception ex)
            {
                flag = false;
                Source.Display.Show(ex.Message, Source.DisplayColor.danger, 2);
            }
            return flag;
        }

        // Process memory size
        private void GetMaxSize_TrackMaximumSizeEvent(object sender, Source.MemoryEventArgs e)
        {
            model.max_size_memory = e.MaxSizeMemory;
        }
    }
    #endregion
}
