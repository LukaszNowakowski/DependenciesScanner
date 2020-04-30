namespace DependenciesReader.DependencyStrategies.BuildDependenciesGraph
{
    using DependenciesReader.ProjectStructure;
    using System.Collections.ObjectModel;

    public class GraphNode
    {
        public Solution Solution { get; set; }

        public Collection<GraphNode> IncomingDependencies { get; } = new Collection<GraphNode>();

        public Collection<GraphNode> OutgoingDependencies { get; } = new Collection<GraphNode>();
    }
}
