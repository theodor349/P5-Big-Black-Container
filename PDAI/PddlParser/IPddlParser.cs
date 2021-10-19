using Shared.Models;

namespace Parser.Pddl
{
    public interface IPddlParser
    {
        Domain Parse(string domainFolderPath, int maxProblems = int.MaxValue);
    }
}