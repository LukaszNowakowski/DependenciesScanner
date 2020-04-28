namespace UnitTests.FileSystemReaderTests
{
    using System.IO.Abstractions.TestingHelpers;

    using DependenciesReader;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class FileSystemReaderTests
    {
        protected FileSystemReader FileSystemReader { get; private set; }

        protected MockFileSystem FileSystem { get; } = new MockFileSystem();

        [TestInitialize]
        public virtual void Initialize()
        {
            this.FileSystemReader = new FileSystemReader(this.FileSystem);
        }
    }
}
