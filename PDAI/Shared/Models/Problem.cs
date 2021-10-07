using System.Collections.Generic;

namespace Shared.Models
{
    public class Problem
    {
        public string Name { get; set; }
        public List<PredicateOperator> InitalState { get; set; } = new List<PredicateOperator>();
        public List<PredicateOperator> GoalState { get; set; } = new List<PredicateOperator>();
        public List<ActionOperator> GoodOperators { get; set; } = new List<ActionOperator>();
        public List<ActionOperator> BadOperators { get; set; } = new List<ActionOperator>();
    }
}
