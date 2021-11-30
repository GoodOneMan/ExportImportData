using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Navisworks.Api;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;

namespace ImportDataOPM.AppUnits
{
    class DataImporter
    {
        public DataImporter(ModelItem model, Dictionary<string, string> dataElement, Dictionary<string, string> failds)
        {
            Dictionary<string, string> prepared_data = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> data in dataElement)
            {
                try
                {
                    prepared_data.Add(failds[data.Key], data.Value);
                }
                catch
                {

                }
            }

            AddData(model, prepared_data);
        }

        private void AddData(ModelItem model, Dictionary<string, string> preparedData)
        {
            ComApi.InwOpState10 opState = ComBridge.State;
            ComApi.InwOaPath oaPath = ComBridge.ToInwOaPath(model);
            ComApi.InwGUIPropertyNode2 propn = (ComApi.InwGUIPropertyNode2)opState.GetGUIPropertyNode(oaPath, true);
            ComApi.InwOaPropertyVec category = (ComApi.InwOaPropertyVec)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOaPropertyVec, null, null);

            try
            {
                foreach (KeyValuePair<string, string> data in preparedData)
                {

                    ComApi.InwOaProperty property = (ComApi.InwOaProperty)opState.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOaProperty, null, null);

                    property.name = data.Key;
                    property.UserName = data.Key;
                    property.value = data.Value;
                    
                    category.Properties().Add(property);
                    property = null;

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            propn.SetUserDefined(0, "OpenPlant Modeler", "DGN_Data", category);
        }
    }
}
