using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PddlParser.Internal;
using Shared;
using Shared.ExtensionMethods;

namespace PDAI.Helpers
{
    public class AllActionsDataGenerator
    {
        private readonly Settings _settings;
        private bool isFirstRun = true;

        public AllActionsDataGenerator(Settings _settings)
        {
            this._settings = _settings;
        }

        public void runSettings()
        {
            List<string> Actions = GetAllActions();
            while (true)
            {
                foreach(var action in Actions)
                {
                    RunAction(action);
                }
                isFirstRun = false;
            }

        }

        private void RunAction(string action)
        {
            RunSplit(action);
     
        }

        private void RunSplit(string action)
        {
            SetInput(action);
            Train(action);
            Test(action);
            SaveResults(action);
        }

        private void SetInput(string action)
        {
            ConstraintHelper ch = new ConstraintHelper();
            List<string> trainingFolders = SystemExtensions.GetTrainingFolders(action);

            foreach (var trainingFolder in trainingFolders)
            {
                if (!isFirstRun)
                    ch.IncrementConstraintValues(Path.Combine(trainingFolder, "bias.pl"), 1, 1, 1);
            }
        }

        private void Train(string action)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            var threads = new List<Task>();
            foreach (string trainingFolder in trainingFolders)
                threads.Add(RunPopper(trainingFolder, _settings.TargetFolder, _settings.Beta, _settings.MaxRuntime));
            Task.WaitAll(threads.ToArray());
        }

        private void Test(string action)
        {
            var threads = new List<Task>();
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            foreach (var trainingFolder in trainingFolders)
                threads.Add(RunTest(trainingFolder));
            Task.WaitAll(threads.ToArray());
        }

        private async Task RunTest(string trainPath)
        {
            await Task.Run(() =>
            {
                string testerPath = Path.Combine(_settings.TargetFolder, "tester.py");
                Process process = new();
                process.StartInfo.FileName = SystemExtensions.GetPythonPath();
                process.StartInfo.Arguments = testerPath + " " + trainPath;
                StartProcess(process, false);
            });
        }

        private void SaveResults(string action)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            foreach (var trainingFolder in trainingFolders)
                TempFileManager.SaveStats(_settings.TargetFolder, trainingFolder);
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

        private static void StartProcess(Process process, bool noConsole, int maxRuntime = int.MaxValue)
        {
            process.StartInfo.CreateNoWindow = noConsole;
            process.Start();
            process.WaitForExit(maxRuntime);
            process.Kill();
        }

        private List<string> GetDomains()
        {
            return Directory.GetDirectories(Path.Combine(_settings.TargetFolder, "domainfiles")).ToList();
        }

        private List<string> GetActions(string domain)
        {
            return Directory.GetDirectories(domain).ToList();
        }

        private List<string> GetAllActions()
        {
            List<string> res = new List<string>();
            var domains = GetDomains();

            foreach (var domain in domains)
            {
                res.AddRange(GetActions(domain));
            }

            return res;
        }
    }
}
