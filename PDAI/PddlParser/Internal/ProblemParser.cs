using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;
using Shared.Models;

namespace Parser.Pddl.Internal
{
    internal class ProblemParser {
        class InitGoalState
        {
            public List<PredicateOperator> Init { get; set; }
            public List<PredicateOperator> Goal { get; set; }
        }

        static string tempFile { get; set; } = "decompressed";
        static string goodOperatorFile { get; set; } = "good_operators";
        static string allOperatorFile { get; set; } = "all_operators.bz2";

        internal Problem Parse(string folderPath)
        {
            var problem = new Problem();

            problem.GoodOperators = GetGoodOperators(folderPath);
            problem.BadOperators = GetBadOperators(problem.GoodOperators, folderPath);
            var states = GetInitAndGoalState(folderPath);
            problem.InitalState = states.Init;
            problem.GoalState = states.Goal;

            if (problem.GoodOperators is null || problem.BadOperators is null)
                return null;
            else if (problem.InitalState is null || problem.GoalState is null)
                return null;
            else
                return problem;
        }

        private InitGoalState GetInitAndGoalState(string folderPath)
        {
            return null;
        }

        private List<ActionOperator> GetBadOperators(List<ActionOperator> goodOperators, string folderPath)
        {
            if (goodOperatorFile is null)
                return null;

            var allOperators = ReadActionBz2File(folderPath + "/" + allOperatorFile, folderPath + "/" + tempFile);
            return allOperators.Where(x => !goodOperators.Contains(x)).ToList();
        }

        private List<ActionOperator> GetGoodOperators(string folderPath)
        {
            return ReadActionFile(folderPath + "/" + goodOperatorFile);
        }

        private List<ActionOperator> ReadActionFile(string path) 
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

        private List<ActionOperator> ReadActionBz2File(string filePath, string tempfilePath)
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

            var res = ReadActionFile(tempfilePath);
            File.Delete(tempfilePath);
            return res;
        }
    }
}
