namespace UnitTests.FileSystemReaderTests
{
    using System.IO.Abstractions.TestingHelpers;
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GetSolutionsTests : FileSystemReaderTests
    {
        [TestMethod]
        public void FindsCorrectSolution()
        {
            this.FileSystem.AddFile(@"X:\WorkspaceRoot\Solution1\Solution.sln", new MockFileData("SolutionContents"));
            var result = this.FileSystemReader.GetSolutions(@"X:\WorkspaceRoot");
            result.Should()
                .NotBeNull()
                .And.HaveCount(1);
            var solution = result.First();
            solution.Should()
                .NotBeNull();
            solution.Directory.Should()
                .Be("Solution1");
            solution.FileName.Should()
                .Be("Solution.sln");
            solution.Projects.Should()
                .BeEmpty();
        }

        [TestMethod]
        public void FindsNestedSolution()
        {
            this.FileSystem.AddFile(@"X:\WorkspaceRoot\Modules\Security\Authentication\IdentityServer.sln", new MockFileData("SolutionContents"));
            var result = this.FileSystemReader.GetSolutions(@"X:\WorkspaceRoot");
            result.Should()
                .NotBeNull()
                .And.HaveCount(1);
            var solution = result.First();
            solution.Should()
                .NotBeNull();
            solution.Directory.Should()
                .Be(@"Modules\Security\Authentication");
            solution.FileName.Should()
                .Be("IdentityServer.sln");
            solution.Projects.Should()
                .BeEmpty();
        }

        [TestMethod]
        public void FindsMultipleSolutions()
        {
            this.FileSystem.AddFile(@"X:\Projects\Security\IdentityServer\IdentityServer.sln", new MockFileData("SolutionContents"));
            this.FileSystem.AddFile(@"X:\Projects\Users\Api\UserData\UserData.sln", new MockFileData("SolutionContents"));
            var result = this.FileSystemReader.GetSolutions(@"X:\Projects");
            result.Should()
                .NotBeNull()
                .And.HaveCount(2);
            var identityServer = result.ElementAt(0);
            identityServer.Should()
                .NotBeNull();
            identityServer.Directory.Should()
                .Be(@"Security\IdentityServer");
            identityServer.FileName.Should()
                .Be("IdentityServer.sln");
            var userData = result.ElementAt(1);
            userData.Should()
                .NotBeNull();
            userData.Directory.Should()
                .Be(@"Users\Api\UserData");
            userData.FileName.Should()
                .Be("UserData.sln");
        }

        [TestMethod]
        public void FindsSingleSolutionProject()
        {
            this.FileSystem.AddFile(@"X:\WorkspaceRoot\Solution1\Solution.sln", new MockFileData("SolutionContents"));
            this.FileSystem.AddFile(
                @"X:\WorkspaceRoot\Solution1\DataAccess\DataAccess.csproj",
                new MockFileData("<Project />"));
            var result = this.FileSystemReader.GetSolutions(@"X:\WorkspaceRoot");
            result.Should()
                .NotBeNull()
                .And.HaveCount(1);
            var solution = result.First();
            solution.Should()
                .NotBeNull();
            solution.Directory.Should()
                .Be("Solution1");
            solution.FileName.Should()
                .Be("Solution.sln");
            solution.Projects.Should()
                .HaveCount(1);
            var dataAccess = solution.Projects[0];
            dataAccess.Should()
                .NotBeNull();
            dataAccess.Directory.Should()
                .Be("DataAccess");
            dataAccess.FileName.Should()
                .Be("DataAccess.csproj");
        }
    }
}
