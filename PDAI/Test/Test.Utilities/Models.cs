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

        public static ActionOperator GetActionOperator(string actionString)
        {
            return new ActionOperator(actionString);
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

        public static List<ActionOperator> GetActionOperatorList(List<string> actionStrings)
        {
            List<ActionOperator> actionOperators = new List<ActionOperator>();

            foreach (string actionString in actionStrings)
            {
                actionOperators.Add(GetActionOperator(actionString));
            }

            return actionOperators;
        }

        public static Problem GetProblem(string name, List<PredicateOperator> initalState, List<PredicateOperator> goalState)
        {
            Problem problem = new Problem();

            problem.Name = name;
            problem.InitalState = initalState;
            problem.GoalState = goalState;

            return problem;
        }

        public static Problem GetProblem(string name, List<ActionOperator> goodOperator, List<ActionOperator> badOperator)
        {
            Problem problem = new Problem();

            problem.Name = name;
            problem.GoodOperators = goodOperator;
            problem.BadOperators = badOperator;

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

        public static List<Problem> GetProblemList(List<string> names, List<List<ActionOperator>> goodoperators, List<List<ActionOperator>> badoperators)
        {
            List<Problem> problems = new List<Problem>();

            for (int i = 0; i < names.Count; i++)
            {
                Problem problem = new Problem();

                problem.Name = names[i];
                problem.GoodOperators = goodoperators[i];
                problem.BadOperators = badoperators[i];

                problems.Add(problem);
            }

            return problems;
        }

        public static List<Problem> GetProblemList(List<List<PredicateOperator>> initialstates, List<List<PredicateOperator>> goalStates)
        {
            List<Problem> problems = new List<Problem>();

            for (int i = 0; i < initialstates.Count; i++)
            {
                Problem problem = new Problem();

                problem.InitalState = initialstates[i];
                problem.GoalState = goalStates[i];

                problems.Add(problem);
            }

            return problems;
        }

        public static List<Predicate> GetPredicateList(List<string> names)
        {
            List<Predicate> predicates = new List<Predicate>();

            foreach (string name in names)
            {
                predicates.Add(GetPredicate(name));
            }

            return predicates;
        }

        public static List<Predicate> GetPredicateListFromNumOfParams(List<string> names, List<int> numOfParamsList)
        {
            List<Predicate> predicates = new List<Predicate>();

            for (int i = 0; i < names.Count; i++)
            {
                Predicate pred = GetPredicate(names[i]);
                pred.Parameters = new();

                for (int j = 0; j < numOfParamsList[i]; j++)
                {
                    pred.Parameters.Add(new Parameter());
                }

                predicates.Add(pred);
            }

            return predicates;
        }

        public static Predicate GetPredicate(string name)
        {
            Predicate predicate = new Predicate();
            predicate.Name = name;
            return predicate;
        }

        public static Parameter GetParameter(string type)
        {
            Parameter parameter = new Parameter();
            parameter.Entity = new Entity();
            parameter.Entity.Type = type;
            return parameter;
        }

        public static List<Parameter> GetParameterList(List<string> types)
        {
            List<Parameter> parameters = new List<Parameter>();

            foreach (string type in types)
            {
                parameters.Add(GetParameter(type));
            }

            return parameters;
        }

        public static Shared.Models.Action GetAction(string name, List<string> paramTypes)
        {
            Shared.Models.Action action = new Shared.Models.Action();
            action.Name = name;
            action.Parameters = GetParameterList(paramTypes);
            return action;
        }

        public static List<Predicate> GetPredicateList(List<string> names, List<List<string>> paramTypes)
        {
            List<Predicate> predicates = new List<Predicate>();
            
            for (int i = 0; i < names.Count; i++)
            {
                Predicate predicate = new Predicate();
                predicate.Name = names[i];
                predicate.Parameters = GetParameterList(paramTypes[i]);
                predicates.Add(predicate);
            }

            return predicates;
        }

        public static List<Predicate> GetSatellitePredicates()
        {
            List<string> names = new List<string>() { "on_board", "supports", "pointing", "power_avail", "power_on", "calibrated", "have_image", "calibration_target" };
            List<List<string>> paramTypes = new List<List<string>>()
            {
                new List<string>() { "instrument", "satellite" },
                new List<string>() { "instrument", "mode" },
                new List<string>() { "satellite", "direction" },
                new List<string>() { "satellite" },
                new List<string>() { "instrument" },
                new List<string>() { "instrument" },
                new List<string>() { "direction", "mode" },
                new List<string>() { "instrument", "direction" },
            };

            return GetPredicateList(names, paramTypes);
        }

        public static Domain GetDomain(List<Problem> problems, List<Predicate> predicates)
        {
            Domain domain = new Domain();
            domain.Problems = problems;
            domain.Predicates = predicates;
            return domain;
        }

        public static Entity GetEntity(string type)
        {
            Entity entity = new();
            entity.Type = type;
            entity.Children = new List<Entity>();
            return entity;
        }

        public static Entity GetEntity(string type, List<Entity> children)
        {
            Entity entity = new();
            entity.Type = type;
            entity.Children = children;
            return entity;
        }

        public static List<Entity> GetEntityList(List<string> types, List<List<Entity>> childrenList)
        {
            List<Entity> entities = new();

            for (int i = 0; i < types.Count; i++)
            {
                entities.Add(GetEntity(types[i], childrenList[i]));
            }

            return entities;
        }

        public static List<Entity> GetEntityList(List<string> types)
        {
            List<Entity> entities = new();

            foreach (string type in types)
            {
                entities.Add(GetEntity(type));
            }

            return entities;
        }

        public static Clause GetClause(string name, int numOfParams)
        {
            Clause clause = new();
            clause.Name = name;
            clause.Parameters = new();
            for (int i = 0; i < numOfParams; i++)
            {
                clause.Parameters.Add(new());
            }
            return clause;
        }

        public static List<Clause> GetClauseList(List<string> names, List<int> numOfParamsList)
        {
            List<Clause> clauses = new();

            for (int i = 0; i < names.Count; i++)
            {
                clauses.Add(GetClause(names[i], numOfParamsList[i]));
            }

            return clauses;
        }

    }
}
