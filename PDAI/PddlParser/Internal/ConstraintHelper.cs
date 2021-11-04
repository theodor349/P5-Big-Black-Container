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

            var oldMaxClauses = lines[0].FirstOrDefault(c => char.IsDigit(c));
            var oldMaxBody = lines[1].FirstOrDefault(c => char.IsDigit(c));
            var oldMaxVars = lines[2].FirstOrDefault(c => char.IsDigit(c));

            int incremented_Clauses = (int)char.GetNumericValue(oldMaxClauses) + increment_max_clauses;
            int incremented_Body = (int)char.GetNumericValue(oldMaxBody) + increment_max_body;
            int incremented_Vars = (int)char.GetNumericValue(oldMaxVars) + increment_max_vars;

            lines[0] = "max_clauses(" + incremented_Clauses + ").";
            lines[1] = "max_body(" + incremented_Body + ").";
            lines[2] = "max_vars(" + incremented_Vars + ").";

            File.WriteAllLines(filepath, lines);
        }
    }
}
