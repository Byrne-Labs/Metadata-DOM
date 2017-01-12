using ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private static readonly DirectoryInfo ValidationFailedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../ValidationFailedTests"));
        private static readonly DirectoryInfo TestAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../TestAssemblies"));

        [Fact]
        [Trait("Category", "Debug helper")]
        public void TestReflectionComparisonOnFailedAssemblyDirectoryAssemblies()
        {
            if (ReadFailedAssemblyDirectory.Exists)
            {
                var assemblyFiles = ReadFailedAssemblyDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories);
                foreach (var assemblyFile in assemblyFiles)
                {
                    Assert.True(CheckMetadata(assemblyFile, null));
                }
            }
        }

        [Fact]
        [Trait("Category", "Debug helper")]
        public void TestReflectionComparisonOnSpecificAssembly()
        {
            Assert.True(CheckMetadata(new FileInfo(@"C:\dev\code\Byrne-Labs\Metadata-DOM\test\ByrneLabs.Commons.MetadataDom.Tests\bin\Debug\ReadFailedTests\NullReferenceException\gac_32\intuit.spc.map.windowsfirewallutilities\v4.0_6.0.39.0__30bbd97113d631f1\intuit.spc.map.windowsfirewallutilities.dll"), null));
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestReflectionComparisonOnSampleAssembly()
        {
            Assert.True(CheckMetadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")), null));
        }

        [Fact]
        [Trait("Category", "Slow")]
        public void TestReflectionComparisonOnCopiedAssemblies()
        {
            var assemblyFiles = CopyAllGacAssemblies();
            var startTime = DateTime.Now;
            var pass = true;
            Parallel.ForEach(assemblyFiles, assemblyFile => { pass &= CheckMetadata(assemblyFile, null); });

            var executionTime = DateTime.Now.Subtract(startTime);
            _output.WriteLine($"Total execution time: {executionTime.TotalSeconds} seconds");
            _output.WriteLine($"\t{assemblyFiles.Count()} files loaded");

            Assert.True(pass);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestReflectionComparisonOnPrebuiltAssemblies()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var assemblyFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => !"EmptyType.dll".Equals(file.Name)).Where(file=>!file.Name.Equals("Interop.Mock01.Impl.dll")).ToList();
            foreach (var assemblyFile in assemblyFiles)
            {
                Assert.True(CheckMetadata(assemblyFile, null));
            }
        }

        private bool CheckMetadata(FileInfo assemblyFile, FileInfo pdbFile)
        {
            var reflectionComparisonDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../../../../ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison/bin/Debug/netcoreapp1.0"));
            var processStartInfo = new ProcessStartInfo("dotnet")
            {
                Arguments = $"exec \"{Path.Combine(reflectionComparisonDirectory.FullName, "ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison.dll")}\" \"{assemblyFile.FullName}\"" + (pdbFile==null?string.Empty : $" \"{pdbFile.FullName}\""),
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

            return TestAssemblyDirectory.EnumerateFiles("*", SearchOption.AllDirectories);
        }

    }
}
