using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;


namespace ImportDataOPM.AppTest
{
    class ModelTest
    {
        DataModelBentleyOPM.Model model = null;
        string fileModel = null;

        Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
        ModelItemCollection collection = null;
        AppUnits.MessageForm messageForm = null;
        int countElement = 0;

        public ModelTest()
        {
            LoadFileModel();

            messageForm = new AppUnits.MessageForm();
            messageForm.Show();

            BinaryDeserialize();
            Init();

            messageForm.Close();

            MessageBox.Show(" свойства перенесены ", "импорт завершен", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        #region # Get Model
        public DataModelBentleyOPM.Model GetModel()
        {
            return model;
        }

        private void BinaryDeserialize()
        {
            messageForm.SetHeader("Файл с данными загружается");

            if (File.Exists(fileModel))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream(fileModel, FileMode.OpenOrCreate))
                {
                    model = (DataModelBentleyOPM.Model)formatter.Deserialize(fs);
                }
            }
        }
        
        private void LoadFileModel()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileModel = openFileDialog.FileName;
            }
        }
        #endregion

        #region # Model handler
        // step 1 get selection item
        // step 2 search reference dgn
        // Элемент LcOaNode -> Внутренний тип (LcOaSceneBaseClassName):LcDgnReference | Имя (LcOaSceneBaseUserName):Model_DK_1_4-TXx.dgn, Default
        // Ссылка ReferenceProps -> Имя пути (Pathname):D:\Models\VSK_SKD-ND\WorkFiles\Model_DK_1_4-TXx.dgn
        // step 3 search level dgn
        // Элемент LcOaNode -> Внутренний тип (LcOaSceneBaseClassName):LcDgnLevel | Имя (LcOaSceneBaseUserName):8_24-2
        // step 4 add lineNumber in level item
        // step 5 interop level item and add property element
        // ID объекта LcDgnElementId -> Значение (LcOaNat64AttributeValue):46681

        public void Init_False()
        {
            // step 1
            collection = doc.CurrentSelection.SelectedItems;

            if (collection.Count == 0)
            {
                doc.CurrentSelection.SelectAll();
                collection = doc.CurrentSelection.SelectedItems;
            }

            // step 2
            ModelItemCollection collection_DgnReference = GetDgnReference();

            
            foreach(ModelItem dgnReference in collection_DgnReference)
            {
                ModelItemEnumerableCollection childrens = dgnReference.Children;
                var enumerator = childrens.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    ModelItem dgnLevel = enumerator.Current;

                    // step 3
                    if (dgnLevel.IsLayer)
                    {
                        string fileName = "";
                        string levelName = "";
                        
                        fileName = dgnReference.PropertyCategories.FindPropertyByDisplayName("Ссылка", "Имя пути").Value.ToDisplayString();
                        levelName = dgnLevel.DisplayName;

                        try
                        {
                            var item = model.ModelRef.First((x)=>{

                                string f = Path.GetFileName(fileName);
                                string m = Path.GetFileName(x.Name);

                                return f.Equals(m);

                            }).Level.First((x) => {

                                return x.Name == levelName;

                            });
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        
                    }
                }
            }
        }

        public void Init()
        {
            // step 1
            collection = doc.CurrentSelection.SelectedItems;

            if (collection.Count == 0)
            {
                doc.CurrentSelection.SelectAll();
                collection = doc.CurrentSelection.SelectedItems;
            }
            
            foreach (var modRef in model.ModelRef)
            {
                string filePath = modRef.Name;

                messageForm.SetHeader(filePath);

                ModelItemCollection GetDgnReferences = GetDgnReference(filePath);

                foreach (ModelItem GetDgnReference in GetDgnReferences)
                {
                    var enumerator = GetDgnReference.Children.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ModelItem dgnLevel = enumerator.Current;

                        // step 3
                        if (dgnLevel.IsLayer)
                        {
                            foreach(var level in modRef.Level)
                            {
                                if (level.Name.Equals(dgnLevel.DisplayName))
                                {
                                    // Line number 
                                    foreach (var lines in level.Lines)
                                    {
                                        // Add line number
                                        AddData(dgnLevel, lines.Properties, "MicroStation - Трубопроводная линия");
                                    }
                                    
                                    // Elements
                                    foreach(var element in level.Elements)
                                    {
                                        foreach (ModelItem dgnLevelChildren in dgnLevel.Descendants)
                                        {
                                            PropertyCategoryCollection propertyCategories = dgnLevelChildren.PropertyCategories;

                                            foreach (PropertyCategory category in propertyCategories)
                                            {
                                                if (category.DisplayName.Equals("ID объекта"))
                                                {
                                                    DataPropertyCollection dataProperties = category.Properties;

                                                    foreach (DataProperty property in dataProperties)
                                                    {
                                                        if (property.DisplayName == "Значение")
                                                        {
                                                            string value = property.Value.ToDisplayString();

                                                            if (value.Equals(element.ElementID))
                                                            {
                                                                AddData(dgnLevelChildren, element.Properties, "MicroStation - Данные на элементе");

                                                                messageForm.SetCounter(countElement++);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public ModelItemCollection GetDgnReference()
        {
            string catUserName = "Элемент";
            string catInternalName = "LcOaNode";

            string propUserName = "Внутренний тип";
            string propInternalName = "LcOaSceneBaseClassName";
            
            string value = "LcDgnReference";

            return SearchCategoryAndProperty(collection, catUserName, catInternalName, propUserName, propInternalName, value);
        }

        public ModelItemCollection GetDgnReference(string filePath)
        {
            // Ссылка ReferenceProps -> Имя пути (Pathname):D:\Models\VSK_SKD-ND\WorkFiles\Model_DK_1_4-TXx.dgn

            string catUserName = "Ссылка";
            string catInternalName = "ReferenceProps";

            string propUserName = "Имя пути";
            string propInternalName = "Pathname";

            string value = filePath;

            return SearchCategoryAndProperty(collection, catUserName, catInternalName, propUserName, propInternalName, value);
        }

        #region # Helper method
        // find item (com)
        public ModelItemCollection SearchCategoty(string internalName, string name)
        {
            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(internalName, name);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = ComBridge.ToInwOpSelection(collection);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;
            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }

        // find item (com)
        public ModelItemCollection SearchCategoryAndProperty(ModelItemCollection coll, string catUserName = "", string catInternalName = "", string propUserName = "", string propInternalName = "", string value = "")
        {
            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(catInternalName, catUserName);
            opFindCondition.SetPropertyNames(propInternalName, propUserName);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_EQUAL;
            opFindCondition.value = value;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }

        private void AddData(ModelItem model, List<DataModelBentleyOPM.Property> preparedData, string header)
        {
            ComApi.InwOpState10 opState = ComBridge.State;
            ComApi.InwOaPath oaPath = ComBridge.ToInwOaPath(model);
            ComApi.InwGUIPropertyNode2 propn = (ComApi.InwGUIPropertyNode2)opState.GetGUIPropertyNode(oaPath, true);
            ComApi.InwOaPropertyVec category = (ComApi.InwOaPropertyVec)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOaPropertyVec, null, null);

            try
            {
                foreach (DataModelBentleyOPM.Property data in preparedData)
                {

                    ComApi.InwOaProperty property = (ComApi.InwOaProperty)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOaProperty, null, null);

                    property.name = data.Name;
                    property.UserName = data.Translation;
                    property.value = data.Value;

                    category.Properties().Add(property);
                    property = null;

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            propn.SetUserDefined(0, header, "DGN_Data", category);
        }
        #endregion
        #endregion
    }
}
