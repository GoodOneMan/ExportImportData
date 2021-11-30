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
    class ModelsHandlerModificationOne
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

        public ModelsHandlerModificationOne()
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
            List<BCOM.ModelReferences> list_ModelReferences = ModelReferencesHandler();
            List<Structures.Line> list_LineNumber = LineNamberHandler();

            structure_Model.Name = designFile.FullName.ToLower();
            structure_Model.ModelRef = new List<Structures.ModelRef>();
            structure_Model.LineNumbers = list_LineNumber;

            // Message form
            messageForm.SetHeader("Поиск графических элементов");

            foreach (BCOM.ModelReferences models in list_ModelReferences)
            {
                var enumerator_Models = models.GetEnumerator();
                while (enumerator_Models.MoveNext())
                {
                    BCOM.ModelReference model = enumerator_Models.Current as BCOM.ModelReference;
                    BCOM.ElementCache elementCache = model.GraphicalElementCache;
                    int countIndex = elementCache.Count;

                    for (int index = 1; index <= countIndex; index++)
                    {
                        BCOM.Element elementBCOM = elementCache.GetElement(index);
                        
                        if (!elementBCOM.IsComponentElement && elementCache.IsElementValid(index))
                        {
                            BIME.Element elementBIM = BIME.Element.FromElementID((ulong)elementBCOM.ID64, (IntPtr)elementBCOM.ModelReference.MdlModelRefP());
                            
                            string LINENUMBER = "LINENUMBER_NULL";
                            List<string> LEVELS = new List<string>();
                            string FILEPATH = "FILEPATH_NULL";

                            ECInstanceList ecInstanceList = PropertyManager.Instance.GetElementProperties(elementBIM, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);
                            if (ecInstanceList.Count > 0)
                            {
                                IECInstance iECInstance = ecInstanceList[0];
                                IECPropertyValue eCPropertyValue = iECInstance.FindPropertyValue("LINENUMBER", false, false, false);

                                if (eCPropertyValue != null)
                                {
                                    LINENUMBER = eCPropertyValue.StringValue;
                                }
                            }

                            GetFullLevel(elementBIM, ref LEVELS);

                            //FILEPATH = elementBCOM.ModelReference.DesignFile.FullName.ToLower();
                            FILEPATH = elementBCOM.ModelReference.DesignFile.FullName;

                            SetElementInStructure(elementBIM, elementBCOM, FILEPATH, LINENUMBER, LEVELS, list_LineNumber);
                        }
                    }
                }
            }
        }

        #region Help methods
        private void GetFullLevel(BIME.Element element, ref List<string> list)
        {
            BIME.ComplexElement complexElement = element as BIME.ComplexElement;
            if (complexElement != null && complexElement.ChildCount != 0)
            {
                foreach (var item in complexElement.Components)
                {
                    BIME.ComplexElement inComplexElement = item as BIME.ComplexElement;
                    if (inComplexElement != null)
                    {
                        GetFullLevel(inComplexElement, ref list);
                    }
                    else
                    {
                        BIME.Element inElement = item as BIME.Element;
                        try
                        {
                            if (CheckType(inElement) && !list.Contains(inElement.Level.Name))

                                list.Add(inElement.Level.Name);
                        }
                        catch(Exception ex)
                        {
                            list_MessageList.Add("GetFullLevel " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                try
                {
                    if (CheckType(element) && !list.Contains(element.Level.Name))

                        list.Add(element.Level.Name);
                }
                catch (Exception ex)
                {
                    list_MessageList.Add("GetFullLevel " + ex.Message);
                }
            }
        }
        private void GetPropertyElement(BCOM.Element element, List<Structures.Property> list)
        {
            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);
            Dictionary<string, string> fields = MappingProperty.GetPropertyNameElement();

            foreach (KeyValuePair<string, string> prop in fields)
            {
                if (propertyHandler.SelectByAccessString(prop.Key))
                {
                    try
                    {
                        Structures.Property property = new Structures.Property();
                        property.Name = prop.Key;
                        property.Translation = prop.Value;
                        property.Value = propertyHandler.GetDisplayString();

                        list.Add(property);
                    }
                    catch(Exception ex)
                    {
                        list_MessageList.Add("GetPropertyElement " + ex.Message);
                    }
                }
            }
        }
        
        // Get Linenumber list
        private List<Structures.Line> LineNamberHandler()
        {
            // Message form
            messageForm.SetHeader("Поиск трубопроводных линий");

            List<Structures.Line> list = new List<Structures.Line>();
            Bentley.ECObjects.Instance.ECInstanceList ecList = Bentley.Plant.Utilities.DgnUtilities.GetInstance().GetAllInstancesFromDgn("PIPING_NETWORK_SYSTEM");

            foreach (IECInstance item in ecList)
            {
                // Message form
                messageForm.SetCounter(counterLineNumber++);

                list.Add(GetStructureLineNamber(item));
            }

            return list;
        }
        private Structures.Line GetStructureLineNamber(IECInstance ecInstance)
        {
            Structures.Line structure_Line = new Structures.Line();
            structure_Line.Properties = new List<Structures.Property>();

            IECPropertyValue name = ecInstance.FindPropertyValue("NAME", false, false, false);
            if (name != null)
                structure_Line.Name = name.StringValue;

            IEnumerator<IECPropertyValue> enumeratorPV = ecInstance.GetEnumerator(true);

            while (enumeratorPV.MoveNext())
            {
                try
                {
                    IECPropertyValue eCPropertyValue = enumeratorPV.Current;
                    Structures.Property property = new Structures.Property();

                    property.Name = eCPropertyValue.AccessString;
                    //property.Translation = eCPropertyValue.NativeValue.ToString();
                    property.Translation = eCPropertyValue.AccessString;
                    property.Value = eCPropertyValue.StringValue;

                    structure_Line.Properties.Add(property);
                }
                catch (Exception ex)
                {
                    list_MessageList.Add("GetStructureLineNamber " + ex.Message);
                }
            }

            return structure_Line;
        }
        
        // Get ModelRef list
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
        private void SetElementInStructure(BIME.Element elementBIM, BCOM.Element elementBCOM, string FILEPATH, string LINENUMBER, List<string> LEVELS, List<Structures.Line> list_LineNumber)
        {
            if (CheckElement(elementBCOM))
                return;

            Structures.ModelRef modelRef = structure_Model.ModelRef.FirstOrDefault(modRef => modRef.Name == FILEPATH);
            if (modelRef != null)
            {
                foreach (string item in LEVELS)
                {
                    Structures.Level level = modelRef.Level.FirstOrDefault(l => l.Name == item);
                    if (level != null)
                    {
                        Structures.Element elementS = new Structures.Element();
                        elementS.ElementID = elementBIM.ElementID.ToString();
                        elementS.Name = elementBIM.Type.ToString();

                        elementS.Properties = new List<Structures.Property>();
                        GetPropertyElement(elementBCOM, elementS.Properties);

                        level.Elements.Add(elementS);

                        Structures.Line line = level.Lines.FirstOrDefault(l => l.Name == LINENUMBER);
                        if (line == null)
                        {
                            list_LineNumber.FirstOrDefault(l => l.Name == LINENUMBER);
                            if (line != null)
                                level.Lines.Add(line);
                        }
                    }
                    else
                    {
                        level = new Structures.Level();
                        level.Name = item;

                        level.Lines = new List<Structures.Line>();

                        Structures.Line line = list_LineNumber.FirstOrDefault(l => l.Name == LINENUMBER);

                        if (line != null)
                            level.Lines.Add(line);

                        level.Elements = new List<Structures.Element>();

                        Structures.Element elementS = new Structures.Element();
                        elementS.ElementID = elementBIM.ElementID.ToString();
                        elementS.Name = elementBIM.Type.ToString();

                        elementS.Properties = new List<Structures.Property>();
                        GetPropertyElement(elementBCOM, elementS.Properties);

                        level.Elements.Add(elementS);
                        modelRef.Level.Add(level);
                    }
                }
            }
            else
            {
                modelRef = new Structures.ModelRef();
                modelRef.Name = FILEPATH;
                modelRef.Level = new List<Structures.Level>();

                foreach (string item in LEVELS)
                {
                    Structures.Level level = new Structures.Level();
                    level.Name = item;
                    
                    level.Lines = new List<Structures.Line>();

                    Structures.Line line = list_LineNumber.FirstOrDefault(l => l.Name == LINENUMBER);

                    if (line != null)
                        level.Lines.Add(line);

                    level.Elements = new List<Structures.Element>();

                    Structures.Element elementS = new Structures.Element();
                    elementS.ElementID = elementBIM.ElementID.ToString();
                    elementS.Name = elementBIM.Type.ToString();

                    elementS.Properties = new List<Structures.Property>();
                    GetPropertyElement(elementBCOM, elementS.Properties);

                    level.Elements.Add(elementS);
                    modelRef.Level.Add(level);
                }
                structure_Model.ModelRef.Add(modelRef);
            }
            
            messageForm.SetCounter(counterElement++);
        }
        
        // Checked method
        private bool CheckElement(BCOM.Element elementBCOM)
        {
            bool flag = true;
            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(elementBCOM);
            string[] fullFields = propertyHandler.GetAccessStrings();

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
                flag = false;
            }
            return flag;
        }
        private bool CheckType(BIME.Element elementBIM)
        {
            bool flag = true;
            //if (elementBIM.Type != BIME.Element.ElementType.BsplineKnot
            //    && elementBIM.Type != BIME.Element.ElementType.DgnStoreComp
            //    && elementBIM.Type != BIME.Element.ElementType.DgnStoreHeader)

            if (elementBIM.Type == BIME.Element.ElementType.BsplineKnot
                || elementBIM.Type == BIME.Element.ElementType.DgnStoreComp
                || elementBIM.Type == BIME.Element.ElementType.DgnStoreHeader)

                flag = false;

            return flag;
        }
        #endregion

    }
}
