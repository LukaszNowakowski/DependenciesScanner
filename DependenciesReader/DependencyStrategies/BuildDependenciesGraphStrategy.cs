namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using DependenciesReader.DependencyStrategies.BuildDependenciesGraph;
    using DependenciesReader.ProjectStructure;

    public class BuildDependenciesGraphStrategy : IStrategy
    {
        public void CreateReport(IList<Solution> projects, Action<string> reportWriter)
        {
            var nodes = this.CreateNodes(projects);
            foreach (var node in nodes)
            {
                reportWriter(Path.Combine(node.Solution.Directory, node.Solution.FileName));
                foreach (var incomingDependency in node.IncomingDependencies)
                {
                    var reportItem = string.Format(
                        CultureInfo.InvariantCulture,
                        "\t<= {0}",
                        incomingDependency.Solution.FileName);
                    reportWriter(reportItem);
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
    }
}
