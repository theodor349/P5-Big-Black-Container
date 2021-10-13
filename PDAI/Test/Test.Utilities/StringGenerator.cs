using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Utilities
{
    public class StringGenerator
    {
        public static List<string> GetTypesList(int amount, string type)
        {
            var res = new List<string>();
            res.Add("(:types");
            for (int i = 0; i < amount; i++)
            {
                res.Add(type);
            }
            res.Add(")");
            return res;
        }
    }
}
