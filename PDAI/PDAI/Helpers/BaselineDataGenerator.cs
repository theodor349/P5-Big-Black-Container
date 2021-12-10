using System;
using System.Collections.Generic;
using System.IO;
using PddlParser.Internal;
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

            Console.WriteLine(settings.UseAllowSingletons);
            Console.WriteLine(settings.UseNonDatalog);

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

        internal override void SetInput(string trainingFolder)
        {
            ConstraintHelper ch = new ConstraintHelper();

            ch.IncrementConstraintValues(Path.Combine(trainingFolder, "bias.pl"), 0, 0, 3);
            //ch.AddNonDatalog(Path.Combine(trainingFolder, "bias.pl"));
            //ch.AddAllowSingletons(Path.Combine(trainingFolder, "bias.pl"));
        }
    }
}
