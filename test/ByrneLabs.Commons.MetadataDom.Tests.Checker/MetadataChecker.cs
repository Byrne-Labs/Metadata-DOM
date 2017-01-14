using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    public class MetadataChecker
    {

        private readonly CheckState _checkState;

        public MetadataChecker(CheckState checkState)
        {
            _checkState = checkState;
        }

        private void CheckCodeElement(CodeElement codeElement, bool excludeAssemblies)
        {
            if (!_checkState.HasBeenChecked(codeElement))
            {
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
                        else if (codeElementsPropertyValue?.Any() == true && codeElementsPropertyValue.GetType().IsConstructedGenericType && typeof(CodeElement).GetTypeInfo().IsAssignableFrom(codeElementsPropertyValue.GetType().GetTypeInfo().GetGenericArguments().First()))
                        {
                            discoveredCodeElements.AddRange(codeElementsPropertyValue.Cast<CodeElement>());
                        }
                    }
                    catch (Exception exception)
                    {
                        _checkState.AddException(exception, codeElement, CheckPhase.MetadataCheck);
                    }
                }
                foreach (var discoveredCodeElement in discoveredCodeElements.Where(discoveredCodeElement => discoveredCodeElement != null && !(excludeAssemblies && discoveredCodeElement is AssemblyDefinition)).Except(_checkState.CheckedMetadataElements).Distinct())
                {
                    CheckCodeElement(discoveredCodeElement, excludeAssemblies);
                }
            }
        }

        public void Check()
        {
            /*
             * While not necessary, checking the declared types first makes debugging easier. -- Jonathan Byrne 12/17/2016
             */
            foreach (var typeDefinition in _checkState.Metadata.TypeDefinitions)
            {
                CheckCodeElement(typeDefinition, true);
            }

            CheckCodeElement(_checkState.Metadata, false);
        }
    }
}
