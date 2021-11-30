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
    class TestR
    {
        BCOM.Application app = null;
        BCOM.DesignFile designFile = null;
        BCOM.ModelReference modelReference = null;
        StreamWriter sw = new StreamWriter(@"D:\DataTestEC.txt");
        List<string> messageList = new List<string>(); 


        public TestR()
        {
            app = BIM.Utilities.ComApp;
            designFile = app.ActiveDesignFile;
            modelReference = app.ActiveModelReference;

            Init();

            sw.Close();
        }

        public void Init()
        {
            #region # Test 7
            
            List<BCOM.ModelReferences> list_ModelReferences = ModelReferencesHandler();
            List<Structures.Line> list_LineNumber = new List<Structures.Line>();
            
            // Search LineNumber and, add to list
            foreach (BCOM.ModelReferences models in list_ModelReferences)
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
                                list_LineNumber.Add(GetStructureLineNamber(element));
                            }
                        }
                    }
                }
            }

            // Structures.Model
            Structures.Model structure_Model = new Structures.Model();
            structure_Model.ModelRef = new List<Structures.ModelRef>();
            structure_Model.Name = designFile.FullName;

            // Go through all the elements
            foreach (BCOM.ModelReferences models in list_ModelReferences)
            {
                var enumerator_Models = models.GetEnumerator();

                while (enumerator_Models.MoveNext())
                {
                    BCOM.ModelReference model = enumerator_Models.Current as BCOM.ModelReference;
                    

                    if(model.CanBePlacedAsCell && model.Type == BCOM.MsdModelType.Normal)
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
                                if(graphicalElementContains)
                                    structure_ModelReference.Level.Add(sructure_Level);
                            }
                        }

                        // Add Structures.ModelReference to Structures.Model
                        structure_Model.ModelRef.Add(structure_ModelReference);
                    }
                }
            }

            WriteData(list_LineNumber, structure_Model);

            #endregion

            #region # Test 6
            //List<BCOM.ModelReferences> listModelReferences = ModelReferencesHandler();
            //List<BCOM.Element> ElementLineNumbers = new List<BCOM.Element>();

            //foreach (BCOM.ModelReferences models in listModelReferences)
            //{
            //    var enumeratorModels = models.GetEnumerator();

            //    while (enumeratorModels.MoveNext())
            //    {
            //        BCOM.ModelReference model = enumeratorModels.Current as BCOM.ModelReference;
            //        BCOM.ElementEnumerator elementEnumerator = model.Scan();

            //        while (elementEnumerator.MoveNext())
            //        {
            //            BCOM.Element element = elementEnumerator.Current;
            //            Bentley.Internal.MicroStation.Elements.Element elementM = Bentley.Internal.MicroStation.Elements.Element.FromElementID((ulong)element.ID64, (IntPtr)element.ModelReference.MdlModelRefP());
            //            ECInstanceList elementProperties = PropertyManager.Instance.GetElementProperties(elementM, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);

            //            if (elementProperties.Count > 0)
            //            {
            //                IECInstance iECInstance = elementProperties[0];
            //                string name = iECInstance.ClassDefinition.Name;

            //                if (name == "PIPING_NETWORK_SYSTEM")
            //                {
            //                    ElementLineNumbers.Add(element);
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            #region # Test 5
            //List<BCOM.ModelReferences> listModelReferences = ModelReferencesHandler();

            //foreach (BCOM.ModelReferences models in listModelReferences)
            //{
            //    var enumeratorModels = models.GetEnumerator();

            //    while (enumeratorModels.MoveNext())
            //    {
            //        BCOM.ModelReference model = enumeratorModels.Current as BCOM.ModelReference;

            //        BCOM.ElementEnumerator elementEnumerator = model.Scan();

            //        while (elementEnumerator.MoveNext())
            //        {
            //            BCOM.Element element = elementEnumerator.Current;
            //            Bentley.Internal.MicroStation.Elements.Element elementM = Bentley.Internal.MicroStation.Elements.Element.FromElementID((ulong)element.ID64, (IntPtr)element.ModelReference.MdlModelRefP());
            //            ECInstanceList elementProperties = PropertyManager.Instance.GetElementProperties(elementM, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);

            //            if (elementProperties.Count > 0)
            //            {
            //                IECInstance iECInstance = elementProperties[0];
            //                string name = iECInstance.ClassDefinition.Name;

            //                if(name == "PIPING_NETWORK_SYSTEM")
            //                {
            //                    sw.WriteLine("+++ PIPING_NETWORK_SYSTEM");
            //                    GetProperties(element);

            //                    BCOM.ElementEnumerator elementCacheEnum = element.Cache.Scan();

            //                    while (elementCacheEnum.MoveNext())
            //                    {
            //                        BCOM.Element el = elementCacheEnum.Current;
            //                        sw.WriteLine("+++ ELEMENT_CACHE");
            //                        GetProperties(el);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            #region # Test 4
            //List<BCOM.ModelReferences> listModelReferences = ModelReferencesHandler();

            //foreach (BCOM.ModelReferences models in listModelReferences)
            //{
            //    var enumeratorModels = models.GetEnumerator();

            //    while (enumeratorModels.MoveNext())
            //    {
            //        BCOM.ModelReference model = enumeratorModels.Current as BCOM.ModelReference;

            //        BCOM.ElementEnumerator elementEnumerator = model.Scan();

            //        while (elementEnumerator.MoveNext())
            //        {
            //            BCOM.Element element = elementEnumerator.Current;

            //            Bentley.Internal.MicroStation.Elements.Element elementM = Bentley.Internal.MicroStation.Elements.Element.FromElementID((ulong)element.ID64, (IntPtr)element.ModelReference.MdlModelRefP());
            //            ECInstanceList elementProperties = PropertyManager.Instance.GetElementProperties(elementM, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);

            //            if (elementProperties.Count > 0)
            //            {
            //                IECInstance iECInstance = elementProperties[0];
            //                string name = iECInstance.ClassDefinition.Name;

            //                sw.WriteLine(" +++ " + name);
            //            }


            //            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

            //            string[] names = propertyHandler.GetAccessStrings();

            //            foreach (string name in names)
            //            {
            //                if (propertyHandler.SelectByAccessString(name))
            //                {
            //                    try
            //                    {
            //                        sw.WriteLine(name + "  " + propertyHandler.GetDisplayString());
            //                    }
            //                    catch
            //                    {
            //                        sw.WriteLine(name + " Error ");
            //                    }
            //                }
            //            }

            //            sw.WriteLine(new String('#', 100));
            //        }

            //        sw.WriteLine(new String('/', 100));
            //    }
            //}
            #endregion

            #region # Test 3
            //List<BCOM.ModelReferences> listModelReferences = ModelReferencesHandler();

            //foreach(BCOM.ModelReferences models in listModelReferences)
            //{
            //    var enumeratorModels = models.GetEnumerator();

            //    while (enumeratorModels.MoveNext())
            //    {
            //        BCOM.ModelReference model = enumeratorModels.Current as BCOM.ModelReference;

            //        BCOM.ElementEnumerator elementEnumerator = model.Scan();

            //        while (elementEnumerator.MoveNext())
            //        {
            //            BCOM.Element elementSelect = elementEnumerator.Current;

            //            Bentley.Internal.MicroStation.Elements.Element element = Bentley.Internal.MicroStation.Elements.Element.FromElementID((ulong)elementSelect.ID64, (IntPtr)elementSelect.ModelReference.MdlModelRefP());
            //            ECInstanceList elementProperties = PropertyManager.Instance.GetElementProperties(element, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);

            //            if (elementProperties.Count > 0)
            //            {
            //                IECInstance iECInstance = elementProperties[0];
            //                string name = iECInstance.ClassDefinition.Name;

            //                sw.WriteLine(name);
            //            }
            //        }
            //    }
            //}
            #endregion

            #region # Test 2
            //BCOM.ElementEnumerator elementEnumerator = designFile.DefaultModelReference.Scan();

            //List<string> list = new List<string>();

            //while (elementEnumerator.MoveNext())
            //{
            //    BCOM.Element elementSelect = elementEnumerator.Current;

            //    Bentley.Internal.MicroStation.Elements.Element element = Bentley.Internal.MicroStation.Elements.Element.FromElementID((ulong)elementSelect.ID64, (IntPtr)elementSelect.ModelReference.MdlModelRefP());
            //    ECInstanceList elementProperties = PropertyManager.Instance.GetElementProperties(element, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);

            //    if (elementProperties.Count > 0)
            //    {
            //        IECInstance iECInstance = elementProperties[0];
            //        string name = iECInstance.ClassDefinition.Name;

            //        if (!list.Contains(name))
            //        {
            //            list.Add(name);
            //        }

            //    }
            //}

            //foreach(string name in list)
            //{
            //    sw.WriteLine(name);
            //}

            #endregion

            #region # Test 1
            //BCOM.ElementScanCriteria scan_criteria = new BCOM.ElementScanCriteriaClass();
            //scan_criteria.ExcludeAllTypes();
            ////scan_criteria.IncludeType(BCOM.MsdElementType.ReferenceAttachment);
            //scan_criteria.IncludeType(BCOM.MsdElementType.ElementType44);


            //BCOM.ElementEnumerator elementEnumerator = designFile.DefaultModelReference.Scan(scan_criteria);
            //int count = elementEnumerator.BuildArrayFromContents().Length;

            //while (elementEnumerator.MoveNext())
            //{
            //    //BCOM.Element elementSelect = elementEnumerator.Current;
            //    //Bentley.Internal.MicroStation.Elements.Element element = Bentley.Internal.MicroStation.Elements.Element.FromElementID((ulong)elementSelect.ID64, (IntPtr)elementSelect.ModelReference.MdlModelRefP());

            //    BCOM.Element element = elementEnumerator.Current;

            //    BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

            //    string[] names = propertyHandler.GetAccessStrings();

            //    foreach (string name in names)
            //    {
            //        if (propertyHandler.SelectByAccessString(name))
            //        {
            //            try
            //            {
            //                sw.WriteLine(name + "  " + propertyHandler.GetDisplayString());
            //            }
            //            catch
            //            {
            //                sw.WriteLine(name + " Error ");
            //            }
            //        }
            //    }

            //    sw.WriteLine(new String('#', 100));
            //}


            //var list = ModelReferencesHandler();
            //ModelReferenceHandler(list);
            #endregion
        }

        #region # 1
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
            foreach(BCOM.Attachment attachment in attachments)
            {
                try
                {
                    list.Add(attachment.DesignFile.Models);
                }
                catch(Exception ex)
                {
                    messageList.Add(ex.Message);
                }
                
                if (attachment.Attachments.Count != 0)
                    RecursionModelReference(attachment.Attachments, list);
            }
        }
        #endregion

        #region # 2
        private void ModelReferenceHandler(List<BCOM.ModelReferences> list)
        {
            foreach(BCOM.ModelReferences modelReferences in list)
            {
                var enumeratorModel = modelReferences.GetEnumerator();

                while (enumeratorModel.MoveNext())
                {
                    BCOM.ModelReference model = enumeratorModel.Current as BCOM.ModelReference;

                    //sw.WriteLine("  " + model.Name + " " + model.DefaultLogicalName);

                    if (model.CanBePlacedAsCell && model.Type == BCOM.MsdModelType.Normal)
                    {
                        BCOM.Levels levels = model.Levels;

                        var enumeratorLevel = levels.GetEnumerator();

                        while (enumeratorLevel.MoveNext())
                        {
                            BCOM.Level level = enumeratorLevel.Current as BCOM.Level;

                            if (level.IsInUseWithinModel(model))
                            {
                                //sw.WriteLine("      " + level.Name + "  " + level.ID);

                                BCOM.ElementEnumerator elementEnumerator = model.Scan();
                                while (elementEnumerator.MoveNext())
                                {
                                    BCOM.Element element = elementEnumerator.Current;

                                    BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

                                    //if (propertyHandler.SelectByAccessString("EC_CLASSNAME"))
                                    //{

                                    //    if (propertyHandler.GetDisplayString() == "PIPING_NETWORK_SYSTEM")
                                    //    {
                                    //        string[] names = propertyHandler.GetAccessStrings();

                                    //        foreach (string name in names)
                                    //        {
                                    //            if (propertyHandler.SelectByAccessString(name))
                                    //            { 
                                    //                try
                                    //                {
                                    //                    sw.WriteLine(name + propertyHandler.GetAccessStrings());
                                    //                }
                                    //                catch
                                    //                {
                                    //                    sw.WriteLine(name + " Error ");
                                    //                }

                                    //            }
                                    //        }
                                    //    }
                                    //}

                                    if (propertyHandler.SelectByAccessString("EC_CLASS_NAME"))
                                    {
                                        if (propertyHandler.GetDisplayString() == "PIPING_NETWORK_SYSTEM")
                                        {
                                            string[] names = propertyHandler.GetAccessStrings();

                                            foreach (string name in names)
                                            {
                                                if (propertyHandler.SelectByAccessString(name))
                                                {
                                                    try
                                                    {
                                                        sw.WriteLine(name + "  " + propertyHandler.GetDisplayString());
                                                    }
                                                    catch
                                                    {
                                                        sw.WriteLine(name + " Error ");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //
                                //BCOM.ElementScanCriteria scan_criteria = new BCOM.ElementScanCriteriaClass();
                                //scan_criteria.ExcludeAllLevels();
                                //scan_criteria.IncludeLevel(level);

                                //BCOM.ElementEnumerator elements = model.Scan(scan_criteria);

                                //while (elements.MoveNext())
                                //{
                                //    BCOM.Element element = elements.Current;

                                //    if (element.IsGraphical)
                                //    {

                                        
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
        }

        public void ECWorker()
        {
            DgnUtilities instance2 = DgnUtilities.GetInstance();
            ECInstanceList allInstancesFromDgn = instance2.GetAllInstancesFromDgn("PIPING_NETWORK_SYSTEM");

            var allInstancesEnumerator = allInstancesFromDgn.GetEnumerator();

            while (allInstancesEnumerator.MoveNext())
            {
                IECInstance instance = allInstancesEnumerator.Current;
                var obj = instance.ReferenceObject as Bentley.Internal.MicroStation.Elements.Cell;
                string IDInstance = instance.InstanceId;

                sw.WriteLine(instance.ClassDefinition.Name);
                sw.WriteLine(instance.ClassDefinition.DisplayLabel);
                sw.WriteLine(instance.ClassDefinition.Description);
                sw.WriteLine(instance.InstanceId);

                var instanceEnumerator = instance.GetEnumerator();
                while (instanceEnumerator.MoveNext())
                {
                    IECPropertyValue propertyValue = instanceEnumerator.Current;

                    string name = propertyValue.AccessString;
                    string value = propertyValue.StringValue;

                    sw.WriteLine("     " + name + " - " + value + " " + propertyValue.XmlStringValue);
                }

                sw.WriteLine(new String('#', 100));
            }
        }

        public void ECWorker1()
        {
            Bentley.ECSystem.Session.ECSession eCSession = Bentley.ECSystem.Session.SessionManager.CreateSession();
            Bentley.Interop.MicroStationDGN.Application comApp = app;
            string text1 = Bentley.ECPlugin.Common.ECRepositoryConnectionHelper.BuildLocation(comApp.ActiveDesignFile.FullName, comApp.ActiveModelReference.Name);
            Bentley.ECSystem.Repository.RepositoryConnectionService service = Bentley.ECSystem.Repository.RepositoryConnectionServiceFactory.GetService();
            DgnECConnectionOptions dgnECConnectionOptions = new DgnECConnectionOptions();
            dgnECConnectionOptions.ReferenceModelScope = (ReferenceModelScopeOptions)(0);// ReferenceModelScopeOptions.
            dgnECConnectionOptions.AllowEditing = (true);
            var m_dgnECConnection = service.Open(eCSession, DgnECPersistence.PluginID, text1, dgnECConnectionOptions.ConnectionInfoObject, null);

            ECInstanceList RelDHSList = Bentley.Plant.Utilities.DgnUtilities.GetInstance().GetRelationshipInstances("PIPING_NETWORK_SYSTEM", "PIPING_NETWORK_SEGMENT", "PIPELINE_HAS_SEGMENT", m_dgnECConnection);
            List<string> PipeHasSupportList = new List<string>();
        }
        #endregion


        #region # Helper Functions
        public void GetProperties(BCOM.Element element)
        {
            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

            string[] names = propertyHandler.GetAccessStrings();

            foreach (string name in names)
            {
                if (propertyHandler.SelectByAccessString(name))
                {
                    try
                    {
                        sw.WriteLine(name + "  " + propertyHandler.GetDisplayString());
                    }
                    catch
                    {
                        sw.WriteLine(name + " Error ");
                    }
                }
            }
        }

        public void GetLineNumbersElements()
        {
            DgnUtilities instance = DgnUtilities.GetInstance();
            ECInstanceList allInstancesFromDgn = instance.GetAllInstancesFromDgn("PIPING_NETWORK_SYSTEM");

            IECInstance[] IECInstanceArray = allInstancesFromDgn.ToArray();

            for (int i = 0; i < IECInstanceArray.Length; i++)
            {
                Bentley.Internal.MicroStation.ModelReference model = IECInstanceArray[i].ReferenceObject as Bentley.Internal.MicroStation.ModelReference;
            }
        }

        public Structures.Line GetStructureLineNamber(BCOM.Element element)
        {
            Structures.Line lineNumber = new Structures.Line();
            lineNumber.Properties = new List<Structures.Property>();

            string lineName = "";
            string specification = "";
            string unitName  = "";
            string elementID = "";

            BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

            string[] names = propertyHandler.GetAccessStrings();

            foreach (string name in names)
            {
                if (propertyHandler.SelectByAccessString(name))
                {
                    try
                    {
                        if(name == "NAME")
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

                        lineNumber.Properties.Add(property);
                    }
                    catch
                    {
                        //sw.WriteLine(name + " Error ");
                    }
                }
            }

            lineNumber.ElementID = elementID;

            lineName = lineName.Replace(specification, "");
            lineName = lineName.Replace(unitName, "");

            lineNumber.Name = lineName;
            
            return lineNumber;
        }

        public void SetElementToLevel(List<Structures.Line> InList, List<Structures.Element> elements, List<Structures.Line> OutList, BCOM.Element element, List<string> RepeatingLine)
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
                    catch
                    {
                        //sw.WriteLine(name + " Error ");
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

        public void WriteData(List<Structures.Line> lineNumberList, Structures.Model model)
        {
            foreach(Structures.Line lineNumber in lineNumberList)
            {
                sw.WriteLine(lineNumber.Name + " ?? " + lineNumber.ElementID);

                foreach (var property in lineNumber.Properties)
                {
                    sw.WriteLine("   " + property.Name + "  ??  " + property.Value);
                }
            }

            sw.WriteLine(new String('#', 100));

            foreach (var modelReference in model.ModelRef)
            {
                sw.WriteLine(modelReference.Name);

                foreach(var level in modelReference.Level)
                {
                    sw.WriteLine("   " + level.Name);

                    foreach (var element in level.Elements)
                    {
                        sw.WriteLine("      " + element.ElementID);

                        foreach (var property in element.Properties)
                        {
                            sw.WriteLine("         " + property.Name + "  ??  " + property.Value);
                        }
                    }
                }
            }
        }
        
        #endregion
    }

    class TestRDLLImport
    {

        public TestRDLLImport()
        {
            DgnModelRef();
        }


        // mdlMesh_newPolyfaceFromXYTriangulation
        [DllImport("stdbspline.dll")]
        public static extern int mdlMesh_newPolyfaceFromXYTriangulation (out int ppMeshDescr, BCOM.Point3d[] xyzArray, int numXYZ);
        public static void Mesh(string unparsed)
        {
            BCOM.Application app = BIM.Utilities.ComApp;
            char[] delimiterChars = { ' ' };
            string myLine;
            List<BCOM.Point3d> meshPnts = new List<BCOM.Point3d>();
            StreamReader sr = new StreamReader(@"d:\data-13.asc");
            while (null != (myLine = sr.ReadLine()))
            {
                string[] sArray = myLine.Split(delimiterChars);
                meshPnts.Add(app.Point3dFromXYZ(double.Parse(sArray[0]), double.Parse(sArray[1]), double.Parse(sArray[2])));
            }
            sr.Close();
            int ppMeshDescr;
            BCOM.Point3d[] xyzArray = meshPnts.ToArray();
            if (0 == mdlMesh_newPolyfaceFromXYTriangulation(out ppMeshDescr, xyzArray, meshPnts.Count))
            {
                BCOM.Element meshEl = app.MdlCreateElementFromElementDescrP(ppMeshDescr);
                app.ActiveModelReference.AddElement(meshEl);
            }
        }

        // mdlModelRef_getDgnFile
        [DllImport("stdmdlbltin.dll")]
        public static extern int mdlModelRef_getDgnFile(int modelRef);

        public void DgnModelRef()
        {
            BCOM.Application _app = null;
            BCOM.DesignFile _design_file = null;
            BCOM.ModelReference _model_reference = null;

            _app = BIM.Utilities.ComApp;
            _design_file = _app.ActiveDesignFile;
            _model_reference = _app.ActiveModelReference;

            //try
            //{
            //    int modelRef = _model_reference.MdlModelRefP();
            //    int FileObjP = _design_file.MdlFileObjP();
            //    var hresult = mdlModelRef_getDgnFile(modelRef);
            //}
            //catch
            //{

            //}

            BCOM.Attachments attachments = _model_reference.Attachments;

            if (attachments.Count != 0)
            {
                var attachmentEnumerator = attachments.GetEnumerator();

                while (attachmentEnumerator.MoveNext())
                {
                    BCOM.Attachment attachment = attachmentEnumerator.Current as BCOM.Attachment;

                    int modelRefP = attachment.MdlModelRefP();
                    var hresult = mdlModelRef_getDgnFile(modelRefP);
                }
            }
        }
    }
}
