using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using System.Linq;

namespace ImportDataOPM.AppUnits
{
    class ModelHandlerElementId
    {
        DataModelBentleyOPM.Model model = null;

        Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
        ModelItemCollection collection = null;

        MessageForm messageForm = null;

        int countElement = 0;

        public ModelHandlerElementId()
        {
            messageForm = new MessageForm();
            messageForm.Show();

            if ((model = new DataLoader(messageForm).GetModel()) != null)
            {
                Init();

                messageForm.Close();
                MessageBox.Show(" свойства перенесены ", "импорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                messageForm.Close();
                MessageBox.Show(" ошибка ", "данные нет !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        public void Init()
        {

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
                            foreach (var level in modRef.Level)
                            {
                                // Line number 
                                foreach (var lines in level.Lines)
                                {
                                    // Add line number
                                    AddData(dgnLevel, lines.Properties, "MicroStation - Трубопроводная линия");
                                }

                                // Elements
                                foreach (var element in level.Elements)
                                {
                                    foreach (ModelItem dgnLevelChildren in dgnLevel.Descendants)
                                    {
                                        PropertyCategoryCollection propertyCategories = dgnLevelChildren.PropertyCategories;

                                        #region Implement
                                        //var pc = propertyCategories.FindCategoryByName("LcDgnElementId");
                                        var categories = propertyCategories.Where(pc => pc.Name == "LcDgnElementId");

                                        // check re-adding
                                        if (propertyCategories.FindCategoryByDisplayName("MicroStation - Данные на элементе") != null)
                                            continue;
                                        #endregion

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
                                                            // Line number 
                                                            foreach (var lines in level.Lines)
                                                            {
                                                                // Add line number
                                                                AddData(dgnLevelChildren, lines.Properties, "MicroStation - Трубопроводная линия");
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

        public ModelItemCollection GetDgnReference(string filePath)
        {
            string catUserName = "Ссылка";
            string catInternalName = "ReferenceProps";
            string propUserName = "Имя пути";
            string propInternalName = "Pathname";

            string value = filePath;

            // Test \\Sstore\3D\12_otd\Sibur_Polilab_model\Polilab-TK.dgn
            //value = value.Replace(@"D:\data_file\", @"\\Sstore\3D\12_otd\Sibur_Polilab_model\");
            //value = @"\\Sstore\3d\12_otd\WorkSpace\Projects\Sibur_Polilab\Polilab-TK1-PIPE.dgn";

            value = @"\\Sstore\3D\11_otd\Sibur_Polilab\Polilab-ES'.dgn";

            return SearchCategoryAndProperty(collection, catUserName, catInternalName, propUserName, propInternalName, value);
        }


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

        //
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

        //
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
