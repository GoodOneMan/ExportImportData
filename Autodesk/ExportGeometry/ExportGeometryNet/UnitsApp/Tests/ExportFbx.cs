using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api;
using System.Runtime.InteropServices;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;

namespace ExportGeometry.UnitsApp.Tests
{
    class ExportFbx
    {
        Autodesk.Navisworks.Api.Plugins.PluginRecord FBXPluginrecord = Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("NativeExportPluginAdaptor_LcFbxExporterPlugin_Export.Navisworks");

        public ExportFbx()
        {
            SelectItems();
        }

        private void SelectItems()
        {
            ComApi.InwOpState10 opState = ComApiBridge.State;

            string internal_name = "LcOpGeometryProperty";
            string user_name = "Геометрия";


            ComApi.InwOpSelection selection = new FI.FinderItem()._SearchByCategory(internal_name, user_name);

            opState.SelectionHidden[selection] = true;
            
            ComApi.InwSelectionPathsColl paths = selection.Paths();

            ModelItemCollection model_collection = new ModelItemCollection();

            int name = 0;
            int count_item = 0;

            int count = paths.Count;

            if (count > 0)
            {
                var enum_paths = paths.GetEnumerator();
                while (enum_paths.MoveNext())
                {
                    ComApi.InwOaPath3 path = enum_paths.Current as ComApi.InwOaPath3;
                    model_collection.Add(ComApiBridge.ToModelItem(path));

                    name++;
                    count_item++;
                    if (count_item == 400)
                    {
                        opState.SelectionHidden[ComApiBridge.ToInwOpSelection(model_collection)] = false;
                        ExportRun(name.ToString());
                        opState.SelectionHidden[ComApiBridge.ToInwOpSelection(model_collection)] = true;

                        model_collection = new ModelItemCollection();
                        count_item = 0;
                    }
                }
            }
        }

        private void ExportRun(string name)
        {
            if (FBXPluginrecord != null)
            {
                if (!FBXPluginrecord.IsLoaded)
                {
                    FBXPluginrecord.LoadPlugin();
                }

                //save path of the FBX
                string[] pa = { "D:\\model_fbx\\" + name + "_.fbx" };

                //way 1: by base class of plugin

                //Plugin FBXplugin = FBXPluginrecord.LoadedPlugin as Plugin;


                //FBXplugin.GetType().InvokeMember("Execute", System.Reflection.BindingFlags.InvokeMethod, null, FBXplugin, pa);

                //way 2: by specific class of export plugin

                Autodesk.Navisworks.Internal.ApiImplementation.NativeExportPluginAdaptor FBXplugin = FBXPluginrecord.LoadedPlugin as Autodesk.Navisworks.Internal.ApiImplementation.NativeExportPluginAdaptor;

                
                FBXplugin.Execute(pa);
            }
        }
    }
}
