using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    public static class SignatureCreater
    {
        public static string GetTextSignature(TypeInfo reflectedType, MemberInfo memberInfo)
        {
            string textSignature;
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull -- Using try cast for all possible classes would be slower than checking the type. -- Jonathan Byrne 01/21/2017
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

        public static string GetTextSignature(TypeInfo reflectedType, TypeInfo typeInfo, bool includeGenericArguments = false)
        {
            string fullName;
            if (typeInfo.GenericTypeArguments.Length > 0)
            {
                fullName = $"{(string.IsNullOrEmpty(typeInfo.Namespace) ? string.Empty : typeInfo.Namespace + ".")}{typeInfo.Name}[{string.Join(",", typeInfo.GenericTypeArguments.Select(genericTypeArgument => GetTextSignature(reflectedType, genericTypeArgument.GetTypeInfo())))}]";
            }
            else if (typeInfo.GetGenericArguments().Length > 0 && includeGenericArguments)
            {
                fullName = $"{(string.IsNullOrEmpty(typeInfo.Namespace) ? string.Empty : typeInfo.Namespace + ".")}{typeInfo.Name}[{string.Join(",", typeInfo.GetGenericArguments().Select(genericTypeArgument => GetTextSignature(reflectedType, genericTypeArgument.GetTypeInfo())))}]";
            }
            else if (typeInfo.FullName != null)
            {
                fullName = typeInfo.FullName;
            }
            else if (typeInfo.IsGenericParameter || typeInfo.Namespace == null || typeInfo.HasElementType && typeInfo.GetElementType().IsGenericParameter)
            {
                fullName = typeInfo.Name;
            }
            else
            {
                fullName = typeInfo.Namespace + "." + typeInfo.Name;
            }

            return fullName;
        }

        public static string GetTextSignature(TypeInfo reflectedType, PropertyInfo propertyInfo) =>
            $"{reflectedType.FullName}.{propertyInfo.Name}" +
            ("Item".Equals(propertyInfo.Name) && propertyInfo.GetMethod?.GetParameters().Any() == true ? $"[{string.Join(", ", propertyInfo.GetMethod?.GetParameters().Select(parameter => GetTextSignature(reflectedType, parameter.ParameterType.GetTypeInfo())))}]" : string.Empty);

        public static string GetTextSignature(TypeInfo reflectedType, FieldInfo fieldInfo) => $"{reflectedType.FullName}.{fieldInfo.Name}";

        public static string GetTextSignature(TypeInfo reflectedType, EventInfo eventInfo) => $"{reflectedType.FullName}.{eventInfo.Name}";

        public static string GetTextSignature(TypeInfo reflectedType, MethodInfo methodInfo)
        {
            var basicName = $"{GetTextSignature(reflectedType, reflectedType)}.{methodInfo.Name}";
            var nonIndexProperty = methodInfo.IsSpecialName && (methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_") || methodInfo.Name.StartsWith("remove_") || methodInfo.Name.StartsWith("add_") || methodInfo.Name.StartsWith("invoke_")) && !("get_Item".Equals(methodInfo.Name) || "set_Item".Equals(methodInfo.Name));
            var genericParameters = nonIndexProperty || !methodInfo.IsGenericMethod ? string.Empty : $"<{ string.Join(", ", methodInfo.GetGenericArguments().Select(genericTypeParameter => genericTypeParameter.Name)) }>";
            var parameters = nonIndexProperty ? string.Empty : $"({string.Join(", ", methodInfo.GetParameters().Select(parameter => GetTextSignature(reflectedType, parameter.ParameterType.GetTypeInfo(), true)))})";

            return basicName + genericParameters + parameters;
        }

        public static string GetTextSignature(TypeInfo reflectedType, ConstructorInfo constructorInfo)
        {
            return $"{reflectedType.FullName}{constructorInfo.Name}({string.Join(", ", constructorInfo.GetParameters().Select(parameter => GetTextSignature(reflectedType, parameter.ParameterType.GetTypeInfo(), true)))})";
        }
    }
}
