using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopperWriter
{
    class BackgroundGenerator
    {
        public void Write(Domain domain)
        {
            
        }

        public List<string> GetPredicates(List<Problem> problems)
        {
            return null;
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
