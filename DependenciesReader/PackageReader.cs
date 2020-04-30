namespace DependenciesReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json.Linq;

    public class PackageReader : IPackageReader
    {
        public Location GetPackages(string path)
        {
            var references = this.ReadPackages(path);
            return new Location(path, references);
        }

        private IEnumerable<PackageReference> ReadPackages(string path)
        {
            var json = JObject.Parse(File.ReadAllText(path));
            var packages = json["dependencies"];
            if (packages == null)
            {
                yield break;
            }

            foreach (var child in packages.Children<JProperty>())
            {
                var current = new PackageReference(child.Name, child.Value.ToString());
                yield return current;
            }
        }
    }
}
