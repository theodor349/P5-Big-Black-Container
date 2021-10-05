using System;
using System.Collections.Generic;

namespace Shared.Models
{
    public class Action
    {
        public string Name { get; set; }
        public List<Type> InputTypes { get; set; } = new List<Type>();

        public Action()
        {
            InputTypes.Add(typeof(int));
        }
    }
}
