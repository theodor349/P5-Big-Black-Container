using Shared;
using Shared.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAI.Helpers
{
    internal class SaveDataHelper : DataGenerator
    {
        public SaveDataHelper(Settings settings) : base(settings)
        {
            Run();
        }

        internal override void Run()
        {
            var actions = SystemExtensions.GetAllActions();

            foreach (var action in actions)
            {
                Logger.Log("Saving action: " + new FileInfo(action).Name);
                try
                {
                    Test(action);
                    SaveResults(action);
                }
                catch (Exception)
                {
                    Logger.Log("-- Unable to save: " + new FileInfo(action).Name);
                }
            }
        }
    }
}
