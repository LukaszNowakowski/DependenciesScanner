namespace DependenciesReader
{
    using System.Collections.Generic;

    public interface IFileSystemReader
    {
        IEnumerable<string> GetPackages(string directory);
    }
}
