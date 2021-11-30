using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoupdateModels.App
{
    class CompetitorProcess
    {
        #region property
        // List model
        List<Structure.Model> _model = null;
        // Total memory
        long _total_physical_memory = 0;
        // SysInfo
        Source.SysInfo _sys_info = null;
        // List int process id
        List<int> process_id = null;
        // Array max_size and index
        Dictionary<long, int> buffer = null;
        #endregion


        public CompetitorProcess(List<Structure.Model> model)
        {
            _model = SortByMaxSize(model);
            _total_physical_memory = (long)Source.SysInfo.GetTotalMemory();

            _sys_info = new Source.SysInfo();
            _sys_info.FreeMemoryEvent += _sys_info_FreeMemoryEvent;
            Task.Factory.StartNew(_sys_info.GetFreeMemoryCounter);
        }




        // Sort list by max size
        public List<Structure.Model> SortByMaxSize(List<Structure.Model> models)
        {
            List<Structure.Model> list = new List<Structure.Model>();

            var sortModel = from m in models
                            orderby m.max_size_memory descending
                            select m;

            foreach (Structure.Model model in sortModel)
                list.Add(model);

            return list;
        }

        // Get free memory
        private void _sys_info_FreeMemoryEvent(object sender, Source.FreeMemoryEventArgs e)
        {
            Source.Display.Show(Source.Helper.SizeMemory(e.free_size_memory), Source.DisplayColor.warning, 1);
        }
    }
}
