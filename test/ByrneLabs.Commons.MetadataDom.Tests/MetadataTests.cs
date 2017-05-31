using System;
using System.IO;
using System.Linq;
using System.Text;
using ByrneLabs.Commons.MetadataDom.Tests.Checker;
using Xunit;
using Xunit.Abstractions;

namespace ByrneLabs.Commons.MetadataDom.Tests
{
    public class MetadataTests
    {
        private static readonly string[] LoadableFileExtensions = { "exe", "dll", "pdb", "mod", "obj", "wmd" };
        private static readonly DirectoryInfo ResourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName, "Resources"));
        private readonly ITestOutputHelper _output;

        public MetadataTests(ITestOutputHelper output)
        {
            _output = output;
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
            var loadableFiles = ResourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles.Where(file => !file.Name.Equals("EmptyType.dll")))
            {
                CheckMetadata(loadableFile);
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnPrebuiltResources()
        {
            var loadableFiles = ResourceDirectory.GetFiles("*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles)
            {
                CheckMetadata(loadableFile.Extension.Equals(".pdb") ? null : loadableFile, loadableFile.Extension.Equals(".pdb") ? loadableFile : null);
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnSampleAssembly() => CheckMetadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));

        private bool CheckMetadata(FileInfo assemblyFile, FileInfo pdbFile = null, bool assertSuccess = true)
        {
            var checkState = BaseChecker.CheckOnlyMetadata(assemblyFile, pdbFile);
            var outputMessage = new StringBuilder();
            outputMessage.Append('-', 120).AppendLine();
            outputMessage.AppendLine(assemblyFile?.FullName ?? pdbFile?.FullName).AppendLine();
            var success = checkState.Success;

            if (assemblyFile != null && ".dll".Equals(assemblyFile.Extension))
            {
                if (checkState.Metadata?.TypeDefinitions.Any() != true)
                {
                    outputMessage.AppendLine("No type definitions loaded from metadata");
                    success = false;
                }
            }
            if (pdbFile != null && checkState.Metadata?.Documents.Any() != true)
            {
                outputMessage.AppendLine("No type documents loaded from metadata");
                success = false;
            }

            outputMessage.AppendLine(checkState.ErrorLogText);

            if (!success)
            {
                _output.WriteLine(outputMessage.ToString());
            }

            if (assertSuccess)
            {
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

            return success;
        }
    }
}
