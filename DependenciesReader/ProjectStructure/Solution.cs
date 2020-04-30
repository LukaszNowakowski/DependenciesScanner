namespace DependenciesReader.ProjectStructure
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    public class Solution
    {
        public Solution(string directory, string fileName, IEnumerable<Project> projects)
        {
            this.Directory = directory;
            this.FileName = fileName;
            this.Projects = projects.ToReadOnlyCollection();
        }

        public string Directory { get; }

        public string FileName { get; }

        public ReadOnlyCollection<Project> Projects { get; }

        public IEnumerable<string> OutputNames => this.Projects.Select(p => p.OutputName);

        public IEnumerable<Dependency> Dependencies =>
            this.Projects.SelectMany(p => p.Dependencies)
                .Distinct(new Dependency.Comparer());

        public string AbsolutePath(string baseDirectory)
        {
            return Path.Combine(baseDirectory, this.Directory, this.FileName);
        }
    }
}
