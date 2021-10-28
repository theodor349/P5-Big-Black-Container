using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDAI.Helpers;
using PddlParser.Internal;

namespace PDAI
{
    // John 
    public class DataGenerator
    {
        public void GenerateData(string inputFolderPath, string outputFolderPath)
        {
            string domainFolderPath = Path.Combine(outputFolderPath, "domainfiles", Path.GetFileName(inputFolderPath));
            var actionsPaths = Directory.GetDirectories(domainFolderPath).ToList();
            var biasEnumerator = new BiasVarEnumerator();

            foreach (var actionPath in actionsPaths)
            {
                GenerateForAction(outputFolderPath, domainFolderPath, actionPath, biasEnumerator);
            }
        }

        private void GenerateForAction(string rootPath, string domainPath, string actionPath, IBiasEnumerator biasEnumerator)
        {
            for (int i = 0; i < 10; i++)
            {
                SetInput(actionPath, i, biasEnumerator);
                Train();
                Test();
                SaveResults();
            }
        }

        private void SetInput(string actionPath, int iteration, IBiasEnumerator biasEnumerator)
        {
            // gå ind i mappen
            ConstraintHelper ch = new ConstraintHelper();
            var directoryInfo = new DirectoryInfo(actionPath);
            int directoryCount = directoryInfo.GetDirectories().Length;
            List<string> lines = new List<string>();
            List<string> actionsPaths = Directory.GetDirectories(actionPath).ToList();

            for (int i = 0; i < directoryCount; i++)
            {
                // access hver træningssplit og opdater bias fil (vars, body, clauses)

            }

        }

        private void Train()
        {
            var threads = new List<Task>();
            for (int j = 0; j < 10; j++)
                threads.Add(RunPopper(""));
            Task.WaitAll(threads.ToArray());
        }

        private async Task RunPopper(string trainingFolder)
        {
            await Task.Run(() =>
            {
            });
        }

        private void Test()
        {
            var threads = new List<Task>();
            for (int j = 0; j < 10; j++)
                threads.Add(RunTest());
            Task.WaitAll(threads.ToArray());
        }

        private async Task RunTest()
        {
            await Task.Run(() =>
            {
            });
        }

        private void SaveResults()
        {

        }
    }
}
