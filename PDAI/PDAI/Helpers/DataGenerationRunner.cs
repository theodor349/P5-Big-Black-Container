using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAI.Helpers
{
    interface IDataGenerationRunner
    {
        public void Run(Action<BiasSetup> action);
    }

    public class BiasRunnerInfinite : IDataGenerationRunner
    {
        private int _minVars;

        public BiasRunnerInfinite(int minVars, int forward)
        {
            _minVars = minVars;
        }

        public int Forward { get; set; }

        public void Run(Action<BiasSetup> action)
        {
            for (int x = 0; x < Forward; x++)
            {
                for (int y = 0; y < x; y++)
                {
                    for (int z = 0; z < x; z++)
                    {
                        action(new BiasSetup()
                        {
                            Var = x + _minVars,
                            Body = y + 5,
                            Clause = z + 5,
                        });
                    }
                }
            }
        }
    }
}
