using System;
using System.IO;
using Parser.Pddl;
using PddlParser.Internal;
using Shared;
using Shared.ExtensionMethods;
using Shared.Models;
using Writer.Popper;

namespace PDAI
{
    class Program
    {
        static Settings _settings => Settings.Current;

        static void Main(string[] args)
        {
            Settings.Current = new Settings(args);

            string inputFolderPath = _settings.DomainFolder;
            string outputFolderPath = _settings.TargetFolder;
            int maxProblems = _settings.MaxProblems;
            double splitPercent = _settings.SplitPercent;
            int numOfChunks = _settings.NumChunks;
            bool runInfinite = _settings.RunInfinite;
            int maxRuntime = _settings.MaxRuntime;
            int beta = _settings.Beta;

            var domain = Parse(inputFolderPath, maxProblems);
            domain.Name = Path.GetFileName(inputFolderPath);
            Write(outputFolderPath, domain, splitPercent, numOfChunks);

            if (runInfinite)
                new InifiniteDataGenerator().GenerateData();
            else
                new DataGenerator().GenerateData(outputFolderPath, domain.Name, beta, maxRuntime);
        }

        private static void Write(string outputFolderPath, Domain domain, double splitPercent, int numOfChunks)
        {
            outputFolderPath = Path.Combine(outputFolderPath, "domainfiles", domain.Name);

            var writer = new Popper();
            writer.Write(domain, outputFolderPath, splitPercent, numOfChunks);
        }

        private static Domain Parse(string inputFolderPath, int maxProblems)
        {
            IPddlParser parser = new Parser.Pddl.PddlParser();
            var domain = parser.Parse(inputFolderPath, maxProblems);
            return domain;
        }
    }
}
