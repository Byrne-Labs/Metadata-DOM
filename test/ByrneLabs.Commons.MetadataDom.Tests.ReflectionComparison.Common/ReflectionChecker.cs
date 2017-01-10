﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.ReflectionComparison.Common
{
    public static class ReflectionChecker
    {
        public static IEnumerable<string> CompareCodeElementsToReflectionData(Assembly assembly, ReflectionData reflectionData)
        {
            var checkedMetadataElements = new List<CodeElement>();
            var checkedReflectionElements = new List<object>();
            var errors = new List<string>();

            foreach (var reflectionType in assembly.DefinedTypes)
            {
                var metadataType = reflectionData.TypeDefinitions.SingleOrDefault(codeElement => codeElement.FullName.Equals(reflectionType.FullName));
                if (metadataType == null)
                {
                    errors.Add($"Could not find type {reflectionType.FullName}");
                }
                else
                {
                    CompareCodeElementsToReflectionData(metadataType, reflectionType, checkedMetadataElements, checkedReflectionElements, errors);
                }
            }
            foreach (var module in assembly.Modules)
            {
                CompareCodeElementsToReflectionData(reflectionData.ModuleDefinition, module, checkedMetadataElements, checkedReflectionElements, errors);
            }

            errors.AddRange(reflectionData.MemberDefinitions.Except(checkedMetadataElements.OfType<IMember>()).Select(member => $"Did not find member {member.FullName} in assembly reflection"));

            return errors;
        }

        private static string GetTextSignature(TypeInfo reflectedType, MemberInfo memberInfo)
        {
            string textSignature;
            if (memberInfo is TypeInfo)
            {
                textSignature = GetTextSignature(reflectedType, (TypeInfo)memberInfo);
            }
            else if (memberInfo is PropertyInfo)
            {
                textSignature = GetTextSignature(reflectedType, (PropertyInfo)memberInfo);
            }
            else if (memberInfo is FieldInfo)
            {
                textSignature = GetTextSignature(reflectedType, (FieldInfo)memberInfo);
            }
            else if (memberInfo is EventInfo)
            {
                textSignature = GetTextSignature(reflectedType, (EventInfo)memberInfo);
            }
            else if (memberInfo is MethodInfo)
            {
                textSignature = GetTextSignature(reflectedType, (MethodInfo)memberInfo);
            }
            else if (memberInfo is ConstructorInfo)
            {
                textSignature = GetTextSignature(reflectedType, (ConstructorInfo)memberInfo);
            }
            else
            {
                throw new InvalidOperationException($"Unknown member info type {memberInfo.GetType().FullName}");
            }

            return textSignature;
        }

        private static Type GetMetadataType(MemberInfo memberInfo)
        {
            Type metadataType;
            if (memberInfo is TypeInfo)
            {
                metadataType = typeof(TypeDefinition);
            }
            else if (memberInfo is PropertyInfo)
            {
                metadataType = typeof(PropertyDefinition);
            }
            else if (memberInfo is FieldInfo)
            {
                metadataType = typeof(FieldDefinition);
            }
            else if (memberInfo is EventInfo)
            {
                metadataType = typeof(EventDefinition);
            }
            else if (memberInfo is MethodInfo)
            {
                metadataType = typeof(MethodDefinition);
            }
            else if (memberInfo is ConstructorInfo)
            {
                metadataType = typeof(ConstructorDefinition);
            }
            else
            {
                throw new InvalidOperationException($"Unknown member info type {memberInfo.GetType().FullName}");
            }

            return metadataType;
        }

        private static string GetTextSignature(TypeInfo reflectedType, TypeInfo typeInfo)
        {
            return typeInfo.FullName;
        }
        private static string GetTextSignature(TypeInfo reflectedType, PropertyInfo propertyInfo)
        {
            return $"{reflectedType.FullName}.{propertyInfo.Name}";
        }
        private static string GetTextSignature(TypeInfo reflectedType, FieldInfo fieldInfo)
        {
            return $"{reflectedType.FullName}.{fieldInfo.Name}";
        }
        private static string GetTextSignature(TypeInfo reflectedType, EventInfo eventInfo)
        {
            return $"{reflectedType.FullName}.{eventInfo.Name}";
        }

        private static string GetTextSignature(TypeInfo reflectedType, MethodInfo methodInfo)
        {
            return $"{reflectedType.FullName}.{methodInfo.Name}" + (methodInfo.IsSpecialName ? string.Empty : $"({string.Join(", ", methodInfo.GetParameters().Select(parameter => parameter.ParameterType.FullName))})");
        }

        private static string GetTextSignature(TypeInfo reflectedType, ConstructorInfo constructorInfo)
        {
            return $"{reflectedType.FullName}({string.Join(", ", constructorInfo.GetParameters().Select(parameter => parameter.ParameterType.FullName))})";
        }

        private static bool WasChecked(CodeElement metadataElement, object reflectionElement, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements)
        {
            var wasChecked = checkedReflectionElements.Contains(reflectionElement);
            if (!wasChecked)
            {
                checkedMetadataElements.Add(metadataElement);
                checkedReflectionElements.Add(reflectionElement);
            }

            return wasChecked;
        }

        private static void CompareCodeElementsToReflectionData(TypeDefinition metadataType, TypeInfo reflectionType, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataType, reflectionType, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }
            foreach (var reflectionMember in reflectionType.DeclaredMembers)
            {
                var reflectionMemberSignature = GetTextSignature(reflectionType, reflectionMember);

                var metadataMember = metadataType.Members.SingleOrDefault(codeElement => codeElement.GetType() == GetMetadataType(reflectionMember) && codeElement.TextSignature.Equals(reflectionMemberSignature));
                if (metadataMember == null)
                {
                    errors.Add($"Could not find member {reflectionMember.Name} on type {reflectionType.FullName}");
                }
                if (reflectionMember is TypeInfo)
                {
                    CompareCodeElementsToReflectionData((TypeDefinition)metadataMember, (TypeInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is PropertyInfo)
                {
                    CompareCodeElementsToReflectionData((PropertyDefinition)metadataMember, (PropertyInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is ConstructorInfo)
                {
                    CompareCodeElementsToReflectionData((ConstructorDefinition)metadataMember, (ConstructorInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is MethodInfo)
                {
                    CompareCodeElementsToReflectionData((MethodDefinition)metadataMember, (MethodInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is EventInfo)
                {
                    CompareCodeElementsToReflectionData((EventDefinition)metadataMember, (EventInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else if (reflectionMember is FieldInfo)
                {
                    CompareCodeElementsToReflectionData((FieldDefinition)metadataMember, (FieldInfo)reflectionMember, checkedMetadataElements, checkedReflectionElements, errors);
                }
                else
                {
                    throw new BadImageException($"Could not handle member type {reflectionMember.GetType().FullName}");
                }
            }
        }

        private static void CompareCodeElementsToReflectionData(PropertyDefinition metadataProperty, PropertyInfo reflectionProperty, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataProperty, reflectionProperty, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }
        }

        private static void CompareCodeElementsToReflectionData(ConstructorDefinition metadataConstructor, ConstructorInfo reflectionConstructor, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataConstructor, reflectionConstructor, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }
        }

        private static void CompareCodeElementsToReflectionData(MethodDefinition metadataMethod, MethodInfo reflectionMethod, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataMethod, reflectionMethod, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }
        }

        private static void CompareCodeElementsToReflectionData(EventDefinition metadataEvent, EventInfo reflectionEvent, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataEvent, reflectionEvent, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }
        }

        private static void CompareCodeElementsToReflectionData(FieldDefinition metadataField, FieldInfo reflectionField, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataField, reflectionField, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }
        }

        private static void CompareCodeElementsToReflectionData(ModuleDefinition metadataModule, Module reflectionModule, ICollection<CodeElement> checkedMetadataElements, ICollection<object> checkedReflectionElements, ICollection<string> errors)
        {
            if (WasChecked(metadataModule, reflectionModule, checkedMetadataElements, checkedReflectionElements))
            {
                return;
            }
        }
    }
}
