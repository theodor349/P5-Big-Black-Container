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
            
        }
    }
}
