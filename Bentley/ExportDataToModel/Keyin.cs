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
            // read model
            //AppUnits.ModelsHandler model = new AppUnits.ModelsHandler();
            //AppUnits.ModelsHandlerModification model = new AppUnits.ModelsHandlerModification();
            AppUnits.ModelsHandlerModificationOne model = new AppUnits.ModelsHandlerModificationOne();

            DataModelBentleyOPM.Model structure = model.GetStructure();

            // Write to log message
            List<string> messages = model.GetMessages();

            // Serialization data
            new AppUnits.PostProcessingModel(structure);

            MessageBox.Show(" свойства перенесены ", "создание базы", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        public static void CmdTest(string unparsed)
        {
            //new AppTest.ModelsHandler();
            MessageBox.Show("Test ok");
        }
    }
}
