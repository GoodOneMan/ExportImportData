using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace CompressionResource
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input file path");
            string file = Console.ReadLine();

            if (File.Exists(file))
            {
                string dirSave = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
                string fileSave = dirSave + Path.GetFileName(file) + ".deflated";


                byte[] assembly = File.ReadAllBytes(file);

                using (FileStream fileStream = File.Open(fileSave, FileMode.Create))
                using (DeflateStream stream = new DeflateStream(fileStream, CompressionMode.Compress))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(assembly);
                }

                Console.WriteLine("File save: " + fileSave);
            }
            else
            {
                Console.WriteLine("File not found");
            }

            Console.ReadKey();
        }
    }
}
