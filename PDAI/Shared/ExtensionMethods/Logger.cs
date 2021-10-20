using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ExtensionMethods
{
    public static class Logger
    {
        public static void Log(string message)
        {
            var time = DateTime.Now.ToString("dd/MM HH:mm");
            Console.WriteLine(time + " | " + message);
        }
    }
}
