using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAI.Helpers
{

    public class BiasIncrement
    {
        public int Var { get; set; }
        public int Body { get; set; }
        public int Clause { get; set; }
    }
    public interface IBiasEnumerator
    {
        BiasIncrement GetIncrement(int iteration);
    }

    public class BiasVarEnumerator : IBiasEnumerator
    {
        public BiasIncrement GetIncrement(int iteration)
        {
            return new BiasIncrement()
            {
                Var = iteration % int.MaxValue,
                Body = 0,
                Clause = 0,
            };
        }
    }

    public class BiasAllEnumerator : IBiasEnumerator
    {
        public BiasIncrement GetIncrement(int iteration)
        {
            return new BiasIncrement()
            {
                Var = iteration % 3 >= 0 ? 1 : 0,
                Body = iteration % 3 >= 1 ? 1 : 0,
                Clause = iteration % 3 >= 2 ? 1 : 0,
            };
        }
    }
}
