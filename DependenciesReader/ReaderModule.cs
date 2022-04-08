namespace DependenciesReader
{
    using System.IO.Abstractions;

    using Autofac;
    
    public class ReaderModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<FileSystemReader>()
                .AsImplementedInterfaces();
            builder.RegisterType<ProjectDetailsReader>()
                .AsImplementedInterfaces();
            builder.RegisterType<ProjectGenerator>()
                .AsImplementedInterfaces();
            builder.RegisterType<SolutionGenerator>()
                .AsImplementedInterfaces();
            builder.RegisterType<FileSystem>()
                .As<IFileSystem>();
            builder.RegisterType<DependencyStrategies.BuildDependenciesGraph.DependenciesGraphProvider>()
                .AsImplementedInterfaces();
            ReaderModule.RegisterDependencyStrategy<DependencyStrategies.DisplayPackagesStrategy>(builder, Activity.DisplayPackages);
            ReaderModule.RegisterDependencyStrategy<DependencyStrategies.SearchChildrenStrategy>(builder, Activity.SearchChildren);
            ReaderModule.RegisterDependencyStrategy<DependencyStrategies.BuildDependenciesGraphStrategy>(builder, Activity.BuildDependenciesGraph);
            ReaderModule.RegisterDependencyStrategy<DependencyStrategies.BuildDependenciesLayersStrategy>(builder, Activity.BuildDependenciesLayers);
            ReaderModule.RegisterDependencyStrategy<DependencyStrategies.ClusteringStrategy>(builder, Activity.Clustering);
        }

        private static void RegisterDependencyStrategy<TStrategy>(ContainerBuilder builder, Activity activity)
            where TStrategy : DependencyStrategies.IStrategy
        {
            builder.RegisterType<TStrategy>()
                .As<DependencyStrategies.IStrategy>()
                .Named<DependencyStrategies.IStrategy>(activity.ToString());
        }
    }
}
