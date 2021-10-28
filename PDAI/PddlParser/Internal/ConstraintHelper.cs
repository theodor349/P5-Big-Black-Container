using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PddlParser.Internal
{
    public class ConstraintHelper
    {
        public void ChangeConstraint(string fileName, int max_clauses, int max_body, int max_vars)
        {

            string path = fileName;
            var first3Lines = File.ReadLines(path).ToList();

            first3Lines[0] = "max_clauses(" + max_clauses + ")";
            first3Lines[1] = "max_body(" + max_body + ")";
            first3Lines[2] = "max_vars(" + max_vars + ")";

            /*
            List<string> maxNames = new List<string> { "clauses", "body", "vars" };
            List<int> param = new List<int> { max_clauses, max_body, max_vars};

            for (int i = 0; i < 3; i++)
            {
                first3Lines[i] = "max_" + maxNames[i] + "(" + param[i] + ")";
            }*/


            File.WriteAllLines(path, first3Lines);;
        }

        public void IncrementConstraintValues(string fileName, int increment_max_clauses, int increment_max_body, int increment_max_vars)
        {
            string path = fileName;
            var first3Lines = File.ReadLines(path).ToList();

            var oldMaxClauses = first3Lines[0].FirstOrDefault(c => char.IsDigit(c));
            var oldMaxBody = first3Lines[1].FirstOrDefault(c => char.IsDigit(c));
            var oldMaxVars = first3Lines[2].FirstOrDefault(c => char.IsDigit(c));

            int incremented_Clauses = (int)char.GetNumericValue(oldMaxClauses) + increment_max_clauses;
            int incremented_Body = (int)char.GetNumericValue(oldMaxBody) + increment_max_body;
            int incremented_Vars = (int)char.GetNumericValue(oldMaxVars) + increment_max_vars;

            first3Lines[0] = "max_clauses(" + incremented_Clauses + ")";
            first3Lines[1] = "max_body(" + incremented_Body + ")";
            first3Lines[2] = "max_vars(" + incremented_Vars + ")";

            File.WriteAllLines(path, first3Lines); ;
        }
    }
}
