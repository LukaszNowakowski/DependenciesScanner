namespace DependenciesReader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    internal static class Program
    {
        private const string DefaultDirectory = @"C:\AzureDevOpsWorkspaces\Packages";

        private static readonly IFileSystem FileSystem = new FileSystem();

        private static readonly IProjectDetailsReader ProjectDetailsReader = new ProjectDetailsReader(FileSystem);

        private static readonly IFileSystemReader FileSystemReader = new FileSystemReader(FileSystem, ProjectDetailsReader);

        private static readonly IPackageReader PackageReader = new PackageReader();

        private static readonly Dictionary<Activity, DependencyStrategies.IStrategy> DependencyStrategy = new Dictionary<Activity, DependencyStrategies.IStrategy>()
                                                                                                              {
                                                                                                                  { Activity.SearchChildren, new DependencyStrategies.SearchChildrenStrategy() },
                                                                                                                  { Activity.DisplayPackages, new DependencyStrategies.DisplayPackagesStrategy() }
                                                                                                              };

        private static string rootDirectory = string.Empty;

        private static void Main()
        {
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
                if (DependencyStrategy.ContainsKey(activity))
                {
                    var strategy = DependencyStrategy[activity];
                    var projects = GetProjects(rootDirectory).ToList();
                    strategy.CreateReport(projects, Console.WriteLine);
                }
            }
            while (activity != Activity.Exit);
        }

        private static IEnumerable<Location> GetProjects(string rootDirectory)
        {
            var files = FileSystemReader.GetPackages(rootDirectory);
            var projects = new List<Location>();
            foreach (var file in files)
            {
                var project = PackageReader.GetPackages(file);
                yield return project;
            }
        }

        private static Activity SelectActivity()
        {
            var items = new Dictionary<Activity, string>()
                            {
                                { Activity.Exit, "[1] - Exit" },
                                { Activity.SearchChildren, "[2] - Search for projects using reference" },
                                { Activity.DisplayPackages, "[3] - Display list of packages and versions" }
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
            var result = -1;
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
    }
}
