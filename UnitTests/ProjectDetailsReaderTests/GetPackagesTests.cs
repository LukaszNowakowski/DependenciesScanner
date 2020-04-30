namespace UnitTests.ProjectDetailsReaderTests
{
    using System;
    using System.IO.Abstractions.TestingHelpers;
    using System.Linq;

    using DependenciesReader;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TestStack.BDDfy;

    [TestClass]
    public class GetPackagesTests
    {
        private MockFileSystem fileSystem = new MockFileSystem();

        private ProjectDetailsReader reader;

        private string inputPath;

        private PackageReference[] foundReferences;

        [TestMethod]
        public void PackagesAreRetrievedFromExistingFile()
        {
            this.Given(t => t.InputPathIs(@"X:\Workspace\Solution\Project\Project.csproj"))
                .And(t => t.WithPackageFile(@"X:\Workspace\Solution\Project\project.json", Resources.PackagesFile))
                .And(t => t.ReaderIsCreated())
                .When(t => t.PackagesAreRetrieved())
                .Then(t => t.PackageWasFound("Reference1.Data", "1.0.0"))
                .And(t => t.PackageWasFound("Reference2.Logic", "2.0.0"))
                .And(t => t.PackageWasFound("Reference3.Service", "3.0.0"))
                .BDDfy();
        }

        [TestMethod]
        public void EmptyCollectionIsRetrievedWhenFileNotExists()
        {
            this.Given(t => t.InputPathIs(@"X:\Workspace\Solution\Project\Project.csproj"))
                .And(t => t.ReaderIsCreated())
                .When(t => t.PackagesAreRetrieved())
                .Then(t => t.NoPackagesFound())
                .BDDfy();
        }

        private void InputPathIs(string value)
        {
            this.inputPath = value;
        }

        private void WithPackageFile(string path, string contents)
        {
            this.fileSystem.AddFile(path, contents);
        }

        private void ReaderIsCreated()
        {
            this.reader = new ProjectDetailsReader(this.fileSystem);
        }

        private void PackagesAreRetrieved()
        {
            this.foundReferences = this.reader.GetPackages(this.inputPath)
                .ToArray();
        }

        private void PackageWasFound(string name, string version)
        {
            this.foundReferences.Any(
                    p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                         && p.Version.Equals(version, StringComparison.InvariantCultureIgnoreCase))
                .Should()
                .BeTrue();
        }

        private void NoPackagesFound()
        {
            this.foundReferences.Should()
                .BeEmpty();
        }
    }
}
