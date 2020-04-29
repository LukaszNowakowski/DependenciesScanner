namespace DependenciesReader
{
    using System.Collections.Generic;

    public interface IFileSystemReader
    {
        IEnumerable<ProjectStructure.Solution> GetSolutions(string directory);
    }
}
