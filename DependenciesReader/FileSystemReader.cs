namespace DependenciesReader
{
    using System.Collections.Generic;
    using System.IO;

    public class FileSystemReader : IFileSystemReader
    {
        public IEnumerable<string> GetPackages(string directory)
        {
            return Directory.GetFiles(directory, "project.json", SearchOption.AllDirectories);
        }
    }
}
