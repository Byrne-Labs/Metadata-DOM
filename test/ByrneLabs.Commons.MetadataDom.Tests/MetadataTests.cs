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
        private static readonly DirectoryInfo FailedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "FailedTestAssemblies"));
        private static readonly string[] LoadableFileExtensions = { "exe", "dll", "pdb", "mod", "obj", "wmd" };
        private static readonly DirectoryInfo PassedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "PassedTestAssemblies"));
        private static readonly DirectoryInfo TestAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "TestAssemblies"));

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        private static void AssertHasDebugMetadata(ReflectionData reflectionData) => Assert.True(reflectionData.Documents.Any());

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        private static void AssertHasMetadata(ReflectionData reflectionData)
        {
            Assert.NotNull(reflectionData.AssemblyDefinition);
            Assert.True(reflectionData.TypeDefinitions.Any());
        }

        private void AssertValid(ReflectionData reflectionData)
        {
            var checkedMetadataElements = new List<CodeElement>();
            /*
             * While not necessary, checking the declared types first makes debugging easier. -- Jonathan Byrne 12/17/2016
             */
            foreach (var typeDefinition in reflectionData.TypeDefinitions)
            {
                CheckCodeElement(typeDefinition, checkedMetadataElements, true);
            }

            CheckCodeElement(reflectionData, checkedMetadataElements, false);
        }

        private static void CheckCodeElement(CodeElement codeElement, ICollection<CodeElement> checkedMetadataElements, bool excludeAssemblies)
        {
            if (!checkedMetadataElements.Contains(codeElement))
            {
                checkedMetadataElements.Add(codeElement);
                var discoveredCodeElements = new List<CodeElement>();
                foreach (var property in codeElement.GetType().GetTypeInfo().GetProperties())
                {
                    try
                    {
                        var propertyValue = property.GetValue(codeElement);
                        var codeElementPropertyValue = propertyValue as CodeElement;
                        var codeElementsPropertyValue = propertyValue as IEnumerable;
                        if (codeElementPropertyValue != null)
                        {
                            discoveredCodeElements.Add(codeElementPropertyValue);
                        }
                        else if (codeElementsPropertyValue?.GetType().IsConstructedGenericType == true && typeof(CodeElement).GetTypeInfo().IsAssignableFrom(codeElementsPropertyValue.GetType().GetTypeInfo().GetGenericArguments().First()))
                        {
                            discoveredCodeElements.AddRange(codeElementsPropertyValue.Cast<CodeElement>());
                        }
                    }
                    catch (TargetInvocationException exception)
                    {
                        if (!(exception.InnerException is NotSupportedException || exception.InnerException is NotImplementedException))
                        {
                            throw;
                        }
                    }
                }
                foreach (var discoveredCodeElement in discoveredCodeElements.Where(discoveredCodeElement => discoveredCodeElement != null && !(excludeAssemblies && discoveredCodeElement is AssemblyDefinition)).Except(checkedMetadataElements).Distinct())
                {
                    CheckCodeElement(discoveredCodeElement, checkedMetadataElements, excludeAssemblies);
                }
            }
        }

        private static IEnumerable<FileInfo> CopyAllGacAssemblies()
        {
            if (!TestAssemblyDirectory.Exists)
            {
                TestAssemblyDirectory.Create();
                var oldGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("systemroot")}\\assembly");

                Parallel.ForEach(oldGacDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))), assembly =>
                {
                    var newAssemblyLocation = new FileInfo(assembly.FullName.Replace(oldGacDirectory.FullName, TestAssemblyDirectory.FullName));
                    newAssemblyLocation.Directory.Create();
                    assembly.CopyTo(newAssemblyLocation.FullName);
                });

                var newGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("windir")}\\Microsoft.NET\\assembly");
                Parallel.ForEach(newGacDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))), assembly =>
                {
                    var newAssemblyLocation = new FileInfo(assembly.FullName.Replace(newGacDirectory.FullName, TestAssemblyDirectory.FullName));
                    newAssemblyLocation.Directory.Create();
                    assembly.CopyTo(newAssemblyLocation.FullName);
                });
            }

            return TestAssemblyDirectory.EnumerateFiles("*", SearchOption.AllDirectories);
        }

        private static IEnumerable<FileInfo> GetGacAssemblies()
        {
            var oldGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("systemroot")}\\assembly");
            var newGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("windir")}\\Microsoft.NET\\assembly");

            var gacAssemblies = new List<FileInfo>();
            if (oldGacDirectory.Exists)
            {
                gacAssemblies.AddRange(oldGacDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))));
            }

            if (newGacDirectory.Exists)
            {
                gacAssemblies.AddRange(newGacDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))));
            }

            var random = new Random();

            return gacAssemblies.OrderBy(x => random.Next()).ToList();
        }

        private void SmokeTestOnAssemblies(IEnumerable<FileInfo> assemblyFiles, bool prefetch)
        {
            var startTime = DateTime.Now;
            var exceptions = new ConcurrentDictionary<FileInfo, Exception>();
            Parallel.ForEach(assemblyFiles, assemblyFile =>
            {
                try
                {
                }
                catch (Exception exception)
                {
                    exceptions.TryAdd(assemblyFile, exception);
                }
            });

            var executionTime = DateTime.Now.Subtract(startTime);
            _output.WriteLine($"Total execution time: {executionTime.TotalSeconds} seconds");
            _output.WriteLine($"\t{assemblyFiles.Count()} files loaded");
            if (exceptions.Any())
            {
                _output.WriteLine(string.Join("\r\n\r\n", exceptions.Select(exception => $"Assembly {exception.Key.FullName} failed with exception:\r\n{exception.Value}")));
            }
        }

        private void SmokeTestOnCopiedAssemblies(bool prefetch)
        {
            var assemblyFiles = CopyAllGacAssemblies();
            var startTime = DateTime.Now;
            var exceptions = new ConcurrentDictionary<FileInfo, Exception>();
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 100
            };
            Parallel.ForEach(assemblyFiles, parallelOptions, assemblyFile =>
            {
                var assemblyStartTime = DateTime.Now;
                var originalAssemblyDirectory = assemblyFile.Directory;
                try
                {
                    using (var reflectionData = new ReflectionData(prefetch, assemblyFile))
                    {
                        AssertValid(reflectionData);
                    }

                    var passedAssemblyFile = new FileInfo(assemblyFile.FullName.ToLower().Replace(TestAssemblyDirectory.FullName.ToLower(), PassedAssemblyDirectory.FullName));
                    passedAssemblyFile.Directory.Create();
                    assemblyFile.MoveTo(passedAssemblyFile.FullName);
                }
                catch (Exception exception)
                {
                    _output.WriteLine($"Assembly {assemblyFile.FullName} failed with exception:\r\n{exception}");
                    var newAssemblyFile = new FileInfo(assemblyFile.FullName.ToLower().Replace(TestAssemblyDirectory.FullName.ToLower(), FailedAssemblyDirectory.FullName));
                    newAssemblyFile.Directory.Create();
                    assemblyFile.MoveTo(newAssemblyFile.FullName);
                    exceptions.TryAdd(assemblyFile, exception);
                }
                while (!originalAssemblyDirectory.GetFileSystemInfos().Any())
                {
                    originalAssemblyDirectory.Delete();
                    originalAssemblyDirectory = originalAssemblyDirectory.Parent;
                }

                var assemblyExecutionTime = DateTime.Now.Subtract(assemblyStartTime);
                _output.WriteLine($"{assemblyFile.FullName} execution time: {assemblyExecutionTime.TotalSeconds} seconds");
            });

            var executionTime = DateTime.Now.Subtract(startTime);
            _output.WriteLine($"Total execution time: {executionTime.TotalSeconds} seconds");
            _output.WriteLine($"\t{assemblyFiles.Count()} files loaded");
            if (exceptions.Any())
            {
                _output.WriteLine(string.Join("\r\n\r\n", exceptions.Select(exception => $"Assembly {exception.Key.FullName} failed with exception:\r\n{exception.Value}")));
            }
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void QuickRandomSmokeTestOnDotNetFrameworkWithoutPrefetch() => SmokeTestOnAssemblies(GetGacAssemblies().Take(20), false);

        [Fact]
        [Trait("Speed", "Fast")]
        public void QuickRandomSmokeTestOnDotNetFrameworkWithPrefetch() => SmokeTestOnAssemblies(GetGacAssemblies().Take(20), true);

        [Fact]
        [Trait("Speed", "Slow")]
        public void SmokeTestOnCopiedAssembliesWithPrefetch() => SmokeTestOnCopiedAssemblies(true);

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnFailedAssemblies()
        {
            if (FailedAssemblyDirectory.Exists)
            {
                var files = FailedAssemblyDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories);
                SmokeTestOnAssemblies(files, true);
                SmokeTestOnAssemblies(files, false);
            }
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnAssemblyAndPdbWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
            AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnAssemblyAndPdbWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
            AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnAssemblyWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnAssemblyWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnPdbWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(reflectionData);
            AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnOwnPdbWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(reflectionData);
            AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnPrebuildResources()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var loadableFiles = resourceDirectory.GetFiles("*", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles)
            {
                using (var reflectionData = new ReflectionData(loadableFile.Extension.Equals(".pdb") ? null : loadableFile, loadableFile.Extension.Equals(".pdb") ? loadableFile : null))
                {
                    AssertValid(reflectionData);
                }
            }
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnPrebuildAssemblyResources()
        {
            var resourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, @"Resources"));
            var loadableFiles = resourceDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file => LoadableFileExtensions.Contains(file.Extension.Substring(1))).ToList();
            foreach (var loadableFile in loadableFiles.Where(file => !file.Name.Equals("EmptyType.dll")))
            {
                using (var reflectionData = new ReflectionData(loadableFile))
                {
                    AssertValid(reflectionData);
                }
            }
        }

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnSampleAssembly()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
        }
    }
}
