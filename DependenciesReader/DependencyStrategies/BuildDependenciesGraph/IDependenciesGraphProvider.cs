namespace DependenciesReader.DependencyStrategies.BuildDependenciesGraph
{
    using System.Collections.Generic;

    using DependenciesReader.ProjectStructure;

    public interface IDependenciesGraphProvider
    {
        IList<GraphNode> CreateGraph(IList<Solution> solutions);
    }
}
