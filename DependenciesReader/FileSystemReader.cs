namespace DependenciesReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    using DependenciesReader.ProjectStructure;

    public class FileSystemReader : IFileSystemReader
    {
        private readonly IFileSystem fileSystem;

        private readonly IProjectDetailsReader projectDetailsReader;

        public FileSystemReader(IFileSystem fileSystem, IProjectDetailsReader projectDetailsReader)
        {
            this.fileSystem = fileSystem;
            this.projectDetailsReader = projectDetailsReader;
        }

        public IEnumerable<string> GetPackages(string directory)
        {
            return Directory.GetFiles(directory, "project.json", SearchOption.AllDirectories);
        }

        public IEnumerable<Solution> GetSolutions(string directory)
        {
            var solutionFiles = this.fileSystem.Directory.GetFiles(directory, "*.sln", SearchOption.AllDirectories);
            return solutionFiles.Select(location => this.CreateSolution(location, directory));
        }

        private static string MakeRelativePath(string location, string rootDirectory)
        {
            var baseUri = new Uri(rootDirectory.TrimEnd('\\') + "\\");
            var solutionUri = new Uri(location.TrimEnd('\\') + "\\");
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(solutionUri)
                .ToString()
                .Replace('/', Path.DirectorySeparatorChar))
                .TrimEnd('\\');
        }

        private Solution CreateSolution(string location, string rootDirectory)
        {
            var solutionFile = Path.GetFileName(location);
            var solutionDirectory = Path.GetDirectoryName(location);
            var relativeDirectory = MakeRelativePath(solutionDirectory, rootDirectory);
            return new Solution(relativeDirectory, solutionFile, this.CreateProjects(rootDirectory, relativeDirectory));
        }

        private IEnumerable<Project> CreateProjects(string rootDirectory, string solutionDirectory)
        {
            var directory = this.fileSystem.Path.Combine(rootDirectory, solutionDirectory);
            var projectFiles = this.fileSystem.Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);
            return projectFiles.Select(location => this.CreateProject(location, directory));
        }

        private Project CreateProject(string location, string solutionLocation)
        {
            var projectFile = this.fileSystem.Path.GetFileName(location);
            var projectDirectory = this.fileSystem.Path.GetDirectoryName(location);
            var relativeDirectory = MakeRelativePath(projectDirectory, solutionLocation);
            return new Project(relativeDirectory, projectFile, this.projectDetailsReader.GetOutputName(location));
        }
    }
}
