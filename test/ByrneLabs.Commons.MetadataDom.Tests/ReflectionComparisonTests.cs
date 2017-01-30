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

                var possiblePdbFile = new FileInfo(assemblyFile.FullName.Substring(0, assemblyFile.FullName.Length - 4) + ".pdb");
                pdbFile = pdbFile ?? (possiblePdbFile.Exists ? possiblePdbFile : null);

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

                var testMessage = new StringBuilder();

                testMessage.Append('-', 80).AppendLine();
                testMessage.AppendLine(assemblyFile.FullName);
                switch (process.ExitCode)
                {
                    case 0:
                        testMessage.AppendLine($"{assemblyFile.FullName} succeeded with processor time {process.TotalProcessorTime.TotalSeconds} seconds");
                        break;
                    case 1:
                        testMessage.AppendLine($"{assemblyFile.FullName} failed with processor time {process.TotalProcessorTime.TotalSeconds} seconds with the errors {output}");
                        break;
                    default:
                        testMessage.AppendLine($"{assemblyFile.FullName} faulted with processor time {process.TotalProcessorTime.TotalSeconds} seconds and the error:{Environment.NewLine}{error}");
                        break;
                }

                try
                {
                    if (assemblyFile.FullName.ToLower().StartsWith(BaseTestsDirectory.FullName.ToLower()))
                    {
                        assemblyFile.Directory.Delete(true);
                        var assemblyDirectory = assemblyFile.Directory.Parent;
                        while (!assemblyDirectory.GetFileSystemInfos().Any())
                        {
                            assemblyDirectory.Delete();
                            assemblyDirectory = assemblyDirectory.Parent;
                        }
                    }
                }
                catch (Exception exception)
                {
                    testMessage.AppendLine("Attempting to delete the assembly directory failed with the exception:").AppendLine(exception.ToString());
                }

                testMessage.Append('-', 80);
                _output.WriteLine(testMessage.ToString());
                if (process.ExitCode != 0)
                {
                    Debug.WriteLine(testMessage.ToString());
                }

                return process.ExitCode == 0;
            }
        }

        private static IEnumerable<FileInfo> CopyAllGacAssemblies()
        {
            if (!TestsNotRunDirectory.Exists)
            {
                var filesToTest = new List<FileInfo>();
                var oldGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("systemroot")}\\assembly");
                var newGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("windir")}\\Microsoft.NET\\assembly");
                var nugetDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("userprofile")}\\.nuget\\packages");

                filesToTest.AddRange(oldGacDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories));
                filesToTest.AddRange(oldGacDirectory.EnumerateFiles("*.exe", SearchOption.AllDirectories));
                filesToTest.AddRange(newGacDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories));
                filesToTest.AddRange(newGacDirectory.EnumerateFiles("*.exe", SearchOption.AllDirectories));
                filesToTest.AddRange(nugetDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories));
                filesToTest.AddRange(nugetDirectory.EnumerateFiles("*.exe", SearchOption.AllDirectories));

                Parallel.ForEach(filesToTest, fileToTest =>
                {
                    var newAssemblyLocation = new FileInfo(Path.Combine(TestsNotRunDirectory.FullName, Guid.NewGuid().ToString(), fileToTest.Name));
                    newAssemblyLocation.Directory.Create();
                    File.WriteAllText(Path.Combine(newAssemblyLocation.DirectoryName, "info.txt"), $"Original location: {fileToTest.FullName}");
                    fileToTest.CopyTo(newAssemblyLocation.FullName);
                    var pdbFile = new FileInfo(fileToTest.FullName.Substring(0, fileToTest.FullName.Length - 4) + ".pdb");
                    if (pdbFile.Exists)
                    {
                        pdbFile.CopyTo(newAssemblyLocation.FullName.Substring(0, newAssemblyLocation.FullName.Length - 4) + ".pdb");
                    }
                });
            }

            return TestsNotRunDirectory.EnumerateFiles("*", SearchOption.AllDirectories).Where(file => file.Extension.Equals(".dll") || file.Extension.Equals(".exe")).ToList();
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "These must be files, not directories")]
        private static ProcessStartInfo GetNetCoreProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
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
        private static ProcessStartInfo GetNetFrameworkProcess(FileInfo assemblyFile, FileInfo pdbFile = null)
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
        public void TestReflectionComparisonOnCopiedAssemblies() => TestReflectionComparison(CopyAllGacAssemblies().OrderBy(file => file.Length).ToList());

        private void TestReflectionComparison(IEnumerable<FileInfo> assemblyFiles)
        {
            var startTime = DateTime.Now;
            var pass = true;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 8 };
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
            // ReSharper disable once LoopCanBeConvertedToQuery -- This is much easier to read as a loop. -- Jonathan Byrne 01/21/2017
            foreach (var assemblyFile in assemblyFiles)
            {
                pass &= CheckMetadataInProcess(assemblyFile);
            }

            Assert.True(pass);
        }

        [Fact]
        [Trait("Category", "Fast")]
        public void TestReflectionComparisonSampleAssembly() => Assert.True(CheckMetadataOutOfProcess(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll"))));

        [Fact]
        [Trait("Category", "Slow")]
        public void TestReflectionComparisonOnSampleAssemblies() => TestReflectionComparison(SampleBuild.GetSampleAssemblies(500));

        [Fact]
        [Trait("Category", "Debug helper")]
        public void TestReflectionComparisonOnSpecificAssembly() => Assert.True(CheckMetadataInProcess(new FileInfo(@"C:\dev\code\Byrne-Labs\Metadata-DOM\test\ByrneLabs.Commons.MetadataDom.Tests\bin\Debug\Tests\FaultedMetadataCheck\Microsoft.GroupPolicy.Management.Interop--2.0.0.0--\Microsoft.GroupPolicy.Management.Interop.dll")));
    }
}
