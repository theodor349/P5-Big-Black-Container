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
        public void Write(Shared.Models.Action action, List<Problem> problems, string path)
        {
            var t = File.WriteAllLinesAsync(path, GetActions(action, problems));
            t.Wait();
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

        public List<string> GetActions(Shared.Models.Action action, List<Problem> problems)
        {
            List<string> actions = new List<string>();

            foreach (Problem problem in problems)
            {
                foreach (ActionOperator goodOperator in problem.GoodOperators)
                {
                    if (goodOperator.Name == action.Name)
                    {
                        actions.Add(ActionToString(goodOperator, problem.Name, true));
                    }
                }
                foreach (ActionOperator badOperator in problem.BadOperators)
                {
                    if (badOperator.Name == action.Name)
                    {
                        actions.Add(ActionToString(badOperator, problem.Name, false));
                    }
                }
            }

            return actions;
        }
    }
}
