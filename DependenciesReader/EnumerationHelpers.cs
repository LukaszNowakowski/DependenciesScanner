namespace DependenciesReader
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class EnumerationHelpers
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items)
        {
            return new ReadOnlyCollection<T>((items ?? Enumerable.Empty<T>()).ToList());
        }
    }
}
