using System.Collections.Generic;

namespace Shared.Models
{
    public class Predicate
    {
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
