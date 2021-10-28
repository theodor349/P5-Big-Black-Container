using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            long iterations = 1;
            int beta = 2;
            int maxRuntime = 1 * 1 * 60 * 1000; // ms, sec, min, hour

            foreach (var actionPath in actionsPaths)
            {
                GenerateForAction(outputFolderPath, domainFolderPath, actionPath, biasEnumerator, iterations, beta, maxRuntime);
            }
        }

        private void GenerateForAction(string rootPath, string domainPath, string actionPath, IBiasEnumerator biasEnumerator, long iterations, int beta, int maxRuntime)
        {
            for (int i = 0; i < iterations; i++)
            {
                SetInput(actionPath, i, biasEnumerator);
                Train(actionPath, rootPath, beta, maxRuntime);
                Test();
                SaveResults();
            }
        }

        private void SetInput(string actionPath, int iteration, IBiasEnumerator biasEnumerator)
        {
            ConstraintHelper ch = new ConstraintHelper();
            var directoryInfo = new DirectoryInfo(actionPath);
            int directoryCount = directoryInfo.GetDirectories().Length;
            List<string> actionsPaths = Directory.GetDirectories(actionPath).ToList();

            for (int i = 0; i < directoryCount; i++)
            {
                var biasIncrement = biasEnumerator.GetIncrement(iteration);
                ch.IncrementConstraintValues(actionsPaths[i] + "/bias.pl", biasIncrement.Clause, biasIncrement.Body, biasIncrement.Var);
            }
        }

        private void Train(string actionFolderPath, string rootPath, int beta, int maxRuntime)
        {
            var trainingFolders = Directory.GetDirectories(actionFolderPath);
            var threads = new List<Task>();
            foreach (var trainingFolder in trainingFolders)
                threads.Add(RunPopper(trainingFolder, rootPath, beta, maxRuntime));
            Task.WaitAll(threads.ToArray());
        }

        private async Task RunPopper(string trainPath, string rootPath, int beta, int maxRuntime)
        {
            await Task.Run(() =>
            {
                string popperPath = Path.Combine(rootPath, "popper\\popper.py");

                Process popperProcess = new();
                popperProcess.StartInfo.FileName = GetPythonExePath();
                popperProcess.StartInfo.CreateNoWindow = true;
                popperProcess.StartInfo.Arguments = popperPath + " " + trainPath + " " + beta;

                popperProcess.Start();
                popperProcess.WaitForExit(maxRuntime);
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

        private string GetPythonExePath()
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            string pythonPath = null;
            foreach (string p in path.Split(new char[] { ';' }))
            {
                string fullPath = Path.Combine(p, "python.exe");
                if (File.Exists(fullPath))
                {
                    pythonPath = fullPath;
                    break;
                }
            }

            if (pythonPath == null)
                throw new Exception("Unable to find python exe in Environment variables :(");
            else
                return pythonPath;
        }
    }
}
