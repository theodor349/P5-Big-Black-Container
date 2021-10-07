using System;
using Parser.Pddl;
using Writer.Popper;

namespace PDAI
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = args[0];
            IPddlParser parser = new PddlParser();
            parser.Parse(folderPath);
        }
    }
}
