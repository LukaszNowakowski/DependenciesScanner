namespace DependenciesReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Autofac;

    using DependenciesReader.DependencyStrategies;
    using DependenciesReader.ProjectStructure;

    internal static class Program
    {
        private const string DefaultDirectory = @"C:\AzureDevOpsWorkspaces\Packages";
        
        private static IContainer container;

        private static string rootDirectory = string.Empty;

        private static void Main()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ReaderModule>();
            container = containerBuilder.Build();
            Console.Write("Directory path ('{0}' if empty): ", DefaultDirectory);
            rootDirectory = Console.ReadLine();
            if (string.IsNullOrEmpty(rootDirectory))
            {
                rootDirectory = DefaultDirectory;
            }

            Activity activity;
            do
            {
                activity = SelectActivity();
                var strategy = GetStrategy(activity);
                if (strategy != null)
                {
                    var projects = GetProjects(rootDirectory)
                        .ToList();
                    strategy.CreateReport(projects, Console.WriteLine);
                }
            }
            while (activity != Activity.Exit);
        }

        private static IEnumerable<Solution> GetProjects(string rootDirectory)
        {
            var fileSystemReader = container.Resolve<IFileSystemReader>();
            return fileSystemReader.GetSolutions(rootDirectory);
        }

        private static Activity SelectActivity()
        {
            var items = new Dictionary<Activity, string>()
                            {
                                { Activity.Exit, "[1] - Exit" },
                                { Activity.SearchChildren, "[2] - Search for projects using reference" },
                                { Activity.DisplayPackages, "[3] - Display list of packages and versions" },
                                { Activity.BuildDependenciesGraph, "[4] - Build dependencies graph" },
                                { Activity.BuildDependenciesLayers, "[5] - Build dependencies layers" }
                            };

            foreach (var item in items)
            {
                Console.WriteLine(item.Value);
            }

            var selection = ReadSelection("Select action: ");
            return (Activity)selection;
        }

        private static int ReadSelection(string prompt)
        {
            int result;
            do
            {
                Console.Write(prompt);
                var entry = Console.ReadLine();
                if (!int.TryParse(entry, out result))
                {
                    result = -1;
                }
            }
            while (result <= 0);

            return result;
        }

        private static IStrategy GetStrategy(Activity activity)
        {
            if (container.TryResolveNamed(activity.ToString(), typeof(IStrategy), out object strategy))
            {
                return (IStrategy)strategy;
            }

            return null;
        }
    }
}
