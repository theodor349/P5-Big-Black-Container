using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.ExtensionMethods;

namespace PDAI.Helpers
{

    internal class TrainingDataSplitDataGenerator : DataGenerator
    {
        public TrainingDataSplitDataGenerator(Settings settings) : base(settings)
        {
            runActionsInParallel = false;
            runSplitsInParallel = true;

            Run();
        }

        internal override void Run()
        {
            Program.GenerateDomainfilesFolder(true);
            var actions = SystemExtensions.GetAllActions();
            while (true)
            {
                if(!isFirstIteration)
                    Program.GenerateDomainfilesFolder(true);
                RunIteration(actions);
                iteration++;
            }
        }

        private void RunIteration(List<string> actions)
        {
            SystemExtensions.RunnInParallel(actions, x => Train(x), _settings.Cores, runActionsInParallel);
            foreach (var action in actions)
            {
                Test(action);
                SaveResults(action);
            }
        }
    }
}
