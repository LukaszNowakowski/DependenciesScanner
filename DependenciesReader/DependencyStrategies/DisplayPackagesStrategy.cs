namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using DependenciesReader.ProjectStructure;

    public class DisplayPackagesStrategy : IStrategy
    {
        public void CreateReport(IList<Solution> projects, Action<string> reportWriter)
        {
            var packages = projects.SelectMany(p => p.Dependencies)
                .Distinct(new Dependency.Comparer())
                .OrderBy(p => p.Name)
                .GroupBy(p => p.Name.ToLowerInvariant());
            foreach (var package in packages)
            {
                reportWriter(package.Key);
                var versions = package.Select(p => p.Version)
                    .Distinct()
                    .OrderBy(v => v);
                foreach (var version in versions)
                {
                    reportWriter(string.Format(CultureInfo.InvariantCulture, "\t{0}", version));
                }
            }
        }
    }
}
