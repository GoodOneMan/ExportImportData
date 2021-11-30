using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace ImportDataOPM.AppUnits
{
    class DataLoader
    {
        DataModelBentleyOPM.Model model = null;
        string filePath = null;

        MessageForm messageForm = null;

        public DataLoader(MessageForm form)
        {
            messageForm = form;

            LoadFileModel();

            BinaryDeserialize();
        }


        public DataModelBentleyOPM.Model GetModel()
        {
            return model;
        }

        private void LoadFileModel()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
        }

        private void BinaryDeserialize()
        {
            if (File.Exists(filePath))
            {
                messageForm.SetHeader("Файл с данными загружается");

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        model = (DataModelBentleyOPM.Model)formatter.Deserialize(fs);
                    }
                }
                catch(Exception ex)
                {
                    messageForm.SetHeader(ex.Message);
                }
            }
            else
            {
                messageForm.SetHeader("Файл не найден");
            }
        }
    }
}
