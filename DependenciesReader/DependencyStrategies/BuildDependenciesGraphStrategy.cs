namespace DependenciesReader.DependencyStrategies
{
    using DependenciesReader.DependencyStrategies.BuildDependenciesGraph;
    using DependenciesReader.ProjectStructure;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class BuildDependenciesGraphStrategy : IStrategy
    {
        private readonly IDependenciesGraphProvider graphProvider;

        public BuildDependenciesGraphStrategy(IDependenciesGraphProvider graphProvider)
        {
            this.graphProvider = graphProvider;
        }

        public void CreateReport(IList<Solution> projects, Action<string> reportWriter)
        {
            var nodes = this.graphProvider.CreateGraph(projects);
            reportWriter("elements: [");
            var clusterNames = ClusteringStrategy.GetAllClusters();
            foreach (var clusterName in clusterNames)
            {
                reportWriter($"\tv{{ data: {{ id: '{clusterName}', label: '{clusterName}' }} }},");
            }

            var outputs = nodes.ToList();
            foreach (var node in outputs)
            {
                var name = node.Solution.Projects.Single(NotTestProject).OutputName;
                var cluster = ClusteringStrategy.GetClusterName(name);
                var nodeLine = string.Format(
                    CultureInfo.InvariantCulture,
                    "\t{{ data:  {{ id: '{0}', label: '{0}', parent: '{1}' }} }},",
                    name,
                    cluster);
                reportWriter(nodeLine);
            }

            foreach (var node in nodes)
            {
                for (var i = 0; i < node.OutgoingDependencies.Count; i++)
                {
                    var child = node.OutgoingDependencies[i];
                    var start = node.Solution.Projects.Single(NotTestProject).OutputName;
                    var end = child.Solution.Projects.Single(NotTestProject).OutputName;
                    var edgeLine = string.Format(
                        CultureInfo.InvariantCulture,
                        "\t{{ data: {{ id: '{0}->{1}', source: '{0}', target: '{1}', directed: 'true'}} }}{2}",
                        start,
                        end,
                        i == outputs.Count - 1 ? string.Empty : ",");
                    reportWriter(edgeLine);
                }
            }

            reportWriter("\t]");
        }

        private static bool NotTestProject(Project project)
        {
            if (project.OutputName.Contains("UnitTests"))
            {
                return false;
            }

            if (project.OutputName.Contains("Tests"))
            {
                return false;
            }

            if (project.OutputName.Contains("UsageExample"))
            {
                return false;
            }

            if (project.OutputName.Contains("BasicApplication"))
            {
                return false;
            }

            return true;
        }
    }
}
