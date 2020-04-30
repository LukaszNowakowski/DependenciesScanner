namespace DependenciesReader
{
    using DependenciesReader.ProjectStructure;

    public interface IProjectGenerator
    {
        Project CreateProject(string location, string solutionLocation);
    }
}
