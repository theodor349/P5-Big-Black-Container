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
        }
    }

    class Problem
    {
        public List<string> Positives { get; set; }
        public List<string> Negatives { get; set; }

        public Problem(string folderPath)
        {
            var positive = ReadFile(folderPath + "/" + "good_operators");
            var all = ReadBz2File(folderPath + "/" + "all_operators.bz2");

            Console.WriteLine("Loading done");
        }

        private List<string> ReadBz2File(string filePath)
        {
            var fileStreamIn = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var bz2Stream = new BZip2InputStream(fileStreamIn);
        }

        private List<string> ReadFile(string filePath)
        {
            return File.ReadAllLines(filePath).ToList();
        }
    }
}
