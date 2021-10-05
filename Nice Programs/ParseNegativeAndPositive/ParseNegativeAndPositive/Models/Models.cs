using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseNegativeAndPositive.Models
{
    public class Domain
    {
        public List<Action> Actions { get; set; } = new List<Action>();
        public List<Predicate> Predicates { get; set; } = new List<Predicate>();
        public List<Entity> Entities { get; set; } = new List<Entity>();
        public List<Problem> Problems { get; set; }
    }

    public class Action
    {
        public string Name { get; set; }
        public List<Type> InputTypes { get; set; } = new List<Type>();
            
        public Action()
        {
            InputTypes.Add(typeof(int));
        }
    }

    public class Predicate
    {
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }

    public class Parameter
    {
        public string Name { get; set; }
        public Entity Entity { get; set; }
    }

    public class Entity
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Entity Parent { get; set; }
    }

    public class Problem
    {
        public List<PredicateOperator> InitalState { get; set; } = new List<PredicateOperator>();
        public List<PredicateOperator> GoalState { get; set; } = new List<PredicateOperator>();
        public List<ActionOperator> GoodOperators { get; set; } = new List<ActionOperator>();
        public List<ActionOperator> BadOperators { get; set; } = new List<ActionOperator>();
    }

    public class Operator
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();
    }
    public class ActionOperator : Operator {}
    public class PredicateOperator : Operator {}
}