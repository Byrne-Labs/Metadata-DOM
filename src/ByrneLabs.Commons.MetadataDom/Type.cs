#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
using System;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class Type : MemberInfo
    {
        internal const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        internal const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public abstract Assembly Assembly { get; }

        public abstract string AssemblyQualifiedName { get; }

        public abstract Type BaseType { get; }

        public abstract bool IsConstructedGenericType { get; }

        public abstract bool IsEnum { get; }

        public abstract bool IsGenericParameter { get; }

        public abstract bool IsGenericType { get; }

        public abstract string Namespace { get; }

        public abstract RuntimeTypeHandle TypeHandle { get; }

        public abstract Type UnderlyingSystemType { get; }

        public TypeAttributes Attributes => GetAttributeFlagsImpl();

        public virtual Type[] GenericTypeArguments => GetGenericArguments();

        public bool IsAbstract => (GetAttributeFlagsImpl() & TypeAttributes.Abstract) != 0;

        public bool IsAnsiClass => (GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.AnsiClass;

        public bool IsArray => IsArrayImpl();

        public bool IsAutoClass => (GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.AutoClass;

        public bool IsAutoLayout => (GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.AutoLayout;

        public bool IsByRef => IsByRefImpl();

        public bool IsClass => (GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Class && !IsValueType;

        public bool IsCOMObject => IsCOMObjectImpl();

        public bool IsExplicitLayout => (GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout;

        public bool IsImport => (GetAttributeFlagsImpl() & TypeAttributes.Import) != 0;

        public bool IsInterface => (GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface;

        public bool IsLayoutSequential => (GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.SequentialLayout;

        public bool IsMarshalByRef => IsMarshalByRefImpl();

        public bool IsNested => DeclaringType != null;

        public bool IsNestedAssembly => (GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedAssembly;

        public bool IsNestedFamANDAssem => (GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamANDAssem;

        public bool IsNestedFamily => (GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily;

        public bool IsNestedFamORAssem => (GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamORAssem;

        public bool IsNestedPrivate => (GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPrivate;

        public bool IsNestedPublic => (GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic;

        public bool IsNotPublic => (GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NotPublic;

        public bool IsPointer => IsPointerImpl();

        public bool IsPublic => (GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.Public;

        public bool IsSealed => (GetAttributeFlagsImpl() & TypeAttributes.Sealed) != 0;

        public virtual bool IsSerializable => (GetAttributeFlagsImpl() & TypeAttributes.Serializable) != 0;

        public override bool IsSpecialName => (GetAttributeFlagsImpl() & TypeAttributes.SpecialName) != 0;

        public bool IsUnicodeClass => (GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.UnicodeClass;

        public bool IsValueType => IsValueTypeImpl();

        public abstract int GetArrayRank();

        public abstract ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);

        public abstract Type GetElementType();

        public abstract EventInfo GetEvent(string name, BindingFlags bindingAttr);

        public abstract EventInfo[] GetEvents(BindingFlags bindingAttr);

        public abstract FieldInfo GetField(string name, BindingFlags bindingAttr);

        public abstract FieldInfo[] GetFields(BindingFlags bindingAttr);

        public abstract Type[] GetGenericArguments();

        public abstract Type GetGenericTypeDefinition();

        public abstract Type GetInterface(string name, bool ignoreCase);

        public abstract InterfaceMapping GetInterfaceMap(Type interfaceType);

        public abstract Type[] GetInterfaces();

        public abstract MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr);

        public abstract MemberInfo[] GetMembers(BindingFlags bindingAttr);

        public abstract MethodInfo[] GetMethods(BindingFlags bindingAttr);

        public abstract Type GetNestedType(string name, BindingFlags bindingAttr);

        public abstract Type[] GetNestedTypes(BindingFlags bindingAttr);

        public abstract PropertyInfo[] GetProperties(BindingFlags bindingAttr);

        public ConstructorInfo[] GetConstructors() => GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        public virtual EventInfo[] GetEvents() => GetEvents(DefaultLookup);

        public FieldInfo GetField(string name) => GetField(name, DefaultLookup);

        public FieldInfo[] GetFields() => GetFields(DefaultLookup);

        public MemberInfo[] GetMembers() => GetMembers(DefaultLookup);

        public MethodInfo[] GetMethods() => GetMethods(DefaultLookup);

        public Type[] GetNestedTypes() => GetNestedTypes(DefaultLookup);

        public PropertyInfo[] GetProperties() => GetProperties(DefaultLookup);

        protected abstract TypeAttributes GetAttributeFlagsImpl();

        protected abstract bool HasElementTypeImpl();

        protected abstract bool IsArrayImpl();

        protected abstract bool IsByRefImpl();

        protected abstract bool IsCOMObjectImpl();

        protected abstract bool IsMarshalByRefImpl();

        protected abstract bool IsPointerImpl();

        protected abstract bool IsPrimitiveImpl();

        protected abstract bool IsValueTypeImpl();
    }
}

#endif
