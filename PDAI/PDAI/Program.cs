using System;
using Parser.Pddl;

namespace PDAI
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = args[0];
            var parser = new PddlParser();
            parser.Parse(folderPath);
        }
    }
}
