namespace DependenciesReader.ProjectStructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Dependency
    {
        public Dependency(string name, string version)
        {
            this.Name = name;
            this.Version = version;
        }

        public string Name { get; }

        public string Version { get; }

        public class Comparer : IEqualityComparer<Dependency>
        {
            public bool Equals(Dependency x, Dependency y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase)
                       && x.Version.Equals(y.Version, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode(Dependency obj)
            {
                const int HashingBase = (int)342974287;

                int hash = HashingBase;
                hash ^= !object.ReferenceEquals(null, obj.Name) ? obj.Name.ToLowerInvariant().GetHashCode() : 0;
                hash ^= !object.ReferenceEquals(null, obj.Version) ? obj.Version.ToLowerInvariant().GetHashCode() : 0;
                return hash;
            }
        }
    }
}
