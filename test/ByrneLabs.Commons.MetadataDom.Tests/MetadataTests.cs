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

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        private static void AssertHasDebugMetadata(Metadata metadata) => Assert.True(metadata.Documents.Any());

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        private static void AssertHasMetadata(Metadata metadata)
        {
            Assert.NotNull(metadata.AssemblyDefinition);
            Assert.True(metadata.TypeDefinitions.Any());
        }

        private void AssertValid(Metadata metadata)
        {
            CheckCodeElement(metadata, new List<CodeElement>());
            OutputMetadataSummary(new[] { metadata });
        }

        private static void CheckCodeElement(CodeElement codeElement, ICollection<CodeElement> checkedCodeElements)
        {
            if (!checkedCodeElements.Contains(codeElement))
            {
                checkedCodeElements.Add(codeElement);
                foreach (var property in codeElement.GetType().GetTypeInfo().GetProperties())
                {
                    var propertyValue = property.GetValue(codeElement);
                    var codeElementPropertyValue = propertyValue as CodeElement;
                    var codeElementsPropertyValue = propertyValue as IEnumerable;
                    if (codeElementPropertyValue != null)
                    {
                        CheckCodeElement(codeElementPropertyValue, checkedCodeElements);
                    }
                    else if (codeElementsPropertyValue?.GetType().IsConstructedGenericType == true && typeof(CodeElement).GetTypeInfo().IsAssignableFrom(codeElementsPropertyValue.GetType().GetTypeInfo().GetGenericArguments().First()))
                    {
                        foreach (var childCodeElement in codeElementsPropertyValue.Cast<CodeElement>())
                        {
                            CheckCodeElement(childCodeElement, checkedCodeElements);
                        }
                    }
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

        private void OutputMetadataSummary(IEnumerable<Metadata> allMetadata)
        {
            var startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Count(metadata => !metadata.HasMetadata)} files without metadata ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.AssemblyFiles?.Count).Sum()} AssemblyFiles ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.AssemblyReferences?.Count).Sum()} AssemblyReferences ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.CustomDebugInformation?.Count).Sum()} CustomDebugInformation ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.DeclarativeSecurityAttributes?.Count).Sum()} DeclarativeSecurityAttributes ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.Documents?.Count).Sum()} Documents ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.EventDefinitions?.Count).Sum()} EventDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.ExportedTypes?.Count).Sum()} ExportedTypes ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.FieldDefinitions?.Count).Sum()} FieldDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.ImportScopes?.Count).Sum()} ImportScopes ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.LocalConstants?.Count).Sum()} LocalConstants ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.LocalScopes?.Count).Sum()} LocalScopes ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.LocalVariables?.Count).Sum()} LocalVariables ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.ManifestResources?.Count).Sum()} ManifestResources ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.MemberReferences?.Count).Sum()} MemberReferences ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.MethodDebugInformation?.Count).Sum()} MethodDebugInformation ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.MethodDefinitions?.Count).Sum()} MethodDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.PropertyDefinitions?.Count).Sum()} PropertyDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.TypeDefinitions?.Count).Sum()} TypeDefinitions ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
            startTime = DateTime.Now;
            _output.WriteLine($"\t{allMetadata.Select(metadata => metadata.TypeReferences?.Count).Sum()} TypeReferences ({DateTime.Now.Subtract(startTime).TotalSeconds} seconds)");
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
                    using (var metadata = new Metadata(prefetch, assemblyFile))
                    {
                        AssertValid(metadata);
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
            var metadata = new Metadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(metadata);
            AssertHasMetadata(metadata);
            AssertHasDebugMetadata(metadata);
        }

        [Fact]
        public void TestOnOwnAssemblyAndPdbWithPrefetch()
        {
            var metadata = new Metadata(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")), new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(metadata);
            AssertHasMetadata(metadata);
            AssertHasDebugMetadata(metadata);
        }

        [Fact]
        public void TestOnOwnAssemblyWithoutPrefetch()
        {
            var metadata = new Metadata(new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            AssertValid(metadata);
            AssertHasMetadata(metadata);
        }

        [Fact]
        public void TestOnOwnAssemblyWithPrefetch()
        {
            var metadata = new Metadata(true, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.dll")));
            AssertValid(metadata);
            AssertHasMetadata(metadata);
        }

        [Fact]
        public void TestWithOwnPdbWithoutPrefetch()
        {
            var metadata = new Metadata(null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(metadata);
            AssertHasDebugMetadata(metadata);
        }

        [Fact]
        public void TestWithOwnPdbWithPrefetch()
        {
            var metadata = new Metadata(true, null, new FileInfo(Path.Combine(AppContext.BaseDirectory, "ByrneLabs.Commons.MetadataDom.pdb")));
            AssertValid(metadata);
            AssertHasDebugMetadata(metadata);
        }
    }
}
