using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Domain
    {
        public string Name { get; set; }
        public List<Action> Actions { get; set; } = new List<Action>();
        public List<Predicate> Predicates { get; set; } = new List<Predicate>();
        public List<Entity> Entities { get; set; } = new List<Entity>();
        public List<Problem> Problems { get; set; } = new List<Problem>();
    }
}
