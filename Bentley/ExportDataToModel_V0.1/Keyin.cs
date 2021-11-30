using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ExportDataToModel
{
    public sealed class Keyin
    {
        public static void CmdExportData(string unparsed)
        {
            //AppUnits.ModelHandlers.Model _model = new AppUnits.ModelHandlers.Model();
            //AppUnits.ModelHandlers.ModelStructure _model_data = _model.GetStructure();

            //int total_elements = _model.GetTotalElementCounter(); // ??

            //string data_source = @"D:\\";

            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            //if(fbd.ShowDialog() == DialogResult.OK)
            //{
            //    data_source = fbd.SelectedPath + "\\";
            //}
            //data_source += _model_data.Name.Replace(".dgn", ".db");

            //if (File.Exists(data_source))
            //    File.Delete(data_source);


            //AppUnits.DataBaseHandlers.DataHandler _data_base = new AppUnits.DataBaseHandlers.DataHandler(data_source);

            //foreach (AppUnits.ModelHandlers.Attachment attachment in _model_data.Models)
            //{
            //    _data_base.CreateTableAttachment(attachment.Name);
            //    _data_base.InflateModelData(attachment);
            //}

            //AppUnits.Config.SWLogClose();
            //_data_base.CloseConnect();

            //System.Windows.Forms.MessageBox.Show(" свойства перенесены ", "создание базы", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }


        public static void CmdTest(string unparsed)
        {
            //AppUnits.ModelHandlers.Model _model = new AppUnits.ModelHandlers.Model();
            //AppUnits.ModelHandlers.ModelStructure _model_data = _model.GetStructure();

            //var test = new AppUnits.TestR();
            var model = new AppUnits.ModelsHandler();
            var structure = model.GetStructure();
            var messages = model.GetMessages();

            MessageBox.Show("Ok");
        }
    }
}
