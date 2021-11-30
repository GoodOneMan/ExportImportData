using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;

namespace ImportDataOPM.AppTest.ModelStructure
{
    class TestOne
    {
        Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
        StreamWriter sw = null;

        public TestOne()
        {
            sw = new StreamWriter(@"D:\dgnLevel.txt");
            RunTest_2();
            sw.Close();
        }

        private void RunTest_1()
        {
            List<string> compareData = new List<string>();

            ModelItemCollection modelItems = doc.CurrentSelection.SelectedItems;

            string userCategory = "Элемент";
            string internalCategory = "LcOaNode";
            string userProperty = "Внутренний тип";
            string intrenalProperty = "LcOaSceneBaseClassName";
            string propertyValue = "LcDgnLevel";

            ModelItemCollection modelItemsResult = SearchModelItems(modelItems, userCategory, internalCategory, userProperty, intrenalProperty, propertyValue);

            

            foreach (ModelItem modelItem in modelItemsResult)
            {
                //sw.WriteLine(modelItem.Parent.DisplayName + " " + modelItem.Parent.ClassDisplayName + " " + modelItem.Parent.ClassName + " " + modelItem.DisplayName);
                string str = modelItem.Parent.DisplayName + " " + modelItem.Parent.ClassName + " " + modelItem.DisplayName;

                if (!compareData.Contains(str))
                {
                    compareData.Add(str);
                }
                else
                {
                    sw.WriteLine(str);
                }
            }

            
        }

        private void RunTest_2()
        {
            ModelItemCollection modelItems = doc.CurrentSelection.SelectedItems;
            string userCategory = "Элемент";
            string internalCategory = "LcOaNode";
            string userProperty = "Внутренний тип";
            string intrenalProperty = "LcOaSceneBaseClassName";
            string propertyValue = "LcDgnLevel";
            ModelItemCollection modelItemsResult = SearchModelItems(modelItems, userCategory, internalCategory, userProperty, intrenalProperty, propertyValue);

            List<StructOne> list = new List<StructOne>();

            foreach(ModelItem modelItem in modelItemsResult)
            {
                ModelItem parentItem = modelItem.Parent;

                string type = "";
                string link = "";

                if(parentItem.ClassName == "LcDgnReference")
                {
                    link = parentItem.PropertyCategories.FindPropertyByCombinedName(new NamedConstant("ReferenceProps", "Ссылка"), new NamedConstant("Pathname", "Имя пути")).Value.ToDisplayString().ToLower();
                    type = "LcDgnReference";
                }
                else if(parentItem.ClassName == "LcOaPartition")
                {
                    link = parentItem.PropertyCategories.FindPropertyByCombinedName(new NamedConstant("LcOaNode", "Элемент"), new NamedConstant("LcOaPartitionSourceFilename", "Имя файла источника")).Value.ToDisplayString().ToLower();
                    type = "LcOaPartition";
                }

                StructOne item = list.FirstOrDefault(strOne => strOne.Type == type && strOne.Link == link);

                if(item != null)
                {
                    item.CollectionLevel.Add(modelItem);
                }
                else
                {
                    item = new StructOne()
                    {
                        Type = type,
                        Link = link
                    };

                    item.CollectionLevel.Add(modelItem);
                    list.Add(item);
                }
            }

            int countlistItem = list.Count;


            foreach(var StructOneItem in list)
            {
                sw.WriteLine(StructOneItem.Link);
            }

        }

        private ModelItemCollection SearchModelItems(ModelItemCollection coll, string userCategory, string internalCategory, string userProperty, string intrenalProperty, string propertyValue)
        {

            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internalCategory, userCategory);
            opFindCondition.SetPropertyNames(intrenalProperty, userProperty);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            opFindCondition.value = propertyValue;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }
    }

    class StructOne
    {
        public string Type { get; set; }
        public string Link { get; set; }
        public List<ModelItem> CollectionLevel { get; set; }

        public StructOne()
        {
            CollectionLevel = new List<ModelItem>();
        }
    } 
}
