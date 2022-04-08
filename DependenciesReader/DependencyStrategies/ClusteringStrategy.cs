namespace DependenciesReader.DependencyStrategies
{
    using DependenciesReader.DependencyStrategies.BuildDependenciesGraph;
    using DependenciesReader.ProjectStructure;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;

    public class ClusteringStrategy : IStrategy
    {
        private static readonly ReadOnlyDictionary<string, string> ClusterAssignments =
            new ReadOnlyDictionary<string, string>(CreateClusterAssignments());

        private readonly IDependenciesGraphProvider graphProvider;

        public ClusteringStrategy(IDependenciesGraphProvider graphProvider)
        {
            this.graphProvider = graphProvider;
        }

        public static string GetClusterName(string outputName)
        {
            if (!ClusterAssignments.ContainsKey(outputName))
            {
                return string.Empty;
            }

            return ClusterAssignments[outputName];
        }

        public static IEnumerable<string> GetAllClusters()
        {
            return ClusterAssignments.Values.AsEnumerable().Distinct();
        }

        public void CreateReport(IList<Solution> projects, Action<string> reportWriter)
        {
            var nodes = this.graphProvider.CreateGraph(projects);
            var clusterNames = ClusterAssignments.Values.AsEnumerable().Distinct();
            var clusters = new Collection<Cluster>();
            foreach (var clusterName in clusterNames)
            {
                var clusterPackages = ClusterAssignments
                    .Where(c => c.Value == clusterName)
                    .Select(c => c.Key);
                var solutions = clusterPackages
                    .Select(packageName => projects.SingleOrDefault(p => p.OutputNames.Any(on => on == packageName)))
                    .Where(s => s != null);
                clusters.Add(new Cluster(clusterName, solutions));
            }

            // Display clusters relations.
            var clusterComparer = new ClusterEqualityComparer();
            foreach (var source in clusters)
            {
                var targetClusters = this.FindTargetClusters(source, clusters)
                    .Distinct(clusterComparer);
                reportWriter($"{{ data: {{ id: '{source.Name}', label: '{source.Name}' }} }},");
                foreach (var target in targetClusters)
                {
                    reportWriter(
                        $"{{ data: {{ id: '{source.Name}->{target.Name}', source: '{source.Name}', target: '{target.Name}', arrow: 'triangle' }} }},");
                }
            }

            ////// Find two way cluster dependencies
            ////var clusterComparer = new ClusterEqualityComparer();
            ////foreach (var source in clusters)
            ////{
            ////    var targetClusters = this.FindTargetClusters(source, clusters)
            ////        .Distinct(clusterComparer);
            ////    var sourceDependencies = source.Solutions
            ////        .SelectMany(s => s.Dependencies)
            ////        .Select(d => d.Name)
            ////        .Distinct()
            ////        .ToList();
            ////    foreach (var target in targetClusters)
            ////    {
            ////        var targetOutputs = target.Solutions
            ////            .SelectMany(s => s.OutputNames)
            ////            .Distinct()
            ////            .ToList();
            ////        foreach (var currentOutput in targetOutputs)
            ////        {
            ////            if (sourceDependencies.Contains(currentOutput))
            ////            {
            ////                reportWriter("===========");
            ////                reportWriter($"Found back dependency in between clusters {source.Name} and {target.Name}.");
            ////                reportWriter($"Dependency name: {currentOutput}");
            ////                var sourceSolutions = source.Solutions
            ////                    .Where(s => s.Dependencies.Any(d => d.Name == currentOutput))
            ////                    .Select(s => s.FileName)
            ////                    .Distinct();
            ////                reportWriter("Source solutions:");
            ////                foreach (var sourceSolution in sourceSolutions)
            ////                {
            ////                    reportWriter($"    {sourceSolution}");
            ////                }

            ////                var targetSolutions = target.Solutions
            ////                    .Where(s => s.OutputNames.Contains(currentOutput))
            ////                    .Distinct();
            ////                foreach (var targetSolution in targetSolutions)
            ////                {
            ////                    reportWriter($"    {targetSolution}");
            ////                }

            ////                reportWriter("");
            ////            }
            ////        }
            ////    }
            ////}

            ////// Display dependencies between clusters.
            ////var testClusters = clusters.Where(c => c.Name == "WebClient" || c.Name == "WebHttp");
            ////var validDependencies = testClusters
            ////    .SelectMany(c => c.Solutions)
            ////    .SelectMany(s => s.OutputNames)
            ////    .ToList();
            ////foreach (var testCluster in testClusters)
            ////{
            ////    reportWriter($"{{ data: {{ id: '{testCluster.Name}', label: '{testCluster.Name}' }} }},");
            ////    foreach (var solution in testCluster.Solutions)
            ////    {
            ////        reportWriter($"{{ data: {{ id: '{solution.Projects.Single(NotTestProject).OutputName}', label: '{solution.Projects.Single(NotTestProject).OutputName}', parent: '{testCluster.Name}' }} }},");
            ////        var node = nodes.FirstOrDefault(n => n.Solution == solution);
            ////        if (node == null)
            ////        {
            ////            reportWriter($"Solution '{solution.FileName}' is missing");
            ////            continue;
            ////        }

            ////        for (int i = 0; i < node.OutgoingDependencies.Count; i++)
            ////        {
            ////            var child = node.OutgoingDependencies[i];
            ////            var start = node.Solution.Projects.Single(NotTestProject).OutputName;
            ////            var end = child.Solution.Projects.Single(NotTestProject).OutputName;
            ////            if (!validDependencies.Contains(start))
            ////            {
            ////                continue;
            ////            }

            ////            if (!validDependencies.Contains(end))
            ////            {
            ////                continue;
            ////            }

            ////            var edgeLine = string.Format(
            ////                CultureInfo.InvariantCulture,
            ////                "{{ data: {{ id: '{0}->{1}', source: '{0}', target: '{1}', arrow: 'triangle' }} }},",
            ////                start,
            ////                end);
            ////            reportWriter(edgeLine);
            ////        }
            ////    }
            ////}
        }

        private IEnumerable<Cluster> FindTargetClusters(Cluster clusterToCheck, IList<Cluster> allClusters)
        {
            var clusterToCheckOutputs = clusterToCheck.Solutions
                .SelectMany(s => s.OutputNames)
                .Distinct()
                .ToList();

            foreach (var currentCluster in allClusters)
            {
                if (currentCluster == clusterToCheck)
                {
                    continue;
                }

                var currentClusterDependencies = currentCluster.Solutions
                    .SelectMany(s => s.Dependencies)
                    .Select(d => d.Name)
                    .Distinct();

                foreach (var dependency in currentClusterDependencies)
                {
                    if (clusterToCheckOutputs.Contains(dependency))
                    {
                        yield return currentCluster;
                    }
                }
            }
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

        private static IDictionary<string, string> CreateClusterAssignments()
        {
            return new Dictionary<string, string>()
            {
                { "Agdf.Audit.Message", "Audit" },
                { "Agdf.Audit.Security", "Audit" },
                { "Agdf.Business.Validation", "Business" },
                { "Agdf.Domain.CarE.Resources", "Resources" },
                { "Agdf.Domain.CarE.Resources.Mappers", "Resources" },
                { "Agdf.Domain.DomainModel", "Resources" },
                { "Agdf.Domain.DomainValues", "Core" },
                { "Agdf.Domain.DomainValues.Audit", "Core" },
                { "Agdf.Domain.DomainValues.Auth", "Core" },
                { "Agdf.Domain.DomainValues.CommercialOffers", "Core" },
                { "Agdf.Domain.DomainValues.Documents", "Core" },
                { "Agdf.Domain.DomainValues.Finance", "Core" },
                { "Agdf.Domain.Enumerations", "Core" },
                { "Agdf.Domain.Enumerations.Taxonomy", "Core" },
                { "Agdf.Domain.HealthE.Resources", "Resources" },
                { "Agdf.Domain.HealthE.Resources.Mappers", "Resources" },
                { "Agdf.Domain.Mappings.ProductInterface", "Resources" },
                { "Agdf.Domain.MarketingAuto.Resources", "Resources" },
                { "Agdf.Domain.MarketingAuto.Resources.Mappers", "Resources" },
                { "Agdf.Domain.Resources", "Resources" },
                { "Agdf.Domain.Resources.Mappers", "Resources" },
                { "Agdf.DomainModel.Mapping", "Resources" },
                { "Agdf.DomainValues.Logging", "Core" },
                { "Agdf.DomainValues.ProductInterface", "Core" },
                { "Agdf.DomainValues.Rating", "Core" },
                { "Agdf.DomainValues.Taxonomy", "Core" },
                { "Agdf.Extensions", "Core" },
                { "Agdf.IO", "IO" },
                { "Agdf.IO.Storage", "IO" },
                { "Agdf.IO.Storage.StructureMap", "IO" },
                { "Agdf.Jobs", "Business" },
                { "Agdf.Lookups", "Business" },
                { "Agdf.Taxonomy", "Core" },
                { "Agdf.Taxonomy.FluentConfiguration", "Core" },
                { "Agdf.Web.Client", "WebClient" },
                { "Agdf.Web.Client.Auditing", "WebClient" },
                { "Agdf.Web.Client.Configuration", "WebClient" },
                { "Agdf.Web.Client.ETag", "WebClient" },
                { "Agdf.Web.Http", "WebHttp" },
                { "Agdf.Web.Http.Audit", "Core" },
                { "Agdf.Web.Http.Auth.Tokens", "WebHttp" },
                { "Agdf.Web.Http.Auth.Tokens.Configuration", "WebHttp" },
                { "Agdf.Web.Http.Diagnostic", "WebHttp" },
                { "Agdf.Web.Http.FireAndForget", "WebHttp" },
                { "Agdf.Web.Http.FireAndForget.StructureMap", "WebHttp" },
                { "Agdf.Web.Http.Hypermedia", "WebHttp" },
                { "Agdf.Web.Http.Logging", "WebHttp" },
                { "Agdf.Web.Http.MultiTenancy", "Core" },
                { "Agdf.Web.Http.Routing", "WebHttp" },
                { "Agdf.Web.Http.Serialization", "WebHttp" },
                { "Agdf.Web.Http.Setup", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Audit", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Auth.Tokens", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Container", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Documentation", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.DomainMappings", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Errors", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Filters", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.FluentRouting", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.FluentValidation", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Logging", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.ProductInterfaceMapping", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Routes", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Serialization", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.Swagger", "WebHttpSetup" },
                { "Agdf.Web.Http.Setup.WebClientsRegistration", "WebHttpSetup" },
                { "Agdf.Web.Http.StructureMap", "WebHttp" },
                { "Agdf.Web.Http.Swagger", "WebHttp" },
                { "Agdf.Web.Http.TestHelpers", "WebHttp" },
                { "Agdf.Web.Http.Validation", "WebHttp" },
                { "Agdf.Web.Http.Validation.DomainModel", "WebHttp" },
                { "Agdf.Web.Http.Versioning", "WebHttp" },
                { "Axa.Darwin.Business.Validation", "Business" },
                { "Axa.Darwin.Configuration", "Core" },
                { "Axa.Darwin.Core", "Core" },
                { "Axa.Darwin.Data", "Core" },
                { "Axa.Darwin.Data.FailureHandling", "Core" },
                { "Axa.Darwin.DomainModel", "Resources" },
                { "Axa.Darwin.DomainModel.Mapping", "Resources" },
                { "Axa.Darwin.DomainValues", "Core" },
                { "Axa.Darwin.DomainValues.Audit", "Core" },
                { "Axa.Darwin.DomainValues.Serialization", "Core" },
                { "Axa.Darwin.IO.Storage", "IO" },
                { "Axa.Darwin.IO.Storage.Azure", "IO" },
                { "Axa.Darwin.IO.Storage.Network", "IO" },
                { "Axa.Darwin.IO.Storage.TestHelpers", "IO" },
                { "Axa.Darwin.Logging", "Core" },
                { "Axa.Darwin.Logging.Sql", "Core" },
                { "Axa.Darwin.Lookups", "Core" },
                { "Axa.Darwin.ProductInterface", "Core" },
                { "Axa.Darwin.ProductInterface.DomainMapper.StructureMap", "Resources" },
                { "Axa.Darwin.ProductInterface.FluentAssertions", "Resources" },
                { "Axa.Darwin.ProductInterface.Protobuf", "Resources" },
                { "Axa.Darwin.ProductInterface.Serialization", "Resources" },
                { "Axa.Darwin.Protobuf", "Core" },
                { "Axa.Darwin.Quotes", "Business" },
                { "Axa.Darwin.Quotes.StructureMap", "Business" },
                { "Axa.Darwin.Rating", "Rating" },
                { "Axa.Darwin.Rating.ConditionEvaluation", "Rating" },
                { "Axa.Darwin.Rating.StructureMap", "Rating" },
                { "Axa.Darwin.Rating.Tracing", "Rating" },
                { "Axa.Darwin.RatingEngine", "Rating" },
                { "Axa.Darwin.ServiceModel", "Core" },
                { "Axa.Darwin.ServiceModel.Host.StructureMap", "Core" },
                { "Axa.Darwin.StateMachine", "Core" },
                { "Axa.Darwin.StructureMap", "Core" },
                { "Axa.Darwin.StructureMap.FluentValidation", "Core" },
                { "Axa.Darwin.TestStack", "Core" },
                { "Axa.Darwin.Web.Client", "WebClient" },
                { "Axa.Darwin.Web.Client.Auditing", "WebClient" },
                { "Axa.Darwin.Web.Client.Retry", "WebClient" },
                { "Axa.Darwin.Web.Client.StructureMap", "WebClient" },
                { "Axa.Darwin.Web.Client.Testing", "WebClient" },
                { "Axa.Darwin.Web.Http.AppMonitor", "Business" },
                { "Axa.Darwin.Web.Http.AppMonitor.Host.Owin", "Business" },
                { "Axa.Darwin.Web.Http.StructureMap", "WebHttpSetup" },
                { "Axa.Darwin.Web.Mvc", "Core" },
                { "Axa.Darwin.WindowsService", "Business" },
                { "Axa.Darwin.Xml", "Core" },
            };
        }

        private class Cluster
        {
            public Cluster(
                string name,
                IEnumerable<Solution> solutions)
            {
                this.Name = name;
                this.Solutions = new ReadOnlyCollection<Solution>((solutions ?? Enumerable.Empty<Solution>()).ToList());
            }

            public string Name { get; }

            public ReadOnlyCollection<Solution> Solutions { get; }
        }

        private class ClusterEqualityComparer : IEqualityComparer<Cluster>
        {
            public bool Equals(Cluster x, Cluster y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(Cluster obj)
            {
                return (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }
    }
}
