namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using DependenciesReader.ProjectStructure;

    public class SearchChildrenStrategy : IStrategy
    {
        public void CreateReport(IList<Solution> projects, Action<string> reportWriter)
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

        private IEnumerable<string> FindData(IList<Solution> projects, string dependencyName, string versionNumber)
        {
            var result = projects.Where(
                proj => proj.Dependencies.Any(
                    pack => pack.Name.Equals(dependencyName, StringComparison.InvariantCultureIgnoreCase)
                            && (string.IsNullOrEmpty(versionNumber)
                                || pack.Version.Equals(versionNumber, StringComparison.InvariantCultureIgnoreCase))));
            foreach (var location in result)
            {
                yield return location.Directory;
                foreach (var package in location.Dependencies.Where(p => p.Name.Equals(dependencyName, StringComparison.InvariantCultureIgnoreCase)))
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
