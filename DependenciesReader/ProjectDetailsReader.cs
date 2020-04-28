namespace DependenciesReader
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

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
            return element?.Value;
        }
    }
}
