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

        private readonly ISolutionGenerator solutionGenerator;

        public FileSystemReader(IFileSystem fileSystem, ISolutionGenerator solutionGenerator)
        {
            this.fileSystem = fileSystem;
            this.solutionGenerator = solutionGenerator;
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

        private Solution CreateSolution(string location, string rootDirectory)
        {
            return this.solutionGenerator.Create(rootDirectory, location);
        }
    }
}
