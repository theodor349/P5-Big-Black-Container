using Parser.Pddl.Internal;
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
            var domainParser = new DomainParser();
            var problemParser = new ProblemParser();

            var domain = new Domain();
            domainParser.Parser(domainFolderPath + "/domain.pddl", domain);

            var problemsFolder = Directory.GetDirectories(domainFolderPath + "/runs/optimal");

            var threads = new List<Task<Problem>>();
            for (int i = 0; i < problemsFolder.Length; i++)
            {
                //var problem = problemParser.Parse(problemsFolder[i]);
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

        private class ProblemThread
        {
            public Problem ProblemData { get; set; }
            public string Folder { get; set; }
            public ProblemParser ProblemParser { get; set; }

            public void DoWork()
            {
                //var problemThread = new ProblemThread();
                //problemThread.Folder = problemsFolder[i];
                //problemThread.ProblemParser = problemParser;
                //var t = new Thread(new ThreadStart(problemThread.DoWork));
                //t.Start();

                ProblemData = ProblemParser.Parse(Folder);
            }
        }
    }
}
