#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class MethodBase : MemberInfo
    {
        public abstract MethodAttributes Attributes { get; }

        public abstract bool ContainsGenericParameters { get; }

        public abstract bool IsGenericMethod { get; }

        public abstract bool IsGenericMethodDefinition { get; }

        public abstract IEnumerable<ParameterInfo> Parameters { get; }

        public bool IsAbstract => (Attributes & MethodAttributes.Abstract) != 0;

        public bool IsAssembly => (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;

        public bool IsConstructor => this is ConstructorInfo && !IsStatic && (Attributes & MethodAttributes.RTSpecialName) == MethodAttributes.RTSpecialName;

        public bool IsFamily => (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;

        public bool IsFamilyAndAssembly => (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem;

        public bool IsFamilyOrAssembly => (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem;

        public bool IsFinal => (Attributes & MethodAttributes.Final) != 0;

        public bool IsHideBySig => (Attributes & MethodAttributes.HideBySig) != 0;

        public bool IsPrivate => (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;

        public bool IsPublic => (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;

        public override bool IsSpecialName => (Attributes & MethodAttributes.SpecialName) != 0;

        public bool IsStatic => (Attributes & MethodAttributes.Static) != 0;

        public bool IsVirtual => (Attributes & MethodAttributes.Virtual) != 0;

        public override MemberTypes MemberType => MemberTypes.Method;

        public MethodImplAttributes MethodImplementationFlags => GetMethodImplementationFlags();

        public abstract Type[] GetGenericArguments();

        public abstract MethodBody GetMethodBody();

        public abstract MethodImplAttributes GetMethodImplementationFlags();

        public abstract ParameterInfo[] GetParameters();

        public abstract ParameterInfo ReturnParameter { get; }
    }
}

#endif
