using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison;
using Xunit;

namespace ByrneLabs.Commons.MetadataDom.Tests
{
    public class MetadataTests
    {
        private static readonly string[] LoadableFileExtensions = { "exe", "dll", "pdb", "mod", "obj", "wmd" };

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        public static void AssertHasDebugMetadata(Metadata metadata) => Assert.True(metadata.Documents.Any());

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        public static void AssertHasMetadata(Metadata metadata)
        {
            Assert.NotNull(metadata.AssemblyDefinition);
            Assert.True(metadata.TypeDefinitions.Any());
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnAssemblyAndPdbWithoutPrefetch()
        {
            var metadata = new Metadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            MetadataChecker.CheckMetadata(metadata);
            AssertHasMetadata(metadata);
            AssertHasDebugMetadata(metadata);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnAssemblyAndPdbWithPrefetch()
        {
            var metadata = new Metadata(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            MetadataChecker.CheckMetadata(metadata);
            AssertHasMetadata(metadata);
            AssertHasDebugMetadata(metadata);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnAssemblyWithoutPrefetch()
        {
            var metadata = new Metadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            MetadataChecker.CheckMetadata(metadata);
            AssertHasMetadata(metadata);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnAssemblyWithPrefetch()
        {
            var metadata = new Metadata(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            MetadataChecker.CheckMetadata(metadata);
            AssertHasMetadata(metadata);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnPdbWithoutPrefetch()
        {
            var metadata = new Metadata(null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            MetadataChecker.CheckMetadata(metadata);
            AssertHasDebugMetadata(metadata);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnPdbWithPrefetch()
        {
            var metadata = new Metadata(true, null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            MetadataChecker.CheckMetadata(metadata);
            AssertHasDebugMetadata(metadata);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnPrebuiltAssemblyResources()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var loadableFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles.Where(file => !file.Name.Equals("EmptyType.dll")))
            {
                using (var metadata = new Metadata(loadableFile))
                {
                    MetadataChecker.CheckMetadata(metadata);
                }
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
                using (var metadata = new Metadata(loadableFile.Extension.Equals(".pdb") ? null : loadableFile, loadableFile.Extension.Equals(".pdb") ? loadableFile : null))
                {
                    MetadataChecker.CheckMetadata(metadata);
                }
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnSampleAssembly()
        {
            var metadata = new Metadata(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));
            MetadataChecker.CheckMetadata(metadata);
            AssertHasMetadata(metadata);
        }
    }
}
