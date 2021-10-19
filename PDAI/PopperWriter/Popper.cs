using PopperWriter;
using Shared.Models;
using System;
using System.IO;

namespace Writer.Popper
{
    public class Popper
    {
        public void Write(Domain domain, string folderPath)
        {
            var bgGenerator = new BackgroundGenerator();
            var biasGenerator = new BiasGenerator();
            var exampleGenerator = new ExampleGenerator();

            Directory.Delete(folderPath, true);
            Directory.CreateDirectory(folderPath);

            foreach (var a in domain.Actions)
            {
                string path = folderPath + "/" + a.Name;
                Directory.CreateDirectory(path);
                bgGenerator.Write(domain, folderPath + "/" + a.Name + "/bk.pl");
                biasGenerator.Write(a, domain, path + "/bias.pl");
                exampleGenerator.Write(a, domain.Problems, path + "/exs.pl");
            }
        }
    }
}
