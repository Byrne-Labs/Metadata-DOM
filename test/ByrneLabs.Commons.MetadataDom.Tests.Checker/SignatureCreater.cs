using System;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    public static class SignatureCreater
    {
        public static string GetTextSignature(System.Reflection.TypeInfo reflectedType, MemberInfo memberInfo)
        {
            string textSignature;
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull -- Using try cast for all possible classes would be slower than checking the type. -- Jonathan Byrne 01/21/2017
            if (memberInfo is System.Reflection.TypeInfo)
            {
                textSignature = GetTextSignature((System.Reflection.TypeInfo) memberInfo);
            }
            else if (memberInfo is System.Reflection.PropertyInfo)
            {
                textSignature = GetTextSignature(reflectedType, (System.Reflection.PropertyInfo) memberInfo);
            }
            else if (memberInfo is System.Reflection.FieldInfo)
            {
                textSignature = GetTextSignature(reflectedType, (System.Reflection.FieldInfo) memberInfo);
            }
            else if (memberInfo is System.Reflection.EventInfo)
            {
                textSignature = GetTextSignature(reflectedType, (System.Reflection.EventInfo) memberInfo);
            }
            else if (memberInfo is System.Reflection.MethodInfo)
            {
                textSignature = GetTextSignature(reflectedType, (System.Reflection.MethodInfo) memberInfo);
            }
            else if (memberInfo is System.Reflection.ConstructorInfo)
            {
                textSignature = GetTextSignature(reflectedType, (System.Reflection.ConstructorInfo) memberInfo);
            }
            else
            {
                throw new InvalidOperationException($"Unknown member info type {memberInfo.GetType().FullName}");
            }

            return textSignature;
        }

        public static string GetTextSignature(this System.Reflection.TypeInfo typeInfo, bool includeGenericArguments = false)
        {
            string parent;
            string name;
            string genericParameters;
            if (typeInfo.IsNested && typeInfo.FullName == null && !typeInfo.IsGenericParameter)
            {
                parent = GetTextSignature(typeInfo, includeGenericArguments) + "+";
                name = typeInfo.Name;
            }
            else if (typeInfo.GenericTypeArguments.Length > 0)
            {
                parent = string.IsNullOrEmpty(typeInfo.Namespace) ? string.Empty : typeInfo.Namespace + ".";
                name = typeInfo.Name;
            }
            else if (typeInfo.GetGenericArguments().Length > 0 && includeGenericArguments)
            {
                parent = string.IsNullOrEmpty(typeInfo.Namespace) ? string.Empty : typeInfo.Namespace + ".";
                name = typeInfo.Name;
            }
            else if (typeInfo.FullName != null)
            {
                parent = string.Empty;
                name = typeInfo.FullName;
            }
            else if (typeInfo.IsGenericParameter || typeInfo.Namespace == null || typeInfo.HasElementType && typeInfo.GetElementType().IsGenericParameter)
            {
                parent = string.Empty;
                name = typeInfo.Name;
            }
            else
            {
                parent = typeInfo.Namespace + ".";
                name = typeInfo.Name;
            }

            if (typeInfo.GenericTypeArguments.Length > 0)
            {
                genericParameters = $"[{string.Join(",", typeInfo.GenericTypeArguments.Select(genericTypeArgument => GetTextSignature(genericTypeArgument.GetTypeInfo(), includeGenericArguments)))}]";
            }
            else if (typeInfo.GetGenericArguments().Length > 0 && includeGenericArguments)
            {
                genericParameters = $"[{string.Join(",", typeInfo.GetGenericArguments().Select(genericTypeArgument => GetTextSignature(genericTypeArgument.GetTypeInfo(), includeGenericArguments)))}]";
            }
            else
            {
                genericParameters = string.Empty;
            }

            var textSignature = parent + name + genericParameters;

            return textSignature;
        }

        public static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.PropertyInfo propertyInfo) =>
            $"{reflectedType.FullName}.{propertyInfo.Name}" +
            ("Item".Equals(propertyInfo.Name) && propertyInfo.GetMethod?.GetParameters().Any() == true ? $"[{string.Join(", ", propertyInfo.GetMethod?.GetParameters().Select(parameter => GetTextSignature(parameter.ParameterType.GetTypeInfo())))}]" : string.Empty);

        public static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.FieldInfo fieldInfo) => $"{reflectedType.FullName}.{fieldInfo.Name}";

        public static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.EventInfo eventInfo) => $"{reflectedType.FullName}.{eventInfo.Name}";

        public static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.MethodInfo methodInfo)
        {
            var basicName = $"{GetTextSignature(reflectedType)}.{methodInfo.Name}";
            var nonIndexProperty = methodInfo.IsSpecialName && (methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_") || methodInfo.Name.StartsWith("remove_") || methodInfo.Name.StartsWith("add_") || methodInfo.Name.StartsWith("raise_") || methodInfo.Name.Contains(".get_") || methodInfo.Name.Contains(".set_") || methodInfo.Name.Contains(".remove_") || methodInfo.Name.Contains(".add_") || methodInfo.Name.Contains(".raise_")) && !("get_Item".Equals(methodInfo.Name) && methodInfo.GetParameters().Length > 0 || "set_Item".Equals(methodInfo.Name) && methodInfo.GetParameters().Length > 1);
            var genericParameters = nonIndexProperty || !methodInfo.IsGenericMethod ? string.Empty : $"<{string.Join(", ", methodInfo.GetGenericArguments().Select(genericTypeParameter => genericTypeParameter.Name))}>";
            var parameters = nonIndexProperty ? string.Empty : $"({string.Join(", ", methodInfo.GetParameters().Select(parameter => GetTextSignature(parameter.ParameterType.GetTypeInfo(), true)))})";
            var textSignature = basicName + genericParameters + parameters;

            return textSignature;
        }

        public static string GetTextSignature(this System.Reflection.MemberInfo memberInfo) => GetTextSignature(memberInfo.DeclaringType.GetTypeInfo(), memberInfo);

        public static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.ConstructorInfo constructorInfo)
        {
            var basicName = $"{GetTextSignature(reflectedType)}{constructorInfo.Name}";
            var parameters = $"({string.Join(", ", constructorInfo.GetParameters().Select(parameter => GetTextSignature(parameter.ParameterType.GetTypeInfo(), true)))})";
            var textSignature = basicName + parameters;

            return textSignature;
        }
    }
}
