using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PddlParser.Internal
{
    internal static class RegHelper
    {
        public static List<string> GetProblemSection(List<string> lines, Regex regex)
        {
            var line = string.Join(' ', lines).Replace("\t", "");
            var res = regex.Matches(line).ToList().ConvertAll(x => TrimStateLine(x.Value));

            return res;
        }

        private static string TrimStateLine(string line)
        {
            List<string> words = line.Split().ToList();
            words.RemoveAt(0);
            words.RemoveAt(words.Count - 1);
            return string.Join(' ', words);
        }
    }
}
