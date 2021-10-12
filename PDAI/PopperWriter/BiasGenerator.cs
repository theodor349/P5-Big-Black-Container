using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopperWriter
{
    internal class BiasGenerator
    {
        public void Write(Shared.Models.Action action, List<Predicate> predicates)
        {
        }

        public List<string> GetPredicateDeclarations()
        {
            return null;
        }

        public string GetPredicateDecleration(Predicate predicate, bool isHeadPred, bool isGoal)
        {
            string predDecl = (isHeadPred ? "head_pred" : "body_pred") + "(";
            if (!isHeadPred)
            {
                predDecl += isGoal ? "goal_" : "init_";
            }
            predDecl += predicate.Name + "," + (predicate.Parameters.Count + 1) + ").";

            return predDecl;
        }

        public List<Predicate> GetUsedPredicates(List<Predicate> possiblePredicates, List<PredicateOperator> predicates)
        {
            List<Predicate> usedPredicates = new List<Predicate>();

            foreach (Predicate pred in possiblePredicates)
            {
                if (predicates.Any(p => p.Name == pred.Name))
                {
                    usedPredicates.Add(pred);
                }
            }

            return usedPredicates;
        }
    }
}
