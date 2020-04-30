namespace DependenciesReader
{
    using System;
    using System.Collections.Generic;

    public class PackageEqualityComparer : IEqualityComparer<PackageReference>
    {
        public bool Equals(PackageReference x, PackageReference y)
        {
            return string.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase)
                   && string.Equals(x.Version, y.Version, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(PackageReference obj)
        {
            return obj.Name.ToLowerInvariant().GetHashCode();
        }
    }
}
