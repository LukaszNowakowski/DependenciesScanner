namespace DependenciesReader
{
    using System;
    using System.IO.Abstractions;

    public static class PathHelpers
    {
        public static string MakeRelativePath(this IFileSystem fileSystem, string @from, string to)
        {
            var baseUri = new Uri(@from.TrimEnd('\\') + "\\");
            var solutionUri = new Uri(to.TrimEnd('\\') + "\\");
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(solutionUri)
                    .ToString()
                    .Replace('/', fileSystem.Path.DirectorySeparatorChar))
                .TrimEnd('\\');
        }
    }
}
