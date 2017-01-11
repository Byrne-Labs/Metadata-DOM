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
    public class MetadataChecker
    {

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        public static void AssertHasDebugMetadata(ReflectionData reflectionData) => Assert.True(reflectionData.Documents.Any());

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "It is an assert method using the variable only for asserts makes sense")]
        public static void AssertHasMetadata(ReflectionData reflectionData)
        {
            Assert.NotNull(reflectionData.AssemblyDefinition);
            Assert.True(reflectionData.TypeDefinitions.Any());
        }

        public static void AssertValid(ReflectionData reflectionData)
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

        public static void CheckCodeElement(CodeElement codeElement, ICollection<CodeElement> checkedMetadataElements, bool excludeAssemblies)
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

    }
}
