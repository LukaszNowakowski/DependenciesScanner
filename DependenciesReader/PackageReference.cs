namespace DependenciesReader
{
    public class PackageReference
    {
        public PackageReference(string name, string version)
        {
            this.Name = name;
            this.Version = version;
        }

        public string Name { get; }

        public string Version { get; }
    }
}
