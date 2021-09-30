using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParseNegativeAndPositive
{
    class Program
    {
        static void Main(string[] args)
        {
            var problem = new Problem(@"C:\Users\theod\Documents\P5\useful-actions-dataset-main\blocksworld\runs\optimal\p4-1");
        }
    }

    class Problem
    {
        public List<string> Positives { get; set; }
        public List<string> Negatives { get; set; }

        public Problem(string folderPath)
        {
            var positive = ReadFile(folderPath + "/good_operators");
            var all = ReadBz2File(folderPath + "/all_operators.bz2", folderPath + "/decompressed.txt");

            Console.WriteLine("Loading done");
        }

        private List<string> ReadBz2File(string filePath, string tempfilePath)
        {
            var zipFileName = new FileInfo(filePath);
            using var fileToDecompressAsStream = zipFileName.OpenRead();
            using var decompressedStream = File.Create(tempfilePath);

            try
            {
                BZip2.Decompress(fileToDecompressAsStream, decompressedStream, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var res = ReadFile(tempfilePath);
            File.Delete(tempfilePath);
            return res;
        }

        private List<string> ReadFile(string filePath)
        {
            return File.ReadAllLines(filePath).ToList();
        }
    }
}
