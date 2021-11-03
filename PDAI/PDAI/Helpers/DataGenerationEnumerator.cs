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
                Var = 1,
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

    public class BiasRandomEnumerator : IBiasEnumerator
    {
        public BiasIncrement GetIncrement(int iteration)
        {
            var rand = new Random().Next();

            return new BiasIncrement()
            {
                Var = rand % 3 == 0 ? 1 : 0,
                Body = rand % 3 == 1 ? 1 : 0,
                Clause = rand % 3 == 2 ? 1 : 0,
            };
        }
    }

    public class BiasBodyEnumerator : IBiasEnumerator
    {
        public BiasIncrement GetIncrement(int iteration)
        {
            return new BiasIncrement()
            {
                Var = 0,
                Body = 1,
                Clause = 0,
            };
        }
    }

    public class BiasClauseEnumerator : IBiasEnumerator
    {
        public BiasIncrement GetIncrement(int iteration)
        {
            return new BiasIncrement()
            {
                Var = 0,
                Body = 0,
                Clause = 1,
            };
        }
    }

    public class BiasAllReverseEnumerator : IBiasEnumerator
    {
        public BiasIncrement GetIncrement(int iteration)
        {
            return new BiasIncrement()
            {
                Var = iteration % 3 >= 2 ? 1 : 0,
                Body = iteration % 3 >= 1 ? 1 : 0,
                Clause = iteration % 3 >= 0 ? 1 : 0,
            };
        }
    }

}
