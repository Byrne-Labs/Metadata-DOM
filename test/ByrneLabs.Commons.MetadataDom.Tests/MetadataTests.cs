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
        private static readonly string[] LoadableFileExtensions = { "exe", "dll", "pdb", "mod", "obj", "wmd" };

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
            CheckCodeElement(reflectionData, new List<CodeElement>());
            OutputMetadataSummary(new[] { reflectionData });
        }

        private static void CheckCodeElement(CodeElement codeElement, ICollection<CodeElement> checkedCodeElements)
        {
            if (!checkedCodeElements.Contains(codeElement))
            {
                checkedCodeElements.Add(codeElement);
                var discoveredCodeElements = new List<CodeElement>();
                foreach (var property in codeElement.GetType().GetTypeInfo().GetProperties())
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
                var assemblies = checkedCodeElements.OfType<AssemblyDefinition>().ToArray();
                foreach (var discoveredCodeElement in discoveredCodeElements.Where(discoveredCodeElement => discoveredCodeElement != null).Except(checkedCodeElements).Distinct())
                {
                    CheckCodeElement(discoveredCodeElement, checkedCodeElements);
                }
            }
        }

        private static IEnumerable<FileInfo> GetGacAssemblies()
        {
            var oldGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("systemroot")}\\assembly");
            var newGacDirectory = new DirectoryInfo($"{Environment.GetEnvironmentVariable("windir")}\\Microsoft.NET\\assembly");

            var gacAssemblies = new List<FileInfo>();
            if (oldGacDirectory.Exists)
            {
                gacAssemblies.AddRange(oldGacDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories));
            }

            if (newGacDirectory.Exists)
            {
                gacAssemblies.AddRange(newGacDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories));
            }

            return gacAssemblies.ToList();
        }

        private void OutputMetadataSummary(IEnumerable<ReflectionData> allMetadata)
        {
            var startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Count(reflectionData => !reflectionData.HasMetadata)} files without reflectionData ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.AssemblyFiles?.Count()).Sum()} AssemblyFiles ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.AssemblyReferences?.Count()).Sum()} AssemblyReferences ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.CustomDebugInformation?.Count()).Sum()} CustomDebugInformation ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.DeclarativeSecurityAttributes?.Count()).Sum()} DeclarativeSecurityAttributes ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.Documents?.Count()).Sum()} Documents ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.EventDefinitions?.Count()).Sum()} EventDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.ExportedTypes?.Count()).Sum()} ExportedTypes ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.FieldDefinitions?.Count()).Sum()} FieldDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.ImportScopes?.Count()).Sum()} ImportScopes ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.LocalConstants?.Count()).Sum()} LocalConstants ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.LocalScopes?.Count()).Sum()} LocalScopes ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.LocalVariables?.Count()).Sum()} LocalVariables ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.ManifestResources?.Count()).Sum()} ManifestResources ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.MemberReferences?.Count()).Sum()} MemberReferences ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.MethodDebugInformation?.Count()).Sum()} MethodDebugInformation ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.MethodDefinitions?.Count()).Sum()} MethodDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.PropertyDefinitions?.Count()).Sum()} PropertyDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.TypeDefinitions?.Count()).Sum()} TypeDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(reflectionData => reflectionData.TypeReferences?.Count()).Sum()} TypeReferences ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
        }

        private void SmokeTestOnDotNetFramework(bool prefetch)
        {
            var assemblyFiles = GetGacAssemblies().Take(10);
            var startTime = DateTime.Now;
            var exceptions = new ConcurrentDictionary<FileInfo, Exception>();
            Parallel.ForEach(assemblyFiles, assemblyFile =>
            {
                try
                {
                    using (var reflectionData = new ReflectionData(prefetch, assemblyFile))
                    {
                        AssertValid(reflectionData);
                    }
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

        [Fact]
        public void SmokeTestOnDotNetFrameworkWithoutPrefetch() => SmokeTestOnDotNetFramework(false);

        [Fact]
        public void SmokeTestOnDotNetFrameworkWithPrefetch() => SmokeTestOnDotNetFramework(true);

        [Fact]
        public void TestOnOwnAssemblyAndPdbWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
            AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        public void TestOnOwnAssemblyAndPdbWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
            AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        public void TestOnOwnAssemblyWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
        }

        [Fact]
        public void TestOnOwnAssemblyWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
        }

        [Fact]
        public void TestOnOwnPdbWithoutPrefetch()
        {
            var reflectionData = new ReflectionData(null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(reflectionData);
            AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
        public void TestOnOwnPdbWithPrefetch()
        {
            var reflectionData = new ReflectionData(true, null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(reflectionData);
            AssertHasDebugMetadata(reflectionData);
        }

        [Fact]
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
        public void TestOnSampleAssembly()
        {
            var reflectionData = new ReflectionData(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll")));
            AssertValid(reflectionData);
            AssertHasMetadata(reflectionData);
        }
    }
}
