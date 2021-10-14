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

        public List<string> GetPredicateDeclarations(Shared.Models.Action action, Domain domain)
        {
            List<string> predDeclStrings = new();
            List<PredicateOperator> allInitPredicates = new();
            List<PredicateOperator> allGoalPredicates = new();

            foreach (Problem problem in domain.Problems)
            {
                problem.InitalState.ForEach(x => allInitPredicates.Add(x));
                problem.GoalState.ForEach(x => allGoalPredicates.Add(x));
            }

            List<Predicate> usedInitPreds = GetUsedPredicates(domain.Predicates, allInitPredicates);
            List<Predicate> usedGoalPreds = GetUsedPredicates(domain.Predicates, allGoalPredicates);

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
                    replacedParams.Insert(i, parameters[i].Children[j]);
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
            }

            return newList;
        }
    }
}
