using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PddlParser.Internal
{
    internal static class TypesParser
    {
        public static List<Entity> Parse(List<string> lines)
        {
            Regex reg = new Regex(@"\(:types.*\)");
            var text = lines.Aggregate((s, y) => y = s + " " + y.Trim());
            text = reg.Match(text).Value.Replace("(:types", "").Replace(")", "").Trim();

            return null;
        }

        private static int GetEndIndex(string text, int start)
        {
            for (int i = start; i < text.Length; i++)
            {
                var c = text[i];
                if (c.Equals(')'))
                    return i;
            }
            throw new KeyNotFoundException("Was unable to find the end of the " + start + " section");
        }

        private static int GetStartIndex(string text, string word)
        {
            return text.IndexOf(word);
        }
    }
}
