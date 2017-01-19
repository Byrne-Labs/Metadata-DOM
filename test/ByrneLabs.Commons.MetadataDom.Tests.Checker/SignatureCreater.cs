using System;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    public static class SignatureCreater
    {
        public static string GetTextSignature(TypeInfo reflectedType, MemberInfo memberInfo)
        {
            string textSignature;
            if (memberInfo is TypeInfo)
            {
                textSignature = GetTextSignature(reflectedType, (TypeInfo) memberInfo);
            }
            else if (memberInfo is PropertyInfo)
            {
                textSignature = GetTextSignature(reflectedType, (PropertyInfo) memberInfo);
            }
            else if (memberInfo is FieldInfo)
            {
                textSignature = GetTextSignature(reflectedType, (FieldInfo) memberInfo);
            }
            else if (memberInfo is EventInfo)
            {
                textSignature = GetTextSignature(reflectedType, (EventInfo) memberInfo);
            }
            else if (memberInfo is MethodInfo)
            {
                textSignature = GetTextSignature(reflectedType, (MethodInfo) memberInfo);
            }
            else if (memberInfo is ConstructorInfo)
            {
                textSignature = GetTextSignature(reflectedType, (ConstructorInfo) memberInfo);
            }
            else
            {
                throw new InvalidOperationException($"Unknown member info type {memberInfo.GetType().FullName}");
            }

            return textSignature;
        }

        public static string GetTextSignature(TypeInfo reflectedType, TypeInfo typeInfo) => typeInfo.FullName;

        public static string GetTextSignature(TypeInfo reflectedType, PropertyInfo propertyInfo) => $"{reflectedType.FullName}.{propertyInfo.Name}";

        public static string GetTextSignature(TypeInfo reflectedType, FieldInfo fieldInfo) => $"{reflectedType.FullName}.{fieldInfo.Name}";

        public static string GetTextSignature(TypeInfo reflectedType, EventInfo eventInfo) => $"{reflectedType.FullName}.{eventInfo.Name}";

        public static string GetTextSignature(TypeInfo reflectedType, MethodInfo methodInfo)
        {
            return $"{reflectedType.FullName}.{methodInfo.Name}" + (methodInfo.IsSpecialName ? string.Empty : $"({string.Join(", ", methodInfo.GetParameters().Select(parameter => parameter.ParameterType.FullName))})");
        }

        public static string GetTextSignature(TypeInfo reflectedType, ConstructorInfo constructorInfo)
        {
            return $"{reflectedType.FullName}({string.Join(", ", constructorInfo.GetParameters().Select(parameter => parameter.ParameterType.FullName))})";
        }
    }
}
