namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using DependenciesReader.DependencyStrategies.BuildDependenciesGraph;
    using DependenciesReader.ProjectStructure;

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
            foreach (var node in nodes)
            {
                reportWriter(node.Solution.AbsolutePath(@"\"));
                foreach (var child in node.OutgoingDependencies)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "\t=> {0}",
                        child.Solution.AbsolutePath(@"\"));
                    reportWriter(message);
                }
            }
        }
    }
}
