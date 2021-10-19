using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;
using Shared.Models;
using System.Text.RegularExpressions;

namespace Parser.Pddl.Internal
{
    internal class ProblemParser {

        static string tempFile { get; set; } = "decompressed";
        static string goodOperatorFile { get; set; } = "good_operators";
        static string allOperatorFile { get; set; } = "all_operators.bz2";
        static string problemFile { get; set; } = "problem.pddl";

        private Regex initReg = new Regex(@"(?i)\(:init(?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)");
        private Regex goalReg = new Regex(@"(?i)\(and(?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)");
        private Regex opReg = new Regex(@"\([\s\S]*?\)");
        
        internal Problem Parse(string folderPath)
        {
            var problem = new Problem();

            problem.GoodOperators = GetGoodOperators(folderPath);
            if (problem.GoodOperators is null || problem.BadOperators is null)
                return null;

            problem.BadOperators = GetBadOperators(problem.GoodOperators, folderPath);
            problem.InitalState = GetStatePredicates(initReg, folderPath);
            problem.GoalState = GetStatePredicates(goalReg, folderPath);

            if (problem.InitalState is null || problem.GoalState is null)
                return null;
            else
                return problem;
        }

        private List<PredicateOperator> GetStatePredicates(Regex regex, string folderPath)
        {
            List<PredicateOperator> res = new List<PredicateOperator>(); 
            string problemPath = folderPath + "/" + problemFile;

            var lines = File.ReadAllLines(problemPath);
            var line = string.Join(' ', lines).Replace("\t", "");
            var stateLine = TrimStateLine(regex.Match(line).Value);

            foreach (Match match in opReg.Matches(stateLine))
            {
                res.Add(new PredicateOperator(match.Value));
            }
            return res;
        }

        private string TrimStateLine(string line) {
            List<string> words = line.Split().ToList();
            words.RemoveAt(0);
            words.RemoveAt(words.Count - 1);
            return string.Join(' ', words);
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
