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
            //List<string> balancedExamples = AddExamplesToBalance(trainingFolder);

            File.WriteAllLines(Path.Combine(trainingFolder, "exs.pl"), balancedExamples);
        }

        private List<string> GetExampleFile(string trainingFolder)
        {
            return File.ReadLines(Path.Combine(trainingFolder, "exs.pl")).ToList();
        }

        private int CountPositiveExamples(List<string> examples)
        {
            int numOfPositiveExamples = 0;
            for(int i = 1; i < examples.Count; i++)
            {
                if (examples[i].StartsWith("pos"))
                {
                    numOfPositiveExamples++;
                }
            }

            return numOfPositiveExamples;
        }

        private List<string> TrimExamplesToBalance(string trainingFolder)
        {
            List<string> examples = GetExampleFile(trainingFolder);
            int numOfPositiveExamples = CountPositiveExamples(examples);
            int numOfNegativeExamples = (examples.Count - 1) - numOfPositiveExamples;

            Random random = new Random();

            if (numOfPositiveExamples == 0 || numOfNegativeExamples == 0)
            {
                return examples;
            }
            while (numOfPositiveExamples < numOfNegativeExamples)
            {
                List<int> negativeIndices = examples.Select((example, index) => example.StartsWith("neg") ? index : -1).Where(i => i != -1).ToList();
                int index = negativeIndices[random.Next(negativeIndices.Count)];

                examples.RemoveAt(index);
                numOfNegativeExamples--;
            }

            while (numOfNegativeExamples < numOfPositiveExamples)
            {
                List<int> positiveIndices = examples.Select((example, index) => example.StartsWith("pos") ? index : -1).Where(i => i != -1).ToList();
                int index = positiveIndices[random.Next(positiveIndices.Count)];

                examples.RemoveAt(index);
                numOfPositiveExamples--;
            }

            return examples;
        }

        private List<string> AddExamplesToBalance(string trainingFolder)
        {
            List<string> examples = GetExampleFile(trainingFolder);
            int numOfPositiveExamples = CountPositiveExamples(examples);
            int numOfNegativeExamples = (examples.Count - 1) - numOfPositiveExamples;
            Random random = new Random();

            if (numOfPositiveExamples == 0 || numOfNegativeExamples == 0) {
                return examples;
            }
            while (numOfPositiveExamples < numOfNegativeExamples)
            {
                List<int> positiveIndices = examples.Select((example, index) => example.StartsWith("pos") ? index : -1).Where(i => i != -1).ToList();
                int index = positiveIndices[random.Next(positiveIndices.Count)];

                examples.Add(examples[index]);
                numOfPositiveExamples++;
            }

            while (numOfNegativeExamples < numOfPositiveExamples)
            {
                List<int> negativeIndices = examples.Select((example, index) => example.StartsWith("neg") ? index : -1).Where(i => i != -1).ToList();
                int index = negativeIndices[random.Next(negativeIndices.Count)];

                examples.Add(examples[index]);
                numOfNegativeExamples++;
            }

            return examples;
        }
    }
}
