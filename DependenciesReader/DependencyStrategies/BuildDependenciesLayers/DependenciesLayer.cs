namespace DependenciesReader.DependencyStrategies.BuildDependenciesLayers
{
    using System.Collections.ObjectModel;

    using DependenciesReader.DependencyStrategies.BuildDependenciesGraph;

    public class DependenciesLayer
    {
        public int Number { get; set; }

        public Collection<GraphNode> Nodes { get; } = new Collection<GraphNode>();
    }
}
