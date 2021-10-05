using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;
using Shared.Models;

namespace Parser.Pddl.Internal
{
    internal class ProblemParser {
        internal void Parse(string folderPath, Domain domain)
        {

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
