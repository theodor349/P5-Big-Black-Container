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

            if (!GetOperators(problem, folderPath))
                return null;
            
            return problem;
        }

        private bool GetOperators(Problem problem, string folderPath)
        {
            var goodOperators = ReadFile(folderPath + "/" + goodOperatorFile);
            var allOperators = ReadBz2File(folderPath + "/" + allOperatorFile, folderPath + "/" + tempFile);

            if (goodOperators is null || allOperators is null)
                return false;

            problem.GoodOperators = goodOperators;
            problem.BadOperators = allOperators.Where(x => !goodOperators.Contains(x)).ToList();
            return true;
        }

        private List<ActionOperator> ReadFile(string path) 
        {
            if (!File.Exists(path))
                return null;

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
            if (!File.Exists(filePath))
                return null;

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
