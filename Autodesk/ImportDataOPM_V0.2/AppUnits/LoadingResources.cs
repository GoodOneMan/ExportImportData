using ImportDataOPM.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImportDataOPM.AppUnits
{
    class LoadingResources
    {
        public LoadingResources()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AppDomain_AssemblyResolve;
        }

        private Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("DataModelBentleyOPM"))
            {
                Console.WriteLine("Resolving assembly: {0}", args.Name);

                // Загрузка запакованной сборки из ресурсов, ее распаковка и подстановка
                using (var resource = new MemoryStream(Resources.DataModelBentleyOPM_dll))
                using (var deflated = new DeflateStream(resource, CompressionMode.Decompress))
                using (var reader = new BinaryReader(deflated))
                {
                    var one_megabyte = 1024 * 1024;
                    var buffer = reader.ReadBytes(one_megabyte);
                    return Assembly.Load(buffer);
                }
            }

            return null;
        }
    }
}
