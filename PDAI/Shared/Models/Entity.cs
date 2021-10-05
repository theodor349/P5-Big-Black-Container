using System;

namespace Shared.Models
{
    public class Entity
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Entity Parent { get; set; }
    }
}
