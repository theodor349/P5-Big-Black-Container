using System;
using System.Collections.Generic;

namespace ParseNegativeAndPositive
{
    class Command
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();

        public Command(string line)
        {
            line = line.Replace("(", " ");
            line = line.Replace(")", " ");
            line = line.Replace(",", "");
            line = line.Trim();

            var words = line.Split(" ");
            Name = words[0];
            for (int i = 1; i < words.Length; i++)
            {
                Attributes.Add(words[i]);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Command))
                return false;

            var other = (Command)obj;

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
}
