using Parser.Pddl.Internal;
using Shared.ExtensionMethods;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parser.Pddl
{
    public class PddlParser : IPddlParser
    {
        public Domain Parse(string domainFolderPath, int maxProblems = int.MaxValue)
        {
            Logger.Log("Parsing Domain");
            var domainParser = new DomainParser();
            var domain = new Domain();
            domainParser.Parser(domainFolderPath + "/domain.pddl", domain);

            var problemsFolder = Directory.GetDirectories(domainFolderPath + "/runs/optimal");
            Logger.Log("Found " + Math.Min(problemsFolder.Length, maxProblems) + " problems");
            Logger.Log("Loading Problems");

            var threads = new List<Task<Problem>>();
            for (int i = 0; i < problemsFolder.Length && i < maxProblems; i++)
            {
                threads.Add(ParseTask(problemsFolder[i]));
            }
            Task.WaitAll(threads.ToArray());
            for (int i = 0; i < threads.Count; i++)
            {
                var p = threads[i].Result;
                if(p is not null)
                {
                    p.Name = "p" + i;
                    domain.Problems.Add(p);
                }
            }

            return domain;
        }

        private async Task<Problem> ParseTask(string folderPath)
        {
            var problemParser = new ProblemParser();
            return await Task.Run(() =>
            {
                return problemParser.Parse(folderPath);
            });
        }
    }
}
