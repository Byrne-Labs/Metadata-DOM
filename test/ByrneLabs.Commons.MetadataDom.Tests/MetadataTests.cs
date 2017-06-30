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
        public void TestOnOwnAssembly() => CheckMetadata(CheckTypes.MetadataTypes, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnAssemblyAndPdb() => CheckMetadata(CheckTypes.Metadata, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnOwnPdb() => CheckMetadata(CheckTypes.MetadataSymbols, null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnPrebuiltAssemblyResources()
        {
            var loadableFiles = ResourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles.Where(file => !file.Name.Equals("EmptyType.dll", StringComparison.Ordinal)))
            {
                CheckMetadata(CheckTypes.Metadata, loadableFile);
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnPrebuiltResources()
        {
            var loadableFiles = ResourceDirectory.GetFiles("*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles)
            {
                CheckMetadata(CheckTypes.Metadata, loadableFile.Extension.Equals(".pdb", StringComparison.Ordinal) ? null : loadableFile, loadableFile.Extension.Equals(".pdb", StringComparison.Ordinal) ? loadableFile : null);
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnSampleAssembly() => CheckMetadata(CheckTypes.Metadata, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));

        [Fact]
        [Trait("Category", "Fast")]
        public void TestOnSamplePdbBasicSymbolFormats()
        {
            var basicSymbolFormats = SampleBuild.BuildBasicSymbolFormats();
            var success = true;
            foreach (var build in basicSymbolFormats)
            {
                success &= CheckMetadata(CheckTypes.MetadataSymbols, build.AssemblyFile, build.SymbolsFile, false);
            }

            Assert.True(success);
        }

        private bool CheckMetadata(CheckTypes checkType, FileInfo assemblyFile, FileInfo pdbFile = null, bool assertSuccess = true)
        {
            var checkState = BaseChecker.Check(checkType, assemblyFile, pdbFile);
            var outputMessage = new StringBuilder();
            outputMessage.Append('-', 120).AppendLine();
            outputMessage.AppendLine(assemblyFile?.FullName ?? pdbFile?.FullName).AppendLine();
            var success = checkState.Success;

            if (assemblyFile != null && ".dll".Equals(assemblyFile.Extension, StringComparison.Ordinal) && checkState.Metadata?.TypeDefinitions.Any() != true)
            {
                outputMessage.AppendLine("No type definitions loaded from metadata");
                success = false;
            }
            if (pdbFile != null && checkState.Metadata?.Documents.Any() != true)
            {
                outputMessage.AppendLine("No type documents loaded from metadata");
                success = false;
            }

            outputMessage.AppendLine(checkState.ErrorLogText);

            if (assemblyFile != null && ".dll".Equals(assemblyFile.Extension, StringComparison.Ordinal))
            {
                if (checkState.Metadata.AssemblyDefinition == null)
                {
                    checkState.AddError("Assembly definition null");
                }
                if (!checkState.Metadata.TypeDefinitions.Any())
                {
                    checkState.AddError("No type definitions loaded");
                }
            }
            if (pdbFile != null)
            {
                if (!checkState.Metadata.Documents.Any())
                {
                    checkState.AddError("No documents loaded");
                }
                if (!checkState.Metadata.MethodDebugInformation.Any())
                {
                    checkState.AddError("No method debug information loaded");
                }
            }

            if (!success)
            {
                _output.WriteLine(outputMessage.ToString());
            }
            if (assertSuccess)
            {
                Assert.True(checkState.Success);
            }

            return success;
        }
    }
}
