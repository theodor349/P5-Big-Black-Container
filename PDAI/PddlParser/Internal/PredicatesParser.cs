using Shared.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace PddlParser.Internal
{
    internal static class PredicatesParser
    {
        private static Regex predicateReg = new Regex(@"(?i)\(:predicates(?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)");
        private static Regex opReg = new Regex(@"\([\s\S]*?\)");
        public static List<Predicate> Parse(List<string> lines, List<Entity> entities)
        {
            var res = new List<Predicate>();
            var predicateText = RegHelper.GetProblemSection(lines, predicateReg);
            var predicateLines = RegHelper.GetProblemSection(predicateText, opReg);

            foreach (var predicateLine in predicateLines) 
            {
                res.Add(new Predicate(predicateLine, entities));
            }
            return res;
        }
    }
}
