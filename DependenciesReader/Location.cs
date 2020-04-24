namespace DependenciesReader
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class Location
    {
        public Location(string path, IEnumerable<PackageReference> packages)
        {
            this.Path = path;
            this.Packages = new ReadOnlyCollection<PackageReference>((packages ?? Enumerable.Empty<PackageReference>()).ToList());
        }

        public string Path { get; }

        public ReadOnlyCollection<PackageReference> Packages { get; }
    }
}
