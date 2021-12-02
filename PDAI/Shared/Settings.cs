using NDesk.Options;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Settings
    {
        public static Settings Current;

        public int Cores { get; set; } = 6;
        public bool ShowHelp { get; set; }
        public string DomainFolder { get; set; } 
        public string TargetFolder { get; set; } 
        public int MaxProblems { get; set; } = int.MaxValue;
        public double SplitPercent { get; set; } = 0.2;
        public int NumChunks { get; set; } = 1;
        public List<string> ActionsToRun { get; set; }
        public int MaxRuntime { get; set; } = 1 * 4 * 60 * 1000;
        public int Beta { get; set; } = 2;
        public int Iterations { get; set; } = 1;
        public int Program { get; set; }

        // Dynamic
        public List<string> ActionFolders => Directory.GetDirectories(Path.Combine(TargetFolder, "domainfiles", Domain)).ToList();
        public string Domain => Path.GetFileName(DomainFolder);
        public string ActionToRunPath
        {
            get
            {
                var res = ActionFolders.FirstOrDefault(x => Path.GetFileName(x).ToLower().Equals(ActionsToRun.FirstOrDefault()?.ToLower()));
                if (string.IsNullOrWhiteSpace(res))
                    throw new ArgumentOutOfRangeException("Unable to finde the action: " + ActionsToRun);
                return res;
            }
        }
        public List<string> OutputDomainFolderss => Directory.GetDirectories(Path.Combine(TargetFolder, "domainfiles")).ToList();

        public Settings(string[] args)
        {
            Cores = Environment.ProcessorCount;
            Console.WriteLine("Found Cores: " + Cores);
            if (Cores > 6)
                Cores /= 2;
            Console.WriteLine("Final Cores: " + Cores);

            var p = new OptionSet()
            {
                { "h|help", "Prints help to the console", v => ShowHelp = true },
                { "d|domain-folder=", "Path to the input domain folder", v => DomainFolder = v },
                { "t|target-folder=", "Path to the output folder", v => TargetFolder = v },
                { "p|max-problems=", "Max number of ms each iteration can run for: " + MaxProblems, v => MaxProblems = int.Parse(v) },
                { "s|split-percent=", "Percentage of problems used for testing: " + SplitPercent, v => SplitPercent = double.Parse(v) },
                { "c|chunks=", "Number of chunks to split the training data into: " + NumChunks, v => NumChunks = int.Parse(v) },
                { "a|actions=", "Name of the actions to run (, seperated)", v => ActionsToRun = v.Split(",").ToList().ConvertAll(x => x.ToLower()) },
                { "R|max-runtime=", "Max ms each iteration can run: " + MaxRuntime, v => MaxRuntime = int.Parse(v) },
                { "b|beta=", "Value used in the F-Score, id set to 0 then it will become dynamic: " + Beta, v => Beta = int.Parse(v) },
                { "i|iterations=", "Number of iterations: " + Iterations, v => Iterations = int.Parse(v) },
                { "P|program=", "The program to run: " + Program, v => Program = int.Parse(v) },
                { "C|cores=", "The number of physical cores the program may use: " + Cores, v => Cores = int.Parse(v) },
            };
            try
            {
                var extra = p.Parse(args);
                if (ShowHelp)
                    p.WriteOptionDescriptions(Console.Out);
                if (!Directory.Exists(DomainFolder))
                    throw new ArgumentException("Unable to find domain folder");
            }
            catch (OptionException e)
            {
                Console.Write("PDAI: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `--help' for more information.");
                return;
            }
        }
    }
}
