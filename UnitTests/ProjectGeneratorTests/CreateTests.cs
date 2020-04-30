namespace UnitTests.ProjectGeneratorTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO.Abstractions;
    using System.IO.Abstractions.TestingHelpers;
    using System.Linq;

    using DependenciesReader;
    using DependenciesReader.ProjectStructure;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using TestStack.BDDfy;

    [TestClass]
    public class CreateTests
    {
        private readonly MockFileSystem fileSystem = new MockFileSystem();

        private readonly Mock<IProjectDetailsReader> projectDetailsReader = new Mock<IProjectDetailsReader>();

        private readonly Collection<PackageReference> references = new Collection<PackageReference>();

        private string projectLocation;

        private string solutionLocation;

        private ProjectGenerator projectGenerator;

        private Project generatedProject;

        [TestInitialize]
        public void Initialize()
        {
            this.projectDetailsReader.Setup(pdr => pdr.GetPackages(It.IsAny<string>()))
                .Returns(this.references);
        }

        [TestMethod]
        public void ProjectWithoutDependenciesIsGeneratedCorrectly()
        {
            const string ProjectLocation = @"X:\Workspace\Solution\Project\Project.csproj";
            this.Given(t => t.ProjectLocationIs(ProjectLocation))
                .And(t => t.SolutionLocationIs(@"X:\Workspace\Solution"))
                .And(t => t.ProjectGeneratorIsCreated())
                .And(t => t.OutputNameIsSetupTo("Totally.Cool.Library"))
                .When(t => t.ProjectIsCreated())
                .Then(t => t.OutputNameIs("Totally.Cool.Library"))
                .And(t => t.ProjectPathIs("Project"))
                .And(t => t.ProjectNameIs("Project.csproj"))
                .BDDfy();
        }

        [TestMethod]
        public void ProjectDependenciesAreRetrieved()
        {
            const string ProjectLocation = @"X:\Workspace\Project\Solution\DataAccess1\DataAccess.csproj";
            this.Given(t => t.ProjectLocationIs(ProjectLocation))
                .And(t => t.SolutionLocationIs(@"X:\Workspace\Project\Solution"))
                .And(t => t.ProjectGeneratorIsCreated())
                .And(t => t.OutputNameIsSetupTo("Totally.Cool.Library"))
                .And(t => t.WithReference(new PackageReference("Some.Reference", "1.0.0")))
                .And(t => t.WithReference(new PackageReference("Other.Reference", "2.0.0")))
                .And(t => t.WithReference(new PackageReference("External.Reference", "3.0.0")))
                .When(t => t.ProjectIsCreated())
                .Then(t => t.OutputNameIs("Totally.Cool.Library"))
                .And(t => t.ProjectPathIs("DataAccess1"))
                .And(t => t.ProjectNameIs("DataAccess.csproj"))
                .And(t => t.ProjectDependenciesContain("some.reference", "1.0.0"))
                .And(t => t.ProjectDependenciesContain("other.reference", "2.0.0"))
                .And(t => t.ProjectDependenciesContain("external.reference", "3.0.0"))
                .BDDfy();
        }

        private void ProjectLocationIs(string value)
        {
            this.projectLocation = value;
        }

        private void SolutionLocationIs(string value)
        {
            this.solutionLocation = value;
        }

        private void ProjectGeneratorIsCreated()
        {
            this.projectGenerator = new ProjectGenerator(this.fileSystem, this.projectDetailsReader.Object);
        }

        private void ProjectIsCreated()
        {
            this.generatedProject = this.projectGenerator.CreateProject(this.projectLocation, this.solutionLocation);
        }

        private void OutputNameIsSetupTo(string value)
        {
            this.projectDetailsReader.Setup(pdr => pdr.GetOutputName(It.IsAny<string>()))
                .Returns(value);
        }

        private void ProjectPathIs(string value)
        {
            this.generatedProject.Directory.Should()
                .Be(value);
        }

        private void ProjectNameIs(string value)
        {
            this.generatedProject.FileName.Should()
                .Be(value);
        }

        private void OutputNameIs(string value)
        {
            this.generatedProject.OutputName.Should()
                .Be(value);
        }

        private void WithReference(PackageReference reference)
        {
            this.references.Add(reference);
        }

        private void ProjectDependenciesContain(string name, string version)
        {
            this.generatedProject.Dependencies.Any(
                    d => d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                         && d.Version.Equals(version, StringComparison.InvariantCultureIgnoreCase))
                .Should()
                .BeTrue();
        }
    }
}
