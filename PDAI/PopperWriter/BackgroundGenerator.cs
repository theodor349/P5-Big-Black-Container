using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopperWriter
{
    internal class BackgroundGenerator
    {
        public void Write(Domain domain, string path)
        {
            List<string> predicates = GetPredicates(domain.Problems);

            File.WriteAllLinesAsync(path, predicates);
        }

        public List<string> GetPredicates(List<Problem> problems)
        {
            List<string> predicates = new List<string>();

            foreach (Problem problem in problems)
            {
                foreach (PredicateOperator initPred in problem.InitalState)
                {
                    predicates.Add(PredicateToString(initPred, problem.Name, false));
                }
                foreach (PredicateOperator goalPred in problem.GoalState)
                {
                    predicates.Add(PredicateToString(goalPred, problem.Name, true));
                }
            }

            return predicates;
        }

        public string PredicateToString(PredicateOperator predicate, string problemName, bool isGoal)
        {
            string predString = isGoal ? "goal_" : "init_";
            predString += predicate.Name + "(";   

            foreach (string attr in predicate.Attributes)
            {
                predString += attr + ",";
            }

            predString += problemName + ").";

            return predString;
        }
    }
}
