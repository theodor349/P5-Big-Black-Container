using System;
using System.Collections.Generic;

namespace Shared.Models
{
    public class Problem : IComparable
    {
        public string Name { get; set; }
        public List<PredicateOperator> InitalState { get; set; } = new List<PredicateOperator>();
        public List<PredicateOperator> GoalState { get; set; } = new List<PredicateOperator>();
        public List<ActionOperator> GoodOperators { get; set; } = new List<ActionOperator>();
        public List<ActionOperator> BadOperators { get; set; } = new List<ActionOperator>();

        public int CompareTo(object obj)
        {
            Problem otherProblem = obj as Problem;

            int thisSum = this.GoodOperators.Count + this.BadOperators.Count;
            int otherSum = otherProblem.GoodOperators.Count + otherProblem.BadOperators.Count;

            if (thisSum < otherSum)
            {
                return -1;
            }
            else if (thisSum > otherSum)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
