using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Clause
    {
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
