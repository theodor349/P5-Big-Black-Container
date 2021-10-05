using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParseNegativeAndPositive
{
    class Problem
    {
        public string Name { get; set; }
        public List<Command> Positives { get; set; } = new List<Command>();
        public List<Command> Negatives { get; set; } = new List<Command>();

        public Problem(string folderPath)
        {
            var positiveLines = ReadFile(folderPath + "/good_operators");
            var allLines = ReadBz2File(folderPath + "/all_operators.bz2", folderPath + "/decompressed.txt");

            var positives = ParseLines(positiveLines);
            var negatives = ParseLines(allLines);

            RemovePositives(negatives, positives);
        }

        private void RemovePositives(List<Command> negatives, List<Command> positives)
        {
            var toBeRemoved = new List<int>();
            for (int i = 0; i < negatives.Count; i++)
            {
                Command command = negatives[i];
                if (positives.Contains(command))
                    toBeRemoved.Add(i);
            }

            toBeRemoved.Reverse();
            foreach (var index in toBeRemoved)
            {
                negatives.RemoveAt(index);
            }
        }

        private List<Command> ParseLines(List<string> lines)
        {
            var res = new List<Command>();
            foreach (var line in lines)
            {
                res.Add(new Command(line));
            }
            return res;
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
