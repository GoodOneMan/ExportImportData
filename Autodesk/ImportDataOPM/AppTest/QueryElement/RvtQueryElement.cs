using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using System.IO;
using System.Runtime.InteropServices;

namespace ImportDataOPM.AppTest.QueryElement
{
    class RvtQueryElement
    {
        List<string> rvtFamilyList = null;
        List<string> rvtCategoryList = null;

        public RvtQueryElement()
        {
            GetListFamilyAndCategory();

            PublishFbxRvt("LcRevitPropertyElementFamily", rvtFamilyList.ToArray());
        }

        #region # 1
        private void GetListFamilyAndCategory()
        {
            rvtFamilyList = new List<string>();
            rvtCategoryList = new List<string>();

            ModelItemCollection selectCollection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;
            //ModelItemCollection searchCollection = SearchModelItems(selectCollection, "Объект", "LcRevitData_Element");
            ModelItemCollection searchCollection = SearchModelItems(selectCollection, "", "LcRevitData_Element");

            foreach (ModelItem item in searchCollection)
            {
                foreach (PropertyCategory propertyCategory in item.PropertyCategories)
                {
                    if (propertyCategory.Name == "LcRevitData_Element")
                    {
                        foreach (DataProperty property in propertyCategory.Properties)
                        {
                            if (property.Name == "LcRevitPropertyElementFamily")
                            {
                                if (!rvtFamilyList.Contains(property.Value.ToDisplayString()))
                                    rvtFamilyList.Add(property.Value.ToDisplayString());
                            }

                            if (property.Name == "LcRevitPropertyElementCategory")
                            {
                                if (!rvtCategoryList.Contains(property.Value.ToDisplayString()))
                                    rvtCategoryList.Add(property.Value.ToDisplayString());
                            }
                        }
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(@"D:\FamCat.txt"))
            {
                sw.WriteLine("Family");

                foreach (string f in rvtFamilyList)
                {
                    sw.WriteLine("     " + f);
                }

                sw.WriteLine("Category");

                foreach (string c in rvtCategoryList)
                {
                    sw.WriteLine("     " + c);
                }
            }
        }

        #endregion  

        #region Publish Fbx
        Autodesk.Navisworks.Api.Plugins.PluginRecord FBXPluginrecord = Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("NativeExportPluginAdaptor_LcFbxExporterPlugin_Export.Navisworks");

        private void PublishFbxRvt(string type, string[] values)
        {

            #region # 1
            //ModelItemCollection selectCollection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;

            //ComApi.InwOpState10 opState = ComBridge.State;

            //opState.SelectionHidden[ComBridge.ToInwOpSelection(selectCollection)] = true;

            //int count = 1;
            //foreach (string value in values)
            //{

            //    // Com
            //    //ComApi.InwOpSelection searchCollection = SearchModelItemsCom(selectCollection, "", "LcRevitData_Element", "", type, value);

            //    //opState.SelectionHidden[searchCollection] = false;
            //    //ExportRun(value + "_" + count.ToString());
            //    //opState.SelectionHidden[searchCollection] = true;

            //    //count++;
            //    //searchCollection = null;

            //    ModelItemCollection searchCollection = SearchModelItems(selectCollection, "", "LcRevitData_Element", "", type, value);

            //    ComApi.InwOpSelection selection = ComBridge.ToInwOpSelection(searchCollection);


            //    opState.SelectionHidden[ComBridge.ToInwOpSelection(searchCollection)] = false;
            //    //ExportRun(value + "_" + count.ToString());
            //    ExportRun(count.ToString());
            //    opState.SelectionHidden[ComBridge.ToInwOpSelection(searchCollection)] = true;
            //    System.Windows.Forms.MessageBox.Show("");
            //}
            #endregion

            #region # 2
            //ComApi.InwOpState10 opState = ComBridge.State;

            //ModelItemCollection selectCollection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;
            //ModelItemCollection searchCollection = SearchModelItems(selectCollection, "", "LcRevitData_Element");

            //ComApi.InwOpSelection selection = ComBridge.ToInwOpSelection(searchCollection);

            //selection.Invert();
            //opState.SelectionHidden[selection] = true;

            ////selection.Invert();
            ////opState.SelectionHidden[selection] = true;

            ////Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Clear();
            ////Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectAll();
            ////ModelItemCollection AllCollection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;

            //ModelItemCollection searchWorkerCollection = SearchModelItems(selectCollection, "", "LcRevitData_Element");
            //opState.SelectionHidden[ComBridge.ToInwOpSelection(searchWorkerCollection)] = true;

            //int count = 1;
            //foreach (string value in values)
            //{
            //    ComApi.InwOpSelection comCollection = SearchModelItemsCom(AllCollection, "", "LcRevitData_Element", "", type, value);
            //    opState.SelectionHidden[comCollection] = false;
            //    ExportRun(value + "_" + count.ToString());
            //    opState.SelectionHidden[comCollection] = true;

            //    System.Windows.Forms.MessageBox.Show("Ok");

            //    count++;
            //    comCollection = null;


            //    //ModelItemCollection comCollection = SearchModelItems(searchWorkerCollection, "", "LcRevitData_Element", "", type, value);
            //    //opState.SelectionHidden[ComBridge.ToInwOpSelection(comCollection)] = false;
            //    ////ExportRun(value + "_" + count.ToString());
            //    //opState.SelectionHidden[ComBridge.ToInwOpSelection(comCollection)] = true;
            //    //System.Windows.Forms.MessageBox.Show("Ok");

            //    //count++;
            //    //comCollection = null;
            //}

            #endregion

            #region # 3
            ComApi.InwOpState10 opState = ComBridge.State;
            ModelItemCollection selectCollection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;
            ModelItemCollection searchCollection = SearchModelItems(selectCollection, "", "LcRevitData_Element");
            ComApi.InwOpSelection selection = ComBridge.ToInwOpSelection(searchCollection);
            selection.Invert();
            opState.SelectionHidden[selection] = true;


            ModelItemCollection selectNewCollection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;

            foreach (string value in values)
            {
                ModelItemCollection comCollection = SearchModelItems(selectNewCollection, "", "LcRevitData_Element", "", type, value);
                opState.SelectionHidden[ComBridge.ToInwOpSelection(comCollection)] = true;
            }

            int count = 1;
            foreach (string value in values)
            {
                ModelItemCollection comCollection = SearchModelItems(selectNewCollection, "", "LcRevitData_Element", "", type, value);

                opState.SelectionHidden[ComBridge.ToInwOpSelection(comCollection)] = false;
                ExportRun(value + "_" + count.ToString());
                opState.SelectionHidden[ComBridge.ToInwOpSelection(comCollection)] = true;

                count++;
                comCollection = null;
            }
            #endregion
        }

        private void ExportRun(string name)
        {
            try
            {
                if (FBXPluginrecord != null)
                {
                    if (!FBXPluginrecord.IsLoaded)
                    {
                        FBXPluginrecord.LoadPlugin();
                    }

                    string[] pa = { "D:\\ExportFbx\\" + name + "_.fbx" };

                    Autodesk.Navisworks.Internal.ApiImplementation.NativeExportPluginAdaptor FBXplugin = FBXPluginrecord.LoadedPlugin as Autodesk.Navisworks.Internal.ApiImplementation.NativeExportPluginAdaptor;
                   
                    FBXplugin.Execute(pa);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        #endregion


        #region Search item
        
        private ModelItemCollection SearchModelItems(ModelItemCollection coll, string userCategory, string internalCategory)
        {
            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internalCategory, userCategory);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }

        private ModelItemCollection SearchModelItems(ModelItemCollection coll, string userCategory, string internalCategory, string userProperty, string intrenalProperty, string propertyValue)
        {
            dynamic value = null;

            try
            {
                value = Convert.ToInt32(propertyValue);
            }
            catch
            {
                value = propertyValue;
            }

            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internalCategory, userCategory);
            opFindCondition.SetPropertyNames(intrenalProperty, userProperty);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            opFindCondition.value = value;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }

        private ComApi.InwOpSelection SearchModelItemsCom(ModelItemCollection coll, string userCategory, string internalCategory, string userProperty, string intrenalProperty, string propertyValue)
        {
            dynamic value = null;

            try
            {
                value = Convert.ToInt32(propertyValue);
            }
            catch
            {
                value = propertyValue;
            }

            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internalCategory, userCategory);
            opFindCondition.SetPropertyNames(intrenalProperty, userProperty);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            opFindCondition.value = value;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return opSelSet2.selection;
        }

        private ComApi.InwOpSelection SearchModelItemsCom(ModelItemCollection coll, string userCategory, string internalCategory)
        {
            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internalCategory, userCategory);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;
            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return opSelSet2.selection;
        }
        #endregion

    }
}
