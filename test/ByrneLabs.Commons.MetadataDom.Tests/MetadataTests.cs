using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ByrneLabs.Commons.MetadataDom.Tests.Checker;
using Xunit;
using Xunit.Abstractions;

namespace ByrneLabs.Commons.MetadataDom.Tests
{
    public class MetadataTests
    {
        public MetadataTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;
        private static readonly string[] LoadableFileExtensions = { "exe", "dll", "pdb", "mod", "obj", "wmd" };

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        public static void AssertHasMetadata(Metadata metadata)
        {
        }

        private void CheckMetadata(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checkState = BaseChecker.CheckOnlyMetadata(assemblyFile, pdbFile);
            _output.WriteLine(checkState.ErrorLogText);
            Assert.True(checkState.Success);
            if (assemblyFile != null && ".dll".Equals(assemblyFile.Extension))
            {
                Assert.NotNull(checkState.Metadata.AssemblyDefinition);
                Assert.True(checkState.Metadata.TypeDefinitions.Any());
            }
            if (pdbFile != null)
            {
                Assert.True(checkState.Metadata.Documents.Any());
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnAssembly() => CheckMetadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnAssemblyAndPdb() => CheckMetadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnPdb() => CheckMetadata(null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnPrebuiltAssemblyResources()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var loadableFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles.Where(file => !file.Name.Equals("EmptyType.dll")))
            {
                CheckMetadata(loadableFile);
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnPrebuiltResources()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var loadableFiles = resourceDirectory.GetFiles("*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles)
            {
                CheckMetadata(loadableFile.Extension.Equals(".pdb") ? null : loadableFile, loadableFile.Extension.Equals(".pdb") ? loadableFile : null);
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnSampleAssembly() => CheckMetadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));
    }
}
