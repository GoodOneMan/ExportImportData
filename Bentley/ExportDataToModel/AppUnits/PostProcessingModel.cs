using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ExportDataToModel.AppUnits
{
    class PostProcessingModel
    {
        DataModelBentleyOPM.Model model = null;

        public PostProcessingModel(DataModelBentleyOPM.Model structure)
        {
            model = structure;

            BinarySerialize();
        }

        private void BinarySerialize()
        {
            string tempFile = Path.GetTempFileName();
            string fileSave = Path.GetDirectoryName(model.Name) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(model.Name) + ".dat";

            // создаем объект BinaryFormatter
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream(tempFile, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, model);
            }

            try
            {
                File.Copy(tempFile, fileSave, true);
            }
            catch
            {
                File.Copy(tempFile, fileSave + "_tmp", true);
            }
        }
    }
}
