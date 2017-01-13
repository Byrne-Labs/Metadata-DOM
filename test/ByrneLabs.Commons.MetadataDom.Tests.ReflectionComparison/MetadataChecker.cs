using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison
{
    public class MetadataChecker
    {
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
                        var codeElementsPropertyValue = (propertyValue as IEnumerable)?.Cast<object>();
                        if (codeElementPropertyValue != null)
                        {
                            discoveredCodeElements.Add(codeElementPropertyValue);
                        }
                        else if (codeElementsPropertyValue != null && codeElementsPropertyValue.Any() && codeElementsPropertyValue.GetType().IsConstructedGenericType && typeof(CodeElement).GetTypeInfo().IsAssignableFrom(codeElementsPropertyValue.GetType().GetTypeInfo().GetGenericArguments().First()))
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

        public static void CheckMetadata(Metadata metadata)
        {
            var checkedMetadataElements = new List<CodeElement>();
            /*
             * While not necessary, checking the declared types first makes debugging easier. -- Jonathan Byrne 12/17/2016
             */
            foreach (var typeDefinition in metadata.TypeDefinitions)
            {
                CheckCodeElement(typeDefinition, checkedMetadataElements, true);
            }

            CheckCodeElement(metadata, checkedMetadataElements, false);
        }
    }
}
