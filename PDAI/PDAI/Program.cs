﻿using System;
using Parser.Pddl;
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

            bool v = a.Equals(b);

            string folderPath = args[0];
            IPddlParser parser = new PddlParser();
            parser.Parse(folderPath);
        }
    }
}
