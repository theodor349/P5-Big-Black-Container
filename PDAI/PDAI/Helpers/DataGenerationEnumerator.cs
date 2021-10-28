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
        BiasIncrement GetIncrement(long iteration);
    }

    public class BiasVarEnumerator : IBiasEnumerator
    {
        public BiasIncrement GetIncrement(long iteration)
        {
            return new BiasIncrement()
            {
                Var = 1,
                Body = 0,
                Clause = 0,
            };
        }
    }
}
