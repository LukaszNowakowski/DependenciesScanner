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
            reportWriter("elements: {");
            reportWriter("\tnodes: [");
            var outputs = nodes.ToList();
            for (int i = 0; i < outputs.Count; i++)
            {
                var node = outputs[i];
                var nodeLine = string.Format(
                    CultureInfo.InvariantCulture,
                    "\t\t{{ data:  {{ id: '{0}', weight: {1} }} }}{2}",
                    node.Solution.AbsolutePath(@"\"),
                    node.OutgoingDependencies.Count + 1,
                    i == outputs.Count - 1 ? string.Empty : ",");
                reportWriter(nodeLine);
            }

            reportWriter("\t],");
            reportWriter("\tedges: [");
            foreach (var node in nodes)
            {
                for (int i = 0; i < node.OutgoingDependencies.Count; i++)
                {
                    var child = node.OutgoingDependencies[i];
                    var start = node.Solution.AbsolutePath(@"\");
                    var end = child.Solution.AbsolutePath(@"\");
                    var edgeLine = string.Format(
                        CultureInfo.InvariantCulture,
                        "\t\t{{ data: {{ source: '{0}', target: '{1}', directed: 'true'}} }}{2}",
                        start,
                        end,
                        i == outputs.Count - 1 ? string.Empty : ",");
                    reportWriter(edgeLine);
                }
            }

            reportWriter("\t]");
            reportWriter("}");
        }
    }
}
