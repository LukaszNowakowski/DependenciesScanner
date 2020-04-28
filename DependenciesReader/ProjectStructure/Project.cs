namespace DependenciesReader.ProjectStructure
{
    public class Project
    {
        public Project(string directory, string fileName)
        {
            this.Directory = directory;
            this.FileName = fileName;
        }

        public string Directory { get; }

        public string FileName { get; }
    }
}
