namespace DependenciesReader.DependencyStrategies
{
    using System;
    using System.Collections.Generic;

    public interface IStrategy
    {
        void CreateReport(IList<Location> projects, Action<string> reportWriter);
    }
}
