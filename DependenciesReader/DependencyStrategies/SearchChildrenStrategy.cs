namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class SearchChildrenStrategy : IStrategy
    {
        public void CreateReport(IList<Location> projects, Action<string> reportWriter)
        {
            Console.Write("Dependency name: ");
            var dependencyName = Console.ReadLine();
            Console.Write("Version number (or empty for all versions): ");
            var versionNumber = Console.ReadLine();
            foreach (var entry in this.FindData(projects, dependencyName, versionNumber))
            {
                reportWriter(entry);
            }
        }

        private IEnumerable<string> FindData(IList<Location> projects, string dependencyName, string versionNumber)
        {
            var result = projects.Where(
                proj => proj.Packages.Any(
                    pack => pack.Name.Equals(dependencyName, StringComparison.InvariantCultureIgnoreCase)
                            && (string.IsNullOrEmpty(versionNumber)
                                || pack.Version.Equals(versionNumber, StringComparison.InvariantCultureIgnoreCase))));
            foreach (var location in result)
            {
                yield return location.Path;
                foreach (var package in location.Packages.Where(p => p.Name.Equals(dependencyName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    yield return string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - {1}",
                        package.Name,
                        package.Version);
                }
            }
        }
    }
}
