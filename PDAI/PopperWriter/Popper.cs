using PopperWriter;
using Shared.ExtensionMethods;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Writer.Popper
{
    public class Popper
    {
        public static int MinVars = 0;

        public void Write(Domain domain, string folderPath, double testPercent, int numOfSplits)
        {
            domain.Problems.Sort();

            int numOfTestProblems = (int)Math.Round(domain.Problems.Count * testPercent);

            List<Problem> testProblems = domain.Problems.GetRange(domain.Problems.Count - numOfTestProblems, numOfTestProblems);
            domain.Problems.RemoveRange(domain.Problems.Count - numOfTestProblems, numOfTestProblems);

            List<List<Problem>> chunks = GetChunks(domain.Problems, numOfSplits);

            Logger.Log("Printing to Popper");

            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
            Directory.CreateDirectory(folderPath);

            List<Task> threads = new();
            for (int i = 1; i <= 4; i++)
            {
                foreach (var a in domain.Actions)
                {
                    threads.Add(PrintAction(folderPath, testPercent, a, domain, chunks, testProblems, i));
                }
            }
            Task.WaitAll(threads.ToArray());
        }

        private static List<List<Problem>> GetChunks(List<Problem> problems, int numOfChunks)
        {
            List<List<Problem>> chunkies = problems.ChunkBy(numOfChunks);
            List<List<Problem>> chunks = new(new List<Problem>[numOfChunks]);

            for (int i = 0; i < chunkies.Count; i++)
            {
                chunks[i] = new();
                chunks[i].AddRange(chunkies[i]);
                for (int j = 0; j < i; j++)
                {
                    chunks[i].AddRange(chunkies[j]);
                }
            }

            return chunks;
        }

        private static Task PrintAction(string folderPath, double testPercent, Shared.Models.Action action, Domain domain, List<List<Problem>> chunks, List<Problem> testProblems, int actionNumber)
        {
            BackgroundGenerator bgGenerator = new();
            BiasGenerator biasGenerator = new();
            ExampleGenerator exampleGenerator = new();

            return Task.Run(() =>
            {
                string path = Path.Combine(folderPath, action.Name + actionNumber);
                Directory.CreateDirectory(path);

                foreach (List<Problem> chunk in chunks)
                {
                    double chunkPercent = Math.Round(((double)chunk.Count / (double)chunks[^1].Count) * 100);

                    string chunkPath = Path.Combine(path, chunkPercent.ToString());
                    Directory.CreateDirectory(chunkPath);

                    bgGenerator.Write(chunk, Path.Combine(chunkPath, "bk.pl"));
                    biasGenerator.Write(action, chunk, domain.Predicates, Path.Combine(chunkPath, "bias.pl"));
                    exampleGenerator.Write(action, chunk, chunkPath);

                    string domainName = Path.GetFileName(folderPath);

                    PrintStats(domainName, action.Name, chunkPath, chunkPercent, testPercent, chunk);
                }

                string testPath = Path.Combine(path, "test");
                Directory.CreateDirectory(testPath);
                bgGenerator.Write(testProblems, Path.Combine(testPath, "bk.pl"));
                exampleGenerator.WriteTest(action, testProblems, testPath);

                Logger.Log("Done with action: " + action.Name);
            });
        }


        private static void PrintStats(string domainName, string actionName, string chunkPath, double chunkPercent, double testPercent, List<Problem> chunk)
        {
            int numberOfUsefulActions = 0;
            int numberOfUselessActions = 0;
            foreach (Problem problem in chunk)
            {
                numberOfUsefulActions += problem.GoodOperators.Count;
                numberOfUselessActions += problem.BadOperators.Count;
            }

            Task t = File.WriteAllTextAsync(Path.Combine(chunkPath, "temp0.csv"),
                domainName + "," +
                actionName + "," +
                testPercent.ToString(CultureInfo.InvariantCulture) + "," +
                (chunkPercent / 100).ToString(CultureInfo.InvariantCulture) + "," +
                chunk.Count + "," +
                numberOfUsefulActions + "," +
                numberOfUselessActions);
            t.Wait();
        }
    }
}
