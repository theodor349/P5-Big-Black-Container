using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PDAI.Helpers
{
    public class BalancingExamplesGenerator
    {
        public void GenerateBalanceExampleFile(string trainingFolder)
        {
            List<string> balancedExamples = TrimExamplesToBalance(trainingFolder);
            //List<string> balancedExamples = AddExamplesToBalance();

            File.WriteAllLines("exs.pl", balancedExamples);
        }

        private List<string> GetExampleFile(string trainingFolder)
        {
            return File.ReadLines(Path.Combine(trainingFolder, "exs.pl")).ToList();
        }

        private int CountPositiveExamples(string trainingFolder)
        {
            List<string> examples = GetExampleFile(trainingFolder);
            int numOfPositiveExamples = 0;
            for(int i = 0; i < examples.Count; i++)
            {
                if (examples[i].ToString().StartsWith("pos"))
                {
                    numOfPositiveExamples++;
                }
            }

            return numOfPositiveExamples;
        }

        private List<string> TrimExamplesToBalance(string trainingFolder)
        {
            List<string> examples = GetExampleFile(trainingFolder);
            int numOfPositiveExamples = CountPositiveExamples(trainingFolder);
            int numOfNegativeExamples = examples.Count - numOfPositiveExamples;

            while (numOfPositiveExamples < numOfNegativeExamples)
            {
                List<int> negativeIndices = examples.Select((example, index) => example.StartsWith("neg") ? index : -1).Where(i => i != -1).ToList();
                int index = new Random().Next(0, negativeIndices.Count - 1);

                examples.RemoveAt(index);
                numOfNegativeExamples--;
            }

            while (numOfNegativeExamples < numOfPositiveExamples)
            {
                List<int> positiveIndices = examples.Select((example, index) => example.StartsWith("pos") ? index : -1).Where(i => i != -1).ToList();
                int index = new Random().Next(0, positiveIndices.Count - 1);

                examples.RemoveAt(index);
                numOfPositiveExamples--;
            }

            return examples;
        }

        private List<string> AddExamplesToBalance(string trainingFolder)
        {
            int numOfPositiveExamples = CountPositiveExamples(trainingFolder);
            List<string> examples = GetExampleFile(trainingFolder);
            int numOfNegativeExamples = examples.Count - numOfPositiveExamples;

            while (numOfPositiveExamples < numOfNegativeExamples)
            {
                List<int> positiveIndices = examples.Select((example, index) => example.StartsWith("pos") ? index : -1).Where(i => i != -1).ToList();
                int index = new Random().Next(0, positiveIndices.Count - 1);

                examples.Add(examples[index]);
                numOfPositiveExamples++;
            }

            while (numOfNegativeExamples < numOfPositiveExamples)
            {
                List<int> negativeIndices = examples.Select((example, index) => example.StartsWith("neg") ? index : -1).Where(i => i != -1).ToList();
                int index = new Random().Next(0, negativeIndices.Count - 1);

                examples.Add(examples[index]);
                numOfNegativeExamples++;
            }

            return examples;
        }
    }
}
