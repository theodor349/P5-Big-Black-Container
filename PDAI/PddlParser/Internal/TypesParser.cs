using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PddlParser.Internal
{
    internal static class TypesParser
    {
        public static List<Entity> Parse(List<string> lines)
        {
            var res = new List<Entity>();
            var searchWord = "(:types";
            int startRow = GetStartLine(lines, searchWord);
            int endRow = GetEndLine(lines, startRow);

            for (int i = startRow; i <= endRow; i++)
            {
                ParseLine(lines[i], res);
            }

            return res;
        }

        private static void ParseLine(string line, List<Entity> entities)
        {
            line = line
                .Replace("(:types", "")
                .Replace(")", "")
                .Trim();
            var words = line.Split(" ");

            bool isParent = false;
            var newEntities = new List<Entity>();
            foreach (var word in words)
            {
                var w = word.Trim();
                if (string.IsNullOrWhiteSpace(w))
                    continue;

                if (isParent)
                    entities.Where(x => x.Type.Equals(w)).FirstOrDefault().Children.AddRange(newEntities);
                else
                {
                    if (w.Equals("-"))
                        isParent = true;
                    else
                        newEntities.Add(new Entity()
                        {
                            Type = w
                        });
                }
            }

            entities.AddRange(newEntities);
        }

        private static int GetEndLine(List<string> lines, int start)
        {
            for (int i = start; i < lines.Count; i++)
            {
                if (lines[i].Contains(")"))
                    return i;
            }

            throw new KeyNotFoundException("Was unable to find the end of the types section");
        }

        private static int GetStartLine(List<string> lines, string word)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].ToLower().Contains(word.ToLower()))
                    return i;
            }
            throw new KeyNotFoundException("Was unable to find the start of the types section");
        }
    }
}
