namespace UnitTests.ProjectDetailsReaderTests
{
    using System;
    using System.IO;
    using System.IO.Abstractions.TestingHelpers;

    using DependenciesReader;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TestStack.BDDfy;

    [TestClass]
    public class GetOutputNameTests
    {
        private MockFileSystem fileSystem = new MockFileSystem();

        private ProjectDetailsReader reader;

        private string projectPath;

        private string outputName;

        private Action retrievalAction;

        [TestMethod]
        public void ReadsOutputNameWhenItsInProjectFile()
        {
            const string FilePath = @"X:\Solution\Project\Project.csproj";
            this.Given(t => t.ProjectFileAt(FilePath, Resources.ProjectFileWithAssemblyName))
                .And(t => t.ReaderIsCreated())
                .And(t => t.InputPathIs(@"X:\Solution\Project\Project.csproj"))
                .When(t => t.OutputNameIsRetrieved())
                .Then(t => t.OutputNameIs("ASSEMBLY_NAME"))
                .BDDfy();
        }

        [TestMethod]
        public void ReadsProjectFileNameWhenItsInProjectFile()
        {
            const string FilePath = @"X:\Solution\Project\Project.csproj";
            this.Given(t => t.ProjectFileAt(FilePath, Resources.ProjectFileWithoutAssemblyName))
                .And(t => t.ReaderIsCreated())
                .And(t => t.InputPathIs(@"X:\Solution\Project\Project.csproj"))
                .When(t => t.OutputNameIsRetrieved())
                .Then(t => t.OutputNameIs("Project"))
                .BDDfy();
        }

        [TestMethod]
        public void ReadsProjectFileNameWhenItIsDotNetStandardProject()
        {
            const string FilePath = @"X:\Solution\Project\DotNetStandard.csproj";
            this.Given(t => t.ProjectFileAt(FilePath, Resources.ProjectFileInDotNetStandard))
                .And(t => t.ReaderIsCreated())
                .And(t => t.InputPathIs(@"X:\Solution\Project\DotNetStandard.csproj"))
                .When(t => t.OutputNameIsRetrieved())
                .Then(t => t.OutputNameIs("DotNetStandard"))
                .BDDfy();
        }

        [TestMethod]
        public void ThrowsExceptionWhenProjectFileDoesNotExist()
        {
            const string FilePath = @"X:\Solution\Project\Project.csproj";
            this.Given(t => t.ProjectFileAt(FilePath, Resources.ProjectFileWithAssemblyName))
                .And(t => t.ReaderIsCreated())
                .And(t => t.InputPathIs(@"X:\Solution\Project\Project1.csproj"))
                .When(t => t.OutputNameIsRetrievedWithExceptionCheck())
                .Then(t => t.ExceptionShouldBeThrown<FileNotFoundException>(e => e.FileName.Equals(@"X:\Solution\Project\Project1.csproj")))
                .BDDfy();
        }

        private void ProjectFileAt(string path, string fileContents)
        {
            this.fileSystem.AddFile(path, new MockFileData(fileContents));
        }

        private void ReaderIsCreated()
        {
            this.reader = new ProjectDetailsReader(this.fileSystem);
        }

        private void InputPathIs(string value)
        {
            this.projectPath = value;
        }

        private void OutputNameIsRetrieved()
        {
            this.outputName = this.reader.GetOutputName(this.projectPath);
        }

        private void OutputNameIsRetrievedWithExceptionCheck()
        {
            this.retrievalAction = () => this.outputName = this.reader.GetOutputName(this.projectPath);
        }

        private void ExceptionShouldBeThrown<TException>(Func<TException, bool> exceptionValidator)
            where TException : Exception
        {
            this.retrievalAction.Should()
                .Throw<TException>()
                .And.Should()
                .Match(e => exceptionValidator((TException)e));
        }

        private void OutputNameIs(string value)
        {
            this.outputName.Should()
                .Be(value);
        }
    }
}
