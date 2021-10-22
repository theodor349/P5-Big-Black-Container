using Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopperWriter
{
    internal class BiasGenerator
    {
        public void Write(Shared.Models.Action action, List<Problem> problems, List<Predicate> predicates, string path)
        {
            List<string> lines = new();

            List<Predicate> usedInitPreds = GetUsedPreds(problems, predicates, true);
            List<Predicate> usedGoalPreds = GetUsedPreds(problems, predicates, false);

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

            int maxVars = Math.Max(usedInitPreds.Select(x => x.Parameters.Count).Max(), usedGoalPreds.Select(x => x.Parameters.Count).Max());
            maxVars = Math.Max(maxVars, action.Parameters.Count);

            lines.AddRange(GetConstraints(maxVars));
            lines.AddRange(GetClauseDeclarations(action, usedInitPreds, usedGoalPreds));
            lines.AddRange(GetTypeDeclerations(allClauses, allClausesPre).Distinct().ToList());

            var t = File.WriteAllLinesAsync(path, lines);
            t.Wait();
        }

        public List<Predicate> GetUsedPreds(List<Problem> problems, List<Predicate> predicates, bool initPreds)
        {
            List<PredicateOperator> allPredicates = new();

            foreach (Problem problem in problems)
            {
                if (initPreds)
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

        public string GetClauseDecleration(Clause clause, bool isHeadPred, bool isGoal)
        {
            string predDecl = (isHeadPred ? "head_pred" : "body_pred") + "(";
            if (!isHeadPred)
            {
                predDecl += isGoal ? "goal_" : "init_";
            }
            predDecl += clause.Name + "," + (clause.Parameters.Count + 1) + ").";

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

        public List<string> GetTypeDeclerations(List<Clause> clauses, List<string> preStrings)
        {
            List<string> typeDecls = new();

            for (int i = 0; i < clauses.Count; i++)
            {
                typeDecls.AddRange(GetTypeDecleration(preStrings[i] + clauses[i].Name, clauses[i].Parameters.Select(x => x.Entity).ToList(), true));
            }

            return typeDecls;
        }

        public List<string> GetTypeDecleration(string clauseName, List<Entity> parameters, bool originalCall)
        {
            List<string> typeDecls = new();

            string decl = "type(" + clauseName + ",(";

            for (int i = 0; i < parameters.Count; i++)
            {
                decl += parameters[i].Type + ",";
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

        public string GetDirectionDecleration(Clause clause)
        {
            string decl = "direction(" + clause.Name + ",(";

            for (int i = 0; i < clause.Parameters.Count; i++)
            {
                decl += "in,";
            }

            decl += "out)).";

            return decl;
        }

        public List<string> GetDirectionDeclerations(List<Clause> clauses)
        {
            List<string> decls = new();

            foreach(Clause clause in clauses)
            {
                decls.Add(GetDirectionDecleration(clause));
            }

            return decls;
        }

        public List<string> GetConstraints(int maxVars)
        {
            return new List<string>() { "max_clauses(5).", "max_body(5).", "max_vars(" + (maxVars + 2) + ")." };
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
