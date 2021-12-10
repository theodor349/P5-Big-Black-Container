using System.Collections.Generic;
using System.IO;
using PddlParser.Internal;
using Shared;
using Shared.ExtensionMethods;

namespace PDAI.Helpers
{
    internal abstract class DataGenerator
    {
        internal readonly Settings _settings;
        private readonly DataGenerationHelper _dataGenHelper;

        internal bool runActionsInParallel = true;
        internal bool runSplitsInParallel = true;
        internal bool randomSplits = false;

        internal bool isFirstIteration => iteration == 0;
        internal int iteration = 0;

        public DataGenerator(Settings settings)
        {
            _settings = settings;
            _dataGenHelper = new DataGenerationHelper(_settings);
        }

        internal abstract void Run();

        internal virtual void GenerateDomainfilesFolder(bool randomSplit = false)
        {
            Program.GenerateDomainfilesFolder(randomSplit);
        }

        internal virtual void SetInput(string trainingFolder)
        {
            ConstraintHelper ch = new ConstraintHelper();

            ch.IncrementConstraintValues(Path.Combine(trainingFolder, "bias.pl"), 0, 0, 1);
        }

        internal virtual void Train(string action)
        {
            Logger.Log("Running Action: " + new FileInfo(action).Name);
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            SystemExtensions.RunnInParallel(trainingFolders, x => SetInput(x), _settings.Cores, false);
            SystemExtensions.RunnInParallel(trainingFolders, x => RunPopper(x), _settings.Cores, runSplitsInParallel);
        }

        internal virtual void Test(string action)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            SystemExtensions.RunnInParallel(trainingFolders, x => _dataGenHelper.RunTest(x), _settings.Cores);
        }

        internal virtual void SaveResults(string action)
        {
            var trainingFolders = SystemExtensions.GetTrainingFolders(action);
            foreach (var trainingFolder in trainingFolders)
                TempFileManager.SaveStats(_settings.TargetFolder, trainingFolder);
        }

        private void RunPopper(string trainPath)
        {
            _dataGenHelper.RunPopper(trainPath, _settings.TargetFolder, _settings.Beta, _settings.MaxRuntime);
        }
    }
}
