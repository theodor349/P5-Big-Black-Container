using PopperWriter;
using Shared.ExtensionMethods;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Writer.Popper
{
    public class Popper
    {
        public void Write(Domain domain, string folderPath)
        {
            Logger.Log("Printing to Popper");

            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
            Directory.CreateDirectory(folderPath);

            var threads = new List<Task>();
            foreach (var a in domain.Actions)
            {
                threads.Add(PrintAction(folderPath, a, domain));
            }
            Task.WaitAll(threads.ToArray());
        }

        private Task PrintAction(string folderPath, Shared.Models.Action a, Domain domain)
        {
            var bgGenerator = new BackgroundGenerator();
            var biasGenerator = new BiasGenerator();
            var exampleGenerator = new ExampleGenerator();

            return Task.Run(() =>
            {
                string path = folderPath + "/" + a.Name;
                Directory.CreateDirectory(path);
                bgGenerator.Write(domain, folderPath + "/" + a.Name + "/bk.pl");
                biasGenerator.Write(a, domain, path + "/bias.pl");
                exampleGenerator.Write(a, domain.Problems, path + "/exs.pl");
                Logger.Log("Done with action: " + a.Name);
            });
        }
    }
}
