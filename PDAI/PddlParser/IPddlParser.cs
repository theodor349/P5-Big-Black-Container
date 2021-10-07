namespace Parser.Pddl
{
    public interface IPddlParser
    {
        void Parse(string domainFolderPath, int maxProblems = int.MaxValue);
    }
}