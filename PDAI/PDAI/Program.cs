using System;
using Parser.Pddl;
using Shared.ExtensionMethods;
using Shared.Models;
using Writer.Popper;

namespace PDAI
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFolderPath = args[0];
            string outputFolderPath = args[1];
            int maxProblems = int.MaxValue;
            if (args.Length > 2)
                maxProblems = int.Parse(args[2]);
            if (maxProblems == -1)
                maxProblems = int.MaxValue;
            double splitPercent = 0.2;
            int numOfChunks = 1;
            if (args.Length > 3)
                splitPercent = double.Parse(args[3]);
            if (args.Length > 4)
                numOfChunks = int.Parse(args[4]);

            var domain = Parse(inputFolderPath, maxProblems);
            Write(outputFolderPath, domain, splitPercent, numOfChunks);
        }

        private static void Write(string outputFolderPath, Domain domain, double splitPercent, int numOfChunks)
        {
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
