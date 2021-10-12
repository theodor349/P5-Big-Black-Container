using Shared.Models;
using System;
using System.Collections.Generic;

namespace Test.Utilities
{
    public class Models
    {
        public static PredicateOperator GetPredicateOperator(string predString)
        {
            return new PredicateOperator(predString);
        }

        public static Problem GetProblem(string name, List<PredicateOperator> initalState, List<PredicateOperator> goalState)
        {
            Problem problem = new Problem();

            problem.Name = name;

            return problem;
        }

    }
}
