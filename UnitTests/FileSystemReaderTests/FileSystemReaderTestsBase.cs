namespace UnitTests.FileSystemReaderTests
{
    using System.IO.Abstractions.TestingHelpers;

    using DependenciesReader;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class FileSystemReaderTests
    {
        protected FileSystemReader FileSystemReader { get; private set; }

        protected MockFileSystem FileSystem { get; } = new MockFileSystem();

        protected ProjectDetailsReader DetailsReader { get; private set; }

        [TestInitialize]
        public virtual void Initialize()
        {
            this.DetailsReader = new ProjectDetailsReader(this.FileSystem);
            this.FileSystemReader = new FileSystemReader(this.FileSystem, this.DetailsReader);
        }
    }
}
