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
        private readonly DataGenerationHelper _dataGenHelper;
        BalancingExamplesGenerator _balancingExamplesGenerator = new BalancingExamplesGenerator();
        private bool isFirstRun => iteration == 0;
        private int iteration = 0;

        public AllActionsDataGenerator(Settings _settings)
        {
            this._settings = _settings;
            _dataGenHelper = new DataGenerationHelper(this._settings);
        }

        public void runSettings()
        {
            List<string> actions = GetAllActions();

            Logger.Log("%%%%% Iteration: " + iteration);

            foreach (var action in actions)
                SetInput(action);
            SystemExtensions.RunnInParallel(actions, x => Train(x), _settings.Cores);
            foreach (var action in actions)
            {
                Test(action);
                SaveResults(action);
            }
            iteration++;
        }

        private void SetInput(string action)
        {
            ConstraintHelper ch = new ConstraintHelper();
            List<string> trainingFolders = SystemExtensions.GetTrainingFolders(action);

            foreach (var trainingFolder in trainingFolders)
            {
                //_balancingExamplesGenerator.GenerateBalanceExampleFile(trainingFolder);

                if (isFirstRun)
                    ch.IncrementConstraintValues(Path.Combine(trainingFolder, "bias.pl"), 0, 0, 2);
            }
        }

        private void Train(string action)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            SystemExtensions.RunnInParallel(trainingFolders, x => RunPopper(x, _settings.TargetFolder, _settings.Beta, _settings.MaxRuntime), _settings.Cores);
        }

        private void Test(string action)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            SystemExtensions.RunnInParallel(trainingFolders, x => _dataGenHelper.RunTest(x), _settings.Cores);
        }

        private void SaveResults(string action)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            foreach (var trainingFolder in trainingFolders)
                TempFileManager.SaveStats(_settings.TargetFolder, trainingFolder);
        }

        private void RunPopper(string trainPath, string rootPath, int beta, int maxRuntime)
        {
            if (beta == 0)
                beta = GetDynamicBeta(trainPath);

            _dataGenHelper.RunPopper(trainPath, rootPath, beta, maxRuntime);
        }

        private int GetDynamicBeta(string trainPath)
        {
            var beta = GetWeightedBeta(trainPath);
            return beta;
        }

        private int GetIterationBeta()
        {
            return (int) (2 * Math.Pow(iteration + 1, 3.4276));
        }

        private static int GetWeightedBeta(string trainPath)
        {
            var lines = File.ReadAllLines(Path.Combine(trainPath, "exs.pl"));
            int pos = 0;
            int neg = 0;
            foreach (var line in lines)
            {
                if (line.StartsWith("pos"))
                    pos++;
                else
                    neg++;
            }
            if (pos == 0)
                return 1;
            return neg / pos;
        }

        private List<string> GetDomains()
        {
            return Directory.GetDirectories(Path.Combine(_settings.TargetFolder, "domainfiles")).ToList();
        }

        private List<string> GetActions(string domain)
        {
            string[] actions = Directory.GetDirectories(domain);
            if (_settings.ActionsToRun is null)
                return actions.ToList();
            else
                return actions.Where(x => _settings.ActionsToRun.Contains(new FileInfo(x).Name.ToLower())).ToList();
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
