namespace DependenciesReader
{
    using System.Collections.Generic;

    public interface IFileSystemReader
    {
        IEnumerable<string> GetPackages(string directory);

        IEnumerable<ProjectStructure.Solution> GetSolutions(string directory);
    }
}
