using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PddlParser.Internal
{
    internal static class ActionsParser
    {
        private static Regex actionReg = new Regex(@"(?i)\(:action(?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)");

        public static List<Shared.Models.Action> Parse(List<string> lines, List<Entity> entities)
        {
            var res = new List<Shared.Models.Action>();
            var actionTexts = RegHelper.GetProblemSection(lines, actionReg);
            foreach (var actionText in actionTexts)
            {
                res.Add(new Shared.Models.Action(actionText, entities));
            }

            return res;
        }
    }
}
