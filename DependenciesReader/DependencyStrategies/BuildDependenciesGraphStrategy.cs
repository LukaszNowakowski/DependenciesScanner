namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;

    using DependenciesReader.ProjectStructure;

    public class BuildDependenciesGraphStrategy : IStrategy
    {
        public void CreateReport(IList<Solution> projects, Action<string> reportWriter)
        {
            throw new NotImplementedException();
        }
    }
}
