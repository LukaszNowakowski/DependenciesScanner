namespace DependenciesReader
{
    using DependenciesReader.ProjectStructure;

    public interface ISolutionGenerator
    {
        Solution Create(string rootDirectory, string location);
    }
}
