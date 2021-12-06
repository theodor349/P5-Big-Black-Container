using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PddlParser.Internal
{
    public class ConstraintHelper
    {
        public void ChangeConstraint(string filepath, int max_clauses, int max_body, int max_vars)
        {
            List<string> lines = File.ReadLines(filepath).ToList();
            lines[0] = "max_clauses(" + max_clauses + ").";
            lines[1] = "max_body(" + max_body + ").";
            lines[2] = "max_vars(" + max_vars + ").";

            File.WriteAllLines(filepath, lines);
        }

        public void IncrementConstraintValues(string filepath, int increment_max_clauses, int increment_max_body, int increment_max_vars)
        {
            var lines = File.ReadLines(filepath).ToList();

            int lineIndex = 0;
            if (lines[0].ToLower().Equals("enable_pi."))
                lineIndex = 1;

            var oldMaxClauses = new string(lines[lineIndex].SkipWhile(c => !char.IsDigit(c))
                         .TakeWhile(c => char.IsDigit(c))
                         .ToArray());
            var oldMaxBody = new string(lines[1].SkipWhile(c => !char.IsDigit(c))
                         .TakeWhile(c => char.IsDigit(c))
                         .ToArray());
            var oldMaxVars = new string(lines[2].SkipWhile(c => !char.IsDigit(c))
                         .TakeWhile(c => char.IsDigit(c))
                         .ToArray());

            int incremented_Clauses = int.Parse(oldMaxClauses) + increment_max_clauses;
            int incremented_Body = int.Parse(oldMaxBody) + increment_max_body;
            int incremented_Vars = int.Parse(oldMaxVars) + increment_max_vars;

            lines[0] = "max_clauses(" + incremented_Clauses + ").";
            lines[1] = "max_body(" + incremented_Body + ").";
            lines[2] = "max_vars(" + incremented_Vars + ").";

            File.WriteAllLines(filepath, lines);
        }

        public void AddRecursion(string filepath)
        {
            File.AppendAllText(filepath, "enable_recursion." + Environment.NewLine);
        }

        public void AddPredicateInvension(string filepath)
        {
            File.AppendAllText(filepath, "enable_pi." + Environment.NewLine);
        }
    }
}
