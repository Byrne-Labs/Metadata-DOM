using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnAssemblyAndPdbWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            MetadataChecker.AssertValid(reflectionData);
            MetadataChecker.AssertHasMetadata(reflectionData);
            MetadataChecker.AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnAssemblyAndPdbWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            MetadataChecker.AssertValid(reflectionData);
            MetadataChecker.AssertHasMetadata(reflectionData);
            MetadataChecker.AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnAssemblyWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            MetadataChecker.AssertValid(reflectionData);
            MetadataChecker.AssertHasMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnAssemblyWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            MetadataChecker.AssertValid(reflectionData);
            MetadataChecker.AssertHasMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnPdbWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            MetadataChecker.AssertValid(reflectionData);
            MetadataChecker.AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnPdbWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            MetadataChecker.AssertValid(reflectionData);
            MetadataChecker.AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnPrebuiltAssemblyResources()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var loadableFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles.Where(file => !file.Name.Equals("EmptyType.dll")))
            {
                using (var reflectionData = new ReflectionData(loadableFile))
                {
                    MetadataChecker.AssertValid(reflectionData);
                }
            }
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnPrebuiltResources()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var loadableFiles = resourceDirectory.GetFiles("*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles)
            {
                using (var reflectionData = new ReflectionData(loadableFile.Extension.Equals(".pdb") ? null : loadableFile, loadableFile.Extension.Equals(".pdb") ? loadableFile : null))
                {
                    MetadataChecker.AssertValid(reflectionData);
                }
            }
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnSampleAssembly()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));
            MetadataChecker.AssertValid(reflectionData);
            MetadataChecker.AssertHasMetadata(reflectionData);
        }
    }
}
