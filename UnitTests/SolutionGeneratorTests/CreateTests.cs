namespace UnitTests.SolutionGeneratorTests
{
    using System.Collections.Generic;
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

        private readonly Mock<IProjectGenerator> projectGenerator = new Mock<IProjectGenerator>();

        private readonly Dictionary<string, Project> projects = new Dictionary<string, Project>();

        private string fullPath;

        private string rootDirectory;

        private SolutionGenerator generator;

        private Solution createdSolution;

        [TestInitialize]
        public void Initialize()
        {
            this.projectGenerator.Setup(pg => pg.CreateProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string location, string solutionLocation) => this.projects[location]);
        }

        [TestMethod]
        public void SolutionWithoutProjectsIsFound()
        {
            this.Given(t => t.FullPathIs(@"X:\Workspace\Library\Library.sln"))
                .And(t => t.RootDirectoryIs(@"X:\Workspace"))
                .And(t => t.GeneratorIsCreated())
                .And(t => t.FileSystemHasSolutionFileAt(@"X:\Workspace\Library\Library.sln"))
                .When(t => t.SolutionIsCreated())
                .Then(t => t.SolutionPathIs("Library"))
                .And(t => t.SolutionNameIs("Library.sln"))
                .BDDfy();
        }

        [TestMethod]
        public void SolutionHasProjectsFromFileSystem()
        {
            this.Given(t => t.FullPathIs(@"X:\Workspace\Library\Library.sln"))
                .And(t => t.RootDirectoryIs(@"X:\Workspace"))
                .And(t => t.GeneratorIsCreated())
                .And(t => t.FileSystemHasSolutionFileAt(@"X:\Workspace\Library\Library.sln"))
                .And(
                    t => t.WithProjectAt(
                        @"X:\Workspace\Library\DataAccess\DataAccess.csproj",
                        new Project("a", "b", "c", Enumerable.Empty<Dependency>())))
                .And(
                    t => t.WithProjectAt(
                        @"X:\Workspace\Library\Dto\Dto.csproj",
                        new Project("a", "b", "c", Enumerable.Empty<Dependency>())))
                .When(t => t.SolutionIsCreated())
                .Then(t => t.SolutionPathIs("Library"))
                .And(t => t.SolutionNameIs("Library.sln"))
                .And(t => t.ProjectsCollectionIs(this.projects.Values.ToArray()))
                .BDDfy();
        }

        private void FullPathIs(string value)
        {
            this.fullPath = value;
        }

        private void RootDirectoryIs(string value)
        {
            this.rootDirectory = value;
        }

        private void GeneratorIsCreated()
        {
            this.generator = new SolutionGenerator(this.fileSystem, this.projectGenerator.Object);
        }

        private void SolutionIsCreated()
        {
            this.createdSolution = this.generator.Create(this.rootDirectory, this.fullPath);
        }

        private void SolutionPathIs(string value)
        {
            this.createdSolution.Directory.Should()
                .Be(value);
        }

        private void SolutionNameIs(string value)
        {
            this.createdSolution.FileName.Should()
                .Be(value);
        }

        private void ProjectsCollectionIs(params Project[] projects)
        {
            this.createdSolution.Projects.Should()
                .BeEquivalentTo(projects.OfType<object>());
        }

        private void FileSystemHasSolutionFileAt(string fullPath)
        {
            this.fileSystem.AddFile(fullPath, new MockFileData("Solution file contents"));
        }

        private void WithProjectAt(string path, Project project)
        {
            this.projects.Add(path, project);
            this.fileSystem.AddFile(path, new MockFileData("Project file contents"));
        }
    }
}
