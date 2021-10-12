using PddlParser.Internal;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Pddl.Internal
{
    internal class DomainParser
    {
        internal void Parser(string filePath, Domain domain)
        {
            domain.Entities = GetTypes(filePath);
            domain.Actions = GetActions(filePath, domain.Entities);
            domain.Predicates = GetPredicates(filePath, domain.Entities);
        }

        private List<Predicate> GetPredicates(string filePath, List<Entity> entities)
        {
            List<string> lines = File.ReadAllLines(filePath).ToList();
            return PredicatesParser.Parse(lines, entities);
        }

        private List<Shared.Models.Action> GetActions(string filePath, List<Entity> entities)
        {
            List<string> lines = File.ReadAllLines(filePath).ToList();
            return ActionsParser.Parse(lines, entities);
        }

        private List<Entity> GetTypes(string filePath)
        {
            List<string> lines = File.ReadAllLines(filePath).ToList();
            return TypesParser.Parse(lines);
        }
    }
}
