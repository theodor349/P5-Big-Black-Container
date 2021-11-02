using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDAI.Helpers;
using PddlParser.Internal;
using Shared.ExtensionMethods;

namespace PDAI
{
    // John 
    public class DataGenerator
    {
        public void GenerateData(string inputFolderPath, string rootBbcFolder)
        {
            Console.WriteLine("");
            Logger.Log("Generating Data");
            string domainFolderPath = Path.Combine(rootBbcFolder, "domainfiles", Path.GetFileName(inputFolderPath));
            var actionsPaths = Directory.GetDirectories(domainFolderPath).ToList();

            var biasEnumerator = new BiasAllEnumerator();
            int iterations = 5;
            int beta = 2;
            int maxRuntime = 1 * 4 * 60 * 1000; // hour, min, sec, ms 

            foreach (var actionPath in actionsPaths)
            {
                Logger.Log("Generating data for action: " + Path.GetFileName(actionPath));
                GenerateForAction(rootBbcFolder, domainFolderPath, actionPath, biasEnumerator, iterations, beta, maxRuntime);
                Console.WriteLine("");
            }
        }

        private void GenerateForAction(string rootPath, string domainPath, string actionPath, IBiasEnumerator biasEnumerator, int iterations, int beta, int maxRuntime)
        {
            for (int i = 0; i < iterations; i++)
            {
                Logger.Log("Iteration: " + i);
                SetInput(actionPath, i, biasEnumerator);
                Train(actionPath, rootPath, beta, maxRuntime);
                Test(rootPath, actionPath);
                SaveResults(rootPath, actionPath);
            }
        }

        private void SetInput(string actionPath, int iteration, IBiasEnumerator biasEnumerator)
        {
            ConstraintHelper ch = new ConstraintHelper();
            var directoryInfo = new DirectoryInfo(actionPath);
            int directoryCount = directoryInfo.GetDirectories().Length;
            List<string> trainingFolders = GetTrainingFolders(actionPath);

            foreach (var trainingFolder in trainingFolders)
            {
                var biasIncrement = biasEnumerator.GetIncrement(iteration);
                ch.IncrementConstraintValues(Path.Combine(trainingFolder, "bias.pl"), biasIncrement.Clause, biasIncrement.Body, biasIncrement.Var);
            }
        }

        private void Train(string actionFolderPath, string rootPath, int beta, int maxRuntime)
        {
            var trainingFolders = GetTrainingFolders(actionFolderPath);
            var threads = new List<Task>();
            foreach (string trainingFolder in trainingFolders)
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
                popperProcess.StartInfo.CreateNoWindow = false;
                popperProcess.StartInfo.Arguments = popperPath + " " + trainPath + " " + beta;

                popperProcess.Start();
                popperProcess.WaitForExit(maxRuntime);
                popperProcess.Kill();
            });
        }

        private void Test(string rootPath, string actionFolderPath)
        {
            var threads = new List<Task>();
            var trainingFolders = GetTrainingFolders(actionFolderPath);
            foreach (var trainingFolder in trainingFolders)
                threads.Add(RunTest(rootPath, trainingFolder));
            Task.WaitAll(threads.ToArray());
        }

        private async Task RunTest(string rootPath, string trainPath)
        {
            await Task.Run(() =>
            {
                string testerPath = Path.Combine(rootPath, "tester.py");

                Process process = new();
                process.StartInfo.FileName = GetPythonExePath();
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.Arguments = testerPath + " " + trainPath;

                process.Start();
                process.WaitForExit();
                process.Kill();
            });
        }

        private void SaveResults(string rootPath, string actionFolderPath)
        {
            var trainingFolders = GetTrainingFolders(actionFolderPath);
            foreach (var trainingFolder in trainingFolders)
                TempFileManager.SaveStats(rootPath, trainingFolder);
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

        private static List<string> GetTrainingFolders(string actionFolderPath, bool includeTest = false)
        {
            var res = Directory.GetDirectories(actionFolderPath).ToList();
            if (includeTest)
                return res;
            else 
                return res.Where(x => !new DirectoryInfo(x).Name.ToLower().Equals("test")).ToList();
        }
    }
}
