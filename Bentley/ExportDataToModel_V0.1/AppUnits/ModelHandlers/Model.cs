using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using BCOM = Bentley.Interop.MicroStationDGN;
using BIM = Bentley.MicroStation.InteropServices;
using System.Threading;

namespace ExportDataToModel.AppUnits.ModelHandlers
{
    class Model
    {
        BCOM.Application _app = null;
        BCOM.DesignFile _design_file = null;
        BCOM.ModelReference _model_reference = null;

        ModelStructure _data = null;

        Dictionary<string, string> _fields = null;
        StreamWriter _log = null;


        public Model()
        {
            _app = BIM.Utilities.ComApp;
            _design_file = _app.ActiveDesignFile;
            _model_reference = _app.ActiveModelReference;

            _fields = Config.GetFields();
            _log = Config.SWLog();
        }

        public ModelStructure GetStructure()
        {
            Init();


            return _data;
        }

        #region Init Structure Model
        private void Init()
        {
            _data = new ModelStructure();
            _data.Name = _design_file.Name;
            _data.Models = new List<Attachment>();

            BCOM.Attachments attachments = _model_reference.Attachments;

            if (attachments.Count == 0)
            {
                GetModel(_model_reference);
            }
            else
            {
                GetModel(_model_reference);
                GetAttachments(_model_reference);
            }
        }

        // Attachments
        private void GetModel(BCOM.ModelReference model)
        {
            _data.Models.Add(
                    new Attachment
                    {
                        Name = model.DesignFile.Name,
                        Levels = GetLevels(model)
                    }
                );
        }
        // Models
        private void GetAttachments(BCOM.ModelReference model)
        {
            RecursionAttachments(_model_reference.Attachments);
        }
        // Recursive traversal of attachments sheet filling
        private void RecursionAttachments(BCOM.Attachments attachments)
        {
            foreach (BCOM.Attachment attachment in attachments)
            {
                string name = null;

                if (attachment.LogicalName.Equals(""))
                {
                    name = attachment.AttachName;
                }
                else
                {
                    name = attachment.LogicalName + ", " + attachment.AttachName;
                }

                try
                {
                    BCOM.ModelReference model = attachment.DesignFile.DefaultModelReference;
                    _data.Models.Add(
                        new Attachment
                        {
                            Name = name,
                            Levels = GetLevels(attachment.DesignFile.DefaultModelReference)
                        }
                    );
                }
                catch { }
                if (attachment.Attachments != null)
                    RecursionAttachments(attachment.Attachments);
            }
        }

        #region Elements in model
        private List<Level> GetLevels(BCOM.ModelReference model)
        {
            List<Level> list = new List<Level>();
            BCOM.Levels levels = model.Levels;

            foreach (BCOM.Level level in levels)
            {
                if (level.IsInUse)
                {
                    try
                    {
                        BCOM.ElementScanCriteria scan_criteria = new BCOM.ElementScanCriteriaClass();
                        scan_criteria.ExcludeAllLevels();
                        scan_criteria.IncludeLevel(level);

                        BCOM.ElementEnumerator elements = model.Scan(scan_criteria);

                        list.Add(
                            new Level
                            {
                                Name = level.Name,
                                Elements = GetElements(elements)
                            }
                        );
                    }
                    catch (Exception ex)
                    {
                        _log.WriteLine("Error : Model -> GetLevels: " + ex.Message);
                    }
                }
            }
            return list;
        }
        // Element list
        private List<Element> GetElements(BCOM.ElementEnumerator elements)
        {
            List<Element> list = new List<Element>();

            while (elements.MoveNext())
            {
                BCOM.Element element = elements.Current;

                if (element.IsGraphical)
                {
                    string id = element.ID.ToString();

                    list.Add(
                        new Element
                        {
                            ID_Element = id,
                            Properties = GetProperties(element)
                        }
                    );
                }
            }
            return list;
        }
        // Property list
        private List<Property> GetProperties(BCOM.Element element)
        {
            List<Property> list = new List<Property>();
            BCOM.PropertyHandler propery_handler = _app.CreatePropertyHandler(element);
            foreach (KeyValuePair<string, string> field in _fields)
            {
                try
                {
                    if (propery_handler.SelectByAccessString(field.Key))
                    {
                        if (!propery_handler.GetDisplayString().Equals(""))
                        {
                            list.Add(
                                new Property
                                {
                                    Name = field.Key,
                                    Value = propery_handler.GetDisplayString()
                                }
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.WriteLine("Error : Model -> GetProperties: " + field.Key + " - " + ex.Message);
                }
            }

            return list;
        }
        #endregion

        #endregion

    }
}
