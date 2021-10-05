﻿using Parser.Internal;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Pddl
{
    public class PddlParser
    {
        // C:\Users\theod\Documents\P5\useful-actions-dataset-main\depots
        public void Parse(string domainFolderPath, int maxProblems = int.MaxValue)
        {
            var domainParser = new DomainParser();
            var problemParser = new ProblemParser();

            var domain = new Domain();
            domainParser.Parser("fileName.pddl", domain);

            var problemsFolder = new List<string>();
            foreach (var folder in problemsFolder)
            {
                problemParser.Parse(folder, domain);
            }
        }
    }
}
