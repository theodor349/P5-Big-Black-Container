using System;
using System.Collections.Generic;

namespace Shared.Models
{
    public class Entity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<Entity> Children { get; set; }
    }
}
