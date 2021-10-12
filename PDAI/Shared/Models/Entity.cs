using System;

namespace Shared.Models
{
    public class Entity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Entity Parent { get; set; }
    }
}
