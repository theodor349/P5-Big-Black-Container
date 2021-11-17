using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PDAI.Helpers;
using PddlParser.Internal;
using Shared.ExtensionMethods;

namespace PDAI
{
    // John 
    public class DataGenerator
    {
        public void GenerateData(string rootBbcFolder, string domainName, int beta, int maxRuntime)
        {
            Console.WriteLine("");
            Logger.Log("Generating Data");
            var actionsPaths = SystemExtensions.GetActionFolders(rootBbcFolder, domainName);

            var biasEnumerator = new BiasBodyEnumerator();
            int iterations = 1;

            foreach (var actionPath in actionsPaths)
            {
                string actionName = Path.GetFileName(actionPath);
                if (actionName != "ag__finish_round_renew")
                    continue;

                Logger.Log("Generating data for action: " + actionName);
                GenerateForAction(rootBbcFolder, actionPath, biasEnumerator, iterations, beta, maxRuntime);
                Console.WriteLine("");
            }
        }

        private void GenerateForAction(string rootPath, string actionPath, IBiasEnumerator biasEnumerator, int iterations, int beta, int maxRuntime)
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
            List<string> trainingFolders = SystemExtensions.GetTrainingFolders(actionPath);

            foreach (var trainingFolder in trainingFolders)
            {
                //var biasIncrement = biasEnumerator.GetIncrement(iteration);
                //ch.IncrementConstraintValues(Path.Combine(trainingFolder, "bias.pl"), biasIncrement.Clause, biasIncrement.Body, biasIncrement.Var);
                ch.ChangeConstraint(Path.Combine(trainingFolder, "bias.pl"), 6, 6, 6);
            }
        }

        private void Train(string actionFolderPath, string rootPath, int beta, int maxRuntime)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(actionFolderPath);
            /*var threads = new List<Task>();
            foreach (string trainingFolder in trainingFolders)
                threads.Add(RunPopper(trainingFolder, rootPath, beta, maxRuntime));
            Task.WaitAll(threads.ToArray());*/
            foreach (string trainingFolder in trainingFolders)
            {
                Logger.Log("Now training: " + trainingFolder);
                Task trainTask = RunPopper(trainingFolder, rootPath, beta, maxRuntime);
                trainTask.Wait(maxRuntime);
            }
        }

        public async Task RunPopper(string trainPath, string rootPath, int beta, int maxRuntime)
        {
            await Task.Run(() =>
            {
                string popperPath = Path.Combine(rootPath, "popper/popper.py");
                Process popperProcess = new();
                popperProcess.StartInfo.FileName = SystemExtensions.GetPythonPath();
                popperProcess.StartInfo.Arguments = popperPath + " " + trainPath + " " + beta + " --stats --info";
                StartProcess(popperProcess, false, maxRuntime);
            });
        }

        private void Test(string rootPath, string actionFolderPath)
        {
            var threads = new List<Task>();
            var trainingFolders = SystemExtensions.GetTrainingFolders(actionFolderPath);
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
                process.StartInfo.FileName = SystemExtensions.GetPythonPath();
                process.StartInfo.Arguments = testerPath + " " + trainPath;
                StartProcess(process, false);
            });
        }

        private static void StartProcess(Process process, bool noConsole, int maxRuntime = int.MaxValue)
        {
            process.StartInfo.CreateNoWindow = noConsole;
            process.Start();
            process.WaitForExit(maxRuntime);
            process.Kill();
        }

        private void SaveResults(string rootPath, string actionFolderPath)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(actionFolderPath);
            foreach (var trainingFolder in trainingFolders)
                TempFileManager.SaveStats(rootPath, trainingFolder);
        }
    }
}
