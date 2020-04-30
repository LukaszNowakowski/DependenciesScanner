namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;

    using DependenciesReader.ProjectStructure;

    public interface IStrategy
    {
        void CreateReport(IList<Solution> projects, Action<string> reportWriter);
    }
}
