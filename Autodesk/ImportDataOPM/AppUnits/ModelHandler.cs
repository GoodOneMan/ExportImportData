using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;

namespace ImportDataOPM.AppUnits
{
    class ModelHandler
    {
        DataModelBentleyOPM.Model model = null;
        Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
        ModelItemCollection collection = null;
        MessageForm messageForm = null;
        int countElement = 0;


        #region Construct partition
        public ModelHandler(int flag)
        {
            messageForm = new MessageForm();
            messageForm.Show();

            Dictionary<ModelItem, string> partitions = SearchPartition();

            foreach (var partition in partitions)
            {
                model = new DataLoader(messageForm, partition.Value).GetModel();

                if (model == null)
                    continue;

                Init(partition.Key);
            }

            messageForm.Close();
            MessageBox.Show(" свойства перенесены ", "импорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private Dictionary<ModelItem, string> SearchPartition()
        {
            Dictionary<ModelItem, string> partitions = new Dictionary<ModelItem, string>();

            string catUserName = "Элемент";
            string catInternalName = "LcOaNode";
            string propUserName = "Внутренний тип";
            string propInternalName = "LcOaSceneBaseClassName";
            string value = "LcOaPartition";

            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectAll();
            ModelItemCollection collection = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems;
            ModelItemCollection collPart = SearchCategoryAndProperty(collection, catUserName, catInternalName, propUserName, propInternalName, value);

            foreach (ModelItem item in collection)
            {
                DataProperty dataProperty = item.PropertyCategories.FindPropertyByCombinedName(new NamedConstant("LcOaNode", "Элемент"), new NamedConstant("LcOaSceneBaseClassName", "Внутренний тип"));
                if (dataProperty != null && dataProperty.Value.ToDisplayString() == "LcOaPartition")
                {
                    collPart.Add(item);
                }
            }

            foreach (ModelItem modelItem in collPart)
            {
                DataProperty dataProperty = modelItem.PropertyCategories.FindPropertyByCombinedName(new NamedConstant("LcOaNode", "Элемент"), new NamedConstant("LcOaPartitionSourceFilename", "Имя файла источника"));
                if (dataProperty != null)
                {
                    if (Path.GetExtension(dataProperty.Value.ToDisplayString()).ToLower() == ".dgn")
                    {
                        string fileData = dataProperty.Value.ToDisplayString();
                        fileData = fileData.Remove(fileData.Length - 4, 4) + ".dat";

                        partitions.Add(modelItem, fileData);
                    }
                }
            }

            return partitions;
        }
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

        public void Init(ModelItem partition)
        {
            collection = new ModelItemCollection();
            collection.Add(partition);

            // step 1
            DataModelBentleyOPM.ModelRef modelRef = model.ModelRef.Find(item => item.Name.Contains(partition.DisplayName));

            if(modelRef != null && modelRef.Level.Count > 0)
            {
                ModelItemCollection LevelReferences = GetLevelReference();
            }

            // step 2
            ModelItemCollection DgnReferences = GetDgnReference();

            foreach(ModelItem DgnRef in DgnReferences)
            {
                DataProperty referenceNwd = DgnRef.PropertyCategories.FindPropertyByCombinedName(
                    new NamedConstant("ReferenceProps", "Ссылка"), new NamedConstant("Pathname", "Имя пути"));

                if(referenceNwd != null)
                {
                    string pathNwd = referenceNwd.Value.ToDisplayString().ToLower();

                    foreach(DataModelBentleyOPM.ModelRef referenceDgn in model.ModelRef)
                    {
                        string pathDgn = referenceDgn.Name.ToLower();

                        if(pathNwd == pathDgn && referenceDgn.Level.Count > 0)
                        {
                            collection.Clear();
                            collection.Add(DgnRef);
                            ModelItemCollection LevelReferences = GetLevelReference();

                            foreach(ModelItem levelNwd in LevelReferences)
                            {
                                foreach (var levelDgn in referenceDgn.Level)
                                {
                                    if (levelDgn.Name.Equals(levelNwd.DisplayName))
                                    {
                                        // Line number 
                                        foreach (var lines in levelDgn.Lines)
                                        {
                                            // Add line number
                                            AddData(levelNwd, lines.Properties, "MicroStation - Трубопроводная линия");
                                        }

                                        // Elements
                                        foreach (var element in levelDgn.Elements)
                                        {
                                            //if (element.ElementID == "179523")
                                            //    continue;

                                            foreach (ModelItem dgnLevelChildren in levelNwd.Descendants)
                                            {
                                                PropertyCategoryCollection propertyCategories = dgnLevelChildren.PropertyCategories;

                                                foreach (PropertyCategory category in propertyCategories)
                                                {
                                                    if (category.Name.Equals("LcDgnElementId"))
                                                    {
                                                        DataPropertyCollection dataProperties = category.Properties;

                                                        foreach (DataProperty property in dataProperties)
                                                        {
                                                            if (property.Name.Equals("LcOaNat64AttributeValue"))
                                                            {
                                                                string value = property.Value.ToDisplayString();

                                                                if (value.Equals(element.ElementID))
                                                                {

                                                                    DataModelBentleyOPM.Property LINENUMBER = element.Properties.Find(p => p.Name == "LINENUMBER");

                                                                    if (LINENUMBER != null)
                                                                    {
                                                                        if(model.LineNumbers != null)
                                                                        {
                                                                            DataModelBentleyOPM.Line line = model.LineNumbers.Find(l => l.Name == LINENUMBER.Value);

                                                                            if (line != null)
                                                                            {
                                                                                AddData(dgnLevelChildren, line.Properties, "MicroStation - Трубопроводная линия");
                                                                            }
                                                                        }
                                                                    }

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
        }
        #endregion
        
        #region GetDgnReference GetLevelReference
        public ModelItemCollection GetDgnReference()
        {
            string catUserName = "Ссылка";
            string catInternalName = "ReferenceProps";
            return SearchCategoty(collection, catUserName, catInternalName);
        }
        public ModelItemCollection SearchCategoty(ModelItemCollection coll, string internalName = "", string name = "")
        {
            ComApi.InwOpState10 opState = ComBridge.State;

            ComApi.InwOpSelectionSet2 opSelSet2 = (ComApi.InwOpSelectionSet2)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpSelectionSet);
            opSelSet2.name = "ByCategorySelection";

            ComApi.InwOpFindSpec opFindSpec = (ComApi.InwOpFindSpec)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindSpec);
            ComApi.InwOpFindCondition opFindCondition = (ComApi.InwOpFindCondition)(object)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOpFindCondition);

            opFindCondition.SetAttributeNames(name, internalName);
            opFindCondition.Condition = ComApi.nwEFindCondition.eFind_HAS_ATTRIB;

            opFindSpec.selection = ComBridge.ToInwOpSelection(coll);
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_BELOW_SELECTED_PATHS;
            opFindSpec.ResultDisjoint = false;
            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }
        public ModelItemCollection GetLevelReference()
        {
            string catUserName = "Элемент";
            string catInternalName = "LcOaNode";
            string propUserName = "Внутренний тип";
            string propInternalName = "LcOaSceneBaseClassName";
            string value = "LcDgnLevel";
            return SearchCategoryAndPropertyLevel(collection, catUserName, catInternalName, propUserName, propInternalName, value);
        }
        public ModelItemCollection SearchCategoryAndPropertyLevel(ModelItemCollection coll, string catUserName = "", string catInternalName = "", string propUserName = "", string propInternalName = "", string value = "")
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
            //opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_SELECTED_PATHS;
            opFindSpec.SearchMode = ComApi.nwESearchMode.eSearchMode_ALL_PATHS;
            opFindSpec.ResultDisjoint = false;

            opFindSpec.Conditions().Add(opFindCondition);

            opSelSet2.ImplicitFindSpec = opFindSpec;

            return ComBridge.ToModelItemCollection(opSelSet2.selection);
        }
        #endregion
        
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
                    if (!data.Value.Equals(""))
                    {
                        ComApi.InwOaProperty property = (ComApi.InwOaProperty)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOaProperty, null, null);

                        property.name = data.Name;
                        property.UserName = data.Translation;
                        property.value = data.Value;

                        category.Properties().Add(property);
                        property = null;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            propn.SetUserDefined(0, header, "DGN_Data", category);
        }
    }
}
