using System.Collections.Generic;
using Shared;
using Shared.ExtensionMethods;

namespace PDAI.Helpers
{
    internal class BaselineDataGenerator : DataGenerator
    {
        public BaselineDataGenerator(Settings settings) : base(settings)
        {
            settings.NumChunks = 1;
            settings.SplitPercent = 0.5f;
            runActionsInParallel = true;
            runSplitsInParallel = false;

            GenerateDomainfilesFolder(randomSplits);
            Run();
        }

        internal override void Run()
        {
            var actions = SystemExtensions.GetAllActions();

            SystemExtensions.RunnInParallel(actions, x => Train(x), _settings.Cores, runActionsInParallel);
            foreach (var action in actions)
            {
                Test(action);
                SaveResults(action);
            }
        }
    }
}
