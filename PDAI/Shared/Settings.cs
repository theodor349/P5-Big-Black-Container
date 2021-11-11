using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Settings
    {
        public static Settings Current;

        public bool ShowHelp { get; set; }
        public string DomainFolder { get; set; }    // d    --domain-folder
        public string TargetFolder { get; set; }    // t
        public int MaxProblems { get; set; }        // p
        public double SplitPercent { get; set; }    // s
        public int NumChunks { get; set; }          // c
        public bool RunInfinite { get; set; }       // r
        public string ActionToRun { get; set; }     // a
        public int MaxRuntime { get; set; }         // R
        public int Beta { get; set; }               // b
        public int Iterations { get; set; }         // i

        public Settings(string[] args)
        {
            var p = new OptionSet()
            {
                { "h|help", "Prints help to the console", v => ShowHelp = true },
                { "d|domain-folder=", "Path to the input domain folder", v => DomainFolder = v },
                { "t|target-folder=", "Path to the output folder", v => TargetFolder = v },
                { "p|max-problems=", "Max number of ms each iteration can run for", v => MaxProblems = int.Parse(v) },
                { "s|split-percent=", "Percentage of problems used for testing", v => SplitPercent = double.Parse(v) },
                { "c|chunks=", "Number of chunks to split the training data into", v => NumChunks = int.Parse(v) },
                { "r|run-infinite", "Should it run the 'Infinite' data generator", v => RunInfinite = true },
                { "a|action", "Name of the action to run", v => ActionToRun = v },
                { "R|max-runtime=", "Max ms each iteration can run", v => MaxRuntime = int.Parse(v) },
                { "b|beta=", "Value used in the F-Score", v => Beta = int.Parse(v) },
                { "i|iterations=", "Number of iterations", v => Iterations = int.Parse(v) },
            };
            try
            {
                var extra = p.Parse(args);
                if (ShowHelp)
                    p.WriteOptionDescriptions(Console.Out);
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
