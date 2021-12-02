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

            string outputFolderPath = _settings.TargetFolder;
            int maxProblems = _settings.MaxProblems;
            double splitPercent = _settings.SplitPercent;
            int numOfChunks = _settings.NumChunks;

            GenerateDomainfilesFolder(outputFolderPath, maxProblems, splitPercent, numOfChunks);

            switch (_settings.Program)
            {
                case 0:
                    new AllActionsDataGenerator(Settings.Current).runSettings();
                    break;
                case 1:
                    new DataSetStatisticsGenerator(_settings);
                    break;
                case 2:
                    new InifiniteDataGenerator().GenerateData();
                    break;
                case 3:
                    new TrainingDataSplitDataGenerator(_settings);
                    break;
                default:
                    Console.WriteLine("I do not know what program that is: " + _settings.Program);
                    break;
            }
        }

        public static void GenerateDomainfilesFolder(string outputFolderPath, int maxProblems, double splitPercent, int numOfChunks, bool randomSplit = false)
        {
            var folders = Directory.GetDirectories(_settings.DomainFolder).Where(x => !new FileInfo(x).Name.Equals(".git")).ToList();
            Domain domain = null;
            foreach (var domainFolder in folders)
            {
                string name = Path.GetFileName(domainFolder);
                Console.WriteLine("Domain.Name: " + name);
                domain = Parse(domainFolder, maxProblems);
                domain.Name = name;
                Write(outputFolderPath, domain, splitPercent, numOfChunks, randomSplit);
            }
        }

        private static void Write(string outputFolderPath, Domain domain, double splitPercent, int numOfChunks, bool randomSplit)
        {
            outputFolderPath = Path.Combine(outputFolderPath, "domainfiles");
            outputFolderPath = Path.Combine(outputFolderPath, domain.Name);
            Console.WriteLine(outputFolderPath);

            var writer = new Popper();
            writer.Write(domain, outputFolderPath, splitPercent, numOfChunks, randomSplit);
        }

        private static Domain Parse(string inputFolderPath, int maxProblems)
        {
            IPddlParser parser = new Parser.Pddl.PddlParser();
            var domain = parser.Parse(inputFolderPath, maxProblems);
            return domain;
        }
    }
}
