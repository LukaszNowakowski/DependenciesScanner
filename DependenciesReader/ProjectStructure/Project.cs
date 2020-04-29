namespace DependenciesReader.ProjectStructure
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class Project
    {
        public Project(string directory, string fileName, string outputName, IEnumerable<Dependency> dependencies)
        {
            this.Directory = directory;
            this.FileName = fileName;
            this.OutputName = outputName;
            this.Dependencies = dependencies.ToReadOnlyCollection();
        }

        public string Directory { get; }

        public string FileName { get; }

        public string OutputName { get; }

        public ReadOnlyCollection<Dependency> Dependencies { get; }
    }
}
