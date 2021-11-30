using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GlueTwoTextFilesForTranslation_BentleyProperties
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> list_EN = GetLines(@"D:\DataTestPropertyEn.txt");
            List<string> list_RU = GetLines(@"D:\DataTestPropertyRu.txt");

            WriteToFile(list_EN, list_RU, @"D:\DataTestPropertyTranslation.txt");
        }

        private static List<string> GetLines(string file)
        {
            List<string> list = new List<string>();
            using(StreamReader sr = new StreamReader(file))
            {
                string line = "";

                while((line = sr.ReadLine()) != null)
                {
                    list.Add(line.Trim());
                }
            }
            return list;
        }

        private static void WriteToFile(List<string> list_EN, List<string> list_RU, string file)
        {
            using(StreamWriter sw = new StreamWriter(file))
            {
                int count_en = list_EN.Count;
                int count_ru = list_RU.Count;

                if(count_en == count_ru)
                {
                    for(int i = 0; i < count_en; i++)
                    {
                        sw.WriteLine(list_EN[i] + " = " + list_RU[i]);
                    }

                    Console.WriteLine("Ok");
                }
                else
                {
                    Console.WriteLine("Error");
                }

            }

            Console.ReadKey();
        }
    }
}
