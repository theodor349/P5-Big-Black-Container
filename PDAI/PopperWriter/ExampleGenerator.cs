using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace PopperWriter
{
    internal class ExampleGenerator
    {
        public void Write(List<Problem> problems, string path)
        {
            File.WriteAllLinesAsync(path, GetActions(problems));
        }

        public string ActionToString(ActionOperator action, string problemName, bool isPositive)
        {
            string actionString = isPositive ? "pos(" : "neg(";
            actionString += action.Name + "(";

            foreach (string attr in action.Attributes)
            {
                actionString += attr + ",";
            }

            actionString += problemName + ")).";

            return actionString;
        }

        public List<string> GetActions(List<Problem> problems)
        {
            List<string> actions = new List<string>();

            foreach (Problem problem in problems)
            {
                foreach (ActionOperator goodOperator in problem.GoodOperators)
                {
                    actions.Add(ActionToString(goodOperator, problem.Name, true));
                }
                foreach (ActionOperator badOperator in problem.BadOperators)
                {
                    actions.Add(ActionToString(badOperator, problem.Name, false));
                }
            }

            return actions;
        }
    }
}
