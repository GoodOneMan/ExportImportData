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


namespace TestProject.TestUnits
{
    class OPMSchems
    {
        BCOM.Application app = null;
        BCOM.DesignFile designFile = null;

        List<string> list_NameProperty = new List<string>();

        StreamWriter sw_propertyValue = new StreamWriter(@"D:\propertyValue.txt");
        StreamWriter sw_propertyName = new StreamWriter(@"D:\propertyName.txt");

        public OPMSchems()
        {
            app = BIM.Utilities.ComApp;
            designFile = app.ActiveDesignFile;

            Init();

            sw_propertyValue.Close();
            sw_propertyName.Close();
        }

        // Run Code
        private void Init()
        {

            BCOM.ElementScanCriteria criteriaScan = new BCOM.ElementScanCriteriaClass();
            criteriaScan.ExcludeNonGraphical();

            BCOM.ElementEnumerator elementEnumeratoe = designFile.DefaultModelReference.GraphicalElementCache.Scan();
            //BCOM.ElementEnumerator elementEnumeratoe = designFile.DefaultModelReference.GraphicalElementCache.Scan(criteriaScan);
            //BCOM.ElementEnumerator elementEnumeratoe = designFile.DefaultModelReference.Scan(criteriaScan);

            while (elementEnumeratoe.MoveNext())
            {
                BCOM.Element element = elementEnumeratoe.Current;

                BCOM.PropertyHandler propertyHandler = app.CreatePropertyHandler(element);

                string[] names = propertyHandler.GetAccessStrings();
                
                foreach(string name in names)
                {
                    if (!list_NameProperty.Contains(name))
                        list_NameProperty.Add(name);

                    if (propertyHandler.SelectByAccessString(name))
                    {
                        try
                        {
                            sw_propertyValue.WriteLine(name + "     " + propertyHandler.GetDisplayString());
                        }
                        catch (Exception ex)
                        {
                            sw_propertyValue.WriteLine(name + "     Error - " + ex.Message);
                        }
                    }
                }
            }

            // 
            foreach(string name in list_NameProperty)
            {
                sw_propertyName.WriteLine(name);
            }
        }
    }
}
