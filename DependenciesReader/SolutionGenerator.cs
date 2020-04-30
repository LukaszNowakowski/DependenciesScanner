namespace DependenciesReader
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    using DependenciesReader.ProjectStructure;

    public class SolutionGenerator : ISolutionGenerator
    {
        private readonly IFileSystem fileSystem;

        private readonly IProjectGenerator projectGenerator;

        public SolutionGenerator(IFileSystem fileSystem, IProjectGenerator projectGenerator)
        {
            this.fileSystem = fileSystem;
            this.projectGenerator = projectGenerator;
        }

        public Solution Create(string rootDirectory, string location)
        {
            var solutionFile = this.fileSystem.Path.GetFileName(location);
            var solutionDirectory = this.fileSystem.Path.GetDirectoryName(location);
            var relativeDirectory = this.fileSystem.MakeRelativePath(rootDirectory, solutionDirectory);
            return new Solution(relativeDirectory, solutionFile, this.CreateProjects(rootDirectory, relativeDirectory));
        }

        private IEnumerable<Project> CreateProjects(string rootDirectory, string solutionDirectory)
        {
            var directory = this.fileSystem.Path.Combine(rootDirectory, solutionDirectory);
            var projectFiles = this.fileSystem.Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);
            return projectFiles.Select(location => this.projectGenerator.CreateProject(location, directory));
        }
    }
}
