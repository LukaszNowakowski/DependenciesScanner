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
            builder.RegisterType<PackageReader>()
                .AsImplementedInterfaces();
            builder.RegisterType<ProjectGenerator>()
                .AsImplementedInterfaces();
            builder.RegisterType<SolutionGenerator>()
                .AsImplementedInterfaces();
            builder.RegisterType<FileSystem>()
                .As<IFileSystem>();
        }
    }
}
