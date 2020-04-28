namespace DependenciesReader.ProjectStructure
{
    public class Project
    {
        public Project(string directory, string fileName, string outputName)
        {
            this.Directory = directory;
            this.FileName = fileName;
            this.OutputName = outputName;
        }

        public string Directory { get; }

        public string FileName { get; }

        public string OutputName { get; }
    }
}
