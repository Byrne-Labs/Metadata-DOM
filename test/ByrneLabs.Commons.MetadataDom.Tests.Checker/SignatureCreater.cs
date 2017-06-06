using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public static class SignatureCreater
    {
        private static readonly Dictionary<Tuple<System.Reflection.TypeInfo, MemberInfo>, string> _textSignatures = new Dictionary<Tuple<System.Reflection.TypeInfo, MemberInfo>, string>();

        public static string GetTextSignature(this MemberInfo memberInfo) => GetTextSignature(memberInfo?.DeclaringType?.GetTypeInfo(), memberInfo);

        [SuppressMessage("ReSharper", "CanBeReplacedWithTryCastAndCheckForNull", Justification = "Using try cast for all possible classes would be slower than checking the type. -- Jonathan Byrne 01/21/2017")]
        public static string GetTextSignature(System.Reflection.TypeInfo reflectedType, MemberInfo memberInfo)
        {
            string textSignature;
            if (memberInfo is IMemberInfo)
            {
                textSignature = ((IMemberInfo) memberInfo).TextSignature;
            }
            else
            {
                var key = new Tuple<System.Reflection.TypeInfo, MemberInfo>(reflectedType, memberInfo);
                if (!_textSignatures.ContainsKey(key))
                {
                    if (memberInfo is System.Reflection.TypeInfo)
                    {
                        textSignature = GetTextSignature((System.Reflection.TypeInfo) memberInfo, false);
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

                    _textSignatures.Add(key, textSignature);
                }
                else
                {
                    textSignature = _textSignatures[key];
                }
            }

            return textSignature;
        }

        private static string GetTextSignature(this System.Reflection.TypeInfo typeInfo, bool includeGenericArguments)
        {
            string parent;
            string name;
            string genericParameters;
            if (typeInfo.IsNested && typeInfo.FullName == null && !typeInfo.IsGenericParameter)
            {
                parent = GetTextSignature(typeInfo.DeclaringType as System.Reflection.TypeInfo, includeGenericArguments) + "+";
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

        private static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.PropertyInfo propertyInfo) => $"{reflectedType.FullName}.{propertyInfo.Name}" + ("Item".Equals(propertyInfo.Name) && propertyInfo.GetMethod?.GetParameters().Any() == true ? $"[{string.Join(", ", propertyInfo.GetMethod?.GetParameters().Select(parameter => GetTextSignature(parameter.ParameterType.GetTypeInfo())))}]" : string.Empty);

        private static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.FieldInfo fieldInfo) => $"{reflectedType.FullName}.{fieldInfo.Name}";

        private static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.EventInfo eventInfo) => $"{reflectedType.FullName}.{eventInfo.Name}";

        private static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.MethodInfo methodInfo)
        {
            var basicName = $"{GetTextSignature(reflectedType)}.{methodInfo.Name}";
            var nonIndexProperty = methodInfo.IsSpecialName && (methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_") || methodInfo.Name.StartsWith("remove_") || methodInfo.Name.StartsWith("add_") || methodInfo.Name.StartsWith("raise_") || methodInfo.Name.Contains(".get_") || methodInfo.Name.Contains(".set_") || methodInfo.Name.Contains(".remove_") || methodInfo.Name.Contains(".add_") || methodInfo.Name.Contains(".raise_")) && !("get_Item".Equals(methodInfo.Name) && methodInfo.GetParameters().Length > 0 || "set_Item".Equals(methodInfo.Name) && methodInfo.GetParameters().Length > 1);
            var genericParameters = nonIndexProperty || !methodInfo.IsGenericMethod ? string.Empty : $"`{methodInfo.GetGenericArguments().Length}";
            var parameters = nonIndexProperty ? string.Empty : $"({string.Join(", ", methodInfo.GetParameters().Select(parameter => parameter.ParameterType.IsGenericParameter ? parameter.ParameterType.Name : parameter.ParameterType.ToString()))})";
            var textSignature = basicName + genericParameters + parameters;

            return textSignature;
        }

        private static string GetTextSignature(System.Reflection.TypeInfo reflectedType, System.Reflection.ConstructorInfo constructorInfo)
        {
            var basicName = $"{GetTextSignature(reflectedType)}{constructorInfo.Name}";
            var parameters = $"({string.Join(", ", constructorInfo.GetParameters().Select(parameter => GetTextSignature(parameter.ParameterType.GetTypeInfo(), true)))})";
            var textSignature = basicName + parameters;

            return textSignature;
        }
    }
}
