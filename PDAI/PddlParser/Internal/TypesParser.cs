using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PddlParser.Internal
{
    internal static class TypesParser
    {
        public static List<Entity> Parse(List<string> lines)
        {
            var text = lines.Aggregate((x, t) => t += " " + x);
            int start = GetStartIndex(text, ":type");
            int end = GetEndIndex(text, start);

            return null;
        }

        private static int GetEndIndex(string text, int start)
        {
            throw new NotImplementedException();
        }

        private static int GetStartIndex(string text, string word)
        {
            throw new NotImplementedException();
        }
    }
}
