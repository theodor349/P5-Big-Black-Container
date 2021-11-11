using Shared.ExtensionMethods;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Writer.Popper;

namespace PopperWriter
{
    internal class BiasGenerator
    {
        public void Write(Shared.Models.Action action, List<Problem> problems, List<Predicate> predicates, string path)
        {
            List<string> lines = new();

            List<Predicate> usedInitPreds = CollectAllUsedPredicates(problems, predicates, true);
            List<Predicate> usedGoalPreds = CollectAllUsedPredicates(problems, predicates, false);

            int maxInitalValue = usedInitPreds.Select(x => x.Parameters.Count).Max();
            int maxGoalValue = usedInitPreds.Select(x => x.Parameters.Count).Max();
            int maxVars = Math.Max(maxInitalValue, maxGoalValue);
            maxVars = Math.Max(maxVars, action.Parameters.Count);

            List<Clause> allClauses = new();
            allClauses.Add(action);
            allClauses.AddRange(usedInitPreds);
            allClauses.AddRange(usedGoalPreds);

            List<string> allClausesPre = new();
            allClausesPre.Add("");
            foreach (var p in usedInitPreds)
            {
                allClausesPre.Add("init_");
            }
            foreach (var p in usedGoalPreds)
            {
                allClausesPre.Add("goal_");
            }

            lines.AddRange(GetConstraints(maxVars));
            lines.AddRange(GetClauseDeclarations(action, usedInitPreds, usedGoalPreds));
            lines.AddRange(GetTypeDeclerations(allClauses, allClausesPre).Distinct().ToList());
            lines.AddRange(GetDirectionDeclerations(allClauses, allClausesPre));

            var t = File.WriteAllLinesAsync(path, lines);
            t.Wait();
        }

        public List<Predicate> CollectAllUsedPredicates(List<Problem> problems, List<Predicate> predicates, bool isInitialState)
        {
            List<PredicateOperator> allPredicates = new();

            foreach (Problem problem in problems)
            {
                if (isInitialState)
                {
                    problem.InitalState.ForEach(x => allPredicates.Add(x));
                }
                else
                {
                    problem.GoalState.ForEach(x => allPredicates.Add(x));
                }
            }

            return GetUsedPredicates(predicates, allPredicates);
        }

        public List<string> GetConstraints(int maxVars)
        {
            Popper.MinVars = (maxVars + 2);
            return new List<string>() { "max_clauses(5).", "max_body(5).", "max_vars(" + Popper.MinVars + ")." }; // maxVars is added by two. +1 for problem and +1 for make the vars one value above the minimum. 
        }

        public List<string> GetClauseDeclarations(Shared.Models.Action action, List<Predicate> usedInitPreds, List<Predicate> usedGoalPreds)
        {
            List<string> predDeclStrings = new();

            predDeclStrings.Add(GetClauseDecleration(action, true, true));

            foreach (Predicate pred in usedInitPreds)
            {
                predDeclStrings.Add(GetClauseDecleration(pred, false, false));
            }

            foreach (Predicate pred in usedGoalPreds)
            {
                predDeclStrings.Add(GetClauseDecleration(pred, false, true));
            }

            return predDeclStrings;
        }

        public List<string> GetTypeDeclerations(List<Clause> clauses, List<string> stateNames)
        {
            List<string> typeDecls = new();

            for (int i = 0; i < clauses.Count; i++)
            {
                typeDecls.AddRange(GetTypeDecleration(stateNames[i] + clauses[i].Name, clauses[i].Parameters.Select(x => x.Entity).ToList(), true));
            }

            return typeDecls;
        }

        public List<string> GetDirectionDeclerations(List<Clause> clauses, List<string> preStrings)
        {
            List<string> decls = new();

            for (int i = 0; i < clauses.Count; i++)
            {
                decls.Add(GetDirectionDecleration(clauses[i], preStrings[i]));
            }

            return decls;
        }

        private string GetClauseDecleration(Clause clause, bool isHeadPred, bool isGoal)
        {
            string predDecl = (isHeadPred ? "head_pred" : "body_pred") + "(";
            if (!isHeadPred)
            {
                predDecl += isGoal ? "goal_" : "init_";
            }
            predDecl += clause.Name + "," + (clause.Parameters.Count + 1) + ").";

            return predDecl;
        }

        private string GetDirectionDecleration(Clause clause, string stateName)
        {
            string decl = "direction(" + stateName + clause.Name + ",(";

            for (int i = 0; i < clause.Parameters.Count; i++)
            {
                decl += "out,";
            }

            decl += "in)).";

            return decl;
        }

        private List<Predicate> GetUsedPredicates(List<Predicate> possiblePredicates, List<PredicateOperator> predicates)
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

        private List<string> GetTypeDecleration(string clauseName, List<Entity> parameters, bool originalCall)
        {
            List<string> typeDecls = new();

            string decl = "type(" + clauseName + ",(";

            for (int i = 0; i < parameters.Count; i++)
            {
                decl += parameters[i].Type.FirstCharToLowerCase() + ",";
            }
            decl += "problem";

            for (int i = 0; i < parameters.Count; i++)
            {
                for (int j = 0; j < parameters[i].Children.Count; j++)
                {
                    List<Entity> replacedParams = CloneEntityList(parameters);
                    replacedParams[i] = parameters[i].Children[j];
                    typeDecls.AddRange(GetTypeDecleration(clauseName, replacedParams, false));
                }
            }

            decl += originalCall ? "))." : "));";
            typeDecls.Add(decl);

            return typeDecls;
        }

        private List<Entity> CloneEntityList(List<Entity> entityList)
        {
            List<Entity> newList = new();

            foreach (Entity entity in entityList)
            {
                Entity newEntity = new();
                newEntity.Name = entity.Name;
                newEntity.Type = entity.Type;
                newEntity.Children = CloneEntityList(entity.Children);
                newList.Add(newEntity);
            }

            return newList;
        }
    }
}
