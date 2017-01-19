using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore;
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
        private static readonly DirectoryInfo BaseTestsDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, @"..\Tests"));
        private static readonly DirectoryInfo TestsNotRunDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, @"..\Tests\NotRun"));

        private bool CheckMetadataInProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checkState = NetCoreChecker.Check(assemblyFile, pdbFile);
            _output.WriteLine(checkState.LogText);
            return checkState.Success;
        }

        private bool CheckMetadataOutOfProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            using (var process = new Process())
            using (var outputWaitHandle = new AutoResetEvent(false))
            using (var errorWaitHandle = new AutoResetEvent(false))
            {
                var output = new StringBuilder();
                var error = new StringBuilder();

                process.StartInfo = GetNetFrameworkProcess(assemblyFile, pdbFile);
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        error.AppendLine(e.Data);
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                switch (process.ExitCode)
                {
                    case 0:
                        _output.WriteLine($"{assemblyFile.FullName} succeeded with processor time {process.TotalProcessorTime.TotalSeconds} seconds");
                        break;
                    case 1:
                        _output.WriteLine($"{assemblyFile.FullName} failed with processor time {process.TotalProcessorTime.TotalSeconds} seconds with the errors {output}");
                        break;
                    default:
                        _output.WriteLine($"{assemblyFile.FullName} faulted with processor time {process.TotalProcessorTime.TotalSeconds} seconds and the error:{Environment.NewLine}{error}");
                        break;
                }

                if (assemblyFile.FullName.ToLower().StartsWith(BaseTestsDirectory.FullName.ToLower()))
                {
                    assemblyFile.Delete();
                    var assemblyDirectory = assemblyFile.Directory;
                    while (!assemblyDirectory.GetFileSystemInfos().Any())
                    {
                        assemblyDirectory.Delete();
                        assemblyDirectory = assemblyDirectory.Parent;
                    }
                }

                return process.ExitCode == 0;
            }
        }

        private static IEnumerable<FileInfo> CopyAllGacAssemblies()
        {
            if (!TestsNotRunDirectory.Exists)
            {
                TestsNotRunDirectory.Create();
                var oldGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("systemroot")}\\assembly");

                Parallel.ForEach(oldGacDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories), assembly =>
                {
                    var newAssemblyLocation = new FileInfo(assembly.FullName.Replace(oldGacDirectory.FullName, TestsNotRunDirectory.FullName));
                    newAssemblyLocation.Directory.Create();
                    assembly.CopyTo(newAssemblyLocation.FullName);
                });

                var newGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("windir")}\\Microsoft.NET\\assembly");
                Parallel.ForEach(newGacDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories), assembly =>
                {
                    var newAssemblyLocation = new FileInfo(assembly.FullName.Replace(newGacDirectory.FullName, TestsNotRunDirectory.FullName));
                    newAssemblyLocation.Directory.Create();
                    assembly.CopyTo(newAssemblyLocation.FullName);
                });
            }

            return TestsNotRunDirectory.EnumerateFiles("*", SearchOption.AllDirectories).Where(file => file.Extension.Equals(".dll") || file.Extension.Equals(".exe")).ToList();
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "These must be files, not directories")]
        private ProcessStartInfo GetNetCoreProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checkerDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../../../../ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore/bin/Debug/netcoreapp1.0"));
            var processStartInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = checkerDirectory.FullName,
                Arguments = $"exec \"{Path.Combine(checkerDirectory.FullName, "ByrneLabs.Commons.MetadataDom.Tests.Checker.NetCore.dll")}\" \"{BaseTestsDirectory}\" \"{assemblyFile.FullName}\"" + (pdbFile == null ? string.Empty : $" \"{pdbFile.FullName}\""),
                RedirectStandardError = true
            };

            return processStartInfo;
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "These must be files, not directories")]
        private ProcessStartInfo GetNetFrameworkProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var processStartInfo = new ProcessStartInfo(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\ByrneLabs.Commons.MetadataDom.Tests.Checker.NetFramework\bin\Debug\ByrneLabs.Commons.MetadataDom.Tests.Checker.NetFramework.exe"))
            {
                Arguments = $"\"{BaseTestsDirectory}\" \"{assemblyFile.FullName}\"" + (pdbFile == null ? string.Empty : $" \"{pdbFile.FullName}\""),
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            return processStartInfo;
        }

        [Fact]
        [Trait("Category", "Slow")]
        public void TestReflectionComparisonOnCopiedAssemblies()
        {
            var assemblyFiles = CopyAllGacAssemblies().OrderBy(file => file.Length).ToList();
            var startTime = DateTime.Now;
            var pass = true;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 12 };
            Parallel.ForEach(assemblyFiles, parallelOptions, assemblyFile => pass &= CheckMetadataOutOfProcess(assemblyFile));

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
            var assemblyFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => !"EmptyType.dll".Equals(file.Name)).Where(file => !file.Name.Contains("Interop.Mock01")).ToList();
            var pass = true;
            foreach (var assemblyFile in assemblyFiles)
            {
                pass &= CheckMetadataInProcess(assemblyFile);
            }
            Assert.True(pass);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestReflectionComparisonOnSampleAssembly() => Assert.True(CheckMetadataOutOfProcess(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll"))));

        [Fact]
        [Trait("Category", "Debug helper")]
        public void TestReflectionComparisonOnSpecificAssembly() => Assert.True(CheckMetadataInProcess(new FileInfo(@"C:\dev\code\Byrne-Labs\Metadata-DOM\test\ByrneLabs.Commons.MetadataDom.Tests\bin\Debug\Tests\FailedValidation\gac_64\msbuild\v4.0_14.0.0.0__b03f5f7f11d50a3a\msbuild.exe")));
    }
}
