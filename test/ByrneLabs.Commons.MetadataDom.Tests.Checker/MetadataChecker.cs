using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ByrneLabs.Commons.MetadataDom.TypeSystem;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    internal class MetadataChecker
    {
        private const int MAX_PROPERTY_ERRORS = 10;
        private readonly CheckState _checkState;
        /*
         * Some properties exists in .NET Core 2.0 but not .NET Standard 2.0.  This means that we do not override it in our .NET Standard class and the .NET Core default implementation throws an exception when called with reflection. -- Jonathan Byrne 05/26/2017
         */
        private readonly string[] _coreOnlyProperties = { "IsSZArray", "IsVariableBoundArray" };
        private readonly List<MemberInfo> _ignoredMembers = new List<MemberInfo>();
        private readonly Dictionary<MemberInfo, int> _memberErrorCount = new Dictionary<MemberInfo, int>();

        public MetadataChecker(CheckState checkState)
        {
            _checkState = checkState;
        }

        public void Check(bool checkTypes, bool checkSymbols)
        {
            if (checkTypes)
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

            if (_checkState.Metadata.HasDebugMetadata && checkSymbols)
            {
                foreach (var methodDefinition in _checkState.Metadata.MethodDefinitions.Where(methodDefinition => methodDefinition.DebugInformation?.SequencePointsBlob?.Bytes?.Any() == true && methodDefinition.SequencePoints != null && !methodDefinition.SequencePoints.Any()))
                {
                    _checkState.AddError($"No sequence points loaded for method {methodDefinition.FullName} for {_checkState.Metadata.AssemblyFile.DirectoryName}");
                }
                foreach (var methodDefinition in _checkState.Metadata.MethodDefinitions.Where(methodDefinition => methodDefinition.DebugInformation?.SequencePointsBlob?.Bytes?.Any() == true && methodDefinition.SourceCode == null))
                {
                    _checkState.AddError($"No source code loaded for method {methodDefinition.FullName} for {_checkState.Metadata.AssemblyFile.DirectoryName}");
                }
                foreach (var constructorDefinition in _checkState.Metadata.ConstructorDefinitions.Where(constructorDefinition => constructorDefinition.DebugInformation?.SequencePointsBlob?.Bytes?.Any() == true && constructorDefinition.SequencePoints != null && !constructorDefinition.SequencePoints.Any()))
                {
                    _checkState.AddError($"No sequence points loaded for method {constructorDefinition.FullName} for {_checkState.Metadata.AssemblyFile.DirectoryName}");
                }
                foreach (var constructorDefinition in _checkState.Metadata.ConstructorDefinitions.Where(constructorDefinition => constructorDefinition.DebugInformation?.SequencePointsBlob?.Bytes?.Any() == true && constructorDefinition.SourceCode == null))
                {
                    _checkState.AddError($"No source code loaded for method {constructorDefinition.FullName} for {_checkState.Metadata.AssemblyFile.DirectoryName}");
                }
            }
        }

        private void CheckCodeElement(IManagedCodeElement codeElement, bool excludeAssemblies)
        {
            if (!_checkState.HasBeenChecked(codeElement))
            {
                var discoveredCodeElements = new List<IManagedCodeElement>();
                foreach (var property in codeElement.GetType().GetTypeInfo().GetProperties().Except(_ignoredMembers).Where(property => !_coreOnlyProperties.Contains(property.Name)).Cast<System.Reflection.PropertyInfo>())
                {
                    try
                    {
                        var propertyValue = property.GetValue(codeElement);
                        var codeElementPropertyValue = propertyValue as IManagedCodeElement;
                        var codeElementsPropertyValue = (propertyValue as IEnumerable)?.OfType<IManagedCodeElement>();
                        /*
                         * The following code is horribly hackish but ImmutableArrays do not support most LINQ methods and as generic structs, are a little clumsy to handle. -- Jonathan Byrne 01/21/2017
                         */
                        if ((propertyValue?.GetType().FullName).StartsWith("System.Collections.Immutable.ImmutableArray`1", StringComparison.Ordinal) && typeof(IManagedCodeElement).GetTypeInfo().IsAssignableFrom(propertyValue.GetType().GenericTypeArguments[0]))
                        {
                        }
                        if (codeElementsPropertyValue != null)
                        {
                            discoveredCodeElements.AddRange(codeElementsPropertyValue);
                        }
                        else if (codeElementPropertyValue != null)
                        {
                            discoveredCodeElements.Add(codeElementPropertyValue);
                        }
                    }
                    catch (Exception exception)
                    {
                        RecordMemberError(exception, codeElement, property);
                    }
                }
                foreach (var discoveredCodeElement in discoveredCodeElements.Where(discoveredCodeElement => discoveredCodeElement != null && !(excludeAssemblies && discoveredCodeElement is AssemblyDefinition)).Except(_checkState.CheckedMetadataElements).Distinct())
                {
                    CheckCodeElement(discoveredCodeElement, excludeAssemblies);
                }
            }
        }

        private void RecordMemberError(Exception exception, IManagedCodeElement codeElement, MemberInfo property)
        {
            Exception realException;
            var targetInvocationException = exception as TargetInvocationException;
            if (targetInvocationException != null)
            {
                realException = exception.InnerException;
            }
            else
            {
                realException = exception;
            }
            if (realException is NotSupportedException)
            {
                //We can ignore not supported exceptions
                _ignoredMembers.Add(property);
            }
            else if (!(realException is InvalidOperationException))
            {
                _checkState.AddException(realException, codeElement, CheckPhase.MetadataCheck);
            }

            if (!_memberErrorCount.ContainsKey(property))
            {
                _memberErrorCount.Add(property, 0);
            }
            _memberErrorCount[property]++;

            if (_memberErrorCount[property] > MAX_PROPERTY_ERRORS || realException is NotImplementedException)
            {
                _ignoredMembers.Add(property);
            }
        }
    }
}
