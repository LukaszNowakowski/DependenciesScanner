namespace UnitTests.ProjectDetailsReaderTests
{
    using System.IO.Abstractions.TestingHelpers;

    using DependenciesReader;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GetOutputNameTests
    {
        private const string ProjectFileContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{B2A76D41-E1C6-4F9A-B064-AF428A88E97F}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>Agdf.Audit.Security</RootNamespace>
    <AssemblyName>ASSEMBLY_NAME</AssemblyName>
    <TargetFrameworkVersion>v4.6.1</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <Deterministic>true</Deterministic>
    <SccProjectName>SAK</SccProjectName>
    <SccLocalPath>SAK</SccLocalPath>
    <SccAuxPath>SAK</SccAuxPath>
    <SccProvider>SAK</SccProvider>
  </PropertyGroup>
  <PropertyGroup>
    <BranchRootPath>..\</BranchRootPath>
    <BuildTargets>$(BranchRootPath)Sandpiper.Build</BuildTargets>
    <OutputPath>bin\$(Configuration)\</OutputPath>
    <DebugSymbols>true</DebugSymbols>
    <NoWarn>7035</NoWarn>
  </PropertyGroup>
</Project>";

        private MockFileSystem fileSystem = new MockFileSystem();

        private ProjectDetailsReader reader;

        [TestInitialize]
        public void Setup()
        {
            this.reader = new ProjectDetailsReader(this.fileSystem);
        }

        [TestMethod]
        public void ReadsOutputNameWhenItsInProjectFile()
        {
            this.fileSystem.AddFile(@"X:\Solution\Project\Project.csproj", new MockFileData(ProjectFileContents));
            var outputName = this.reader.GetOutputName(@"X:\Solution\Project\Project.csproj");
            outputName.Should()
                .Be("ASSEMBLY_NAME");
        }
    }
}
