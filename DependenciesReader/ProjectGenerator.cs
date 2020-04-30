namespace DependenciesReader
{
    using System.IO.Abstractions;
    using System.Linq;

    using DependenciesReader.ProjectStructure;

    public class ProjectGenerator : IProjectGenerator
    {
        private readonly IFileSystem fileSystem;

        private readonly IProjectDetailsReader projectDetailsReader;

        public ProjectGenerator(IFileSystem fileSystem, IProjectDetailsReader projectDetailsReader)
        {
            this.fileSystem = fileSystem;
            this.projectDetailsReader = projectDetailsReader;
        }

        public Project CreateProject(string location, string solutionLocation)
        {
            var projectFile = this.fileSystem.Path.GetFileName(location);
            var projectDirectory = this.fileSystem.Path.GetDirectoryName(location);
            var relativeDirectory = this.fileSystem.MakeRelativePath(solutionLocation, projectDirectory);
            var outputName = this.projectDetailsReader.GetOutputName(location);
            var dependencies = this.projectDetailsReader.GetPackages(location)
                .Select(p => new Dependency(p.Name, p.Version));
            return new Project(relativeDirectory, projectFile, outputName, dependencies);
        }
    }
}
