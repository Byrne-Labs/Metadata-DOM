using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore;
using ByrneLabs.Commons.MetadataDom.Tests.Checker;
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

        private void CheckMetadataInProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checkState = NetCoreChecker.Check(assemblyFile, pdbFile);
            _output.WriteLine(checkState.LogText);
            Assert.True(checkState.Success);
        }

        private bool CheckMetadataOutOfProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            return NetCoreCheckMetadataOutOfProcess(assemblyFile, pdbFile);
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

        private bool NetCoreCheckMetadataOutOfProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checkerDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../../../../ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore/bin/Debug/netcoreapp1.0"));
            var processStartInfo = new ProcessStartInfo("dotnet")
            {
                Arguments = $"exec \"{Path.Combine(checkerDirectory.FullName, "ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison.dll")}\" \"{AppContext.BaseDirectory}\" \"{assemblyFile.FullName}\"" + (pdbFile == null ? string.Empty : $" \"{pdbFile.FullName}\""),
                WorkingDirectory = checkerDirectory.FullName
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

        private bool NetFrameworkCheckMetadataOutOfProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var processStartInfo = new ProcessStartInfo("ByrneLabs.Commons.MetadataDom.Tests.Checker.NetFramework.exe")
            {
                Arguments = $"\"{AppContext.BaseDirectory}\" \"{assemblyFile.FullName}\"" + (pdbFile == null ? string.Empty : $" \"{pdbFile.FullName}\""),
                WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "../../../../ByrneLabs.Commons.MetadataDom.Tests.Checker.NetFramework/bin/Debug/")
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

        [Fact]
        [Trait("Category", "Slow")]
        public void TestReflectionComparisonOnCopiedAssemblies()
        {
            var assemblyFiles = CopyAllGacAssemblies();
            var startTime = DateTime.Now;
            var pass = true;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 6 };
            Parallel.ForEach(assemblyFiles, parallelOptions, assemblyFile => pass &= CheckMetadataOutOfProcess(assemblyFile));

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
                    CheckMetadataInProcess(assemblyFile);
                }
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestReflectionComparisonOnPrebuiltAssemblies()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var assemblyFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => !"EmptyType.dll".Equals(file.Name)).Where(file => !file.Name.Contains("Interop.Mock01")).ToList();
            foreach (var assemblyFile in assemblyFiles)
            {
                CheckMetadataInProcess(assemblyFile, null);
            }
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestReflectionComparisonOnSampleAssembly()
        {
            CheckMetadataOutOfProcess(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));
        }

        [Fact]
        [Trait("Category", "Debug helper")]
        public void TestReflectionComparisonOnSpecificAssembly()
        {
            CheckMetadataInProcess(new FileInfo(@"C:\dev\code\Byrne-Labs\Metadata-DOM\test\ByrneLabs.Commons.MetadataDom.Tests\bin\Debug\ValidationFailedTests\gac_msil\microsoft.practices.objectbuilder2\2.2.0.0__31bf3856ad364e35\microsoft.practices.objectbuilder2.dll"));
        }
    }
}
