using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;
using Shared.Models;

namespace Parser.Pddl.Internal
{
    internal class ProblemParser {

        static string tempFile { get; set; } = "decompressed";
        static string goodOperatorFile { get; set; } = "good_operators";
        static string allOperatorFile { get; set; } = "all_operators.bz2";

        internal Problem Parse(string folderPath)
        {
            var problem = new Problem();
            var goodOperators = ReadFile(folderPath + "/" + goodOperatorFile);
            var allOperators = ReadBz2File(folderPath + "/" + allOperatorFile, folderPath + "/" + tempFile);

            for (int i = goodOperators.Count - 1; i > 0; i--)
            {
                if(goodOperators.Contains(allOperators[i]))  
                {
                    allOperators.RemoveAt(i);
                }
            }

            problem.GoodOperators = goodOperators;
            problem.BadOperators = allOperators;
            return problem;
        }

        private List<ActionOperator> ReadFile(string path) 
        {
            var res = new List<ActionOperator>();

            var lines = File.ReadAllLines(path);
            foreach(string line in lines) 
            {
                res.Add(new ActionOperator(line));
            }

            return res;
        }

        private List<ActionOperator> ReadBz2File(string filePath, string tempfilePath)
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
    }
}
