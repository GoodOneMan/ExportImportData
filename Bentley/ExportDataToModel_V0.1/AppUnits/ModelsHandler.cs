using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BCOM = Bentley.Interop.MicroStationDGN;
using BIM = Bentley.MicroStation.InteropServices;
using System.Runtime.InteropServices;

using Bentley.ECObjects.Instance;
using Bentley.Plant.Utilities;
using Bentley.Properties;
using Bentley.DGNECPlugin;
using Structures = DataModelBentleyOPM;

namespace ExportDataToModel.AppUnits
{
    class ModelsHandler
    {

        BCOM.Application app = null;
        BCOM.DesignFile designFile = null;
        BCOM.ModelReference modelReference = null;
        List<string> list_MessageList = null;
        Structures.Model structure_Model = null;


        public ModelsHandler()
        {
            app = BIM.Utilities.ComApp;
            designFile = app.ActiveDesignFile;
            list_MessageList = new List<string>();
            structure_Model = new Structures.Model();

            // Run
            Init();

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
            List<BCOM.ModelReferences> list_ModelReferences = ModelReferencesHandler();
            List<Structures.Line> list_LineNumber = LineNamberHandler(list_ModelReferences);
            
            structure_Model.ModelRef = new List<Structures.ModelRef>();
            structure_Model.Name = designFile.FullName;

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
                                BCOM.ElementScanCriteria scanCriteria = new BCOM.ElementScanCriteriaClass();
                                scanCriteria.ExcludeAllLevels();
                                scanCriteria.IncludeLevel(level);

                                BCOM.ElementEnumerator enumerator_Elements = model.Scan(scanCriteria);

                                while (enumerator_Elements.MoveNext())
                                {
                                    BCOM.Element element = enumerator_Elements.Current;

                                    if (!element.IsHidden && element.IsGraphical && !element.IsTextElement() && !element.IsLineElement())
                                    {
                                        graphicalElementContains = true;
                                        SetElementToLevel(list_LineNumber, sructure_Level.Elements, sructure_Level.Lines, element, list_RepeatingLine);
                                    }
                                }

                                // Add Structures.Level to Structures.ModelReference
                                if (graphicalElementContains)
                                    structure_ModelReference.Level.Add(sructure_Level);
                            }
                        }

                        // Add Structures.ModelReference to Structures.Model
                        structure_Model.ModelRef.Add(structure_ModelReference);
                    }
                }
            }

        }

        #region # Get List<BCOM.ModelReferences>
        private List<BCOM.ModelReferences> ModelReferencesHandler()
        {
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

        #region # Get List<Structures.Line>
        private List<Structures.Line> LineNamberHandler(List<BCOM.ModelReferences> list_ModelRef)
        {
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
                                list.Add(GetStructureLineNamber(element));
                            }
                        }
                    }
                }
            }

            return list;
        }
        #endregion

        #region # Helper method
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

            foreach (string name in names)
            {
                if (propertyHandler.SelectByAccessString(name))
                {
                    try
                    {
                        if (name == "NAME")
                        {
                            lineName = propertyHandler.GetDisplayString();
                        }
                        if (name == "SPECIFICATION")
                        {
                            specification = "-" + propertyHandler.GetDisplayString();
                        }
                        if (name == "UNIT_NAME")
                        {
                            unitName = propertyHandler.GetDisplayString() + "-";
                        }
                        if (name == "ElementID")
                        {
                            elementID = propertyHandler.GetDisplayString();
                        }

                        Structures.Property property = new Structures.Property();

                        property.Name = name;
                        property.Value = propertyHandler.GetDisplayString();

                        structure_Line.Properties.Add(property);
                    }
                    catch(Exception ex)
                    {
                        list_MessageList.Add("Error in GetStructureLineNamber: " + name + " (" + ex.Message + ")");
                    }
                }
            }

            structure_Line.ElementID = elementID;

            lineName = lineName.Replace(specification, "");
            lineName = lineName.Replace(unitName, "");
            structure_Line.Name = lineName;

            return structure_Line;
        }

        private void SetElementToLevel(List<Structures.Line> InList, List<Structures.Element> elements, List<Structures.Line> OutList, BCOM.Element element, List<string> RepeatingLine)
        {
            // Init
            Structures.Element structure_Element = new Structures.Element();
            structure_Element.ElementID = element.ID64.ToString();
            structure_Element.Properties = new List<Structures.Property>();

            string lineName = "";

            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

            string[] names = propertyHandler.GetAccessStrings();

            foreach (string name in names)
            {
                if (propertyHandler.SelectByAccessString(name))
                {
                    try
                    {
                        if (name == "LINENUMBER")
                        {
                            lineName = propertyHandler.GetDisplayString();
                        }

                        Structures.Property property = new Structures.Property();
                        property.Name = name;
                        property.Value = propertyHandler.GetDisplayString();
                        
                        structure_Element.Properties.Add(property);

                    }
                    catch(Exception ex)
                    {
                        list_MessageList.Add("Error in SetElementToLevel: " + name + " (" + ex.Message + ")");
                    }
                }
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
        #endregion
    }
}
