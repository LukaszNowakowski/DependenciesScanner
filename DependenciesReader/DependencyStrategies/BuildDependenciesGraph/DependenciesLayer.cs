namespace DependenciesReader.DependencyStrategies.BuildDependenciesGraph
{
    using System.Collections.ObjectModel;

    using DependenciesReader.ProjectStructure;

    public class DependenciesLayer
    {
        public int Number { get; set; }

        public Collection<Solution> Solutions { get; } = new Collection<Solution>();
    }
}
