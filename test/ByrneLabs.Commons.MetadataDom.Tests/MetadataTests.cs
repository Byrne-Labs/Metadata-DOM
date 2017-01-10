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
        private static readonly DirectoryInfo ValidationFailedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "ValidationFailedTestAssemblies"));
        private static readonly DirectoryInfo UnloadableAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "UnloadableTestAssemblies"));
        private static readonly DirectoryInfo ReadFailedAssemblyDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "ReadFailedTestAssemblies"));
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

        [Fact]
        [Trait("Speed", "Fast")]
        public void TestOnValidationFailedAssemblyDirectoryAssemblies()
        {
            if (ValidationFailedAssemblyDirectory.Exists)
            {
                var assemblyFiles = ValidationFailedAssemblyDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories);
                foreach (var assemblyFile in assemblyFiles)
                {
                    Assert.True(TestCopiedAssembly(assemblyFile));
                }
            }
        }

        public bool TestCopiedAssembly(FileInfo assemblyFile)
        {
            bool pass = true;
            var exceptions = new ConcurrentDictionary<FileInfo, Exception>();
            var assemblyStartTime = DateTime.Now;
            var originalAssemblyDirectory = assemblyFile.Directory;
            try
            {
                using (var reflectionData = new ReflectionData(true, assemblyFile))
                {
                    AssertValid(reflectionData);
                }

                var errors = ReflectionChecker.Check(assemblyFile);
                pass = !errors.Any();
                DirectoryInfo newFileDirectory;
                if (errors.Any())
                {
                    _output.WriteLine($"Assembly {assemblyFile.FullName} failed with the errors:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
                    newFileDirectory = ValidationFailedAssemblyDirectory;
                }
                else
                {
                    newFileDirectory = PassedAssemblyDirectory;
                }
                var newFileLocation = new FileInfo(assemblyFile.FullName.ToLower().Replace(TestAssemblyDirectory.FullName.ToLower(), newFileDirectory.FullName));
                newFileLocation.Directory.Create();
                assemblyFile.MoveTo(newFileLocation.FullName);
            }
            catch (Exception exception)
            {
                pass = false;
                _output.WriteLine($"Assembly {assemblyFile.FullName} failed with exception:\r\n{exception}");
                var newAssemblyFile = new FileInfo(assemblyFile.FullName.ToLower().Replace(TestAssemblyDirectory.FullName.ToLower(), ReadFailedAssemblyDirectory.FullName));
                newAssemblyFile.Directory.Create();
                assemblyFile.MoveTo(newAssemblyFile.FullName);
            }

            while (!originalAssemblyDirectory.GetFileSystemInfos().Any())
            {
                originalAssemblyDirectory.Delete();
                originalAssemblyDirectory = originalAssemblyDirectory.Parent;
            }

            var assemblyExecutionTime = DateTime.Now.Subtract(assemblyStartTime);
            _output.WriteLine($"{assemblyFile.FullName} execution time: {assemblyExecutionTime.TotalSeconds} seconds");

            return pass;
        }


        [Fact]
        [Trait("Speed", "Slow")]
        public void TestOnCopiedAssemblies()
        {
            var assemblyFiles = CopyAllGacAssemblies();
            var startTime = DateTime.Now;
            bool pass = true;
            Parallel.ForEach(assemblyFiles, assemblyFile =>
            {
                pass &= TestCopiedAssembly(assemblyFile);
            });

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
        public void TestOnPrebuiltResources()
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
        public void TestOnPrebuiltAssemblyResources()
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
