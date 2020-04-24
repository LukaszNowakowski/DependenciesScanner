namespace DependenciesReader
{
    using System.Collections.Generic;

    public interface IPackageReader
    {
        Location GetPackages(string path);
    }
}
