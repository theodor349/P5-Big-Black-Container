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
            var line = "at a b c";
            var a = new ActionOperator(line);
            var b = new ActionOperator(line);

            string inputFolderPath = args[0];
            string outputFolderPath = args[1];
            int maxProblems = int.MaxValue;
            if (args.Length > 2)
                maxProblems = int.Parse(args[2]);

            IPddlParser parser = new Parser.Pddl.PddlParser();
            var domain = parser.Parse(inputFolderPath, maxProblems);

            var writer = new Popper();
            writer.Write(domain, outputFolderPath, 0.2, 4);
        }
    }
}
