namespace DependenciesReader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class EnumerationHelpers
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items)
        {
            return new ReadOnlyCollection<T>((items ?? Enumerable.Empty<T>()).ToList());
        }

        public static void AddRange<T>(this Collection<T> items, params T[] newItems)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (newItems == null)
            {
                return;
            }

            foreach (var item in newItems)
            {
                items.Add(item);
            }
        }
    }
}
