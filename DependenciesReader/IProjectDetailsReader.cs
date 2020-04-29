namespace DependenciesReader
{
    using System.Collections.Generic;

    public interface IProjectDetailsReader
    {
        string GetOutputName(string fullPath);

        IEnumerable<PackageReference> GetPackages(string path);
    }
}
