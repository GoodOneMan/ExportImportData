using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoupdateModels.Source
{
    class Helper
    {
        // Size memory
        public static string SizeMemory(long memory)
        {
            try
            {
                double sizeinbytes = Convert.ToDouble(memory);
                double sizeinkbytes = Math.Round((sizeinbytes / 1024), 3);
                double sizeinmbytes = Math.Round((sizeinkbytes / 1024), 3);
                double sizeingbytes = Math.Round((sizeinmbytes / 1024), 3);
                if (sizeingbytes > 1)
                    return string.Format("{0} GB", sizeingbytes); //размер в гигабайтах
                else if (sizeinmbytes > 1)
                    return string.Format("{0} MB", sizeinmbytes); //возвращает размер в мегабайтах, если размер файла менее одного гигабайта
                else if (sizeinkbytes > 1)
                    return string.Format("{0} KB", sizeinkbytes); //возвращает размер в килобайтах, если размер файла менее одного мегабайта
                else
                    return string.Format("{0} B", sizeinbytes); //возвращает размер в байтах, если размер файла менее одного килобайта
            }
            catch { return "Ошибка получения размера файла"; } //перехват ошибок и возврат сообщения об ошибке
        }

        // File is lock
        public static bool FileIsLocked(string filename, FileAccess file_access)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, file_access);
                fs.Close();
                return false;
            }
            catch (IOException)
            {
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
