using PDAI.Helpers;
using PddlParser.Internal;
using Shared;
using Shared.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAI
{
    // Johnny Bravo
    public class InifiniteDataGenerator
    {
        public static Settings _setting => Settings.Current;

        public void GenerateData()
        {
            Console.WriteLine("");
            Logger.Log("Generating Data");
            Console.WriteLine("Working in folder: " + _setting.ActionToRunPath);
            Logger.Log("Generating data for action: " + Path.GetFileName(_setting.ActionToRunPath));

            GenerateForActionRunForEver(_setting.TargetFolder, _setting.ActionToRunPath, _setting.Beta, _setting.MaxRuntime);
            Console.WriteLine("");
        }

        private void GenerateForActionRunForEver(string rootPath, string actionPath, int beta, int maxRuntime)
        {
            new BiasRunnerConstant(vars: 8, body: 15, clause: 8).Run((x) =>
            {
                Logger.Log("Var: " + x.Var + " body: " + x.Body + " clause: " + x.Clause);
                SetInput(actionPath, x);
                Train(actionPath, rootPath, beta, maxRuntime);
                Test(rootPath, actionPath);
                SaveResults(rootPath, actionPath);
            });
        }

        private void SetInput(string actionPath, BiasSetup bias)
        {
            ConstraintHelper ch = new ConstraintHelper();
            List<string> trainingFolders = SystemExtensions.GetTrainingFolders(actionPath);

            foreach (var trainingFolder in trainingFolders)
            {
                ch.ChangeConstraint(Path.Combine(trainingFolder, "bias.pl"), bias.Clause, bias.Body, bias.Var);
            }
        }

        private void Train(string actionFolderPath, string rootPath, int beta, int maxRuntime)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(actionFolderPath);
            var threads = new List<Task>();
            foreach (string trainingFolder in trainingFolders)
                threads.Add(RunPopper(trainingFolder, rootPath, beta, maxRuntime));
            Task.WaitAll(threads.ToArray());
        }

        private async Task RunPopper(string trainPath, string rootPath, int beta, int maxRuntime)
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
