using System;
using System.IO;
using Parser.Pddl;
using PddlParser.Internal;
using Shared.ExtensionMethods;
using Shared.Models;
using Writer.Popper;

namespace PDAI
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFolderPath = "";
            string outputFolderPath = "";
            int maxProblems = int.MaxValue;
            double splitPercent = 0.2;
            int numOfChunks = 1;
            bool runInfinite = false;
            int actionToRun = 0;
            int maxRuntime = 1 * 4 * 60 * 1000; // hour, min, sec, ms
            int beta = 2;
            int forward = int.MaxValue;
            if (args.Length > 1)
            {
                inputFolderPath = args[0];
                outputFolderPath = args[1];
            }
            if(args.Length > 2)
            {
                maxProblems = int.Parse(args[2]);
                if (maxProblems == -1)
                    maxProblems = int.MaxValue;
            }
            if (args.Length > 3)
                splitPercent = double.Parse(args[3]);
            if (args.Length > 4)
                numOfChunks = int.Parse(args[4]);
            if (args.Length > 5)
                runInfinite = int.Parse(args[5]) == 1;
            if (args.Length > 6)
                actionToRun = int.Parse(args[6]);
            if (args.Length > 7)
                maxRuntime = int.Parse(args[7]);
            if (args.Length > 8)
                beta = int.Parse(args[8]);
            if (args.Length > 9)
                forward = int.Parse(args[9]);


            var domain = Parse(inputFolderPath, maxProblems);
            domain.Name = Path.GetFileName(inputFolderPath);
            Write(outputFolderPath, domain, splitPercent, numOfChunks);

            if (runInfinite)
                new InifiniteDataGenerator().GenerateData(outputFolderPath, domain.Name, beta, maxRuntime, actionToRun, Popper.MinVars, forward);
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
