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

        public static List<PredicateOperator> GetPredicateOperatorList(List<string> predStrings)
        {
            List<PredicateOperator> predOperators = new List<PredicateOperator>();

            foreach (string predString in predStrings)
            {
                predOperators.Add(GetPredicateOperator(predString));
            }

            return predOperators;
        }

        public static Problem GetProblem(string name, List<PredicateOperator> initalState, List<PredicateOperator> goalState)
        {
            Problem problem = new Problem();

            problem.Name = name;
            problem.InitalState = initalState;
            problem.GoalState = goalState;

            return problem;
        }

        public static List<Problem> GetProblemList(List<string> names, List<List<PredicateOperator>> initialstates, List<List<PredicateOperator>> goalStates)
        {
            List<Problem> problems = new List<Problem>();

            for (int i = 0; i < names.Count; i++)
            {
                Problem problem = new Problem();

                problem.Name = names[i];
                problem.InitalState = initialstates[i];
                problem.GoalState = goalStates[i];

                problems.Add(problem);
            }

            return problems;
        }

    }
}
