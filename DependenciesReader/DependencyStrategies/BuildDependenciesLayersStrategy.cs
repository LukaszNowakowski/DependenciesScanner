namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    using DependenciesReader.DependencyStrategies.BuildDependenciesGraph;
    using DependenciesReader.DependencyStrategies.BuildDependenciesLayers;
    using DependenciesReader.ProjectStructure;

    public class BuildDependenciesLayersStrategy : IStrategy
    {
        private IDependenciesGraphProvider graphProvider;

        public BuildDependenciesLayersStrategy(IDependenciesGraphProvider graphProvider)
        {
            this.graphProvider = graphProvider;
        }

        public void CreateReport(IList<Solution> projects, Action<string> reportWriter)
        {
            var nodes = this.graphProvider.CreateGraph(projects);
            var layers = this.BuildLayers(nodes);
            foreach (var layer in layers)
            {
                reportWriter(layer.Number.ToString("Layer 0", CultureInfo.InvariantCulture));
                foreach (var node in layer.Nodes)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "\t- {0}",
                        node.Solution.AbsolutePath(@"\"));
                    reportWriter(message);
                }
            }
        }

        private static DependenciesLayer BuildLayer(IList<GraphNode> allNodes, int number)
        {
            var firstLayer = new DependenciesLayer { Number = number };
            firstLayer.Nodes.AddRange(
                allNodes.Where(n => n.IncomingDependencies.Count == 0)
                    .ToArray());
            ClearUsedDependencies(allNodes, firstLayer);
            return firstLayer;
        }

        private static void ClearUsedDependencies(IList<GraphNode> allNodes, DependenciesLayer currentLayer)
        {
            foreach (var solution in currentLayer.Nodes)
            {
                foreach (var node in allNodes)
                {
                    if (node.IncomingDependencies.Contains(solution))
                    {
                        node.IncomingDependencies.Remove(solution);
                    }
                }
            }
        }

        private IList<DependenciesLayer> BuildLayers(IList<GraphNode> allNodes)
        {
            var candidateNodes = new Collection<GraphNode>(allNodes);
            var result = new Collection<DependenciesLayer>();
            var layerNumber = 1;
            while (candidateNodes.Count > 0)
            {
                var firstLayer = BuildLayer(candidateNodes, layerNumber);
                result.Add(firstLayer);
                candidateNodes = new Collection<GraphNode>(candidateNodes.Where(n => firstLayer.Nodes.All(l => l != n)).ToList());
                layerNumber++;
            }

            return result;
        }
    }
}
