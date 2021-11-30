using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoupdateModels.Source
{
    // Event memory size
    public class MemoryEventArgs : EventArgs
    {
        public long MaxSizeMemory { get; set; }
    }

    // Delegat
    public delegate void MaxSizeMemoryDelegat(object sender, MemoryEventArgs e);

    // Process roamer
    class TrackingProcess
    {
        #region property
        // This process
        Process process = null;
        // All processes (init this !!!)
        static List<int> processes_id = new List<int>();
        // Internal data
        int id = 0;
        // Event
        public event MaxSizeMemoryDelegat TrackMaximumSizeEvent = null;
        #endregion

        // Tracking process
        public TrackingProcess(int _id)
        {
            id = _id;
            process = Process.GetProcessById(id);
            
        }

        #region Calculate memory
        public void CalculateMemory()
        {
            long _tmp_memory = 0;
            long _current_memory = 0;

            bool flag = true;

            while (flag)
            {
                if (process.HasExited)
                    flag = false;

                if (!process.HasExited)
                {
                    try
                    {
                        _current_memory = GetProcessPrivateWorkingSet64Size(process);
                        if (_tmp_memory > _current_memory)
                        {
                            Thread.Sleep(100);
                        }
                        else
                        {
                            _tmp_memory = _current_memory;
                        }
                    }
                    catch
                    {
                        flag = false;
                    }
                }
            }

            if (TrackMaximumSizeEvent != null)
            {
                var e = new MemoryEventArgs { MaxSizeMemory = _tmp_memory };
                TrackMaximumSizeEvent(this, e);
            }
        }

        public static long GetProcessPrivateWorkingSet64Size(Process roamer)
        {
            long process_size = 0;
            if (roamer == null)
                return process_size;
            string instanceName = GetProcessInstanceName(roamer.Id);
            var counter = new PerformanceCounter("Process", "Working Set - Private", instanceName, true);
            process_size = counter.RawValue;
            return process_size;
        }

        public static string GetProcessInstanceName(int process_id)
        {
            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
            string[] instances = cat.GetInstanceNames();
            foreach (string instance in instances)
            {
                using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true))
                {
                    try
                    {
                        int val = (int)cnt.RawValue;
                        if (val == process_id)
                            return instance;
                    }
                    catch{}
                }
            }
            throw new Exception("Could not find performance counter ");
        }
        #endregion

        // Get Proccess id
        public static int GetIdProcess()
        {
            int id = 0;
            Process[] processes = Process.GetProcesses();
            int count = processes.Length;
            for (int i = 0; i < count; i++)
            {
                if (processes[i].ProcessName == "Roamer")
                {
                    bool flag = true;
                    foreach (int pid in processes_id)
                    {
                        if (pid == processes[i].Id)
                            flag = false;
                    }
                    if (flag)
                        id = processes[i].Id;
                }
            }
            return id;
        }
        
        // Reset list id
        public static void ResetListId()
        {
            processes_id.Clear();

            Process[] processes = Process.GetProcesses();
            foreach(Process process in processes)
            {
                if (process.ProcessName == "Roamer")
                {
                    lock (processes_id)
                    {
                        processes_id.Add(process.Id);
                    }
                }
            }
        }
    }
}
