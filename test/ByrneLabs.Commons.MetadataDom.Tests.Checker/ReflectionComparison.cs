using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using ByrneLabs.Commons.MetadataDom.TypeSystem;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    internal class ReflectionComparison
    {
        private static readonly Dictionary<Tuple<MemberInfo, MemberInfo, string>, int> _exceptionCount = new Dictionary<Tuple<MemberInfo, MemberInfo, string>, int>();
        private static readonly List<Tuple<MemberInfo, MemberInfo>> _ignoredMembers = new List<Tuple<MemberInfo, MemberInfo>>();
        private static readonly Dictionary<Tuple<Type, Type>, IEnumerable<Tuple<MemberInfo, MemberInfo>>> _membersToCompare = new Dictionary<Tuple<Type, Type>, IEnumerable<Tuple<MemberInfo, MemberInfo>>>();
        private static readonly string[] _ignoredMethodNames = { "GetHashCode", "GetType" };
        private static readonly string[] _ignoredPropertyNames = { "DeclaredMembers" };
        public readonly CheckState _checkState;

        public ReflectionComparison(CheckState checkState)
        {
            _checkState = checkState;
        }

        private static void CheckMetadataMemberException(Tuple<MemberInfo, MemberInfo> member, Exception exception)
        {
            lock (_ignoredMembers)
            {
                if (_ignoredMembers.Contains(member))
                {
                    return;
                }

                var targetInvocationException = exception as TargetInvocationException;
                var notSupportedException = targetInvocationException?.InnerException as NotSupportedException;
                if (notSupportedException != null && (
                        notSupportedException.Message.Equals(NotSupportedHelper.FutureVersion().Message) ||
                        notSupportedException.Message.Equals(NotSupportedHelper.NotValidForMetadata().Message) ||
                        notSupportedException.Message.Equals(NotSupportedHelper.NotValidForMetadataType(member.Item1.DeclaringType).Message)))
                {
                    _ignoredMembers.Add(member);
                }
                else if (targetInvocationException != null)
                {
                    var key = new Tuple<MemberInfo, MemberInfo, string>(member.Item1, member.Item2, targetInvocationException.InnerException.ToString());
                    if (!_exceptionCount.ContainsKey(key))
                    {
                        _exceptionCount.Add(key, 0);
                    }
                    if (_exceptionCount[key] == 5)
                    {
                        _ignoredMembers.Add(member);
                    }
                    else
                    {
                        _exceptionCount[key]++;
                    }
                }
            }
        }

        private static IEnumerable<Tuple<MemberInfo, MemberInfo>> FindMembersToCompare(Type metadataType, Type reflectionType)
        {
            var metadataTypeInfo = metadataType.GetTypeInfo();
            var reflectionTypeInfo = reflectionType.GetTypeInfo();
            lock (_ignoredMembers)
                lock (_membersToCompare)
                {
                    var key = new Tuple<Type, Type>(metadataType, reflectionType);
                    if (!_membersToCompare.ContainsKey(key))
                    {
                        var allProperties = metadataTypeInfo.DeclaredProperties.Select(property => property.Name).Intersect(reflectionTypeInfo.DeclaredProperties.Select(property => property.Name)).Distinct().Except(_ignoredPropertyNames);
                        var properties = (
                            from propertyName in allProperties
                            let metadataPropertyInfo = metadataTypeInfo.GetProperty(propertyName)
                            let reflectionPropertyInfo = reflectionTypeInfo.GetProperty(propertyName)
                            where metadataPropertyInfo.PropertyType == reflectionPropertyInfo.PropertyType || typeof(IManagedCodeElement).GetTypeInfo().IsAssignableFrom(metadataPropertyInfo.PropertyType) || typeof(MemberInfo).GetTypeInfo().IsAssignableFrom(reflectionPropertyInfo.PropertyType)
                            select new Tuple<MemberInfo, MemberInfo>(metadataPropertyInfo, reflectionPropertyInfo)).ToList();

                        var allMethods = metadataTypeInfo.GetMethods().Where(method => method.GetParameters().Length == 0 && (!method.IsSpecialName || !method.Name.StartsWith("get_"))).Select(method => method.Name)
                            .Intersect(reflectionTypeInfo.GetMethods().Where(method => method.GetParameters().Length == 0 && (!method.IsSpecialName || !method.Name.StartsWith("get_"))).Select(method => method.Name)).Distinct().Except(_ignoredMethodNames);
                        var methods = (
                            from methodName in allMethods
                            let metadataMethodInfo = metadataTypeInfo.GetMethod(methodName, new Type[] { })
                            let reflectionMethodInfo = reflectionTypeInfo.GetMethod(methodName, new Type[] { })
                            where
                            metadataMethodInfo.ReturnType == reflectionMethodInfo.ReturnType ||
                            metadataMethodInfo.ReturnType != typeof(object) && metadataMethodInfo.ReturnType != typeof(object) && (metadataMethodInfo.ReturnType.IsAssignableFrom(reflectionMethodInfo.ReturnType) || reflectionMethodInfo.ReturnType.IsAssignableFrom(metadataMethodInfo.ReturnType)) ||
                            typeof(IManagedCodeElement).GetTypeInfo().IsAssignableFrom(metadataMethodInfo.ReturnType) || typeof(MemberInfo).GetTypeInfo().IsAssignableFrom(reflectionMethodInfo.ReturnType)
                            select new Tuple<MemberInfo, MemberInfo>(metadataMethodInfo, reflectionMethodInfo)).ToList();

                        _membersToCompare.Add(key, properties.Union(methods).ToArray());
                    }

                    return _membersToCompare[key].Except(_ignoredMembers);
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

            _checkState.AddErrors(_checkState.Metadata.MemberDefinitions.Except(_checkState.ComparedMetadataMembers).Select(member => $"Could not find {member.MemberType.ToString().ToLower()} {member.TextSignature} with reflection"));
        }

        private void CompareCodeElementsToReflectionData(IManagedCodeElement metadataElement, object reflectionElement)
        {
            if (metadataElement is TypeBase && reflectionElement is System.Reflection.TypeInfo)
            {
                CompareCodeElementsToReflectionData((TypeBase)metadataElement, (System.Reflection.TypeInfo)reflectionElement);
            }
            else if (metadataElement is TypeBase && reflectionElement is Type)
            {
                CompareCodeElementsToReflectionData((TypeBase)metadataElement, ((Type)reflectionElement).GetTypeInfo());
            }
            else if (metadataElement is PropertyDefinition && reflectionElement is System.Reflection.PropertyInfo)
            {
                CompareCodeElementsToReflectionData((PropertyDefinition)metadataElement, (System.Reflection.PropertyInfo)reflectionElement);
            }
            else if (metadataElement is ConstructorDefinition && reflectionElement is System.Reflection.ConstructorInfo)
            {
                CompareCodeElementsToReflectionData((ConstructorDefinition)metadataElement, (System.Reflection.ConstructorInfo)reflectionElement);
            }
            else if (metadataElement is MethodDefinition && reflectionElement is System.Reflection.MethodInfo)
            {
                CompareCodeElementsToReflectionData((MethodDefinition)metadataElement, (System.Reflection.MethodInfo)reflectionElement);
            }
            else if (metadataElement is EventDefinition && reflectionElement is System.Reflection.EventInfo)
            {
                CompareCodeElementsToReflectionData((EventDefinition)metadataElement, (System.Reflection.EventInfo)reflectionElement);
            }
            else if (metadataElement is FieldDefinition && reflectionElement is System.Reflection.FieldInfo)
            {
                CompareCodeElementsToReflectionData((FieldDefinition)metadataElement, (System.Reflection.FieldInfo)reflectionElement);
            }
            else if (metadataElement is CustomAttribute && reflectionElement is System.Reflection.CustomAttributeData)
            {
                CompareCodeElementsToReflectionData((CustomAttribute)metadataElement, (System.Reflection.CustomAttributeData)reflectionElement);
            }
            else if (metadataElement is Parameter && reflectionElement is System.Reflection.ParameterInfo)
            {
                CompareCodeElementsToReflectionData((Parameter)metadataElement, (System.Reflection.ParameterInfo)reflectionElement);
            }
            else if (metadataElement is ModuleDefinition && reflectionElement is System.Reflection.Module)
            {
                CompareCodeElementsToReflectionData((ModuleDefinition)metadataElement, (System.Reflection.Module)reflectionElement);
            }
            else if (metadataElement is AssemblyDefinition && reflectionElement is System.Reflection.Assembly)
            {
                CompareCodeElementsToReflectionData((AssemblyDefinition)metadataElement, (System.Reflection.Assembly)reflectionElement);
            }
            else
            {
                _checkState.AddError($"Could not handle metadata element type {metadataElement.GetType().FullName} and reflection element type {reflectionElement.GetType().FullName}");
            }
        }

        private void CompareCodeElementsToReflectionData(TypeBase metadataType, System.Reflection.TypeInfo reflectionType)
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
                    CompareCodeElementsToReflectionData((IManagedCodeElement)metadataElementType, reflectionElementType);
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
                    var byToken = metadataType.DeclaredMembers.Where(member => member.MetadataToken == reflectionMember.MetadataToken).ToArray();
                    var reflectionTextSignature = SignatureCreater.GetTextSignature(reflectionType, reflectionMember);
                    var byName = metadataType.DeclaredMembers.Where(member => member.MemberType == reflectionMember.MemberType && member.GetTextSignature().Equals(reflectionTextSignature)).ToArray();
                    if (byToken.Length == 1 && byName.Length == 1)
                    {
                        CompareCodeElementsToReflectionData((IManagedCodeElement)byToken.Single(), reflectionMember);
                    }
                    else
                    {
                        _checkState.ComparedMetadataMembers.AddRange(byToken.Union(byName).Cast<IMemberInfo>());
                        _checkState.ComparedReflectionMembers.Add(reflectionMember);
                        if (byToken.Length == 0 && byName.Length == 0)
                        {
                            _checkState.AddError($"Could not find {reflectionMember.MemberType.ToString().ToLower()} \"{reflectionTextSignature}\" with metadata by token or name");
                        }
                        else if (byToken.Length > 1)
                        {
                            _checkState.AddError($"Found multiple {reflectionMember.MemberType.ToString().ToLower()} \"{reflectionTextSignature}\" with metadata by token");
                        }
                        else if (byName.Length > 1)
                        {
                            _checkState.AddError($"Found multiple {reflectionMember.MemberType.ToString().ToLower()} \"{reflectionTextSignature}\" with metadata by name");
                        }
                        else if (byToken.Length == 0)
                        {
                            _checkState.AddError($"Found {reflectionMember.MemberType.ToString().ToLower()} \"{reflectionTextSignature}\" in metadata with the metadata token {byName[0].MetadataToken} instead of {reflectionMember.MetadataToken}");
                        }
                        else
                        {
                            _checkState.AddError($"Found {reflectionMember.MemberType.ToString().ToLower()} \"{reflectionTextSignature}\" in metadata with the name {byToken[0].GetTextSignature()}");
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

        private void CompareCodeElementsToReflectionData(Parameter metadataParameter, System.Reflection.ParameterInfo reflectionParameter)
        {
            if (_checkState.HaveBeenCompared(metadataParameter, reflectionParameter))
            {
                return;
            }

            CompareElementProperties(metadataParameter.FullName, metadataParameter, reflectionParameter);
        }

        private void CompareCodeElementsToReflectionData(CustomAttribute metadataAttribute, System.Reflection.CustomAttributeData reflectionAttribute)
        {
            if (_checkState.HaveBeenCompared(metadataAttribute, reflectionAttribute))
            {
                return;
            }

            CompareElementProperties(metadataAttribute.AttributeType.FullName, metadataAttribute, reflectionAttribute);
        }

        private void CompareCodeElementsToReflectionData(PropertyDefinition metadataProperty, System.Reflection.PropertyInfo reflectionProperty)
        {
            if (_checkState.HaveBeenCompared(metadataProperty, reflectionProperty))
            {
                return;
            }

            CompareTypes($"Property {metadataProperty.FullName}", (TypeBase)metadataProperty.PropertyType, reflectionProperty.PropertyType);

            CompareElementProperties(metadataProperty.FullName, metadataProperty, reflectionProperty);
        }

        private void CompareCodeElementsToReflectionData(MethodBase metadataMethodBase, MethodBase reflectionMethodBase)
        {
            if (_checkState.HaveBeenCompared((IManagedCodeElement)metadataMethodBase, reflectionMethodBase))
            {
                return;
            }

            if (reflectionMethodBase.GetParameters().Length != metadataMethodBase.GetParameters().Length)
            {
                _checkState.AddError($"{metadataMethodBase.GetTextSignature()} has {reflectionMethodBase.GetParameters().Length} parameters in reflection but {metadataMethodBase.GetParameters().Length} in metadata");
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

                    CompareTypes($"The parameter named {reflectionParameter.Name} with position {reflectionParameter.Position} on {metadataMethodBase.GetTextSignature()}", (TypeBase)metadataParameter.ParameterType, reflectionParameter.ParameterType);
                    CompareElementProperties(((Parameter)metadataParameter).FullName, (IManagedCodeElement)metadataParameter, reflectionParameter);
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, metadataMethodBase, CheckPhase.ReflectionComparison);
            }

            CompareElementProperties(metadataMethodBase.GetTextSignature(), (IManagedCodeElement)metadataMethodBase, reflectionMethodBase);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "We want a different method for each memember type")]
        private void CompareCodeElementsToReflectionData(EventDefinition metadataEvent, System.Reflection.EventInfo reflectionEvent)
        {
            if (_checkState.HaveBeenCompared(metadataEvent, reflectionEvent))
            {
                return;
            }

            CompareElementProperties(metadataEvent.FullName, metadataEvent, reflectionEvent);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "We want a different method for each memember type")]
        private void CompareCodeElementsToReflectionData(FieldDefinition metadataField, System.Reflection.FieldInfo reflectionField)
        {
            if (_checkState.HaveBeenCompared(metadataField, reflectionField))
            {
                return;
            }

            CompareElementProperties(metadataField.FullName, metadataField, reflectionField);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This method is only for ModuleDefinition clases")]
        private void CompareCodeElementsToReflectionData(ModuleDefinition metadataModule, System.Reflection.Module reflectionModule)
        {
            if (_checkState.HaveBeenCompared(metadataModule, reflectionModule))
            {
                return;
            }

            CompareElementProperties("<module>", metadataModule, reflectionModule);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This method is only for AssemblyDefinition clases")]
        private void CompareCodeElementsToReflectionData(AssemblyDefinition metadataAssembly, System.Reflection.Assembly reflectionAssembly)
        {
            if (_checkState.HaveBeenCompared(metadataAssembly, reflectionAssembly))
            {
                return;
            }

            CompareElementProperties("<module>", metadataAssembly, reflectionAssembly);
        }

        private void CompareCodeElementsToReflectionData(string elementName, IEnumerable<object> metadataEnumerable, IEnumerable<object> reflectionEnumerable)
        {
            if (metadataEnumerable.Count() != reflectionEnumerable.Count())
            {
                _checkState.AddError($"{elementName} has {metadataEnumerable.Count()} items in metadata but {reflectionEnumerable.Count()} items in reflection");
            }
            else
            {
                for (var index = 0; index < metadataEnumerable.Count(); index++)
                {
                    var metadataItem = metadataEnumerable.Skip(index).First();
                    if (metadataItem is IManagedCodeElement)
                    {
                        var reflectionItem = reflectionEnumerable.Skip(index).First();
                        CompareCodeElementsToReflectionData((IManagedCodeElement)metadataItem, reflectionItem);
                    }
                }
            }
        }

        private void CompareElementProperties(string elementName, IManagedCodeElement metadataElement, object reflectionElement)
        {
            try
            {
                var membersToCompare = FindMembersToCompare(metadataElement.GetType(), reflectionElement.GetType());
                var propertiesToCompare = membersToCompare.Where(member => member.Item1 is System.Reflection.PropertyInfo).ToArray();
                foreach (var propertyToCompare in propertiesToCompare)
                {
                    object metadataValue = null;
                    object reflectionValue = null;
                    Exception metadataException = null;
                    try
                    {
                        metadataValue = ((System.Reflection.PropertyInfo)propertyToCompare.Item1).GetValue(metadataElement);
                    }
                    catch (Exception exception)
                    {
                        CheckMetadataMemberException(propertyToCompare, exception);
                        metadataException = exception;
                    }
                    Exception reflectionException = null;
                    try
                    {
                        reflectionValue = ((System.Reflection.PropertyInfo)propertyToCompare.Item2).GetValue(reflectionElement);
                    }
                    catch (Exception exception)
                    {
                        reflectionException = exception;
                    }
                    if (CompareExceptions($"{elementName}.{propertyToCompare.Item1.Name}", metadataException, reflectionException))
                    {
                        CompareElementValues($"{elementName}.{propertyToCompare.Item1.Name}", metadataElement, reflectionElement, metadataValue, reflectionValue);
                    }
                }

                var methodsToCompare = membersToCompare.Where(member => member.Item1 is System.Reflection.MethodInfo).ToArray();
                foreach (var methodToCompare in methodsToCompare)
                {
                    object metadataValue = null;
                    object reflectionValue = null;
                    Exception metadataException = null;
                    try
                    {
                        metadataValue = ((System.Reflection.MethodInfo)methodToCompare.Item1).Invoke(metadataElement, null);
                    }
                    catch (Exception exception)
                    {
                        CheckMetadataMemberException(methodToCompare, exception);
                        metadataException = exception;
                    }
                    Exception reflectionException = null;
                    try
                    {
                        reflectionValue = ((System.Reflection.MethodInfo)methodToCompare.Item2).Invoke(reflectionElement, null);
                    }
                    catch (Exception exception)
                    {
                        reflectionException = exception;
                    }
                    if (CompareExceptions($"{elementName}.{methodToCompare.Item1.Name}()", metadataException, reflectionException))
                    {
                        CompareElementValues($"{elementName}.{methodToCompare.Item1.Name}()", metadataElement, reflectionElement, metadataValue, reflectionValue);
                    }
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, metadataElement, CheckPhase.ReflectionComparison);
            }
        }

        private void CompareElementValues(string elementName, IManagedCodeElement metadataElement, object reflectionElement, object metadataValue, object reflectionValue)
        {
            if (metadataValue is IEnumerable && reflectionValue is IEnumerable && !(metadataValue is string || reflectionValue is string))
            {
                CompareCodeElementsToReflectionData(elementName, ((IEnumerable)metadataValue).Cast<object>(), ((IEnumerable)reflectionValue).Cast<object>());
            }
            else if (!(metadataValue is IManagedCodeElement) && metadataValue?.GetType() == reflectionValue?.GetType() && !Equals(metadataValue, reflectionValue))
            {
                _checkState.AddError($"{elementName} has a value of {metadataValue} in metadata but a value of {reflectionValue} in reflection");
            }
            else
            {
                CompareCodeElementsToReflectionData(metadataElement, reflectionElement);
            }
        }

        private bool CompareExceptions(string elementName, Exception metadataException, Exception reflectionException)
        {
            if (metadataException == null || reflectionException == null)
            {
                return true;
            }

            if (metadataException == null)
            {
                _checkState.AddException(reflectionException, $"{elementName} on reflection", CheckPhase.ReflectionComparison);
            }
            else if (reflectionException == null)
            {
                _checkState.AddException(metadataException, $"{elementName} on reflection", CheckPhase.ReflectionComparison);
            }
            else if (metadataException.GetType() != reflectionException.GetType())
            {
                _checkState.AddError($"{elementName} threw {metadataException.GetType().Name} on metadata but threw {reflectionException.GetType().Name} on relection.\n{metadataException}\n{reflectionException}");
            }
            return false;
        }

        private void CompareTypes(string sourceName, Type metadataType, Type reflectionType)
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
