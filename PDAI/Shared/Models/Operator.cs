using System.Collections.Generic;

namespace Shared.Models
{
    public class Operator
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();

        public Operator(string line)
        {
            var words = line
            .Replace('(',' ')
            .Replace(')',' ')
            .Replace(",", "")
            .Trim()
            .Split(' ');

            Name = words[0];

            for (int i = 1; i < words.Length; i++)
            {
                Attributes.Add(words[i]);
            }
        }

        public override bool Equals(object obj) 
        {
           if (obj is not Operator)
                return false;

            var other = (Operator)obj;

            if (!Name.Equals(other.Name))
                return Name.Equals(other.Name);
            if (Attributes.Count != other.Attributes.Count)
                return Attributes.Count.Equals(other.Attributes.Count);

            for (int i = 0; i < Attributes.Count; i++)
            {
                var a = Attributes[i];
                var b = other.Attributes[i];
                if (!a.Equals(b))
                    return a.Equals(b);
            }
            return true; 
        }
    }

    public class ActionOperator : Operator
    {
        public ActionOperator(string line) : base(line)
        {
        }
    }
    public class PredicateOperator : Operator
    {
        public PredicateOperator(string line) : base(line)
        {
        }
    }

}
