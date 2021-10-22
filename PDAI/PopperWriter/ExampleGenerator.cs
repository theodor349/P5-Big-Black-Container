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
            List<string> actions = GetActions(action, problems);
            actions.Insert(0, ":-style_check(-discontiguous).");

            Task t = File.WriteAllLinesAsync(path + "/exs.pl", actions);
            t.Wait();
        }

        public void WriteTest(Shared.Models.Action action, List<Problem> problems, string path)
        {
            List<string> actions = GetActions(action, problems);
            List<string> output = new();

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].StartsWith("pos"))
                {
                    output.Add("true");
                }
                else
                {
                    output.Add("false");
                }
                actions[i] = actions[i].Remove(0, 4);
                actions[i] = actions[i].Remove(actions[i].Length - 2, 1);
            }

            Task inputTask = File.WriteAllLinesAsync(path + "/input.txt", actions);
            Task outputTask = File.WriteAllLinesAsync(path + "/output.txt", output);
            inputTask.Wait();
            outputTask.Wait();
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
