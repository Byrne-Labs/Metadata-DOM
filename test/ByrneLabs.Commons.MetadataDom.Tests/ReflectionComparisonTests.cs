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
    public class ReflectionComparisonTests
    {
        public ReflectionComparisonTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;
        private static readonly DirectoryInfo PassedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../PassedTests"));
        private static readonly DirectoryInfo ReadFailedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../ReadFailedTests"));
        private static readonly DirectoryInfo ValidationFailedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../ValidationFailedTests"));
        private static readonly DirectoryInfo TestAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "../TestAssemblies"));

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestReflectionComparisonOnFailedAssemblyDirectoryAssemblies()
        {
            if (ReadFailedAssemblyDirectory.Exists)
            {
                var assemblyFiles = ReadFailedAssemblyDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories);
                foreach (var assemblyFile in assemblyFiles)
                {
                    Assert.True(TestCopiedAssembly(assemblyFile));
                }
            }
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestReflectionComparisonOnSpecificAssembly()
        {
            Assert.True(TestCopiedAssembly(new FileInfo(@"C:\dev\code\Byrne-Labs\Metadata-DOM\test\ByrneLabs.Commons.MetadataDom.Tests\bin\Debug\ValidationFailedTests\gac\microsoft.stdformat\7.0.3300.0__b03f5f7f11d50a3a\microsoft.stdformat.dll")));
        }



        [Fact]
        [Trait("Speed", "Fast")]
        public void TestReflectionComparisonOnSampleAssembly()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));
            MetadataChecker.AssertValid(reflectionData);
            MetadataChecker.AssertHasMetadata(reflectionData);
        }


        [Fact]
        [Trait("Speed", "Slow")]
        public void TestReflectionComparisonOnCopiedAssemblies()
        {
            var assemblyFiles = CopyAllGacAssemblies();
            var startTime = DateTime.Now;
            var pass = true;
            Parallel.ForEach(assemblyFiles, assemblyFile => { pass &= TestCopiedAssembly(assemblyFile); });

            var executionTime = DateTime.Now.Subtract(startTime);
            _output.WriteLine($"Total execution time: {executionTime.TotalSeconds} seconds");
            _output.WriteLine($"\t{assemblyFiles.Count()} files loaded");

            Assert.True(pass);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestReflectionComparisonOnPrebuiltAssemblies()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var assemblyFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => !"EmptyType.dll".Equals(file.Name)).ToList();
            foreach (var assemblyFile in assemblyFiles)
            {
                var errors = ReflectionChecker.Check(assemblyFile);
                if (errors.Any())
                {
                    _output.WriteLine(string.Join(Environment.NewLine, errors));
                }
                Assert.Empty(errors);
            }
        }

        private bool TestCopiedAssembly(FileInfo assemblyFile)
        {
            var errors = new List<string>();
            var assemblyStartTime = DateTime.Now;
            var originalAssemblyDirectory = assemblyFile.Directory;
            DirectoryInfo newFileDirectory;

            try
            {
                using (var reflectionData = new ReflectionData(true, assemblyFile))
                {
                    MetadataChecker.AssertValid(reflectionData);
                }

                errors.AddRange(ReflectionChecker.Check(assemblyFile));
                if (errors.Any())
                {
                    _output.WriteLine($"Assembly {assemblyFile.FullName} failed with the errors:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
                    newFileDirectory = ValidationFailedAssemblyDirectory;
                }
                else
                {
                    newFileDirectory = PassedAssemblyDirectory;
                }
            }
            catch (Exception exception)
            {
                Exception realException = exception;
                while (realException is TargetInvocationException && realException.InnerException != null)
                {
                    realException = realException.InnerException;
                }
                errors.Add($"Assembly {assemblyFile.FullName} failed with exception:\r\n{realException}");

                newFileDirectory = new DirectoryInfo(Path.Combine(ReadFailedAssemblyDirectory.FullName, realException.GetType().Name));
            }
            FileInfo newFileLocation;
            if (assemblyFile.FullName.ToLower().StartsWith(TestAssemblyDirectory.FullName.ToLower()))
            {
                newFileLocation = new FileInfo(assemblyFile.FullName.ToLower().Replace(TestAssemblyDirectory.FullName.ToLower(), newFileDirectory.FullName));
                newFileLocation.Directory.Create();
                assemblyFile.MoveTo(newFileLocation.FullName);
            }
            else
            {
                newFileLocation = assemblyFile;
            }

            while (!originalAssemblyDirectory.GetFileSystemInfos().Any())
            {
                originalAssemblyDirectory.Delete();
                originalAssemblyDirectory = originalAssemblyDirectory.Parent;
            }

            if (errors.Any())
            {
                File.WriteAllLines(newFileLocation.FullName.Substring(0, newFileLocation.FullName.Length - 4) + ".log", errors);
            }

            var assemblyExecutionTime = DateTime.Now.Subtract(assemblyStartTime);
            _output.WriteLine($"{assemblyFile.FullName} execution time: {assemblyExecutionTime.TotalSeconds} seconds");

            return !errors.Any();
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
