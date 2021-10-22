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

            var domain = Parse(inputFolderPath, maxProblems);
            Write(outputFolderPath, domain);
        }

        private static void Write(string outputFolderPath, Domain domain)
        {
            var writer = new Popper();
            writer.Write(domain, outputFolderPath, 0.2, 4);
        }

        private static Domain Parse(string inputFolderPath, int maxProblems)
        {
            IPddlParser parser = new Parser.Pddl.PddlParser();
            var domain = parser.Parse(inputFolderPath, maxProblems);
            return domain;
        }
    }
}
