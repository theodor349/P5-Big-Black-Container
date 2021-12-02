using System;
using System.IO;
using System.Linq;
using Parser.Pddl;
using PDAI.Helpers;
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

            var folders = Directory.GetDirectories(_settings.DomainFolder).Where(x => !new FileInfo(x).Name.Equals(".git")).ToList();
            Domain domain = null;
            foreach (var domainFolder in folders)
            {
                string name = Path.GetFileName(domainFolder);
                Console.WriteLine("Domain.Name: " + name);
                domain = Parse(domainFolder, maxProblems);
                domain.Name = name;
                Write(outputFolderPath, domain, splitPercent, numOfChunks);
            }

            if (runInfinite)
                new InifiniteDataGenerator().GenerateData();

            switch (_settings.Program)
            {
                case 0:
                    new AllActionsDataGenerator(Settings.Current).runSettings();
                    break;
                case 1:
                    new DataSetStatisticsGenerator(_settings);
                    break;
                default:
                    Console.WriteLine("I do not know what program that is: " + _settings.Program);
                    break;
            }
        }

        private static void Write(string outputFolderPath, Domain domain, double splitPercent, int numOfChunks)
        {
            outputFolderPath = Path.Combine(outputFolderPath, "domainfiles");
            outputFolderPath = Path.Combine(outputFolderPath, domain.Name);
            Console.WriteLine(outputFolderPath);

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
