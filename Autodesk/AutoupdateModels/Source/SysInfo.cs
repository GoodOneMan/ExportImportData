using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Threading;

namespace AutoupdateModels.Source
{
    public class FreeMemoryEventArgs : EventArgs
    {
        public long free_size_memory { get; set; } 
    }

    public delegate void FreeMemoryDelegat(object sender, FreeMemoryEventArgs e);

    class SysInfo
    {
        // Event free memory loop
        public event FreeMemoryDelegat FreeMemoryEvent;

        // Get free memory counter
        public void GetFreeMemoryCounter()
        {
            long free_memory = 0;
            bool flag = true;

            while (flag)
            {
                if(FreeMemoryEvent != null)
                {
                    PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    free_memory = ramCounter.RawValue * 1024 * 1024;
                    FreeMemoryEventArgs e = new FreeMemoryEventArgs() { free_size_memory = free_memory };
                    FreeMemoryEvent(this, e);
                }

                Thread.Sleep(1500);
            }
        }
        
        // returning free memory space
        public static long GetFreeMemory()
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            return ramCounter.RawValue;
        }

        // a refund of the full memory space
        public static ulong GetTotalMemory()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

    }
}
