using System.Collections.Generic;

namespace Shared.Models
{
    public class Operator
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();
    }

    public class ActionOperator : Operator { }
    public class PredicateOperator : Operator { }
}
