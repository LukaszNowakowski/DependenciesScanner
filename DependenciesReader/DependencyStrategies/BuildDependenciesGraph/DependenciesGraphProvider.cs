namespace DependenciesReader.DependencyStrategies.BuildDependenciesGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DependenciesReader.ProjectStructure;

    public class DependenciesGraphProvider :IDependenciesGraphProvider
    {
        public IList<GraphNode> CreateGraph(IList<Solution> solutions)
        {
            var nodes = this.CreateNodes(solutions).ToList();
            this.FindDependencies(nodes);
            return new List<GraphNode>(nodes);
        }

        private IEnumerable<GraphNode> CreateNodes(IList<Solution> projects)
        {
            foreach (var project in projects)
            {
                yield return new GraphNode() { Solution = project };
            }
        }

        private void FindDependencies(IList<GraphNode> allNodes)
        {
            foreach (var node in allNodes)
            {
                this.FillNodeDependencies(node, allNodes);
            }
        }

        private void FillNodeDependencies(GraphNode current, IList<GraphNode> allNodes)
        {
            foreach (var candidate in allNodes)
            {
                if (candidate == current)
                {
                    continue;
                }

                if (current.Solution.Dependencies.Any(
                    d => candidate.Solution.OutputNames.Any(
                        on => on.Equals(d.Name, StringComparison.InvariantCultureIgnoreCase))))
                {
                    current.IncomingDependencies.Add(candidate);
                    candidate.OutgoingDependencies.Add(current);
                }
            }
        }
    }
}
