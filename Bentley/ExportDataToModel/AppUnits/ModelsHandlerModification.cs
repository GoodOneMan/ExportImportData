using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BCOM = Bentley.Interop.MicroStationDGN;
using BIM = Bentley.MicroStation.InteropServices;
using BIME = Bentley.Internal.MicroStation.Elements;
using System.Runtime.InteropServices;

using Bentley.ECObjects.Instance;
using Bentley.Plant.Utilities;
using Bentley.Properties;
using Bentley.DGNECPlugin;
using Structures = DataModelBentleyOPM;

namespace ExportDataToModel.AppUnits
{
    class ModelsHandlerModification
    {
        BCOM.Application app = null;
        BCOM.DesignFile designFile = null;
        List<string> list_MessageList = null;
        Dictionary<string, string> mapping = null;
        Structures.Model structure_Model = null;
        MessageForm messageForm = null;
        int counterElement = 0;
        int counterLineNumber = 0;

        string AppName = "";

        public ModelsHandlerModification()
        {
            app = BIM.Utilities.ComApp;

            AppName = app.Name;

            designFile = app.ActiveDesignFile;
            list_MessageList = new List<string>();
            structure_Model = new Structures.Model();
            messageForm = new MessageForm();
            mapping = new MappingProperty().GetMapping();
            messageForm.Show();

            // Run
            Init();

            messageForm.Close();
        }

        public Structures.Model GetStructure()
        {
            return structure_Model;
        }

        public List<string> GetMessages()
        {
            return list_MessageList;
        }

        // Run Code
        private void Init()
        {
            #region Test
            BCOM.ACSManager aCSManager = app.ACSManager;
            BCOM.Settings settings = app.ActiveSettings;
            BCOM.Workspace workspace = app.ActiveWorkspace;

            #endregion

            List<BCOM.ModelReferences> list_ModelReferences = ModelReferencesHandler();
            List<Structures.Line> list_LineNumber = LineNamberHandler(list_ModelReferences);

            structure_Model.ModelRef = new List<Structures.ModelRef>();
            structure_Model.Name = designFile.FullName;

            // Message form
            messageForm.SetHeader("Поиск графических элементов");

            // Go through all the elements
            foreach (BCOM.ModelReferences models in list_ModelReferences)
            {
                var enumerator_Models = models.GetEnumerator();

                while (enumerator_Models.MoveNext())
                {
                    BCOM.ModelReference model = enumerator_Models.Current as BCOM.ModelReference;

                    if (model.CanBePlacedAsCell && model.Type == BCOM.MsdModelType.Normal)
                    {
                        // Init Structures.ModelReference
                        Structures.ModelRef structure_ModelReference = new Structures.ModelRef();
                        structure_ModelReference.Level = new List<Structures.Level>();

                        // ??? Имя файла как в Navisworks (..., Default; Acc-x, ..., Default)
                        structure_ModelReference.Name = model.DesignFile.FullName;

                        BCOM.Levels levels = model.Levels;
                        var enumerator_Levels = levels.GetEnumerator();

                        while (enumerator_Levels.MoveNext())
                        {
                            BCOM.Level level = enumerator_Levels.Current as BCOM.Level;

                            if (level.IsInUseWithinModel(model))
                            {
                                // level contains graphical elements
                                bool graphicalElementContains = false;

                                List<string> list_RepeatingLine = new List<string>();

                                // Init Structures.Level
                                Structures.Level sructure_Level = new Structures.Level();
                                sructure_Level.Elements = new List<Structures.Element>();
                                sructure_Level.Lines = new List<Structures.Line>();
                                sructure_Level.Name = level.Name;

                                // Search element in model

                                #region OPM
                                if (AppName == "OpenPlantModeler")
                                {
                                    BCOM.ElementScanCriteria scanCriteria = new BCOM.ElementScanCriteriaClass();
                                    scanCriteria.ExcludeAllLevels();
                                    scanCriteria.IncludeLevel(level);

                                    BCOM.ElementEnumerator enumerator_Elements = app.ActiveModelReference.GraphicalElementCache.Scan(scanCriteria);

                                    while (enumerator_Elements.MoveNext())
                                    {
                                        BCOM.Element element = enumerator_Elements.Current;

                                        if (!element.IsHidden && element.IsGraphical && !element.IsTextElement() && !element.IsLineElement())
                                        {
                                            // Message form
                                            messageForm.SetCounter(counterElement++);

                                            graphicalElementContains = true;
                                            SetElementToLevelEC(list_LineNumber, sructure_Level.Elements, sructure_Level.Lines, element, list_RepeatingLine);
                                        }
                                    }

                                    // Add Structures.Level to Structures.ModelReference
                                    if (graphicalElementContains)
                                        structure_ModelReference.Level.Add(sructure_Level);
                                }
                                #endregion

                                #region BRCM
                                if (AppName == "BRCM")
                                {
                                    BCOM.ElementEnumerator enumerator_Elements = model.Scan();

                                    while (enumerator_Elements.MoveNext())
                                    {
                                        BCOM.Element element = enumerator_Elements.Current;
                                        try
                                        {
                                            if (element.IsGraphical)
                                            {
                                                // Message form
                                                messageForm.SetCounter(counterElement++);

                                                graphicalElementContains = true;
                                                SetElementToLevelEC(list_LineNumber, sructure_Level.Elements, sructure_Level.Lines, element, list_RepeatingLine);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            string err = ex.Message;
                                            list_MessageList.Add("Error in BRCM: (" + ex.Message + ")");
                                        }
                                    }

                                    // Add Structures.Level to Structures.ModelReference
                                    if (graphicalElementContains)
                                        structure_ModelReference.Level.Add(sructure_Level);

                                }
                                #endregion

                            }
                        }

                        // Add Structures.ModelReference to Structures.Model
                        structure_Model.ModelRef.Add(structure_ModelReference);
                    }
                }
            }
        }

        #region # Helper method

        #region # Get List<Structures.Line>
        private List<Structures.Line> LineNamberHandler(List<BCOM.ModelReferences> list_ModelRef)
        {
            // Message form
            messageForm.SetHeader("Поиск трубопроводных линий");

            List<Structures.Line> list = new List<Structures.Line>();

            // Search LineNumber and, add to list
            foreach (BCOM.ModelReferences models in list_ModelRef)
            {
                var enumerator_Models = models.GetEnumerator();

                while (enumerator_Models.MoveNext())
                {
                    BCOM.ModelReference model = enumerator_Models.Current as BCOM.ModelReference;
                    BCOM.ElementEnumerator enumerator_Elements = model.Scan();

                    while (enumerator_Elements.MoveNext())
                    {
                        BCOM.Element element = enumerator_Elements.Current;
                        Bentley.Internal.MicroStation.Elements.Element element_A = Bentley.Internal.MicroStation.Elements.Element.FromElementID((ulong)element.ID64, (IntPtr)element.ModelReference.MdlModelRefP());
                        ECInstanceList list_elementProperties = PropertyManager.Instance.GetElementProperties(element_A, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);

                        if (list_elementProperties.Count > 0)
                        {
                            IECInstance iECInstance = list_elementProperties[0];
                            string name = iECInstance.ClassDefinition.Name;

                            if (name == "PIPING_NETWORK_SYSTEM")
                            {
                                // Message form
                                messageForm.SetCounter(counterLineNumber++);

                                list.Add(GetStructureLineNamber(element));
                            }
                        }
                    }
                }
            }

            return list;
        }
        #endregion

        #region # Get List<BCOM.ModelReferences>
        private List<BCOM.ModelReferences> ModelReferencesHandler()
        {
            // Message form
            messageForm.SetHeader("Обработка подключенных моделей");
            messageForm.SetCounter(0);

            List<BCOM.ModelReferences> list = new List<BCOM.ModelReferences>();

            BCOM.ModelReferences firstModelReferences = designFile.Models;
            list.Add(firstModelReferences);

            var enumerator = firstModelReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BCOM.ModelReference model = enumerator.Current as BCOM.ModelReference;

                if (model.Attachments.Count != 0)
                {
                    RecursionModelReference(model.Attachments, list);
                }
            }

            return list;
        }

        private void RecursionModelReference(BCOM.Attachments attachments, List<BCOM.ModelReferences> list)
        {
            foreach (BCOM.Attachment attachment in attachments)
            {
                try
                {
                    list.Add(attachment.DesignFile.Models);
                }
                catch (Exception ex)
                {
                    list_MessageList.Add("Error in RecursionModelReference: " + attachment.AttachName + " (" + ex.Message + ")");

                }

                if (attachment.Attachments.Count != 0)
                    RecursionModelReference(attachment.Attachments, list);
            }
        }
        #endregion

        #region # Full property
        //private Structures.Line GetStructureLineNamber(BCOM.Element element)
        //{
        //    Structures.Line structure_Line = new Structures.Line();

        //    structure_Line.Properties = new List<Structures.Property>();

        //    string lineName = "";
        //    string specification = "";
        //    string unitName = "";
        //    string elementID = "";

        //    BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

        //    string[] names = propertyHandler.GetAccessStrings();

        //    foreach (string name in names)
        //    {
        //        if (propertyHandler.SelectByAccessString(name))
        //        {
        //            try
        //            {
        //                if (name == "NAME")
        //                {
        //                    lineName = propertyHandler.GetDisplayString();
        //                }
        //                if (name == "SPECIFICATION")
        //                {
        //                    specification = "-" + propertyHandler.GetDisplayString();
        //                }
        //                if (name == "UNIT_NAME")
        //                {
        //                    unitName = propertyHandler.GetDisplayString() + "-";
        //                }
        //                if (name == "ElementID")
        //                {
        //                    elementID = propertyHandler.GetDisplayString();
        //                }

        //                Structures.Property property = new Structures.Property();

        //                property.Name = name;
        //                property.Translation = GetTranslation(name);
        //                property.Value = propertyHandler.GetDisplayString();

        //                structure_Line.Properties.Add(property);
        //            }
        //            catch (Exception ex)
        //            {
        //                list_MessageList.Add("Error in GetStructureLineNamber: " + name + " (" + ex.Message + ")");
        //            }
        //        }
        //    }

        //    structure_Line.ElementID = elementID;

        //    lineName = lineName.Replace(specification, "");
        //    lineName = lineName.Replace(unitName, "");
        //    structure_Line.Name = lineName;

        //    return structure_Line;
        //}

        //private void SetElementToLevel(List<Structures.Line> InList, List<Structures.Element> elements, List<Structures.Line> OutList, BCOM.Element element, List<string> RepeatingLine)
        //{
        //    // Init
        //    Structures.Element structure_Element = new Structures.Element();
        //    structure_Element.ElementID = element.ID64.ToString();
        //    structure_Element.Properties = new List<Structures.Property>();

        //    string lineName = "";

        //    BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

        //    string[] names = propertyHandler.GetAccessStrings();

        //    foreach (string name in names)
        //    {
        //        if (propertyHandler.SelectByAccessString(name))
        //        {
        //            try
        //            {
        //                if (name == "LINENUMBER")
        //                {
        //                    lineName = propertyHandler.GetDisplayString();
        //                }

        //                Structures.Property property = new Structures.Property();
        //                property.Name = name;
        //                property.Translation = GetTranslation(name);
        //                property.Value = propertyHandler.GetDisplayString();

        //                structure_Element.Properties.Add(property);

        //            }
        //            catch (Exception ex)
        //            {
        //                list_MessageList.Add("Error in SetElementToLevel: " + name + " (" + ex.Message + ")");
        //            }
        //        }
        //    }

        //    // Add
        //    elements.Add(structure_Element);

        //    //if(!lineName.Equals("") && !lineName.Contains("Нет-"))
        //    //{
        //    //    foreach (Structures.Line line in InList)
        //    //    {
        //    //        if (line.Name == lineName)
        //    //        {
        //    //            if (!RepeatingLine.Contains(line.Name))
        //    //            {
        //    //                RepeatingLine.Add(line.Name);
        //    //                // Add
        //    //                OutList.Add(line);
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    foreach (Structures.Line line in InList)
        //    {
        //        if (line.Name == lineName)
        //        {
        //            if (!RepeatingLine.Contains(line.Name))
        //            {
        //                RepeatingLine.Add(line.Name);
        //                // Add
        //                OutList.Add(line);
        //            }
        //        }
        //    }
        //}

        //private string GetTranslation(string name)
        //{
        //    var enumerator_Translation = mapping.Where(k => k.Key == name).GetEnumerator();

        //    while (enumerator_Translation.MoveNext())
        //    {
        //        name = enumerator_Translation.Current.Value;
        //    }

        //    return name;
        //}
        #endregion

        #region # Concret property
        private Structures.Line GetStructureLineNamber(BCOM.Element element)
        {
            Structures.Line structure_Line = new Structures.Line();

            structure_Line.Properties = new List<Structures.Property>();

            string lineName = "";
            string specification = "";
            string unitName = "";
            string elementID = "";

            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);
            string[] names = propertyHandler.GetAccessStrings();

            var fields = MappingProperty.GetPropertyNameLineNumber();

            foreach (KeyValuePair<string, string> prop in fields)
            {
                if (propertyHandler.SelectByAccessString(prop.Key))
                {
                    try
                    {
                        if (prop.Key == "NAME")
                        {
                            lineName = propertyHandler.GetDisplayString();
                        }
                        if (prop.Key == "SPECIFICATION")
                        {
                            specification = "-" + propertyHandler.GetDisplayString();
                        }
                        if (prop.Key == "UNIT_NAME")
                        {
                            unitName = propertyHandler.GetDisplayString() + "-";
                        }
                        if (prop.Key == "ElementID")
                        {
                            elementID = propertyHandler.GetDisplayString();
                        }

                        Structures.Property property = new Structures.Property();

                        property.Name = prop.Key;
                        property.Translation = prop.Value;
                        property.Value = propertyHandler.GetDisplayString();

                        structure_Line.Properties.Add(property);
                    }
                    catch (Exception ex)
                    {
                        list_MessageList.Add("Error in GetStructureLineNamber: " + prop.Key + " (" + ex.Message + ")");
                    }
                }
            }

            structure_Line.ElementID = elementID;

            lineName = lineName.Replace(specification, "");
            lineName = lineName.Replace(unitName, "");
            structure_Line.Name = lineName;

            return structure_Line;
        }

        private void SetElementToLevelCom(List<Structures.Line> InList, List<Structures.Element> elements, List<Structures.Line> OutList, BCOM.Element element, List<string> RepeatingLine)
        {
            // Init
            Structures.Element structure_Element = new Structures.Element();
            structure_Element.ElementID = element.ID64.ToString();
            structure_Element.Properties = new List<Structures.Property>();

            string lineName = "";

            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

            // See full property name
            string[] fullFields = propertyHandler.GetAccessStrings();

            var fields = MappingProperty.GetPropertyNameElement();

            foreach (KeyValuePair<string, string> prop in fields)
            {
                if (propertyHandler.SelectByAccessString(prop.Key))
                {
                    try
                    {
                        if (prop.Key == "LINENUMBER")
                        {
                            lineName = propertyHandler.GetDisplayString();
                        }

                        Structures.Property property = new Structures.Property();
                        property.Name = prop.Key;
                        property.Translation = prop.Value;
                        property.Value = propertyHandler.GetDisplayString();

                        structure_Element.Properties.Add(property);

                    }
                    catch (Exception ex)
                    {
                        list_MessageList.Add("Error in SetElementToLevel: " + prop.Key + " (" + ex.Message + ")");
                    }
                }
            }

            // Add
            elements.Add(structure_Element);

            //if(!lineName.Equals("") && !lineName.Contains("Нет-"))
            //{
            //    foreach (Structures.Line line in InList)
            //    {
            //        if (line.Name == lineName)
            //        {
            //            if (!RepeatingLine.Contains(line.Name))
            //            {
            //                RepeatingLine.Add(line.Name);
            //                // Add
            //                OutList.Add(line);
            //            }
            //        }
            //    }
            //}

            foreach (Structures.Line line in InList)
            {
                if (line.Name == lineName)
                {
                    if (!RepeatingLine.Contains(line.Name))
                    {
                        RepeatingLine.Add(line.Name);
                        // Add
                        OutList.Add(line);
                    }
                }
            }
        }

        private void SetElementToLevelEC(List<Structures.Line> InList, List<Structures.Element> elements, List<Structures.Line> OutList, BCOM.Element comElement, List<string> RepeatingLine)
        {
            // Init
            Structures.Element structure_Element = new Structures.Element();
            structure_Element.ElementID = comElement.ID64.ToString();
            structure_Element.Properties = new List<Structures.Property>();

            string lineName = "";
            bool ecClass = true;

            BIME.Element element = BIME.Element.FromElementID((ulong)comElement.ID64, (IntPtr)comElement.ModelReference.MdlModelRefP());
            ECInstanceList ecInstances = Bentley.Properties.PropertyManager.Instance.GetElementProperties(element, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);

            IECInstance instance = ecInstances[0];

            // See full property name
            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(comElement);
            string[] fullFields = propertyHandler.GetAccessStrings();

            var fields = MappingProperty.GetPropertyNameElement();

            if (fullFields.FirstOrDefault(p => p == "EC_CLASS_NAME") != null ||
                fullFields.FirstOrDefault(p => p == "Pipeline") != null ||
                fullFields.FirstOrDefault(p => p == "LENGTH") != null ||
                fullFields.FirstOrDefault(p => p == "CatalogID") != null ||
                fullFields.FirstOrDefault(p => p == "UNIT_OF_MEASURE") != null ||
                fullFields.FirstOrDefault(p => p == "SERVICE") != null ||
                fullFields.FirstOrDefault(p => p == "LINENUMBER") != null ||
                fullFields.FirstOrDefault(p => p == "ALT_P_MARK") != null ||
                fullFields.FirstOrDefault(p => p == "COMPONENT_NAME") != null ||
                fullFields.FirstOrDefault(p => p == "INSULATION") != null ||
                fullFields.FirstOrDefault(p => p == "SYSTEM") != null ||
                fullFields.FirstOrDefault(p => p == "MANUFACTURER") != null)
            {
                foreach (KeyValuePair<string, string> prop in fields)
                {
                    try
                    {
                        IECPropertyValue propertyValue = instance.GetPropertyValue(prop.Key);

                        string valueProperty = propertyValue.StringValue;
                        string nameProperty = propertyValue.Property.Name;
                        string displayLabel = propertyValue.Property.DisplayLabel;


                        if (prop.Key == "LINENUMBER")
                        {
                            lineName = valueProperty;
                        }

                        Structures.Property property = new Structures.Property();
                        property.Name = nameProperty;
                        property.Translation = displayLabel;
                        property.Value = valueProperty;

                        structure_Element.Properties.Add(property);

                        if (nameProperty == "EC_CLASS_NAME")
                            ecClass = false;

                    }
                    catch (Exception ex)
                    {
                        list_MessageList.Add("Error in SetElementToLevel: " + prop.Key + " (" + ex.Message + ")");
                    }
                }

                if (ecClass)
                {
                    Structures.Property property = new Structures.Property();
                    property.Name = "EC_CLASS_NAME";
                    property.Translation = "Имя класса EC";
                    property.Value = instance.ClassDefinition.Name;

                    structure_Element.Properties.Add(property);
                }


                // Add
                elements.Add(structure_Element);

                foreach (Structures.Line line in InList)
                {
                    if (line.Name == lineName)
                    {
                        if (!RepeatingLine.Contains(line.Name))
                        {
                            RepeatingLine.Add(line.Name);
                            // Add
                            OutList.Add(line);
                        }
                    }
                }
            }
        }

        #endregion
        #endregion
    }
}
