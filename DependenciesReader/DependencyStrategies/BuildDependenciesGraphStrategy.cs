namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using DependenciesReader.DependencyStrategies.BuildDependenciesGraph;
    using DependenciesReader.ProjectStructure;

    public class BuildDependenciesGraphStrategy : IStrategy
    {
        public void CreateReport(IList<Solution> projects, Action<string> reportWriter)
        {
            var nodes = this.CreateNodes(projects).ToList();
            this.FindDependencies(nodes);
            var layers = this.BuildLayers(nodes);
            foreach (var layer in layers)
            {
                reportWriter(layer.Number.ToString());
                foreach (var solution in layer.Solutions)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, "\t- {0}", solution.AbsolutePath(@"\"));
                    reportWriter(message);
                }
            }
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

        private IList<DependenciesLayer> BuildLayers(IList<GraphNode> allNodes)
        {
            var result = new Collection<DependenciesLayer>();
            var firstLayer = new DependenciesLayer{ Number = 1 };
            firstLayer.Solutions.AddRange(
                allNodes.Where(n => n.IncomingDependencies.Count == 0)
                    .Select(n => n.Solution)
                    .ToArray());
            result.Add(firstLayer);
            return result;
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
