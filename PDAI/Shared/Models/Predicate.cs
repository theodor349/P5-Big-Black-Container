using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shared.Models
{
    public class Predicate : Clause
    {
        private Regex multipleTypeReg = new Regex(@"(\?\w*.)*-.\w*");
        public Predicate(string text, List<Entity> entities)
        {
            text = text.Trim('(', ')');
            Name = text.Split(" ").First();
            text = text.Substring(Name.Length).Trim();
            if (entities.Count > 1)
            {
                HandleMultipleTypes(text, entities);
            }
            else
            {
                HandleSingleType(text, entities);
            }
        }

        private void HandleSingleType(string text, List<Entity> entities)
        {
            Parameters = GetParameters(text, entities.FirstOrDefault());
        }

        private List<Parameter> GetParameters(string text, Entity entity)
        {
            var res = new List<Parameter>();
            var words = text.Split("?");
            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word))
                    continue;

                res.Add(new Parameter()
                {
                    Name = word.Trim(),
                    Entity = entity,
                });
            }
            return res;
        }

        private void HandleMultipleTypes(string text, List<Entity> entities)
        {
            foreach (Match match in multipleTypeReg.Matches(text))
            {
                var split = match.Value.Split("-");
                var t = entities.Where(x => x.Type.Equals(split[1].Trim())).FirstOrDefault();

                Parameters.AddRange(GetParameters(split[0], t));
            }
        }
    }
}
