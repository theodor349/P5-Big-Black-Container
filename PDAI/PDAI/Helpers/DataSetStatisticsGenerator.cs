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
    public class DataSetStatisticsGenerator
    {
        private readonly Settings _settings;
        private string outputFile => Path.Combine(_settings.TargetFolder, "domainfiles", "stats.csv");

        public DataSetStatisticsGenerator(Shared.Settings settings)
        {
            _settings = settings;
            File.WriteAllLines(outputFile, new string[] { "Domain" + "\t" + "Action" + "\t" + "Problems" + "\t" + "Positives" + "\t" + "Negative" });
            GenerateStats();
        }

        private void GenerateStats()
        {
            var domains = _settings.OutputDomainFolderss;
            foreach (var domain in domains)
            {
                DoDomain(domain);
            }
        }

        private void DoDomain(string domain)
        {
            string domainName = new FileInfo(domain).Name;
            var actions = Directory.GetDirectories(domain);
            foreach (var action in actions)
            {
                DoAction(domainName, action);
            }
        }

        private void DoAction(string domainName, string action)
        {
            string actionName = new FileInfo(action).Name;
            var examples = File.ReadAllLines(Path.Combine(action, "100", "exs.pl"));
            int pos = 0;
            int neg = 0;
            foreach (var line in examples)
            {
                if (line.StartsWith("neg"))
                    pos++;
                else if (line.StartsWith("pos"))
                    neg++;
            }
            File.AppendAllLines(outputFile, new string[]
            {
                domainName + "\t" + actionName + "\t" + "\t" + pos + "\t" + neg
            });
        }
    }
}
