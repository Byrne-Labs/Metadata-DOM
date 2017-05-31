using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using ByrneLabs.Commons.MetadataDom.TypeSystem;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    internal class ReflectionComparison
    {
        private static readonly Dictionary<Tuple<Type, Type>, IEnumerable<Tuple<System.Reflection.PropertyInfo, System.Reflection.PropertyInfo>>> _propertiesToCompare = new Dictionary<Tuple<Type, Type>, IEnumerable<Tuple<System.Reflection.PropertyInfo, System.Reflection.PropertyInfo>>>();
        public readonly CheckState _checkState;

        public ReflectionComparison(CheckState checkState)
        {
            _checkState = checkState;
        }

        private static IEnumerable<Tuple<System.Reflection.PropertyInfo, System.Reflection.PropertyInfo>> FindPropertiesToCompare(Type metadataType, Type reflectionType)
        {
            var metadataTypeInfo = metadataType.GetTypeInfo();
            var reflectionTypeInfo = reflectionType.GetTypeInfo();
            lock (_propertiesToCompare)
            {
                var key = new Tuple<Type, Type>(metadataType, reflectionType);
                if (!_propertiesToCompare.ContainsKey(key))
                {
                    var allProperties = metadataTypeInfo.GetProperties().Select(property => property.Name).Intersect(reflectionTypeInfo.GetProperties().Select(property => property.Name));
                    var properties = (
                        from propertyName in allProperties
                        let metadataPropertyInfo = metadataTypeInfo.GetProperty(propertyName)
                        let reflectionPropertyInfo = reflectionTypeInfo.GetProperty(propertyName)
                        where metadataPropertyInfo.PropertyType == reflectionPropertyInfo.PropertyType || typeof(IManagedCodeElement).GetTypeInfo().IsAssignableFrom(metadataPropertyInfo.PropertyType) || typeof(MemberInfo).GetTypeInfo().IsAssignableFrom(reflectionPropertyInfo.PropertyType)
                        select new Tuple<System.Reflection.PropertyInfo, System.Reflection.PropertyInfo>(metadataPropertyInfo, reflectionPropertyInfo)).ToList();

                    _propertiesToCompare.Add(key, properties);
                }

                return _propertiesToCompare[key];
            }
        }

        public void Check()
        {
            try
            {
                CompareCodeElementsToReflectionData(_checkState.Metadata.AssemblyDefinition, _checkState.Assembly);
                foreach (var reflectionType in _checkState.Assembly.DefinedTypes.Where(type => Equals(type.Assembly, _checkState.Assembly)))
                {
                    try
                    {
                        var metadataType = _checkState.Metadata.TypeDefinitions.SingleOrDefault(codeElement => codeElement.MetadataToken == reflectionType.MetadataToken) ?? _checkState.Metadata.TypeDefinitions.SingleOrDefault(codeElement => codeElement.MemberType == reflectionType.MemberType && codeElement.FullName.Equals(reflectionType.FullName));
                        if (metadataType == null)
                        {
                            _checkState.AddError($"Could not find type {reflectionType.FullName} with metadata");
                        }
                        else
                        {
                            CompareCodeElementsToReflectionData(metadataType, reflectionType);
                        }
                    }
                    catch (Exception exception)
                    {
                        _checkState.AddException(exception, reflectionType, CheckPhase.ReflectionComparison);
                    }
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, _checkState.Assembly, CheckPhase.ReflectionComparison);
            }

            try
            {
                foreach (var module in _checkState.Assembly.Modules)
                {
                    CompareCodeElementsToReflectionData(_checkState.Metadata.ModuleDefinition, module);
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, _checkState.Assembly, CheckPhase.ReflectionComparison);
            }

            _checkState.AddErrors(_checkState.Metadata.MemberDefinitions.Except(_checkState.ComparedMetadataMembers).Select(member => $"Could not find {member.MemberType} {member.TextSignature} with reflection"));
        }

        private void CompareCodeElementsToReflectionData(IManagedCodeElement metadataElement, object reflectionElement)
        {
            if (metadataElement is TypeBase && reflectionElement is TypeInfo)
            {
                CompareCodeElementsToReflectionData((TypeDefinition) metadataElement, (TypeInfo) reflectionElement);
            }
            else if (metadataElement is TypeBase && reflectionElement is Type)
            {
                CompareCodeElementsToReflectionData((TypeDefinition) metadataElement, ((Type) reflectionElement).GetTypeInfo());
            }
            else if (metadataElement is PropertyDefinition && reflectionElement is PropertyInfo)
            {
                CompareCodeElementsToReflectionData((PropertyDefinition) metadataElement, (PropertyInfo) reflectionElement);
            }
            else if (metadataElement is ConstructorDefinition && reflectionElement is ConstructorInfo)
            {
                CompareCodeElementsToReflectionData((ConstructorDefinition) metadataElement, (ConstructorInfo) reflectionElement);
            }
            else if (metadataElement is MethodDefinition && reflectionElement is MethodInfo)
            {
                CompareCodeElementsToReflectionData((MethodDefinition) metadataElement, (MethodInfo) reflectionElement);
            }
            else if (metadataElement is EventDefinition && reflectionElement is EventInfo)
            {
                CompareCodeElementsToReflectionData((EventDefinition) metadataElement, (EventInfo) reflectionElement);
            }
            else if (metadataElement is FieldDefinition && reflectionElement is FieldInfo)
            {
                CompareCodeElementsToReflectionData((FieldDefinition) metadataElement, (FieldInfo) reflectionElement);
            }
            else if (metadataElement is CustomAttributeData && reflectionElement is CustomAttributeData)
            {
                CompareCodeElementsToReflectionData((CustomAttributeData) metadataElement, (CustomAttributeData) reflectionElement);
            }
            else if (metadataElement is Parameter && reflectionElement is ParameterInfo)
            {
                CompareCodeElementsToReflectionData((Parameter) metadataElement, (ParameterInfo) reflectionElement);
            }
            else if (metadataElement is ModuleDefinition && reflectionElement is Module)
            {
                CompareCodeElementsToReflectionData((ModuleDefinition) metadataElement, (Module) reflectionElement);
            }
            else if (metadataElement is AssemblyDefinition && reflectionElement is Assembly)
            {
                CompareCodeElementsToReflectionData((AssemblyDefinition) metadataElement, (Assembly) reflectionElement);
            }
            else
            {
                _checkState.AddError($"Could not handle metadata element type {metadataElement.GetType().FullName} and reflection element type {reflectionElement.GetType().FullName}");
            }
        }

        private void CompareCodeElementsToReflectionData(TypeDefinition metadataType, TypeInfo reflectionType)
        {
            if (_checkState.HaveBeenCompared(metadataType, reflectionType))
            {
                return;
            }

            try
            {
                var metadataElementType = metadataType.ElementType;
                var reflectionElementType = reflectionType.GetElementType();
                if (metadataElementType != null && reflectionElementType != null)
                {
                    CompareCodeElementsToReflectionData((IManagedCodeElement) metadataElementType, reflectionElementType);
                }
                else if (metadataElementType != null || reflectionElementType != null)
                {
                    _checkState.AddError($"{metadataType.FullName}.ElementType has a value of {metadataElementType} in metadata but a value of {reflectionElementType} in reflection");
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, metadataType, CheckPhase.ReflectionComparison);
            }
            try
            {
                if (reflectionType.IsArray && metadataType.ArrayRank != reflectionType.GetArrayRank())
                {
                    _checkState.AddError($"{metadataType.FullName}.ArrayRank has a value of {metadataType.ArrayRank} in metadata but a value of {reflectionType.GetArrayRank()} in reflection");
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, metadataType, CheckPhase.ReflectionComparison);
            }

            try
            {
                foreach (var reflectionMember in reflectionType.DeclaredMembers)
                {
                    var byToken = metadataType.GetMembers().Where(member => member.MetadataToken == reflectionMember.MetadataToken).ToArray();
                    var reflectionTextSignature = SignatureCreater.GetTextSignature(reflectionType, reflectionMember);
                    var byName = metadataType.GetMembers().Where(member => member.MemberType == reflectionMember.MemberType && member.GetTextSignature().Equals(reflectionTextSignature)).ToArray();
                    if (byToken.Length == 1 && byName.Length == 1)
                    {
                        CompareCodeElementsToReflectionData((IManagedCodeElement) byToken.Single(), reflectionMember);
                    }
                    else
                    {
                        _checkState.ComparedMetadataMembers.AddRange(byToken.Union(byName).Cast<IMemberInfo>());
                        _checkState.ComparedReflectionMembers.Add(reflectionMember);
                        if (byToken.Length == 0 && byName.Length == 0)
                        {
                            _checkState.AddError($"Could not find {reflectionMember.MemberType} {reflectionTextSignature} with metadata by token or name");
                        }
                        else if (byToken.Length > 1)
                        {
                            _checkState.AddError($"Found multiple {reflectionMember.MemberType} {reflectionTextSignature} with metadata by token");
                        }
                        else if (byName.Length > 1)
                        {
                            _checkState.AddError($"Found multiple {reflectionMember.MemberType} {reflectionTextSignature} with metadata by name");
                        }
                        else if (byToken.Length == 0)
                        {
                            _checkState.AddError($"Found {reflectionMember.MemberType} {reflectionTextSignature} in metadata with the metadata token {byName[0].MetadataToken} instead of {reflectionMember.MetadataToken}");
                        }
                        else
                        {
                            _checkState.AddError($"Found {reflectionMember.MemberType} {reflectionTextSignature} in metadata with the name {byToken[0].GetTextSignature()}");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, metadataType, CheckPhase.ReflectionComparison);
            }

            CompareElementProperties(metadataType.FullNameWithoutAssemblies, metadataType, reflectionType);
        }

        private void CompareCodeElementsToReflectionData(Parameter metadataParameter, ParameterInfo reflectionParameter)
        {
            if (_checkState.HaveBeenCompared(metadataParameter, reflectionParameter))
            {
                return;
            }

            CompareElementProperties(metadataParameter.FullName, metadataParameter, reflectionParameter);
        }

        private void CompareCodeElementsToReflectionData(CustomAttributeData metadataAttribute, CustomAttributeData reflectionAttribute)
        {
            if (_checkState.HaveBeenCompared((IManagedCodeElement) metadataAttribute, reflectionAttribute))
            {
                return;
            }

            CompareElementProperties(metadataAttribute.AttributeType.FullName, metadataAttribute, reflectionAttribute);
        }

        private void CompareCodeElementsToReflectionData(PropertyDefinition metadataProperty, PropertyInfo reflectionProperty)
        {
            if (_checkState.HaveBeenCompared(metadataProperty, reflectionProperty))
            {
                return;
            }

            CompareTypes($"Property {metadataProperty.FullName}", (TypeBase) metadataProperty.PropertyType, reflectionProperty.PropertyType);

            CompareElementProperties(metadataProperty.FullName, metadataProperty, reflectionProperty);
        }

        private void CompareCodeElementsToReflectionData(MethodBase metadataMethodBase, MethodBase reflectionMethodBase)
        {
            if (_checkState.HaveBeenCompared((IManagedCodeElement) metadataMethodBase, reflectionMethodBase))
            {
                return;
            }

            if (reflectionMethodBase.GetParameters().Length != metadataMethodBase.GetParameters().Count())
            {
                _checkState.AddError($"{metadataMethodBase.GetTextSignature()} has {reflectionMethodBase.GetParameters().Length} parameters in reflection but {metadataMethodBase.GetParameters().Count()} in metadata");
            }
            try
            {
                foreach (var reflectionParameter in reflectionMethodBase.GetParameters())
                {
                    var metadataParameter = metadataMethodBase.GetParameters().SingleOrDefault(parameter => parameter.Position == reflectionParameter.Position);
                    if (metadataParameter == null)
                    {
                        _checkState.AddError($"The parameter named {reflectionParameter.Name} with position {reflectionParameter.Position} on {metadataMethodBase.GetTextSignature()} could not be found in metadata");
                        continue;
                    }

                    CompareTypes($"The parameter named {reflectionParameter.Name} with position {reflectionParameter.Position} on {metadataMethodBase.GetTextSignature()}", (TypeBase) metadataParameter.ParameterType, reflectionParameter.ParameterType);
                    CompareElementProperties(((Parameter) metadataParameter).FullName, metadataParameter, reflectionParameter);
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, metadataMethodBase, CheckPhase.ReflectionComparison);
            }

            CompareElementProperties(metadataMethodBase.GetTextSignature(), metadataMethodBase, reflectionMethodBase);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "We want a different method for each memember type")]
        private void CompareCodeElementsToReflectionData(EventDefinition metadataEvent, EventInfo reflectionEvent)
        {
            if (_checkState.HaveBeenCompared(metadataEvent, reflectionEvent))
            {
                return;
            }

            CompareElementProperties(metadataEvent.FullName, metadataEvent, reflectionEvent);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "We want a different method for each memember type")]
        private void CompareCodeElementsToReflectionData(FieldDefinition metadataField, FieldInfo reflectionField)
        {
            if (_checkState.HaveBeenCompared(metadataField, reflectionField))
            {
                return;
            }

            CompareElementProperties(metadataField.FullName, metadataField, reflectionField);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This method is only for ModuleDefinition clases")]
        private void CompareCodeElementsToReflectionData(ModuleDefinition metadataModule, Module reflectionModule)
        {
            if (_checkState.HaveBeenCompared(metadataModule, reflectionModule))
            {
                return;
            }

            CompareElementProperties("<module>", metadataModule, reflectionModule);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This method is only for AssemblyDefinition clases")]
        private void CompareCodeElementsToReflectionData(AssemblyDefinition metadataAssembly, Assembly reflectionAssembly)
        {
            if (_checkState.HaveBeenCompared(metadataAssembly, reflectionAssembly))
            {
                return;
            }

            CompareElementProperties("<module>", metadataAssembly, reflectionAssembly);
        }

        private void CompareElementProperties(string elementName, object metadataElement, object reflectionElement)
        {
            try
            {
                foreach (var propertyToCompare in FindPropertiesToCompare(metadataElement.GetType(), reflectionElement.GetType()))
                {
                    object metadataPropertyValue = null;
                    object reflectionPropertyValue = null;
                    var valueRetrievalFaulted = false;
                    try
                    {
                        metadataPropertyValue = propertyToCompare.Item1.GetValue(metadataElement);
                    }
                    catch (Exception exception)
                    {
                        valueRetrievalFaulted = true;
                        _checkState.AddException(exception, $"Property {propertyToCompare.Item1.Name} on {elementName} metadata", CheckPhase.ReflectionComparison);
                    }
                    try
                    {
                        reflectionPropertyValue = propertyToCompare.Item2.GetValue(reflectionElement);
                    }
                    catch (Exception exception)
                    {
                        valueRetrievalFaulted = true;
                        _checkState.AddException(exception, $"Property {propertyToCompare.Item2.Name} on {elementName} reflection", CheckPhase.ReflectionComparison);
                    }
                    if (!valueRetrievalFaulted)
                    {
                        if (propertyToCompare.Item1.PropertyType == propertyToCompare.Item2.PropertyType && !Equals(metadataPropertyValue, reflectionPropertyValue))
                        {
                            _checkState.AddError($"{elementName}.{propertyToCompare.Item1.Name} has a value of {metadataPropertyValue} in metadata but a value of {reflectionPropertyValue} in reflection");
                        }
                        else if (metadataElement is IManagedCodeElement)
                        {
                            CompareCodeElementsToReflectionData((IManagedCodeElement) metadataElement, reflectionElement);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Invalid property to compare {elementName}.{propertyToCompare.Item1.Name}");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, metadataElement, CheckPhase.ReflectionComparison);
            }
        }

        private void CompareTypes(string sourceName, TypeBase metadataType, Type reflectionType)
        {
            if (!Equals(metadataType.Namespace, reflectionType.Namespace))
            {
                _checkState.AddError($"{sourceName} reflection type namespace \"{reflectionType.Namespace}\" does not match metadata type namespace \"{metadataType.Namespace}\"");
            }
            if (!Equals(metadataType.Name, reflectionType.Name))
            {
                _checkState.AddError($"{sourceName} reflection type name \"{reflectionType.Name}\" does not match metadata type name \"{metadataType.Name}\"");
            }
        }
    }
}
