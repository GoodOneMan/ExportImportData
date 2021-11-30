using Bentley.ECObjects.Instance;
using Bentley.ECObjects.Schema;
using BIM = Bentley.Internal.MicroStation;
using BIME = Bentley.Internal.MicroStation.Elements;
using BCOM = Bentley.Interop.MicroStationDGN;
using BMI = Bentley.MicroStation.InteropServices;
using Bentley.Properties;
using Bentley.Building.Mechanical.Api;
using Bentley.EC.Persistence;
using Bentley.EC.Persistence.Query;
using Bentley.DGNECPlugin;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;

namespace ExportDataToModel.AppTest
{
    class ModelsHandler
    {
        BCOM.Application app = null;
        List<string> list_MessageList = new List<string>();

        StreamWriter sw = new StreamWriter(@"D:\type.txt");
        public ModelsHandler()
        {
            app = BMI.Utilities.ComApp;

            //Init_1();

            sw.Close();
        }

        #region Init
        public void Init_1()
        {
            var list = GetModelReferenceList();

            foreach(BCOM.ModelReference model in list)
            {
                BCOM.ElementEnumerator elements = model.GraphicalElementCache.Scan();

                while (elements.MoveNext())
                {
                    BCOM.Element comElement = elements.Current;

                    BIME.Element element = BIME.Element.FromElementID((ulong)comElement.ID64, (IntPtr)comElement.ModelReference.MdlModelRefP());
                    ECInstanceList elementProperties = Bentley.Properties.PropertyManager.Instance.GetElementProperties(element, (Bentley.ECObjects.UI.ECPropertyPane.PropertyCategory)(-1), true);

                    sw.Write("IECInstance:  ");
                    foreach (IECInstance instance in elementProperties)
                    {
                        sw.Write(instance.ClassDefinition.Name + "   " + instance.GetType().ToString() + " - ");
                    }


                    sw.WriteLine(element.Type.ToString() + "  " + comElement.Type.ToString());
                }
            }
        }
        #endregion

        #region Recursion attachments
        private List<BCOM.ModelReference> GetModelReferenceList()
        {
            List<BCOM.ModelReference> list = new List<BCOM.ModelReference>();
            BCOM.ModelReference modelRef = app.ActiveModelReference;

            list.Add(modelRef);

            BCOM.Attachments attachments = modelRef.Attachments;

            if(attachments.Count != 0)
            {
                RecursionAttachments(attachments, list);
            }
            return list;
        }

        private void RecursionAttachments(BCOM.Attachments attachments, List<BCOM.ModelReference> list)
        {
            foreach(BCOM.Attachment attachment in attachments)
            {
                try
                {
                    list.Add(attachment.DesignFile.DefaultModelReference);
                }
                catch (Exception ex)
                {
                    list_MessageList.Add("Error in RecursionModelReference: " + attachment.AttachName + " (" + ex.Message + ")");

                }

                if (attachment.Attachments.Count != 0)
                    RecursionAttachments(attachment.Attachments, list);
            }
        }
        #endregion
    }
}
