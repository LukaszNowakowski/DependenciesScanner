namespace DependenciesReader
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Newtonsoft.Json.Linq;

    public class ProjectDetailsReader : IProjectDetailsReader
    {
        private readonly IFileSystem fileSystem;

        public ProjectDetailsReader(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string GetOutputName(string fullPath)
        {
            if (!this.fileSystem.File.Exists(fullPath))
            {
                throw new FileNotFoundException("Project file not found", fullPath);
            }

            XDocument document = null;
            using (var fileStream = this.fileSystem.File.Open(fullPath, FileMode.Open))
            {
                document = XDocument.Load(fileStream);
            }

            var manager = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace("vs", "http://schemas.microsoft.com/developer/msbuild/2003");
            var element = document.XPathSelectElement("/vs:Project/vs:PropertyGroup/vs:AssemblyName", manager);
            if (element != null)
            {
                return element?.Value;
            }

            return this.fileSystem.Path.GetFileNameWithoutExtension(fullPath);
        }

        public IEnumerable<PackageReference> GetPackages(string path)
        {
            var packagesPath = this.PackagesPath(path);
            if (!this.fileSystem.File.Exists(packagesPath))
            {
                yield break;
            }

            var json = JObject.Parse(this.fileSystem.File.ReadAllText(packagesPath));
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

        private string PackagesPath(string projectPath)
        {
            var parent = this.fileSystem.Path.GetDirectoryName(projectPath);
            var result = this.fileSystem.Path.Combine(parent, "project.json");
            return result;
        }
    }
}
