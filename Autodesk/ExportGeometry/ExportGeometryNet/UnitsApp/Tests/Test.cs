using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.Navisworks.Api;
using System.Runtime.InteropServices;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using System.Diagnostics;
using System.Numerics;

namespace ExportGeometry.UnitsApp.Tests
{
    //
    class Test
    {
        ComApi.InwOpState3 opState = ComApiBridge.State;

        StreamWriter sw = new StreamWriter(@"D:\section.txt");

        string start_path = @"D:\UpTest\";

        string cat_internal_name = "ReferenceProps";
        string cat_user_name = "Ссылка";
        string prop_user_name = "Имя пути";

        List<Model_s> list = new List<Model_s>();
        List<Model_s> new_list = new List<Model_s>();

        public Test()
        {
            Run();
            //Run_1();
            //Run_2();
            sw.Close();
        }
        
        private void Run()
        {
            ModelItemCollection search_coll = _search_reference(cat_internal_name, cat_user_name);

            foreach(ModelItem model_item in search_coll)
            {
                //ModelItemCollection coll = new ModelItemCollection();
                //coll.CopyFrom(model_item.Children);
                //Selection selection = new Selection();
                //selection.CopyFrom(coll);

                //DoWork

                string dir_path = start_path + Path.GetFileNameWithoutExtension(FileName(model_item)) + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(dir_path);
                WriteToFile(model_item, dir_path, "link.txt", true);

                ModelItemCollection coll = GetReferenceCollection(model_item);

                if (coll.Count != 0)
                {
                    RecurtionModelItem(coll, dir_path);
                }
            }
        }
        
        private void RecurtionModelItem(ModelItemCollection in_coll, string path)
        {
            foreach(ModelItem model_item in in_coll)
            {
                ModelItemCollection out_coll = GetReferenceCollection(model_item);
                if(out_coll.Count != 0)
                {
                    string dir_path = path + Path.GetFileNameWithoutExtension(FileName(model_item)) + Path.DirectorySeparatorChar;
                    Directory.CreateDirectory(dir_path);

                    RecurtionModelItem(out_coll, dir_path);
                }
                
                WriteToFile(model_item, path, "link.txt", true);
            }
        }

        private string FileName(ModelItem model_item)
        {
            DataProperty dp = model_item.PropertyCategories.FindPropertyByDisplayName(cat_user_name, prop_user_name);

            if(dp != null)
            {
                return dp.Value.ToDisplayString();
            }

            return "";
        }

        private void WriteToFile(ModelItem model_item, string path, string file, bool apend)
        {
            StreamWriter sw = new StreamWriter(path + file, apend);
            sw.WriteLine(FileName(model_item));
            sw.Close();
        }

        private ModelItemCollection GetReferenceCollection(ModelItem model_item)
        {
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Clear();
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Add(model_item);
            return _search_reference(cat_internal_name, cat_user_name);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        #region 1
        private void Run_2()
        {
            Init();

            //foreach (Model_s mod in list)
            //{
            //    sw.WriteLine(mod.index);
            //    sw.WriteLine(mod.name);
            //    sw.WriteLine(mod.path);
            //}

            HandlerList();
        }

        private void Run_1()
        {

            //ModelItemCollection collection_finder = _search_reference(cat_internal_name, cat_user_name);

            //foreach (ModelItem model_item in collection_finder)
            //{
            //    DataProperty data_propery = model_item.PropertyCategories.FindPropertyByDisplayName(cat_user_name, prop_user_name);

            //    if (data_propery != null)
            //    {
            //        string path_o = start_path + Path.GetFileNameWithoutExtension(data_propery.Value.ToDisplayString());

            //        Directory.CreateDirectory(path_o);

            //        if (model_item.Children.Count() != 0)
            //        {
            //            CrDir(model_item, path_o);
            //        }

            //        //sw.WriteLine(data_propery.Value.ToDisplayString());
            //    }
            //}


            ModelItemCollection collection_finder_1 = _search_ref_part();

        }

        private void Init()
        {
            ModelItemCollection refer_coll = _search_reference(cat_internal_name, cat_user_name);
            //ModelItemCollection refer_coll = _search_ref_part();

            foreach (ModelItem model_item in refer_coll)
            {
                DataProperty data_propery = model_item.PropertyCategories.FindPropertyByDisplayName(cat_user_name, prop_user_name);

                if (data_propery != null)
                {
                    Model_s model = new Model_s();

                    model.arr_data = GetArrayData(model_item);
                    model.name = Path.GetFileNameWithoutExtension(data_propery.Value.ToDisplayString());
                    model.path = data_propery.Value.ToDisplayString();
                    model.parent = model_item.Parent.DisplayName;
                    list.Add(model);
                }
            }
        }

        // to position the model in the file
        private Array GetArrayData(ModelItem model_item)
        {
            ComApi.InwOaPath3 opPath3 = ComApiBridge.ToInwOaPath(model_item) as ComApi.InwOaPath3;
            Array array_data = (Array)(object)opPath3.ArrayData;
            return array_data;
        }

        private ModelItemCollection _search_reference(string internal_name = "", string user_name = "")
        {
            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internal_name, user_name);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = ComApiBridge.ToInwOpSelection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);

            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            //opFindSpec.ResultDisjoint = false;
            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComApiBridge.ToModelItemCollection(opSelSet2.selection);
        }

        private ModelItemCollection _search_ref_part()
        {
            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";
            
            ComApi.InwOpFindSpec opFindSpec = opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec) as ComApi.InwOpFindSpec;

            ComApi.InwOpFindCondition findCondition1 = opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition) as ComApi.InwOpFindCondition;
            ComApi.InwOpFindCondition findCondition2 = opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition) as ComApi.InwOpFindCondition;

            findCondition1.StartGroup = false;
            findCondition1.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;
            findCondition1.SetAttributeNames("ReferenceProps", "Ссылка");


            findCondition2.StartGroup = false;
            findCondition2.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            findCondition2.SetAttributeNames("LcOaNode", "Элемент");
            findCondition2.SetPropertyNames("LcOaSceneBaseClassName", "Внутренний тип");
            findCondition2.value = "LcOaPartition";

            opFindSpec.selection = ComApiBridge.ToInwOpSelection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);

            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_ALL_PATHS;
            opFindSpec.ResultDisjoint = false;

            //add first find condition
            opFindSpec.Conditions().Add(findCondition1);
            //add second find condition
            opFindSpec.Conditions().Add(findCondition2);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComApiBridge.ToModelItemCollection(opSelSet2.selection);
        }

        private void CrDir(ModelItem model_item_in, string path)
        {
            foreach (ModelItem model_item in model_item_in.Children)
            {
                if (!model_item.IsLayer)
                {
                    DataProperty data_propery = model_item.PropertyCategories.FindPropertyByDisplayName(cat_user_name, prop_user_name);
                    if (data_propery != null)
                    {
                        Directory.CreateDirectory(path + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(data_propery.Value.ToDisplayString()));

                        if (model_item.Children.Count() != 0)
                        {
                            CrDir(model_item, path + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(data_propery.Value.ToDisplayString()));
                        }
                    }
                }
            }
        }

        private void HandlerList()
        {
            foreach(Model_s mod in list)
            {
                RecurtionModel(mod, mod.arr_data.Length);
            }
        }

        private void RecurtionModel(Model_s mod, int index)
        {
            foreach(Model_s mod1 in list)
            {
                if(index == mod1.arr_data.Length)
                {
                    if(mod.arr_data.GetValue(index) != mod1.arr_data.GetValue(index))
                    {
                        if (!new_list.Contains(mod1))
                        {
                            new_list.Add(mod1);
                            sw.WriteLine(mod1.name);
                        }
                    }
                }
            }
        }
        #endregion
    }


    struct Model_s
    {
        public Array arr_data;
        public string path;
        public string name;
        public string parent;
    }


}
