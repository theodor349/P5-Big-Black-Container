using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PDAI.Helpers
{
    public class BalancingExamplesGenerator
    {
        public void BalanceExampleFile(string filename)
        {
            List<string> balancedExamples = TrimFile(filename);
            File.WriteAllLines(filename, balancedExamples);
        }

        private List<string> GetExampleFile(string filename)
        {
            return File.ReadLines(filename).ToList();
        }

        private int CountPositiveExamples(string filename)
        {
            List<string> examples = GetExampleFile(filename);
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

        private List<string> TrimFile(string filename)
        {
            int numOfPositiveExamples = CountPositiveExamples(filename);
            List<string> examples = GetExampleFile(filename);

            int numOfNegativeExamples = examples.Count - numOfPositiveExamples;

            while (numOfPositiveExamples < numOfNegativeExamples)
            {
                List<int> negativeIndices = examples.Select((example, index) => example.StartsWith("neg") ? index : -1).Where(i => i != -1).ToList();
                int index = new Random().Next(0, negativeIndices.Count - 1);

                examples.RemoveAt(index);
                numOfNegativeExamples--;
            }

            return examples;
        }
    }
}
