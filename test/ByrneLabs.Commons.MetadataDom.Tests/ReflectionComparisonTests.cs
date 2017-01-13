using ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ByrneLabs.Commons.MetadataDom.Tests
{
    public class ReflectionComparisonTests
    {
        public ReflectionComparisonTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;
        private static readonly DirectoryInfo ReadFailedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../ReadFailedTests"));
        private static readonly DirectoryInfo TestAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../TestAssemblies"));
        private static readonly DirectoryInfo ValidationFailedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../ValidationFailedTests"));

        private bool CheckMetadataInProcess(FileInfo assemblyFile, FileInfo pdbFile)
        {
            ReflectionChecker.BaseDirectory = AppContext.BaseDirectory;
            return ReflectionChecker.Check(assemblyFile, pdbFile);
        }

        private bool CheckMetadataOutOfProcess(FileInfo assemblyFile, FileInfo pdbFile)
        {
            var reflectionComparisonDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../../../../ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison/bin/Debug/netcoreapp1.0"));
            var processStartInfo = new ProcessStartInfo("dotnet")
            {
                Arguments = $"exec \"{Path.Combine(reflectionComparisonDirectory.FullName, "ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison.dll")}\" \"{AppContext.BaseDirectory}\" \"{assemblyFile.FullName}\"" + (pdbFile == null ? string.Empty : $" \"{pdbFile.FullName}\""),
                WorkingDirectory = reflectionComparisonDirectory.FullName
            };
            var process = Process.Start(processStartInfo);
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                _output.WriteLine($"{assemblyFile.FullName} failed with processor time: {process.TotalProcessorTime.TotalSeconds} seconds");
            }
            else
            {
                _output.WriteLine($"{assemblyFile.FullName} succeeded with processor time: {process.TotalProcessorTime.TotalSeconds} seconds");
            }
            return process.ExitCode == 0;
        }

        private static IEnumerable<FileInfo> CopyAllGacAssemblies()
        {
            if (!TestAssemblyDirectory.Exists)
            {
                TestAssemblyDirectory.Create();
                var oldGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("systemroot")}\\assembly");

                Parallel.ForEach(oldGacDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories), assembly =>
                {
                    var newAssemblyLocation = new FileInfo(assembly.FullName.Replace(oldGacDirectory.FullName, TestAssemblyDirectory.FullName));
                    newAssemblyLocation.Directory.Create();
                    assembly.CopyTo(newAssemblyLocation.FullName);
                });

                var newGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("windir")}\\Microsoft.NET\\assembly");
                Parallel.ForEach(newGacDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories), assembly =>
                {
                    var newAssemblyLocation = new FileInfo(assembly.FullName.Replace(newGacDirectory.FullName, TestAssemblyDirectory.FullName));
                    newAssemblyLocation.Directory.Create();
                    assembly.CopyTo(newAssemblyLocation.FullName);
                });
            }

            return TestAssemblyDirectory.EnumerateFiles("*", SearchOption.AllDirectories).Where(file => file.Extension.Equals(".dll") || file.Extension.Equals(".exe")).ToList();
        }

        [Fact]
        [Trait("Category", "Slow")]
        public void TestReflectionComparisonOnCopiedAssemblies()
        {
            var assemblyFiles = CopyAllGacAssemblies();
            var startTime = DateTime.Now;
            var pass = true;
            Parallel.ForEach(assemblyFiles, assemblyFile => { pass &= CheckMetadataOutOfProcess(assemblyFile, null); });

            var executionTime = DateTime.Now.Subtract(startTime);
            _output.WriteLine($"Total execution time: {executionTime.TotalSeconds} seconds");
            _output.WriteLine($"\t{assemblyFiles.Count()} files loaded");

            Assert.True(pass);
        }

        [Fact]
        [Trait("Category", "Debug helper")]
        public void TestReflectionComparisonOnFailedAssemblyDirectoryAssemblies()
        {
            if (ReadFailedAssemblyDirectory.Exists)
            {
                var assemblyFiles = ReadFailedAssemblyDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories);
                foreach (var assemblyFile in assemblyFiles)
                {
                    Assert.True(CheckMetadataInProcess(assemblyFile, null));
                }
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestReflectionComparisonOnPrebuiltAssemblies()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var assemblyFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => !"EmptyType.dll".Equals(file.Name)).Where(file => !file.Name.Equals("Interop.Mock01.Impl.dll")).ToList();
            foreach (var assemblyFile in assemblyFiles)
            {
                Assert.True(CheckMetadataInProcess(assemblyFile, null));
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestReflectionComparisonOnSampleAssembly()
        {
            Assert.True(CheckMetadataOutOfProcess(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")), null));
        }

        [Fact]
        [Trait("Category", "Debug helper")]
        public void TestReflectionComparisonOnSpecificAssembly()
        {
            Assert.True(CheckMetadataInProcess(new FileInfo(@"C:\dev\code\Byrne-Labs\Metadata-DOM\test\ByrneLabs.Commons.MetadataDom.Tests\bin\Debug\ValidationFailedTests\gac_msil\microsoft.visualstudio.tools.office.project.word\v4.0_14.0.0.0__b03f5f7f11d50a3a\microsoft.visualstudio.tools.office.project.word.dll"), null));
        }
    }
}
